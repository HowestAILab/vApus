namespace vApus.Stresstest
{
    partial class SyntaxItemControl
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
            this.lblSyntaxItemLabel = new System.Windows.Forms.Label();
            this.split = new System.Windows.Forms.SplitContainer();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.tglCollapse = new vApus.Gui.ToggleButton();
            this.toolTip = new System.Windows.Forms.ToolTip();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSyntaxItemLabel
            // 
            this.lblSyntaxItemLabel.AutoSize = true;
            this.lblSyntaxItemLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSyntaxItemLabel.ForeColor = System.Drawing.Color.Blue;
            this.lblSyntaxItemLabel.Location = new System.Drawing.Point(0, 1);
            this.lblSyntaxItemLabel.Name = "lblSyntaxItemLabel";
            this.lblSyntaxItemLabel.Size = new System.Drawing.Size(117, 13);
            this.lblSyntaxItemLabel.TabIndex = 0;
            this.lblSyntaxItemLabel.Text = "lblSyntaxItemLabel:";
            // 
            // split
            // 
            this.split.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.split.BackColor = System.Drawing.Color.WhiteSmoke;
            this.split.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.split.Location = new System.Drawing.Point(-1, 18);
            this.split.Name = "split";
            this.split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split.Panel1
            // 
            this.split.Panel1.Padding = new System.Windows.Forms.Padding(3);
            this.split.Panel1MinSize = 0;
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.txtDescription);
            this.split.Panel2MinSize = 0;
            this.split.Size = new System.Drawing.Size(280, 281);
            this.split.SplitterDistance = 222;
            this.split.TabIndex = 2;
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(0, 0);
            this.txtDescription.MinimumSize = new System.Drawing.Size(278, 50);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.Size = new System.Drawing.Size(278, 50);
            this.txtDescription.TabIndex = 0;
            // 
            // tglCollapse
            // 
            this.tglCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tglCollapse.Appearance = System.Windows.Forms.Appearance.Button;
            this.tglCollapse.Checked = true;
            this.tglCollapse.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.tglCollapse.Location = new System.Drawing.Point(259, 1);
            this.tglCollapse.Name = "tglCollapse";
            this.tglCollapse.Size = new System.Drawing.Size(16, 16);
            this.tglCollapse.TabIndex = 1;
            this.tglCollapse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.tglCollapse, "Click to collapse or expand.");
            this.tglCollapse.UseVisualStyleBackColor = false;
            this.tglCollapse.CheckedChanged += new System.EventHandler(this.tglCollapse_CheckedChanged);
            // 
            // SyntaxItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tglCollapse);
            this.Controls.Add(this.split);
            this.Controls.Add(this.lblSyntaxItemLabel);
            this.MaximumSize = new System.Drawing.Size(280, 300);
            this.MinimumSize = new System.Drawing.Size(280, 2);
            this.Name = "SyntaxItemControl";
            this.Size = new System.Drawing.Size(278, 298);
            this.split.Panel2.ResumeLayout(false);
            this.split.Panel2.PerformLayout();
            this.split.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSyntaxItemLabel;
        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.TextBox txtDescription;
        private vApus.Gui.ToggleButton tglCollapse;
        private System.Windows.Forms.ToolTip toolTip;

    }
}
