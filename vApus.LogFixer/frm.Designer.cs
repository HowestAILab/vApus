namespace vApus.LogFixer
{
    partial class frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm));
            this.txtLogFiles = new System.Windows.Forms.TextBox();
            this.btnChooseLogFiles = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLogRuleSet = new System.Windows.Forms.Button();
            this.txtLogRuleSet = new System.Windows.Forms.TextBox();
            this.tc = new System.Windows.Forms.TabControl();
            this.ofdLogFiles = new System.Windows.Forms.OpenFileDialog();
            this.ofdLogRuleSet = new System.Windows.Forms.OpenFileDialog();
            this.btnEditLogRuleSet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLogFiles
            // 
            this.txtLogFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogFiles.BackColor = System.Drawing.Color.White;
            this.txtLogFiles.Location = new System.Drawing.Point(100, 14);
            this.txtLogFiles.Name = "txtLogFiles";
            this.txtLogFiles.ReadOnly = true;
            this.txtLogFiles.Size = new System.Drawing.Size(864, 20);
            this.txtLogFiles.TabIndex = 0;
            // 
            // btnChooseLogFiles
            // 
            this.btnChooseLogFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseLogFiles.AutoSize = true;
            this.btnChooseLogFiles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnChooseLogFiles.Location = new System.Drawing.Point(970, 12);
            this.btnChooseLogFiles.Name = "btnChooseLogFiles";
            this.btnChooseLogFiles.Size = new System.Drawing.Size(26, 23);
            this.btnChooseLogFiles.TabIndex = 1;
            this.btnChooseLogFiles.Text = "...";
            this.btnChooseLogFiles.UseVisualStyleBackColor = true;
            this.btnChooseLogFiles.Click += new System.EventHandler(this.btnChooseLogFiles_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Log File(s):";
            // 
            // btnLogRuleSet
            // 
            this.btnLogRuleSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogRuleSet.AutoSize = true;
            this.btnLogRuleSet.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLogRuleSet.Location = new System.Drawing.Point(970, 38);
            this.btnLogRuleSet.Name = "btnLogRuleSet";
            this.btnLogRuleSet.Size = new System.Drawing.Size(26, 23);
            this.btnLogRuleSet.TabIndex = 4;
            this.btnLogRuleSet.Text = "...";
            this.btnLogRuleSet.UseVisualStyleBackColor = true;
            this.btnLogRuleSet.Click += new System.EventHandler(this.btnLogRuleSet_Click);
            // 
            // txtLogRuleSet
            // 
            this.txtLogRuleSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogRuleSet.BackColor = System.Drawing.Color.White;
            this.txtLogRuleSet.Location = new System.Drawing.Point(100, 40);
            this.txtLogRuleSet.Name = "txtLogRuleSet";
            this.txtLogRuleSet.ReadOnly = true;
            this.txtLogRuleSet.Size = new System.Drawing.Size(864, 20);
            this.txtLogRuleSet.TabIndex = 3;
            this.txtLogRuleSet.TextChanged += new System.EventHandler(this.txtLogRuleSet_TextChanged);
            // 
            // tc
            // 
            this.tc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tc.Location = new System.Drawing.Point(12, 70);
            this.tc.Name = "tc";
            this.tc.SelectedIndex = 0;
            this.tc.Size = new System.Drawing.Size(984, 648);
            this.tc.TabIndex = 5;
            // 
            // ofdLogFiles
            // 
            this.ofdLogFiles.Filter = "TXT Files|*.txt|All Files|*.*";
            this.ofdLogFiles.Multiselect = true;
            // 
            // ofdLogRuleSet
            // 
            this.ofdLogRuleSet.Filter = "XML Files|*.xml";
            // 
            // btnEditLogRuleSet
            // 
            this.btnEditLogRuleSet.AutoSize = true;
            this.btnEditLogRuleSet.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnEditLogRuleSet.Enabled = false;
            this.btnEditLogRuleSet.Location = new System.Drawing.Point(12, 38);
            this.btnEditLogRuleSet.Name = "btnEditLogRuleSet";
            this.btnEditLogRuleSet.Size = new System.Drawing.Size(82, 23);
            this.btnEditLogRuleSet.TabIndex = 2;
            this.btnEditLogRuleSet.Text = "Log Rule Set:";
            this.btnEditLogRuleSet.UseVisualStyleBackColor = true;
            this.btnEditLogRuleSet.Click += new System.EventHandler(this.btnEditLogRuleSet_Click);
            // 
            // frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.btnEditLogRuleSet);
            this.Controls.Add(this.tc);
            this.Controls.Add(this.btnLogRuleSet);
            this.Controls.Add(this.txtLogRuleSet);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnChooseLogFiles);
            this.Controls.Add(this.txtLogFiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "frm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Log Fixer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLogFiles;
        private System.Windows.Forms.Button btnChooseLogFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLogRuleSet;
        private System.Windows.Forms.TextBox txtLogRuleSet;
        private System.Windows.Forms.TabControl tc;
        private System.Windows.Forms.OpenFileDialog ofdLogFiles;
        private System.Windows.Forms.OpenFileDialog ofdLogRuleSet;
        private System.Windows.Forms.Button btnEditLogRuleSet;
    }
}

