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
            ((System.ComponentModel.ISupportInitialize)(this.picSort)).BeginInit();
            this.SuspendLayout();
            // 
            // flp
            // 
            this.flp.AutoScroll = true;
            this.flp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flp.Location = new System.Drawing.Point(0, 0);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(692, 542);
            this.flp.TabIndex = 0;
            this.flp.Visible = false;
            // 
            // lblUsage
            // 
            this.lblUsage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblUsage.AutoSize = true;
            this.lblUsage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsage.ForeColor = System.Drawing.Color.DimGray;
            this.lblUsage.Location = new System.Drawing.Point(182, 258);
            this.lblUsage.Name = "lblUsage";
            this.lblUsage.Size = new System.Drawing.Size(327, 26);
            this.lblUsage.TabIndex = 4;
            this.lblUsage.Text = "Add Client to the Distributed Test clicking the \'+ button\'.\r\nSelect a Client to c" +
    "onfigure it.";
            this.lblUsage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // picSort
            // 
            this.picSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picSort.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picSort.Image = ((System.Drawing.Image)(resources.GetObject("picSort.Image")));
            this.picSort.Location = new System.Drawing.Point(676, 3);
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
            // ConfigureSlaves
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.picSort);
            this.Controls.Add(this.lblUsage);
            this.Controls.Add(this.flp);
            this.Name = "ConfigureSlaves";
            this.Size = new System.Drawing.Size(692, 542);
            ((System.ComponentModel.ISupportInitialize)(this.picSort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flp;
        private System.Windows.Forms.Label lblUsage;
        private System.Windows.Forms.PictureBox picSort;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
