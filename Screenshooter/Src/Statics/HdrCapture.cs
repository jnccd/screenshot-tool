using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.DXGI.Resource;
using Texture2D = SharpDX.Direct3D11.Texture2D;
using Rectangle = System.Drawing.Rectangle;

namespace ScreenshotTool
{
    /// <summary>
    /// HDR aware desktop capture.
    ///
    /// On a display with HDR switched on, Windows composites the desktop in scRGB
    /// (linear, 1.0 == 80 nits) and places SDR white at the "SDR brightness" level -
    /// here that is 300 nits, i.e. 3.75x. The GDI path (Graphics.CopyFromScreen) only
    /// sees an 8 bit SDR rendering of that composition: content the compositor handles
    /// in HDR (a video surface in a browser, for example) is scaled up by 3.75 and then
    /// clipped at 1.0, which is why exactly those parts of a screenshot look blown out
    /// while the rest of the window is fine.
    ///
    /// Reading the composition through DXGI desktop duplication in FP16 gives the real
    /// scRGB values. Dividing by the SDR white level and encoding back to sRGB
    /// reproduces plain SDR content exactly (verified pixel for pixel against GDI)
    /// while HDR highlights are rolled off instead of clipped.
    /// </summary>
    internal static class HdrCapture
    {
        static readonly object gate = new object();
        static List<OutputSession> sessions;
        static DateTime lastScan = DateTime.MinValue;
        static DateTime lastRefresh = DateTime.MinValue;
        static readonly TimeSpan rescanInterval = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Capture diagnostics. HDR capture depends on the driver, the display state and
        /// the desktop composition, so when a screenshot looks wrong this log says what
        /// actually happened without needing a debugger attached.
        /// </summary>
        internal static string CaptureLogPath => Diagnostics.LogPath;

        static void Log(string message) => Diagnostics.Log(message);
        // QueryDisplayConfig costs about 10 ms, far too much to do per GIF frame
        static readonly TimeSpan refreshInterval = TimeSpan.FromMilliseconds(500);

        static HdrCapture()
        {
            try
            {
                // a display mode / HDR toggle invalidates every duplication
                Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, e) =>
                {
                    lock (gate)
                        Reset();
                };
            }
            catch (Exception e)
            {
                // SystemEvents needs a message pump; without one the TTL rescan still works
                Debug.WriteLine("Could not listen for display changes: " + e.Message);
            }
        }

        /// <summary>
        /// Captures the given physical desktop rectangle. Returns null when no display
        /// is in HDR mode (callers then use plain GDI, which is already correct).
        /// </summary>
        public static Bitmap Capture(Rectangle physicalRect)
        {
            if (physicalRect.Width <= 0 || physicalRect.Height <= 0)
                return null;

            lock (gate)
            {
                if (!EnsureSessions())
                    return null;

                // The SDR white level is the user's "SDR content brightness" slider and
                // can change at any time, so re-read it before every capture instead of
                // baking it in when the session was created.
                if (!RefreshHdrState())
                    return null;

                Bitmap result = null;
                var watch = Stopwatch.StartNew();
                bool usedHdr = false;
                try
                {
                    result = new Bitmap(physicalRect.Width, physicalRect.Height, PixelFormat.Format32bppRgb);
                    using (Graphics g = Graphics.FromImage(result))
                    {
                        // GDI is pixel exact for SDR content, so start from it and let the
                        // HDR capable outputs paint over their own area.
                        g.CopyFromScreen(physicalRect.X, physicalRect.Y, 0, 0, physicalRect.Size, CopyPixelOperation.SourceCopy);

                        foreach (OutputSession session in sessions)
                        {
                            Rectangle area = Rectangle.Intersect(physicalRect, session.PhysicalBounds);
                            if (area.IsEmpty)
                                continue;

                            using (Bitmap frame = session.CaptureRegion(area))
                            {
                                if (frame == null)
                                    continue;      // keep the GDI pixels for this output
                                g.DrawImage(frame,
                                    new Rectangle(area.X - physicalRect.X, area.Y - physicalRect.Y, area.Width, area.Height),
                                    0, 0, frame.Width, frame.Height, GraphicsUnit.Pixel);
                                usedHdr = true;
                            }
                        }
                    }
                    Log($"capture {physicalRect} sessions={sessions.Count} hdrFrames={usedHdr} {watch.ElapsedMilliseconds}ms");
                    return result;
                }
                catch (Exception e)
                {
                    Log($"capture {physicalRect} FAILED: {e.GetType().Name}: {e.Message} ({watch.ElapsedMilliseconds}ms)");
                    Debug.WriteLine("HDR capture failed, falling back to GDI: " + e);
                    result?.Dispose();
                    Reset();
                    return null;
                }
            }
        }

        /// <summary>
        /// Re-reads HDR state and the SDR white level for every session. Returns false
        /// when HDR has been switched off in the meantime - then plain GDI is correct
        /// again and the cached duplications are dropped.
        /// </summary>
        static bool RefreshHdrState()
        {
            if (DateTime.UtcNow - lastRefresh < refreshInterval)
                return true;      // recent enough; a display change event forces a reset
            lastRefresh = DateTime.UtcNow;

            foreach (OutputSession session in sessions)
            {
                bool hdrEnabled;
                double whiteScale;
                if (!DisplayHdrState(session.DeviceName, out hdrEnabled, out whiteScale) || !hdrEnabled)
                {
                    Log($"{session.DeviceName}: HDR is no longer enabled, dropping the capture session");
                    Reset();
                    return false;
                }
                session.UpdateWhiteScale(whiteScale);
            }
            return true;
        }

        static bool EnsureSessions()
        {
            if (sessions != null && sessions.Count > 0)
                return true;

            // HDR may have been switched on since the last look; rescan now and then
            // instead of only once per process.
            if (DateTime.UtcNow - lastScan < rescanInterval)
                return false;
            lastScan = DateTime.UtcNow;

            Reset();
            sessions = new List<OutputSession>();
            try
            {
                using (var factory = new Factory1())
                {
                    for (int a = 0; a < factory.GetAdapterCount1(); a++)
                    {
                        using (Adapter1 adapter = factory.GetAdapter1(a))
                        {
                            for (int o = 0; o < adapter.GetOutputCount(); o++)
                            {
                                using (SharpDX.DXGI.Output output = adapter.GetOutput(o))
                                {
                                    OutputSession session = TryCreateSession(adapter, output);
                                    if (session != null)
                                        sessions.Add(session);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not start HDR capture: " + e.Message);
                Reset();
                return false;
            }
            return sessions.Count > 0;
        }

        static OutputSession TryCreateSession(Adapter1 adapter, SharpDX.DXGI.Output output)
        {
            OutputDescription description;
            try { description = output.Description; }
            catch { return null; }
            if (!description.IsAttachedToDesktop)
                return null;

            var bounds = new Rectangle(
                description.DesktopBounds.Left, description.DesktopBounds.Top,
                description.DesktopBounds.Right - description.DesktopBounds.Left,
                description.DesktopBounds.Bottom - description.DesktopBounds.Top);
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return null;

            double whiteScale;
            bool hdrEnabled;
            if (!DisplayHdrState(description.DeviceName, out hdrEnabled, out whiteScale) || !hdrEnabled)
                return null;      // SDR display: GDI already gives correct pixels

            Device device = null;
            OutputDuplication duplication = null;
            try
            {
                device = new Device(adapter, DeviceCreationFlags.BgraSupport,
                    new[] { FeatureLevel.Level_11_1, FeatureLevel.Level_11_0, FeatureLevel.Level_10_1 });

                // Driver quirk: the first duplication created in the process has to come
                // from the legacy API, otherwise DuplicateOutput1 fails with
                // DXGI_ERROR_UNSUPPORTED even though HDR mode is active.
                using (var output1 = output.QueryInterface<Output1>())
                using (output1.DuplicateOutput(device)) { }

                using (var output5 = output.QueryInterface<Output5>())
                    duplication = output5.DuplicateOutput1(device, 0, 1, new[] { Format.R16G16B16A16_Float });

                var session = new OutputSession(description.DeviceName, device, duplication, bounds, whiteScale);
                if (!session.Prime())
                {
                    Debug.WriteLine($"HDR capture not usable for {description.DeviceName}");
                    session.Dispose();
                    return null;
                }
                Log($"output {description.DeviceName}: HDR session ready bounds={bounds} whiteLevel={whiteScale:N3}");
                return session;
            }
            catch (Exception e)
            {
                Log($"output {description.DeviceName}: no HDR session - {e.GetType().Name}: {e.Message}");
                Debug.WriteLine($"HDR capture not available for {description.DeviceName}: {e.Message}");
                duplication?.Dispose();
                device?.Dispose();
                return null;
            }
        }

        static void Reset()
        {
            if (sessions != null)
            {
                foreach (OutputSession session in sessions)
                    session.Dispose();
                sessions = null;
            }
            lastRefresh = DateTime.MinValue;
        }

        // ---------------------------------------------------------------- one output
        sealed class OutputSession : IDisposable
        {
            readonly Device device;
            readonly OutputDuplication duplication;
            readonly Rectangle desktopBounds;   // as DXGI reports it (may be DPI virtualised)
            double whiteScale;

            Texture2D staging;
            Format format;
            bool haveFrame;
            byte[] lut;
            double lutWhite = -1;
            bool lutHeadroom;
            int width, height;
            double scaleX = 1.0, scaleY = 1.0;

            public OutputSession(string deviceName, Device device, OutputDuplication duplication, Rectangle desktopBounds, double whiteScale)
            {
                DeviceName = deviceName;
                this.device = device;
                this.duplication = duplication;
                this.desktopBounds = desktopBounds;
                this.whiteScale = whiteScale;
            }

            /// <summary>GDI name of the display, used to look up its HDR state.</summary>
            public string DeviceName { get; }

            /// <summary>
            /// Adopts a changed SDR white level (the "SDR content brightness" slider).
            /// The tone map lookup table depends on it, so it has to be rebuilt.
            /// </summary>
            public void UpdateWhiteScale(double scale)
            {
                if (scale <= 0 || Math.Abs(scale - whiteScale) < 0.001)
                    return;
                whiteScale = scale;
                lut = null;
            }

            /// <summary>Where this output sits on the physical desktop.</summary>
            public Rectangle PhysicalBounds
            {
                get
                {
                    return new Rectangle(
                        (int)Math.Round(desktopBounds.X * scaleX),
                        (int)Math.Round(desktopBounds.Y * scaleY),
                        width, height);
                }
            }

            /// <summary>
            /// Learns the frame size and the desktop-coordinate to pixel ratio, which is
            /// needed before a rectangle can be mapped onto the frame.
            /// </summary>
            public bool Prime()
            {
                Resource frame = null;
                OutputDuplicateFrameInformation info;
                for (int attempt = 0; attempt < 4 && frame == null; attempt++)
                {
                    Result result = duplication.TryAcquireNextFrame(attempt == 0 ? 700 : 200, out info, out frame);
                    if (!result.Success)
                        break;
                    if (info.AccumulatedFrames > 0 || info.LastPresentTime != 0)
                        break;
                    duplication.ReleaseFrame();
                    frame = null;
                }
                if (frame == null)
                    return false;

                using (frame)
                using (Texture2D texture = frame.QueryInterface<Texture2D>())
                {
                    Texture2DDescription description = texture.Description;
                    width = description.Width;
                    height = description.Height;
                    format = description.Format;
                    if (width <= 0 || height <= 0 || desktopBounds.Width <= 0 || desktopBounds.Height <= 0)
                        return false;
                    scaleX = width / (double)desktopBounds.Width;
                    scaleY = height / (double)desktopBounds.Height;
                    CreateStaging();
                }
                duplication.ReleaseFrame();
                return true;
            }

            void CreateStaging()
            {
                staging?.Dispose();
                haveFrame = false;
                staging = new Texture2D(device, new Texture2DDescription
                {
                    Width = width,
                    Height = height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = format,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Staging,
                    BindFlags = BindFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Read,
                    OptionFlags = ResourceOptionFlags.None
                });
            }

            /// <summary>Captures a physical sub rectangle of this output as SDR BGRA.</summary>
            public Bitmap CaptureRegion(Rectangle wanted)
            {
                // Desktop duplication only delivers a frame when the desktop changed. On a
                // static screen (exactly what a screenshot tool is pointed at) there is nothing
                // new, and the frame copied last time is still the current content. Giving up
                // here used to fall back to GDI, which cannot represent HDR content at all -
                // that is what turned HDR video into a flat bright white.
                Resource frame = null;
                OutputDuplicateFrameInformation info;
                Result result = duplication.TryAcquireNextFrame(haveFrame ? 60 : 500, out info, out frame);
                bool fresh = result.Success;
                if (!fresh)
                {
                    if (result.Code != unchecked((int)0x887A0027))     // DXGI_ERROR_WAIT_TIMEOUT
                    {
                        Log($"{DeviceName}: acquiring a frame failed (0x{result.Code:X8})");
                        haveFrame = false;
                        return null;
                    }
                    if (!haveFrame)
                    {
                        Log($"{DeviceName}: no frame from desktop duplication yet");
                        return null;
                    }
                }

                DeviceContext context = device.ImmediateContext;
                if (fresh)
                {
                    try
                    {
                        using (Texture2D texture = frame.QueryInterface<Texture2D>())
                        {
                            Texture2DDescription description = texture.Description;
                            if (description.Width != width || description.Height != height || description.Format != format)
                            {
                                // the display mode changed under us; rebuild on the next capture
                                width = description.Width;
                                height = description.Height;
                                format = description.Format;
                                scaleX = width / (double)desktopBounds.Width;
                                scaleY = height / (double)desktopBounds.Height;
                                CreateStaging();
                            }

                            context.CopyResource(texture, staging);
                            haveFrame = true;
                        }
                    }
                    finally
                    {
                        duplication.ReleaseFrame();
                        frame?.Dispose();
                    }
                }

                DataBox box = context.MapSubresource(staging, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);
                try
                {
                    return Convert(box, wanted);
                }
                finally
                {
                    context.UnmapSubresource(staging, 0);
                }
            }

            Bitmap Convert(DataBox box, Rectangle wanted)
            {
                Rectangle physical = PhysicalBounds;
                int left = Math.Max(0, wanted.X - physical.X);
                int top = Math.Max(0, wanted.Y - physical.Y);
                int w = Math.Min(wanted.Width, width - left);
                int h = Math.Min(wanted.Height, height - top);
                if (w <= 0 || h <= 0)
                    return null;

                if (format != Format.B8G8R8A8_UNorm && format != Format.R16G16B16A16_Float)
                {
                    Log($"{DeviceName}: unexpected frame format {format}, keeping GDI pixels");
                    return null;      // caller keeps the GDI pixels
                }

                var bitmap = new Bitmap(w, h, PixelFormat.Format32bppRgb);
                BitmapData bits = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                try
                {
                    if (format == Format.B8G8R8A8_UNorm)
                    {
                        var row = new byte[w * 4];
                        for (int y = 0; y < h; y++)
                        {
                            IntPtr source = IntPtr.Add(box.DataPointer, (top + y) * box.RowPitch + left * 4);
                            Marshal.Copy(source, row, 0, row.Length);
                            Marshal.Copy(row, 0, IntPtr.Add(bits.Scan0, y * bits.Stride), row.Length);
                        }
                    }
                    else
                    {
                        // Does this frame actually contain HDR content? Pure SDR frames keep the
                        // exact mapping; frames with highlights get a highlight range, otherwise
                        // everything above SDR white collapses into one washed out near-white.
                        float peak = SamplePeak(box, left, top, w, h);
                        bool headroom = peak > whiteScale * 1.02;
                        if (lut == null || lutWhite != whiteScale || lutHeadroom != headroom)
                        {
                            lut = BuildToneMapLut(whiteScale, headroom);
                            lutWhite = whiteScale;
                            lutHeadroom = headroom;
                        }
                        Log($"{DeviceName}: frame peak={peak:F2} white={whiteScale:F2} headroom={headroom}");

                        // rows are disjoint, so this scales to a full 4K monitor
                        Parallel.For(0, h, y =>
                        {
                            IntPtr source = IntPtr.Add(box.DataPointer, (top + y) * box.RowPitch + left * 8);
                            var row = new byte[w * 4];
                            for (int x = 0; x < w; x++)
                            {
                                int offset = x * 8;
                                row[x * 4 + 0] = lut[(ushort)Marshal.ReadInt16(source, offset + 4)];   // B
                                row[x * 4 + 1] = lut[(ushort)Marshal.ReadInt16(source, offset + 2)];   // G
                                row[x * 4 + 2] = lut[(ushort)Marshal.ReadInt16(source, offset + 0)];   // R
                                row[x * 4 + 3] = 255;
                            }
                            Marshal.Copy(row, 0, IntPtr.Add(bits.Scan0, y * bits.Stride), row.Length);
                        });
                    }
                    return bitmap;
                }
                catch
                {
                    bitmap.Dispose();
                    throw;
                }
                finally
                {
                    try { bitmap.UnlockBits(bits); } catch { }
                }
            }

            /// <summary>Maps every half float pattern to the tone mapped sRGB byte.</summary>
            static byte[] BuildToneMapLut(double whiteScale, bool headroom)
            {
                var lut = new byte[65536];
                for (int i = 0; i < lut.Length; i++)
                    lut[i] = ToneMap(HalfToFloat(i), whiteScale, headroom);
                return lut;
            }

            /// <summary>
            /// Largest positive half float in the frame. Half float patterns compare
            /// monotonically while the sign bit is clear, so no conversion is needed per pixel.
            /// </summary>
            static float SamplePeak(DataBox box, int left, int top, int w, int h)
            {
                ushort peak = 0;
                for (int y = top; y < top + h; y += 7)
                {
                    IntPtr row = IntPtr.Add(box.DataPointer, y * box.RowPitch + left * 8);
                    for (int x = 0; x < w; x += 7)
                    {
                        int offset = x * 8;
                        for (int c = 0; c < 3; c++)
                        {
                            ushort value = (ushort)Marshal.ReadInt16(row, offset + c * 2);
                            if ((value & 0x8000) != 0)
                                continue;      // negative
                            if (value > peak)
                                peak = value;
                        }
                    }
                }
                return HalfToFloat(peak);
            }

            /// <summary>scRGB (linear, 1.0 == 80 nits) to an SDR sRGB byte.</summary>
            static byte ToneMap(float scrgb, double whiteScale, bool headroom)
            {
                double sdr = scrgb / whiteScale;
                if (double.IsNaN(sdr) || sdr < 0)
                    sdr = 0;

                double value;
                if (!headroom)
                {
                    // Pure SDR frame: reproduced exactly.
                    value = sdr;
                }
                else
                {
                    // SDR white sits slightly below output white and the rest of the range
                    // carries the highlights. A flat "everything above the knee" curve would
                    // instead clip all HDR detail into one bright, washed out white.
                    const double sdrOutput = 0.8;
                    const double highlightRange = 4.0;     // HDR up to 4x SDR white keeps detail
                    if (sdr <= 1)
                        value = sdr * sdrOutput;
                    else
                    {
                        double u = Math.Min(1.0, (sdr - 1) / (highlightRange - 1));
                        value = sdrOutput + (1 - sdrOutput) * (Math.Log(1 + 9 * u) / Math.Log(10));
                    }
                }

                double encoded = value <= 0.0031308 ? 12.92 * value : 1.055 * Math.Pow(value, 1 / 2.4) - 0.055;
                int result = (int)Math.Round(encoded * 255.0);
                return (byte)(result < 0 ? 0 : (result > 255 ? 255 : result));
            }

            static float HalfToFloat(int half)
            {
                int sign = (half >> 15) & 1, exponent = (half >> 10) & 0x1F, mantissa = half & 0x3FF;
                if (exponent == 0x1F)
                    return mantissa == 0 ? (sign == 0 ? float.PositiveInfinity : float.NegativeInfinity) : float.NaN;
                if (exponent == 0)
                    return (sign == 0 ? 1f : -1f) * (mantissa / 1024f) * (float)Math.Pow(2, -14);
                return (sign == 0 ? 1f : -1f) * (1f + mantissa / 1024f) * (float)Math.Pow(2, exponent - 15);
            }

            public void Dispose()
            {
                staging?.Dispose();
                haveFrame = false;
                duplication?.Dispose();
                device?.Dispose();
            }
        }

        // ------------------------------------------------------- display information
        [StructLayout(LayoutKind.Sequential)] struct Luid { public uint LowPart; public int HighPart; }
        [StructLayout(LayoutKind.Sequential)] struct PathSource { public Luid adapterId; public uint id; public uint modeInfoIdx; public uint statusFlags; }
        [StructLayout(LayoutKind.Sequential)]
        struct PathTarget
        {
            public Luid adapterId; public uint id; public uint modeInfoIdx; public uint outputTechnology;
            public uint rotation; public uint scaling; public uint refreshRateNumerator; public uint refreshRateDenominator;
            public uint scanLineOrdering; [MarshalAs(UnmanagedType.Bool)] public bool targetAvailable; public uint statusFlags;
        }
        // NOTE: the trailing flags field exists since the Windows 10 2004 SDK; leaving it
        // out makes QueryDisplayConfig write past the array and corrupts the heap.
        [StructLayout(LayoutKind.Sequential)] struct PathInfo { public PathSource source; public PathTarget target; public uint flags; }
        [StructLayout(LayoutKind.Sequential)] struct DeviceInfoHeader { public uint type; public uint size; public Luid adapterId; public uint id; }
        [StructLayout(LayoutKind.Sequential)] struct AdvancedColorInfo { public DeviceInfoHeader header; public uint value; public uint colorEncoding; public uint bitsPerColorChannel; }
        [StructLayout(LayoutKind.Sequential)] struct SdrWhiteLevel { public DeviceInfoHeader header; public uint level; }
        [StructLayout(LayoutKind.Sequential)]
        struct SourceName
        {
            public DeviceInfoHeader header;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public byte[] name;
            public string Name
            {
                get
                {
                    if (name == null) return "";
                    return System.Text.Encoding.Unicode.GetString(name).TrimEnd('\0');
                }
            }
        }

        [DllImport("user32.dll")] static extern int GetDisplayConfigBufferSizes(uint flags, out uint pathCount, out uint modeCount);
        [DllImport("user32.dll")] static extern int QueryDisplayConfig(uint flags, ref uint pathCount, [Out] PathInfo[] paths, ref uint modeCount, [Out] byte[] modes, IntPtr topologyId);
        [DllImport("user32.dll")] static extern int DisplayConfigGetDeviceInfo(ref AdvancedColorInfo info);
        [DllImport("user32.dll")] static extern int DisplayConfigGetDeviceInfo(ref SdrWhiteLevel info);
        [DllImport("user32.dll")] static extern int DisplayConfigGetDeviceInfo(ref SourceName info);

        const uint OnlyActivePaths = 2;
        const uint GetAdvancedColorInfo = 9;
        const uint GetSdrWhiteLevel = 11;
        const uint GetSourceName = 1;

        /// <summary>
        /// Is this output in HDR (advanced colour) mode, and at which SDR white level?
        /// The white level is in units of 1/1000 of 80 nits.
        /// </summary>
        static bool DisplayHdrState(string gdiDeviceName, out bool hdrEnabled, out double whiteScale)
        {
            hdrEnabled = false;
            whiteScale = 1.0;

            uint pathCount, modeCount;
            if (GetDisplayConfigBufferSizes(OnlyActivePaths, out pathCount, out modeCount) != 0)
                return false;

            var paths = new PathInfo[pathCount];
            var modes = new byte[modeCount * 256];   // PathModeInfo is ~80 bytes; over-allocate
            if (QueryDisplayConfig(OnlyActivePaths, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero) != 0)
                return false;

            for (int i = 0; i < pathCount; i++)
            {
                var sourceName = new SourceName();
                sourceName.header.type = GetSourceName;
                sourceName.header.size = (uint)Marshal.SizeOf(typeof(SourceName));
                sourceName.header.adapterId = paths[i].source.adapterId;
                sourceName.header.id = paths[i].source.id;
                if (DisplayConfigGetDeviceInfo(ref sourceName) != 0)
                    continue;
                if (!string.Equals(sourceName.Name, gdiDeviceName, StringComparison.OrdinalIgnoreCase))
                    continue;

                var advancedColor = new AdvancedColorInfo();
                advancedColor.header.type = GetAdvancedColorInfo;
                advancedColor.header.size = (uint)Marshal.SizeOf(typeof(AdvancedColorInfo));
                advancedColor.header.adapterId = paths[i].target.adapterId;
                advancedColor.header.id = paths[i].target.id;

                var whiteLevel = new SdrWhiteLevel();
                whiteLevel.header.type = GetSdrWhiteLevel;
                whiteLevel.header.size = (uint)Marshal.SizeOf(typeof(SdrWhiteLevel));
                whiteLevel.header.adapterId = paths[i].target.adapterId;
                whiteLevel.header.id = paths[i].target.id;

                if (DisplayConfigGetDeviceInfo(ref advancedColor) != 0)
                    return false;
                if (DisplayConfigGetDeviceInfo(ref whiteLevel) != 0)
                    return false;

                hdrEnabled = (advancedColor.value & 2) != 0;
                if (hdrEnabled && whiteLevel.level > 0)
                    whiteScale = whiteLevel.level / 1000.0;
                return true;
            }
            return false;
        }
    }
}
