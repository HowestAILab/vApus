namespace vApus.DistributedTesting
{
    partial class DistributedTestView
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
            tmrSchedule.Stop();
            try
            {
                SolutionTree.SolutionComponent.SolutionComponentChanged -= SolutionComponent_SolutionComponentChanged;
            }
            catch { }

            foreach (TileStresstestSelectorControl tileStresstestSelectorControl in flpStresstestTileStresstests.Controls)
                tileStresstestSelectorControl.StopMonitorIfAny();

            if (_distributedTestCore != null)
                _distributedTestCore.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DistributedTestView));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnSchedule = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.btnLinkageOverview = new System.Windows.Forms.ToolStripButton();
            this.cboRunSynchronization = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tc = new vApus.Util.TabControlWithAdjustableBorders();
            this.tpConfigure = new System.Windows.Forms.TabPage();
            this.splitConfigure = new System.Windows.Forms.SplitContainer();
            this.btnRemove = new System.Windows.Forms.Button();
            this.tvwTiles = new System.Windows.Forms.TreeView();
            this.btnAddTile = new System.Windows.Forms.Button();
            this.btnResetToDefaults = new System.Windows.Forms.Button();
            this.propertiesSolutionComponentPropertyPanel = new vApus.SolutionTree.SolutionComponentPropertyPanel();
            this.tpStresstest = new System.Windows.Forms.TabPage();
            this.splitStresstest = new System.Windows.Forms.SplitContainer();
            this.pnlDistributedTestStresstest = new System.Windows.Forms.Panel();
            this.btnCollapseExpandStresstest = new System.Windows.Forms.Button();
            this.flpStresstestTiles = new System.Windows.Forms.FlowLayoutPanel();
            this.flpStresstestTileStresstests = new System.Windows.Forms.FlowLayoutPanel();
            this.distributedStresstestControl = new vApus.DistributedTesting.DistributedStresstestControl();
            this.tpReport = new System.Windows.Forms.TabPage();
            this.splitReport = new System.Windows.Forms.SplitContainer();
            this.pnlDistributedTestReport = new System.Windows.Forms.Panel();
            this.btnCollapseExpandReport = new System.Windows.Forms.Button();
            this.flpReportTiles = new System.Windows.Forms.FlowLayoutPanel();
            this.flpReportTileStresstests = new System.Windows.Forms.FlowLayoutPanel();
            this.tcReport = new vApus.Util.TabControlWithAdjustableBorders();
            this.tpStresstestReport = new System.Windows.Forms.TabPage();
            this.btnOpenRFilesFolder = new System.Windows.Forms.Button();
            this.stresstestReportControl = new vApus.Stresstest.StresstestReportControl();
            this.tmrSchedule = new System.Windows.Forms.Timer(this.components);
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.tmrProgressDelayCountDown = new System.Windows.Forms.Timer(this.components);
            this.tmrProgress = new System.Windows.Forms.Timer(this.components);
            this.tmrCheckTvwTiles = new System.Windows.Forms.Timer(this.components);
            this.toolStrip.SuspendLayout();
            this.tc.SuspendLayout();
            this.tpConfigure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitConfigure)).BeginInit();
            this.splitConfigure.Panel1.SuspendLayout();
            this.splitConfigure.Panel2.SuspendLayout();
            this.splitConfigure.SuspendLayout();
            this.tpStresstest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitStresstest)).BeginInit();
            this.splitStresstest.Panel1.SuspendLayout();
            this.splitStresstest.Panel2.SuspendLayout();
            this.splitStresstest.SuspendLayout();
            this.pnlDistributedTestStresstest.SuspendLayout();
            this.tpReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitReport)).BeginInit();
            this.splitReport.Panel1.SuspendLayout();
            this.splitReport.Panel2.SuspendLayout();
            this.splitReport.SuspendLayout();
            this.pnlDistributedTestReport.SuspendLayout();
            this.tcReport.SuspendLayout();
            this.tpStresstestReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStart,
            this.btnSchedule,
            this.btnStop,
            this.btnLinkageOverview,
            this.cboRunSynchronization,
            this.toolStripLabel1});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.MinimumSize = new System.Drawing.Size(0, 40);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(884, 40);
            this.toolStrip.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 37);
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnSchedule
            // 
            this.btnSchedule.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSchedule.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSchedule.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSchedule.Margin = new System.Windows.Forms.Padding(-9, 1, 0, 2);
            this.btnSchedule.Name = "btnSchedule";
            this.btnSchedule.Size = new System.Drawing.Size(68, 37);
            this.btnSchedule.Tag = "";
            this.btnSchedule.Text = "Schedule...";
            this.btnSchedule.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSchedule.Click += new System.EventHandler(this.btnSchedule_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(90, 37);
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnLinkageOverview
            // 
            this.btnLinkageOverview.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnLinkageOverview.Image = ((System.Drawing.Image)(resources.GetObject("btnLinkageOverview.Image")));
            this.btnLinkageOverview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLinkageOverview.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.btnLinkageOverview.Name = "btnLinkageOverview";
            this.btnLinkageOverview.Size = new System.Drawing.Size(120, 37);
            this.btnLinkageOverview.Text = "Linkage Overview";
            this.btnLinkageOverview.Click += new System.EventHandler(this.btnLinkageOverview_Click);
            // 
            // cboRunSynchronization
            // 
            this.cboRunSynchronization.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.cboRunSynchronization.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRunSynchronization.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboRunSynchronization.Items.AddRange(new object[] {
            "None",
            "Break on First Finished",
            "Break on Last Finished"});
            this.cboRunSynchronization.Name = "cboRunSynchronization";
            this.cboRunSynchronization.Size = new System.Drawing.Size(141, 40);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(59, 37);
            this.toolStripLabel1.Text = "Run Sync:";
            // 
            // tc
            // 
            this.tc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tc.BottomVisible = false;
            this.tc.Controls.Add(this.tpConfigure);
            this.tc.Controls.Add(this.tpStresstest);
            this.tc.Controls.Add(this.tpReport);
            this.tc.LeftVisible = false;
            this.tc.Location = new System.Drawing.Point(0, 43);
            this.tc.Name = "tc";
            this.tc.RightVisible = false;
            this.tc.SelectedIndex = 0;
            this.tc.Size = new System.Drawing.Size(884, 618);
            this.tc.TabIndex = 1;
            this.tc.TopVisible = true;
            // 
            // tpConfigure
            // 
            this.tpConfigure.BackColor = System.Drawing.Color.White;
            this.tpConfigure.Controls.Add(this.splitConfigure);
            this.tpConfigure.Location = new System.Drawing.Point(0, 22);
            this.tpConfigure.Name = "tpConfigure";
            this.tpConfigure.Padding = new System.Windows.Forms.Padding(3);
            this.tpConfigure.Size = new System.Drawing.Size(883, 595);
            this.tpConfigure.TabIndex = 0;
            this.tpConfigure.Text = "Configure";
            // 
            // splitConfigure
            // 
            this.splitConfigure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitConfigure.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitConfigure.Location = new System.Drawing.Point(3, 3);
            this.splitConfigure.Name = "splitConfigure";
            // 
            // splitConfigure.Panel1
            // 
            this.splitConfigure.Panel1.Controls.Add(this.btnRemove);
            this.splitConfigure.Panel1.Controls.Add(this.tvwTiles);
            this.splitConfigure.Panel1.Controls.Add(this.btnAddTile);
            // 
            // splitConfigure.Panel2
            // 
            this.splitConfigure.Panel2.Controls.Add(this.btnResetToDefaults);
            this.splitConfigure.Panel2.Controls.Add(this.propertiesSolutionComponentPropertyPanel);
            this.splitConfigure.Size = new System.Drawing.Size(877, 589);
            this.splitConfigure.SplitterDistance = 300;
            this.splitConfigure.TabIndex = 1;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.BackColor = System.Drawing.SystemColors.Control;
            this.btnRemove.Enabled = false;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.Location = new System.Drawing.Point(225, 562);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // tvwTiles
            // 
            this.tvwTiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwTiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvwTiles.CheckBoxes = true;
            this.tvwTiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvwTiles.HideSelection = false;
            this.tvwTiles.Location = new System.Drawing.Point(5, 3);
            this.tvwTiles.Name = "tvwTiles";
            this.tvwTiles.ShowLines = false;
            this.tvwTiles.Size = new System.Drawing.Size(295, 553);
            this.tvwTiles.TabIndex = 0;
            this.tvwTiles.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvwTiles_BeforeCheck);
            this.tvwTiles.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvwTiles_AfterCheck);
            this.tvwTiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwTiles_AfterSelect);
            // 
            // btnAddTile
            // 
            this.btnAddTile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddTile.BackColor = System.Drawing.SystemColors.Control;
            this.btnAddTile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddTile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddTile.Location = new System.Drawing.Point(5, 562);
            this.btnAddTile.Name = "btnAddTile";
            this.btnAddTile.Size = new System.Drawing.Size(218, 23);
            this.btnAddTile.TabIndex = 1;
            this.btnAddTile.Text = "Add Tile";
            this.btnAddTile.UseVisualStyleBackColor = false;
            this.btnAddTile.Click += new System.EventHandler(this.btnAddTile_Click);
            // 
            // btnResetToDefaults
            // 
            this.btnResetToDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetToDefaults.BackColor = System.Drawing.SystemColors.Control;
            this.btnResetToDefaults.Enabled = false;
            this.btnResetToDefaults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetToDefaults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetToDefaults.Location = new System.Drawing.Point(3, 562);
            this.btnResetToDefaults.Name = "btnResetToDefaults";
            this.btnResetToDefaults.Size = new System.Drawing.Size(564, 23);
            this.btnResetToDefaults.TabIndex = 1;
            this.btnResetToDefaults.Text = "Reset to Defaults";
            this.btnResetToDefaults.UseVisualStyleBackColor = false;
            this.btnResetToDefaults.Click += new System.EventHandler(this.btnResetToDefaults_Click);
            // 
            // propertiesSolutionComponentPropertyPanel
            // 
            this.propertiesSolutionComponentPropertyPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesSolutionComponentPropertyPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.propertiesSolutionComponentPropertyPanel.Location = new System.Drawing.Point(3, 3);
            this.propertiesSolutionComponentPropertyPanel.Name = "propertiesSolutionComponentPropertyPanel";
            this.propertiesSolutionComponentPropertyPanel.Size = new System.Drawing.Size(564, 553);
            this.propertiesSolutionComponentPropertyPanel.SolutionComponent = null;
            this.propertiesSolutionComponentPropertyPanel.TabIndex = 0;
            // 
            // tpStresstest
            // 
            this.tpStresstest.BackColor = System.Drawing.Color.White;
            this.tpStresstest.Controls.Add(this.splitStresstest);
            this.tpStresstest.Location = new System.Drawing.Point(0, 19);
            this.tpStresstest.Name = "tpStresstest";
            this.tpStresstest.Size = new System.Drawing.Size(199, 80);
            this.tpStresstest.TabIndex = 1;
            this.tpStresstest.Text = "Stresstest";
            // 
            // splitStresstest
            // 
            this.splitStresstest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitStresstest.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitStresstest.Location = new System.Drawing.Point(0, 0);
            this.splitStresstest.Name = "splitStresstest";
            this.splitStresstest.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitStresstest.Panel1
            // 
            this.splitStresstest.Panel1.Controls.Add(this.pnlDistributedTestStresstest);
            // 
            // splitStresstest.Panel2
            // 
            this.splitStresstest.Panel2.Controls.Add(this.distributedStresstestControl);
            this.splitStresstest.Size = new System.Drawing.Size(199, 80);
            this.splitStresstest.SplitterDistance = 51;
            this.splitStresstest.TabIndex = 3;
            // 
            // pnlDistributedTestStresstest
            // 
            this.pnlDistributedTestStresstest.BackColor = System.Drawing.Color.White;
            this.pnlDistributedTestStresstest.Controls.Add(this.btnCollapseExpandStresstest);
            this.pnlDistributedTestStresstest.Controls.Add(this.flpStresstestTiles);
            this.pnlDistributedTestStresstest.Controls.Add(this.flpStresstestTileStresstests);
            this.pnlDistributedTestStresstest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDistributedTestStresstest.Location = new System.Drawing.Point(0, 0);
            this.pnlDistributedTestStresstest.Name = "pnlDistributedTestStresstest";
            this.pnlDistributedTestStresstest.Padding = new System.Windows.Forms.Padding(3);
            this.pnlDistributedTestStresstest.Size = new System.Drawing.Size(199, 51);
            this.pnlDistributedTestStresstest.TabIndex = 0;
            // 
            // btnCollapseExpandStresstest
            // 
            this.btnCollapseExpandStresstest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseExpandStresstest.BackColor = System.Drawing.Color.White;
            this.btnCollapseExpandStresstest.FlatAppearance.BorderSize = 0;
            this.btnCollapseExpandStresstest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollapseExpandStresstest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpandStresstest.Location = new System.Drawing.Point(173, 24);
            this.btnCollapseExpandStresstest.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnCollapseExpandStresstest.Name = "btnCollapseExpandStresstest";
            this.btnCollapseExpandStresstest.Size = new System.Drawing.Size(20, 21);
            this.btnCollapseExpandStresstest.TabIndex = 2;
            this.btnCollapseExpandStresstest.TabStop = false;
            this.btnCollapseExpandStresstest.Text = "+";
            this.btnCollapseExpandStresstest.UseVisualStyleBackColor = false;
            this.btnCollapseExpandStresstest.Click += new System.EventHandler(this.btnCollapseExpandStresstest_Click);
            // 
            // flpStresstestTiles
            // 
            this.flpStresstestTiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpStresstestTiles.BackColor = System.Drawing.Color.WhiteSmoke;
            this.flpStresstestTiles.Location = new System.Drawing.Point(3, 22);
            this.flpStresstestTiles.Name = "flpStresstestTiles";
            this.flpStresstestTiles.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.flpStresstestTiles.Size = new System.Drawing.Size(193, 26);
            this.flpStresstestTiles.TabIndex = 1;
            // 
            // flpStresstestTileStresstests
            // 
            this.flpStresstestTileStresstests.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpStresstestTileStresstests.AutoScroll = true;
            this.flpStresstestTileStresstests.Location = new System.Drawing.Point(3, 3);
            this.flpStresstestTileStresstests.Name = "flpStresstestTileStresstests";
            this.flpStresstestTileStresstests.Padding = new System.Windows.Forms.Padding(3);
            this.flpStresstestTileStresstests.Size = new System.Drawing.Size(193, 13);
            this.flpStresstestTileStresstests.TabIndex = 0;
            // 
            // distributedStresstestControl
            // 
            this.distributedStresstestControl.DistributedTest = null;
            this.distributedStresstestControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.distributedStresstestControl.Location = new System.Drawing.Point(0, 0);
            this.distributedStresstestControl.Margin = new System.Windows.Forms.Padding(0);
            this.distributedStresstestControl.Name = "distributedStresstestControl";
            this.distributedStresstestControl.Size = new System.Drawing.Size(199, 25);
            this.distributedStresstestControl.TabIndex = 0;
            this.distributedStresstestControl.DrillDownChanged += new System.EventHandler(this.distributedStresstestControl_DrillDownChanged);
            // 
            // tpReport
            // 
            this.tpReport.BackColor = System.Drawing.Color.White;
            this.tpReport.Controls.Add(this.splitReport);
            this.tpReport.Location = new System.Drawing.Point(0, 19);
            this.tpReport.Name = "tpReport";
            this.tpReport.Size = new System.Drawing.Size(199, 80);
            this.tpReport.TabIndex = 2;
            this.tpReport.Text = "Report";
            // 
            // splitReport
            // 
            this.splitReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitReport.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitReport.Location = new System.Drawing.Point(0, 0);
            this.splitReport.Name = "splitReport";
            this.splitReport.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitReport.Panel1
            // 
            this.splitReport.Panel1.Controls.Add(this.pnlDistributedTestReport);
            // 
            // splitReport.Panel2
            // 
            this.splitReport.Panel2.Controls.Add(this.tcReport);
            this.splitReport.Size = new System.Drawing.Size(199, 80);
            this.splitReport.SplitterDistance = 51;
            this.splitReport.TabIndex = 4;
            // 
            // pnlDistributedTestReport
            // 
            this.pnlDistributedTestReport.BackColor = System.Drawing.Color.White;
            this.pnlDistributedTestReport.Controls.Add(this.btnCollapseExpandReport);
            this.pnlDistributedTestReport.Controls.Add(this.flpReportTiles);
            this.pnlDistributedTestReport.Controls.Add(this.flpReportTileStresstests);
            this.pnlDistributedTestReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDistributedTestReport.Location = new System.Drawing.Point(0, 0);
            this.pnlDistributedTestReport.Name = "pnlDistributedTestReport";
            this.pnlDistributedTestReport.Padding = new System.Windows.Forms.Padding(3);
            this.pnlDistributedTestReport.Size = new System.Drawing.Size(199, 51);
            this.pnlDistributedTestReport.TabIndex = 2;
            // 
            // btnCollapseExpandReport
            // 
            this.btnCollapseExpandReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseExpandReport.BackColor = System.Drawing.Color.White;
            this.btnCollapseExpandReport.FlatAppearance.BorderSize = 0;
            this.btnCollapseExpandReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollapseExpandReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpandReport.Location = new System.Drawing.Point(173, 24);
            this.btnCollapseExpandReport.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnCollapseExpandReport.Name = "btnCollapseExpandReport";
            this.btnCollapseExpandReport.Size = new System.Drawing.Size(20, 21);
            this.btnCollapseExpandReport.TabIndex = 3;
            this.btnCollapseExpandReport.TabStop = false;
            this.btnCollapseExpandReport.Text = "+";
            this.btnCollapseExpandReport.UseVisualStyleBackColor = false;
            this.btnCollapseExpandReport.Click += new System.EventHandler(this.btnCollapseExpandReport_Click);
            // 
            // flpReportTiles
            // 
            this.flpReportTiles.BackColor = System.Drawing.Color.WhiteSmoke;
            this.flpReportTiles.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flpReportTiles.Location = new System.Drawing.Point(3, 22);
            this.flpReportTiles.Name = "flpReportTiles";
            this.flpReportTiles.Size = new System.Drawing.Size(193, 26);
            this.flpReportTiles.TabIndex = 1;
            // 
            // flpReportTileStresstests
            // 
            this.flpReportTileStresstests.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpReportTileStresstests.AutoScroll = true;
            this.flpReportTileStresstests.Location = new System.Drawing.Point(3, 3);
            this.flpReportTileStresstests.Name = "flpReportTileStresstests";
            this.flpReportTileStresstests.Padding = new System.Windows.Forms.Padding(3);
            this.flpReportTileStresstests.Size = new System.Drawing.Size(193, 13);
            this.flpReportTileStresstests.TabIndex = 0;
            // 
            // tcReport
            // 
            this.tcReport.BottomVisible = false;
            this.tcReport.Controls.Add(this.tpStresstestReport);
            this.tcReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcReport.LeftVisible = false;
            this.tcReport.Location = new System.Drawing.Point(0, 0);
            this.tcReport.Name = "tcReport";
            this.tcReport.RightVisible = false;
            this.tcReport.SelectedIndex = 0;
            this.tcReport.Size = new System.Drawing.Size(199, 25);
            this.tcReport.TabIndex = 5;
            this.tcReport.TopVisible = false;
            // 
            // tpStresstestReport
            // 
            this.tpStresstestReport.Controls.Add(this.btnOpenRFilesFolder);
            this.tpStresstestReport.Controls.Add(this.stresstestReportControl);
            this.tpStresstestReport.Location = new System.Drawing.Point(0, 19);
            this.tpStresstestReport.Name = "tpStresstestReport";
            this.tpStresstestReport.Padding = new System.Windows.Forms.Padding(3);
            this.tpStresstestReport.Size = new System.Drawing.Size(198, 5);
            this.tpStresstestReport.TabIndex = 0;
            this.tpStresstestReport.Text = "Stresstest Report";
            this.tpStresstestReport.UseVisualStyleBackColor = true;
            // 
            // btnOpenRFilesFolder
            // 
            this.btnOpenRFilesFolder.AutoSize = true;
            this.btnOpenRFilesFolder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOpenRFilesFolder.BackColor = System.Drawing.SystemColors.Control;
            this.btnOpenRFilesFolder.Enabled = false;
            this.btnOpenRFilesFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenRFilesFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenRFilesFolder.Location = new System.Drawing.Point(331, 251);
            this.btnOpenRFilesFolder.MaximumSize = new System.Drawing.Size(168, 24);
            this.btnOpenRFilesFolder.Name = "btnOpenRFilesFolder";
            this.btnOpenRFilesFolder.Size = new System.Drawing.Size(162, 24);
            this.btnOpenRFilesFolder.TabIndex = 4;
            this.btnOpenRFilesFolder.Text = "Open the R-files Folder...";
            this.btnOpenRFilesFolder.UseVisualStyleBackColor = false;
            this.btnOpenRFilesFolder.Click += new System.EventHandler(this.btnOpenRFilesFolder_Click);
            // 
            // stresstestReportControl
            // 
            this.stresstestReportControl.CanSaveRFile = false;
            this.stresstestReportControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stresstestReportControl.Location = new System.Drawing.Point(3, 3);
            this.stresstestReportControl.Name = "stresstestReportControl";
            this.stresstestReportControl.Size = new System.Drawing.Size(192, 0);
            this.stresstestReportControl.TabIndex = 3;
            // 
            // tmrSchedule
            // 
            this.tmrSchedule.Tick += new System.EventHandler(this.tmrSchedule_Tick);
            // 
            // fbd
            // 
            this.fbd.Description = "Select where you want to save the result files (R-files) after the test completed" +
    ". This is needed for the result fetching and displaying to work.";
            // 
            // tmrProgressDelayCountDown
            // 
            this.tmrProgressDelayCountDown.Interval = 1000;
            this.tmrProgressDelayCountDown.Tick += new System.EventHandler(this.tmrProgressDelayCountDown_Tick);
            // 
            // tmrProgress
            // 
            this.tmrProgress.Tick += new System.EventHandler(this.tmrProgress_Tick);
            // 
            // tmrCheckTvwTiles
            // 
            this.tmrCheckTvwTiles.Interval = 200;
            this.tmrCheckTvwTiles.Tick += new System.EventHandler(this.tmrCheckTvwTiles_Tick);
            // 
            // DistributedTestView
            // 
            this.ClientSize = new System.Drawing.Size(884, 662);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.tc);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DistributedTestView";
            this.Text = "DistributedTestView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DistributedTestView_FormClosing);
            this.Shown += new System.EventHandler(this.DistributedTestView_Shown);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.tc.ResumeLayout(false);
            this.tpConfigure.ResumeLayout(false);
            this.splitConfigure.Panel1.ResumeLayout(false);
            this.splitConfigure.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitConfigure)).EndInit();
            this.splitConfigure.ResumeLayout(false);
            this.tpStresstest.ResumeLayout(false);
            this.splitStresstest.Panel1.ResumeLayout(false);
            this.splitStresstest.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitStresstest)).EndInit();
            this.splitStresstest.ResumeLayout(false);
            this.pnlDistributedTestStresstest.ResumeLayout(false);
            this.tpReport.ResumeLayout(false);
            this.splitReport.Panel1.ResumeLayout(false);
            this.splitReport.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitReport)).EndInit();
            this.splitReport.ResumeLayout(false);
            this.pnlDistributedTestReport.ResumeLayout(false);
            this.tcReport.ResumeLayout(false);
            this.tpStresstestReport.ResumeLayout(false);
            this.tpStresstestReport.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.ToolStripButton btnStop;
        private vApus.Util.TabControlWithAdjustableBorders tc;
        private System.Windows.Forms.TabPage tpConfigure;
        private System.Windows.Forms.SplitContainer splitConfigure;
        private System.Windows.Forms.Button btnAddTile;
        private SolutionTree.SolutionComponentPropertyPanel propertiesSolutionComponentPropertyPanel;
        private System.Windows.Forms.TabPage tpStresstest;
        private System.Windows.Forms.TreeView tvwTiles;
        private System.Windows.Forms.Button btnResetToDefaults;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Panel pnlDistributedTestStresstest;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cboRunSynchronization;
        private System.Windows.Forms.ToolStripButton btnSchedule;
        private System.Windows.Forms.Timer tmrSchedule;
        private System.Windows.Forms.FolderBrowserDialog fbd;
        private System.Windows.Forms.TabPage tpReport;
        private System.Windows.Forms.FlowLayoutPanel flpStresstestTileStresstests;
        private System.Windows.Forms.Panel pnlDistributedTestReport;
        private System.Windows.Forms.FlowLayoutPanel flpReportTiles;
        private System.Windows.Forms.FlowLayoutPanel flpReportTileStresstests;
        private Stresstest.StresstestReportControl stresstestReportControl;
        private System.Windows.Forms.ToolStripButton btnLinkageOverview;
        private System.Windows.Forms.SplitContainer splitReport;
        private System.Windows.Forms.SplitContainer splitStresstest;
        private System.Windows.Forms.Button btnOpenRFilesFolder;
        private DistributedStresstestControl distributedStresstestControl;
        private System.Windows.Forms.Timer tmrProgressDelayCountDown;
        private System.Windows.Forms.Timer tmrProgress;
        private System.Windows.Forms.Button btnCollapseExpandStresstest;
        private System.Windows.Forms.Button btnCollapseExpandReport;
        private Util.TabControlWithAdjustableBorders tcReport;
        private System.Windows.Forms.TabPage tpStresstestReport;
        private System.Windows.Forms.FlowLayoutPanel flpStresstestTiles;
        private System.Windows.Forms.Timer tmrCheckTvwTiles;
    }
}