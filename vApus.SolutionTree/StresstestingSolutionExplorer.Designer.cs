namespace vApus.SolutionTree
{
    partial class StresstestingSolutionExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StresstestingSolutionExplorer));
            this.tvw = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // tvw
            // 
            this.tvw.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvw.HideSelection = false;
            this.tvw.ImageIndex = 0;
            this.tvw.ImageList = this.imageList;
            this.tvw.Location = new System.Drawing.Point(0, 0);
            this.tvw.Name = "tvw";
            this.tvw.SelectedImageIndex = 0;
            this.tvw.Size = new System.Drawing.Size(234, 410);
            this.tvw.TabIndex = 0;
            this.tvw.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvw_BeforeLabelEdit);
            this.tvw.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvw_AfterLabelEdit);
            this.tvw.DoubleClick += new System.EventHandler(this.tvw_DoubleClick);
            this.tvw.Enter += new System.EventHandler(this.tvw_Enter);
            this.tvw.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tvw_KeyDown);
            this.tvw.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvw_KeyUp);
            this.tvw.Leave += new System.EventHandler(this.tvw_Leave);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "DefaultTreeNodeImage.PNG");
            // 
            // StresstestingSolutionExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 410);
            this.Controls.Add(this.tvw);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StresstestingSolutionExplorer";
            this.Text = "Stresstesting Solution Explorer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvw;
        private System.Windows.Forms.ImageList imageList;
    }
}

