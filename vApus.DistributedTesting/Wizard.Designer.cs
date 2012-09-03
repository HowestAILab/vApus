namespace vApus.DistributedTesting
{
    partial class Wizard
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wizard));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkReview = new System.Windows.Forms.CheckBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dgvClients = new System.Windows.Forms.DataGridView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblNotAssignedTests = new System.Windows.Forms.Label();
            this.nudTests = new System.Windows.Forms.NumericUpDown();
            this.nudTiles = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblResultPath = new System.Windows.Forms.Label();
            this.llblResultPath = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.pnlRunSync = new System.Windows.Forms.Panel();
            this.cboRunSync = new System.Windows.Forms.ComboBox();
            this.chkUseRDP = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlAddClientAndSlaves = new System.Windows.Forms.Panel();
            this.nudSlavesPerCores = new System.Windows.Forms.NumericUpDown();
            this.rdbSlavesPerCores = new System.Windows.Forms.RadioButton();
            this.nudSlavesPerClient = new System.Windows.Forms.NumericUpDown();
            this.rdbSlavesPerClient = new System.Windows.Forms.RadioButton();
            this.pnlGenerateAndAddTiles = new System.Windows.Forms.Panel();
            this.rdbDoNotAddTiles = new System.Windows.Forms.RadioButton();
            this.rdbStartFromScratch = new System.Windows.Forms.RadioButton();
            this.rdbAppendTiles = new System.Windows.Forms.RadioButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.clmIPorHostName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmUserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDomain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmPassword = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmSlaves = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmTests = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmCpuCores = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTests)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTiles)).BeginInit();
            this.pnlRunSync.SuspendLayout();
            this.pnlAddClientAndSlaves.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSlavesPerCores)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSlavesPerClient)).BeginInit();
            this.pnlGenerateAndAddTiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.chkReview);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.dgvClients);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.lblNotAssignedTests);
            this.panel1.Controls.Add(this.nudTests);
            this.panel1.Controls.Add(this.nudTiles);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.lblResultPath);
            this.panel1.Controls.Add(this.llblResultPath);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.pnlRunSync);
            this.panel1.Controls.Add(this.chkUseRDP);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pnlAddClientAndSlaves);
            this.panel1.Controls.Add(this.pnlGenerateAndAddTiles);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(764, 562);
            this.panel1.TabIndex = 1;
            // 
            // chkReview
            // 
            this.chkReview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkReview.AutoSize = true;
            this.chkReview.Checked = true;
            this.chkReview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReview.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkReview.Location = new System.Drawing.Point(323, 531);
            this.chkReview.Name = "chkReview";
            this.chkReview.Size = new System.Drawing.Size(261, 17);
            this.chkReview.TabIndex = 27;
            this.chkReview.Text = "Review Connection Usage before Closing";
            this.chkReview.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(676, 448);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(70, 23);
            this.btnRefresh.TabIndex = 24;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // dgvClients
            // 
            this.dgvClients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvClients.BackgroundColor = System.Drawing.Color.White;
            this.dgvClients.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmIPorHostName,
            this.clmUserName,
            this.clmDomain,
            this.clmPassword,
            this.clmSlaves,
            this.clmTests,
            this.clmCpuCores});
            this.dgvClients.EnableHeadersVisualStyles = false;
            this.dgvClients.Location = new System.Drawing.Point(20, 295);
            this.dgvClients.Name = "dgvClients";
            this.dgvClients.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvClients.Size = new System.Drawing.Size(726, 150);
            this.dgvClients.TabIndex = 23;
            this.dgvClients.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvClients_CellFormatting);
            this.dgvClients.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvClients_EditingControlShowing);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(671, 527);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(590, 527);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 21;
            this.btnOK.Text = "OK";
            this.toolTip.SetToolTip(this.btnOK, "Connections will be redistributed over existing tile stresstests if needed.");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblNotAssignedTests
            // 
            this.lblNotAssignedTests.AutoSize = true;
            this.lblNotAssignedTests.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblNotAssignedTests.Location = new System.Drawing.Point(17, 479);
            this.lblNotAssignedTests.Name = "lblNotAssignedTests";
            this.lblNotAssignedTests.Size = new System.Drawing.Size(178, 13);
            this.lblNotAssignedTests.TabIndex = 20;
            this.lblNotAssignedTests.Text = "0 Tests are not Assigned to a Slave.";
            // 
            // nudTests
            // 
            this.nudTests.Location = new System.Drawing.Point(149, 201);
            this.nudTests.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTests.Name = "nudTests";
            this.nudTests.Size = new System.Drawing.Size(50, 20);
            this.nudTests.TabIndex = 14;
            this.toolTip.SetToolTip(this.nudTests, "Default value is based on the number of stresstests in the solution.");
            this.nudTests.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudTiles
            // 
            this.nudTiles.Location = new System.Drawing.Point(149, 171);
            this.nudTiles.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTiles.Name = "nudTiles";
            this.nudTiles.Size = new System.Drawing.Size(50, 20);
            this.nudTiles.TabIndex = 13;
            this.nudTiles.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 203);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(126, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Number of Tests per Tile:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 173);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Number of Tiles:";
            // 
            // lblResultPath
            // 
            this.lblResultPath.AutoSize = true;
            this.lblResultPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblResultPath.Location = new System.Drawing.Point(17, 113);
            this.lblResultPath.Name = "lblResultPath";
            this.lblResultPath.Size = new System.Drawing.Size(62, 13);
            this.lblResultPath.TabIndex = 9;
            this.lblResultPath.Text = "Result Path";
            // 
            // llblResultPath
            // 
            this.llblResultPath.Image = ((System.Drawing.Image)(resources.GetObject("llblResultPath.Image")));
            this.llblResultPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.llblResultPath.Location = new System.Drawing.Point(17, 92);
            this.llblResultPath.Name = "llblResultPath";
            this.llblResultPath.Size = new System.Drawing.Size(90, 15);
            this.llblResultPath.TabIndex = 8;
            this.llblResultPath.TabStop = true;
            this.llblResultPath.Text = "Result Path...";
            this.llblResultPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.llblResultPath, "The stresstesting results will be saved to the chosen path.");
            this.llblResultPath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblResultPath_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Run Synchronization:";
            // 
            // pnlRunSync
            // 
            this.pnlRunSync.BackColor = System.Drawing.Color.Silver;
            this.pnlRunSync.Controls.Add(this.cboRunSync);
            this.pnlRunSync.Location = new System.Drawing.Point(128, 58);
            this.pnlRunSync.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.pnlRunSync.Name = "pnlRunSync";
            this.pnlRunSync.Size = new System.Drawing.Size(127, 23);
            this.pnlRunSync.TabIndex = 6;
            // 
            // cboRunSync
            // 
            this.cboRunSync.BackColor = System.Drawing.Color.White;
            this.cboRunSync.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRunSync.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboRunSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboRunSync.FormattingEnabled = true;
            this.cboRunSync.Items.AddRange(new object[] {
            "None",
            "Break on First",
            "Break on Last"});
            this.cboRunSync.Location = new System.Drawing.Point(1, 1);
            this.cboRunSync.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboRunSync.Name = "cboRunSync";
            this.cboRunSync.Size = new System.Drawing.Size(125, 21);
            this.cboRunSync.TabIndex = 0;
            this.toolTip.SetToolTip(this.cboRunSync, resources.GetString("cboRunSync.ToolTip"));
            // 
            // chkUseRDP
            // 
            this.chkUseRDP.AutoSize = true;
            this.chkUseRDP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkUseRDP.Location = new System.Drawing.Point(20, 30);
            this.chkUseRDP.Name = "chkUseRDP";
            this.chkUseRDP.Size = new System.Drawing.Size(205, 17);
            this.chkUseRDP.TabIndex = 5;
            this.chkUseRDP.Text = "Use the vApus Remote Desktop Client";
            this.toolTip.SetToolTip(this.chkUseRDP, "Check this if you want vApus to open remote desktop connections to the used clien" +
                    "ts.\r\nRegardless if you check it or not, you need to be logged into the clients t" +
                    "o be able to stresstest.");
            this.chkUseRDP.UseVisualStyleBackColor = true;
            this.chkUseRDP.CheckedChanged += new System.EventHandler(this.chkUseRDP_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 273);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 12, 3, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Add Clients and Slaves";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 151);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 12, 3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Generate and Add Tiles";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Default Test Settings";
            // 
            // pnlAddClientAndSlaves
            // 
            this.pnlAddClientAndSlaves.Controls.Add(this.nudSlavesPerCores);
            this.pnlAddClientAndSlaves.Controls.Add(this.rdbSlavesPerCores);
            this.pnlAddClientAndSlaves.Controls.Add(this.nudSlavesPerClient);
            this.pnlAddClientAndSlaves.Controls.Add(this.rdbSlavesPerClient);
            this.pnlAddClientAndSlaves.Location = new System.Drawing.Point(12, 448);
            this.pnlAddClientAndSlaves.Name = "pnlAddClientAndSlaves";
            this.pnlAddClientAndSlaves.Size = new System.Drawing.Size(451, 28);
            this.pnlAddClientAndSlaves.TabIndex = 25;
            // 
            // nudSlavesPerCores
            // 
            this.nudSlavesPerCores.Location = new System.Drawing.Point(86, 3);
            this.nudSlavesPerCores.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSlavesPerCores.Name = "nudSlavesPerCores";
            this.nudSlavesPerCores.Size = new System.Drawing.Size(50, 20);
            this.nudSlavesPerCores.TabIndex = 17;
            this.toolTip.SetToolTip(this.nudSlavesPerCores, "Default value is based on the number of stresstests in the solution.");
            this.nudSlavesPerCores.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSlavesPerCores.ValueChanged += new System.EventHandler(this.nudSlavesPerCores_ValueChanged);
            // 
            // rdbSlavesPerCores
            // 
            this.rdbSlavesPerCores.AutoSize = true;
            this.rdbSlavesPerCores.Checked = true;
            this.rdbSlavesPerCores.Location = new System.Drawing.Point(8, 3);
            this.rdbSlavesPerCores.Name = "rdbSlavesPerCores";
            this.rdbSlavesPerCores.Size = new System.Drawing.Size(247, 17);
            this.rdbSlavesPerCores.TabIndex = 16;
            this.rdbSlavesPerCores.TabStop = true;
            this.rdbSlavesPerCores.Text = "1 Slave per                    CPU Core(s) of a Client";
            this.toolTip.SetToolTip(this.rdbSlavesPerCores, "If possible the processor affinity will be set for the slaves.\r\n(See the \'Slaves\'" +
                    " tab page when finished)");
            this.rdbSlavesPerCores.UseVisualStyleBackColor = true;
            this.rdbSlavesPerCores.CheckedChanged += new System.EventHandler(this.rdbSlavesPerCores_CheckedChanged);
            // 
            // nudSlavesPerClient
            // 
            this.nudSlavesPerClient.Location = new System.Drawing.Point(291, 3);
            this.nudSlavesPerClient.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSlavesPerClient.Name = "nudSlavesPerClient";
            this.nudSlavesPerClient.Size = new System.Drawing.Size(50, 20);
            this.nudSlavesPerClient.TabIndex = 19;
            this.toolTip.SetToolTip(this.nudSlavesPerClient, "Default value is based on the number of stresstests in the solution.");
            this.nudSlavesPerClient.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSlavesPerClient.ValueChanged += new System.EventHandler(this.nudSlavesPerClient_ValueChanged);
            // 
            // rdbSlavesPerClient
            // 
            this.rdbSlavesPerClient.AutoSize = true;
            this.rdbSlavesPerClient.Location = new System.Drawing.Point(275, 3);
            this.rdbSlavesPerClient.Name = "rdbSlavesPerClient";
            this.rdbSlavesPerClient.Size = new System.Drawing.Size(161, 17);
            this.rdbSlavesPerClient.TabIndex = 18;
            this.rdbSlavesPerClient.Text = "                 Slave(s) per Client";
            this.toolTip.SetToolTip(this.rdbSlavesPerClient, "If possible the processor affinity will be set for the slaves.\r\n(See the \'Slaves\'" +
                    " tab page when finished)");
            this.rdbSlavesPerClient.UseVisualStyleBackColor = true;
            // 
            // pnlGenerateAndAddTiles
            // 
            this.pnlGenerateAndAddTiles.Controls.Add(this.rdbDoNotAddTiles);
            this.pnlGenerateAndAddTiles.Controls.Add(this.rdbStartFromScratch);
            this.pnlGenerateAndAddTiles.Controls.Add(this.rdbAppendTiles);
            this.pnlGenerateAndAddTiles.Location = new System.Drawing.Point(12, 226);
            this.pnlGenerateAndAddTiles.Name = "pnlGenerateAndAddTiles";
            this.pnlGenerateAndAddTiles.Size = new System.Drawing.Size(451, 27);
            this.pnlGenerateAndAddTiles.TabIndex = 26;
            // 
            // rdbDoNotAddTiles
            // 
            this.rdbDoNotAddTiles.AutoSize = true;
            this.rdbDoNotAddTiles.Location = new System.Drawing.Point(217, 3);
            this.rdbDoNotAddTiles.Name = "rdbDoNotAddTiles";
            this.rdbDoNotAddTiles.Size = new System.Drawing.Size(230, 17);
            this.rdbDoNotAddTiles.TabIndex = 2;
            this.rdbDoNotAddTiles.Text = "Only Reassign the Stresstests to the Slaves";
            this.toolTip.SetToolTip(this.rdbDoNotAddTiles, "No Tiles are added to the distributed test if you choose this.\r\nConnections will " +
                    "be redistributed over existing tile stresstests if needed.");
            this.rdbDoNotAddTiles.UseVisualStyleBackColor = true;
            this.rdbDoNotAddTiles.CheckedChanged += new System.EventHandler(this.rdbsGenerateAndtAddTiles_CheckedChanged);
            // 
            // rdbStartFromScratch
            // 
            this.rdbStartFromScratch.AutoSize = true;
            this.rdbStartFromScratch.Location = new System.Drawing.Point(101, 3);
            this.rdbStartFromScratch.Name = "rdbStartFromScratch";
            this.rdbStartFromScratch.Size = new System.Drawing.Size(110, 17);
            this.rdbStartFromScratch.TabIndex = 1;
            this.rdbStartFromScratch.Text = "Start from Scratch";
            this.toolTip.SetToolTip(this.rdbStartFromScratch, "Remove the existing tiles and add new ones.");
            this.rdbStartFromScratch.UseVisualStyleBackColor = true;
            this.rdbStartFromScratch.CheckedChanged += new System.EventHandler(this.rdbsGenerateAndtAddTiles_CheckedChanged);
            // 
            // rdbAppendTiles
            // 
            this.rdbAppendTiles.AutoSize = true;
            this.rdbAppendTiles.Checked = true;
            this.rdbAppendTiles.Location = new System.Drawing.Point(8, 3);
            this.rdbAppendTiles.Name = "rdbAppendTiles";
            this.rdbAppendTiles.Size = new System.Drawing.Size(87, 17);
            this.rdbAppendTiles.TabIndex = 0;
            this.rdbAppendTiles.TabStop = true;
            this.rdbAppendTiles.Text = "Append Tiles";
            this.toolTip.SetToolTip(this.rdbAppendTiles, "Connections will be redistributed over existing tile stresstests if needed.");
            this.rdbAppendTiles.UseVisualStyleBackColor = true;
            this.rdbAppendTiles.CheckedChanged += new System.EventHandler(this.rdbsGenerateAndtAddTiles_CheckedChanged);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 20;
            // 
            // clmIPorHostName
            // 
            this.clmIPorHostName.HeaderText = "* IP or Host Name";
            this.clmIPorHostName.Name = "clmIPorHostName";
            this.clmIPorHostName.ToolTipText = "Typing the IP address is adviced.\nIf you want to avoid firewall issues the slave must be in the same IP range as the master.";
            // 
            // clmUserName
            // 
            this.clmUserName.HeaderText = "User Name (RDP)";
            this.clmUserName.Name = "clmUserName";
            // 
            // clmDomain
            // 
            this.clmDomain.HeaderText = "Domain";
            this.clmDomain.Name = "clmDomain";
            this.clmDomain.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clmDomain.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmPassword
            // 
            this.clmPassword.HeaderText = "Password";
            this.clmPassword.Name = "clmPassword";
            // 
            // clmSlaves
            // 
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.clmSlaves.DefaultCellStyle = dataGridViewCellStyle1;
            this.clmSlaves.HeaderText = "Number of Slaves (0)";
            this.clmSlaves.Name = "clmSlaves";
            this.clmSlaves.ReadOnly = true;
            this.clmSlaves.Width = 85;
            // 
            // clmTests
            // 
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.clmTests.DefaultCellStyle = dataGridViewCellStyle2;
            this.clmTests.HeaderText = "Number of Tests (0/?)";
            this.clmTests.Name = "clmTests";
            this.clmTests.ReadOnly = true;
            this.clmTests.Width = 85;
            // 
            // clmCpuCores
            // 
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.clmCpuCores.DefaultCellStyle = dataGridViewCellStyle3;
            this.clmCpuCores.HeaderText = "Number of CPU Cores";
            this.clmCpuCores.Name = "clmCpuCores";
            this.clmCpuCores.ReadOnly = true;
            this.clmCpuCores.Width = 85;
            // 
            // Wizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(764, 562);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(780, 600);
            this.MinimizeBox = false;
            this.Name = "Wizard";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wizard - Let vApus build your distributed test ";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTests)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTiles)).EndInit();
            this.pnlRunSync.ResumeLayout(false);
            this.pnlAddClientAndSlaves.ResumeLayout(false);
            this.pnlAddClientAndSlaves.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSlavesPerCores)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSlavesPerClient)).EndInit();
            this.pnlGenerateAndAddTiles.ResumeLayout(false);
            this.pnlGenerateAndAddTiles.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkUseRDP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pnlRunSync;
        private System.Windows.Forms.ComboBox cboRunSync;
        private System.Windows.Forms.LinkLabel llblResultPath;
        private System.Windows.Forms.Label lblResultPath;
        private System.Windows.Forms.NumericUpDown nudTests;
        private System.Windows.Forms.NumericUpDown nudTiles;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nudSlavesPerClient;
        private System.Windows.Forms.RadioButton rdbSlavesPerClient;
        private System.Windows.Forms.NumericUpDown nudSlavesPerCores;
        private System.Windows.Forms.RadioButton rdbSlavesPerCores;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblNotAssignedTests;
        private System.Windows.Forms.DataGridView dgvClients;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel pnlAddClientAndSlaves;
        private System.Windows.Forms.Panel pnlGenerateAndAddTiles;
        private System.Windows.Forms.RadioButton rdbStartFromScratch;
        private System.Windows.Forms.RadioButton rdbAppendTiles;
        private System.Windows.Forms.RadioButton rdbDoNotAddTiles;
        private System.Windows.Forms.CheckBox chkReview;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmIPorHostName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmUserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDomain;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPassword;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSlaves;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmTests;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmCpuCores;

    }
}