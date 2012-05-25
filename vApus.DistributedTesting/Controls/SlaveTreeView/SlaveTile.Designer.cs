namespace vApus.DistributedTesting
{
    partial class SlaveTile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SlaveTile));
            this.chkUse = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fixedNumericUpDown1 = new vApus.Util.FixedNumericUpDown();
            this.lblStatus = new System.Windows.Forms.Label();
            this.picStatur = new System.Windows.Forms.PictureBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.llblTest = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.fixedNumericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatur)).BeginInit();
            this.SuspendLayout();
            // 
            // chkUse
            // 
            this.chkUse.AutoSize = true;
            this.chkUse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkUse.Location = new System.Drawing.Point(304, 5);
            this.chkUse.Name = "chkUse";
            this.chkUse.Size = new System.Drawing.Size(42, 17);
            this.chkUse.TabIndex = 0;
            this.chkUse.Text = "Use";
            this.chkUse.UseVisualStyleBackColor = true;
            this.chkUse.CheckedChanged += new System.EventHandler(this.chkUse_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port: ";
            // 
            // fixedNumericUpDown1
            // 
            this.fixedNumericUpDown1.Location = new System.Drawing.Point(35, 33);
            this.fixedNumericUpDown1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.fixedNumericUpDown1.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.fixedNumericUpDown1.Minimum = new decimal(new int[] {
            1337,
            0,
            0,
            0});
            this.fixedNumericUpDown1.Name = "fixedNumericUpDown1";
            this.fixedNumericUpDown1.Size = new System.Drawing.Size(50, 20);
            this.fixedNumericUpDown1.TabIndex = 3;
            this.fixedNumericUpDown1.Value = new decimal(new int[] {
            1337,
            0,
            0,
            0});
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(32, 7);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(66, 13);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Client Offline";
            // 
            // picStatur
            // 
            this.picStatur.Image = global::vApus.DistributedTesting.Properties.Resources.Cancelled;
            this.picStatur.Location = new System.Drawing.Point(4, 4);
            this.picStatur.Margin = new System.Windows.Forms.Padding(4);
            this.picStatur.Name = "picStatur";
            this.picStatur.Size = new System.Drawing.Size(16, 16);
            this.picStatur.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picStatur.TabIndex = 6;
            this.picStatur.TabStop = false;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Cancelled.png");
            this.imageList.Images.SetKeyName(1, "Error.png");
            this.imageList.Images.SetKeyName(2, "Busy.png");
            this.imageList.Images.SetKeyName(3, "Busy2.png");
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(182, 35);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(16, 13);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "-1";
            // 
            // llblTest
            // 
            this.llblTest.AutoSize = true;
            this.llblTest.Location = new System.Drawing.Point(80, 62);
            this.llblTest.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.llblTest.Name = "llblTest";
            this.llblTest.Size = new System.Drawing.Size(16, 13);
            this.llblTest.TabIndex = 9;
            this.llblTest.TabStop = true;
            this.llblTest.Text = "...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(91, 35);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Processor Affinity:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 62);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Assigned Test:";
            // 
            // SlaveTile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.llblTest);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.picStatur);
            this.Controls.Add(this.fixedNumericUpDown1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkUse);
            this.Name = "SlaveTile";
            this.Size = new System.Drawing.Size(350, 84);
            ((System.ComponentModel.ISupportInitialize)(this.fixedNumericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatur)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkUse;
        private System.Windows.Forms.Label label1;
        private Util.FixedNumericUpDown fixedNumericUpDown1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.PictureBox picStatur;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel llblTest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
