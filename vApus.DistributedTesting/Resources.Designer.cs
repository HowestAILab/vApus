namespace vApus.DistributedTesting
{
    partial class Resources
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Press \'Refresh Resources\'");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Resources));
            this.pbRefreshResourcePool = new System.Windows.Forms.ProgressBar();
            this.slavesSplit = new System.Windows.Forms.SplitContainer();
            this.tvwResourcePool = new System.Windows.Forms.TreeView();
            this.resourceSolutionComponentPropertyPanel = new vApus.SolutionTree.SolutionComponentPropertyPanel();
            this.btnRefreshResourcePool = new System.Windows.Forms.Button();
            this.imageListResources = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.slavesSplit)).BeginInit();
            this.slavesSplit.Panel1.SuspendLayout();
            this.slavesSplit.Panel2.SuspendLayout();
            this.slavesSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbRefreshResourcePool
            // 
            this.pbRefreshResourcePool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbRefreshResourcePool.Location = new System.Drawing.Point(6, 450);
            this.pbRefreshResourcePool.Name = "pbRefreshResourcePool";
            this.pbRefreshResourcePool.Size = new System.Drawing.Size(697, 20);
            this.pbRefreshResourcePool.TabIndex = 11;
            // 
            // slavesSplit
            // 
            this.slavesSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.slavesSplit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.slavesSplit.Location = new System.Drawing.Point(6, 9);
            this.slavesSplit.Name = "slavesSplit";
            this.slavesSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // slavesSplit.Panel1
            // 
            this.slavesSplit.Panel1.Controls.Add(this.tvwResourcePool);
            // 
            // slavesSplit.Panel2
            // 
            this.slavesSplit.Panel2.Controls.Add(this.resourceSolutionComponentPropertyPanel);
            this.slavesSplit.Size = new System.Drawing.Size(697, 435);
            this.slavesSplit.SplitterDistance = 209;
            this.slavesSplit.TabIndex = 10;
            // 
            // tvwResourcePool
            // 
            this.tvwResourcePool.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwResourcePool.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvwResourcePool.ForeColor = System.Drawing.Color.Gray;
            this.tvwResourcePool.ImageIndex = 0;
            this.tvwResourcePool.ImageList = this.imageListResources;
            this.tvwResourcePool.Location = new System.Drawing.Point(3, 0);
            this.tvwResourcePool.Name = "tvwResourcePool";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Press \'Refresh Resources\'";
            this.tvwResourcePool.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvwResourcePool.SelectedImageIndex = 0;
            this.tvwResourcePool.Size = new System.Drawing.Size(692, 207);
            this.tvwResourcePool.TabIndex = 8;
            // 
            // resourceSolutionComponentPropertyPanel
            // 
            this.resourceSolutionComponentPropertyPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.resourceSolutionComponentPropertyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resourceSolutionComponentPropertyPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.resourceSolutionComponentPropertyPanel.Location = new System.Drawing.Point(0, 0);
            this.resourceSolutionComponentPropertyPanel.Name = "resourceSolutionComponentPropertyPanel";
            this.resourceSolutionComponentPropertyPanel.Size = new System.Drawing.Size(695, 220);
            this.resourceSolutionComponentPropertyPanel.SolutionComponent = null;
            this.resourceSolutionComponentPropertyPanel.TabIndex = 9;
            // 
            // btnRefreshResourcePool
            // 
            this.btnRefreshResourcePool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshResourcePool.Location = new System.Drawing.Point(6, 476);
            this.btnRefreshResourcePool.Name = "btnRefreshResourcePool";
            this.btnRefreshResourcePool.Size = new System.Drawing.Size(697, 23);
            this.btnRefreshResourcePool.TabIndex = 9;
            this.btnRefreshResourcePool.Text = "Refresh Resources";
            this.btnRefreshResourcePool.UseVisualStyleBackColor = true;
            this.btnRefreshResourcePool.Click += new System.EventHandler(this.btnRefreshResourcePool_Click);
            // 
            // imageListResources
            // 
            this.imageListResources.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListResources.ImageStream")));
            this.imageListResources.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListResources.Images.SetKeyName(0, "Offline.png");
            this.imageListResources.Images.SetKeyName(1, "OnlineComputer.png");
            this.imageListResources.Images.SetKeyName(2, "OnlineSlave.png");
            // 
            // Resources
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 509);
            this.Controls.Add(this.pbRefreshResourcePool);
            this.Controls.Add(this.slavesSplit);
            this.Controls.Add(this.btnRefreshResourcePool);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "Resources";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Resources";
            this.slavesSplit.Panel1.ResumeLayout(false);
            this.slavesSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.slavesSplit)).EndInit();
            this.slavesSplit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbRefreshResourcePool;
        private System.Windows.Forms.SplitContainer slavesSplit;
        private System.Windows.Forms.TreeView tvwResourcePool;
        private SolutionTree.SolutionComponentPropertyPanel resourceSolutionComponentPropertyPanel;
        private System.Windows.Forms.Button btnRefreshResourcePool;
        private System.Windows.Forms.ImageList imageListResources;

    }
}