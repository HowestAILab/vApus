namespace vApus.Gui
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.pnl = new System.Windows.Forms.Panel();
            this.txtChannel = new System.Windows.Forms.TextBox();
            this.lblWebsite = new System.Windows.Forms.LinkLabel();
            this.tc = new System.Windows.Forms.TabControl();
            this.tbpAuthors = new System.Windows.Forms.TabPage();
            this.authorGrid = new vApus.Gui.AuthorGrid();
            this.tbpHistory = new System.Windows.Forms.TabPage();
            this.rtxtHistory = new System.Windows.Forms.RichTextBox();
            this.tbpLicenses = new System.Windows.Forms.TabPage();
            this.rtxtLicenses = new System.Windows.Forms.RichTextBox();
            this.txtCopyright = new System.Windows.Forms.TextBox();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.pnl.SuspendLayout();
            this.tc.SuspendLayout();
            this.tbpAuthors.SuspendLayout();
            this.tbpHistory.SuspendLayout();
            this.tbpLicenses.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl
            // 
            this.pnl.BackColor = System.Drawing.Color.White;
            this.pnl.Controls.Add(this.txtChannel);
            this.pnl.Controls.Add(this.lblWebsite);
            this.pnl.Controls.Add(this.tc);
            this.pnl.Controls.Add(this.txtCopyright);
            this.pnl.Controls.Add(this.txtVersion);
            this.pnl.Controls.Add(this.lblDescription);
            this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl.Location = new System.Drawing.Point(0, 0);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(628, 441);
            this.pnl.TabIndex = 4;
            // 
            // txtChannel
            // 
            this.txtChannel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChannel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtChannel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChannel.ForeColor = System.Drawing.Color.SteelBlue;
            this.txtChannel.Location = new System.Drawing.Point(31, 56);
            this.txtChannel.Name = "txtChannel";
            this.txtChannel.Size = new System.Drawing.Size(577, 13);
            this.txtChannel.TabIndex = 13;
            this.txtChannel.TabStop = false;
            this.txtChannel.Text = "Channel";
            // 
            // lblWebsite
            // 
            this.lblWebsite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWebsite.Location = new System.Drawing.Point(29, 93);
            this.lblWebsite.Name = "lblWebsite";
            this.lblWebsite.Size = new System.Drawing.Size(580, 18);
            this.lblWebsite.TabIndex = 12;
            this.lblWebsite.TabStop = true;
            this.lblWebsite.Text = "http://www.sizingservers.be";
            this.lblWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblWebsite_LinkClicked);
            // 
            // tc
            // 
            this.tc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tc.Controls.Add(this.tbpAuthors);
            this.tc.Controls.Add(this.tbpHistory);
            this.tc.Controls.Add(this.tbpLicenses);
            this.tc.Location = new System.Drawing.Point(20, 111);
            this.tc.Name = "tc";
            this.tc.SelectedIndex = 0;
            this.tc.Size = new System.Drawing.Size(589, 317);
            this.tc.TabIndex = 10;
            // 
            // tbpAuthors
            // 
            this.tbpAuthors.Controls.Add(this.authorGrid);
            this.tbpAuthors.Location = new System.Drawing.Point(4, 22);
            this.tbpAuthors.Name = "tbpAuthors";
            this.tbpAuthors.Padding = new System.Windows.Forms.Padding(3);
            this.tbpAuthors.Size = new System.Drawing.Size(581, 291);
            this.tbpAuthors.TabIndex = 0;
            this.tbpAuthors.Text = "Authors";
            this.tbpAuthors.UseVisualStyleBackColor = true;
            // 
            // authorGrid
            // 
            this.authorGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.authorGrid.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.authorGrid.Location = new System.Drawing.Point(3, 3);
            this.authorGrid.Name = "authorGrid";
            this.authorGrid.Size = new System.Drawing.Size(575, 285);
            this.authorGrid.TabIndex = 0;
            // 
            // tbpHistory
            // 
            this.tbpHistory.Controls.Add(this.rtxtHistory);
            this.tbpHistory.Location = new System.Drawing.Point(4, 22);
            this.tbpHistory.Name = "tbpHistory";
            this.tbpHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tbpHistory.Size = new System.Drawing.Size(581, 291);
            this.tbpHistory.TabIndex = 1;
            this.tbpHistory.Text = "History";
            this.tbpHistory.UseVisualStyleBackColor = true;
            // 
            // rtxtHistory
            // 
            this.rtxtHistory.BackColor = System.Drawing.Color.White;
            this.rtxtHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtHistory.Location = new System.Drawing.Point(3, 3);
            this.rtxtHistory.Name = "rtxtHistory";
            this.rtxtHistory.ReadOnly = true;
            this.rtxtHistory.Size = new System.Drawing.Size(575, 285);
            this.rtxtHistory.TabIndex = 1;
            this.rtxtHistory.TabStop = false;
            this.rtxtHistory.Text = "";
            // 
            // tbpLicenses
            // 
            this.tbpLicenses.Controls.Add(this.rtxtLicenses);
            this.tbpLicenses.Location = new System.Drawing.Point(4, 22);
            this.tbpLicenses.Name = "tbpLicenses";
            this.tbpLicenses.Padding = new System.Windows.Forms.Padding(3);
            this.tbpLicenses.Size = new System.Drawing.Size(581, 291);
            this.tbpLicenses.TabIndex = 2;
            this.tbpLicenses.Text = "Licenses";
            this.tbpLicenses.UseVisualStyleBackColor = true;
            // 
            // rtxtLicenses
            // 
            this.rtxtLicenses.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtLicenses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtLicenses.Location = new System.Drawing.Point(3, 3);
            this.rtxtLicenses.Name = "rtxtLicenses";
            this.rtxtLicenses.Size = new System.Drawing.Size(575, 285);
            this.rtxtLicenses.TabIndex = 0;
            this.rtxtLicenses.Text = "";
            // 
            // txtCopyright
            // 
            this.txtCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCopyright.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCopyright.ForeColor = System.Drawing.Color.SteelBlue;
            this.txtCopyright.Location = new System.Drawing.Point(31, 75);
            this.txtCopyright.Name = "txtCopyright";
            this.txtCopyright.Size = new System.Drawing.Size(577, 13);
            this.txtCopyright.TabIndex = 7;
            this.txtCopyright.TabStop = false;
            this.txtCopyright.Text = "Copyright";
            // 
            // txtVersion
            // 
            this.txtVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVersion.ForeColor = System.Drawing.Color.SteelBlue;
            this.txtVersion.Location = new System.Drawing.Point(31, 38);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(577, 13);
            this.txtVersion.TabIndex = 6;
            this.txtVersion.TabStop = false;
            this.txtVersion.Text = "Version";
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(18, 15);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(590, 18);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 441);
            this.Controls.Add(this.pnl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "About";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.pnl.ResumeLayout(false);
            this.pnl.PerformLayout();
            this.tc.ResumeLayout(false);
            this.tbpAuthors.ResumeLayout(false);
            this.tbpHistory.ResumeLayout(false);
            this.tbpLicenses.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnl;
        private System.Windows.Forms.TabControl tc;
        private System.Windows.Forms.TabPage tbpAuthors;
        private System.Windows.Forms.TabPage tbpHistory;
        private System.Windows.Forms.RichTextBox rtxtHistory;
        private System.Windows.Forms.TabPage tbpLicenses;
        private System.Windows.Forms.RichTextBox rtxtLicenses;
        private System.Windows.Forms.TextBox txtCopyright;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.Label lblDescription;
        private AuthorGrid authorGrid;
        private System.Windows.Forms.LinkLabel lblWebsite;
        private System.Windows.Forms.TextBox txtChannel;


    }
}