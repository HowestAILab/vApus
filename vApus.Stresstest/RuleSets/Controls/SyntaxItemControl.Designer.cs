namespace vApus.Stresstest
{
    partial class OldSyntaxItemControl
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
            this.lblSyntaxItemLabel = new System.Windows.Forms.Label();
            this.split = new System.Windows.Forms.SplitContainer();
            this.rtxtDescription = new System.Windows.Forms.RichTextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSyntaxItemLabel
            // 
            this.lblSyntaxItemLabel.AutoSize = true;
            this.lblSyntaxItemLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSyntaxItemLabel.ForeColor = System.Drawing.Color.Blue;
            this.lblSyntaxItemLabel.Location = new System.Drawing.Point(0, 1);
            this.lblSyntaxItemLabel.Name = "lblSyntaxItemLabel";
            this.lblSyntaxItemLabel.Size = new System.Drawing.Size(124, 16);
            this.lblSyntaxItemLabel.TabIndex = 0;
            this.lblSyntaxItemLabel.Text = "lblSyntaxItemLabel:";
            // 
            // split
            // 
            this.split.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split.Location = new System.Drawing.Point(-1, 18);
            this.split.Name = "split";
            this.split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split.Panel1
            // 
            this.split.Panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.split.Panel1.Padding = new System.Windows.Forms.Padding(5, 1, 3, 1);
            this.split.Panel1MinSize = 0;
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.rtxtDescription);
            this.split.Panel2.Padding = new System.Windows.Forms.Padding(5, 0, 5, 5);
            this.split.Panel2MinSize = 0;
            this.split.Size = new System.Drawing.Size(352, 281);
            this.split.SplitterDistance = 222;
            this.split.SplitterWidth = 2;
            this.split.TabIndex = 2;
            this.split.TabStop = false;
            // 
            // rtxtDescription
            // 
            this.rtxtDescription.BackColor = System.Drawing.Color.White;
            this.rtxtDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtDescription.Location = new System.Drawing.Point(5, 0);
            this.rtxtDescription.MinimumSize = new System.Drawing.Size(342, 52);
            this.rtxtDescription.Name = "rtxtDescription";
            this.rtxtDescription.ReadOnly = true;
            this.rtxtDescription.Size = new System.Drawing.Size(342, 52);
            this.rtxtDescription.TabIndex = 0;
            this.rtxtDescription.TabStop = false;
            this.rtxtDescription.Text = "";
            // 
            // SyntaxItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.Controls.Add(this.split);
            this.Controls.Add(this.lblSyntaxItemLabel);
            this.MaximumSize = new System.Drawing.Size(350, 300);
            this.MinimumSize = new System.Drawing.Size(350, 2);
            this.Name = "SyntaxItemControl";
            this.Size = new System.Drawing.Size(350, 300);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSyntaxItemLabel;
        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.RichTextBox rtxtDescription;
        private System.Windows.Forms.ToolTip toolTip;

    }
}
