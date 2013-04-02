namespace vApus.Stresstest {
    partial class NewLogView {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewLogView));
            this.split = new System.Windows.Forms.SplitContainer();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.picFilter = new System.Windows.Forms.PictureBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnRecord = new System.Windows.Forms.ToolStripButton();
            this.btnImportLogFiles = new System.Windows.Forms.ToolStripButton();
            this.btnRedetermineTokens = new System.Windows.Forms.ToolStripButton();
            this.btnBulkEditLog = new System.Windows.Forms.ToolStripButton();
            this.logTreeView = new vApus.Stresstest.LogTreeView();
            this.editUserAction = new vApus.Stresstest.EditUserAction();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFilter)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // split
            // 
            this.split.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split.Location = new System.Drawing.Point(0, 43);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.pnlFilter);
            this.split.Panel1.Controls.Add(this.logTreeView);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.editUserAction);
            this.split.Size = new System.Drawing.Size(1046, 552);
            this.split.SplitterDistance = 348;
            this.split.TabIndex = 0;
            // 
            // pnlFilter
            // 
            this.pnlFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFilter.Controls.Add(this.picFilter);
            this.pnlFilter.Controls.Add(this.txtFilter);
            this.pnlFilter.Location = new System.Drawing.Point(0, 521);
            this.pnlFilter.MinimumSize = new System.Drawing.Size(227, 21);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(348, 21);
            this.pnlFilter.TabIndex = 2;
            // 
            // picFilter
            // 
            this.picFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picFilter.Image = global::vApus.Stresstest.Properties.Resources.find;
            this.picFilter.Location = new System.Drawing.Point(328, 1);
            this.picFilter.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.picFilter.Name = "picFilter";
            this.picFilter.Size = new System.Drawing.Size(20, 20);
            this.picFilter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFilter.TabIndex = 8;
            this.picFilter.TabStop = false;
            this.toolTip.SetToolTip(this.picFilter, "Type comma to split filter entries and \'return\' to submit. Wild card * can be use" +
        "d. Not case sensitive.");
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.HideSelection = false;
            this.txtFilter.Location = new System.Drawing.Point(3, 1);
            this.txtFilter.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.txtFilter.MinimumSize = new System.Drawing.Size(100, 4);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(345, 20);
            this.txtFilter.TabIndex = 0;
            this.txtFilter.TabStop = false;
            this.toolTip.SetToolTip(this.txtFilter, "Type comma to split filter entries and \'return\' to submit. Wild card * can be use" +
        "d. Not case sensitive.");
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 20;
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRecord,
            this.btnImportLogFiles,
            this.btnRedetermineTokens,
            this.btnBulkEditLog});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.MinimumSize = new System.Drawing.Size(0, 40);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1046, 40);
            this.toolStrip.TabIndex = 26;
            // 
            // btnRecord
            // 
            this.btnRecord.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecord.Image = ((System.Drawing.Image)(resources.GetObject("btnRecord.Image")));
            this.btnRecord.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(111, 37);
            this.btnRecord.Text = "Record";
            this.btnRecord.ToolTipText = "Record HTTP(S)";
            // 
            // btnImportLogFiles
            // 
            this.btnImportLogFiles.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportLogFiles.Image = ((System.Drawing.Image)(resources.GetObject("btnImportLogFiles.Image")));
            this.btnImportLogFiles.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnImportLogFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportLogFiles.Name = "btnImportLogFiles";
            this.btnImportLogFiles.Size = new System.Drawing.Size(110, 37);
            this.btnImportLogFiles.Text = "Import";
            this.btnImportLogFiles.ToolTipText = "Import Log Files...";
            // 
            // btnRedetermineTokens
            // 
            this.btnRedetermineTokens.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnRedetermineTokens.Image = global::vApus.Stresstest.Properties.Resources.RedetermineTokens1;
            this.btnRedetermineTokens.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedetermineTokens.Name = "btnRedetermineTokens";
            this.btnRedetermineTokens.Size = new System.Drawing.Size(23, 37);
            this.btnRedetermineTokens.ToolTipText = resources.GetString("btnRedetermineTokens.ToolTipText");
            // 
            // btnBulkEditLog
            // 
            this.btnBulkEditLog.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnBulkEditLog.Image = global::vApus.Stresstest.Properties.Resources.Log;
            this.btnBulkEditLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBulkEditLog.Name = "btnBulkEditLog";
            this.btnBulkEditLog.Size = new System.Drawing.Size(23, 37);
            this.btnBulkEditLog.ToolTipText = "Bulk Edit Log...";
            // 
            // logTreeView
            // 
            this.logTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTreeView.Location = new System.Drawing.Point(0, 0);
            this.logTreeView.Name = "logTreeView";
            this.logTreeView.Size = new System.Drawing.Size(348, 523);
            this.logTreeView.TabIndex = 0;
            // 
            // editUserAction
            // 
            this.editUserAction.BackColor = System.Drawing.Color.White;
            this.editUserAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editUserAction.Location = new System.Drawing.Point(0, 0);
            this.editUserAction.Name = "editUserAction";
            this.editUserAction.Size = new System.Drawing.Size(694, 552);
            this.editUserAction.TabIndex = 0;
            // 
            // NewLogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1046, 595);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.split);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "NewLogView";
            this.Text = "NewLogView";
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFilter)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer split;
        private LogTreeView logTreeView;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.PictureBox picFilter;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnImportLogFiles;
        private System.Windows.Forms.ToolStripButton btnRecord;
        private System.Windows.Forms.ToolStripButton btnRedetermineTokens;
        private System.Windows.Forms.ToolStripButton btnBulkEditLog;
        private EditUserAction editUserAction;
    }
}