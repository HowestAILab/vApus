namespace vApus.StressTest {
    partial class FindAndReplaceDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.chkReplaceAll = new System.Windows.Forms.CheckBox();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnReplaceWith = new System.Windows.Forms.Button();
            this.btnFind = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.chkWholeWords = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkReplaceAll
            // 
            this.chkReplaceAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkReplaceAll.AutoSize = true;
            this.chkReplaceAll.Location = new System.Drawing.Point(489, 47);
            this.chkReplaceAll.Name = "chkReplaceAll";
            this.chkReplaceAll.Size = new System.Drawing.Size(37, 17);
            this.chkReplaceAll.TabIndex = 6;
            this.chkReplaceAll.Text = "All";
            this.chkReplaceAll.UseVisualStyleBackColor = true;
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMatchCase.AutoSize = true;
            this.chkMatchCase.Location = new System.Drawing.Point(489, 17);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(83, 17);
            this.chkMatchCase.TabIndex = 3;
            this.chkMatchCase.Text = "Match case";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // txtReplace
            // 
            this.txtReplace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReplace.HideSelection = false;
            this.txtReplace.Location = new System.Drawing.Point(153, 44);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(330, 20);
            this.txtReplace.TabIndex = 5;
            this.txtReplace.TextChanged += new System.EventHandler(this.txtReplace_TextChanged);
            this.txtReplace.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtReplace_KeyDown);
            // 
            // txtFind
            // 
            this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFind.HideSelection = false;
            this.txtFind.Location = new System.Drawing.Point(61, 15);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(325, 20);
            this.txtFind.TabIndex = 1;
            this.toolTip.SetToolTip(this.txtFind, "Wild cards * + - \"\" can be used.");
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFind_KeyDown);
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCount.Location = new System.Drawing.Point(12, -214);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(0, 13);
            this.lblCount.TabIndex = 13;
            // 
            // btnReplaceWith
            // 
            this.btnReplaceWith.AutoSize = true;
            this.btnReplaceWith.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnReplaceWith.BackColor = System.Drawing.Color.White;
            this.btnReplaceWith.Enabled = false;
            this.btnReplaceWith.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReplaceWith.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReplaceWith.Location = new System.Drawing.Point(12, 42);
            this.btnReplaceWith.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnReplaceWith.Name = "btnReplaceWith";
            this.btnReplaceWith.Size = new System.Drawing.Size(135, 24);
            this.btnReplaceWith.TabIndex = 4;
            this.btnReplaceWith.Text = "Replace found with";
            this.btnReplaceWith.UseVisualStyleBackColor = false;
            this.btnReplaceWith.Click += new System.EventHandler(this.btnReplaceWith_Click);
            // 
            // btnFind
            // 
            this.btnFind.AutoSize = true;
            this.btnFind.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFind.BackColor = System.Drawing.Color.White;
            this.btnFind.Enabled = false;
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFind.Location = new System.Drawing.Point(12, 12);
            this.btnFind.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(43, 24);
            this.btnFind.TabIndex = 0;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = false;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 20;
            // 
            // chkWholeWords
            // 
            this.chkWholeWords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkWholeWords.AutoSize = true;
            this.chkWholeWords.Location = new System.Drawing.Point(392, 17);
            this.chkWholeWords.Name = "chkWholeWords";
            this.chkWholeWords.Size = new System.Drawing.Size(91, 17);
            this.chkWholeWords.TabIndex = 2;
            this.chkWholeWords.Text = "Whole words";
            this.chkWholeWords.UseVisualStyleBackColor = true;
            // 
            // FindAndReplaceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 78);
            this.Controls.Add(this.chkWholeWords);
            this.Controls.Add(this.chkReplaceAll);
            this.Controls.Add(this.chkMatchCase);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.txtFind);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.btnReplaceWith);
            this.Controls.Add(this.btnFind);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(9999, 116);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 116);
            this.Name = "FindAndReplaceDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Find and replace";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkReplaceAll;
        private System.Windows.Forms.CheckBox chkMatchCase;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnReplaceWith;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox chkWholeWords;
    }
}