namespace vApus.LogFixer
{
    partial class EditLineControl
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.txtScrollingLogEntry = new System.Windows.Forms.TextBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.lblIndex = new System.Windows.Forms.Label();
            this.picValidation = new System.Windows.Forms.PictureBox();
            this.btnRestore = new System.Windows.Forms.Button();
            this.rtxtLogEntry = new System.Windows.Forms.RichTextBox();
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelRemove = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picValidation)).BeginInit();
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
            this.splitContainer.Panel1.Controls.Add(this.txtScrollingLogEntry);
            this.splitContainer.Panel1.Controls.Add(this.btnEdit);
            this.splitContainer.Panel1.Controls.Add(this.lblIndex);
            this.splitContainer.Panel1.Controls.Add(this.picValidation);
            this.splitContainer.Panel1.Controls.Add(this.btnRestore);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.rtxtLogEntry);
            this.splitContainer.Size = new System.Drawing.Size(600, 198);
            this.splitContainer.SplitterDistance = 25;
            this.splitContainer.TabIndex = 0;
            // 
            // txtScrollingLogEntry
            // 
            this.txtScrollingLogEntry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScrollingLogEntry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(250)))));
            this.txtScrollingLogEntry.Location = new System.Drawing.Point(79, 3);
            this.txtScrollingLogEntry.Name = "txtScrollingLogEntry";
            this.txtScrollingLogEntry.ReadOnly = true;
            this.txtScrollingLogEntry.Size = new System.Drawing.Size(448, 20);
            this.txtScrollingLogEntry.TabIndex = 0;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.BackColor = System.Drawing.SystemColors.Control;
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Location = new System.Drawing.Point(530, 2);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(48, 22);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "Edit...";
            this.btnEdit.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // lblIndex
            // 
            this.lblIndex.AutoSize = true;
            this.lblIndex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIndex.Location = new System.Drawing.Point(7, 3);
            this.lblIndex.Name = "lblIndex";
            this.lblIndex.Size = new System.Drawing.Size(15, 15);
            this.lblIndex.TabIndex = 1;
            this.lblIndex.Text = "1";
            // 
            // picValidation
            // 
            this.picValidation.Location = new System.Drawing.Point(60, 4);
            this.picValidation.Name = "picValidation";
            this.picValidation.Size = new System.Drawing.Size(16, 16);
            this.picValidation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picValidation.TabIndex = 1;
            this.picValidation.TabStop = false;
            // 
            // btnRestore
            // 
            this.btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestore.BackColor = System.Drawing.SystemColors.Control;
            this.btnRestore.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestore.Location = new System.Drawing.Point(518, 2);
            this.btnRestore.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(59, 22);
            this.btnRestore.TabIndex = 2;
            this.btnRestore.Text = "Restore";
            this.btnRestore.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRestore.UseVisualStyleBackColor = false;
            this.btnRestore.Visible = false;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
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
            // btnCollapseExpand
            // 
            this.btnCollapseExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseExpand.BackColor = System.Drawing.Color.White;
            this.btnCollapseExpand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollapseExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpand.Location = new System.Drawing.Point(581, -2);
            this.btnCollapseExpand.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnCollapseExpand.Name = "btnCollapseExpand";
            this.btnCollapseExpand.Size = new System.Drawing.Size(20, 32);
            this.btnCollapseExpand.TabIndex = 1;
            this.btnCollapseExpand.Text = "-";
            this.btnCollapseExpand.UseVisualStyleBackColor = false;
            this.btnCollapseExpand.Click += new System.EventHandler(this.btnCollapseExpand_Click);
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
            // 
            // EditLineControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnCollapseExpand);
            this.Controls.Add(this.splitContainer);
            this.Name = "EditLineControl";
            this.Size = new System.Drawing.Size(600, 198);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picValidation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.RichTextBox rtxtLogEntry;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelRemove;
        private System.Windows.Forms.PictureBox picValidation;
        private System.Windows.Forms.TextBox txtScrollingLogEntry;
        private System.Windows.Forms.Button btnCollapseExpand;
        private System.Windows.Forms.Label lblIndex;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnRestore;

    }
}
