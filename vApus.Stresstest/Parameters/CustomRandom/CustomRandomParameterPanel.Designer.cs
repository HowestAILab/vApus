namespace vApus.Stresstest
{
    partial class CustomRandomParameterPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomRandomParameterPanel));
            this.chkUnique = new System.Windows.Forms.CheckBox();
            this.chkGenerateWhileTesting = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.compileCustomRandom = new vApus.Stresstest.TestCustomRandom();
            this.cbGenerate = new vApus.Stresstest.CodeBlock();
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
            // chkGenerateWhileTesting
            // 
            this.chkGenerateWhileTesting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkGenerateWhileTesting.AutoSize = true;
            this.chkGenerateWhileTesting.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkGenerateWhileTesting.Location = new System.Drawing.Point(462, 298);
            this.chkGenerateWhileTesting.Name = "chkGenerateWhileTesting";
            this.chkGenerateWhileTesting.Size = new System.Drawing.Size(213, 22);
            this.chkGenerateWhileTesting.TabIndex = 2;
            this.chkGenerateWhileTesting.Text = "Generate while stresstesting";
            this.toolTip.SetToolTip(this.chkGenerateWhileTesting, resources.GetString("chkGenerateWhileTesting.ToolTip"));
            this.chkGenerateWhileTesting.UseVisualStyleBackColor = true;
            this.chkGenerateWhileTesting.CheckedChanged += new System.EventHandler(this.chkGenerateWhileTesting_CheckedChanged);
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
            this.compileCustomRandom.CompileErrorButtonClicked += new System.EventHandler<vApus.Stresstest.TestCustomRandom.CompileErrorButtonClickedEventArgs>(this.compileCustomRandom_CompileErrorButtonClicked);
            // 
            // cbGenerate
            // 
            this.cbGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbGenerate.BackColor = System.Drawing.Color.White;
            this.cbGenerate.CanCollapse = false;
            this.cbGenerate.Collapsed = false;
            this.cbGenerate.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbGenerate.Footer = "}";
            this.cbGenerate.FooterVisible = true;
            this.cbGenerate.Header = "public string Generate() {";
            this.cbGenerate.HeaderVisible = true;
            this.cbGenerate.LineNumberOffset = 1;
            this.cbGenerate.Location = new System.Drawing.Point(12, 12);
            this.cbGenerate.Name = "cbGenerate";
            this.cbGenerate.ParentLevelControl = false;
            this.cbGenerate.ReadOnly = false;
            this.cbGenerate.ShowLineNumbers = true;
            this.cbGenerate.Size = new System.Drawing.Size(891, 101);
            this.cbGenerate.TabIndex = 0;
            this.cbGenerate.CodeTextChangedDelayed += new System.EventHandler(this.cbGenerate_CodeTextChangedDelayed);
            // 
            // CustomRandomParameterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkGenerateWhileTesting);
            this.Controls.Add(this.chkUnique);
            this.Controls.Add(this.compileCustomRandom);
            this.Controls.Add(this.cbGenerate);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CustomRandomParameterPanel";
            this.Size = new System.Drawing.Size(915, 499);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CodeBlock cbGenerate;
        private TestCustomRandom compileCustomRandom;
        private System.Windows.Forms.CheckBox chkUnique;
        private System.Windows.Forms.CheckBox chkGenerateWhileTesting;
        private System.Windows.Forms.ToolTip toolTip;
    }
}