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
