using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiScreenScreenshot
{
    public partial class Form1 : Form
    {
        Color Contrl = Color.FromKnownColor(KnownColor.Control);
        List<Bitmap> RecordedImages = new List<Bitmap>();
        int RecordedImagesIndex = 0;
        int ticks = 0;
        float Ratio;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InterceptKeys._hookID = InterceptKeys.SetHook(InterceptKeys._proc);

            pBox.SizeMode = PictureBoxSizeMode.StretchImage;
            RecordedImages.Add(GetFullScreenshot());

            if (config.Default.path == "<Unset>" || !Directory.Exists(config.Default.path))
                config.Default.path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (config.Default.windowSize.Width != 0)
                Size = config.Default.windowSize;
            else
            {

            }

            UpdateUI();
        }
        
        public Size GetProperRatioSize(Size S, float Ratio)
        {
            S.Width = (int)(S.Height * Ratio);
            S.Height = (int)(S.Width * (1/ Ratio));
            return S;
        }
        public void AddScreenShot()
        {
            RecordedImages.Add(GetFullScreenshot());
            UpdateUI();
        }
        public void ActivateKeyStrokeFeedback()
        {
            this.BackColor = Color.FromArgb(255, 0, 0);
            ticks = 0;
            timer1.Enabled = true;
        }
        public Bitmap GetFullScreenshot()
        {
            Rectangle ImageDimensions = new Rectangle(0,0,1,1);
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
        public void UpdateUI()
        {
            pBox.Image = RecordedImages[RecordedImagesIndex];

            if (RecordedImagesIndex == 0)
                bPrevious.Enabled = false;
            else
                bPrevious.Enabled = true;
            if (RecordedImagesIndex == RecordedImages.Count - 1)
                bNext.Enabled = false;
            else
                bNext.Enabled = true;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            string fileName = "Screenshot";
            int index = 0;

            foreach (string s in Directory.GetFiles(config.Default.path))
            {
                if (Path.GetFileNameWithoutExtension(s) == fileName)
                {
                    index++;
                    fileName = "Screenshot" + index;
                }
            }

            RecordedImages[RecordedImagesIndex].Save(config.Default.path + "\\" + fileName + ".png");
        }
        private void bPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.SelectedPath = config.Default.path;
            FBD.Description = "Set the folder ill dump all the screensohts in.";
            if (FBD.ShowDialog() == DialogResult.OK)
                config.Default.path = FBD.SelectedPath;
            config.Default.Save();
        }
        private void bPrevious_Click(object sender, EventArgs e)
        {
            RecordedImagesIndex--;
            UpdateUI();
        }
        private void bNext_Click(object sender, EventArgs e)
        {
            RecordedImagesIndex++;
            UpdateUI();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            config.Default.windowSize = Size;
            config.Default.Save();
            InterceptKeys.UnhookWindowsHookEx(InterceptKeys._hookID);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int animLength = 15;
            float percentage = ticks / (float)animLength;
            ticks++;

            if (ticks == animLength)
            {
                timer1.Enabled = false;
                this.BackColor = Contrl;
            }
            else
            {
                this.BackColor = Color.FromArgb((int)(255 * (1 - percentage) + Contrl.R * percentage), 
                    (int)(Contrl.G * percentage), (int)(Contrl.B * percentage));
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            int slurpSize = 15;
            Size R = GetProperRatioSize(pBox.Size, Ratio);

            if (R.Width + slurpSize > Width && R.Width - slurpSize < Width)
                Width = R.Width;
            if (R.Height + slurpSize > Height && R.Height - slurpSize < Height)
                Height = R.Height;
        }
    }
}
