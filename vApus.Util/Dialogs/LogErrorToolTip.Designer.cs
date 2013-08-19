namespace vApus.Util {
    partial class LogErrorToolTip {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogErrorToolTip));
            this.pic = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblText = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.SuspendLayout();
            // 
            // pic
            // 
            this.pic.Image = ((System.Drawing.Image)(resources.GetObject("pic.Image")));
            this.pic.Location = new System.Drawing.Point(12, 12);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(24, 24);
            this.pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic.TabIndex = 0;
            this.pic.TabStop = false;
            this.pic.Click += new System.EventHandler(this.LogErrorToolTip_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(42, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(32, 13);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Title";
            this.lblTitle.Click += new System.EventHandler(this.LogErrorToolTip_Click);
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(42, 26);
            this.lblText.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(97, 15);
            this.lblText.TabIndex = 2;
            this.lblText.Text = "Click for details...";
            this.lblText.Click += new System.EventHandler(this.LogErrorToolTip_Click);
            // 
            // timer
            // 
            this.timer.Interval = 20000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // LogErrorToolTip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(240, 50);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pic);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LogErrorToolTip";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "LogToolTip";
            this.VisibleChanged += new System.EventHandler(this.LogErrorToolTip_VisibleChanged);
            this.Click += new System.EventHandler(this.LogErrorToolTip_Click);
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pic;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Timer timer;
    }
}