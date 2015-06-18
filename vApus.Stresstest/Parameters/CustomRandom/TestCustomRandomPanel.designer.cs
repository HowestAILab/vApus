namespace vApus.StressTest
{
    partial class TestCustomRandomPanel
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
            this.components = new System.ComponentModel.Container();
            this.btnTestCode = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.flpCompileLog = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // btnTestCode
            // 
            this.btnTestCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestCode.AutoSize = true;
            this.btnTestCode.BackColor = System.Drawing.Color.White;
            this.btnTestCode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTestCode.Location = new System.Drawing.Point(461, 138);
            this.btnTestCode.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnTestCode.Name = "btnTestCode";
            this.btnTestCode.Size = new System.Drawing.Size(85, 24);
            this.btnTestCode.TabIndex = 1;
            this.btnTestCode.Text = "Test Code";
            this.toolTip.SetToolTip(this.btnTestCode, "Returns three generated values.");
            this.btnTestCode.UseVisualStyleBackColor = false;
            this.btnTestCode.Click += new System.EventHandler(this.btnTryCompile_Click);
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
            // TestCustomRandom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flpCompileLog);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.btnTestCode);
            this.Name = "TestCustomRandom";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.Size = new System.Drawing.Size(558, 173);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTestCode;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.FlowLayoutPanel flpCompileLog;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
