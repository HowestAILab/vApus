namespace vApus.DistributedTest
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
            this.pnlRunSync = new System.Windows.Forms.Panel();
            this.cboRunSync = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picAddTile = new System.Windows.Forms.PictureBox();
            this.nudMaxBreakOnLast = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.picStressTestStatus = new System.Windows.Forms.PictureBox();
            this.chkUseRDP = new System.Windows.Forms.CheckBox();
            this.pnlRunSync.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAddTile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxBreakOnLast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStressTestStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlRunSync
            // 
            this.pnlRunSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRunSync.BackColor = System.Drawing.Color.Silver;
            this.pnlRunSync.Controls.Add(this.cboRunSync);
            this.pnlRunSync.Location = new System.Drawing.Point(349, 6);
            this.pnlRunSync.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.pnlRunSync.Name = "pnlRunSync";
            this.pnlRunSync.Size = new System.Drawing.Size(127, 23);
            this.pnlRunSync.TabIndex = 1;
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
            this.cboRunSync.Click += new System.EventHandler(this._Enter);
            this.cboRunSync.Enter += new System.EventHandler(this._Enter);
            this.cboRunSync.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.cboRunSync.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
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
            this.label1.Size = new System.Drawing.Size(212, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Distributed test";
            this.label1.Click += new System.EventHandler(this._Enter);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // picAddTile
            // 
            this.picAddTile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picAddTile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAddTile.Image = ((System.Drawing.Image)(resources.GetObject("picAddTile.Image")));
            this.picAddTile.Location = new System.Drawing.Point(513, 10);
            this.picAddTile.Name = "picAddTile";
            this.picAddTile.Size = new System.Drawing.Size(16, 16);
            this.picAddTile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picAddTile.TabIndex = 20;
            this.picAddTile.TabStop = false;
            this.toolTip.SetToolTip(this.picAddTile, "Add tile <ctrl+i>.");
            this.picAddTile.Click += new System.EventHandler(this.picAddTile_Click);
            // 
            // nudMaxBreakOnLast
            // 
            this.nudMaxBreakOnLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudMaxBreakOnLast.Location = new System.Drawing.Point(475, 8);
            this.nudMaxBreakOnLast.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudMaxBreakOnLast.Name = "nudMaxBreakOnLast";
            this.nudMaxBreakOnLast.Size = new System.Drawing.Size(32, 20);
            this.nudMaxBreakOnLast.TabIndex = 26;
            this.toolTip.SetToolTip(this.nudMaxBreakOnLast, "Set the maximum number of reruns for a run in a tiles stress test for the Break on " +
        "Last Run Sync.\r\n(0 = infinite)");
            this.nudMaxBreakOnLast.Visible = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(292, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Run sync:";
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
            // picStressTestStatus
            // 
            this.picStressTestStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picStressTestStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picStressTestStatus.Location = new System.Drawing.Point(513, 10);
            this.picStressTestStatus.Name = "picStressTestStatus";
            this.picStressTestStatus.Size = new System.Drawing.Size(16, 16);
            this.picStressTestStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picStressTestStatus.TabIndex = 25;
            this.picStressTestStatus.TabStop = false;
            this.picStressTestStatus.Visible = false;
            this.picStressTestStatus.Click += new System.EventHandler(this._Enter);
            // 
            // chkUseRDP
            // 
            this.chkUseRDP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkUseRDP.AutoSize = true;
            this.chkUseRDP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkUseRDP.Location = new System.Drawing.Point(221, 9);
            this.chkUseRDP.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.chkUseRDP.Name = "chkUseRDP";
            this.chkUseRDP.Size = new System.Drawing.Size(68, 17);
            this.chkUseRDP.TabIndex = 0;
            this.chkUseRDP.Text = "Use RDP";
            this.chkUseRDP.UseVisualStyleBackColor = true;
            // 
            // DistributedTestTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUseRDP);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlRunSync);
            this.Controls.Add(this.picAddTile);
            this.Controls.Add(this.picStressTestStatus);
            this.Controls.Add(this.nudMaxBreakOnLast);
            this.Controls.Add(this.label2);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DistributedTestTreeViewItem";
            this.Size = new System.Drawing.Size(532, 35);
            this.Enter += new System.EventHandler(this._Enter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            this.pnlRunSync.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picAddTile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxBreakOnLast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStressTestStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlRunSync;
        private System.Windows.Forms.ComboBox cboRunSync;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picAddTile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picStressTestStatus;
        private System.Windows.Forms.CheckBox chkUseRDP;
        private System.Windows.Forms.NumericUpDown nudMaxBreakOnLast;

    }
}
