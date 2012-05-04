namespace vApus.Gui
{
    partial class TestValueControlPanel
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
            this.button1 = new System.Windows.Forms.Button();
            this.valueControlPanel1 = new vApus.Util.ValueControlPanel();
            this.button2 = new System.Windows.Forms.Button();
            this.lblValueChanged = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(12, 667);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "SetValues";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // valueControlPanel1
            // 
            this.valueControlPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueControlPanel1.BackColor = System.Drawing.Color.White;
            this.valueControlPanel1.Location = new System.Drawing.Point(0, 0);
            this.valueControlPanel1.Name = "valueControlPanel1";
            this.valueControlPanel1.Size = new System.Drawing.Size(985, 652);
            this.valueControlPanel1.TabIndex = 0;
            this.valueControlPanel1.ValueChanged += new System.EventHandler<vApus.Util.ValueControlPanel.ValueChangedEventArgs>(this.valueControlPanel1_ValueChanged);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(93, 667);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Lock Unlock";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblValueChanged
            // 
            this.lblValueChanged.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblValueChanged.AutoSize = true;
            this.lblValueChanged.Location = new System.Drawing.Point(193, 672);
            this.lblValueChanged.Name = "lblValueChanged";
            this.lblValueChanged.Size = new System.Drawing.Size(0, 13);
            this.lblValueChanged.TabIndex = 3;
            // 
            // TestValueControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 702);
            this.Controls.Add(this.lblValueChanged);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.valueControlPanel1);
            this.Name = "TestValueControlPanel";
            this.Text = "TestValueControlPanel";
            this.Load += new System.EventHandler(this.TestValueControlPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Util.ValueControlPanel valueControlPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblValueChanged;
    }
}