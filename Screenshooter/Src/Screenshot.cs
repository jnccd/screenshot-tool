using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool
{
    public class Screenshot
    {
        private Bitmap image;
        public Bitmap Image { get {
                lock (this)
                {
                    if (image != null)
                        return image;
                    else if (!Path.IsNullOrWhiteSpace())
                    {
                        image = (Bitmap)Bitmap.FromFile(Path);
                        return image;
                    }
                    return null;
                }
            } private set { } }

        public string FileName { get; private set; }
        public bool Saved { get; set; }
        public string Path { get; private set; }
        private bool _saving = false;

        public Screenshot(Bitmap Image, string FileName)
        {
            this.image = Image;
            this.FileName = FileName;
            Saved = false;
        }
        public Screenshot(string path)
        {
            FileName = System.IO.Path.GetFileNameWithoutExtension(path);
            Saved = true;
            Path = path;
        }
        
        public void Save()
        {
            _saving = true;
            Bitmap savingImage;
            lock (this)
            {
                savingImage = (Bitmap)image.Clone();
            }
            if (!Saved)
            {
                Path = config.Default.path + "\\" + FileName + ".png";
                savingImage.Save(Path);
                Saved = true;
            }
            savingImage.Dispose();
            _saving = false;
        }
        public void PutInClipboard()
        {
            lock (this)
            {
                Clipboard.SetImage(image);
            }
        }
        public void DisposeImageCache()
        {
            lock (this)
            {
                if (Saved && image != null)
                {
                    image.Dispose();
                    image = null;
                }
            }
        }
        public void Delete()
        {
            while (_saving)
                Task.Delay(200).Wait();
            lock (this)
            {
                try
                {
                    if (File.Exists(Path))
                        FileSystem.DeleteFile(Path,
                                UIOption.OnlyErrorDialogs,
                                RecycleOption.SendToRecycleBin,
                                UICancelOption.ThrowException);
                }
                catch (Exception e) { Debug.WriteLine(e); }
            }
        }
    }
}
