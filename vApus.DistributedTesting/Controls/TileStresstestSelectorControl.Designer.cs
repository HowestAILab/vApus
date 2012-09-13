namespace vApus.DistributedTesting
{
    partial class TileStresstestSelectorControl
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
            this.lblTileStresstest = new System.Windows.Forms.Label();
            this.pic = new System.Windows.Forms.PictureBox();
            this.pb = new vApus.Util.EventProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTileStresstest
            // 
            this.lblTileStresstest.AutoSize = true;
            this.lblTileStresstest.Location = new System.Drawing.Point(24, 5);
            this.lblTileStresstest.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTileStresstest.Name = "lblTileStresstest";
            this.lblTileStresstest.Size = new System.Drawing.Size(93, 16);
            this.lblTileStresstest.TabIndex = 0;
            this.lblTileStresstest.Text = "Tile Stresstest";
            this.lblTileStresstest.SizeChanged += new System.EventHandler(this.lbl_SizeChanged);
            this.lblTileStresstest.Click += new System.EventHandler(this.lblTileStresstest_Click);
            this.lblTileStresstest.Enter += new System.EventHandler(this.TileStresstsControl_Enter);
            // 
            // pic
            // 
            this.pic.Location = new System.Drawing.Point(4, 4);
            this.pic.Margin = new System.Windows.Forms.Padding(4);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(21, 20);
            this.pic.TabIndex = 1;
            this.pic.TabStop = false;
            // 
            // pb
            // 
            this.pb.BackColor = System.Drawing.SystemColors.Control;
            this.pb.BeginOfTimeFrame = new System.DateTime(((long)(0)));
            this.pb.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pb.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pb.EndOfTimeFrame = new System.DateTime(9999, 12, 31, 23, 59, 59, 999);
            this.pb.EventToolTip = true;
            this.pb.Location = new System.Drawing.Point(0, 31);
            this.pb.Margin = new System.Windows.Forms.Padding(0);
            this.pb.Name = "pb";
            this.pb.ProgressBarColor = System.Drawing.Color.SteelBlue;
            this.pb.Size = new System.Drawing.Size(127, 6);
            this.pb.TabIndex = 19;
            // 
            // TileStresstestSelectorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.Controls.Add(this.pic);
            this.Controls.Add(this.lblTileStresstest);
            this.Controls.Add(this.pb);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Blue;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TileStresstestSelectorControl";
            this.Size = new System.Drawing.Size(127, 37);
            this.Enter += new System.EventHandler(this.TileStresstsControl_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTileStresstest;
        private System.Windows.Forms.PictureBox pic;
        private vApus.Util.EventProgressBar pb;
    }
}
