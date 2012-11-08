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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            JumpStart.Done -= JumpStart_Done;
            Stop(false);
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
            this.btnWizard = new System.Windows.Forms.ToolStripButton();
            this.tmrSetGui = new System.Windows.Forms.Timer(this.components);
            this.split = new System.Windows.Forms.SplitContainer();
            this.tcTree = new vApus.Util.TabControlWithAdjustableBorders();
            this.tpTests = new System.Windows.Forms.TabPage();
            this.testTreeView = new vApus.DistributedTesting.TestTreeView();
            this.tpSlaves = new System.Windows.Forms.TabPage();
            this.slaveTreeView = new vApus.DistributedTesting.SlaveTreeView();
            this.tcTest = new vApus.Util.TabControlWithAdjustableBorders();
            this.tpConfigureTest = new System.Windows.Forms.TabPage();
            this.configureTileStresstest = new vApus.DistributedTesting.ConfigureTileStresstest();
            this.configureSlaves = new vApus.DistributedTesting.ConfigureSlaves();
            this.tpStresstest = new System.Windows.Forms.TabPage();
            this.stresstestControl = new vApus.Stresstest.StresstestControl();
            this.distributedStresstestControl = new vApus.DistributedTesting.DistributedStresstestControl();
            this.tpReport = new System.Windows.Forms.TabPage();
            this.tmrSchedule = new System.Windows.Forms.Timer(this.components);
            this.tmrProgressDelayCountDown = new System.Windows.Forms.Timer(this.components);
            this.tmrProgress = new System.Windows.Forms.Timer(this.components);
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
            this.tpStresstest.SuspendLayout();
            this.tpReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStart,
            this.btnSchedule,
            this.btnStop,
            this.btnWizard});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.MinimumSize = new System.Drawing.Size(0, 40);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(911, 40);
            this.toolStrip.TabIndex = 2;
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
            this.btnWizard.Click += new System.EventHandler(this.btnWizard_Click);
            // 
            // tmrSetGui
            // 
            this.tmrSetGui.Enabled = true;
            this.tmrSetGui.Interval = 1000;
            this.tmrSetGui.Tick += new System.EventHandler(this.tmrSetGui_Tick);
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
            // testTreeView
            // 
            this.testTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testTreeView.Location = new System.Drawing.Point(3, 3);
            this.testTreeView.Name = "testTreeView";
            this.testTreeView.Size = new System.Drawing.Size(294, 619);
            this.testTreeView.TabIndex = 0;
            this.testTreeView.AfterSelect += new System.EventHandler(this.testTreeView_AfterSelect);
            this.testTreeView.EventClicked += new System.EventHandler<vApus.Util.EventProgressChart.ProgressEventEventArgs>(this.testTreeView_EventClicked);
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
            // slaveTreeView
            // 
            this.slaveTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slaveTreeView.Location = new System.Drawing.Point(3, 3);
            this.slaveTreeView.Name = "slaveTreeView";
            this.slaveTreeView.Size = new System.Drawing.Size(294, 619);
            this.slaveTreeView.TabIndex = 0;
            this.slaveTreeView.AfterSelect += new System.EventHandler(this.slaveTreeView_AfterSelect);
            // 
            // tcTest
            // 
            this.tcTest.BottomVisible = false;
            this.tcTest.Controls.Add(this.tpConfigureTest);
            this.tcTest.Controls.Add(this.tpStresstest);
            this.tcTest.Controls.Add(this.tpReport);
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
            this.tpConfigureTest.Controls.Add(this.configureTileStresstest);
            this.tpConfigureTest.Controls.Add(this.configureSlaves);
            this.tpConfigureTest.Location = new System.Drawing.Point(4, 19);
            this.tpConfigureTest.Name = "tpConfigureTest";
            this.tpConfigureTest.Padding = new System.Windows.Forms.Padding(3);
            this.tpConfigureTest.Size = new System.Drawing.Size(601, 628);
            this.tpConfigureTest.TabIndex = 0;
            this.tpConfigureTest.Text = "Configure";
            // 
            // configureTileStresstest
            // 
            this.configureTileStresstest.BackColor = System.Drawing.Color.White;
            this.configureTileStresstest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configureTileStresstest.Location = new System.Drawing.Point(3, 3);
            this.configureTileStresstest.Name = "configureTileStresstest";
            this.configureTileStresstest.Size = new System.Drawing.Size(595, 622);
            this.configureTileStresstest.TabIndex = 0;
            // 
            // configureSlaves
            // 
            this.configureSlaves.BackColor = System.Drawing.Color.White;
            this.configureSlaves.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configureSlaves.Location = new System.Drawing.Point(3, 3);
            this.configureSlaves.Name = "configureSlaves";
            this.configureSlaves.Size = new System.Drawing.Size(595, 622);
            this.configureSlaves.TabIndex = 1;
            this.configureSlaves.Visible = false;
            this.configureSlaves.GoToAssignedTest += new System.EventHandler(this.configureSlaves_GoToAssignedTest);
            // 
            // tpStresstest
            // 
            this.tpStresstest.BackColor = System.Drawing.Color.White;
            this.tpStresstest.Controls.Add(this.stresstestControl);
            this.tpStresstest.Controls.Add(this.distributedStresstestControl);
            this.tpStresstest.Location = new System.Drawing.Point(4, 19);
            this.tpStresstest.Name = "tpStresstest";
            this.tpStresstest.Padding = new System.Windows.Forms.Padding(3);
            this.tpStresstest.Size = new System.Drawing.Size(601, 628);
            this.tpStresstest.TabIndex = 1;
            this.tpStresstest.Text = "Stresstest";
            // 
            // stresstestControl
            // 
            this.stresstestControl.BackColor = System.Drawing.Color.Transparent;
            this.stresstestControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stresstestControl.Location = new System.Drawing.Point(3, 3);
            this.stresstestControl.MonitorConfigurationControlVisible = false;
            this.stresstestControl.Name = "stresstestControl";
            this.stresstestControl.Size = new System.Drawing.Size(595, 622);
            this.stresstestControl.TabIndex = 0;
            // 
            // distributedStresstestControl
            // 
            this.distributedStresstestControl.BackColor = System.Drawing.SystemColors.Control;
            this.distributedStresstestControl.DistributedTest = null;
            this.distributedStresstestControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.distributedStresstestControl.Location = new System.Drawing.Point(3, 3);
            this.distributedStresstestControl.Name = "distributedStresstestControl";
            this.distributedStresstestControl.Size = new System.Drawing.Size(595, 622);
            this.distributedStresstestControl.TabIndex = 1;
            // 
            // tpReport
            // 
            this.tpReport.BackColor = System.Drawing.Color.White;
            this.tpReport.Location = new System.Drawing.Point(4, 19);
            this.tpReport.Name = "tpReport";
            this.tpReport.Padding = new System.Windows.Forms.Padding(3);
            this.tpReport.Size = new System.Drawing.Size(601, 628);
            this.tpReport.TabIndex = 2;
            this.tpReport.Text = "Report";
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
            this.tpStresstest.ResumeLayout(false);
            this.tpReport.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage tpStresstest;
        private System.Windows.Forms.TabPage tpReport;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.ToolStripButton btnSchedule;
        private System.Windows.Forms.ToolStripButton btnStop;
        private Stresstest.StresstestControl stresstestControl;
        private ConfigureTileStresstest configureTileStresstest;
        private SlaveTreeView slaveTreeView;
        private System.Windows.Forms.Timer tmrSetGui;
        private vApus.DistributedTesting.ConfigureSlaves configureSlaves;
        private DistributedStresstestControl distributedStresstestControl;
        private System.Windows.Forms.Timer tmrSchedule;
        private System.Windows.Forms.Timer tmrProgressDelayCountDown;
        private System.Windows.Forms.Timer tmrProgress;
        private System.Windows.Forms.ToolStripButton btnWizard;
    }
}