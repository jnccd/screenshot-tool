using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool
{
    public static class ScreenshotHelper
    {
        public static decimal[] AllScreenScalingFactors = DLLImports.GetScreenScalingfactors();
        public static decimal MaxScreenScalingFactor = AllScreenScalingFactors.Max();
        public static Rectangle AllScreenBounds = GetGlobalScreenBounds();

        private static Rectangle GetGlobalScreenBounds()
        {
            Rectangle AllScreensDimensions = new Rectangle(0, 0, 1, 1);
            foreach ((Screen S, int i) in Screen.AllScreens.WithIndex())
            {
                if (S.Bounds.X < AllScreensDimensions.X)
                    AllScreensDimensions.X = S.Bounds.X;
                if (S.Bounds.Y < AllScreensDimensions.Y)
                    AllScreensDimensions.Y = S.Bounds.Y;
                var bottom = S.Bounds.X + (int)(S.Bounds.Width * AllScreenScalingFactors[i]) + 1;
                if (bottom > AllScreensDimensions.Width)
                    AllScreensDimensions.Width = bottom;
                var right = S.Bounds.Y + (int)(S.Bounds.Height * AllScreenScalingFactors[i]) + 1;
                if (right > AllScreensDimensions.Height)
                    AllScreensDimensions.Height = right;
            }
            AllScreensDimensions.Width -= AllScreensDimensions.X;
            AllScreensDimensions.Height -= AllScreensDimensions.Y;

            return AllScreensDimensions;
        }
        public static Bitmap GetFullScreenshot()
        {
            Bitmap bmp = new Bitmap(AllScreenBounds.Width, AllScreenBounds.Height, PixelFormat.Format32bppRgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(AllScreenBounds.X, AllScreenBounds.Y, 0, 0, new Size(AllScreenBounds.Width, AllScreenBounds.Height), CopyPixelOperation.SourceCopy);
            return bmp;
        }
        public static Bitmap GetRectScreenshot(Rectangle rect)
        {
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppRgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(rect.X, rect.Y, 0, 0, new Size(rect.Width, rect.Height), CopyPixelOperation.SourceCopy);
            return bmp;
        }
        public static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            using (Graphics g = Graphics.FromImage(bmp))
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }
    }
}
