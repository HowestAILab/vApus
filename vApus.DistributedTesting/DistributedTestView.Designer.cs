namespace vApus.DistributedTest {
    partial class DistributedTestView {
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

            JumpStart.Done -= JumpStart_Done;
            Stop();

            try {
                if (_refreshDetailedResultsTimer != null) {
                    try {
                        _refreshDetailedResultsTimer.Stop();
                        _refreshDetailedResultsTimer.Dispose();
                    } catch { }
                }
                _refreshDetailedResultsTimer = null;
            } catch { }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DistributedTestView));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnSchedule = new System.Windows.Forms.ToolStripButton();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.btnWizard = new System.Windows.Forms.ToolStripButton();
            this.tmrRefreshGui = new System.Windows.Forms.Timer(this.components);
            this.split = new System.Windows.Forms.SplitContainer();
            this.tcTree = new vApus.Util.TabControlWithAdjustableBorders();
            this.tpTests = new System.Windows.Forms.TabPage();
            this.tpSlaves = new System.Windows.Forms.TabPage();
            this.tcTest = new vApus.Util.TabControlWithAdjustableBorders();
            this.tpConfigureTest = new System.Windows.Forms.TabPage();
            this.tpStressTest = new System.Windows.Forms.TabPage();
            this.fastResultsControl = new vApus.StressTest.FastResultsControl();
            this.tpDetailedResults = new System.Windows.Forms.TabPage();
            this.detailedResultsControl = new vApus.StressTest.DetailedResultsControl();
            this.tmrSchedule = new System.Windows.Forms.Timer(this.components);
            this.tmrProgressDelayCountDown = new System.Windows.Forms.Timer(this.components);
            this.tmrProgress = new System.Windows.Forms.Timer(this.components);
            this.testTreeView = new vApus.DistributedTest.TestTreeView();
            this.slaveTreeView = new vApus.DistributedTest.SlaveTreeView();
            this.tileOverview = new vApus.DistributedTest.Controls.TestTreeView.DistributedTestOrTileOverview();
            this.configureTileStressTest = new vApus.DistributedTest.ConfigureTileStressTest();
            this.configureSlaves = new vApus.DistributedTest.ConfigureSlaves();
            this.distributedTestControl = new vApus.DistributedTest.OveralFastResultsControl();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.tcTree.SuspendLayout();
            this.tpTests.SuspendLayout();
            this.tpSlaves.SuspendLayout();
            this.tcTest.SuspendLayout();
            this.tpConfigureTest.SuspendLayout();
            this.tpStressTest.SuspendLayout();
            this.tpDetailedResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSchedule,
            this.btnStart,
            this.btnStop,
            this.btnWizard});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.MinimumSize = new System.Drawing.Size(0, 40);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(911, 40);
            this.toolStrip.TabIndex = 2;
            // 
            // btnSchedule
            // 
            this.btnSchedule.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSchedule.Image = ((System.Drawing.Image)(resources.GetObject("btnSchedule.Image")));
            this.btnSchedule.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSchedule.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSchedule.Name = "btnSchedule";
            this.btnSchedule.Size = new System.Drawing.Size(23, 37);
            this.btnSchedule.Tag = "";
            this.btnSchedule.ToolTipText = "Schedule...";
            this.btnSchedule.Click += new System.EventHandler(this.btnSchedule_Click);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 37);
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(90, 37);
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnWizard
            // 
            this.btnWizard.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnWizard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWizard.Image = ((System.Drawing.Image)(resources.GetObject("btnWizard.Image")));
            this.btnWizard.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnWizard.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnWizard.Name = "btnWizard";
            this.btnWizard.Size = new System.Drawing.Size(83, 37);
            this.btnWizard.Text = "Wizard...";
            this.btnWizard.ToolTipText = "Wizard...";
            this.btnWizard.Click += new System.EventHandler(this.btnWizard_Click);
            // 
            // tmrRefreshGui
            // 
            this.tmrRefreshGui.Enabled = true;
            this.tmrRefreshGui.Tick += new System.EventHandler(this.tmrRefreshGui_Tick);
            // 
            // split
            // 
            this.split.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split.BackColor = System.Drawing.Color.White;
            this.split.Location = new System.Drawing.Point(0, 43);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.tcTree);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.tcTest);
            this.split.Size = new System.Drawing.Size(911, 648);
            this.split.SplitterDistance = 301;
            this.split.TabIndex = 1;
            // 
            // tcTree
            // 
            this.tcTree.BottomVisible = false;
            this.tcTree.Controls.Add(this.tpTests);
            this.tcTree.Controls.Add(this.tpSlaves);
            this.tcTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcTree.LeftVisible = false;
            this.tcTree.Location = new System.Drawing.Point(0, 0);
            this.tcTree.Margin = new System.Windows.Forms.Padding(0);
            this.tcTree.Name = "tcTree";
            this.tcTree.RightVisible = false;
            this.tcTree.SelectedIndex = 0;
            this.tcTree.Size = new System.Drawing.Size(301, 648);
            this.tcTree.TabIndex = 0;
            this.tcTree.TopVisible = true;
            this.tcTree.SelectedIndexChanged += new System.EventHandler(this.tpTree_SelectedIndexChanged);
            // 
            // tpTests
            // 
            this.tpTests.BackColor = System.Drawing.Color.White;
            this.tpTests.Controls.Add(this.testTreeView);
            this.tpTests.Location = new System.Drawing.Point(0, 22);
            this.tpTests.Name = "tpTests";
            this.tpTests.Padding = new System.Windows.Forms.Padding(3);
            this.tpTests.Size = new System.Drawing.Size(300, 625);
            this.tpTests.TabIndex = 0;
            this.tpTests.Text = "Tests (0/0)";
            // 
            // tpSlaves
            // 
            this.tpSlaves.BackColor = System.Drawing.Color.White;
            this.tpSlaves.Controls.Add(this.slaveTreeView);
            this.tpSlaves.Location = new System.Drawing.Point(0, 22);
            this.tpSlaves.Name = "tpSlaves";
            this.tpSlaves.Padding = new System.Windows.Forms.Padding(3);
            this.tpSlaves.Size = new System.Drawing.Size(300, 625);
            this.tpSlaves.TabIndex = 1;
            this.tpSlaves.Text = "Slaves (0/0)";
            // 
            // tcTest
            // 
            this.tcTest.BottomVisible = false;
            this.tcTest.Controls.Add(this.tpConfigureTest);
            this.tcTest.Controls.Add(this.tpStressTest);
            this.tcTest.Controls.Add(this.tpDetailedResults);
            this.tcTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcTest.LeftVisible = true;
            this.tcTest.Location = new System.Drawing.Point(0, 0);
            this.tcTest.Name = "tcTest";
            this.tcTest.RightVisible = false;
            this.tcTest.SelectedIndex = 0;
            this.tcTest.Size = new System.Drawing.Size(606, 648);
            this.tcTest.TabIndex = 0;
            this.tcTest.TopVisible = true;
            // 
            // tpConfigureTest
            // 
            this.tpConfigureTest.BackColor = System.Drawing.Color.White;
            this.tpConfigureTest.Controls.Add(this.configureTileStressTest);
            this.tpConfigureTest.Controls.Add(this.configureSlaves);
            this.tpConfigureTest.Controls.Add(this.tileOverview);
            this.tpConfigureTest.Location = new System.Drawing.Point(4, 22);
            this.tpConfigureTest.Name = "tpConfigureTest";
            this.tpConfigureTest.Padding = new System.Windows.Forms.Padding(3);
            this.tpConfigureTest.Size = new System.Drawing.Size(601, 625);
            this.tpConfigureTest.TabIndex = 0;
            this.tpConfigureTest.Text = "Configure";
            // 
            // tpStressTest
            // 
            this.tpStressTest.BackColor = System.Drawing.Color.White;
            this.tpStressTest.Controls.Add(this.fastResultsControl);
            this.tpStressTest.Controls.Add(this.distributedTestControl);
            this.tpStressTest.Location = new System.Drawing.Point(4, 22);
            this.tpStressTest.Name = "tpStressTest";
            this.tpStressTest.Padding = new System.Windows.Forms.Padding(3);
            this.tpStressTest.Size = new System.Drawing.Size(601, 625);
            this.tpStressTest.TabIndex = 1;
            this.tpStressTest.Text = "Stress test";
            // 
            // fastResultsControl
            // 
            this.fastResultsControl.BackColor = System.Drawing.Color.Transparent;
            this.fastResultsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastResultsControl.Location = new System.Drawing.Point(3, 3);
            this.fastResultsControl.MonitorConfigurationControlAndKeyValuePairControlVisible = true;
            this.fastResultsControl.Name = "fastResultsControl";
            this.fastResultsControl.Size = new System.Drawing.Size(595, 619);
            this.fastResultsControl.TabIndex = 0;
            // 
            // tpDetailedResults
            // 
            this.tpDetailedResults.BackColor = System.Drawing.Color.White;
            this.tpDetailedResults.Controls.Add(this.detailedResultsControl);
            this.tpDetailedResults.Location = new System.Drawing.Point(4, 22);
            this.tpDetailedResults.Name = "tpDetailedResults";
            this.tpDetailedResults.Size = new System.Drawing.Size(601, 625);
            this.tpDetailedResults.TabIndex = 3;
            this.tpDetailedResults.Text = "Detailed results";
            // 
            // detailedResultsControl
            // 
            this.detailedResultsControl.BackColor = System.Drawing.SystemColors.Control;
            this.detailedResultsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailedResultsControl.Enabled = false;
            this.detailedResultsControl.Location = new System.Drawing.Point(0, 0);
            this.detailedResultsControl.Name = "detailedResultsControl";
            this.detailedResultsControl.Size = new System.Drawing.Size(601, 625);
            this.detailedResultsControl.TabIndex = 0;
            // 
            // tmrSchedule
            // 
            this.tmrSchedule.Tick += new System.EventHandler(this.tmrSchedule_Tick);
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
            // testTreeView
            // 
            this.testTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testTreeView.Location = new System.Drawing.Point(3, 3);
            this.testTreeView.Name = "testTreeView";
            this.testTreeView.Size = new System.Drawing.Size(294, 619);
            this.testTreeView.TabIndex = 0;
            this.testTreeView.AfterSelect += new System.EventHandler(this.testTreeView_AfterSelect);
            this.testTreeView.TileStressTestTreeViewItemDoubleClicked += new System.EventHandler(this.testTreeView_TileStressTestTreeViewItemDoubleClicked);
            this.testTreeView.EventClicked += new System.EventHandler<vApus.Util.EventProgressChart.ProgressEventEventArgs>(this.testTreeView_ProgressEventClicked);
            // 
            // slaveTreeView
            // 
            this.slaveTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slaveTreeView.Location = new System.Drawing.Point(3, 3);
            this.slaveTreeView.Name = "slaveTreeView";
            this.slaveTreeView.Size = new System.Drawing.Size(294, 619);
            this.slaveTreeView.TabIndex = 0;
            this.slaveTreeView.AfterSelect += new System.EventHandler(this.slaveTreeView_AfterSelect);
            this.slaveTreeView.ClientTreeViewItemDoubleClicked += new System.EventHandler(this.slaveTreeView_ClientTreeViewItemDoubleClicked);
            // 
            // tileOverview
            // 
            this.tileOverview.BackColor = System.Drawing.Color.White;
            this.tileOverview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileOverview.Location = new System.Drawing.Point(3, 3);
            this.tileOverview.Name = "tileOverview";
            this.tileOverview.Size = new System.Drawing.Size(595, 619);
            this.tileOverview.TabIndex = 2;
            // 
            // configureTileStressTest
            // 
            this.configureTileStressTest.AutoScroll = true;
            this.configureTileStressTest.BackColor = System.Drawing.Color.White;
            this.configureTileStressTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configureTileStressTest.Location = new System.Drawing.Point(3, 3);
            this.configureTileStressTest.Name = "configureTileStressTest";
            this.configureTileStressTest.Size = new System.Drawing.Size(595, 619);
            this.configureTileStressTest.TabIndex = 0;
            // 
            // configureSlaves
            // 
            this.configureSlaves.BackColor = System.Drawing.Color.White;
            this.configureSlaves.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configureSlaves.Location = new System.Drawing.Point(3, 3);
            this.configureSlaves.Name = "configureSlaves";
            this.configureSlaves.Size = new System.Drawing.Size(595, 619);
            this.configureSlaves.TabIndex = 1;
            this.configureSlaves.Visible = false;
            this.configureSlaves.GoToAssignedTest += new System.EventHandler(this.configureSlaves_GoToAssignedTest);
            // 
            // distributedTestControl
            // 
            this.distributedTestControl.BackColor = System.Drawing.SystemColors.Control;
            this.distributedTestControl.DistributedTest = null;
            this.distributedTestControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.distributedTestControl.Location = new System.Drawing.Point(3, 3);
            this.distributedTestControl.Name = "distributedTestControl";
            this.distributedTestControl.Size = new System.Drawing.Size(595, 619);
            this.distributedTestControl.TabIndex = 1;
            // 
            // DistributedTestView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 691);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.split);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DistributedTestView";
            this.Text = "DistributedTestView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DistributedTestView_FormClosing);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.tcTree.ResumeLayout(false);
            this.tpTests.ResumeLayout(false);
            this.tpSlaves.ResumeLayout(false);
            this.tcTest.ResumeLayout(false);
            this.tpConfigureTest.ResumeLayout(false);
            this.tpStressTest.ResumeLayout(false);
            this.tpDetailedResults.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Util.TabControlWithAdjustableBorders tcTree;
        private System.Windows.Forms.TabPage tpTests;
        private System.Windows.Forms.TabPage tpSlaves;
        private TestTreeView testTreeView;
        private System.Windows.Forms.SplitContainer split;
        private Util.TabControlWithAdjustableBorders tcTest;
        private System.Windows.Forms.TabPage tpConfigureTest;
        private System.Windows.Forms.TabPage tpStressTest;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.ToolStripButton btnSchedule;
        private System.Windows.Forms.ToolStripButton btnStop;
        private StressTest.FastResultsControl fastResultsControl;
        private ConfigureTileStressTest configureTileStressTest;
        private SlaveTreeView slaveTreeView;
        private System.Windows.Forms.Timer tmrRefreshGui;
        private vApus.DistributedTest.ConfigureSlaves configureSlaves;
        private OveralFastResultsControl distributedTestControl;
        private System.Windows.Forms.Timer tmrSchedule;
        private System.Windows.Forms.Timer tmrProgressDelayCountDown;
        private System.Windows.Forms.Timer tmrProgress;
        private System.Windows.Forms.ToolStripButton btnWizard;
        private System.Windows.Forms.TabPage tpDetailedResults;
        private StressTest.DetailedResultsControl detailedResultsControl;
        private Controls.TestTreeView.DistributedTestOrTileOverview tileOverview;
    }
}