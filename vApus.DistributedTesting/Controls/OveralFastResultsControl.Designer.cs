using vApus.Stresstest;
namespace vApus.DistributedTesting {
    partial class OveralFastResultsControl {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.dgvFastResults = new System.Windows.Forms.DataGridView();
            this.pnlFastResultListing = new System.Windows.Forms.Panel();
            this.flpFastResultsHeader = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFastResults = new System.Windows.Forms.Label();
            this.lblUpdatesIn = new System.Windows.Forms.Label();
            this.lbtnStresstests = new vApus.Util.LinkButton();
            this.flpFastMetrics = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlBorder = new System.Windows.Forms.Panel();
            this.cboDrillDown = new System.Windows.Forms.ComboBox();
            this.lblStarted = new System.Windows.Forms.Label();
            this.lblMeasuredRuntime = new System.Windows.Forms.Label();
            this.lblStopped = new System.Windows.Forms.Label();
            this.chkReadable = new System.Windows.Forms.CheckBox();
            this.btnSaveDisplayedResults = new System.Windows.Forms.Button();
            this.eventView = new vApus.Util.EventView();
            this.flpMetricsMaster = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.kvpRunningTests = new vApus.Util.KeyValuePairControl();
            this.kvpOK = new vApus.Util.KeyValuePairControl();
            this.kvpCancelled = new vApus.Util.KeyValuePairControl();
            this.kvpFailed = new vApus.Util.KeyValuePairControl();
            this.kvmMasterCPUUsage = new vApus.Util.KeyValuePairControl();
            this.kvmMasterMemoryUsage = new vApus.Util.KeyValuePairControl();
            this.kvmMasterNic = new vApus.Util.KeyValuePairControl();
            this.kvmMasterNicsSent = new vApus.Util.KeyValuePairControl();
            this.kvmMasterNicsReceived = new vApus.Util.KeyValuePairControl();
            this.btnMasterExportMessages = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFastResults)).BeginInit();
            this.pnlFastResultListing.SuspendLayout();
            this.flpFastResultsHeader.SuspendLayout();
            this.flpFastMetrics.SuspendLayout();
            this.pnlBorder.SuspendLayout();
            this.flpMetricsMaster.SuspendLayout();
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
            this.splitContainer.Panel1.Controls.Add(this.dgvFastResults);
            this.splitContainer.Panel1.Controls.Add(this.pnlFastResultListing);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.eventView);
            this.splitContainer.Panel2.Controls.Add(this.flpMetricsMaster);
            this.splitContainer.Size = new System.Drawing.Size(937, 700);
            this.splitContainer.SplitterDistance = 500;
            this.splitContainer.TabIndex = 0;
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
            this.dgvFastResults.Location = new System.Drawing.Point(1, 100);
            this.dgvFastResults.Name = "dgvFastResults";
            this.dgvFastResults.ReadOnly = true;
            this.dgvFastResults.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvFastResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgvFastResults.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFastResults.Size = new System.Drawing.Size(936, 401);
            this.dgvFastResults.TabIndex = 3;
            this.dgvFastResults.VirtualMode = true;
            this.dgvFastResults.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvFastResults_CellValueNeeded);
            this.dgvFastResults.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvFastResults_Scroll);
            // 
            // pnlFastResultListing
            // 
            this.pnlFastResultListing.BackColor = System.Drawing.Color.White;
            this.pnlFastResultListing.Controls.Add(this.flpFastResultsHeader);
            this.pnlFastResultListing.Controls.Add(this.flpFastMetrics);
            this.pnlFastResultListing.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFastResultListing.Location = new System.Drawing.Point(0, 0);
            this.pnlFastResultListing.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.pnlFastResultListing.Name = "pnlFastResultListing";
            this.pnlFastResultListing.Size = new System.Drawing.Size(937, 100);
            this.pnlFastResultListing.TabIndex = 0;
            this.pnlFastResultListing.Text = "Fast Results";
            // 
            // flpFastResultsHeader
            // 
            this.flpFastResultsHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpFastResultsHeader.AutoScroll = true;
            this.flpFastResultsHeader.Controls.Add(this.lblFastResults);
            this.flpFastResultsHeader.Controls.Add(this.lblUpdatesIn);
            this.flpFastResultsHeader.Controls.Add(this.lbtnStresstests);
            this.flpFastResultsHeader.Location = new System.Drawing.Point(0, 0);
            this.flpFastResultsHeader.Margin = new System.Windows.Forms.Padding(0);
            this.flpFastResultsHeader.Name = "flpFastResultsHeader";
            this.flpFastResultsHeader.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.flpFastResultsHeader.Size = new System.Drawing.Size(897, 40);
            this.flpFastResultsHeader.TabIndex = 1;
            // 
            // lblFastResults
            // 
            this.lblFastResults.AutoSize = true;
            this.lblFastResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFastResults.ForeColor = System.Drawing.Color.Black;
            this.lblFastResults.Location = new System.Drawing.Point(3, 9);
            this.lblFastResults.Margin = new System.Windows.Forms.Padding(3, 6, 5, 3);
            this.lblFastResults.Name = "lblFastResults";
            this.lblFastResults.Size = new System.Drawing.Size(99, 20);
            this.lblFastResults.TabIndex = 17;
            this.lblFastResults.Text = "Fast Results";
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
            // lbtnStresstests
            // 
            this.lbtnStresstests.Active = true;
            this.lbtnStresstests.ActiveLinkColor = System.Drawing.Color.Black;
            this.lbtnStresstests.AutoSize = true;
            this.lbtnStresstests.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbtnStresstests.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnStresstests.ForeColor = System.Drawing.Color.Black;
            this.lbtnStresstests.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbtnStresstests.LinkColor = System.Drawing.Color.Black;
            this.lbtnStresstests.Location = new System.Drawing.Point(118, 9);
            this.lbtnStresstests.Margin = new System.Windows.Forms.Padding(5, 6, 0, 3);
            this.lbtnStresstests.Name = "lbtnStresstests";
            this.lbtnStresstests.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnStresstests.RadioButtonBehavior = true;
            this.lbtnStresstests.Size = new System.Drawing.Size(103, 22);
            this.lbtnStresstests.TabIndex = 0;
            this.lbtnStresstests.TabStop = true;
            this.lbtnStresstests.Text = "The Stresstests";
            this.lbtnStresstests.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnStresstests.Visible = false;
            this.lbtnStresstests.VisitedLinkColor = System.Drawing.Color.Black;
            // 
            // flpFastMetrics
            // 
            this.flpFastMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpFastMetrics.Controls.Add(this.label1);
            this.flpFastMetrics.Controls.Add(this.pnlBorder);
            this.flpFastMetrics.Controls.Add(this.lblStarted);
            this.flpFastMetrics.Controls.Add(this.lblMeasuredRuntime);
            this.flpFastMetrics.Controls.Add(this.lblStopped);
            this.flpFastMetrics.Controls.Add(this.chkReadable);
            this.flpFastMetrics.Controls.Add(this.btnSaveDisplayedResults);
            this.flpFastMetrics.Location = new System.Drawing.Point(-1, 40);
            this.flpFastMetrics.Name = "flpFastMetrics";
            this.flpFastMetrics.Size = new System.Drawing.Size(939, 60);
            this.flpFastMetrics.TabIndex = 0;
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
            // pnlBorder
            // 
            this.pnlBorder.BackColor = System.Drawing.Color.Silver;
            this.pnlBorder.Controls.Add(this.cboDrillDown);
            this.pnlBorder.Location = new System.Drawing.Point(86, 3);
            this.pnlBorder.Name = "pnlBorder";
            this.pnlBorder.Size = new System.Drawing.Size(127, 23);
            this.pnlBorder.TabIndex = 0;
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
            // lblStopped
            // 
            this.lblStopped.AutoSize = true;
            this.lblStopped.BackColor = System.Drawing.SystemColors.Control;
            this.lblStopped.Font = new System.Drawing.Font("Consolas", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStopped.Location = new System.Drawing.Point(222, 3);
            this.lblStopped.Margin = new System.Windows.Forms.Padding(6, 3, 0, 0);
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
            this.chkReadable.Location = new System.Drawing.Point(225, 7);
            this.chkReadable.Margin = new System.Windows.Forms.Padding(3, 7, 0, 3);
            this.chkReadable.Name = "chkReadable";
            this.chkReadable.Size = new System.Drawing.Size(69, 17);
            this.chkReadable.TabIndex = 4;
            this.chkReadable.Text = "Readable";
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
            this.btnSaveDisplayedResults.Location = new System.Drawing.Point(297, 3);
            this.btnSaveDisplayedResults.MaximumSize = new System.Drawing.Size(183, 24);
            this.btnSaveDisplayedResults.Name = "btnSaveDisplayedResults";
            this.btnSaveDisplayedResults.Size = new System.Drawing.Size(183, 24);
            this.btnSaveDisplayedResults.TabIndex = 2;
            this.btnSaveDisplayedResults.Text = "Save All Displayed Results...";
            this.btnSaveDisplayedResults.UseVisualStyleBackColor = false;
            this.btnSaveDisplayedResults.Click += new System.EventHandler(this.btnSaveDisplayedResults_Click);
            // 
            // eventView
            // 
            this.eventView.BackColor = System.Drawing.Color.White;
            this.eventView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventView.Location = new System.Drawing.Point(0, 61);
            this.eventView.Name = "eventView";
            this.eventView.Size = new System.Drawing.Size(937, 135);
            this.eventView.TabIndex = 2;
            // 
            // flpMetricsMaster
            // 
            this.flpMetricsMaster.BackColor = System.Drawing.Color.White;
            this.flpMetricsMaster.Controls.Add(this.label5);
            this.flpMetricsMaster.Controls.Add(this.kvpRunningTests);
            this.flpMetricsMaster.Controls.Add(this.kvpOK);
            this.flpMetricsMaster.Controls.Add(this.kvpCancelled);
            this.flpMetricsMaster.Controls.Add(this.kvpFailed);
            this.flpMetricsMaster.Controls.Add(this.kvmMasterCPUUsage);
            this.flpMetricsMaster.Controls.Add(this.kvmMasterMemoryUsage);
            this.flpMetricsMaster.Controls.Add(this.kvmMasterNic);
            this.flpMetricsMaster.Controls.Add(this.kvmMasterNicsSent);
            this.flpMetricsMaster.Controls.Add(this.kvmMasterNicsReceived);
            this.flpMetricsMaster.Controls.Add(this.btnMasterExportMessages);
            this.flpMetricsMaster.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpMetricsMaster.Location = new System.Drawing.Point(0, 0);
            this.flpMetricsMaster.Margin = new System.Windows.Forms.Padding(0);
            this.flpMetricsMaster.Name = "flpMetricsMaster";
            this.flpMetricsMaster.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.flpMetricsMaster.Size = new System.Drawing.Size(937, 61);
            this.flpMetricsMaster.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(3, 6);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(312, 20);
            this.label5.TabIndex = 19;
            this.label5.Text = "Client Monitoring";
            // 
            // kvpRunningTests
            // 
            this.kvpRunningTests.BackColor = System.Drawing.Color.LightSteelBlue;
            this.kvpRunningTests.Key = "Running Tests";
            this.kvpRunningTests.Location = new System.Drawing.Point(324, 9);
            this.kvpRunningTests.Margin = new System.Windows.Forms.Padding(3, 6, 0, 6);
            this.kvpRunningTests.Name = "kvpRunningTests";
            this.kvpRunningTests.Size = new System.Drawing.Size(108, 16);
            this.kvpRunningTests.TabIndex = 8;
            this.kvpRunningTests.TabStop = false;
            this.kvpRunningTests.Tooltip = "The number of Running Tests.";
            this.kvpRunningTests.Value = "0";
            // 
            // kvpOK
            // 
            this.kvpOK.BackColor = System.Drawing.Color.LightGreen;
            this.kvpOK.Key = "OK";
            this.kvpOK.Location = new System.Drawing.Point(441, 9);
            this.kvpOK.Margin = new System.Windows.Forms.Padding(9, 6, 6, 0);
            this.kvpOK.Name = "kvpOK";
            this.kvpOK.Size = new System.Drawing.Size(43, 16);
            this.kvpOK.TabIndex = 15;
            this.kvpOK.TabStop = false;
            this.kvpOK.Tooltip = "The number of Succesfully Finished Tests.";
            this.kvpOK.Value = "0";
            this.kvpOK.Visible = false;
            // 
            // kvpCancelled
            // 
            this.kvpCancelled.BackColor = System.Drawing.Color.Orange;
            this.kvpCancelled.Key = "Cancelled";
            this.kvpCancelled.Location = new System.Drawing.Point(493, 9);
            this.kvpCancelled.Margin = new System.Windows.Forms.Padding(3, 6, 6, 0);
            this.kvpCancelled.Name = "kvpCancelled";
            this.kvpCancelled.Size = new System.Drawing.Size(82, 16);
            this.kvpCancelled.TabIndex = 16;
            this.kvpCancelled.TabStop = false;
            this.kvpCancelled.Tooltip = "The number of Cancelled Tests.";
            this.kvpCancelled.Value = "0";
            this.kvpCancelled.Visible = false;
            // 
            // kvpFailed
            // 
            this.kvpFailed.BackColor = System.Drawing.Color.OrangeRed;
            this.kvpFailed.Key = "Failed";
            this.kvpFailed.Location = new System.Drawing.Point(584, 9);
            this.kvpFailed.Margin = new System.Windows.Forms.Padding(3, 6, 6, 0);
            this.kvpFailed.Name = "kvpFailed";
            this.kvpFailed.Size = new System.Drawing.Size(60, 16);
            this.kvpFailed.TabIndex = 17;
            this.kvpFailed.TabStop = false;
            this.kvpFailed.Tooltip = "The number of Failed Tests.";
            this.kvpFailed.Value = "0";
            this.kvpFailed.Visible = false;
            // 
            // kvmMasterCPUUsage
            // 
            this.kvmMasterCPUUsage.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmMasterCPUUsage.Key = "CPU";
            this.kvmMasterCPUUsage.Location = new System.Drawing.Point(653, 9);
            this.kvmMasterCPUUsage.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmMasterCPUUsage.Name = "kvmMasterCPUUsage";
            this.kvmMasterCPUUsage.Size = new System.Drawing.Size(105, 16);
            this.kvmMasterCPUUsage.TabIndex = 8;
            this.kvmMasterCPUUsage.TabStop = false;
            this.kvmMasterCPUUsage.Tooltip = "Try to keep this below 60 % to ensure that the client is not the bottleneck.";
            this.kvmMasterCPUUsage.Value = "N/A";
            // 
            // kvmMasterMemoryUsage
            // 
            this.kvmMasterMemoryUsage.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmMasterMemoryUsage.Key = "Memory";
            this.kvmMasterMemoryUsage.Location = new System.Drawing.Point(3, 37);
            this.kvmMasterMemoryUsage.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmMasterMemoryUsage.Name = "kvmMasterMemoryUsage";
            this.kvmMasterMemoryUsage.Size = new System.Drawing.Size(123, 16);
            this.kvmMasterMemoryUsage.TabIndex = 8;
            this.kvmMasterMemoryUsage.TabStop = false;
            this.kvmMasterMemoryUsage.Tooltip = "Make sure you have sufficient memory to ensure that the client is not the bottlen" +
    "eck.";
            this.kvmMasterMemoryUsage.Value = "N/A";
            // 
            // kvmMasterNic
            // 
            this.kvmMasterNic.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmMasterNic.Key = "Nic";
            this.kvmMasterNic.Location = new System.Drawing.Point(129, 37);
            this.kvmMasterNic.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmMasterNic.Name = "kvmMasterNic";
            this.kvmMasterNic.Size = new System.Drawing.Size(139, 16);
            this.kvmMasterNic.TabIndex = 12;
            this.kvmMasterNic.TabStop = false;
            this.kvmMasterNic.Tooltip = "Make sure that the NIC is not the bottleneck (Most used displayed).";
            this.kvmMasterNic.Value = "N/A";
            // 
            // kvmMasterNicsSent
            // 
            this.kvmMasterNicsSent.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmMasterNicsSent.Key = "Tx";
            this.kvmMasterNicsSent.Location = new System.Drawing.Point(129, 37);
            this.kvmMasterNicsSent.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmMasterNicsSent.Name = "kvmMasterNicsSent";
            this.kvmMasterNicsSent.Size = new System.Drawing.Size(139, 16);
            this.kvmMasterNicsSent.TabIndex = 12;
            this.kvmMasterNicsSent.TabStop = false;
            this.kvmMasterNicsSent.Tooltip = "Make sure that the NIC is not the bottleneck (Most used displayed).";
            this.kvmMasterNicsSent.Value = "N/A";
            // 
            // kvmMasterNicsReceived
            // 
            this.kvmMasterNicsReceived.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmMasterNicsReceived.Key = "Rx";
            this.kvmMasterNicsReceived.Location = new System.Drawing.Point(271, 37);
            this.kvmMasterNicsReceived.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmMasterNicsReceived.Name = "kvmMasterNicsReceived";
            this.kvmMasterNicsReceived.Size = new System.Drawing.Size(167, 16);
            this.kvmMasterNicsReceived.TabIndex = 13;
            this.kvmMasterNicsReceived.TabStop = false;
            this.kvmMasterNicsReceived.Tooltip = "Make sure that the NIC is not the bottleneck (Most used displayed).";
            this.kvmMasterNicsReceived.Value = "N/A";
            // 
            // btnMasterExportMessages
            // 
            this.btnMasterExportMessages.AutoSize = true;
            this.btnMasterExportMessages.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMasterExportMessages.BackColor = System.Drawing.SystemColors.Control;
            this.btnMasterExportMessages.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnMasterExportMessages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMasterExportMessages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMasterExportMessages.Location = new System.Drawing.Point(450, 31);
            this.btnMasterExportMessages.Margin = new System.Windows.Forms.Padding(12, 0, 3, 3);
            this.btnMasterExportMessages.MaximumSize = new System.Drawing.Size(127, 24);
            this.btnMasterExportMessages.Name = "btnMasterExportMessages";
            this.btnMasterExportMessages.Size = new System.Drawing.Size(127, 24);
            this.btnMasterExportMessages.TabIndex = 14;
            this.btnMasterExportMessages.Text = "Export Messages...";
            this.btnMasterExportMessages.UseVisualStyleBackColor = false;
            this.btnMasterExportMessages.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // OveralFastResultsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.splitContainer);
            this.Name = "OveralFastResultsControl";
            this.Size = new System.Drawing.Size(937, 700);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFastResults)).EndInit();
            this.pnlFastResultListing.ResumeLayout(false);
            this.flpFastResultsHeader.ResumeLayout(false);
            this.flpFastResultsHeader.PerformLayout();
            this.flpFastMetrics.ResumeLayout(false);
            this.flpFastMetrics.PerformLayout();
            this.pnlBorder.ResumeLayout(false);
            this.flpMetricsMaster.ResumeLayout(false);
            this.flpMetricsMaster.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpMetricsMaster;
        private vApus.Util.KeyValuePairControl kvmMasterCPUUsage;
        private vApus.Util.KeyValuePairControl kvmMasterMemoryUsage;
        private vApus.Util.KeyValuePairControl kvmMasterNic;
        private vApus.Util.KeyValuePairControl kvmMasterNicsSent;
        private vApus.Util.KeyValuePairControl kvmMasterNicsReceived;
        private System.Windows.Forms.Button btnMasterExportMessages;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlFastResultListing;
        private System.Windows.Forms.FlowLayoutPanel flpFastMetrics;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDrillDown;
        private System.Windows.Forms.Label lblStarted;
        private System.Windows.Forms.Label lblMeasuredRuntime;
        private System.Windows.Forms.Label lblStopped;
        private System.Windows.Forms.Button btnSaveDisplayedResults;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel pnlBorder;
        private Util.KeyValuePairControl kvpRunningTests;
        private Util.KeyValuePairControl kvpOK;
        private Util.KeyValuePairControl kvpCancelled;
        private Util.KeyValuePairControl kvpFailed;
        private Util.EventView eventView;
        private System.Windows.Forms.DataGridView dgvFastResults;
        private System.Windows.Forms.CheckBox chkReadable;
        private System.Windows.Forms.FlowLayoutPanel flpFastResultsHeader;
        private System.Windows.Forms.Label lblFastResults;
        private System.Windows.Forms.Label lblUpdatesIn;
        private Util.LinkButton lbtnStresstests;
    }
}
