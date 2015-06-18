namespace vApus.DistributedTest
{
    partial class TileTreeViewItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileTreeViewItem));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picDuplicate = new System.Windows.Forms.PictureBox();
            this.picDelete = new System.Windows.Forms.PictureBox();
            this.chk = new System.Windows.Forms.CheckBox();
            this.picAddTileStressTest = new System.Windows.Forms.PictureBox();
            this.lblTile = new System.Windows.Forms.Label();
            this.picCollapseExpand = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAddTileStressTest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCollapseExpand)).BeginInit();
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
            this.chk.Location = new System.Drawing.Point(8, 7);
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
            this.chk.MouseEnter += new System.EventHandler(this._MouseEnter);
            // 
            // picAddTileStressTest
            // 
            this.picAddTileStressTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picAddTileStressTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAddTileStressTest.Image = ((System.Drawing.Image)(resources.GetObject("picAddTileStressTest.Image")));
            this.picAddTileStressTest.Location = new System.Drawing.Point(513, 6);
            this.picAddTileStressTest.Name = "picAddTileStressTest";
            this.picAddTileStressTest.Size = new System.Drawing.Size(16, 16);
            this.picAddTileStressTest.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picAddTileStressTest.TabIndex = 17;
            this.picAddTileStressTest.TabStop = false;
            this.toolTip.SetToolTip(this.picAddTileStressTest, "Add tile stress test <ctrl+i>.");
            this.picAddTileStressTest.Visible = false;
            this.picAddTileStressTest.Click += new System.EventHandler(this.picAddTileStressTest_Click);
            // 
            // lblTile
            // 
            this.lblTile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTile.AutoEllipsis = true;
            this.lblTile.Location = new System.Drawing.Point(26, 6);
            this.lblTile.MinimumSize = new System.Drawing.Size(0, 13);
            this.lblTile.Name = "lblTile";
            this.lblTile.Size = new System.Drawing.Size(481, 13);
            this.lblTile.TabIndex = 19;
            this.lblTile.Click += new System.EventHandler(this._Enter);
            // 
            // picCollapseExpand
            // 
            this.picCollapseExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picCollapseExpand.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picCollapseExpand.Image = global::vApus.DistributedTest.Properties.Resources.Collapse_small;
            this.picCollapseExpand.Location = new System.Drawing.Point(579, 6);
            this.picCollapseExpand.Name = "picCollapseExpand";
            this.picCollapseExpand.Size = new System.Drawing.Size(16, 16);
            this.picCollapseExpand.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picCollapseExpand.TabIndex = 18;
            this.picCollapseExpand.TabStop = false;
            this.picCollapseExpand.Click += new System.EventHandler(this.picCollapseExpand_Click);
            this.picCollapseExpand.MouseEnter += new System.EventHandler(this._MouseEnter);
            // 
            // TileTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picCollapseExpand);
            this.Controls.Add(this.picAddTileStressTest);
            this.Controls.Add(this.picDuplicate);
            this.Controls.Add(this.chk);
            this.Controls.Add(this.picDelete);
            this.Controls.Add(this.lblTile);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TileTreeViewItem";
            this.Size = new System.Drawing.Size(598, 32);
            this.Click += new System.EventHandler(this._Enter);
            this.Enter += new System.EventHandler(this._Enter);
            this.MouseEnter += new System.EventHandler(this._MouseEnter);
            this.MouseLeave += new System.EventHandler(this._MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAddTileStressTest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCollapseExpand)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picDelete;
        private System.Windows.Forms.CheckBox chk;
        private System.Windows.Forms.PictureBox picDuplicate;
        private System.Windows.Forms.PictureBox picCollapseExpand;
        private System.Windows.Forms.Label lblTile;
        private System.Windows.Forms.PictureBox picAddTileStressTest;
    }
}
