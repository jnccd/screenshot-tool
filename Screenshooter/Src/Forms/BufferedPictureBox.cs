using System.Windows.Forms;

namespace ScreenshotTool
{
    /// <summary>
    /// PictureBox that paints double buffered. Redrawing the zoomed screenshot is the
    /// most expensive thing the UI does, and without a buffer every repaint first fills
    /// the background and then draws the image on top of it - twice the pixels, and
    /// flicker while the window is being resized.
    /// </summary>
    public class BufferedPictureBox : PictureBox
    {
        public BufferedPictureBox()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
    }
}
