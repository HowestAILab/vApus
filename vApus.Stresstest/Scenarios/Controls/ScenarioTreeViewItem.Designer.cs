namespace vApus.StressTest {
    partial class ScenarioTreeViewItem {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScenarioTreeViewItem));
            this.pnlRuleSet = new System.Windows.Forms.Panel();
            this.cboRuleSet = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picAddUserAction = new System.Windows.Forms.PictureBox();
            this.picClearUserActions = new System.Windows.Forms.PictureBox();
            this.picPasteUserAction = new System.Windows.Forms.PictureBox();
            this.picValid = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlRuleSet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAddUserAction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picClearUserActions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPasteUserAction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picValid)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlRuleSet
            // 
            this.pnlRuleSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRuleSet.BackColor = System.Drawing.Color.Silver;
            this.pnlRuleSet.Controls.Add(this.cboRuleSet);
            this.pnlRuleSet.Location = new System.Drawing.Point(88, 6);
            this.pnlRuleSet.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.pnlRuleSet.Name = "pnlRuleSet";
            this.pnlRuleSet.Size = new System.Drawing.Size(358, 23);
            this.pnlRuleSet.TabIndex = 0;
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
            this.cboRuleSet.Size = new System.Drawing.Size(356, 21);
            this.cboRuleSet.TabIndex = 0;
            this.cboRuleSet.TabStop = false;
            this.cboRuleSet.SelectedIndexChanged += new System.EventHandler(this.cboRuleSet_SelectedIndexChanged);
            this.cboRuleSet.Enter += new System.EventHandler(this._Enter);
            this.cboRuleSet.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.cboRuleSet.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoEllipsis = true;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.MinimumSize = new System.Drawing.Size(0, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Scenario";
            this.label1.Click += new System.EventHandler(this._Enter);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // picAddUserAction
            // 
            this.picAddUserAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picAddUserAction.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAddUserAction.Image = ((System.Drawing.Image)(resources.GetObject("picAddUserAction.Image")));
            this.picAddUserAction.Location = new System.Drawing.Point(452, 10);
            this.picAddUserAction.Name = "picAddUserAction";
            this.picAddUserAction.Size = new System.Drawing.Size(16, 16);
            this.picAddUserAction.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picAddUserAction.TabIndex = 20;
            this.picAddUserAction.TabStop = false;
            this.toolTip.SetToolTip(this.picAddUserAction, "Add user action <ctrl+i>.");
            this.picAddUserAction.Click += new System.EventHandler(this.picAddUserAction_Click);
            // 
            // picClearUserActions
            // 
            this.picClearUserActions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picClearUserActions.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picClearUserActions.Image = ((System.Drawing.Image)(resources.GetObject("picClearUserActions.Image")));
            this.picClearUserActions.Location = new System.Drawing.Point(493, 10);
            this.picClearUserActions.Margin = new System.Windows.Forms.Padding(0);
            this.picClearUserActions.Name = "picClearUserActions";
            this.picClearUserActions.Size = new System.Drawing.Size(16, 16);
            this.picClearUserActions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picClearUserActions.TabIndex = 35;
            this.picClearUserActions.TabStop = false;
            this.toolTip.SetToolTip(this.picClearUserActions, "Clear user actions.");
            this.picClearUserActions.Click += new System.EventHandler(this.picClearUserActions_Click);
            // 
            // picPasteUserAction
            // 
            this.picPasteUserAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPasteUserAction.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPasteUserAction.Image = ((System.Drawing.Image)(resources.GetObject("picPasteUserAction.Image")));
            this.picPasteUserAction.Location = new System.Drawing.Point(474, 10);
            this.picPasteUserAction.Name = "picPasteUserAction";
            this.picPasteUserAction.Size = new System.Drawing.Size(16, 16);
            this.picPasteUserAction.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPasteUserAction.TabIndex = 37;
            this.picPasteUserAction.TabStop = false;
            this.toolTip.SetToolTip(this.picPasteUserAction, "Paste user action <ctrl+p>.");
            this.picPasteUserAction.Click += new System.EventHandler(this.picPasteUserAction_Click);
            // 
            // picValid
            // 
            this.picValid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picValid.Location = new System.Drawing.Point(514, 10);
            this.picValid.Margin = new System.Windows.Forms.Padding(0);
            this.picValid.Name = "picValid";
            this.picValid.Size = new System.Drawing.Size(16, 16);
            this.picValid.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picValid.TabIndex = 36;
            this.picValid.TabStop = false;
            this.picValid.Click += new System.EventHandler(this._Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Rule Set:";
            this.label2.Click += new System.EventHandler(this._Enter);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(532, 2);
            this.panel1.TabIndex = 23;
            // 
            // ScenarioTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picPasteUserAction);
            this.Controls.Add(this.picValid);
            this.Controls.Add(this.picClearUserActions);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlRuleSet);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.picAddUserAction);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ScenarioTreeViewItem";
            this.Size = new System.Drawing.Size(532, 35);
            this.Click += new System.EventHandler(this._Enter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this._KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            this.pnlRuleSet.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picAddUserAction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picClearUserActions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPasteUserAction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picValid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlRuleSet;
        private System.Windows.Forms.ComboBox cboRuleSet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picAddUserAction;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picClearUserActions;
        private System.Windows.Forms.PictureBox picValid;
        private System.Windows.Forms.PictureBox picPasteUserAction;

    }
}
