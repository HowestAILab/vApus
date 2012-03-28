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
            this.lblWebsite = new System.Windows.Forms.LinkLabel();
            this.tc = new System.Windows.Forms.TabControl();
            this.tbpAuthors = new System.Windows.Forms.TabPage();
            this.authorGrid = new vApus.Gui.AuthorGrid();
            this.tbpHistoryOfChanges = new System.Windows.Forms.TabPage();
            this.rtxtHistoryOfChanges = new System.Windows.Forms.RichTextBox();
            this.tbpLicenses = new System.Windows.Forms.TabPage();
            this.rtxtLicenses = new System.Windows.Forms.RichTextBox();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.pnl.SuspendLayout();
            this.tc.SuspendLayout();
            this.tbpAuthors.SuspendLayout();
            this.tbpHistoryOfChanges.SuspendLayout();
            this.tbpLicenses.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl
            // 
            this.pnl.BackColor = System.Drawing.Color.White;
            this.pnl.Controls.Add(this.lblWebsite);
            this.pnl.Controls.Add(this.tc);
            this.pnl.Controls.Add(this.lblCopyright);
            this.pnl.Controls.Add(this.lblVersion);
            this.pnl.Controls.Add(this.lblDescription);
            this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl.Location = new System.Drawing.Point(0, 0);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(628, 441);
            this.pnl.TabIndex = 4;
            // 
            // lblWebsite
            // 
            this.lblWebsite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWebsite.Location = new System.Drawing.Point(29, 79);
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
            this.tc.Controls.Add(this.tbpHistoryOfChanges);
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
            // tbpHistoryOfChanges
            // 
            this.tbpHistoryOfChanges.Controls.Add(this.rtxtHistoryOfChanges);
            this.tbpHistoryOfChanges.Location = new System.Drawing.Point(4, 22);
            this.tbpHistoryOfChanges.Name = "tbpHistoryOfChanges";
            this.tbpHistoryOfChanges.Padding = new System.Windows.Forms.Padding(3);
            this.tbpHistoryOfChanges.Size = new System.Drawing.Size(581, 291);
            this.tbpHistoryOfChanges.TabIndex = 1;
            this.tbpHistoryOfChanges.Text = "History of Changes";
            this.tbpHistoryOfChanges.UseVisualStyleBackColor = true;
            // 
            // rtxtHistoryOfChanges
            // 
            this.rtxtHistoryOfChanges.BackColor = System.Drawing.Color.White;
            this.rtxtHistoryOfChanges.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtHistoryOfChanges.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtHistoryOfChanges.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtHistoryOfChanges.Location = new System.Drawing.Point(3, 3);
            this.rtxtHistoryOfChanges.Name = "rtxtHistoryOfChanges";
            this.rtxtHistoryOfChanges.ReadOnly = true;
            this.rtxtHistoryOfChanges.Size = new System.Drawing.Size(575, 285);
            this.rtxtHistoryOfChanges.TabIndex = 1;
            this.rtxtHistoryOfChanges.TabStop = false;
            this.rtxtHistoryOfChanges.Text = "";
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
            // lblCopyright
            // 
            this.lblCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyright.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblCopyright.Location = new System.Drawing.Point(28, 61);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(580, 18);
            this.lblCopyright.TabIndex = 7;
            this.lblCopyright.Text = "Copyright";
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblVersion.Location = new System.Drawing.Point(28, 38);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(580, 18);
            this.lblVersion.TabIndex = 6;
            this.lblVersion.Text = "Version";
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
            this.tc.ResumeLayout(false);
            this.tbpAuthors.ResumeLayout(false);
            this.tbpHistoryOfChanges.ResumeLayout(false);
            this.tbpLicenses.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnl;
        private System.Windows.Forms.TabControl tc;
        private System.Windows.Forms.TabPage tbpAuthors;
        private System.Windows.Forms.TabPage tbpHistoryOfChanges;
        private System.Windows.Forms.RichTextBox rtxtHistoryOfChanges;
        private System.Windows.Forms.TabPage tbpLicenses;
        private System.Windows.Forms.RichTextBox rtxtLicenses;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblDescription;
        private AuthorGrid authorGrid;
        private System.Windows.Forms.LinkLabel lblWebsite;


    }
}