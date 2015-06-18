using vApus.Util;
namespace vApus.StressTest {
    partial class CustomRandomParameterPanel {
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
            this.chkUnique = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.compileCustomRandom = new vApus.StressTest.TestCustomRandomPanel();
            this.ctxtGenerate = new vApus.Util.CodeTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ctxtGenerate)).BeginInit();
            this.SuspendLayout();
            // 
            // chkUnique
            // 
            this.chkUnique.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkUnique.AutoSize = true;
            this.chkUnique.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUnique.Location = new System.Drawing.Point(12, 298);
            this.chkUnique.Name = "chkUnique";
            this.chkUnique.Size = new System.Drawing.Size(441, 22);
            this.chkUnique.TabIndex = 1;
            this.chkUnique.Text = "Return value is unique for each call (keep infinite loops in mind!)";
            this.chkUnique.UseVisualStyleBackColor = true;
            this.chkUnique.CheckedChanged += new System.EventHandler(this.chkUnique_CheckedChanged);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            // 
            // compileCustomRandom
            // 
            this.compileCustomRandom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.compileCustomRandom.BackColor = System.Drawing.Color.White;
            this.compileCustomRandom.Location = new System.Drawing.Point(0, 326);
            this.compileCustomRandom.Name = "compileCustomRandom";
            this.compileCustomRandom.Padding = new System.Windows.Forms.Padding(9);
            this.compileCustomRandom.Size = new System.Drawing.Size(915, 173);
            this.compileCustomRandom.TabIndex = 3;
            this.compileCustomRandom.CompileErrorButtonClicked += new System.EventHandler<vApus.StressTest.TestCustomRandomPanel.CompileErrorButtonClickedEventArgs>(this.compileCustomRandom_CompileErrorButtonClicked);
            // 
            // ctxtGenerate
            // 
            this.ctxtGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctxtGenerate.AutoScrollMinSize = new System.Drawing.Size(25, 15);
            this.ctxtGenerate.BackBrush = null;
            this.ctxtGenerate.CharHeight = 15;
            this.ctxtGenerate.CharWidth = 7;
            this.ctxtGenerate.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ctxtGenerate.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.ctxtGenerate.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.ctxtGenerate.IsReplaceMode = false;
            this.ctxtGenerate.Location = new System.Drawing.Point(0, 0);
            this.ctxtGenerate.Name = "ctxtGenerate";
            this.ctxtGenerate.Paddings = new System.Windows.Forms.Padding(0);
            this.ctxtGenerate.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.ctxtGenerate.Size = new System.Drawing.Size(915, 292);
            this.ctxtGenerate.TabIndex = 0;
            this.ctxtGenerate.Zoom = 100;
            // 
            // CustomRandomParameterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUnique);
            this.Controls.Add(this.compileCustomRandom);
            this.Controls.Add(this.ctxtGenerate);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CustomRandomParameterPanel";
            this.Size = new System.Drawing.Size(915, 499);
            ((System.ComponentModel.ISupportInitialize)(this.ctxtGenerate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private CodeTextBox ctxtGenerate;
        private TestCustomRandomPanel compileCustomRandom;
        private System.Windows.Forms.CheckBox chkUnique;
        private System.Windows.Forms.ToolTip toolTip;
    }
}