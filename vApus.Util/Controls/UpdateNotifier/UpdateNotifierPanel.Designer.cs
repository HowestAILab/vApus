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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateNotifierPanel));
            this.btnSet = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlRefresh = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lbl = new System.Windows.Forms.Label();
            this.pic = new System.Windows.Forms.PictureBox();
            this.flpConnectTo = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.pnlBorderChannel = new System.Windows.Forms.Panel();
            this.cboChannel = new System.Windows.Forms.ComboBox();
            this.btnUpdateManually = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.pnlRefresh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.flpConnectTo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pnlBorderChannel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSet
            // 
            this.btnSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSet.BackColor = System.Drawing.Color.White;
            this.btnSet.Enabled = false;
            this.btnSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSet.Location = new System.Drawing.Point(297, 206);
            this.btnSet.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(75, 24);
            this.btnSet.TabIndex = 4;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = false;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.BackColor = System.Drawing.Color.White;
            this.btnClear.Enabled = false;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(216, 206);
            this.btnClear.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 24);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pnlRefresh);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox1.Size = new System.Drawing.Size(360, 188);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // pnlRefresh
            // 
            this.pnlRefresh.BackColor = System.Drawing.Color.White;
            this.pnlRefresh.Controls.Add(this.btnRefresh);
            this.pnlRefresh.Controls.Add(this.lbl);
            this.pnlRefresh.Controls.Add(this.pic);
            this.pnlRefresh.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRefresh.Enabled = false;
            this.pnlRefresh.Location = new System.Drawing.Point(0, 155);
            this.pnlRefresh.Name = "pnlRefresh";
            this.pnlRefresh.Padding = new System.Windows.Forms.Padding(3);
            this.pnlRefresh.Size = new System.Drawing.Size(360, 33);
            this.pnlRefresh.TabIndex = 1;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.AutoSize = true;
            this.btnRefresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(332, 5);
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
            // flpConnectTo
            // 
            this.flpConnectTo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpConnectTo.Controls.Add(this.panel1);
            this.flpConnectTo.Controls.Add(this.panel2);
            this.flpConnectTo.Controls.Add(this.panel3);
            this.flpConnectTo.Controls.Add(this.panel4);
            this.flpConnectTo.Controls.Add(this.panel5);
            this.flpConnectTo.Location = new System.Drawing.Point(12, 20);
            this.flpConnectTo.Name = "flpConnectTo";
            this.flpConnectTo.Padding = new System.Windows.Forms.Padding(3);
            this.flpConnectTo.Size = new System.Drawing.Size(360, 146);
            this.flpConnectTo.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtHost);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(144, 31);
            this.panel1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 7);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Host:";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(38, 5);
            this.txtHost.Margin = new System.Windows.Forms.Padding(0, 5, 6, 6);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(100, 20);
            this.txtHost.TabIndex = 0;
            this.txtHost.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.nudPort);
            this.panel2.Location = new System.Drawing.Point(147, 3);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(92, 31);
            this.panel2.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 7);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Port:";
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(36, 5);
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
            5222,
            0,
            0,
            0});
            this.nudPort.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.txtUsername);
            this.panel3.Location = new System.Drawing.Point(3, 34);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(170, 31);
            this.panel3.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 7);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Username:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(64, 5);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(0, 5, 6, 6);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(100, 20);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.TextChanged += new System.EventHandler(this.txtUsername_TextChanged);
            this.txtUsername.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            // 
            // panel4
            // 
            this.panel4.AutoSize = true;
            this.panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel4.Controls.Add(this.label8);
            this.panel4.Controls.Add(this.txtPassword);
            this.panel4.Location = new System.Drawing.Point(173, 34);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(168, 31);
            this.panel4.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 7);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(62, 5);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(0, 5, 6, 6);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            // 
            // panel5
            // 
            this.panel5.AutoSize = true;
            this.panel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel5.Controls.Add(this.label9);
            this.panel5.Controls.Add(this.pnlBorderChannel);
            this.panel5.Location = new System.Drawing.Point(3, 65);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(161, 32);
            this.panel5.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 7);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Channel:";
            // 
            // pnlBorderChannel
            // 
            this.pnlBorderChannel.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderChannel.Controls.Add(this.cboChannel);
            this.pnlBorderChannel.Location = new System.Drawing.Point(55, 3);
            this.pnlBorderChannel.Margin = new System.Windows.Forms.Padding(0, 3, 6, 6);
            this.pnlBorderChannel.Name = "pnlBorderChannel";
            this.pnlBorderChannel.Size = new System.Drawing.Size(100, 23);
            this.pnlBorderChannel.TabIndex = 4;
            // 
            // cboChannel
            // 
            this.cboChannel.BackColor = System.Drawing.Color.White;
            this.cboChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChannel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboChannel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboChannel.FormattingEnabled = true;
            this.cboChannel.Items.AddRange(new object[] {
            "Stable",
            "Nightly"});
            this.cboChannel.Location = new System.Drawing.Point(1, 1);
            this.cboChannel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboChannel.Name = "cboChannel";
            this.cboChannel.Size = new System.Drawing.Size(98, 21);
            this.cboChannel.TabIndex = 0;
            this.cboChannel.SelectedIndexChanged += new System.EventHandler(this.cboChannel_SelectedIndexChanged);
            // 
            // btnUpdateManually
            // 
            this.btnUpdateManually.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpdateManually.AutoSize = true;
            this.btnUpdateManually.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUpdateManually.BackColor = System.Drawing.Color.White;
            this.btnUpdateManually.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateManually.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateManually.Location = new System.Drawing.Point(12, 206);
            this.btnUpdateManually.Margin = new System.Windows.Forms.Padding(0, 3, 6, 6);
            this.btnUpdateManually.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnUpdateManually.Name = "btnUpdateManually";
            this.btnUpdateManually.Size = new System.Drawing.Size(126, 24);
            this.btnUpdateManually.TabIndex = 2;
            this.btnUpdateManually.Text = "Update Manually...";
            this.btnUpdateManually.UseVisualStyleBackColor = false;
            this.btnUpdateManually.Click += new System.EventHandler(this.btnUpdateManually_Click);
            // 
            // UpdateNotifierPanel
            // 
            this.ClientSize = new System.Drawing.Size(384, 242);
            this.Controls.Add(this.flpConnectTo);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnUpdateManually);
            this.Name = "UpdateNotifierPanel";
            this.groupBox1.ResumeLayout(false);
            this.pnlRefresh.ResumeLayout(false);
            this.pnlRefresh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.flpConnectTo.ResumeLayout(false);
            this.flpConnectTo.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.pnlBorderChannel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel pnlRefresh;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.PictureBox pic;
        private System.Windows.Forms.FlowLayoutPanel flpConnectTo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel pnlBorderChannel;
        private System.Windows.Forms.ComboBox cboChannel;
        private System.Windows.Forms.Button btnUpdateManually;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
    }
}