namespace vApus.DistributedTesting.Controls.SlaveTreeView.SlaveWizard
{
    partial class AddClients
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
            this.rtxt = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtxt
            // 
            this.rtxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxt.Location = new System.Drawing.Point(0, 0);
            this.rtxt.Margin = new System.Windows.Forms.Padding(0);
            this.rtxt.Name = "rtxt";
            this.rtxt.Size = new System.Drawing.Size(300, 300);
            this.rtxt.TabIndex = 2;
            this.rtxt.Text = "";
            // 
            // AddClients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtxt);
            this.Name = "AddClients";
            this.Size = new System.Drawing.Size(300, 300);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxt;

    }
}
