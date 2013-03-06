namespace vApus.Stresstest {
    partial class ChartsControl {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.flpCharts = new System.Windows.Forms.FlowLayoutPanel();
            this.btnExportAllToExcel = new System.Windows.Forms.Button();
            this.btnSaveSelectedImage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // flpCharts
            // 
            this.flpCharts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpCharts.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpCharts.Location = new System.Drawing.Point(0, 0);
            this.flpCharts.Name = "flpCharts";
            this.flpCharts.Size = new System.Drawing.Size(444, 225);
            this.flpCharts.TabIndex = 0;
            // 
            // btnExportAllToExcel
            // 
            this.btnExportAllToExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportAllToExcel.AutoSize = true;
            this.btnExportAllToExcel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnExportAllToExcel.BackColor = System.Drawing.SystemColors.Control;
            this.btnExportAllToExcel.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnExportAllToExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportAllToExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportAllToExcel.Location = new System.Drawing.Point(307, 231);
            this.btnExportAllToExcel.MaximumSize = new System.Drawing.Size(165, 24);
            this.btnExportAllToExcel.Name = "btnExportAllToExcel";
            this.btnExportAllToExcel.Size = new System.Drawing.Size(134, 24);
            this.btnExportAllToExcel.TabIndex = 2;
            this.btnExportAllToExcel.Text = "Export all to Excel...";
            this.btnExportAllToExcel.UseVisualStyleBackColor = false;
            // 
            // btnSaveSelectedImage
            // 
            this.btnSaveSelectedImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveSelectedImage.AutoSize = true;
            this.btnSaveSelectedImage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveSelectedImage.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveSelectedImage.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnSaveSelectedImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveSelectedImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveSelectedImage.Location = new System.Drawing.Point(149, 231);
            this.btnSaveSelectedImage.MaximumSize = new System.Drawing.Size(165, 24);
            this.btnSaveSelectedImage.Name = "btnSaveSelectedImage";
            this.btnSaveSelectedImage.Size = new System.Drawing.Size(152, 24);
            this.btnSaveSelectedImage.TabIndex = 1;
            this.btnSaveSelectedImage.Text = "Save Selected Image...";
            this.btnSaveSelectedImage.UseVisualStyleBackColor = false;
            // 
            // ChartsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnExportAllToExcel);
            this.Controls.Add(this.btnSaveSelectedImage);
            this.Controls.Add(this.flpCharts);
            this.Name = "ChartsControl";
            this.Size = new System.Drawing.Size(444, 258);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpCharts;
        private System.Windows.Forms.Button btnExportAllToExcel;
        private System.Windows.Forms.Button btnSaveSelectedImage;
    }
}
