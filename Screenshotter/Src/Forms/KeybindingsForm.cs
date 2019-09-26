using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool
{
    public partial class KeybindingsForm : Form
    {
        public KeybindingsForm()
        {
            InitializeComponent();
        }

        private void KeybindingsForm_Load(object sender, EventArgs e)
        {
            ComboBoxSpeicalInstant.Items.AddRange(Shortcut.Specials);
            ComboBoxKeyInstant.Items.AddRange(Shortcut.AllKeys.Select(x => x.ToString()).ToArray());
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {

        }
    }
}
