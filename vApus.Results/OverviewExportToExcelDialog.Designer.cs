namespace vApus.Results {
    partial class OverviewExportToExcelDialog {
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
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.chkIncludeFullMonitorResults = new System.Windows.Forms.CheckBox();
            this.btnRichExport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(12, 9);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(491, 16);
            this.lblDescription.TabIndex = 9;
            this.lblDescription.Text = "Export test and monitor results per concurrency for the selected results database" +
    "s.";
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
            this.btnExportToExcel.Location = new System.Drawing.Point(405, 121);
            this.btnExportToExcel.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(117, 24);
            this.btnExportToExcel.TabIndex = 20;
            this.btnExportToExcel.Text = "Export to Excel...";
            this.btnExportToExcel.UseVisualStyleBackColor = false;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Excel file|*.xlsx";
            this.saveFileDialog.Title = "Export test and monitor results per concurrency for the selected results database" +
    "s.";
            // 
            // chkIncludeFullMonitorResults
            // 
            this.chkIncludeFullMonitorResults.AutoSize = true;
            this.chkIncludeFullMonitorResults.Checked = true;
            this.chkIncludeFullMonitorResults.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIncludeFullMonitorResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.chkIncludeFullMonitorResults.Location = new System.Drawing.Point(25, 59);
            this.chkIncludeFullMonitorResults.Name = "chkIncludeFullMonitorResults";
            this.chkIncludeFullMonitorResults.Size = new System.Drawing.Size(300, 20);
            this.chkIncludeFullMonitorResults.TabIndex = 11;
            this.chkIncludeFullMonitorResults.Text = "Include full monitor results in extra work sheets.";
            this.chkIncludeFullMonitorResults.UseVisualStyleBackColor = true;
            // 
            // btnRichExport
            // 
            this.btnRichExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRichExport.AutoSize = true;
            this.btnRichExport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRichExport.BackColor = System.Drawing.SystemColors.Control;
            this.btnRichExport.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnRichExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRichExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRichExport.Location = new System.Drawing.Point(12, 121);
            this.btnRichExport.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnRichExport.Name = "btnRichExport";
            this.btnRichExport.Size = new System.Drawing.Size(148, 24);
            this.btnRichExport.TabIndex = 12;
            this.btnRichExport.Text = "Switch to rich export...";
            this.btnRichExport.UseVisualStyleBackColor = false;
            this.btnRichExport.Click += new System.EventHandler(this.btnRichExport_Click);
            // 
            // OverviewExportToExcelDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(534, 157);
            this.Controls.Add(this.btnRichExport);
            this.Controls.Add(this.chkIncludeFullMonitorResults);
            this.Controls.Add(this.btnExportToExcel);
            this.Controls.Add(this.lblDescription);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OverviewExportToExcelDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Overview export to excel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OverviewExportToExcelDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnExportToExcel;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.CheckBox chkIncludeFullMonitorResults;
        private System.Windows.Forms.Button btnRichExport;
    }
}