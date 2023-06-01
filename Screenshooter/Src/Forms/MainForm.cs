using AnimatedGif;
using ScreenshotTool.Properties;
using ScreenshotTool.Src.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool
{
    public partial class MainForm : Form
    {
        public static KeyboardHook keyHook = new KeyboardHook(true);

        // Images
        int imagesIndex = 0;
        readonly List<Screenshot> images = new List<Screenshot>();

        // UI
        readonly List<Button> middleButtons = new List<Button>();
        Point pMouseDown = new Point(0,0);
        Point pMouseCurrently = new Point(0, 0);
        Point pMouseLast = new Point(0, 0);
        bool isMouseDown = false;
        DateTime lastKeyDownEvent = DateTime.Now;
        readonly KeybindingsForm keybindingsForm = new KeybindingsForm();
        ColorView colorView = new ColorView();
        TextRecognitionView textView = new TextRecognitionView();

        // Gif
        bool recordingGif = false;
        bool processingGif = false;
        readonly List<Bitmap> gifShots = new List<Bitmap>();

        // Snipper active
        readonly SnippingToolWindow snipper = new SnippingToolWindow();
        bool snippingWindowActive = false;
        bool shownGifSnipperHelp = false;

        // Edit
        ToolStripMenuItem mode;
        readonly int drawRadius = 10;

        public Shortcut instantKeys;
        public Shortcut cropKeys;
        public Shortcut gifKeys;

        float HUDvisibility = 0;
        float HUDVisiblity
        {
            get
            {

                if (HUDvisibility < 1)
                    return HUDvisibility;
                else
                    return 1;
            }
        }
        
        const int halfExtraPreviewImages = 4;
        const int previewImageWidth = 100;
        const int previewImageHeight = 56;
        const int previewImageOutlineThickness = 7;
        const int previewImagePadding = 12;
        const int savedSignFontSize = 15;

        public MainForm()
        {
            InitializeComponent();
            keyHook.KeyUp += KeyHook_KeyUp;
            keyHook.KeyDown += KeyHook_KeyDown;
            CurrentlyFocusedWindow.SetEventHook();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (config.Default.path == "<Unset>" || !Directory.Exists(config.Default.path))
                config.Default.path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (config.Default.windowSize.Width != 0)
                Size = config.Default.windowSize;
            if (config.Default.PrimaryColor.Name == "0")
                config.Default.PrimaryColor = Color.Red;

            if (config.Default.instantShortcut.IsNullOrWhiteSpace()) instantKeys = Shortcut.DefaultInstantKeys;
            else instantKeys = new Shortcut().FromString(config.Default.instantShortcut);

            if (config.Default.cropShortcut.IsNullOrWhiteSpace()) cropKeys = Shortcut.DefaultCropKeys;
            else cropKeys = new Shortcut().FromString(config.Default.cropShortcut);

            if (config.Default.gifShortcut.IsNullOrWhiteSpace()) gifKeys = Shortcut.DefaultGifKeys;
            else gifKeys = new Shortcut().FromString(config.Default.gifShortcut);

            middleButtons.Add(bSave);
            middleButtons.Add(bDelete);

            int MiddleButtonWidth = 0;
            foreach (Button B in middleButtons)
                MiddleButtonWidth += B.Width + 6;
            MiddleButtonWidth -= 6;
            for (int i = 0; i < middleButtons.Count; i++)
                middleButtons[i].Location = new Point(Width / 2 - MiddleButtonWidth / 2 + i * (6 + middleButtons[i].Width) - 8, middleButtons[i].Location.Y);

            int MinWidth = MiddleButtonWidth + 6 * (middleButtons.Count + 1) + 90;
            MinimumSize = new Size(MinWidth, MinWidth);

            MainForm_SizeChanged(null, EventArgs.Empty);
            Minimize();
            
            string[] files = Directory.GetFiles(config.Default.path).
                Where(s => (s.EndsWith(".png") || s.EndsWith(".gif")) && 
                           Path.GetFileNameWithoutExtension(s).Contains("Screenshot_")).
                OrderBy(x => x).
                Reverse().
                ToArray();
            if (files.Length > 0)
                images.AddRange(files.Select(x => new Screenshot(x)).ToArray());
            else
                images.Add(new Screenshot(ScreenshotHelper.GetFullScreenshot(), GetScreenshotName()));
            imagesIndex = images.Count - 1;

            noneMenuItem.Checked = true;

            UpdateWindowRatioWidth();
            UpdateUI();
        }

        // Image
        public void AddScreenShot()
        {
            try
            {
                System.Media.SystemSounds.Exclamation.Play();
                images.Add(new Screenshot(ScreenshotHelper.GetFullScreenshot(), GetScreenshotName()));
                imagesIndex = images.Count - 1;
                UpdateUI();
            }
            catch (Exception e)
            {
                if (MessageBox.Show("Oopsie woopsie, it seems like I cant make that screenshot!\nDo you want to see the error message in detail?",
                    "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    MessageBox.Show(e.Message + "\n\n" + e.InnerException + "\n\n" + e.StackTrace);
            }
        }
        public void AddScreenShotSnippingToolStyle()
        {
            if (snippingWindowActive)
                return;

            lock (snipper)
            {
                try
                {
                    snippingWindowActive = true;
                    System.Media.SystemSounds.Exclamation.Play();
                    snipper.ShowDialog();
                    if (snipper.output != null)
                        Task.Run(() => this.InvokeIfRequired(() => {
                            try
                            {
                                images.Add(new Screenshot(snipper.output, GetScreenshotName()));
                                imagesIndex = images.Count - 1;
                                UpdateUI();

                                BSave_Click(null, EventArgs.Empty);

                                Point mouseDownPoint = snipper.pMouseDown;
                                Rectangle imageDimensions = snipper.ImageDimensions;
                                snipper.InvokeIfRequired(() => Location = new Point(mouseDownPoint.X + imageDimensions.X - 8 - pBox.Location.X,
                                    mouseDownPoint.Y - 32 - pBox.Location.Y));

                                WindowState = FormWindowState.Normal;
                                DLLImports.SetForegroundWindow(Handle);

                                SetOriginalSize();

                                // Ausgleichen des blankParts
                                int imgWidth = pBox.Image.Width;
                                int imgHeight = pBox.Image.Height;
                                int boxWidth = pBox.Size.Width;
                                int boxHeight = pBox.Size.Height;
                                float X = 0;
                                float Y = 0;
                                if (imgWidth / imgHeight > boxWidth / boxHeight)
                                {
                                    float scale = boxWidth / (float)imgWidth;
                                    float blankPart = (boxHeight - scale * imgHeight) / 2;
                                    Y = blankPart;
                                }
                                else
                                {
                                    float scale = boxHeight / (float)imgHeight;
                                    float blankPart = (boxWidth - scale * imgWidth) / 2;
                                    X = blankPart;
                                }
                                Location = new Point(Location.X - (int)X, Location.Y - (int)Y);
                                snipper.CleanUp();
                            }
                            catch (Exception e)
                            {
                                if (MessageBox.Show("Oopsie woopsie, it seems like I cant make that screenshot!\nDo you want to see the error message in detail?", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    MessageBox.Show(e.Message + "\n\n" + e.InnerException + "\n\n" + e.StackTrace);
                                }
                            }
                            snippingWindowActive = false;
                        }));
                    else
                        snippingWindowActive = false;
                }
                catch (Exception e)
                {
                    if (MessageBox.Show("Oopsie woopsie, it seems like I cant make that screenshot!\nDo you want to see the error message in detail?", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        MessageBox.Show(e.Message + "\n\n" + e.InnerException + "\n\n" + e.StackTrace);
                    }
                }
            }
        }
        public string GetScreenshotName()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1999, 5, 4));
            string fileName = "Screenshot_" + (long.MaxValue - (long)t.TotalMilliseconds);
            try { fileName += "_" + CurrentlyFocusedWindow.ProcessName; }
            catch { }
            return fileName;
        }
        public void SaveCurrentImage()
        {
            images[imagesIndex].Save();
            images[imagesIndex].PutInClipboard();
            UpdateUI();
        }
        public void CopyCurrentImageToClipboard()
        {
            if (CurrentScreenshot.Image.IsAnimatedGif())
            {
                string[] files = new string[1]; files[0] = CurrentScreenshot.Path;
                this.DoDragDrop(new DataObject(DataFormats.FileDrop, files), DragDropEffects.Copy);
            }
            CurrentScreenshot.PutInClipboard();
        }
        public void DeleteCurrentImage()
        {
            if (images.Count > 1)
            {
                CurrentScreenshot.DisposeImageCache();
                CurrentScreenshot.Delete();
                images.RemoveAt(imagesIndex);
                if (imagesIndex > images.Count - 1)
                    imagesIndex = images.Count - 1;
                UpdateUI();
            }
        }
        public Screenshot CurrentScreenshot { get { return images[imagesIndex]; } }

        // GIF
        public void StartRecordingGif()
        {
            if (processingGif | recordingGif)
                return;

            if (snipper.gifArea.Width == 0 || snipper.gifArea.Height == 0)
            {
                if (!shownGifSnipperHelp)
                {
                    shownGifSnipperHelp = true;
                    DialogResult result = MessageBox.Show("You need to select an gif area in the SelectionForm™ before recording a gif!", "Missing Gif Area", MessageBoxButtons.OK);
                    if (result == DialogResult.OK)
                        shownGifSnipperHelp = false;
                }
                return;
            }

            processingGif = true;
            recordingGif = true;
            gifShots.Clear();

            Task.Factory.StartNew(() =>
            {
                List<Task> runners = new List<Task>();
                for (int i = 0; i < 750 && recordingGif; i++)
                {
                    runners.Add(Task.Factory.StartNew(() =>
                    {
                        gifShots.Add(ScreenshotHelper.GetRectScreenshot(snipper.gifArea));
                        Debug.WriteLine($"Made gif shot {i}!");
                    }));

                    Task.Delay(33).Wait();
                }
                Debug.WriteLine("waiting for close...");
                Task.WaitAll(runners.ToArray());
                Debug.WriteLine("closed");

                string path = config.Default.path + "\\" + GetScreenshotName() + ".gif";
                using (FileStream s = new FileStream(path, FileMode.Create))
                {
                    using AnimatedGifCreator c = new AnimatedGifCreator(s, 40);
                    foreach (Bitmap b in gifShots)
                        c.AddFrame(b, -1, GifQuality.Default);
                }

                images.Add(new Screenshot(path));
                imagesIndex = images.Count - 1;
                CurrentScreenshot.Save();

                this.InvokeIfRequired(() => UpdateUI());

                processingGif = false;
            });
        }
        public void StopRecordingGif()
        {
            recordingGif = false;
            Debug.WriteLine("recordingGif is now false");
        }

        // UI
        public void UpdateUI()
        {
            Text = $"Screenshot Tool - {images.Count} saved screenshots!" +
                $"{(CurrentScreenshot.Path.IsNullOrWhiteSpace() ? "" : $" - {Path.GetFileNameWithoutExtension(CurrentScreenshot.Path)} ")} - Dir: {config.Default.path}";
            pBox.Image = images[imagesIndex].Image;
            if (images[imagesIndex].Saved)
                bSave.Text = "To Clipboard";
            else
                bSave.Text = "Save";

            if (mode == textRecognitionMenuItem)
                textView.UpdateReadings();

            bDelete.Enabled = images.Count > 1;
            bPrevious.Enabled = imagesIndex != 0;
            bNext.Enabled = imagesIndex != images.Count - 1;
        }
        public void UpdateWindowRatioWidth()
        {
            Size R = GetProperRatioSize(pBox.Size, true, images[imagesIndex].Image.Width /
                images[imagesIndex].Image.Height);
            Width += R.Width - pBox.Width;
            Height += R.Height - pBox.Height;
        }
        public void UpdateWindowRatioHeight()
        {
            Size R = GetProperRatioSize(pBox.Size, false, images[imagesIndex].Image.Width /
                images[imagesIndex].Image.Height);
            Height += R.Height - pBox.Height;
            Width += R.Width - pBox.Width;
        }
        private void ResetHudVisibility() => HUDvisibility = (7.5f - HUDvisibility) / 3f;
        public void Minimize() => DLLImports.ShowWindow(this.Handle, 2);
        public void SetModeToNone() => ChangeEditMode(noneMenuItem, false);
        // Window Size
        public void SetOriginalSize()
        {
            Width = images[imagesIndex].Image.Width + Width - pBox.Width;
            Height = images[imagesIndex].Image.Height + Height - pBox.Height;

            // doppelt hält besser :thonk:
            Width = images[imagesIndex].Image.Width + Width - pBox.Width;
            Height = images[imagesIndex].Image.Height + Height - pBox.Height;
        }
        public void CenterAroundMouse()
        {
            Location = new Point(MousePosition.X - Width / 2, MousePosition.Y - Height / 2);
        }
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
        Rectangle GetRectangleFromPoints(Point P1, Point P2)
        {
            int X = Math.Min(P1.X, P2.X);
            int Width = Math.Max(P1.X, P2.X) - X;
            int Y = Math.Min(P1.Y, P2.Y);
            int Height = Math.Max(P1.Y, P2.Y) - Y;
            return new Rectangle(X, Y, Width, Height);
        }
        public Point ZoomPicBoxCoordsToImageCoords(Point P, PictureBox pBox)
        {
            int imgWidth = pBox.Image.Width;
            int imgHeight = pBox.Image.Height;
            int boxWidth = pBox.Size.Width;
            int boxHeight = pBox.Size.Height;

            //This variable will hold the result
            float X = P.X;
            float Y = P.Y;
            //Comparing the aspect ratio of both the control and the image itself.
            if (imgWidth / imgHeight > boxWidth / boxHeight)
            {
                //If true, that means that the image is stretched through the width of the control.
                //'In other words: the image is limited by the width.

                //The scale of the image in the Picture Box.
                float scale = boxWidth / (float)imgWidth;

                //Since the image is in the middle, this code is used to determinate the empty space in the height
                //'by getting the difference between the box height and the image actual displayed height and dividing it by 2.
                float blankPart = (boxHeight - scale * imgHeight) / 2;

                Y -= blankPart;

                //Scaling the results.
                X /= scale;
                Y /= scale;
            }
            else
            {
                //If true, that means that the image is stretched through the height of the control.
                //'In other words: the image is limited by the height.

                //The scale of the image in the Picture Box.
                float scale = boxHeight / (float)imgHeight;

                //Since the image is in the middle, this code is used to determinate the empty space in the width
                //'by getting the difference between the box width and the image actual displayed width and dividing it by 2.
                float blankPart = (boxWidth - scale * imgWidth) / 2;
                X -= blankPart;

                //Scaling the results.
                X /= scale;
                Y /= scale;
            }
            return new Point((int)X, (int)Y);
        }
        private void ChangeEditMode(ToolStripMenuItem newMode, bool CloseWindow = true)
        {
            if (newMode == colorPickerMenuItem)
            {
                if (colorView == null || colorView.IsDisposed)
                    colorView = new ColorView();
                colorView.Show();
            }
            else if (CloseWindow)
                colorView.Close();

            if (newMode == textRecognitionMenuItem)
            {
                if (textView == null || textView.IsDisposed)
                    textView = new TextRecognitionView();
                textView.Show();
            }
            else if (CloseWindow)
                textView.Close();

            if (newMode == chooseColorMenuItem)
            {
                var LeDialog = new ColorDialog
                {
                    AllowFullOpen = true,
                    AnyColor = true,
                    Color = config.Default.PrimaryColor
                };
                if (LeDialog.ShowDialog() == DialogResult.OK)
                    config.Default.PrimaryColor = LeDialog.Color;
            }

            foreach (ToolStripItem item in editMenuItem.DropDownItems)
                if (item is ToolStripMenuItem)
                    (item as ToolStripMenuItem).Checked = item == newMode;

            mode = newMode;
        }
        
        // Button Events
        private void BSave_Click(object sender, EventArgs e)
        {
            if (images[imagesIndex].Saved)
                CopyCurrentImageToClipboard();
            else
                SaveCurrentImage();
        }
        private void BPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog
            {
                SelectedPath = config.Default.path,
                Description = "Set the folder ill dump all the screensohts in."
            };
            if (FBD.ShowDialog() == DialogResult.OK)
                config.Default.path = FBD.SelectedPath;
            config.Default.Save();
            UpdateUI();
        }
        private void BPrevious_Click(object sender, EventArgs e)
        {
            if (imagesIndex + 4 < images.Count)
                images[imagesIndex + 4].DisposeImageCache();
            imagesIndex--;

            ResetHudVisibility();
            UpdateUI();
        }
        private void BNext_Click(object sender, EventArgs e)
        {
            if (imagesIndex - 4 >= 0)
                images[imagesIndex - 4].DisposeImageCache();
            imagesIndex++;

            ResetHudVisibility();
            UpdateUI();
        }
        private void BOpen_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "/open , \"" + config.Default.path);
        }
        private void BCropScreenshot_Click(object sender, EventArgs e)
        {
            AddScreenShotSnippingToolStyle();
        }
        private void BScreenshot_Click(object sender, EventArgs e)
        {
            AddScreenShot();
        }
        private void BDelete_Click(object sender, EventArgs e) => DeleteCurrentImage();

        // PictureBox Events
        private void PBox_Paint(object sender, PaintEventArgs e)
        {
            if (isMouseDown && mode == cropMenuItem)
            {
                Rectangle ee = GetRectangleFromPoints(pMouseDown, pMouseCurrently);
                using (Pen pen = new Pen(config.Default.PrimaryColor, 1))
                    e.Graphics.DrawRectangle(pen, ee);
            }
            // Saved title
            if (images[imagesIndex].Saved && pBox.Height > 8)
            {
                //try
                //{
                //    using (Pen pen = new Pen(Color.Red, 1))
                //        e.Graphics.DrawString("Saved!", new Font("BigNoodleTitling", Math.Min(savedSignFontSize, pBox.Height) + 1, FontStyle.Italic),
                //            Brushes.Red, new PointF(0, HUDVisiblity * (savedSignFontSize + 15) - savedSignFontSize - 15));
                //}
                //catch
                //{
                    using (Pen pen = new Pen(Color.Red, 1))
                        e.Graphics.DrawString("Saved!", new Font("Arial", Math.Min(savedSignFontSize, pBox.Height) + 1, FontStyle.Regular),
                            Brushes.Red, new PointF(0, HUDVisiblity * (savedSignFontSize + 15) - savedSignFontSize - 15));
                //}
            }
            // Previews
            for (int i = imagesIndex - halfExtraPreviewImages; i < imagesIndex + halfExtraPreviewImages + 1; i++)
            {
                if (i >= 0 && i < images.Count)
                {
                    int index = i - imagesIndex;
                    Rectangle draw = new Rectangle(pBox.Width / 2 - (previewImageWidth/2) + index * (previewImageWidth + previewImagePadding), 
                        pBox.Height - (int)Math.Min((previewImageHeight + previewImageOutlineThickness * 2) * HUDVisiblity, pBox.Height) + previewImageOutlineThickness, 
                        previewImageWidth, previewImageHeight);
                    
                    if (index == 0)
                        using (Pen pen = new Pen(Color.Black, previewImageOutlineThickness))
                            e.Graphics.DrawRectangle(pen, draw);
                    e.Graphics.DrawImage(images[i].Image, draw);
                }
            }
        }
        private void PBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                //m.MenuItems.Add(new MenuItem("Fix Width Ratio", ((object s, EventArgs ev) =>
                //{
                //    try
                //    {
                //        UpdateWindowRatioWidth();
                //    }
                //    catch { }
                //})));
                //m.MenuItems.Add(new MenuItem("Fix Height Ratio", ((object s, EventArgs ev) =>
                //{
                //    try
                //    {
                //        UpdateWindowRatioHeight();
                //    }
                //    catch { }
                //})));
                GraphicsUnit Unit = GraphicsUnit.Pixel;
                if (images[imagesIndex].Image.GetBounds(ref Unit).Width == ScreenshotHelper.allScreenBounds.Width)
                {
                    int i = 1;
                    foreach (Screen S in Screen.AllScreens)
                    {
                        m.MenuItems.Add(new MenuItem("Crop to " + i.ToShitEnglishNumberThingy() + " Screen", ((object s, EventArgs ev) =>
                        {
                            try
                            {
                                images.Insert(imagesIndex + 1, new Screenshot(ScreenshotHelper.CropImage(images[imagesIndex].Image,
                                    new Rectangle(S.Bounds.X - ScreenshotHelper.allScreenBounds.X,
                                    S.Bounds.Y - ScreenshotHelper.allScreenBounds.Y,
                                    S.Bounds.Width, S.Bounds.Height)), images[imagesIndex].FileName + "_CROPPED"));
                                imagesIndex++;
                                UpdateUI();
                            }
                            catch { }
                        })));
                        i++;
                    }
                }
                m.MenuItems.Add(new MenuItem("1:1 Size", ((object s, EventArgs ev) =>
                {
                    try
                    {
                        SetOriginalSize();
                    }
                    catch { }
                })));
                m.MenuItems.Add(new MenuItem("Smol Size", ((object s, EventArgs ev) =>
                {
                    try
                    {
                        Height = 350;
                        Width = 350;
                        CenterAroundMouse();
                    }
                    catch { }
                })));
                m.Show(pBox, e.Location);
            }
        }
        private void PBox_MouseMove(object sender, MouseEventArgs e)
        {
            pMouseCurrently = e.Location;
            ResetHudVisibility();

            if (isMouseDown && mode == colorPickerMenuItem)
            {
                try
                {
                    Point p = ZoomPicBoxCoordsToImageCoords(pMouseCurrently, pBox);
                    colorView.Update(CurrentScreenshot.Image.GetPixel(p.X, p.Y));
                } catch { }
            }
            if (isMouseDown && mode == drawMenuItem)
            {
                if (CurrentScreenshot.Saved)
                {
                    images.Insert(imagesIndex + 1, new Screenshot((Bitmap)images[imagesIndex].Image.Clone(), 
                        images[imagesIndex].FileName + "_DRAWN"));
                    imagesIndex += 1;
                    UpdateUI();
                }

                using (Graphics g = Graphics.FromImage(CurrentScreenshot.Image))
                {
                    Point mCur = ZoomPicBoxCoordsToImageCoords(pMouseCurrently, pBox);
                    Point mLast = ZoomPicBoxCoordsToImageCoords(pMouseLast, pBox);

                    int length = (int)Math.Sqrt(Math.Pow(pMouseCurrently.X - pMouseLast.X, 2) + Math.Pow(pMouseCurrently.Y - pMouseLast.Y, 2));
                    if (length > drawRadius / 2)
                        for (int i = 0; i < length; i++)
                        {
                            float uwu = i / (float)length;
                            int X = (int)(uwu * (mCur.X - drawRadius) + (1 - uwu) * (mLast.X - drawRadius));
                            int Y = (int)(uwu * (mCur.Y - drawRadius) + (1 - uwu) * (mLast.Y - drawRadius));
                            g.FillEllipse(new SolidBrush(config.Default.PrimaryColor), new Rectangle(X, Y, drawRadius * 2, drawRadius * 2));
                        }
                    else
                        g.FillEllipse(new SolidBrush(config.Default.PrimaryColor), 
                            new Rectangle(mCur.X - drawRadius, mCur.Y - drawRadius, drawRadius * 2, drawRadius * 2));
                }
            }

            pBox.Image = CurrentScreenshot.Image;
            pBox.Refresh();
            pMouseLast = e.Location;
        }
        private void PBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pMouseDown = e.Location;
                isMouseDown = true;

                if (mode == colorPickerMenuItem)
                {
                    try
                    {
                        Point p = ZoomPicBoxCoordsToImageCoords(pMouseCurrently, pBox);
                        colorView.Update(CurrentScreenshot.Image.GetPixel(p.X, p.Y));
                    }
                    catch { }
                }
            }
        }
        private void PBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && mode == cropMenuItem)
            {
                Rectangle crop = GetRectangleFromPoints(
                        ZoomPicBoxCoordsToImageCoords(pMouseDown, pBox),
                        ZoomPicBoxCoordsToImageCoords(pMouseCurrently, pBox));
                if (crop.Width == 0 || crop.Height == 0)
                {
                    isMouseDown = false;
                    return;
                }
                images.Insert(imagesIndex + 1, new Screenshot(ScreenshotHelper.CropImage(images[imagesIndex].Image, crop), 
                    images[imagesIndex].FileName + "_CROPPED"));
                imagesIndex += 1;
                UpdateUI();
            }
            isMouseDown = false;
        }

        // Other Events
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (images.Skip(1).ToList().Exists(x => !x.Saved) && 
                MessageBox.Show("Oi, you have unsaved Images! Do you really want to close me?", "Close?", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            config.Default.windowSize = Size;
            config.Default.instantShortcut = instantKeys.ToString();
            config.Default.cropShortcut = cropKeys.ToString();
            config.Default.gifShortcut = gifKeys.ToString();
            config.Default.Save();
        }
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            ResetHudVisibility();

            // Buttons
            bPrevious.Width = bSave.Location.X - bPrevious.Location.X - 6;
            bNext.Location = new Point(bDelete.Location.X + bDelete.Width + 6, bNext.Location.Y);
            bNext.Width = pBox.Width + pBox.Location.X - bNext.Location.X;

            Graphics graphics = this.CreateGraphics();
            float dpiY = graphics.DpiY, dpiX = graphics.DpiX;
            
            int buttonHeight = (int)(46 * (96 / dpiY));
            int spacing = (int)(8 * (96 / dpiX));

            int nextPosX = bNext.Location.X;
            if (middleButtons.Count > 0)
                nextPosX = middleButtons.Last().Location.X + middleButtons.Last().Size.Width + spacing;

            if (Height < 120)
            {
                foreach (Button b in middleButtons)
                    b.Location = new Point(b.Location.X, buttonHeight);
                bNext.Location = new Point(nextPosX, buttonHeight);
                bPrevious.Location = new Point(bPrevious.Location.X, buttonHeight);
            }
            else
            {
                foreach (Button b in middleButtons)
                    b.Location = new Point(b.Location.X, Height - (120 - buttonHeight));
                bNext.Location = new Point(nextPosX, Height - (120 - buttonHeight));
                bPrevious.Location = new Point(bPrevious.Location.X, Height - (120 - buttonHeight));
            }

            //// Snapping
            //int slurpSize = 10;
            //Size R = GetProperRatioSize(pBox.Size, Math.Abs(LastSize.Width - Width) > Math.Abs(LastSize.Height - Height),
            //    RecordedImages[0].Width / RecordedImages[0].Height);

            //if (R.Width + slurpSize > pBox.Width && R.Width - slurpSize < pBox.Width)
            //    Width += R.Width - pBox.Width;
            //if (R.Height + slurpSize > pBox.Height && R.Height - slurpSize < pBox.Height)
            //    Height += R.Height - pBox.Height;

            if (WindowState == FormWindowState.Maximized)
                isMouseDown = false;
        }
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            HudDisappearance.Enabled = this.WindowState != FormWindowState.Minimized;
        }
        private void HudDisappearance_Tick(object sender, EventArgs e)
        {
            HUDvisibility -= 0.06f;
            if (HUDvisibility < 0)
                HUDvisibility = 0;
            try { pBox.Refresh(); } catch { }
        }
        private void KeyHook_KeyDown(Keys key, bool Shift, bool Ctrl, bool Alt)
        {
            Debug.WriteLine($"Pressed {key}, {Shift}, {Ctrl}, {Alt}");

            if (DateTime.Now.Subtract(lastKeyDownEvent).TotalMilliseconds > 300)
            {
                if (instantKeys.IsPressed(Shift, Ctrl, Alt, key))
                {
                    AddScreenShot();
                    lastKeyDownEvent = DateTime.Now;
                }
                else if (cropKeys.IsPressed(Shift, Ctrl, Alt, key))
                {
                    AddScreenShotSnippingToolStyle();
                    lastKeyDownEvent = DateTime.Now;
                }
                else if (gifKeys.IsPressed(Shift, Ctrl, Alt, key))
                {
                    if (!recordingGif)
                        StartRecordingGif();
                    else
                        StopRecordingGif();
                    lastKeyDownEvent = DateTime.Now;
                }
            }
        }
        private void KeyHook_KeyUp(Keys key, bool Shift, bool Ctrl, bool Alt)
        {
            Debug.WriteLine($"Released {key}, {Shift}, {Ctrl}, {Alt}");

            //if (gifKeys.IsPressed(Shift, Ctrl, Alt, key))
            //{
                
            //}
        }

        // ToolStrip
        private void NeuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            images.Add(new Screenshot(new Bitmap(1000, 1000), "newBitmap"));
            imagesIndex = images.Count - 1;
            UpdateUI();
        }
        private void ShowFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentScreenshot.Saved)
                Process.Start("explorer.exe", "/select, \"" + CurrentScreenshot.Path + "\"");
            else
                Process.Start(config.Default.path);
        }
        private void FileToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = new System.Collections.Specialized.StringCollection
            {
                images[imagesIndex].Path
            };
            Clipboard.SetFileDropList(path);
        }
        private void ImageToClipboardToolStripMenuItem_Click(object sender, EventArgs e) => CopyCurrentImageToClipboard();
        private void BeendenToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();
        private void KeybindingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            keybindingsForm.RefreshDefaults();
            keybindingsForm.ShowDialog();
        }
        private void ChangeFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog
            {
                SelectedPath = config.Default.path
            };
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                config.Default.path = FBD.SelectedPath;
                Program.Restart();
                Application.Exit();
            }
        }
        private void NoneMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(noneMenuItem);
        private void CropMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(cropMenuItem);
        private void DrawMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(drawMenuItem);
        private void ColorPickerMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(colorPickerMenuItem);
        private void TextRecognitionToolStripMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(textRecognitionMenuItem);
        private void ChooseColorMenuItem_Click(object sender, EventArgs e) => ChangeEditMode(chooseColorMenuItem);
    }
}
