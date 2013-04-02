namespace vApus.Stresstest
{
    partial class UserActionTreeViewItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserActionTreeViewItem));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picValid = new System.Windows.Forms.PictureBox();
            this.picDuplicate = new System.Windows.Forms.PictureBox();
            this.picDelete = new System.Windows.Forms.PictureBox();
            this.picActionize = new System.Windows.Forms.PictureBox();
            this.lblUserAction = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picValid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picActionize)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // picValid
            // 
            this.picValid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picValid.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picValid.Image = ((System.Drawing.Image)(resources.GetObject("picValid.Image")));
            this.picValid.Location = new System.Drawing.Point(579, 6);
            this.picValid.Name = "picValid";
            this.picValid.Size = new System.Drawing.Size(16, 16);
            this.picValid.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picValid.TabIndex = 20;
            this.picValid.TabStop = false;
            this.picValid.Visible = false;
            this.picValid.Click += new System.EventHandler(this.picStatus_Click);
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
            this.toolTip.SetToolTip(this.picDuplicate, "Duplicate <ctrl+d>");
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
            this.toolTip.SetToolTip(this.picDelete, "Remove <ctrl+r>");
            this.picDelete.Visible = false;
            this.picDelete.Click += new System.EventHandler(this.picDelete_Click);
            // 
            // picActionize
            // 
            this.picActionize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picActionize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picActionize.Image = global::vApus.Stresstest.Properties.Resources.Actionize;
            this.picActionize.Location = new System.Drawing.Point(513, 6);
            this.picActionize.Name = "picActionize";
            this.picActionize.Size = new System.Drawing.Size(16, 16);
            this.picActionize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picActionize.TabIndex = 21;
            this.picActionize.TabStop = false;
            this.toolTip.SetToolTip(this.picActionize, "Actionize...");
            this.picActionize.Visible = false;
            this.picActionize.Click += new System.EventHandler(this.picRemoteDesktop_Click);
            // 
            // lblUserAction
            // 
            this.lblUserAction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserAction.AutoEllipsis = true;
            this.lblUserAction.Location = new System.Drawing.Point(3, 6);
            this.lblUserAction.MinimumSize = new System.Drawing.Size(0, 13);
            this.lblUserAction.Name = "lblUserAction";
            this.lblUserAction.Size = new System.Drawing.Size(504, 13);
            this.lblUserAction.TabIndex = 19;
            this.lblUserAction.Text = "User Action";
            // 
            // UserActionTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picActionize);
            this.Controls.Add(this.picValid);
            this.Controls.Add(this.picDuplicate);
            this.Controls.Add(this.picDelete);
            this.Controls.Add(this.lblUserAction);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserActionTreeViewItem";
            this.Size = new System.Drawing.Size(598, 32);
            ((System.ComponentModel.ISupportInitialize)(this.picValid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picActionize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picDelete;
        private System.Windows.Forms.PictureBox picDuplicate;
        private System.Windows.Forms.Label lblUserAction;
        private System.Windows.Forms.PictureBox picValid;
        private System.Windows.Forms.PictureBox picActionize;
    }
}
