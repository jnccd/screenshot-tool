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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bPath = new System.Windows.Forms.Button();
            this.pBox = new System.Windows.Forms.PictureBox();
            this.bSave = new System.Windows.Forms.Button();
            this.bPrevious = new System.Windows.Forms.Button();
            this.bNext = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.bOpen = new System.Windows.Forms.Button();
            this.bCropScreenshot = new System.Windows.Forms.Button();
            this.bScreenshot = new System.Windows.Forms.Button();
            this.bDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).BeginInit();
            this.SuspendLayout();
            // 
            // bPath
            // 
            this.bPath.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bPath.Location = new System.Drawing.Point(486, 152);
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
            this.pBox.Location = new System.Drawing.Point(12, 41);
            this.pBox.Name = "pBox";
            this.pBox.Size = new System.Drawing.Size(744, 105);
            this.pBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pBox.TabIndex = 1;
            this.pBox.TabStop = false;
            this.pBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pBox_Paint);
            this.pBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pBox_MouseClick);
            this.pBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pBox_MouseDown);
            this.pBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pBox_MouseMove);
            this.pBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pBox_MouseUp);
            // 
            // bSave
            // 
            this.bSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bSave.Location = new System.Drawing.Point(162, 152);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(102, 23);
            this.bSave.TabIndex = 2;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bPrevious
            // 
            this.bPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bPrevious.Location = new System.Drawing.Point(12, 152);
            this.bPrevious.Name = "bPrevious";
            this.bPrevious.Size = new System.Drawing.Size(144, 23);
            this.bPrevious.TabIndex = 3;
            this.bPrevious.Text = "<-";
            this.bPrevious.UseVisualStyleBackColor = true;
            this.bPrevious.Click += new System.EventHandler(this.bPrevious_Click);
            // 
            // bNext
            // 
            this.bNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bNext.Location = new System.Drawing.Point(594, 152);
            this.bNext.Name = "bNext";
            this.bNext.Size = new System.Drawing.Size(162, 23);
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
            // bOpen
            // 
            this.bOpen.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bOpen.Location = new System.Drawing.Point(378, 152);
            this.bOpen.Name = "bOpen";
            this.bOpen.Size = new System.Drawing.Size(102, 23);
            this.bOpen.TabIndex = 6;
            this.bOpen.Text = "Open Folder";
            this.bOpen.UseVisualStyleBackColor = true;
            this.bOpen.Click += new System.EventHandler(this.bOpen_Click);
            // 
            // bCropScreenshot
            // 
            this.bCropScreenshot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bCropScreenshot.Location = new System.Drawing.Point(392, 12);
            this.bCropScreenshot.Name = "bCropScreenshot";
            this.bCropScreenshot.Size = new System.Drawing.Size(364, 23);
            this.bCropScreenshot.TabIndex = 7;
            this.bCropScreenshot.Text = "Selection Screenshot [ALT + Pause]";
            this.bCropScreenshot.UseVisualStyleBackColor = true;
            this.bCropScreenshot.Click += new System.EventHandler(this.bCropScreenshot_Click);
            // 
            // bScreenshot
            // 
            this.bScreenshot.Location = new System.Drawing.Point(12, 12);
            this.bScreenshot.Name = "bScreenshot";
            this.bScreenshot.Size = new System.Drawing.Size(374, 23);
            this.bScreenshot.TabIndex = 8;
            this.bScreenshot.Text = "Quick Screenshot [Pause]";
            this.bScreenshot.UseVisualStyleBackColor = true;
            this.bScreenshot.Click += new System.EventHandler(this.bScreenshot_Click);
            // 
            // bDelete
            // 
            this.bDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bDelete.Location = new System.Drawing.Point(270, 152);
            this.bDelete.Name = "bDelete";
            this.bDelete.Size = new System.Drawing.Size(102, 23);
            this.bDelete.TabIndex = 9;
            this.bDelete.Text = "Delete";
            this.bDelete.UseVisualStyleBackColor = true;
            this.bDelete.Click += new System.EventHandler(this.bDelete_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 187);
            this.Controls.Add(this.bDelete);
            this.Controls.Add(this.bScreenshot);
            this.Controls.Add(this.bCropScreenshot);
            this.Controls.Add(this.bOpen);
            this.Controls.Add(this.bNext);
            this.Controls.Add(this.bPrevious);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.pBox);
            this.Controls.Add(this.bPath);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(16, 86);
            this.Name = "Form1";
            this.Text = "Multi Screen Screenshot";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
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
        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.Button bCropScreenshot;
        private System.Windows.Forms.Button bScreenshot;
        private System.Windows.Forms.Button bDelete;
    }
}

