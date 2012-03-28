namespace vApus.DistributedTesting
{
    partial class LinkageControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinkageControl));
            this.rtxt = new System.Windows.Forms.RichTextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.split = new System.Windows.Forms.SplitContainer();
            this.lblProgress = new System.Windows.Forms.Label();
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.lbl = new System.Windows.Forms.Label();
            this.pnl = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnJumpStart = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.pnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtxt
            // 
            this.rtxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.rtxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxt.Location = new System.Drawing.Point(6, 0);
            this.rtxt.Name = "rtxt";
            this.rtxt.ReadOnly = true;
            this.rtxt.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxt.Size = new System.Drawing.Size(394, 137);
            this.rtxt.TabIndex = 0;
            this.rtxt.Text = "";
            // 
            // split
            // 
            this.split.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.split.IsSplitterFixed = true;
            this.split.Location = new System.Drawing.Point(0, 0);
            this.split.Name = "split";
            this.split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.lblProgress);
            this.split.Panel1.Controls.Add(this.btnCollapseExpand);
            this.split.Panel1.Controls.Add(this.lbl);
            // 
            // split.Panel2
            // 
            this.split.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.split.Panel2.Controls.Add(this.rtxt);
            this.split.Size = new System.Drawing.Size(400, 171);
            this.split.SplitterDistance = 30;
            this.split.TabIndex = 3;
            // 
            // lblProgress
            // 
            this.lblProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.Location = new System.Drawing.Point(218, 3);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(144, 20);
            this.lblProgress.TabIndex = 6;
            this.lblProgress.Text = "Progress";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblProgress.Visible = false;
            // 
            // btnCollapseExpand
            // 
            this.btnCollapseExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseExpand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollapseExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpand.Location = new System.Drawing.Point(368, 2);
            this.btnCollapseExpand.Name = "btnCollapseExpand";
            this.btnCollapseExpand.Size = new System.Drawing.Size(28, 20);
            this.btnCollapseExpand.TabIndex = 5;
            this.btnCollapseExpand.TabStop = false;
            this.btnCollapseExpand.Text = "-";
            this.btnCollapseExpand.UseVisualStyleBackColor = true;
            this.btnCollapseExpand.Click += new System.EventHandler(this.btnCollapseExpand_Click);
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl.Location = new System.Drawing.Point(2, 3);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(53, 20);
            this.lbl.TabIndex = 0;
            this.lbl.Text = "Slave";
            // 
            // pnl
            // 
            this.pnl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnl.Controls.Add(this.lblMessage);
            this.pnl.Controls.Add(this.btnJumpStart);
            this.pnl.Location = new System.Drawing.Point(0, 172);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(401, 29);
            this.pnl.TabIndex = 4;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(3, 7);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 13);
            this.lblMessage.TabIndex = 3;
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lblMessage.Visible = false;
            // 
            // btnJumpStart
            // 
            this.btnJumpStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnJumpStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnJumpStart.Enabled = false;
            this.btnJumpStart.FlatAppearance.BorderSize = 0;
            this.btnJumpStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJumpStart.Image = ((System.Drawing.Image)(resources.GetObject("btnJumpStart.Image")));
            this.btnJumpStart.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnJumpStart.Location = new System.Drawing.Point(318, 2);
            this.btnJumpStart.Name = "btnJumpStart";
            this.btnJumpStart.Size = new System.Drawing.Size(79, 23);
            this.btnJumpStart.TabIndex = 2;
            this.btnJumpStart.Text = "Jump Start";
            this.btnJumpStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip.SetToolTip(this.btnJumpStart, "You can remotely launch a slave from here.");
            this.btnJumpStart.UseVisualStyleBackColor = true;
            this.btnJumpStart.Click += new System.EventHandler(this.btnJumpStartKill_Click);
            // 
            // LinkageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.pnl);
            this.Controls.Add(this.split);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "LinkageControl";
            this.Size = new System.Drawing.Size(400, 200);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel1.PerformLayout();
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.pnl.ResumeLayout(false);
            this.pnl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxt;
        private System.Windows.Forms.Button btnJumpStart;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.Button btnCollapseExpand;
        private System.Windows.Forms.Panel pnl;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblMessage;
    }
}
