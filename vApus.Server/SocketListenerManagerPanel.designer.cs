namespace vApus.Server
{
	partial class SocketListenerManagerPanel
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
            this.components = new System.ComponentModel.Container();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnRestart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.llblAvailableIPAddresses = new System.Windows.Forms.LinkLabel();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.btnCopyHost = new System.Windows.Forms.Button();
            this.chkPreferred = new System.Windows.Forms.CheckBox();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.btnSet = new System.Windows.Forms.Button();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblIP = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStart.BackColor = System.Drawing.Color.White;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(173, 327);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 24);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStop.BackColor = System.Drawing.Color.White;
            this.btnStop.Enabled = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(335, 327);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 24);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnRestart
            // 
            this.btnRestart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRestart.BackColor = System.Drawing.Color.White;
            this.btnRestart.Enabled = false;
            this.btnRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestart.Location = new System.Drawing.Point(254, 327);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(75, 24);
            this.btnRestart.TabIndex = 2;
            this.btnRestart.Text = "Restart";
            this.btnRestart.UseVisualStyleBackColor = false;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(262, 26);
            this.label1.TabIndex = 3;
            this.label1.Text = "Used for managing the socket listener for master-slave\r\ncommunication.";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(6, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Status:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.llblAvailableIPAddresses);
            this.groupBox1.Controls.Add(this.txtHost);
            this.groupBox1.Controls.Add(this.btnCopyHost);
            this.groupBox1.Controls.Add(this.chkPreferred);
            this.groupBox1.Controls.Add(this.nudPort);
            this.groupBox1.Controls.Add(this.btnSet);
            this.groupBox1.Controls.Add(this.lblPort);
            this.groupBox1.Controls.Add(this.lblIP);
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Location = new System.Drawing.Point(12, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(561, 283);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // llblAvailableIPAddresses
            // 
            this.llblAvailableIPAddresses.ActiveLinkColor = System.Drawing.Color.Blue;
            this.llblAvailableIPAddresses.AutoSize = true;
            this.llblAvailableIPAddresses.Location = new System.Drawing.Point(38, 60);
            this.llblAvailableIPAddresses.Name = "llblAvailableIPAddresses";
            this.llblAvailableIPAddresses.Size = new System.Drawing.Size(78, 13);
            this.llblAvailableIPAddresses.TabIndex = 6;
            this.llblAvailableIPAddresses.TabStop = true;
            this.llblAvailableIPAddresses.Text = "IP Addresses...";
            this.llblAvailableIPAddresses.VisitedLinkColor = System.Drawing.Color.Blue;
            this.llblAvailableIPAddresses.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblIPAddresses_LinkClicked);
            // 
            // txtHost
            // 
            this.txtHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHost.BackColor = System.Drawing.Color.White;
            this.txtHost.Location = new System.Drawing.Point(41, 37);
            this.txtHost.Name = "txtHost";
            this.txtHost.ReadOnly = true;
            this.txtHost.Size = new System.Drawing.Size(422, 20);
            this.txtHost.TabIndex = 5;
            // 
            // btnCopyHost
            // 
            this.btnCopyHost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyHost.AutoSize = true;
            this.btnCopyHost.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCopyHost.BackColor = System.Drawing.Color.White;
            this.btnCopyHost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopyHost.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopyHost.Location = new System.Drawing.Point(469, 36);
            this.btnCopyHost.MaximumSize = new System.Drawing.Size(47, 24);
            this.btnCopyHost.Name = "btnCopyHost";
            this.btnCopyHost.Size = new System.Drawing.Size(47, 24);
            this.btnCopyHost.TabIndex = 1;
            this.btnCopyHost.Text = "Copy";
            this.btnCopyHost.UseVisualStyleBackColor = false;
            this.btnCopyHost.Click += new System.EventHandler(this.btnCopyHost_Click);
            // 
            // chkPreferred
            // 
            this.chkPreferred.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPreferred.AutoSize = true;
            this.chkPreferred.Location = new System.Drawing.Point(41, 120);
            this.chkPreferred.Name = "chkPreferred";
            this.chkPreferred.Size = new System.Drawing.Size(138, 17);
            this.chkPreferred.TabIndex = 3;
            this.chkPreferred.Text = "Preferred (now -> [Port])";
            this.toolTip.SetToolTip(this.chkPreferred, "If possible the socket will listen on this IP and port on the next startup.");
            this.chkPreferred.UseVisualStyleBackColor = true;
            this.chkPreferred.CheckedChanged += new System.EventHandler(this.chkPreferred_CheckedChanged);
            // 
            // nudPort
            // 
            this.nudPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPort.Location = new System.Drawing.Point(41, 90);
            this.nudPort.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nudPort.Minimum = new decimal(new int[] {
            1337,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(475, 20);
            this.nudPort.TabIndex = 2;
            this.nudPort.Value = new decimal(new int[] {
            1337,
            0,
            0,
            0});
            this.nudPort.ValueChanged += new System.EventHandler(this.nudPort_ValueChanged);
            // 
            // btnSet
            // 
            this.btnSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSet.BackColor = System.Drawing.Color.White;
            this.btnSet.Enabled = false;
            this.btnSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSet.Location = new System.Drawing.Point(41, 221);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(475, 24);
            this.btnSet.TabIndex = 4;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = false;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(6, 92);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 13);
            this.lblPort.TabIndex = 4;
            this.lblPort.Text = "Port:";
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(6, 40);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(32, 13);
            this.lblIP.TabIndex = 4;
            this.lblIP.Text = "Host:";
            // 
            // SocketListenerManagerPanel
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(585, 362);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "SocketListenerManagerPanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnRestart;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.CheckBox chkPreferred;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnCopyHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.LinkLabel llblAvailableIPAddresses;
	}
}