namespace vApus.DistributedTesting
{
    partial class NewDistributedTestView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewDistributedTestView));
            this.tpTree = new vApus.Util.TabControlWithAdjustableBorders();
            this.tpTests = new System.Windows.Forms.TabPage();
            this.tpSlaves = new System.Windows.Forms.TabPage();
            this.split = new System.Windows.Forms.SplitContainer();
            this.tabControlWithAdjustableBorders1 = new vApus.Util.TabControlWithAdjustableBorders();
            this.tpConfigureTest = new System.Windows.Forms.TabPage();
            this.tpStresstest = new System.Windows.Forms.TabPage();
            this.stresstestControl1 = new vApus.Stresstest.StresstestControl();
            this.tpReport = new System.Windows.Forms.TabPage();
            this.stresstestReportControl1 = new vApus.Stresstest.StresstestReportControl();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnSchedule = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.testTreeView = new vApus.DistributedTesting.TestTreeView();
            this.configureTileStresstest = new vApus.DistributedTesting.ConfigureTileStresstest();
            this.tpTree.SuspendLayout();
            this.tpTests.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.tabControlWithAdjustableBorders1.SuspendLayout();
            this.tpConfigureTest.SuspendLayout();
            this.tpStresstest.SuspendLayout();
            this.tpReport.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tpTree
            // 
            this.tpTree.BottomVisible = false;
            this.tpTree.Controls.Add(this.tpTests);
            this.tpTree.Controls.Add(this.tpSlaves);
            this.tpTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpTree.LeftVisible = false;
            this.tpTree.Location = new System.Drawing.Point(0, 0);
            this.tpTree.Margin = new System.Windows.Forms.Padding(0);
            this.tpTree.Name = "tpTree";
            this.tpTree.RightVisible = false;
            this.tpTree.SelectedIndex = 0;
            this.tpTree.Size = new System.Drawing.Size(250, 573);
            this.tpTree.TabIndex = 0;
            this.tpTree.TopVisible = true;
            // 
            // tpTests
            // 
            this.tpTests.Controls.Add(this.testTreeView);
            this.tpTests.Location = new System.Drawing.Point(0, 22);
            this.tpTests.Name = "tpTests";
            this.tpTests.Padding = new System.Windows.Forms.Padding(3);
            this.tpTests.Size = new System.Drawing.Size(246, 550);
            this.tpTests.TabIndex = 0;
            this.tpTests.Text = "Tests (#0/0)";
            this.tpTests.UseVisualStyleBackColor = true;
            // 
            // tpSlaves
            // 
            this.tpSlaves.Location = new System.Drawing.Point(0, 22);
            this.tpSlaves.Name = "tpSlaves";
            this.tpSlaves.Padding = new System.Windows.Forms.Padding(3);
            this.tpSlaves.Size = new System.Drawing.Size(246, 550);
            this.tpSlaves.TabIndex = 1;
            this.tpSlaves.Text = "Slaves (#0/0)";
            this.tpSlaves.UseVisualStyleBackColor = true;
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
            this.split.Panel1.Controls.Add(this.tpTree);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.tabControlWithAdjustableBorders1);
            this.split.Size = new System.Drawing.Size(756, 573);
            this.split.SplitterDistance = 250;
            this.split.SplitterWidth = 1;
            this.split.TabIndex = 1;
            // 
            // tabControlWithAdjustableBorders1
            // 
            this.tabControlWithAdjustableBorders1.BottomVisible = false;
            this.tabControlWithAdjustableBorders1.Controls.Add(this.tpConfigureTest);
            this.tabControlWithAdjustableBorders1.Controls.Add(this.tpStresstest);
            this.tabControlWithAdjustableBorders1.Controls.Add(this.tpReport);
            this.tabControlWithAdjustableBorders1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlWithAdjustableBorders1.LeftVisible = true;
            this.tabControlWithAdjustableBorders1.Location = new System.Drawing.Point(0, 0);
            this.tabControlWithAdjustableBorders1.Name = "tabControlWithAdjustableBorders1";
            this.tabControlWithAdjustableBorders1.RightVisible = false;
            this.tabControlWithAdjustableBorders1.SelectedIndex = 0;
            this.tabControlWithAdjustableBorders1.Size = new System.Drawing.Size(505, 573);
            this.tabControlWithAdjustableBorders1.TabIndex = 0;
            this.tabControlWithAdjustableBorders1.TopVisible = true;
            // 
            // tpConfigureTest
            // 
            this.tpConfigureTest.Controls.Add(this.configureTileStresstest);
            this.tpConfigureTest.Location = new System.Drawing.Point(0, 22);
            this.tpConfigureTest.Name = "tpConfigureTest";
            this.tpConfigureTest.Padding = new System.Windows.Forms.Padding(3);
            this.tpConfigureTest.Size = new System.Drawing.Size(504, 550);
            this.tpConfigureTest.TabIndex = 0;
            this.tpConfigureTest.Text = "Configure";
            this.tpConfigureTest.UseVisualStyleBackColor = true;
            // 
            // tpStresstest
            // 
            this.tpStresstest.Controls.Add(this.stresstestControl1);
            this.tpStresstest.Location = new System.Drawing.Point(0, 22);
            this.tpStresstest.Name = "tpStresstest";
            this.tpStresstest.Padding = new System.Windows.Forms.Padding(3);
            this.tpStresstest.Size = new System.Drawing.Size(504, 550);
            this.tpStresstest.TabIndex = 1;
            this.tpStresstest.Text = "Stresstest";
            this.tpStresstest.UseVisualStyleBackColor = true;
            // 
            // stresstestControl1
            // 
            this.stresstestControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stresstestControl1.Location = new System.Drawing.Point(3, 3);
            this.stresstestControl1.MonitorConfigurationControlVisible = false;
            this.stresstestControl1.Name = "stresstestControl1";
            this.stresstestControl1.Size = new System.Drawing.Size(498, 544);
            this.stresstestControl1.TabIndex = 0;
            // 
            // tpReport
            // 
            this.tpReport.Controls.Add(this.stresstestReportControl1);
            this.tpReport.Location = new System.Drawing.Point(0, 22);
            this.tpReport.Name = "tpReport";
            this.tpReport.Padding = new System.Windows.Forms.Padding(3);
            this.tpReport.Size = new System.Drawing.Size(504, 550);
            this.tpReport.TabIndex = 2;
            this.tpReport.Text = "Report";
            this.tpReport.UseVisualStyleBackColor = true;
            // 
            // stresstestReportControl1
            // 
            this.stresstestReportControl1.CanSaveRFile = true;
            this.stresstestReportControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stresstestReportControl1.Location = new System.Drawing.Point(3, 3);
            this.stresstestReportControl1.Name = "stresstestReportControl1";
            this.stresstestReportControl1.Size = new System.Drawing.Size(498, 544);
            this.stresstestReportControl1.TabIndex = 0;
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStart,
            this.btnSchedule,
            this.btnStop});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.MinimumSize = new System.Drawing.Size(0, 40);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(756, 40);
            this.toolStrip.TabIndex = 2;
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
            // 
            // testTreeView
            // 
            this.testTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testTreeView.Location = new System.Drawing.Point(3, 3);
            this.testTreeView.Name = "testTreeView";
            this.testTreeView.Size = new System.Drawing.Size(240, 544);
            this.testTreeView.TabIndex = 0;
            this.testTreeView.AfterSelect += new System.EventHandler(this.testTreeView_AfterSelect);
            // 
            // configureTileStresstest
            // 
            this.configureTileStresstest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configureTileStresstest.Location = new System.Drawing.Point(3, 3);
            this.configureTileStresstest.Name = "configureTileStresstest";
            this.configureTileStresstest.Size = new System.Drawing.Size(498, 544);
            this.configureTileStresstest.TabIndex = 0;
            // 
            // NewDistributedTestView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 616);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.split);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "NewDistributedTestView";
            this.Text = "DistributedTestView";
            this.tpTree.ResumeLayout(false);
            this.tpTests.ResumeLayout(false);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.tabControlWithAdjustableBorders1.ResumeLayout(false);
            this.tpConfigureTest.ResumeLayout(false);
            this.tpStresstest.ResumeLayout(false);
            this.tpReport.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Util.TabControlWithAdjustableBorders tpTree;
        private System.Windows.Forms.TabPage tpTests;
        private System.Windows.Forms.TabPage tpSlaves;
        private TestTreeView testTreeView;
        private System.Windows.Forms.SplitContainer split;
        private Util.TabControlWithAdjustableBorders tabControlWithAdjustableBorders1;
        private System.Windows.Forms.TabPage tpConfigureTest;
        private System.Windows.Forms.TabPage tpStresstest;
        private System.Windows.Forms.TabPage tpReport;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.ToolStripButton btnSchedule;
        private System.Windows.Forms.ToolStripButton btnStop;
        private Stresstest.StresstestControl stresstestControl1;
        private Stresstest.StresstestReportControl stresstestReportControl1;
        private ConfigureTileStresstest configureTileStresstest;
    }
}