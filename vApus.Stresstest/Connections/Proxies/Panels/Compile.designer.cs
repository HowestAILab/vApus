namespace vApus.Stresstest
{
    partial class Compile
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnTryCompile = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.flpCompileLog = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // btnTryCompile
            // 
            this.btnTryCompile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTryCompile.AutoSize = true;
            this.btnTryCompile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnTryCompile.BackColor = System.Drawing.Color.White;
            this.btnTryCompile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTryCompile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTryCompile.Location = new System.Drawing.Point(461, 138);
            this.btnTryCompile.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnTryCompile.Name = "btnTryCompile";
            this.btnTryCompile.Size = new System.Drawing.Size(85, 24);
            this.btnTryCompile.TabIndex = 1;
            this.btnTryCompile.Text = "Try Compile";
            this.btnTryCompile.UseVisualStyleBackColor = false;
            this.btnTryCompile.Click += new System.EventHandler(this.btnTryCompile_Click);
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCount.Location = new System.Drawing.Point(12, 143);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(0, 13);
            this.lblCount.TabIndex = 3;
            // 
            // flpCompileLog
            // 
            this.flpCompileLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpCompileLog.AutoScroll = true;
            this.flpCompileLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpCompileLog.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flpCompileLog.Location = new System.Drawing.Point(12, 12);
            this.flpCompileLog.Name = "flpCompileLog";
            this.flpCompileLog.Size = new System.Drawing.Size(534, 120);
            this.flpCompileLog.TabIndex = 0;
            this.flpCompileLog.SizeChanged += new System.EventHandler(this.flpCompileLog_SizeChanged);
            // 
            // Compile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flpCompileLog);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.btnTryCompile);
            this.Name = "Compile";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.Size = new System.Drawing.Size(558, 173);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTryCompile;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.FlowLayoutPanel flpCompileLog;
    }
}
