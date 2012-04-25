namespace vApus.Util
{
    partial class DisableFirewallAutoUpdatePanel
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.kvpFirewall = new vApus.Util.KeyValuePairControl();
            this.kvpWindowsAutoUpdate = new vApus.Util.KeyValuePairControl();
            this.btnDisableThem = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.kvpFirewall);
            this.groupBox1.Controls.Add(this.kvpWindowsAutoUpdate);
            this.groupBox1.Location = new System.Drawing.Point(12, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 173);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            // 
            // kvpFirewall
            // 
            this.kvpFirewall.BackColor = System.Drawing.Color.LightGreen;
            this.kvpFirewall.Key = "Windows Firewall";
            this.kvpFirewall.Location = new System.Drawing.Point(6, 22);
            this.kvpFirewall.Name = "kvpFirewall";
            this.kvpFirewall.Size = new System.Drawing.Size(132, 24);
            this.kvpFirewall.TabIndex = 15;
            this.kvpFirewall.TabStop = false;
            this.kvpFirewall.Tooltip = "";
            this.kvpFirewall.Value = "Off";
            // 
            // kvpWindowsAutoUpdate
            // 
            this.kvpWindowsAutoUpdate.BackColor = System.Drawing.Color.LightGreen;
            this.kvpWindowsAutoUpdate.Key = "Windows Auto Update";
            this.kvpWindowsAutoUpdate.Location = new System.Drawing.Point(6, 55);
            this.kvpWindowsAutoUpdate.Name = "kvpWindowsAutoUpdate";
            this.kvpWindowsAutoUpdate.Size = new System.Drawing.Size(160, 24);
            this.kvpWindowsAutoUpdate.TabIndex = 16;
            this.kvpWindowsAutoUpdate.TabStop = false;
            this.kvpWindowsAutoUpdate.Tooltip = "";
            this.kvpWindowsAutoUpdate.Value = "Off";
            // 
            // btnDisableThem
            // 
            this.btnDisableThem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisableThem.BackColor = System.Drawing.Color.White;
            this.btnDisableThem.Enabled = false;
            this.btnDisableThem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisableThem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisableThem.Location = new System.Drawing.Point(12, 227);
            this.btnDisableThem.Margin = new System.Windows.Forms.Padding(0, 3, 6, 6);
            this.btnDisableThem.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnDisableThem.Name = "btnDisableThem";
            this.btnDisableThem.Size = new System.Drawing.Size(268, 24);
            this.btnDisableThem.TabIndex = 18;
            this.btnDisableThem.Text = "Disable Them";
            this.btnDisableThem.UseVisualStyleBackColor = false;
            this.btnDisableThem.Click += new System.EventHandler(this.btnDisableThem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(268, 39);
            this.label1.TabIndex = 19;
            this.label1.Text = "The Windows firewall can block vApus communication.\r\nWindows auto update can rest" +
    "art the computer while a\r\nstresstest is running.";
            // 
            // DisableFirewallAutoUpdatePanel
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDisableThem);
            this.Controls.Add(this.groupBox1);
            this.Name = "DisableFirewallAutoUpdatePanel";
            this.Text = "DisableFirewallAutoUpdatePanel";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private KeyValuePairControl kvpFirewall;
        private KeyValuePairControl kvpWindowsAutoUpdate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDisableThem;
        private System.Windows.Forms.Label label1;

    }
}