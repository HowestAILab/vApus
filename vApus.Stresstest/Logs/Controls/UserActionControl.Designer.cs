namespace vApus.Stresstest
{
    partial class UserActionControl
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picPin = new System.Windows.Forms.PictureBox();
            this.nudOccurance = new System.Windows.Forms.NumericUpDown();
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.chkIndex = new System.Windows.Forms.CheckBox();
            this.txtUserAction = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOccurance)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::vApus.Stresstest.Properties.Resources.PinGreyedOut;
            this.picPin.Location = new System.Drawing.Point(543, 7);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(16, 16);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picPin.TabIndex = 7;
            this.picPin.TabStop = false;
            this.toolTip.SetToolTip(this.picPin, "Pin this user action in place. This does nothing if the occurance equals zero! Fo" +
        "r fast distribution the pinned user action can occur only once.");
            this.picPin.Click += new System.EventHandler(this.picPin_Click);
            // 
            // nudOccurance
            // 
            this.nudOccurance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudOccurance.Location = new System.Drawing.Point(454, 6);
            this.nudOccurance.Margin = new System.Windows.Forms.Padding(9, 3, 3, 3);
            this.nudOccurance.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.nudOccurance.Name = "nudOccurance";
            this.nudOccurance.Size = new System.Drawing.Size(50, 20);
            this.nudOccurance.TabIndex = 3;
            this.toolTip.SetToolTip(this.nudOccurance, "Set how many times this user action occures in the log.\r\nAction and Log Entry Dis" +
        "tribution in the stresstest determines how this value will be used.");
            this.nudOccurance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudOccurance.ValueChanged += new System.EventHandler(this.nudOccurance_ValueChanged);
            // 
            // btnCollapseExpand
            // 
            this.btnCollapseExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpand.Location = new System.Drawing.Point(565, 5);
            this.btnCollapseExpand.Name = "btnCollapseExpand";
            this.btnCollapseExpand.Size = new System.Drawing.Size(28, 37);
            this.btnCollapseExpand.TabIndex = 4;
            this.btnCollapseExpand.TabStop = false;
            this.btnCollapseExpand.Text = "+";
            this.toolTip.SetToolTip(this.btnCollapseExpand, "Collapse or expand.");
            this.btnCollapseExpand.UseVisualStyleBackColor = false;
            this.btnCollapseExpand.Click += new System.EventHandler(this.btnCollapseExpand_Click);
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblCount.Location = new System.Drawing.Point(58, 29);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(125, 13);
            this.lblCount.TabIndex = 10;
            this.lblCount.Text = "Contains 1 Log Entry";
            // 
            // chkIndex
            // 
            this.chkIndex.AutoSize = true;
            this.chkIndex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkIndex.Location = new System.Drawing.Point(7, 6);
            this.chkIndex.Name = "chkIndex";
            this.chkIndex.Size = new System.Drawing.Size(34, 19);
            this.chkIndex.TabIndex = 1;
            this.chkIndex.Text = "1";
            this.chkIndex.UseVisualStyleBackColor = true;
            this.chkIndex.CheckedChanged += new System.EventHandler(this.chkIndex_CheckedChanged);
            // 
            // txtUserAction
            // 
            this.txtUserAction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserAction.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtUserAction.Location = new System.Drawing.Point(60, 6);
            this.txtUserAction.Name = "txtUserAction";
            this.txtUserAction.Size = new System.Drawing.Size(382, 20);
            this.txtUserAction.TabIndex = 2;
            this.txtUserAction.Text = "Give this user action a label.";
            this.txtUserAction.Enter += new System.EventHandler(this.txtUserAction_Enter);
            this.txtUserAction.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtUserAction_KeyUp);
            this.txtUserAction.Leave += new System.EventHandler(this.txtUserAction_Leave);
            // 
            // UserActionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.chkIndex);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.nudOccurance);
            this.Controls.Add(this.txtUserAction);
            this.Controls.Add(this.btnCollapseExpand);
            this.Name = "UserActionControl";
            this.Size = new System.Drawing.Size(598, 49);
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOccurance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkIndex;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.NumericUpDown nudOccurance;
        private System.Windows.Forms.TextBox txtUserAction;
        private System.Windows.Forms.Button btnCollapseExpand;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
