namespace vApus.Gui
{
    partial class CleanTempDataPanel
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDeleteUpdateTempFiles = new System.Windows.Forms.Button();
            this.btnOpenUpdateTempFiles = new System.Windows.Forms.Button();
            this.btnDeleteLogs = new System.Windows.Forms.Button();
            this.btnOpenLogs = new System.Windows.Forms.Button();
            this.btnDeleteConnectionProxyTempFiles = new System.Windows.Forms.Button();
            this.btnOpenConnectionProxyTempFiles = new System.Windows.Forms.Button();
            this.btnDeleteSlaveSideResults = new System.Windows.Forms.Button();
            this.btnOpenSlaveSideResults = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnDeleteUpdateTempFiles);
            this.groupBox1.Controls.Add(this.btnOpenUpdateTempFiles);
            this.groupBox1.Controls.Add(this.btnDeleteLogs);
            this.groupBox1.Controls.Add(this.btnOpenLogs);
            this.groupBox1.Controls.Add(this.btnDeleteConnectionProxyTempFiles);
            this.groupBox1.Controls.Add(this.btnOpenConnectionProxyTempFiles);
            this.groupBox1.Location = new System.Drawing.Point(12, 109);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(397, 129);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // btnDeleteUpdateTempFiles
            // 
            this.btnDeleteUpdateTempFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteUpdateTempFiles.AutoSize = true;
            this.btnDeleteUpdateTempFiles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDeleteUpdateTempFiles.BackColor = System.Drawing.Color.White;
            this.btnDeleteUpdateTempFiles.Enabled = false;
            this.btnDeleteUpdateTempFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteUpdateTempFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteUpdateTempFiles.Image = global::vApus.Gui.Properties.Resources.delete16x16;
            this.btnDeleteUpdateTempFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDeleteUpdateTempFiles.Location = new System.Drawing.Point(367, 82);
            this.btnDeleteUpdateTempFiles.Name = "btnDeleteUpdateTempFiles";
            this.btnDeleteUpdateTempFiles.Size = new System.Drawing.Size(24, 24);
            this.btnDeleteUpdateTempFiles.TabIndex = 7;
            this.btnDeleteUpdateTempFiles.Tag = "UpdateTempFiles";
            this.btnDeleteUpdateTempFiles.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDeleteUpdateTempFiles.UseVisualStyleBackColor = false;
            this.btnDeleteUpdateTempFiles.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnOpenUpdateTempFiles
            // 
            this.btnOpenUpdateTempFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenUpdateTempFiles.BackColor = System.Drawing.Color.White;
            this.btnOpenUpdateTempFiles.Enabled = false;
            this.btnOpenUpdateTempFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenUpdateTempFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenUpdateTempFiles.Image = global::vApus.Gui.Properties.Resources.FolderOpen_16x16_72;
            this.btnOpenUpdateTempFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenUpdateTempFiles.Location = new System.Drawing.Point(6, 81);
            this.btnOpenUpdateTempFiles.Name = "btnOpenUpdateTempFiles";
            this.btnOpenUpdateTempFiles.Size = new System.Drawing.Size(355, 25);
            this.btnOpenUpdateTempFiles.TabIndex = 6;
            this.btnOpenUpdateTempFiles.Tag = "UpdateTempFiles";
            this.btnOpenUpdateTempFiles.Text = "     UpdateTempFiles... [?MB]";
            this.btnOpenUpdateTempFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenUpdateTempFiles.UseVisualStyleBackColor = false;
            this.btnOpenUpdateTempFiles.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnDeleteLogs
            // 
            this.btnDeleteLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteLogs.AutoSize = true;
            this.btnDeleteLogs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDeleteLogs.BackColor = System.Drawing.Color.White;
            this.btnDeleteLogs.Enabled = false;
            this.btnDeleteLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteLogs.Image = global::vApus.Gui.Properties.Resources.delete16x16;
            this.btnDeleteLogs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDeleteLogs.Location = new System.Drawing.Point(367, 51);
            this.btnDeleteLogs.Name = "btnDeleteLogs";
            this.btnDeleteLogs.Size = new System.Drawing.Size(24, 24);
            this.btnDeleteLogs.TabIndex = 5;
            this.btnDeleteLogs.Tag = "Logs";
            this.btnDeleteLogs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDeleteLogs.UseVisualStyleBackColor = false;
            this.btnDeleteLogs.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnOpenLogs
            // 
            this.btnOpenLogs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenLogs.BackColor = System.Drawing.Color.White;
            this.btnOpenLogs.Enabled = false;
            this.btnOpenLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenLogs.Image = global::vApus.Gui.Properties.Resources.FolderOpen_16x16_72;
            this.btnOpenLogs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenLogs.Location = new System.Drawing.Point(6, 50);
            this.btnOpenLogs.Name = "btnOpenLogs";
            this.btnOpenLogs.Size = new System.Drawing.Size(355, 25);
            this.btnOpenLogs.TabIndex = 4;
            this.btnOpenLogs.Tag = "Logs";
            this.btnOpenLogs.Text = "     Logs... [?MB]";
            this.btnOpenLogs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenLogs.UseVisualStyleBackColor = false;
            this.btnOpenLogs.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnDeleteConnectionProxyTempFiles
            // 
            this.btnDeleteConnectionProxyTempFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteConnectionProxyTempFiles.AutoSize = true;
            this.btnDeleteConnectionProxyTempFiles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDeleteConnectionProxyTempFiles.BackColor = System.Drawing.Color.White;
            this.btnDeleteConnectionProxyTempFiles.Enabled = false;
            this.btnDeleteConnectionProxyTempFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteConnectionProxyTempFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteConnectionProxyTempFiles.Image = global::vApus.Gui.Properties.Resources.delete16x16;
            this.btnDeleteConnectionProxyTempFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDeleteConnectionProxyTempFiles.Location = new System.Drawing.Point(367, 19);
            this.btnDeleteConnectionProxyTempFiles.Name = "btnDeleteConnectionProxyTempFiles";
            this.btnDeleteConnectionProxyTempFiles.Size = new System.Drawing.Size(24, 24);
            this.btnDeleteConnectionProxyTempFiles.TabIndex = 3;
            this.btnDeleteConnectionProxyTempFiles.Tag = "ConnectionProxyTempFiles";
            this.btnDeleteConnectionProxyTempFiles.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDeleteConnectionProxyTempFiles.UseVisualStyleBackColor = false;
            this.btnDeleteConnectionProxyTempFiles.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnOpenConnectionProxyTempFiles
            // 
            this.btnOpenConnectionProxyTempFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenConnectionProxyTempFiles.BackColor = System.Drawing.Color.White;
            this.btnOpenConnectionProxyTempFiles.Enabled = false;
            this.btnOpenConnectionProxyTempFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenConnectionProxyTempFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenConnectionProxyTempFiles.Image = global::vApus.Gui.Properties.Resources.FolderOpen_16x16_72;
            this.btnOpenConnectionProxyTempFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenConnectionProxyTempFiles.Location = new System.Drawing.Point(6, 19);
            this.btnOpenConnectionProxyTempFiles.Name = "btnOpenConnectionProxyTempFiles";
            this.btnOpenConnectionProxyTempFiles.Size = new System.Drawing.Size(355, 25);
            this.btnOpenConnectionProxyTempFiles.TabIndex = 2;
            this.btnOpenConnectionProxyTempFiles.Tag = "ConnectionProxyTempFiles";
            this.btnOpenConnectionProxyTempFiles.Text = "     ConnectionProxyTempFiles... [?MB]";
            this.btnOpenConnectionProxyTempFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenConnectionProxyTempFiles.UseVisualStyleBackColor = false;
            this.btnOpenConnectionProxyTempFiles.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnDeleteSlaveSideResults
            // 
            this.btnDeleteSlaveSideResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteSlaveSideResults.AutoSize = true;
            this.btnDeleteSlaveSideResults.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDeleteSlaveSideResults.BackColor = System.Drawing.Color.White;
            this.btnDeleteSlaveSideResults.Enabled = false;
            this.btnDeleteSlaveSideResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteSlaveSideResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteSlaveSideResults.Image = global::vApus.Gui.Properties.Resources.delete16x16;
            this.btnDeleteSlaveSideResults.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDeleteSlaveSideResults.Location = new System.Drawing.Point(367, 20);
            this.btnDeleteSlaveSideResults.Name = "btnDeleteSlaveSideResults";
            this.btnDeleteSlaveSideResults.Size = new System.Drawing.Size(24, 24);
            this.btnDeleteSlaveSideResults.TabIndex = 1;
            this.btnDeleteSlaveSideResults.Tag = "SlaveSideResults";
            this.btnDeleteSlaveSideResults.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDeleteSlaveSideResults.UseVisualStyleBackColor = false;
            this.btnDeleteSlaveSideResults.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnOpenSlaveSideResults
            // 
            this.btnOpenSlaveSideResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenSlaveSideResults.BackColor = System.Drawing.Color.White;
            this.btnOpenSlaveSideResults.Enabled = false;
            this.btnOpenSlaveSideResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenSlaveSideResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenSlaveSideResults.Image = global::vApus.Gui.Properties.Resources.FolderOpen_16x16_72;
            this.btnOpenSlaveSideResults.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenSlaveSideResults.Location = new System.Drawing.Point(6, 19);
            this.btnOpenSlaveSideResults.Name = "btnOpenSlaveSideResults";
            this.btnOpenSlaveSideResults.Size = new System.Drawing.Size(355, 25);
            this.btnOpenSlaveSideResults.TabIndex = 0;
            this.btnOpenSlaveSideResults.Tag = "SlaveSideResults";
            this.btnOpenSlaveSideResults.Text = "     SlaveSideResults... [?MB]";
            this.btnOpenSlaveSideResults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip.SetToolTip(this.btnOpenSlaveSideResults, "The folder where the results for distributed tests are saved, slave-side.");
            this.btnOpenSlaveSideResults.UseVisualStyleBackColor = false;
            this.btnOpenSlaveSideResults.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteAll.BackColor = System.Drawing.Color.White;
            this.btnDeleteAll.Enabled = false;
            this.btnDeleteAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteAll.Location = new System.Drawing.Point(12, 247);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(397, 25);
            this.btnDeleteAll.TabIndex = 2;
            this.btnDeleteAll.Text = "Delete All [?MB]";
            this.btnDeleteAll.UseVisualStyleBackColor = false;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(396, 32);
            this.label1.TabIndex = 7;
            this.label1.Text = "Here you can delete or clean (eg the current log cannot be deleted) the following" +
    " directories.";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnOpenSlaveSideResults);
            this.groupBox2.Controls.Add(this.btnDeleteSlaveSideResults);
            this.groupBox2.Location = new System.Drawing.Point(12, 48);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(397, 55);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // CleanTempDataPanel
            // 
            this.ClientSize = new System.Drawing.Size(421, 284);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "CleanTempDataPanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOpenSlaveSideResults;
        private System.Windows.Forms.Button btnDeleteSlaveSideResults;
        private System.Windows.Forms.Button btnDeleteUpdateTempFiles;
        private System.Windows.Forms.Button btnOpenUpdateTempFiles;
        private System.Windows.Forms.Button btnDeleteLogs;
        private System.Windows.Forms.Button btnOpenLogs;
        private System.Windows.Forms.Button btnDeleteConnectionProxyTempFiles;
        private System.Windows.Forms.Button btnOpenConnectionProxyTempFiles;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.GroupBox groupBox2;

    }
}