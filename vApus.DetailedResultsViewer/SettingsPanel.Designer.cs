namespace vApus.DetailedResultsViewer {
    partial class SettingsPanel {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsPanel));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblConnectToMySQL = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblResults = new System.Windows.Forms.Label();
            this.picDatabases = new System.Windows.Forms.PictureBox();
            this.dgvDatabases = new System.Windows.Forms.DataGridView();
            this.llblRefresh = new System.Windows.Forms.LinkLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pnlBorderStressTest = new System.Windows.Forms.Panel();
            this.cboStressTest = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnDeleteSelectedDbs = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnOverviewExportToExcel = new System.Windows.Forms.Button();
            this.filterResults = new vApus.DetailedResultsViewer.FilterResultsControl();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDatabases)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatabases)).BeginInit();
            this.pnlBorderStressTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // filterResults
            // 
            this.filterResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom)));
            this.filterResults.BackColor = System.Drawing.Color.White;
            this.filterResults.Location = new System.Drawing.Point(9, 54);
            this.filterResults.Name = "filterResults";
            this.filterResults.Size = new System.Drawing.Size(520, 100);
            this.filterResults.TabIndex = 1;
            this.filterResults.FilterChanged += new System.EventHandler(this.filterDatabases_FilterChanged);
            // 
            // lblConnectToMySQL
            // 
            this.lblConnectToMySQL.AutoSize = true;
            this.lblConnectToMySQL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectToMySQL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblConnectToMySQL.Location = new System.Drawing.Point(32, 12);
            this.lblConnectToMySQL.Name = "lblConnectToMySQL";
            this.lblConnectToMySQL.Size = new System.Drawing.Size(221, 16);
            this.lblConnectToMySQL.TabIndex = 0;
            this.lblConnectToMySQL.TabStop = true;
            this.lblConnectToMySQL.Text = "Connect to a results MySQL server...";
            this.lblConnectToMySQL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblConnectToMySQL_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResults.Location = new System.Drawing.Point(29, 13);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(160, 16);
            this.lblResults.TabIndex = 9;
            this.lblResults.Text = "Select a results database";
            // 
            // picDatabases
            // 
            this.picDatabases.Image = ((System.Drawing.Image)(resources.GetObject("picDatabases.Image")));
            this.picDatabases.Location = new System.Drawing.Point(12, 12);
            this.picDatabases.Name = "picDatabases";
            this.picDatabases.Size = new System.Drawing.Size(16, 16);
            this.picDatabases.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picDatabases.TabIndex = 10;
            this.picDatabases.TabStop = false;
            // 
            // dgvDatabases
            // 
            this.dgvDatabases.AllowUserToAddRows = false;
            this.dgvDatabases.AllowUserToDeleteRows = false;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.White;
            this.dgvDatabases.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvDatabases.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDatabases.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvDatabases.BackgroundColor = System.Drawing.Color.White;
            this.dgvDatabases.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDatabases.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvDatabases.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvDatabases.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDatabases.EnableHeadersVisualStyles = false;
            this.dgvDatabases.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.dgvDatabases.Location = new System.Drawing.Point(23, 39);
            this.dgvDatabases.Name = "dgvDatabases";
            this.dgvDatabases.ReadOnly = true;
            this.dgvDatabases.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvDatabases.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgvDatabases.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvDatabases.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDatabases.Size = new System.Drawing.Size(502, 195);
            this.dgvDatabases.TabIndex = 2;
            this.dgvDatabases.VirtualMode = true;
            this.dgvDatabases.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDatabases_RowEnter);
            // 
            // llblRefresh
            // 
            this.llblRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llblRefresh.AutoSize = true;
            this.llblRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.llblRefresh.Location = new System.Drawing.Point(499, 14);
            this.llblRefresh.Name = "llblRefresh";
            this.llblRefresh.Size = new System.Drawing.Size(44, 13);
            this.llblRefresh.TabIndex = 12;
            this.llblRefresh.TabStop = true;
            this.llblRefresh.Text = "Refresh";
            this.llblRefresh.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblRefresh_LinkClicked);
            // 
            // pnlBorderStressTest
            // 
            this.pnlBorderStressTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBorderStressTest.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderStressTest.Controls.Add(this.cboStressTest);
            this.pnlBorderStressTest.Location = new System.Drawing.Point(172, 290);
            this.pnlBorderStressTest.Name = "pnlBorderStressTest";
            this.pnlBorderStressTest.Size = new System.Drawing.Size(353, 23);
            this.pnlBorderStressTest.TabIndex = 20;
            // 
            // cboStressTest
            // 
            this.cboStressTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboStressTest.BackColor = System.Drawing.Color.White;
            this.cboStressTest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStressTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboStressTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboStressTest.FormattingEnabled = true;
            this.cboStressTest.Location = new System.Drawing.Point(1, 1);
            this.cboStressTest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cboStressTest.Name = "cboStressTest";
            this.cboStressTest.Size = new System.Drawing.Size(351, 21);
            this.cboStressTest.TabIndex = 0;
            this.cboStressTest.SelectedIndexChanged += new System.EventHandler(this.cboStressTest_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(33, 293);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 16);
            this.label1.TabIndex = 14;
            this.label1.Text = "Select a stress test";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(12, 293);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 15;
            this.pictureBox2.TabStop = false;
            // 
            // btnDeleteSelectedDbs
            // 
            this.btnDeleteSelectedDbs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteSelectedDbs.AutoSize = true;
            this.btnDeleteSelectedDbs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDeleteSelectedDbs.BackColor = System.Drawing.SystemColors.Control;
            this.btnDeleteSelectedDbs.Enabled = false;
            this.btnDeleteSelectedDbs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteSelectedDbs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteSelectedDbs.Location = new System.Drawing.Point(417, 240);
            this.btnDeleteSelectedDbs.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnDeleteSelectedDbs.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnDeleteSelectedDbs.Name = "btnDeleteSelectedDbs";
            this.btnDeleteSelectedDbs.Size = new System.Drawing.Size(108, 24);
            this.btnDeleteSelectedDbs.TabIndex = 16;
            this.btnDeleteSelectedDbs.Text = "Delete selected";
            this.btnDeleteSelectedDbs.UseVisualStyleBackColor = false;
            this.btnDeleteSelectedDbs.Click += new System.EventHandler(this.btnDeleteSelectedDbs_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel1.Controls.Add(this.lblConnectToMySQL);
            this.splitContainer1.Panel1.Controls.Add(this.llblRefresh);
            this.splitContainer1.Panel1.Controls.Add(this.filterResults);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.btnOverviewExportToExcel);
            this.splitContainer1.Panel2.Controls.Add(this.picDatabases);
            this.splitContainer1.Panel2.Controls.Add(this.pnlBorderStressTest);
            this.splitContainer1.Panel2.Controls.Add(this.btnDeleteSelectedDbs);
            this.splitContainer1.Panel2.Controls.Add(this.lblResults);
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox2);
            this.splitContainer1.Panel2.Controls.Add(this.dgvDatabases);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(546, 497);
            this.splitContainer1.SplitterDistance = 168;
            this.splitContainer1.TabIndex = 17;
            // 
            // btnOverviewExportToExcel
            // 
            this.btnOverviewExportToExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOverviewExportToExcel.AutoSize = true;
            this.btnOverviewExportToExcel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOverviewExportToExcel.BackColor = System.Drawing.SystemColors.Control;
            this.btnOverviewExportToExcel.Enabled = false;
            this.btnOverviewExportToExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOverviewExportToExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOverviewExportToExcel.Location = new System.Drawing.Point(23, 240);
            this.btnOverviewExportToExcel.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnOverviewExportToExcel.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnOverviewExportToExcel.Name = "btnOverviewExportToExcel";
            this.btnOverviewExportToExcel.Size = new System.Drawing.Size(172, 24);
            this.btnOverviewExportToExcel.TabIndex = 15;
            this.btnOverviewExportToExcel.Text = "Export overview to Excel...";
            this.btnOverviewExportToExcel.UseVisualStyleBackColor = false;
            this.btnOverviewExportToExcel.Click += new System.EventHandler(this.btnOverviewExportToExcel_Click);
            // 
            // SettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(546, 497);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsPanel";
            this.Text = "Select a results database";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDatabases)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatabases)).EndInit();
            this.pnlBorderStressTest.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel lblConnectToMySQL;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.PictureBox picDatabases;
        private System.Windows.Forms.DataGridView dgvDatabases;
        private System.Windows.Forms.LinkLabel llblRefresh;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel pnlBorderStressTest;
        private System.Windows.Forms.ComboBox cboStressTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnDeleteSelectedDbs;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnOverviewExportToExcel;
        private vApus.DetailedResultsViewer.FilterResultsControl filterResults;
    }
}