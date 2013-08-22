namespace vApus.Stresstest
{
    partial class FindAndReplace
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
            this.btnFind = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.flpFoundReplaced = new System.Windows.Forms.FlowLayoutPanel();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.chkWholeWords = new System.Windows.Forms.CheckBox();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.btnReplaceWith = new System.Windows.Forms.Button();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.chkReplaceAll = new System.Windows.Forms.CheckBox();
            this.btnSwitchValues = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnFind
            // 
            this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFind.AutoSize = true;
            this.btnFind.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFind.BackColor = System.Drawing.Color.White;
            this.btnFind.Enabled = false;
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFind.Location = new System.Drawing.Point(12, 108);
            this.btnFind.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(43, 24);
            this.btnFind.TabIndex = 1;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = false;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCount.Location = new System.Drawing.Point(12, 143);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(0, 13);
            this.lblCount.TabIndex = 3;
            // 
            // flpFoundReplaced
            // 
            this.flpFoundReplaced.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpFoundReplaced.AutoScroll = true;
            this.flpFoundReplaced.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpFoundReplaced.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flpFoundReplaced.Location = new System.Drawing.Point(12, 12);
            this.flpFoundReplaced.Name = "flpFoundReplaced";
            this.flpFoundReplaced.Size = new System.Drawing.Size(534, 90);
            this.flpFoundReplaced.TabIndex = 0;
            this.flpFoundReplaced.SizeChanged += new System.EventHandler(this.flpFoundReplaced_SizeChanged);
            // 
            // txtFind
            // 
            this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFind.HideSelection = false;
            this.txtFind.Location = new System.Drawing.Point(61, 111);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(291, 20);
            this.txtFind.TabIndex = 2;
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            this.txtFind.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtFind_KeyUp);
            // 
            // chkWholeWords
            // 
            this.chkWholeWords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkWholeWords.AutoSize = true;
            this.chkWholeWords.Location = new System.Drawing.Point(358, 113);
            this.chkWholeWords.Name = "chkWholeWords";
            this.chkWholeWords.Size = new System.Drawing.Size(91, 17);
            this.chkWholeWords.TabIndex = 3;
            this.chkWholeWords.Text = "Whole Words";
            this.chkWholeWords.UseVisualStyleBackColor = true;
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMatchCase.AutoSize = true;
            this.chkMatchCase.Location = new System.Drawing.Point(463, 113);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(83, 17);
            this.chkMatchCase.TabIndex = 4;
            this.chkMatchCase.Text = "Match Case";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // btnReplaceWith
            // 
            this.btnReplaceWith.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReplaceWith.AutoSize = true;
            this.btnReplaceWith.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnReplaceWith.BackColor = System.Drawing.Color.White;
            this.btnReplaceWith.Enabled = false;
            this.btnReplaceWith.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReplaceWith.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReplaceWith.Location = new System.Drawing.Point(12, 138);
            this.btnReplaceWith.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnReplaceWith.Name = "btnReplaceWith";
            this.btnReplaceWith.Size = new System.Drawing.Size(135, 24);
            this.btnReplaceWith.TabIndex = 5;
            this.btnReplaceWith.Text = "Replace Found With";
            this.btnReplaceWith.UseVisualStyleBackColor = false;
            this.btnReplaceWith.Click += new System.EventHandler(this.btnReplaceWith_Click);
            // 
            // txtReplace
            // 
            this.txtReplace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReplace.HideSelection = false;
            this.txtReplace.Location = new System.Drawing.Point(153, 140);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(199, 20);
            this.txtReplace.TabIndex = 6;
            this.txtReplace.TextChanged += new System.EventHandler(this.txtReplace_TextChanged);
            this.txtReplace.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtReplace_KeyUp);
            // 
            // chkReplaceAll
            // 
            this.chkReplaceAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkReplaceAll.AutoSize = true;
            this.chkReplaceAll.Location = new System.Drawing.Point(358, 143);
            this.chkReplaceAll.Name = "chkReplaceAll";
            this.chkReplaceAll.Size = new System.Drawing.Size(37, 17);
            this.chkReplaceAll.TabIndex = 8;
            this.chkReplaceAll.Text = "All";
            this.chkReplaceAll.UseVisualStyleBackColor = true;
            this.chkReplaceAll.CheckedChanged += new System.EventHandler(this.chkReplaceAll_CheckedChanged);
            // 
            // btnSwitchValues
            // 
            this.btnSwitchValues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSwitchValues.AutoSize = true;
            this.btnSwitchValues.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSwitchValues.BackColor = System.Drawing.Color.White;
            this.btnSwitchValues.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSwitchValues.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSwitchValues.Location = new System.Drawing.Point(447, 138);
            this.btnSwitchValues.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnSwitchValues.Name = "btnSwitchValues";
            this.btnSwitchValues.Size = new System.Drawing.Size(99, 24);
            this.btnSwitchValues.TabIndex = 7;
            this.btnSwitchValues.Text = "Switch Values";
            this.btnSwitchValues.UseVisualStyleBackColor = false;
            this.btnSwitchValues.Click += new System.EventHandler(this.btnSwitchValues_Click);
            // 
            // FindAndReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSwitchValues);
            this.Controls.Add(this.chkReplaceAll);
            this.Controls.Add(this.chkMatchCase);
            this.Controls.Add(this.chkWholeWords);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.txtFind);
            this.Controls.Add(this.flpFoundReplaced);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.btnReplaceWith);
            this.Controls.Add(this.btnFind);
            this.Name = "FindAndReplace";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.Size = new System.Drawing.Size(558, 173);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.FlowLayoutPanel flpFoundReplaced;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.CheckBox chkWholeWords;
        private System.Windows.Forms.CheckBox chkMatchCase;
        private System.Windows.Forms.Button btnReplaceWith;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.CheckBox chkReplaceAll;
        private System.Windows.Forms.Button btnSwitchValues;
    }
}
