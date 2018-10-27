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
        public string fileName;
        
        public Screenshot(Bitmap Image, string fileName)
        {
            this.Image = Image;
            this.fileName = fileName;
        }
    }
}
