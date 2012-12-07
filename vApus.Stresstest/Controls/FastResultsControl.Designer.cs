namespace vApus.Stresstest
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.pnl = new System.Windows.Forms.Panel();
            this.pnlFastResults = new System.Windows.Forms.Panel();
            this.flpFastResultsHeader = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblUpdatesIn = new System.Windows.Forms.Label();
            this.lbtnStresstest = new vApus.Util.LinkButton();
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
            this.flpConfiguration = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.kvpStresstest = new vApus.Util.KeyValuePairControl();
            this.kvpConnection = new vApus.Util.KeyValuePairControl();
            this.kvpConnectionProxy = new vApus.Util.KeyValuePairControl();
            this.kvpLog = new vApus.Util.KeyValuePairControl();
            this.kvpLogRuleSet = new vApus.Util.KeyValuePairControl();
            this.btnMonitor = new System.Windows.Forms.Button();
            this.kvpConcurrencies = new vApus.Util.KeyValuePairControl();
            this.kvpRuns = new vApus.Util.KeyValuePairControl();
            this.kvpDelay = new vApus.Util.KeyValuePairControl();
            this.kvpShuffle = new vApus.Util.KeyValuePairControl();
            this.kvpDistribute = new vApus.Util.KeyValuePairControl();
            this.kvpMonitorBefore = new vApus.Util.KeyValuePairControl();
            this.kvpMonitorAfter = new vApus.Util.KeyValuePairControl();
            this.epnlMessages = new vApus.Util.EventPanel();
            this.flpMetrics = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.kvmThreadsInUse = new vApus.Util.KeyValuePairControl();
            this.kvmCPUUsage = new vApus.Util.KeyValuePairControl();
            this.kvmContextSwitchesPerSecond = new vApus.Util.KeyValuePairControl();
            this.kvmMemoryUsage = new vApus.Util.KeyValuePairControl();
            this.kvmNicsSent = new vApus.Util.KeyValuePairControl();
            this.kvmNicsReceived = new vApus.Util.KeyValuePairControl();
            this.btnExport = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnl.SuspendLayout();
            this.pnlFastResults.SuspendLayout();
            this.flpFastResultsHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFastResults)).BeginInit();
            this.flpFastMetrics.SuspendLayout();
            this.pnlBorderDrillDown.SuspendLayout();
            this.flpConfiguration.SuspendLayout();
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
            this.splitContainer.Panel1.Controls.Add(this.pnl);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.epnlMessages);
            this.splitContainer.Panel2.Controls.Add(this.flpMetrics);
            this.splitContainer.Size = new System.Drawing.Size(897, 639);
            this.splitContainer.SplitterDistance = 384;
            this.splitContainer.TabIndex = 2;
            // 
            // pnl
            // 
            this.pnl.Controls.Add(this.pnlFastResults);
            this.pnl.Controls.Add(this.flpConfiguration);
            this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl.Location = new System.Drawing.Point(0, 0);
            this.pnl.Name = "pnl";
            this.pnl.Padding = new System.Windows.Forms.Padding(3);
            this.pnl.Size = new System.Drawing.Size(897, 384);
            this.pnl.TabIndex = 1;
            this.pnl.Text = "[Put title here]";
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
            this.pnlFastResults.Location = new System.Drawing.Point(0, 104);
            this.pnlFastResults.Name = "pnlFastResults";
            this.pnlFastResults.Size = new System.Drawing.Size(897, 280);
            this.pnlFastResults.TabIndex = 1;
            this.pnlFastResults.Text = "Fast Results";
            // 
            // flpFastResultsHeader
            // 
            this.flpFastResultsHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpFastResultsHeader.AutoScroll = true;
            this.flpFastResultsHeader.Controls.Add(this.label4);
            this.flpFastResultsHeader.Controls.Add(this.lblUpdatesIn);
            this.flpFastResultsHeader.Controls.Add(this.lbtnStresstest);
            this.flpFastResultsHeader.Location = new System.Drawing.Point(0, 0);
            this.flpFastResultsHeader.Margin = new System.Windows.Forms.Padding(0);
            this.flpFastResultsHeader.Name = "flpFastResultsHeader";
            this.flpFastResultsHeader.Size = new System.Drawing.Size(897, 37);
            this.flpFastResultsHeader.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(3, 6);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 6, 5, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "Fast Results";
            // 
            // lblUpdatesIn
            // 
            this.lblUpdatesIn.AutoSize = true;
            this.lblUpdatesIn.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdatesIn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblUpdatesIn.Location = new System.Drawing.Point(107, 7);
            this.lblUpdatesIn.Margin = new System.Windows.Forms.Padding(0, 7, 6, 3);
            this.lblUpdatesIn.Name = "lblUpdatesIn";
            this.lblUpdatesIn.Size = new System.Drawing.Size(0, 18);
            this.lblUpdatesIn.TabIndex = 9999;
            // 
            // lbtnStresstest
            // 
            this.lbtnStresstest.Active = true;
            this.lbtnStresstest.ActiveLinkColor = System.Drawing.Color.Black;
            this.lbtnStresstest.AutoSize = true;
            this.lbtnStresstest.BackColor = System.Drawing.Color.LightBlue;
            this.lbtnStresstest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnStresstest.ForeColor = System.Drawing.Color.Black;
            this.lbtnStresstest.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbtnStresstest.LinkColor = System.Drawing.Color.Black;
            this.lbtnStresstest.Location = new System.Drawing.Point(116, 6);
            this.lbtnStresstest.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnStresstest.MinimumSize = new System.Drawing.Size(0, 24);
            this.lbtnStresstest.Name = "lbtnStresstest";
            this.lbtnStresstest.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnStresstest.RadioButtonBehavior = true;
            this.lbtnStresstest.Size = new System.Drawing.Size(95, 24);
            this.lbtnStresstest.TabIndex = 0;
            this.lbtnStresstest.TabStop = true;
            this.lbtnStresstest.Text = "The Stresstest";
            this.lbtnStresstest.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnStresstest.Visible = false;
            this.lbtnStresstest.VisitedLinkColor = System.Drawing.Color.Black;
            this.lbtnStresstest.ActiveChanged += new System.EventHandler(this.lbtnStresstest_ActiveChanged);
            // 
            // dgvFastResults
            // 
            this.dgvFastResults.AllowUserToAddRows = false;
            this.dgvFastResults.AllowUserToDeleteRows = false;
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
            this.dgvFastResults.Location = new System.Drawing.Point(0, 96);
            this.dgvFastResults.Name = "dgvFastResults";
            this.dgvFastResults.ReadOnly = true;
            this.dgvFastResults.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvFastResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvFastResults.Size = new System.Drawing.Size(897, 184);
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
            this.flpFastMetrics.Location = new System.Drawing.Point(-1, 37);
            this.flpFastMetrics.Name = "flpFastMetrics";
            this.flpFastMetrics.Size = new System.Drawing.Size(899, 60);
            this.flpFastMetrics.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Drill down to";
            // 
            // pnlBorderDrillDown
            // 
            this.pnlBorderDrillDown.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderDrillDown.Controls.Add(this.cboDrillDown);
            this.pnlBorderDrillDown.Location = new System.Drawing.Point(86, 3);
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
            this.lblStarted.Location = new System.Drawing.Point(216, 6);
            this.lblStarted.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblStarted.Name = "lblStarted";
            this.lblStarted.Size = new System.Drawing.Size(0, 16);
            this.lblStarted.TabIndex = 0;
            // 
            // lblMeasuredRuntime
            // 
            this.lblMeasuredRuntime.AutoSize = true;
            this.lblMeasuredRuntime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMeasuredRuntime.Location = new System.Drawing.Point(216, 6);
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
            this.btnRerunning.Location = new System.Drawing.Point(219, 3);
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
            this.lblStopped.Location = new System.Drawing.Point(317, 3);
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
            this.chkReadable.Location = new System.Drawing.Point(326, 7);
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
            this.btnSaveDisplayedResults.Location = new System.Drawing.Point(398, 3);
            this.btnSaveDisplayedResults.MaximumSize = new System.Drawing.Size(165, 24);
            this.btnSaveDisplayedResults.Name = "btnSaveDisplayedResults";
            this.btnSaveDisplayedResults.Size = new System.Drawing.Size(165, 24);
            this.btnSaveDisplayedResults.TabIndex = 4;
            this.btnSaveDisplayedResults.Text = "Save Displayed Results...";
            this.btnSaveDisplayedResults.UseVisualStyleBackColor = false;
            this.btnSaveDisplayedResults.Click += new System.EventHandler(this.btnSaveDisplayedResults_Click);
            // 
            // flpConfiguration
            // 
            this.flpConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpConfiguration.AutoScroll = true;
            this.flpConfiguration.BackColor = System.Drawing.Color.White;
            this.flpConfiguration.Controls.Add(this.label3);
            this.flpConfiguration.Controls.Add(this.kvpStresstest);
            this.flpConfiguration.Controls.Add(this.kvpConnection);
            this.flpConfiguration.Controls.Add(this.kvpConnectionProxy);
            this.flpConfiguration.Controls.Add(this.kvpLog);
            this.flpConfiguration.Controls.Add(this.kvpLogRuleSet);
            this.flpConfiguration.Controls.Add(this.btnMonitor);
            this.flpConfiguration.Controls.Add(this.kvpConcurrencies);
            this.flpConfiguration.Controls.Add(this.kvpRuns);
            this.flpConfiguration.Controls.Add(this.kvpDelay);
            this.flpConfiguration.Controls.Add(this.kvpShuffle);
            this.flpConfiguration.Controls.Add(this.kvpDistribute);
            this.flpConfiguration.Controls.Add(this.kvpMonitorBefore);
            this.flpConfiguration.Controls.Add(this.kvpMonitorAfter);
            this.flpConfiguration.Location = new System.Drawing.Point(0, 0);
            this.flpConfiguration.Name = "flpConfiguration";
            this.flpConfiguration.Size = new System.Drawing.Size(897, 105);
            this.flpConfiguration.TabIndex = 0;
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
            this.label3.TabIndex = 16;
            this.label3.Text = "Configuration";
            // 
            // kvpStresstest
            // 
            this.kvpStresstest.BackColor = System.Drawing.Color.LightBlue;
            this.kvpStresstest.Key = "Stresstest";
            this.kvpStresstest.Location = new System.Drawing.Point(116, 6);
            this.kvpStresstest.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpStresstest.Name = "kvpStresstest";
            this.kvpStresstest.Size = new System.Drawing.Size(69, 24);
            this.kvpStresstest.TabIndex = 14;
            this.kvpStresstest.TabStop = false;
            this.kvpStresstest.Tooltip = "";
            this.kvpStresstest.Value = "";
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
            this.kvpConnectionProxy.Key = "Connection Proxy";
            this.kvpConnectionProxy.Location = new System.Drawing.Point(268, 6);
            this.kvpConnectionProxy.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpConnectionProxy.Name = "kvpConnectionProxy";
            this.kvpConnectionProxy.Size = new System.Drawing.Size(112, 24);
            this.kvpConnectionProxy.TabIndex = 15;
            this.kvpConnectionProxy.TabStop = false;
            this.kvpConnectionProxy.Tooltip = "This is used in and defines the connection.";
            this.kvpConnectionProxy.Value = "";
            // 
            // kvpLog
            // 
            this.kvpLog.BackColor = System.Drawing.Color.LightBlue;
            this.kvpLog.Key = "Log";
            this.kvpLog.Location = new System.Drawing.Point(383, 6);
            this.kvpLog.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpLog.Name = "kvpLog";
            this.kvpLog.Size = new System.Drawing.Size(34, 24);
            this.kvpLog.TabIndex = 8;
            this.kvpLog.TabStop = false;
            this.kvpLog.Tooltip = "The log used to test the application.";
            this.kvpLog.Value = "";
            // 
            // kvpLogRuleSet
            // 
            this.kvpLogRuleSet.BackColor = System.Drawing.SystemColors.Control;
            this.kvpLogRuleSet.Key = "Log Rule Set";
            this.kvpLogRuleSet.Location = new System.Drawing.Point(420, 6);
            this.kvpLogRuleSet.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpLogRuleSet.Name = "kvpLogRuleSet";
            this.kvpLogRuleSet.Size = new System.Drawing.Size(87, 24);
            this.kvpLogRuleSet.TabIndex = 17;
            this.kvpLogRuleSet.TabStop = false;
            this.kvpLogRuleSet.Tooltip = "This is used in and defines the log entries.";
            this.kvpLogRuleSet.Value = "";
            // 
            // btnMonitor
            // 
            this.btnMonitor.AutoSize = true;
            this.btnMonitor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMonitor.BackColor = System.Drawing.Color.LightBlue;
            this.btnMonitor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMonitor.Location = new System.Drawing.Point(510, 5);
            this.btnMonitor.Margin = new System.Windows.Forms.Padding(3, 5, 0, 3);
            this.btnMonitor.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnMonitor.Name = "btnMonitor";
            this.btnMonitor.Size = new System.Drawing.Size(73, 24);
            this.btnMonitor.TabIndex = 18;
            this.btnMonitor.Text = "Monitor...";
            this.toolTip.SetToolTip(this.btnMonitor, "The monitors used to link stresstest results to performance counters. Maximum 5 a" +
        "llowed.");
            this.btnMonitor.UseVisualStyleBackColor = false;
            this.btnMonitor.Click += new System.EventHandler(this.btnMonitor_Click);
            // 
            // kvpConcurrencies
            // 
            this.kvpConcurrencies.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpConcurrencies.Key = "Concurrencies";
            this.kvpConcurrencies.Location = new System.Drawing.Point(595, 6);
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
            this.kvpRuns.Location = new System.Drawing.Point(692, 6);
            this.kvpRuns.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpRuns.Name = "kvpRuns";
            this.kvpRuns.Size = new System.Drawing.Size(42, 24);
            this.kvpRuns.TabIndex = 8;
            this.kvpRuns.TabStop = false;
            this.kvpRuns.Tooltip = "A static multiplier of the runtime for each concurrency level. Must be greater th" +
    "an zero.";
            this.kvpRuns.Value = "";
            // 
            // kvpDelay
            // 
            this.kvpDelay.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpDelay.Key = "Delay";
            this.kvpDelay.Location = new System.Drawing.Point(737, 6);
            this.kvpDelay.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpDelay.Name = "kvpDelay";
            this.kvpDelay.Size = new System.Drawing.Size(45, 24);
            this.kvpDelay.TabIndex = 9;
            this.kvpDelay.TabStop = false;
            this.kvpDelay.Tooltip = "The delay in milliseconds between the execution of log entries per user.\r\nKeep th" +
    "is zero to have an ASAP test.";
            this.kvpDelay.Value = "";
            // 
            // kvpShuffle
            // 
            this.kvpShuffle.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpShuffle.Key = "Shuffle";
            this.kvpShuffle.Location = new System.Drawing.Point(785, 6);
            this.kvpShuffle.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpShuffle.Name = "kvpShuffle";
            this.kvpShuffle.Size = new System.Drawing.Size(53, 24);
            this.kvpShuffle.TabIndex = 10;
            this.kvpShuffle.TabStop = false;
            this.kvpShuffle.Tooltip = "The actions and loose log entries will be shuffled for each concurrent user when " +
    "testing, creating unique usage patterns.";
            this.kvpShuffle.Value = "";
            // 
            // kvpDistribute
            // 
            this.kvpDistribute.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpDistribute.Key = "Distribute";
            this.kvpDistribute.Location = new System.Drawing.Point(3, 39);
            this.kvpDistribute.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpDistribute.Name = "kvpDistribute";
            this.kvpDistribute.Size = new System.Drawing.Size(67, 24);
            this.kvpDistribute.TabIndex = 10;
            this.kvpDistribute.TabStop = false;
            this.kvpDistribute.Tooltip = "Action and Loose Log Entry Distribution\nFast: The length of the log stays the sam" +
    "e, entries are picked by chance based on the occurance.\nFull: entries are execut" +
    "ed X times the occurance.";
            this.kvpDistribute.Value = "";
            // 
            // kvpMonitorBefore
            // 
            this.kvpMonitorBefore.BackColor = System.Drawing.Color.GhostWhite;
            this.kvpMonitorBefore.Key = "Monitor Before";
            this.kvpMonitorBefore.Location = new System.Drawing.Point(73, 39);
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
            this.kvpMonitorAfter.Key = "Monitor After";
            this.kvpMonitorAfter.Location = new System.Drawing.Point(172, 39);
            this.kvpMonitorAfter.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpMonitorAfter.Name = "kvpMonitorAfter";
            this.kvpMonitorAfter.Size = new System.Drawing.Size(86, 24);
            this.kvpMonitorAfter.TabIndex = 20;
            this.kvpMonitorAfter.TabStop = false;
            this.kvpMonitorAfter.Tooltip = "Continue monitoring after the test is finished, expressed in minutes with a max o" +
    "f 60.";
            this.kvpMonitorAfter.Value = "";
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
            this.epnlMessages.Size = new System.Drawing.Size(897, 190);
            this.epnlMessages.TabIndex = 3;
            this.epnlMessages.CollapsedChanged += new System.EventHandler(this.epnlMessages_CollapsedChanged);
            // 
            // flpMetrics
            // 
            this.flpMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpMetrics.BackColor = System.Drawing.Color.White;
            this.flpMetrics.Controls.Add(this.label2);
            this.flpMetrics.Controls.Add(this.kvmThreadsInUse);
            this.flpMetrics.Controls.Add(this.kvmCPUUsage);
            this.flpMetrics.Controls.Add(this.kvmContextSwitchesPerSecond);
            this.flpMetrics.Controls.Add(this.kvmMemoryUsage);
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
            this.label2.Text = "Client Monitoring and Stresstest Messages";
            // 
            // kvmThreadsInUse
            // 
            this.kvmThreadsInUse.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmThreadsInUse.Key = "Threads in Use";
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
            // kvmCPUUsage
            // 
            this.kvmCPUUsage.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmCPUUsage.Key = "CPU Usage";
            this.kvmCPUUsage.Location = new System.Drawing.Point(439, 9);
            this.kvmCPUUsage.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmCPUUsage.Name = "kvmCPUUsage";
            this.kvmCPUUsage.Size = new System.Drawing.Size(105, 16);
            this.kvmCPUUsage.TabIndex = 8;
            this.kvmCPUUsage.TabStop = false;
            this.kvmCPUUsage.Tooltip = "Try to keep this below 60 % to ensure that the client is not the bottleneck.";
            this.kvmCPUUsage.Value = "N/A";
            // 
            // kvmContextSwitchesPerSecond
            // 
            this.kvmContextSwitchesPerSecond.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmContextSwitchesPerSecond.Key = "Context Switches / s";
            this.kvmContextSwitchesPerSecond.Location = new System.Drawing.Point(547, 9);
            this.kvmContextSwitchesPerSecond.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmContextSwitchesPerSecond.Name = "kvmContextSwitchesPerSecond";
            this.kvmContextSwitchesPerSecond.Size = new System.Drawing.Size(158, 16);
            this.kvmContextSwitchesPerSecond.TabIndex = 8;
            this.kvmContextSwitchesPerSecond.TabStop = false;
            this.kvmContextSwitchesPerSecond.Tooltip = "";
            this.kvmContextSwitchesPerSecond.Value = "N/A";
            // 
            // kvmMemoryUsage
            // 
            this.kvmMemoryUsage.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmMemoryUsage.Key = "Memory Usage";
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
            // kvmNicsSent
            // 
            this.kvmNicsSent.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmNicsSent.Key = "NIC Usage (Sent)";
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
            this.kvmNicsReceived.Key = "NIC Usage (Received)";
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
            this.btnExport.Text = "Export Messages...";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // StresstestControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.splitContainer);
            this.Name = "StresstestControl";
            this.Size = new System.Drawing.Size(897, 639);
            this.SizeChanged += new System.EventHandler(this.StresstestControl_SizeChanged);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pnl.ResumeLayout(false);
            this.pnlFastResults.ResumeLayout(false);
            this.flpFastResultsHeader.ResumeLayout(false);
            this.flpFastResultsHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFastResults)).EndInit();
            this.flpFastMetrics.ResumeLayout(false);
            this.flpFastMetrics.PerformLayout();
            this.pnlBorderDrillDown.ResumeLayout(false);
            this.flpConfiguration.ResumeLayout(false);
            this.flpConfiguration.PerformLayout();
            this.flpMetrics.ResumeLayout(false);
            this.flpMetrics.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel pnl;
        private System.Windows.Forms.FlowLayoutPanel flpConfiguration;
        private System.Windows.Forms.Label label3;
        private vApus.Util.KeyValuePairControl kvpStresstest;
        private vApus.Util.KeyValuePairControl kvpConnection;
        private vApus.Util.KeyValuePairControl kvpConnectionProxy;
        private vApus.Util.KeyValuePairControl kvpLog;
        private vApus.Util.KeyValuePairControl kvpConcurrencies;
        private vApus.Util.KeyValuePairControl kvpRuns;
        private vApus.Util.KeyValuePairControl kvpDelay;
        private vApus.Util.KeyValuePairControl kvpShuffle;
        private vApus.Util.KeyValuePairControl kvpDistribute;
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
        private vApus.Util.KeyValuePairControl kvmContextSwitchesPerSecond;
        private vApus.Util.KeyValuePairControl kvmMemoryUsage;
        private vApus.Util.KeyValuePairControl kvmNicsSent;
        private vApus.Util.KeyValuePairControl kvmNicsReceived;
        private vApus.Util.KeyValuePairControl kvpLogRuleSet;
        private Util.EventPanel epnlMessages;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnMonitor;
        private System.Windows.Forms.Button btnRerunning;
        private System.Windows.Forms.Panel pnlBorderDrillDown;
        private Util.KeyValuePairControl kvpMonitorBefore;
        private Util.KeyValuePairControl kvpMonitorAfter;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox chkReadable;
        private System.Windows.Forms.DataGridView dgvFastResults;
        private Util.LinkButton lbtnStresstest;
        private System.Windows.Forms.FlowLayoutPanel flpFastResultsHeader;
    }
}
