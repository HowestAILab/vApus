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
            this.toolStripEdit = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.lblCount = new System.Windows.Forms.ToolStripLabel();
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
            this.logSolutionComponentPropertyPanel.Enabled = false;
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
            // toolStripEdit
            // 
            this.toolStripEdit.AutoSize = false;
            this.toolStripEdit.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.lblCount});
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
        private vApus.Util.LargeList largelist;
        private System.Windows.Forms.ToolStripButton btnImportLogFiles;
        private System.Windows.Forms.CheckBox chk;
        private System.Windows.Forms.ToolStripLabel lblCount;
        private System.Windows.Forms.ToolStripButton btnBulkEditLog;
        private System.Windows.Forms.Button btnCollapseExpand;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripButton btnRecord;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton btnExportToTextFile;
        private System.Windows.Forms.ToolStripButton btnRedetermineTokens;

    }
}