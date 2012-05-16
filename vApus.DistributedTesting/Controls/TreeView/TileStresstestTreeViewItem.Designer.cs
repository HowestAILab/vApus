namespace vApus.DistributedTesting
{
    partial class TileStresstestTreeViewItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileStresstestTreeViewItem));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picDuplicate = new System.Windows.Forms.PictureBox();
            this.picDelete = new System.Windows.Forms.PictureBox();
            this.chk = new System.Windows.Forms.CheckBox();
            this.txtTileStresstest = new System.Windows.Forms.TextBox();
            this.lblTileStresstest = new System.Windows.Forms.Label();
            this.eventProgressBar = new vApus.Util.EventProgressBar();
            this.picStresstestStatus = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStresstestStatus)).BeginInit();
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
            this.toolTip.SetToolTip(this.picDuplicate, "Duplicate <ctrl+d>");
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
            this.toolTip.SetToolTip(this.picDelete, "Remove <ctrl+r>");
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
            this.toolTip.SetToolTip(this.chk, "Use <ctrl+u>");
            this.chk.UseVisualStyleBackColor = true;
            this.chk.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            this.chk.Enter += new System.EventHandler(this._Enter);
            this.chk.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.chk.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            this.chk.Leave += new System.EventHandler(this._Leave);
            // 
            // txtTileStresstest
            // 
            this.txtTileStresstest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTileStresstest.Location = new System.Drawing.Point(40, 3);
            this.txtTileStresstest.Name = "txtTileStresstest";
            this.txtTileStresstest.Size = new System.Drawing.Size(489, 20);
            this.txtTileStresstest.TabIndex = 0;
            this.txtTileStresstest.Visible = false;
            this.txtTileStresstest.Enter += new System.EventHandler(this._Enter);
            this.txtTileStresstest.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.txtTileStresstest.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            this.txtTileStresstest.Leave += new System.EventHandler(this._Leave);
            // 
            // lblTileStresstest
            // 
            this.lblTileStresstest.AutoSize = true;
            this.lblTileStresstest.Location = new System.Drawing.Point(40, 6);
            this.lblTileStresstest.Name = "lblTileStresstest";
            this.lblTileStresstest.Size = new System.Drawing.Size(0, 13);
            this.lblTileStresstest.TabIndex = 20;
            // 
            // eventProgressBar
            // 
            this.eventProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventProgressBar.BeginOfTimeFrame = new System.DateTime(((long)(0)));
            this.eventProgressBar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.eventProgressBar.EndOfTimeFrame = new System.DateTime(2012, 5, 16, 8, 0, 0, 0);
            this.eventProgressBar.EventToolTip = true;
            this.eventProgressBar.Location = new System.Drawing.Point(22, 23);
            this.eventProgressBar.Margin = new System.Windows.Forms.Padding(0);
            this.eventProgressBar.Name = "eventProgressBar";
            this.eventProgressBar.ProgressBarColor = System.Drawing.Color.SteelBlue;
            this.eventProgressBar.Size = new System.Drawing.Size(551, 6);
            this.eventProgressBar.TabIndex = 21;
            this.eventProgressBar.Visible = false;
            // 
            // picStresstestStatus
            // 
            this.picStresstestStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picStresstestStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picStresstestStatus.Image = global::vApus.DistributedTesting.Properties.Resources.Busy;
            this.picStresstestStatus.Location = new System.Drawing.Point(22, 6);
            this.picStresstestStatus.Name = "picStresstestStatus";
            this.picStresstestStatus.Size = new System.Drawing.Size(16, 16);
            this.picStresstestStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picStresstestStatus.TabIndex = 22;
            this.picStresstestStatus.TabStop = false;
            this.picStresstestStatus.Visible = false;
            this.picStresstestStatus.Click += new System.EventHandler(this.picStresstestStatus_Click);
            // 
            // TileStresstestTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.eventProgressBar);
            this.Controls.Add(this.txtTileStresstest);
            this.Controls.Add(this.lblTileStresstest);
            this.Controls.Add(this.picDuplicate);
            this.Controls.Add(this.chk);
            this.Controls.Add(this.picDelete);
            this.Controls.Add(this.picStresstestStatus);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TileStresstestTreeViewItem";
            this.Size = new System.Drawing.Size(598, 32);
            this.Enter += new System.EventHandler(this._Enter);
            this.Leave += new System.EventHandler(this._Leave);
            this.MouseEnter += new System.EventHandler(this._MouseEnter);
            this.MouseLeave += new System.EventHandler(this._MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStresstestStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picDelete;
        private System.Windows.Forms.CheckBox chk;
        private System.Windows.Forms.TextBox txtTileStresstest;
        private System.Windows.Forms.PictureBox picDuplicate;
        private System.Windows.Forms.Label lblTileStresstest;
        private Util.EventProgressBar eventProgressBar;
        private System.Windows.Forms.PictureBox picStresstestStatus;
    }
}
