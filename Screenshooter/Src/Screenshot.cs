using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
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
                if (image != null)
                    return image;
                else if (!Path.IsNullOrWhiteSpace())
                {
                    image = (Bitmap)Bitmap.FromFile(Path);
                    return image;
                }
                return null;
            } private set { } }

        public string FileName { get; private set; }
        public bool Saved { get; private set; }
        public string Path { get; private set; }

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
            if (!Saved)
            {
                Path = config.Default.path + "\\" + FileName + ".png";
                image.Save(Path);
                Saved = true;
            }
        }
        public void PutInClipboard()
        {
            Clipboard.SetImage(image);
        }
        public void DisposeImageCache()
        {
            if (Saved && image != null)
            {
                image.Dispose();
                image = null;
            }
        }
        public void Delete()
        {
            if (File.Exists(Path))
                FileSystem.DeleteFile(Path,
                        UIOption.AllDialogs,
                        RecycleOption.SendToRecycleBin,
                        UICancelOption.ThrowException);
        }
    }
}
