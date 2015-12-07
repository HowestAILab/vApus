namespace vApus.Gui {
    partial class FirstStepsView {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstStepsView));
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.llblContact = new System.Windows.Forms.LinkLabel();
            this.panel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.llblExportingResults = new System.Windows.Forms.LinkLabel();
            this.llblSavingResults = new System.Windows.Forms.LinkLabel();
            this.llblProgressNotification = new System.Windows.Forms.LinkLabel();
            this.llblFirewall = new System.Windows.Forms.LinkLabel();
            this.llblUpdate = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(165, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(636, 36);
            this.label1.TabIndex = 1;
            this.label1.Text = "Virtualized Application Unique Stress Testing";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(114)))), ((int)(((byte)(114)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(960, 82);
            this.panel1.TabIndex = 2;
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Location = new System.Drawing.Point(3, 0);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(596, 19);
            this.lblCopyright.TabIndex = 3;
            this.lblCopyright.Text = "Copyright 2007- 2015 © Sizing Servers Lab at HoWest, the university-college of We" +
    "st-Flanders.";
            // 
            // llblContact
            // 
            this.llblContact.AutoSize = true;
            this.llblContact.Location = new System.Drawing.Point(605, 0);
            this.llblContact.Name = "llblContact";
            this.llblContact.Size = new System.Drawing.Size(57, 19);
            this.llblContact.TabIndex = 4;
            this.llblContact.TabStop = true;
            this.llblContact.Text = "Contact";
            this.llblContact.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblContact_LinkClicked);
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel2.Controls.Add(this.lblCopyright);
            this.panel2.Controls.Add(this.llblContact);
            this.panel2.Location = new System.Drawing.Point(167, 531);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(665, 19);
            this.panel2.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(24, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 32);
            this.label2.TabIndex = 6;
            this.label2.Text = "First steps";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.AutoScroll = true;
            this.panel3.Controls.Add(this.llblExportingResults);
            this.panel3.Controls.Add(this.llblSavingResults);
            this.panel3.Controls.Add(this.llblProgressNotification);
            this.panel3.Controls.Add(this.llblFirewall);
            this.panel3.Controls.Add(this.llblUpdate);
            this.panel3.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel3.Location = new System.Drawing.Point(30, 147);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(12, 6, 12, 5);
            this.panel3.Size = new System.Drawing.Size(942, 378);
            this.panel3.TabIndex = 7;
            // 
            // llblExportingResults
            // 
            this.llblExportingResults.ActiveLinkColor = System.Drawing.Color.Black;
            this.llblExportingResults.AutoEllipsis = true;
            this.llblExportingResults.AutoSize = true;
            this.llblExportingResults.DisabledLinkColor = System.Drawing.Color.Black;
            this.llblExportingResults.LinkColor = System.Drawing.Color.Black;
            this.llblExportingResults.Location = new System.Drawing.Point(15, 201);
            this.llblExportingResults.Margin = new System.Windows.Forms.Padding(3, 0, 3, 20);
            this.llblExportingResults.Name = "llblExportingResults";
            this.llblExportingResults.Size = new System.Drawing.Size(565, 23);
            this.llblExportingResults.TabIndex = 4;
            this.llblExportingResults.TabStop = true;
            this.llblExportingResults.Tag = "6";
            this.llblExportingResults.Text = "Set auto-export results to Excel when a test is successfully finished";
            this.llblExportingResults.VisitedLinkColor = System.Drawing.Color.Black;
            this.llblExportingResults.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_LinkClicked);
            // 
            // llblSavingResults
            // 
            this.llblSavingResults.ActiveLinkColor = System.Drawing.Color.Black;
            this.llblSavingResults.AutoEllipsis = true;
            this.llblSavingResults.AutoSize = true;
            this.llblSavingResults.DisabledLinkColor = System.Drawing.Color.Black;
            this.llblSavingResults.LinkColor = System.Drawing.Color.Black;
            this.llblSavingResults.Location = new System.Drawing.Point(15, 158);
            this.llblSavingResults.Margin = new System.Windows.Forms.Padding(3, 0, 3, 20);
            this.llblSavingResults.Name = "llblSavingResults";
            this.llblSavingResults.Size = new System.Drawing.Size(650, 23);
            this.llblSavingResults.TabIndex = 3;
            this.llblSavingResults.TabStop = true;
            this.llblSavingResults.Tag = "5";
            this.llblSavingResults.Text = "Set a connection to a MySQL server for storing stress test and monitor results";
            this.llblSavingResults.VisitedLinkColor = System.Drawing.Color.Black;
            this.llblSavingResults.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_LinkClicked);
            // 
            // llblProgressNotification
            // 
            this.llblProgressNotification.ActiveLinkColor = System.Drawing.Color.Black;
            this.llblProgressNotification.AutoEllipsis = true;
            this.llblProgressNotification.AutoSize = true;
            this.llblProgressNotification.DisabledLinkColor = System.Drawing.Color.Black;
            this.llblProgressNotification.LinkColor = System.Drawing.Color.Black;
            this.llblProgressNotification.Location = new System.Drawing.Point(15, 115);
            this.llblProgressNotification.Margin = new System.Windows.Forms.Padding(3, 0, 3, 20);
            this.llblProgressNotification.Name = "llblProgressNotification";
            this.llblProgressNotification.Size = new System.Drawing.Size(642, 23);
            this.llblProgressNotification.TabIndex = 2;
            this.llblProgressNotification.TabStop = true;
            this.llblProgressNotification.Tag = "4";
            this.llblProgressNotification.Text = "Enable test progress notification to be notified via e-mail when you are AFK";
            this.llblProgressNotification.VisitedLinkColor = System.Drawing.Color.Black;
            this.llblProgressNotification.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_LinkClicked);
            // 
            // llblFirewall
            // 
            this.llblFirewall.ActiveLinkColor = System.Drawing.Color.Black;
            this.llblFirewall.AutoEllipsis = true;
            this.llblFirewall.AutoSize = true;
            this.llblFirewall.DisabledLinkColor = System.Drawing.Color.Black;
            this.llblFirewall.LinkColor = System.Drawing.Color.Black;
            this.llblFirewall.Location = new System.Drawing.Point(15, 49);
            this.llblFirewall.Margin = new System.Windows.Forms.Padding(3, 0, 3, 20);
            this.llblFirewall.Name = "llblFirewall";
            this.llblFirewall.Size = new System.Drawing.Size(830, 46);
            this.llblFirewall.TabIndex = 1;
            this.llblFirewall.TabStop = true;
            this.llblFirewall.Tag = "7";
            this.llblFirewall.Text = "Disable the Windows firewall and Windows update to ensure that communication won\'" +
    "t be blocked\r\nand that the client will not restart automatically to install upda" +
    "tes";
            this.llblFirewall.VisitedLinkColor = System.Drawing.Color.Black;
            this.llblFirewall.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_LinkClicked);
            // 
            // llblUpdate
            // 
            this.llblUpdate.ActiveLinkColor = System.Drawing.Color.Black;
            this.llblUpdate.AutoEllipsis = true;
            this.llblUpdate.AutoSize = true;
            this.llblUpdate.DisabledLinkColor = System.Drawing.Color.Black;
            this.llblUpdate.LinkColor = System.Drawing.Color.Black;
            this.llblUpdate.Location = new System.Drawing.Point(15, 6);
            this.llblUpdate.Margin = new System.Windows.Forms.Padding(3, 0, 3, 20);
            this.llblUpdate.Name = "llblUpdate";
            this.llblUpdate.Size = new System.Drawing.Size(508, 23);
            this.llblUpdate.TabIndex = 0;
            this.llblUpdate.TabStop = true;
            this.llblUpdate.Tag = "0";
            this.llblUpdate.Text = "Set an update server to be notified about new vApus updates";
            this.llblUpdate.VisitedLinkColor = System.Drawing.Color.Black;
            this.llblUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_LinkClicked);
            // 
            // FirstStepsView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(984, 562);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FirstStepsView";
            this.Text = "First steps";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FirstStepsView_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.LinkLabel llblContact;
        private System.Windows.Forms.FlowLayoutPanel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.LinkLabel llblExportingResults;
        private System.Windows.Forms.LinkLabel llblSavingResults;
        private System.Windows.Forms.LinkLabel llblProgressNotification;
        private System.Windows.Forms.LinkLabel llblFirewall;
        private System.Windows.Forms.LinkLabel llblUpdate;
    }
}