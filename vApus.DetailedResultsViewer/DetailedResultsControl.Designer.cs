using RandomUtils.Log;
using System;
namespace vApus.StressTest {
    partial class DetailedResultsControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            try {
                if (_cancellationTokenSource != null) _cancellationTokenSource.Cancel();
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed cancelling.", ex, new object[] { disposing });
            }

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailedResultsControl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.pnlBorderCollapse = new System.Windows.Forms.Panel();
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.flpConfiguration = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.lbtnDescription = new vApus.Util.LinkButton();
            this.lbtnTags = new vApus.Util.LinkButton();
            this.lbtnvApusInstance = new vApus.Util.LinkButton();
            this.lbtnStressTest = new vApus.Util.LinkButton();
            this.lbtnMonitors = new vApus.Util.LinkButton();
            this.splitQueryData = new System.Windows.Forms.SplitContainer();
            this.pnlBorderExecute = new System.Windows.Forms.Panel();
            this.btnExecute = new System.Windows.Forms.Button();
            this.codeTextBox = new vApus.Util.CodeTextBox();
            this.chkShowCellView = new System.Windows.Forms.CheckBox();
            this.splitData = new System.Windows.Forms.SplitContainer();
            this.dgvDetailedResults = new System.Windows.Forms.DataGridView();
            this.fctxtCellView = new FastColoredTextBoxNS.FastColoredTextBox();
            this.flpDetailedMetrics = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlBorderShow = new System.Windows.Forms.Panel();
            this.cboShow = new System.Windows.Forms.ComboBox();
            this.lblStarted = new System.Windows.Forms.Label();
            this.lblMeasuredRuntime = new System.Windows.Forms.Label();
            this.pnlBorderMonitors = new System.Windows.Forms.Panel();
            this.cboMonitors = new System.Windows.Forms.ComboBox();
            this.lblStopped = new System.Windows.Forms.Label();
            this.chkAdvanced = new System.Windows.Forms.CheckBox();
            this.btnSaveDisplayedResults = new System.Windows.Forms.Button();
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.btnDeleteResults = new System.Windows.Forms.Button();
            this.lblLoading = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnlBorderCollapse.SuspendLayout();
            this.flpConfiguration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitQueryData)).BeginInit();
            this.splitQueryData.Panel1.SuspendLayout();
            this.splitQueryData.Panel2.SuspendLayout();
            this.splitQueryData.SuspendLayout();
            this.pnlBorderExecute.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.codeTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitData)).BeginInit();
            this.splitData.Panel1.SuspendLayout();
            this.splitData.Panel2.SuspendLayout();
            this.splitData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailedResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fctxtCellView)).BeginInit();
            this.flpDetailedMetrics.SuspendLayout();
            this.pnlBorderShow.SuspendLayout();
            this.pnlBorderMonitors.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer.Panel1.Controls.Add(this.pnlBorderCollapse);
            this.splitContainer.Panel1.Controls.Add(this.flpConfiguration);
            this.splitContainer.Panel1MinSize = 39;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer.Panel2.Controls.Add(this.splitQueryData);
            this.splitContainer.Panel2.Controls.Add(this.flpDetailedMetrics);
            this.splitContainer.Panel2.Controls.Add(this.label1);
            this.splitContainer.Size = new System.Drawing.Size(897, 639);
            this.splitContainer.SplitterDistance = 75;
            this.splitContainer.TabIndex = 0;
            // 
            // pnlBorderCollapse
            // 
            this.pnlBorderCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBorderCollapse.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderCollapse.Controls.Add(this.btnCollapseExpand);
            this.pnlBorderCollapse.Location = new System.Drawing.Point(872, 6);
            this.pnlBorderCollapse.Name = "pnlBorderCollapse";
            this.pnlBorderCollapse.Size = new System.Drawing.Size(22, 23);
            this.pnlBorderCollapse.TabIndex = 1;
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
            this.flpConfiguration.Controls.Add(this.label3);
            this.flpConfiguration.Controls.Add(this.lbtnDescription);
            this.flpConfiguration.Controls.Add(this.lbtnTags);
            this.flpConfiguration.Controls.Add(this.lbtnvApusInstance);
            this.flpConfiguration.Controls.Add(this.lbtnStressTest);
            this.flpConfiguration.Controls.Add(this.lbtnMonitors);
            this.flpConfiguration.Location = new System.Drawing.Point(0, 0);
            this.flpConfiguration.Name = "flpConfiguration";
            this.flpConfiguration.Size = new System.Drawing.Size(866, 75);
            this.flpConfiguration.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(3, 6);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 6, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Configuration";
            // 
            // lbtnDescription
            // 
            this.lbtnDescription.Active = true;
            this.lbtnDescription.ActiveLinkColor = System.Drawing.Color.Black;
            this.lbtnDescription.AutoSize = true;
            this.lbtnDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbtnDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnDescription.ForeColor = System.Drawing.Color.Black;
            this.lbtnDescription.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbtnDescription.LinkColor = System.Drawing.Color.Black;
            this.lbtnDescription.Location = new System.Drawing.Point(116, 6);
            this.lbtnDescription.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnDescription.Name = "lbtnDescription";
            this.lbtnDescription.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnDescription.RadioButtonBehavior = true;
            this.lbtnDescription.Size = new System.Drawing.Size(79, 22);
            this.lbtnDescription.TabIndex = 1;
            this.lbtnDescription.TabStop = true;
            this.lbtnDescription.Text = "Description";
            this.lbtnDescription.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnDescription.VisitedLinkColor = System.Drawing.Color.Black;
            this.lbtnDescription.ActiveChanged += new System.EventHandler(this.lbtnDescription_ActiveChanged);
            // 
            // lbtnTags
            // 
            this.lbtnTags.Active = false;
            this.lbtnTags.ActiveLinkColor = System.Drawing.Color.DimGray;
            this.lbtnTags.AutoSize = true;
            this.lbtnTags.BackColor = System.Drawing.Color.Transparent;
            this.lbtnTags.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnTags.ForeColor = System.Drawing.Color.DimGray;
            this.lbtnTags.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnTags.LinkColor = System.Drawing.Color.DimGray;
            this.lbtnTags.Location = new System.Drawing.Point(198, 6);
            this.lbtnTags.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnTags.Name = "lbtnTags";
            this.lbtnTags.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnTags.RadioButtonBehavior = true;
            this.lbtnTags.Size = new System.Drawing.Size(41, 20);
            this.lbtnTags.TabIndex = 2;
            this.lbtnTags.TabStop = true;
            this.lbtnTags.Text = "Tags";
            this.lbtnTags.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnTags.VisitedLinkColor = System.Drawing.Color.DimGray;
            this.lbtnTags.ActiveChanged += new System.EventHandler(this.lbtnTags_ActiveChanged);
            // 
            // lbtnvApusInstance
            // 
            this.lbtnvApusInstance.Active = false;
            this.lbtnvApusInstance.ActiveLinkColor = System.Drawing.Color.DimGray;
            this.lbtnvApusInstance.AutoSize = true;
            this.lbtnvApusInstance.BackColor = System.Drawing.Color.Transparent;
            this.lbtnvApusInstance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnvApusInstance.ForeColor = System.Drawing.Color.DimGray;
            this.lbtnvApusInstance.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnvApusInstance.LinkColor = System.Drawing.Color.DimGray;
            this.lbtnvApusInstance.Location = new System.Drawing.Point(242, 6);
            this.lbtnvApusInstance.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnvApusInstance.Name = "lbtnvApusInstance";
            this.lbtnvApusInstance.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnvApusInstance.RadioButtonBehavior = true;
            this.lbtnvApusInstance.Size = new System.Drawing.Size(100, 20);
            this.lbtnvApusInstance.TabIndex = 3;
            this.lbtnvApusInstance.TabStop = true;
            this.lbtnvApusInstance.Text = "vApus instance";
            this.lbtnvApusInstance.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnvApusInstance.VisitedLinkColor = System.Drawing.Color.DimGray;
            this.lbtnvApusInstance.ActiveChanged += new System.EventHandler(this.lbtnvApusInstance_ActiveChanged);
            // 
            // lbtnStressTest
            // 
            this.lbtnStressTest.Active = false;
            this.lbtnStressTest.ActiveLinkColor = System.Drawing.Color.DimGray;
            this.lbtnStressTest.AutoSize = true;
            this.lbtnStressTest.BackColor = System.Drawing.Color.Transparent;
            this.lbtnStressTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnStressTest.ForeColor = System.Drawing.Color.DimGray;
            this.lbtnStressTest.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnStressTest.LinkColor = System.Drawing.Color.DimGray;
            this.lbtnStressTest.Location = new System.Drawing.Point(345, 6);
            this.lbtnStressTest.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnStressTest.Name = "lbtnStressTest";
            this.lbtnStressTest.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnStressTest.RadioButtonBehavior = true;
            this.lbtnStressTest.Size = new System.Drawing.Size(73, 20);
            this.lbtnStressTest.TabIndex = 4;
            this.lbtnStressTest.TabStop = true;
            this.lbtnStressTest.Text = "Stress test";
            this.lbtnStressTest.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnStressTest.VisitedLinkColor = System.Drawing.Color.DimGray;
            this.lbtnStressTest.ActiveChanged += new System.EventHandler(this.lbtnStressTest_ActiveChanged);
            // 
            // lbtnMonitors
            // 
            this.lbtnMonitors.Active = false;
            this.lbtnMonitors.ActiveLinkColor = System.Drawing.Color.DimGray;
            this.lbtnMonitors.AutoSize = true;
            this.lbtnMonitors.BackColor = System.Drawing.Color.Transparent;
            this.flpConfiguration.SetFlowBreak(this.lbtnMonitors, true);
            this.lbtnMonitors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnMonitors.ForeColor = System.Drawing.Color.DimGray;
            this.lbtnMonitors.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnMonitors.LinkColor = System.Drawing.Color.DimGray;
            this.lbtnMonitors.Location = new System.Drawing.Point(421, 6);
            this.lbtnMonitors.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnMonitors.Name = "lbtnMonitors";
            this.lbtnMonitors.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnMonitors.RadioButtonBehavior = true;
            this.lbtnMonitors.Size = new System.Drawing.Size(61, 20);
            this.lbtnMonitors.TabIndex = 6;
            this.lbtnMonitors.TabStop = true;
            this.lbtnMonitors.Text = "Monitors";
            this.lbtnMonitors.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnMonitors.VisitedLinkColor = System.Drawing.Color.DimGray;
            this.lbtnMonitors.ActiveChanged += new System.EventHandler(this.lbtnMonitors_ActiveChanged);
            // 
            // splitQueryData
            // 
            this.splitQueryData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitQueryData.BackColor = System.Drawing.SystemColors.Control;
            this.splitQueryData.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitQueryData.Location = new System.Drawing.Point(0, 100);
            this.splitQueryData.Name = "splitQueryData";
            this.splitQueryData.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitQueryData.Panel1
            // 
            this.splitQueryData.Panel1.BackColor = System.Drawing.Color.White;
            this.splitQueryData.Panel1.Controls.Add(this.pnlBorderExecute);
            this.splitQueryData.Panel1.Controls.Add(this.codeTextBox);
            this.splitQueryData.Panel1MinSize = 50;
            // 
            // splitQueryData.Panel2
            // 
            this.splitQueryData.Panel2.BackColor = System.Drawing.Color.White;
            this.splitQueryData.Panel2.Controls.Add(this.chkShowCellView);
            this.splitQueryData.Panel2.Controls.Add(this.splitData);
            this.splitQueryData.Size = new System.Drawing.Size(897, 460);
            this.splitQueryData.SplitterDistance = 100;
            this.splitQueryData.TabIndex = 2;
            // 
            // pnlBorderExecute
            // 
            this.pnlBorderExecute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlBorderExecute.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderExecute.Controls.Add(this.btnExecute);
            this.pnlBorderExecute.Location = new System.Drawing.Point(3, 0);
            this.pnlBorderExecute.Name = "pnlBorderExecute";
            this.pnlBorderExecute.Size = new System.Drawing.Size(26, 100);
            this.pnlBorderExecute.TabIndex = 20;
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.BackColor = System.Drawing.Color.White;
            this.btnExecute.FlatAppearance.BorderSize = 0;
            this.btnExecute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecute.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExecute.Image = ((System.Drawing.Image)(resources.GetObject("btnExecute.Image")));
            this.btnExecute.Location = new System.Drawing.Point(1, 1);
            this.btnExecute.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(24, 98);
            this.btnExecute.TabIndex = 1;
            this.btnExecute.TabStop = false;
            this.toolTip.SetToolTip(this.btnExecute, "Execute the script.");
            this.btnExecute.UseVisualStyleBackColor = false;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // codeTextBox
            // 
            this.codeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.codeTextBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.codeTextBox.AutoScrollMinSize = new System.Drawing.Size(0, 14);
            this.codeTextBox.BackBrush = null;
            this.codeTextBox.CharHeight = 14;
            this.codeTextBox.CharWidth = 8;
            this.codeTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.codeTextBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.codeTextBox.IsReplaceMode = false;
            this.codeTextBox.Location = new System.Drawing.Point(30, 0);
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.Paddings = new System.Windows.Forms.Padding(0);
            this.codeTextBox.PreferredLineWidth = 65536;
            this.codeTextBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.codeTextBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("codeTextBox.ServiceColors")));
            this.codeTextBox.Size = new System.Drawing.Size(864, 100);
            this.codeTextBox.TabIndex = 0;
            this.codeTextBox.WordWrap = true;
            this.codeTextBox.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.CharWrapControlWidth;
            this.codeTextBox.Zoom = 100;
            // 
            // chkShowCellView
            // 
            this.chkShowCellView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowCellView.AutoSize = true;
            this.chkShowCellView.Location = new System.Drawing.Point(8, 329);
            this.chkShowCellView.Name = "chkShowCellView";
            this.chkShowCellView.Size = new System.Drawing.Size(97, 17);
            this.chkShowCellView.TabIndex = 35;
            this.chkShowCellView.Text = "Show cell view";
            this.toolTip.SetToolTip(this.chkShowCellView, "Show a view when a cell is selected, if checked.");
            this.chkShowCellView.UseVisualStyleBackColor = true;
            this.chkShowCellView.CheckedChanged += new System.EventHandler(this.chkShowCellView_CheckedChanged);
            // 
            // splitData
            // 
            this.splitData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitData.BackColor = System.Drawing.SystemColors.Control;
            this.splitData.Location = new System.Drawing.Point(-1, -2);
            this.splitData.Name = "splitData";
            this.splitData.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitData.Panel1
            // 
            this.splitData.Panel1.BackColor = System.Drawing.Color.White;
            this.splitData.Panel1.Controls.Add(this.dgvDetailedResults);
            // 
            // splitData.Panel2
            // 
            this.splitData.Panel2.Controls.Add(this.fctxtCellView);
            this.splitData.Panel2Collapsed = true;
            this.splitData.Size = new System.Drawing.Size(899, 325);
            this.splitData.SplitterDistance = 160;
            this.splitData.TabIndex = 34;
            // 
            // dgvDetailedResults
            // 
            this.dgvDetailedResults.AllowUserToAddRows = false;
            this.dgvDetailedResults.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvDetailedResults.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDetailedResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvDetailedResults.BackgroundColor = System.Drawing.Color.White;
            this.dgvDetailedResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetailedResults.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvDetailedResults.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 9.75F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDetailedResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDetailedResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetailedResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetailedResults.EnableHeadersVisualStyles = false;
            this.dgvDetailedResults.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.dgvDetailedResults.Location = new System.Drawing.Point(0, 0);
            this.dgvDetailedResults.Name = "dgvDetailedResults";
            this.dgvDetailedResults.ReadOnly = true;
            this.dgvDetailedResults.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 9.75F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDetailedResults.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDetailedResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgvDetailedResults.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvDetailedResults.Size = new System.Drawing.Size(899, 325);
            this.dgvDetailedResults.TabIndex = 0;
            this.dgvDetailedResults.VirtualMode = true;
            this.dgvDetailedResults.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetailedResults_CellEnter);
            // 
            // fctxtCellView
            // 
            this.fctxtCellView.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fctxtCellView.AutoScrollMinSize = new System.Drawing.Size(0, 14);
            this.fctxtCellView.BackBrush = null;
            this.fctxtCellView.CharHeight = 14;
            this.fctxtCellView.CharWidth = 8;
            this.fctxtCellView.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctxtCellView.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctxtCellView.Enabled = false;
            this.fctxtCellView.IsReplaceMode = false;
            this.fctxtCellView.Location = new System.Drawing.Point(0, 0);
            this.fctxtCellView.Name = "fctxtCellView";
            this.fctxtCellView.Paddings = new System.Windows.Forms.Padding(0);
            this.fctxtCellView.PreferredLineWidth = 65536;
            this.fctxtCellView.ReadOnly = true;
            this.fctxtCellView.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctxtCellView.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fctxtCellView.ServiceColors")));
            this.fctxtCellView.Size = new System.Drawing.Size(899, 161);
            this.fctxtCellView.TabIndex = 2;
            this.fctxtCellView.WordWrap = true;
            this.fctxtCellView.Zoom = 100;
            // 
            // flpDetailedMetrics
            // 
            this.flpDetailedMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpDetailedMetrics.Controls.Add(this.label2);
            this.flpDetailedMetrics.Controls.Add(this.pnlBorderShow);
            this.flpDetailedMetrics.Controls.Add(this.lblStarted);
            this.flpDetailedMetrics.Controls.Add(this.lblMeasuredRuntime);
            this.flpDetailedMetrics.Controls.Add(this.pnlBorderMonitors);
            this.flpDetailedMetrics.Controls.Add(this.lblStopped);
            this.flpDetailedMetrics.Controls.Add(this.chkAdvanced);
            this.flpDetailedMetrics.Controls.Add(this.btnSaveDisplayedResults);
            this.flpDetailedMetrics.Controls.Add(this.btnExportToExcel);
            this.flpDetailedMetrics.Controls.Add(this.btnDeleteResults);
            this.flpDetailedMetrics.Controls.Add(this.lblLoading);
            this.flpDetailedMetrics.Location = new System.Drawing.Point(-1, 40);
            this.flpDetailedMetrics.Name = "flpDetailedMetrics";
            this.flpDetailedMetrics.Size = new System.Drawing.Size(899, 60);
            this.flpDetailedMetrics.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(5, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 6, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Show";
            // 
            // pnlBorderShow
            // 
            this.pnlBorderShow.BackColor = System.Drawing.Color.LightSteelBlue;
            this.pnlBorderShow.Controls.Add(this.cboShow);
            this.pnlBorderShow.Location = new System.Drawing.Point(49, 2);
            this.pnlBorderShow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.pnlBorderShow.Name = "pnlBorderShow";
            this.pnlBorderShow.Size = new System.Drawing.Size(402, 25);
            this.pnlBorderShow.TabIndex = 1;
            // 
            // cboShow
            // 
            this.cboShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboShow.BackColor = System.Drawing.Color.White;
            this.cboShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboShow.FormattingEnabled = true;
            this.cboShow.Items.AddRange(new object[] {
            "Average concurrency results",
            "Average user actions",
            "Average requests",
            "Errors",
            "User action composition",
            "Machine configurations",
            "Average monitor results",
            "Messages"});
            this.cboShow.Location = new System.Drawing.Point(2, 2);
            this.cboShow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboShow.Name = "cboShow";
            this.cboShow.Size = new System.Drawing.Size(398, 21);
            this.cboShow.TabIndex = 0;
            this.cboShow.SelectedIndexChanged += new System.EventHandler(this.cboShow_SelectedIndexChanged);
            // 
            // lblStarted
            // 
            this.lblStarted.AutoSize = true;
            this.lblStarted.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStarted.Location = new System.Drawing.Point(454, 6);
            this.lblStarted.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblStarted.Name = "lblStarted";
            this.lblStarted.Size = new System.Drawing.Size(0, 16);
            this.lblStarted.TabIndex = 0;
            // 
            // lblMeasuredRuntime
            // 
            this.lblMeasuredRuntime.AutoSize = true;
            this.lblMeasuredRuntime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMeasuredRuntime.Location = new System.Drawing.Point(454, 6);
            this.lblMeasuredRuntime.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblMeasuredRuntime.Name = "lblMeasuredRuntime";
            this.lblMeasuredRuntime.Size = new System.Drawing.Size(0, 16);
            this.lblMeasuredRuntime.TabIndex = 0;
            // 
            // pnlBorderMonitors
            // 
            this.pnlBorderMonitors.BackColor = System.Drawing.Color.LightSteelBlue;
            this.pnlBorderMonitors.Controls.Add(this.cboMonitors);
            this.pnlBorderMonitors.Location = new System.Drawing.Point(457, 2);
            this.pnlBorderMonitors.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.pnlBorderMonitors.Name = "pnlBorderMonitors";
            this.pnlBorderMonitors.Size = new System.Drawing.Size(302, 25);
            this.pnlBorderMonitors.TabIndex = 2;
            this.pnlBorderMonitors.Visible = false;
            // 
            // cboMonitors
            // 
            this.cboMonitors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboMonitors.BackColor = System.Drawing.Color.White;
            this.cboMonitors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMonitors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMonitors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMonitors.FormattingEnabled = true;
            this.cboMonitors.Location = new System.Drawing.Point(2, 2);
            this.cboMonitors.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboMonitors.Name = "cboMonitors";
            this.cboMonitors.Size = new System.Drawing.Size(298, 21);
            this.cboMonitors.TabIndex = 0;
            this.cboMonitors.SelectedIndexChanged += new System.EventHandler(this.cboMonitors_SelectedIndexChanged);
            // 
            // lblStopped
            // 
            this.lblStopped.AutoSize = true;
            this.lblStopped.BackColor = System.Drawing.SystemColors.Control;
            this.lblStopped.Font = new System.Drawing.Font("Consolas", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStopped.Location = new System.Drawing.Point(768, 3);
            this.lblStopped.Margin = new System.Windows.Forms.Padding(6, 3, 6, 0);
            this.lblStopped.Name = "lblStopped";
            this.lblStopped.Size = new System.Drawing.Size(0, 20);
            this.lblStopped.TabIndex = 0;
            // 
            // chkAdvanced
            // 
            this.chkAdvanced.AutoSize = true;
            this.chkAdvanced.Checked = true;
            this.chkAdvanced.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAdvanced.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkAdvanced.Location = new System.Drawing.Point(777, 7);
            this.chkAdvanced.Margin = new System.Windows.Forms.Padding(3, 7, 0, 3);
            this.chkAdvanced.Name = "chkAdvanced";
            this.chkAdvanced.Size = new System.Drawing.Size(72, 17);
            this.chkAdvanced.TabIndex = 3;
            this.chkAdvanced.Text = "Advanced";
            this.toolTip.SetToolTip(this.chkAdvanced, "Check this if you want to execute your own SQL script on the database.");
            this.chkAdvanced.UseVisualStyleBackColor = true;
            this.chkAdvanced.CheckedChanged += new System.EventHandler(this.chkAdvanced_CheckedChanged);
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
            this.btnSaveDisplayedResults.Location = new System.Drawing.Point(3, 33);
            this.btnSaveDisplayedResults.MaximumSize = new System.Drawing.Size(165, 24);
            this.btnSaveDisplayedResults.Name = "btnSaveDisplayedResults";
            this.btnSaveDisplayedResults.Size = new System.Drawing.Size(158, 24);
            this.btnSaveDisplayedResults.TabIndex = 4;
            this.btnSaveDisplayedResults.Text = "Save displayed results...";
            this.btnSaveDisplayedResults.UseVisualStyleBackColor = false;
            this.btnSaveDisplayedResults.Click += new System.EventHandler(this.btnSaveDisplayedResults_Click);
            // 
            // btnExportToExcel
            // 
            this.btnExportToExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportToExcel.AutoSize = true;
            this.btnExportToExcel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnExportToExcel.BackColor = System.Drawing.SystemColors.Control;
            this.btnExportToExcel.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnExportToExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportToExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportToExcel.Location = new System.Drawing.Point(167, 33);
            this.btnExportToExcel.MaximumSize = new System.Drawing.Size(165, 24);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(117, 24);
            this.btnExportToExcel.TabIndex = 5;
            this.btnExportToExcel.Text = "Export to Excel...";
            this.btnExportToExcel.UseVisualStyleBackColor = false;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // btnDeleteResults
            // 
            this.btnDeleteResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteResults.AutoSize = true;
            this.btnDeleteResults.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDeleteResults.BackColor = System.Drawing.SystemColors.Control;
            this.btnDeleteResults.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnDeleteResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteResults.Location = new System.Drawing.Point(290, 33);
            this.btnDeleteResults.MaximumSize = new System.Drawing.Size(165, 24);
            this.btnDeleteResults.Name = "btnDeleteResults";
            this.btnDeleteResults.Size = new System.Drawing.Size(97, 24);
            this.btnDeleteResults.TabIndex = 6;
            this.btnDeleteResults.Text = "Delete results";
            this.btnDeleteResults.UseVisualStyleBackColor = false;
            this.btnDeleteResults.Click += new System.EventHandler(this.btnDeleteResults_Click);
            // 
            // lblLoading
            // 
            this.lblLoading.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblLoading.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoading.Location = new System.Drawing.Point(393, 37);
            this.lblLoading.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(170, 13);
            this.lblLoading.TabIndex = 10;
            this.lblLoading.Text = "Loading, please be patient...";
            this.toolTip.SetToolTip(this.lblLoading, "These counters are not valid for the chosen monitor source, undo your previous ac" +
        "tion or \'get\' the counters again.");
            this.lblLoading.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Detailed results";
            // 
            // DetailedResultsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitContainer);
            this.Name = "DetailedResultsControl";
            this.Size = new System.Drawing.Size(897, 639);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pnlBorderCollapse.ResumeLayout(false);
            this.flpConfiguration.ResumeLayout(false);
            this.flpConfiguration.PerformLayout();
            this.splitQueryData.Panel1.ResumeLayout(false);
            this.splitQueryData.Panel2.ResumeLayout(false);
            this.splitQueryData.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitQueryData)).EndInit();
            this.splitQueryData.ResumeLayout(false);
            this.pnlBorderExecute.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.codeTextBox)).EndInit();
            this.splitData.Panel1.ResumeLayout(false);
            this.splitData.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitData)).EndInit();
            this.splitData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailedResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fctxtCellView)).EndInit();
            this.flpDetailedMetrics.ResumeLayout(false);
            this.flpDetailedMetrics.PerformLayout();
            this.pnlBorderShow.ResumeLayout(false);
            this.pnlBorderMonitors.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.FlowLayoutPanel flpConfiguration;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlBorderCollapse;
        private System.Windows.Forms.Button btnCollapseExpand;
        private System.Windows.Forms.Label label1;
        private Util.LinkButton lbtnDescription;
        private Util.LinkButton lbtnTags;
        private Util.LinkButton lbtnvApusInstance;
        private Util.LinkButton lbtnStressTest;
        private System.Windows.Forms.FlowLayoutPanel flpDetailedMetrics;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlBorderShow;
        private System.Windows.Forms.ComboBox cboShow;
        private System.Windows.Forms.Label lblStarted;
        private System.Windows.Forms.Label lblMeasuredRuntime;
        private System.Windows.Forms.Label lblStopped;
        private System.Windows.Forms.CheckBox chkAdvanced;
        private System.Windows.Forms.Button btnSaveDisplayedResults;
        private System.Windows.Forms.SplitContainer splitQueryData;
        private vApus.Util.CodeTextBox codeTextBox;
        private System.Windows.Forms.DataGridView dgvDetailedResults;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Panel pnlBorderExecute;
        private System.Windows.Forms.ToolTip toolTip;
        private Util.LinkButton lbtnMonitors;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Button btnExportToExcel;
        private System.Windows.Forms.Button btnDeleteResults;
        private System.Windows.Forms.SplitContainer splitData;
        private FastColoredTextBoxNS.FastColoredTextBox fctxtCellView;
        private System.Windows.Forms.CheckBox chkShowCellView;
        private System.Windows.Forms.Panel pnlBorderMonitors;
        private System.Windows.Forms.ComboBox cboMonitors;
    }
}
