namespace vApus.StressTest
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
            this.nudOccurance = new System.Windows.Forms.NumericUpDown();
            this.picLinkColor = new System.Windows.Forms.PictureBox();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.picDuplicate = new System.Windows.Forms.PictureBox();
            this.picDelete = new System.Windows.Forms.PictureBox();
            this.lblUserAction = new System.Windows.Forms.Label();
            this.picValid = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudOccurance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLinkColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picValid)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // nudOccurance
            // 
            this.nudOccurance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudOccurance.Location = new System.Drawing.Point(501, 4);
            this.nudOccurance.Margin = new System.Windows.Forms.Padding(9, 3, 3, 3);
            this.nudOccurance.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.nudOccurance.Name = "nudOccurance";
            this.nudOccurance.Size = new System.Drawing.Size(50, 20);
            this.nudOccurance.TabIndex = 23;
            this.nudOccurance.TabStop = false;
            this.toolTip.SetToolTip(this.nudOccurance, "Set how many times this user action occures in the scenario.\r\nAction Distribution in t" +
        "he stress test determines how this value will be used.");
            this.nudOccurance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudOccurance.Visible = false;
            this.nudOccurance.ValueChanged += new System.EventHandler(this.nudOccurance_ValueChanged);
            // 
            // picLinkColor
            // 
            this.picLinkColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picLinkColor.Location = new System.Drawing.Point(441, 6);
            this.picLinkColor.Name = "picLinkColor";
            this.picLinkColor.Size = new System.Drawing.Size(4, 16);
            this.picLinkColor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLinkColor.TabIndex = 79;
            this.picLinkColor.TabStop = false;
            this.toolTip.SetToolTip(this.picLinkColor, "This color is shared between linked user actions.");
            this.picLinkColor.Visible = false;
            this.picLinkColor.Click += new System.EventHandler(this._Enter);
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::vApus.StressTest.Properties.Resources.PinGreyedOut;
            this.picPin.Location = new System.Drawing.Point(557, 6);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(16, 16);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picPin.TabIndex = 22;
            this.picPin.TabStop = false;
            this.toolTip.SetToolTip(this.picPin, "Pin this user action in place. This does nothing if the occurance equals zero!");
            this.picPin.Visible = false;
            this.picPin.Click += new System.EventHandler(this.picPin_Click);
            // 
            // picDuplicate
            // 
            this.picDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picDuplicate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picDuplicate.Image = ((System.Drawing.Image)(resources.GetObject("picDuplicate.Image")));
            this.picDuplicate.Location = new System.Drawing.Point(451, 6);
            this.picDuplicate.Name = "picDuplicate";
            this.picDuplicate.Size = new System.Drawing.Size(16, 16);
            this.picDuplicate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDuplicate.TabIndex = 17;
            this.picDuplicate.TabStop = false;
            this.toolTip.SetToolTip(this.picDuplicate, "Duplicate <ctrl+d>.");
            this.picDuplicate.Visible = false;
            this.picDuplicate.Click += new System.EventHandler(this.picDuplicate_Click);
            // 
            // picDelete
            // 
            this.picDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picDelete.Image = ((System.Drawing.Image)(resources.GetObject("picDelete.Image")));
            this.picDelete.Location = new System.Drawing.Point(473, 6);
            this.picDelete.Name = "picDelete";
            this.picDelete.Size = new System.Drawing.Size(16, 16);
            this.picDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picDelete.TabIndex = 15;
            this.picDelete.TabStop = false;
            this.toolTip.SetToolTip(this.picDelete, "Remove <ctrl+r>.");
            this.picDelete.Visible = false;
            this.picDelete.Click += new System.EventHandler(this.picDelete_Click);
            // 
            // lblUserAction
            // 
            this.lblUserAction.AutoEllipsis = true;
            this.lblUserAction.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserAction.Location = new System.Drawing.Point(3, 6);
            this.lblUserAction.MinimumSize = new System.Drawing.Size(0, 13);
            this.lblUserAction.Name = "lblUserAction";
            this.lblUserAction.Size = new System.Drawing.Size(486, 16);
            this.lblUserAction.TabIndex = 19;
            this.lblUserAction.Click += new System.EventHandler(this._Enter);
            // 
            // picValid
            // 
            this.picValid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picValid.Location = new System.Drawing.Point(579, 6);
            this.picValid.Name = "picValid";
            this.picValid.Size = new System.Drawing.Size(16, 16);
            this.picValid.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picValid.TabIndex = 20;
            this.picValid.TabStop = false;
            this.picValid.Click += new System.EventHandler(this._Enter);
            // 
            // UserActionTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picLinkColor);
            this.Controls.Add(this.nudOccurance);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.picValid);
            this.Controls.Add(this.picDuplicate);
            this.Controls.Add(this.picDelete);
            this.Controls.Add(this.lblUserAction);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserActionTreeViewItem";
            this.Size = new System.Drawing.Size(598, 32);
            this.Click += new System.EventHandler(this._Enter);
            ((System.ComponentModel.ISupportInitialize)(this.nudOccurance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLinkColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDuplicate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picValid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picDelete;
        private System.Windows.Forms.PictureBox picDuplicate;
        private System.Windows.Forms.Label lblUserAction;
        private System.Windows.Forms.PictureBox picValid;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.NumericUpDown nudOccurance;
        private System.Windows.Forms.PictureBox picLinkColor;
    }
}
