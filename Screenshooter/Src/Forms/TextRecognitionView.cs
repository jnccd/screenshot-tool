using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using System.Net;
using System.IO;

namespace ScreenshotTool
{
    public partial class TextRecognitionView : Form
    {
        //readonly GoogleTranslator translator = new GoogleTranslator();
        const string tessdataPath = @"./tessdata";

        public TextRecognitionView()
        {
            InitializeComponent();
        }

        public void UpdateReadings()
        {
            try
            {
                using var engine = new TesseractEngine(tessdataPath, cLang.Text, EngineMode.Default);
                using var img = Pix.LoadFromFile(Program.mainForm.CurrentScreenshot.Path);
                using var page = engine.Process(img);
                using var iter = page.GetIterator();

                string text = page.GetText().Replace("\n", "\r\n");
                string conf = page.GetMeanConfidence().ToString();

                if (cLang.Text.StartsWith("chi"))
                    text = text.Replace(" ", "");

                tOutText.Text = text;
                lConf.Text = $"Confidence: {conf}";
            }
            catch
            {
                tOutText.Text = "Error reading the images text with the given parameters!";
                lConf.Text = $"Confidence: x";
            }
        }

        // Events
        private void TextRecognitionView_Load(object sender, EventArgs e)
        {
            UpdateReadings();

            // Fill combobox options
            string tessFilePattern = "*.traineddata";
            cLang.Items.AddRange(
                new DirectoryInfo(tessdataPath).
                GetFiles(tessFilePattern).
                    Select(x => x.ToString().Remove(x.ToString().Length - tessFilePattern.Length + 1)).
                    ToArray());
        }

        private void bTranslate_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    throw new NotImplementedException();

            //    Language from = string.IsNullOrWhiteSpace(tSourceLang.Text) || tSourceLang.Text == "Auto" ? Language.Auto : GoogleTranslator.GetLanguageByName(tSourceLang.Text);
            //    Language to = GoogleTranslator.GetLanguageByName(tTargetLang.Text);

            //    TranslationResult result = translator.TranslateLiteAsync(tOutText.Text, from, to).Result;

            //    tTranslate.Text = result.MergedTranslation;
            //}
            //catch
            //{
            //    tTranslate.Text = "Error translating the text with the given parameters!";
            //}
        }

        private void TextRecognitionView_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.mainForm.SetModeToNone();
        }

        private void bSearch_Click(object sender, EventArgs e)
        {
            Process.Start($"https://www.google.de/search?q={WebUtility.UrlEncode(tOutText.SelectedText)}");
        }

        private void cLang_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateReadings();
            }
        }

        private void cLang_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateReadings();
        }

        private void bTranslateBrowser_Click(object sender, EventArgs e)
        {
            Process.Start($"https://translate.google.de/?sl=auto&tl=en&text={WebUtility.UrlEncode(tOutText.SelectedText)}&op=translate");
        }
    }
}
