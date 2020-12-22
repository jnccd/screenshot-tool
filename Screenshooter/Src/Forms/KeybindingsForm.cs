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
            RefreshDefaults();
        }

        public void RefreshDefaults()
        {
            ComboBoxSpeicalInstant.Items.Clear();
            ComboBoxSpeicalInstant.Items.AddRange(Shortcut.Specials);
            ComboBoxKeyInstant.Items.Clear();
            ComboBoxKeyInstant.Items.AddRange(Shortcut.AllKeys.Select(x => x.ToString()).OrderBy(x => x).ToArray());

            ComboBoxSpeicalCrop.Items.Clear();
            ComboBoxSpeicalCrop.Items.AddRange(Shortcut.Specials);
            ComboBoxKeyCrop.Items.Clear();
            ComboBoxKeyCrop.Items.AddRange(Shortcut.AllKeys.Select(x => x.ToString()).OrderBy(x => x).ToArray());

            ComboBoxSpeicalGif.Items.Clear();
            ComboBoxSpeicalGif.Items.AddRange(Shortcut.Specials);
            ComboBoxKeyGif.Items.Clear();
            ComboBoxKeyGif.Items.AddRange(Shortcut.AllKeys.Select(x => x.ToString()).OrderBy(x => x).ToArray());

            ComboBoxSpeicalInstant.Text = Program.mainForm.instantKeys.SpecialKey();
            ComboBoxKeyInstant.Text = Program.mainForm.instantKeys.Key.ToString();

            ComboBoxSpeicalCrop.Text = Program.mainForm.cropKeys.SpecialKey();
            ComboBoxKeyCrop.Text = Program.mainForm.cropKeys.Key.ToString();

            ComboBoxSpeicalGif.Text = Program.mainForm.gifKeys.SpecialKey();
            ComboBoxKeyGif.Text = Program.mainForm.gifKeys.Key.ToString();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                Program.mainForm.instantKeys = new Shortcut()
                {
                    Shift = ComboBoxSpeicalInstant.Text == Shortcut.Specials[0],
                    Ctrl = ComboBoxSpeicalInstant.Text == Shortcut.Specials[1],
                    Alt = ComboBoxSpeicalInstant.Text == Shortcut.Specials[2],
                    Key = (Keys)Enum.Parse(typeof(Keys), ComboBoxKeyInstant.Text)
                };
                Program.mainForm.cropKeys = new Shortcut()
                {
                    Shift = ComboBoxSpeicalCrop.Text == Shortcut.Specials[0],
                    Ctrl = ComboBoxSpeicalCrop.Text == Shortcut.Specials[1],
                    Alt = ComboBoxSpeicalCrop.Text == Shortcut.Specials[2],
                    Key = (Keys)Enum.Parse(typeof(Keys), ComboBoxKeyCrop.Text)
                };
                Program.mainForm.gifKeys = new Shortcut()
                {
                    Shift = ComboBoxSpeicalGif.Text == Shortcut.Specials[0],
                    Ctrl = ComboBoxSpeicalGif.Text == Shortcut.Specials[1],
                    Alt = ComboBoxSpeicalGif.Text == Shortcut.Specials[2],
                    Key = (Keys)Enum.Parse(typeof(Keys), ComboBoxKeyGif.Text)
                };

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
