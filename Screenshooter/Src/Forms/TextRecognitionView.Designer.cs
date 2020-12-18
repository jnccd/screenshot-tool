namespace ScreenshotTool
{
    partial class TextRecognitionView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tOutText = new System.Windows.Forms.TextBox();
            this.lConf = new System.Windows.Forms.Label();
            this.tLang = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tOutText
            // 
            this.tOutText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tOutText.Location = new System.Drawing.Point(12, 39);
            this.tOutText.Multiline = true;
            this.tOutText.Name = "tOutText";
            this.tOutText.ReadOnly = true;
            this.tOutText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tOutText.Size = new System.Drawing.Size(565, 346);
            this.tOutText.TabIndex = 0;
            // 
            // lConf
            // 
            this.lConf.AutoSize = true;
            this.lConf.Location = new System.Drawing.Point(140, 14);
            this.lConf.Name = "lConf";
            this.lConf.Size = new System.Drawing.Size(83, 17);
            this.lConf.TabIndex = 1;
            this.lConf.Text = "Confidence:";
            // 
            // tLang
            // 
            this.tLang.Location = new System.Drawing.Point(12, 11);
            this.tLang.Name = "tLang";
            this.tLang.Size = new System.Drawing.Size(122, 22);
            this.tLang.TabIndex = 2;
            this.tLang.Text = "eng";
            this.tLang.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TLang_KeyDown);
            // 
            // TextRecognitionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 397);
            this.Controls.Add(this.tLang);
            this.Controls.Add(this.lConf);
            this.Controls.Add(this.tOutText);
            this.Name = "TextRecognitionView";
            this.Text = "Text Recognition";
            this.Load += new System.EventHandler(this.TextRecognitionView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tOutText;
        private System.Windows.Forms.Label lConf;
        private System.Windows.Forms.TextBox tLang;
    }
}