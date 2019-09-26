using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool
{
    public class Shortcut
    {
        public static readonly string[] Specials = new string[] { "Shift", "Ctrl", "Alt" };
        public static readonly Keys[] AllKeys = (Keys[])Enum.GetValues(typeof(Keys));

        bool Shift;
        bool Ctrl;
        bool Alt;
        Keys Key;
    }
}
