namespace vApus.UpdateTool
{
    partial class Update
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Update));
            this.lvwUpdate = new vApus.Util.ExtendedListView();
            this.clmPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmLocalMD5Checksum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmRemoteMD5Checksum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmDownloadProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lvwUpdate
            // 
            this.lvwUpdate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwUpdate.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmPath,
            this.clmLocalMD5Checksum,
            this.clmRemoteMD5Checksum,
            this.clmDownloadProgress});
            this.lvwUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwUpdate.FullRowSelect = true;
            this.lvwUpdate.Location = new System.Drawing.Point(0, 0);
            this.lvwUpdate.MultiSelect = false;
            this.lvwUpdate.Name = "lvwUpdate";
            this.lvwUpdate.Size = new System.Drawing.Size(836, 536);
            this.lvwUpdate.TabIndex = 0;
            this.lvwUpdate.UseCompatibleStateImageBehavior = false;
            this.lvwUpdate.View = System.Windows.Forms.View.Details;
            // 
            // clmPath
            // 
            this.clmPath.Text = "Path";
            this.clmPath.Width = 430;
            // 
            // clmLocalMD5Checksum
            // 
            this.clmLocalMD5Checksum.Text = "Local MD5 Checksum";
            this.clmLocalMD5Checksum.Width = 117;
            // 
            // clmRemoteMD5Checksum
            // 
            this.clmRemoteMD5Checksum.Text = "Remote MD5 Checksum";
            this.clmRemoteMD5Checksum.Width = 128;
            // 
            // clmDownloadProgress
            // 
            this.clmDownloadProgress.Text = "";
            this.clmDownloadProgress.Width = 100;
            // 
            // Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(836, 536);
            this.Controls.Add(this.lvwUpdate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "Update";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "vApus Updater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Update_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private vApus.Util.ExtendedListView lvwUpdate;
        private System.Windows.Forms.ColumnHeader clmPath;
        private System.Windows.Forms.ColumnHeader clmRemoteMD5Checksum;
        private System.Windows.Forms.ColumnHeader clmLocalMD5Checksum;
        private System.Windows.Forms.ColumnHeader clmDownloadProgress;
    }
}