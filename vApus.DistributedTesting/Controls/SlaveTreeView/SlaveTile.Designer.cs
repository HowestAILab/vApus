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
            this.nudPort = new vApus.Util.FixedNumericUpDown();
            this.picStatus = new System.Windows.Forms.PictureBox();
            this.llblPA = new System.Windows.Forms.LinkLabel();
            this.llblTest = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picDuplicate = new System.Windows.Forms.PictureBox();
            this.picDelete = new System.Windows.Forms.PictureBox();
            this.imageListStatus = new System.Windows.Forms.ImageList(this.components);
            this.tmrRotateStatus = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).BeginInit();
            this.SuspendLayout();
            // 
            // chkUse
            // 
            this.chkUse.AutoSize = true;
            this.chkUse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkUse.Location = new System.Drawing.Point(3, 8);
            this.chkUse.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.chkUse.Name = "chkUse";
            this.chkUse.Size = new System.Drawing.Size(12, 11);
            this.chkUse.TabIndex = 0;
            this.chkUse.UseVisualStyleBackColor = true;
            this.chkUse.CheckedChanged += new System.EventHandler(this.chkUse_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Slave on Port: ";
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(95, 5);
            this.nudPort.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.nudPort.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudPort.Minimum = new decimal(new int[] {
            1337,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(50, 20);
            this.nudPort.TabIndex = 3;
            this.nudPort.Value = new decimal(new int[] {
            1337,
            0,
            0,
            0});
            this.nudPort.ValueChanged += new System.EventHandler(this.nudPort_ValueChanged);
            // 
            // picStatus
            // 
            this.picStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picStatus.Image = global::vApus.DistributedTesting.Properties.Resources.Cancelled;
            this.picStatus.Location = new System.Drawing.Point(330, 7);
            this.picStatus.Margin = new System.Windows.Forms.Padding(4);
            this.picStatus.Name = "picStatus";
            this.picStatus.Size = new System.Drawing.Size(16, 16);
            this.picStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picStatus.TabIndex = 6;
            this.picStatus.TabStop = false;
            this.toolTip.SetToolTip(this.picStatus, "Client Offline");
            // 
            // llblPA
            // 
            this.llblPA.AutoSize = true;
            this.llblPA.Location = new System.Drawing.Point(95, 36);
            this.llblPA.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.llblPA.Name = "llblPA";
            this.llblPA.Size = new System.Drawing.Size(16, 13);
            this.llblPA.TabIndex = 8;
            this.llblPA.TabStop = true;
            this.llblPA.Text = "...";
            this.llblPA.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblPA_LinkClicked);
            // 
            // llblTest
            // 
            this.llblTest.AutoSize = true;
            this.llblTest.Location = new System.Drawing.Point(95, 62);
            this.llblTest.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.llblTest.Name = "llblTest";
            this.llblTest.Size = new System.Drawing.Size(16, 13);
            this.llblTest.TabIndex = 9;
            this.llblTest.TabStop = true;
            this.llblTest.Text = "...";
            this.llblTest.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblTest_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 36);
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
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // picDuplicate
            // 
            this.picDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picDuplicate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picDuplicate.Image = ((System.Drawing.Image)(resources.GetObject("picDuplicate.Image")));
            this.picDuplicate.Location = new System.Drawing.Point(283, 7);
            this.picDuplicate.Name = "picDuplicate";
            this.picDuplicate.Size = new System.Drawing.Size(16, 16);
            this.picDuplicate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDuplicate.TabIndex = 19;
            this.picDuplicate.TabStop = false;
            this.toolTip.SetToolTip(this.picDuplicate, "Duplicate <ctrl+d>");
            this.picDuplicate.Click += new System.EventHandler(this.picDuplicate_Click);
            // 
            // picDelete
            // 
            this.picDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picDelete.Image = ((System.Drawing.Image)(resources.GetObject("picDelete.Image")));
            this.picDelete.Location = new System.Drawing.Point(305, 7);
            this.picDelete.Name = "picDelete";
            this.picDelete.Size = new System.Drawing.Size(16, 16);
            this.picDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picDelete.TabIndex = 18;
            this.picDelete.TabStop = false;
            this.toolTip.SetToolTip(this.picDelete, "Remove <ctrl+r>");
            this.picDelete.Click += new System.EventHandler(this.picDelete_Click);
            // 
            // imageListStatus
            // 
            this.imageListStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListStatus.ImageStream")));
            this.imageListStatus.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListStatus.Images.SetKeyName(0, "Cancelled.png");
            this.imageListStatus.Images.SetKeyName(1, "ok.png");
            this.imageListStatus.Images.SetKeyName(2, "Busy.png");
            this.imageListStatus.Images.SetKeyName(3, "Busy2.png");
            this.imageListStatus.Images.SetKeyName(4, "Error.png");
            // 
            // tmrRotateStatus
            // 
            this.tmrRotateStatus.Interval = 500;
            // 
            // SlaveTile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.picDuplicate);
            this.Controls.Add(this.picDelete);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.llblTest);
            this.Controls.Add(this.llblPA);
            this.Controls.Add(this.picStatus);
            this.Controls.Add(this.nudPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkUse);
            this.Name = "SlaveTile";
            this.Size = new System.Drawing.Size(350, 84);
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkUse;
        private System.Windows.Forms.Label label1;
        private Util.FixedNumericUpDown nudPort;
        private System.Windows.Forms.PictureBox picStatus;
        private System.Windows.Forms.LinkLabel llblPA;
        private System.Windows.Forms.LinkLabel llblTest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ImageList imageListStatus;
        private System.Windows.Forms.Timer tmrRotateStatus;
        private System.Windows.Forms.PictureBox picDuplicate;
        private System.Windows.Forms.PictureBox picDelete;
    }
}
