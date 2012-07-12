namespace vApus.DistributedTesting
{
    partial class ClientsAndSlavesTreeViewItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientsAndSlavesTreeViewItem));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picAddClient = new System.Windows.Forms.PictureBox();
            this.picRefresh = new System.Windows.Forms.PictureBox();
            this.lblRunSync = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.picWizard = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picAddClient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWizard)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // picAddClient
            // 
            this.picAddClient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picAddClient.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAddClient.Image = ((System.Drawing.Image)(resources.GetObject("picAddClient.Image")));
            this.picAddClient.Location = new System.Drawing.Point(451, 6);
            this.picAddClient.Name = "picAddClient";
            this.picAddClient.Size = new System.Drawing.Size(23, 23);
            this.picAddClient.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picAddClient.TabIndex = 20;
            this.picAddClient.TabStop = false;
            this.toolTip.SetToolTip(this.picAddClient, "Add Client <ctrl+i>");
            this.picAddClient.Click += new System.EventHandler(this.picAddClient_Click);
            // 
            // picRefresh
            // 
            this.picRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picRefresh.Image = ((System.Drawing.Image)(resources.GetObject("picRefresh.Image")));
            this.picRefresh.Location = new System.Drawing.Point(509, 6);
            this.picRefresh.Name = "picRefresh";
            this.picRefresh.Size = new System.Drawing.Size(23, 23);
            this.picRefresh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picRefresh.TabIndex = 24;
            this.picRefresh.TabStop = false;
            this.toolTip.SetToolTip(this.picRefresh, "Refresh<f5>");
            this.picRefresh.Click += new System.EventHandler(this.picRefresh_Click);
            // 
            // lblRunSync
            // 
            this.lblRunSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRunSync.AutoSize = true;
            this.lblRunSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRunSync.Location = new System.Drawing.Point(378, 11);
            this.lblRunSync.Name = "lblRunSync";
            this.lblRunSync.Size = new System.Drawing.Size(0, 13);
            this.lblRunSync.TabIndex = 21;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(532, 2);
            this.panel1.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoEllipsis = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.MinimumSize = new System.Drawing.Size(0, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(442, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Clients and Slaves";
            this.label1.Click += new System.EventHandler(this._Enter);
            // 
            // picWizard
            // 
            this.picWizard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picWizard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picWizard.Image = ((System.Drawing.Image)(resources.GetObject("picWizard.Image")));
            this.picWizard.Location = new System.Drawing.Point(480, 6);
            this.picWizard.Name = "picWizard";
            this.picWizard.Size = new System.Drawing.Size(23, 23);
            this.picWizard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picWizard.TabIndex = 25;
            this.picWizard.TabStop = false;
            this.toolTip.SetToolTip(this.picWizard, "Wizard...");
            this.picWizard.Click += new System.EventHandler(this.picWizard_Click);
            // 
            // ClientsAndSlavesTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picWizard);
            this.Controls.Add(this.picRefresh);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picAddClient);
            this.Controls.Add(this.lblRunSync);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ClientsAndSlavesTreeViewItem";
            this.Size = new System.Drawing.Size(532, 35);
            this.Click += new System.EventHandler(this.ClientsAndSlavesTreeViewItem_Click);
            this.Enter += new System.EventHandler(this._Enter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.picAddClient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWizard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picAddClient;
        private System.Windows.Forms.Label lblRunSync;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picRefresh;
        private System.Windows.Forms.PictureBox picWizard;

    }
}
