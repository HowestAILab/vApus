namespace vApus.DistributedTesting
{
    partial class RemoteDesktopClient
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteDesktopClient));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.rdc = new vApus.Util.RemoteDesktopClient();
            this.SuspendLayout();
            // 
            // rdc
            // 
            this.rdc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdc.Location = new System.Drawing.Point(0, 0);
            this.rdc.Name = "rdc";
            this.rdc.Size = new System.Drawing.Size(1281, 820);
            this.rdc.TabIndex = 0;
            this.rdc.RdpException += new System.EventHandler<vApus.Util.RemoteDesktopClient.RdpExceptionEventArgs>(this.rdc_RdpException);
            this.rdc.AllConnectionsClosed += new System.EventHandler(this.rdc_AllConnectionsClosed);
            // 
            // RemoteDesktopClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1281, 820);
            this.Controls.Add(this.rdc);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "RemoteDesktopClient";
            this.Text = "Remote Desktop Client";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private Util.RemoteDesktopClient rdc;
    }
}