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
        public Bitmap Image { get; private set; }
        public string FileName { get; private set; }
        public bool Saved { get; private set; }
        public string Path { get; private set; }

        public Screenshot(Bitmap Image, string FileName)
        {
            this.Image = Image;
            this.FileName = FileName;
            Saved = false;
        }
        public Screenshot(string path)
        {
            this.Image = (Bitmap)Bitmap.FromFile(path);
            this.FileName = System.IO.Path.GetFileName(path);
            Saved = true;
            this.Path = path;
        }

        public void Save()
        {
            if (!Saved)
            {
                Image.Save(config.Default.path + "\\" + FileName + ".png");
                Saved = true;
            }
        }
        public void PutInClipboard()
        {
            Clipboard.SetImage(Image);
        }
    }
}
