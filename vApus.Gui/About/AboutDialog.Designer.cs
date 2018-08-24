namespace vApus.Gui
{
    partial class AboutDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.pnl = new System.Windows.Forms.Panel();
            this.picHowest = new System.Windows.Forms.PictureBox();
            this.picSSL = new System.Windows.Forms.PictureBox();
            this.picNMCT = new System.Windows.Forms.PictureBox();
            this.txtChannel = new System.Windows.Forms.TextBox();
            this.tc = new System.Windows.Forms.TabControl();
            this.tbpAuthors = new System.Windows.Forms.TabPage();
            this.authorGrid = new vApus.Gui.AuthorGrid();
            this.tbpHistory = new System.Windows.Forms.TabPage();
            this.rtxtHistory = new System.Windows.Forms.RichTextBox();
            this.tbpLicense = new System.Windows.Forms.TabPage();
            this.lblLicense = new System.Windows.Forms.RichTextBox();
            this.btnActivateLicense = new System.Windows.Forms.Button();
            this.btnRequestLicense = new System.Windows.Forms.Button();
            this.tbpLicenses = new System.Windows.Forms.TabPage();
            this.rtxtLicenses = new System.Windows.Forms.RichTextBox();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.pnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHowest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSSL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNMCT)).BeginInit();
            this.tc.SuspendLayout();
            this.tbpAuthors.SuspendLayout();
            this.tbpHistory.SuspendLayout();
            this.tbpLicense.SuspendLayout();
            this.tbpLicenses.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl
            // 
            this.pnl.BackColor = System.Drawing.Color.White;
            this.pnl.Controls.Add(this.picHowest);
            this.pnl.Controls.Add(this.picSSL);
            this.pnl.Controls.Add(this.picNMCT);
            this.pnl.Controls.Add(this.txtChannel);
            this.pnl.Controls.Add(this.tc);
            this.pnl.Controls.Add(this.txtVersion);
            this.pnl.Controls.Add(this.lblDescription);
            this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl.Location = new System.Drawing.Point(0, 0);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(784, 561);
            this.pnl.TabIndex = 4;
            // 
            // picHowest
            // 
            this.picHowest.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.picHowest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picHowest.Image = global::vApus.Gui.Properties.Resources.howest;
            this.picHowest.Location = new System.Drawing.Point(590, 479);
            this.picHowest.Name = "picHowest";
            this.picHowest.Size = new System.Drawing.Size(174, 70);
            this.picHowest.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picHowest.TabIndex = 16;
            this.picHowest.TabStop = false;
            this.picHowest.Click += new System.EventHandler(this.picHowest_Click);
            // 
            // picSSL
            // 
            this.picSSL.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.picSSL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picSSL.Image = global::vApus.Gui.Properties.Resources.SSL;
            this.picSSL.Location = new System.Drawing.Point(270, 426);
            this.picSSL.Name = "picSSL";
            this.picSSL.Size = new System.Drawing.Size(237, 125);
            this.picSSL.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picSSL.TabIndex = 15;
            this.picSSL.TabStop = false;
            this.picSSL.Click += new System.EventHandler(this.picSSL_Click);
            // 
            // picNMCT
            // 
            this.picNMCT.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.picNMCT.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picNMCT.Image = global::vApus.Gui.Properties.Resources.NMCT;
            this.picNMCT.Location = new System.Drawing.Point(21, 475);
            this.picNMCT.Name = "picNMCT";
            this.picNMCT.Size = new System.Drawing.Size(144, 74);
            this.picNMCT.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picNMCT.TabIndex = 14;
            this.picNMCT.TabStop = false;
            this.picNMCT.Click += new System.EventHandler(this.picNMCT_Click);
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
            this.txtChannel.Size = new System.Drawing.Size(733, 13);
            this.txtChannel.TabIndex = 13;
            this.txtChannel.TabStop = false;
            this.txtChannel.Text = "Channel";
            // 
            // tc
            // 
            this.tc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tc.Controls.Add(this.tbpAuthors);
            this.tc.Controls.Add(this.tbpHistory);
            this.tc.Controls.Add(this.tbpLicense);
            this.tc.Controls.Add(this.tbpLicenses);
            this.tc.Location = new System.Drawing.Point(20, 111);
            this.tc.Name = "tc";
            this.tc.SelectedIndex = 0;
            this.tc.Size = new System.Drawing.Size(745, 300);
            this.tc.TabIndex = 10;
            // 
            // tbpAuthors
            // 
            this.tbpAuthors.Controls.Add(this.authorGrid);
            this.tbpAuthors.Location = new System.Drawing.Point(4, 22);
            this.tbpAuthors.Name = "tbpAuthors";
            this.tbpAuthors.Padding = new System.Windows.Forms.Padding(3);
            this.tbpAuthors.Size = new System.Drawing.Size(737, 274);
            this.tbpAuthors.TabIndex = 0;
            this.tbpAuthors.Text = "Authors";
            this.tbpAuthors.UseVisualStyleBackColor = true;
            // 
            // authorGrid
            // 
            this.authorGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.authorGrid.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.authorGrid.Location = new System.Drawing.Point(3, 3);
            this.authorGrid.Margin = new System.Windows.Forms.Padding(4);
            this.authorGrid.Name = "authorGrid";
            this.authorGrid.Size = new System.Drawing.Size(731, 268);
            this.authorGrid.TabIndex = 0;
            // 
            // tbpHistory
            // 
            this.tbpHistory.Controls.Add(this.rtxtHistory);
            this.tbpHistory.Location = new System.Drawing.Point(4, 22);
            this.tbpHistory.Name = "tbpHistory";
            this.tbpHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tbpHistory.Size = new System.Drawing.Size(737, 274);
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
            this.rtxtHistory.Size = new System.Drawing.Size(731, 268);
            this.rtxtHistory.TabIndex = 1;
            this.rtxtHistory.TabStop = false;
            this.rtxtHistory.Text = resources.GetString("rtxtHistory.Text");
            // 
            // tbpLicense
            // 
            this.tbpLicense.Controls.Add(this.lblLicense);
            this.tbpLicense.Controls.Add(this.btnActivateLicense);
            this.tbpLicense.Controls.Add(this.btnRequestLicense);
            this.tbpLicense.Location = new System.Drawing.Point(4, 22);
            this.tbpLicense.Margin = new System.Windows.Forms.Padding(2);
            this.tbpLicense.Name = "tbpLicense";
            this.tbpLicense.Size = new System.Drawing.Size(737, 274);
            this.tbpLicense.TabIndex = 3;
            this.tbpLicense.Text = "License";
            this.tbpLicense.UseVisualStyleBackColor = true;
            // 
            // lblLicense
            // 
            this.lblLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLicense.BackColor = System.Drawing.Color.White;
            this.lblLicense.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblLicense.Location = new System.Drawing.Point(5, 10);
            this.lblLicense.Margin = new System.Windows.Forms.Padding(2);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(727, 231);
            this.lblLicense.TabIndex = 5;
            this.lblLicense.Text = "Checking license...";
            // 
            // btnActivateLicense
            // 
            this.btnActivateLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActivateLicense.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnActivateLicense.BackColor = System.Drawing.Color.White;
            this.btnActivateLicense.Enabled = false;
            this.btnActivateLicense.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnActivateLicense.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActivateLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnActivateLicense.Location = new System.Drawing.Point(620, 245);
            this.btnActivateLicense.Margin = new System.Windows.Forms.Padding(2);
            this.btnActivateLicense.Name = "btnActivateLicense";
            this.btnActivateLicense.Size = new System.Drawing.Size(112, 24);
            this.btnActivateLicense.TabIndex = 4;
            this.btnActivateLicense.Text = "Activate License";
            this.btnActivateLicense.UseVisualStyleBackColor = false;
            this.btnActivateLicense.Click += new System.EventHandler(this.btnActivateLicense_Click);
            // 
            // btnRequestLicense
            // 
            this.btnRequestLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRequestLicense.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRequestLicense.BackColor = System.Drawing.Color.White;
            this.btnRequestLicense.Enabled = false;
            this.btnRequestLicense.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnRequestLicense.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRequestLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnRequestLicense.Location = new System.Drawing.Point(496, 245);
            this.btnRequestLicense.Margin = new System.Windows.Forms.Padding(2);
            this.btnRequestLicense.Name = "btnRequestLicense";
            this.btnRequestLicense.Size = new System.Drawing.Size(120, 24);
            this.btnRequestLicense.TabIndex = 3;
            this.btnRequestLicense.Text = "Request license...";
            this.btnRequestLicense.UseVisualStyleBackColor = false;
            this.btnRequestLicense.Click += new System.EventHandler(this.btnRequestLicense_Click);
            // 
            // tbpLicenses
            // 
            this.tbpLicenses.Controls.Add(this.rtxtLicenses);
            this.tbpLicenses.Location = new System.Drawing.Point(4, 22);
            this.tbpLicenses.Name = "tbpLicenses";
            this.tbpLicenses.Padding = new System.Windows.Forms.Padding(3);
            this.tbpLicenses.Size = new System.Drawing.Size(737, 274);
            this.tbpLicenses.TabIndex = 2;
            this.tbpLicenses.Text = "Third-party licenses";
            this.tbpLicenses.UseVisualStyleBackColor = true;
            // 
            // rtxtLicenses
            // 
            this.rtxtLicenses.BackColor = System.Drawing.Color.White;
            this.rtxtLicenses.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtLicenses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtLicenses.Location = new System.Drawing.Point(3, 3);
            this.rtxtLicenses.Name = "rtxtLicenses";
            this.rtxtLicenses.ReadOnly = true;
            this.rtxtLicenses.Size = new System.Drawing.Size(731, 268);
            this.rtxtLicenses.TabIndex = 0;
            this.rtxtLicenses.Text = "";
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
            this.txtVersion.Size = new System.Drawing.Size(733, 13);
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
            this.lblDescription.Size = new System.Drawing.Size(746, 18);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description";
            // 
            // ofd
            // 
            this.ofd.Filter = "License files|*.license";
            this.ofd.InitialDirectory = ".";
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.pnl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AboutDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.pnl.ResumeLayout(false);
            this.pnl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHowest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSSL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNMCT)).EndInit();
            this.tc.ResumeLayout(false);
            this.tbpAuthors.ResumeLayout(false);
            this.tbpHistory.ResumeLayout(false);
            this.tbpLicense.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.Label lblDescription;
        private AuthorGrid authorGrid;
        private System.Windows.Forms.TextBox txtChannel;
        private System.Windows.Forms.TabPage tbpLicense;
        private System.Windows.Forms.RichTextBox lblLicense;
        private System.Windows.Forms.Button btnActivateLicense;
        private System.Windows.Forms.Button btnRequestLicense;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.PictureBox picHowest;
        private System.Windows.Forms.PictureBox picSSL;
        private System.Windows.Forms.PictureBox picNMCT;
    }
}