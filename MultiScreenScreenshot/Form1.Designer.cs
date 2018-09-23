namespace MultiScreenScreenshot
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bPath = new System.Windows.Forms.Button();
            this.pBox = new System.Windows.Forms.PictureBox();
            this.bSave = new System.Windows.Forms.Button();
            this.bPrevious = new System.Windows.Forms.Button();
            this.bNext = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).BeginInit();
            this.SuspendLayout();
            // 
            // bPath
            // 
            this.bPath.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bPath.Location = new System.Drawing.Point(387, 428);
            this.bPath.Name = "bPath";
            this.bPath.Size = new System.Drawing.Size(102, 23);
            this.bPath.TabIndex = 0;
            this.bPath.Text = "Change Path";
            this.bPath.UseVisualStyleBackColor = true;
            this.bPath.Click += new System.EventHandler(this.bPath_Click);
            // 
            // pBox
            // 
            this.pBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pBox.Location = new System.Drawing.Point(12, 12);
            this.pBox.Name = "pBox";
            this.pBox.Size = new System.Drawing.Size(744, 410);
            this.pBox.TabIndex = 1;
            this.pBox.TabStop = false;
            // 
            // bSave
            // 
            this.bSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bSave.Location = new System.Drawing.Point(279, 428);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(102, 23);
            this.bSave.TabIndex = 2;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bPrevious
            // 
            this.bPrevious.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bPrevious.Location = new System.Drawing.Point(12, 428);
            this.bPrevious.Name = "bPrevious";
            this.bPrevious.Size = new System.Drawing.Size(261, 23);
            this.bPrevious.TabIndex = 3;
            this.bPrevious.Text = "<-";
            this.bPrevious.UseVisualStyleBackColor = true;
            this.bPrevious.Click += new System.EventHandler(this.bPrevious_Click);
            // 
            // bNext
            // 
            this.bNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bNext.Location = new System.Drawing.Point(495, 428);
            this.bNext.Name = "bNext";
            this.bNext.Size = new System.Drawing.Size(261, 23);
            this.bNext.TabIndex = 4;
            this.bNext.Text = "->";
            this.bNext.UseVisualStyleBackColor = true;
            this.bNext.Click += new System.EventHandler(this.bNext_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 463);
            this.Controls.Add(this.bNext);
            this.Controls.Add(this.bPrevious);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.pBox);
            this.Controls.Add(this.bPath);
            this.MinimumSize = new System.Drawing.Size(784, 502);
            this.Name = "Form1";
            this.Text = "Multi Screen Screenshot";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bPath;
        private System.Windows.Forms.PictureBox pBox;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bPrevious;
        private System.Windows.Forms.Button bNext;
        private System.Windows.Forms.Timer timer1;
    }
}

