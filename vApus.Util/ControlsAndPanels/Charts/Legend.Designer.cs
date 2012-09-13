namespace vApus.Util
{
    partial class Legend
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.chooseColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkToggle = new System.Windows.Forms.CheckBox();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.tvw = new System.Windows.Forms.TreeView();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cmnu.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmnu
            // 
            this.cmnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chooseColorToolStripMenuItem});
            this.cmnu.Name = "cmnu";
            this.cmnu.Size = new System.Drawing.Size(156, 26);
            this.cmnu.Opening += new System.ComponentModel.CancelEventHandler(this.cmnu_Opening);
            // 
            // chooseColorToolStripMenuItem
            // 
            this.chooseColorToolStripMenuItem.Name = "chooseColorToolStripMenuItem";
            this.chooseColorToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.chooseColorToolStripMenuItem.Text = "Choose Color...";
            this.chooseColorToolStripMenuItem.Click += new System.EventHandler(this.chooseColorToolStripMenuItem_Click);
            // 
            // chkToggle
            // 
            this.chkToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkToggle.AutoSize = true;
            this.chkToggle.Checked = true;
            this.chkToggle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkToggle.Enabled = false;
            this.chkToggle.Location = new System.Drawing.Point(107, 130);
            this.chkToggle.Name = "chkToggle";
            this.chkToggle.Size = new System.Drawing.Size(65, 17);
            this.chkToggle.TabIndex = 1;
            this.chkToggle.Text = "Toggle";
            this.chkToggle.UseVisualStyleBackColor = true;
            this.chkToggle.CheckedChanged += new System.EventHandler(this.chkToggle_CheckedChanged);
            // 
            // colorDialog
            // 
            this.colorDialog.FullOpen = true;
            this.colorDialog.SolidColorOnly = true;
            // 
            // tvw
            // 
            this.tvw.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvw.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvw.CheckBoxes = true;
            this.tvw.ContextMenuStrip = this.cmnu;
            this.tvw.FullRowSelect = true;
            this.tvw.HideSelection = false;
            this.tvw.Location = new System.Drawing.Point(4, 4);
            this.tvw.Name = "tvw";
            this.tvw.ShowRootLines = false;
            this.tvw.Size = new System.Drawing.Size(168, 120);
            this.tvw.TabIndex = 2;
            this.toolTip.SetToolTip(this.tvw, "Right-click to set another color for the selected series.");
            this.tvw.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvw_AfterCheck);
            this.tvw.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvw_AfterSelect);
            // 
            // Legend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tvw);
            this.Controls.Add(this.chkToggle);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Legend";
            this.Size = new System.Drawing.Size(175, 150);
            this.toolTip.SetToolTip(this, "Right-click to set another color for the selected series.");
            this.SizeChanged += new System.EventHandler(this.Legend_SizeChanged);
            this.cmnu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkToggle;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.ContextMenuStrip cmnu;
        private System.Windows.Forms.ToolStripMenuItem chooseColorToolStripMenuItem;
        private System.Windows.Forms.TreeView tvw;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
