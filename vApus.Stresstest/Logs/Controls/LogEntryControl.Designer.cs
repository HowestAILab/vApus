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
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.picParallel = new System.Windows.Forms.PictureBox();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.picIgnoreDelay = new System.Windows.Forms.PictureBox();
            this.nudOccurance = new System.Windows.Forms.NumericUpDown();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.chkIndex = new System.Windows.Forms.CheckBox();
            this.picValidation = new System.Windows.Forms.PictureBox();
            this.txtScrollingLogEntry = new System.Windows.Forms.TextBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.rtxtLogEntry = new System.Windows.Forms.RichTextBox();
            this.nudParallelOffsetInMs = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.picParallel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIgnoreDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOccurance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picValidation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudParallelOffsetInMs)).BeginInit();
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
            // btnCollapseExpand
            // 
            this.btnCollapseExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseExpand.BackColor = System.Drawing.Color.White;
            this.btnCollapseExpand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollapseExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpand.Location = new System.Drawing.Point(581, -2);
            this.btnCollapseExpand.Name = "btnCollapseExpand";
            this.btnCollapseExpand.Size = new System.Drawing.Size(20, 32);
            this.btnCollapseExpand.TabIndex = 1;
            this.btnCollapseExpand.TabStop = false;
            this.btnCollapseExpand.Text = "-";
            this.toolTip.SetToolTip(this.btnCollapseExpand, "Collapse or expand.");
            this.btnCollapseExpand.UseVisualStyleBackColor = false;
            this.btnCollapseExpand.Click += new System.EventHandler(this.btnCollapseExpand_Click);
            // 
            // picParallel
            // 
            this.picParallel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picParallel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picParallel.Image = ((System.Drawing.Image)(resources.GetObject("picParallel.Image")));
            this.picParallel.Location = new System.Drawing.Point(515, 4);
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
            this.picPin.Location = new System.Drawing.Point(559, 4);
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
            this.picIgnoreDelay.Location = new System.Drawing.Point(537, 4);
            this.picIgnoreDelay.Name = "picIgnoreDelay";
            this.picIgnoreDelay.Size = new System.Drawing.Size(16, 16);
            this.picIgnoreDelay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picIgnoreDelay.TabIndex = 1;
            this.picIgnoreDelay.TabStop = false;
            this.toolTip.SetToolTip(this.picIgnoreDelay, resources.GetString("picIgnoreDelay.ToolTip"));
            this.picIgnoreDelay.Click += new System.EventHandler(this.picIgnoreDelay_Click);
            // 
            // nudOccurance
            // 
            this.nudOccurance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudOccurance.Location = new System.Drawing.Point(458, 3);
            this.nudOccurance.Margin = new System.Windows.Forms.Padding(9, 3, 3, 3);
            this.nudOccurance.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.nudOccurance.Name = "nudOccurance";
            this.nudOccurance.Size = new System.Drawing.Size(50, 20);
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
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.picParallel);
            this.splitContainer.Panel1.Controls.Add(this.chkIndex);
            this.splitContainer.Panel1.Controls.Add(this.picPin);
            this.splitContainer.Panel1.Controls.Add(this.picIgnoreDelay);
            this.splitContainer.Panel1.Controls.Add(this.picValidation);
            this.splitContainer.Panel1.Controls.Add(this.nudOccurance);
            this.splitContainer.Panel1.Controls.Add(this.txtScrollingLogEntry);
            this.splitContainer.Panel1.Controls.Add(this.btnEdit);
            this.splitContainer.Panel1.Controls.Add(this.nudParallelOffsetInMs);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.rtxtLogEntry);
            this.splitContainer.Size = new System.Drawing.Size(600, 198);
            this.splitContainer.SplitterDistance = 25;
            this.splitContainer.TabIndex = 0;
            // 
            // chkIndex
            // 
            this.chkIndex.AutoSize = true;
            this.chkIndex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkIndex.Location = new System.Drawing.Point(7, 3);
            this.chkIndex.Name = "chkIndex";
            this.chkIndex.Size = new System.Drawing.Size(34, 19);
            this.chkIndex.TabIndex = 0;
            this.chkIndex.Text = "1";
            this.chkIndex.UseVisualStyleBackColor = true;
            this.chkIndex.CheckedChanged += new System.EventHandler(this.chkIndex_CheckedChanged);
            // 
            // picValidation
            // 
            this.picValidation.Image = global::vApus.Stresstest.Properties.Resources.LogEntryOK;
            this.picValidation.Location = new System.Drawing.Point(60, 4);
            this.picValidation.Name = "picValidation";
            this.picValidation.Size = new System.Drawing.Size(16, 16);
            this.picValidation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picValidation.TabIndex = 1;
            this.picValidation.TabStop = false;
            // 
            // txtScrollingLogEntry
            // 
            this.txtScrollingLogEntry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScrollingLogEntry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(250)))));
            this.txtScrollingLogEntry.Location = new System.Drawing.Point(79, 3);
            this.txtScrollingLogEntry.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.txtScrollingLogEntry.Name = "txtScrollingLogEntry";
            this.txtScrollingLogEntry.ReadOnly = true;
            this.txtScrollingLogEntry.Size = new System.Drawing.Size(323, 20);
            this.txtScrollingLogEntry.TabIndex = 1;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.BackColor = System.Drawing.SystemColors.Control;
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Location = new System.Drawing.Point(402, 2);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(45, 22);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.TabStop = false;
            this.btnEdit.Text = "Edit...";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // rtxtLogEntry
            // 
            this.rtxtLogEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtLogEntry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(250)))));
            this.rtxtLogEntry.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtLogEntry.DetectUrls = false;
            this.rtxtLogEntry.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtLogEntry.Location = new System.Drawing.Point(7, 3);
            this.rtxtLogEntry.Name = "rtxtLogEntry";
            this.rtxtLogEntry.ReadOnly = true;
            this.rtxtLogEntry.Size = new System.Drawing.Size(590, 163);
            this.rtxtLogEntry.TabIndex = 0;
            this.rtxtLogEntry.Text = "";
            // 
            // nudParallelOffsetInMs
            // 
            this.nudParallelOffsetInMs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudParallelOffsetInMs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.nudParallelOffsetInMs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.nudParallelOffsetInMs.Location = new System.Drawing.Point(534, 3);
            this.nudParallelOffsetInMs.Margin = new System.Windows.Forms.Padding(0);
            this.nudParallelOffsetInMs.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.nudParallelOffsetInMs.Name = "nudParallelOffsetInMs";
            this.nudParallelOffsetInMs.Size = new System.Drawing.Size(47, 20);
            this.nudParallelOffsetInMs.TabIndex = 1;
            this.toolTip.SetToolTip(this.nudParallelOffsetInMs, "The offset in ms before this \'parallel log entry\' is executed (this simulates wha" +
        "t browsers do).");
            this.nudParallelOffsetInMs.Visible = false;
            // 
            // LogEntryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnCollapseExpand);
            this.Controls.Add(this.splitContainer);
            this.Name = "LogEntryControl";
            this.Size = new System.Drawing.Size(600, 198);
            ((System.ComponentModel.ISupportInitialize)(this.picParallel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIgnoreDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOccurance)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picValidation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudParallelOffsetInMs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.RichTextBox rtxtLogEntry;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelRemove;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.PictureBox picIgnoreDelay;
        private System.Windows.Forms.PictureBox picValidation;
        private System.Windows.Forms.NumericUpDown nudOccurance;
        private System.Windows.Forms.TextBox txtScrollingLogEntry;
        private System.Windows.Forms.Button btnCollapseExpand;
        private System.Windows.Forms.CheckBox chkIndex;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.PictureBox picParallel;
        private System.Windows.Forms.NumericUpDown nudParallelOffsetInMs;

    }
}
