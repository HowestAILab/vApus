﻿namespace vApus.DistributedTest {
    partial class TileStressTestTreeViewItem {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
                try {
                    if (_timeSinceStartRun != null && _timeSinceStartRun.IsRunning)
                        _timeSinceStartRun.Stop();
                } catch { }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileStressTestTreeViewItem));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picDuplicate = new System.Windows.Forms.PictureBox();
            this.picDelete = new System.Windows.Forms.PictureBox();
            this.chk = new System.Windows.Forms.CheckBox();
            this.lblTileStressTest = new System.Windows.Forms.Label();
            this.picStressTestStatus = new System.Windows.Forms.PictureBox();
            this.eventProgressChart = new vApus.Util.EventProgressChart();
            this.lblExclamation = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStressTestStatus)).BeginInit();
            this.SuspendLayout();
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
            this.picDuplicate.Location = new System.Drawing.Point(535, 6);
            this.picDuplicate.Name = "picDuplicate";
            this.picDuplicate.Size = new System.Drawing.Size(16, 16);
            this.picDuplicate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDuplicate.TabIndex = 17;
            this.picDuplicate.TabStop = false;
            this.toolTip.SetToolTip(this.picDuplicate, "Duplicate <ctrl+d>.");
            this.picDuplicate.Visible = false;
            this.picDuplicate.Click += new System.EventHandler(this.picDuplicate_Click);
            // 
            // picDelete
            // 
            this.picDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picDelete.Image = ((System.Drawing.Image)(resources.GetObject("picDelete.Image")));
            this.picDelete.Location = new System.Drawing.Point(557, 6);
            this.picDelete.Name = "picDelete";
            this.picDelete.Size = new System.Drawing.Size(16, 16);
            this.picDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picDelete.TabIndex = 15;
            this.picDelete.TabStop = false;
            this.toolTip.SetToolTip(this.picDelete, "Remove <ctrl+r>.");
            this.picDelete.Visible = false;
            this.picDelete.Click += new System.EventHandler(this.picDelete_Click);
            // 
            // chk
            // 
            this.chk.AutoSize = true;
            this.chk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chk.Location = new System.Drawing.Point(22, 7);
            this.chk.Name = "chk";
            this.chk.Size = new System.Drawing.Size(12, 11);
            this.chk.TabIndex = 11;
            this.chk.TabStop = false;
            this.toolTip.SetToolTip(this.chk, "Use <ctrl+u>.");
            this.chk.UseVisualStyleBackColor = true;
            this.chk.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            this.chk.Enter += new System.EventHandler(this._Enter);
            this.chk.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.chk.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            // 
            // lblTileStressTest
            // 
            this.lblTileStressTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTileStressTest.AutoEllipsis = true;
            this.lblTileStressTest.Location = new System.Drawing.Point(40, 6);
            this.lblTileStressTest.MinimumSize = new System.Drawing.Size(0, 13);
            this.lblTileStressTest.Name = "lblTileStressTest";
            this.lblTileStressTest.Size = new System.Drawing.Size(489, 13);
            this.lblTileStressTest.TabIndex = 20;
            this.lblTileStressTest.Click += new System.EventHandler(this._Enter);
            this.lblTileStressTest.DoubleClick += new System.EventHandler(this._DoubleClick);
            // 
            // picStressTestStatus
            // 
            this.picStressTestStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picStressTestStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picStressTestStatus.Location = new System.Drawing.Point(578, 6);
            this.picStressTestStatus.Name = "picStressTestStatus";
            this.picStressTestStatus.Size = new System.Drawing.Size(16, 16);
            this.picStressTestStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picStressTestStatus.TabIndex = 22;
            this.picStressTestStatus.TabStop = false;
            this.picStressTestStatus.Click += new System.EventHandler(this._Enter);
            this.picStressTestStatus.DoubleClick += new System.EventHandler(this._DoubleClick);
            // 
            // eventProgressChart
            // 
            this.eventProgressChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventProgressChart.BeginOfTimeFrame = new System.DateTime(((long)(0)));
            this.eventProgressChart.BehaveAsBar = true;
            this.eventProgressChart.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.eventProgressChart.EventToolTip = true;
            this.eventProgressChart.Location = new System.Drawing.Point(42, 25);
            this.eventProgressChart.Margin = new System.Windows.Forms.Padding(0);
            this.eventProgressChart.Name = "eventProgressChart";
            this.eventProgressChart.ProgressBarColor = System.Drawing.Color.LightSteelBlue;
            this.eventProgressChart.Size = new System.Drawing.Size(552, 5);
            this.eventProgressChart.TabIndex = 21;
            this.eventProgressChart.Visible = false;
            this.eventProgressChart.EventClick += new System.EventHandler<vApus.Util.EventProgressChart.ProgressEventEventArgs>(this.eventProgressChart_EventClick);
            this.eventProgressChart.Enter += new System.EventHandler(this._Enter);
            // 
            // lblExclamation
            // 
            this.lblExclamation.AutoSize = true;
            this.lblExclamation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblExclamation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblExclamation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExclamation.Location = new System.Drawing.Point(8, 6);
            this.lblExclamation.Margin = new System.Windows.Forms.Padding(0);
            this.lblExclamation.Name = "lblExclamation";
            this.lblExclamation.Size = new System.Drawing.Size(11, 13);
            this.lblExclamation.TabIndex = 23;
            this.lblExclamation.Text = "!";
            this.lblExclamation.Visible = false;
            // 
            // TileStressTestTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblExclamation);
            this.Controls.Add(this.picDelete);
            this.Controls.Add(this.eventProgressChart);
            this.Controls.Add(this.lblTileStressTest);
            this.Controls.Add(this.picDuplicate);
            this.Controls.Add(this.chk);
            this.Controls.Add(this.picStressTestStatus);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TileStressTestTreeViewItem";
            this.Size = new System.Drawing.Size(598, 32);
            this.Click += new System.EventHandler(this._Enter);
            this.DoubleClick += new System.EventHandler(this._DoubleClick);
            this.Enter += new System.EventHandler(this._Enter);
            this.MouseEnter += new System.EventHandler(this._MouseEnter);
            this.MouseLeave += new System.EventHandler(this._MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStressTestStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picDelete;
        private System.Windows.Forms.CheckBox chk;
        private System.Windows.Forms.PictureBox picDuplicate;
        private System.Windows.Forms.Label lblTileStressTest;
        private Util.EventProgressChart eventProgressChart;
        private System.Windows.Forms.PictureBox picStressTestStatus;
        private System.Windows.Forms.Label lblExclamation;
    }
}
