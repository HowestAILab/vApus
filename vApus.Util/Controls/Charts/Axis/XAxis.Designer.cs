namespace vApus.Util
{
    partial class XAxis
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
            this.llblScrollToBeginning = new System.Windows.Forms.LinkLabel();
            this.llblScrollBack = new System.Windows.Forms.LinkLabel();
            this.llblScrollForth = new System.Windows.Forms.LinkLabel();
            this.llblScrollToEnd = new System.Windows.Forms.LinkLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.xAxisLabel = new vApus.Util.XAxisLabel();
            ((System.ComponentModel.ISupportInitialize)(this.xAxisLabel)).BeginInit();
            this.SuspendLayout();
            // 
            // llblScrollToBeginning
            // 
            this.llblScrollToBeginning.ActiveLinkColor = System.Drawing.Color.Blue;
            this.llblScrollToBeginning.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.llblScrollToBeginning.AutoSize = true;
            this.llblScrollToBeginning.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.llblScrollToBeginning.Enabled = false;
            this.llblScrollToBeginning.LinkColor = System.Drawing.Color.Blue;
            this.llblScrollToBeginning.Location = new System.Drawing.Point(3, 5);
            this.llblScrollToBeginning.Name = "llblScrollToBeginning";
            this.llblScrollToBeginning.Size = new System.Drawing.Size(21, 13);
            this.llblScrollToBeginning.TabIndex = 0;
            this.llblScrollToBeginning.TabStop = true;
            this.llblScrollToBeginning.Text = "|<<";
            this.toolTip.SetToolTip(this.llblScrollToBeginning, "Scroll to Beginning");
            this.llblScrollToBeginning.Click += new System.EventHandler(this.llblScrollToBeginning_Click);
            // 
            // llblScrollBack
            // 
            this.llblScrollBack.ActiveLinkColor = System.Drawing.Color.Blue;
            this.llblScrollBack.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.llblScrollBack.AutoSize = true;
            this.llblScrollBack.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.llblScrollBack.Enabled = false;
            this.llblScrollBack.LinkColor = System.Drawing.Color.Blue;
            this.llblScrollBack.Location = new System.Drawing.Point(30, 5);
            this.llblScrollBack.Name = "llblScrollBack";
            this.llblScrollBack.Size = new System.Drawing.Size(15, 13);
            this.llblScrollBack.TabIndex = 0;
            this.llblScrollBack.TabStop = true;
            this.llblScrollBack.Text = "|<";
            this.toolTip.SetToolTip(this.llblScrollBack, "Scroll Back");
            this.llblScrollBack.MouseDown += new System.Windows.Forms.MouseEventHandler(this.llblScrollBack_MouseDown);
            // 
            // llblScrollForth
            // 
            this.llblScrollForth.ActiveLinkColor = System.Drawing.Color.Blue;
            this.llblScrollForth.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.llblScrollForth.AutoSize = true;
            this.llblScrollForth.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.llblScrollForth.Enabled = false;
            this.llblScrollForth.LinkColor = System.Drawing.Color.Blue;
            this.llblScrollForth.Location = new System.Drawing.Point(257, 5);
            this.llblScrollForth.Name = "llblScrollForth";
            this.llblScrollForth.Size = new System.Drawing.Size(15, 13);
            this.llblScrollForth.TabIndex = 0;
            this.llblScrollForth.TabStop = true;
            this.llblScrollForth.Text = ">|";
            this.toolTip.SetToolTip(this.llblScrollForth, "Scroll Forth");
            this.llblScrollForth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.llblScrollForth_MouseDown);
            // 
            // llblScrollToEnd
            // 
            this.llblScrollToEnd.ActiveLinkColor = System.Drawing.Color.Blue;
            this.llblScrollToEnd.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.llblScrollToEnd.AutoSize = true;
            this.llblScrollToEnd.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.llblScrollToEnd.Enabled = false;
            this.llblScrollToEnd.LinkColor = System.Drawing.Color.Blue;
            this.llblScrollToEnd.Location = new System.Drawing.Point(278, 5);
            this.llblScrollToEnd.Name = "llblScrollToEnd";
            this.llblScrollToEnd.Size = new System.Drawing.Size(21, 13);
            this.llblScrollToEnd.TabIndex = 0;
            this.llblScrollToEnd.TabStop = true;
            this.llblScrollToEnd.Text = ">>|";
            this.toolTip.SetToolTip(this.llblScrollToEnd, "Scroll to End");
            this.llblScrollToEnd.Click += new System.EventHandler(this.llblScrollToEnd_Click);
            // 
            // xAxisLabel
            // 
            this.xAxisLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.xAxisLabel.BackColor = System.Drawing.Color.White;
            this.xAxisLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.xAxisLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xAxisLabel.Location = new System.Drawing.Point(-1, -1);
            this.xAxisLabel.Name = "xAxisLabel";
            this.xAxisLabel.Size = new System.Drawing.Size(304, 25);
            this.xAxisLabel.TabIndex = 1;
            this.xAxisLabel.TabStop = false;
            // 
            // XAxis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.xAxisLabel);
            this.Controls.Add(this.llblScrollToEnd);
            this.Controls.Add(this.llblScrollForth);
            this.Controls.Add(this.llblScrollBack);
            this.Controls.Add(this.llblScrollToBeginning);
            this.Name = "XAxis";
            this.Size = new System.Drawing.Size(302, 23);
            ((System.ComponentModel.ISupportInitialize)(this.xAxisLabel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel llblScrollToBeginning;
        private System.Windows.Forms.LinkLabel llblScrollBack;
        private System.Windows.Forms.LinkLabel llblScrollForth;
        private System.Windows.Forms.LinkLabel llblScrollToEnd;
        private XAxisLabel xAxisLabel;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
