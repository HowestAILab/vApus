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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.picParallel = new System.Windows.Forms.PictureBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.chkIndex = new System.Windows.Forms.CheckBox();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.picIgnoreDelay = new System.Windows.Forms.PictureBox();
            this.picValidation = new System.Windows.Forms.PictureBox();
            this.nudOccurance = new System.Windows.Forms.NumericUpDown();
            this.txtScrollingLogEntry = new System.Windows.Forms.TextBox();
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.txtLogEntry = new System.Windows.Forms.TextBox();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelRemove = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picParallel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIgnoreDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picValidation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOccurance)).BeginInit();
            this.SuspendLayout();
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
            this.splitContainer.Panel1.Controls.Add(this.btnEdit);
            this.splitContainer.Panel1.Controls.Add(this.chkIndex);
            this.splitContainer.Panel1.Controls.Add(this.picPin);
            this.splitContainer.Panel1.Controls.Add(this.picIgnoreDelay);
            this.splitContainer.Panel1.Controls.Add(this.picValidation);
            this.splitContainer.Panel1.Controls.Add(this.nudOccurance);
            this.splitContainer.Panel1.Controls.Add(this.txtScrollingLogEntry);
            this.splitContainer.Panel1.Controls.Add(this.btnCollapseExpand);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.statusStrip);
            this.splitContainer.Panel2.Controls.Add(this.txtLogEntry);
            this.splitContainer.Size = new System.Drawing.Size(600, 198);
            this.splitContainer.SplitterDistance = 25;
            this.splitContainer.TabIndex = 0;
            // 
            // picParallel
            // 
            this.picParallel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picParallel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picParallel.Image = ((System.Drawing.Image)(resources.GetObject("picParallel.Image")));
            this.picParallel.Location = new System.Drawing.Point(503, 4);
            this.picParallel.Name = "picParallel";
            this.picParallel.Size = new System.Drawing.Size(16, 16);
            this.picParallel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picParallel.TabIndex = 6;
            this.picParallel.TabStop = false;
            this.toolTip.SetToolTip(this.picParallel, resources.GetString("picParallel.ToolTip"));
            this.picParallel.Visible = false;
            this.picParallel.Click += new System.EventHandler(this.picParallel_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Location = new System.Drawing.Point(370, 2);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(48, 22);
            this.btnEdit.TabIndex = 5;
            this.btnEdit.TabStop = false;
            this.btnEdit.Text = "Edit...";
            this.btnEdit.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // chkIndex
            // 
            this.chkIndex.AutoSize = true;
            this.chkIndex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkIndex.Location = new System.Drawing.Point(7, 3);
            this.chkIndex.Name = "chkIndex";
            this.chkIndex.Size = new System.Drawing.Size(34, 19);
            this.chkIndex.TabIndex = 1;
            this.chkIndex.Text = "1";
            this.chkIndex.UseVisualStyleBackColor = true;
            this.chkIndex.CheckedChanged += new System.EventHandler(this.chkIndex_CheckedChanged);
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::vApus.Stresstest.Properties.Resources.PinGreyedOut;
            this.picPin.Location = new System.Drawing.Point(547, 4);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(16, 16);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picPin.TabIndex = 1;
            this.picPin.TabStop = false;
            this.toolTip.SetToolTip(this.picPin, "Pin this log entry in place.");
            this.picPin.Click += new System.EventHandler(this.picPin_Click);
            // 
            // picIgnoreDelay
            // 
            this.picIgnoreDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picIgnoreDelay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picIgnoreDelay.Image = global::vApus.Stresstest.Properties.Resources.Delay;
            this.picIgnoreDelay.Location = new System.Drawing.Point(525, 4);
            this.picIgnoreDelay.Name = "picIgnoreDelay";
            this.picIgnoreDelay.Size = new System.Drawing.Size(16, 16);
            this.picIgnoreDelay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picIgnoreDelay.TabIndex = 1;
            this.picIgnoreDelay.TabStop = false;
            this.toolTip.SetToolTip(this.picIgnoreDelay, "Use or ignore the set delays that occurs after the execution of this log entry (s" +
        "tresstest properties).");
            this.picIgnoreDelay.Click += new System.EventHandler(this.picIgnoreDelay_Click);
            // 
            // picValidation
            // 
            this.picValidation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picValidation.Image = global::vApus.Stresstest.Properties.Resources.LogEntryOK;
            this.picValidation.Location = new System.Drawing.Point(60, 4);
            this.picValidation.Name = "picValidation";
            this.picValidation.Size = new System.Drawing.Size(16, 16);
            this.picValidation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picValidation.TabIndex = 1;
            this.picValidation.TabStop = false;
            // 
            // nudOccurance
            // 
            this.nudOccurance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudOccurance.Location = new System.Drawing.Point(424, 3);
            this.nudOccurance.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.nudOccurance.Name = "nudOccurance";
            this.nudOccurance.Size = new System.Drawing.Size(75, 20);
            this.nudOccurance.TabIndex = 3;
            this.toolTip.SetToolTip(this.nudOccurance, "Set how many times this log entry occures in the user action or log.\r\nAction and " +
        "Log Entry Distribution in the stresstest determines how this value will be used." +
        "\r\n");
            this.nudOccurance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudOccurance.ValueChanged += new System.EventHandler(this.nudOccurance_ValueChanged);
            // 
            // txtScrollingLogEntry
            // 
            this.txtScrollingLogEntry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScrollingLogEntry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(250)))));
            this.txtScrollingLogEntry.Location = new System.Drawing.Point(79, 3);
            this.txtScrollingLogEntry.Name = "txtScrollingLogEntry";
            this.txtScrollingLogEntry.ReadOnly = true;
            this.txtScrollingLogEntry.Size = new System.Drawing.Size(285, 20);
            this.txtScrollingLogEntry.TabIndex = 2;
            // 
            // btnCollapseExpand
            // 
            this.btnCollapseExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpand.Location = new System.Drawing.Point(569, 2);
            this.btnCollapseExpand.Name = "btnCollapseExpand";
            this.btnCollapseExpand.Size = new System.Drawing.Size(28, 22);
            this.btnCollapseExpand.TabIndex = 4;
            this.btnCollapseExpand.TabStop = false;
            this.btnCollapseExpand.Text = "-";
            this.toolTip.SetToolTip(this.btnCollapseExpand, "Collapse or expand.");
            this.btnCollapseExpand.UseVisualStyleBackColor = false;
            this.btnCollapseExpand.Click += new System.EventHandler(this.btnCollapseExpand_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 147);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(600, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // txtLogEntry
            // 
            this.txtLogEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogEntry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(250)))));
            this.txtLogEntry.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogEntry.Location = new System.Drawing.Point(7, 3);
            this.txtLogEntry.Multiline = true;
            this.txtLogEntry.Name = "txtLogEntry";
            this.txtLogEntry.ReadOnly = true;
            this.txtLogEntry.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogEntry.Size = new System.Drawing.Size(590, 142);
            this.txtLogEntry.TabIndex = 0;
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
            this.toolTip.IsBalloon = true;
            // 
            // LogEntryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitContainer);
            this.Name = "LogEntryControl";
            this.Size = new System.Drawing.Size(600, 198);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picParallel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIgnoreDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picValidation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOccurance)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox txtLogEntry;
        private System.Windows.Forms.StatusStrip statusStrip;
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

    }
}
