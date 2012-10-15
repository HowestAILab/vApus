namespace vApus.Stresstest
{
    partial class LogView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogView));
            this.toolStripImport = new System.Windows.Forms.ToolStrip();
            this.btnImportLogFiles = new System.Windows.Forms.ToolStripButton();
            this.btnExportToTextFile = new System.Windows.Forms.ToolStripButton();
            this.btnRecord = new System.Windows.Forms.ToolStripButton();
            this.btnRedetermineTokens = new System.Windows.Forms.ToolStripButton();
            this.btnBulkEditLog = new System.Windows.Forms.ToolStripButton();
            this.lblCurrentConcurrent = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.logSolutionComponentPropertyPanel = new vApus.SolutionTree.SolutionComponentPropertyPanel();
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.chk = new System.Windows.Forms.CheckBox();
            this.errorAndFindSelector = new vApus.Stresstest.ErrorAndFindSelector();
            this.toolStripEdit = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.lblCount = new System.Windows.Forms.ToolStripLabel();
            this.btnLevelDown = new System.Windows.Forms.ToolStripButton();
            this.btnLevelUp = new System.Windows.Forms.ToolStripButton();
            this.btnUp = new System.Windows.Forms.ToolStripButton();
            this.btnDown = new System.Windows.Forms.ToolStripButton();
            this.separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddLogEntry = new System.Windows.Forms.ToolStripButton();
            this.btnRemove = new System.Windows.Forms.ToolStripButton();
            this.separator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnActionizeUnactionize = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.btnPaste = new System.Windows.Forms.ToolStripButton();
            this.largelist = new vApus.Util.LargeList();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripImport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolStripEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripImport
            // 
            this.toolStripImport.Enabled = false;
            this.toolStripImport.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripImport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImportLogFiles,
            this.btnExportToTextFile,
            this.btnRecord,
            this.btnRedetermineTokens,
            this.btnBulkEditLog});
            this.toolStripImport.Location = new System.Drawing.Point(0, 0);
            this.toolStripImport.Name = "toolStripImport";
            this.toolStripImport.Size = new System.Drawing.Size(941, 25);
            this.toolStripImport.TabIndex = 25;
            // 
            // btnImportLogFiles
            // 
            this.btnImportLogFiles.Image = ((System.Drawing.Image)(resources.GetObject("btnImportLogFiles.Image")));
            this.btnImportLogFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportLogFiles.Name = "btnImportLogFiles";
            this.btnImportLogFiles.Size = new System.Drawing.Size(121, 22);
            this.btnImportLogFiles.Text = "Import Log Files...";
            this.btnImportLogFiles.Click += new System.EventHandler(this.btnImportLogFiles_Click);
            // 
            // btnExportToTextFile
            // 
            this.btnExportToTextFile.Image = ((System.Drawing.Image)(resources.GetObject("btnExportToTextFile.Image")));
            this.btnExportToTextFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportToTextFile.Name = "btnExportToTextFile";
            this.btnExportToTextFile.Size = new System.Drawing.Size(129, 22);
            this.btnExportToTextFile.Text = "Export to Text File...";
            this.btnExportToTextFile.Click += new System.EventHandler(this.btnExportToTextFile_Click);
            // 
            // btnRecord
            // 
            this.btnRecord.Image = ((System.Drawing.Image)(resources.GetObject("btnRecord.Image")));
            this.btnRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(97, 22);
            this.btnRecord.Text = "Record HTTP";
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btnRedetermineTokens
            // 
            this.btnRedetermineTokens.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnRedetermineTokens.Image = global::vApus.Stresstest.Properties.Resources.RedetermineTokens1;
            this.btnRedetermineTokens.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedetermineTokens.Name = "btnRedetermineTokens";
            this.btnRedetermineTokens.Size = new System.Drawing.Size(144, 22);
            this.btnRedetermineTokens.Text = "Redetermine Tokens...";
            this.btnRedetermineTokens.ToolTipText = resources.GetString("btnRedetermineTokens.ToolTipText");
            this.btnRedetermineTokens.Click += new System.EventHandler(this.btnRedetermineTokens_Click);
            // 
            // btnBulkEditLog
            // 
            this.btnBulkEditLog.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnBulkEditLog.Image = global::vApus.Stresstest.Properties.Resources.Log;
            this.btnBulkEditLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBulkEditLog.Name = "btnBulkEditLog";
            this.btnBulkEditLog.Size = new System.Drawing.Size(105, 22);
            this.btnBulkEditLog.Text = "Bulk Edit Log...";
            this.btnBulkEditLog.Click += new System.EventHandler(this.btnBulkEditLog_Click);
            // 
            // lblCurrentConcurrent
            // 
            this.lblCurrentConcurrent.AutoSize = true;
            this.lblCurrentConcurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentConcurrent.Location = new System.Drawing.Point(102, 52);
            this.lblCurrentConcurrent.Name = "lblCurrentConcurrent";
            this.lblCurrentConcurrent.Size = new System.Drawing.Size(0, 13);
            this.lblCurrentConcurrent.TabIndex = 27;
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 28);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.logSolutionComponentPropertyPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer.Panel2.Controls.Add(this.btnCollapseExpand);
            this.splitContainer.Panel2.Controls.Add(this.chk);
            this.splitContainer.Panel2.Controls.Add(this.errorAndFindSelector);
            this.splitContainer.Panel2.Controls.Add(this.toolStripEdit);
            this.splitContainer.Panel2.Controls.Add(this.largelist);
            this.splitContainer.Size = new System.Drawing.Size(941, 434);
            this.splitContainer.SplitterDistance = 115;
            this.splitContainer.TabIndex = 26;
            // 
            // logSolutionComponentPropertyPanel
            // 
            this.logSolutionComponentPropertyPanel.BackColor = System.Drawing.Color.White;
            this.logSolutionComponentPropertyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logSolutionComponentPropertyPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.logSolutionComponentPropertyPanel.Location = new System.Drawing.Point(0, 0);
            this.logSolutionComponentPropertyPanel.Name = "logSolutionComponentPropertyPanel";
            this.logSolutionComponentPropertyPanel.Size = new System.Drawing.Size(941, 115);
            this.logSolutionComponentPropertyPanel.SolutionComponent = null;
            this.logSolutionComponentPropertyPanel.TabIndex = 2;
            // 
            // btnCollapseExpand
            // 
            this.btnCollapseExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseExpand.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btnCollapseExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpand.Location = new System.Drawing.Point(889, 0);
            this.btnCollapseExpand.Name = "btnCollapseExpand";
            this.btnCollapseExpand.Size = new System.Drawing.Size(28, 25);
            this.btnCollapseExpand.TabIndex = 28;
            this.btnCollapseExpand.TabStop = false;
            this.btnCollapseExpand.Text = "-";
            this.toolTip.SetToolTip(this.btnCollapseExpand, "Collapse or expand all user actions.");
            this.btnCollapseExpand.UseVisualStyleBackColor = false;
            this.btnCollapseExpand.Click += new System.EventHandler(this.btnCollapseExpand_Click);
            // 
            // chk
            // 
            this.chk.AutoSize = true;
            this.chk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chk.Location = new System.Drawing.Point(11, 5);
            this.chk.Name = "chk";
            this.chk.Size = new System.Drawing.Size(12, 11);
            this.chk.TabIndex = 0;
            this.chk.UseVisualStyleBackColor = true;
            this.chk.CheckStateChanged += new System.EventHandler(this.chk_CheckStateChanged);
            // 
            // errorAndFindSelector
            // 
            this.errorAndFindSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errorAndFindSelector.Found = null;
            this.errorAndFindSelector.Location = new System.Drawing.Point(330, 0);
            this.errorAndFindSelector.Name = "errorAndFindSelector";
            this.errorAndFindSelector.Size = new System.Drawing.Size(553, 25);
            this.errorAndFindSelector.TabIndex = 4;
            this.errorAndFindSelector.SelectError += new System.EventHandler<vApus.Stresstest.SelectErrorEventArgs>(this.errorAndFindSelector_SelectError);
            this.errorAndFindSelector.Find += new System.EventHandler<vApus.Stresstest.FindEventArgs>(this.errorAndFindSelector_Find);
            // 
            // toolStripEdit
            // 
            this.toolStripEdit.AutoSize = false;
            this.toolStripEdit.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.lblCount,
            this.btnLevelDown,
            this.btnLevelUp,
            this.btnUp,
            this.btnDown,
            this.separator2,
            this.btnAddLogEntry,
            this.btnRemove,
            this.separator3,
            this.btnActionizeUnactionize,
            this.toolStripSeparator1,
            this.btnCopy,
            this.btnPaste});
            this.toolStripEdit.Location = new System.Drawing.Point(0, 0);
            this.toolStripEdit.Name = "toolStripEdit";
            this.toolStripEdit.Size = new System.Drawing.Size(941, 25);
            this.toolStripEdit.TabIndex = 3;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(28, 22);
            this.toolStripLabel1.Text = "       ";
            // 
            // lblCount
            // 
            this.lblCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(0, 22);
            // 
            // btnLevelDown
            // 
            this.btnLevelDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLevelDown.Enabled = false;
            this.btnLevelDown.Image = ((System.Drawing.Image)(resources.GetObject("btnLevelDown.Image")));
            this.btnLevelDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLevelDown.Name = "btnLevelDown";
            this.btnLevelDown.Size = new System.Drawing.Size(23, 22);
            this.btnLevelDown.Text = "Level Down";
            this.btnLevelDown.ToolTipText = "Level Down";
            this.btnLevelDown.Click += new System.EventHandler(this.btnLevelDown_Click);
            // 
            // btnLevelUp
            // 
            this.btnLevelUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLevelUp.Enabled = false;
            this.btnLevelUp.Image = ((System.Drawing.Image)(resources.GetObject("btnLevelUp.Image")));
            this.btnLevelUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLevelUp.Name = "btnLevelUp";
            this.btnLevelUp.Size = new System.Drawing.Size(23, 22);
            this.btnLevelUp.Text = "Level Up";
            this.btnLevelUp.ToolTipText = "Level Up";
            this.btnLevelUp.Click += new System.EventHandler(this.btnLevelUp_Click);
            // 
            // btnUp
            // 
            this.btnUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUp.Enabled = false;
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(23, 22);
            this.btnUp.Text = "Move Up";
            this.btnUp.ToolTipText = "Move Up";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDown.Enabled = false;
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(23, 22);
            this.btnDown.Text = "Move Down";
            this.btnDown.ToolTipText = "Move Down";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // separator2
            // 
            this.separator2.Name = "separator2";
            this.separator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAddLogEntry
            // 
            this.btnAddLogEntry.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddLogEntry.Image = ((System.Drawing.Image)(resources.GetObject("btnAddLogEntry.Image")));
            this.btnAddLogEntry.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddLogEntry.Name = "btnAddLogEntry";
            this.btnAddLogEntry.Size = new System.Drawing.Size(23, 22);
            this.btnAddLogEntry.Text = "Add Log Entry";
            this.btnAddLogEntry.ToolTipText = "Add Log Entry";
            this.btnAddLogEntry.Click += new System.EventHandler(this.btnAddLogEntry_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemove.Enabled = false;
            this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
            this.btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(23, 22);
            this.btnRemove.Text = "Remove";
            this.btnRemove.ToolTipText = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // separator3
            // 
            this.separator3.Name = "separator3";
            this.separator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnActionizeUnactionize
            // 
            this.btnActionizeUnactionize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnActionizeUnactionize.Enabled = false;
            this.btnActionizeUnactionize.Image = global::vApus.Stresstest.Properties.Resources.Actionize;
            this.btnActionizeUnactionize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnActionizeUnactionize.Name = "btnActionizeUnactionize";
            this.btnActionizeUnactionize.Size = new System.Drawing.Size(23, 22);
            this.btnActionizeUnactionize.Text = "Actionize Selected Log Entries";
            this.btnActionizeUnactionize.Click += new System.EventHandler(this.btnActionizeUnactionize_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnCopy
            // 
            this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCopy.Enabled = false;
            this.btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnCopy.Image")));
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(23, 22);
            this.btnCopy.Text = "Copy checked items";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPaste.Image = ((System.Drawing.Image)(resources.GetObject("btnPaste.Image")));
            this.btnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(23, 22);
            this.btnPaste.Text = "Paste at the end";
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // largelist
            // 
            this.largelist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.largelist.Location = new System.Drawing.Point(-1, 26);
            this.largelist.Name = "largelist";
            this.largelist.Size = new System.Drawing.Size(943, 290);
            this.largelist.SizeMode = vApus.Util.SizeMode.StretchHorizontal;
            this.largelist.TabIndex = 2;
            // 
            // LogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 462);
            this.Controls.Add(this.toolStripImport);
            this.Controls.Add(this.lblCurrentConcurrent);
            this.Controls.Add(this.splitContainer);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "LogView";
            this.Text = "LogView";
            this.toolStripImport.ResumeLayout(false);
            this.toolStripImport.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.toolStripEdit.ResumeLayout(false);
            this.toolStripEdit.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripImport;
        private System.Windows.Forms.Label lblCurrentConcurrent;
        private System.Windows.Forms.SplitContainer splitContainer;
        private vApus.SolutionTree.SolutionComponentPropertyPanel logSolutionComponentPropertyPanel;
        private System.Windows.Forms.ToolStrip toolStripEdit;
        private System.Windows.Forms.ToolStripButton btnLevelDown;
        private System.Windows.Forms.ToolStripButton btnLevelUp;
        private System.Windows.Forms.ToolStripButton btnUp;
        private System.Windows.Forms.ToolStripButton btnDown;
        private System.Windows.Forms.ToolStripSeparator separator2;
        private System.Windows.Forms.ToolStripButton btnAddLogEntry;
        private System.Windows.Forms.ToolStripButton btnRemove;
        private System.Windows.Forms.ToolStripSeparator separator3;
        private System.Windows.Forms.ToolStripButton btnActionizeUnactionize;
        private vApus.Util.LargeList largelist;
        private System.Windows.Forms.ToolStripButton btnImportLogFiles;
        private ErrorAndFindSelector errorAndFindSelector;
        private System.Windows.Forms.CheckBox chk;
        private System.Windows.Forms.ToolStripLabel lblCount;
        private System.Windows.Forms.ToolStripButton btnBulkEditLog;
        private System.Windows.Forms.Button btnCollapseExpand;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripButton btnRecord;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton btnExportToTextFile;
        private System.Windows.Forms.ToolStripButton btnRedetermineTokens;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripButton btnPaste;

    }
}