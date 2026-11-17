using AnimatedGif;
using ScreenshotTool.Properties;
using ScreenshotTool.Src.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool
{
    public partial class MainForm : Form
    {
        public static KeyboardHook keyHook = new KeyboardHook(true);

        // Images
        int imagesIndex = 0;
        readonly List<Screenshot> images = new List<Screenshot>();
        const int imageLoadParallelism = 4;
        const int resizePreviewWidth = 2048;
        bool showingResizePreview;
        readonly System.Windows.Forms.Timer resizeSettleTimer = new System.Windows.Forms.Timer { Interval = 250 };
        int imageLoadVersion = 0;
        readonly List<Action> pendingImageActions = new List<Action>();

        // UI
        readonly List<Button> middleButtons = new List<Button>();
        Point pMouseDown = new Point(0, 0);
        Point pMouseCurrently = new Point(0, 0);
        Point pMouseLast = new Point(0, 0);
        bool isMouseDown = false;
        DateTime lastKeyDownEvent = DateTime.Now;
        readonly KeybindingsForm keybindingsForm = new KeybindingsForm();
        ColorView colorView = new ColorView();
        TextRecognitionView textView = new TextRecognitionView();

        // Gif
        bool recordingGif = false;
        bool processingGif = false;
        readonly List<Bitmap> gifShots = new List<Bitmap>();

        float CurrentDpi()
        {
            try { using (Graphics graphics = CreateGraphics()) return graphics.DpiX; }
            catch { return 96f; }
        }

        [DllImport("user32.dll")]
        static extern uint GetDpiForWindow(IntPtr hwnd);

        /// <summary>
        /// DPI of the monitor the window is on. CreateGraphics reports the DPI of the monitor
        /// the form is created for, which is not necessarily the one it opens on, so the
        /// startup scaling has to be normalised against this.
        /// </summary>
        float WindowDpi()
        {
            try
            {
                if (IsHandleCreated)
                {
                    uint dpi = GetDpiForWindow(Handle);
                    if (dpi >= 48)
                        return dpi;
                }
            }
            catch { }
            return CurrentDpi();
        }

        void CaptureBaseFonts()
        {
            baseFonts = new Dictionary<Control, Font>();
            baseFonts[this] = Font;
            foreach (Control control in AllControls(this))
                baseFonts[control] = control.Font;
        }

        /// <summary>Re-applies the load time fonts scaled by the current dpiScale.</summary>
        void ApplyFonts()
        {
            if (baseFonts == null)
                return;
            foreach (KeyValuePair<Control, Font> entry in baseFonts)
            {
                Font scaled = ScaleFont(entry.Value, dpiScale);
                if (!ReferenceEquals(entry.Key.Font, scaled))
                    entry.Key.Font = scaled;
            }
        }

        /// <summary>
        /// Keeps the window from being shrunk below what the button row needs to stay
        /// readable - measured from the text, so it follows the font size.
        /// </summary>
        void UpdateMinimumSize()
        {
            // The two text buttons must stay readable; the arrows only carry a glyph, so they
            // may shrink to a small square.
            int textMinimum = TextRenderer.MeasureText("To Clipboard", bSave.Font).Width + 2 * S(10);
            int arrowMinimum = Math.Max(S(24), textMinimum / 3);

            int margin = Math.Max(2, S(6));
            int gap = Math.Max(2, S(6));
            int rowHeight = Math.Max(12, (int)Math.Round(baseButtonHeight * dpiScale));

            // tall enough for the row plus a usable strip of image, wide enough for the row.
            // The image strip is a physical requirement (it has to stay usable on the screen
            // you are looking at), so it follows the monitor DPI and not the DPI the window
            // happened to load at - otherwise it was half as tall on the 4K screen.
            int fixedTextWidth = Math.Max(textMinimum, S(150));
            int imageStrip = Math.Max(S(24), (int)Math.Round(48 * CurrentDpi() / 96.0));
            int minClientWidth = 2 * margin + 3 * gap + 2 * fixedTextWidth + 2 * arrowMinimum;
            int minClientHeight = menuStrip.Bottom + margin + imageStrip + gap + rowHeight + margin;

            Rectangle work = Screen.FromControl(this).WorkingArea;
            Size wanted = new Size(
                Math.Min(minClientWidth + Math.Max(Width - ClientSize.Width, 0), work.Width),
                Math.Min(minClientHeight + Math.Max(Height - ClientSize.Height, 0), work.Height));
            MinimumSize = wanted;
        }

        /// <summary>
        /// Places the image box and the button row from the client size alone, so the result
        /// has the same proportions at any window size and DPI: the row spans the full width,
        /// the image box fills everything above it, and both start below the menu strip.
        /// </summary>
        void LayoutChildren()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;

            int margin = Math.Max(2, S(6));
            int gap = Math.Max(2, S(6));
            if (baseButtonHeight == 0)
                baseButtonHeight = Math.Max(12, bSave.Height);
            int rowHeight = Math.Max(12, (int)Math.Round(baseButtonHeight * dpiScale));
            int rowTop = ClientSize.Height - rowHeight - margin;
            if (rowTop < menuStrip.Bottom + margin)
                rowTop = menuStrip.Bottom + margin;

            int available = ClientSize.Width - 2 * margin - 3 * gap;
            if (available > 0)
            {
                // The label decides how narrow a button may get; "To Clipboard" is the widest
                // label, so all four share its minimum and stay readable (the window itself is
                // kept wide enough by MinimumSize).
                int textMinimum = TextRenderer.MeasureText("To Clipboard", bSave.Font).Width + 2 * S(10);
                int arrowMinimum = Math.Max(S(24), textMinimum / 3);

                // "To Clipboard" and "Delete" keep a fixed width and sit centred; only the two
                // arrows grow with the window, which keeps the pair centred by construction.
                int textWidth = Math.Max(textMinimum, S(150));
                int arrowWidth = Math.Max(arrowMinimum, (available - 2 * textWidth) / 2);
                int previousWidth = arrowWidth;
                int saveWidth = textWidth;
                int deleteWidth = textWidth;
                int nextWidth = Math.Max(arrowMinimum, available - previousWidth - saveWidth - deleteWidth);

                bPrevious.SetBounds(margin, rowTop, previousWidth, rowHeight);
                bSave.SetBounds(bPrevious.Right + gap, rowTop, saveWidth, rowHeight);
                bDelete.SetBounds(bSave.Right + gap, rowTop, deleteWidth, rowHeight);
                bNext.SetBounds(bDelete.Right + gap, rowTop, nextWidth, rowHeight);
            }

            UpdateMinimumSize();

            int imageTop = menuStrip.Bottom + margin;
            int imageHeight = rowTop - gap - imageTop;
            if (imageHeight > 0)
                pBox.SetBounds(margin, imageTop, ClientSize.Width - 2 * margin, imageHeight);

        }

        /// <summary>
        /// How much the window tree has been scaled relative to the DPI it was built at.
        /// The layout constants are captured once at that DPI and multiplied by this, so a
        /// later layout pass stays consistent with a tree that was scaled on a DPI change.
        /// </summary>
        float dpiScale = 1f;


        /// <summary>DPI the window was built at; dpiScale is always relative to it.</summary>
        float baseDpi;

        /// <summary>
        /// The window size in the DPI the form was built at. Keeping this (instead of
        /// multiplying the current size) is what stops the window from growing or shrinking
        /// a little on every trip between monitors with different scaling.
        /// </summary>
        Size logicalClientSize;

        /// <summary>Fonts as they were at load, so scaling is always applied to the original.</summary>
        System.Collections.Generic.Dictionary<Control, Font> baseFonts;

        bool adjustingForDpi;

        /// <summary>When the last DPI change was applied, to debounce a monitor crossing.</summary>
        DateTime lastDpiChange = DateTime.MinValue;

        /// <summary>
        /// True while a monitor crossing is still settling. Crossing the edge produces several
        /// DPI messages and WinForms resizes the window for its own scaling at the same time;
        /// freezing the logical size in that window is what stops the two from feeding back
        /// into each other and making the window oscillate.
        /// </summary>
        bool crossingMonitors => (DateTime.UtcNow - lastDpiChange).TotalMilliseconds < 400;

        /// <summary>Design pixels to device pixels at the current monitor scale.</summary>
        int S(int designPixels) => Math.Max(1, (int)Math.Round(designPixels * dpiScale));

        /// <summary>Monitor DPI relative to 96. The HUD constants are authored at 96 dpi.</summary>
        float dpiFactor = 1f;

        /// <summary>Design pixels (96 dpi) to device pixels on the current monitor.</summary>
        int Px(int designPixels) => Math.Max(1, (int)Math.Round(designPixels * dpiFactor));

        /// <summary>
        /// Height the buttons got when the form loaded. The row uses it (scaled by dpiScale)
        /// so the buttons always fill it exactly, instead of a guessed fraction of the window.
        /// </summary>
        int baseButtonHeight;

        // Snipper active
        readonly SnippingToolWindow snipper = new SnippingToolWindow();
        bool snippingWindowActive = false;
        bool shownGifSnipperHelp = false;

        // Edit
        ToolStripMenuItem mode;
        readonly int drawRadius = 10;

        public Shortcut instantKeys;
        public Shortcut cropKeys;
        public Shortcut gifKeys;

        float HUDvisibility = 0;
        Font hudFont;
        int hudFontSize = -1;
        float HUDVisiblity
        {
            get
            {

                if (HUDvisibility < 1)
                    return HUDvisibility;
                else
                    return 1;
            }
        }

        const int halfExtraPreviewImages = 4;
        const int previewImageWidth = 100;
        const int previewImageHeight = 56;
        const int previewImageOutlineThickness = 7;
        const int previewImagePadding = 12;
        const int savedSignFontSize = 15;

        public MainForm()
        {
            InitializeComponent();
            // the window icon is embedded once in the exe instead of inside every form resource
            Icon = Program.AppIcon == null ? null : (Icon)Program.AppIcon.Clone();

            // These are laid out by LayoutChildren, so nothing else may fight for their
            // bounds: the designer anchors kept recomputing them on every resize and they
            // ignore the menu strip, which left the image box under the toolbar.
            foreach (Control laidOut in new Control[] { pBox, bPrevious, bSave, bDelete, bNext })
                laidOut.Anchor = AnchorStyles.None;

            // Restore the window where it was closed. Without Manual, WinForms repositions the
            // form when it is shown (WindowsDefaultLocation), discarding the Location restored
            // in MainForm_Load and dropping the window at the top left of the primary screen.
            StartPosition = FormStartPosition.Manual;
            Diagnostics.Log($"startup: hook id={keyHook.HookId} error={keyHook.LastError} integrity={Diagnostics.IntegrityLevel()} crop={cropKeys} instant={instantKeys} gif={gifKeys}");
            Diagnostics.Log("ctor: before event hook");
            keyHook.KeyUp += KeyHook_KeyUp;
            keyHook.KeyDown += KeyHook_KeyDown;
            CurrentlyFocusedWindow.SetEventHook();
            Diagnostics.Log("ctor: after event hook");

            // Restores the full quality image once the window stopped being resized
            resizeSettleTimer.Tick += ResizeSettle_Tick;
        }

        bool savedGeometryApplied;

        /// <summary>
        /// Puts the window where it was closed. The tool minimises itself during Load (it lives
        /// in the tray) and Windows ignores position changes for a minimised window - it restores
        /// to the placement captured when the window was minimised, which is the default corner.
        /// So this has to run once the window is actually normal: on first show, or when it is
        /// restored from the tray.
        /// </summary>
        void ApplySavedGeometry()
        {
            if (savedGeometryApplied || WindowState != FormWindowState.Normal)
                return;
            savedGeometryApplied = true;

            if (config.Default.windowLocation.IsNullOrWhiteSpace())
                return;
            string[] parts = config.Default.windowLocation.Split(',');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int savedX) &&
                int.TryParse(parts[1], out int savedY))
                Location = new Point(savedX, savedY);
        }


        bool startedMinimized;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ApplySavedGeometry();

            // Minimising here (rather than in Load) is what makes Windows store the correct
            // placement for the tray restore; doing it before the window has its final position
            // is what dropped it in the corner. It costs a brief flash on startup.
            if (!startedMinimized)
            {
                startedMinimized = true;
                Minimize();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplySavedGeometry();
        }

        [StructLayout(LayoutKind.Sequential)]
        struct WindowRect { public int Left, Top, Right, Bottom; }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // WM_DPICHANGED: Windows hands over the window rectangle that keeps the logical
            // size on the new monitor. Adopting it (and re-running the layout) is what makes
            // the UI rescale when the window is dragged between monitors with different
            // scaling. Without it the window keeps its old physical size, so the UI looks
            // oversized on the lower DPI monitor.
            if (m.Msg == 0x02E0 && m.LParam != IntPtr.Zero)
            {
                // Windows hands over the rectangle that keeps the logical size on the new
                // monitor, so its ratio is exactly the factor the window tree needs. Scaling
                // the tree and keeping that layout (instead of re-deriving it) is what makes
                // the UI follow the monitor scaling without mangling the row.
                WindowRect suggested = (WindowRect)Marshal.PtrToStructure(m.LParam, typeof(WindowRect));
                int newWidth = suggested.Right - suggested.Left;
                int newHeight = suggested.Bottom - suggested.Top;
                float ratio = Width > 0 ? newWidth / (float)Width : 1f;
                try
                {
                if (Math.Abs(ratio - 1f) > 0.01f && Width > 0 && Height > 0)
                {
                    // Absolute, never incremental: the new monitor's DPI comes straight from
                    // the message (LOWORD of wParam) and the scale is computed against the DPI
                    // the window was built at, so repeated trips between monitors cannot
                    // compound into a window that is far too big or far too small.
                    if (logicalClientSize.Width <= 0 || logicalClientSize.Height <= 0)
                        logicalClientSize = ClientSize;
                    if (baseDpi <= 0)
                        baseDpi = CurrentDpi();
                    int messageDpi = (int)(m.WParam.ToInt64() & 0xFFFF);
                    float newDpi = messageDpi > 0 ? messageDpi : baseDpi * dpiScale * ratio;
                    float wantedScale = Math.Max(0.25f, Math.Min(4f, newDpi / baseDpi));

                    // Crossing the monitor edge produces several of these messages. Acting on
                    // every one of them - and on the ones that do not change the scale at all -
                    // is what made the window wobble, so only a real scale change is applied.
                    if (crossingMonitors)
                    {
                        Diagnostics.Log($"dpi message debounced: dpi {newDpi:F0}, scale stays {dpiScale:F2}");
                    }
                    else if (Math.Abs(wantedScale - dpiScale) > 0.001f)
                    {
                        lastDpiChange = DateTime.UtcNow;
                        dpiScale = wantedScale;
                        Size wantedClient = new Size(
                            (int)Math.Round(logicalClientSize.Width * dpiScale),
                            (int)Math.Round(logicalClientSize.Height * dpiScale));
                        adjustingForDpi = true;
                        try
                        {
                            ApplyFonts();
                            dpiFactor = Math.Max(0.5f, newDpi / 96f);
                            Bounds = new Rectangle(suggested.Left, suggested.Top,
                                wantedClient.Width + Math.Max(Width - ClientSize.Width, 0),
                                wantedClient.Height + Math.Max(Height - ClientSize.Height, 0));
                        }
                        finally
                        {
                            adjustingForDpi = false;
                        }
                        LayoutChildren();
                        ShowCurrentImage();
                        Diagnostics.Log($"dpi changed: dpi {newDpi:F0}, base {baseDpi:F0}, ratio {ratio:F2}, " +
                                        $"scale {dpiScale:F2}, window {Width}x{Height}, logical {logicalClientSize.Width}x{logicalClientSize.Height}");
                    }
                    else
                    {
                        Diagnostics.Log($"dpi message ignored: dpi {newDpi:F0}, scale already {dpiScale:F2}");
                    }
                }
                }
                catch (Exception dpiError)
                {
                    // A DPI change must never take the tool down: revert to the state the
                    // window had and leave a trace of what went wrong.
                    Diagnostics.Log($"dpi change FAILED - {dpiError}");
                    dpiScale = 1f;
                    adjustingForDpi = false;
                    try { LayoutChildren(); } catch { }
                }
            }
        }

        /// <summary>
        /// Scales the fonts of the window by the given factor. The app builds its fonts with
        /// pixel sizes, so they do not follow the monitor DPI on their own - without this the
        /// text stays at the old monitor's size while the rest of the window rescales.
        /// </summary>
        void ScaleFonts(float ratio)
        {
            Font = ScaleFont(Font, ratio);
            foreach (Control control in AllControls(this))
                control.Font = ScaleFont(control.Font, ratio);
        }

        /// <summary>Scales every control and font of the window by the given factor.</summary>
        void ScaleWindowTree(float ratio)
        {
            Font = ScaleFont(Font, ratio);
            foreach (Control control in AllControls(this))
            {
                Rectangle bounds = control.Bounds;
                control.Bounds = new Rectangle(
                    (int)Math.Round(bounds.X * ratio), (int)Math.Round(bounds.Y * ratio),
                    (int)Math.Round(bounds.Width * ratio), (int)Math.Round(bounds.Height * ratio));
                control.Font = ScaleFont(control.Font, ratio);
            }
        }

        static Font ScaleFont(Font font, float ratio)
        {
            if (font == null || Math.Abs(ratio - 1f) < 0.001f)
                return font;
            try { return new Font(font.FontFamily, font.SizeInPoints * ratio, font.Style, font.Unit); }
            catch { return font; }
        }

        static IEnumerable<Control> AllControls(Control root)
        {
            foreach (Control child in root.Controls)
            {
                yield return child;
                foreach (Control descendant in AllControls(child))
                    yield return descendant;
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            Diagnostics.Log("load: start");
            try {
            if (config.Default.path == "<Unset>" || !Directory.Exists(config.Default.path))
                config.Default.path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (!config.Default.windowLocation.IsNullOrWhiteSpace())
            {
                string[] locationParts = config.Default.windowLocation.Split(',');
                if (locationParts.Length == 2 &&
                    int.TryParse(locationParts[0], out int savedX) &&
                    int.TryParse(locationParts[1], out int savedY))
                    Location = new Point(savedX, savedY);
            }
            else
            {
                // first run: no stored position, so open centred instead of at 0,0
                Rectangle primaryWork = Screen.PrimaryScreen.WorkingArea;
                Location = new Point(
                    primaryWork.X + (primaryWork.Width - Width) / 2,
                    primaryWork.Y + (primaryWork.Height - Height) / 2);
            }
            if (config.Default.windowSize.Width != 0)
            {
                // The saved size is in 96 dpi units, but an aware process works in real
                // pixels - restoring it unscaled is what clipped the button row.
                float windowScale = 1f;
                using (Graphics dpiGraphics = CreateGraphics())
                    windowScale = dpiGraphics.DpiX / 96f;
                Size savedSize = config.Default.windowSize;
                Size = new Size((int)(savedSize.Width * windowScale), (int)(savedSize.Height * windowScale));
            }
            // A missing or unparsable setting unboxes to null here, and that used to throw
            // and abort this handler - which left the shortcut objects null, so every
            // hotkey silently threw inside the keyboard hook and stopped working.
            if (!(config.Default["PrimaryColor"] is Color primaryColor) || primaryColor.Name == "0")
                config.Default.PrimaryColor = Color.Red;

            if (config.Default.instantShortcut.IsNullOrWhiteSpace()) instantKeys = Shortcut.DefaultInstantKeys;
            else instantKeys = new Shortcut().FromString(config.Default.instantShortcut);

            if (config.Default.cropShortcut.IsNullOrWhiteSpace()) cropKeys = Shortcut.DefaultCropKeys;
            else cropKeys = new Shortcut().FromString(config.Default.cropShortcut);

            if (config.Default.gifShortcut.IsNullOrWhiteSpace()) gifKeys = Shortcut.DefaultGifKeys;
            else gifKeys = new Shortcut().FromString(config.Default.gifShortcut);
            Diagnostics.Log($"load: shortcuts instant={instantKeys} crop={cropKeys} gif={gifKeys}");
            } catch (Exception loadException) { Diagnostics.Log("load: FAILED " + loadException); throw; }

            middleButtons.Add(bSave);
            middleButtons.Add(bDelete);

            int MiddleButtonWidth = 0;
            foreach (Button B in middleButtons)
                MiddleButtonWidth += B.Width + 6;
            MiddleButtonWidth -= 6;
            for (int i = 0; i < middleButtons.Count; i++)
                middleButtons[i].Location = new Point(Width / 2 - MiddleButtonWidth / 2 + i * (6 + middleButtons[i].Width) - 8, middleButtons[i].Location.Y);

            int MinWidth = MiddleButtonWidth + 6 * (middleButtons.Count + 1) + 90;
            MinimumSize = new Size(MinWidth, MinWidth);

            MainForm_SizeChanged(null, EventArgs.Empty);
            // Minimising happens in OnShown: Windows captures the placement when a window is
            // minimised and restores to that, so doing it here (before the window has its final
            // position) is what made it come back in the corner.

            Diagnostics.Log("load: listing the screenshot folder");
            bool listingWasCached;
            string[] listedFiles = ScreenshotFolderCache.Load(config.Default.path, out listingWasCached);
            if (listedFiles.Length > 0)
            {
                images.AddRange(listedFiles.Select(x => new Screenshot(x)));
                imagesIndex = images.Count - 1;
            }
            else
            {
                images.Add(new Screenshot(ScreenshotHelper.GetFullScreenshot(), GetScreenshotName()));
                imagesIndex = 0;
            }

            if (listingWasCached)
            {
                // The window is already usable; rescan off the UI thread and only touch the
                // strip when the folder really changed, so the listing can never end up stale.
                string scannedFolder = config.Default.path;
                string[] shownListing = listedFiles;
                Task.Run(() =>
                {
                    string[] fresh;
                    try { fresh = ScreenshotFolderCache.Scan(scannedFolder); }
                    catch (Exception scanError)
                    {
                        Diagnostics.Log("load: background scan failed - " + scanError.Message);
                        return;
                    }
                    ScreenshotFolderCache.Save(scannedFolder, fresh);
                    if (!fresh.SequenceEqual(shownListing, StringComparer.Ordinal))
                        try { BeginInvoke((MethodInvoker)(() => ApplyListing(fresh))); } catch (ObjectDisposedException) { }
                    else
                        Diagnostics.Log("load: cached listing is still current");
                });
            }
            else
            {
                ScreenshotFolderCache.Save(config.Default.path, listedFiles);
            }

            noneMenuItem.Checked = true;

            UpdateWindowRatioWidth();
            UpdateUI();

            // Baseline for the per-monitor scaling: the DPI this window was built at, its
            // logical size, and the fonts before any scaling is applied.

            // The window can open on either monitor. WinForms scaled it for the DPI it assumed
            // while the form was created, so if that is not the monitor it landed on, the whole
            // UI is off by that ratio until the first monitor change. Normalise it right here.
            float assumedDpi = CurrentDpi();
            float landedDpi = WindowDpi();

            // baseDpi must be the DPI the fonts and the layout constants were built at - the one
            // WinForms used while creating the form - because every later scale is measured
            // against it and logicalClientSize is stored in that space. Using the landed monitor
            // here made the first crossing compute 192/96 on top of an already doubled size,
            // which is why a window opened on the 1x screen came back at about 4x on the 4K one.
            baseDpi = assumedDpi > 0 ? assumedDpi : 96f;
            dpiFactor = Math.Max(0.5f, landedDpi / 96f);
            CaptureBaseFonts();

            float startupScale = landedDpi > 0 ? Math.Max(0.25f, Math.Min(4f, landedDpi / baseDpi)) : 1f;
            if (Math.Abs(startupScale - 1f) > 0.01f)
            {
                dpiScale = startupScale;
                ApplyFonts();
                adjustingForDpi = true;
                try
                {
                    ClientSize = new Size(
                        (int)Math.Round(ClientSize.Width * startupScale),
                        (int)Math.Round(ClientSize.Height * startupScale));
                }
                finally
                {
                    adjustingForDpi = false;
                }
                Diagnostics.Log($"startup: landed on {landedDpi:F0} dpi, built for {baseDpi:F0}, " +
                                $"scaled by {startupScale:F2}, client {ClientSize.Width}x{ClientSize.Height}");
            }
            logicalClientSize = new Size(
                (int)Math.Round(ClientSize.Width / Math.Max(dpiScale, 0.25f)),
                (int)Math.Round(ClientSize.Height / Math.Max(dpiScale, 0.25f)));

            // Keep the stored geometry usable: the window may have been saved on the other
            // monitor, so clamp it into the working area of the screen it belongs to, and never
            // outside the virtual desktop - but do not drag it onto the primary screen.
            Rectangle virtualScreen = SystemInformation.VirtualScreen;
            Rectangle targetScreen = Screen.FromRectangle(new Rectangle(Location, Size)).WorkingArea;
            Size fit = Size;
            fit.Width = Math.Min(fit.Width, targetScreen.Width);
            fit.Height = Math.Min(fit.Height, targetScreen.Height);
            Size = fit;
            Location = new Point(
                Math.Max(virtualScreen.Left, Math.Min(Location.X, virtualScreen.Right - Width)),
                Math.Max(virtualScreen.Top, Math.Min(Location.Y, virtualScreen.Bottom - Height)));

            LayoutChildren();
        }

        /// <summary>Replaces the strip with a freshly scanned listing, keeping the selection.</summary>
        void ApplyListing(string[] files)
        {
            string viewing = CurrentScreenshot?.Path;
            images.Clear();
            foreach (string file in files)
                images.Add(new Screenshot(file));
            imagesIndex = viewing == null ? 0 : images.FindIndex(x => string.Equals(x.Path, viewing, StringComparison.OrdinalIgnoreCase));
            if (imagesIndex < 0)
                imagesIndex = Math.Max(0, images.Count - 1);
            UpdateUI();
            Diagnostics.Log($"load: listing refreshed, {files.Length} screenshot(s)");
        }

        // Image
        public void AddScreenShot()
        {
            try
            {
                System.Media.SystemSounds.Exclamation.Play();
                images.Add(new Screenshot(ScreenshotHelper.GetFullScreenshot(), GetScreenshotName()));
                imagesIndex = images.Count - 1;
                UpdateUI();
            }
            catch (Exception e)
            {
                if (MessageBox.Show("Oopsie woopsie, it seems like I cant make that screenshot!\nDo you want to see the error message in detail?",
                    "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    MessageBox.Show(e.Message + "\n\n" + e.InnerException + "\n\n" + e.StackTrace);
            }
        }
        public void AddScreenShotSnippingToolStyle()
        {
            if (snippingWindowActive)
                return;

            lock (snipper)
            {
                try
                {
                    snippingWindowActive = true;
                    System.Media.SystemSounds.Exclamation.Play();
                    // Capture first: the snipper window covers the whole screen, and the
                    // HDR aware capture has to read the screen while it is still visible.
                    Diagnostics.Log("snipper: capturing");
                    snipper.Prepare(ScreenshotHelper.GetFullScreenshot());
                    Diagnostics.Log("snipper: showing the selection window");
                    snipper.ShowDialog();
                    Diagnostics.Log($"snipper: closed, output={(snipper.output == null ? "none" : "crop")}");
                    if (snipper.output != null)
                    {
                        try
                        {
                            images.Add(new Screenshot(snipper.output, GetScreenshotName()));
                            imagesIndex = images.Count - 1;
                            UpdateUI();

                            BSave_Click(null, EventArgs.Empty);

                            Point mouseDownPoint = snipper.cropStartPosInScreenCoords;
                            Rectangle imageDimensions = ScreenshotHelper.AllScreenBounds;
                            snipper.InvokeIfRequired(() => this.Location = new Point(mouseDownPoint.X - 20, mouseDownPoint.Y - 58));

                            WindowState = FormWindowState.Normal;
                            DLLImports.SetForegroundWindow(Handle);

                            SetOriginalSize();

                            // Ausgleichen des blankParts
                            Bitmap shownImage = CurrentScreenshot.CachedImage;
                            int imgWidth = shownImage.Width;
                            int imgHeight = shownImage.Height;
                            int boxWidth = pBox.Size.Width;
                            int boxHeight = pBox.Size.Height;
                            float X = 0;
                            float Y = 0;
                            if (imgWidth / imgHeight > boxWidth / boxHeight)
                            {
                                float scale = boxWidth / (float)imgWidth;
                                float blankPart = (boxHeight - scale * imgHeight) / 2;
                                Y = blankPart;
                            }
                            else
                            {
                                float scale = boxHeight / (float)imgHeight;
                                float blankPart = (boxWidth - scale * imgWidth) / 2;
                                X = blankPart;
                            }
                            Location = new Point(Location.X - (int)X, Location.Y - (int)Y);
                            snipper.CleanUp();
                        }
                        catch (Exception e)
                        {
                            Diagnostics.Log($"snipper: failed - {e.GetType().Name}: {e.Message}");
                            if (MessageBox.Show("Oopsie woopsie, it seems like I cant make that screenshot!\nDo you want to see the error message in detail?", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                MessageBox.Show(e.Message + "\n\n" + e.InnerException + "\n\n" + e.StackTrace);
                            }
                        }
                        snippingWindowActive = false;
                    }
                    else
                        snippingWindowActive = false;
                }
                catch (Exception e)
                {
                    if (MessageBox.Show("Oopsie woopsie, it seems like I cant make that screenshot!\nDo you want to see the error message in detail?", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        MessageBox.Show(e.Message + "\n\n" + e.InnerException + "\n\n" + e.StackTrace);
                    }
                }
            }
        }
        public string GetScreenshotName()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1999, 5, 4));
            string fileName = "Screenshot_" + (long.MaxValue - (long)t.TotalMilliseconds);
            try { fileName += "_" + CurrentlyFocusedWindow.ProcessName; }
            catch { }
            return fileName;
        }
        public void SaveCurrentImage()
        {
            Task.Run(() => images[imagesIndex].Save());
            images[imagesIndex].PutInClipboard();
            UpdateUI();
        }
        public void CopyCurrentImageToClipboard()
        {
            RunWhenCurrentImageLoaded(() =>
            {
                if (CurrentScreenshot.CachedImage.IsAnimatedGif())
                {
                    string[] files = new string[1]; files[0] = CurrentScreenshot.Path;
                    this.DoDragDrop(new DataObject(DataFormats.FileDrop, files), DragDropEffects.Copy);
                }
                CurrentScreenshot.PutInClipboard();
            });
        }
        public void DeleteCurrentImage()
        {
            if (images.Count > 1)
            {
                Screenshot deleting = CurrentScreenshot;
                deleting.DisposeImageCache();
                // Dropping the file (and waiting for a save that is still running)
                // talks to the drive, so it must not happen on the UI thread.
                Task.Run(() => deleting.Delete());
                images.RemoveAt(imagesIndex);
                if (imagesIndex > images.Count - 1)
                    imagesIndex = images.Count - 1;
                UpdateUI();
            }
        }
        public Screenshot CurrentScreenshot { get { return images[imagesIndex]; } }

        // Image cache
        /// <summary>
        /// Makes sure the current image and the ones drawn as previews are in memory.
        /// The files are read in parallel on background threads, so a slow drive (a
        /// spun down NAS) never freezes the UI - the images just pop in once they are
        /// there. Work for a window the user has already navigated away from is
        /// dropped as long as it has not started reading yet.
        /// </summary>
        private void RequestImageLoads()
        {
            // The list is only touched on the UI thread, so take the window with us.
            List<Screenshot> window = new List<Screenshot>();
            foreach (int i in PreviewWindowIndices(imagesIndex))
                window.Add(images[i]);

            if (!window.Any(s => s.CachedImage == null))
                return;

            int version = ++imageLoadVersion;

            Task.Run(() =>
            {
                try
                {
                    Parallel.ForEach(window, new ParallelOptions { MaxDegreeOfParallelism = imageLoadParallelism }, shot =>
                    {
                        if (Volatile.Read(ref imageLoadVersion) != version)
                            return;
                        if (shot.CachedImage != null)
                            return;

                        shot.LoadOnCurrentThread();
                        // build the preview here, on the worker, so a repaint never
                        // has to scale a full size image down to thumbnail size
                        shot.GetThumbnail(previewImageWidth, previewImageHeight);
                        OnImageLoaded(shot);
                    });
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Loading images failed: {e}");
                }
            });
        }

        /// <summary>Indices of the preview window around center, nearest images first.</summary>
        private IEnumerable<int> PreviewWindowIndices(int center)
        {
            yield return center;
            for (int offset = 1; offset <= halfExtraPreviewImages; offset++)
            {
                if (center - offset >= 0)
                    yield return center - offset;
                if (center + offset < images.Count)
                    yield return center + offset;
            }
        }

        /// <summary>
        /// Shows a bitmap in the picture box. Assigning PictureBox.Image invalidates
        /// the control even when it is handed the very same bitmap, and a repaint
        /// scales the whole image again - doing that for every mouse move is what
        /// pegged a core.
        /// </summary>
        private void ShowImageInPictureBox(Bitmap image)
        {
            if (!ReferenceEquals(pBox.Image, image))
                pBox.Image = image;
        }

        /// <summary>
        /// Shows the current image. While the window is being resized a cached, much
        /// smaller copy of it is used: every repaint scales the image down to the panel,
        /// and doing that for a big multi monitor screenshot on every single size change
        /// is what made dragging the window edge eat a whole core. Full quality comes
        /// back shortly after the resizing stopped.
        /// </summary>
        private void ShowCurrentImage()
        {
            // MainForm_SizeChanged runs while the form loads, before any screenshot exists
            if (images.Count == 0)
            {
                ShowImageInPictureBox(null);
                return;
            }

            Screenshot current = CurrentScreenshot;
            Bitmap image = current.CachedImage;
            if (image == null)
            {
                ShowImageInPictureBox(null);
                return;
            }

            if (showingResizePreview)
            {
                int width = Math.Min(resizePreviewWidth, image.Width);
                Bitmap preview = current.GetScaled(width, Math.Max(1, (int)((long)image.Height * width / image.Width)));
                if (preview != null)
                    image = preview;
            }

            ShowImageInPictureBox(image);
        }
        /// <summary>
        /// Leaves the fast resize preview and puts the full quality image back on
        /// screen. Has to happen before anything drops the cached copies, otherwise
        /// the picture box could be left holding a disposed bitmap.
        /// </summary>
        private void StopResizePreview()
        {
            resizeSettleTimer.Stop();
            if (!showingResizePreview)
                return;

            showingResizePreview = false;
            ShowCurrentImage();
        }
        private void ResizeSettle_Tick(object sender, EventArgs e)
        {
            StopResizePreview();     // full quality again, once resizing has stopped
        }

        /// <summary>Called from a background thread once an image has been read from disk.</summary>
        private void OnImageLoaded(Screenshot shot)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            try
            {
                this.InvokeIfRequired(() =>
                {
                    if (IsDisposed)
                        return;
                    if (shot == CurrentScreenshot)
                    {
                        ShowCurrentImage();
                        if (shot.CachedImage != null)
                            RunPendingImageActions();
                    }
                    pBox.Invalidate();
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Refreshing after loading an image failed: {e.Message}");
            }
        }

        /// <summary>
        /// Runs work that needs the pixels of the current image as soon as it has been
        /// read from disk, so a button click never has to wait for a slow drive.
        /// </summary>
        private void RunWhenCurrentImageLoaded(Action action)
        {
            if (CurrentScreenshot.CachedImage != null)
            {
                action();
                return;
            }

            pendingImageActions.Add(action);
            RequestImageLoads();
        }
        private void RunPendingImageActions()
        {
            if (pendingImageActions.Count == 0)
                return;

            Action[] actions = pendingImageActions.ToArray();
            pendingImageActions.Clear();
            foreach (Action action in actions)
            {
                try { action(); }
                catch (Exception e) { Debug.WriteLine($"Deferred image action failed: {e}"); }
            }
        }

        // GIF
        public void StartRecordingGif()
        {
            if (processingGif | recordingGif)
                return;

            if (snipper.gifArea.Width == 0 || snipper.gifArea.Height == 0)
            {
                if (!shownGifSnipperHelp)
                {
                    shownGifSnipperHelp = true;
                    DialogResult result = MessageBox.Show("You need to select an gif area in the SelectionForm™ before recording a gif!", "Missing Gif Area", MessageBoxButtons.OK);
                    if (result == DialogResult.OK)
                        shownGifSnipperHelp = false;
                }
                return;
            }

            processingGif = true;
            recordingGif = true;
            gifShots.Clear();

            Task.Factory.StartNew(() =>
            {
                List<Task> runners = new List<Task>();
                for (int i = 0; i < 750 && recordingGif; i++)
                {
                    runners.Add(Task.Factory.StartNew(() =>
                    {
                        gifShots.Add(ScreenshotHelper.GetRectScreenshot(snipper.gifArea));
                        Debug.WriteLine($"Made gif shot {i}!");
                    }));

                    Task.Delay(33).Wait();
                }
                Debug.WriteLine("waiting for close...");
                Task.WaitAll(runners.ToArray());
                Debug.WriteLine("closed");

                string path = config.Default.path + "\\" + GetScreenshotName() + ".gif";
                using (FileStream s = new FileStream(path, FileMode.Create))
                {
                    using AnimatedGifCreator c = new AnimatedGifCreator(s, 40);
                    foreach (Bitmap b in gifShots)
                        c.AddFrame(b, -1, GifQuality.Default);
                }

                images.Add(new Screenshot(path));
                imagesIndex = images.Count - 1;
                CurrentScreenshot.Save();

                this.InvokeIfRequired(() => UpdateUI());

                processingGif = false;
            });
        }
        public void StopRecordingGif()
        {
            recordingGif = false;
            Debug.WriteLine("recordingGif is now false");
        }

        // UI
        public void UpdateUI()
        {
            Screenshot current = CurrentScreenshot;
            Text = $"Screenshot Tool - {images.Count} saved screenshots!" +
                $"{(current.Path.IsNullOrWhiteSpace() ? "" : $" - {Path.GetFileNameWithoutExtension(current.Path)} ")} - Dir: {config.Default.path}";
            // null while the file is still being read; it is set by OnImageLoaded
            ShowCurrentImage();
            if (current.Saved)
                bSave.Text = "To Clipboard";
            else
                bSave.Text = "Save";

            if (mode == textRecognitionMenuItem)
                textView.UpdateReadings();

            bDelete.Enabled = images.Count > 1;
            bPrevious.Enabled = imagesIndex != 0;
            bNext.Enabled = imagesIndex != images.Count - 1;

            RequestImageLoads();
        }
        public void UpdateWindowRatioWidth()
        {
            Screenshot current = CurrentScreenshot;
            if (current.CachedImage == null)
            {
                RunWhenCurrentImageLoaded(UpdateWindowRatioWidth);
                return;
            }

            Size R = GetProperRatioSize(pBox.Size, true, current.CachedImage.Width /
                current.CachedImage.Height);
            Width += R.Width - pBox.Width;
            Height += R.Height - pBox.Height;
        }
        public void UpdateWindowRatioHeight()
        {
            Screenshot current = CurrentScreenshot;
            if (current.CachedImage == null)
            {
                RunWhenCurrentImageLoaded(UpdateWindowRatioHeight);
                return;
            }

            Size R = GetProperRatioSize(pBox.Size, false, current.CachedImage.Width /
                current.CachedImage.Height);
            Height += R.Height - pBox.Height;
            Width += R.Width - pBox.Width;
        }
        private void ResetHudVisibility()
        {
            // While the HUD is fully visible nothing it draws would change, so do not
            // restart the fade animation - moving the mouse would otherwise repaint the
            // picture box again and again for no visible difference.
            if (HUDvisibility >= 1)
                return;

            HUDvisibility = (7.5f - HUDvisibility) / 3f;
            HudDisappearance.Enabled = true;
            try { pBox.Invalidate(); } catch { }     // show the HUD now, the timer fades it out
        }
        public void Minimize() => DLLImports.ShowWindow(this.Handle, 2);
        public void SetModeToNone() => ChangeEditMode(noneMenuItem, false);
        // Window Size
        public void SetOriginalSize()
        {
            Screenshot current = CurrentScreenshot;
            if (current.CachedImage == null)
            {
                RunWhenCurrentImageLoaded(SetOriginalSize);
                return;
            }

            // doppelt hält besser :thonk:
            for (int i = 0; i < 2; i++)
            {
                Width = (int)(current.CachedImage.Width / ScreenshotHelper.MaxScreenScalingFactor) + Width - pBox.Width;
                Height = (int)(current.CachedImage.Height / ScreenshotHelper.MaxScreenScalingFactor) + Height - pBox.Height;
            }
        }
        public void CenterAroundMouse()
        {
            Location = new Point(MousePosition.X - Width / 2, MousePosition.Y - Height / 2);
        }
        public Size GetProperRatioSize(Size S, bool WidthFirst, float Ratio)
        {
            if (WidthFirst)
            {
                S.Width = (int)(S.Height * Ratio);
                S.Height = (int)(S.Width * (1 / Ratio));
            }
            else
            {
                S.Height = (int)(S.Width * (1 / Ratio));
                S.Width = (int)(S.Height * Ratio);
            }
            return S;
        }
        Rectangle GetRectangleFromPoints(Point P1, Point P2)
        {
            int X = Math.Min(P1.X, P2.X);
            int Width = Math.Max(P1.X, P2.X) - X;
            int Y = Math.Min(P1.Y, P2.Y);
            int Height = Math.Max(P1.Y, P2.Y) - Y;
            return new Rectangle(X, Y, Width, Height);
        }
        public Point ZoomPicBoxCoordsToImageCoords(Point P, PictureBox pBox)
        {
            Bitmap image = CurrentScreenshot.CachedImage;
            if (image == null)
                return P;

            int imgWidth = image.Width;
            int imgHeight = image.Height;
            int boxWidth = pBox.Size.Width;
            int boxHeight = pBox.Size.Height;

            //This variable will hold the result
            float X = P.X;
            float Y = P.Y;
            //Comparing the aspect ratio of both the control and the image itself.
            if (imgWidth / imgHeight > boxWidth / boxHeight)
            {
                //If true, that means that the image is stretched through the width of the control.
                //'In other words: the image is limited by the width.

                //The scale of the image in the Picture Box.
                float scale = boxWidth / (float)imgWidth;

                //Since the image is in the middle, this code is used to determinate the empty space in the height
                //'by getting the difference between the box height and the image actual displayed height and dividing it by 2.
                float blankPart = (boxHeight - scale * imgHeight) / 2;

                Y -= blankPart;

                //Scaling the results.
                X /= scale;
                Y /= scale;
            }
            else
            {
                //If true, that means that the image is stretched through the height of the control.
                //'In other words: the image is limited by the height.

                //The scale of the image in the Picture Box.
                float scale = boxHeight / (float)imgHeight;

                //Since the image is in the middle, this code is used to determinate the empty space in the width
                //'by getting the difference between the box width and the image actual displayed width and dividing it by 2.
                float blankPart = (boxWidth - scale * imgWidth) / 2;
                X -= blankPart;

                //Scaling the results.
                X /= scale;
                Y /= scale;
            }
            return new Point((int)X, (int)Y);
        }
        private void ChangeEditMode(ToolStripMenuItem newMode, bool CloseWindow = true)
        {
            if (newMode == colorPickerMenuItem)
            {
                if (colorView == null || colorView.IsDisposed)
                    colorView = new ColorView();
                colorView.Show();
            }
            else if (CloseWindow)
                colorView.Close();

            if (newMode == textRecognitionMenuItem)
            {
                if (textView == null || textView.IsDisposed)
                    textView = new TextRecognitionView();
                textView.Show();
            }
            else if (CloseWindow)
                textView.Close();

            if (newMode == chooseColorMenuItem)
            {
                var LeDialog = new ColorDialog
                {
                    AllowFullOpen = true,
                    AnyColor = true,
                    Color = config.Default.PrimaryColor
                };
                if (LeDialog.ShowDialog() == DialogResult.OK)
                    config.Default.PrimaryColor = LeDialog.Color;
            }

            foreach (ToolStripItem item in editMenuItem.DropDownItems)
                if (item is ToolStripMenuItem)
                    (item as ToolStripMenuItem).Checked = item == newMode;

            mode = newMode;
        }

        // Button Events
        private void BSave_Click(object sender, EventArgs e)
        {
            if (images[imagesIndex].Saved)
                CopyCurrentImageToClipboard();
            else
                SaveCurrentImage();
        }
        private void BPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog
            {
                SelectedPath = config.Default.path,
                Description = "Set the folder ill dump all the screensohts in."
            };
            if (FBD.ShowDialog() == DialogResult.OK)
                config.Default.path = FBD.SelectedPath;
            config.Default.Save();
            UpdateUI();
        }
        private void BPrevious_Click(object sender, EventArgs e)
        {
            if (imagesIndex + 4 < images.Count)
                images[imagesIndex + 4].DisposeImageCache();
            imagesIndex--;

            pendingImageActions.Clear();
            ResetHudVisibility();
            UpdateUI();
        }
        private void BNext_Click(object sender, EventArgs e)
        {
            if (imagesIndex - 4 >= 0)
                images[imagesIndex - 4].DisposeImageCache();
            imagesIndex++;

            pendingImageActions.Clear();
            ResetHudVisibility();
            UpdateUI();
        }
        private void BOpen_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "/open , \"" + config.Default.path);
        }
        private void BCropScreenshot_Click(object sender, EventArgs e)
        {
            AddScreenShotSnippingToolStyle();
        }
        private void BScreenshot_Click(object sender, EventArgs e)
        {
            AddScreenShot();
        }
        private void BDelete_Click(object sender, EventArgs e) => DeleteCurrentImage();

        // PictureBox Events
        /// <summary>
        /// Font for the HUD texts. Cached, because the picture box can repaint many
        /// times per second and creating a Font for every paint is needlessly slow.
        /// </summary>
        private Font GetHudFont()
        {
            int size = Math.Min(Px(savedSignFontSize), Math.Max(pBox.Height, 1)) + 1;
            if (hudFont == null || hudFontSize != size)
            {
                if (hudFont != null)
                    hudFont.Dispose();
                hudFont = new Font("Arial", size, FontStyle.Italic);
                hudFontSize = size;
            }
            return hudFont;
        }
        private void PBox_Paint(object sender, PaintEventArgs e)
        {
            if (isMouseDown && mode == cropMenuItem)
            {
                Rectangle ee = GetRectangleFromPoints(pMouseDown, pMouseCurrently);
                using (Pen pen = new Pen(config.Default.PrimaryColor, 1))
                    e.Graphics.DrawRectangle(pen, ee);
            }

            Screenshot current = CurrentScreenshot;

            // The image is still on its way from disk - drawing here must never wait for it
            if (current.CachedImage == null && !current.LoadFailed && pBox.Height > 8)
            {
                Font font = GetHudFont();
                using (StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                    e.Graphics.DrawString("Loading...", font, Brushes.Gray,
                        new RectangleF(0, 0, pBox.Width, pBox.Height), format);
            }

            // Unsaved title
            if (!current.Saved && pBox.Height > 8)
            {
                //try
                //{
                //    using (Pen pen = new Pen(Color.Red, 1))
                //        e.Graphics.DrawString("Saved!", new Font("BigNoodleTitling", Math.Min(savedSignFontSize, pBox.Height) + 1, FontStyle.Italic),
                //            Brushes.Red, new PointF(0, HUDVisiblity * (savedSignFontSize + 15) - savedSignFontSize - 15));
                //}
                //catch
                //{
                using (Pen pen = new Pen(Color.Red, 1))
                    e.Graphics.DrawString("Unsaved!", GetHudFont(),
                        Brushes.Red, new PointF(0, HUDVisiblity * (Px(savedSignFontSize) + Px(15)) - Px(savedSignFontSize) - Px(15)));
                //}
            }
            // Previews (the constants are 96 dpi values, so they follow the monitor DPI)
            int previewWidth = Px(previewImageWidth);
            int previewHeight = Px(previewImageHeight);
            int previewGap = Px(previewImagePadding);
            int previewOutline = Px(previewImageOutlineThickness);
            for (int i = imagesIndex - halfExtraPreviewImages; i < imagesIndex + halfExtraPreviewImages + 1; i++)
            {
                if (i >= 0 && i < images.Count)
                {
                    int index = i - imagesIndex;
                    Rectangle draw = new Rectangle(pBox.Width / 2 - (previewWidth / 2) + index * (previewWidth + previewGap),
                        pBox.Height - (int)Math.Min((previewHeight + previewOutline * 2) * HUDVisiblity, pBox.Height) + previewOutline,
                        previewWidth, previewHeight);

                    if (index == 0)
                        using (Pen pen = new Pen(Color.Black, previewOutline))
                            e.Graphics.DrawRectangle(pen, draw);

                    // Neighbours that are not in memory yet are requested by
                    // RequestImageLoads and simply skipped until they arrive.
                    Bitmap preview = images[i].GetThumbnail(previewWidth, previewHeight);
                    if (preview != null)
                        e.Graphics.DrawImage(preview, draw);
                }
            }
        }
        private void PBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                //m.MenuItems.Add(new MenuItem("Fix Width Ratio", ((object s, EventArgs ev) =>
                //{
                //    try
                //    {
                //        UpdateWindowRatioWidth();
                //    }
                //    catch { }
                //})));
                //m.MenuItems.Add(new MenuItem("Fix Height Ratio", ((object s, EventArgs ev) =>
                //{
                //    try
                //    {
                //        UpdateWindowRatioHeight();
                //    }
                //    catch { }
                //})));
                GraphicsUnit Unit = GraphicsUnit.Pixel;
                Bitmap current = CurrentScreenshot.CachedImage;
                if (current != null && current.GetBounds(ref Unit).Width == ScreenshotHelper.AllScreenBounds.Width)
                {
                    int i = 1;
                    foreach (Screen S in Screen.AllScreens)
                    {
                        m.MenuItems.Add(new MenuItem("Crop to " + i.ToShitEnglishNumberThingy() + " Screen", ((object s, EventArgs ev) =>
                        {
                            try
                            {
                                images.Insert(imagesIndex + 1, new Screenshot(ScreenshotHelper.CropImage(current,
                                    new Rectangle(S.Bounds.X - ScreenshotHelper.AllScreenBounds.X,
                                    S.Bounds.Y - ScreenshotHelper.AllScreenBounds.Y,
                                    S.Bounds.Width, S.Bounds.Height)), images[imagesIndex].FileName + "_CROPPED"));
                                imagesIndex++;
                                UpdateUI();
                            }
                            catch { }
                        })));
                        i++;
                    }
                }
                m.MenuItems.Add(new MenuItem("1:1 Size", ((object s, EventArgs ev) =>
                {
                    try
                    {
                        SetOriginalSize();
                    }
                    catch { }
                })));
                m.MenuItems.Add(new MenuItem("Smol Size", ((object s, EventArgs ev) =>
                {
                    try
                    {
                        Height = 350;
                        Width = 350;
                        CenterAroundMouse();
                    }
                    catch { }
                })));
                m.Show(pBox, e.Location);
            }
        }
        private void PBox_MouseMove(object sender, MouseEventArgs e)
        {
            pMouseCurrently = e.Location;
            ResetHudVisibility();

            Bitmap current = CurrentScreenshot.CachedImage;
            ShowImageInPictureBox(current);

            bool needsRepaint = false;

            if (isMouseDown && current != null && mode == colorPickerMenuItem)
            {
                try
                {
                    Point p = ZoomPicBoxCoordsToImageCoords(pMouseCurrently, pBox);
                    colorView.Update(current.GetPixel(p.X, p.Y));
                }
                catch { }
            }
            if (isMouseDown && current != null && mode == drawMenuItem)
            {
                if (CurrentScreenshot.Saved)
                {
                    images.Insert(imagesIndex + 1, new Screenshot((Bitmap)current.Clone(),
                        CurrentScreenshot.FileName + "_DRAWN"));
                    imagesIndex += 1;
                    UpdateUI();
                }

                Bitmap drawTarget = CurrentScreenshot.CachedImage;
                if (drawTarget != null)
                    using (Graphics g = Graphics.FromImage(drawTarget))
                    {
                        Point mCur = ZoomPicBoxCoordsToImageCoords(pMouseCurrently, pBox);
                        Point mLast = ZoomPicBoxCoordsToImageCoords(pMouseLast, pBox);

                        int length = (int)Math.Sqrt(Math.Pow(pMouseCurrently.X - pMouseLast.X, 2) + Math.Pow(pMouseCurrently.Y - pMouseLast.Y, 2));
                        if (length > drawRadius / 2)
                            for (int i = 0; i < length; i++)
                            {
                                float uwu = i / (float)length;
                                int X = (int)(uwu * (mCur.X - drawRadius) + (1 - uwu) * (mLast.X - drawRadius));
                                int Y = (int)(uwu * (mCur.Y - drawRadius) + (1 - uwu) * (mLast.Y - drawRadius));
                                g.FillEllipse(new SolidBrush(config.Default.PrimaryColor), new Rectangle(X, Y, drawRadius * 2, drawRadius * 2));
                            }
                        else
                            g.FillEllipse(new SolidBrush(config.Default.PrimaryColor),
                                new Rectangle(mCur.X - drawRadius, mCur.Y - drawRadius, drawRadius * 2, drawRadius * 2));
                    }

                needsRepaint = true;    // the drawn pixels changed
                StopResizePreview();    // never drop a bitmap the picture box still shows
                CurrentScreenshot.InvalidateScaledCopies();
            }
            if (isMouseDown && mode == cropMenuItem)
                needsRepaint = true;    // the selection rectangle follows the mouse

            ShowCurrentImage();
            if (needsRepaint)
                pBox.Invalidate();      // coalesced instead of Refresh(): never blocks the UI thread
            pMouseLast = e.Location;
        }
        private void PBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pMouseDown = e.Location;
                isMouseDown = true;

                if (mode == colorPickerMenuItem)
                {
                    try
                    {
                        Bitmap current = CurrentScreenshot.CachedImage;
                        if (current != null)
                        {
                            Point p = ZoomPicBoxCoordsToImageCoords(pMouseCurrently, pBox);
                            colorView.Update(current.GetPixel(p.X, p.Y));
                        }
                    }
                    catch { }
                }
            }
        }
        private void PBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && mode == cropMenuItem)
            {
                Rectangle crop = GetRectangleFromPoints(
                        ZoomPicBoxCoordsToImageCoords(pMouseDown, pBox),
                        ZoomPicBoxCoordsToImageCoords(pMouseCurrently, pBox));
                Bitmap current = CurrentScreenshot.CachedImage;
                if (current == null || crop.Width == 0 || crop.Height == 0)
                {
                    isMouseDown = false;
                    return;
                }
                images.Insert(imagesIndex + 1, new Screenshot(ScreenshotHelper.CropImage(current, crop),
                    CurrentScreenshot.FileName + "_CROPPED"));
                imagesIndex += 1;
                UpdateUI();
            }
            isMouseDown = false;
        }

        // Other Events
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (images.Skip(1).ToList().Exists(x => !x.Saved) &&
                MessageBox.Show("Oi, you have unsaved Images! Do you really want to close me?", "Close?", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (hudFont != null)
                hudFont.Dispose();
            resizeSettleTimer.Stop();
            resizeSettleTimer.Dispose();

            // Saving the user settings writes to %LOCALAPPDATA%; if that is not writable
            // (roaming profile, restricted token, antivirus) the exception would surface
            // as a crash dialog while closing the tool - losing the settings is bad enough.
            try
            {
                config.Default.windowSize = Size;
                config.Default.windowLocation = $"{Location.X},{Location.Y}";
                config.Default.instantShortcut = instantKeys.ToString();
                config.Default.cropShortcut = cropKeys.ToString();
                config.Default.gifShortcut = gifKeys.ToString();
                config.Default.Save();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Could not save the settings: " + exception);
            }
        }
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            ResetHudVisibility();

            // Repaint from a cached smaller copy until the resizing stops
            if (!showingResizePreview)
            {
                showingResizePreview = true;
                ShowCurrentImage();
            }
            resizeSettleTimer.Stop();
            resizeSettleTimer.Start();

            if (!adjustingForDpi && !crossingMonitors && dpiScale > 0)
                logicalClientSize = new Size((int)Math.Round(ClientSize.Width / dpiScale),
                                             (int)Math.Round(ClientSize.Height / dpiScale));

            // The geometry comes from one place, so it can never disagree with itself.
            LayoutChildren();
        }
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            HudDisappearance.Enabled = this.WindowState != FormWindowState.Minimized;
        }
        private void HudDisappearance_Tick(object sender, EventArgs e)
        {
            var disappearSpeed = 0.15f;
            float visibleBefore = HUDVisiblity;      // the clamped value the painting uses

            HUDvisibility -= disappearSpeed;
            if (HUDvisibility <= -disappearSpeed)
            {
                HUDvisibility = -disappearSpeed;
                // Fully hidden: stop the animation instead of repainting the picture
                // box forever, there is nothing left to change.
                HudDisappearance.Enabled = false;
            }

            if (Math.Abs(HUDVisiblity - visibleBefore) > 0.001f)
            {
                // Only when the HUD really moves - and asynchronously, Refresh() would
                // paint right here and stall the UI thread for every single tick.
                try { pBox.Invalidate(); } catch { }
            }
        }
        private void KeyHook_KeyDown(Keys key, bool Shift, bool Ctrl, bool Alt)
        {
            Debug.WriteLine($"Pressed {key}, {Shift}, {Ctrl}, {Alt}");

            // Only the configured hotkeys are logged, so ordinary typing never ends up in the file
            bool isShortcut = instantKeys.IsPressed(Shift, Ctrl, Alt, key) || cropKeys.IsPressed(Shift, Ctrl, Alt, key) || gifKeys.IsPressed(Shift, Ctrl, Alt, key);
            if (isShortcut)
                Diagnostics.Log($"hotkey {key} shift={Shift} ctrl={Ctrl} alt={Alt} gap={DateTime.Now.Subtract(lastKeyDownEvent).TotalMilliseconds:F0}ms");

            // This runs inside a WH_KEYBOARD_LL hook: Windows silently removes a hook whose
            // callback takes too long, so the actual work is posted to the message queue
            // instead of being done here. Capturing the screen takes a few hundred ms and
            // the snipper keeps a modal loop open, both far too slow for a hook callback.
            if (DateTime.Now.Subtract(lastKeyDownEvent).TotalMilliseconds > 300)
            {
                if (instantKeys.IsPressed(Shift, Ctrl, Alt, key))
                {
                    lastKeyDownEvent = DateTime.Now;
                    BeginInvoke((MethodInvoker)AddScreenShot);
                }
                else if (cropKeys.IsPressed(Shift, Ctrl, Alt, key))
                {
                    lastKeyDownEvent = DateTime.Now;
                    BeginInvoke((MethodInvoker)AddScreenShotSnippingToolStyle);
                }
                else if (gifKeys.IsPressed(Shift, Ctrl, Alt, key))
                {
                    lastKeyDownEvent = DateTime.Now;
                    if (!recordingGif)
                        BeginInvoke((MethodInvoker)StartRecordingGif);
                    else
                        BeginInvoke((MethodInvoker)StopRecordingGif);
                }
            }
        }
        private void KeyHook_KeyUp(Keys key, bool Shift, bool Ctrl, bool Alt)
        {
            Debug.WriteLine($"Released {key}, {Shift}, {Ctrl}, {Alt}");

            //if (gifKeys.IsPressed(Shift, Ctrl, Alt, key))
            //{

            //}
        }

        // ToolStrip
        private void NeuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            images.Add(new Screenshot(new Bitmap(1000, 1000), "newBitmap"));
            imagesIndex = images.Count - 1;
            UpdateUI();
        }
        private void ShowFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentScreenshot.Saved)
                Process.Start("explorer.exe", "/select, \"" + CurrentScreenshot.Path + "\"");
            else
                Process.Start(config.Default.path);
        }
        private void FileToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = new System.Collections.Specialized.StringCollection
            {
                images[imagesIndex].Path
            };
            Clipboard.SetFileDropList(path);
        }
        private void ImageToClipboardToolStripMenuItem_Click(object sender, EventArgs e) => CopyCurrentImageToClipboard();
        private void BeendenToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();
        private void KeybindingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            keybindingsForm.RefreshDefaults();
            keybindingsForm.ShowDialog();
        }
        private void ChangeFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog
            {
                SelectedPath = config.Default.path
            };
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                config.Default.path = FBD.SelectedPath;
                Program.Restart();
                Application.Exit();
            }
        }
        private void NoneMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(noneMenuItem);
        private void CropMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(cropMenuItem);
        private void DrawMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(drawMenuItem);
        private void ColorPickerMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(colorPickerMenuItem);
        private void TextRecognitionToolStripMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(textRecognitionMenuItem);
        private void ChooseColorMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(chooseColorMenuItem);
        private void topMostToolStripMenuItem_CheckedChanged(object sender, EventArgs e) => this.TopMost = topMostToolStripMenuItem.Checked;
    }
}
