namespace vApus.Stresstest
{
    partial class StresstestReportControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StresstestReportControl));
            this.split2 = new System.Windows.Forms.SplitContainer();
            this.flpSaveConfiguration = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.kvpSolution = new vApus.Util.KeyValuePairControl();
            this.kvpStresstest = new vApus.Util.KeyValuePairControl();
            this.kvpConnection = new vApus.Util.KeyValuePairControl();
            this.kvpConnectionProxy = new vApus.Util.KeyValuePairControl();
            this.kvpLog = new vApus.Util.KeyValuePairControl();
            this.kvpLogRuleSet = new vApus.Util.KeyValuePairControl();
            this.kvpMonitor = new vApus.Util.KeyValuePairControl();
            this.kvpConcurrentUsers = new vApus.Util.KeyValuePairControl();
            this.kvpPrecision = new vApus.Util.KeyValuePairControl();
            this.kvpDynamicRunMultiplier = new vApus.Util.KeyValuePairControl();
            this.kvpDelay = new vApus.Util.KeyValuePairControl();
            this.kvpShuffle = new vApus.Util.KeyValuePairControl();
            this.kvpDistribute = new vApus.Util.KeyValuePairControl();
            this.panel = new System.Windows.Forms.Panel();
            this.btnSaveTheRFile = new System.Windows.Forms.Button();
            this.btnSaveTheConfigurationAndTheChosenResultSet = new System.Windows.Forms.Button();
            this.flpDetailedResultsListing = new System.Windows.Forms.FlowLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.rdbAverages = new System.Windows.Forms.RadioButton();
            this.lblPivotLevel = new System.Windows.Forms.Label();
            this.pnlBorderPivotLevel = new System.Windows.Forms.Panel();
            this.cboPivotLevel = new System.Windows.Forms.ComboBox();
            this.rdbUsers = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.rdbErrors = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.tbtnConcurrentUsers = new vApus.Util.ToggleButton();
            this.tbtnPrecision = new vApus.Util.ToggleButton();
            this.tbtnRun = new vApus.Util.ToggleButton();
            this.tbtnUser = new vApus.Util.ToggleButton();
            this.tbtnUserAction = new vApus.Util.ToggleButton();
            this.tbtnLogEntry = new vApus.Util.ToggleButton();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dgvDetailedResultsListing = new System.Windows.Forms.DataGridView();
            this.clmDRLStartedAtSentAt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLMeasuredTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLConcurrentUsers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLPrecision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLRun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLUser = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLUserAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLLogEntry = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLLogEntriesProcessed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLThroughput = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLResponseTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLMaxResponseTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLPercentileMaxResponseTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLDelay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDRLErrors = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblWait = new System.Windows.Forms.Label();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.split2)).BeginInit();
            this.split2.Panel1.SuspendLayout();
            this.split2.Panel2.SuspendLayout();
            this.split2.SuspendLayout();
            this.flpSaveConfiguration.SuspendLayout();
            this.panel.SuspendLayout();
            this.flpDetailedResultsListing.SuspendLayout();
            this.pnlBorderPivotLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailedResultsListing)).BeginInit();
            this.SuspendLayout();
            // 
            // split2
            // 
            this.split2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.split2.Location = new System.Drawing.Point(0, 0);
            this.split2.Name = "split2";
            this.split2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split2.Panel1
            // 
            this.split2.Panel1.Controls.Add(this.flpSaveConfiguration);
            // 
            // split2.Panel2
            // 
            this.split2.Panel2.Controls.Add(this.panel);
            this.split2.Size = new System.Drawing.Size(800, 423);
            this.split2.SplitterDistance = 150;
            this.split2.TabIndex = 4;
            // 
            // flpSaveConfiguration
            // 
            this.flpSaveConfiguration.AutoScroll = true;
            this.flpSaveConfiguration.BackColor = System.Drawing.Color.White;
            this.flpSaveConfiguration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpSaveConfiguration.Controls.Add(this.label3);
            this.flpSaveConfiguration.Controls.Add(this.kvpSolution);
            this.flpSaveConfiguration.Controls.Add(this.kvpStresstest);
            this.flpSaveConfiguration.Controls.Add(this.kvpConnection);
            this.flpSaveConfiguration.Controls.Add(this.kvpConnectionProxy);
            this.flpSaveConfiguration.Controls.Add(this.kvpLog);
            this.flpSaveConfiguration.Controls.Add(this.kvpLogRuleSet);
            this.flpSaveConfiguration.Controls.Add(this.kvpMonitor);
            this.flpSaveConfiguration.Controls.Add(this.kvpConcurrentUsers);
            this.flpSaveConfiguration.Controls.Add(this.kvpPrecision);
            this.flpSaveConfiguration.Controls.Add(this.kvpDynamicRunMultiplier);
            this.flpSaveConfiguration.Controls.Add(this.kvpDelay);
            this.flpSaveConfiguration.Controls.Add(this.kvpShuffle);
            this.flpSaveConfiguration.Controls.Add(this.kvpDistribute);
            this.flpSaveConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpSaveConfiguration.Location = new System.Drawing.Point(0, 0);
            this.flpSaveConfiguration.Name = "flpSaveConfiguration";
            this.flpSaveConfiguration.Size = new System.Drawing.Size(800, 150);
            this.flpSaveConfiguration.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(3, 6);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 6, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 20);
            this.label3.TabIndex = 39;
            this.label3.Text = "Configuration";
            // 
            // kvpSolution
            // 
            this.kvpSolution.BackColor = System.Drawing.Color.LightBlue;
            this.kvpSolution.Key = "Solution";
            this.kvpSolution.Location = new System.Drawing.Point(116, 6);
            this.kvpSolution.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpSolution.Name = "kvpSolution";
            this.kvpSolution.Size = new System.Drawing.Size(59, 24);
            this.kvpSolution.TabIndex = 44;
            this.kvpSolution.TabStop = false;
            this.kvpSolution.Tooltip = "";
            this.kvpSolution.Value = "";
            // 
            // kvpStresstest
            // 
            this.kvpStresstest.BackColor = System.Drawing.Color.LightBlue;
            this.kvpStresstest.Key = "Stresstest";
            this.kvpStresstest.Location = new System.Drawing.Point(178, 6);
            this.kvpStresstest.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpStresstest.Name = "kvpStresstest";
            this.kvpStresstest.Size = new System.Drawing.Size(69, 24);
            this.kvpStresstest.TabIndex = 37;
            this.kvpStresstest.TabStop = false;
            this.kvpStresstest.Tooltip = "";
            this.kvpStresstest.Value = "";
            // 
            // kvpConnection
            // 
            this.kvpConnection.BackColor = System.Drawing.Color.LightBlue;
            this.kvpConnection.Key = "Connection";
            this.kvpConnection.Location = new System.Drawing.Point(250, 6);
            this.kvpConnection.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpConnection.Name = "kvpConnection";
            this.kvpConnection.Size = new System.Drawing.Size(77, 24);
            this.kvpConnection.TabIndex = 31;
            this.kvpConnection.TabStop = false;
            this.kvpConnection.Tooltip = "The connection to the application to test.";
            this.kvpConnection.Value = "";
            // 
            // kvpConnectionProxy
            // 
            this.kvpConnectionProxy.BackColor = System.Drawing.SystemColors.Control;
            this.kvpConnectionProxy.Key = "Connection Proxy";
            this.kvpConnectionProxy.Location = new System.Drawing.Point(330, 6);
            this.kvpConnectionProxy.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpConnectionProxy.Name = "kvpConnectionProxy";
            this.kvpConnectionProxy.Size = new System.Drawing.Size(112, 24);
            this.kvpConnectionProxy.TabIndex = 38;
            this.kvpConnectionProxy.TabStop = false;
            this.kvpConnectionProxy.Tooltip = "This is used in and defines the connection.";
            this.kvpConnectionProxy.Value = "";
            // 
            // kvpLog
            // 
            this.kvpLog.BackColor = System.Drawing.Color.LightBlue;
            this.kvpLog.Key = "Log";
            this.kvpLog.Location = new System.Drawing.Point(445, 6);
            this.kvpLog.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpLog.Name = "kvpLog";
            this.kvpLog.Size = new System.Drawing.Size(34, 24);
            this.kvpLog.TabIndex = 29;
            this.kvpLog.TabStop = false;
            this.kvpLog.Tooltip = "The log used to test the application.";
            this.kvpLog.Value = "";
            // 
            // kvpLogRuleSet
            // 
            this.kvpLogRuleSet.BackColor = System.Drawing.SystemColors.Control;
            this.kvpLogRuleSet.Key = "Log Rule Set";
            this.kvpLogRuleSet.Location = new System.Drawing.Point(482, 6);
            this.kvpLogRuleSet.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpLogRuleSet.Name = "kvpLogRuleSet";
            this.kvpLogRuleSet.Size = new System.Drawing.Size(87, 24);
            this.kvpLogRuleSet.TabIndex = 40;
            this.kvpLogRuleSet.TabStop = false;
            this.kvpLogRuleSet.Tooltip = "This is used in and defines the log entries.";
            this.kvpLogRuleSet.Value = "";
            // 
            // kvpMonitor
            // 
            this.kvpMonitor.BackColor = System.Drawing.Color.LightBlue;
            this.kvpMonitor.Key = "Monitor";
            this.kvpMonitor.Location = new System.Drawing.Point(572, 6);
            this.kvpMonitor.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpMonitor.Name = "kvpMonitor";
            this.kvpMonitor.Size = new System.Drawing.Size(55, 24);
            this.kvpMonitor.TabIndex = 42;
            this.kvpMonitor.TabStop = false;
            this.kvpMonitor.Tooltip = "";
            this.kvpMonitor.Value = "";
            // 
            // kvpConcurrentUsers
            // 
            this.kvpConcurrentUsers.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpConcurrentUsers.Key = "Concurrent Users";
            this.kvpConcurrentUsers.Location = new System.Drawing.Point(639, 6);
            this.kvpConcurrentUsers.Margin = new System.Windows.Forms.Padding(12, 6, 0, 3);
            this.kvpConcurrentUsers.Name = "kvpConcurrentUsers";
            this.kvpConcurrentUsers.Size = new System.Drawing.Size(111, 24);
            this.kvpConcurrentUsers.TabIndex = 30;
            this.kvpConcurrentUsers.TabStop = false;
            this.kvpConcurrentUsers.Tooltip = "The count(s) of the concurrent users generated, the minimum given value equals on" +
    "e.";
            this.kvpConcurrentUsers.Value = "";
            // 
            // kvpPrecision
            // 
            this.kvpPrecision.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpPrecision.Key = "Precision";
            this.kvpPrecision.Location = new System.Drawing.Point(3, 39);
            this.kvpPrecision.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpPrecision.Name = "kvpPrecision";
            this.kvpPrecision.Size = new System.Drawing.Size(65, 24);
            this.kvpPrecision.TabIndex = 28;
            this.kvpPrecision.TabStop = false;
            this.kvpPrecision.Tooltip = "A static multiplier of the runtime for each concurrency level. Must be greater th" +
    "an zero.";
            this.kvpPrecision.Value = "";
            // 
            // kvpDynamicRunMultiplier
            // 
            this.kvpDynamicRunMultiplier.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpDynamicRunMultiplier.Key = "Dynamic Run Multiplier";
            this.kvpDynamicRunMultiplier.Location = new System.Drawing.Point(71, 39);
            this.kvpDynamicRunMultiplier.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpDynamicRunMultiplier.Name = "kvpDynamicRunMultiplier";
            this.kvpDynamicRunMultiplier.Size = new System.Drawing.Size(143, 24);
            this.kvpDynamicRunMultiplier.TabIndex = 32;
            this.kvpDynamicRunMultiplier.TabStop = false;
            this.kvpDynamicRunMultiplier.Tooltip = resources.GetString("kvpDynamicRunMultiplier.Tooltip");
            this.kvpDynamicRunMultiplier.Value = "";
            // 
            // kvpDelay
            // 
            this.kvpDelay.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpDelay.Key = "Delay";
            this.kvpDelay.Location = new System.Drawing.Point(217, 39);
            this.kvpDelay.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpDelay.Name = "kvpDelay";
            this.kvpDelay.Size = new System.Drawing.Size(45, 24);
            this.kvpDelay.TabIndex = 33;
            this.kvpDelay.TabStop = false;
            this.kvpDelay.Tooltip = "The delay in milliseconds between the execution of log entries per user.\nKeep thi" +
    "s and zero to have an ASAP test.";
            this.kvpDelay.Value = "";
            // 
            // kvpShuffle
            // 
            this.kvpShuffle.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpShuffle.Key = "Shuffle";
            this.kvpShuffle.Location = new System.Drawing.Point(265, 39);
            this.kvpShuffle.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpShuffle.Name = "kvpShuffle";
            this.kvpShuffle.Size = new System.Drawing.Size(53, 24);
            this.kvpShuffle.TabIndex = 34;
            this.kvpShuffle.TabStop = false;
            this.kvpShuffle.Tooltip = "The actions and loose log entries will be shuffled for each concurrent user when " +
    "testing, creating unique usage patterns.\nObligatory for Fast Action and Log Entr" +
    "y Distribution.";
            this.kvpShuffle.Value = "";
            // 
            // kvpDistribute
            // 
            this.kvpDistribute.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpDistribute.Key = "Distribute";
            this.kvpDistribute.Location = new System.Drawing.Point(321, 39);
            this.kvpDistribute.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpDistribute.Name = "kvpDistribute";
            this.kvpDistribute.Size = new System.Drawing.Size(67, 24);
            this.kvpDistribute.TabIndex = 35;
            this.kvpDistribute.TabStop = false;
            this.kvpDistribute.Tooltip = "Action and Loose Log Entry Distribution\nFast: The length of the log stays the sam" +
    "e, entries are picked by chance based on the occurance.\nFull: entries are execut" +
    "ed X times the occurance.";
            this.kvpDistribute.Value = "";
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.Color.White;
            this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel.Controls.Add(this.btnSaveTheRFile);
            this.panel.Controls.Add(this.btnSaveTheConfigurationAndTheChosenResultSet);
            this.panel.Controls.Add(this.flpDetailedResultsListing);
            this.panel.Controls.Add(this.dgvDetailedResultsListing);
            this.panel.Controls.Add(this.lblWait);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(800, 269);
            this.panel.TabIndex = 4;
            // 
            // btnSaveTheRFile
            // 
            this.btnSaveTheRFile.AutoSize = true;
            this.btnSaveTheRFile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveTheRFile.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveTheRFile.Enabled = false;
            this.btnSaveTheRFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveTheRFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveTheRFile.Location = new System.Drawing.Point(326, 93);
            this.btnSaveTheRFile.MaximumSize = new System.Drawing.Size(116, 24);
            this.btnSaveTheRFile.Name = "btnSaveTheRFile";
            this.btnSaveTheRFile.Size = new System.Drawing.Size(116, 24);
            this.btnSaveTheRFile.TabIndex = 2;
            this.btnSaveTheRFile.Text = "Save the R-file...";
            this.toolTip.SetToolTip(this.btnSaveTheRFile, "Save the results as bytes on file, this can be opened using vApus.Report.exe (Too" +
        "ls > Report...).");
            this.btnSaveTheRFile.UseVisualStyleBackColor = false;
            this.btnSaveTheRFile.Click += new System.EventHandler(this.btnSaveTheRFile_Click);
            // 
            // btnSaveTheConfigurationAndTheChosenResultSet
            // 
            this.btnSaveTheConfigurationAndTheChosenResultSet.AutoSize = true;
            this.btnSaveTheConfigurationAndTheChosenResultSet.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveTheConfigurationAndTheChosenResultSet.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveTheConfigurationAndTheChosenResultSet.Enabled = false;
            this.btnSaveTheConfigurationAndTheChosenResultSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveTheConfigurationAndTheChosenResultSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveTheConfigurationAndTheChosenResultSet.Location = new System.Drawing.Point(3, 93);
            this.btnSaveTheConfigurationAndTheChosenResultSet.MaximumSize = new System.Drawing.Size(317, 24);
            this.btnSaveTheConfigurationAndTheChosenResultSet.Name = "btnSaveTheConfigurationAndTheChosenResultSet";
            this.btnSaveTheConfigurationAndTheChosenResultSet.Size = new System.Drawing.Size(317, 24);
            this.btnSaveTheConfigurationAndTheChosenResultSet.TabIndex = 1;
            this.btnSaveTheConfigurationAndTheChosenResultSet.Text = "Save the Configuration and the Chosen Result Set...";
            this.btnSaveTheConfigurationAndTheChosenResultSet.UseVisualStyleBackColor = false;
            this.btnSaveTheConfigurationAndTheChosenResultSet.Click += new System.EventHandler(this.btnSaveTheConfigurationAndTheChosenResultSet_Click);
            // 
            // flpDetailedResultsListing
            // 
            this.flpDetailedResultsListing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpDetailedResultsListing.AutoScroll = true;
            this.flpDetailedResultsListing.Controls.Add(this.label10);
            this.flpDetailedResultsListing.Controls.Add(this.label6);
            this.flpDetailedResultsListing.Controls.Add(this.rdbAverages);
            this.flpDetailedResultsListing.Controls.Add(this.lblPivotLevel);
            this.flpDetailedResultsListing.Controls.Add(this.pnlBorderPivotLevel);
            this.flpDetailedResultsListing.Controls.Add(this.rdbUsers);
            this.flpDetailedResultsListing.Controls.Add(this.label9);
            this.flpDetailedResultsListing.Controls.Add(this.rdbErrors);
            this.flpDetailedResultsListing.Controls.Add(this.label5);
            this.flpDetailedResultsListing.Controls.Add(this.tbtnConcurrentUsers);
            this.flpDetailedResultsListing.Controls.Add(this.tbtnPrecision);
            this.flpDetailedResultsListing.Controls.Add(this.tbtnRun);
            this.flpDetailedResultsListing.Controls.Add(this.tbtnUser);
            this.flpDetailedResultsListing.Controls.Add(this.tbtnUserAction);
            this.flpDetailedResultsListing.Controls.Add(this.tbtnLogEntry);
            this.flpDetailedResultsListing.Controls.Add(this.btnRefresh);
            this.flpDetailedResultsListing.Location = new System.Drawing.Point(0, 0);
            this.flpDetailedResultsListing.Name = "flpDetailedResultsListing";
            this.flpDetailedResultsListing.Size = new System.Drawing.Size(799, 87);
            this.flpDetailedResultsListing.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(3, 6);
            this.label10.Margin = new System.Windows.Forms.Padding(3, 6, 6, 3);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(176, 20);
            this.label10.TabIndex = 18;
            this.label10.Text = "Detailed Results Listing";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(188, 9);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 16);
            this.label6.TabIndex = 14;
            this.label6.Text = "for";
            // 
            // rdbAverages
            // 
            this.rdbAverages.AutoSize = true;
            this.rdbAverages.Checked = true;
            this.rdbAverages.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbAverages.Location = new System.Drawing.Point(217, 7);
            this.rdbAverages.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.rdbAverages.Name = "rdbAverages";
            this.rdbAverages.Size = new System.Drawing.Size(85, 20);
            this.rdbAverages.TabIndex = 0;
            this.rdbAverages.TabStop = true;
            this.rdbAverages.Text = "Averages";
            this.rdbAverages.UseVisualStyleBackColor = true;
            this.rdbAverages.CheckedChanged += new System.EventHandler(this.filterRDB_CheckedChanged);
            // 
            // lblPivotLevel
            // 
            this.lblPivotLevel.AutoSize = true;
            this.lblPivotLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPivotLevel.Location = new System.Drawing.Point(308, 9);
            this.lblPivotLevel.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblPivotLevel.Name = "lblPivotLevel";
            this.lblPivotLevel.Size = new System.Drawing.Size(72, 16);
            this.lblPivotLevel.TabIndex = 13;
            this.lblPivotLevel.Text = "pivot level:";
            // 
            // pnlBorderPivotLevel
            // 
            this.pnlBorderPivotLevel.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderPivotLevel.Controls.Add(this.cboPivotLevel);
            this.pnlBorderPivotLevel.Location = new System.Drawing.Point(386, 6);
            this.pnlBorderPivotLevel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.pnlBorderPivotLevel.Name = "pnlBorderPivotLevel";
            this.pnlBorderPivotLevel.Size = new System.Drawing.Size(127, 23);
            this.pnlBorderPivotLevel.TabIndex = 1;
            // 
            // cboPivotLevel
            // 
            this.cboPivotLevel.BackColor = System.Drawing.Color.White;
            this.cboPivotLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPivotLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboPivotLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPivotLevel.FormattingEnabled = true;
            this.cboPivotLevel.Items.AddRange(new object[] {
            "Concurrent Users",
            "Precision",
            "Run"});
            this.cboPivotLevel.Location = new System.Drawing.Point(1, 1);
            this.cboPivotLevel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.cboPivotLevel.Name = "cboPivotLevel";
            this.cboPivotLevel.Size = new System.Drawing.Size(125, 21);
            this.cboPivotLevel.TabIndex = 1;
            // 
            // rdbUsers
            // 
            this.rdbUsers.AutoSize = true;
            this.rdbUsers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbUsers.Location = new System.Drawing.Point(519, 7);
            this.rdbUsers.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.rdbUsers.Name = "rdbUsers";
            this.rdbUsers.Size = new System.Drawing.Size(62, 20);
            this.rdbUsers.TabIndex = 2;
            this.rdbUsers.Text = "Users";
            this.rdbUsers.UseVisualStyleBackColor = true;
            this.rdbUsers.CheckedChanged += new System.EventHandler(this.filterRDB_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(587, 9);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 16);
            this.label9.TabIndex = 21;
            this.label9.Text = "or just";
            // 
            // rdbErrors
            // 
            this.rdbErrors.AutoSize = true;
            this.rdbErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbErrors.Location = new System.Drawing.Point(636, 7);
            this.rdbErrors.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.rdbErrors.Name = "rdbErrors";
            this.rdbErrors.Size = new System.Drawing.Size(62, 20);
            this.rdbErrors.TabIndex = 3;
            this.rdbErrors.Text = "Errors";
            this.rdbErrors.UseVisualStyleBackColor = true;
            this.rdbErrors.CheckedChanged += new System.EventHandler(this.filterRDB_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(704, 9);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "filter level:";
            // 
            // tbtnConcurrentUsers
            // 
            this.tbtnConcurrentUsers.Appearance = System.Windows.Forms.Appearance.Button;
            this.tbtnConcurrentUsers.AutoSize = true;
            this.tbtnConcurrentUsers.BackColor = System.Drawing.SystemColors.Control;
            this.tbtnConcurrentUsers.Checked = true;
            this.tbtnConcurrentUsers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tbtnConcurrentUsers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbtnConcurrentUsers.Location = new System.Drawing.Point(3, 38);
            this.tbtnConcurrentUsers.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.tbtnConcurrentUsers.Name = "tbtnConcurrentUsers";
            this.tbtnConcurrentUsers.Size = new System.Drawing.Size(115, 23);
            this.tbtnConcurrentUsers.TabIndex = 4;
            this.tbtnConcurrentUsers.Text = "Concurrent Users";
            this.tbtnConcurrentUsers.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tbtnConcurrentUsers.UseVisualStyleBackColor = false;
            this.tbtnConcurrentUsers.CheckedChanged += new System.EventHandler(this.filter_CheckedChanged);
            // 
            // tbtnPrecision
            // 
            this.tbtnPrecision.Appearance = System.Windows.Forms.Appearance.Button;
            this.tbtnPrecision.AutoSize = true;
            this.tbtnPrecision.BackColor = System.Drawing.SystemColors.Control;
            this.tbtnPrecision.Checked = true;
            this.tbtnPrecision.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tbtnPrecision.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbtnPrecision.Location = new System.Drawing.Point(124, 38);
            this.tbtnPrecision.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.tbtnPrecision.Name = "tbtnPrecision";
            this.tbtnPrecision.Size = new System.Drawing.Size(69, 23);
            this.tbtnPrecision.TabIndex = 5;
            this.tbtnPrecision.Text = "Precision";
            this.tbtnPrecision.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tbtnPrecision.UseVisualStyleBackColor = false;
            this.tbtnPrecision.Visible = false;
            this.tbtnPrecision.CheckedChanged += new System.EventHandler(this.filter_CheckedChanged);
            // 
            // tbtnRun
            // 
            this.tbtnRun.Appearance = System.Windows.Forms.Appearance.Button;
            this.tbtnRun.AutoSize = true;
            this.tbtnRun.BackColor = System.Drawing.SystemColors.Control;
            this.tbtnRun.Checked = true;
            this.tbtnRun.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tbtnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbtnRun.Location = new System.Drawing.Point(199, 38);
            this.tbtnRun.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.tbtnRun.Name = "tbtnRun";
            this.tbtnRun.Size = new System.Drawing.Size(40, 23);
            this.tbtnRun.TabIndex = 6;
            this.tbtnRun.Text = "Run";
            this.tbtnRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tbtnRun.UseVisualStyleBackColor = false;
            this.tbtnRun.Visible = false;
            this.tbtnRun.CheckedChanged += new System.EventHandler(this.filter_CheckedChanged);
            // 
            // tbtnUser
            // 
            this.tbtnUser.Appearance = System.Windows.Forms.Appearance.Button;
            this.tbtnUser.AutoSize = true;
            this.tbtnUser.BackColor = System.Drawing.SystemColors.Control;
            this.tbtnUser.Checked = true;
            this.tbtnUser.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tbtnUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbtnUser.Location = new System.Drawing.Point(245, 38);
            this.tbtnUser.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.tbtnUser.Name = "tbtnUser";
            this.tbtnUser.Size = new System.Drawing.Size(43, 23);
            this.tbtnUser.TabIndex = 7;
            this.tbtnUser.Text = "User";
            this.tbtnUser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tbtnUser.UseVisualStyleBackColor = false;
            this.tbtnUser.Visible = false;
            this.tbtnUser.CheckedChanged += new System.EventHandler(this.filter_CheckedChanged);
            // 
            // tbtnUserAction
            // 
            this.tbtnUserAction.Appearance = System.Windows.Forms.Appearance.Button;
            this.tbtnUserAction.AutoSize = true;
            this.tbtnUserAction.BackColor = System.Drawing.SystemColors.Control;
            this.tbtnUserAction.Checked = true;
            this.tbtnUserAction.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tbtnUserAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbtnUserAction.Location = new System.Drawing.Point(303, 38);
            this.tbtnUserAction.Margin = new System.Windows.Forms.Padding(12, 6, 3, 3);
            this.tbtnUserAction.Name = "tbtnUserAction";
            this.tbtnUserAction.Size = new System.Drawing.Size(83, 23);
            this.tbtnUserAction.TabIndex = 8;
            this.tbtnUserAction.Text = "User Action";
            this.tbtnUserAction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tbtnUserAction.UseVisualStyleBackColor = false;
            this.tbtnUserAction.CheckedChanged += new System.EventHandler(this.filter_CheckedChanged);
            // 
            // tbtnLogEntry
            // 
            this.tbtnLogEntry.Appearance = System.Windows.Forms.Appearance.Button;
            this.tbtnLogEntry.AutoSize = true;
            this.tbtnLogEntry.BackColor = System.Drawing.SystemColors.Control;
            this.tbtnLogEntry.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbtnLogEntry.Location = new System.Drawing.Point(392, 38);
            this.tbtnLogEntry.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.tbtnLogEntry.Name = "tbtnLogEntry";
            this.tbtnLogEntry.Size = new System.Drawing.Size(71, 23);
            this.tbtnLogEntry.TabIndex = 9;
            this.tbtnLogEntry.Text = "Log Entry";
            this.tbtnLogEntry.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tbtnLogEntry.UseVisualStyleBackColor = false;
            this.tbtnLogEntry.CheckedChanged += new System.EventHandler(this.filter_CheckedChanged);
            // 
            // btnRefresh
            // 
            this.btnRefresh.AutoSize = true;
            this.btnRefresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(490, 36);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(24, 4, 3, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(83, 26);
            this.btnRefresh.TabIndex = 10;
            this.btnRefresh.Text = "      Refresh";
            this.toolTip.SetToolTip(this.btnRefresh, "If the report does not look like it should, try clicking this button.");
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // dgvDetailedResultsListing
            // 
            this.dgvDetailedResultsListing.AllowUserToAddRows = false;
            this.dgvDetailedResultsListing.AllowUserToDeleteRows = false;
            this.dgvDetailedResultsListing.AllowUserToResizeRows = false;
            this.dgvDetailedResultsListing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDetailedResultsListing.BackgroundColor = System.Drawing.Color.White;
            this.dgvDetailedResultsListing.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetailedResultsListing.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvDetailedResultsListing.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvDetailedResultsListing.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetailedResultsListing.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmDRLStartedAtSentAt,
            this.clmDRLMeasuredTime,
            this.clmDRLConcurrentUsers,
            this.clmDRLPrecision,
            this.clmDRLRun,
            this.clmDRLUser,
            this.clmDRLUserAction,
            this.clmDRLLogEntry,
            this.clmDRLLogEntriesProcessed,
            this.clmDRLThroughput,
            this.clmDRLResponseTime,
            this.clmDRLMaxResponseTime,
            this.clmDRLPercentileMaxResponseTime,
            this.clmDRLDelay,
            this.clmDRLErrors});
            this.dgvDetailedResultsListing.EnableHeadersVisualStyles = false;
            this.dgvDetailedResultsListing.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.dgvDetailedResultsListing.Location = new System.Drawing.Point(0, 132);
            this.dgvDetailedResultsListing.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.dgvDetailedResultsListing.Name = "dgvDetailedResultsListing";
            this.dgvDetailedResultsListing.ReadOnly = true;
            this.dgvDetailedResultsListing.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvDetailedResultsListing.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvDetailedResultsListing.Size = new System.Drawing.Size(799, 136);
            this.dgvDetailedResultsListing.TabIndex = 3;
            this.dgvDetailedResultsListing.VirtualMode = true;
            this.dgvDetailedResultsListing.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvDetailedResultsListing_CellValueNeeded);
            // 
            // clmDRLStartedAtSentAt
            // 
            this.clmDRLStartedAtSentAt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLStartedAtSentAt.HeaderText = "Started At";
            this.clmDRLStartedAtSentAt.Name = "clmDRLStartedAtSentAt";
            this.clmDRLStartedAtSentAt.ReadOnly = true;
            this.clmDRLStartedAtSentAt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLStartedAtSentAt.Width = 61;
            // 
            // clmDRLMeasuredTime
            // 
            this.clmDRLMeasuredTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLMeasuredTime.HeaderText = "Measured Time";
            this.clmDRLMeasuredTime.Name = "clmDRLMeasuredTime";
            this.clmDRLMeasuredTime.ReadOnly = true;
            this.clmDRLMeasuredTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLMeasuredTime.Width = 68;
            // 
            // clmDRLConcurrentUsers
            // 
            this.clmDRLConcurrentUsers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLConcurrentUsers.HeaderText = "Concurrent Users";
            this.clmDRLConcurrentUsers.Name = "clmDRLConcurrentUsers";
            this.clmDRLConcurrentUsers.ReadOnly = true;
            this.clmDRLConcurrentUsers.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLConcurrentUsers.Width = 112;
            // 
            // clmDRLPrecision
            // 
            this.clmDRLPrecision.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLPrecision.HeaderText = "Precision";
            this.clmDRLPrecision.Name = "clmDRLPrecision";
            this.clmDRLPrecision.ReadOnly = true;
            this.clmDRLPrecision.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLPrecision.Width = 75;
            // 
            // clmDRLRun
            // 
            this.clmDRLRun.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLRun.HeaderText = "Run";
            this.clmDRLRun.Name = "clmDRLRun";
            this.clmDRLRun.ReadOnly = true;
            this.clmDRLRun.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLRun.Width = 33;
            // 
            // clmDRLUser
            // 
            this.clmDRLUser.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLUser.HeaderText = "User";
            this.clmDRLUser.Name = "clmDRLUser";
            this.clmDRLUser.ReadOnly = true;
            this.clmDRLUser.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLUser.Width = 40;
            // 
            // clmDRLUserAction
            // 
            this.clmDRLUserAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLUserAction.HeaderText = "User Action";
            this.clmDRLUserAction.Name = "clmDRLUserAction";
            this.clmDRLUserAction.ReadOnly = true;
            this.clmDRLUserAction.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLUserAction.Width = 80;
            // 
            // clmDRLLogEntry
            // 
            this.clmDRLLogEntry.HeaderText = "Log Entry";
            this.clmDRLLogEntry.Name = "clmDRLLogEntry";
            this.clmDRLLogEntry.ReadOnly = true;
            this.clmDRLLogEntry.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLLogEntry.Width = 50;
            // 
            // clmDRLLogEntriesProcessed
            // 
            this.clmDRLLogEntriesProcessed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLLogEntriesProcessed.HeaderText = "Log Entries Processed";
            this.clmDRLLogEntriesProcessed.Name = "clmDRLLogEntriesProcessed";
            this.clmDRLLogEntriesProcessed.ReadOnly = true;
            this.clmDRLLogEntriesProcessed.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLLogEntriesProcessed.Width = 143;
            // 
            // clmDRLThroughput
            // 
            this.clmDRLThroughput.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLThroughput.HeaderText = "Throughput / s";
            this.clmDRLThroughput.Name = "clmDRLThroughput";
            this.clmDRLThroughput.ReadOnly = true;
            this.clmDRLThroughput.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLThroughput.Width = 86;
            // 
            // clmDRLResponseTime
            // 
            this.clmDRLResponseTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLResponseTime.HeaderText = "Response Time in ms";
            this.clmDRLResponseTime.Name = "clmDRLResponseTime";
            this.clmDRLResponseTime.ReadOnly = true;
            this.clmDRLResponseTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLResponseTime.Width = 99;
            // 
            // clmDRLMaxResponseTime
            // 
            this.clmDRLMaxResponseTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLMaxResponseTime.HeaderText = "Max. Response Time";
            this.clmDRLMaxResponseTime.Name = "clmDRLMaxResponseTime";
            this.clmDRLMaxResponseTime.ReadOnly = true;
            this.clmDRLMaxResponseTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLMaxResponseTime.Width = 124;
            // 
            // clmDRLPercentileMaxResponseTime
            // 
            this.clmDRLPercentileMaxResponseTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLPercentileMaxResponseTime.HeaderText = "95 % Percentile (Max. Response Time)";
            this.clmDRLPercentileMaxResponseTime.Name = "clmDRLPercentileMaxResponseTime";
            this.clmDRLPercentileMaxResponseTime.ReadOnly = true;
            this.clmDRLPercentileMaxResponseTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLPercentileMaxResponseTime.Width = 206;
            // 
            // clmDRLDelay
            // 
            this.clmDRLDelay.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLDelay.HeaderText = "Delay in ms";
            this.clmDRLDelay.Name = "clmDRLDelay";
            this.clmDRLDelay.ReadOnly = true;
            this.clmDRLDelay.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLDelay.Width = 49;
            // 
            // clmDRLErrors
            // 
            this.clmDRLErrors.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmDRLErrors.HeaderText = "Error(s)";
            this.clmDRLErrors.Name = "clmDRLErrors";
            this.clmDRLErrors.ReadOnly = true;
            this.clmDRLErrors.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDRLErrors.Width = 68;
            // 
            // lblWait
            // 
            this.lblWait.AutoSize = true;
            this.lblWait.BackColor = System.Drawing.Color.Orange;
            this.lblWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWait.ForeColor = System.Drawing.Color.Black;
            this.lblWait.Location = new System.Drawing.Point(448, 98);
            this.lblWait.Name = "lblWait";
            this.lblWait.Size = new System.Drawing.Size(308, 16);
            this.lblWait.TabIndex = 15;
            this.lblWait.Text = "[Please wait, loading file (can take a while)]";
            this.lblWait.Visible = false;
            // 
            // StresstestReportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.split2);
            this.Name = "StresstestReportControl";
            this.Size = new System.Drawing.Size(800, 423);
            this.split2.Panel1.ResumeLayout(false);
            this.split2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split2)).EndInit();
            this.split2.ResumeLayout(false);
            this.flpSaveConfiguration.ResumeLayout(false);
            this.flpSaveConfiguration.PerformLayout();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.flpDetailedResultsListing.ResumeLayout(false);
            this.flpDetailedResultsListing.PerformLayout();
            this.pnlBorderPivotLevel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailedResultsListing)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer split2;
        private System.Windows.Forms.FlowLayoutPanel flpSaveConfiguration;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.DataGridView dgvDetailedResultsListing;
        private System.Windows.Forms.Label lblWait;
        private System.Windows.Forms.Button btnSaveTheConfigurationAndTheChosenResultSet;
        private System.Windows.Forms.FlowLayoutPanel flpDetailedResultsListing;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label5;
        private Util.ToggleButton tbtnConcurrentUsers;
        private Util.ToggleButton tbtnPrecision;
        private Util.ToggleButton tbtnRun;
        private Util.ToggleButton tbtnUserAction;
        private Util.ToggleButton tbtnLogEntry;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rdbAverages;
        private System.Windows.Forms.Label lblPivotLevel;
        private System.Windows.Forms.ComboBox cboPivotLevel;
        private System.Windows.Forms.RadioButton rdbUsers;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rdbErrors;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnSaveTheRFile;
        private Util.ToggleButton tbtnUser;
        private System.Windows.Forms.Label label3;
        private Util.KeyValuePairControl kvpStresstest;
        private Util.KeyValuePairControl kvpConnection;
        private Util.KeyValuePairControl kvpConnectionProxy;
        private Util.KeyValuePairControl kvpLog;
        private Util.KeyValuePairControl kvpLogRuleSet;
        private Util.KeyValuePairControl kvpMonitor;
        private Util.KeyValuePairControl kvpConcurrentUsers;
        private Util.KeyValuePairControl kvpPrecision;
        private Util.KeyValuePairControl kvpDynamicRunMultiplier;
        private Util.KeyValuePairControl kvpDelay;
        private Util.KeyValuePairControl kvpShuffle;
        private Util.KeyValuePairControl kvpDistribute;
        private Util.KeyValuePairControl kvpSolution;
        private System.Windows.Forms.Panel pnlBorderPivotLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLStartedAtSentAt;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLMeasuredTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLConcurrentUsers;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLPrecision;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLRun;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLUser;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLUserAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLLogEntry;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLLogEntriesProcessed;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLThroughput;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLResponseTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLMaxResponseTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLPercentileMaxResponseTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLDelay;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDRLErrors;

    }
}
