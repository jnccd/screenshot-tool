namespace ScreenshotTool
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pBox = new System.Windows.Forms.PictureBox();
            this.bSave = new System.Windows.Forms.Button();
            this.bPrevious = new System.Windows.Forms.Button();
            this.bNext = new System.Windows.Forms.Button();
            this.bDelete = new System.Windows.Forms.Button();
            this.HudDisappearance = new System.Windows.Forms.Timer(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.imageToClipboardMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToClipboardMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cropMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorPickerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.chooseColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.KeybindingsMenuEntry = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pBox
            // 
            this.pBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pBox.Location = new System.Drawing.Point(12, 27);
            this.pBox.Name = "pBox";
            this.pBox.Size = new System.Drawing.Size(744, 232);
            this.pBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pBox.TabIndex = 1;
            this.pBox.TabStop = false;
            this.pBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PBox_Paint);
            this.pBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PBox_MouseClick);
            this.pBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PBox_MouseDown);
            this.pBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PBox_MouseMove);
            this.pBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PBox_MouseUp);
            // 
            // bSave
            // 
            this.bSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bSave.Location = new System.Drawing.Point(284, 265);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(102, 23);
            this.bSave.TabIndex = 2;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.BSave_Click);
            // 
            // bPrevious
            // 
            this.bPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bPrevious.Location = new System.Drawing.Point(12, 265);
            this.bPrevious.Name = "bPrevious";
            this.bPrevious.Size = new System.Drawing.Size(266, 23);
            this.bPrevious.TabIndex = 3;
            this.bPrevious.Text = "<-";
            this.bPrevious.UseVisualStyleBackColor = true;
            this.bPrevious.Click += new System.EventHandler(this.BPrevious_Click);
            // 
            // bNext
            // 
            this.bNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bNext.Location = new System.Drawing.Point(500, 265);
            this.bNext.Name = "bNext";
            this.bNext.Size = new System.Drawing.Size(256, 23);
            this.bNext.TabIndex = 4;
            this.bNext.Text = "->";
            this.bNext.UseVisualStyleBackColor = true;
            this.bNext.Click += new System.EventHandler(this.BNext_Click);
            // 
            // bDelete
            // 
            this.bDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bDelete.Location = new System.Drawing.Point(392, 265);
            this.bDelete.Name = "bDelete";
            this.bDelete.Size = new System.Drawing.Size(102, 23);
            this.bDelete.TabIndex = 9;
            this.bDelete.Text = "Delete";
            this.bDelete.UseVisualStyleBackColor = true;
            this.bDelete.Click += new System.EventHandler(this.BDelete_Click);
            // 
            // HudDisappearance
            // 
            this.HudDisappearance.Enabled = true;
            this.HudDisappearance.Interval = 20;
            this.HudDisappearance.Tick += new System.EventHandler(this.HudDisappearance_Tick);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.editMenuItem,
            this.extrasToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(768, 24);
            this.menuStrip.TabIndex = 10;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showFolderMenuItem,
            this.changeFolderMenuItem,
            this.toolStripSeparator,
            this.imageToClipboardMenuItem,
            this.fileToClipboardMenuItem,
            this.toolStripSeparator2,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileMenuItem.Text = "File";
            // 
            // showFolderMenuItem
            // 
            this.showFolderMenuItem.Name = "showFolderMenuItem";
            this.showFolderMenuItem.Size = new System.Drawing.Size(176, 22);
            this.showFolderMenuItem.Text = "Show in Explorer";
            this.showFolderMenuItem.Click += new System.EventHandler(this.ShowFolderToolStripMenuItem_Click);
            // 
            // changeFolderMenuItem
            // 
            this.changeFolderMenuItem.Name = "changeFolderMenuItem";
            this.changeFolderMenuItem.Size = new System.Drawing.Size(176, 22);
            this.changeFolderMenuItem.Text = "Change Folder";
            this.changeFolderMenuItem.Click += new System.EventHandler(this.ChangeFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(173, 6);
            // 
            // imageToClipboardMenuItem
            // 
            this.imageToClipboardMenuItem.Name = "imageToClipboardMenuItem";
            this.imageToClipboardMenuItem.Size = new System.Drawing.Size(176, 22);
            this.imageToClipboardMenuItem.Text = "Image to Clipboard";
            this.imageToClipboardMenuItem.Click += new System.EventHandler(this.ImageToClipboardToolStripMenuItem_Click);
            // 
            // fileToClipboardMenuItem
            // 
            this.fileToClipboardMenuItem.Name = "fileToClipboardMenuItem";
            this.fileToClipboardMenuItem.Size = new System.Drawing.Size(176, 22);
            this.fileToClipboardMenuItem.Text = "File to Clipboard";
            this.fileToClipboardMenuItem.Click += new System.EventHandler(this.FileToClipboardToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(173, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(176, 22);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.BeendenToolStripMenuItem_Click);
            // 
            // editMenuItem
            // 
            this.editMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cropMenuItem,
            this.drawMenuItem,
            this.colorPickerMenuItem,
            this.toolStripSeparator1,
            this.chooseColorToolStripMenuItem});
            this.editMenuItem.Name = "editMenuItem";
            this.editMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editMenuItem.Text = "Edit";
            // 
            // cropMenuItem
            // 
            this.cropMenuItem.CheckOnClick = true;
            this.cropMenuItem.Name = "cropMenuItem";
            this.cropMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cropMenuItem.Text = "Crop";
            this.cropMenuItem.Click += new System.EventHandler(this.CropMenuItem_Click);
            // 
            // drawMenuItem
            // 
            this.drawMenuItem.CheckOnClick = true;
            this.drawMenuItem.Name = "drawMenuItem";
            this.drawMenuItem.Size = new System.Drawing.Size(180, 22);
            this.drawMenuItem.Text = "Draw";
            this.drawMenuItem.Click += new System.EventHandler(this.DrawMenuItem_Click);
            // 
            // colorPickerMenuItem
            // 
            this.colorPickerMenuItem.Name = "colorPickerMenuItem";
            this.colorPickerMenuItem.Size = new System.Drawing.Size(180, 22);
            this.colorPickerMenuItem.Text = "ColorViewer";
            this.colorPickerMenuItem.Click += new System.EventHandler(this.ColorPickerMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // chooseColorToolStripMenuItem
            // 
            this.chooseColorToolStripMenuItem.Name = "chooseColorToolStripMenuItem";
            this.chooseColorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.chooseColorToolStripMenuItem.Text = "Choose Color";
            this.chooseColorToolStripMenuItem.Click += new System.EventHandler(this.ChooseColorMenuItem_Click);
            // 
            // extrasToolStripMenuItem
            // 
            this.extrasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.KeybindingsMenuEntry});
            this.extrasToolStripMenuItem.Name = "extrasToolStripMenuItem";
            this.extrasToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.extrasToolStripMenuItem.Text = "Options";
            // 
            // KeybindingsMenuEntry
            // 
            this.KeybindingsMenuEntry.Name = "KeybindingsMenuEntry";
            this.KeybindingsMenuEntry.Size = new System.Drawing.Size(139, 22);
            this.KeybindingsMenuEntry.Text = "Keybindings";
            this.KeybindingsMenuEntry.Click += new System.EventHandler(this.KeybindingsToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 300);
            this.Controls.Add(this.bDelete);
            this.Controls.Add(this.bNext);
            this.Controls.Add(this.bPrevious);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.pBox);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(32, 120);
            this.Name = "MainForm";
            this.Text = "Multi Screen Screenshot";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pBox;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bPrevious;
        private System.Windows.Forms.Button bNext;
        private System.Windows.Forms.Button bDelete;
        private System.Windows.Forms.Timer HudDisappearance;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extrasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem KeybindingsMenuEntry;
        private System.Windows.Forms.ToolStripMenuItem fileToClipboardMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showFolderMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeFolderMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageToClipboardMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cropMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorPickerMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem chooseColorToolStripMenuItem;
    }
}

