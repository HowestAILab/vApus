namespace vApus.Util
{
    partial class UpdateNotifierPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateNotifierPanel));
            this.btnSet = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.pnlConnectTo = new System.Windows.Forms.Panel();
            this.btnBrowseRSAPrivateKey = new System.Windows.Forms.Button();
            this.txtPrivateRSAKey = new System.Windows.Forms.TextBox();
            this.cboChannel = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.chkSmartUpdate = new System.Windows.Forms.CheckBox();
            this.btnForceUpdate = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.pnlRefresh = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lbl = new System.Windows.Forms.Label();
            this.pic = new System.Windows.Forms.PictureBox();
            this.pnlConnectTo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.pnlRefresh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSet
            // 
            this.btnSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSet.BackColor = System.Drawing.Color.White;
            this.btnSet.Enabled = false;
            this.btnSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSet.Location = new System.Drawing.Point(938, 541);
            this.btnSet.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(100, 24);
            this.btnSet.TabIndex = 4;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = false;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.AutoSize = true;
            this.btnClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClear.BackColor = System.Drawing.Color.White;
            this.btnClear.Enabled = false;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(884, 541);
            this.btnClear.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 24);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // pnlConnectTo
            // 
            this.pnlConnectTo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlConnectTo.Controls.Add(this.btnBrowseRSAPrivateKey);
            this.pnlConnectTo.Controls.Add(this.txtPrivateRSAKey);
            this.pnlConnectTo.Controls.Add(this.cboChannel);
            this.pnlConnectTo.Controls.Add(this.label9);
            this.pnlConnectTo.Controls.Add(this.label8);
            this.pnlConnectTo.Controls.Add(this.label7);
            this.pnlConnectTo.Controls.Add(this.txtUsername);
            this.pnlConnectTo.Controls.Add(this.label6);
            this.pnlConnectTo.Controls.Add(this.nudPort);
            this.pnlConnectTo.Controls.Add(this.label5);
            this.pnlConnectTo.Controls.Add(this.txtHost);
            this.pnlConnectTo.Controls.Add(this.chkSmartUpdate);
            this.pnlConnectTo.Location = new System.Drawing.Point(12, 12);
            this.pnlConnectTo.Name = "pnlConnectTo";
            this.pnlConnectTo.Padding = new System.Windows.Forms.Padding(3);
            this.pnlConnectTo.Size = new System.Drawing.Size(1026, 454);
            this.pnlConnectTo.TabIndex = 0;
            // 
            // btnBrowseRSAPrivateKey
            // 
            this.btnBrowseRSAPrivateKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseRSAPrivateKey.AutoSize = true;
            this.btnBrowseRSAPrivateKey.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBrowseRSAPrivateKey.BackColor = System.Drawing.Color.White;
            this.btnBrowseRSAPrivateKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseRSAPrivateKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseRSAPrivateKey.Location = new System.Drawing.Point(930, 71);
            this.btnBrowseRSAPrivateKey.Margin = new System.Windows.Forms.Padding(0, 3, 6, 6);
            this.btnBrowseRSAPrivateKey.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnBrowseRSAPrivateKey.Name = "btnBrowseRSAPrivateKey";
            this.btnBrowseRSAPrivateKey.Size = new System.Drawing.Size(31, 24);
            this.btnBrowseRSAPrivateKey.TabIndex = 4;
            this.btnBrowseRSAPrivateKey.Text = "...";
            this.toolTip.SetToolTip(this.btnBrowseRSAPrivateKey, "Update all files regardless if they need to be updated.");
            this.btnBrowseRSAPrivateKey.UseVisualStyleBackColor = false;
            this.btnBrowseRSAPrivateKey.Click += new System.EventHandler(this.btnBrowseRSAPrivateKey_Click);
            // 
            // txtPrivateRSAKey
            // 
            this.txtPrivateRSAKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrivateRSAKey.Location = new System.Drawing.Point(123, 71);
            this.txtPrivateRSAKey.Margin = new System.Windows.Forms.Padding(0, 5, 6, 6);
            this.txtPrivateRSAKey.Name = "txtPrivateRSAKey";
            this.txtPrivateRSAKey.Size = new System.Drawing.Size(788, 20);
            this.txtPrivateRSAKey.TabIndex = 3;
            this.toolTip.SetToolTip(this.txtPrivateRSAKey, "Path to a passwordless private RSA key.");
            this.txtPrivateRSAKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            // 
            // cboChannel
            // 
            this.cboChannel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboChannel.BackColor = System.Drawing.Color.White;
            this.cboChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChannel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboChannel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboChannel.FormattingEnabled = true;
            this.cboChannel.Items.AddRange(new object[] {
            "Stable",
            "Nightly"});
            this.cboChannel.Location = new System.Drawing.Point(124, 103);
            this.cboChannel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboChannel.Name = "cboChannel";
            this.cboChannel.Size = new System.Drawing.Size(784, 21);
            this.cboChannel.TabIndex = 5;
            this.cboChannel.SelectedIndexChanged += new System.EventHandler(this.cboChannel_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 110);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Channel:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 74);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Private RSA key:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 41);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Username:";
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Location = new System.Drawing.Point(123, 38);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(0, 5, 6, 6);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(788, 20);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(923, 11);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Port:";
            // 
            // nudPort
            // 
            this.nudPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPort.Location = new System.Drawing.Point(967, 8);
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
            22,
            0,
            0,
            0});
            this.nudPort.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 10);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Host:";
            // 
            // txtHost
            // 
            this.txtHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHost.Location = new System.Drawing.Point(123, 8);
            this.txtHost.Margin = new System.Windows.Forms.Padding(0, 5, 6, 6);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(788, 20);
            this.txtHost.TabIndex = 0;
            this.txtHost.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            // 
            // chkSmartUpdate
            // 
            this.chkSmartUpdate.AutoSize = true;
            this.chkSmartUpdate.Location = new System.Drawing.Point(12, 167);
            this.chkSmartUpdate.Name = "chkSmartUpdate";
            this.chkSmartUpdate.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.chkSmartUpdate.Size = new System.Drawing.Size(122, 23);
            this.chkSmartUpdate.TabIndex = 5;
            this.chkSmartUpdate.Text = "Smart update slaves";
            this.toolTip.SetToolTip(this.chkSmartUpdate, resources.GetString("chkSmartUpdate.ToolTip"));
            this.chkSmartUpdate.UseVisualStyleBackColor = true;
            this.chkSmartUpdate.CheckedChanged += new System.EventHandler(this.chkSmartUpdate_CheckedChanged);
            // 
            // btnForceUpdate
            // 
            this.btnForceUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnForceUpdate.AutoSize = true;
            this.btnForceUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnForceUpdate.BackColor = System.Drawing.Color.White;
            this.btnForceUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnForceUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnForceUpdate.Location = new System.Drawing.Point(12, 541);
            this.btnForceUpdate.Margin = new System.Windows.Forms.Padding(0, 3, 6, 6);
            this.btnForceUpdate.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnForceUpdate.Name = "btnForceUpdate";
            this.btnForceUpdate.Size = new System.Drawing.Size(94, 24);
            this.btnForceUpdate.TabIndex = 2;
            this.btnForceUpdate.Text = "Force update";
            this.toolTip.SetToolTip(this.btnForceUpdate, "Update all files regardless if they need to be updated.");
            this.btnForceUpdate.UseVisualStyleBackColor = false;
            this.btnForceUpdate.Click += new System.EventHandler(this.btnUpdateManually_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "All files|*.*";
            // 
            // pnlRefresh
            // 
            this.pnlRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRefresh.BackColor = System.Drawing.Color.White;
            this.pnlRefresh.Controls.Add(this.btnRefresh);
            this.pnlRefresh.Controls.Add(this.lbl);
            this.pnlRefresh.Controls.Add(this.pic);
            this.pnlRefresh.Enabled = false;
            this.pnlRefresh.Location = new System.Drawing.Point(12, 502);
            this.pnlRefresh.Name = "pnlRefresh";
            this.pnlRefresh.Padding = new System.Windows.Forms.Padding(3);
            this.pnlRefresh.Size = new System.Drawing.Size(1026, 33);
            this.pnlRefresh.TabIndex = 5;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.AutoSize = true;
            this.btnRefresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(998, 5);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(22, 22);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbl.Location = new System.Drawing.Point(28, 10);
            this.lbl.MinimumSize = new System.Drawing.Size(0, 16);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(100, 16);
            this.lbl.TabIndex = 0;
            this.lbl.Text = "Please refresh...";
            // 
            // pic
            // 
            this.pic.Image = global::vApus.Util.Properties.Resources.Warning;
            this.pic.Location = new System.Drawing.Point(6, 8);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(16, 16);
            this.pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pic.TabIndex = 2;
            this.pic.TabStop = false;
            // 
            // UpdateNotifierPanel
            // 
            this.ClientSize = new System.Drawing.Size(1050, 580);
            this.Controls.Add(this.pnlRefresh);
            this.Controls.Add(this.pnlConnectTo);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.btnForceUpdate);
            this.Name = "UpdateNotifierPanel";
            this.pnlConnectTo.ResumeLayout(false);
            this.pnlConnectTo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.pnlRefresh.ResumeLayout(false);
            this.pnlRefresh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Panel pnlConnectTo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cboChannel;
        private System.Windows.Forms.Button btnForceUpdate;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox chkSmartUpdate;
        private System.Windows.Forms.TextBox txtPrivateRSAKey;
        private System.Windows.Forms.Button btnBrowseRSAPrivateKey;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel pnlRefresh;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.PictureBox pic;
    }
}