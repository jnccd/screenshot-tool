using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool
{
    public static class Extensions
    {
        public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
        {
            if (obj.InvokeRequired)
            {
                var args = new object[0];
                obj.Invoke(action, args);
            }
            else
            {
                action();
            }
        }
        public static string ToShitEnglishNumberThingy(this int i)
        {
            if (i == 1)
                return "1st";
            if (i == 2)
                return "2nd";
            if (i == 3)
                return "3rd";
            else
                return i + "th";
        }
    }
}
