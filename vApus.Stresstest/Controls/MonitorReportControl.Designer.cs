namespace vApus.Stresstest
{
    partial class MonitorReportControl
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
            this.pnlFastResultListing = new System.Windows.Forms.Panel();
            this.flpFastMetrics = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlBorderDrillDownAverages = new System.Windows.Forms.Panel();
            this.cboDrillDownAverages = new System.Windows.Forms.ComboBox();
            this.btnSaveDisplayedAverages = new System.Windows.Forms.Button();
            this.btnWarningAverages = new System.Windows.Forms.Button();
            this.lblWarningInvalidAverages = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lvwAveragesListing = new System.Windows.Forms.ListView();
            this.clmConcurrentUsers = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmPrecision = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmRun = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmFill = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblUpdatesIn = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlBorderDrillDownDetailed = new System.Windows.Forms.Panel();
            this.cboDrillDownDetailed = new System.Windows.Forms.ComboBox();
            this.btnSaveCheckedDetailedResults = new System.Windows.Forms.Button();
            this.btnWarningDetailed = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.flpDetailedResults = new System.Windows.Forms.FlowLayoutPanel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.btnConfiguration = new System.Windows.Forms.Button();
            this.pnlFastResultListing.SuspendLayout();
            this.flpFastMetrics.SuspendLayout();
            this.pnlBorderDrillDownAverages.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlBorderDrillDownDetailed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlFastResultListing
            // 
            this.pnlFastResultListing.BackColor = System.Drawing.Color.White;
            this.pnlFastResultListing.Controls.Add(this.flpFastMetrics);
            this.pnlFastResultListing.Controls.Add(this.label4);
            this.pnlFastResultListing.Controls.Add(this.lvwAveragesListing);
            this.pnlFastResultListing.Controls.Add(this.lblUpdatesIn);
            this.pnlFastResultListing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFastResultListing.Location = new System.Drawing.Point(0, 0);
            this.pnlFastResultListing.Name = "pnlFastResultListing";
            this.pnlFastResultListing.Size = new System.Drawing.Size(655, 211);
            this.pnlFastResultListing.TabIndex = 2;
            this.pnlFastResultListing.Text = "Fast Results";
            // 
            // flpFastMetrics
            // 
            this.flpFastMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpFastMetrics.Controls.Add(this.label1);
            this.flpFastMetrics.Controls.Add(this.pnlBorderDrillDownAverages);
            this.flpFastMetrics.Controls.Add(this.btnSaveDisplayedAverages);
            this.flpFastMetrics.Controls.Add(this.btnWarningAverages);
            this.flpFastMetrics.Controls.Add(this.lblWarningInvalidAverages);
            this.flpFastMetrics.Location = new System.Drawing.Point(-1, 37);
            this.flpFastMetrics.Name = "flpFastMetrics";
            this.flpFastMetrics.Size = new System.Drawing.Size(657, 50);
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
            // pnlBorderDrillDownAverages
            // 
            this.pnlBorderDrillDownAverages.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderDrillDownAverages.Controls.Add(this.cboDrillDownAverages);
            this.pnlBorderDrillDownAverages.Location = new System.Drawing.Point(86, 3);
            this.pnlBorderDrillDownAverages.Name = "pnlBorderDrillDownAverages";
            this.pnlBorderDrillDownAverages.Size = new System.Drawing.Size(127, 23);
            this.pnlBorderDrillDownAverages.TabIndex = 0;
            // 
            // cboDrillDownAverages
            // 
            this.cboDrillDownAverages.BackColor = System.Drawing.Color.White;
            this.cboDrillDownAverages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDrillDownAverages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboDrillDownAverages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDrillDownAverages.FormattingEnabled = true;
            this.cboDrillDownAverages.Items.AddRange(new object[] {
            "Concurrent Users",
            "Precision",
            "Run"});
            this.cboDrillDownAverages.Location = new System.Drawing.Point(1, 1);
            this.cboDrillDownAverages.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboDrillDownAverages.Name = "cboDrillDownAverages";
            this.cboDrillDownAverages.Size = new System.Drawing.Size(125, 21);
            this.cboDrillDownAverages.TabIndex = 0;
            this.cboDrillDownAverages.SelectedIndexChanged += new System.EventHandler(this.cboDrillDownAverages_SelectedIndexChanged);
            // 
            // btnSaveDisplayedAverages
            // 
            this.btnSaveDisplayedAverages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveDisplayedAverages.AutoSize = true;
            this.btnSaveDisplayedAverages.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveDisplayedAverages.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveDisplayedAverages.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnSaveDisplayedAverages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveDisplayedAverages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveDisplayedAverages.Location = new System.Drawing.Point(219, 3);
            this.btnSaveDisplayedAverages.MaximumSize = new System.Drawing.Size(176, 24);
            this.btnSaveDisplayedAverages.Name = "btnSaveDisplayedAverages";
            this.btnSaveDisplayedAverages.Size = new System.Drawing.Size(176, 24);
            this.btnSaveDisplayedAverages.TabIndex = 1;
            this.btnSaveDisplayedAverages.Text = "Save Displayed Averages...";
            this.btnSaveDisplayedAverages.UseVisualStyleBackColor = false;
            this.btnSaveDisplayedAverages.Click += new System.EventHandler(this.btnSaveDisplayedAverages_Click);
            // 
            // btnWarningAverages
            // 
            this.btnWarningAverages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWarningAverages.AutoSize = true;
            this.btnWarningAverages.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnWarningAverages.BackColor = System.Drawing.SystemColors.Control;
            this.btnWarningAverages.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnWarningAverages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWarningAverages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWarningAverages.Location = new System.Drawing.Point(401, 3);
            this.btnWarningAverages.MaximumSize = new System.Drawing.Size(176, 24);
            this.btnWarningAverages.Name = "btnWarningAverages";
            this.btnWarningAverages.Size = new System.Drawing.Size(23, 24);
            this.btnWarningAverages.TabIndex = 18;
            this.btnWarningAverages.Text = "!";
            this.btnWarningAverages.UseVisualStyleBackColor = false;
            this.btnWarningAverages.Click += new System.EventHandler(this.btnWarning_Click);
            // 
            // lblWarningInvalidAverages
            // 
            this.lblWarningInvalidAverages.AutoEllipsis = true;
            this.lblWarningInvalidAverages.AutoSize = true;
            this.lblWarningInvalidAverages.BackColor = System.Drawing.Color.Orange;
            this.lblWarningInvalidAverages.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblWarningInvalidAverages.ForeColor = System.Drawing.Color.Black;
            this.lblWarningInvalidAverages.Location = new System.Drawing.Point(3, 36);
            this.lblWarningInvalidAverages.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.lblWarningInvalidAverages.MaximumSize = new System.Drawing.Size(1000, 18);
            this.lblWarningInvalidAverages.Name = "lblWarningInvalidAverages";
            this.lblWarningInvalidAverages.Size = new System.Drawing.Size(632, 18);
            this.lblWarningInvalidAverages.TabIndex = 20;
            this.lblWarningInvalidAverages.Text = "One or more counters became unavailable, therefore their averages are invalid [Sc" +
    "roll to see them]";
            this.lblWarningInvalidAverages.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(2, 7);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "Averages Listing";
            // 
            // lvwAveragesListing
            // 
            this.lvwAveragesListing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwAveragesListing.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwAveragesListing.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmConcurrentUsers,
            this.clmPrecision,
            this.clmRun,
            this.clmFill});
            this.lvwAveragesListing.FullRowSelect = true;
            this.lvwAveragesListing.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwAveragesListing.Location = new System.Drawing.Point(-1, 93);
            this.lvwAveragesListing.MultiSelect = false;
            this.lvwAveragesListing.Name = "lvwAveragesListing";
            this.lvwAveragesListing.Size = new System.Drawing.Size(657, 119);
            this.lvwAveragesListing.TabIndex = 2;
            this.lvwAveragesListing.UseCompatibleStateImageBehavior = false;
            this.lvwAveragesListing.View = System.Windows.Forms.View.Details;
            // 
            // clmConcurrentUsers
            // 
            this.clmConcurrentUsers.Text = "Concurrent Users";
            this.clmConcurrentUsers.Width = 94;
            // 
            // clmPrecision
            // 
            this.clmPrecision.Text = "Precision";
            this.clmPrecision.Width = 55;
            // 
            // clmRun
            // 
            this.clmRun.Text = "Run";
            this.clmRun.Width = 32;
            // 
            // clmFill
            // 
            this.clmFill.Text = "";
            this.clmFill.Width = 1;
            // 
            // lblUpdatesIn
            // 
            this.lblUpdatesIn.AutoSize = true;
            this.lblUpdatesIn.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdatesIn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblUpdatesIn.Location = new System.Drawing.Point(157, 9);
            this.lblUpdatesIn.Name = "lblUpdatesIn";
            this.lblUpdatesIn.Size = new System.Drawing.Size(0, 18);
            this.lblUpdatesIn.TabIndex = 0;
            this.lblUpdatesIn.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.flpDetailedResults);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(655, 245);
            this.panel1.TabIndex = 3;
            this.panel1.Text = "Fast Results";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.pnlBorderDrillDownDetailed);
            this.flowLayoutPanel1.Controls.Add(this.btnSaveCheckedDetailedResults);
            this.flowLayoutPanel1.Controls.Add(this.btnWarningDetailed);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(-1, 37);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(657, 50);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Drill down to";
            // 
            // pnlBorderDrillDownDetailed
            // 
            this.pnlBorderDrillDownDetailed.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderDrillDownDetailed.Controls.Add(this.cboDrillDownDetailed);
            this.pnlBorderDrillDownDetailed.Location = new System.Drawing.Point(86, 3);
            this.pnlBorderDrillDownDetailed.Name = "pnlBorderDrillDownDetailed";
            this.pnlBorderDrillDownDetailed.Size = new System.Drawing.Size(127, 23);
            this.pnlBorderDrillDownDetailed.TabIndex = 0;
            // 
            // cboDrillDownDetailed
            // 
            this.cboDrillDownDetailed.BackColor = System.Drawing.Color.White;
            this.cboDrillDownDetailed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDrillDownDetailed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboDrillDownDetailed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDrillDownDetailed.FormattingEnabled = true;
            this.cboDrillDownDetailed.Items.AddRange(new object[] {
            "Stresstest",
            "Concurrent Users",
            "Precision",
            "Run"});
            this.cboDrillDownDetailed.Location = new System.Drawing.Point(1, 1);
            this.cboDrillDownDetailed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboDrillDownDetailed.Name = "cboDrillDownDetailed";
            this.cboDrillDownDetailed.Size = new System.Drawing.Size(125, 21);
            this.cboDrillDownDetailed.TabIndex = 0;
            this.cboDrillDownDetailed.SelectedIndexChanged += new System.EventHandler(this.cboDrillDownDetailed_SelectedIndexChanged);
            // 
            // btnSaveCheckedDetailedResults
            // 
            this.btnSaveCheckedDetailedResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveCheckedDetailedResults.AutoSize = true;
            this.btnSaveCheckedDetailedResults.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveCheckedDetailedResults.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveCheckedDetailedResults.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnSaveCheckedDetailedResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCheckedDetailedResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveCheckedDetailedResults.Location = new System.Drawing.Point(219, 3);
            this.btnSaveCheckedDetailedResults.MaximumSize = new System.Drawing.Size(176, 24);
            this.btnSaveCheckedDetailedResults.Name = "btnSaveCheckedDetailedResults";
            this.btnSaveCheckedDetailedResults.Size = new System.Drawing.Size(157, 24);
            this.btnSaveCheckedDetailedResults.TabIndex = 18;
            this.btnSaveCheckedDetailedResults.Text = "Save Checked to TXT...";
            this.btnSaveCheckedDetailedResults.UseVisualStyleBackColor = false;
            this.btnSaveCheckedDetailedResults.Click += new System.EventHandler(this.btnSaveCheckedDetailedResults_Click);
            // 
            // btnWarningDetailed
            // 
            this.btnWarningDetailed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWarningDetailed.AutoSize = true;
            this.btnWarningDetailed.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnWarningDetailed.BackColor = System.Drawing.SystemColors.Control;
            this.btnWarningDetailed.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnWarningDetailed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWarningDetailed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWarningDetailed.Location = new System.Drawing.Point(382, 3);
            this.btnWarningDetailed.MaximumSize = new System.Drawing.Size(176, 24);
            this.btnWarningDetailed.Name = "btnWarningDetailed";
            this.btnWarningDetailed.Size = new System.Drawing.Size(23, 24);
            this.btnWarningDetailed.TabIndex = 19;
            this.btnWarningDetailed.Text = "!";
            this.btnWarningDetailed.UseVisualStyleBackColor = false;
            this.btnWarningDetailed.Click += new System.EventHandler(this.btnWarning_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(2, 7);
            this.label7.Margin = new System.Windows.Forms.Padding(3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(126, 20);
            this.label7.TabIndex = 17;
            this.label7.Text = "Detailed Results";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label8.Location = new System.Drawing.Point(157, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(0, 18);
            this.label8.TabIndex = 0;
            this.label8.Visible = false;
            // 
            // flpDetailedResults
            // 
            this.flpDetailedResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpDetailedResults.Location = new System.Drawing.Point(0, 93);
            this.flpDetailedResults.Name = "flpDetailedResults";
            this.flpDetailedResults.Size = new System.Drawing.Size(655, 152);
            this.flpDetailedResults.TabIndex = 3;
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 40);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.pnlFastResultListing);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panel1);
            this.splitContainer.Panel2MinSize = 200;
            this.splitContainer.Size = new System.Drawing.Size(655, 460);
            this.splitContainer.SplitterDistance = 211;
            this.splitContainer.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.btnConfiguration);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(655, 40);
            this.panel2.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(2, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 20);
            this.label3.TabIndex = 18;
            this.label3.Text = "Configuration";
            // 
            // btnConfiguration
            // 
            this.btnConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConfiguration.AutoSize = true;
            this.btnConfiguration.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnConfiguration.BackColor = System.Drawing.SystemColors.Control;
            this.btnConfiguration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfiguration.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfiguration.Location = new System.Drawing.Point(218, 8);
            this.btnConfiguration.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnConfiguration.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnConfiguration.Name = "btnConfiguration";
            this.btnConfiguration.Size = new System.Drawing.Size(137, 24);
            this.btnConfiguration.TabIndex = 4;
            this.btnConfiguration.Text = "Show and/or Save...";
            this.btnConfiguration.UseVisualStyleBackColor = false;
            this.btnConfiguration.Click += new System.EventHandler(this.btnConfiguration_Click);
            // 
            // MonitorReportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitContainer);
            this.Name = "MonitorReportControl";
            this.Size = new System.Drawing.Size(655, 500);
            this.pnlFastResultListing.ResumeLayout(false);
            this.pnlFastResultListing.PerformLayout();
            this.flpFastMetrics.ResumeLayout(false);
            this.flpFastMetrics.PerformLayout();
            this.pnlBorderDrillDownAverages.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.pnlBorderDrillDownDetailed.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlFastResultListing;
        private System.Windows.Forms.FlowLayoutPanel flpFastMetrics;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDrillDownAverages;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView lvwAveragesListing;
        private System.Windows.Forms.ColumnHeader clmConcurrentUsers;
        private System.Windows.Forms.ColumnHeader clmPrecision;
        private System.Windows.Forms.ColumnHeader clmRun;
        private System.Windows.Forms.Label lblUpdatesIn;
        private System.Windows.Forms.Button btnSaveDisplayedAverages;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSaveCheckedDetailedResults;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboDrillDownDetailed;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.FlowLayoutPanel flpDetailedResults;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ColumnHeader clmFill;
        private System.Windows.Forms.Button btnWarningAverages;
        private System.Windows.Forms.Button btnWarningDetailed;
        private System.Windows.Forms.Panel pnlBorderDrillDownAverages;
        private System.Windows.Forms.Panel pnlBorderDrillDownDetailed;
        private System.Windows.Forms.Label lblWarningInvalidAverages;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnConfiguration;
        private System.Windows.Forms.Label label3;
    }
}
