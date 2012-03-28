namespace vApus.Monitor
{
    partial class LocalOrRemoteSMT
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocalOrRemoteSMT));
            this.btnOK = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.rdbRemote = new System.Windows.Forms.RadioButton();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.chkSaveSettings = new System.Windows.Forms.CheckBox();
            this.rdbLocal = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.AutoSize = true;
            this.btnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(112, 106);
            this.btnOK.MaximumSize = new System.Drawing.Size(75, 24);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(36, 24);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(154, 106);
            this.btnCancel.MaximumSize = new System.Drawing.Size(75, 24);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(58, 24);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // rdbRemote
            // 
            this.rdbRemote.AutoSize = true;
            this.rdbRemote.Location = new System.Drawing.Point(12, 42);
            this.rdbRemote.Name = "rdbRemote";
            this.rdbRemote.Size = new System.Drawing.Size(87, 17);
            this.rdbRemote.TabIndex = 1;
            this.rdbRemote.Text = "Remote at IP";
            this.rdbRemote.UseVisualStyleBackColor = true;
            // 
            // txtIP
            // 
            this.txtIP.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::vApus.Monitor.Properties.Settings.Default, "txtIP", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtIP.Location = new System.Drawing.Point(112, 41);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(100, 20);
            this.txtIP.TabIndex = 4;
            this.txtIP.Text = global::vApus.Monitor.Properties.Settings.Default.txtIP;
            this.toolTip.SetToolTip(this.txtIP, "IP 127.0.0.1 equals \'Local\'");
            this.txtIP.TextChanged += new System.EventHandler(this.txtIP_TextChanged);
            // 
            // chkSaveSettings
            // 
            this.chkSaveSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSaveSettings.AutoSize = true;
            this.chkSaveSettings.Checked = global::vApus.Monitor.Properties.Settings.Default.chkSaveSettings;
            this.chkSaveSettings.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::vApus.Monitor.Properties.Settings.Default, "chkSaveSettings", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkSaveSettings.Location = new System.Drawing.Point(12, 111);
            this.chkSaveSettings.Name = "chkSaveSettings";
            this.chkSaveSettings.Size = new System.Drawing.Size(90, 17);
            this.chkSaveSettings.TabIndex = 2;
            this.chkSaveSettings.Text = "Save settings";
            this.toolTip.SetToolTip(this.chkSaveSettings, "The settings can be saved when a valid IP is given.");
            this.chkSaveSettings.UseVisualStyleBackColor = true;
            // 
            // rdbLocal
            // 
            this.rdbLocal.AutoSize = true;
            this.rdbLocal.Checked = global::vApus.Monitor.Properties.Settings.Default.rdbLocal;
            this.rdbLocal.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::vApus.Monitor.Properties.Settings.Default, "rdbLocal", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rdbLocal.Location = new System.Drawing.Point(12, 18);
            this.rdbLocal.Name = "rdbLocal";
            this.rdbLocal.Size = new System.Drawing.Size(51, 17);
            this.rdbLocal.TabIndex = 0;
            this.rdbLocal.TabStop = true;
            this.rdbLocal.Text = "Local";
            this.rdbLocal.UseVisualStyleBackColor = true;
            // 
            // LocalOrRemoteSMT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(224, 142);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkSaveSettings);
            this.Controls.Add(this.rdbRemote);
            this.Controls.Add(this.rdbLocal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LocalOrRemoteSMT";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Local or Remote SMT";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdbLocal;
        private System.Windows.Forms.RadioButton rdbRemote;
        private System.Windows.Forms.CheckBox chkSaveSettings;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnCancel;
    }
}