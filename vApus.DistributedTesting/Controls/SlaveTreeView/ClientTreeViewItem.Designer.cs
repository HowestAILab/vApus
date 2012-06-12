namespace vApus.DistributedTesting
{
    partial class ClientTreeViewItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientTreeViewItem));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.chk = new System.Windows.Forms.CheckBox();
            this.txtClient = new System.Windows.Forms.TextBox();
            this.lblClient = new System.Windows.Forms.Label();
            this.imageListStatus = new System.Windows.Forms.ImageList(this.components);
            this.picAddSlave = new System.Windows.Forms.PictureBox();
            this.picStatus = new System.Windows.Forms.PictureBox();
            this.picDuplicate = new System.Windows.Forms.PictureBox();
            this.picDelete = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picAddSlave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // chk
            // 
            this.chk.AutoSize = true;
            this.chk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chk.Location = new System.Drawing.Point(8, 7);
            this.chk.Name = "chk";
            this.chk.Size = new System.Drawing.Size(12, 11);
            this.chk.TabIndex = 11;
            this.chk.TabStop = false;
            this.toolTip.SetToolTip(this.chk, "Use <ctrl+u>");
            this.chk.UseVisualStyleBackColor = true;
            this.chk.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            this.chk.Enter += new System.EventHandler(this._Enter);
            this.chk.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.chk.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            this.chk.MouseEnter += new System.EventHandler(this._MouseEnter);
            // 
            // txtClient
            // 
            this.txtClient.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClient.Location = new System.Drawing.Point(117, 3);
            this.txtClient.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.txtClient.Name = "txtClient";
            this.txtClient.Size = new System.Drawing.Size(383, 20);
            this.txtClient.TabIndex = 0;
            this.txtClient.Visible = false;
            this.txtClient.Enter += new System.EventHandler(this._Enter);
            this.txtClient.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.txtClient.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            this.txtClient.Leave += new System.EventHandler(this.txtClient_Leave);
            // 
            // lblClient
            // 
            this.lblClient.AutoSize = true;
            this.lblClient.Location = new System.Drawing.Point(26, 6);
            this.lblClient.Name = "lblClient";
            this.lblClient.Size = new System.Drawing.Size(88, 13);
            this.lblClient.TabIndex = 19;
            this.lblClient.Text = "Host Name or IP:";
            this.lblClient.Click += new System.EventHandler(this._Enter);
            // 
            // imageListStatus
            // 
            this.imageListStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListStatus.ImageStream")));
            this.imageListStatus.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListStatus.Images.SetKeyName(0, "Cancelled.png");
            this.imageListStatus.Images.SetKeyName(1, "OK.png");
            this.imageListStatus.Images.SetKeyName(2, "Wait.png");
            this.imageListStatus.Images.SetKeyName(3, "Wait_2.png");
            this.imageListStatus.Images.SetKeyName(4, "Busy.png");
            this.imageListStatus.Images.SetKeyName(5, "Error.png");
            // 
            // picAddSlave
            // 
            this.picAddSlave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picAddSlave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAddSlave.Image = ((System.Drawing.Image)(resources.GetObject("picAddSlave.Image")));
            this.picAddSlave.Location = new System.Drawing.Point(506, 6);
            this.picAddSlave.Name = "picAddSlave";
            this.picAddSlave.Size = new System.Drawing.Size(16, 16);
            this.picAddSlave.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picAddSlave.TabIndex = 21;
            this.picAddSlave.TabStop = false;
            this.toolTip.SetToolTip(this.picAddSlave, "Add Slave<ctrl+i>");
            this.picAddSlave.Visible = false;
            this.picAddSlave.Click += new System.EventHandler(this.picAddSlave_Click);
            // 
            // picStatus
            // 
            this.picStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picStatus.Image = ((System.Drawing.Image)(resources.GetObject("picStatus.Image")));
            this.picStatus.Location = new System.Drawing.Point(578, 6);
            this.picStatus.Name = "picStatus";
            this.picStatus.Size = new System.Drawing.Size(16, 16);
            this.picStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picStatus.TabIndex = 20;
            this.picStatus.TabStop = false;
            this.toolTip.SetToolTip(this.picStatus, "Offline <f5>");
            this.picStatus.Click += new System.EventHandler(this.picStatus_Click);
            // 
            // picDuplicate
            // 
            this.picDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picDuplicate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picDuplicate.Image = ((System.Drawing.Image)(resources.GetObject("picDuplicate.Image")));
            this.picDuplicate.Location = new System.Drawing.Point(528, 6);
            this.picDuplicate.Name = "picDuplicate";
            this.picDuplicate.Size = new System.Drawing.Size(16, 16);
            this.picDuplicate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDuplicate.TabIndex = 17;
            this.picDuplicate.TabStop = false;
            this.toolTip.SetToolTip(this.picDuplicate, "Duplicate <ctrl+d>");
            this.picDuplicate.Visible = false;
            this.picDuplicate.Click += new System.EventHandler(this.picDuplicate_Click);
            // 
            // picDelete
            // 
            this.picDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picDelete.Image = ((System.Drawing.Image)(resources.GetObject("picDelete.Image")));
            this.picDelete.Location = new System.Drawing.Point(550, 6);
            this.picDelete.Name = "picDelete";
            this.picDelete.Size = new System.Drawing.Size(16, 16);
            this.picDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picDelete.TabIndex = 15;
            this.picDelete.TabStop = false;
            this.toolTip.SetToolTip(this.picDelete, "Remove <ctrl+r>");
            this.picDelete.Visible = false;
            this.picDelete.Click += new System.EventHandler(this.picDelete_Click);
            // 
            // ClientTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picAddSlave);
            this.Controls.Add(this.picStatus);
            this.Controls.Add(this.txtClient);
            this.Controls.Add(this.picDuplicate);
            this.Controls.Add(this.chk);
            this.Controls.Add(this.picDelete);
            this.Controls.Add(this.lblClient);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ClientTreeViewItem";
            this.Size = new System.Drawing.Size(598, 32);
            this.Enter += new System.EventHandler(this._Enter);
            this.MouseEnter += new System.EventHandler(this._MouseEnter);
            this.MouseLeave += new System.EventHandler(this._MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.picAddSlave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picDelete;
        private System.Windows.Forms.CheckBox chk;
        private System.Windows.Forms.TextBox txtClient;
        private System.Windows.Forms.PictureBox picDuplicate;
        private System.Windows.Forms.Label lblClient;
        private System.Windows.Forms.PictureBox picStatus;
        private System.Windows.Forms.ImageList imageListStatus;
        private System.Windows.Forms.PictureBox picAddSlave;
    }
}
