namespace vApus.Util
{
    partial class ChartArea
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
            this.lblChartAreaTitle = new System.Windows.Forms.Label();
            this.lblYTitle = new System.Windows.Forms.Label();
            this.lblXTitle = new System.Windows.Forms.Label();
            this.pnlChartControlHolder = new System.Windows.Forms.Panel();
            this.pnlCBOView = new System.Windows.Forms.Panel();
            this.cboView = new System.Windows.Forms.ComboBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.yAxis = new vApus.Util.YAxis();
            this.xAxis = new vApus.Util.XAxis();
            this.pnlChartControlHolder.SuspendLayout();
            this.pnlCBOView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yAxis)).BeginInit();
            this.SuspendLayout();
            // 
            // lblChartAreaTitle
            // 
            this.lblChartAreaTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblChartAreaTitle.AutoEllipsis = true;
            this.lblChartAreaTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChartAreaTitle.Location = new System.Drawing.Point(60, 0);
            this.lblChartAreaTitle.Name = "lblChartAreaTitle";
            this.lblChartAreaTitle.Size = new System.Drawing.Size(439, 30);
            this.lblChartAreaTitle.TabIndex = 0;
            this.lblChartAreaTitle.Text = "Title";
            this.lblChartAreaTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblYTitle
            // 
            this.lblYTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblYTitle.AutoEllipsis = true;
            this.lblYTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYTitle.Location = new System.Drawing.Point(6, 33);
            this.lblYTitle.Name = "lblYTitle";
            this.lblYTitle.Size = new System.Drawing.Size(15, 197);
            this.lblYTitle.TabIndex = 1;
            this.lblYTitle.Text = "Y";
            this.lblYTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblXTitle
            // 
            this.lblXTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblXTitle.AutoEllipsis = true;
            this.lblXTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblXTitle.Location = new System.Drawing.Point(60, 236);
            this.lblXTitle.Name = "lblXTitle";
            this.lblXTitle.Size = new System.Drawing.Size(439, 23);
            this.lblXTitle.TabIndex = 2;
            this.lblXTitle.Text = "X";
            this.lblXTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlChartControlHolder
            // 
            this.pnlChartControlHolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlChartControlHolder.BackColor = System.Drawing.Color.DimGray;
            this.pnlChartControlHolder.Controls.Add(this.pnlCBOView);
            this.pnlChartControlHolder.Location = new System.Drawing.Point(0, 27);
            this.pnlChartControlHolder.Margin = new System.Windows.Forms.Padding(0);
            this.pnlChartControlHolder.Name = "pnlChartControlHolder";
            this.pnlChartControlHolder.Size = new System.Drawing.Size(440, 146);
            this.pnlChartControlHolder.TabIndex = 3;
            this.pnlChartControlHolder.MouseEnter += new System.EventHandler(this.pnlChartControlHolder_MouseEnter);
            this.pnlChartControlHolder.MouseLeave += new System.EventHandler(this.pnlChartControlHolder_MouseLeave);
            // 
            // pnlCBOView
            // 
            this.pnlCBOView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlCBOView.BackColor = System.Drawing.Color.DimGray;
            this.pnlCBOView.Controls.Add(this.cboView);
            this.pnlCBOView.Location = new System.Drawing.Point(312, 0);
            this.pnlCBOView.Name = "pnlCBOView";
            this.pnlCBOView.Size = new System.Drawing.Size(129, 32);
            this.pnlCBOView.TabIndex = 1;
            this.pnlCBOView.Visible = false;
            // 
            // cboView
            // 
            this.cboView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboView.FormattingEnabled = true;
            this.cboView.Items.AddRange(new object[] {
            "Collapsed",
            "Expanded"});
            this.cboView.Location = new System.Drawing.Point(4, 7);
            this.cboView.Name = "cboView";
            this.cboView.Size = new System.Drawing.Size(121, 21);
            this.cboView.TabIndex = 0;
            this.cboView.SelectedIndexChanged += new System.EventHandler(this.cboView_SelectedIndexChanged);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(31, 33);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.yAxis);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer.Panel2.Controls.Add(this.pnlChartControlHolder);
            this.splitContainer.Panel2.Controls.Add(this.xAxis);
            this.splitContainer.Size = new System.Drawing.Size(468, 197);
            this.splitContainer.SplitterDistance = 25;
            this.splitContainer.TabIndex = 6;
            // 
            // yAxis
            // 
            this.yAxis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.yAxis.BackColor = System.Drawing.Color.White;
            this.yAxis.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yAxis.Location = new System.Drawing.Point(0, -1);
            this.yAxis.MinimumSize = new System.Drawing.Size(5, 100);
            this.yAxis.Name = "yAxis";
            this.yAxis.Size = new System.Drawing.Size(23, 199);
            this.yAxis.TabIndex = 4;
            this.yAxis.TabStop = false;
            this.yAxis.YAxisViewState = vApus.Util.YAxisViewState.OnlyShowSelected;
            // 
            // xAxis
            // 
            this.xAxis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xAxis.BackColor = System.Drawing.Color.White;
            this.xAxis.Location = new System.Drawing.Point(0, 173);
            this.xAxis.Name = "xAxis";
            this.xAxis.Size = new System.Drawing.Size(440, 25);
            this.xAxis.TabIndex = 5;
            // 
            // ChartArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblChartAreaTitle);
            this.Controls.Add(this.lblYTitle);
            this.Controls.Add(this.lblXTitle);
            this.Controls.Add(this.splitContainer);
            this.Name = "ChartArea";
            this.Size = new System.Drawing.Size(499, 262);
            this.SizeChanged += new System.EventHandler(this.ChartArea_SizeChanged);
            this.Validated += new System.EventHandler(this.ChartArea_Validated);
            this.pnlChartControlHolder.ResumeLayout(false);
            this.pnlCBOView.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.yAxis)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Label lblChartAreaTitle;
        private System.Windows.Forms.Label lblYTitle;
        private System.Windows.Forms.Label lblXTitle;
        private System.Windows.Forms.Panel pnlChartControlHolder;
        private YAxis yAxis;
        private XAxis xAxis;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel pnlCBOView;
        private System.Windows.Forms.ComboBox cboView;
    }
}
