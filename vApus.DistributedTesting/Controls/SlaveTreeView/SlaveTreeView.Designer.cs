namespace vApus.DistributedTesting
{
    partial class SlaveTreeView
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
            this.largeList = new vApus.Util.LargeList();
            this.SuspendLayout();
            // 
            // largeList
            // 
            this.largeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.largeList.Location = new System.Drawing.Point(0, 0);
            this.largeList.Name = "largeList";
            this.largeList.Size = new System.Drawing.Size(360, 433);
            this.largeList.SizeMode = vApus.Util.SizeMode.StretchHorizontal;
            this.largeList.TabIndex = 1;
            // 
            // SlaveTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.largeList);
            this.Name = "SlaveTreeView";
            this.Size = new System.Drawing.Size(360, 433);
            this.ResumeLayout(false);

        }

        #endregion

        private Util.LargeList largeList;
    }
}
