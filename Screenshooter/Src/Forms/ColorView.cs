using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool.Src.Forms
{
    public partial class ColorView : Form
    {
        public ColorView()
        {
            InitializeComponent();
        }

        public void Update(Color c)
        {
            tbR.Text = c.R.ToString();
            tbG.Text = c.G.ToString();
            tbB.Text = c.B.ToString();

            tbHTML.Text = ColorTranslator.ToHtml(c);
        }

        private void ColorView_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.mainForm.SetModeToNone();
        }

        private void ColorView_Load(object sender, EventArgs e)
        {

        }
    }
}
