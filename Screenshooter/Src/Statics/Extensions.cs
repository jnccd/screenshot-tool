using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool
{
    public static class Extensions
    {
        // Image 
        public static Image Stretch(this Image image, Size size, bool dispose = true)
        {
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(image, 0, 0, size.Width, size.Height);
            }

            if (dispose)
            {
                image.Dispose();
            }

            return bitmap;
        }
        // Gif
        public static bool IsAnimatedGif(this Bitmap bitmap)
        {
            return GetFrameCount(bitmap) > 1;
        }
        public static int GetFrameCount(this Bitmap bitmap)
        {
            FrameDimension dimensions = new FrameDimension(bitmap.FrameDimensionsList[0]);
            return bitmap.GetFrameCount(dimensions);
        }

        //String
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
        public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);

        // Other
        public static Point Scale(this Point p, double d) =>
            new Point((int)(p.X * d), (int)(p.Y * d));
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) =>
            self.Select((item, index) => (item, index));
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
