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
            this.lbl = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.picHowest = new System.Windows.Forms.PictureBox();
            this.picSSL = new System.Windows.Forms.PictureBox();
            this.picNMCT = new System.Windows.Forms.PictureBox();
            this.llblHelp = new System.Windows.Forms.LinkLabel();
            this.llblSavingResults = new System.Windows.Forms.LinkLabel();
            this.llblProgressNotification = new System.Windows.Forms.LinkLabel();
            this.llblFirewall = new System.Windows.Forms.LinkLabel();
            this.llblUpdate = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHowest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSSL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNMCT)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 21F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Location = new System.Drawing.Point(21, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(926, 34);
            this.label1.TabIndex = 1;
            this.label1.Text = "Virtualized Application Unique Stress Testing: First steps";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(960, 92);
            this.panel1.TabIndex = 2;
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl.Location = new System.Drawing.Point(3, 0);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(838, 15);
            this.lbl.TabIndex = 3;
            this.lbl.Text = "Actively developed from 2007 to 2018 @ Sizing Servers Lab affiliated with the IT " +
    "bachelor degree NMCT at Howest, the university-college of West-Flanders.";
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel2.Controls.Add(this.lbl);
            this.panel2.Location = new System.Drawing.Point(78, 730);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(894, 19);
            this.panel2.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.AutoScroll = true;
            this.panel3.Controls.Add(this.picHowest);
            this.panel3.Controls.Add(this.picSSL);
            this.panel3.Controls.Add(this.picNMCT);
            this.panel3.Controls.Add(this.llblHelp);
            this.panel3.Controls.Add(this.llblSavingResults);
            this.panel3.Controls.Add(this.llblProgressNotification);
            this.panel3.Controls.Add(this.llblFirewall);
            this.panel3.Controls.Add(this.llblUpdate);
            this.panel3.Font = new System.Drawing.Font("Verdana", 13F);
            this.panel3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel3.Location = new System.Drawing.Point(12, 147);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(12, 6, 12, 5);
            this.panel3.Size = new System.Drawing.Size(960, 577);
            this.panel3.TabIndex = 7;
            // 
            // picHowest
            // 
            this.picHowest.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.picHowest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picHowest.Image = global::vApus.Gui.Properties.Resources.howest;
            this.picHowest.Location = new System.Drawing.Point(698, 426);
            this.picHowest.Name = "picHowest";
            this.picHowest.Size = new System.Drawing.Size(261, 105);
            this.picHowest.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picHowest.TabIndex = 7;
            this.picHowest.TabStop = false;
            this.picHowest.Click += new System.EventHandler(this.picHowest_Click);
            // 
            // picSSL
            // 
            this.picSSL.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.picSSL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picSSL.Image = global::vApus.Gui.Properties.Resources.SSL;
            this.picSSL.Location = new System.Drawing.Point(288, 343);
            this.picSSL.Name = "picSSL";
            this.picSSL.Size = new System.Drawing.Size(356, 188);
            this.picSSL.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picSSL.TabIndex = 6;
            this.picSSL.TabStop = false;
            this.picSSL.Click += new System.EventHandler(this.picSSL_Click);
            // 
            // picNMCT
            // 
            this.picNMCT.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.picNMCT.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picNMCT.Image = global::vApus.Gui.Properties.Resources.NMCT;
            this.picNMCT.Location = new System.Drawing.Point(3, 420);
            this.picNMCT.Name = "picNMCT";
            this.picNMCT.Size = new System.Drawing.Size(216, 111);
            this.picNMCT.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picNMCT.TabIndex = 5;
            this.picNMCT.TabStop = false;
            this.picNMCT.Click += new System.EventHandler(this.picNMCT_Click);
            // 
            // llblHelp
            // 
            this.llblHelp.ActiveLinkColor = System.Drawing.Color.Black;
            this.llblHelp.AutoEllipsis = true;
            this.llblHelp.AutoSize = true;
            this.llblHelp.DisabledLinkColor = System.Drawing.Color.Black;
            this.llblHelp.Font = new System.Drawing.Font("Verdana", 15F);
            this.llblHelp.LinkColor = System.Drawing.Color.Black;
            this.llblHelp.Location = new System.Drawing.Point(15, 16);
            this.llblHelp.Margin = new System.Windows.Forms.Padding(3, 0, 3, 20);
            this.llblHelp.Name = "llblHelp";
            this.llblHelp.Size = new System.Drawing.Size(154, 25);
            this.llblHelp.TabIndex = 0;
            this.llblHelp.TabStop = true;
            this.llblHelp.Tag = "";
            this.llblHelp.Text = "Read the Help";
            this.llblHelp.VisitedLinkColor = System.Drawing.Color.Black;
            this.llblHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblHelp_LinkClicked);
            // 
            // llblSavingResults
            // 
            this.llblSavingResults.ActiveLinkColor = System.Drawing.Color.Black;
            this.llblSavingResults.AutoEllipsis = true;
            this.llblSavingResults.AutoSize = true;
            this.llblSavingResults.DisabledLinkColor = System.Drawing.Color.Black;
            this.llblSavingResults.LinkColor = System.Drawing.Color.Black;
            this.llblSavingResults.Location = new System.Drawing.Point(15, 218);
            this.llblSavingResults.Margin = new System.Windows.Forms.Padding(3, 0, 3, 20);
            this.llblSavingResults.Name = "llblSavingResults";
            this.llblSavingResults.Size = new System.Drawing.Size(843, 22);
            this.llblSavingResults.TabIndex = 4;
            this.llblSavingResults.TabStop = true;
            this.llblSavingResults.Tag = "3";
            this.llblSavingResults.Text = "Enable publishing counters, metrics and messages for storing stress test and moni" +
    "tor results";
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
            this.llblProgressNotification.Location = new System.Drawing.Point(15, 175);
            this.llblProgressNotification.Margin = new System.Windows.Forms.Padding(3, 0, 3, 20);
            this.llblProgressNotification.Name = "llblProgressNotification";
            this.llblProgressNotification.Size = new System.Drawing.Size(690, 22);
            this.llblProgressNotification.TabIndex = 3;
            this.llblProgressNotification.TabStop = true;
            this.llblProgressNotification.Tag = "5";
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
            this.llblFirewall.Location = new System.Drawing.Point(15, 109);
            this.llblFirewall.Margin = new System.Windows.Forms.Padding(3, 0, 3, 20);
            this.llblFirewall.Name = "llblFirewall";
            this.llblFirewall.Size = new System.Drawing.Size(896, 44);
            this.llblFirewall.TabIndex = 2;
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
            this.llblUpdate.Location = new System.Drawing.Point(15, 66);
            this.llblUpdate.Margin = new System.Windows.Forms.Padding(3, 0, 3, 20);
            this.llblUpdate.Name = "llblUpdate";
            this.llblUpdate.Size = new System.Drawing.Size(562, 22);
            this.llblUpdate.TabIndex = 1;
            this.llblUpdate.TabStop = true;
            this.llblUpdate.Tag = "6";
            this.llblUpdate.Text = "Set an update server to be notified about new vApus updates";
            this.llblUpdate.VisitedLinkColor = System.Drawing.Color.Black;
            this.llblUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_LinkClicked);
            // 
            // FirstStepsView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(984, 761);
            this.Controls.Add(this.panel3);
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
            ((System.ComponentModel.ISupportInitialize)(this.picHowest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSSL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNMCT)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.FlowLayoutPanel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.LinkLabel llblSavingResults;
        private System.Windows.Forms.LinkLabel llblProgressNotification;
        private System.Windows.Forms.LinkLabel llblFirewall;
        private System.Windows.Forms.LinkLabel llblUpdate;
        private System.Windows.Forms.LinkLabel llblHelp;
        private System.Windows.Forms.PictureBox picNMCT;
        private System.Windows.Forms.PictureBox picSSL;
        private System.Windows.Forms.PictureBox picHowest;
    }
}