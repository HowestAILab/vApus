using vApus.Stresstest;
namespace vApus.DistributedTesting
{
    partial class DistributedStresstestControl
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.dgvFastResults = new System.Windows.Forms.DataGridView();
            this.clmA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmG = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmJ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFastResultListing = new System.Windows.Forms.Panel();
            this.flpFastMetrics = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlBorder = new System.Windows.Forms.Panel();
            this.cboDrillDown = new System.Windows.Forms.ComboBox();
            this.lblStarted = new System.Windows.Forms.Label();
            this.lblMeasuredRuntime = new System.Windows.Forms.Label();
            this.lblStopped = new System.Windows.Forms.Label();
            this.btnSaveDisplayedResults = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.flpMetricsMaster = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.kvmMasterCPUUsage = new vApus.Util.KeyValuePairControl();
            this.kvmMasterContextSwitchesPerSecond = new vApus.Util.KeyValuePairControl();
            this.kvmMasterMemoryUsage = new vApus.Util.KeyValuePairControl();
            this.kvmMasterNicsSent = new vApus.Util.KeyValuePairControl();
            this.kvmMasterNicsReceived = new vApus.Util.KeyValuePairControl();
            this.btnMasterExportMessages = new System.Windows.Forms.Button();
            this.epnlMasterMessages = new vApus.Util.EventPanel();
            this.kvmFailed = new vApus.Util.KeyValuePairControl();
            this.kvmCancelled = new vApus.Util.KeyValuePairControl();
            this.kvmOK = new vApus.Util.KeyValuePairControl();
            this.kvmRunningTests = new vApus.Util.KeyValuePairControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFastResults)).BeginInit();
            this.pnlFastResultListing.SuspendLayout();
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
            this.splitContainer.Panel2.Controls.Add(this.flpMetricsMaster);
            this.splitContainer.Panel2.Controls.Add(this.epnlMasterMessages);
            this.splitContainer.Panel2MinSize = 350;
            this.splitContainer.Size = new System.Drawing.Size(937, 700);
            this.splitContainer.SplitterDistance = 346;
            this.splitContainer.TabIndex = 0;
            // 
            // dgvFastResults
            // 
            this.dgvFastResults.AllowUserToAddRows = false;
            this.dgvFastResults.AllowUserToDeleteRows = false;
            this.dgvFastResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFastResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFastResults.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
            this.dgvFastResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFastResults.ColumnHeadersVisible = false;
            this.dgvFastResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmA,
            this.clmB,
            this.clmC,
            this.clmD,
            this.clmE,
            this.clmF,
            this.clmG,
            this.clmH,
            this.clmI,
            this.clmJ,
            this.clmK,
            this.clmL,
            this.clmM});
            this.dgvFastResults.Location = new System.Drawing.Point(0, 100);
            this.dgvFastResults.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.dgvFastResults.Name = "dgvFastResults";
            this.dgvFastResults.ReadOnly = true;
            this.dgvFastResults.Size = new System.Drawing.Size(937, 246);
            this.dgvFastResults.TabIndex = 1;
            // 
            // clmA
            // 
            this.clmA.HeaderText = "A";
            this.clmA.Name = "clmA";
            this.clmA.ReadOnly = true;
            this.clmA.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmB
            // 
            this.clmB.HeaderText = "B";
            this.clmB.Name = "clmB";
            this.clmB.ReadOnly = true;
            this.clmB.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmC
            // 
            this.clmC.HeaderText = "C";
            this.clmC.Name = "clmC";
            this.clmC.ReadOnly = true;
            this.clmC.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmD
            // 
            this.clmD.HeaderText = "D";
            this.clmD.Name = "clmD";
            this.clmD.ReadOnly = true;
            this.clmD.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmE
            // 
            this.clmE.HeaderText = "E";
            this.clmE.Name = "clmE";
            this.clmE.ReadOnly = true;
            this.clmE.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmF
            // 
            this.clmF.HeaderText = "F";
            this.clmF.Name = "clmF";
            this.clmF.ReadOnly = true;
            this.clmF.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmG
            // 
            this.clmG.HeaderText = "G";
            this.clmG.Name = "clmG";
            this.clmG.ReadOnly = true;
            this.clmG.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmH
            // 
            this.clmH.HeaderText = "H";
            this.clmH.Name = "clmH";
            this.clmH.ReadOnly = true;
            this.clmH.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmI
            // 
            this.clmI.HeaderText = "I";
            this.clmI.Name = "clmI";
            this.clmI.ReadOnly = true;
            this.clmI.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmJ
            // 
            this.clmJ.HeaderText = "J";
            this.clmJ.Name = "clmJ";
            this.clmJ.ReadOnly = true;
            this.clmJ.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmK
            // 
            this.clmK.HeaderText = "K";
            this.clmK.Name = "clmK";
            this.clmK.ReadOnly = true;
            this.clmK.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmL
            // 
            this.clmL.HeaderText = "L";
            this.clmL.Name = "clmL";
            this.clmL.ReadOnly = true;
            this.clmL.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmM
            // 
            this.clmM.HeaderText = "M";
            this.clmM.Name = "clmM";
            this.clmM.ReadOnly = true;
            this.clmM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // pnlFastResultListing
            // 
            this.pnlFastResultListing.BackColor = System.Drawing.Color.White;
            this.pnlFastResultListing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFastResultListing.Controls.Add(this.flpFastMetrics);
            this.pnlFastResultListing.Controls.Add(this.label4);
            this.pnlFastResultListing.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFastResultListing.Location = new System.Drawing.Point(0, 0);
            this.pnlFastResultListing.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.pnlFastResultListing.Name = "pnlFastResultListing";
            this.pnlFastResultListing.Size = new System.Drawing.Size(937, 100);
            this.pnlFastResultListing.TabIndex = 0;
            this.pnlFastResultListing.Text = "Fast Results";
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
            this.flpFastMetrics.Controls.Add(this.btnSaveDisplayedResults);
            this.flpFastMetrics.Location = new System.Drawing.Point(-1, 37);
            this.flpFastMetrics.Name = "flpFastMetrics";
            this.flpFastMetrics.Size = new System.Drawing.Size(937, 50);
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
            "Concurrent Users",
            "Precision",
            "Run"});
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
            // btnSaveDisplayedResults
            // 
            this.btnSaveDisplayedResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveDisplayedResults.AutoSize = true;
            this.btnSaveDisplayedResults.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveDisplayedResults.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveDisplayedResults.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnSaveDisplayedResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveDisplayedResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveDisplayedResults.Location = new System.Drawing.Point(225, 3);
            this.btnSaveDisplayedResults.MaximumSize = new System.Drawing.Size(183, 24);
            this.btnSaveDisplayedResults.Name = "btnSaveDisplayedResults";
            this.btnSaveDisplayedResults.Size = new System.Drawing.Size(183, 24);
            this.btnSaveDisplayedResults.TabIndex = 1;
            this.btnSaveDisplayedResults.Text = "Save All Displayed Results...";
            this.btnSaveDisplayedResults.UseVisualStyleBackColor = false;
            this.btnSaveDisplayedResults.Click += new System.EventHandler(this.btnSaveDisplayedResults_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(2, 7);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "Fast Results Listing";
            // 
            // flpMetricsMaster
            // 
            this.flpMetricsMaster.BackColor = System.Drawing.Color.White;
            this.flpMetricsMaster.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpMetricsMaster.Controls.Add(this.label5);
            this.flpMetricsMaster.Controls.Add(this.kvmRunningTests);
            this.flpMetricsMaster.Controls.Add(this.kvmOK);
            this.flpMetricsMaster.Controls.Add(this.kvmCancelled);
            this.flpMetricsMaster.Controls.Add(this.kvmFailed);
            this.flpMetricsMaster.Controls.Add(this.kvmMasterCPUUsage);
            this.flpMetricsMaster.Controls.Add(this.kvmMasterContextSwitchesPerSecond);
            this.flpMetricsMaster.Controls.Add(this.kvmMasterMemoryUsage);
            this.flpMetricsMaster.Controls.Add(this.kvmMasterNicsSent);
            this.flpMetricsMaster.Controls.Add(this.kvmMasterNicsReceived);
            this.flpMetricsMaster.Controls.Add(this.btnMasterExportMessages);
            this.flpMetricsMaster.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpMetricsMaster.Location = new System.Drawing.Point(0, 0);
            this.flpMetricsMaster.Margin = new System.Windows.Forms.Padding(0);
            this.flpMetricsMaster.Name = "flpMetricsMaster";
            this.flpMetricsMaster.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.flpMetricsMaster.Size = new System.Drawing.Size(937, 80);
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
            this.label5.Size = new System.Drawing.Size(188, 20);
            this.label5.TabIndex = 19;
            this.label5.Text = "Distributed Test Progress";
            // 
            // kvmMasterCPUUsage
            // 
            this.kvmMasterCPUUsage.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmMasterCPUUsage.Key = "CPU Usage";
            this.kvmMasterCPUUsage.Location = new System.Drawing.Point(529, 9);
            this.kvmMasterCPUUsage.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmMasterCPUUsage.Name = "kvmMasterCPUUsage";
            this.kvmMasterCPUUsage.Size = new System.Drawing.Size(105, 16);
            this.kvmMasterCPUUsage.TabIndex = 8;
            this.kvmMasterCPUUsage.TabStop = false;
            this.kvmMasterCPUUsage.Tooltip = "Try to keep this below 60 % to ensure that the client is not the bottleneck.";
            this.kvmMasterCPUUsage.Value = "N/A";
            // 
            // kvmMasterContextSwitchesPerSecond
            // 
            this.kvmMasterContextSwitchesPerSecond.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmMasterContextSwitchesPerSecond.Key = "Context Switches / s";
            this.kvmMasterContextSwitchesPerSecond.Location = new System.Drawing.Point(637, 9);
            this.kvmMasterContextSwitchesPerSecond.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmMasterContextSwitchesPerSecond.Name = "kvmMasterContextSwitchesPerSecond";
            this.kvmMasterContextSwitchesPerSecond.Size = new System.Drawing.Size(158, 16);
            this.kvmMasterContextSwitchesPerSecond.TabIndex = 8;
            this.kvmMasterContextSwitchesPerSecond.TabStop = false;
            this.kvmMasterContextSwitchesPerSecond.Tooltip = "";
            this.kvmMasterContextSwitchesPerSecond.Value = "N/A";
            // 
            // kvmMasterMemoryUsage
            // 
            this.kvmMasterMemoryUsage.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmMasterMemoryUsage.Key = "Memory Usage";
            this.kvmMasterMemoryUsage.Location = new System.Drawing.Point(798, 9);
            this.kvmMasterMemoryUsage.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.kvmMasterMemoryUsage.Name = "kvmMasterMemoryUsage";
            this.kvmMasterMemoryUsage.Size = new System.Drawing.Size(123, 16);
            this.kvmMasterMemoryUsage.TabIndex = 8;
            this.kvmMasterMemoryUsage.TabStop = false;
            this.kvmMasterMemoryUsage.Tooltip = "Make sure you have sufficient memory to ensure that the client is not the bottlen" +
    "eck.";
            this.kvmMasterMemoryUsage.Value = "N/A";
            // 
            // kvmMasterNicsSent
            // 
            this.kvmMasterNicsSent.BackColor = System.Drawing.Color.GhostWhite;
            this.kvmMasterNicsSent.Key = "NIC Usage (Sent)";
            this.kvmMasterNicsSent.Location = new System.Drawing.Point(3, 44);
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
            this.kvmMasterNicsReceived.Key = "NIC Usage (Received)";
            this.kvmMasterNicsReceived.Location = new System.Drawing.Point(145, 44);
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
            this.btnMasterExportMessages.Location = new System.Drawing.Point(324, 38);
            this.btnMasterExportMessages.Margin = new System.Windows.Forms.Padding(12, 0, 3, 3);
            this.btnMasterExportMessages.MaximumSize = new System.Drawing.Size(127, 24);
            this.btnMasterExportMessages.Name = "btnMasterExportMessages";
            this.btnMasterExportMessages.Size = new System.Drawing.Size(127, 24);
            this.btnMasterExportMessages.TabIndex = 14;
            this.btnMasterExportMessages.Text = "Export Messages...";
            this.btnMasterExportMessages.UseVisualStyleBackColor = false;
            this.btnMasterExportMessages.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // epnlMasterMessages
            // 
            this.epnlMasterMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.epnlMasterMessages.BackColor = System.Drawing.SystemColors.Control;
            this.epnlMasterMessages.BeginOfTimeFrame = new System.DateTime(((long)(0)));
            this.epnlMasterMessages.Collapsed = false;
            this.epnlMasterMessages.Cursor = System.Windows.Forms.Cursors.Default;
            this.epnlMasterMessages.EndOfTimeFrame = new System.DateTime(9999, 12, 31, 23, 59, 59, 999);
            this.epnlMasterMessages.ExpandOnErrorEvent = false;
            this.epnlMasterMessages.Location = new System.Drawing.Point(0, 80);
            this.epnlMasterMessages.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.epnlMasterMessages.Name = "epnlMasterMessages";
            this.epnlMasterMessages.ProgressBarColor = System.Drawing.Color.SteelBlue;
            this.epnlMasterMessages.Size = new System.Drawing.Size(937, 271);
            this.epnlMasterMessages.TabIndex = 4;
            // 
            // kvmFailed
            // 
            this.kvmFailed.BackColor = System.Drawing.Color.OrangeRed;
            this.kvmFailed.Key = "Failed";
            this.kvmFailed.Location = new System.Drawing.Point(460, 9);
            this.kvmFailed.Margin = new System.Windows.Forms.Padding(3, 6, 6, 0);
            this.kvmFailed.Name = "kvmFailed";
            this.kvmFailed.Size = new System.Drawing.Size(60, 23);
            this.kvmFailed.TabIndex = 17;
            this.kvmFailed.TabStop = false;
            this.kvmFailed.Tooltip = "The number of Failed Tests.";
            this.kvmFailed.Value = "0";
            this.kvmFailed.Visible = false;
            // 
            // kvmCancelled
            // 
            this.kvmCancelled.BackColor = System.Drawing.Color.Orange;
            this.kvmCancelled.Key = "Cancelled";
            this.kvmCancelled.Location = new System.Drawing.Point(369, 9);
            this.kvmCancelled.Margin = new System.Windows.Forms.Padding(3, 6, 6, 0);
            this.kvmCancelled.Name = "kvmCancelled";
            this.kvmCancelled.Size = new System.Drawing.Size(82, 23);
            this.kvmCancelled.TabIndex = 16;
            this.kvmCancelled.TabStop = false;
            this.kvmCancelled.Tooltip = "The number of Cancelled Tests.";
            this.kvmCancelled.Value = "0";
            this.kvmCancelled.Visible = false;
            // 
            // kvmOK
            // 
            this.kvmOK.BackColor = System.Drawing.Color.LightGreen;
            this.kvmOK.Key = "OK";
            this.kvmOK.Location = new System.Drawing.Point(317, 9);
            this.kvmOK.Margin = new System.Windows.Forms.Padding(9, 6, 6, 0);
            this.kvmOK.Name = "kvmOK";
            this.kvmOK.Size = new System.Drawing.Size(43, 23);
            this.kvmOK.TabIndex = 15;
            this.kvmOK.TabStop = false;
            this.kvmOK.Tooltip = "The number of Succesfully Finished Tests.";
            this.kvmOK.Value = "0";
            this.kvmOK.Visible = false;
            // 
            // kvmRunningTests
            // 
            this.kvmRunningTests.BackColor = System.Drawing.Color.LightSteelBlue;
            this.kvmRunningTests.Key = "Running Tests";
            this.kvmRunningTests.Location = new System.Drawing.Point(200, 9);
            this.kvmRunningTests.Margin = new System.Windows.Forms.Padding(3, 6, 0, 6);
            this.kvmRunningTests.Name = "kvmRunningTests";
            this.kvmRunningTests.Size = new System.Drawing.Size(108, 23);
            this.kvmRunningTests.TabIndex = 8;
            this.kvmRunningTests.TabStop = false;
            this.kvmRunningTests.Tooltip = "The number of Running Tests.";
            this.kvmRunningTests.Value = "0";
            // 
            // DistributedStresstestControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "DistributedStresstestControl";
            this.Size = new System.Drawing.Size(937, 700);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFastResults)).EndInit();
            this.pnlFastResultListing.ResumeLayout(false);
            this.pnlFastResultListing.PerformLayout();
            this.flpFastMetrics.ResumeLayout(false);
            this.flpFastMetrics.PerformLayout();
            this.pnlBorder.ResumeLayout(false);
            this.flpMetricsMaster.ResumeLayout(false);
            this.flpMetricsMaster.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Util.EventPanel epnlMasterMessages;
        private System.Windows.Forms.FlowLayoutPanel flpMetricsMaster;
        private vApus.Util.KeyValuePairControl kvmMasterCPUUsage;
        private vApus.Util.KeyValuePairControl kvmMasterContextSwitchesPerSecond;
        private vApus.Util.KeyValuePairControl kvmMasterMemoryUsage;
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
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSaveDisplayedResults;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.DataGridView dgvFastResults;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmA;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmB;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmC;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmD;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmE;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmF;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmG;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmH;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmI;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmJ;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmK;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmL;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmM;
        private System.Windows.Forms.Panel pnlBorder;
        private Util.KeyValuePairControl kvmRunningTests;
        private Util.KeyValuePairControl kvmOK;
        private Util.KeyValuePairControl kvmCancelled;
        private Util.KeyValuePairControl kvmFailed;
    }
}
