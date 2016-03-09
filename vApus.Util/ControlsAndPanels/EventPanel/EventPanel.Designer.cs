namespace vApus.Util
{
    partial class EventPanel
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
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.eventProgressBar = new vApus.Util.EventProgressChart();
            this.cboFilter = new System.Windows.Forms.ComboBox();
            this.eventView = new vApus.Util.EventView();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCollapseExpand
            // 
            this.btnCollapseExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseExpand.BackColor = System.Drawing.Color.White;
            this.btnCollapseExpand.FlatAppearance.BorderSize = 0;
            this.btnCollapseExpand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollapseExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpand.Location = new System.Drawing.Point(477, 2);
            this.btnCollapseExpand.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnCollapseExpand.Name = "btnCollapseExpand";
            this.btnCollapseExpand.Size = new System.Drawing.Size(20, 21);
            this.btnCollapseExpand.TabIndex = 1;
            this.btnCollapseExpand.TabStop = false;
            this.btnCollapseExpand.Text = "-";
            this.toolTip.SetToolTip(this.btnCollapseExpand, "Collapse or expand.");
            this.btnCollapseExpand.UseVisualStyleBackColor = false;
            this.btnCollapseExpand.Click += new System.EventHandler(this.btnCollapseExpand_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.eventProgressBar);
            this.splitContainer.Panel1.Controls.Add(this.cboFilter);
            this.splitContainer.Panel1.Controls.Add(this.btnCollapseExpand);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.eventView);
            this.splitContainer.Size = new System.Drawing.Size(500, 200);
            this.splitContainer.SplitterDistance = 25;
            this.splitContainer.SplitterWidth = 1;
            this.splitContainer.TabIndex = 4;
            // 
            // eventProgressBar
            // 
            this.eventProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventProgressBar.BackColor = System.Drawing.SystemColors.ControlDark;
            this.eventProgressBar.BeginOfTimeFrame = new System.DateTime(((long)(0)));
            this.eventProgressBar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.eventProgressBar.EventToolTip = true;
            this.eventProgressBar.Location = new System.Drawing.Point(3, 2);
            this.eventProgressBar.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.eventProgressBar.Name = "eventProgressBar";
            this.eventProgressBar.ProgressBarColor = System.Drawing.Color.LightSteelBlue;
            this.eventProgressBar.Size = new System.Drawing.Size(398, 21);
            this.eventProgressBar.TabIndex = 1;
            this.eventProgressBar.EventClick += new System.EventHandler<vApus.Util.EventProgressChart.ProgressEventEventArgs>(this.eventProgressBar_EventClick);
            // 
            // cboFilter
            // 
            this.cboFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboFilter.FormattingEnabled = true;
            this.cboFilter.Items.AddRange(new object[] {
            "Info",
            "Warning",
            "Error"});
            this.cboFilter.Location = new System.Drawing.Point(404, 2);
            this.cboFilter.Name = "cboFilter";
            this.cboFilter.Size = new System.Drawing.Size(70, 21);
            this.cboFilter.TabIndex = 0;
            this.toolTip.SetToolTip(this.cboFilter, "Filter level...");
            this.cboFilter.SelectedIndexChanged += new System.EventHandler(this.cboFilter_SelectedIndexChanged);
            // 
            // eventView
            // 
            this.eventView.BackColor = System.Drawing.Color.White;
            this.eventView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventView.Location = new System.Drawing.Point(0, 0);
            this.eventView.Margin = new System.Windows.Forms.Padding(0);
            this.eventView.Name = "eventView";
            this.eventView.Size = new System.Drawing.Size(500, 174);
            this.eventView.TabIndex = 2;
            // 
            // EventPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.splitContainer);
            this.Name = "EventPanel";
            this.Size = new System.Drawing.Size(500, 200);
            this.Resize += new System.EventHandler(this.EventPanel_Resize);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private EventProgressChart eventProgressBar;
        private EventView eventView;
        private System.Windows.Forms.Button btnCollapseExpand;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ComboBox cboFilter;

    }
}
