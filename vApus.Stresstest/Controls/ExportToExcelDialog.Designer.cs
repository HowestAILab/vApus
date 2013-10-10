namespace vApus.Stresstest {
    partial class ExportToExcelDialog {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportToExcelDialog));
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.picOverview = new System.Windows.Forms.PictureBox();
            this.flpGeneral = new System.Windows.Forms.FlowLayoutPanel();
            this.picTop5HeaviestUserActions = new System.Windows.Forms.PictureBox();
            this.picAverageUserActions = new System.Windows.Forms.PictureBox();
            this.picErrors = new System.Windows.Forms.PictureBox();
            this.picUserActionComposition = new System.Windows.Forms.PictureBox();
            this.picMonitor = new System.Windows.Forms.PictureBox();
            this.picRunsOverTime = new System.Windows.Forms.PictureBox();
            this.chkMonitorDataToDifferentFiles = new System.Windows.Forms.CheckBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.chkMonitorData = new System.Windows.Forms.CheckBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlBorderStresstest = new System.Windows.Forms.Panel();
            this.cboStresstest = new System.Windows.Forms.ComboBox();
            this.flpMonitors = new System.Windows.Forms.FlowLayoutPanel();
            this.flpSpecialized = new System.Windows.Forms.FlowLayoutPanel();
            this.chkGeneral = new System.Windows.Forms.CheckBox();
            this.chkSpecialized = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picOverview)).BeginInit();
            this.flpGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTop5HeaviestUserActions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAverageUserActions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserActionComposition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonitor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRunsOverTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.pnlBorderStresstest.SuspendLayout();
            this.flpMonitors.SuspendLayout();
            this.flpSpecialized.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExportToExcel
            // 
            this.btnExportToExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportToExcel.AutoSize = true;
            this.btnExportToExcel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnExportToExcel.BackColor = System.Drawing.SystemColors.Control;
            this.btnExportToExcel.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnExportToExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportToExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportToExcel.Location = new System.Drawing.Point(653, 489);
            this.btnExportToExcel.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(117, 24);
            this.btnExportToExcel.TabIndex = 1;
            this.btnExportToExcel.Text = "Export to Excel...";
            this.btnExportToExcel.UseVisualStyleBackColor = false;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xlsx";
            this.saveFileDialog.FileName = "results";
            this.saveFileDialog.Filter = "Excel files|*.xlsx";
            // 
            // picOverview
            // 
            this.picOverview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picOverview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picOverview.Image = ((System.Drawing.Image)(resources.GetObject("picOverview.Image")));
            this.picOverview.Location = new System.Drawing.Point(6, 6);
            this.picOverview.Name = "picOverview";
            this.picOverview.Size = new System.Drawing.Size(141, 89);
            this.picOverview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picOverview.TabIndex = 6;
            this.picOverview.TabStop = false;
            this.toolTip.SetToolTip(this.picOverview, "Overview / Cumulative Response Times vs Achieved Throughput Example");
            this.picOverview.Click += new System.EventHandler(this.pic_Click);
            // 
            // flpGeneral
            // 
            this.flpGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flpGeneral.AutoScroll = true;
            this.flpGeneral.BackColor = System.Drawing.SystemColors.Control;
            this.flpGeneral.Controls.Add(this.picOverview);
            this.flpGeneral.Controls.Add(this.picTop5HeaviestUserActions);
            this.flpGeneral.Controls.Add(this.picAverageUserActions);
            this.flpGeneral.Controls.Add(this.picErrors);
            this.flpGeneral.Controls.Add(this.picUserActionComposition);
            this.flpGeneral.Location = new System.Drawing.Point(28, 83);
            this.flpGeneral.Name = "flpGeneral";
            this.flpGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.flpGeneral.Size = new System.Drawing.Size(742, 101);
            this.flpGeneral.TabIndex = 7;
            // 
            // picTop5HeaviestUserActions
            // 
            this.picTop5HeaviestUserActions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTop5HeaviestUserActions.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picTop5HeaviestUserActions.Image = ((System.Drawing.Image)(resources.GetObject("picTop5HeaviestUserActions.Image")));
            this.picTop5HeaviestUserActions.Location = new System.Drawing.Point(153, 6);
            this.picTop5HeaviestUserActions.Name = "picTop5HeaviestUserActions";
            this.picTop5HeaviestUserActions.Size = new System.Drawing.Size(141, 89);
            this.picTop5HeaviestUserActions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picTop5HeaviestUserActions.TabIndex = 7;
            this.picTop5HeaviestUserActions.TabStop = false;
            this.toolTip.SetToolTip(this.picTop5HeaviestUserActions, "Top 5 Heaviest User Actions Example");
            this.picTop5HeaviestUserActions.Click += new System.EventHandler(this.pic_Click);
            // 
            // picAverageUserActions
            // 
            this.picAverageUserActions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picAverageUserActions.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAverageUserActions.Image = ((System.Drawing.Image)(resources.GetObject("picAverageUserActions.Image")));
            this.picAverageUserActions.Location = new System.Drawing.Point(300, 6);
            this.picAverageUserActions.Name = "picAverageUserActions";
            this.picAverageUserActions.Size = new System.Drawing.Size(141, 89);
            this.picAverageUserActions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picAverageUserActions.TabIndex = 9;
            this.picAverageUserActions.TabStop = false;
            this.toolTip.SetToolTip(this.picAverageUserActions, "Average User Actions Example");
            this.picAverageUserActions.Click += new System.EventHandler(this.pic_Click);
            // 
            // picErrors
            // 
            this.picErrors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picErrors.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picErrors.Image = ((System.Drawing.Image)(resources.GetObject("picErrors.Image")));
            this.picErrors.Location = new System.Drawing.Point(447, 6);
            this.picErrors.Name = "picErrors";
            this.picErrors.Size = new System.Drawing.Size(141, 89);
            this.picErrors.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picErrors.TabIndex = 10;
            this.picErrors.TabStop = false;
            this.toolTip.SetToolTip(this.picErrors, "Errors Example");
            this.picErrors.Click += new System.EventHandler(this.pic_Click);
            // 
            // picUserActionComposition
            // 
            this.picUserActionComposition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picUserActionComposition.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picUserActionComposition.Image = ((System.Drawing.Image)(resources.GetObject("picUserActionComposition.Image")));
            this.picUserActionComposition.Location = new System.Drawing.Point(594, 6);
            this.picUserActionComposition.Name = "picUserActionComposition";
            this.picUserActionComposition.Size = new System.Drawing.Size(141, 89);
            this.picUserActionComposition.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picUserActionComposition.TabIndex = 11;
            this.picUserActionComposition.TabStop = false;
            this.toolTip.SetToolTip(this.picUserActionComposition, "User Action Composition Example");
            this.picUserActionComposition.Click += new System.EventHandler(this.pic_Click);
            // 
            // picMonitor
            // 
            this.picMonitor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picMonitor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picMonitor.Image = ((System.Drawing.Image)(resources.GetObject("picMonitor.Image")));
            this.picMonitor.Location = new System.Drawing.Point(6, 6);
            this.picMonitor.Name = "picMonitor";
            this.picMonitor.Size = new System.Drawing.Size(141, 89);
            this.picMonitor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picMonitor.TabIndex = 8;
            this.picMonitor.TabStop = false;
            this.toolTip.SetToolTip(this.picMonitor, "Monitor Example \r\nNote: since monitor values are heterogeneous charts must be mad" +
        "e manually");
            this.picMonitor.Click += new System.EventHandler(this.pic_Click);
            // 
            // picRunsOverTime
            // 
            this.picRunsOverTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picRunsOverTime.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picRunsOverTime.Image = ((System.Drawing.Image)(resources.GetObject("picRunsOverTime.Image")));
            this.picRunsOverTime.Location = new System.Drawing.Point(6, 6);
            this.picRunsOverTime.Name = "picRunsOverTime";
            this.picRunsOverTime.Size = new System.Drawing.Size(141, 89);
            this.picRunsOverTime.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picRunsOverTime.TabIndex = 13;
            this.picRunsOverTime.TabStop = false;
            this.toolTip.SetToolTip(this.picRunsOverTime, "Runs over Time Example");
            this.picRunsOverTime.Click += new System.EventHandler(this.pic_Click);
            // 
            // chkMonitorDataToDifferentFiles
            // 
            this.chkMonitorDataToDifferentFiles.Checked = true;
            this.chkMonitorDataToDifferentFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMonitorDataToDifferentFiles.ForeColor = System.Drawing.SystemColors.GrayText;
            this.chkMonitorDataToDifferentFiles.Location = new System.Drawing.Point(153, 6);
            this.chkMonitorDataToDifferentFiles.Name = "chkMonitorDataToDifferentFiles";
            this.chkMonitorDataToDifferentFiles.Size = new System.Drawing.Size(166, 89);
            this.chkMonitorDataToDifferentFiles.TabIndex = 12;
            this.chkMonitorDataToDifferentFiles.TabStop = false;
            this.chkMonitorDataToDifferentFiles.Text = "Export to different Excel file(s)";
            this.toolTip.SetToolTip(this.chkMonitorDataToDifferentFiles, resources.GetString("chkMonitorDataToDifferentFiles.ToolTip"));
            this.chkMonitorDataToDifferentFiles.UseVisualStyleBackColor = true;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(12, 13);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(503, 32);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.Text = "Charts and data will be exported to one or more Excel files.\r\nBelow you can see e" +
    "xamples of the different sheets that the document(s) will contain.";
            // 
            // chkMonitorData
            // 
            this.chkMonitorData.AutoSize = true;
            this.chkMonitorData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMonitorData.Location = new System.Drawing.Point(12, 204);
            this.chkMonitorData.Name = "chkMonitorData";
            this.chkMonitorData.Size = new System.Drawing.Size(99, 17);
            this.chkMonitorData.TabIndex = 22;
            this.chkMonitorData.TabStop = false;
            this.chkMonitorData.Text = "Monitor Data";
            this.toolTip.SetToolTip(this.chkMonitorData, "If any...");
            this.chkMonitorData.UseVisualStyleBackColor = true;
            this.chkMonitorData.CheckedChanged += new System.EventHandler(this.chkCharts_CheckedChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(15, 493);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 18;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(37, 493);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 16);
            this.label1.TabIndex = 17;
            this.label1.Text = "Select a Stresstest";
            // 
            // pnlBorderStresstest
            // 
            this.pnlBorderStresstest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBorderStresstest.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderStresstest.Controls.Add(this.cboStresstest);
            this.pnlBorderStresstest.Location = new System.Drawing.Point(162, 489);
            this.pnlBorderStresstest.Name = "pnlBorderStresstest";
            this.pnlBorderStresstest.Size = new System.Drawing.Size(430, 23);
            this.pnlBorderStresstest.TabIndex = 0;
            // 
            // cboStresstest
            // 
            this.cboStresstest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboStresstest.BackColor = System.Drawing.Color.White;
            this.cboStresstest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStresstest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboStresstest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboStresstest.FormattingEnabled = true;
            this.cboStresstest.Location = new System.Drawing.Point(1, 1);
            this.cboStresstest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboStresstest.Name = "cboStresstest";
            this.cboStresstest.Size = new System.Drawing.Size(428, 21);
            this.cboStresstest.TabIndex = 0;
            // 
            // flpMonitors
            // 
            this.flpMonitors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flpMonitors.AutoScroll = true;
            this.flpMonitors.BackColor = System.Drawing.SystemColors.Control;
            this.flpMonitors.Controls.Add(this.picMonitor);
            this.flpMonitors.Controls.Add(this.chkMonitorDataToDifferentFiles);
            this.flpMonitors.Location = new System.Drawing.Point(28, 224);
            this.flpMonitors.Name = "flpMonitors";
            this.flpMonitors.Padding = new System.Windows.Forms.Padding(3);
            this.flpMonitors.Size = new System.Drawing.Size(742, 101);
            this.flpMonitors.TabIndex = 19;
            // 
            // flpSpecialized
            // 
            this.flpSpecialized.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flpSpecialized.AutoScroll = true;
            this.flpSpecialized.BackColor = System.Drawing.SystemColors.Control;
            this.flpSpecialized.Controls.Add(this.picRunsOverTime);
            this.flpSpecialized.Location = new System.Drawing.Point(28, 363);
            this.flpSpecialized.Name = "flpSpecialized";
            this.flpSpecialized.Padding = new System.Windows.Forms.Padding(3);
            this.flpSpecialized.Size = new System.Drawing.Size(742, 101);
            this.flpSpecialized.TabIndex = 20;
            // 
            // chkGeneral
            // 
            this.chkGeneral.AutoSize = true;
            this.chkGeneral.Checked = true;
            this.chkGeneral.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkGeneral.Location = new System.Drawing.Point(15, 63);
            this.chkGeneral.Name = "chkGeneral";
            this.chkGeneral.Size = new System.Drawing.Size(70, 17);
            this.chkGeneral.TabIndex = 21;
            this.chkGeneral.TabStop = false;
            this.chkGeneral.Text = "General";
            this.chkGeneral.UseVisualStyleBackColor = true;
            this.chkGeneral.CheckedChanged += new System.EventHandler(this.chkCharts_CheckedChanged);
            // 
            // chkSpecialized
            // 
            this.chkSpecialized.AutoSize = true;
            this.chkSpecialized.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSpecialized.Location = new System.Drawing.Point(15, 343);
            this.chkSpecialized.Name = "chkSpecialized";
            this.chkSpecialized.Size = new System.Drawing.Size(91, 17);
            this.chkSpecialized.TabIndex = 23;
            this.chkSpecialized.TabStop = false;
            this.chkSpecialized.Text = "Specialized";
            this.chkSpecialized.UseVisualStyleBackColor = true;
            this.chkSpecialized.CheckedChanged += new System.EventHandler(this.chkCharts_CheckedChanged);
            // 
            // ExportToExcelDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(782, 524);
            this.Controls.Add(this.chkSpecialized);
            this.Controls.Add(this.chkMonitorData);
            this.Controls.Add(this.chkGeneral);
            this.Controls.Add(this.flpSpecialized);
            this.Controls.Add(this.flpMonitors);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlBorderStresstest);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.flpGeneral);
            this.Controls.Add(this.btnExportToExcel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportToExcelDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export to Excel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaveChartsDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.picOverview)).EndInit();
            this.flpGeneral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTop5HeaviestUserActions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAverageUserActions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserActionComposition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonitor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRunsOverTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.pnlBorderStresstest.ResumeLayout(false);
            this.flpMonitors.ResumeLayout(false);
            this.flpSpecialized.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExportToExcel;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.PictureBox picOverview;
        private System.Windows.Forms.FlowLayoutPanel flpGeneral;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picTop5HeaviestUserActions;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlBorderStresstest;
        private System.Windows.Forms.ComboBox cboStresstest;
        private System.Windows.Forms.PictureBox picMonitor;
        private System.Windows.Forms.PictureBox picAverageUserActions;
        private System.Windows.Forms.PictureBox picErrors;
        private System.Windows.Forms.PictureBox picUserActionComposition;
        private System.Windows.Forms.CheckBox chkMonitorDataToDifferentFiles;
        private System.Windows.Forms.PictureBox picRunsOverTime;
        private System.Windows.Forms.FlowLayoutPanel flpMonitors;
        private System.Windows.Forms.FlowLayoutPanel flpSpecialized;
        private System.Windows.Forms.CheckBox chkGeneral;
        private System.Windows.Forms.CheckBox chkMonitorData;
        private System.Windows.Forms.CheckBox chkSpecialized;
    }
}