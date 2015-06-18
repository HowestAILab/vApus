namespace vApus.DetailedResultsViewer {
    partial class MySQLServerDialog {
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
            this.savingResultsPanel = new vApus.Results.SavingResultsPanel();
            this.SuspendLayout();
            // 
            // savingResultsPanel
            // 
            this.savingResultsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.savingResultsPanel.Location = new System.Drawing.Point(0, 0);
            this.savingResultsPanel.Name = "savingResultsPanel";
            this.savingResultsPanel.ShowDescription = false;
            this.savingResultsPanel.ShowLocalHostWarning = false;
            this.savingResultsPanel.Size = new System.Drawing.Size(484, 262);
            this.savingResultsPanel.TabIndex = 0;
            this.savingResultsPanel.Text = "SavingResultsPanel";
            // 
            // MySQLServerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 262);
            this.Controls.Add(this.savingResultsPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MySQLServerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect to a results MySQL server";
            this.ResumeLayout(false);

        }

        #endregion

        private vApus.Results.SavingResultsPanel savingResultsPanel;

    }
}