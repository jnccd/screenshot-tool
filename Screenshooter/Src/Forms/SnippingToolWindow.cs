using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public Rectangle ImageDimensions;

        public SnippingToolWindow()
        {
            InitializeComponent();
        }
        private void SnippingToolWindow_Load(object sender, EventArgs e)
        {
            TopMost = true;
            ImageDimensions = new Rectangle(0, 0, 1, 1);
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

            fullScreenshot = new Bitmap(ImageDimensions.Width, ImageDimensions.Height, PixelFormat.Format32bppRgb);
            using (Graphics graphics = Graphics.FromImage(fullScreenshot))
                graphics.CopyFromScreen(ImageDimensions.X, ImageDimensions.Y, 0, 0, new Size(ImageDimensions.Width, ImageDimensions.Height), CopyPixelOperation.SourceCopy);
            Location = new Point(ImageDimensions.X, ImageDimensions.Y);
            Size = new Size(ImageDimensions.Width, ImageDimensions.Height);
            Bitmap TransparentScreenshot = (Bitmap)fullScreenshot.Clone();
            TransparentScreenshot.MakeTransparent(Color.Beige);
            pBox.Image = TransparentScreenshot;

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
                Rectangle crop = GetRectangleFromPoints(
                        new Point((int)(pMouseDown.X * (double)fullScreenshot.Width / pBox.Width),
                            (int)(pMouseDown.Y * (double)fullScreenshot.Height / pBox.Height)),
                        new Point((int)(pMouseCurrently.X * (double)fullScreenshot.Width / pBox.Width),
                            (int)(pMouseCurrently.Y * (double)fullScreenshot.Height / pBox.Height)));
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
                gifArea = GetRectangleFromPoints(
                        new Point((int)(pMouseDown.X * (double)fullScreenshot.Width / pBox.Width),
                            (int)(pMouseDown.Y * (double)fullScreenshot.Height / pBox.Height)),
                        new Point((int)(pMouseCurrently.X * (double)fullScreenshot.Width / pBox.Width),
                            (int)(pMouseCurrently.Y * (double)fullScreenshot.Height / pBox.Height)));
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
