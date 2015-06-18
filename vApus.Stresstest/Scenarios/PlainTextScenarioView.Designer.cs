namespace vApus.StressTest {
    partial class PlaintTextScenarioView {
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.txtFind = new System.Windows.Forms.TextBox();
            this.llblFindAndReplace = new System.Windows.Forms.LinkLabel();
            this.picFind = new System.Windows.Forms.PictureBox();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.fctxt = new FastColoredTextBoxNS.FastColoredTextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.lbl = new System.Windows.Forms.Label();
            this.pnlRuleSet = new System.Windows.Forms.Panel();
            this.cboRuleSet = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picFind)).BeginInit();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fctxt)).BeginInit();
            this.pnlRuleSet.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 20;
            // 
            // txtFind
            // 
            this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFind.HideSelection = false;
            this.txtFind.Location = new System.Drawing.Point(3, 1);
            this.txtFind.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.txtFind.MinimumSize = new System.Drawing.Size(100, 4);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(275, 20);
            this.txtFind.TabIndex = 0;
            this.txtFind.TabStop = false;
            this.toolTip.SetToolTip(this.txtFind, "Wild cards * + - \"\" can be used. No whole words; Not case sensitive.");
            this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFind_KeyDown);
            // 
            // llblFindAndReplace
            // 
            this.llblFindAndReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llblFindAndReplace.BackColor = System.Drawing.Color.White;
            this.llblFindAndReplace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.llblFindAndReplace.DisabledLinkColor = System.Drawing.Color.Black;
            this.llblFindAndReplace.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.llblFindAndReplace.LinkColor = System.Drawing.Color.Black;
            this.llblFindAndReplace.Location = new System.Drawing.Point(299, 1);
            this.llblFindAndReplace.Name = "llblFindAndReplace";
            this.llblFindAndReplace.Size = new System.Drawing.Size(20, 20);
            this.llblFindAndReplace.TabIndex = 9;
            this.llblFindAndReplace.TabStop = true;
            this.llblFindAndReplace.Text = "...";
            this.llblFindAndReplace.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip.SetToolTip(this.llblFindAndReplace, "Find and Replace...");
            this.llblFindAndReplace.VisitedLinkColor = System.Drawing.Color.Black;
            this.llblFindAndReplace.Click += new System.EventHandler(this.llblFindAndReplace_Click);
            // 
            // picFind
            // 
            this.picFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picFind.BackColor = System.Drawing.Color.White;
            this.picFind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picFind.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picFind.Enabled = false;
            this.picFind.Image = global::vApus.StressTest.Properties.Resources.find;
            this.picFind.Location = new System.Drawing.Point(278, 1);
            this.picFind.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.picFind.Name = "picFind";
            this.picFind.Size = new System.Drawing.Size(20, 20);
            this.picFind.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFind.TabIndex = 8;
            this.picFind.TabStop = false;
            this.toolTip.SetToolTip(this.picFind, "Find next.");
            this.picFind.Click += new System.EventHandler(this.picFind_Click);
            // 
            // pnlFilter
            // 
            this.pnlFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFilter.Controls.Add(this.picFind);
            this.pnlFilter.Controls.Add(this.llblFindAndReplace);
            this.pnlFilter.Controls.Add(this.txtFind);
            this.pnlFilter.Location = new System.Drawing.Point(319, 562);
            this.pnlFilter.MinimumSize = new System.Drawing.Size(227, 21);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(319, 21);
            this.pnlFilter.TabIndex = 2;
            // 
            // fctxt
            // 
            this.fctxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fctxt.AutoScrollMinSize = new System.Drawing.Size(0, 14);
            this.fctxt.BackBrush = null;
            this.fctxt.CharHeight = 14;
            this.fctxt.CharWidth = 8;
            this.fctxt.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctxt.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctxt.IsReplaceMode = false;
            this.fctxt.Location = new System.Drawing.Point(0, 64);
            this.fctxt.Margin = new System.Windows.Forms.Padding(0);
            this.fctxt.Name = "fctxt";
            this.fctxt.Paddings = new System.Windows.Forms.Padding(0);
            this.fctxt.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctxt.Size = new System.Drawing.Size(760, 491);
            this.fctxt.TabIndex = 0;
            this.fctxt.ToolTip = null;
            this.fctxt.WordWrap = true;
            this.fctxt.Zoom = 100;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.AutoSize = true;
            this.btnApply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnApply.BackColor = System.Drawing.Color.White;
            this.btnApply.Enabled = false;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.Location = new System.Drawing.Point(700, 560);
            this.btnApply.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnApply.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(50, 24);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUndo.AutoSize = true;
            this.btnUndo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUndo.BackColor = System.Drawing.Color.White;
            this.btnUndo.Enabled = false;
            this.btnUndo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUndo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUndo.Location = new System.Drawing.Point(644, 560);
            this.btnUndo.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnUndo.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(49, 24);
            this.btnUndo.TabIndex = 3;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = false;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // lbl
            // 
            this.lbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl.Location = new System.Drawing.Point(12, 9);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(735, 51);
            this.lbl.TabIndex = 4;
            this.lbl.Text = "Put each item on a new line:\r\n*A user action like so <!--Foobar-->\r\n*A request " +
    "like it is defined in the used rule set";
            // 
            // pnlRuleSet
            // 
            this.pnlRuleSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlRuleSet.BackColor = System.Drawing.Color.Silver;
            this.pnlRuleSet.Controls.Add(this.cboRuleSet);
            this.pnlRuleSet.Location = new System.Drawing.Point(63, 561);
            this.pnlRuleSet.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.pnlRuleSet.Name = "pnlRuleSet";
            this.pnlRuleSet.Size = new System.Drawing.Size(250, 23);
            this.pnlRuleSet.TabIndex = 1;
            // 
            // cboRuleSet
            // 
            this.cboRuleSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRuleSet.BackColor = System.Drawing.Color.White;
            this.cboRuleSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRuleSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboRuleSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboRuleSet.FormattingEnabled = true;
            this.cboRuleSet.Location = new System.Drawing.Point(1, 1);
            this.cboRuleSet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboRuleSet.Name = "cboRuleSet";
            this.cboRuleSet.Size = new System.Drawing.Size(248, 21);
            this.cboRuleSet.TabIndex = 0;
            this.cboRuleSet.TabStop = false;
            this.cboRuleSet.SelectedIndexChanged += new System.EventHandler(this.cboRuleSet_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 566);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Rule set:";
            // 
            // PlaintTextScenarioView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(762, 595);
            this.Controls.Add(this.pnlRuleSet);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbl);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.fctxt);
            this.Controls.Add(this.pnlFilter);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PlaintTextScenarioView";
            this.Text = "PlainTextScenarioView";
            ((System.ComponentModel.ISupportInitialize)(this.picFind)).EndInit();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fctxt)).EndInit();
            this.pnlRuleSet.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.PictureBox picFind;
        private System.Windows.Forms.LinkLabel llblFindAndReplace;
        private System.Windows.Forms.TextBox txtFind;
        private FastColoredTextBoxNS.FastColoredTextBox fctxt;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.Panel pnlRuleSet;
        private System.Windows.Forms.ComboBox cboRuleSet;
        private System.Windows.Forms.Label label2;
    }
}