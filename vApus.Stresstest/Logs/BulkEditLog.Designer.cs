namespace vApus.Stresstest
{
    partial class BulkEditLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BulkEditLog));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.flpUsedTokens = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.cboParameterScope = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.split2 = new System.Windows.Forms.SplitContainer();
            this.fastColoredTextBoxView = new FastColoredTextBoxNS.FastColoredTextBox();
            this.rtxtDescription = new System.Windows.Forms.RichTextBox();
            this.tcTools = new System.Windows.Forms.TabControl();
            this.tpApplyFilter = new System.Windows.Forms.TabPage();
            this.fastColoredTextBoxApplyFilter = new FastColoredTextBoxNS.FastColoredTextBox();
            this.rdbORWise = new System.Windows.Forms.RadioButton();
            this.rdbANDWise = new System.Windows.Forms.RadioButton();
            this.chkFilterMatchCase = new System.Windows.Forms.CheckBox();
            this.chkFilterWholeWords = new System.Windows.Forms.CheckBox();
            this.chkFilterOnUsedTokens = new System.Windows.Forms.CheckBox();
            this.tpReplace = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSwitchValues = new System.Windows.Forms.Button();
            this.chkReplaceMatchCase = new System.Windows.Forms.CheckBox();
            this.chkReplaceWholeWords = new System.Windows.Forms.CheckBox();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.btnReplaceWith = new System.Windows.Forms.Button();
            this.rdbReplaceInFiltered = new System.Windows.Forms.RadioButton();
            this.rdbReplaceInAllLogEntries = new System.Windows.Forms.RadioButton();
            this.tpErrors = new System.Windows.Forms.TabPage();
            this.tvw = new System.Windows.Forms.TreeView();
            this.btnEditSelectedLogEntry = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.split1 = new System.Windows.Forms.SplitContainer();
            this.btnUndoRedo = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.flpNotUsedTokens = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.split2)).BeginInit();
            this.split2.Panel1.SuspendLayout();
            this.split2.Panel2.SuspendLayout();
            this.split2.SuspendLayout();
            this.tcTools.SuspendLayout();
            this.tpApplyFilter.SuspendLayout();
            this.tpReplace.SuspendLayout();
            this.tpErrors.SuspendLayout();
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
            this.imageList.Images.SetKeyName(0, "Error.png");
            // 
            // flpUsedTokens
            // 
            this.flpUsedTokens.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpUsedTokens.AutoScroll = true;
            this.flpUsedTokens.BackColor = System.Drawing.Color.White;
            this.flpUsedTokens.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpUsedTokens.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.flpUsedTokens.Location = new System.Drawing.Point(9, 110);
            this.flpUsedTokens.Name = "flpUsedTokens";
            this.flpUsedTokens.Size = new System.Drawing.Size(334, 297);
            this.flpUsedTokens.TabIndex = 1;
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
            this.cboParameterScope.Size = new System.Drawing.Size(333, 21);
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
            this.label4.Size = new System.Drawing.Size(189, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "This is for every user executing this log";
            // 
            // split2
            // 
            this.split2.BackColor = System.Drawing.Color.White;
            this.split2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.split2.Location = new System.Drawing.Point(0, 0);
            this.split2.Name = "split2";
            this.split2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split2.Panel1
            // 
            this.split2.Panel1.AutoScroll = true;
            this.split2.Panel1.Controls.Add(this.fastColoredTextBoxView);
            this.split2.Panel1.Controls.Add(this.rtxtDescription);
            // 
            // split2.Panel2
            // 
            this.split2.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.split2.Panel2.Controls.Add(this.tcTools);
            this.split2.Panel2MinSize = 150;
            this.split2.Size = new System.Drawing.Size(748, 762);
            this.split2.SplitterDistance = 482;
            this.split2.TabIndex = 2;
            // 
            // fastColoredTextBoxView
            // 
            this.fastColoredTextBoxView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fastColoredTextBoxView.AutoScrollMinSize = new System.Drawing.Size(2, 15);
            this.fastColoredTextBoxView.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fastColoredTextBoxView.Location = new System.Drawing.Point(0, 50);
            this.fastColoredTextBoxView.Name = "fastColoredTextBoxView";
            this.fastColoredTextBoxView.PreferredLineWidth = 65536;
            this.fastColoredTextBoxView.ReadOnly = true;
            this.fastColoredTextBoxView.ShowLineNumbers = false;
            this.fastColoredTextBoxView.Size = new System.Drawing.Size(748, 430);
            this.fastColoredTextBoxView.TabIndex = 3;
            // 
            // rtxtDescription
            // 
            this.rtxtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtDescription.BackColor = System.Drawing.Color.White;
            this.rtxtDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtDescription.ForeColor = System.Drawing.Color.Gray;
            this.rtxtDescription.Location = new System.Drawing.Point(4, 3);
            this.rtxtDescription.Name = "rtxtDescription";
            this.rtxtDescription.ReadOnly = true;
            this.rtxtDescription.Size = new System.Drawing.Size(740, 44);
            this.rtxtDescription.TabIndex = 2;
            this.rtxtDescription.Text = "";
            // 
            // tcTools
            // 
            this.tcTools.Controls.Add(this.tpApplyFilter);
            this.tcTools.Controls.Add(this.tpReplace);
            this.tcTools.Controls.Add(this.tpErrors);
            this.tcTools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcTools.ImageList = this.imageList;
            this.tcTools.Location = new System.Drawing.Point(0, 0);
            this.tcTools.Name = "tcTools";
            this.tcTools.SelectedIndex = 0;
            this.tcTools.Size = new System.Drawing.Size(748, 276);
            this.tcTools.TabIndex = 4;
            this.tcTools.SelectedIndexChanged += new System.EventHandler(this.tcTools_SelectedIndexChanged);
            // 
            // tpApplyFilter
            // 
            this.tpApplyFilter.Controls.Add(this.fastColoredTextBoxApplyFilter);
            this.tpApplyFilter.Controls.Add(this.rdbORWise);
            this.tpApplyFilter.Controls.Add(this.rdbANDWise);
            this.tpApplyFilter.Controls.Add(this.chkFilterMatchCase);
            this.tpApplyFilter.Controls.Add(this.chkFilterWholeWords);
            this.tpApplyFilter.Controls.Add(this.chkFilterOnUsedTokens);
            this.tpApplyFilter.Location = new System.Drawing.Point(4, 23);
            this.tpApplyFilter.Name = "tpApplyFilter";
            this.tpApplyFilter.Padding = new System.Windows.Forms.Padding(3);
            this.tpApplyFilter.Size = new System.Drawing.Size(740, 249);
            this.tpApplyFilter.TabIndex = 6;
            this.tpApplyFilter.Text = "Apply Filter";
            this.tpApplyFilter.UseVisualStyleBackColor = true;
            // 
            // fastColoredTextBoxApplyFilter
            // 
            this.fastColoredTextBoxApplyFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fastColoredTextBoxApplyFilter.AutoScrollMinSize = new System.Drawing.Size(2, 15);
            this.fastColoredTextBoxApplyFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fastColoredTextBoxApplyFilter.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fastColoredTextBoxApplyFilter.Location = new System.Drawing.Point(3, 3);
            this.fastColoredTextBoxApplyFilter.Name = "fastColoredTextBoxApplyFilter";
            this.fastColoredTextBoxApplyFilter.PreferredLineWidth = 65536;
            this.fastColoredTextBoxApplyFilter.ShowLineNumbers = false;
            this.fastColoredTextBoxApplyFilter.Size = new System.Drawing.Size(734, 216);
            this.fastColoredTextBoxApplyFilter.TabIndex = 6;
            this.fastColoredTextBoxApplyFilter.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fastColoredTextBoxApplyFilter_TextChangedDelayed);
            // 
            // rdbORWise
            // 
            this.rdbORWise.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.rdbORWise.AutoSize = true;
            this.rdbORWise.Location = new System.Drawing.Point(333, 224);
            this.rdbORWise.Name = "rdbORWise";
            this.rdbORWise.Size = new System.Drawing.Size(68, 17);
            this.rdbORWise.TabIndex = 5;
            this.rdbORWise.Text = "OR-Wise";
            this.rdbORWise.UseVisualStyleBackColor = true;
            // 
            // rdbANDWise
            // 
            this.rdbANDWise.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.rdbANDWise.AutoSize = true;
            this.rdbANDWise.Checked = true;
            this.rdbANDWise.Location = new System.Drawing.Point(252, 224);
            this.rdbANDWise.Name = "rdbANDWise";
            this.rdbANDWise.Size = new System.Drawing.Size(75, 17);
            this.rdbANDWise.TabIndex = 4;
            this.rdbANDWise.TabStop = true;
            this.rdbANDWise.Text = "AND-Wise";
            this.rdbANDWise.UseVisualStyleBackColor = true;
            this.rdbANDWise.CheckedChanged += new System.EventHandler(this.rdbANDWise_CheckedChanged);
            // 
            // chkFilterMatchCase
            // 
            this.chkFilterMatchCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkFilterMatchCase.AutoSize = true;
            this.chkFilterMatchCase.Location = new System.Drawing.Point(651, 225);
            this.chkFilterMatchCase.Name = "chkFilterMatchCase";
            this.chkFilterMatchCase.Size = new System.Drawing.Size(83, 17);
            this.chkFilterMatchCase.TabIndex = 3;
            this.chkFilterMatchCase.Text = "Match Case";
            this.chkFilterMatchCase.UseVisualStyleBackColor = true;
            this.chkFilterMatchCase.CheckedChanged += new System.EventHandler(this.chkFilterMatchCase_CheckedChanged);
            // 
            // chkFilterWholeWords
            // 
            this.chkFilterWholeWords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkFilterWholeWords.AutoSize = true;
            this.chkFilterWholeWords.Location = new System.Drawing.Point(554, 225);
            this.chkFilterWholeWords.Name = "chkFilterWholeWords";
            this.chkFilterWholeWords.Size = new System.Drawing.Size(91, 17);
            this.chkFilterWholeWords.TabIndex = 2;
            this.chkFilterWholeWords.Text = "Whole Words";
            this.chkFilterWholeWords.UseVisualStyleBackColor = true;
            this.chkFilterWholeWords.CheckedChanged += new System.EventHandler(this.chkFilterWholeWords_CheckedChanged);
            // 
            // chkFilterOnUsedTokens
            // 
            this.chkFilterOnUsedTokens.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkFilterOnUsedTokens.AutoSize = true;
            this.chkFilterOnUsedTokens.Location = new System.Drawing.Point(8, 225);
            this.chkFilterOnUsedTokens.Name = "chkFilterOnUsedTokens";
            this.chkFilterOnUsedTokens.Size = new System.Drawing.Size(90, 17);
            this.chkFilterOnUsedTokens.TabIndex = 1;
            this.chkFilterOnUsedTokens.Text = "Used Tokens";
            this.chkFilterOnUsedTokens.UseVisualStyleBackColor = true;
            this.chkFilterOnUsedTokens.CheckedChanged += new System.EventHandler(this.chkFilterOnUsedTokens_CheckedChanged);
            // 
            // tpReplace
            // 
            this.tpReplace.Controls.Add(this.label2);
            this.tpReplace.Controls.Add(this.btnSwitchValues);
            this.tpReplace.Controls.Add(this.chkReplaceMatchCase);
            this.tpReplace.Controls.Add(this.chkReplaceWholeWords);
            this.tpReplace.Controls.Add(this.txtReplace);
            this.tpReplace.Controls.Add(this.txtFind);
            this.tpReplace.Controls.Add(this.btnReplaceWith);
            this.tpReplace.Controls.Add(this.rdbReplaceInFiltered);
            this.tpReplace.Controls.Add(this.rdbReplaceInAllLogEntries);
            this.tpReplace.Location = new System.Drawing.Point(4, 23);
            this.tpReplace.Name = "tpReplace";
            this.tpReplace.Padding = new System.Windows.Forms.Padding(3);
            this.tpReplace.Size = new System.Drawing.Size(740, 249);
            this.tpReplace.TabIndex = 5;
            this.tpReplace.Text = "Replace";
            this.tpReplace.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Find:";
            // 
            // btnSwitchValues
            // 
            this.btnSwitchValues.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSwitchValues.AutoSize = true;
            this.btnSwitchValues.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSwitchValues.BackColor = System.Drawing.SystemColors.Control;
            this.btnSwitchValues.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSwitchValues.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSwitchValues.Location = new System.Drawing.Point(411, 130);
            this.btnSwitchValues.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnSwitchValues.Name = "btnSwitchValues";
            this.btnSwitchValues.Size = new System.Drawing.Size(99, 24);
            this.btnSwitchValues.TabIndex = 20;
            this.btnSwitchValues.Text = "Switch Values";
            this.btnSwitchValues.UseVisualStyleBackColor = false;
            this.btnSwitchValues.Click += new System.EventHandler(this.btnSwitchValues_Click);
            // 
            // chkReplaceMatchCase
            // 
            this.chkReplaceMatchCase.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkReplaceMatchCase.AutoSize = true;
            this.chkReplaceMatchCase.Location = new System.Drawing.Point(505, 102);
            this.chkReplaceMatchCase.Name = "chkReplaceMatchCase";
            this.chkReplaceMatchCase.Size = new System.Drawing.Size(83, 17);
            this.chkReplaceMatchCase.TabIndex = 17;
            this.chkReplaceMatchCase.Text = "Match Case";
            this.chkReplaceMatchCase.UseVisualStyleBackColor = true;
            // 
            // chkReplaceWholeWords
            // 
            this.chkReplaceWholeWords.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkReplaceWholeWords.AutoSize = true;
            this.chkReplaceWholeWords.Location = new System.Drawing.Point(411, 102);
            this.chkReplaceWholeWords.Name = "chkReplaceWholeWords";
            this.chkReplaceWholeWords.Size = new System.Drawing.Size(91, 17);
            this.chkReplaceWholeWords.TabIndex = 16;
            this.chkReplaceWholeWords.Text = "Whole Words";
            this.chkReplaceWholeWords.UseVisualStyleBackColor = true;
            // 
            // txtReplace
            // 
            this.txtReplace.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtReplace.HideSelection = false;
            this.txtReplace.Location = new System.Drawing.Point(195, 132);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(210, 20);
            this.txtReplace.TabIndex = 19;
            // 
            // txtFind
            // 
            this.txtFind.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtFind.HideSelection = false;
            this.txtFind.Location = new System.Drawing.Point(87, 100);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(318, 20);
            this.txtFind.TabIndex = 15;
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            // 
            // btnReplaceWith
            // 
            this.btnReplaceWith.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReplaceWith.AutoSize = true;
            this.btnReplaceWith.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnReplaceWith.BackColor = System.Drawing.SystemColors.Control;
            this.btnReplaceWith.Enabled = false;
            this.btnReplaceWith.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReplaceWith.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReplaceWith.Location = new System.Drawing.Point(54, 130);
            this.btnReplaceWith.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnReplaceWith.Name = "btnReplaceWith";
            this.btnReplaceWith.Size = new System.Drawing.Size(135, 24);
            this.btnReplaceWith.TabIndex = 18;
            this.btnReplaceWith.Text = "Replace Found With";
            this.btnReplaceWith.UseVisualStyleBackColor = false;
            this.btnReplaceWith.Click += new System.EventHandler(this.btnReplaceWith_Click);
            // 
            // rdbReplaceInFiltered
            // 
            this.rdbReplaceInFiltered.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdbReplaceInFiltered.AutoSize = true;
            this.rdbReplaceInFiltered.Location = new System.Drawing.Point(118, 224);
            this.rdbReplaceInFiltered.Name = "rdbReplaceInFiltered";
            this.rdbReplaceInFiltered.Size = new System.Drawing.Size(95, 17);
            this.rdbReplaceInFiltered.TabIndex = 13;
            this.rdbReplaceInFiltered.Text = "Only In Filtered";
            this.rdbReplaceInFiltered.UseVisualStyleBackColor = true;
            // 
            // rdbReplaceInAllLogEntries
            // 
            this.rdbReplaceInAllLogEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdbReplaceInAllLogEntries.AutoSize = true;
            this.rdbReplaceInAllLogEntries.Checked = true;
            this.rdbReplaceInAllLogEntries.Location = new System.Drawing.Point(8, 224);
            this.rdbReplaceInAllLogEntries.Name = "rdbReplaceInAllLogEntries";
            this.rdbReplaceInAllLogEntries.Size = new System.Drawing.Size(104, 17);
            this.rdbReplaceInAllLogEntries.TabIndex = 12;
            this.rdbReplaceInAllLogEntries.TabStop = true;
            this.rdbReplaceInAllLogEntries.Text = "In All Log Entries";
            this.rdbReplaceInAllLogEntries.UseVisualStyleBackColor = true;
            // 
            // tpErrors
            // 
            this.tpErrors.Controls.Add(this.tvw);
            this.tpErrors.Controls.Add(this.btnEditSelectedLogEntry);
            this.tpErrors.ImageIndex = 0;
            this.tpErrors.Location = new System.Drawing.Point(4, 23);
            this.tpErrors.Name = "tpErrors";
            this.tpErrors.Padding = new System.Windows.Forms.Padding(3);
            this.tpErrors.Size = new System.Drawing.Size(740, 249);
            this.tpErrors.TabIndex = 1;
            this.tpErrors.Text = "Errors";
            this.tpErrors.UseVisualStyleBackColor = true;
            // 
            // tvw
            // 
            this.tvw.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvw.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvw.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.tvw.HideSelection = false;
            this.tvw.Location = new System.Drawing.Point(6, 6);
            this.tvw.Name = "tvw";
            this.tvw.Size = new System.Drawing.Size(727, 209);
            this.tvw.TabIndex = 5;
            // 
            // btnEditSelectedLogEntry
            // 
            this.btnEditSelectedLogEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditSelectedLogEntry.AutoSize = true;
            this.btnEditSelectedLogEntry.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnEditSelectedLogEntry.BackColor = System.Drawing.SystemColors.Control;
            this.btnEditSelectedLogEntry.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditSelectedLogEntry.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditSelectedLogEntry.Location = new System.Drawing.Point(568, 220);
            this.btnEditSelectedLogEntry.MaximumSize = new System.Drawing.Size(165, 24);
            this.btnEditSelectedLogEntry.Name = "btnEditSelectedLogEntry";
            this.btnEditSelectedLogEntry.Size = new System.Drawing.Size(165, 24);
            this.btnEditSelectedLogEntry.TabIndex = 8;
            this.btnEditSelectedLogEntry.Text = "Edit Selected Log Entry...";
            this.btnEditSelectedLogEntry.UseVisualStyleBackColor = false;
            this.btnEditSelectedLogEntry.Click += new System.EventHandler(this.btnEditSelectedLogEntry_Click);
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
            this.split1.Panel1.Controls.Add(this.split2);
            // 
            // split1.Panel2
            // 
            this.split1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.split1.Panel2.Controls.Add(this.btnUndoRedo);
            this.split1.Panel2.Controls.Add(this.btnCancel);
            this.split1.Panel2.Controls.Add(this.btnOK);
            this.split1.Panel2.Controls.Add(this.label1);
            this.split1.Panel2.Controls.Add(this.flpNotUsedTokens);
            this.split1.Panel2.Controls.Add(this.flpUsedTokens);
            this.split1.Panel2.Controls.Add(this.label5);
            this.split1.Panel2.Controls.Add(this.label4);
            this.split1.Panel2.Controls.Add(this.cboParameterScope);
            this.split1.Panel2.Controls.Add(this.label3);
            this.split1.Size = new System.Drawing.Size(1106, 762);
            this.split1.SplitterDistance = 748;
            this.split1.TabIndex = 9;
            // 
            // btnUndoRedo
            // 
            this.btnUndoRedo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUndoRedo.AutoSize = true;
            this.btnUndoRedo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUndoRedo.BackColor = System.Drawing.Color.White;
            this.btnUndoRedo.Enabled = false;
            this.btnUndoRedo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUndoRedo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnUndoRedo.Location = new System.Drawing.Point(9, 729);
            this.btnUndoRedo.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnUndoRedo.Name = "btnUndoRedo";
            this.btnUndoRedo.Size = new System.Drawing.Size(49, 24);
            this.btnUndoRedo.TabIndex = 2;
            this.btnUndoRedo.Text = "Undo";
            this.btnUndoRedo.UseVisualStyleBackColor = false;
            this.btnUndoRedo.Click += new System.EventHandler(this.btnUndoRedo_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Location = new System.Drawing.Point(284, 729);
            this.btnCancel.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(58, 24);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.AutoSize = true;
            this.btnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.Enabled = false;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnOK.Location = new System.Drawing.Point(241, 729);
            this.btnOK.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(36, 24);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // flpNotUsedTokens
            // 
            this.flpNotUsedTokens.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpNotUsedTokens.AutoScroll = true;
            this.flpNotUsedTokens.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.flpNotUsedTokens.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpNotUsedTokens.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.flpNotUsedTokens.Location = new System.Drawing.Point(9, 450);
            this.flpNotUsedTokens.Name = "flpNotUsedTokens";
            this.flpNotUsedTokens.Size = new System.Drawing.Size(333, 272);
            this.flpNotUsedTokens.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 427);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Not used in the log:";
            // 
            // BulkEditLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1106, 762);
            this.Controls.Add(this.split1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "BulkEditLog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bulk Edit Log";
            this.split2.Panel1.ResumeLayout(false);
            this.split2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split2)).EndInit();
            this.split2.ResumeLayout(false);
            this.tcTools.ResumeLayout(false);
            this.tpApplyFilter.ResumeLayout(false);
            this.tpApplyFilter.PerformLayout();
            this.tpReplace.ResumeLayout(false);
            this.tpReplace.PerformLayout();
            this.tpErrors.ResumeLayout(false);
            this.tpErrors.PerformLayout();
            this.split1.Panel1.ResumeLayout(false);
            this.split1.Panel2.ResumeLayout(false);
            this.split1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split1)).EndInit();
            this.split1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.SplitContainer split2;
        private System.Windows.Forms.FlowLayoutPanel flpUsedTokens;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboParameterScope;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.SplitContainer split1;
        private System.Windows.Forms.TabControl tcTools;
        private System.Windows.Forms.TabPage tpReplace;
        private System.Windows.Forms.TabPage tpErrors;
        private System.Windows.Forms.TabPage tpApplyFilter;
        private System.Windows.Forms.RichTextBox rtxtDescription;
        private System.Windows.Forms.RadioButton rdbReplaceInFiltered;
        private System.Windows.Forms.RadioButton rdbReplaceInAllLogEntries;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSwitchValues;
        private System.Windows.Forms.CheckBox chkReplaceMatchCase;
        private System.Windows.Forms.CheckBox chkReplaceWholeWords;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Button btnReplaceWith;
        private System.Windows.Forms.FlowLayoutPanel flpNotUsedTokens;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkFilterOnUsedTokens;
        private System.Windows.Forms.CheckBox chkFilterMatchCase;
        private System.Windows.Forms.CheckBox chkFilterWholeWords;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnUndoRedo;
        private System.Windows.Forms.Button btnEditSelectedLogEntry;
        private System.Windows.Forms.TreeView tvw;
        private System.Windows.Forms.RadioButton rdbORWise;
        private System.Windows.Forms.RadioButton rdbANDWise;
        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBoxView;
        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBoxApplyFilter;
    }
}