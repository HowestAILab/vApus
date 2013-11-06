namespace vApus.Monitor
{
    partial class MonitorView
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
                {
                    try {
                        System.Exception stopEx;
                        _monitorProxy.Stop(out stopEx);
                    }
                    catch { }
                    try { _monitorProxy.Dispose(); }
                    catch { }
                    _monitorProxy = null;
                }
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnSchedule = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.tmrProgressDelayCountDown = new System.Windows.Forms.Timer(this.components);
            this.tpConfigure = new System.Windows.Forms.TabPage();
            this.split = new System.Windows.Forms.SplitContainer();
            this.lblMonitorSourceMismatch = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.parameterPanel = new vApus.Monitor.MonitorParameterPanel();
            this.propertyPanel = new vApus.SolutionTree.SolutionComponentPropertyPanel();
            this.btnConfiguration = new System.Windows.Forms.Button();
            this.btnGetCounters = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.llblUncheckAllVisible = new System.Windows.Forms.LinkLabel();
            this.picFilter = new System.Windows.Forms.PictureBox();
            this.llblCheckAllVisible = new System.Windows.Forms.LinkLabel();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.lvwEntities = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmEntities = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmChosenCounters = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imgListEntityState = new System.Windows.Forms.ImageList(this.components);
            this.tvwCounters = new System.Windows.Forms.TreeView();
            this.tpMonitor = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblCountDown = new System.Windows.Forms.Label();
            this.picFilterMonitorControlColumns = new System.Windows.Forms.PictureBox();
            this.txtFilterMonitorControlColumns = new System.Windows.Forms.TextBox();
            this.btnSaveFilteredMonitoredCounters = new System.Windows.Forms.Button();
            this.btnSaveAllMonitorCounters = new System.Windows.Forms.Button();
            this.monitorControl = new vApus.Monitor.MonitorControl();
            this.tc = new vApus.Util.TabControlWithAdjustableBorders();
            this.tmrSchedule = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolStrip.SuspendLayout();
            this.tpConfigure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFilter)).BeginInit();
            this.tpMonitor.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFilterMonitorControlColumns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.monitorControl)).BeginInit();
            this.tc.SuspendLayout();
            this.SuspendLayout();
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
            this.tpConfigure.Size = new System.Drawing.Size(1007, 659);
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
            this.split.Panel1.Controls.Add(this.lblMonitorSourceMismatch);
            this.split.Panel1.Controls.Add(this.label4);
            this.split.Panel1.Controls.Add(this.parameterPanel);
            this.split.Panel1.Controls.Add(this.propertyPanel);
            this.split.Panel1.Controls.Add(this.btnConfiguration);
            this.split.Panel1.Controls.Add(this.btnGetCounters);
            // 
            // split.Panel2
            // 
            this.split.Panel2.BackColor = System.Drawing.Color.White;
            this.split.Panel2.Controls.Add(this.panel1);
            this.split.Panel2.Controls.Add(this.lvwEntities);
            this.split.Panel2.Controls.Add(this.tvwCounters);
            this.split.Size = new System.Drawing.Size(1001, 653);
            this.split.SplitterDistance = 325;
            this.split.SplitterWidth = 2;
            this.split.TabIndex = 0;
            // 
            // lblMonitorSourceMismatch
            // 
            this.lblMonitorSourceMismatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMonitorSourceMismatch.AutoEllipsis = true;
            this.lblMonitorSourceMismatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblMonitorSourceMismatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMonitorSourceMismatch.Location = new System.Drawing.Point(362, 304);
            this.lblMonitorSourceMismatch.Name = "lblMonitorSourceMismatch";
            this.lblMonitorSourceMismatch.Size = new System.Drawing.Size(636, 13);
            this.lblMonitorSourceMismatch.TabIndex = 9;
            this.lblMonitorSourceMismatch.Text = "These counters are not valid for the chosen monitor source, undo your previous ac" +
    "tion or \'get\' the counters again.";
            this.toolTip.SetToolTip(this.lblMonitorSourceMismatch, "These counters are not valid for the chosen monitor source, undo your previous ac" +
        "tion or \'get\' the counters again.");
            this.lblMonitorSourceMismatch.Visible = false;
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
            this.parameterPanel.AutoSelectControl = false;
            this.parameterPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.parameterPanel.Location = new System.Drawing.Point(597, 23);
            this.parameterPanel.Name = "parameterPanel";
            this.parameterPanel.ParametersWithValues = null;
            this.parameterPanel.Size = new System.Drawing.Size(404, 272);
            this.parameterPanel.TabIndex = 1;
            this.parameterPanel.ParameterValueChanged += new System.EventHandler(this.parameterPanel_ParameterValueChanged);
            // 
            // propertyPanel
            // 
            this.propertyPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyPanel.BackColor = System.Drawing.Color.Transparent;
            this.propertyPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyPanel.Location = new System.Drawing.Point(3, 3);
            this.propertyPanel.Margin = new System.Windows.Forms.Padding(0);
            this.propertyPanel.Name = "propertyPanel";
            this.propertyPanel.Size = new System.Drawing.Size(594, 292);
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
            this.btnConfiguration.Location = new System.Drawing.Point(262, 298);
            this.btnConfiguration.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnConfiguration.MaximumSize = new System.Drawing.Size(94, 24);
            this.btnConfiguration.Name = "btnConfiguration";
            this.btnConfiguration.Size = new System.Drawing.Size(94, 24);
            this.btnConfiguration.TabIndex = 3;
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
            this.btnGetCounters.Location = new System.Drawing.Point(3, 298);
            this.btnGetCounters.Name = "btnGetCounters";
            this.btnGetCounters.Size = new System.Drawing.Size(253, 24);
            this.btnGetCounters.TabIndex = 2;
            this.btnGetCounters.Text = "Get Counters";
            this.btnGetCounters.UseVisualStyleBackColor = false;
            this.btnGetCounters.Click += new System.EventHandler(this.btnGetCounters_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.llblUncheckAllVisible);
            this.panel1.Controls.Add(this.picFilter);
            this.panel1.Controls.Add(this.llblCheckAllVisible);
            this.panel1.Controls.Add(this.txtFilter);
            this.panel1.Location = new System.Drawing.Point(362, 4);
            this.panel1.MinimumSize = new System.Drawing.Size(227, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(355, 21);
            this.panel1.TabIndex = 1;
            // 
            // llblUncheckAllVisible
            // 
            this.llblUncheckAllVisible.ActiveLinkColor = System.Drawing.Color.Black;
            this.llblUncheckAllVisible.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llblUncheckAllVisible.AutoSize = true;
            this.llblUncheckAllVisible.BackColor = System.Drawing.SystemColors.Control;
            this.llblUncheckAllVisible.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.llblUncheckAllVisible.Enabled = false;
            this.llblUncheckAllVisible.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llblUncheckAllVisible.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.llblUncheckAllVisible.LinkColor = System.Drawing.Color.Black;
            this.llblUncheckAllVisible.Location = new System.Drawing.Point(236, 1);
            this.llblUncheckAllVisible.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.llblUncheckAllVisible.MinimumSize = new System.Drawing.Size(2, 20);
            this.llblUncheckAllVisible.Name = "llblUncheckAllVisible";
            this.llblUncheckAllVisible.Size = new System.Drawing.Size(117, 20);
            this.llblUncheckAllVisible.TabIndex = 9;
            this.llblUncheckAllVisible.TabStop = true;
            this.llblUncheckAllVisible.Text = "Uncheck all visible";
            this.llblUncheckAllVisible.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.llblUncheckAllVisible.VisitedLinkColor = System.Drawing.Color.Black;
            this.llblUncheckAllVisible.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblUncheckAllVisible_LinkClicked);
            // 
            // picFilter
            // 
            this.picFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picFilter.Image = global::vApus.Monitor.Properties.Resources.find;
            this.picFilter.Location = new System.Drawing.Point(108, 1);
            this.picFilter.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.picFilter.Name = "picFilter";
            this.picFilter.Size = new System.Drawing.Size(20, 20);
            this.picFilter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFilter.TabIndex = 8;
            this.picFilter.TabStop = false;
            this.toolTip.SetToolTip(this.picFilter, "Type comma to split filter entries and \'return\' to submit. Wild card * can be use" +
        "d. Not case sensitive.\r\nAll entries are in OR-relation with each other.");
            this.picFilter.Click += new System.EventHandler(this.picFilter_Click);
            // 
            // llblCheckAllVisible
            // 
            this.llblCheckAllVisible.ActiveLinkColor = System.Drawing.Color.Black;
            this.llblCheckAllVisible.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llblCheckAllVisible.AutoSize = true;
            this.llblCheckAllVisible.BackColor = System.Drawing.SystemColors.Control;
            this.llblCheckAllVisible.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.llblCheckAllVisible.Enabled = false;
            this.llblCheckAllVisible.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llblCheckAllVisible.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.llblCheckAllVisible.LinkColor = System.Drawing.Color.Black;
            this.llblCheckAllVisible.Location = new System.Drawing.Point(131, 1);
            this.llblCheckAllVisible.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.llblCheckAllVisible.MinimumSize = new System.Drawing.Size(2, 20);
            this.llblCheckAllVisible.Name = "llblCheckAllVisible";
            this.llblCheckAllVisible.Size = new System.Drawing.Size(102, 20);
            this.llblCheckAllVisible.TabIndex = 1;
            this.llblCheckAllVisible.TabStop = true;
            this.llblCheckAllVisible.Text = "Check all visible";
            this.llblCheckAllVisible.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.llblCheckAllVisible.VisitedLinkColor = System.Drawing.Color.Black;
            this.llblCheckAllVisible.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblCheckAllVisible_LinkClicked);
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.HideSelection = false;
            this.txtFilter.Location = new System.Drawing.Point(1, 1);
            this.txtFilter.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.txtFilter.MinimumSize = new System.Drawing.Size(100, 4);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(108, 20);
            this.txtFilter.TabIndex = 0;
            this.txtFilter.TabStop = false;
            this.toolTip.SetToolTip(this.txtFilter, "Type comma to split filter entries and \'return\' to submit. Wild card * can be use" +
        "d. Not case sensitive.\r\nAll entries are in OR-relation with each other.");
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            this.txtFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFilter_KeyDown);
            this.txtFilter.Leave += new System.EventHandler(this.txtFilter_Leave);
            // 
            // lvwEntities
            // 
            this.lvwEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvwEntities.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwEntities.CheckBoxes = true;
            this.lvwEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.clmEntities,
            this.clmChosenCounters});
            this.lvwEntities.FullRowSelect = true;
            this.lvwEntities.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwEntities.HideSelection = false;
            this.lvwEntities.Location = new System.Drawing.Point(3, 3);
            this.lvwEntities.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lvwEntities.MultiSelect = false;
            this.lvwEntities.Name = "lvwEntities";
            this.lvwEntities.Size = new System.Drawing.Size(353, 323);
            this.lvwEntities.SmallImageList = this.imgListEntityState;
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
            // clmEntities
            // 
            this.clmEntities.Text = "Entities";
            this.clmEntities.Width = 185;
            // 
            // clmChosenCounters
            // 
            this.clmChosenCounters.Text = "Chosen Counters";
            this.clmChosenCounters.Width = 105;
            // 
            // imgListEntityState
            // 
            this.imgListEntityState.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListEntityState.ImageStream")));
            this.imgListEntityState.TransparentColor = System.Drawing.Color.Transparent;
            this.imgListEntityState.Images.SetKeyName(0, "Online.png");
            this.imgListEntityState.Images.SetKeyName(1, "Offline.png");
            this.imgListEntityState.Images.SetKeyName(2, "Suspended.png");
            // 
            // tvwCounters
            // 
            this.tvwCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwCounters.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvwCounters.CheckBoxes = true;
            this.tvwCounters.Location = new System.Drawing.Point(359, 33);
            this.tvwCounters.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.tvwCounters.Name = "tvwCounters";
            this.tvwCounters.Size = new System.Drawing.Size(642, 293);
            this.tvwCounters.TabIndex = 2;
            this.tvwCounters.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvwCounter_AfterCheck);
            this.tvwCounters.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvwCounter_BeforeExpand);
            // 
            // tpMonitor
            // 
            this.tpMonitor.BackColor = System.Drawing.Color.White;
            this.tpMonitor.Controls.Add(this.flowLayoutPanel1);
            this.tpMonitor.Controls.Add(this.picFilterMonitorControlColumns);
            this.tpMonitor.Controls.Add(this.txtFilterMonitorControlColumns);
            this.tpMonitor.Controls.Add(this.btnSaveFilteredMonitoredCounters);
            this.tpMonitor.Controls.Add(this.btnSaveAllMonitorCounters);
            this.tpMonitor.Controls.Add(this.monitorControl);
            this.tpMonitor.Location = new System.Drawing.Point(0, 22);
            this.tpMonitor.Name = "tpMonitor";
            this.tpMonitor.Size = new System.Drawing.Size(1007, 659);
            this.tpMonitor.TabIndex = 1;
            this.tpMonitor.Text = "Monitor";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.lblCountDown);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(208, 47);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(788, 24);
            this.flowLayoutPanel1.TabIndex = 12;
            // 
            // lblCountDown
            // 
            this.lblCountDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCountDown.AutoSize = true;
            this.lblCountDown.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblCountDown.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblCountDown.Location = new System.Drawing.Point(700, 3);
            this.lblCountDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.lblCountDown.Name = "lblCountDown";
            this.lblCountDown.Size = new System.Drawing.Size(88, 18);
            this.lblCountDown.TabIndex = 2;
            this.lblCountDown.Text = "Updates in";
            this.lblCountDown.Visible = false;
            // 
            // picFilterMonitorControlColumns
            // 
            this.picFilterMonitorControlColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picFilterMonitorControlColumns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picFilterMonitorControlColumns.Image = global::vApus.Monitor.Properties.Resources.find;
            this.picFilterMonitorControlColumns.Location = new System.Drawing.Point(976, 16);
            this.picFilterMonitorControlColumns.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.picFilterMonitorControlColumns.Name = "picFilterMonitorControlColumns";
            this.picFilterMonitorControlColumns.Size = new System.Drawing.Size(20, 20);
            this.picFilterMonitorControlColumns.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFilterMonitorControlColumns.TabIndex = 11;
            this.picFilterMonitorControlColumns.TabStop = false;
            this.toolTip.SetToolTip(this.picFilterMonitorControlColumns, "Type comma to split filter entries and \'return\' to submit. Wild card * can be use" +
        "d. Not case sensitive.\r\nAll entries are in OR-relation with each other.");
            this.picFilterMonitorControlColumns.Click += new System.EventHandler(this.picFilterMonitorControlColumns_Click);
            // 
            // txtFilterMonitorControlColumns
            // 
            this.txtFilterMonitorControlColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilterMonitorControlColumns.HideSelection = false;
            this.txtFilterMonitorControlColumns.Location = new System.Drawing.Point(12, 16);
            this.txtFilterMonitorControlColumns.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.txtFilterMonitorControlColumns.Name = "txtFilterMonitorControlColumns";
            this.txtFilterMonitorControlColumns.Size = new System.Drawing.Size(965, 20);
            this.txtFilterMonitorControlColumns.TabIndex = 0;
            this.txtFilterMonitorControlColumns.TabStop = false;
            this.toolTip.SetToolTip(this.txtFilterMonitorControlColumns, "Type comma to split filter entries and \'return\' to submit. Wild card * can be use" +
        "d. Not case sensitive.\r\nAll entries are in OR-relation with each other.");
            this.txtFilterMonitorControlColumns.TextChanged += new System.EventHandler(this.txtFilterMonitorControlColumns_TextChanged);
            this.txtFilterMonitorControlColumns.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFilterMonitorControlColumns_KeyDown);
            this.txtFilterMonitorControlColumns.Leave += new System.EventHandler(this.txtFilterMonitorControlColumns_Leave);
            // 
            // btnSaveFilteredMonitoredCounters
            // 
            this.btnSaveFilteredMonitoredCounters.AutoSize = true;
            this.btnSaveFilteredMonitoredCounters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveFilteredMonitoredCounters.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveFilteredMonitoredCounters.Enabled = false;
            this.btnSaveFilteredMonitoredCounters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveFilteredMonitoredCounters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveFilteredMonitoredCounters.Location = new System.Drawing.Point(12, 47);
            this.btnSaveFilteredMonitoredCounters.MaximumSize = new System.Drawing.Size(106, 24);
            this.btnSaveFilteredMonitoredCounters.Name = "btnSaveFilteredMonitoredCounters";
            this.btnSaveFilteredMonitoredCounters.Size = new System.Drawing.Size(106, 24);
            this.btnSaveFilteredMonitoredCounters.TabIndex = 1;
            this.btnSaveFilteredMonitoredCounters.Text = "Save Filtered...";
            this.toolTip.SetToolTip(this.btnSaveFilteredMonitoredCounters, "To filter the counters in a (large) counter collection. Wild card * can be used. " +
        "Not case sensitive. All entries are in OR-relation with each other.");
            this.btnSaveFilteredMonitoredCounters.UseVisualStyleBackColor = false;
            this.btnSaveFilteredMonitoredCounters.Click += new System.EventHandler(this.btnSaveFilteredMonitoredCounters_Click);
            // 
            // btnSaveAllMonitorCounters
            // 
            this.btnSaveAllMonitorCounters.AutoSize = true;
            this.btnSaveAllMonitorCounters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveAllMonitorCounters.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveAllMonitorCounters.Enabled = false;
            this.btnSaveAllMonitorCounters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveAllMonitorCounters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveAllMonitorCounters.Location = new System.Drawing.Point(124, 47);
            this.btnSaveAllMonitorCounters.MaximumSize = new System.Drawing.Size(78, 24);
            this.btnSaveAllMonitorCounters.Name = "btnSaveAllMonitorCounters";
            this.btnSaveAllMonitorCounters.Size = new System.Drawing.Size(78, 24);
            this.btnSaveAllMonitorCounters.TabIndex = 2;
            this.btnSaveAllMonitorCounters.Text = "Save All...";
            this.toolTip.SetToolTip(this.btnSaveAllMonitorCounters, "To filter the counters in a (large) counter collection. Wild card * can be used. " +
        "Not case sensitive. All entries are in OR-relation with each other.");
            this.btnSaveAllMonitorCounters.UseVisualStyleBackColor = false;
            this.btnSaveAllMonitorCounters.Click += new System.EventHandler(this.btnSaveAllMonitorCounters_Click);
            // 
            // monitorControl
            // 
            this.monitorControl.AllowUserToAddRows = false;
            this.monitorControl.AllowUserToDeleteRows = false;
            this.monitorControl.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.monitorControl.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.monitorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.monitorControl.BackgroundColor = System.Drawing.Color.White;
            this.monitorControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.monitorControl.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.monitorControl.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.monitorControl.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.monitorControl.DoubleBuffered = true;
            this.monitorControl.EnableHeadersVisualStyles = false;
            this.monitorControl.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.monitorControl.Location = new System.Drawing.Point(0, 80);
            this.monitorControl.Name = "monitorControl";
            this.monitorControl.ReadOnly = true;
            this.monitorControl.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.monitorControl.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.monitorControl.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.monitorControl.Size = new System.Drawing.Size(1007, 579);
            this.monitorControl.TabIndex = 3;
            this.monitorControl.VirtualMode = true;
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
            this.tc.Size = new System.Drawing.Size(1008, 682);
            this.tc.TabIndex = 1;
            this.tc.TopVisible = true;
            // 
            // tmrSchedule
            // 
            this.tmrSchedule.Tick += new System.EventHandler(this.tmrSchedule_Tick);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 20;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "TXT Files | *.txt";
            // 
            // MonitorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 726);
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
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFilter)).EndInit();
            this.tpMonitor.ResumeLayout(false);
            this.tpMonitor.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFilterMonitorControlColumns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.monitorControl)).EndInit();
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
        private System.Windows.Forms.ListView lvwEntities;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader clmEntities;
        private System.Windows.Forms.ImageList imgListEntityState;
        private System.Windows.Forms.TreeView tvwCounters;
        private System.Windows.Forms.ToolStripButton btnSchedule;
        private System.Windows.Forms.Timer tmrSchedule;
        private System.Windows.Forms.Label lblCountDown;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.ColumnHeader clmChosenCounters;
        private System.Windows.Forms.Label lblMonitorSourceMismatch;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label label4;
        private MonitorParameterPanel parameterPanel;
        private vApus.Monitor.MonitorControl monitorControl;
        private System.Windows.Forms.Button btnSaveFilteredMonitoredCounters;
        private System.Windows.Forms.Button btnSaveAllMonitorCounters;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.PictureBox picFilter;
        private System.Windows.Forms.LinkLabel llblCheckAllVisible;
        private System.Windows.Forms.PictureBox picFilterMonitorControlColumns;
        private System.Windows.Forms.TextBox txtFilterMonitorControlColumns;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel llblUncheckAllVisible;
    }
}