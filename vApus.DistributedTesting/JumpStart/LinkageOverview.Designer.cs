namespace vApus.DistributedTesting
{
    partial class LinkageOverview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinkageOverview));
            this.flp = new System.Windows.Forms.FlowLayoutPanel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel6 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnConnectionStrings = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnRefresh = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnKillAll = new System.Windows.Forms.Button();
            this.btnJumpStartAll = new System.Windows.Forms.Button();
            this.pb = new System.Windows.Forms.ProgressBar();
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // flp
            // 
            this.flp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flp.AutoScroll = true;
            this.flp.BackColor = System.Drawing.Color.White;
            this.flp.Location = new System.Drawing.Point(0, 33);
            this.flp.Margin = new System.Windows.Forms.Padding(4);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(1134, 503);
            this.flp.TabIndex = 4;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel6,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel4,
            this.toolStripStatusLabel5,
            this.btnConnectionStrings,
            this.btnRefresh});
            this.statusStrip.Location = new System.Drawing.Point(0, 540);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1134, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabel6
            // 
            this.toolStripStatusLabel6.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel6.Name = "toolStripStatusLabel6";
            this.toolStripStatusLabel6.Size = new System.Drawing.Size(51, 17);
            this.toolStripStatusLabel6.Text = "Legend:";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel1.Image = global::vApus.DistributedTesting.Properties.Resources.Offline;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(59, 17);
            this.toolStripStatusLabel1.Text = "Offline";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel2.Image = global::vApus.DistributedTesting.Properties.Resources.OnlineComputer;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(115, 17);
            this.toolStripStatusLabel2.Text = "Online Computer";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel4.Image = global::vApus.DistributedTesting.Properties.Resources.OnlineSlave;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(88, 17);
            this.toolStripStatusLabel4.Text = "Online Slave";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(562, 17);
            this.toolStripStatusLabel5.Spring = true;
            // 
            // btnConnectionStrings
            // 
            this.btnConnectionStrings.ActiveLinkColor = System.Drawing.Color.Black;
            this.btnConnectionStrings.BackColor = System.Drawing.Color.Transparent;
            this.btnConnectionStrings.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnectionStrings.IsLink = true;
            this.btnConnectionStrings.LinkColor = System.Drawing.Color.Black;
            this.btnConnectionStrings.Name = "btnConnectionStrings";
            this.btnConnectionStrings.Size = new System.Drawing.Size(146, 17);
            this.btnConnectionStrings.Text = "Show Connection Strings";
            this.btnConnectionStrings.VisitedLinkColor = System.Drawing.Color.Black;
            this.btnConnectionStrings.Click += new System.EventHandler(this.btnConnectionStrings_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.ActiveLinkColor = System.Drawing.Color.Black;
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.IsLink = true;
            this.btnRefresh.LinkColor = System.Drawing.Color.Black;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(67, 17);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.VisitedLinkColor = System.Drawing.Color.Black;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            // 
            // btnKillAll
            // 
            this.btnKillAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnKillAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnKillAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnKillAll.Enabled = false;
            this.btnKillAll.FlatAppearance.BorderSize = 0;
            this.btnKillAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKillAll.Image = ((System.Drawing.Image)(resources.GetObject("btnKillAll.Image")));
            this.btnKillAll.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnKillAll.Location = new System.Drawing.Point(1010, 4);
            this.btnKillAll.Name = "btnKillAll";
            this.btnKillAll.Size = new System.Drawing.Size(70, 26);
            this.btnKillAll.TabIndex = 21;
            this.btnKillAll.Text = "Kill All";
            this.btnKillAll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip.SetToolTip(this.btnKillAll, "This will kill all slaves on the online computers (if any and if the right creden" +
        "tials are given), even the ones who weren\'t jump started!");
            this.btnKillAll.UseVisualStyleBackColor = false;
            this.btnKillAll.Click += new System.EventHandler(this.btnKillAll_Click);
            // 
            // btnJumpStartAll
            // 
            this.btnJumpStartAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnJumpStartAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnJumpStartAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnJumpStartAll.Enabled = false;
            this.btnJumpStartAll.FlatAppearance.BorderSize = 0;
            this.btnJumpStartAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJumpStartAll.Image = ((System.Drawing.Image)(resources.GetObject("btnJumpStartAll.Image")));
            this.btnJumpStartAll.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnJumpStartAll.Location = new System.Drawing.Point(889, 3);
            this.btnJumpStartAll.Name = "btnJumpStartAll";
            this.btnJumpStartAll.Size = new System.Drawing.Size(115, 26);
            this.btnJumpStartAll.TabIndex = 3;
            this.btnJumpStartAll.Text = "Jump Start All";
            this.btnJumpStartAll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip.SetToolTip(this.btnJumpStartAll, "This will jump start all slaves on the online computers (if any an" +
        "d if the right credentials are given) who weren\'t started already!");
            this.btnJumpStartAll.UseVisualStyleBackColor = false;
            this.btnJumpStartAll.Click += new System.EventHandler(this.btnJumpStartAll_Click);
            // 
            // pb
            // 
            this.pb.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pb.Location = new System.Drawing.Point(0, 535);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(1134, 5);
            this.pb.TabIndex = 19;
            // 
            // btnCollapseExpand
            // 
            this.btnCollapseExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseExpand.BackColor = System.Drawing.Color.White;
            this.btnCollapseExpand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollapseExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpand.Location = new System.Drawing.Point(1086, 3);
            this.btnCollapseExpand.Name = "btnCollapseExpand";
            this.btnCollapseExpand.Size = new System.Drawing.Size(36, 26);
            this.btnCollapseExpand.TabIndex = 20;
            this.btnCollapseExpand.TabStop = false;
            this.btnCollapseExpand.Text = "-";
            this.btnCollapseExpand.UseVisualStyleBackColor = false;
            this.btnCollapseExpand.Click += new System.EventHandler(this.btnCollapseExpand_Click);
            // 
            // LinkageOverview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 562);
            this.Controls.Add(this.btnKillAll);
            this.Controls.Add(this.btnCollapseExpand);
            this.Controls.Add(this.pb);
            this.Controls.Add(this.btnJumpStartAll);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.flp);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "LinkageOverview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Linkage Overview [vApus must be installed at \'C:\\Program Files\\Sizing Servers\\vAp" +
    "us\' for Jump Start to work!]";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LinkageOverview_FormClosing);
            this.Shown += new System.EventHandler(this.LinkageOverview_Shown);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flp;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel btnConnectionStrings;
        private System.Windows.Forms.ToolStripStatusLabel btnRefresh;
        private System.Windows.Forms.Button btnJumpStartAll;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ProgressBar pb;
        private System.Windows.Forms.Button btnCollapseExpand;
        private System.Windows.Forms.Button btnKillAll;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel6;
    }
}