namespace vApus.Util
{
    partial class Chart
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
            this.split = new System.Windows.Forms.SplitContainer();
            this.chartArea = new vApus.Util.ChartArea();
            this.llblCollapseExpand = new System.Windows.Forms.LinkLabel();
            this.legend = new vApus.Util.Legend();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.SuspendLayout();
            // 
            // split
            // 
            this.split.BackColor = System.Drawing.Color.DimGray;
            this.split.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.Location = new System.Drawing.Point(0, 0);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.chartArea);
            this.split.Panel1.Controls.Add(this.llblCollapseExpand);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.legend);
            this.split.Size = new System.Drawing.Size(450, 250);
            this.split.SplitterDistance = 315;
            this.split.SplitterWidth = 1;
            this.split.TabIndex = 0;
            // 
            // chartArea
            // 
            this.chartArea.AutoScrollXAxis = vApus.Util.AutoScrollXAxis.KeepAtEnd;
            this.chartArea.BackColor = System.Drawing.Color.White;
            this.chartArea.ChartAreaTitleText = "Title";
            this.chartArea.ChartAreaTitleTextVisible = true;
            this.chartArea.ChartType = vApus.Util.ChartType.LineChart;
            this.chartArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartArea.Location = new System.Drawing.Point(0, 0);
            this.chartArea.Name = "chartArea";
            this.chartArea.Size = new System.Drawing.Size(294, 250);
            this.chartArea.TabIndex = 2;
            this.chartArea.XTitleText = "X";
            this.chartArea.XTitleVisible = true;
            this.chartArea.YAxisViewState = vApus.Util.YAxisViewState.OnlyShowSelected;
            this.chartArea.YTitleText = "Y";
            this.chartArea.YTitleVisible = true;
            // 
            // llblCollapseExpand
            // 
            this.llblCollapseExpand.ActiveLinkColor = System.Drawing.Color.Blue;
            this.llblCollapseExpand.BackColor = System.Drawing.Color.White;
            this.llblCollapseExpand.DisabledLinkColor = System.Drawing.Color.Blue;
            this.llblCollapseExpand.Dock = System.Windows.Forms.DockStyle.Right;
            this.llblCollapseExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llblCollapseExpand.ForeColor = System.Drawing.Color.Blue;
            this.llblCollapseExpand.Location = new System.Drawing.Point(294, 0);
            this.llblCollapseExpand.Name = "llblCollapseExpand";
            this.llblCollapseExpand.Size = new System.Drawing.Size(21, 250);
            this.llblCollapseExpand.TabIndex = 1;
            this.llblCollapseExpand.TabStop = true;
            this.llblCollapseExpand.Text = ">>";
            this.llblCollapseExpand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.llblCollapseExpand, "Hide Legend");
            this.llblCollapseExpand.Click += new System.EventHandler(this.llblCollapseExpand_Click);
            // 
            // legend
            // 
            this.legend.BackColor = System.Drawing.Color.White;
            this.legend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.legend.Location = new System.Drawing.Point(0, 0);
            this.legend.Name = "legend";
            this.legend.Size = new System.Drawing.Size(134, 250);
            this.legend.TabIndex = 1;
            // 
            // Chart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.Controls.Add(this.split);
            this.MinimumSize = new System.Drawing.Size(450, 250);
            this.Name = "Chart";
            this.Size = new System.Drawing.Size(450, 250);
            this.SizeChanged += new System.EventHandler(this.Chart_SizeChanged);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer split;
        private Legend legend;
        private System.Windows.Forms.LinkLabel llblCollapseExpand;
        private ChartArea chartArea;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
