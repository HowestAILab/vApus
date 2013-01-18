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
            this.label1 = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lvwDatabases = new System.Windows.Forms.ListView();
            this.clmDatabase = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filterDatabases = new vApus.DetailedResultsViewer.Filter();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(20, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 13);
            this.label1.TabIndex = 0;
            this.label1.TabStop = true;
            this.label1.Text = "Connect to a MySQL server...";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 12);
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
            this.lvwDatabases.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwDatabases.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmDatabase});
            this.lvwDatabases.Location = new System.Drawing.Point(23, 194);
            this.lvwDatabases.Name = "lvwDatabases";
            this.lvwDatabases.Size = new System.Drawing.Size(511, 291);
            this.lvwDatabases.TabIndex = 7;
            this.lvwDatabases.UseCompatibleStateImageBehavior = false;
            this.lvwDatabases.View = System.Windows.Forms.View.List;
            // 
            // clmDatabase
            // 
            this.clmDatabase.Text = "Database";
            this.clmDatabase.Width = 117;
            // 
            // filterDatabases
            // 
            this.filterDatabases.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterDatabases.BackColor = System.Drawing.Color.White;
            this.filterDatabases.Location = new System.Drawing.Point(3, 34);
            this.filterDatabases.Name = "filterDatabases";
            this.filterDatabases.Size = new System.Drawing.Size(531, 154);
            this.filterDatabases.TabIndex = 8;
            // 
            // SettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(546, 497);
            this.Controls.Add(this.filterDatabases);
            this.Controls.Add(this.lvwDatabases);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsPanel";
            this.Text = "Databases";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListView lvwDatabases;
        private System.Windows.Forms.ColumnHeader clmDatabase;
        private Filter filterDatabases;
    }
}