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
            this.linkButton1 = new vApus.Util.LinkButton();
            this.linkButton2 = new vApus.Util.LinkButton();
            this.linkButton3 = new vApus.Util.LinkButton();
            this.splitQueryData = new System.Windows.Forms.SplitContainer();
            this.btnExecute = new System.Windows.Forms.Button();
            this.dgvDetailedResults = new System.Windows.Forms.DataGridView();
            this.flpDetailedMetrics = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlBorderShow = new System.Windows.Forms.Panel();
            this.cboShow = new System.Windows.Forms.ComboBox();
            this.lblStarted = new System.Windows.Forms.Label();
            this.lblMeasuredRuntime = new System.Windows.Forms.Label();
            this.lblStopped = new System.Windows.Forms.Label();
            this.chkReadable = new System.Windows.Forms.CheckBox();
            this.btnSaveDisplayedResults = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlBorderExecute = new System.Windows.Forms.Panel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.codeTextBox = new vApus.Stresstest.CodeTextBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailedResults)).BeginInit();
            this.flpDetailedMetrics.SuspendLayout();
            this.pnlBorderShow.SuspendLayout();
            this.pnlBorderExecute.SuspendLayout();
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
            this.splitContainer.Panel1MinSize = 34;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer.Panel2.Controls.Add(this.splitQueryData);
            this.splitContainer.Panel2.Controls.Add(this.flpDetailedMetrics);
            this.splitContainer.Panel2.Controls.Add(this.label1);
            this.splitContainer.Size = new System.Drawing.Size(897, 639);
            this.splitContainer.SplitterDistance = 68;
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
            this.flpConfiguration.Controls.Add(this.linkButton1);
            this.flpConfiguration.Controls.Add(this.linkButton2);
            this.flpConfiguration.Controls.Add(this.linkButton3);
            this.flpConfiguration.Location = new System.Drawing.Point(0, 0);
            this.flpConfiguration.Name = "flpConfiguration";
            this.flpConfiguration.Size = new System.Drawing.Size(866, 68);
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
            this.label3.TabIndex = 0;
            this.label3.Text = "Configuration";
            // 
            // lbtnDescription
            // 
            this.lbtnDescription.Active = true;
            this.lbtnDescription.ActiveLinkColor = System.Drawing.Color.Black;
            this.lbtnDescription.AutoSize = true;
            this.lbtnDescription.BackColor = System.Drawing.Color.LightBlue;
            this.lbtnDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnDescription.ForeColor = System.Drawing.Color.Black;
            this.lbtnDescription.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbtnDescription.LinkColor = System.Drawing.Color.Black;
            this.lbtnDescription.Location = new System.Drawing.Point(116, 6);
            this.lbtnDescription.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnDescription.MinimumSize = new System.Drawing.Size(0, 24);
            this.lbtnDescription.Name = "lbtnDescription";
            this.lbtnDescription.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnDescription.RadioButtonBehavior = true;
            this.lbtnDescription.Size = new System.Drawing.Size(77, 24);
            this.lbtnDescription.TabIndex = 1;
            this.lbtnDescription.TabStop = true;
            this.lbtnDescription.Text = "Description";
            this.lbtnDescription.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnDescription.VisitedLinkColor = System.Drawing.Color.Black;
            this.lbtnDescription.ActiveChanged += new System.EventHandler(this.lbtnDescription_ActiveChanged);
            // 
            // linkButton1
            // 
            this.linkButton1.Active = false;
            this.linkButton1.ActiveLinkColor = System.Drawing.Color.Blue;
            this.linkButton1.AutoSize = true;
            this.linkButton1.BackColor = System.Drawing.Color.Transparent;
            this.linkButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.linkButton1.ForeColor = System.Drawing.Color.Blue;
            this.linkButton1.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkButton1.LinkColor = System.Drawing.Color.Blue;
            this.linkButton1.Location = new System.Drawing.Point(196, 6);
            this.linkButton1.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.linkButton1.MinimumSize = new System.Drawing.Size(0, 24);
            this.linkButton1.Name = "linkButton1";
            this.linkButton1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.linkButton1.RadioButtonBehavior = true;
            this.linkButton1.Size = new System.Drawing.Size(37, 24);
            this.linkButton1.TabIndex = 2;
            this.linkButton1.TabStop = true;
            this.linkButton1.Text = "Tags";
            this.linkButton1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.linkButton1.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkButton1.ActiveChanged += new System.EventHandler(this.lbtnDescription_ActiveChanged);
            // 
            // linkButton2
            // 
            this.linkButton2.Active = false;
            this.linkButton2.ActiveLinkColor = System.Drawing.Color.Blue;
            this.linkButton2.AutoSize = true;
            this.linkButton2.BackColor = System.Drawing.Color.Transparent;
            this.linkButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.linkButton2.ForeColor = System.Drawing.Color.Blue;
            this.linkButton2.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkButton2.LinkColor = System.Drawing.Color.Blue;
            this.linkButton2.Location = new System.Drawing.Point(236, 6);
            this.linkButton2.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.linkButton2.MinimumSize = new System.Drawing.Size(0, 24);
            this.linkButton2.Name = "linkButton2";
            this.linkButton2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.linkButton2.RadioButtonBehavior = true;
            this.linkButton2.Size = new System.Drawing.Size(92, 24);
            this.linkButton2.TabIndex = 3;
            this.linkButton2.TabStop = true;
            this.linkButton2.Text = "vApus Instances";
            this.linkButton2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.linkButton2.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkButton2.ActiveChanged += new System.EventHandler(this.lbtnDescription_ActiveChanged);
            // 
            // linkButton3
            // 
            this.linkButton3.Active = false;
            this.linkButton3.ActiveLinkColor = System.Drawing.Color.Blue;
            this.linkButton3.AutoSize = true;
            this.linkButton3.BackColor = System.Drawing.Color.Transparent;
            this.flpConfiguration.SetFlowBreak(this.linkButton3, true);
            this.linkButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.linkButton3.ForeColor = System.Drawing.Color.Blue;
            this.linkButton3.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkButton3.LinkColor = System.Drawing.Color.Blue;
            this.linkButton3.Location = new System.Drawing.Point(331, 6);
            this.linkButton3.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.linkButton3.MinimumSize = new System.Drawing.Size(0, 24);
            this.linkButton3.Name = "linkButton3";
            this.linkButton3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.linkButton3.RadioButtonBehavior = true;
            this.linkButton3.Size = new System.Drawing.Size(59, 24);
            this.linkButton3.TabIndex = 4;
            this.linkButton3.TabStop = true;
            this.linkButton3.Text = "Stresstest";
            this.linkButton3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.linkButton3.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkButton3.ActiveChanged += new System.EventHandler(this.lbtnDescription_ActiveChanged);
            // 
            // splitQueryData
            // 
            this.splitQueryData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitQueryData.BackColor = System.Drawing.SystemColors.Control;
            this.splitQueryData.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitQueryData.Location = new System.Drawing.Point(0, 96);
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
            this.splitQueryData.Size = new System.Drawing.Size(897, 471);
            this.splitQueryData.SplitterDistance = 100;
            this.splitQueryData.TabIndex = 2;
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
            this.btnExecute.Size = new System.Drawing.Size(20, 98);
            this.btnExecute.TabIndex = 1;
            this.btnExecute.TabStop = false;
            this.toolTip.SetToolTip(this.btnExecute, "Execute the script.");
            this.btnExecute.UseVisualStyleBackColor = false;
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
            this.dgvDetailedResults.Size = new System.Drawing.Size(897, 367);
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
            this.flpDetailedMetrics.Controls.Add(this.chkReadable);
            this.flpDetailedMetrics.Controls.Add(this.btnSaveDisplayedResults);
            this.flpDetailedMetrics.Location = new System.Drawing.Point(-1, 37);
            this.flpDetailedMetrics.Name = "flpDetailedMetrics";
            this.flpDetailedMetrics.Size = new System.Drawing.Size(899, 60);
            this.flpDetailedMetrics.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Show";
            // 
            // pnlBorderShow
            // 
            this.pnlBorderShow.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderShow.Controls.Add(this.cboShow);
            this.pnlBorderShow.Location = new System.Drawing.Point(47, 3);
            this.pnlBorderShow.Name = "pnlBorderShow";
            this.pnlBorderShow.Size = new System.Drawing.Size(127, 23);
            this.pnlBorderShow.TabIndex = 1;
            // 
            // cboShow
            // 
            this.cboShow.BackColor = System.Drawing.Color.White;
            this.cboShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboShow.FormattingEnabled = true;
            this.cboShow.Items.AddRange(new object[] {
            "Concurrencies",
            "Runs"});
            this.cboShow.Location = new System.Drawing.Point(1, 1);
            this.cboShow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboShow.Name = "cboShow";
            this.cboShow.Size = new System.Drawing.Size(125, 21);
            this.cboShow.TabIndex = 0;
            // 
            // lblStarted
            // 
            this.lblStarted.AutoSize = true;
            this.lblStarted.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStarted.Location = new System.Drawing.Point(177, 6);
            this.lblStarted.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblStarted.Name = "lblStarted";
            this.lblStarted.Size = new System.Drawing.Size(0, 16);
            this.lblStarted.TabIndex = 0;
            // 
            // lblMeasuredRuntime
            // 
            this.lblMeasuredRuntime.AutoSize = true;
            this.lblMeasuredRuntime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMeasuredRuntime.Location = new System.Drawing.Point(177, 6);
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
            this.lblStopped.Location = new System.Drawing.Point(183, 3);
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
            this.chkReadable.Location = new System.Drawing.Point(192, 7);
            this.chkReadable.Margin = new System.Windows.Forms.Padding(3, 7, 0, 3);
            this.chkReadable.Name = "chkReadable";
            this.chkReadable.Size = new System.Drawing.Size(72, 17);
            this.chkReadable.TabIndex = 2;
            this.chkReadable.Text = "Advanced";
            this.toolTip.SetToolTip(this.chkReadable, "Check this if you want to execute your own SQL script on the database.");
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
            this.btnSaveDisplayedResults.Location = new System.Drawing.Point(267, 3);
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
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Detailed Results";
            // 
            // pnlBorderExecute
            // 
            this.pnlBorderExecute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBorderExecute.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderExecute.Controls.Add(this.btnExecute);
            this.pnlBorderExecute.Location = new System.Drawing.Point(872, 0);
            this.pnlBorderExecute.Name = "pnlBorderExecute";
            this.pnlBorderExecute.Size = new System.Drawing.Size(22, 100);
            this.pnlBorderExecute.TabIndex = 20;
            // 
            // codeTextBox
            // 
            this.codeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.codeTextBox.AutoScrollMinSize = new System.Drawing.Size(0, 15);
            this.codeTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.codeTextBox.Location = new System.Drawing.Point(0, 0);
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.PreferredLineWidth = 65536;
            this.codeTextBox.Size = new System.Drawing.Size(866, 100);
            this.codeTextBox.TabIndex = 0;
            this.codeTextBox.WordWrap = true;
            this.codeTextBox.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.CharWrapControlWidth;
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailedResults)).EndInit();
            this.flpDetailedMetrics.ResumeLayout(false);
            this.flpDetailedMetrics.PerformLayout();
            this.pnlBorderShow.ResumeLayout(false);
            this.pnlBorderExecute.ResumeLayout(false);
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
        private Util.LinkButton linkButton1;
        private Util.LinkButton linkButton2;
        private Util.LinkButton linkButton3;
        private System.Windows.Forms.FlowLayoutPanel flpDetailedMetrics;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlBorderShow;
        private System.Windows.Forms.ComboBox cboShow;
        private System.Windows.Forms.Label lblStarted;
        private System.Windows.Forms.Label lblMeasuredRuntime;
        private System.Windows.Forms.Label lblStopped;
        private System.Windows.Forms.CheckBox chkReadable;
        private System.Windows.Forms.Button btnSaveDisplayedResults;
        private System.Windows.Forms.SplitContainer splitQueryData;
        private CodeTextBox codeTextBox;
        private System.Windows.Forms.DataGridView dgvDetailedResults;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Panel pnlBorderExecute;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
