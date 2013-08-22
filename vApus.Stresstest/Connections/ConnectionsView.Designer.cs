namespace vApus.Stresstest {
    partial class ConnectionsView {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvConnections = new System.Windows.Forms.DataGridView();
            this.chkShowConnectionStrings = new System.Windows.Forms.CheckBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConnections)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvConnections
            // 
            this.dgvConnections.AllowUserToAddRows = false;
            this.dgvConnections.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            this.dgvConnections.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvConnections.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvConnections.BackgroundColor = System.Drawing.Color.White;
            this.dgvConnections.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvConnections.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvConnections.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvConnections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConnections.EnableHeadersVisualStyles = false;
            this.dgvConnections.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.dgvConnections.Location = new System.Drawing.Point(12, 12);
            this.dgvConnections.Name = "dgvConnections";
            this.dgvConnections.ReadOnly = true;
            this.dgvConnections.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvConnections.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvConnections.Size = new System.Drawing.Size(612, 305);
            this.dgvConnections.TabIndex = 0;
            this.dgvConnections.VirtualMode = true;
            this.dgvConnections.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvConnections_CellFormatting);
            this.dgvConnections.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvConnections_CellPainting);
            // 
            // chkShowConnectionStrings
            // 
            this.chkShowConnectionStrings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowConnectionStrings.AutoSize = true;
            this.chkShowConnectionStrings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkShowConnectionStrings.Location = new System.Drawing.Point(12, 327);
            this.chkShowConnectionStrings.Margin = new System.Windows.Forms.Padding(3, 7, 0, 3);
            this.chkShowConnectionStrings.Name = "chkShowConnectionStrings";
            this.chkShowConnectionStrings.Size = new System.Drawing.Size(142, 17);
            this.chkShowConnectionStrings.TabIndex = 1;
            this.chkShowConnectionStrings.Text = "Show Connection Strings";
            this.chkShowConnectionStrings.UseVisualStyleBackColor = true;
            this.chkShowConnectionStrings.CheckedChanged += new System.EventHandler(this.chkShowConnectionStrings_CheckedChanged);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.AutoSize = true;
            this.btnEdit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnEdit.BackColor = System.Drawing.SystemColors.Control;
            this.btnEdit.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Location = new System.Drawing.Point(516, 323);
            this.btnEdit.MaximumSize = new System.Drawing.Size(165, 24);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(53, 24);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Edit...";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUndo.AutoSize = true;
            this.btnUndo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUndo.BackColor = System.Drawing.SystemColors.Control;
            this.btnUndo.Enabled = false;
            this.btnUndo.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnUndo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUndo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUndo.Location = new System.Drawing.Point(575, 323);
            this.btnUndo.MaximumSize = new System.Drawing.Size(165, 24);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(49, 24);
            this.btnUndo.TabIndex = 4;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = false;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.AutoSize = true;
            this.btnTest.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnTest.BackColor = System.Drawing.SystemColors.Control;
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTest.Location = new System.Drawing.Point(432, 323);
            this.btnTest.MaximumSize = new System.Drawing.Size(73, 24);
            this.btnTest.MinimumSize = new System.Drawing.Size(73, 24);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(73, 24);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // ConnectionsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(636, 359);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.dgvConnections);
            this.Controls.Add(this.chkShowConnectionStrings);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ConnectionsView";
            this.Text = "ConnectionsView";
            ((System.ComponentModel.ISupportInitialize)(this.dgvConnections)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvConnections;
        private System.Windows.Forms.CheckBox chkShowConnectionStrings;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnTest;
    }
}