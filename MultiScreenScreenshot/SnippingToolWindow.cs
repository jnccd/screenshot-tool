using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiScreenScreenshot
{
    public partial class SnippingToolWindow : Form
    {
        public Point pMouseDown = new Point(0, 0);
        Point pMouseCurrently = new Point(0, 0);
        bool IsMouseDown = false;

        public Bitmap output;
        Bitmap fullScreenshot;

        public Rectangle ImageDimensions;

        public SnippingToolWindow()
        {
            InitializeComponent();
        }
        private void SnippingToolWindow_Load(object sender, EventArgs e)
        {
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

            fullScreenshot = new Bitmap(ImageDimensions.Width, ImageDimensions.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(fullScreenshot);
            graphics.CopyFromScreen(ImageDimensions.X, ImageDimensions.Y, 0, 0, new Size(ImageDimensions.Width, ImageDimensions.Height), CopyPixelOperation.SourceCopy);
            Location = new Point(ImageDimensions.X, ImageDimensions.Y);
            Size = new Size(ImageDimensions.Width, ImageDimensions.Height);
            Bitmap TransparentScreenshot = (Bitmap)fullScreenshot.Clone();
            TransparentScreenshot.MakeTransparent(Color.Beige);
            pBox.Image = TransparentScreenshot;

            Program.SetForegroundWindow(this.Handle);
        }

        private void GetRectangleFromPoints(Point P1, Point P2)
        {

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (pBox.Bounds.Contains(pMouseCurrently) && pMouseCurrently.X > pMouseDown.X && pMouseCurrently.Y > pMouseDown.Y)
                {
                    output = Program.CropImage(fullScreenshot,
                        new Rectangle((int)(pMouseDown.X * (double)fullScreenshot.Width / pBox.Width),
                        (int)(pMouseDown.Y * (double)fullScreenshot.Height / pBox.Height),
                        (int)((pMouseCurrently.X - pMouseDown.X) * (double)fullScreenshot.Width / pBox.Width),
                        (int)((pMouseCurrently.Y - pMouseDown.Y) * (double)fullScreenshot.Height / pBox.Height)));
                    this.Close();
                }
                IsMouseDown = false;
            }
            else if (e.Button == MouseButtons.Right)
                this.Close();
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pMouseDown = e.Location;
                IsMouseDown = true;
            }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pMouseCurrently = e.Location;
                pBox.Refresh();
            }
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void pBox_Paint(object sender, PaintEventArgs e)
        {
            if (IsMouseDown)
            {
                Rectangle ee = new Rectangle(pMouseDown.X, pMouseDown.Y, pMouseCurrently.X - pMouseDown.X, pMouseCurrently.Y - pMouseDown.Y);
                using (Pen pen = new Pen(Color.Red, 1))
                    e.Graphics.DrawRectangle(pen, ee);
            }
        }

        private void SnippingToolWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
