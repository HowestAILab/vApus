namespace vApus.Publish {
    partial class PublishPanel {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PublishPanel));
            this.label1 = new System.Windows.Forms.Label();
            this.lblTests = new System.Windows.Forms.Label();
            this.lblMonitors = new System.Windows.Forms.Label();
            this.chkTestsFastConcurrencyResults = new System.Windows.Forms.CheckBox();
            this.chkTestsConfig = new System.Windows.Forms.CheckBox();
            this.chkTestsFastRunResults = new System.Windows.Forms.CheckBox();
            this.chkMonitorsConfig = new System.Windows.Forms.CheckBox();
            this.chkMonitorsMetrics = new System.Windows.Forms.CheckBox();
            this.chkTestsMessages = new System.Windows.Forms.CheckBox();
            this.chkMonitorsHWConfig = new System.Windows.Forms.CheckBox();
            this.btnSet = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.grp = new System.Windows.Forms.GroupBox();
            this.chkRequestResults = new System.Windows.Forms.CheckBox();
            this.txtTcpHost = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkTcp = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nudTcpPort = new System.Windows.Forms.NumericUpDown();
            this.llblDeserialize = new System.Windows.Forms.LinkLabel();
            this.cboLogLevel = new System.Windows.Forms.ComboBox();
            this.chkApplicationLogs = new System.Windows.Forms.CheckBox();
            this.cboMessageLevel = new System.Windows.Forms.ComboBox();
            this.chkTestsClientMonitoring = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnEnable = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTcpPort)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(573, 52);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // lblTests
            // 
            this.lblTests.AutoSize = true;
            this.lblTests.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTests.Location = new System.Drawing.Point(6, 19);
            this.lblTests.Name = "lblTests";
            this.lblTests.Size = new System.Drawing.Size(38, 13);
            this.lblTests.TabIndex = 1;
            this.lblTests.Text = "Tests";
            // 
            // lblMonitors
            // 
            this.lblMonitors.AutoSize = true;
            this.lblMonitors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMonitors.Location = new System.Drawing.Point(201, 19);
            this.lblMonitors.Name = "lblMonitors";
            this.lblMonitors.Size = new System.Drawing.Size(55, 13);
            this.lblMonitors.TabIndex = 3;
            this.lblMonitors.Text = "Monitors";
            // 
            // chkTestsFastConcurrencyResults
            // 
            this.chkTestsFastConcurrencyResults.AutoSize = true;
            this.chkTestsFastConcurrencyResults.Location = new System.Drawing.Point(19, 65);
            this.chkTestsFastConcurrencyResults.Name = "chkTestsFastConcurrencyResults";
            this.chkTestsFastConcurrencyResults.Size = new System.Drawing.Size(141, 17);
            this.chkTestsFastConcurrencyResults.TabIndex = 2;
            this.chkTestsFastConcurrencyResults.Text = "Fast concurrency results";
            this.chkTestsFastConcurrencyResults.UseVisualStyleBackColor = true;
            // 
            // chkTestsConfig
            // 
            this.chkTestsConfig.AutoSize = true;
            this.chkTestsConfig.Location = new System.Drawing.Point(19, 42);
            this.chkTestsConfig.Name = "chkTestsConfig";
            this.chkTestsConfig.Size = new System.Drawing.Size(88, 17);
            this.chkTestsConfig.TabIndex = 1;
            this.chkTestsConfig.Text = "Configuration";
            this.chkTestsConfig.UseVisualStyleBackColor = true;
            // 
            // chkTestsFastRunResults
            // 
            this.chkTestsFastRunResults.AutoSize = true;
            this.chkTestsFastRunResults.Location = new System.Drawing.Point(19, 88);
            this.chkTestsFastRunResults.Name = "chkTestsFastRunResults";
            this.chkTestsFastRunResults.Size = new System.Drawing.Size(95, 17);
            this.chkTestsFastRunResults.TabIndex = 3;
            this.chkTestsFastRunResults.Text = "Fast run resuts";
            this.chkTestsFastRunResults.UseVisualStyleBackColor = true;
            // 
            // chkMonitorsConfig
            // 
            this.chkMonitorsConfig.AutoSize = true;
            this.chkMonitorsConfig.Location = new System.Drawing.Point(214, 42);
            this.chkMonitorsConfig.Name = "chkMonitorsConfig";
            this.chkMonitorsConfig.Size = new System.Drawing.Size(88, 17);
            this.chkMonitorsConfig.TabIndex = 7;
            this.chkMonitorsConfig.Text = "Configuration";
            this.chkMonitorsConfig.UseVisualStyleBackColor = true;
            // 
            // chkMonitorsMetrics
            // 
            this.chkMonitorsMetrics.AutoSize = true;
            this.chkMonitorsMetrics.Location = new System.Drawing.Point(214, 88);
            this.chkMonitorsMetrics.Name = "chkMonitorsMetrics";
            this.chkMonitorsMetrics.Size = new System.Drawing.Size(60, 17);
            this.chkMonitorsMetrics.TabIndex = 9;
            this.chkMonitorsMetrics.Text = "Metrics";
            this.chkMonitorsMetrics.UseVisualStyleBackColor = true;
            // 
            // chkTestsMessages
            // 
            this.chkTestsMessages.AutoSize = true;
            this.chkTestsMessages.Location = new System.Drawing.Point(19, 165);
            this.chkTestsMessages.Name = "chkTestsMessages";
            this.chkTestsMessages.Size = new System.Drawing.Size(74, 17);
            this.chkTestsMessages.TabIndex = 5;
            this.chkTestsMessages.Text = "Messages";
            this.chkTestsMessages.UseVisualStyleBackColor = true;
            // 
            // chkMonitorsHWConfig
            // 
            this.chkMonitorsHWConfig.AutoSize = true;
            this.chkMonitorsHWConfig.Location = new System.Drawing.Point(214, 65);
            this.chkMonitorsHWConfig.Name = "chkMonitorsHWConfig";
            this.chkMonitorsHWConfig.Size = new System.Drawing.Size(136, 17);
            this.chkMonitorsHWConfig.TabIndex = 8;
            this.chkMonitorsHWConfig.Text = "Hardware configuration";
            this.chkMonitorsHWConfig.UseVisualStyleBackColor = true;
            // 
            // btnSet
            // 
            this.btnSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSet.BackColor = System.Drawing.Color.White;
            this.btnSet.Enabled = false;
            this.btnSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSet.Location = new System.Drawing.Point(79, 454);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(537, 25);
            this.btnSet.TabIndex = 2;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = false;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Publish";
            // 
            // grp
            // 
            this.grp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grp.Controls.Add(this.chkRequestResults);
            this.grp.Controls.Add(this.txtTcpHost);
            this.grp.Controls.Add(this.label4);
            this.grp.Controls.Add(this.chkTcp);
            this.grp.Controls.Add(this.label3);
            this.grp.Controls.Add(this.nudTcpPort);
            this.grp.Controls.Add(this.llblDeserialize);
            this.grp.Controls.Add(this.cboLogLevel);
            this.grp.Controls.Add(this.chkApplicationLogs);
            this.grp.Controls.Add(this.cboMessageLevel);
            this.grp.Controls.Add(this.chkTestsClientMonitoring);
            this.grp.Controls.Add(this.lblTests);
            this.grp.Controls.Add(this.lblMonitors);
            this.grp.Controls.Add(this.chkTestsFastConcurrencyResults);
            this.grp.Controls.Add(this.chkTestsConfig);
            this.grp.Controls.Add(this.chkTestsFastRunResults);
            this.grp.Controls.Add(this.label2);
            this.grp.Controls.Add(this.chkMonitorsHWConfig);
            this.grp.Controls.Add(this.chkMonitorsMetrics);
            this.grp.Controls.Add(this.chkTestsMessages);
            this.grp.Controls.Add(this.chkMonitorsConfig);
            this.grp.Enabled = false;
            this.grp.Location = new System.Drawing.Point(12, 64);
            this.grp.Name = "grp";
            this.grp.Size = new System.Drawing.Size(604, 384);
            this.grp.TabIndex = 0;
            this.grp.TabStop = false;
            // 
            // chkRequestResults
            // 
            this.chkRequestResults.AutoSize = true;
            this.chkRequestResults.Location = new System.Drawing.Point(19, 111);
            this.chkRequestResults.Name = "chkRequestResults";
            this.chkRequestResults.Size = new System.Drawing.Size(99, 17);
            this.chkRequestResults.TabIndex = 1015;
            this.chkRequestResults.Text = "Request results";
            this.chkRequestResults.UseVisualStyleBackColor = true;
            // 
            // txtTcpHost
            // 
            this.txtTcpHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTcpHost.Location = new System.Drawing.Point(51, 256);
            this.txtTcpHost.Name = "txtTcpHost";
            this.txtTcpHost.Size = new System.Drawing.Size(453, 20);
            this.txtTcpHost.TabIndex = 1014;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 259);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 1013;
            this.label4.Text = "Host:";
            // 
            // chkTcp
            // 
            this.chkTcp.AutoSize = true;
            this.chkTcp.Location = new System.Drawing.Point(19, 232);
            this.chkTcp.Name = "chkTcp";
            this.chkTcp.Size = new System.Drawing.Size(47, 17);
            this.chkTcp.TabIndex = 1010;
            this.chkTcp.Text = "TCP";
            this.toolTip.SetToolTip(this.chkTcp, "TCP send JSON, UTF8 encoded.\r\nWrite your own TCP client to receive the sent messa" +
        "ges.");
            this.chkTcp.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(513, 259);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 1012;
            this.label3.Text = "Port:";
            // 
            // nudTcpPort
            // 
            this.nudTcpPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudTcpPort.Location = new System.Drawing.Point(542, 257);
            this.nudTcpPort.Margin = new System.Windows.Forms.Padding(0, 5, 6, 6);
            this.nudTcpPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nudTcpPort.Name = "nudTcpPort";
            this.nudTcpPort.Size = new System.Drawing.Size(50, 20);
            this.nudTcpPort.TabIndex = 1011;
            this.nudTcpPort.Value = new decimal(new int[] {
            3337,
            0,
            0,
            0});
            // 
            // llblDeserialize
            // 
            this.llblDeserialize.ActiveLinkColor = System.Drawing.Color.Blue;
            this.llblDeserialize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llblDeserialize.AutoSize = true;
            this.llblDeserialize.DisabledLinkColor = System.Drawing.Color.Blue;
            this.llblDeserialize.Location = new System.Drawing.Point(534, 315);
            this.llblDeserialize.Name = "llblDeserialize";
            this.llblDeserialize.Size = new System.Drawing.Size(64, 13);
            this.llblDeserialize.TabIndex = 1009;
            this.llblDeserialize.TabStop = true;
            this.llblDeserialize.Text = "Deserialize?";
            this.toolTip.SetToolTip(this.llblDeserialize, resources.GetString("llblDeserialize.ToolTip"));
            this.llblDeserialize.VisitedLinkColor = System.Drawing.Color.Blue;
            this.llblDeserialize.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblDeserialize_LinkClicked);
            // 
            // cboLogLevel
            // 
            this.cboLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLogLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboLogLevel.FormattingEnabled = true;
            this.cboLogLevel.Items.AddRange(new object[] {
            "Info",
            "Warning",
            "Error"});
            this.cboLogLevel.Location = new System.Drawing.Point(520, 40);
            this.cboLogLevel.Name = "cboLogLevel";
            this.cboLogLevel.Size = new System.Drawing.Size(72, 21);
            this.cboLogLevel.TabIndex = 11;
            this.toolTip.SetToolTip(this.cboLogLevel, "Publish log entries with this log level and worst.\r\nThis setting depends on the l" +
        "og level setting in the application logging panel.\r\nFatals cannot be published.");
            // 
            // chkApplicationLogs
            // 
            this.chkApplicationLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkApplicationLogs.AutoSize = true;
            this.chkApplicationLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkApplicationLogs.Location = new System.Drawing.Point(482, 18);
            this.chkApplicationLogs.Name = "chkApplicationLogs";
            this.chkApplicationLogs.Size = new System.Drawing.Size(116, 17);
            this.chkApplicationLogs.TabIndex = 10;
            this.chkApplicationLogs.Text = "Application logs";
            this.chkApplicationLogs.UseVisualStyleBackColor = true;
            // 
            // cboMessageLevel
            // 
            this.cboMessageLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMessageLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMessageLevel.FormattingEnabled = true;
            this.cboMessageLevel.Items.AddRange(new object[] {
            "Info",
            "Warning",
            "Error"});
            this.cboMessageLevel.Location = new System.Drawing.Point(99, 163);
            this.cboMessageLevel.Name = "cboMessageLevel";
            this.cboMessageLevel.Size = new System.Drawing.Size(72, 21);
            this.cboMessageLevel.TabIndex = 6;
            this.toolTip.SetToolTip(this.cboMessageLevel, "Publish messages with this log level and worst.");
            // 
            // chkTestsClientMonitoring
            // 
            this.chkTestsClientMonitoring.AutoSize = true;
            this.chkTestsClientMonitoring.Location = new System.Drawing.Point(19, 134);
            this.chkTestsClientMonitoring.Name = "chkTestsClientMonitoring";
            this.chkTestsClientMonitoring.Size = new System.Drawing.Size(103, 17);
            this.chkTestsClientMonitoring.TabIndex = 4;
            this.chkTestsClientMonitoring.Text = "Client monitoring";
            this.chkTestsClientMonitoring.UseVisualStyleBackColor = true;
            // 
            // btnEnable
            // 
            this.btnEnable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEnable.BackColor = System.Drawing.Color.White;
            this.btnEnable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnable.Location = new System.Drawing.Point(12, 454);
            this.btnEnable.Name = "btnEnable";
            this.btnEnable.Size = new System.Drawing.Size(61, 25);
            this.btnEnable.TabIndex = 1;
            this.btnEnable.Text = "Enable";
            this.btnEnable.UseVisualStyleBackColor = false;
            this.btnEnable.Click += new System.EventHandler(this.btnEnable_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 15000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // PublishPanel
            // 
            this.ClientSize = new System.Drawing.Size(628, 491);
            this.Controls.Add(this.btnEnable);
            this.Controls.Add(this.grp);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.label1);
            this.Name = "PublishPanel";
            this.Text = "PublishPanel";
            this.grp.ResumeLayout(false);
            this.grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTcpPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTests;
        private System.Windows.Forms.Label lblMonitors;
        private System.Windows.Forms.CheckBox chkTestsFastConcurrencyResults;
        private System.Windows.Forms.CheckBox chkTestsConfig;
        private System.Windows.Forms.CheckBox chkTestsFastRunResults;
        private System.Windows.Forms.CheckBox chkMonitorsConfig;
        private System.Windows.Forms.CheckBox chkMonitorsMetrics;
        private System.Windows.Forms.CheckBox chkTestsMessages;
        private System.Windows.Forms.CheckBox chkMonitorsHWConfig;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grp;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.CheckBox chkTestsClientMonitoring;
        private System.Windows.Forms.ComboBox cboMessageLevel;
        private System.Windows.Forms.ComboBox cboLogLevel;
        private System.Windows.Forms.CheckBox chkApplicationLogs;
        private System.Windows.Forms.Button btnEnable;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.LinkLabel llblDeserialize;
        private System.Windows.Forms.CheckBox chkTcp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudTcpPort;
        private System.Windows.Forms.TextBox txtTcpHost;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkRequestResults;
    }
}