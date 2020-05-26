namespace ScreenshotTool.Src.Forms
{
    partial class ColorView
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbR = new System.Windows.Forms.TextBox();
            this.tbG = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbHTML = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "R:";
            // 
            // tbR
            // 
            this.tbR.Location = new System.Drawing.Point(36, 12);
            this.tbR.Name = "tbR";
            this.tbR.Size = new System.Drawing.Size(57, 20);
            this.tbR.TabIndex = 1;
            // 
            // tbG
            // 
            this.tbG.Location = new System.Drawing.Point(123, 12);
            this.tbG.Name = "tbG";
            this.tbG.Size = new System.Drawing.Size(57, 20);
            this.tbG.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(99, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "G:";
            // 
            // tbB
            // 
            this.tbB.Location = new System.Drawing.Point(210, 12);
            this.tbB.Name = "tbB";
            this.tbB.Size = new System.Drawing.Size(57, 20);
            this.tbB.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(186, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "B:";
            // 
            // tbHTML
            // 
            this.tbHTML.Location = new System.Drawing.Point(58, 38);
            this.tbHTML.Name = "tbHTML";
            this.tbHTML.Size = new System.Drawing.Size(209, 20);
            this.tbHTML.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "HTML:";
            // 
            // ColorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 71);
            this.Controls.Add(this.tbHTML);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbG);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbR);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorView";
            this.ShowInTaskbar = false;
            this.Text = "ColorView";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbR;
        private System.Windows.Forms.TextBox tbG;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbHTML;
        private System.Windows.Forms.Label label4;
    }
}