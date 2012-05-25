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
            this.flp = new System.Windows.Forms.FlowLayoutPanel();
            this.lblUsage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // flp
            // 
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
            // ConfigureSlaves
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblUsage);
            this.Controls.Add(this.flp);
            this.Name = "ConfigureSlaves";
            this.Size = new System.Drawing.Size(692, 542);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flp;
        private System.Windows.Forms.Label lblUsage;
    }
}
