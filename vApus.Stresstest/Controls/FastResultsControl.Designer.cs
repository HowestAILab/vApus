namespace vApus.StressTest
{
    partial class FastResultsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.splitTop = new System.Windows.Forms.SplitContainer();
            this.pnlBorderCollapse = new System.Windows.Forms.Panel();
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.flpConfiguration = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlScrollConfigTo = new System.Windows.Forms.Panel();
            this.lblConfiguration = new System.Windows.Forms.Label();
            this.kvpStressTest = new vApus.Util.KeyValuePairControl();
            this.kvpConnection = new vApus.Util.KeyValuePairControl();
            this.kvpConnectionProxy = new vApus.Util.KeyValuePairControl();
            this.kvpScenario = new vApus.Util.KeyValuePairControl();
            this.kvpScenarioRuleSet = new vApus.Util.KeyValuePairControl();
            this.kvpMonitor = new vApus.Util.KeyValuePairControl();
            this.kvpConcurrencies = new vApus.Util.KeyValuePairControl();
            this.kvpRuns = new vApus.Util.KeyValuePairControl();
            this.kvpInitialDelay = new vApus.Util.KeyValuePairControl();
            this.kvpDelay = new vApus.Util.KeyValuePairControl();
            this.kvpShuffle = new vApus.Util.KeyValuePairControl();
            this.kvpActionDistribution = new vApus.Util.KeyValuePairControl();
            this.kvpMaximumNumberOfUserActions = new vApus.Util.KeyValuePairControl();
            this.kvpMonitorBefore = new vApus.Util.KeyValuePairControl();
            this.kvpMonitorAfter = new vApus.Util.KeyValuePairControl();
            this.pnlFastResults = new System.Windows.Forms.Panel();
            this.flpFastResultsHeader = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblUpdatesIn = new System.Windows.Forms.Label();
            this.lbtnStressTest = new vApus.Util.LinkButton();
            this.dgvFastResults = new System.Windows.Forms.DataGridView();
            this.flpFastMetrics = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlBorderDrillDown = new System.Windows.Forms.Panel();
            this.cboDrillDown = new System.Windows.Forms.ComboBox();
            this.lblStarted = new System.Windows.Forms.Label();
            this.lblMeasuredRuntime = new System.Windows.Forms.Label();
            this.btnRerunning = new System.Windows.Forms.Button();
            this.lblStopped = new System.Windows.Forms.Label();
            this.chkReadable = new System.Windows.Forms.CheckBox();
            this.btnSaveDisplayedResults = new System.Windows.Forms.Button();
            this.epnlMessages = new vApus.Util.EventPanel();
            this.flpMetrics = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.kvmThreadsInUse = new vApus.Util.KeyValuePairControl();
            this.kvmCPUUsage = new vApus.Util.KeyValuePairControl();
            this.kvmMemoryUsage = new vApus.Util.KeyValuePairControl();
            this.kvmNic = new vApus.Util.KeyValuePairControl();
            this.kvmNicsSent = new vApus.Util.KeyValuePairControl();
            this.kvmNicsReceived = new vApus.Util.KeyValuePairControl();
            this.btnExport = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTop)).BeginInit();
            this.splitTop.Panel1.SuspendLayout();
            this.splitTop.Panel2.SuspendLayout();
            this.splitTop.SuspendLayout();
            this.pnlBorderCollapse.SuspendLayout();
            this.flpConfiguration.SuspendLayout();
            this.pnlFastResults.SuspendLayout();
            this.flpFastResultsHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFastResults)).BeginInit();
            this.flpFastMetrics.SuspendLayout();
            this.pnlBorderDrillDown.SuspendLayout();
            this.flpMetrics.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.splitTop);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.epnlMessages);
            this.splitContainer.Panel2.Controls.Add(this.flpMetrics);
            this.splitContainer.Panel2MinSize = 63;
            this.splitContainer.Size = new System.Drawing.Size(897, 639);
            this.splitContainer.SplitterDistance = 448;
            this.splitContainer.TabIndex = 2;
            // 
            // splitTop
            // 
            this.splitTop.BackColor = System.Drawing.SystemColors.Control;
            this.splitTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitTop.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitTop.Location = new System.Drawing.Point(0, 0);
            this.splitTop.Name = "splitTop";
            this.splitTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitTop.Panel1
            // 
            this.splitTop.Panel1.BackColor = System.Drawing.Color.White;
            this.splitTop.Panel1.Controls.Add(this.pnlBorderCollapse);
            this.splitTop.Panel1.Controls.Add(this.flpConfiguration);
            this.splitTop.Panel1MinSize = 38;
            // 
            // splitTop.Panel2
            // 
            this.splitTop.Panel2.BackColor = System.Drawing.Color.White;
            this.splitTop.Panel2.Controls.Add(this.pnlFastResults);
            this.splitTop.Size = new System.Drawing.Size(897, 448);
            this.splitTop.SplitterDistance = 85;
            this.splitTop.TabIndex = 1;
            this.splitTop.Text = "[Put title here]";
            // 
            // pnlBorderCollapse
            // 
            this.pnlBorderCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBorderCollapse.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderCollapse.Controls.Add(this.btnCollapseExpand);
            this.pnlBorderCollapse.Location = new System.Drawing.Point(872, 6);
            this.pnlBorderCollapse.Name = "pnlBorderCollapse";
            this.pnlBorderCollapse.Size = new System.Drawing.Size(22, 23);
            this.pnlBorderCollapse.TabIndex = 2;
            // 
            // btnCollapseExpand
            // 
            this.btnCollapseExpand.BackColor = System.Drawing.Color.White;
            this.btnCollapseExpand.FlatAppearance.BorderSize = 0;
            this.btnCollapseExpand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollapseExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpand.Location = new System.Drawing.Point(1, 1);
            this.btnCollapseExpand.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnCollapseExpand.Name = "btnCollapseExpand";
            this.btnCollapseExpand.Size = new System.Drawing.Size(20, 21);
            this.btnCollapseExpand.TabIndex = 18;
            this.btnCollapseExpand.TabStop = false;
            this.btnCollapseExpand.Text = "-";
            this.toolTip.SetToolTip(this.btnCollapseExpand, "Collapse or expand.");
            this.btnCollapseExpand.UseVisualStyleBackColor = false;
            this.btnCollapseExpand.Click += new System.EventHandler(this.btnCollapseExpand_Click);
            // 
            // flpConfiguration
            // 
            this.flpConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpConfiguration.AutoScroll = true;
            this.flpConfiguration.BackColor = System.Drawing.Color.White;
            this.flpConfiguration.Controls.Add(this.pnlScrollConfigTo);
            this.flpConfiguration.Controls.Add(this.lblConfiguration);
            this.flpConfiguration.Controls.Add(this.kvpStressTest);
            this.flpConfiguration.Controls.Add(this.kvpConnection);
            this.flpConfiguration.Controls.Add(this.kvpConnectionProxy);
            this.flpConfiguration.Controls.Add(this.kvpScenario);
            this.flpConfiguration.Controls.Add(this.kvpScenarioRuleSet);
            this.flpConfiguration.Controls.Add(this.kvpMonitor);
            this.flpConfiguration.Controls.Add(this.kvpConcurrencies);
            this.flpConfiguration.Controls.Add(this.kvpRuns);
            this.flpConfiguration.Controls.Add(this.kvpInitialDelay);
            this.flpConfiguration.Controls.Add(this.kvpDelay);
            this.flpConfiguration.Controls.Add(this.kvpShuffle);
            this.flpConfiguration.Controls.Add(this.kvpActionDistribution);
            this.flpConfiguration.Controls.Add(this.kvpMaximumNumberOfUserActions);
            this.flpConfiguration.Controls.Add(this.kvpMonitorBefore);
            this.flpConfiguration.Controls.Add(this.kvpMonitorAfter);
            this.flpConfiguration.Location = new System.Drawing.Point(0, 0);
            this.flpConfiguration.Name = "flpConfiguration";
            this.flpConfiguration.Size = new System.Drawing.Size(866, 85);
            this.flpConfiguration.TabIndex = 0;
            // 
            // pnlScrollConfigTo
            // 
            this.pnlScrollConfigTo.Location = new System.Drawing.Point(0, 0);
            this.pnlScrollConfigTo.Margin = new System.Windows.Forms.Padding(0);
            this.pnlScrollConfigTo.Name = "pnlScrollConfigTo";
            this.pnlScrollConfigTo.Size = new System.Drawing.Size(0, 0);
            this.pnlScrollConfigTo.TabIndex = 0;
            // 
            // lblConfiguration
            // 
            this.lblConfiguration.AutoSize = true;
            this.lblConfiguration.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfiguration.ForeColor = System.Drawing.Color.Black;
            this.lblConfiguration.Location = new System.Drawing.Point(3, 6);
            this.lblConfiguration.Margin = new System.Windows.Forms.Padding(3, 6, 6, 3);
            this.lblConfiguration.Name = "lblConfiguration";
            this.lblConfiguration.Size = new System.Drawing.Size(104, 20);
            this.lblConfiguration.TabIndex = 16;
            this.lblConfiguration.Text = "Configuration";
            // 
            // kvpStressTest
            // 
            this.kvpStressTest.BackColor = System.Drawing.Color.LightBlue;
            this.kvpStressTest.Key = "Stress test";
            this.kvpStressTest.Location = new System.Drawing.Point(116, 6);
            this.kvpStressTest.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpStressTest.Name = "kvpStressTest";
            this.kvpStressTest.Size = new System.Drawing.Size(74, 24);
            this.kvpStressTest.TabIndex = 14;
            this.kvpStressTest.TabStop = false;
            this.kvpStressTest.Tooltip = "";
            this.kvpStressTest.Value = "";
            // 
            // kvpConnection
            // 
            this.kvpConnection.BackColor = System.Drawing.Color.LightBlue;
            this.kvpConnection.Key = "Connection";
            this.kvpConnection.Location = new System.Drawing.Point(188, 6);
            this.kvpConnection.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpConnection.Name = "kvpConnection";
            this.kvpConnection.Size = new System.Drawing.Size(77, 24);
            this.kvpConnection.TabIndex = 8;
            this.kvpConnection.TabStop = false;
            this.kvpConnection.Tooltip = "The connection to the application to test.";
            this.kvpConnection.Value = "";
            // 
            // kvpConnectionProxy
            // 
            this.kvpConnectionProxy.BackColor = System.Drawing.SystemColors.Control;
            this.kvpConnectionProxy.Key = "Connection proxy";
            this.kvpConnectionProxy.Location = new System.Drawing.Point(268, 6);
            this.kvpConnectionProxy.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpConnectionProxy.Name = "kvpConnectionProxy";
            this.kvpConnectionProxy.Size = new System.Drawing.Size(112, 24);
            this.kvpConnectionProxy.TabIndex = 15;
            this.kvpConnectionProxy.TabStop = false;
            this.kvpConnectionProxy.Tooltip = "This is used in and defines the connection.";
            this.kvpConnectionProxy.Value = "";
            // 
            // kvpScenario
            // 
            this.kvpScenario.BackColor = System.Drawing.Color.LightBlue;
            this.kvpScenario.Key = "Scenario(s)";
            this.kvpScenario.Location = new System.Drawing.Point(383, 6);
            this.kvpScenario.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpScenario.Name = "kvpScenario";
            this.kvpScenario.Size = new System.Drawing.Size(34, 24);
            this.kvpScenario.TabIndex = 8;
            this.kvpScenario.TabStop = false;
            this.kvpScenario.Tooltip = "The scenario(s) used to test the application.";
            this.kvpScenario.Value = "";
            // 
            // kvpScenarioRuleSet
            // 
            this.kvpScenarioRuleSet.BackColor = System.Drawing.SystemColors.Control;
            this.kvpScenarioRuleSet.Key = "Scenario rule set";
            this.kvpScenarioRuleSet.Location = new System.Drawing.Point(420, 6);
            this.kvpScenarioRuleSet.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpScenarioRuleSet.Name = "kvpScenarioRuleSet";
            this.kvpScenarioRuleSet.Size = new System.Drawing.Size(87, 24);
            this.kvpScenarioRuleSet.TabIndex = 17;
            this.kvpScenarioRuleSet.TabStop = false;
            this.kvpScenarioRuleSet.Tooltip = "This is used in and defines the requests.";
            this.kvpScenarioRuleSet.Value = "";
            // 
            // kvpMonitor
            // 
            this.kvpMonitor.BackColor = System.Drawing.Color.LightBlue;
            this.kvpMonitor.Key = "Monitor(s)";
            this.kvpMonitor.Location = new System.Drawing.Point(510, 6);
            this.kvpMonitor.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpMonitor.Name = "kvpMonitor";
            this.kvpMonitor.Size = new System.Drawing.Size(69, 24);
            this.kvpMonitor.TabIndex = 22;
            this.kvpMonitor.TabStop = false;
            this.kvpMonitor.Tooltip = "The monitors used to link stress test results to performance counters.";
            this.kvpMonitor.Value = "";
            // 
            // kvpConcurrencies
            // 
            this.kvpConcurrencies.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpConcurrencies.Key = "Concurrencies";
            this.kvpConcurrencies.Location = new System.Drawing.Point(591, 6);
            this.kvpConcurrencies.Margin = new System.Windows.Forms.Padding(12, 6, 0, 3);
            this.kvpConcurrencies.Name = "kvpConcurrencies";
            this.kvpConcurrencies.Size = new System.Drawing.Size(94, 24);
            this.kvpConcurrencies.TabIndex = 8;
            this.kvpConcurrencies.TabStop = false;
            this.kvpConcurrencies.Tooltip = "The count(s) of the concurrent users generated, the minimum given value equals on" +
    "e.";
            this.kvpConcurrencies.Value = "";
            // 
            // kvpRuns
            // 
            this.kvpRuns.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpRuns.Key = "Runs";
            this.kvpRuns.Location = new System.Drawing.Point(688, 6);
            this.kvpRuns.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpRuns.Name = "kvpRuns";
            this.kvpRuns.Size = new System.Drawing.Size(42, 24);
            this.kvpRuns.TabIndex = 8;
            this.kvpRuns.TabStop = false;
            this.kvpRuns.Tooltip = "A static multiplier of the runtime for each concurrency level. Must be greater th" +
    "an zero.";
            this.kvpRuns.Value = "";
            // 
            // kvpInitialDelay
            // 
            this.kvpInitialDelay.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpInitialDelay.Key = "Initial delay";
            this.kvpInitialDelay.Location = new System.Drawing.Point(733, 6);
            this.kvpInitialDelay.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpInitialDelay.Name = "kvpInitialDelay";
            this.kvpInitialDelay.Size = new System.Drawing.Size(45, 24);
            this.kvpInitialDelay.TabIndex = 9;
            this.kvpInitialDelay.TabStop = false;
            this.kvpInitialDelay.Tooltip = "The minimum delay in milliseconds before the execution of the first requests per user. This is not used in result calculations, but rather to spread the requests at the start of the test.";
            this.kvpDelay.Value = "";
            // 
            // kvpDelay
            // 
            this.kvpDelay.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpDelay.Key = "Delay";
            this.kvpDelay.Location = new System.Drawing.Point(3, 39);
            this.kvpDelay.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpDelay.Name = "kvpDelay";
            this.kvpDelay.Size = new System.Drawing.Size(45, 24);
            this.kvpDelay.TabIndex = 10;
            this.kvpDelay.TabStop = false;
            this.kvpDelay.Tooltip = "The delay in milliseconds between the execution of requests per user.\r\nKeep th" +
    "is zero to have an ASAP test.";
            this.kvpDelay.Value = "";
            // 
            // kvpShuffle
            // 
            this.kvpShuffle.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpShuffle.Key = "Shuffle";
            this.kvpShuffle.Location = new System.Drawing.Point(781, 6);
            this.kvpShuffle.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpShuffle.Name = "kvpShuffle";
            this.kvpShuffle.Size = new System.Drawing.Size(53, 24);
            this.kvpShuffle.TabIndex = 11;
            this.kvpShuffle.TabStop = false;
            this.kvpShuffle.Tooltip = "The user actions will be shuffled for each concurrent user when testing.";
            this.kvpShuffle.Value = "";
            // 
            // kvpActionDistribution
            // 
            this.kvpActionDistribution.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpActionDistribution.Key = "Action distribution";
            this.kvpActionDistribution.Location = new System.Drawing.Point(123, 39);
            this.kvpActionDistribution.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpActionDistribution.Name = "kvpActionDistribution";
            this.kvpActionDistribution.Size = new System.Drawing.Size(117, 24);
            this.kvpActionDistribution.TabIndex = 12;
            this.kvpActionDistribution.TabStop = false;
            this.kvpActionDistribution.Tooltip = "\"When this is used, user actions are executed X times its occurance. You can use " +
    "\'Shuffle\' and \'Maximum Number of User Actions\' in combination with this to defin" +
    "e unique test patterns for each user.";
            this.kvpActionDistribution.Value = "";
            // 
            // kvpMaximumNumberOfUserActions
            // 
            this.kvpMaximumNumberOfUserActions.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpMaximumNumberOfUserActions.Key = "Maximum number of user actions";
            this.kvpMaximumNumberOfUserActions.Location = new System.Drawing.Point(328, 39);
            this.kvpMaximumNumberOfUserActions.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpMaximumNumberOfUserActions.Name = "kvpMaximumNumberOfUserActions";
            this.kvpMaximumNumberOfUserActions.Size = new System.Drawing.Size(202, 24);
            this.kvpMaximumNumberOfUserActions.TabIndex = 13;
            this.kvpMaximumNumberOfUserActions.TabStop = false;
            this.kvpMaximumNumberOfUserActions.Tooltip = "The maximum number of user actions that a test pattern for a user can contain. Pi" +
    "nned actions however are always picked.";
            this.kvpMaximumNumberOfUserActions.Value = "";
            // 
            // kvpMonitorBefore
            // 
            this.kvpMonitorBefore.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpMonitorBefore.Key = "Monitor before";
            this.kvpMonitorBefore.Location = new System.Drawing.Point(427, 39);
            this.kvpMonitorBefore.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpMonitorBefore.Name = "kvpMonitorBefore";
            this.kvpMonitorBefore.Size = new System.Drawing.Size(96, 24);
            this.kvpMonitorBefore.TabIndex = 19;
            this.kvpMonitorBefore.TabStop = false;
            this.kvpMonitorBefore.Tooltip = "Start monitoring before the test starts, expressed in minutes with a max of 60.";
            this.kvpMonitorBefore.Value = "";
            // 
            // kvpMonitorAfter
            // 
            this.kvpMonitorAfter.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpMonitorAfter.Key = "Monitor after";
            this.kvpMonitorAfter.Location = new System.Drawing.Point(529, 39);
            this.kvpMonitorAfter.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpMonitorAfter.Name = "kvpMonitorAfter";
            this.kvpMonitorAfter.Size = new System.Drawing.Size(86, 24);
            this.kvpMonitorAfter.TabIndex = 20;
            this.kvpMonitorAfter.TabStop = false;
            this.kvpMonitorAfter.Tooltip = "Continue monitoring after the test is finished, expressed in minutes with a max o" +
    "f 60.";
            this.kvpMonitorAfter.Value = "";
            // 
            // pnlFastResults
            // 
            this.pnlFastResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFastResults.BackColor = System.Drawing.Color.White;
            this.pnlFastResults.Controls.Add(this.flpFastResultsHeader);
            this.pnlFastResults.Controls.Add(this.dgvFastResults);
            this.pnlFastResults.Controls.Add(this.flpFastMetrics);
            this.pnlFastResults.Location = new System.Drawing.Point(0, 0);
            this.pnlFastResults.Name = "pnlFastResults";
            this.pnlFastResults.Size = new System.Drawing.Size(897, 359);
            this.pnlFastResults.TabIndex = 1;
            this.pnlFastResults.Text = "Fast results";
            // 
            // flpFastResultsHeader
            // 
            this.flpFastResultsHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpFastResultsHeader.AutoScroll = true;
            this.flpFastResultsHeader.Controls.Add(this.label4);
            this.flpFastResultsHeader.Controls.Add(this.lblUpdatesIn);
            this.flpFastResultsHeader.Controls.Add(this.lbtnStressTest);
            this.flpFastResultsHeader.Location = new System.Drawing.Point(0, 0);
            this.flpFastResultsHeader.Margin = new System.Windows.Forms.Padding(0);
            this.flpFastResultsHeader.Name = "flpFastResultsHeader";
            this.flpFastResultsHeader.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.flpFastResultsHeader.Size = new System.Drawing.Size(897, 40);
            this.flpFastResultsHeader.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(3, 9);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 6, 5, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "Fast results";
            // 
            // lblUpdatesIn
            // 
            this.lblUpdatesIn.AutoSize = true;
            this.lblUpdatesIn.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdatesIn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblUpdatesIn.Location = new System.Drawing.Point(107, 10);
            this.lblUpdatesIn.Margin = new System.Windows.Forms.Padding(0, 7, 6, 3);
            this.lblUpdatesIn.Name = "lblUpdatesIn";
            this.lblUpdatesIn.Size = new System.Drawing.Size(0, 18);
            this.lblUpdatesIn.TabIndex = 9999;
            // 
            // lbtnStressTest
            // 
            this.lbtnStressTest.Active = true;
            this.lbtnStressTest.ActiveLinkColor = System.Drawing.Color.Black;
            this.lbtnStressTest.AutoSize = true;
            this.lbtnStressTest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbtnStressTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnStressTest.ForeColor = System.Drawing.Color.Black;
            this.lbtnStressTest.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbtnStressTest.LinkColor = System.Drawing.Color.Black;
            this.lbtnStressTest.Location = new System.Drawing.Point(118, 9);
            this.lbtnStressTest.Margin = new System.Windows.Forms.Padding(5, 6, 0, 3);
            this.lbtnStressTest.Name = "lbtnStressTest";
            this.lbtnStressTest.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnStressTest.RadioButtonBehavior = true;
            this.lbtnStressTest.Size = new System.Drawing.Size(101, 22);
            this.lbtnStressTest.TabIndex = 0;
            this.lbtnStressTest.TabStop = true;
            this.lbtnStressTest.Text = "The stress test";
            this.lbtnStressTest.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnStressTest.Visible = false;
            this.lbtnStressTest.VisitedLinkColor = System.Drawing.Color.Black;
            this.lbtnStressTest.ActiveChanged += new System.EventHandler(this.lbtnStressTest_ActiveChanged);
            // 
            // dgvFastResults
            // 
            this.dgvFastResults.AllowUserToAddRows = false;
            this.dgvFastResults.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvFastResults.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFastResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFastResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvFastResults.BackgroundColor = System.Drawing.Color.White;
            this.dgvFastResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFastResults.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvFastResults.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvFastResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFastResults.EnableHeadersVisualStyles = false;
            this.dgvFastResults.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.dgvFastResults.Location = new System.Drawing.Point(0, 101);
            this.dgvFastResults.Name = "dgvFastResults";
            this.dgvFastResults.ReadOnly = true;
            this.dgvFastResults.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvFastResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgvFastResults.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFastResults.Size = new System.Drawing.Size(897, 258);
            this.dgvFastResults.TabIndex = 2;
            this.dgvFastResults.VirtualMode = true;
            this.dgvFastResults.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvFastResults_CellValueNeeded);
            this.dgvFastResults.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvFastResults_Scroll);
            // 
            // flpFastMetrics
            // 
            this.flpFastMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpFastMetrics.Controls.Add(this.label1);
            this.flpFastMetrics.Controls.Add(this.pnlBorderDrillDown);
            this.flpFastMetrics.Controls.Add(this.lblStarted);
            this.flpFastMetrics.Controls.Add(this.lblMeasuredRuntime);
            this.flpFastMetrics.Controls.Add(this.btnRerunning);
            this.flpFastMetrics.Controls.Add(this.lblStopped);
            this.flpFastMetrics.Controls.Add(this.chkReadable);
            this.flpFastMetrics.Controls.Add(this.btnSaveDisplayedResults);
            this.flpFastMetrics.Location = new System.Drawing.Point(-1, 40);
            this.flpFastMetrics.Name = "flpFastMetrics";
            this.flpFastMetrics.Size = new System.Drawing.Size(899, 60);
            this.flpFastMetrics.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 6, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Drill down to";
            // 
            // pnlBorderDrillDown
            // 
            this.pnlBorderDrillDown.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderDrillDown.Controls.Add(this.cboDrillDown);
            this.pnlBorderDrillDown.Location = new System.Drawing.Point(88, 3);
            this.pnlBorderDrillDown.Name = "pnlBorderDrillDown";
            this.pnlBorderDrillDown.Size = new System.Drawing.Size(127, 23);
            this.pnlBorderDrillDown.TabIndex = 0;
            // 
            // cboDrillDown
            // 
            this.cboDrillDown.BackColor = System.Drawing.Color.White;
            this.cboDrillDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDrillDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboDrillDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDrillDown.FormattingEnabled = true;
            this.cboDrillDown.Items.AddRange(new object[] {
            "Concurrencies",
            "Runs"});
            this.cboDrillDown.Location = new System.Drawing.Point(1, 1);
            this.cboDrillDown.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboDrillDown.Name = "cboDrillDown";
            this.cboDrillDown.Size = new System.Drawing.Size(125, 21);
            this.cboDrillDown.TabIndex = 0;
            this.cboDrillDown.SelectedIndexChanged += new System.EventHandler(this.cboDrillDown_SelectedIndexChanged);
            // 
            // lblStarted
            // 
            this.lblStarted.AutoSize = true;
            this.lblStarted.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStarted.Location = new System.Drawing.Point(218, 6);
            this.lblStarted.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblStarted.Name = "lblStarted";
            this.lblStarted.Size = new System.Drawing.Size(0, 16);
            this.lblStarted.TabIndex = 0;
            // 
            // lblMeasuredRuntime
            // 
            this.lblMeasuredRuntime.AutoSize = true;
            this.lblMeasuredRuntime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMeasuredRuntime.Location = new System.Drawing.Point(218, 6);
            this.lblMeasuredRuntime.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblMeasuredRuntime.Name = "lblMeasuredRuntime";
            this.lblMeasuredRuntime.Size = new System.Drawing.Size(0, 16);
            this.lblMeasuredRuntime.TabIndex = 0;
            // 
            // btnRerunning
            // 
            this.btnRerunning.AutoSize = true;
            this.btnRerunning.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRerunning.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnRerunning.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRerunning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnRerunning.ForeColor = System.Drawing.Color.White;
            this.btnRerunning.Location = new System.Drawing.Point(221, 3);
            this.btnRerunning.MaximumSize = new System.Drawing.Size(89, 24);
            this.btnRerunning.MinimumSize = new System.Drawing.Size(89, 24);
            this.btnRerunning.Name = "btnRerunning";
            this.btnRerunning.Size = new System.Drawing.Size(89, 24);
            this.btnRerunning.TabIndex = 2;
            this.btnRerunning.Text = "Rerunning...";
            this.btnRerunning.UseVisualStyleBackColor = false;
            this.btnRerunning.Visible = false;
            this.btnRerunning.Click += new System.EventHandler(this.btnRerunning_Click);
            // 
            // lblStopped
            // 
            this.lblStopped.AutoSize = true;
            this.lblStopped.BackColor = System.Drawing.SystemColors.Control;
            this.lblStopped.Font = new System.Drawing.Font("Consolas", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStopped.Location = new System.Drawing.Point(319, 3);
            this.lblStopped.Margin = new System.Windows.Forms.Padding(6, 3, 6, 0);
            this.lblStopped.Name = "lblStopped";
            this.lblStopped.Size = new System.Drawing.Size(0, 20);
            this.lblStopped.TabIndex = 0;
            // 
            // chkReadable
            // 
            this.chkReadable.AutoSize = true;
            this.chkReadable.Checked = true;
            this.chkReadable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReadable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkReadable.Location = new System.Drawing.Point(328, 7);
            this.chkReadable.Margin = new System.Windows.Forms.Padding(3, 7, 0, 3);
            this.chkReadable.Name = "chkReadable";
            this.chkReadable.Size = new System.Drawing.Size(69, 17);
            this.chkReadable.TabIndex = 3;
            this.chkReadable.Text = "Readable";
            this.toolTip.SetToolTip(this.chkReadable, "Uncheck this if you want results you can calculate with.");
            this.chkReadable.UseVisualStyleBackColor = true;
            this.chkReadable.CheckedChanged += new System.EventHandler(this.chkReadable_CheckedChanged);
            // 
            // btnSaveDisplayedResults
            // 
            this.btnSaveDisplayedResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveDisplayedResults.AutoSize = true;
            this.btnSaveDisplayedResults.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveDisplayedResults.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveDisplayedResults.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnSaveDisplayedResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveDisplayedResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveDisplayedResults.Location = new System.Drawing.Point(400, 3);
            this.btnSaveDisplayedResults.MaximumSize = new System.Drawing.Size(165, 24);
            this.btnSaveDisplayedResults.Name = "btnSaveDisplayedResults";
            this.btnSaveDisplayedResults.Size = new System.Drawing.Size(165, 24);
            this.btnSaveDisplayedResults.TabIndex = 4;
            this.btnSaveDisplayedResults.Text = "Save displayed results...";
            this.btnSaveDisplayedResults.UseVisualStyleBackColor = false;
            this.btnSaveDisplayedResults.Click += new System.EventHandler(this.btnSaveDisplayedResults_Click);
            // 
            // epnlMessages
            // 
            this.epnlMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.epnlMessages.BackColor = System.Drawing.SystemColors.Control;
            this.epnlMessages.BeginOfTimeFrame = new System.DateTime(((long)(0)));
            this.epnlMessages.Collapsed = false;
            this.epnlMessages.Cursor = System.Windows.Forms.Cursors.Default;
            this.epnlMessages.ExpandOnErrorEvent = true;
            this.epnlMessages.Location = new System.Drawing.Point(0, 61);
            this.epnlMessages.Margin = new System.Windows.Forms.Padding(0);
            this.epnlMessages.Name = "epnlMessages";
            this.epnlMessages.ProgressBarColor = System.Drawing.Color.SteelBlue;
            this.epnlMessages.Size = new System.Drawing.Size(897, 126);
            this.epnlMessages.TabIndex = 3;
            this.epnlMessages.CollapsedChanged += new System.EventHandler(this.epnlMessages_CollapsedChanged);
            // 
            // flpMetrics
            // 
            this.flpMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpMetrics.AutoScroll = true;
            this.flpMetrics.BackColor = System.Drawing.Color.White;
            this.flpMetrics.Controls.Add(this.label2);
            this.flpMetrics.Controls.Add(this.kvmCPUUsage);
            this.flpMetrics.Controls.Add(this.kvmMemoryUsage);
            this.flpMetrics.Controls.Add(this.kvmThreadsInUse);
            this.flpMetrics.Controls.Add(this.kvmNic);
            this.flpMetrics.Controls.Add(this.kvmNicsSent);
            this.flpMetrics.Controls.Add(this.kvmNicsReceived);
            this.flpMetrics.Controls.Add(this.btnExport);
            this.flpMetrics.Location = new System.Drawing.Point(0, 0);
            this.flpMetrics.Margin = new System.Windows.Forms.Padding(0);
            this.flpMetrics.Name = "flpMetrics";
            this.flpMetrics.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.flpMetrics.Size = new System.Drawing.Size(897, 61);
            this.flpMetrics.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(312, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Client monitoring";

            // 
            // kvmCPUUsage
            // 
            this.kvmCPUUsage.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmCPUUsage.Key = "CPU";
            this.kvmCPUUsage.Location = new System.Drawing.Point(439, 9);
            this.kvmCPUUsage.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmCPUUsage.Name = "kvmCPUUsage";
            this.kvmCPUUsage.Size = new System.Drawing.Size(105, 16);
            this.kvmCPUUsage.TabIndex = 8;
            this.kvmCPUUsage.TabStop = false;
            this.kvmCPUUsage.Tooltip = "Try to keep this below 60 % to ensure that the client is not the bottleneck.";
            this.kvmCPUUsage.Value = "N/A";
            // 
            // kvmMemoryUsage
            // 
            this.kvmMemoryUsage.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmMemoryUsage.Key = "Memory";
            this.kvmMemoryUsage.Location = new System.Drawing.Point(708, 9);
            this.kvmMemoryUsage.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmMemoryUsage.Name = "kvmMemoryUsage";
            this.kvmMemoryUsage.Size = new System.Drawing.Size(123, 16);
            this.kvmMemoryUsage.TabIndex = 8;
            this.kvmMemoryUsage.TabStop = false;
            this.kvmMemoryUsage.Tooltip = "Make sure you have sufficient memory to ensure that the client is not the bottlen" +
    "eck.";
            this.kvmMemoryUsage.Value = "N/A";
            // 
            // kvmThreadsInUse
            // 
            this.kvmThreadsInUse.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmThreadsInUse.Key = "Threads";
            this.kvmThreadsInUse.Location = new System.Drawing.Point(324, 9);
            this.kvmThreadsInUse.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmThreadsInUse.Name = "kvmThreadsInUse";
            this.kvmThreadsInUse.Size = new System.Drawing.Size(112, 16);
            this.kvmThreadsInUse.TabIndex = 8;
            this.kvmThreadsInUse.TabStop = false;
            this.kvmThreadsInUse.Tooltip = "The number of threads in use should remain equal to the concurrent users just unt" +
    "ill the end of the test for tests with low delays.";
            this.kvmThreadsInUse.Value = "0";
            // 
            // kvmNic
            // 
            this.kvmNic.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmNic.Key = "NIC";
            this.kvmNic.Location = new System.Drawing.Point(3, 35);
            this.kvmNic.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmNic.Name = "kvmNic";
            this.kvmNic.Size = new System.Drawing.Size(139, 16);
            this.kvmNic.TabIndex = 10;
            this.kvmNic.TabStop = false;
            this.kvmNic.Tooltip = "Make sure that the NIC is not the bottleneck (Most used displayed).";
            this.kvmNic.Value = "N/A";
            // 
            // kvmNicsSent
            // 
            this.kvmNicsSent.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmNicsSent.Key = "Tx";
            this.kvmNicsSent.Location = new System.Drawing.Point(3, 35);
            this.kvmNicsSent.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmNicsSent.Name = "kvmNicsSent";
            this.kvmNicsSent.Size = new System.Drawing.Size(139, 16);
            this.kvmNicsSent.TabIndex = 10;
            this.kvmNicsSent.TabStop = false;
            this.kvmNicsSent.Tooltip = "Make sure that the NIC is not the bottleneck (Most used displayed).";
            this.kvmNicsSent.Value = "N/A";
            // 
            // kvmNicsReceived
            // 
            this.kvmNicsReceived.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmNicsReceived.Key = "Rx";
            this.kvmNicsReceived.Location = new System.Drawing.Point(145, 35);
            this.kvmNicsReceived.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmNicsReceived.Name = "kvmNicsReceived";
            this.kvmNicsReceived.Size = new System.Drawing.Size(167, 16);
            this.kvmNicsReceived.TabIndex = 11;
            this.kvmNicsReceived.TabStop = false;
            this.kvmNicsReceived.Tooltip = "Make sure that the NIC is not the bottleneck (Most used displayed).";
            this.kvmNicsReceived.Value = "N/A";
            // 
            // btnExport
            // 
            this.btnExport.AutoSize = true;
            this.btnExport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnExport.BackColor = System.Drawing.SystemColors.Control;
            this.btnExport.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.Location = new System.Drawing.Point(324, 29);
            this.btnExport.Margin = new System.Windows.Forms.Padding(12, 0, 3, 3);
            this.btnExport.MaximumSize = new System.Drawing.Size(127, 24);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(127, 24);
            this.btnExport.TabIndex = 12;
            this.btnExport.Text = "Export messages...";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // FastResultsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.splitContainer);
            this.Name = "FastResultsControl";
            this.Size = new System.Drawing.Size(897, 639);
            this.SizeChanged += new System.EventHandler(this.FastResultsControl_SizeChanged);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.splitTop.Panel1.ResumeLayout(false);
            this.splitTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitTop)).EndInit();
            this.splitTop.ResumeLayout(false);
            this.pnlBorderCollapse.ResumeLayout(false);
            this.flpConfiguration.ResumeLayout(false);
            this.flpConfiguration.PerformLayout();
            this.pnlFastResults.ResumeLayout(false);
            this.flpFastResultsHeader.ResumeLayout(false);
            this.flpFastResultsHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFastResults)).EndInit();
            this.flpFastMetrics.ResumeLayout(false);
            this.flpFastMetrics.PerformLayout();
            this.pnlBorderDrillDown.ResumeLayout(false);
            this.flpMetrics.ResumeLayout(false);
            this.flpMetrics.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.SplitContainer splitTop;
        private System.Windows.Forms.FlowLayoutPanel flpConfiguration;
        private System.Windows.Forms.Label lblConfiguration;
        private vApus.Util.KeyValuePairControl kvpStressTest;
        private vApus.Util.KeyValuePairControl kvpConnection;
        private vApus.Util.KeyValuePairControl kvpConnectionProxy;
        private vApus.Util.KeyValuePairControl kvpScenario;
        private vApus.Util.KeyValuePairControl kvpConcurrencies;
        private vApus.Util.KeyValuePairControl kvpRuns;
        private vApus.Util.KeyValuePairControl kvpInitialDelay;
        private vApus.Util.KeyValuePairControl kvpDelay;
        private vApus.Util.KeyValuePairControl kvpShuffle;
        private vApus.Util.KeyValuePairControl kvpActionDistribution;
        private System.Windows.Forms.Panel pnlFastResults;
        private System.Windows.Forms.FlowLayoutPanel flpFastMetrics;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDrillDown;
        private System.Windows.Forms.Label lblStarted;
        private System.Windows.Forms.Label lblMeasuredRuntime;
        private System.Windows.Forms.Label lblStopped;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSaveDisplayedResults;
        private System.Windows.Forms.Label lblUpdatesIn;
        private System.Windows.Forms.FlowLayoutPanel flpMetrics;
        private System.Windows.Forms.Label label2;
        private vApus.Util.KeyValuePairControl kvmThreadsInUse;
        private vApus.Util.KeyValuePairControl kvmCPUUsage;
        private vApus.Util.KeyValuePairControl kvmMemoryUsage;
        private vApus.Util.KeyValuePairControl kvmNic;
        private vApus.Util.KeyValuePairControl kvmNicsSent;
        private vApus.Util.KeyValuePairControl kvmNicsReceived;
        private vApus.Util.KeyValuePairControl kvpScenarioRuleSet;
        private Util.EventPanel epnlMessages;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnRerunning;
        private System.Windows.Forms.Panel pnlBorderDrillDown;
        private Util.KeyValuePairControl kvpMonitorBefore;
        private Util.KeyValuePairControl kvpMonitorAfter;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox chkReadable;
        private System.Windows.Forms.DataGridView dgvFastResults;
        private Util.LinkButton lbtnStressTest;
        private System.Windows.Forms.FlowLayoutPanel flpFastResultsHeader;
        private System.Windows.Forms.Panel pnlBorderCollapse;
        private System.Windows.Forms.Button btnCollapseExpand;
        private System.Windows.Forms.Panel pnlScrollConfigTo;
        private Util.KeyValuePairControl kvpMaximumNumberOfUserActions;
        private Util.KeyValuePairControl kvpMonitor;
    }
}
