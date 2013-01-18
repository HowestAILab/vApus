namespace vApus.DetailedResultsViewer {
    partial class Filter {
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
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Filter));
            this.lbtnTags = new vApus.Util.LinkButton();
            this.lbtnDescription = new vApus.Util.LinkButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.flpTags = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // lbtnTags
            // 
            this.lbtnTags.Active = false;
            this.lbtnTags.ActiveLinkColor = System.Drawing.Color.Blue;
            this.lbtnTags.AutoSize = true;
            this.lbtnTags.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lbtnTags.ForeColor = System.Drawing.Color.Blue;
            this.lbtnTags.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnTags.LinkColor = System.Drawing.Color.Blue;
            this.lbtnTags.Location = new System.Drawing.Point(194, 0);
            this.lbtnTags.Name = "lbtnTags";
            this.lbtnTags.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnTags.RadioButtonBehavior = false;
            this.lbtnTags.Size = new System.Drawing.Size(37, 20);
            this.lbtnTags.TabIndex = 9;
            this.lbtnTags.TabStop = true;
            this.lbtnTags.Text = "Tags";
            this.lbtnTags.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnTags.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // lbtnDescription
            // 
            this.lbtnDescription.Active = false;
            this.lbtnDescription.ActiveLinkColor = System.Drawing.Color.Blue;
            this.lbtnDescription.AutoSize = true;
            this.lbtnDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lbtnDescription.ForeColor = System.Drawing.Color.Blue;
            this.lbtnDescription.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnDescription.LinkColor = System.Drawing.Color.Blue;
            this.lbtnDescription.Location = new System.Drawing.Point(122, 0);
            this.lbtnDescription.Name = "lbtnDescription";
            this.lbtnDescription.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnDescription.RadioButtonBehavior = false;
            this.lbtnDescription.Size = new System.Drawing.Size(66, 20);
            this.lbtnDescription.TabIndex = 8;
            this.lbtnDescription.TabStop = true;
            this.lbtnDescription.Text = "Description";
            this.lbtnDescription.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnDescription.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Filter databases on";
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFilter.Location = new System.Drawing.Point(23, 26);
            this.txtFilter.Multiline = true;
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(288, 50);
            this.txtFilter.TabIndex = 10;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.Location = new System.Drawing.Point(177, 141);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(134, 13);
            this.linkLabel1.TabIndex = 11;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Show/Hide Available Tags";
            // 
            // flpTags
            // 
            this.flpTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpTags.Location = new System.Drawing.Point(23, 82);
            this.flpTags.Name = "flpTags";
            this.flpTags.Size = new System.Drawing.Size(288, 56);
            this.flpTags.TabIndex = 12;
            // 
            // Filter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.flpTags);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.lbtnTags);
            this.Controls.Add(this.lbtnDescription);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label2);
            this.Name = "Filter";
            this.Size = new System.Drawing.Size(314, 154);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Util.LinkButton lbtnTags;
        private Util.LinkButton lbtnDescription;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.FlowLayoutPanel flpTags;


    }
}
