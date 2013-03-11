namespace vApus.Stresstest {
    partial class SaveChartsDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveChartsDialog));
            this.btnSaveCharts = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.picOverview = new System.Windows.Forms.PictureBox();
            this.flpCharts = new System.Windows.Forms.FlowLayoutPanel();
            this.picTop5HeaviestUserActions = new System.Windows.Forms.PictureBox();
            this.picMonitor = new System.Windows.Forms.PictureBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlBorderStresstest = new System.Windows.Forms.Panel();
            this.cboStresstest = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.picOverview)).BeginInit();
            this.flpCharts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTop5HeaviestUserActions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonitor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.pnlBorderStresstest.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveCharts
            // 
            this.btnSaveCharts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveCharts.AutoSize = true;
            this.btnSaveCharts.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveCharts.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveCharts.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnSaveCharts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCharts.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveCharts.Location = new System.Drawing.Point(560, 244);
            this.btnSaveCharts.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnSaveCharts.Name = "btnSaveCharts";
            this.btnSaveCharts.Size = new System.Drawing.Size(100, 24);
            this.btnSaveCharts.TabIndex = 1;
            this.btnSaveCharts.Text = "Save Charts...";
            this.btnSaveCharts.UseVisualStyleBackColor = false;
            this.btnSaveCharts.Click += new System.EventHandler(this.btnSaveCharts_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xlsx";
            this.saveFileDialog.FileName = "charts.xlsx";
            this.saveFileDialog.Filter = "Excel files|*.xlsx";
            // 
            // picOverview
            // 
            this.picOverview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picOverview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picOverview.Image = ((System.Drawing.Image)(resources.GetObject("picOverview.Image")));
            this.picOverview.Location = new System.Drawing.Point(3, 3);
            this.picOverview.Name = "picOverview";
            this.picOverview.Size = new System.Drawing.Size(141, 89);
            this.picOverview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picOverview.TabIndex = 6;
            this.picOverview.TabStop = false;
            this.toolTip.SetToolTip(this.picOverview, "Cummulative Response Times vs Achieved Throughput Example");
            this.picOverview.Click += new System.EventHandler(this.pic_Click);
            // 
            // flpCharts
            // 
            this.flpCharts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpCharts.AutoScroll = true;
            this.flpCharts.Controls.Add(this.picOverview);
            this.flpCharts.Controls.Add(this.picTop5HeaviestUserActions);
            this.flpCharts.Controls.Add(this.picMonitor);
            this.flpCharts.Location = new System.Drawing.Point(12, 63);
            this.flpCharts.Name = "flpCharts";
            this.flpCharts.Size = new System.Drawing.Size(648, 175);
            this.flpCharts.TabIndex = 7;
            // 
            // picTop5HeaviestUserActions
            // 
            this.picTop5HeaviestUserActions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTop5HeaviestUserActions.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picTop5HeaviestUserActions.Image = ((System.Drawing.Image)(resources.GetObject("picTop5HeaviestUserActions.Image")));
            this.picTop5HeaviestUserActions.Location = new System.Drawing.Point(150, 3);
            this.picTop5HeaviestUserActions.Name = "picTop5HeaviestUserActions";
            this.picTop5HeaviestUserActions.Size = new System.Drawing.Size(141, 89);
            this.picTop5HeaviestUserActions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picTop5HeaviestUserActions.TabIndex = 7;
            this.picTop5HeaviestUserActions.TabStop = false;
            this.toolTip.SetToolTip(this.picTop5HeaviestUserActions, "Top 5 Heaviest User Actions Example");
            this.picTop5HeaviestUserActions.Click += new System.EventHandler(this.pic_Click);
            // 
            // picMonitor
            // 
            this.picMonitor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picMonitor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picMonitor.Image = ((System.Drawing.Image)(resources.GetObject("picMonitor.Image")));
            this.picMonitor.Location = new System.Drawing.Point(297, 3);
            this.picMonitor.Name = "picMonitor";
            this.picMonitor.Size = new System.Drawing.Size(141, 89);
            this.picMonitor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picMonitor.TabIndex = 8;
            this.picMonitor.TabStop = false;
            this.toolTip.SetToolTip(this.picMonitor, "Monitor Example");
            this.picMonitor.Click += new System.EventHandler(this.pic_Click);
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(12, 13);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(530, 32);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.Text = "Charts will be saved in an Excel file.\r\nBelow you can see examples of the differe" +
    "nt chart types that the spreadsheet will contain.";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(15, 249);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 18;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(37, 249);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 16);
            this.label1.TabIndex = 17;
            this.label1.Text = "Select a Stresstest";
            // 
            // pnlBorderStresstest
            // 
            this.pnlBorderStresstest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBorderStresstest.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderStresstest.Controls.Add(this.cboStresstest);
            this.pnlBorderStresstest.Location = new System.Drawing.Point(162, 245);
            this.pnlBorderStresstest.Name = "pnlBorderStresstest";
            this.pnlBorderStresstest.Size = new System.Drawing.Size(300, 23);
            this.pnlBorderStresstest.TabIndex = 0;
            // 
            // cboStresstest
            // 
            this.cboStresstest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboStresstest.BackColor = System.Drawing.Color.White;
            this.cboStresstest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStresstest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboStresstest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboStresstest.FormattingEnabled = true;
            this.cboStresstest.Location = new System.Drawing.Point(1, 1);
            this.cboStresstest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboStresstest.Name = "cboStresstest";
            this.cboStresstest.Size = new System.Drawing.Size(298, 21);
            this.cboStresstest.TabIndex = 0;
            // 
            // SaveChartsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(672, 280);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlBorderStresstest);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.flpCharts);
            this.Controls.Add(this.btnSaveCharts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SaveChartsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Save Charts";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaveChartsDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.picOverview)).EndInit();
            this.flpCharts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTop5HeaviestUserActions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonitor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.pnlBorderStresstest.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSaveCharts;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.PictureBox picOverview;
        private System.Windows.Forms.FlowLayoutPanel flpCharts;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picTop5HeaviestUserActions;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlBorderStresstest;
        private System.Windows.Forms.ComboBox cboStresstest;
        private System.Windows.Forms.PictureBox picMonitor;
    }
}