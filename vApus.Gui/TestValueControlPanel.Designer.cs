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
            this.valueControlPanel1 = new vApus.Util.ValueControlPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // valueControlPanel1
            // 
            this.valueControlPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueControlPanel1.BackColor = System.Drawing.Color.White;
            this.valueControlPanel1.Location = new System.Drawing.Point(0, 0);
            this.valueControlPanel1.Name = "valueControlPanel1";
            this.valueControlPanel1.Size = new System.Drawing.Size(525, 429);
            this.valueControlPanel1.TabIndex = 0;
            this.valueControlPanel1.ValueChanged += new System.EventHandler<vApus.Util.ValueControlPanel.ValueChangedEventArgs>(this.valueControlPanel1_ValueChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(33, 444);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "SetValues";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TestValueControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 479);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.valueControlPanel1);
            this.Name = "TestValueControlPanel";
            this.Text = "TestValueControlPanel";
            this.Load += new System.EventHandler(this.TestValueControlPanel_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Util.ValueControlPanel valueControlPanel1;
        private System.Windows.Forms.Button button1;
    }
}