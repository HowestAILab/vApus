namespace vApus.Stresstest
{
    partial class AddEditLogEntry
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddEditLogEntry));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.chkShowLabels = new System.Windows.Forms.CheckBox();
            this.flpTokens = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.cboParameterScope = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkVisualizeWhitespace = new System.Windows.Forms.CheckBox();
            this.split2 = new System.Windows.Forms.SplitContainer();
            this.tvwValidation = new System.Windows.Forms.TreeView();
            this.tc = new System.Windows.Forms.TabControl();
            this.tpEdit = new System.Windows.Forms.TabPage();
            this.fastColoredTextBoxEdit = new FastColoredTextBoxNS.FastColoredTextBox();
            this.tpLogEntryAsImported = new System.Windows.Forms.TabPage();
            this.fastColoredTextBoxAsImported = new FastColoredTextBoxNS.FastColoredTextBox();
            this.tpError = new System.Windows.Forms.TabPage();
            this.rtxtError = new System.Windows.Forms.RichTextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.split1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.split2)).BeginInit();
            this.split2.Panel1.SuspendLayout();
            this.split2.Panel2.SuspendLayout();
            this.split2.SuspendLayout();
            this.tc.SuspendLayout();
            this.tpEdit.SuspendLayout();
            this.tpLogEntryAsImported.SuspendLayout();
            this.tpError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split1)).BeginInit();
            this.split1.Panel1.SuspendLayout();
            this.split1.Panel2.SuspendLayout();
            this.split1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "OK.png");
            this.imageList.Images.SetKeyName(1, "Error.png");
            // 
            // chkShowLabels
            // 
            this.chkShowLabels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowLabels.AutoSize = true;
            this.chkShowLabels.Checked = true;
            this.chkShowLabels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowLabels.Location = new System.Drawing.Point(7, 701);
            this.chkShowLabels.Name = "chkShowLabels";
            this.chkShowLabels.Size = new System.Drawing.Size(87, 17);
            this.chkShowLabels.TabIndex = 1;
            this.chkShowLabels.Text = "Show Labels";
            this.chkShowLabels.UseVisualStyleBackColor = true;
            this.chkShowLabels.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // flpTokens
            // 
            this.flpTokens.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpTokens.AutoScroll = true;
            this.flpTokens.BackColor = System.Drawing.Color.White;
            this.flpTokens.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpTokens.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.flpTokens.Location = new System.Drawing.Point(9, 110);
            this.flpTokens.Name = "flpTokens";
            this.flpTokens.Size = new System.Drawing.Size(334, 577);
            this.flpTokens.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Parameter Tokens:";
            // 
            // cboParameterScope
            // 
            this.cboParameterScope.BackColor = System.Drawing.Color.White;
            this.cboParameterScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboParameterScope.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboParameterScope.FormattingEnabled = true;
            this.cboParameterScope.Items.AddRange(new object[] {
            "<All>",
            "once in the log",
            "once in the parent user action",
            "once in this log entry",
            "once in the leaf node",
            "for every call"});
            this.cboParameterScope.Location = new System.Drawing.Point(9, 53);
            this.cboParameterScope.Name = "cboParameterScope";
            this.cboParameterScope.Size = new System.Drawing.Size(225, 21);
            this.cboParameterScope.TabIndex = 0;
            this.cboParameterScope.SelectedIndexChanged += new System.EventHandler(this.cboParameterScope_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(271, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Show tokens for parameter values that are redetermined";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(192, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "This is for every user executing this log.";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(187, 697);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(268, 697);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkVisualizeWhitespace
            // 
            this.chkVisualizeWhitespace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkVisualizeWhitespace.AutoSize = true;
            this.chkVisualizeWhitespace.Location = new System.Drawing.Point(516, 701);
            this.chkVisualizeWhitespace.Name = "chkVisualizeWhitespace";
            this.chkVisualizeWhitespace.Size = new System.Drawing.Size(127, 17);
            this.chkVisualizeWhitespace.TabIndex = 7;
            this.chkVisualizeWhitespace.Text = "Visualize Whitespace";
            this.chkVisualizeWhitespace.UseVisualStyleBackColor = true;
            this.chkVisualizeWhitespace.CheckedChanged += new System.EventHandler(this.chkVisualizeWhitespace_CheckedChanged);
            // 
            // split2
            // 
            this.split2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split2.BackColor = System.Drawing.Color.White;
            this.split2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.split2.Location = new System.Drawing.Point(3, 3);
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
            this.split2.Size = new System.Drawing.Size(644, 684);
            this.split2.SplitterDistance = 527;
            this.split2.TabIndex = 2;
            // 
            // tvwValidation
            // 
            this.tvwValidation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwValidation.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.tvwValidation.HideSelection = false;
            this.tvwValidation.ImageIndex = 0;
            this.tvwValidation.ImageList = this.imageList;
            this.tvwValidation.Location = new System.Drawing.Point(0, 0);
            this.tvwValidation.Name = "tvwValidation";
            this.tvwValidation.SelectedImageIndex = 0;
            this.tvwValidation.Size = new System.Drawing.Size(644, 527);
            this.tvwValidation.TabIndex = 0;
            this.tvwValidation.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwValidation_AfterSelect);
            // 
            // tc
            // 
            this.tc.Controls.Add(this.tpEdit);
            this.tc.Controls.Add(this.tpLogEntryAsImported);
            this.tc.Controls.Add(this.tpError);
            this.tc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tc.ImageList = this.imageList;
            this.tc.Location = new System.Drawing.Point(0, 0);
            this.tc.Name = "tc";
            this.tc.SelectedIndex = 0;
            this.tc.Size = new System.Drawing.Size(644, 153);
            this.tc.TabIndex = 0;
            // 
            // tpEdit
            // 
            this.tpEdit.Controls.Add(this.fastColoredTextBoxEdit);
            this.tpEdit.Location = new System.Drawing.Point(4, 23);
            this.tpEdit.Name = "tpEdit";
            this.tpEdit.Padding = new System.Windows.Forms.Padding(3);
            this.tpEdit.Size = new System.Drawing.Size(636, 126);
            this.tpEdit.TabIndex = 1;
            this.tpEdit.Text = "Edit";
            this.tpEdit.UseVisualStyleBackColor = true;
            // 
            // fastColoredTextBoxEdit
            // 
            this.fastColoredTextBoxEdit.AllowDrop = true;
            this.fastColoredTextBoxEdit.AutoScrollMinSize = new System.Drawing.Size(0, 14);
            this.fastColoredTextBoxEdit.BackBrush = null;
            this.fastColoredTextBoxEdit.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fastColoredTextBoxEdit.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fastColoredTextBoxEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastColoredTextBoxEdit.IsReplaceMode = false;
            this.fastColoredTextBoxEdit.Location = new System.Drawing.Point(3, 3);
            this.fastColoredTextBoxEdit.Name = "fastColoredTextBoxEdit";
            this.fastColoredTextBoxEdit.Paddings = new System.Windows.Forms.Padding(0);
            this.fastColoredTextBoxEdit.PreferredLineWidth = 65536;
            this.fastColoredTextBoxEdit.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fastColoredTextBoxEdit.ShowLineNumbers = false;
            this.fastColoredTextBoxEdit.Size = new System.Drawing.Size(630, 120);
            this.fastColoredTextBoxEdit.TabIndex = 0;
            this.fastColoredTextBoxEdit.WordWrap = true;
            this.fastColoredTextBoxEdit.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fastColoredTextBoxEdit_TextChangedDelayed);
            // 
            // tpLogEntryAsImported
            // 
            this.tpLogEntryAsImported.Controls.Add(this.fastColoredTextBoxAsImported);
            this.tpLogEntryAsImported.Location = new System.Drawing.Point(4, 23);
            this.tpLogEntryAsImported.Name = "tpLogEntryAsImported";
            this.tpLogEntryAsImported.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogEntryAsImported.Size = new System.Drawing.Size(636, 126);
            this.tpLogEntryAsImported.TabIndex = 2;
            this.tpLogEntryAsImported.Text = "Log Entry as Imported";
            this.tpLogEntryAsImported.UseVisualStyleBackColor = true;
            // 
            // fastColoredTextBoxAsImported
            // 
            this.fastColoredTextBoxAsImported.AllowDrop = true;
            this.fastColoredTextBoxAsImported.AutoScrollMinSize = new System.Drawing.Size(0, 15);
            this.fastColoredTextBoxAsImported.BackBrush = null;
            this.fastColoredTextBoxAsImported.BackColor = System.Drawing.SystemColors.Info;
            this.fastColoredTextBoxAsImported.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fastColoredTextBoxAsImported.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fastColoredTextBoxAsImported.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastColoredTextBoxAsImported.IsReplaceMode = false;
            this.fastColoredTextBoxAsImported.Location = new System.Drawing.Point(3, 3);
            this.fastColoredTextBoxAsImported.Name = "fastColoredTextBoxAsImported";
            this.fastColoredTextBoxAsImported.Paddings = new System.Windows.Forms.Padding(0);
            this.fastColoredTextBoxAsImported.PreferredLineWidth = 65536;
            this.fastColoredTextBoxAsImported.ReadOnly = true;
            this.fastColoredTextBoxAsImported.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fastColoredTextBoxAsImported.ShowLineNumbers = false;
            this.fastColoredTextBoxAsImported.Size = new System.Drawing.Size(630, 120);
            this.fastColoredTextBoxAsImported.TabIndex = 1;
            this.fastColoredTextBoxAsImported.WordWrap = true;
            // 
            // tpError
            // 
            this.tpError.Controls.Add(this.rtxtError);
            this.tpError.ImageIndex = 1;
            this.tpError.Location = new System.Drawing.Point(4, 23);
            this.tpError.Name = "tpError";
            this.tpError.Padding = new System.Windows.Forms.Padding(3);
            this.tpError.Size = new System.Drawing.Size(636, 126);
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
            this.rtxtError.Size = new System.Drawing.Size(630, 120);
            this.rtxtError.TabIndex = 1;
            this.rtxtError.Text = "";
            // 
            // split1
            // 
            this.split1.BackColor = System.Drawing.Color.White;
            this.split1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.split1.Location = new System.Drawing.Point(0, 0);
            this.split1.Name = "split1";
            // 
            // split1.Panel1
            // 
            this.split1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.split1.Panel1.Controls.Add(this.chkVisualizeWhitespace);
            this.split1.Panel1.Controls.Add(this.chkShowLabels);
            this.split1.Panel1.Controls.Add(this.split2);
            // 
            // split1.Panel2
            // 
            this.split1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.split1.Panel2.Controls.Add(this.label1);
            this.split1.Panel2.Controls.Add(this.flpTokens);
            this.split1.Panel2.Controls.Add(this.btnCancel);
            this.split1.Panel2.Controls.Add(this.btnOK);
            this.split1.Panel2.Controls.Add(this.label4);
            this.split1.Panel2.Controls.Add(this.cboParameterScope);
            this.split1.Panel2.Controls.Add(this.label3);
            this.split1.Size = new System.Drawing.Size(1008, 730);
            this.split1.SplitterDistance = 650;
            this.split1.TabIndex = 9;
            // 
            // AddEditLogEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.split1);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "AddEditLogEntry";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddEditLogEntry";
            this.split2.Panel1.ResumeLayout(false);
            this.split2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split2)).EndInit();
            this.split2.ResumeLayout(false);
            this.tc.ResumeLayout(false);
            this.tpEdit.ResumeLayout(false);
            this.tpLogEntryAsImported.ResumeLayout(false);
            this.tpError.ResumeLayout(false);
            this.split1.Panel1.ResumeLayout(false);
            this.split1.Panel1.PerformLayout();
            this.split1.Panel2.ResumeLayout(false);
            this.split1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split1)).EndInit();
            this.split1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvwValidation;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.SplitContainer split2;
        private System.Windows.Forms.CheckBox chkShowLabels;
        private System.Windows.Forms.TabControl tc;
        private System.Windows.Forms.TabPage tpError;
        private System.Windows.Forms.RichTextBox rtxtError;
        private System.Windows.Forms.TabPage tpEdit;
        private System.Windows.Forms.FlowLayoutPanel flpTokens;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboParameterScope;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TabPage tpLogEntryAsImported;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkVisualizeWhitespace;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.SplitContainer split1;
        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBoxEdit;
        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBoxAsImported;
    }
}