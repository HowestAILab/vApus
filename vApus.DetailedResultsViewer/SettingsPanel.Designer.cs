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
            this.lblConnectToMySQL = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblResults = new System.Windows.Forms.Label();
            this.picDatabases = new System.Windows.Forms.PictureBox();
            this.dgvDatabases = new System.Windows.Forms.DataGridView();
            this.llblRefresh = new System.Windows.Forms.LinkLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.filterResults = new vApus.DetailedResultsViewer.FilterResults();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDatabases)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatabases)).BeginInit();
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
            this.lblResults.Location = new System.Drawing.Point(29, 182);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(168, 16);
            this.lblResults.TabIndex = 9;
            this.lblResults.Text = "Select a Results Database";
            // 
            // picDatabases
            // 
            this.picDatabases.Image = ((System.Drawing.Image)(resources.GetObject("picDatabases.Image")));
            this.picDatabases.Location = new System.Drawing.Point(12, 182);
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
            this.dgvDatabases.Location = new System.Drawing.Point(32, 201);
            this.dgvDatabases.Name = "dgvDatabases";
            this.dgvDatabases.ReadOnly = true;
            this.dgvDatabases.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvDatabases.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvDatabases.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDatabases.Size = new System.Drawing.Size(502, 284);
            this.dgvDatabases.TabIndex = 2;
            this.dgvDatabases.VirtualMode = true;
            this.dgvDatabases.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDatabases_RowEnter);
            // 
            // llblRefresh
            // 
            this.llblRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llblRefresh.AutoSize = true;
            this.llblRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.llblRefresh.Location = new System.Drawing.Point(490, 184);
            this.llblRefresh.Name = "llblRefresh";
            this.llblRefresh.Size = new System.Drawing.Size(44, 13);
            this.llblRefresh.TabIndex = 12;
            this.llblRefresh.TabStop = true;
            this.llblRefresh.Text = "Refresh";
            this.llblRefresh.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblRefresh_LinkClicked);
            // 
            // filterResults
            // 
            this.filterResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterResults.BackColor = System.Drawing.Color.White;
            this.filterResults.Location = new System.Drawing.Point(9, 54);
            this.filterResults.Name = "filterResults";
            this.filterResults.Size = new System.Drawing.Size(525, 115);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lblConnectToMySQL;
        private System.Windows.Forms.PictureBox pictureBox1;
        private FilterResults filterResults;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.PictureBox picDatabases;
        private System.Windows.Forms.DataGridView dgvDatabases;
        private System.Windows.Forms.LinkLabel llblRefresh;
        private System.Windows.Forms.ToolTip toolTip;
    }
}