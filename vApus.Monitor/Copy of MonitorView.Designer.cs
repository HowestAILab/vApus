namespace vApus.Monitor
{
    partial class MonitorViewCopy
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
            //Make sure to dispose.
            try
            {
                if (_monitorProxy != null)
                    _monitorProxy.Dispose();
            }
            catch { }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorView));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnSchedule = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.btnLocalOrRemoteSMT = new System.Windows.Forms.ToolStripButton();
            this.tmrProgressDelayCountDown = new System.Windows.Forms.Timer(this.components);
            this.tpConfigure = new System.Windows.Forms.TabPage();
            this.split = new System.Windows.Forms.SplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.parameterPanel = new vApus.Monitor.ParameterPanel();
            this.propertyPanel = new vApus.SolutionTree.SolutionComponentPropertyPanel();
            this.btnConfiguration = new System.Windows.Forms.Button();
            this.btnGetCounters = new System.Windows.Forms.Button();
            this.lblMonitorSourceMismatch = new System.Windows.Forms.Label();
            this.pnlEntitiesAndCounters = new System.Windows.Forms.Panel();
            this.tcCounters = new vApus.Util.TabControlWithAdjustableBorders();
            this.tpCounters = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.lvwEntities = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tvwCounters = new System.Windows.Forms.TreeView();
            this.btnFilter = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.tpCountersInGui = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.lvwEntitiesInGui = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tvwCountersInGui = new System.Windows.Forms.TreeView();
            this.lblLiveMonitoring = new System.Windows.Forms.Label();
            this.btnMonitorReady = new System.Windows.Forms.Button();
            this.tpMonitor = new System.Windows.Forms.TabPage();
            this.dgvLiveMonitoring = new vApus.Monitor.MonitorControl();
            this.lblCountDown = new System.Windows.Forms.Label();
            this.tc = new vApus.Util.TabControlWithAdjustableBorders();
            this.tmrSchedule = new System.Windows.Forms.Timer(this.components);
            this.tmrBatchSaveResults = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip.SuspendLayout();
            this.tpConfigure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.pnlEntitiesAndCounters.SuspendLayout();
            this.tcCounters.SuspendLayout();
            this.tpCounters.SuspendLayout();
            this.tpCountersInGui.SuspendLayout();
            this.tpMonitor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLiveMonitoring)).BeginInit();
            this.tc.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStart,
            this.btnSchedule,
            this.btnStop,
            this.btnLocalOrRemoteSMT});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.MinimumSize = new System.Drawing.Size(0, 40);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1008, 40);
            this.toolStrip.TabIndex = 0;
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
            this.btnSchedule.Enabled = false;
            this.btnSchedule.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSchedule.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSchedule.Margin = new System.Windows.Forms.Padding(-9, 1, 0, 2);
            this.btnSchedule.Name = "btnSchedule";
            this.btnSchedule.Size = new System.Drawing.Size(68, 37);
            this.btnSchedule.Text = "Schedule...";
            this.btnSchedule.TextAlign = System.Drawing.ContentAlignment.BottomRight;
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
            // btnLocalOrRemoteSMT
            // 
            this.btnLocalOrRemoteSMT.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnLocalOrRemoteSMT.Image = ((System.Drawing.Image)(resources.GetObject("btnLocalOrRemoteSMT.Image")));
            this.btnLocalOrRemoteSMT.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnLocalOrRemoteSMT.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLocalOrRemoteSMT.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.btnLocalOrRemoteSMT.Name = "btnLocalOrRemoteSMT";
            this.btnLocalOrRemoteSMT.Size = new System.Drawing.Size(98, 37);
            this.btnLocalOrRemoteSMT.Text = "SMT: <local>";
            this.btnLocalOrRemoteSMT.ToolTipText = "Use the local server monitoring binaries or connect to a remote vApus Server Moni" +
    "toring Tool running somewhere in the network.";
            this.btnLocalOrRemoteSMT.Click += new System.EventHandler(this.btnLocalOrRemoteSMT_Click);
            // 
            // tmrProgressDelayCountDown
            // 
            this.tmrProgressDelayCountDown.Interval = 1000;
            this.tmrProgressDelayCountDown.Tick += new System.EventHandler(this.tmrProgressDelayCountDown_Tick);
            // 
            // tpConfigure
            // 
            this.tpConfigure.Controls.Add(this.split);
            this.tpConfigure.Location = new System.Drawing.Point(0, 22);
            this.tpConfigure.Name = "tpConfigure";
            this.tpConfigure.Padding = new System.Windows.Forms.Padding(3);
            this.tpConfigure.Size = new System.Drawing.Size(1007, 695);
            this.tpConfigure.TabIndex = 0;
            this.tpConfigure.Text = "Configure";
            this.tpConfigure.UseVisualStyleBackColor = true;
            // 
            // split
            // 
            this.split.BackColor = System.Drawing.Color.LightGray;
            this.split.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.Location = new System.Drawing.Point(3, 3);
            this.split.Name = "split";
            this.split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split.Panel1
            // 
            this.split.Panel1.BackColor = System.Drawing.Color.White;
            this.split.Panel1.Controls.Add(this.label4);
            this.split.Panel1.Controls.Add(this.parameterPanel);
            this.split.Panel1.Controls.Add(this.propertyPanel);
            this.split.Panel1.Controls.Add(this.btnConfiguration);
            this.split.Panel1.Controls.Add(this.btnGetCounters);
            // 
            // split.Panel2
            // 
            this.split.Panel2.BackColor = System.Drawing.Color.White;
            this.split.Panel2.Controls.Add(this.lblMonitorSourceMismatch);
            this.split.Panel2.Controls.Add(this.pnlEntitiesAndCounters);
            this.split.Size = new System.Drawing.Size(1001, 689);
            this.split.SplitterDistance = 344;
            this.split.SplitterWidth = 2;
            this.split.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(597, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(160, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Monitor Source Parameters";
            // 
            // parameterPanel
            // 
            this.parameterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.parameterPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.parameterPanel.Location = new System.Drawing.Point(597, 23);
            this.parameterPanel.Name = "parameterPanel";
            this.parameterPanel.ParametersWithValues = null;
            this.parameterPanel.Size = new System.Drawing.Size(404, 291);
            this.parameterPanel.TabIndex = 3;
            this.parameterPanel.ParameterValueChanged += new System.EventHandler(this.parameterPanel_ParameterValueChanged);
            // 
            // propertyPanel
            // 
            this.propertyPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyPanel.BackColor = System.Drawing.Color.Transparent;
            this.propertyPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.propertyPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyPanel.Location = new System.Drawing.Point(3, 3);
            this.propertyPanel.Margin = new System.Windows.Forms.Padding(0);
            this.propertyPanel.Name = "propertyPanel";
            this.propertyPanel.Size = new System.Drawing.Size(594, 311);
            this.propertyPanel.SolutionComponent = null;
            this.propertyPanel.TabIndex = 0;
            // 
            // btnConfiguration
            // 
            this.btnConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConfiguration.AutoSize = true;
            this.btnConfiguration.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnConfiguration.BackColor = System.Drawing.SystemColors.Control;
            this.btnConfiguration.Enabled = false;
            this.btnConfiguration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfiguration.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfiguration.Location = new System.Drawing.Point(262, 317);
            this.btnConfiguration.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnConfiguration.MaximumSize = new System.Drawing.Size(94, 24);
            this.btnConfiguration.Name = "btnConfiguration";
            this.btnConfiguration.Size = new System.Drawing.Size(94, 24);
            this.btnConfiguration.TabIndex = 2;
            this.btnConfiguration.Text = "Configuration";
            this.btnConfiguration.UseVisualStyleBackColor = false;
            this.btnConfiguration.Click += new System.EventHandler(this.btnConfiguration_Click);
            // 
            // btnGetCounters
            // 
            this.btnGetCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGetCounters.BackColor = System.Drawing.SystemColors.Control;
            this.btnGetCounters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetCounters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetCounters.Location = new System.Drawing.Point(3, 317);
            this.btnGetCounters.Name = "btnGetCounters";
            this.btnGetCounters.Size = new System.Drawing.Size(253, 24);
            this.btnGetCounters.TabIndex = 1;
            this.btnGetCounters.Text = "Get Counters";
            this.btnGetCounters.UseVisualStyleBackColor = false;
            this.btnGetCounters.Click += new System.EventHandler(this.btnGetCounters_Click);
            // 
            // lblMonitorSourceMismatch
            // 
            this.lblMonitorSourceMismatch.AutoSize = true;
            this.lblMonitorSourceMismatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblMonitorSourceMismatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMonitorSourceMismatch.Location = new System.Drawing.Point(219, 4);
            this.lblMonitorSourceMismatch.Name = "lblMonitorSourceMismatch";
            this.lblMonitorSourceMismatch.Size = new System.Drawing.Size(655, 13);
            this.lblMonitorSourceMismatch.TabIndex = 9;
            this.lblMonitorSourceMismatch.Text = "These counters are not valid for the chosen monitor source, undo your previous ac" +
    "tion or \'get\' the counters again.";
            this.lblMonitorSourceMismatch.Visible = false;
            // 
            // pnlEntitiesAndCounters
            // 
            this.pnlEntitiesAndCounters.BackColor = System.Drawing.SystemColors.Control;
            this.pnlEntitiesAndCounters.Controls.Add(this.tcCounters);
            this.pnlEntitiesAndCounters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEntitiesAndCounters.Location = new System.Drawing.Point(0, 0);
            this.pnlEntitiesAndCounters.Name = "pnlEntitiesAndCounters";
            this.pnlEntitiesAndCounters.Size = new System.Drawing.Size(1001, 343);
            this.pnlEntitiesAndCounters.TabIndex = 0;
            // 
            // tcCounters
            // 
            this.tcCounters.BottomVisible = true;
            this.tcCounters.Controls.Add(this.tpCounters);
            this.tcCounters.Controls.Add(this.tpCountersInGui);
            this.tcCounters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcCounters.LeftVisible = true;
            this.tcCounters.Location = new System.Drawing.Point(0, 0);
            this.tcCounters.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.tcCounters.Name = "tcCounters";
            this.tcCounters.RightVisible = true;
            this.tcCounters.SelectedIndex = 0;
            this.tcCounters.Size = new System.Drawing.Size(1001, 343);
            this.tcCounters.TabIndex = 7;
            this.tcCounters.TopVisible = true;
            // 
            // tpCounters
            // 
            this.tpCounters.Controls.Add(this.label2);
            this.tpCounters.Controls.Add(this.lvwEntities);
            this.tpCounters.Controls.Add(this.tvwCounters);
            this.tpCounters.Controls.Add(this.btnFilter);
            this.tpCounters.Controls.Add(this.label1);
            this.tpCounters.Controls.Add(this.txtFilter);
            this.tpCounters.Location = new System.Drawing.Point(4, 22);
            this.tpCounters.Name = "tpCounters";
            this.tpCounters.Padding = new System.Windows.Forms.Padding(3);
            this.tpCounters.Size = new System.Drawing.Size(993, 317);
            this.tpCounters.TabIndex = 0;
            this.tpCounters.Text = "Choose Counters [0]";
            this.tpCounters.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(47, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Entities";
            // 
            // lvwEntities
            // 
            this.lvwEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvwEntities.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwEntities.CheckBoxes = true;
            this.lvwEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvwEntities.FullRowSelect = true;
            this.lvwEntities.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwEntities.HideSelection = false;
            this.lvwEntities.Location = new System.Drawing.Point(3, 4);
            this.lvwEntities.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lvwEntities.MultiSelect = false;
            this.lvwEntities.Name = "lvwEntities";
            this.lvwEntities.Size = new System.Drawing.Size(349, 307);
            this.lvwEntities.SmallImageList = this.imageList;
            this.lvwEntities.TabIndex = 0;
            this.lvwEntities.UseCompatibleStateImageBehavior = false;
            this.lvwEntities.View = System.Windows.Forms.View.Details;
            this.lvwEntities.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvwEntities_ItemChecked);
            this.lvwEntities.SelectedIndexChanged += new System.EventHandler(this.lvwEntities_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 40;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "";
            this.columnHeader2.Width = 195;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Chosen Counters";
            this.columnHeader3.Width = 105;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Online.png");
            this.imageList.Images.SetKeyName(1, "Offline.png");
            this.imageList.Images.SetKeyName(2, "Suspended.png");
            // 
            // tvwCounters
            // 
            this.tvwCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwCounters.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvwCounters.CheckBoxes = true;
            this.tvwCounters.Location = new System.Drawing.Point(355, 33);
            this.tvwCounters.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.tvwCounters.Name = "tvwCounters";
            this.tvwCounters.Size = new System.Drawing.Size(631, 278);
            this.tvwCounters.TabIndex = 1;
            this.tvwCounters.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvwCounter_AfterCheck);
            this.tvwCounters.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvwCounter_BeforeExpand);
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilter.AutoSize = true;
            this.btnFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFilter.BackColor = System.Drawing.SystemColors.Control;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilter.Location = new System.Drawing.Point(955, 4);
            this.btnFilter.MaximumSize = new System.Drawing.Size(31, 23);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(31, 23);
            this.btnFilter.TabIndex = 6;
            this.btnFilter.Text = "...";
            this.toolTip.SetToolTip(this.btnFilter, "To filter the counters in a (large) counter collection. Wild card * can be used. " +
        "Not case sensitive. All entries are in OR-relation with each other.");
            this.btnFilter.UseVisualStyleBackColor = false;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(358, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Filter:";
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.BackColor = System.Drawing.Color.White;
            this.txtFilter.HideSelection = false;
            this.txtFilter.Location = new System.Drawing.Point(403, 6);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.ReadOnly = true;
            this.txtFilter.Size = new System.Drawing.Size(546, 20);
            this.txtFilter.TabIndex = 4;
            this.toolTip.SetToolTip(this.txtFilter, "To filter the counters in a (large) counter collection. Wild card * can be used. " +
        "Not case sensitive. All entries are in OR-relation with each other.");
            // 
            // tpCountersInGui
            // 
            this.tpCountersInGui.Controls.Add(this.label3);
            this.tpCountersInGui.Controls.Add(this.lvwEntitiesInGui);
            this.tpCountersInGui.Controls.Add(this.tvwCountersInGui);
            this.tpCountersInGui.Controls.Add(this.lblLiveMonitoring);
            this.tpCountersInGui.Location = new System.Drawing.Point(4, 22);
            this.tpCountersInGui.Name = "tpCountersInGui";
            this.tpCountersInGui.Padding = new System.Windows.Forms.Padding(3);
            this.tpCountersInGui.Size = new System.Drawing.Size(993, 317);
            this.tpCountersInGui.TabIndex = 1;
            this.tpCountersInGui.Text = "Live Monitoring [0]";
            this.tpCountersInGui.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(47, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Entities";
            // 
            // lvwEntitiesInGui
            // 
            this.lvwEntitiesInGui.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvwEntitiesInGui.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwEntitiesInGui.CheckBoxes = true;
            this.lvwEntitiesInGui.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.lvwEntitiesInGui.FullRowSelect = true;
            this.lvwEntitiesInGui.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwEntitiesInGui.HideSelection = false;
            this.lvwEntitiesInGui.Location = new System.Drawing.Point(5, 5);
            this.lvwEntitiesInGui.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lvwEntitiesInGui.MultiSelect = false;
            this.lvwEntitiesInGui.Name = "lvwEntitiesInGui";
            this.lvwEntitiesInGui.Size = new System.Drawing.Size(349, 307);
            this.lvwEntitiesInGui.SmallImageList = this.imageList;
            this.lvwEntitiesInGui.TabIndex = 4;
            this.lvwEntitiesInGui.UseCompatibleStateImageBehavior = false;
            this.lvwEntitiesInGui.View = System.Windows.Forms.View.Details;
            this.lvwEntitiesInGui.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvwEntities_ItemChecked);
            this.lvwEntitiesInGui.SelectedIndexChanged += new System.EventHandler(this.lvwEntities_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "";
            this.columnHeader4.Width = 40;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "";
            this.columnHeader5.Width = 195;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Chosen Counters";
            this.columnHeader6.Width = 105;
            // 
            // tvwCountersInGui
            // 
            this.tvwCountersInGui.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwCountersInGui.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvwCountersInGui.CheckBoxes = true;
            this.tvwCountersInGui.Location = new System.Drawing.Point(357, 34);
            this.tvwCountersInGui.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.tvwCountersInGui.Name = "tvwCountersInGui";
            this.tvwCountersInGui.Size = new System.Drawing.Size(631, 278);
            this.tvwCountersInGui.TabIndex = 5;
            this.tvwCountersInGui.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvwCounter_AfterCheck);
            this.tvwCountersInGui.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvwCounter_BeforeExpand);
            // 
            // lblLiveMonitoring
            // 
            this.lblLiveMonitoring.AutoSize = true;
            this.lblLiveMonitoring.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLiveMonitoring.Location = new System.Drawing.Point(360, 9);
            this.lblLiveMonitoring.Name = "lblLiveMonitoring";
            this.lblLiveMonitoring.Size = new System.Drawing.Size(462, 13);
            this.lblLiveMonitoring.TabIndex = 3;
            this.lblLiveMonitoring.Text = "You can follow a selection of the chosen counters live in the \'Monitor\' tab page." +
    "";
            // 
            // btnMonitorReady
            // 
            this.btnMonitorReady.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMonitorReady.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMonitorReady.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnMonitorReady.Enabled = false;
            this.btnMonitorReady.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMonitorReady.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMonitorReady.Location = new System.Drawing.Point(265, 7);
            this.btnMonitorReady.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnMonitorReady.Name = "btnMonitorReady";
            this.btnMonitorReady.Size = new System.Drawing.Size(638, 24);
            this.btnMonitorReady.TabIndex = 3;
            this.btnMonitorReady.Text = "Click Here when Ready";
            this.btnMonitorReady.UseVisualStyleBackColor = false;
            this.btnMonitorReady.Visible = false;
            this.btnMonitorReady.Click += new System.EventHandler(this.btnMonitorReady_Click);
            // 
            // tpMonitor
            // 
            this.tpMonitor.BackColor = System.Drawing.Color.White;
            this.tpMonitor.Controls.Add(this.dgvLiveMonitoring);
            this.tpMonitor.Controls.Add(this.lblCountDown);
            this.tpMonitor.Location = new System.Drawing.Point(0, 22);
            this.tpMonitor.Name = "tpMonitor";
            this.tpMonitor.Padding = new System.Windows.Forms.Padding(3);
            this.tpMonitor.Size = new System.Drawing.Size(1007, 695);
            this.tpMonitor.TabIndex = 1;
            this.tpMonitor.Text = "Monitor";
            // 
            // dgvLiveMonitoring
            // 
            this.dgvLiveMonitoring.AllowUserToAddRows = false;
            this.dgvLiveMonitoring.AllowUserToDeleteRows = false;
            this.dgvLiveMonitoring.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLiveMonitoring.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLiveMonitoring.Location = new System.Drawing.Point(6, 6);
            this.dgvLiveMonitoring.Name = "dgvLiveMonitoring";
            this.dgvLiveMonitoring.Size = new System.Drawing.Size(995, 661);
            this.dgvLiveMonitoring.TabIndex = 3;
            this.dgvLiveMonitoring.WiwInGui = ((System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<vApusSMT.Base.CounterInfo>>)(resources.GetObject("dgvLiveMonitoring.WiwInGui")));
            // 
            // lblCountDown
            // 
            this.lblCountDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCountDown.AutoSize = true;
            this.lblCountDown.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblCountDown.Location = new System.Drawing.Point(6, 670);
            this.lblCountDown.Name = "lblCountDown";
            this.lblCountDown.Size = new System.Drawing.Size(192, 18);
            this.lblCountDown.TabIndex = 2;
            this.lblCountDown.Text = "Determining Countdown: ";
            this.lblCountDown.Visible = false;
            // 
            // tc
            // 
            this.tc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tc.BottomVisible = false;
            this.tc.Controls.Add(this.tpConfigure);
            this.tc.Controls.Add(this.tpMonitor);
            this.tc.LeftVisible = false;
            this.tc.Location = new System.Drawing.Point(0, 43);
            this.tc.Name = "tc";
            this.tc.RightVisible = false;
            this.tc.SelectedIndex = 0;
            this.tc.Size = new System.Drawing.Size(1008, 718);
            this.tc.TabIndex = 1;
            this.tc.TopVisible = true;
            // 
            // tmrSchedule
            // 
            this.tmrSchedule.Tick += new System.EventHandler(this.tmrSchedule_Tick);
            // 
            // tmrBatchSaveResults
            // 
            this.tmrBatchSaveResults.Interval = 10000;
            this.tmrBatchSaveResults.Tick += new System.EventHandler(this.tmrBatchSaveResults_Tick);
            // 
            // MonitorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 762);
            this.Controls.Add(this.btnMonitorReady);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.tc);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MonitorView";
            this.Text = "MonitorView";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.tpConfigure.ResumeLayout(false);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel1.PerformLayout();
            this.split.Panel2.ResumeLayout(false);
            this.split.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.pnlEntitiesAndCounters.ResumeLayout(false);
            this.tcCounters.ResumeLayout(false);
            this.tpCounters.ResumeLayout(false);
            this.tpCounters.PerformLayout();
            this.tpCountersInGui.ResumeLayout(false);
            this.tpCountersInGui.PerformLayout();
            this.tpMonitor.ResumeLayout(false);
            this.tpMonitor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLiveMonitoring)).EndInit();
            this.tc.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.Timer tmrProgressDelayCountDown;
        private System.Windows.Forms.TabPage tpConfigure;
        private System.Windows.Forms.TabPage tpMonitor;
        private vApus.Util.TabControlWithAdjustableBorders tc;
        private SolutionTree.SolutionComponentPropertyPanel propertyPanel;
        private System.Windows.Forms.Button btnGetCounters;
        private System.Windows.Forms.Button btnConfiguration;
        private System.Windows.Forms.Panel pnlEntitiesAndCounters;
        private System.Windows.Forms.ListView lvwEntities;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Button btnMonitorReady;
        private System.Windows.Forms.TreeView tvwCounters;
        private System.Windows.Forms.ToolStripButton btnSchedule;
        private System.Windows.Forms.Timer tmrSchedule;
        private System.Windows.Forms.Label lblCountDown;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.Timer tmrBatchSaveResults;
        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.ToolStripButton btnLocalOrRemoteSMT;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilter;
        private Util.TabControlWithAdjustableBorders tcCounters;
        private System.Windows.Forms.TabPage tpCounters;
        private System.Windows.Forms.TabPage tpCountersInGui;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label lblLiveMonitoring;
        private System.Windows.Forms.ListView lvwEntitiesInGui;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.TreeView tvwCountersInGui;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblMonitorSourceMismatch;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label label4;
        private ParameterPanel parameterPanel;
        private vApus.Monitor.MonitorControl dgvLiveMonitoring;
    }
}