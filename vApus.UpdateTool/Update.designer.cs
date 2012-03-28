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
            this.cboHost = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.chkSaveHost = new System.Windows.Forms.CheckBox();
            this.chkSaveUsername = new System.Windows.Forms.CheckBox();
            this.chkSavePassword = new System.Windows.Forms.CheckBox();
            this.pnlConnection = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chkGetAll = new System.Windows.Forms.CheckBox();
            this.pnlConnectTo = new System.Windows.Forms.Panel();
            this.rtxtLog = new System.Windows.Forms.RichTextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnUpdateOrReinstall = new System.Windows.Forms.Button();
            this.pbTotal = new System.Windows.Forms.ProgressBar();
            this.pnlUpdate = new System.Windows.Forms.Panel();
            this.tcCommit = new System.Windows.Forms.TabControl();
            this.tpHistory = new System.Windows.Forms.TabPage();
            this.rtxtHistoryOfChanges = new System.Windows.Forms.RichTextBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tpFiles = new System.Windows.Forms.TabPage();
            this.lvwUpdate = new vApus.Util.ExtendedListView();
            this.clmPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmDateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmCurrentVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmDownloadProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlBorder = new System.Windows.Forms.Panel();
            this.pnlConnection.SuspendLayout();
            this.pnlConnectTo.SuspendLayout();
            this.pnlUpdate.SuspendLayout();
            this.tcCommit.SuspendLayout();
            this.tpHistory.SuspendLayout();
            this.tpFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "sftp://";
            // 
            // cboHost
            // 
            this.cboHost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboHost.FormattingEnabled = true;
            this.cboHost.Location = new System.Drawing.Point(51, 9);
            this.cboHost.Name = "cboHost";
            this.cboHost.Size = new System.Drawing.Size(117, 21);
            this.cboHost.TabIndex = 0;
            this.cboHost.SelectedIndexChanged += new System.EventHandler(this.cboHost_SelectedIndexChanged);
            this.cboHost.TextChanged += new System.EventHandler(this.cboHost_TextChanged);
            this.cboHost.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Username:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(282, 9);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(100, 20);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.TextChanged += new System.EventHandler(this.txtUsername_TextChanged);
            this.txtUsername.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(220, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(282, 40);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Port:";
            // 
            // txtPort
            // 
            this.txtPort.ForeColor = System.Drawing.Color.Black;
            this.txtPort.Location = new System.Drawing.Point(51, 40);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(30, 20);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "5222";
            this.txtPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.txtPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPort_KeyPress);
            this.txtPort.Leave += new System.EventHandler(this.txtPort_Leave);
            // 
            // chkSaveHost
            // 
            this.chkSaveHost.AutoSize = true;
            this.chkSaveHost.Checked = true;
            this.chkSaveHost.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveHost.Location = new System.Drawing.Point(94, 42);
            this.chkSaveHost.Name = "chkSaveHost";
            this.chkSaveHost.Size = new System.Drawing.Size(74, 17);
            this.chkSaveHost.TabIndex = 4;
            this.chkSaveHost.Text = "Save host";
            this.chkSaveHost.UseVisualStyleBackColor = true;
            this.chkSaveHost.CheckedChanged += new System.EventHandler(this.chkSaveHost_CheckedChanged);
            this.chkSaveHost.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // chkSaveUsername
            // 
            this.chkSaveUsername.AutoSize = true;
            this.chkSaveUsername.Checked = true;
            this.chkSaveUsername.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveUsername.Location = new System.Drawing.Point(403, 11);
            this.chkSaveUsername.Name = "chkSaveUsername";
            this.chkSaveUsername.Size = new System.Drawing.Size(100, 17);
            this.chkSaveUsername.TabIndex = 2;
            this.chkSaveUsername.Text = "Save username";
            this.chkSaveUsername.UseVisualStyleBackColor = true;
            this.chkSaveUsername.CheckedChanged += new System.EventHandler(this.chkSaveUsername_CheckedChanged);
            this.chkSaveUsername.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // chkSavePassword
            // 
            this.chkSavePassword.AutoSize = true;
            this.chkSavePassword.Enabled = false;
            this.chkSavePassword.Location = new System.Drawing.Point(403, 42);
            this.chkSavePassword.Name = "chkSavePassword";
            this.chkSavePassword.Size = new System.Drawing.Size(169, 17);
            this.chkSavePassword.TabIndex = 6;
            this.chkSavePassword.Text = "Save username and password";
            this.chkSavePassword.UseVisualStyleBackColor = true;
            this.chkSavePassword.CheckedChanged += new System.EventHandler(this.chkSavePassword_CheckedChanged);
            this.chkSavePassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // pnlConnection
            // 
            this.pnlConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlConnection.BackColor = System.Drawing.Color.White;
            this.pnlConnection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlConnection.Controls.Add(this.btnRefresh);
            this.pnlConnection.Controls.Add(this.chkGetAll);
            this.pnlConnection.Controls.Add(this.pnlConnectTo);
            this.pnlConnection.Controls.Add(this.rtxtLog);
            this.pnlConnection.Controls.Add(this.btnConnect);
            this.pnlConnection.Location = new System.Drawing.Point(12, 12);
            this.pnlConnection.Name = "pnlConnection";
            this.pnlConnection.Size = new System.Drawing.Size(638, 154);
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
            this.chkGetAll.Location = new System.Drawing.Point(11, 115);
            this.chkGetAll.Name = "chkGetAll";
            this.chkGetAll.Size = new System.Drawing.Size(114, 30);
            this.chkGetAll.TabIndex = 4;
            this.chkGetAll.Text = "Get versioned\r\nand non-versioned";
            this.chkGetAll.UseVisualStyleBackColor = true;
            // 
            // pnlConnectTo
            // 
            this.pnlConnectTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlConnectTo.Controls.Add(this.label1);
            this.pnlConnectTo.Controls.Add(this.txtUsername);
            this.pnlConnectTo.Controls.Add(this.txtPassword);
            this.pnlConnectTo.Controls.Add(this.label4);
            this.pnlConnectTo.Controls.Add(this.chkSavePassword);
            this.pnlConnectTo.Controls.Add(this.txtPort);
            this.pnlConnectTo.Controls.Add(this.cboHost);
            this.pnlConnectTo.Controls.Add(this.label3);
            this.pnlConnectTo.Controls.Add(this.chkSaveUsername);
            this.pnlConnectTo.Controls.Add(this.chkSaveHost);
            this.pnlConnectTo.Controls.Add(this.label2);
            this.pnlConnectTo.Controls.Add(this.pnlBorder);
            this.pnlConnectTo.Location = new System.Drawing.Point(0, 0);
            this.pnlConnectTo.Name = "pnlConnectTo";
            this.pnlConnectTo.Size = new System.Drawing.Size(636, 80);
            this.pnlConnectTo.TabIndex = 0;
            // 
            // rtxtLog
            // 
            this.rtxtLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtLog.BackColor = System.Drawing.Color.White;
            this.rtxtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtLog.Location = new System.Drawing.Point(131, 86);
            this.rtxtLog.Name = "rtxtLog";
            this.rtxtLog.ReadOnly = true;
            this.rtxtLog.Size = new System.Drawing.Size(494, 59);
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
            this.pbTotal.Size = new System.Drawing.Size(536, 13);
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
            this.pnlUpdate.Size = new System.Drawing.Size(638, 352);
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
            this.tcCommit.Size = new System.Drawing.Size(614, 310);
            this.tcCommit.TabIndex = 0;
            // 
            // tpHistory
            // 
            this.tpHistory.Controls.Add(this.rtxtHistoryOfChanges);
            this.tpHistory.Controls.Add(this.lblVersion);
            this.tpHistory.Location = new System.Drawing.Point(4, 22);
            this.tpHistory.Name = "tpHistory";
            this.tpHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tpHistory.Size = new System.Drawing.Size(606, 284);
            this.tpHistory.TabIndex = 1;
            this.tpHistory.Text = "History";
            this.tpHistory.UseVisualStyleBackColor = true;
            // 
            // rtxtHistoryOfChanges
            // 
            this.rtxtHistoryOfChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtHistoryOfChanges.BackColor = System.Drawing.Color.White;
            this.rtxtHistoryOfChanges.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtHistoryOfChanges.Location = new System.Drawing.Point(9, 30);
            this.rtxtHistoryOfChanges.Name = "rtxtHistoryOfChanges";
            this.rtxtHistoryOfChanges.ReadOnly = true;
            this.rtxtHistoryOfChanges.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxtHistoryOfChanges.Size = new System.Drawing.Size(597, 254);
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
            this.lblVersion.Size = new System.Drawing.Size(49, 13);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "Version";
            // 
            // tpFiles
            // 
            this.tpFiles.Controls.Add(this.lvwUpdate);
            this.tpFiles.Location = new System.Drawing.Point(4, 22);
            this.tpFiles.Name = "tpFiles";
            this.tpFiles.Size = new System.Drawing.Size(606, 284);
            this.tpFiles.TabIndex = 0;
            this.tpFiles.Text = "Files";
            this.tpFiles.UseVisualStyleBackColor = true;
            // 
            // lvwUpdate
            // 
            this.lvwUpdate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwUpdate.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmPath,
            this.clmDateTime,
            this.clmVersion,
            this.clmCurrentVersion,
            this.clmDownloadProgress});
            this.lvwUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwUpdate.FullRowSelect = true;
            this.lvwUpdate.Location = new System.Drawing.Point(0, 0);
            this.lvwUpdate.MultiSelect = false;
            this.lvwUpdate.Name = "lvwUpdate";
            this.lvwUpdate.Size = new System.Drawing.Size(606, 284);
            this.lvwUpdate.TabIndex = 0;
            this.lvwUpdate.UseCompatibleStateImageBehavior = false;
            this.lvwUpdate.View = System.Windows.Forms.View.Details;
            // 
            // clmPath
            // 
            this.clmPath.Text = "Path";
            this.clmPath.Width = 200;
            // 
            // clmDateTime
            // 
            this.clmDateTime.Text = "Date/time";
            this.clmDateTime.Width = 100;
            // 
            // clmVersion
            // 
            this.clmVersion.Text = "Version";
            this.clmVersion.Width = 55;
            // 
            // clmCurrentVersion
            // 
            this.clmCurrentVersion.Text = "Current version";
            this.clmCurrentVersion.Width = 90;
            // 
            // clmDownloadProgress
            // 
            this.clmDownloadProgress.Text = "";
            this.clmDownloadProgress.Width = 100;
            // 
            // pnlBorder
            // 
            this.pnlBorder.BackColor = System.Drawing.Color.Silver;
            this.pnlBorder.Location = new System.Drawing.Point(50, 8);
            this.pnlBorder.Name = "pnlBorder";
            this.pnlBorder.Size = new System.Drawing.Size(119, 23);
            this.pnlBorder.TabIndex = 7;
            // 
            // Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 536);
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
            this.pnlConnectTo.ResumeLayout(false);
            this.pnlConnectTo.PerformLayout();
            this.pnlUpdate.ResumeLayout(false);
            this.tcCommit.ResumeLayout(false);
            this.tpHistory.ResumeLayout(false);
            this.tpHistory.PerformLayout();
            this.tpFiles.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboHost;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.CheckBox chkSaveHost;
        private System.Windows.Forms.CheckBox chkSaveUsername;
        private System.Windows.Forms.CheckBox chkSavePassword;
        private System.Windows.Forms.Panel pnlConnection;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnUpdateOrReinstall;
        private System.Windows.Forms.ProgressBar pbTotal;
        private System.Windows.Forms.Panel pnlUpdate;
        private vApus.Util.ExtendedListView lvwUpdate;
        private System.Windows.Forms.ColumnHeader clmPath;
        private System.Windows.Forms.ColumnHeader clmDateTime;
        private System.Windows.Forms.ColumnHeader clmVersion;
        private System.Windows.Forms.ColumnHeader clmCurrentVersion;
        private System.Windows.Forms.ColumnHeader clmDownloadProgress;
        private System.Windows.Forms.RichTextBox rtxtLog;
        private System.Windows.Forms.Panel pnlConnectTo;
        private System.Windows.Forms.CheckBox chkGetAll;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.TabControl tcCommit;
        private System.Windows.Forms.TabPage tpFiles;
        private System.Windows.Forms.TabPage tpHistory;
        private System.Windows.Forms.RichTextBox rtxtHistoryOfChanges;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Panel pnlBorder;
    }
}