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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextRecognitionView));
            this.tOutText = new System.Windows.Forms.TextBox();
            this.lConf = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tTargetLang = new System.Windows.Forms.TextBox();
            this.bTranslate = new System.Windows.Forms.Button();
            this.tTranslate = new System.Windows.Forms.TextBox();
            this.tSourceLang = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bSearch = new System.Windows.Forms.Button();
            this.cLang = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // tOutText
            // 
            this.tOutText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tOutText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tOutText.Location = new System.Drawing.Point(11, 38);
            this.tOutText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tOutText.Multiline = true;
            this.tOutText.Name = "tOutText";
            this.tOutText.ReadOnly = true;
            this.tOutText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tOutText.Size = new System.Drawing.Size(344, 257);
            this.tOutText.TabIndex = 0;
            // 
            // lConf
            // 
            this.lConf.AutoSize = true;
            this.lConf.Location = new System.Drawing.Point(105, 16);
            this.lConf.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lConf.Name = "lConf";
            this.lConf.Size = new System.Drawing.Size(64, 13);
            this.lConf.TabIndex = 1;
            this.lConf.Text = "Confidence:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 237);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Translate from";
            // 
            // tTargetLang
            // 
            this.tTargetLang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tTargetLang.Location = new System.Drawing.Point(186, 235);
            this.tTargetLang.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tTargetLang.Name = "tTargetLang";
            this.tTargetLang.Size = new System.Drawing.Size(76, 20);
            this.tTargetLang.TabIndex = 4;
            this.tTargetLang.Text = "English";
            // 
            // bTranslate
            // 
            this.bTranslate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bTranslate.Enabled = false;
            this.bTranslate.Location = new System.Drawing.Point(266, 234);
            this.bTranslate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bTranslate.Name = "bTranslate";
            this.bTranslate.Size = new System.Drawing.Size(92, 20);
            this.bTranslate.TabIndex = 5;
            this.bTranslate.Text = "Translate";
            this.bTranslate.UseVisualStyleBackColor = true;
            this.bTranslate.Click += new System.EventHandler(this.bTranslate_Click);
            // 
            // tTranslate
            // 
            this.tTranslate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tTranslate.Location = new System.Drawing.Point(9, 258);
            this.tTranslate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tTranslate.Multiline = true;
            this.tTranslate.Name = "tTranslate";
            this.tTranslate.ReadOnly = true;
            this.tTranslate.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tTranslate.Size = new System.Drawing.Size(349, 33);
            this.tTranslate.TabIndex = 6;
            // 
            // tSourceLang
            // 
            this.tSourceLang.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tSourceLang.Location = new System.Drawing.Point(88, 235);
            this.tSourceLang.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tSourceLang.Name = "tSourceLang";
            this.tSourceLang.Size = new System.Drawing.Size(75, 20);
            this.tSourceLang.TabIndex = 7;
            this.tSourceLang.Text = "Auto";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(166, 237);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "to";
            // 
            // bSearch
            // 
            this.bSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bSearch.Location = new System.Drawing.Point(291, 12);
            this.bSearch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bSearch.Name = "bSearch";
            this.bSearch.Size = new System.Drawing.Size(64, 21);
            this.bSearch.TabIndex = 9;
            this.bSearch.Text = "Search";
            this.bSearch.UseVisualStyleBackColor = true;
            this.bSearch.Click += new System.EventHandler(this.bSearch_Click);
            // 
            // cLang
            // 
            this.cLang.FormattingEnabled = true;
            this.cLang.Location = new System.Drawing.Point(9, 12);
            this.cLang.Name = "cLang";
            this.cLang.Size = new System.Drawing.Size(91, 21);
            this.cLang.TabIndex = 10;
            this.cLang.Text = "eng";
            this.cLang.SelectionChangeCommitted += new System.EventHandler(this.cLang_SelectionChangeCommitted);
            this.cLang.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cLang_KeyDown);
            // 
            // TextRecognitionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 306);
            this.Controls.Add(this.cLang);
            this.Controls.Add(this.bSearch);
            this.Controls.Add(this.lConf);
            this.Controls.Add(this.tOutText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tSourceLang);
            this.Controls.Add(this.tTranslate);
            this.Controls.Add(this.bTranslate);
            this.Controls.Add(this.tTargetLang);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(360, 345);
            this.Name = "TextRecognitionView";
            this.Text = "Text Recognition";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TextRecognitionView_FormClosed);
            this.Load += new System.EventHandler(this.TextRecognitionView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tOutText;
        private System.Windows.Forms.Label lConf;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tTargetLang;
        private System.Windows.Forms.Button bTranslate;
        private System.Windows.Forms.TextBox tTranslate;
        private System.Windows.Forms.TextBox tSourceLang;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bSearch;
        private System.Windows.Forms.ComboBox cLang;
    }
}