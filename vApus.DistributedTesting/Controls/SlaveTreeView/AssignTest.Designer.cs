namespace vApus.DistributedTesting
{
    partial class AssignTest
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvwTiles = new System.Windows.Forms.TreeView();
            this.tvwTileStresstests = new System.Windows.Forms.TreeView();
            this.btnAssign = new System.Windows.Forms.Button();
            this.btnAssignAndGoTo = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvwTiles);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tvwTileStresstests);
            this.splitContainer1.Size = new System.Drawing.Size(484, 270);
            this.splitContainer1.SplitterDistance = 160;
            this.splitContainer1.TabIndex = 0;
            // 
            // tvwTiles
            // 
            this.tvwTiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvwTiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwTiles.HideSelection = false;
            this.tvwTiles.Location = new System.Drawing.Point(0, 0);
            this.tvwTiles.Name = "tvwTiles";
            this.tvwTiles.Size = new System.Drawing.Size(160, 270);
            this.tvwTiles.TabIndex = 0;
            this.tvwTiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwTiles_AfterSelect);
            // 
            // tvwTileStresstests
            // 
            this.tvwTileStresstests.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvwTileStresstests.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwTileStresstests.HideSelection = false;
            this.tvwTileStresstests.Location = new System.Drawing.Point(0, 0);
            this.tvwTileStresstests.Name = "tvwTileStresstests";
            this.tvwTileStresstests.Size = new System.Drawing.Size(320, 270);
            this.tvwTileStresstests.TabIndex = 1;
            this.tvwTileStresstests.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwTileStresstests_AfterSelect);
            // 
            // btnAssign
            // 
            this.btnAssign.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAssign.AutoSize = true;
            this.btnAssign.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAssign.BackColor = System.Drawing.Color.White;
            this.btnAssign.Enabled = false;
            this.btnAssign.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAssign.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAssign.Location = new System.Drawing.Point(296, 276);
            this.btnAssign.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnAssign.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnAssign.Name = "btnAssign";
            this.btnAssign.Size = new System.Drawing.Size(56, 24);
            this.btnAssign.TabIndex = 1;
            this.btnAssign.Text = "Assign";
            this.btnAssign.UseVisualStyleBackColor = false;
            this.btnAssign.Click += new System.EventHandler(this.btnAssign_Click);
            // 
            // btnAssignAndGoTo
            // 
            this.btnAssignAndGoTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAssignAndGoTo.AutoSize = true;
            this.btnAssignAndGoTo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAssignAndGoTo.BackColor = System.Drawing.Color.White;
            this.btnAssignAndGoTo.Enabled = false;
            this.btnAssignAndGoTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAssignAndGoTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAssignAndGoTo.Location = new System.Drawing.Point(358, 276);
            this.btnAssignAndGoTo.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnAssignAndGoTo.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnAssignAndGoTo.Name = "btnAssignAndGoTo";
            this.btnAssignAndGoTo.Size = new System.Drawing.Size(114, 24);
            this.btnAssignAndGoTo.TabIndex = 2;
            this.btnAssignAndGoTo.Text = "Assign and go to";
            this.btnAssignAndGoTo.UseVisualStyleBackColor = false;
            this.btnAssignAndGoTo.Click += new System.EventHandler(this.btnAssignAndGoTo_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.AutoSize = true;
            this.btnClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClear.BackColor = System.Drawing.Color.White;
            this.btnClear.Enabled = false;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(12, 276);
            this.btnClear.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnClear.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 24);
            this.btnClear.TabIndex = 0;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // AssignTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 312);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnAssignAndGoTo);
            this.Controls.Add(this.btnAssign);
            this.Controls.Add(this.splitContainer1);
            this.MinimizeBox = false;
            this.Name = "AssignTest";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvwTiles;
        private System.Windows.Forms.TreeView tvwTileStresstests;
        private System.Windows.Forms.Button btnAssign;
        private System.Windows.Forms.Button btnAssignAndGoTo;
        private System.Windows.Forms.Button btnClear;
    }
}