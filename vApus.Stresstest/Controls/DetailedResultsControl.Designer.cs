namespace vApus.Stresstest.Controls
{
    partial class DetailedResultsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailedResultsControl));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.pnlBorderCollapse = new System.Windows.Forms.Panel();
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.flpConfiguration = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.lbtnDescription = new vApus.Util.LinkButton();
            this.lbtnTags = new vApus.Util.LinkButton();
            this.lbtnvApusInstance = new vApus.Util.LinkButton();
            this.lbtnStresstest = new vApus.Util.LinkButton();
            this.lbtnMonitors = new vApus.Util.LinkButton();
            this.splitQueryData = new System.Windows.Forms.SplitContainer();
            this.pnlBorderExecute = new System.Windows.Forms.Panel();
            this.btnExecute = new System.Windows.Forms.Button();
            this.codeTextBox = new vApus.Stresstest.CodeTextBox();
            this.dgvDetailedResults = new System.Windows.Forms.DataGridView();
            this.flpDetailedMetrics = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlBorderShow = new System.Windows.Forms.Panel();
            this.cboShow = new System.Windows.Forms.ComboBox();
            this.lblStarted = new System.Windows.Forms.Label();
            this.lblMeasuredRuntime = new System.Windows.Forms.Label();
            this.lblStopped = new System.Windows.Forms.Label();
            this.chkAdvanced = new System.Windows.Forms.CheckBox();
            this.btnSaveDisplayedResults = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailedResults)).BeginInit();
            this.flpDetailedMetrics.SuspendLayout();
            this.pnlBorderShow.SuspendLayout();
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
            this.toolTip.SetToolTip(this.btnCollapseExpand, "Collapse or Expand");
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
            this.flpConfiguration.Controls.Add(this.lbtnStresstest);
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
            this.lbtnTags.ActiveLinkColor = System.Drawing.Color.Blue;
            this.lbtnTags.AutoSize = true;
            this.lbtnTags.BackColor = System.Drawing.Color.Transparent;
            this.lbtnTags.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lbtnTags.ForeColor = System.Drawing.Color.Blue;
            this.lbtnTags.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnTags.LinkColor = System.Drawing.Color.Blue;
            this.lbtnTags.Location = new System.Drawing.Point(198, 6);
            this.lbtnTags.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnTags.Name = "lbtnTags";
            this.lbtnTags.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnTags.RadioButtonBehavior = true;
            this.lbtnTags.Size = new System.Drawing.Size(37, 20);
            this.lbtnTags.TabIndex = 2;
            this.lbtnTags.TabStop = true;
            this.lbtnTags.Text = "Tags";
            this.lbtnTags.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnTags.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lbtnTags.ActiveChanged += new System.EventHandler(this.lbtnTags_ActiveChanged);
            // 
            // lbtnvApusInstance
            // 
            this.lbtnvApusInstance.Active = false;
            this.lbtnvApusInstance.ActiveLinkColor = System.Drawing.Color.Blue;
            this.lbtnvApusInstance.AutoSize = true;
            this.lbtnvApusInstance.BackColor = System.Drawing.Color.Transparent;
            this.lbtnvApusInstance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lbtnvApusInstance.ForeColor = System.Drawing.Color.Blue;
            this.lbtnvApusInstance.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnvApusInstance.LinkColor = System.Drawing.Color.Blue;
            this.lbtnvApusInstance.Location = new System.Drawing.Point(238, 6);
            this.lbtnvApusInstance.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnvApusInstance.Name = "lbtnvApusInstance";
            this.lbtnvApusInstance.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnvApusInstance.RadioButtonBehavior = true;
            this.lbtnvApusInstance.Size = new System.Drawing.Size(87, 20);
            this.lbtnvApusInstance.TabIndex = 3;
            this.lbtnvApusInstance.TabStop = true;
            this.lbtnvApusInstance.Text = "vApus Instance";
            this.lbtnvApusInstance.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnvApusInstance.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lbtnvApusInstance.ActiveChanged += new System.EventHandler(this.lbtnvApusInstance_ActiveChanged);
            // 
            // lbtnStresstest
            // 
            this.lbtnStresstest.Active = false;
            this.lbtnStresstest.ActiveLinkColor = System.Drawing.Color.Blue;
            this.lbtnStresstest.AutoSize = true;
            this.lbtnStresstest.BackColor = System.Drawing.Color.Transparent;
            this.lbtnStresstest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lbtnStresstest.ForeColor = System.Drawing.Color.Blue;
            this.lbtnStresstest.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnStresstest.LinkColor = System.Drawing.Color.Blue;
            this.lbtnStresstest.Location = new System.Drawing.Point(328, 6);
            this.lbtnStresstest.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnStresstest.Name = "lbtnStresstest";
            this.lbtnStresstest.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnStresstest.RadioButtonBehavior = true;
            this.lbtnStresstest.Size = new System.Drawing.Size(59, 20);
            this.lbtnStresstest.TabIndex = 4;
            this.lbtnStresstest.TabStop = true;
            this.lbtnStresstest.Text = "Stresstest";
            this.lbtnStresstest.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnStresstest.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lbtnStresstest.ActiveChanged += new System.EventHandler(this.lbtnStresstest_ActiveChanged);
            // 
            // lbtnMonitors
            // 
            this.lbtnMonitors.Active = false;
            this.lbtnMonitors.ActiveLinkColor = System.Drawing.Color.Blue;
            this.lbtnMonitors.AutoSize = true;
            this.lbtnMonitors.BackColor = System.Drawing.Color.Transparent;
            this.flpConfiguration.SetFlowBreak(this.lbtnMonitors, true);
            this.lbtnMonitors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lbtnMonitors.ForeColor = System.Drawing.Color.Blue;
            this.lbtnMonitors.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnMonitors.LinkColor = System.Drawing.Color.Blue;
            this.lbtnMonitors.Location = new System.Drawing.Point(390, 6);
            this.lbtnMonitors.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnMonitors.Name = "lbtnMonitors";
            this.lbtnMonitors.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnMonitors.RadioButtonBehavior = true;
            this.lbtnMonitors.Size = new System.Drawing.Size(53, 20);
            this.lbtnMonitors.TabIndex = 6;
            this.lbtnMonitors.TabStop = true;
            this.lbtnMonitors.Text = "Monitors";
            this.lbtnMonitors.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnMonitors.VisitedLinkColor = System.Drawing.Color.Blue;
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
            this.splitQueryData.Panel2.Controls.Add(this.dgvDetailedResults);
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
            this.codeTextBox.AutoScrollMinSize = new System.Drawing.Size(0, 15);
            this.codeTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.codeTextBox.Location = new System.Drawing.Point(30, 0);
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.PreferredLineWidth = 65536;
            this.codeTextBox.Size = new System.Drawing.Size(864, 100);
            this.codeTextBox.TabIndex = 0;
            this.codeTextBox.WordWrap = true;
            this.codeTextBox.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.CharWrapControlWidth;
            // 
            // dgvDetailedResults
            // 
            this.dgvDetailedResults.AllowUserToAddRows = false;
            this.dgvDetailedResults.AllowUserToDeleteRows = false;
            this.dgvDetailedResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvDetailedResults.BackgroundColor = System.Drawing.Color.White;
            this.dgvDetailedResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetailedResults.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvDetailedResults.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvDetailedResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetailedResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetailedResults.EnableHeadersVisualStyles = false;
            this.dgvDetailedResults.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.dgvDetailedResults.Location = new System.Drawing.Point(0, 0);
            this.dgvDetailedResults.Name = "dgvDetailedResults";
            this.dgvDetailedResults.ReadOnly = true;
            this.dgvDetailedResults.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvDetailedResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvDetailedResults.Size = new System.Drawing.Size(897, 356);
            this.dgvDetailedResults.TabIndex = 0;
            this.dgvDetailedResults.VirtualMode = true;
            // 
            // flpDetailedMetrics
            // 
            this.flpDetailedMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpDetailedMetrics.Controls.Add(this.label2);
            this.flpDetailedMetrics.Controls.Add(this.pnlBorderShow);
            this.flpDetailedMetrics.Controls.Add(this.lblStarted);
            this.flpDetailedMetrics.Controls.Add(this.lblMeasuredRuntime);
            this.flpDetailedMetrics.Controls.Add(this.lblStopped);
            this.flpDetailedMetrics.Controls.Add(this.chkAdvanced);
            this.flpDetailedMetrics.Controls.Add(this.btnSaveDisplayedResults);
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
            this.pnlBorderShow.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderShow.Controls.Add(this.cboShow);
            this.pnlBorderShow.Location = new System.Drawing.Point(49, 3);
            this.pnlBorderShow.Name = "pnlBorderShow";
            this.pnlBorderShow.Size = new System.Drawing.Size(200, 23);
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
            "Average Concurrency Results",
            "Average User Actions",
            "Average Log Entries",
            "Errors"});
            this.cboShow.Location = new System.Drawing.Point(1, 1);
            this.cboShow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboShow.Name = "cboShow";
            this.cboShow.Size = new System.Drawing.Size(198, 21);
            this.cboShow.TabIndex = 0;
            this.cboShow.SelectedIndexChanged += new System.EventHandler(this.cboShow_SelectedIndexChanged);
            // 
            // lblStarted
            // 
            this.lblStarted.AutoSize = true;
            this.lblStarted.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStarted.Location = new System.Drawing.Point(252, 6);
            this.lblStarted.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblStarted.Name = "lblStarted";
            this.lblStarted.Size = new System.Drawing.Size(0, 16);
            this.lblStarted.TabIndex = 0;
            // 
            // lblMeasuredRuntime
            // 
            this.lblMeasuredRuntime.AutoSize = true;
            this.lblMeasuredRuntime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMeasuredRuntime.Location = new System.Drawing.Point(252, 6);
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
            this.lblStopped.Location = new System.Drawing.Point(258, 3);
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
            this.chkAdvanced.Location = new System.Drawing.Point(267, 7);
            this.chkAdvanced.Margin = new System.Windows.Forms.Padding(3, 7, 0, 3);
            this.chkAdvanced.Name = "chkAdvanced";
            this.chkAdvanced.Size = new System.Drawing.Size(72, 17);
            this.chkAdvanced.TabIndex = 2;
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
            this.btnSaveDisplayedResults.Location = new System.Drawing.Point(342, 3);
            this.btnSaveDisplayedResults.MaximumSize = new System.Drawing.Size(165, 24);
            this.btnSaveDisplayedResults.Name = "btnSaveDisplayedResults";
            this.btnSaveDisplayedResults.Size = new System.Drawing.Size(165, 24);
            this.btnSaveDisplayedResults.TabIndex = 3;
            this.btnSaveDisplayedResults.Text = "Save Displayed Results...";
            this.btnSaveDisplayedResults.UseVisualStyleBackColor = false;
            this.btnSaveDisplayedResults.Click += new System.EventHandler(this.btnSaveDisplayedResults_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Detailed Results";
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
            ((System.ComponentModel.ISupportInitialize)(this.splitQueryData)).EndInit();
            this.splitQueryData.ResumeLayout(false);
            this.pnlBorderExecute.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailedResults)).EndInit();
            this.flpDetailedMetrics.ResumeLayout(false);
            this.flpDetailedMetrics.PerformLayout();
            this.pnlBorderShow.ResumeLayout(false);
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
        private Util.LinkButton lbtnStresstest;
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
        private CodeTextBox codeTextBox;
        private System.Windows.Forms.DataGridView dgvDetailedResults;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Panel pnlBorderExecute;
        private System.Windows.Forms.ToolTip toolTip;
        private Util.LinkButton lbtnMonitors;
    }
}
