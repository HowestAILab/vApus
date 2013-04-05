namespace vApus.Stresstest {
    partial class EditUserAction {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditUserAction));
            this.lblLabel = new System.Windows.Forms.Label();
            this.txtLabel = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.picMoveUp = new System.Windows.Forms.PictureBox();
            this.picMoveDown = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudMoveSteps = new System.Windows.Forms.NumericUpDown();
            this.dgvLogEntries = new System.Windows.Forms.DataGridView();
            this.tc = new vApus.Util.TabControlWithAdjustableBorders();
            this.tpStructured = new System.Windows.Forms.TabPage();
            this.tpPlainText = new System.Windows.Forms.TabPage();
            this.fctxtxPlainText = new FastColoredTextBoxNS.FastColoredTextBox();
            this.lbtnEditable = new vApus.Util.LinkButton();
            this.lbtnAsImported = new vApus.Util.LinkButton();
            this.flpConfiguration = new System.Windows.Forms.FlowLayoutPanel();
            this.lblConnection = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnRevertToImported = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picMoveUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMoveDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMoveSteps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogEntries)).BeginInit();
            this.tc.SuspendLayout();
            this.tpStructured.SuspendLayout();
            this.tpPlainText.SuspendLayout();
            this.flpConfiguration.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLabel
            // 
            this.lblLabel.AutoSize = true;
            this.lblLabel.Location = new System.Drawing.Point(21, 39);
            this.lblLabel.Name = "lblLabel";
            this.lblLabel.Size = new System.Drawing.Size(36, 13);
            this.lblLabel.TabIndex = 26;
            this.lblLabel.Text = "Label:";
            // 
            // txtLabel
            // 
            this.txtLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLabel.Location = new System.Drawing.Point(63, 36);
            this.txtLabel.Name = "txtLabel";
            this.txtLabel.Size = new System.Drawing.Size(444, 20);
            this.txtLabel.TabIndex = 25;
            this.txtLabel.TextChanged += new System.EventHandler(this.txtLabel_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(513, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Move:";
            // 
            // picMoveUp
            // 
            this.picMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picMoveUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("picMoveUp.Image")));
            this.picMoveUp.Location = new System.Drawing.Point(555, 36);
            this.picMoveUp.Name = "picMoveUp";
            this.picMoveUp.Size = new System.Drawing.Size(16, 16);
            this.picMoveUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picMoveUp.TabIndex = 28;
            this.picMoveUp.TabStop = false;
            this.picMoveUp.Click += new System.EventHandler(this.picMoveUp_Click);
            // 
            // picMoveDown
            // 
            this.picMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picMoveDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("picMoveDown.Image")));
            this.picMoveDown.Location = new System.Drawing.Point(577, 36);
            this.picMoveDown.Name = "picMoveDown";
            this.picMoveDown.Size = new System.Drawing.Size(16, 16);
            this.picMoveDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picMoveDown.TabIndex = 29;
            this.picMoveDown.TabStop = false;
            this.picMoveDown.Click += new System.EventHandler(this.picMoveDown_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(599, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Steps:";
            // 
            // nudMoveSteps
            // 
            this.nudMoveSteps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudMoveSteps.Location = new System.Drawing.Point(642, 36);
            this.nudMoveSteps.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nudMoveSteps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMoveSteps.Name = "nudMoveSteps";
            this.nudMoveSteps.Size = new System.Drawing.Size(42, 20);
            this.nudMoveSteps.TabIndex = 31;
            this.nudMoveSteps.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // dgvLogEntries
            // 
            this.dgvLogEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLogEntries.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvLogEntries.BackgroundColor = System.Drawing.Color.White;
            this.dgvLogEntries.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLogEntries.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvLogEntries.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvLogEntries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLogEntries.EnableHeadersVisualStyles = false;
            this.dgvLogEntries.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.dgvLogEntries.Location = new System.Drawing.Point(3, 3);
            this.dgvLogEntries.Name = "dgvLogEntries";
            this.dgvLogEntries.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvLogEntries.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvLogEntries.Size = new System.Drawing.Size(656, 285);
            this.dgvLogEntries.TabIndex = 32;
            this.dgvLogEntries.VirtualMode = true;
            // 
            // tc
            // 
            this.tc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tc.BottomVisible = false;
            this.tc.Controls.Add(this.tpStructured);
            this.tc.Controls.Add(this.tpPlainText);
            this.tc.LeftVisible = true;
            this.tc.Location = new System.Drawing.Point(24, 107);
            this.tc.Name = "tc";
            this.tc.RightVisible = false;
            this.tc.SelectedIndex = 0;
            this.tc.Size = new System.Drawing.Size(663, 351);
            this.tc.TabIndex = 33;
            this.tc.TopVisible = true;
            this.tc.SelectedIndexChanged += new System.EventHandler(this.tc_SelectedIndexChanged);
            // 
            // tpStructured
            // 
            this.tpStructured.BackColor = System.Drawing.Color.White;
            this.tpStructured.Controls.Add(this.dgvLogEntries);
            this.tpStructured.Location = new System.Drawing.Point(4, 22);
            this.tpStructured.Name = "tpStructured";
            this.tpStructured.Padding = new System.Windows.Forms.Padding(3);
            this.tpStructured.Size = new System.Drawing.Size(658, 328);
            this.tpStructured.TabIndex = 0;
            this.tpStructured.Text = "Structured";
            // 
            // tpPlainText
            // 
            this.tpPlainText.BackColor = System.Drawing.Color.White;
            this.tpPlainText.Controls.Add(this.fctxtxPlainText);
            this.tpPlainText.Location = new System.Drawing.Point(4, 22);
            this.tpPlainText.Name = "tpPlainText";
            this.tpPlainText.Padding = new System.Windows.Forms.Padding(3);
            this.tpPlainText.Size = new System.Drawing.Size(658, 328);
            this.tpPlainText.TabIndex = 1;
            this.tpPlainText.Text = "Plain Text";
            // 
            // fctxtxPlainText
            // 
            this.fctxtxPlainText.AllowDrop = true;
            this.fctxtxPlainText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fctxtxPlainText.AutoScrollMinSize = new System.Drawing.Size(0, 14);
            this.fctxtxPlainText.BackBrush = null;
            this.fctxtxPlainText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctxtxPlainText.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctxtxPlainText.IsReplaceMode = false;
            this.fctxtxPlainText.Location = new System.Drawing.Point(3, 3);
            this.fctxtxPlainText.Name = "fctxtxPlainText";
            this.fctxtxPlainText.Paddings = new System.Windows.Forms.Padding(0);
            this.fctxtxPlainText.PreferredLineWidth = 65536;
            this.fctxtxPlainText.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctxtxPlainText.Size = new System.Drawing.Size(656, 285);
            this.fctxtxPlainText.TabIndex = 1;
            this.fctxtxPlainText.WordWrap = true;
            // 
            // lbtnEditable
            // 
            this.lbtnEditable.Active = true;
            this.lbtnEditable.ActiveLinkColor = System.Drawing.Color.Black;
            this.lbtnEditable.AutoSize = true;
            this.lbtnEditable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbtnEditable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnEditable.ForeColor = System.Drawing.Color.Black;
            this.lbtnEditable.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbtnEditable.LinkColor = System.Drawing.Color.Black;
            this.lbtnEditable.Location = new System.Drawing.Point(17, 6);
            this.lbtnEditable.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnEditable.Name = "lbtnEditable";
            this.lbtnEditable.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnEditable.RadioButtonBehavior = true;
            this.lbtnEditable.Size = new System.Drawing.Size(61, 22);
            this.lbtnEditable.TabIndex = 34;
            this.lbtnEditable.TabStop = true;
            this.lbtnEditable.Text = "Editable";
            this.lbtnEditable.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnEditable.VisitedLinkColor = System.Drawing.Color.Black;
            this.lbtnEditable.ActiveChanged += new System.EventHandler(this.lbtn_ActiveChanged);
            // 
            // lbtnAsImported
            // 
            this.lbtnAsImported.Active = false;
            this.lbtnAsImported.ActiveLinkColor = System.Drawing.Color.Blue;
            this.lbtnAsImported.AutoSize = true;
            this.lbtnAsImported.BackColor = System.Drawing.Color.Transparent;
            this.lbtnAsImported.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lbtnAsImported.ForeColor = System.Drawing.Color.Blue;
            this.lbtnAsImported.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnAsImported.LinkColor = System.Drawing.Color.Blue;
            this.lbtnAsImported.Location = new System.Drawing.Point(81, 6);
            this.lbtnAsImported.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnAsImported.Name = "lbtnAsImported";
            this.lbtnAsImported.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnAsImported.RadioButtonBehavior = true;
            this.lbtnAsImported.Size = new System.Drawing.Size(69, 20);
            this.lbtnAsImported.TabIndex = 35;
            this.lbtnAsImported.TabStop = true;
            this.lbtnAsImported.Text = "As Imported";
            this.lbtnAsImported.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnAsImported.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lbtnAsImported.ActiveChanged += new System.EventHandler(this.lbtn_ActiveChanged);
            // 
            // flpConfiguration
            // 
            this.flpConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flpConfiguration.Controls.Add(this.lbtnAsImported);
            this.flpConfiguration.Controls.Add(this.lbtnEditable);
            this.flpConfiguration.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpConfiguration.Location = new System.Drawing.Point(534, 423);
            this.flpConfiguration.Name = "flpConfiguration";
            this.flpConfiguration.Size = new System.Drawing.Size(150, 31);
            this.flpConfiguration.TabIndex = 36;
            // 
            // lblConnection
            // 
            this.lblConnection.AutoSize = true;
            this.lblConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnection.Location = new System.Drawing.Point(6, 10);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(73, 13);
            this.lblConnection.TabIndex = 37;
            this.lblConnection.Text = "User Action";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "Log Entries";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnApply.AutoSize = true;
            this.btnApply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnApply.BackColor = System.Drawing.Color.White;
            this.btnApply.Enabled = false;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.Location = new System.Drawing.Point(225, 426);
            this.btnApply.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(50, 25);
            this.btnApply.TabIndex = 40;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Visible = false;
            // 
            // btnUndo
            // 
            this.btnUndo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnUndo.AutoSize = true;
            this.btnUndo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUndo.BackColor = System.Drawing.Color.White;
            this.btnUndo.Enabled = false;
            this.btnUndo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUndo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUndo.Location = new System.Drawing.Point(281, 426);
            this.btnUndo.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(49, 25);
            this.btnUndo.TabIndex = 41;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = false;
            // 
            // btnRevertToImported
            // 
            this.btnRevertToImported.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRevertToImported.AutoSize = true;
            this.btnRevertToImported.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRevertToImported.BackColor = System.Drawing.Color.White;
            this.btnRevertToImported.Enabled = false;
            this.btnRevertToImported.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRevertToImported.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRevertToImported.Location = new System.Drawing.Point(336, 426);
            this.btnRevertToImported.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnRevertToImported.Name = "btnRevertToImported";
            this.btnRevertToImported.Size = new System.Drawing.Size(125, 25);
            this.btnRevertToImported.TabIndex = 42;
            this.btnRevertToImported.Text = "Revert to Imported";
            this.btnRevertToImported.UseVisualStyleBackColor = false;
            // 
            // EditUserAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnRevertToImported);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblConnection);
            this.Controls.Add(this.flpConfiguration);
            this.Controls.Add(this.tc);
            this.Controls.Add(this.nudMoveSteps);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.picMoveDown);
            this.Controls.Add(this.picMoveUp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblLabel);
            this.Controls.Add(this.txtLabel);
            this.Name = "EditUserAction";
            this.Size = new System.Drawing.Size(687, 458);
            ((System.ComponentModel.ISupportInitialize)(this.picMoveUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMoveDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMoveSteps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogEntries)).EndInit();
            this.tc.ResumeLayout(false);
            this.tpStructured.ResumeLayout(false);
            this.tpPlainText.ResumeLayout(false);
            this.flpConfiguration.ResumeLayout(false);
            this.flpConfiguration.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLabel;
        private System.Windows.Forms.TextBox txtLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picMoveUp;
        private System.Windows.Forms.PictureBox picMoveDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudMoveSteps;
        private System.Windows.Forms.DataGridView dgvLogEntries;
        private Util.TabControlWithAdjustableBorders tc;
        private System.Windows.Forms.TabPage tpStructured;
        private System.Windows.Forms.TabPage tpPlainText;
        private FastColoredTextBoxNS.FastColoredTextBox fctxtxPlainText;
        private Util.LinkButton lbtnEditable;
        private Util.LinkButton lbtnAsImported;
        private System.Windows.Forms.FlowLayoutPanel flpConfiguration;
        private System.Windows.Forms.Label lblConnection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnRevertToImported;
    }
}
