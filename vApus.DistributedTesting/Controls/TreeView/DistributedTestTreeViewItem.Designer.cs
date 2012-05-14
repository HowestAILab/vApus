namespace vApus.DistributedTesting
{
    partial class DistributedTestTreeViewItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DistributedTestTreeViewItem));
            this.label3 = new System.Windows.Forms.Label();
            this.pnlRunSync = new System.Windows.Forms.Panel();
            this.cboRunSync = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pnlRunSync.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(3, 6);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 6, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 20);
            this.label3.TabIndex = 17;
            this.label3.Text = "Distributed Test";
            // 
            // pnlRunSync
            // 
            this.pnlRunSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRunSync.BackColor = System.Drawing.Color.Silver;
            this.pnlRunSync.Controls.Add(this.cboRunSync);
            this.pnlRunSync.Location = new System.Drawing.Point(468, 6);
            this.pnlRunSync.Name = "pnlRunSync";
            this.pnlRunSync.Size = new System.Drawing.Size(127, 23);
            this.pnlRunSync.TabIndex = 18;
            // 
            // cboRunSync
            // 
            this.cboRunSync.BackColor = System.Drawing.Color.White;
            this.cboRunSync.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRunSync.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboRunSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboRunSync.FormattingEnabled = true;
            this.cboRunSync.Items.AddRange(new object[] {
            "None",
            "Break on First",
            "Break on Last"});
            this.cboRunSync.Location = new System.Drawing.Point(1, 1);
            this.cboRunSync.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboRunSync.Name = "cboRunSync";
            this.cboRunSync.Size = new System.Drawing.Size(125, 21);
            this.cboRunSync.TabIndex = 0;
            this.toolTip.SetToolTip(this.cboRunSync, resources.GetString("cboRunSync.ToolTip"));
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(405, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Run Sync:";
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // DistributedTestTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlRunSync);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DistributedTestTreeViewItem";
            this.Size = new System.Drawing.Size(598, 35);
            this.pnlRunSync.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlRunSync;
        private System.Windows.Forms.ComboBox cboRunSync;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip;

    }
}
