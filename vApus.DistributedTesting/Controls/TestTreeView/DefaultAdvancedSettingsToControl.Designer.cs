﻿namespace vApus.DistributedTest
{
    partial class DefaultAdvancedSettingsToControl
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
            this.chkDefaultTo = new System.Windows.Forms.CheckBox();
            this.cboStressTests = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // chkDefaultTo
            // 
            this.chkDefaultTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDefaultTo.AutoSize = true;
            this.chkDefaultTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkDefaultTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.chkDefaultTo.ForeColor = System.Drawing.Color.Blue;
            this.chkDefaultTo.Location = new System.Drawing.Point(4, 1);
            this.chkDefaultTo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.chkDefaultTo.Name = "chkDefaultTo";
            this.chkDefaultTo.Size = new System.Drawing.Size(257, 20);
            this.chkDefaultTo.TabIndex = 2;
            this.chkDefaultTo.Text = "Default advanced stress test settings to";
            this.chkDefaultTo.UseVisualStyleBackColor = true;
            this.chkDefaultTo.CheckedChanged += new System.EventHandler(this.chkDefaultTo_CheckedChanged);
            // 
            // cboStressTests
            // 
            this.cboStressTests.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboStressTests.BackColor = System.Drawing.Color.White;
            this.cboStressTests.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStressTests.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboStressTests.FormattingEnabled = true;
            this.cboStressTests.Location = new System.Drawing.Point(22, 20);
            this.cboStressTests.Name = "cboStressTests";
            this.cboStressTests.Size = new System.Drawing.Size(325, 21);
            this.cboStressTests.TabIndex = 3;
            this.cboStressTests.SelectedIndexChanged += new System.EventHandler(this.cboStressTests_SelectedIndexChanged);
            // 
            // DefaultAdvancedSettingsToControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.cboStressTests);
            this.Controls.Add(this.chkDefaultTo);
            this.MaximumSize = new System.Drawing.Size(350, 46);
            this.MinimumSize = new System.Drawing.Size(350, 46);
            this.Name = "DefaultAdvancedSettingsToControl";
            this.Size = new System.Drawing.Size(350, 46);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkDefaultTo;
        private System.Windows.Forms.ComboBox cboStressTests;

    }
}
