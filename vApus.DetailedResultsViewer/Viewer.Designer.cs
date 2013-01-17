namespace vApus.DetailedResultsViewer {
    partial class Viewer {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Viewer));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mySQLServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailedResultsControl = new vApus.Stresstest.Controls.DetailedResultsControl();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mySQLServerToolStripMenuItem,
            this.databaseToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1034, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // mySQLServerToolStripMenuItem
            // 
            this.mySQLServerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mySQLServerToolStripMenuItem.Image")));
            this.mySQLServerToolStripMenuItem.Name = "mySQLServerToolStripMenuItem";
            this.mySQLServerToolStripMenuItem.Size = new System.Drawing.Size(198, 20);
            this.mySQLServerToolStripMenuItem.Text = "My SQL Server Not Connected!";
            this.mySQLServerToolStripMenuItem.Click += new System.EventHandler(this.mySQLServerToolStripMenuItem_Click);
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("databaseToolStripMenuItem.Image")));
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(149, 20);
            this.databaseToolStripMenuItem.Text = "No Database Selected";
            // 
            // detailedResultsControl
            // 
            this.detailedResultsControl.BackColor = System.Drawing.Color.White;
            this.detailedResultsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailedResultsControl.Location = new System.Drawing.Point(0, 24);
            this.detailedResultsControl.Name = "detailedResultsControl";
            this.detailedResultsControl.Size = new System.Drawing.Size(1034, 738);
            this.detailedResultsControl.TabIndex = 0;
            // 
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 762);
            this.Controls.Add(this.detailedResultsControl);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Viewer";
            this.Text = "vApus Detailed Results Viewer";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private vApus.Stresstest.Controls.DetailedResultsControl detailedResultsControl;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem mySQLServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databaseToolStripMenuItem;
    }
}

