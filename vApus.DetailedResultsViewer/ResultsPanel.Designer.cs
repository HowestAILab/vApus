namespace vApus.DetailedResultsViewer {
    partial class ResultsPanel {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultsPanel));
            this.detailedResultsControl = new vApus.StressTest.DetailedResultsControl();
            this.SuspendLayout();
            // 
            // detailedResultsControl
            // 
            this.detailedResultsControl.BackColor = System.Drawing.Color.White;
            this.detailedResultsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailedResultsControl.Location = new System.Drawing.Point(0, 0);
            this.detailedResultsControl.Name = "detailedResultsControl";
            this.detailedResultsControl.Size = new System.Drawing.Size(619, 413);
            this.detailedResultsControl.TabIndex = 2;
            this.detailedResultsControl.ResultsDeleted += new System.EventHandler(this.detailedResultsControl_ResultsDeleted);
            // 
            // ResultsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 413);
            this.Controls.Add(this.detailedResultsControl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ResultsPanel";
            this.Text = "Results";
            this.ResumeLayout(false);

        }

        #endregion

        private StressTest.DetailedResultsControl detailedResultsControl;

    }
}