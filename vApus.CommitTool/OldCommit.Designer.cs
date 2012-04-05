namespace vApus.CommitTool
{
    partial class OldCommit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OldCommit));
            this.label2 = new System.Windows.Forms.Label();
            this.pnlUpdate = new System.Windows.Forms.Panel();
            this.tcCommit = new System.Windows.Forms.TabControl();
            this.tpHistory = new System.Windows.Forms.TabPage();
            this.btnAddNewItem = new System.Windows.Forms.Button();
            this.btnAddNewTitle = new System.Windows.Forms.Button();
            this.rtxtHistoryOfChanges = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.nudVersion = new System.Windows.Forms.NumericUpDown();
            this.tpFiles = new System.Windows.Forms.TabPage();
            this.lvwCommit = new vApus.Util.ExtendedListView();
            this.clmPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmDateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmCurrentVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmDownloadProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCommit = new System.Windows.Forms.Button();
            this.pbTotal = new System.Windows.Forms.ProgressBar();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.pnlConnection = new System.Windows.Forms.Panel();
            this.chkCommitAll = new System.Windows.Forms.CheckBox();
            this.pnlConnectTo = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.rtxtExclude = new System.Windows.Forms.RichTextBox();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.rtxtLog = new System.Windows.Forms.RichTextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.pnlUpdate.SuspendLayout();
            this.tcCommit.SuspendLayout();
            this.tpHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVersion)).BeginInit();
            this.tpFiles.SuspendLayout();
            this.pnlConnection.SuspendLayout();
            this.pnlConnectTo.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "sftp://";
            // 
            // pnlUpdate
            // 
            this.pnlUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlUpdate.BackColor = System.Drawing.Color.White;
            this.pnlUpdate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlUpdate.Controls.Add(this.tcCommit);
            this.pnlUpdate.Controls.Add(this.btnCommit);
            this.pnlUpdate.Controls.Add(this.pbTotal);
            this.pnlUpdate.Location = new System.Drawing.Point(12, 250);
            this.pnlUpdate.Name = "pnlUpdate";
            this.pnlUpdate.Size = new System.Drawing.Size(638, 392);
            this.pnlUpdate.TabIndex = 1;
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
            this.tcCommit.Size = new System.Drawing.Size(614, 350);
            this.tcCommit.TabIndex = 1;
            // 
            // tpHistory
            // 
            this.tpHistory.Controls.Add(this.btnAddNewItem);
            this.tpHistory.Controls.Add(this.btnAddNewTitle);
            this.tpHistory.Controls.Add(this.rtxtHistoryOfChanges);
            this.tpHistory.Controls.Add(this.label6);
            this.tpHistory.Controls.Add(this.nudVersion);
            this.tpHistory.Location = new System.Drawing.Point(4, 22);
            this.tpHistory.Name = "tpHistory";
            this.tpHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tpHistory.Size = new System.Drawing.Size(606, 324);
            this.tpHistory.TabIndex = 1;
            this.tpHistory.Text = "History";
            this.tpHistory.UseVisualStyleBackColor = true;
            // 
            // btnAddNewItem
            // 
            this.btnAddNewItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddNewItem.AutoSize = true;
            this.btnAddNewItem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddNewItem.BackColor = System.Drawing.SystemColors.Control;
            this.btnAddNewItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddNewItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddNewItem.Location = new System.Drawing.Point(108, 295);
            this.btnAddNewItem.MaximumSize = new System.Drawing.Size(95, 24);
            this.btnAddNewItem.Name = "btnAddNewItem";
            this.btnAddNewItem.Size = new System.Drawing.Size(95, 24);
            this.btnAddNewItem.TabIndex = 4;
            this.btnAddNewItem.Text = "Add new item";
            this.btnAddNewItem.UseVisualStyleBackColor = false;
            this.btnAddNewItem.Click += new System.EventHandler(this.btnAddNewItem_Click);
            // 
            // btnAddNewTitle
            // 
            this.btnAddNewTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddNewTitle.AutoSize = true;
            this.btnAddNewTitle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddNewTitle.BackColor = System.Drawing.SystemColors.Control;
            this.btnAddNewTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddNewTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddNewTitle.Location = new System.Drawing.Point(9, 295);
            this.btnAddNewTitle.MaximumSize = new System.Drawing.Size(93, 24);
            this.btnAddNewTitle.Name = "btnAddNewTitle";
            this.btnAddNewTitle.Size = new System.Drawing.Size(93, 24);
            this.btnAddNewTitle.TabIndex = 3;
            this.btnAddNewTitle.Text = "Add new title";
            this.btnAddNewTitle.UseVisualStyleBackColor = false;
            this.btnAddNewTitle.Click += new System.EventHandler(this.btnAddNewTitle_Click);
            // 
            // rtxtHistoryOfChanges
            // 
            this.rtxtHistoryOfChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtHistoryOfChanges.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtHistoryOfChanges.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtHistoryOfChanges.Location = new System.Drawing.Point(9, 35);
            this.rtxtHistoryOfChanges.Name = "rtxtHistoryOfChanges";
            this.rtxtHistoryOfChanges.Size = new System.Drawing.Size(597, 254);
            this.rtxtHistoryOfChanges.TabIndex = 2;
            this.rtxtHistoryOfChanges.Text = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Version";
            // 
            // nudVersion
            // 
            this.nudVersion.Location = new System.Drawing.Point(61, 12);
            this.nudVersion.Maximum = new decimal(new int[] {
            1,
            1,
            0,
            0});
            this.nudVersion.Name = "nudVersion";
            this.nudVersion.Size = new System.Drawing.Size(116, 20);
            this.nudVersion.TabIndex = 0;
            // 
            // tpFiles
            // 
            this.tpFiles.Controls.Add(this.lvwCommit);
            this.tpFiles.Location = new System.Drawing.Point(4, 22);
            this.tpFiles.Name = "tpFiles";
            this.tpFiles.Size = new System.Drawing.Size(606, 324);
            this.tpFiles.TabIndex = 0;
            this.tpFiles.Text = "Files";
            this.tpFiles.UseVisualStyleBackColor = true;
            // 
            // lvwCommit
            // 
            this.lvwCommit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwCommit.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmPath,
            this.clmDateTime,
            this.clmVersion,
            this.clmCurrentVersion,
            this.clmDownloadProgress});
            this.lvwCommit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwCommit.FullRowSelect = true;
            this.lvwCommit.Location = new System.Drawing.Point(0, 0);
            this.lvwCommit.MultiSelect = false;
            this.lvwCommit.Name = "lvwCommit";
            this.lvwCommit.Size = new System.Drawing.Size(606, 324);
            this.lvwCommit.TabIndex = 0;
            this.lvwCommit.UseCompatibleStateImageBehavior = false;
            this.lvwCommit.View = System.Windows.Forms.View.Details;
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
            this.clmCurrentVersion.Text = "Version on server";
            this.clmCurrentVersion.Width = 100;
            // 
            // clmDownloadProgress
            // 
            this.clmDownloadProgress.Text = "";
            this.clmDownloadProgress.Width = 100;
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCommit.AutoSize = true;
            this.btnCommit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCommit.BackColor = System.Drawing.SystemColors.Control;
            this.btnCommit.Enabled = false;
            this.btnCommit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCommit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCommit.Location = new System.Drawing.Point(11, 359);
            this.btnCommit.MaximumSize = new System.Drawing.Size(59, 24);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(59, 24);
            this.btnCommit.TabIndex = 2;
            this.btnCommit.Text = "Commit";
            this.btnCommit.UseVisualStyleBackColor = false;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // pbTotal
            // 
            this.pbTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbTotal.Location = new System.Drawing.Point(76, 365);
            this.pbTotal.Name = "pbTotal";
            this.pbTotal.Size = new System.Drawing.Size(545, 13);
            this.pbTotal.TabIndex = 2;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(460, 9);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.TextChanged += new System.EventHandler(this._TextChanged);
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // pnlConnection
            // 
            this.pnlConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlConnection.BackColor = System.Drawing.Color.White;
            this.pnlConnection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlConnection.Controls.Add(this.chkCommitAll);
            this.pnlConnection.Controls.Add(this.pnlConnectTo);
            this.pnlConnection.Controls.Add(this.rtxtLog);
            this.pnlConnection.Controls.Add(this.btnConnect);
            this.pnlConnection.Location = new System.Drawing.Point(12, 12);
            this.pnlConnection.Name = "pnlConnection";
            this.pnlConnection.Size = new System.Drawing.Size(638, 232);
            this.pnlConnection.TabIndex = 0;
            // 
            // chkCommitAll
            // 
            this.chkCommitAll.AutoSize = true;
            this.chkCommitAll.Location = new System.Drawing.Point(11, 187);
            this.chkCommitAll.Name = "chkCommitAll";
            this.chkCommitAll.Size = new System.Drawing.Size(114, 43);
            this.chkCommitAll.TabIndex = 7;
            this.chkCommitAll.Text = "Commit versioned\r\nand non-versioned\r\n(no clean-up)";
            this.chkCommitAll.UseVisualStyleBackColor = true;
            // 
            // pnlConnectTo
            // 
            this.pnlConnectTo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlConnectTo.Controls.Add(this.label1);
            this.pnlConnectTo.Controls.Add(this.rtxtExclude);
            this.pnlConnectTo.Controls.Add(this.txtHost);
            this.pnlConnectTo.Controls.Add(this.label2);
            this.pnlConnectTo.Controls.Add(this.txtUsername);
            this.pnlConnectTo.Controls.Add(this.txtPassword);
            this.pnlConnectTo.Controls.Add(this.label4);
            this.pnlConnectTo.Controls.Add(this.txtPort);
            this.pnlConnectTo.Controls.Add(this.label3);
            this.pnlConnectTo.Controls.Add(this.label5);
            this.pnlConnectTo.Location = new System.Drawing.Point(0, 0);
            this.pnlConnectTo.Name = "pnlConnectTo";
            this.pnlConnectTo.Size = new System.Drawing.Size(636, 158);
            this.pnlConnectTo.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Files or folders to be excluded:";
            // 
            // rtxtExclude
            // 
            this.rtxtExclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtExclude.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtxtExclude.Location = new System.Drawing.Point(11, 63);
            this.rtxtExclude.Name = "rtxtExclude";
            this.rtxtExclude.Size = new System.Drawing.Size(614, 86);
            this.rtxtExclude.TabIndex = 4;
            this.rtxtExclude.Text = "";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(51, 9);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(100, 20);
            this.txtHost.TabIndex = 0;
            this.txtHost.TextChanged += new System.EventHandler(this._TextChanged);
            this.txtHost.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(292, 9);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(100, 20);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.TextChanged += new System.EventHandler(this._TextChanged);
            this.txtUsername.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(157, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Port:";
            // 
            // txtPort
            // 
            this.txtPort.ForeColor = System.Drawing.Color.Black;
            this.txtPort.Location = new System.Drawing.Point(192, 9);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(30, 20);
            this.txtPort.TabIndex = 1;
            this.txtPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.txtPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPort_KeyPress);
            this.txtPort.Leave += new System.EventHandler(this.txtPort_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(398, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Password:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(228, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Username:";
            // 
            // rtxtLog
            // 
            this.rtxtLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtLog.BackColor = System.Drawing.Color.White;
            this.rtxtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtLog.Location = new System.Drawing.Point(131, 164);
            this.rtxtLog.Name = "rtxtLog";
            this.rtxtLog.ReadOnly = true;
            this.rtxtLog.Size = new System.Drawing.Size(494, 59);
            this.rtxtLog.TabIndex = 6;
            this.rtxtLog.Text = "";
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConnect.BackColor = System.Drawing.SystemColors.Control;
            this.btnConnect.Enabled = false;
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(11, 164);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(114, 23);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // sfd
            // 
            this.sfd.Filter = "*.txt|*.txt";
            this.sfd.InitialDirectory = ".";
            this.sfd.Title = "Choose a location";
            // 
            // Commit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 654);
            this.Controls.Add(this.pnlUpdate);
            this.Controls.Add(this.pnlConnection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Commit";
            this.Text = "Commit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Commit_FormClosing);
            this.pnlUpdate.ResumeLayout(false);
            this.pnlUpdate.PerformLayout();
            this.tcCommit.ResumeLayout(false);
            this.tpHistory.ResumeLayout(false);
            this.tpHistory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVersion)).EndInit();
            this.tpFiles.ResumeLayout(false);
            this.pnlConnection.ResumeLayout(false);
            this.pnlConnection.PerformLayout();
            this.pnlConnectTo.ResumeLayout(false);
            this.pnlConnectTo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlUpdate;
        private vApus.Util.ExtendedListView lvwCommit;
        private System.Windows.Forms.ColumnHeader clmPath;
        private System.Windows.Forms.ColumnHeader clmDateTime;
        private System.Windows.Forms.ColumnHeader clmVersion;
        private System.Windows.Forms.ColumnHeader clmCurrentVersion;
        private System.Windows.Forms.ColumnHeader clmDownloadProgress;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.ProgressBar pbTotal;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Panel pnlConnection;
        private System.Windows.Forms.Panel pnlConnectTo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox rtxtLog;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtxtExclude;
        private System.Windows.Forms.TabControl tcCommit;
        private System.Windows.Forms.TabPage tpFiles;
        private System.Windows.Forms.TabPage tpHistory;
        private System.Windows.Forms.RichTextBox rtxtHistoryOfChanges;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudVersion;
        private System.Windows.Forms.Button btnAddNewItem;
        private System.Windows.Forms.Button btnAddNewTitle;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.CheckBox chkCommitAll;
    }
}

