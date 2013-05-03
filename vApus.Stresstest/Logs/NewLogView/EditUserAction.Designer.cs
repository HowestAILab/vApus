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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditUserAction));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.lblConnection = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnShowHideParameterTokens = new System.Windows.Forms.Button();
            this.picCopy = new System.Windows.Forms.PictureBox();
            this.picDelay = new System.Windows.Forms.PictureBox();
            this.btnMerge = new System.Windows.Forms.Button();
            this.btnSplit = new System.Windows.Forms.Button();
            this.split = new System.Windows.Forms.SplitContainer();
            this.pnlBorderTokens = new System.Windows.Forms.Panel();
            this.cboParameterScope = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.flpTokens = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.flpConfiguration = new System.Windows.Forms.FlowLayoutPanel();
            this.lbtnAsImported = new vApus.Util.LinkButton();
            this.lbtnEditable = new vApus.Util.LinkButton();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnRevertToImported = new System.Windows.Forms.Button();
            this.lblLinkTo = new System.Windows.Forms.Label();
            this.flpLink = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.picMoveUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMoveDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMoveSteps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogEntries)).BeginInit();
            this.tc.SuspendLayout();
            this.tpStructured.SuspendLayout();
            this.tpPlainText.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fctxtxPlainText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.pnlBorderTokens.SuspendLayout();
            this.flpConfiguration.SuspendLayout();
            this.flpLink.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLabel
            // 
            this.lblLabel.AutoSize = true;
            this.lblLabel.Location = new System.Drawing.Point(13, 39);
            this.lblLabel.Name = "lblLabel";
            this.lblLabel.Size = new System.Drawing.Size(36, 13);
            this.lblLabel.TabIndex = 26;
            this.lblLabel.Text = "Label:";
            // 
            // txtLabel
            // 
            this.txtLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLabel.Location = new System.Drawing.Point(61, 36);
            this.txtLabel.Name = "txtLabel";
            this.txtLabel.Size = new System.Drawing.Size(930, 20);
            this.txtLabel.TabIndex = 25;
            this.txtLabel.TextChanged += new System.EventHandler(this.txtLabel_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Move:";
            // 
            // picMoveUp
            // 
            this.picMoveUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("picMoveUp.Image")));
            this.picMoveUp.Location = new System.Drawing.Point(63, 67);
            this.picMoveUp.Name = "picMoveUp";
            this.picMoveUp.Size = new System.Drawing.Size(16, 16);
            this.picMoveUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picMoveUp.TabIndex = 28;
            this.picMoveUp.TabStop = false;
            this.picMoveUp.Click += new System.EventHandler(this.picMoveUp_Click);
            // 
            // picMoveDown
            // 
            this.picMoveDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("picMoveDown.Image")));
            this.picMoveDown.Location = new System.Drawing.Point(83, 67);
            this.picMoveDown.Name = "picMoveDown";
            this.picMoveDown.Size = new System.Drawing.Size(16, 16);
            this.picMoveDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picMoveDown.TabIndex = 29;
            this.picMoveDown.TabStop = false;
            this.picMoveDown.Click += new System.EventHandler(this.picMoveDown_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(105, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Steps:";
            // 
            // nudMoveSteps
            // 
            this.nudMoveSteps.Location = new System.Drawing.Point(148, 65);
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
            this.dgvLogEntries.AllowDrop = true;
            this.dgvLogEntries.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            this.dgvLogEntries.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
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
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgvLogEntries.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvLogEntries.Size = new System.Drawing.Size(982, 386);
            this.dgvLogEntries.TabIndex = 32;
            this.dgvLogEntries.VirtualMode = true;
            this.dgvLogEntries.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvLogEntries_CellValueNeeded);
            this.dgvLogEntries.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvLogEntries_CellValuePushed);
            this.dgvLogEntries.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvLogEntries_DragDrop);
            this.dgvLogEntries.DragOver += new System.Windows.Forms.DragEventHandler(this.dgvLogEntries_DragOver);
            this.dgvLogEntries.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvLogEntries_KeyUp);
            this.dgvLogEntries.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvLogEntries_MouseDown);
            this.dgvLogEntries.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dgvLogEntries_MouseMove);
            // 
            // tc
            // 
            this.tc.BottomVisible = false;
            this.tc.Controls.Add(this.tpStructured);
            this.tc.Controls.Add(this.tpPlainText);
            this.tc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tc.LeftVisible = false;
            this.tc.Location = new System.Drawing.Point(0, 0);
            this.tc.Name = "tc";
            this.tc.RightVisible = false;
            this.tc.SelectedIndex = 0;
            this.tc.Size = new System.Drawing.Size(988, 449);
            this.tc.TabIndex = 33;
            this.tc.TopVisible = true;
            this.tc.SelectedIndexChanged += new System.EventHandler(this.tc_SelectedIndexChanged);
            // 
            // tpStructured
            // 
            this.tpStructured.BackColor = System.Drawing.Color.White;
            this.tpStructured.Controls.Add(this.dgvLogEntries);
            this.tpStructured.Location = new System.Drawing.Point(0, 22);
            this.tpStructured.Name = "tpStructured";
            this.tpStructured.Padding = new System.Windows.Forms.Padding(3);
            this.tpStructured.Size = new System.Drawing.Size(987, 426);
            this.tpStructured.TabIndex = 0;
            this.tpStructured.Text = "Structured";
            // 
            // tpPlainText
            // 
            this.tpPlainText.BackColor = System.Drawing.Color.White;
            this.tpPlainText.Controls.Add(this.fctxtxPlainText);
            this.tpPlainText.Location = new System.Drawing.Point(0, 22);
            this.tpPlainText.Name = "tpPlainText";
            this.tpPlainText.Padding = new System.Windows.Forms.Padding(3);
            this.tpPlainText.Size = new System.Drawing.Size(987, 426);
            this.tpPlainText.TabIndex = 1;
            this.tpPlainText.Text = "Plain Text";
            // 
            // fctxtxPlainText
            // 
            this.fctxtxPlainText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fctxtxPlainText.AutoScrollMinSize = new System.Drawing.Size(0, 14);
            this.fctxtxPlainText.BackBrush = null;
            this.fctxtxPlainText.CharHeight = 14;
            this.fctxtxPlainText.CharWidth = 8;
            this.fctxtxPlainText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctxtxPlainText.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctxtxPlainText.IsReplaceMode = false;
            this.fctxtxPlainText.Location = new System.Drawing.Point(3, 3);
            this.fctxtxPlainText.Name = "fctxtxPlainText";
            this.fctxtxPlainText.Paddings = new System.Windows.Forms.Padding(0);
            this.fctxtxPlainText.PreferredLineWidth = 65536;
            this.fctxtxPlainText.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctxtxPlainText.Size = new System.Drawing.Size(981, 386);
            this.fctxtxPlainText.TabIndex = 1;
            this.fctxtxPlainText.WordWrap = true;
            this.fctxtxPlainText.Zoom = 100;
            this.fctxtxPlainText.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fctxtxPlainText_TextChanged);
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
            this.label3.Location = new System.Drawing.Point(6, 173);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "Log Entries";
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 20;
            // 
            // btnShowHideParameterTokens
            // 
            this.btnShowHideParameterTokens.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnShowHideParameterTokens.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnShowHideParameterTokens.BackColor = System.Drawing.Color.White;
            this.btnShowHideParameterTokens.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowHideParameterTokens.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowHideParameterTokens.Location = new System.Drawing.Point(515, 609);
            this.btnShowHideParameterTokens.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnShowHideParameterTokens.Name = "btnShowHideParameterTokens";
            this.btnShowHideParameterTokens.Size = new System.Drawing.Size(157, 25);
            this.btnShowHideParameterTokens.TabIndex = 72;
            this.btnShowHideParameterTokens.Text = "Show Parameter Tokens";
            this.toolTip.SetToolTip(this.btnShowHideParameterTokens, "Show Parameter Tokens");
            this.btnShowHideParameterTokens.UseVisualStyleBackColor = false;
            this.btnShowHideParameterTokens.Click += new System.EventHandler(this.btnShowHideParameterTokens_Click);
            // 
            // picCopy
            // 
            this.picCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picCopy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picCopy.Image = ((System.Drawing.Image)(resources.GetObject("picCopy.Image")));
            this.picCopy.Location = new System.Drawing.Point(903, 67);
            this.picCopy.Name = "picCopy";
            this.picCopy.Size = new System.Drawing.Size(16, 16);
            this.picCopy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picCopy.TabIndex = 73;
            this.picCopy.TabStop = false;
            this.toolTip.SetToolTip(this.picCopy, "Copy this user action");
            this.picCopy.Click += new System.EventHandler(this.picCopy_Click);
            // 
            // picDelay
            // 
            this.picDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picDelay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picDelay.Image = global::vApus.Stresstest.Properties.Resources.Delay;
            this.picDelay.Location = new System.Drawing.Point(925, 67);
            this.picDelay.Name = "picDelay";
            this.picDelay.Size = new System.Drawing.Size(16, 16);
            this.picDelay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDelay.TabIndex = 74;
            this.picDelay.TabStop = false;
            this.toolTip.SetToolTip(this.picDelay, "Click to NOT use delay after this user action.\r\nDelay is determined in the stress" +
        "test settings.");
            this.picDelay.Click += new System.EventHandler(this.picDelay_Click);
            // 
            // btnMerge
            // 
            this.btnMerge.AutoSize = true;
            this.btnMerge.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMerge.BackColor = System.Drawing.Color.White;
            this.btnMerge.Enabled = false;
            this.btnMerge.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMerge.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMerge.Location = new System.Drawing.Point(3, 3);
            this.btnMerge.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(54, 25);
            this.btnMerge.TabIndex = 71;
            this.btnMerge.Text = "Merge";
            this.toolTip.SetToolTip(this.btnMerge, "Merge all linked user actions into a new one.");
            this.btnMerge.UseVisualStyleBackColor = false;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // btnSplit
            // 
            this.btnSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSplit.AutoSize = true;
            this.btnSplit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSplit.BackColor = System.Drawing.Color.White;
            this.btnSplit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSplit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSplit.Location = new System.Drawing.Point(947, 61);
            this.btnSplit.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnSplit.Name = "btnSplit";
            this.btnSplit.Size = new System.Drawing.Size(44, 25);
            this.btnSplit.TabIndex = 77;
            this.btnSplit.Text = "Split";
            this.toolTip.SetToolTip(this.btnSplit, "Split all log entries in seperate user actions.");
            this.btnSplit.UseVisualStyleBackColor = false;
            this.btnSplit.Click += new System.EventHandler(this.btnSplit_Click);
            // 
            // split
            // 
            this.split.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split.BackColor = System.Drawing.SystemColors.Control;
            this.split.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.split.Location = new System.Drawing.Point(9, 192);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.BackColor = System.Drawing.Color.White;
            this.split.Panel1.Controls.Add(this.tc);
            // 
            // split.Panel2
            // 
            this.split.Panel2.BackColor = System.Drawing.Color.White;
            this.split.Panel2.Controls.Add(this.pnlBorderTokens);
            this.split.Panel2.Controls.Add(this.label4);
            this.split.Panel2.Controls.Add(this.flpTokens);
            this.split.Panel2.Controls.Add(this.label5);
            this.split.Panel2.Controls.Add(this.label6);
            this.split.Panel2Collapsed = true;
            this.split.Size = new System.Drawing.Size(988, 449);
            this.split.SplitterDistance = 588;
            this.split.TabIndex = 46;
            // 
            // pnlBorderTokens
            // 
            this.pnlBorderTokens.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderTokens.Controls.Add(this.cboParameterScope);
            this.pnlBorderTokens.Location = new System.Drawing.Point(9, 47);
            this.pnlBorderTokens.Name = "pnlBorderTokens";
            this.pnlBorderTokens.Size = new System.Drawing.Size(323, 23);
            this.pnlBorderTokens.TabIndex = 49;
            // 
            // cboParameterScope
            // 
            this.cboParameterScope.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboParameterScope.BackColor = System.Drawing.Color.White;
            this.cboParameterScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboParameterScope.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboParameterScope.FormattingEnabled = true;
            this.cboParameterScope.Items.AddRange(new object[] {
            "<All>",
            "once in the log",
            "once in the parent user action",
            "once in this log entry",
            "once in the leaf node",
            "for every call"});
            this.cboParameterScope.Location = new System.Drawing.Point(1, 1);
            this.cboParameterScope.Name = "cboParameterScope";
            this.cboParameterScope.Size = new System.Drawing.Size(321, 21);
            this.cboParameterScope.TabIndex = 7;
            this.cboParameterScope.SelectedIndexChanged += new System.EventHandler(this.cboParameterScope_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(5, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Parameter Tokens:";
            // 
            // flpTokens
            // 
            this.flpTokens.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flpTokens.AutoScroll = true;
            this.flpTokens.BackColor = System.Drawing.Color.White;
            this.flpTokens.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpTokens.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.flpTokens.Location = new System.Drawing.Point(9, 104);
            this.flpTokens.Name = "flpTokens";
            this.flpTokens.Size = new System.Drawing.Size(323, 387);
            this.flpTokens.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(192, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "This is for every user executing this log.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(271, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Show tokens for parameter values that are redetermined";
            // 
            // flpConfiguration
            // 
            this.flpConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flpConfiguration.Controls.Add(this.lbtnAsImported);
            this.flpConfiguration.Controls.Add(this.lbtnEditable);
            this.flpConfiguration.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpConfiguration.Location = new System.Drawing.Point(844, 604);
            this.flpConfiguration.Name = "flpConfiguration";
            this.flpConfiguration.Size = new System.Drawing.Size(150, 31);
            this.flpConfiguration.TabIndex = 48;
            // 
            // lbtnAsImported
            // 
            this.lbtnAsImported.Active = false;
            this.lbtnAsImported.ActiveLinkColor = System.Drawing.Color.DimGray;
            this.lbtnAsImported.AutoSize = true;
            this.lbtnAsImported.BackColor = System.Drawing.Color.Transparent;
            this.lbtnAsImported.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnAsImported.ForeColor = System.Drawing.Color.DimGray;
            this.lbtnAsImported.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnAsImported.LinkColor = System.Drawing.Color.DimGray;
            this.lbtnAsImported.Location = new System.Drawing.Point(70, 6);
            this.lbtnAsImported.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnAsImported.Name = "lbtnAsImported";
            this.lbtnAsImported.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnAsImported.RadioButtonBehavior = true;
            this.lbtnAsImported.Size = new System.Drawing.Size(80, 20);
            this.lbtnAsImported.TabIndex = 35;
            this.lbtnAsImported.TabStop = true;
            this.lbtnAsImported.Text = "As Imported";
            this.lbtnAsImported.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnAsImported.VisitedLinkColor = System.Drawing.Color.DimGray;
            this.lbtnAsImported.ActiveChanged += new System.EventHandler(this.lbtn_ActiveChanged);
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
            this.lbtnEditable.Location = new System.Drawing.Point(6, 6);
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
            // btnApply
            // 
            this.btnApply.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnApply.AutoSize = true;
            this.btnApply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnApply.BackColor = System.Drawing.Color.White;
            this.btnApply.Enabled = false;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.Location = new System.Drawing.Point(328, 609);
            this.btnApply.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(50, 25);
            this.btnApply.TabIndex = 70;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Visible = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnRevertToImported
            // 
            this.btnRevertToImported.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRevertToImported.AutoSize = true;
            this.btnRevertToImported.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRevertToImported.BackColor = System.Drawing.Color.White;
            this.btnRevertToImported.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRevertToImported.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRevertToImported.Location = new System.Drawing.Point(384, 609);
            this.btnRevertToImported.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnRevertToImported.Name = "btnRevertToImported";
            this.btnRevertToImported.Size = new System.Drawing.Size(125, 25);
            this.btnRevertToImported.TabIndex = 71;
            this.btnRevertToImported.Text = "Revert to Imported";
            this.btnRevertToImported.UseVisualStyleBackColor = false;
            this.btnRevertToImported.Click += new System.EventHandler(this.btnRevertToImported_Click);
            // 
            // lblLinkTo
            // 
            this.lblLinkTo.AutoSize = true;
            this.lblLinkTo.Location = new System.Drawing.Point(13, 101);
            this.lblLinkTo.Name = "lblLinkTo";
            this.lblLinkTo.Size = new System.Drawing.Size(42, 13);
            this.lblLinkTo.TabIndex = 75;
            this.lblLinkTo.Text = "Link to:";
            // 
            // flpLink
            // 
            this.flpLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpLink.AutoScroll = true;
            this.flpLink.Controls.Add(this.btnMerge);
            this.flpLink.Location = new System.Drawing.Point(61, 94);
            this.flpLink.Name = "flpLink";
            this.flpLink.Size = new System.Drawing.Size(930, 70);
            this.flpLink.TabIndex = 70;
            // 
            // EditUserAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnSplit);
            this.Controls.Add(this.flpLink);
            this.Controls.Add(this.lblLinkTo);
            this.Controls.Add(this.picDelay);
            this.Controls.Add(this.picCopy);
            this.Controls.Add(this.btnShowHideParameterTokens);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnRevertToImported);
            this.Controls.Add(this.flpConfiguration);
            this.Controls.Add(this.split);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblConnection);
            this.Controls.Add(this.nudMoveSteps);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.picMoveDown);
            this.Controls.Add(this.picMoveUp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblLabel);
            this.Controls.Add(this.txtLabel);
            this.Name = "EditUserAction";
            this.Size = new System.Drawing.Size(1000, 641);
            ((System.ComponentModel.ISupportInitialize)(this.picMoveUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMoveDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMoveSteps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogEntries)).EndInit();
            this.tc.ResumeLayout(false);
            this.tpStructured.ResumeLayout(false);
            this.tpPlainText.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fctxtxPlainText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelay)).EndInit();
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            this.split.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.pnlBorderTokens.ResumeLayout(false);
            this.flpConfiguration.ResumeLayout(false);
            this.flpConfiguration.PerformLayout();
            this.flpLink.ResumeLayout(false);
            this.flpLink.PerformLayout();
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
        private System.Windows.Forms.Label lblConnection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FlowLayoutPanel flpTokens;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.FlowLayoutPanel flpConfiguration;
        private Util.LinkButton lbtnAsImported;
        private Util.LinkButton lbtnEditable;
        private System.Windows.Forms.Panel pnlBorderTokens;
        private System.Windows.Forms.ComboBox cboParameterScope;
        private System.Windows.Forms.Button btnShowHideParameterTokens;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnRevertToImported;
        private System.Windows.Forms.PictureBox picCopy;
        private System.Windows.Forms.PictureBox picDelay;
        private System.Windows.Forms.Label lblLinkTo;
        private System.Windows.Forms.FlowLayoutPanel flpLink;
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.Button btnSplit;
    }
}
