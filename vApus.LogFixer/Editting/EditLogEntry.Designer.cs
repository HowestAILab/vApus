namespace vApus.LogFixer
{
    partial class EditLogEntry
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
            this.split2 = new System.Windows.Forms.SplitContainer();
            this.tvwValidation = new System.Windows.Forms.TreeView();
            this.tc = new System.Windows.Forms.TabControl();
            this.tpEdit = new System.Windows.Forms.TabPage();
            this.fastColoredTextBoxEdit = new FastColoredTextBoxNS.FastColoredTextBox();
            this.tpLogEntryAsImported = new System.Windows.Forms.TabPage();
            this.fastColoredTextBoxAsImported = new FastColoredTextBoxNS.FastColoredTextBox();
            this.tpError = new System.Windows.Forms.TabPage();
            this.rtxtError = new System.Windows.Forms.RichTextBox();
            this.chkVisualizeWhitespace = new System.Windows.Forms.CheckBox();
            this.chkShowNamesAndIndices = new System.Windows.Forms.CheckBox();
            this.chkShowLabels = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.split2)).BeginInit();
            this.split2.Panel1.SuspendLayout();
            this.split2.Panel2.SuspendLayout();
            this.split2.SuspendLayout();
            this.tc.SuspendLayout();
            this.tpEdit.SuspendLayout();
            this.tpLogEntryAsImported.SuspendLayout();
            this.tpError.SuspendLayout();
            this.SuspendLayout();
            // 
            // split2
            // 
            this.split2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split2.BackColor = System.Drawing.Color.White;
            this.split2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.split2.Location = new System.Drawing.Point(0, 0);
            this.split2.Name = "split2";
            this.split2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split2.Panel1
            // 
            this.split2.Panel1.Controls.Add(this.tvwValidation);
            // 
            // split2.Panel2
            // 
            this.split2.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.split2.Panel2.Controls.Add(this.tc);
            this.split2.Panel2MinSize = 150;
            this.split2.Size = new System.Drawing.Size(784, 524);
            this.split2.SplitterDistance = 370;
            this.split2.TabIndex = 0;
            // 
            // tvwValidation
            // 
            this.tvwValidation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwValidation.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.tvwValidation.HideSelection = false;
            this.tvwValidation.Location = new System.Drawing.Point(0, 0);
            this.tvwValidation.Name = "tvwValidation";
            this.tvwValidation.Size = new System.Drawing.Size(784, 370);
            this.tvwValidation.TabIndex = 0;
            // 
            // tc
            // 
            this.tc.Controls.Add(this.tpEdit);
            this.tc.Controls.Add(this.tpLogEntryAsImported);
            this.tc.Controls.Add(this.tpError);
            this.tc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tc.Location = new System.Drawing.Point(0, 0);
            this.tc.Name = "tc";
            this.tc.SelectedIndex = 0;
            this.tc.Size = new System.Drawing.Size(784, 150);
            this.tc.TabIndex = 0;
            // 
            // tpEdit
            // 
            this.tpEdit.Controls.Add(this.fastColoredTextBoxEdit);
            this.tpEdit.Location = new System.Drawing.Point(4, 22);
            this.tpEdit.Name = "tpEdit";
            this.tpEdit.Padding = new System.Windows.Forms.Padding(3);
            this.tpEdit.Size = new System.Drawing.Size(776, 124);
            this.tpEdit.TabIndex = 1;
            this.tpEdit.Text = "Edit";
            this.tpEdit.UseVisualStyleBackColor = true;
            // 
            // fastColoredTextBoxEdit
            // 
            this.fastColoredTextBoxEdit.AutoScrollMinSize = new System.Drawing.Size(0, 15);
            this.fastColoredTextBoxEdit.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fastColoredTextBoxEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastColoredTextBoxEdit.Location = new System.Drawing.Point(3, 3);
            this.fastColoredTextBoxEdit.Name = "fastColoredTextBoxEdit";
            this.fastColoredTextBoxEdit.PreferredLineWidth = 65536;
            this.fastColoredTextBoxEdit.ShowLineNumbers = false;
            this.fastColoredTextBoxEdit.Size = new System.Drawing.Size(770, 118);
            this.fastColoredTextBoxEdit.TabIndex = 1;
            this.fastColoredTextBoxEdit.WordWrap = true;
            this.fastColoredTextBoxEdit.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fastColoredTextBoxEdit_TextChangedDelayed);
            // 
            // tpLogEntryAsImported
            // 
            this.tpLogEntryAsImported.Controls.Add(this.fastColoredTextBoxAsImported);
            this.tpLogEntryAsImported.Location = new System.Drawing.Point(4, 22);
            this.tpLogEntryAsImported.Name = "tpLogEntryAsImported";
            this.tpLogEntryAsImported.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogEntryAsImported.Size = new System.Drawing.Size(776, 124);
            this.tpLogEntryAsImported.TabIndex = 2;
            this.tpLogEntryAsImported.Text = "Log Entry as Imported";
            this.tpLogEntryAsImported.UseVisualStyleBackColor = true;
            // 
            // fastColoredTextBoxAsImported
            // 
            this.fastColoredTextBoxAsImported.AutoScrollMinSize = new System.Drawing.Size(0, 15);
            this.fastColoredTextBoxAsImported.BackColor = System.Drawing.SystemColors.Info;
            this.fastColoredTextBoxAsImported.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fastColoredTextBoxAsImported.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastColoredTextBoxAsImported.Location = new System.Drawing.Point(3, 3);
            this.fastColoredTextBoxAsImported.Name = "fastColoredTextBoxAsImported";
            this.fastColoredTextBoxAsImported.PreferredLineWidth = 65536;
            this.fastColoredTextBoxAsImported.ReadOnly = true;
            this.fastColoredTextBoxAsImported.ShowLineNumbers = false;
            this.fastColoredTextBoxAsImported.Size = new System.Drawing.Size(770, 118);
            this.fastColoredTextBoxAsImported.TabIndex = 2;
            this.fastColoredTextBoxAsImported.WordWrap = true;
            // 
            // tpError
            // 
            this.tpError.Controls.Add(this.rtxtError);
            this.tpError.ImageIndex = 1;
            this.tpError.Location = new System.Drawing.Point(4, 22);
            this.tpError.Name = "tpError";
            this.tpError.Padding = new System.Windows.Forms.Padding(3);
            this.tpError.Size = new System.Drawing.Size(776, 124);
            this.tpError.TabIndex = 0;
            this.tpError.Text = "Error";
            this.tpError.UseVisualStyleBackColor = true;
            // 
            // rtxtError
            // 
            this.rtxtError.BackColor = System.Drawing.SystemColors.Info;
            this.rtxtError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtError.ForeColor = System.Drawing.Color.Red;
            this.rtxtError.Location = new System.Drawing.Point(3, 3);
            this.rtxtError.Name = "rtxtError";
            this.rtxtError.ReadOnly = true;
            this.rtxtError.Size = new System.Drawing.Size(770, 118);
            this.rtxtError.TabIndex = 0;
            this.rtxtError.Text = "";
            // 
            // chkVisualizeWhitespace
            // 
            this.chkVisualizeWhitespace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkVisualizeWhitespace.AutoSize = true;
            this.chkVisualizeWhitespace.Location = new System.Drawing.Point(250, 533);
            this.chkVisualizeWhitespace.Name = "chkVisualizeWhitespace";
            this.chkVisualizeWhitespace.Size = new System.Drawing.Size(127, 17);
            this.chkVisualizeWhitespace.TabIndex = 3;
            this.chkVisualizeWhitespace.Text = "Visualize Whitespace";
            this.chkVisualizeWhitespace.UseVisualStyleBackColor = true;
            this.chkVisualizeWhitespace.CheckedChanged += new System.EventHandler(this.chkVisualizeWhitespace_CheckedChanged);
            // 
            // chkShowNamesAndIndices
            // 
            this.chkShowNamesAndIndices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowNamesAndIndices.AutoSize = true;
            this.chkShowNamesAndIndices.Location = new System.Drawing.Point(97, 533);
            this.chkShowNamesAndIndices.Name = "chkShowNamesAndIndices";
            this.chkShowNamesAndIndices.Size = new System.Drawing.Size(147, 17);
            this.chkShowNamesAndIndices.TabIndex = 2;
            this.chkShowNamesAndIndices.Text = "Show Names and Indices";
            this.chkShowNamesAndIndices.UseVisualStyleBackColor = true;
            this.chkShowNamesAndIndices.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // chkShowLabels
            // 
            this.chkShowLabels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowLabels.AutoSize = true;
            this.chkShowLabels.Checked = true;
            this.chkShowLabels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowLabels.Location = new System.Drawing.Point(4, 533);
            this.chkShowLabels.Name = "chkShowLabels";
            this.chkShowLabels.Size = new System.Drawing.Size(87, 17);
            this.chkShowLabels.TabIndex = 1;
            this.chkShowLabels.Text = "Show Labels";
            this.chkShowLabels.UseVisualStyleBackColor = true;
            this.chkShowLabels.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(705, 529);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(624, 529);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // EditLogEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkVisualizeWhitespace);
            this.Controls.Add(this.chkShowNamesAndIndices);
            this.Controls.Add(this.chkShowLabels);
            this.Controls.Add(this.split2);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "EditLogEntry";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit";
            this.split2.Panel1.ResumeLayout(false);
            this.split2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split2)).EndInit();
            this.split2.ResumeLayout(false);
            this.tc.ResumeLayout(false);
            this.tpEdit.ResumeLayout(false);
            this.tpLogEntryAsImported.ResumeLayout(false);
            this.tpError.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer split2;
        private System.Windows.Forms.TreeView tvwValidation;
        private System.Windows.Forms.TabControl tc;
        private System.Windows.Forms.TabPage tpEdit;
        private System.Windows.Forms.TabPage tpLogEntryAsImported;
        private System.Windows.Forms.TabPage tpError;
        private System.Windows.Forms.RichTextBox rtxtError;
        private System.Windows.Forms.CheckBox chkVisualizeWhitespace;
        private System.Windows.Forms.CheckBox chkShowNamesAndIndices;
        private System.Windows.Forms.CheckBox chkShowLabels;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBoxEdit;
        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBoxAsImported;


    }
}