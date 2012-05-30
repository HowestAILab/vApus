namespace vApus.Stresstest
{
    partial class LogEntryControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogEntryControl));
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelRemove = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.llblEdit = new System.Windows.Forms.LinkLabel();
            this.nudOccurance = new System.Windows.Forms.NumericUpDown();
            this.nudParallelOffsetInMs = new System.Windows.Forms.NumericUpDown();
            this.chkIndex = new System.Windows.Forms.CheckBox();
            this.txtScrollingLogEntry = new System.Windows.Forms.TextBox();
            this.picParallel = new System.Windows.Forms.PictureBox();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.picIgnoreDelay = new System.Windows.Forms.PictureBox();
            this.picValidation = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudOccurance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudParallelOffsetInMs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picParallel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIgnoreDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picValidation)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(531, 19);
            this.toolStripStatusLabel4.Spring = true;
            // 
            // toolStripStatusLabelRemove
            // 
            this.toolStripStatusLabelRemove.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabelRemove.Name = "toolStripStatusLabelRemove";
            this.toolStripStatusLabelRemove.Size = new System.Drawing.Size(54, 19);
            this.toolStripStatusLabelRemove.Text = "Remove";
            this.toolStripStatusLabelRemove.Click += new System.EventHandler(this.toolStripStatusLabelRemove_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // llblEdit
            // 
            this.llblEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llblEdit.BackColor = System.Drawing.Color.White;
            this.llblEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.llblEdit.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.llblEdit.Location = new System.Drawing.Point(583, 4);
            this.llblEdit.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.llblEdit.Name = "llblEdit";
            this.llblEdit.Size = new System.Drawing.Size(16, 21);
            this.llblEdit.TabIndex = 2;
            this.llblEdit.TabStop = true;
            this.llblEdit.Text = "...";
            this.toolTip.SetToolTip(this.llblEdit, "Edit this log entry.");
            this.llblEdit.Click += new System.EventHandler(this.llblEdit_Click);
            // 
            // nudOccurance
            // 
            this.nudOccurance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudOccurance.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.nudOccurance.Location = new System.Drawing.Point(460, 6);
            this.nudOccurance.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.nudOccurance.Name = "nudOccurance";
            this.nudOccurance.Size = new System.Drawing.Size(50, 16);
            this.nudOccurance.TabIndex = 3;
            this.toolTip.SetToolTip(this.nudOccurance, "Set how many times this log entry occures in the user action or log.\r\nAction and " +
        "Log Entry Distribution in the stresstest determines how this value will be used." +
        "\r\n");
            this.nudOccurance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudParallelOffsetInMs
            // 
            this.nudParallelOffsetInMs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudParallelOffsetInMs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.nudParallelOffsetInMs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.nudParallelOffsetInMs.Location = new System.Drawing.Point(539, 6);
            this.nudParallelOffsetInMs.Margin = new System.Windows.Forms.Padding(0);
            this.nudParallelOffsetInMs.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.nudParallelOffsetInMs.Name = "nudParallelOffsetInMs";
            this.nudParallelOffsetInMs.Size = new System.Drawing.Size(38, 16);
            this.nudParallelOffsetInMs.TabIndex = 1;
            this.toolTip.SetToolTip(this.nudParallelOffsetInMs, "The offset in ms before this \'parallel log entry\' is executed (this simulates wha" +
        "t browsers do).");
            this.nudParallelOffsetInMs.Visible = false;
            // 
            // chkIndex
            // 
            this.chkIndex.AutoSize = true;
            this.chkIndex.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkIndex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkIndex.Location = new System.Drawing.Point(9, 3);
            this.chkIndex.Name = "chkIndex";
            this.chkIndex.Size = new System.Drawing.Size(31, 19);
            this.chkIndex.TabIndex = 0;
            this.chkIndex.Text = "1";
            this.chkIndex.UseVisualStyleBackColor = true;
            this.chkIndex.CheckedChanged += new System.EventHandler(this.chkIndex_CheckedChanged);
            // 
            // txtScrollingLogEntry
            // 
            this.txtScrollingLogEntry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScrollingLogEntry.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtScrollingLogEntry.Location = new System.Drawing.Point(79, 7);
            this.txtScrollingLogEntry.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.txtScrollingLogEntry.Name = "txtScrollingLogEntry";
            this.txtScrollingLogEntry.ReadOnly = true;
            this.txtScrollingLogEntry.Size = new System.Drawing.Size(378, 13);
            this.txtScrollingLogEntry.TabIndex = 1;
            // 
            // picParallel
            // 
            this.picParallel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picParallel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picParallel.Image = ((System.Drawing.Image)(resources.GetObject("picParallel.Image")));
            this.picParallel.Location = new System.Drawing.Point(517, 5);
            this.picParallel.Name = "picParallel";
            this.picParallel.Size = new System.Drawing.Size(16, 16);
            this.picParallel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picParallel.TabIndex = 6;
            this.picParallel.TabStop = false;
            this.toolTip.SetToolTip(this.picParallel, resources.GetString("picParallel.ToolTip"));
            this.picParallel.Visible = false;
            this.picParallel.Click += new System.EventHandler(this.picParallel_Click);
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::vApus.Stresstest.Properties.Resources.PinGreyedOut;
            this.picPin.Location = new System.Drawing.Point(561, 5);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(16, 16);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picPin.TabIndex = 1;
            this.picPin.TabStop = false;
            this.toolTip.SetToolTip(this.picPin, "Pin this log entry in place. This does nothing if the occurance equals zero! For " +
        "fast distribution the pinned log entry can occur only once.");
            this.picPin.Click += new System.EventHandler(this.picPin_Click);
            // 
            // picIgnoreDelay
            // 
            this.picIgnoreDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picIgnoreDelay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picIgnoreDelay.Image = global::vApus.Stresstest.Properties.Resources.Delay;
            this.picIgnoreDelay.Location = new System.Drawing.Point(539, 5);
            this.picIgnoreDelay.Name = "picIgnoreDelay";
            this.picIgnoreDelay.Size = new System.Drawing.Size(16, 16);
            this.picIgnoreDelay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picIgnoreDelay.TabIndex = 1;
            this.picIgnoreDelay.TabStop = false;
            this.toolTip.SetToolTip(this.picIgnoreDelay, resources.GetString("picIgnoreDelay.ToolTip"));
            this.picIgnoreDelay.Click += new System.EventHandler(this.picIgnoreDelay_Click);
            // 
            // picValidation
            // 
            this.picValidation.Image = global::vApus.Stresstest.Properties.Resources.LogEntryOK;
            this.picValidation.Location = new System.Drawing.Point(60, 5);
            this.picValidation.Name = "picValidation";
            this.picValidation.Size = new System.Drawing.Size(16, 16);
            this.picValidation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picValidation.TabIndex = 1;
            this.picValidation.TabStop = false;
            // 
            // LogEntryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.picParallel);
            this.Controls.Add(this.chkIndex);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.llblEdit);
            this.Controls.Add(this.picIgnoreDelay);
            this.Controls.Add(this.txtScrollingLogEntry);
            this.Controls.Add(this.picValidation);
            this.Controls.Add(this.nudOccurance);
            this.Controls.Add(this.nudParallelOffsetInMs);
            this.Name = "LogEntryControl";
            this.Size = new System.Drawing.Size(602, 30);
            ((System.ComponentModel.ISupportInitialize)(this.nudOccurance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudParallelOffsetInMs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picParallel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIgnoreDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picValidation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelRemove;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.PictureBox picIgnoreDelay;
        private System.Windows.Forms.PictureBox picValidation;
        private System.Windows.Forms.NumericUpDown nudOccurance;
        private System.Windows.Forms.TextBox txtScrollingLogEntry;
        private System.Windows.Forms.CheckBox chkIndex;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.LinkLabel llblEdit;
        private System.Windows.Forms.PictureBox picParallel;
        private System.Windows.Forms.NumericUpDown nudParallelOffsetInMs;

    }
}
