using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiScreenScreenshot
{
    public static class Program
    {
        public static Form1 MyForm;
        public static KeyboardHook keyHook = new KeyboardHook(true);
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MyForm = new Form1();
            Application.Run(MyForm);
        }

        public static Bitmap GetFullScreenshot()
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

            Bitmap bmp = new Bitmap(ImageDimensions.Width, ImageDimensions.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(ImageDimensions.X, ImageDimensions.Y, 0, 0, new Size(ImageDimensions.Width, ImageDimensions.Height), CopyPixelOperation.SourceCopy);
            return bmp;
        }
        public static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

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
    }
}
