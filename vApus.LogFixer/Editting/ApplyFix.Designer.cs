namespace vApus.LogFixer
{
    partial class ApplyFix
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.rtxtScenario = new System.Windows.Forms.RichTextBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.btnHelp = new System.Windows.Forms.Button();
            this.largeList = new vApus.Util.LargeList();
            this.btnApply = new System.Windows.Forms.Button();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.flp = new System.Windows.Forms.FlowLayoutPanel();
            this.tbtnPreviewError = new vApus.Util.ToggleButton();
            this.tbtnPreviewOK = new vApus.Util.ToggleButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ckmIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmLine = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.flp.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Scenario";
            // 
            // rtxtScenario
            // 
            this.rtxtScenario.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtScenario.BackColor = System.Drawing.Color.White;
            this.rtxtScenario.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtScenario.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtScenario.Location = new System.Drawing.Point(0, 29);
            this.rtxtScenario.Name = "rtxtScenario";
            this.rtxtScenario.Size = new System.Drawing.Size(250, 480);
            this.rtxtScenario.TabIndex = 0;
            this.rtxtScenario.Text = "";
            this.rtxtScenario.TextChanged += new System.EventHandler(this.rtxtScenario_TextChanged);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(12, 12);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.btnHelp);
            this.splitContainer.Panel1.Controls.Add(this.rtxtScenario);
            this.splitContainer.Panel1.Controls.Add(this.label1);
            this.splitContainer.Panel1MinSize = 250;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.largeList);
            this.splitContainer.Panel2.Controls.Add(this.btnApply);
            this.splitContainer.Panel2.Controls.Add(this.chkAll);
            this.splitContainer.Panel2.Controls.Add(this.flp);
            this.splitContainer.Panel2MinSize = 250;
            this.splitContainer.Size = new System.Drawing.Size(760, 547);
            this.splitContainer.SplitterDistance = 250;
            this.splitContainer.TabIndex = 8;
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.AutoSize = true;
            this.btnHelp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHelp.Image = global::vApus.LogFixer.Properties.Resources.AllAnnotations_Help_16x16_72;
            this.btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHelp.Location = new System.Drawing.Point(191, 0);
            this.btnHelp.MinimumSize = new System.Drawing.Size(59, 24);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(59, 24);
            this.btnHelp.TabIndex = 3;
            this.btnHelp.TabStop = false;
            this.btnHelp.Text = "    Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // largeList
            // 
            this.largeList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.largeList.BackColor = System.Drawing.SystemColors.Control;
            this.largeList.Location = new System.Drawing.Point(3, 29);
            this.largeList.Name = "largeList";
            this.largeList.Size = new System.Drawing.Size(503, 480);
            this.largeList.SizeMode = vApus.Util.SizeMode.StretchHorizontal;
            this.largeList.TabIndex = 2;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApply.AutoSize = true;
            this.btnApply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnApply.Location = new System.Drawing.Point(3, 515);
            this.btnApply.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(142, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply to chosen and close";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAll.Location = new System.Drawing.Point(13, 4);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(33, 17);
            this.chkAll.TabIndex = 0;
            this.chkAll.Text = "0";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // flp
            // 
            this.flp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flp.Controls.Add(this.tbtnPreviewError);
            this.flp.Controls.Add(this.tbtnPreviewOK);
            this.flp.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flp.Location = new System.Drawing.Point(276, 0);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(230, 23);
            this.flp.TabIndex = 1;
            // 
            // tbtnPreviewError
            // 
            this.tbtnPreviewError.Appearance = System.Windows.Forms.Appearance.Button;
            this.tbtnPreviewError.AutoSize = true;
            this.tbtnPreviewError.Checked = true;
            this.tbtnPreviewError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tbtnPreviewError.Image = global::vApus.LogFixer.Properties.Resources.Error;
            this.tbtnPreviewError.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tbtnPreviewError.Location = new System.Drawing.Point(189, 0);
            this.tbtnPreviewError.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tbtnPreviewError.Name = "tbtnPreviewError";
            this.tbtnPreviewError.Size = new System.Drawing.Size(38, 23);
            this.tbtnPreviewError.TabIndex = 1;
            this.tbtnPreviewError.Text = "     0";
            this.tbtnPreviewError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tbtnPreviewError.UseVisualStyleBackColor = false;
            this.tbtnPreviewError.CheckedChanged += new System.EventHandler(this.tbtnPreviewError_CheckedChanged);
            // 
            // tbtnPreviewOK
            // 
            this.tbtnPreviewOK.Appearance = System.Windows.Forms.Appearance.Button;
            this.tbtnPreviewOK.AutoSize = true;
            this.tbtnPreviewOK.Image = global::vApus.LogFixer.Properties.Resources.OK;
            this.tbtnPreviewOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tbtnPreviewOK.Location = new System.Drawing.Point(145, 0);
            this.tbtnPreviewOK.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tbtnPreviewOK.Name = "tbtnPreviewOK";
            this.tbtnPreviewOK.Size = new System.Drawing.Size(38, 23);
            this.tbtnPreviewOK.TabIndex = 0;
            this.tbtnPreviewOK.Text = "     0";
            this.tbtnPreviewOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tbtnPreviewOK.UseVisualStyleBackColor = false;
            this.tbtnPreviewOK.CheckedChanged += new System.EventHandler(this.tbtnPreviewOK_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(722, 527);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(50, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ckmIndex
            // 
            this.ckmIndex.Text = "";
            // 
            // clmLine
            // 
            this.clmLine.Text = "";
            this.clmLine.Width = 439;
            // 
            // ApplyFix
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.splitContainer);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "ApplyFix";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Apply Fix";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.flp.ResumeLayout(false);
            this.flp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtxtScenario;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.FlowLayoutPanel flp;
        private vApus.Util.ToggleButton tbtnPreviewError;
        private vApus.Util.ToggleButton tbtnPreviewOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ColumnHeader ckmIndex;
        private System.Windows.Forms.ColumnHeader clmLine;
        private System.Windows.Forms.CheckBox chkAll;
        private vApus.Util.LargeList largeList;
        private System.Windows.Forms.Button btnHelp;
    }
}