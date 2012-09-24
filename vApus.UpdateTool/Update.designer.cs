namespace vApus.UpdateTool
{
    partial class Update
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Update));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlConnection = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chkGetAll = new System.Windows.Forms.CheckBox();
            this.flpConnectTo = new System.Windows.Forms.FlowLayoutPanel();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.rtxtLog = new System.Windows.Forms.RichTextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnUpdateOrReinstall = new System.Windows.Forms.Button();
            this.pbTotal = new System.Windows.Forms.ProgressBar();
            this.pnlUpdate = new System.Windows.Forms.Panel();
            this.tcCommit = new System.Windows.Forms.TabControl();
            this.tpHistory = new System.Windows.Forms.TabPage();
            this.lblChannel = new System.Windows.Forms.Label();
            this.rtxtHistoryOfChanges = new System.Windows.Forms.RichTextBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tpFiles = new System.Windows.Forms.TabPage();
            this.lvwUpdate = new vApus.Util.ExtendedListView();
            this.clmPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmLocalMD5Checksum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmRemoteMD5Checksum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmDownloadProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.pnlConnection.SuspendLayout();
            this.flpConnectTo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.pnlUpdate.SuspendLayout();
            this.tcCommit.SuspendLayout();
            this.tpHistory.SuspendLayout();
            this.tpFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(244, 10);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Username:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(302, 8);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(0, 5, 6, 6);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(100, 20);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.TextChanged += new System.EventHandler(this.txtUsername_TextChanged);
            this.txtUsername.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(414, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(470, 8);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(0, 5, 6, 6);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(153, 10);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Port:";
            // 
            // pnlConnection
            // 
            this.pnlConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlConnection.BackColor = System.Drawing.Color.White;
            this.pnlConnection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlConnection.Controls.Add(this.label5);
            this.pnlConnection.Controls.Add(this.lblChannel);
            this.pnlConnection.Controls.Add(this.btnRefresh);
            this.pnlConnection.Controls.Add(this.chkGetAll);
            this.pnlConnection.Controls.Add(this.flpConnectTo);
            this.pnlConnection.Controls.Add(this.rtxtLog);
            this.pnlConnection.Controls.Add(this.btnConnect);
            this.pnlConnection.Location = new System.Drawing.Point(12, 12);
            this.pnlConnection.Name = "pnlConnection";
            this.pnlConnection.Size = new System.Drawing.Size(812, 154);
            this.pnlConnection.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.AutoSize = true;
            this.btnRefresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRefresh.BackColor = System.Drawing.SystemColors.Control;
            this.btnRefresh.Enabled = false;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.Image = global::vApus.UpdateTool.Properties.Resources.Icon_16x16_Refresh;
            this.btnRefresh.Location = new System.Drawing.Point(100, 86);
            this.btnRefresh.MaximumSize = new System.Drawing.Size(24, 24);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(24, 24);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // chkGetAll
            // 
            this.chkGetAll.AutoSize = true;
            this.chkGetAll.Location = new System.Drawing.Point(131, 91);
            this.chkGetAll.Name = "chkGetAll";
            this.chkGetAll.Size = new System.Drawing.Size(183, 17);
            this.chkGetAll.TabIndex = 4;
            this.chkGetAll.Text = "Get versioned and non-versioned";
            this.chkGetAll.UseVisualStyleBackColor = true;
            // 
            // flpConnectTo
            // 
            this.flpConnectTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpConnectTo.Controls.Add(this.label1);
            this.flpConnectTo.Controls.Add(this.txtHost);
            this.flpConnectTo.Controls.Add(this.label4);
            this.flpConnectTo.Controls.Add(this.nudPort);
            this.flpConnectTo.Controls.Add(this.label2);
            this.flpConnectTo.Controls.Add(this.txtUsername);
            this.flpConnectTo.Controls.Add(this.label3);
            this.flpConnectTo.Controls.Add(this.txtPassword);
            this.flpConnectTo.Location = new System.Drawing.Point(0, 0);
            this.flpConnectTo.Name = "flpConnectTo";
            this.flpConnectTo.Padding = new System.Windows.Forms.Padding(3);
            this.flpConnectTo.Size = new System.Drawing.Size(810, 80);
            this.flpConnectTo.TabIndex = 0;
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(41, 8);
            this.txtHost.Margin = new System.Windows.Forms.Padding(0, 5, 6, 6);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(100, 20);
            this.txtHost.TabIndex = 0;
            this.txtHost.TextChanged += new System.EventHandler(this.txtHost_TextChanged);
            this.txtHost.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(182, 8);
            this.nudPort.Margin = new System.Windows.Forms.Padding(0, 5, 6, 6);
            this.nudPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(50, 20);
            this.nudPort.TabIndex = 1;
            this.nudPort.Value = new decimal(new int[] {
            22, //External port 5222
            0,
            0,
            0});
            // 
            // rtxtLog
            // 
            this.rtxtLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtLog.BackColor = System.Drawing.Color.White;
            this.rtxtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtLog.Location = new System.Drawing.Point(449, 86);
            this.rtxtLog.Name = "rtxtLog";
            this.rtxtLog.ReadOnly = true;
            this.rtxtLog.Size = new System.Drawing.Size(350, 59);
            this.rtxtLog.TabIndex = 3;
            this.rtxtLog.Text = "";
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.SystemColors.Control;
            this.btnConnect.Enabled = false;
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(11, 86);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(83, 24);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnUpdateOrReinstall
            // 
            this.btnUpdateOrReinstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpdateOrReinstall.BackColor = System.Drawing.SystemColors.Control;
            this.btnUpdateOrReinstall.Enabled = false;
            this.btnUpdateOrReinstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateOrReinstall.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateOrReinstall.Location = new System.Drawing.Point(11, 318);
            this.btnUpdateOrReinstall.MaximumSize = new System.Drawing.Size(68, 24);
            this.btnUpdateOrReinstall.Name = "btnUpdateOrReinstall";
            this.btnUpdateOrReinstall.Size = new System.Drawing.Size(68, 24);
            this.btnUpdateOrReinstall.TabIndex = 1;
            this.btnUpdateOrReinstall.Text = "Update";
            this.btnUpdateOrReinstall.UseVisualStyleBackColor = false;
            this.btnUpdateOrReinstall.Click += new System.EventHandler(this.btnUpdateOrReinstall_Click);
            // 
            // pbTotal
            // 
            this.pbTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbTotal.Location = new System.Drawing.Point(85, 324);
            this.pbTotal.Name = "pbTotal";
            this.pbTotal.Size = new System.Drawing.Size(710, 13);
            this.pbTotal.TabIndex = 2;
            // 
            // pnlUpdate
            // 
            this.pnlUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlUpdate.BackColor = System.Drawing.Color.White;
            this.pnlUpdate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlUpdate.Controls.Add(this.tcCommit);
            this.pnlUpdate.Controls.Add(this.btnUpdateOrReinstall);
            this.pnlUpdate.Controls.Add(this.pbTotal);
            this.pnlUpdate.Location = new System.Drawing.Point(12, 172);
            this.pnlUpdate.Name = "pnlUpdate";
            this.pnlUpdate.Size = new System.Drawing.Size(812, 352);
            this.pnlUpdate.TabIndex = 10;
            // 
            // tcCommit
            // 
            this.tcCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcCommit.Controls.Add(this.tpHistory);
            this.tcCommit.Controls.Add(this.tpFiles);
            this.tcCommit.Location = new System.Drawing.Point(11, 3);
            this.tcCommit.Name = "tcCommit";
            this.tcCommit.SelectedIndex = 0;
            this.tcCommit.Size = new System.Drawing.Size(788, 310);
            this.tcCommit.TabIndex = 0;
            // 
            // tpHistory
            // 
            this.tpHistory.Controls.Add(this.rtxtHistoryOfChanges);
            this.tpHistory.Controls.Add(this.lblVersion);
            this.tpHistory.Location = new System.Drawing.Point(4, 22);
            this.tpHistory.Name = "tpHistory";
            this.tpHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tpHistory.Size = new System.Drawing.Size(780, 284);
            this.tpHistory.TabIndex = 1;
            this.tpHistory.Text = "History";
            this.tpHistory.UseVisualStyleBackColor = true;
            // 
            // lblChannel
            // 
            this.lblChannel.AutoSize = true;
            this.lblChannel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChannel.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblChannel.Location = new System.Drawing.Point(257, 120);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(57, 13);
            this.lblChannel.TabIndex = 2;
            this.lblChannel.Text = "Channel:";
            // 
            // rtxtHistoryOfChanges
            // 
            this.rtxtHistoryOfChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtHistoryOfChanges.BackColor = System.Drawing.Color.White;
            this.rtxtHistoryOfChanges.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtHistoryOfChanges.Location = new System.Drawing.Point(9, 58);
            this.rtxtHistoryOfChanges.Name = "rtxtHistoryOfChanges";
            this.rtxtHistoryOfChanges.ReadOnly = true;
            this.rtxtHistoryOfChanges.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxtHistoryOfChanges.Size = new System.Drawing.Size(771, 226);
            this.rtxtHistoryOfChanges.TabIndex = 0;
            this.rtxtHistoryOfChanges.Text = "";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblVersion.Location = new System.Drawing.Point(6, 14);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(53, 13);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "Version:";
            // 
            // tpFiles
            // 
            this.tpFiles.Controls.Add(this.lvwUpdate);
            this.tpFiles.Location = new System.Drawing.Point(4, 22);
            this.tpFiles.Name = "tpFiles";
            this.tpFiles.Size = new System.Drawing.Size(780, 284);
            this.tpFiles.TabIndex = 0;
            this.tpFiles.Text = "Files";
            this.tpFiles.UseVisualStyleBackColor = true;
            // 
            // lvwUpdate
            // 
            this.lvwUpdate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwUpdate.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmPath,
            this.clmLocalMD5Checksum,
            this.clmRemoteMD5Checksum,
            this.clmDownloadProgress});
            this.lvwUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwUpdate.FullRowSelect = true;
            this.lvwUpdate.Location = new System.Drawing.Point(0, 0);
            this.lvwUpdate.MultiSelect = false;
            this.lvwUpdate.Name = "lvwUpdate";
            this.lvwUpdate.Size = new System.Drawing.Size(780, 284);
            this.lvwUpdate.TabIndex = 0;
            this.lvwUpdate.UseCompatibleStateImageBehavior = false;
            this.lvwUpdate.View = System.Windows.Forms.View.Details;
            // 
            // clmPath
            // 
            this.clmPath.Text = "Path";
            this.clmPath.Width = 200;
            // 
            // clmLocalMD5Checksum
            // 
            this.clmLocalMD5Checksum.Text = "Local MD5 Checksum";
            this.clmLocalMD5Checksum.Width = 117;
            // 
            // clmRemoteMD5Checksum
            // 
            this.clmRemoteMD5Checksum.Text = "Remote MD5 Checksum";
            this.clmRemoteMD5Checksum.Width = 128;
            // 
            // clmDownloadProgress
            // 
            this.clmDownloadProgress.Text = "";
            this.clmDownloadProgress.Width = 100;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label5.Location = new System.Drawing.Point(12, 120);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(240, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Can only switch channel in the Update Notifier -->";
            // 
            // Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 536);
            this.Controls.Add(this.pnlUpdate);
            this.Controls.Add(this.pnlConnection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "Update";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update/reinstall";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Update_FormClosing);
            this.pnlConnection.ResumeLayout(false);
            this.pnlConnection.PerformLayout();
            this.flpConnectTo.ResumeLayout(false);
            this.flpConnectTo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.pnlUpdate.ResumeLayout(false);
            this.tcCommit.ResumeLayout(false);
            this.tpHistory.ResumeLayout(false);
            this.tpHistory.PerformLayout();
            this.tpFiles.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnlConnection;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnUpdateOrReinstall;
        private System.Windows.Forms.ProgressBar pbTotal;
        private System.Windows.Forms.Panel pnlUpdate;
        private vApus.Util.ExtendedListView lvwUpdate;
        private System.Windows.Forms.ColumnHeader clmPath;
        private System.Windows.Forms.ColumnHeader clmRemoteMD5Checksum;
        private System.Windows.Forms.ColumnHeader clmLocalMD5Checksum;
        private System.Windows.Forms.ColumnHeader clmDownloadProgress;
        private System.Windows.Forms.RichTextBox rtxtLog;
        private System.Windows.Forms.FlowLayoutPanel flpConnectTo;
        private System.Windows.Forms.CheckBox chkGetAll;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.TabControl tcCommit;
        private System.Windows.Forms.TabPage tpFiles;
        private System.Windows.Forms.TabPage tpHistory;
        private System.Windows.Forms.RichTextBox rtxtHistoryOfChanges;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Label lblChannel;
        private System.Windows.Forms.Label label5;
    }
}