using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiScreenScreenshot
{
    public partial class Form1 : Form
    {
        Color Contrl = Color.FromKnownColor(KnownColor.Control);
        List<Bitmap> RecordedImages = new List<Bitmap>();
        List<Button> MiddleButtons = new List<Button>();
        List<int> SavedImageIndex = new List<int>();
        int RecordedImagesIndex = 0;
        int ticks = 0;
        Size LastSize;
        Point pMouseDown = new Point(0,0);
        Point pMouseCurrently = new Point(0, 0);
        bool IsMouseDown = false;

        public Form1()
        {
            InitializeComponent();
            Program.keyHook.KeyDown += KeyHook_KeyDown;
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            pBox.SizeMode = PictureBoxSizeMode.StretchImage;
            RecordedImages.Add(Program.GetFullScreenshot());

            if (config.Default.path == "<Unset>" || !Directory.Exists(config.Default.path))
                config.Default.path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (config.Default.windowSize.Width != 0)
                Size = config.Default.windowSize;

            MiddleButtons.Add(bSave);
            MiddleButtons.Add(bOpen);
            MiddleButtons.Add(bPath);

            int MiddleButtonWidth = 0;
            foreach (Button B in MiddleButtons)
                MiddleButtonWidth += B.Width + 6;
            MiddleButtonWidth -= 6;
            for (int i = 0; i < MiddleButtons.Count; i++)
                MiddleButtons[i].Location = new Point(Width / 2 - MiddleButtonWidth / 2 + i * (6 + MiddleButtons[i].Width) - 8, MiddleButtons[i].Location.Y);

            MinimumSize = new Size(MiddleButtonWidth + 6 * 6 + 150, MinimumSize.Height);

            Form1_SizeChanged(null, EventArgs.Empty);
            UpdateWindowRatioWidth();
            UpdateUI();

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(300);
                this.InvokeIfRequired(Minimize);
            });
        }
        
        // Helper Methods
        public Size GetProperRatioSize(Size S, bool WidthFirst, float Ratio)
        {
            if (WidthFirst)
            {
                S.Width = (int)(S.Height * Ratio);
                S.Height = (int)(S.Width * (1 / Ratio));
            }
            else
            {
                S.Height = (int)(S.Width * (1 / Ratio));
                S.Width = (int)(S.Height * Ratio);
            }
            return S;
        }
        public void AddScreenShot()
        {
            try
            {
                System.Media.SystemSounds.Exclamation.Play();
                RecordedImages.Add(Program.GetFullScreenshot());
                RecordedImagesIndex = RecordedImages.Count - 1;
                UpdateUI();
            }
            catch (Exception e)
            {
                if (MessageBox.Show("Oopsie woopsie, it seems like I cant make that screenshot!\nDo you want to see the error message in detail?", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MessageBox.Show(e.Message + "\n\n" + e.InnerException + "\n\n" + e.StackTrace);
                }
            }
        }
        public void AddScreenShotSnippingToolStyle()
        {
            try
            {
                System.Media.SystemSounds.Exclamation.Play();
                SnippingToolWindow Snipper = new SnippingToolWindow();
                Snipper.ShowDialog();
                if (Snipper.output != null)
                {
                    RecordedImages.Add(Snipper.output);
                    RecordedImagesIndex = RecordedImages.Count - 1;
                    UpdateUI();

                    bSave_Click(null, EventArgs.Empty);

                    Location = new Point(Snipper.pMouseDown.X + Snipper.ImageDimensions.X - 8 - pBox.Location.X, 
                        Snipper.pMouseDown.Y - 32 - pBox.Location.Y);
                    SetOriginalSize();

                    Program.SetForegroundWindow(this.Handle);
                }
            }
            catch (Exception e)
            {
                if (MessageBox.Show("Oopsie woopsie, it seems like I cant make that screenshot!\nDo you want to see the error message in detail?", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MessageBox.Show(e.Message + "\n\n" + e.InnerException + "\n\n" + e.StackTrace);
                }
            }
        }
        public void ActivateKeyStrokeFeedback()
        {
            this.BackColor = Color.FromArgb(255, 0, 0);
            ticks = 0;
            timer1.Enabled = true;
        }
        public void UpdateUI()
        {
            int count = 0;

            foreach (string s in Directory.GetFiles(config.Default.path))
                if (Path.GetFileNameWithoutExtension(s).Contains("Screenshot"))
                    count++;

            Text = "Multi Screen Screenshot - TargetDir: " + config.Default.path + " - " + count + " saved screenshots!";
            pBox.Image = RecordedImages[RecordedImagesIndex];
            if (SavedImageIndex.Contains(RecordedImagesIndex))
            {
                lSaved.Visible = true;
                bSave.Enabled = false;
            }
            else
            {
                lSaved.Visible = false;
                bSave.Enabled = true;
            }

            if (RecordedImagesIndex == 0)
                bPrevious.Enabled = false;
            else
                bPrevious.Enabled = true;
            if (RecordedImagesIndex == RecordedImages.Count - 1)
                bNext.Enabled = false;
            else
                bNext.Enabled = true;
        }
        public void UpdateWindowRatioWidth()
        {
            Size R = GetProperRatioSize(pBox.Size, true, RecordedImages[RecordedImagesIndex].Width / 
                RecordedImages[RecordedImagesIndex].Height);
            Width += R.Width - pBox.Width;
            Height += R.Height - pBox.Height;
        }
        public void UpdateWindowRatioHeight()
        {
            Size R = GetProperRatioSize(pBox.Size, false, RecordedImages[RecordedImagesIndex].Width /
                RecordedImages[RecordedImagesIndex].Height);
            Height += R.Height - pBox.Height;
            Width += R.Width - pBox.Width;
        }
        public void SetOriginalSize()
        {
            Width = RecordedImages[RecordedImagesIndex].Width + Width - pBox.Width;
            Height = RecordedImages[RecordedImagesIndex].Height + Height - pBox.Height;
        }
        public void Minimize()
        {
            Program.ShowWindow(this.Handle, 2);
        }

        // Button Events
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

            if (!SavedImageIndex.Contains(RecordedImagesIndex))
                SavedImageIndex.Add(RecordedImagesIndex);
            UpdateUI();
        }
        private void bPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.SelectedPath = config.Default.path;
            FBD.Description = "Set the folder ill dump all the screensohts in.";
            if (FBD.ShowDialog() == DialogResult.OK)
                config.Default.path = FBD.SelectedPath;
            config.Default.Save();
            UpdateUI();
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
        private void bOpen_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "/open , \"" + config.Default.path);
        }
        private void bCropScreenshot_Click(object sender, EventArgs e)
        {
            AddScreenShotSnippingToolStyle();
        }
        private void bScreenshot_Click(object sender, EventArgs e)
        {
            AddScreenShot();
        }

        // pBox Events
        private void pBox_Paint(object sender, PaintEventArgs e)
        {
            if (IsMouseDown)
            {
                Rectangle ee = new Rectangle(pMouseDown.X, pMouseDown.Y, pMouseCurrently.X - pMouseDown.X, pMouseCurrently.Y - pMouseDown.Y);
                using (Pen pen = new Pen(Color.Red, 1))
                    e.Graphics.DrawRectangle(pen, ee);
            }
        }
        private void pBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Fix Width Ratio", ((object s, EventArgs ev) =>
                {
                    try
                    {
                        UpdateWindowRatioWidth();
                    }
                    catch { }
                })));
                m.MenuItems.Add(new MenuItem("Fix Height Ratio", ((object s, EventArgs ev) =>
                {
                    try
                    {
                        UpdateWindowRatioHeight();
                    }
                    catch { }
                })));
                m.MenuItems.Add(new MenuItem("1:1 Size", ((object s, EventArgs ev) =>
                {
                    try
                    {
                        SetOriginalSize();
                    }
                    catch { }
                })));
                m.MenuItems.Add(new MenuItem("Smol", ((object s, EventArgs ev) =>
                {
                    try
                    {
                        Width = MinimumSize.Width;
                        Height = MinimumSize.Height;
                    }
                    catch { }
                })));
                m.MenuItems.Add(new MenuItem("Delete", ((object s, EventArgs ev) =>
                {
                    try
                    {
                        if (RecordedImages.Count > 1)
                        {
                            RecordedImages.RemoveAt(RecordedImagesIndex);
                            if (RecordedImagesIndex > RecordedImages.Count - 1)
                                RecordedImagesIndex = RecordedImages.Count - 1;
                            UpdateUI();
                        }
                    }
                    catch { }
                })));
                m.Show(pBox, e.Location);
            }
        }
        // Cropping
        private void pBox_MouseMove(object sender, MouseEventArgs e)
        {
            pMouseCurrently = e.Location;
            pBox.Refresh();
        }
        private void pBox_MouseDown(object sender, MouseEventArgs e)
        {
            pMouseDown = e.Location;
            IsMouseDown = true;
        }
        private void pBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (pBox.Bounds.Contains(pMouseCurrently) && pMouseCurrently.X > pMouseDown.X && pMouseCurrently.Y > pMouseDown.Y)
            {
                RecordedImages.Add(Program.CropImage(RecordedImages[RecordedImagesIndex],
                    new Rectangle((int)(pMouseDown.X * (double)RecordedImages[RecordedImagesIndex].Width / pBox.Width),
                    (int)(pMouseDown.Y * (double)RecordedImages[RecordedImagesIndex].Height / pBox.Height),
                    (int)((pMouseCurrently.X - pMouseDown.X) * (double)RecordedImages[RecordedImagesIndex].Width / pBox.Width),
                    (int)((pMouseCurrently.Y - pMouseDown.Y) * (double)RecordedImages[RecordedImagesIndex].Height / pBox.Height))));
                RecordedImagesIndex = RecordedImages.Count - 1;
                UpdateUI();
            }
            IsMouseDown = false;
        }

        // Other Events
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SavedImageIndex.Count + 1 < RecordedImages.Count && MessageBox.Show("Oi, you have unsaved Images! Do you really want to close me?", "Close?", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            config.Default.windowSize = Size;
            config.Default.Save();

        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            // Buttons
            bPrevious.Width = bSave.Location.X - bPrevious.Location.X - 6;
            bNext.Location = new Point(bPath.Location.X + bPath.Width + 6, bNext.Location.Y);
            bNext.Width = pBox.Width + pBox.Location.X - bNext.Location.X;

            bScreenshot.Width = Width / 2 - 20;
            bCropScreenshot.Location = new Point(bScreenshot.Location.X + bScreenshot.Width + 6, bCropScreenshot.Location.Y);
            bCropScreenshot.Width = Width / 2 - 26;

            //// Disabled Snapping
            //int slurpSize = 10;
            //Size R = GetProperRatioSize(pBox.Size, Math.Abs(LastSize.Width - Width) > Math.Abs(LastSize.Height - Height),
            //    RecordedImages[0].Width / RecordedImages[0].Height);

            //if (R.Width + slurpSize > pBox.Width && R.Width - slurpSize < pBox.Width)
            //    Width += R.Width - pBox.Width;
            //if (R.Height + slurpSize > pBox.Height && R.Height - slurpSize < pBox.Height)
            //    Height += R.Height - pBox.Height;

            if (WindowState == FormWindowState.Maximized)
                IsMouseDown = false;

            LastSize = Size;
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
        private void KeyHook_KeyDown(Keys key, bool Shift, bool Ctrl, bool Alt)
        {
            if (key == Keys.Pause)
            {
                if (Alt)
                    AddScreenShotSnippingToolStyle();
                else
                    AddScreenShot();
            }
        }
    }
}
