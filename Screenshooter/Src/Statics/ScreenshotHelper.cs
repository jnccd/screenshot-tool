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
        public static Rectangle allScreenBounds = GetAllScreenBounds();

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
            Bitmap bmp = new Bitmap(allScreenBounds.Width, allScreenBounds.Height, PixelFormat.Format32bppRgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(allScreenBounds.X, allScreenBounds.Y, 0, 0, new Size(allScreenBounds.Width, allScreenBounds.Height), CopyPixelOperation.SourceCopy);
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
