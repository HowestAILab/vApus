namespace vApus.Util
{
    partial class WindowsFirewallAutoUpdatePanel
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
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.pnlUpdate = new System.Windows.Forms.Panel();
            this.kvpWindowsAutoUpdate = new vApus.Util.KeyValuePairControl();
            this.rdbUpdateOn = new System.Windows.Forms.RadioButton();
            this.rdbUpdateOff = new System.Windows.Forms.RadioButton();
            this.pnlFirewall = new System.Windows.Forms.Panel();
            this.kvpFirewall = new vApus.Util.KeyValuePairControl();
            this.rdbFirewallOn = new System.Windows.Forms.RadioButton();
            this.rdbFirewallOff = new System.Windows.Forms.RadioButton();
            this.btnDisableAll = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox.SuspendLayout();
            this.pnlUpdate.SuspendLayout();
            this.pnlFirewall.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.pnlUpdate);
            this.groupBox.Controls.Add(this.pnlFirewall);
            this.groupBox.Location = new System.Drawing.Point(12, 48);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(268, 173);
            this.groupBox.TabIndex = 17;
            this.groupBox.TabStop = false;
            // 
            // pnlUpdate
            // 
            this.pnlUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlUpdate.BackColor = System.Drawing.Color.LightGreen;
            this.pnlUpdate.Controls.Add(this.kvpWindowsAutoUpdate);
            this.pnlUpdate.Controls.Add(this.rdbUpdateOn);
            this.pnlUpdate.Controls.Add(this.rdbUpdateOff);
            this.pnlUpdate.Location = new System.Drawing.Point(6, 55);
            this.pnlUpdate.Name = "pnlUpdate";
            this.pnlUpdate.Size = new System.Drawing.Size(256, 24);
            this.pnlUpdate.TabIndex = 1;
            // 
            // kvpWindowsAutoUpdate
            // 
            this.kvpWindowsAutoUpdate.BackColor = System.Drawing.Color.Transparent;
            this.kvpWindowsAutoUpdate.Key = "Windows auto update";
            this.kvpWindowsAutoUpdate.Location = new System.Drawing.Point(0, 0);
            this.kvpWindowsAutoUpdate.Margin = new System.Windows.Forms.Padding(0);
            this.kvpWindowsAutoUpdate.Name = "kvpWindowsAutoUpdate";
            this.kvpWindowsAutoUpdate.Size = new System.Drawing.Size(139, 24);
            this.kvpWindowsAutoUpdate.TabIndex = 0;
            this.kvpWindowsAutoUpdate.TabStop = false;
            this.kvpWindowsAutoUpdate.Tooltip = "";
            this.kvpWindowsAutoUpdate.Value = "";
            // 
            // rdbUpdateOn
            // 
            this.rdbUpdateOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rdbUpdateOn.AutoSize = true;
            this.rdbUpdateOn.Checked = true;
            this.rdbUpdateOn.Location = new System.Drawing.Point(169, 3);
            this.rdbUpdateOn.Name = "rdbUpdateOn";
            this.rdbUpdateOn.Size = new System.Drawing.Size(39, 17);
            this.rdbUpdateOn.TabIndex = 1;
            this.rdbUpdateOn.TabStop = true;
            this.rdbUpdateOn.Text = "On";
            this.rdbUpdateOn.UseVisualStyleBackColor = true;
            this.rdbUpdateOn.CheckedChanged += new System.EventHandler(this.rdb_CheckedChanged);
            // 
            // rdbUpdateOff
            // 
            this.rdbUpdateOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rdbUpdateOff.AutoSize = true;
            this.rdbUpdateOff.Location = new System.Drawing.Point(214, 3);
            this.rdbUpdateOff.Name = "rdbUpdateOff";
            this.rdbUpdateOff.Size = new System.Drawing.Size(39, 17);
            this.rdbUpdateOff.TabIndex = 2;
            this.rdbUpdateOff.Text = "Off";
            this.rdbUpdateOff.UseVisualStyleBackColor = true;
            // 
            // pnlFirewall
            // 
            this.pnlFirewall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFirewall.BackColor = System.Drawing.Color.LightGreen;
            this.pnlFirewall.Controls.Add(this.kvpFirewall);
            this.pnlFirewall.Controls.Add(this.rdbFirewallOn);
            this.pnlFirewall.Controls.Add(this.rdbFirewallOff);
            this.pnlFirewall.Location = new System.Drawing.Point(6, 22);
            this.pnlFirewall.Name = "pnlFirewall";
            this.pnlFirewall.Size = new System.Drawing.Size(256, 24);
            this.pnlFirewall.TabIndex = 0;
            // 
            // kvpFirewall
            // 
            this.kvpFirewall.BackColor = System.Drawing.Color.Transparent;
            this.kvpFirewall.Key = "Windows firewall";
            this.kvpFirewall.Location = new System.Drawing.Point(0, 0);
            this.kvpFirewall.Margin = new System.Windows.Forms.Padding(0);
            this.kvpFirewall.Name = "kvpFirewall";
            this.kvpFirewall.Size = new System.Drawing.Size(111, 24);
            this.kvpFirewall.TabIndex = 0;
            this.kvpFirewall.TabStop = false;
            this.kvpFirewall.Tooltip = "";
            this.kvpFirewall.Value = "";
            // 
            // rdbFirewallOn
            // 
            this.rdbFirewallOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rdbFirewallOn.AutoSize = true;
            this.rdbFirewallOn.Checked = true;
            this.rdbFirewallOn.Location = new System.Drawing.Point(169, 3);
            this.rdbFirewallOn.Name = "rdbFirewallOn";
            this.rdbFirewallOn.Size = new System.Drawing.Size(39, 17);
            this.rdbFirewallOn.TabIndex = 1;
            this.rdbFirewallOn.TabStop = true;
            this.rdbFirewallOn.Text = "On";
            this.rdbFirewallOn.UseVisualStyleBackColor = true;
            this.rdbFirewallOn.CheckedChanged += new System.EventHandler(this.rdb_CheckedChanged);
            // 
            // rdbFirewallOff
            // 
            this.rdbFirewallOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rdbFirewallOff.AutoSize = true;
            this.rdbFirewallOff.Location = new System.Drawing.Point(214, 3);
            this.rdbFirewallOff.Name = "rdbFirewallOff";
            this.rdbFirewallOff.Size = new System.Drawing.Size(39, 17);
            this.rdbFirewallOff.TabIndex = 2;
            this.rdbFirewallOff.Text = "Off";
            this.rdbFirewallOff.UseVisualStyleBackColor = true;
            // 
            // btnDisableAll
            // 
            this.btnDisableAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisableAll.BackColor = System.Drawing.Color.White;
            this.btnDisableAll.Enabled = false;
            this.btnDisableAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisableAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisableAll.Location = new System.Drawing.Point(12, 227);
            this.btnDisableAll.Margin = new System.Windows.Forms.Padding(0, 3, 6, 6);
            this.btnDisableAll.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnDisableAll.Name = "btnDisableAll";
            this.btnDisableAll.Size = new System.Drawing.Size(268, 24);
            this.btnDisableAll.TabIndex = 18;
            this.btnDisableAll.Text = "Disable all";
            this.btnDisableAll.UseVisualStyleBackColor = false;
            this.btnDisableAll.Click += new System.EventHandler(this.btnDisableAll_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(421, 26);
            this.label1.TabIndex = 19;
            this.label1.Text = "The Windows firewall can block vApus communication and can break a stress test.\r\nW" +
    "indows auto update can restart the computer while a (scheduled) stress test is ru" +
    "nning.\r\nYou can only do this when running vApus as administrator!";
            // 
            // DisableFirewallAutoUpdatePanel
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDisableAll);
            this.Controls.Add(this.groupBox);
            this.Name = "DisableFirewallAutoUpdatePanel";
            this.Text = "DisableFirewallAutoUpdatePanel";
            this.groupBox.ResumeLayout(false);
            this.pnlUpdate.ResumeLayout(false);
            this.pnlUpdate.PerformLayout();
            this.pnlFirewall.ResumeLayout(false);
            this.pnlFirewall.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private KeyValuePairControl kvpFirewall;
        private KeyValuePairControl kvpWindowsAutoUpdate;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Button btnDisableAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlUpdate;
        private System.Windows.Forms.Panel pnlFirewall;
        private System.Windows.Forms.RadioButton rdbUpdateOn;
        private System.Windows.Forms.RadioButton rdbUpdateOff;
        private System.Windows.Forms.RadioButton rdbFirewallOn;
        private System.Windows.Forms.RadioButton rdbFirewallOff;

    }
}