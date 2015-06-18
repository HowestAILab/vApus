namespace vApus.DetailedResultsViewer {
    partial class FilterResultsControl {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterResultsControl));
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.flpTags = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
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
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "Filter results";
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
            this.txtFilter.TabIndex = 0;
            this.toolTip.SetToolTip(this.txtFilter, "Filter on tags and description. Wild cards *(auto-complete) +: must have word -: " +
        "cannot have word \"\": ommit spaces of a double-quoted piece of text. The filter i" +
        "s not case sensitive.");
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            this.txtFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFilter_KeyDown);
            this.txtFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFilter_KeyPress);
            // 
            // flpTags
            // 
            this.flpTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpTags.AutoScroll = true;
            this.flpTags.Location = new System.Drawing.Point(23, 82);
            this.flpTags.Margin = new System.Windows.Forms.Padding(0);
            this.flpTags.Name = "flpTags";
            this.flpTags.Size = new System.Drawing.Size(288, 65);
            this.flpTags.TabIndex = 1;
            this.flpTags.Scroll += new System.Windows.Forms.ScrollEventHandler(this.flpTags_Scroll);
            // 
            // FilterResultsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.flpTags);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label2);
            this.Name = "FilterResultsControl";
            this.Size = new System.Drawing.Size(314, 150);
            this.Resize += new System.EventHandler(this.FilterResults_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.FlowLayoutPanel flpTags;
        private System.Windows.Forms.ToolTip toolTip;


    }
}
