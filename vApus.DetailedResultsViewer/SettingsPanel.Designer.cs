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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblConnectToMySQL = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblResults = new System.Windows.Forms.Label();
            this.picDatabases = new System.Windows.Forms.PictureBox();
            this.dgvDatabases = new System.Windows.Forms.DataGridView();
            this.llblRefresh = new System.Windows.Forms.LinkLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pnlBorderStresstest = new System.Windows.Forms.Panel();
            this.cboStresstest = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnDeleteSelectedDbs = new System.Windows.Forms.Button();
            this.filterResults = new vApus.DetailedResultsViewer.FilterResultsControl();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDatabases)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatabases)).BeginInit();
            this.pnlBorderStresstest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblConnectToMySQL
            // 
            this.lblConnectToMySQL.AutoSize = true;
            this.lblConnectToMySQL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectToMySQL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblConnectToMySQL.Location = new System.Drawing.Point(32, 14);
            this.lblConnectToMySQL.Name = "lblConnectToMySQL";
            this.lblConnectToMySQL.Size = new System.Drawing.Size(229, 16);
            this.lblConnectToMySQL.TabIndex = 0;
            this.lblConnectToMySQL.TabStop = true;
            this.lblConnectToMySQL.Text = "Connect to a Results MySQL Server...";
            this.lblConnectToMySQL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblConnectToMySQL_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 14);
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
            this.lblResults.Location = new System.Drawing.Point(29, 217);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(168, 16);
            this.lblResults.TabIndex = 9;
            this.lblResults.Text = "Select a Results Database";
            // 
            // picDatabases
            // 
            this.picDatabases.Image = ((System.Drawing.Image)(resources.GetObject("picDatabases.Image")));
            this.picDatabases.Location = new System.Drawing.Point(12, 217);
            this.picDatabases.Name = "picDatabases";
            this.picDatabases.Size = new System.Drawing.Size(16, 16);
            this.picDatabases.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picDatabases.TabIndex = 10;
            this.picDatabases.TabStop = false;
            // 
            // dgvDatabases
            // 
            this.dgvDatabases.MultiSelect = true;
            this.dgvDatabases.AllowUserToAddRows = false;
            this.dgvDatabases.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvDatabases.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDatabases.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDatabases.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvDatabases.BackgroundColor = System.Drawing.Color.White;
            this.dgvDatabases.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDatabases.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvDatabases.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvDatabases.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDatabases.EnableHeadersVisualStyles = false;
            this.dgvDatabases.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.dgvDatabases.Location = new System.Drawing.Point(32, 236);
            this.dgvDatabases.Name = "dgvDatabases";
            this.dgvDatabases.ReadOnly = true;
            this.dgvDatabases.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvDatabases.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgvDatabases.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDatabases.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDatabases.Size = new System.Drawing.Size(502, 165);
            this.dgvDatabases.TabIndex = 2;
            this.dgvDatabases.VirtualMode = true;
            this.dgvDatabases.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDatabases_RowEnter);
            // 
            // llblRefresh
            // 
            this.llblRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llblRefresh.AutoSize = true;
            this.llblRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.llblRefresh.Location = new System.Drawing.Point(490, 217);
            this.llblRefresh.Name = "llblRefresh";
            this.llblRefresh.Size = new System.Drawing.Size(44, 13);
            this.llblRefresh.TabIndex = 12;
            this.llblRefresh.TabStop = true;
            this.llblRefresh.Text = "Refresh";
            this.llblRefresh.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblRefresh_LinkClicked);
            // 
            // pnlBorderStresstest
            // 
            this.pnlBorderStresstest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBorderStresstest.BackColor = System.Drawing.Color.Silver;
            this.pnlBorderStresstest.Controls.Add(this.cboStresstest);
            this.pnlBorderStresstest.Location = new System.Drawing.Point(158, 465);
            this.pnlBorderStresstest.Name = "pnlBorderStresstest";
            this.pnlBorderStresstest.Size = new System.Drawing.Size(377, 23);
            this.pnlBorderStresstest.TabIndex = 13;
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
            this.cboStresstest.Size = new System.Drawing.Size(375, 21);
            this.cboStresstest.TabIndex = 0;
            this.cboStresstest.SelectedIndexChanged += new System.EventHandler(this.cboStresstest_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(33, 469);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 16);
            this.label1.TabIndex = 14;
            this.label1.Text = "Select a Stresstest";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(12, 469);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 15;
            this.pictureBox2.TabStop = false;
            // 
            // btnDeleteSelectedDbs
            // 
            this.btnDeleteSelectedDbs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteSelectedDbs.AutoSize = true;
            this.btnDeleteSelectedDbs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDeleteSelectedDbs.BackColor = System.Drawing.SystemColors.Control;
            this.btnDeleteSelectedDbs.Enabled = false;
            this.btnDeleteSelectedDbs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteSelectedDbs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteSelectedDbs.Location = new System.Drawing.Point(441, 407);
            this.btnDeleteSelectedDbs.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnDeleteSelectedDbs.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnDeleteSelectedDbs.Name = "btnDeleteSelectedDbs";
            this.btnDeleteSelectedDbs.Size = new System.Drawing.Size(94, 24);
            this.btnDeleteSelectedDbs.TabIndex = 16;
            this.btnDeleteSelectedDbs.Text = "Delete Selected";
            this.btnDeleteSelectedDbs.UseVisualStyleBackColor = false;
            this.btnDeleteSelectedDbs.Click += new System.EventHandler(this.btnDeleteSelectedDbs_Click);
            // 
            // filterResults
            // 
            this.filterResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterResults.BackColor = System.Drawing.Color.White;
            this.filterResults.Location = new System.Drawing.Point(9, 54);
            this.filterResults.Name = "filterResults";
            this.filterResults.Size = new System.Drawing.Size(525, 150);
            this.filterResults.TabIndex = 1;
            this.filterResults.FilterChanged += new System.EventHandler(this.filterDatabases_FilterChanged);
            // 
            // SettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(546, 497);
            this.Controls.Add(this.btnDeleteSelectedDbs);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlBorderStresstest);
            this.Controls.Add(this.llblRefresh);
            this.Controls.Add(this.dgvDatabases);
            this.Controls.Add(this.picDatabases);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.filterResults);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblConnectToMySQL);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsPanel";
            this.Text = "Select a Results Database";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDatabases)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatabases)).EndInit();
            this.pnlBorderStresstest.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lblConnectToMySQL;
        private System.Windows.Forms.PictureBox pictureBox1;
        private vApus.DetailedResultsViewer.FilterResultsControl filterResults;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.PictureBox picDatabases;
        private System.Windows.Forms.DataGridView dgvDatabases;
        private System.Windows.Forms.LinkLabel llblRefresh;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel pnlBorderStresstest;
        private System.Windows.Forms.ComboBox cboStresstest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnDeleteSelectedDbs;
    }
}