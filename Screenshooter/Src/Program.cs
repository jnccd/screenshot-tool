using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool
{
    public static class Program
    {
        static readonly string RestartLocation = "Restart.bat";
        static System.IO.Stream appIconStream;

        /// <summary>
        /// The application icon (embedded from icon.ico). Every window uses it, which
        /// keeps the icon out of the form resources - those use a binary resource format
        /// that current MSBuild versions refuse to build.
        /// </summary>
        internal static readonly Icon AppIcon = LoadAppIcon();

        static Icon LoadAppIcon()
        {
            try
            {
                // The manifest name depends on how the resource is embedded, so look it
                // up instead of assuming one (Icon keeps the stream alive, do not dispose)
                foreach (string resource in typeof(Program).Assembly.GetManifestResourceNames())
                {
                    if (!resource.EndsWith("icon.ico", StringComparison.OrdinalIgnoreCase))
                        continue;
                    appIconStream = typeof(Program).Assembly.GetManifestResourceStream(resource);
                    if (appIconStream != null)
                        return new Icon(appIconStream);
                }

                // last resort: the icon the executable itself carries
                return Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not load the application icon: " + e.Message);
                return null;
            }
        }
        public static MainForm mainForm;

        [STAThread]
        static void Main()
        {
            if (File.Exists(RestartLocation))
                File.Delete(RestartLocation);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new MainForm();
            Application.Run(mainForm);
        }

        public static void Restart()
        {
            if (File.Exists(RestartLocation))
                File.Delete(RestartLocation);

            using (StreamWriter sw = File.CreateText(RestartLocation))
                sw.WriteLine(@"@echo off
echo     ____               __                __   _                     
echo    / __ \ ___   _____ / /_ ____ _ _____ / /_ (_)____   ____ _       
echo   / /_/ // _ \ / ___// __// __ `// ___// __// // __ \ / __ `/       
echo  / _, _//  __/(__  )/ /_ / /_/ // /   / /_ / // / / // /_/ /_  _  _ 
echo /_/ ^|_^| ^\___//____/ \__/ \__,_//_/    \__//_//_/ /_/ \__, /(_)(_)(_)
echo                                                     /____/          
ping 127.0.0.1 > nul
start ScreenshotTool.exe");

            Process.Start(RestartLocation);
        }
    }
}
