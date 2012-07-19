namespace vApus.DistributedTesting
{
    partial class Form1
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvClients = new System.Windows.Forms.DataGridView();
            this.clmIpOrHostName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmUserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDomain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmPassword = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmSlaves = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmTests = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvClients
            // 
            this.dgvClients.AllowUserToResizeRows = false;
            this.dgvClients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvClients.BackgroundColor = System.Drawing.Color.White;
            this.dgvClients.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvClients.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmIpOrHostName,
            this.clmUserName,
            this.clmDomain,
            this.clmPassword,
            this.clmSlaves,
            this.clmTests});
            this.dgvClients.EnableHeadersVisualStyles = false;
            this.dgvClients.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.dgvClients.Location = new System.Drawing.Point(41, 142);
            this.dgvClients.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.dgvClients.Name = "dgvClients";
            this.dgvClients.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvClients.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvClients.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgvClients.Size = new System.Drawing.Size(666, 138);
            this.dgvClients.TabIndex = 16;
            this.dgvClients.VirtualMode = true;
            // 
            // clmIpOrHostName
            // 
            this.clmIpOrHostName.HeaderText = "* IP or Host Name";
            this.clmIpOrHostName.Name = "clmIpOrHostName";
            // 
            // clmUserName
            // 
            this.clmUserName.HeaderText = "User Name (RDP)";
            this.clmUserName.Name = "clmUserName";
            // 
            // clmDomain
            // 
            this.clmDomain.HeaderText = "Domain";
            this.clmDomain.Name = "clmDomain";
            // 
            // clmPassword
            // 
            this.clmPassword.HeaderText = "Password";
            this.clmPassword.Name = "clmPassword";
            // 
            // clmSlaves
            // 
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.clmSlaves.DefaultCellStyle = dataGridViewCellStyle1;
            this.clmSlaves.HeaderText = "Number of Slaves (0)";
            this.clmSlaves.Name = "clmSlaves";
            this.clmSlaves.ReadOnly = true;
            // 
            // clmTests
            // 
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.clmTests.DefaultCellStyle = dataGridViewCellStyle2;
            this.clmTests.HeaderText = "Number Of Tests (0)";
            this.clmTests.Name = "clmTests";
            this.clmTests.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 423);
            this.Controls.Add(this.dgvClients);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvClients;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmIpOrHostName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmUserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDomain;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPassword;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSlaves;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmTests;
    }
}