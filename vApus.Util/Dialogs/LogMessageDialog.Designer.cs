namespace vApus.Util
{
    partial class LogMessageDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogMessageDialog));
            this.rtxt = new System.Windows.Forms.RichTextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnReportThisBug = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.llblBug = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // rtxt
            // 
            this.rtxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxt.BackColor = System.Drawing.Color.White;
            this.rtxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxt.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxt.Location = new System.Drawing.Point(12, 12);
            this.rtxt.Name = "rtxt";
            this.rtxt.ReadOnly = true;
            this.rtxt.Size = new System.Drawing.Size(560, 408);
            this.rtxt.TabIndex = 0;
            this.rtxt.Text = "";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(497, 426);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 24);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnReportThisBug
            // 
            this.btnReportThisBug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReportThisBug.BackColor = System.Drawing.Color.White;
            this.btnReportThisBug.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReportThisBug.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReportThisBug.Image = ((System.Drawing.Image)(resources.GetObject("btnReportThisBug.Image")));
            this.btnReportThisBug.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReportThisBug.Location = new System.Drawing.Point(12, 426);
            this.btnReportThisBug.Name = "btnReportThisBug";
            this.btnReportThisBug.Size = new System.Drawing.Size(123, 24);
            this.btnReportThisBug.TabIndex = 1;
            this.btnReportThisBug.Text = "Report this bug";
            this.btnReportThisBug.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnReportThisBug, "Please do not re-report bugs, bugs will not get fixed faster that way.");
            this.btnReportThisBug.UseVisualStyleBackColor = false;
            this.btnReportThisBug.Click += new System.EventHandler(this.btnReportThisBug_Click);
            // 
            // llblBug
            // 
            this.llblBug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.llblBug.AutoSize = true;
            this.llblBug.Location = new System.Drawing.Point(121, 432);
            this.llblBug.Name = "llblBug";
            this.llblBug.Size = new System.Drawing.Size(0, 13);
            this.llblBug.TabIndex = 3;
            this.llblBug.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblBug_LinkClicked);
            // 
            // LogMessageDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(584, 462);
            this.Controls.Add(this.btnReportThisBug);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.rtxt);
            this.Controls.Add(this.llblBug);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 500);
            this.Name = "LogMessageDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogMessageDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxt;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnReportThisBug;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.LinkLabel llblBug;
    }
}