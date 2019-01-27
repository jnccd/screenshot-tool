using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiScreenScreenshot
{
    public class Screenshot
    {
        public Bitmap Image;
        public string FileName;
        public bool Saved;
        
        public Screenshot(Bitmap Image, string FileName)
        {
            this.Image = Image;
            this.FileName = FileName;
            Saved = false;
        }
    }
}
