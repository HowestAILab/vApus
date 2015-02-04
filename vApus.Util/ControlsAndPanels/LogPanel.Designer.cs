namespace vApus.Util {
    partial class LogPanel {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogPanel));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnReport = new System.Windows.Forms.Button();
            this.llblBug = new System.Windows.Forms.LinkLabel();
            this.fileLoggerPanel = new RandomUtils.Log.FileLoggerPanel();
            this.tmr = new System.Windows.Forms.Timer(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.btnReport);
            this.flowLayoutPanel1.Controls.Add(this.llblBug);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(113, 670);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(673, 31);
            this.flowLayoutPanel1.TabIndex = 1001;
            // 
            // btnReport
            // 
            this.btnReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReport.BackColor = System.Drawing.Color.White;
            this.btnReport.Enabled = false;
            this.btnReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReport.Image = ((System.Drawing.Image)(resources.GetObject("btnReport.Image")));
            this.btnReport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReport.Location = new System.Drawing.Point(598, 3);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(72, 24);
            this.btnReport.TabIndex = 2;
            this.btnReport.Text = "Report";
            this.btnReport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReport.UseVisualStyleBackColor = false;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // llblBug
            // 
            this.llblBug.ActiveLinkColor = System.Drawing.Color.Blue;
            this.llblBug.AutoSize = true;
            this.llblBug.DisabledLinkColor = System.Drawing.Color.Blue;
            this.llblBug.Location = new System.Drawing.Point(592, 0);
            this.llblBug.Name = "llblBug";
            this.llblBug.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.llblBug.Size = new System.Drawing.Size(0, 18);
            this.llblBug.TabIndex = 3;
            this.llblBug.VisitedLinkColor = System.Drawing.Color.Blue;
            this.llblBug.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblBug_LinkClicked);
            // 
            // fileLoggerPanel
            // 
            this.fileLoggerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileLoggerPanel.Location = new System.Drawing.Point(0, 0);
            this.fileLoggerPanel.Name = "fileLoggerPanel";
            this.fileLoggerPanel.Size = new System.Drawing.Size(795, 703);
            this.fileLoggerPanel.TabIndex = 1002;
            this.fileLoggerPanel.Text = "FileLoggerPanel";
            // 
            // tmr
            // 
            this.tmr.Interval = 200;
            this.tmr.Tick += new System.EventHandler(this.tmr_Tick);
            // 
            // LogPanel
            // 
            this.ClientSize = new System.Drawing.Size(798, 706);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.fileLoggerPanel);
            this.Name = "LogPanel";
            this.Text = "LogPanel";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.LinkLabel llblBug;
        private RandomUtils.Log.FileLoggerPanel fileLoggerPanel;
        private System.Windows.Forms.Timer tmr;

    }
}