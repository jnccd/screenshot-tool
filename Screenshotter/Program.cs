using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool
{
    public static class Program
    {
        public static MainForm MyForm;
        public static KeyboardHook keyHook = new KeyboardHook(true);
        public static Rectangle AllScreenBounds = GetAllScreenBounds();

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MyForm = new MainForm();
            Application.Run(MyForm);
        }

        static Rectangle GetAllScreenBounds()
        {
            Rectangle ImageDimensions = new Rectangle(0, 0, 1, 1);
            foreach (Screen S in Screen.AllScreens)
            {
                if (S.Bounds.X < ImageDimensions.X)
                    ImageDimensions.X = S.Bounds.X;
                if (S.Bounds.Y < ImageDimensions.Y)
                    ImageDimensions.Y = S.Bounds.Y;
                if (S.Bounds.X + S.Bounds.Width > ImageDimensions.Width)
                    ImageDimensions.Width = S.Bounds.X + S.Bounds.Width;
                if (S.Bounds.Y + S.Bounds.Height > ImageDimensions.Height)
                    ImageDimensions.Height = S.Bounds.Y + S.Bounds.Height;
            }
            ImageDimensions.Width -= ImageDimensions.X;
            ImageDimensions.Height -= ImageDimensions.Y;

            return ImageDimensions;
        }
        public static Bitmap GetFullScreenshot()
        {
            Bitmap bmp = new Bitmap(AllScreenBounds.Width, AllScreenBounds.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(AllScreenBounds.X, AllScreenBounds.Y, 0, 0, new Size(AllScreenBounds.Width, AllScreenBounds.Height), CopyPixelOperation.SourceCopy);
            return bmp;
        }
        public static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            using (Graphics g = Graphics.FromImage(bmp))

                // Draw the given area (section) of the source image
                // at location 0,0 on the empty bitmap (bmp)
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }

        // Imports 
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Extensions
        public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
        {
            if (obj.InvokeRequired)
            {
                var args = new object[0];
                obj.Invoke(action, args);
            }
            else
            {
                action();
            }
        }
        public static string ToShitEnglishNumberThingy(this int i)
        {
            if (i == 1)
                return "1st";
            if (i == 2)
                return "2nd";
            if (i == 3)
                return "3rd";
            else
                return i + "th";    
        }
    }
}
