namespace ScreenshotTool
{
    partial class KeybindingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeybindingsForm));
            this.ComboBoxSpeicalInstant = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ComboBoxKeyInstant = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ComboBoxSpeicalCrop = new System.Windows.Forms.ComboBox();
            this.ComboBoxKeyCrop = new System.Windows.Forms.ComboBox();
            this.ButtonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ComboBoxSpeicalInstant
            // 
            this.ComboBoxSpeicalInstant.FormattingEnabled = true;
            this.ComboBoxSpeicalInstant.Location = new System.Drawing.Point(125, 25);
            this.ComboBoxSpeicalInstant.Name = "ComboBoxSpeicalInstant";
            this.ComboBoxSpeicalInstant.Size = new System.Drawing.Size(121, 21);
            this.ComboBoxSpeicalInstant.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Instant Screenshot";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(122, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Special Key";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(249, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Key";
            // 
            // ComboBoxKeyInstant
            // 
            this.ComboBoxKeyInstant.FormattingEnabled = true;
            this.ComboBoxKeyInstant.Location = new System.Drawing.Point(252, 25);
            this.ComboBoxKeyInstant.Name = "ComboBoxKeyInstant";
            this.ComboBoxKeyInstant.Size = new System.Drawing.Size(121, 21);
            this.ComboBoxKeyInstant.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Cropping Screenshot";
            // 
            // ComboBoxSpeicalCrop
            // 
            this.ComboBoxSpeicalCrop.FormattingEnabled = true;
            this.ComboBoxSpeicalCrop.Location = new System.Drawing.Point(125, 52);
            this.ComboBoxSpeicalCrop.Name = "ComboBoxSpeicalCrop";
            this.ComboBoxSpeicalCrop.Size = new System.Drawing.Size(121, 21);
            this.ComboBoxSpeicalCrop.TabIndex = 6;
            // 
            // ComboBoxKeyCrop
            // 
            this.ComboBoxKeyCrop.FormattingEnabled = true;
            this.ComboBoxKeyCrop.Location = new System.Drawing.Point(252, 52);
            this.ComboBoxKeyCrop.Name = "ComboBoxKeyCrop";
            this.ComboBoxKeyCrop.Size = new System.Drawing.Size(121, 21);
            this.ComboBoxKeyCrop.TabIndex = 7;
            // 
            // ButtonSave
            // 
            this.ButtonSave.Location = new System.Drawing.Point(252, 79);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(120, 23);
            this.ButtonSave.TabIndex = 8;
            this.ButtonSave.Text = "Save";
            this.ButtonSave.UseVisualStyleBackColor = true;
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // KeybindingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 111);
            this.Controls.Add(this.ButtonSave);
            this.Controls.Add(this.ComboBoxKeyCrop);
            this.Controls.Add(this.ComboBoxSpeicalCrop);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ComboBoxKeyInstant);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ComboBoxSpeicalInstant);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 150);
            this.MinimumSize = new System.Drawing.Size(400, 150);
            this.Name = "KeybindingsForm";
            this.Text = "Keybindings";
            this.Load += new System.EventHandler(this.KeybindingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ComboBoxSpeicalInstant;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ComboBoxKeyInstant;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ComboBoxSpeicalCrop;
        private System.Windows.Forms.ComboBox ComboBoxKeyCrop;
        private System.Windows.Forms.Button ButtonSave;
    }
}