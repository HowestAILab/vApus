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
            this.btnAddSlave = new System.Windows.Forms.Button();
            this.txtHostName = new System.Windows.Forms.TextBox();
            this.lblHostName = new System.Windows.Forms.Label();
            this.lblIP = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picSort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // flp
            // 
            this.flp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flp.AutoScroll = true;
            this.flp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flp.Location = new System.Drawing.Point(0, 37);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(692, 505);
            this.flp.TabIndex = 0;
            this.flp.Visible = false;
            // 
            // lblUsage
            // 
            this.lblUsage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblUsage.AutoSize = true;
            this.lblUsage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsage.ForeColor = System.Drawing.Color.DimGray;
            this.lblUsage.Location = new System.Drawing.Point(179, 258);
            this.lblUsage.Name = "lblUsage";
            this.lblUsage.Size = new System.Drawing.Size(333, 26);
            this.lblUsage.TabIndex = 4;
            this.lblUsage.Text = "Add Clients to the Distributed Test clicking the \'+ button\'.\r\nSelect a Client to " +
    "configure it.";
            this.lblUsage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // picSort
            // 
            this.picSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picSort.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picSort.Image = ((System.Drawing.Image)(resources.GetObject("picSort.Image")));
            this.picSort.Location = new System.Drawing.Point(676, 7);
            this.picSort.Margin = new System.Windows.Forms.Padding(0);
            this.picSort.Name = "picSort";
            this.picSort.Size = new System.Drawing.Size(16, 16);
            this.picSort.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picSort.TabIndex = 21;
            this.picSort.TabStop = false;
            this.toolTip.SetToolTip(this.picSort, "Sort on Port");
            this.picSort.Visible = false;
            this.picSort.Click += new System.EventHandler(this.picSort_Click);
            // 
            // picStatus
            // 
            this.picStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picStatus.Image = ((System.Drawing.Image)(resources.GetObject("picStatus.Image")));
            this.picStatus.Location = new System.Drawing.Point(410, 7);
            this.picStatus.Name = "picStatus";
            this.picStatus.Size = new System.Drawing.Size(16, 16);
            this.picStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picStatus.TabIndex = 25;
            this.picStatus.TabStop = false;
            this.toolTip.SetToolTip(this.picStatus, "Client Offline");
            this.picStatus.Visible = false;
            this.picStatus.Click += new System.EventHandler(this.picStatus_Click);
            // 
            // btnAddSlave
            // 
            this.btnAddSlave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSlave.BackColor = System.Drawing.Color.White;
            this.btnAddSlave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddSlave.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnAddSlave.FlatAppearance.BorderSize = 0;
            this.btnAddSlave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnAddSlave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnAddSlave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddSlave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddSlave.Image = ((System.Drawing.Image)(resources.GetObject("btnAddSlave.Image")));
            this.btnAddSlave.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnAddSlave.Location = new System.Drawing.Point(613, 3);
            this.btnAddSlave.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddSlave.Name = "btnAddSlave";
            this.btnAddSlave.Size = new System.Drawing.Size(63, 24);
            this.btnAddSlave.TabIndex = 3;
            this.btnAddSlave.Text = "Slave";
            this.btnAddSlave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddSlave.UseVisualStyleBackColor = false;
            this.btnAddSlave.Visible = false;
            this.btnAddSlave.Click += new System.EventHandler(this.btnAddSlave_Click);
            // 
            // txtHostName
            // 
            this.txtHostName.Location = new System.Drawing.Point(72, 6);
            this.txtHostName.Name = "txtHostName";
            this.txtHostName.Size = new System.Drawing.Size(200, 20);
            this.txtHostName.TabIndex = 0;
            this.txtHostName.Visible = false;
            this.txtHostName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtHostName_KeyUp);
            // 
            // lblHostName
            // 
            this.lblHostName.AutoSize = true;
            this.lblHostName.Location = new System.Drawing.Point(3, 9);
            this.lblHostName.Name = "lblHostName";
            this.lblHostName.Size = new System.Drawing.Size(63, 13);
            this.lblHostName.TabIndex = 24;
            this.lblHostName.Text = "Host Name:";
            this.lblHostName.Visible = false;
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(278, 9);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(20, 13);
            this.lblIP.TabIndex = 27;
            this.lblIP.Text = "IP:";
            this.lblIP.Visible = false;
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(304, 6);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(100, 20);
            this.txtIP.TabIndex = 1;
            this.txtIP.Visible = false;
            this.txtIP.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtIP_KeyUp);
            // 
            // ConfigureSlaves
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnAddSlave);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.picStatus);
            this.Controls.Add(this.lblHostName);
            this.Controls.Add(this.txtHostName);
            this.Controls.Add(this.picSort);
            this.Controls.Add(this.lblUsage);
            this.Controls.Add(this.flp);
            this.Name = "ConfigureSlaves";
            this.Size = new System.Drawing.Size(692, 542);
            ((System.ComponentModel.ISupportInitialize)(this.picSort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).EndInit();
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
        private System.Windows.Forms.Button btnAddSlave;
    }
}
