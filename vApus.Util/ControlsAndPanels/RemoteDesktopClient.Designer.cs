namespace vApus.Util
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
            this.tc = new vApus.Util.TabControlWithAdjustableBorders();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // tc
            // 
            this.tc.BottomVisible = false;
            this.tc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tc.LeftVisible = false;
            this.tc.Location = new System.Drawing.Point(0, 0);
            this.tc.Name = "tc";
            this.tc.RightVisible = false;
            this.tc.SelectedIndex = 0;
            this.tc.Size = new System.Drawing.Size(1281, 820);
            this.tc.TabIndex = 0;
            this.toolTip.SetToolTip(this.tc, "Right-click a tab page header to close the remote desktop.");
            this.tc.TopVisible = false;
            this.tc.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tc_MouseUp);
            // 
            // RemoteDesktopClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1281, 820);
            this.Controls.Add(this.tc);
            this.Name = "RemoteDesktopClient";
            this.Text = "Remote Desktop Client";
            this.ResumeLayout(false);

        }

        #endregion

        private Util.TabControlWithAdjustableBorders tc;
        private System.Windows.Forms.ToolTip toolTip;
    }
}