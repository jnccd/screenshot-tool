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
            this.label1 = new System.Windows.Forms.Label();
            this.tTargetLang = new System.Windows.Forms.TextBox();
            this.bTranslate = new System.Windows.Forms.Button();
            this.tTranslate = new System.Windows.Forms.TextBox();
            this.tSourceLang = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
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
            this.tOutText.Size = new System.Drawing.Size(464, 221);
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
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 270);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Translate from";
            // 
            // tTargetLang
            // 
            this.tTargetLang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tTargetLang.Location = new System.Drawing.Point(248, 267);
            this.tTargetLang.Name = "tTargetLang";
            this.tTargetLang.Size = new System.Drawing.Size(99, 22);
            this.tTargetLang.TabIndex = 4;
            this.tTargetLang.Text = "English";
            // 
            // bTranslate
            // 
            this.bTranslate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bTranslate.Location = new System.Drawing.Point(353, 266);
            this.bTranslate.Name = "bTranslate";
            this.bTranslate.Size = new System.Drawing.Size(122, 24);
            this.bTranslate.TabIndex = 5;
            this.bTranslate.Text = "Translate";
            this.bTranslate.UseVisualStyleBackColor = true;
            this.bTranslate.Click += new System.EventHandler(this.bTranslate_Click);
            // 
            // tTranslate
            // 
            this.tTranslate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tTranslate.Location = new System.Drawing.Point(12, 295);
            this.tTranslate.Multiline = true;
            this.tTranslate.Name = "tTranslate";
            this.tTranslate.ReadOnly = true;
            this.tTranslate.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tTranslate.Size = new System.Drawing.Size(464, 169);
            this.tTranslate.TabIndex = 6;
            // 
            // tSourceLang
            // 
            this.tSourceLang.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tSourceLang.Location = new System.Drawing.Point(117, 267);
            this.tSourceLang.Name = "tSourceLang";
            this.tSourceLang.Size = new System.Drawing.Size(99, 22);
            this.tSourceLang.TabIndex = 7;
            this.tSourceLang.Text = "Auto";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(222, 270);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "to";
            // 
            // TextRecognitionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 476);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tSourceLang);
            this.Controls.Add(this.tTranslate);
            this.Controls.Add(this.bTranslate);
            this.Controls.Add(this.tTargetLang);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tLang);
            this.Controls.Add(this.lConf);
            this.Controls.Add(this.tOutText);
            this.MinimumSize = new System.Drawing.Size(474, 416);
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
        private System.Windows.Forms.TextBox tLang;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tTargetLang;
        private System.Windows.Forms.Button bTranslate;
        private System.Windows.Forms.TextBox tTranslate;
        private System.Windows.Forms.TextBox tSourceLang;
        private System.Windows.Forms.Label label2;
    }
}