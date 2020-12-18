using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace ScreenshotTool
{
    public partial class TextRecognitionView : Form
    {
        public TextRecognitionView()
        {
            InitializeComponent();
        }

        private void UpdateReadings()
        {
            try
            {
                using var engine = new TesseractEngine(@"./tessdata", tLang.Text, EngineMode.Default);
                using var img = Pix.LoadFromFile(Program.mainForm.CurrentScreenshot.Path);
                using var page = engine.Process(img);
                using var iter = page.GetIterator();

                string text = page.GetText().Replace("\n", "\r\n");
                string conf = page.GetMeanConfidence().ToString();

                tOutText.Text = text;
                lConf.Text = $"Confidence: {conf}";
            }
            catch
            {
                tOutText.Text = "Error Reading the images text with the given Parameters!";
                lConf.Text = $"Confidence: x";
            }
        }

        // Events
        private void TextRecognitionView_Load(object sender, EventArgs e)
        {
            UpdateReadings();
        }

        private void TLang_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateReadings();
            }
        }

        private void TextRecognitionView_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.mainForm.SetModeToNone();
        }
    }
}
