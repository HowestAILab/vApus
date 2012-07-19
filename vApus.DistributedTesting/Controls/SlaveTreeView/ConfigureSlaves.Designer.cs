namespace vApus.DistributedTesting
{
    partial class ConfigureSlaves
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureSlaves));
            this.flp = new System.Windows.Forms.FlowLayoutPanel();
            this.lblUsage = new System.Windows.Forms.Label();
            this.picSort = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picStatus = new System.Windows.Forms.PictureBox();
            this.picClearSlaves = new System.Windows.Forms.PictureBox();
            this.picShowRD = new System.Windows.Forms.PictureBox();
            this.picAddSlave = new System.Windows.Forms.PictureBox();
            this.txtHostName = new System.Windows.Forms.TextBox();
            this.lblHostName = new System.Windows.Forms.Label();
            this.lblIP = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lblRDCredentials = new System.Windows.Forms.Label();
            this.lblConnection = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlSettings = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picSort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picClearSlaves)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picShowRD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAddSlave)).BeginInit();
            this.pnlSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // flp
            // 
            this.flp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flp.AutoScroll = true;
            this.flp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flp.Location = new System.Drawing.Point(0, 100);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(1000, 500);
            this.flp.TabIndex = 1;
            this.flp.Visible = false;
            // 
            // lblUsage
            // 
            this.lblUsage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblUsage.AutoSize = true;
            this.lblUsage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsage.ForeColor = System.Drawing.Color.DimGray;
            this.lblUsage.Location = new System.Drawing.Point(333, 287);
            this.lblUsage.Name = "lblUsage";
            this.lblUsage.Size = new System.Drawing.Size(333, 26);
            this.lblUsage.TabIndex = 4;
            this.lblUsage.Text = "Add Clients to the Distributed Test clicking the \'+ button\'.\r\nSelect a Client to " +
    "configure it.";
            this.lblUsage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // picSort
            // 
            this.picSort.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picSort.Image = ((System.Drawing.Image)(resources.GetObject("picSort.Image")));
            this.picSort.Location = new System.Drawing.Point(766, 38);
            this.picSort.Margin = new System.Windows.Forms.Padding(0);
            this.picSort.Name = "picSort";
            this.picSort.Size = new System.Drawing.Size(16, 16);
            this.picSort.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picSort.TabIndex = 21;
            this.picSort.TabStop = false;
            this.toolTip.SetToolTip(this.picSort, "Sort on Port");
            this.picSort.Click += new System.EventHandler(this.picSort_Click);
            // 
            // picStatus
            // 
            this.picStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picStatus.Image = ((System.Drawing.Image)(resources.GetObject("picStatus.Image")));
            this.picStatus.Location = new System.Drawing.Point(181, 65);
            this.picStatus.Name = "picStatus";
            this.picStatus.Size = new System.Drawing.Size(16, 16);
            this.picStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picStatus.TabIndex = 25;
            this.picStatus.TabStop = false;
            this.toolTip.SetToolTip(this.picStatus, "Client Offline");
            this.picStatus.Click += new System.EventHandler(this.picStatus_Click);
            // 
            // picClearSlaves
            // 
            this.picClearSlaves.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picClearSlaves.Image = ((System.Drawing.Image)(resources.GetObject("picClearSlaves.Image")));
            this.picClearSlaves.Location = new System.Drawing.Point(741, 38);
            this.picClearSlaves.Margin = new System.Windows.Forms.Padding(0);
            this.picClearSlaves.Name = "picClearSlaves";
            this.picClearSlaves.Size = new System.Drawing.Size(16, 16);
            this.picClearSlaves.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picClearSlaves.TabIndex = 34;
            this.picClearSlaves.TabStop = false;
            this.toolTip.SetToolTip(this.picClearSlaves, "Clear Slaves");
            this.picClearSlaves.Click += new System.EventHandler(this.picClearSlaves_Click);
            // 
            // picShowRD
            // 
            this.picShowRD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picShowRD.Image = ((System.Drawing.Image)(resources.GetObject("picShowRD.Image")));
            this.picShowRD.Location = new System.Drawing.Point(537, 62);
            this.picShowRD.Margin = new System.Windows.Forms.Padding(0);
            this.picShowRD.Name = "picShowRD";
            this.picShowRD.Size = new System.Drawing.Size(16, 16);
            this.picShowRD.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picShowRD.TabIndex = 35;
            this.picShowRD.TabStop = false;
            this.toolTip.SetToolTip(this.picShowRD, "Show Remote Desktop...");
            this.picShowRD.Click += new System.EventHandler(this.picShowRD_Click);
            // 
            // picAddSlave
            // 
            this.picAddSlave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAddSlave.Image = ((System.Drawing.Image)(resources.GetObject("picAddSlave.Image")));
            this.picAddSlave.Location = new System.Drawing.Point(715, 37);
            this.picAddSlave.Margin = new System.Windows.Forms.Padding(0);
            this.picAddSlave.Name = "picAddSlave";
            this.picAddSlave.Size = new System.Drawing.Size(16, 16);
            this.picAddSlave.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picAddSlave.TabIndex = 36;
            this.picAddSlave.TabStop = false;
            this.toolTip.SetToolTip(this.picAddSlave, "Add Slave");
            this.picAddSlave.Click += new System.EventHandler(this.picAddSlave_Click);
            // 
            // txtHostName
            // 
            this.txtHostName.Location = new System.Drawing.Point(75, 36);
            this.txtHostName.Name = "txtHostName";
            this.txtHostName.Size = new System.Drawing.Size(200, 20);
            this.txtHostName.TabIndex = 0;
            this.txtHostName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtHostName_KeyUp);
            // 
            // lblHostName
            // 
            this.lblHostName.AutoSize = true;
            this.lblHostName.Location = new System.Drawing.Point(6, 39);
            this.lblHostName.Name = "lblHostName";
            this.lblHostName.Size = new System.Drawing.Size(63, 13);
            this.lblHostName.TabIndex = 24;
            this.lblHostName.Text = "Host Name:";
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(37, 65);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(32, 13);
            this.lblIP.TabIndex = 27;
            this.lblIP.Text = "or IP:";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(75, 62);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(100, 20);
            this.txtIP.TabIndex = 1;
            this.txtIP.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtIP_KeyUp);
            // 
            // lblRDCredentials
            // 
            this.lblRDCredentials.AutoSize = true;
            this.lblRDCredentials.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRDCredentials.Location = new System.Drawing.Point(359, 10);
            this.lblRDCredentials.Name = "lblRDCredentials";
            this.lblRDCredentials.Size = new System.Drawing.Size(168, 13);
            this.lblRDCredentials.TabIndex = 28;
            this.lblRDCredentials.Text = "Remote Desktop Credentials";
            // 
            // lblConnection
            // 
            this.lblConnection.AutoSize = true;
            this.lblConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnection.Location = new System.Drawing.Point(6, 10);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(71, 13);
            this.lblConnection.TabIndex = 29;
            this.lblConnection.Text = "Connection";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(428, 36);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(100, 20);
            this.txtUserName.TabIndex = 2;
            this.txtUserName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtRDC_KeyUp);
            this.txtUserName.Leave += new System.EventHandler(this.txtRDC_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(359, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "User Name:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(428, 62);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtRDC_KeyUp);
            this.txtPassword.Leave += new System.EventHandler(this.txtRDC_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(366, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "Password:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(534, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Domain:";
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(586, 36);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(100, 20);
            this.txtDomain.TabIndex = 3;
            this.txtDomain.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtRDC_KeyUp);
            this.txtDomain.Leave += new System.EventHandler(this.txtRDC_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(712, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Slaves";
            // 
            // pnlSettings
            // 
            this.pnlSettings.AutoScroll = true;
            this.pnlSettings.Controls.Add(this.txtDomain);
            this.pnlSettings.Controls.Add(this.picAddSlave);
            this.pnlSettings.Controls.Add(this.lblConnection);
            this.pnlSettings.Controls.Add(this.picShowRD);
            this.pnlSettings.Controls.Add(this.picSort);
            this.pnlSettings.Controls.Add(this.picClearSlaves);
            this.pnlSettings.Controls.Add(this.txtHostName);
            this.pnlSettings.Controls.Add(this.label4);
            this.pnlSettings.Controls.Add(this.lblHostName);
            this.pnlSettings.Controls.Add(this.label3);
            this.pnlSettings.Controls.Add(this.txtUserName);
            this.pnlSettings.Controls.Add(this.label1);
            this.pnlSettings.Controls.Add(this.picStatus);
            this.pnlSettings.Controls.Add(this.lblRDCredentials);
            this.pnlSettings.Controls.Add(this.txtIP);
            this.pnlSettings.Controls.Add(this.label2);
            this.pnlSettings.Controls.Add(this.txtPassword);
            this.pnlSettings.Controls.Add(this.lblIP);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(1000, 100);
            this.pnlSettings.TabIndex = 0;
            this.pnlSettings.Visible = false;
            // 
            // ConfigureSlaves
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblUsage);
            this.Controls.Add(this.flp);
            this.Controls.Add(this.pnlSettings);
            this.Name = "ConfigureSlaves";
            this.Size = new System.Drawing.Size(1000, 600);
            ((System.ComponentModel.ISupportInitialize)(this.picSort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picClearSlaves)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picShowRD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAddSlave)).EndInit();
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flp;
        private System.Windows.Forms.Label lblUsage;
        private System.Windows.Forms.PictureBox picSort;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TextBox txtHostName;
        private System.Windows.Forms.Label lblHostName;
        private System.Windows.Forms.PictureBox picStatus;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lblRDCredentials;
        private System.Windows.Forms.Label lblConnection;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox picClearSlaves;
        private System.Windows.Forms.PictureBox picShowRD;
        private System.Windows.Forms.PictureBox picAddSlave;
        private System.Windows.Forms.Panel pnlSettings;
    }
}
