using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace ScreenshotTool
{
    public partial class SnippingToolWindow : Form
    {
        public Rectangle gifArea = new Rectangle(0, 0, 0, 0);
        public Point pMouseDown = new Point(0, 0);
        Point pMouseCurrently = new Point(0, 0);
        bool IsLeftMouseDown = false;
        bool IsMiddleMouseDown = false;

        public Bitmap output;
        Bitmap fullScreenshot;

        public SnippingToolWindow()
        {
            InitializeComponent();
        }
        private void SnippingToolWindow_Load(object sender, EventArgs e)
        {
            //TopMost = true;
            Rectangle AllScreensDimensions = ScreenshotHelper.AllScreenBounds;
            fullScreenshot = new Bitmap(AllScreensDimensions.Width, AllScreensDimensions.Height, PixelFormat.Format32bppRgb);
            using (Graphics graphics = Graphics.FromImage(fullScreenshot))
                graphics.CopyFromScreen(AllScreensDimensions.X, AllScreensDimensions.Y, 0, 0, new Size(AllScreensDimensions.Width, AllScreensDimensions.Height), CopyPixelOperation.SourceCopy);
            Location = new Point(AllScreensDimensions.X, AllScreensDimensions.Y);
            Size = new Size(AllScreensDimensions.Width, AllScreensDimensions.Height);
            Bitmap TransparentScreenshot = (Bitmap)fullScreenshot.Clone();
            TransparentScreenshot.MakeTransparent(Color.Beige);
            //pBox.Scale(new SizeF((float)ScreenshotHelper.MaxScreenScalingFactor, (float)ScreenshotHelper.MaxScreenScalingFactor));
            pBox.Image = TransparentScreenshot.Stretch(new Size((int)(TransparentScreenshot.Width / (float)ScreenshotHelper.MaxScreenScalingFactor), 
                (int)(TransparentScreenshot.Height / (float)ScreenshotHelper.MaxScreenScalingFactor)));
            //pBox.Image = TransparentScreenshot;

            DLLImports.SetForegroundWindow(this.Handle);
        }

        Rectangle GetRectangleFromPoints(Point P1, Point P2)
        {
            int X = Math.Min(P1.X, P2.X);
            int Width = Math.Max(P1.X, P2.X) - X;
            int Y = Math.Min(P1.Y, P2.Y);
            int Height = Math.Max(P1.Y, P2.Y) - Y;
            return new Rectangle(X, Y, Width, Height);
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Rectangle crop = GetRectangleFromPoints(pMouseDown.Scale((double)ScreenshotHelper.MaxScreenScalingFactor), 
                    pMouseCurrently.Scale((double)ScreenshotHelper.MaxScreenScalingFactor));
                if (crop.Width == 0 || crop.Height == 0)
                {
                    MessageBox.Show("Thats a little too small, dont you think?", "Too Smol", MessageBoxButtons.OK);
                    IsLeftMouseDown = false;
                    return;
                }
                output = ScreenshotHelper.CropImage(fullScreenshot, crop);

                IsLeftMouseDown = false;
                this.Close();
            }
            else if (e.Button == MouseButtons.Middle)
            {
                gifArea = GetRectangleFromPoints(pMouseDown.Scale((double)ScreenshotHelper.MaxScreenScalingFactor),
                    pMouseCurrently.Scale((double)ScreenshotHelper.MaxScreenScalingFactor));
                if (gifArea.Width == 0 || gifArea.Height == 0)
                {
                    MessageBox.Show("Thats a little too small, dont you think?", "Too Smol", MessageBoxButtons.OK);
                    IsMiddleMouseDown = false;
                    return;
                }

                IsMiddleMouseDown = false;
                Debug.WriteLine("Set gif area to: " + gifArea);
                this.Close();
            }
            else if (e.Button == MouseButtons.Right)
                this.Close();
        }
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pMouseDown = e.Location;
                IsLeftMouseDown = true;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                pMouseDown = e.Location;
                IsMiddleMouseDown = true;
                IsLeftMouseDown = false;
            }
        }
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || 
                e.Button == MouseButtons.Middle)
            {
                pMouseCurrently = e.Location;
                pBox.Refresh();
            }
        }

        private void PBox_Paint(object sender, PaintEventArgs e)
        {
            if (IsLeftMouseDown)
            {
                Rectangle ee = GetRectangleFromPoints(pMouseDown, pMouseCurrently);
                using Pen pen = new Pen(Color.Red, 1);
                e.Graphics.DrawRectangle(pen, ee);
            }
            else if (IsMiddleMouseDown)
            {
                Rectangle ee = GetRectangleFromPoints(pMouseDown, pMouseCurrently);
                Debug.WriteLine("Drawing " + ee);
                using Pen pen = new Pen(Color.LightBlue, 1);
                e.Graphics.DrawRectangle(pen, ee);
            }
        }

        private void SnippingToolWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        public void CleanUp()
        {
            fullScreenshot.Dispose();
            pBox.Image.Dispose();
            output = null;
        }
    }
}
