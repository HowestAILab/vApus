using vApus.Util;
namespace vApus.Monitor
{
    partial class AccordeonControl
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnToggle = new System.Windows.Forms.Button();
            this.flp = new System.Windows.Forms.FlowLayoutPanel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.chart = new vApus.Util.Chart();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.flp.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.MinimumSize = new System.Drawing.Size(100, 28);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer.Panel1.Controls.Add(this.btnSave);
            this.splitContainer.Panel1.Controls.Add(this.btnToggle);
            this.splitContainer.Panel1.Controls.Add(this.flp);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.chart);
            this.splitContainer.Size = new System.Drawing.Size(700, 350);
            this.splitContainer.SplitterDistance = 28;
            this.splitContainer.SplitterWidth = 1;
            this.splitContainer.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.BackColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(614, 2);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2, 3, 3, 3);
            this.btnSave.MaximumSize = new System.Drawing.Size(60, 24);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 24);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnToggle
            // 
            this.btnToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToggle.BackColor = System.Drawing.Color.White;
            this.btnToggle.FlatAppearance.BorderSize = 0;
            this.btnToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnToggle.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnToggle.Location = new System.Drawing.Point(677, 3);
            this.btnToggle.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnToggle.Name = "btnToggle";
            this.btnToggle.Size = new System.Drawing.Size(20, 21);
            this.btnToggle.TabIndex = 3;
            this.btnToggle.Text = "-";
            this.btnToggle.UseVisualStyleBackColor = false;
            this.btnToggle.Click += new System.EventHandler(this.btnToggle_Click);
            // 
            // flp
            // 
            this.flp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flp.AutoScroll = true;
            this.flp.BackColor = System.Drawing.Color.White;
            this.flp.Controls.Add(this.lblHeader);
            this.flp.Location = new System.Drawing.Point(1, 1);
            this.flp.Margin = new System.Windows.Forms.Padding(0);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(611, 26);
            this.flp.TabIndex = 2;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblHeader.Location = new System.Drawing.Point(1, 3);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(1, 3, 6, 3);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(96, 20);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Header Text";
            // 
            // chart
            // 
            this.chart.BackColor = System.Drawing.Color.DimGray;
            this.chart.ChartAreaTitleText = "Title";
            this.chart.ChartAreaTitleTextVisible = false;
            this.chart.ChartType = vApus.Util.ChartType.LineChart;
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.LegendViewState = vApus.Util.LegendViewState.Collapsed;
            this.chart.Location = new System.Drawing.Point(0, 0);
            this.chart.MinimumSize = new System.Drawing.Size(450, 250);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(700, 321);
            this.chart.TabIndex = 0;
            this.chart.XTitleText = "X";
            this.chart.XTitleVisible = true;
            this.chart.YAxisViewState = vApus.Util.YAxisViewState.OnlyShowSelected;
            this.chart.YTitleText = "Y";
            this.chart.YTitleVisible = true;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "CSV Files | *.csv";
            // 
            // AccordeonControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitContainer);
            this.MinimumSize = new System.Drawing.Size(100, 28);
            this.Name = "AccordeonControl";
            this.Size = new System.Drawing.Size(700, 350);
            this.SizeChanged += new System.EventHandler(this.AccordeonControl_SizeChanged);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.flp.ResumeLayout(false);
            this.flp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button btnToggle;
        private System.Windows.Forms.FlowLayoutPanel flp;
        private Chart chart;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Label lblHeader;
    }
}