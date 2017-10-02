namespace vApus.Monitor {
    partial class HardwareConfigurationDialog {
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
            this.rtxt = new System.Windows.Forms.RichTextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // rtxt
            // 
            this.rtxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxt.BackColor = System.Drawing.Color.White;
            this.rtxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxt.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxt.Location = new System.Drawing.Point(0, 0);
            this.rtxt.Margin = new System.Windows.Forms.Padding(4);
            this.rtxt.Name = "rtxt";
            this.rtxt.ReadOnly = true;
            this.rtxt.Size = new System.Drawing.Size(789, 545);
            this.rtxt.TabIndex = 0;
            this.rtxt.Text = "";
            this.rtxt.WordWrap = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.Control;
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(0, 546);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(789, 28);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // sfd
            // 
            this.sfd.Filter = "txt files (*.txt) |*.txt";
            this.sfd.RestoreDirectory = true;
            this.sfd.ShowHelp = true;
            // 
            // HardwareConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(789, 574);
            this.Controls.Add(this.rtxt);
            this.Controls.Add(this.btnSave);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(794, 605);
            this.Name = "HardwareConfigurationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxt;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog sfd;

    }
}