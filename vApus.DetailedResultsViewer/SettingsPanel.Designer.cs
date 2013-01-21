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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsPanel));
            this.lblConnectToMySQL = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lvwDatabases = new System.Windows.Forms.ListView();
            this.lblDatabases = new System.Windows.Forms.Label();
            this.picDatabases = new System.Windows.Forms.PictureBox();
            this.filterDatabases = new vApus.DetailedResultsViewer.FilterDatabases();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDatabases)).BeginInit();
            this.SuspendLayout();
            // 
            // lblConnectToMySQL
            // 
            this.lblConnectToMySQL.AutoSize = true;
            this.lblConnectToMySQL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectToMySQL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblConnectToMySQL.Location = new System.Drawing.Point(32, 14);
            this.lblConnectToMySQL.Name = "lblConnectToMySQL";
            this.lblConnectToMySQL.Size = new System.Drawing.Size(179, 16);
            this.lblConnectToMySQL.TabIndex = 0;
            this.lblConnectToMySQL.TabStop = true;
            this.lblConnectToMySQL.Text = "Connect to a MySQL server...";
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
            // lvwDatabases
            // 
            this.lvwDatabases.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwDatabases.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwDatabases.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwDatabases.HideSelection = false;
            this.lvwDatabases.Location = new System.Drawing.Point(31, 161);
            this.lvwDatabases.Name = "lvwDatabases";
            this.lvwDatabases.Size = new System.Drawing.Size(501, 324);
            this.lvwDatabases.TabIndex = 7;
            this.lvwDatabases.UseCompatibleStateImageBehavior = false;
            this.lvwDatabases.View = System.Windows.Forms.View.List;
            this.lvwDatabases.ItemActivate += new System.EventHandler(this.lvwDatabases_ItemActivate);
            // 
            // lblDatabases
            // 
            this.lblDatabases.AutoSize = true;
            this.lblDatabases.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDatabases.Location = new System.Drawing.Point(29, 142);
            this.lblDatabases.Name = "lblDatabases";
            this.lblDatabases.Size = new System.Drawing.Size(75, 16);
            this.lblDatabases.TabIndex = 9;
            this.lblDatabases.Text = "Databases";
            // 
            // picDatabases
            // 
            this.picDatabases.Image = ((System.Drawing.Image)(resources.GetObject("picDatabases.Image")));
            this.picDatabases.Location = new System.Drawing.Point(12, 142);
            this.picDatabases.Name = "picDatabases";
            this.picDatabases.Size = new System.Drawing.Size(16, 16);
            this.picDatabases.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picDatabases.TabIndex = 10;
            this.picDatabases.TabStop = false;
            // 
            // filterDatabases
            // 
            this.filterDatabases.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterDatabases.BackColor = System.Drawing.Color.White;
            this.filterDatabases.Location = new System.Drawing.Point(9, 44);
            this.filterDatabases.Name = "filterDatabases";
            this.filterDatabases.Size = new System.Drawing.Size(525, 95);
            this.filterDatabases.TabIndex = 8;
            this.filterDatabases.CollapsedOrExpandedTags += new System.EventHandler(this.filterDatabases_CollapsedOrExpandedTags);
            // 
            // SettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(546, 497);
            this.Controls.Add(this.picDatabases);
            this.Controls.Add(this.lblDatabases);
            this.Controls.Add(this.filterDatabases);
            this.Controls.Add(this.lvwDatabases);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblConnectToMySQL);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsPanel";
            this.Text = "Databases";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDatabases)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lblConnectToMySQL;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListView lvwDatabases;
        private FilterDatabases filterDatabases;
        private System.Windows.Forms.Label lblDatabases;
        private System.Windows.Forms.PictureBox picDatabases;
    }
}