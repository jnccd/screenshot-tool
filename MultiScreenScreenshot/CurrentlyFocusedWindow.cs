using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MultiScreenScreenshot
{
    public static class CurrentlyFocusedWindow
    {
        delegate void WinEventDelegate(IntPtr hWinEventHook,
            uint eventType, IntPtr hwnd, int idObject,
            int idChild, uint dwEventThread, uint dwmsEventTime);
        const uint WINEVENT_OUTOFCONTEXT = 0;
        const uint EVENT_SYSTEM_FOREGROUND = 3;
        static IntPtr m_hhook;
        static IntPtr hwnd;
        static WinEventDelegate _WinEvent = new WinEventDelegate(WinEventProc);
        static Task T;

        public static string WindowText = "", ProcessName = "";
        public static int ProcessID = 0;

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd,
            StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin,
            uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess,
            uint idThread, uint dwFlags);

        static void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            if (eventType == EVENT_SYSTEM_FOREGROUND && T == null || 
                eventType == EVENT_SYSTEM_FOREGROUND && T.IsCompleted)
            {
                CurrentlyFocusedWindow.hwnd = hwnd;
                T = Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(1000);

                    StringBuilder sb = new StringBuilder(500);
                    GetWindowText(CurrentlyFocusedWindow.hwnd, sb, sb.Capacity);
                    string text = sb.ToString();
                    uint id = 0;
                    GetWindowThreadProcessId(CurrentlyFocusedWindow.hwnd, out id);
                    Process P = Process.GetProcessById((int)id);

                    if (P.ProcessName != "MultiScreenScreenshot.vshost" && P.ProcessName != "MultiScreenScreenshot")
                    {
                        WindowText = text;
                        ProcessID = (int)id;
                        ProcessName = P.ProcessName;
                    }
                });
            }
        }


        public static void SetEventHook()
        {
            m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND,
                    EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
                    _WinEvent, 0, 0, WINEVENT_OUTOFCONTEXT);
        }
    }
}
