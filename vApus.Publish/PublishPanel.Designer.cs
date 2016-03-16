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
            this.chkTestsFastConcurrencyResults = new System.Windows.Forms.Label();
            this.chkTestsConfig = new System.Windows.Forms.Label();
            this.chkTestsFastRunResults = new System.Windows.Forms.Label();
            this.chkMonitorsConfig = new System.Windows.Forms.Label();
            this.chkMonitorsMetrics = new System.Windows.Forms.Label();
            this.chkTestsMessages = new System.Windows.Forms.Label();
            this.chkMonitorsHWConfig = new System.Windows.Forms.Label();
            this.btnSet = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.grp = new System.Windows.Forms.GroupBox();
            this.btnLaunchvApusPublishItemsHandler = new System.Windows.Forms.CheckBox();
            this.chkRequestResults = new System.Windows.Forms.Label();
            this.txtTcpHost = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudTcpPort = new System.Windows.Forms.NumericUpDown();
            this.llblDeserialize = new System.Windows.Forms.LinkLabel();
            this.chkApplicationLogs = new System.Windows.Forms.Label();
            this.chkTestsClientMonitoring = new System.Windows.Forms.Label();
            this.btnEnable = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTcpPort)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(604, 88);
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
            this.chkTestsFastConcurrencyResults.Size = new System.Drawing.Size(122, 13);
            this.chkTestsFastConcurrencyResults.TabIndex = 2;
            this.chkTestsFastConcurrencyResults.Text = "Fast concurrency results";
            // 
            // chkTestsConfig
            // 
            this.chkTestsConfig.AutoSize = true;
            this.chkTestsConfig.Location = new System.Drawing.Point(19, 42);
            this.chkTestsConfig.Name = "chkTestsConfig";
            this.chkTestsConfig.Size = new System.Drawing.Size(69, 13);
            this.chkTestsConfig.TabIndex = 1;
            this.chkTestsConfig.Text = "Configuration";
            // 
            // chkTestsFastRunResults
            // 
            this.chkTestsFastRunResults.AutoSize = true;
            this.chkTestsFastRunResults.Location = new System.Drawing.Point(19, 88);
            this.chkTestsFastRunResults.Name = "chkTestsFastRunResults";
            this.chkTestsFastRunResults.Size = new System.Drawing.Size(76, 13);
            this.chkTestsFastRunResults.TabIndex = 3;
            this.chkTestsFastRunResults.Text = "Fast run resuts";
            // 
            // chkMonitorsConfig
            // 
            this.chkMonitorsConfig.AutoSize = true;
            this.chkMonitorsConfig.Location = new System.Drawing.Point(214, 42);
            this.chkMonitorsConfig.Name = "chkMonitorsConfig";
            this.chkMonitorsConfig.Size = new System.Drawing.Size(69, 13);
            this.chkMonitorsConfig.TabIndex = 7;
            this.chkMonitorsConfig.Text = "Configuration";
            // 
            // chkMonitorsMetrics
            // 
            this.chkMonitorsMetrics.AutoSize = true;
            this.chkMonitorsMetrics.Location = new System.Drawing.Point(214, 88);
            this.chkMonitorsMetrics.Name = "chkMonitorsMetrics";
            this.chkMonitorsMetrics.Size = new System.Drawing.Size(41, 13);
            this.chkMonitorsMetrics.TabIndex = 9;
            this.chkMonitorsMetrics.Text = "Metrics";
            // 
            // chkTestsMessages
            // 
            this.chkTestsMessages.AutoSize = true;
            this.chkTestsMessages.Location = new System.Drawing.Point(19, 158);
            this.chkTestsMessages.Name = "chkTestsMessages";
            this.chkTestsMessages.Size = new System.Drawing.Size(78, 13);
            this.chkTestsMessages.TabIndex = 5;
            this.chkTestsMessages.Text = "Test messages";
            // 
            // chkMonitorsHWConfig
            // 
            this.chkMonitorsHWConfig.AutoSize = true;
            this.chkMonitorsHWConfig.Location = new System.Drawing.Point(214, 65);
            this.chkMonitorsHWConfig.Name = "chkMonitorsHWConfig";
            this.chkMonitorsHWConfig.Size = new System.Drawing.Size(117, 13);
            this.chkMonitorsHWConfig.TabIndex = 8;
            this.chkMonitorsHWConfig.Text = "Hardware configuration";
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
            this.label2.Location = new System.Drawing.Point(6, 234);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Publish over TCP";
            // 
            // grp
            // 
            this.grp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grp.Controls.Add(this.label5);
            this.grp.Controls.Add(this.btnLaunchvApusPublishItemsHandler);
            this.grp.Controls.Add(this.chkRequestResults);
            this.grp.Controls.Add(this.txtTcpHost);
            this.grp.Controls.Add(this.label4);
            this.grp.Controls.Add(this.label3);
            this.grp.Controls.Add(this.nudTcpPort);
            this.grp.Controls.Add(this.llblDeserialize);
            this.grp.Controls.Add(this.chkApplicationLogs);
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
            this.grp.Location = new System.Drawing.Point(12, 100);
            this.grp.Name = "grp";
            this.grp.Size = new System.Drawing.Size(604, 348);
            this.grp.TabIndex = 0;
            this.grp.TabStop = false;
            // 
            // btnLaunchvApusPublishItemsHandler
            // 
            this.btnLaunchvApusPublishItemsHandler.AutoSize = true;
            this.btnLaunchvApusPublishItemsHandler.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLaunchvApusPublishItemsHandler.Location = new System.Drawing.Point(54, 313);
            this.btnLaunchvApusPublishItemsHandler.Name = "btnLaunchvApusPublishItemsHandler";
            this.btnLaunchvApusPublishItemsHandler.Size = new System.Drawing.Size(214, 17);
            this.btnLaunchvApusPublishItemsHandler.TabIndex = 2;
            this.btnLaunchvApusPublishItemsHandler.Text = "Auto-launch vApus publish items handler";
            this.btnLaunchvApusPublishItemsHandler.UseVisualStyleBackColor = false;
            // 
            // chkRequestResults
            // 
            this.chkRequestResults.AutoSize = true;
            this.chkRequestResults.Location = new System.Drawing.Point(19, 111);
            this.chkRequestResults.Name = "chkRequestResults";
            this.chkRequestResults.Size = new System.Drawing.Size(80, 13);
            this.chkRequestResults.TabIndex = 1015;
            this.chkRequestResults.Text = "Request results";
            // 
            // txtTcpHost
            // 
            this.txtTcpHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTcpHost.Location = new System.Drawing.Point(54, 256);
            this.txtTcpHost.Name = "txtTcpHost";
            this.txtTcpHost.Size = new System.Drawing.Size(450, 20);
            this.txtTcpHost.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 259);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 7, 0, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 1013;
            this.label4.Text = "Host:";
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
            this.nudTcpPort.TabIndex = 1;
            this.nudTcpPort.Value = new decimal(new int[] {
            4337,
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
            this.llblDeserialize.TabIndex = 3;
            this.llblDeserialize.TabStop = true;
            this.llblDeserialize.Text = "Deserialize?";
            this.toolTip.SetToolTip(this.llblDeserialize, resources.GetString("llblDeserialize.ToolTip"));
            this.llblDeserialize.VisitedLinkColor = System.Drawing.Color.Blue;
            this.llblDeserialize.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblDeserialize_LinkClicked);
            // 
            // chkApplicationLogs
            // 
            this.chkApplicationLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkApplicationLogs.AutoSize = true;
            this.chkApplicationLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkApplicationLogs.Location = new System.Drawing.Point(482, 18);
            this.chkApplicationLogs.Name = "chkApplicationLogs";
            this.chkApplicationLogs.Size = new System.Drawing.Size(97, 13);
            this.chkApplicationLogs.TabIndex = 10;
            this.chkApplicationLogs.Text = "Application logs";
            // 
            // chkTestsClientMonitoring
            // 
            this.chkTestsClientMonitoring.AutoSize = true;
            this.chkTestsClientMonitoring.Location = new System.Drawing.Point(19, 134);
            this.chkTestsClientMonitoring.Name = "chkTestsClientMonitoring";
            this.chkTestsClientMonitoring.Size = new System.Drawing.Size(84, 13);
            this.chkTestsClientMonitoring.TabIndex = 4;
            this.chkTestsClientMonitoring.Text = "Client monitoring";
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
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 1016;
            this.label5.Text = "Test values";
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

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTests;
        private System.Windows.Forms.Label lblMonitors;
        private System.Windows.Forms.Label chkTestsFastConcurrencyResults;
        private System.Windows.Forms.Label chkTestsConfig;
        private System.Windows.Forms.Label chkTestsFastRunResults;
        private System.Windows.Forms.Label chkMonitorsConfig;
        private System.Windows.Forms.Label chkMonitorsMetrics;
        private System.Windows.Forms.Label chkTestsMessages;
        private System.Windows.Forms.Label chkMonitorsHWConfig;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grp;
        private System.Windows.Forms.Label chkTestsClientMonitoring;
        private System.Windows.Forms.Label chkApplicationLogs;
        private System.Windows.Forms.Button btnEnable;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.LinkLabel llblDeserialize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudTcpPort;
        private System.Windows.Forms.TextBox txtTcpHost;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label chkRequestResults;
        private System.Windows.Forms.CheckBox btnLaunchvApusPublishItemsHandler;
        private System.Windows.Forms.Label label5;
    }
}