using System;
using System.Diagnostics;
using System.IO;

namespace ScreenshotTool
{
    /// <summary>
    /// Small file log used to diagnose capture and hotkey problems on machines where no
    /// debugger is attached. The file is capped so it can never grow without bound.
    /// </summary>
    internal static class Diagnostics
    {
        internal static readonly string LogPath = Path.Combine(Path.GetTempPath(), "ScreenshotTool-capture.log");

        internal static void Log(string message)
        {
            try
            {
                var file = new FileInfo(LogPath);
                if (file.Exists && file.Length > 256 * 1024)
                    file.Delete();
                File.AppendAllText(LogPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + message + Environment.NewLine);
            }
            catch
            {
                // diagnostics must never break the tool
            }
        }

        internal static void Log(string format, params object[] arguments)
        {
            Log(string.Format(format, arguments));
        }

        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
        static extern bool OpenProcessToken(IntPtr process, int access, out IntPtr token);

        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetTokenInformation(IntPtr token, int infoClass, IntPtr info, int infoLength, out int returnLength);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        /// Integrity level of this process. A low integrity process cannot see keyboard
        /// input aimed at higher integrity windows, which silently disables a global hook.
        /// </summary>
        internal static string IntegrityLevel()
        {
            IntPtr token = IntPtr.Zero, buffer = IntPtr.Zero;
            try
            {
                if (!OpenProcessToken(GetCurrentProcess(), 0x0008, out token))
                    return "unavailable";
                int length;
                GetTokenInformation(token, 25, IntPtr.Zero, 0, out length);
                if (length <= 0)
                    return "unavailable";
                buffer = System.Runtime.InteropServices.Marshal.AllocHGlobal(length);
                if (!GetTokenInformation(token, 25, buffer, length, out length))
                    return "unavailable";
                // TOKEN_MANDATORY_LABEL -> SID -> first sub authority is the integrity RID
                IntPtr sid = System.Runtime.InteropServices.Marshal.ReadIntPtr(buffer);
                uint rid = (uint)System.Runtime.InteropServices.Marshal.ReadInt32(sid, 8);
                switch (rid)
                {
                    case 0x0000: return "Untrusted";
                    case 0x1000: return "Low";
                    case 0x2000: return "Medium";
                    case 0x3000: return "High";
                    case 0x4000: return "System";
                    default: return "0x" + rid.ToString("X");
                }
            }
            catch { return "unavailable"; }
            finally
            {
                if (buffer != IntPtr.Zero) System.Runtime.InteropServices.Marshal.FreeHGlobal(buffer);
                if (token != IntPtr.Zero) CloseHandle(token);
            }
        }
    }
}