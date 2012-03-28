namespace vApus.Stresstest
{
    partial class Record
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
            //Cleanup this
            try { _testRequestThroughProxyWaitHandle.Set(); }
            catch { }
            try { _testRequestThroughProxyWaitHandle.Dispose(); }
            catch { }
            _testRequestThroughProxyWaitHandle = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Record));
            this.btnStart = new System.Windows.Forms.Button();
            this.btnAddAction = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvIPsOrDomainNames = new System.Windows.Forms.DataGridView();
            this.clmIPsORDomainNames = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkActionize = new System.Windows.Forms.CheckBox();
            this.dgvPorts = new System.Windows.Forms.DataGridView();
            this.clmPorts = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flpLog = new System.Windows.Forms.FlowLayoutPanel();
            this.kvpTotalEntries = new vApus.Util.KeyValuePairControl();
            this.kvpIgnored = new vApus.Util.KeyValuePairControl();
            this.kvpMalformed = new vApus.Util.KeyValuePairControl();
            this.kvpDiscarded = new vApus.Util.KeyValuePairControl();
            this.kvpInEmptyAction = new vApus.Util.KeyValuePairControl();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.lblTestProxy = new System.Windows.Forms.Label();
            this.pnlSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIPsOrDomainNames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPorts)).BeginInit();
            this.flpLog.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStart.BackColor = System.Drawing.Color.White;
            this.btnStart.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnStart.BackgroundImage")));
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Location = new System.Drawing.Point(12, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(34, 34);
            this.btnStart.TabIndex = 1;
            this.toolTip.SetToolTip(this.btnStart, "This sets the proxy and starts recording.");
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnAddAction
            // 
            this.btnAddAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddAction.BackColor = System.Drawing.Color.White;
            this.btnAddAction.Enabled = false;
            this.btnAddAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddAction.Image = ((System.Drawing.Image)(resources.GetObject("btnAddAction.Image")));
            this.btnAddAction.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddAction.Location = new System.Drawing.Point(92, 6);
            this.btnAddAction.Name = "btnAddAction";
            this.btnAddAction.Size = new System.Drawing.Size(100, 30);
            this.btnAddAction.TabIndex = 3;
            this.btnAddAction.Text = "User Action";
            this.btnAddAction.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnAddAction, "Put the recorded log entries in a new user action.");
            this.btnAddAction.UseVisualStyleBackColor = false;
            this.btnAddAction.Click += new System.EventHandler(this.btnAddAction_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.BackColor = System.Drawing.Color.Silver;
            this.btnStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnStop.BackgroundImage")));
            this.btnStop.Enabled = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Location = new System.Drawing.Point(52, 6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(34, 34);
            this.btnStop.TabIndex = 2;
            this.toolTip.SetToolTip(this.btnStop, "Pause recording, you can change the settings while paused.");
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // pnlSettings
            // 
            this.pnlSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSettings.BackColor = System.Drawing.SystemColors.Control;
            this.pnlSettings.Controls.Add(this.label2);
            this.pnlSettings.Controls.Add(this.label1);
            this.pnlSettings.Controls.Add(this.dgvIPsOrDomainNames);
            this.pnlSettings.Controls.Add(this.chkActionize);
            this.pnlSettings.Controls.Add(this.dgvPorts);
            this.pnlSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(695, 314);
            this.pnlSettings.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(89, 280);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(322, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "<-- You can set this only before you start recording, not in between.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 16);
            this.label1.TabIndex = 22;
            this.label1.Text = "Filter traffic at";
            // 
            // dgvIPsOrDomainNames
            // 
            this.dgvIPsOrDomainNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvIPsOrDomainNames.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvIPsOrDomainNames.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIPsOrDomainNames.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmIPsORDomainNames});
            this.dgvIPsOrDomainNames.EnableHeadersVisualStyles = false;
            this.dgvIPsOrDomainNames.Location = new System.Drawing.Point(12, 28);
            this.dgvIPsOrDomainNames.Name = "dgvIPsOrDomainNames";
            this.dgvIPsOrDomainNames.Size = new System.Drawing.Size(410, 124);
            this.dgvIPsOrDomainNames.TabIndex = 0;
            this.dgvIPsOrDomainNames.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIPsOrDomainNames_CellEndEdit);
            // 
            // clmIPsORDomainNames
            // 
            this.clmIPsORDomainNames.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmIPsORDomainNames.HeaderText = "IPs or Domain Names";
            this.clmIPsORDomainNames.Name = "clmIPsORDomainNames";
            this.clmIPsORDomainNames.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // chkActionize
            // 
            this.chkActionize.AutoSize = true;
            this.chkActionize.Checked = true;
            this.chkActionize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActionize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkActionize.Location = new System.Drawing.Point(12, 277);
            this.chkActionize.Name = "chkActionize";
            this.chkActionize.Size = new System.Drawing.Size(81, 20);
            this.chkActionize.TabIndex = 2;
            this.chkActionize.Text = "Actionize";
            this.toolTip.SetToolTip(this.chkActionize, "This puts all entries in one big user action, you can split this user actions up " +
        "while recording.\r\n\r\nWarning: You can set this only before you start recording, n" +
        "ot in between.\r\n");
            this.chkActionize.UseVisualStyleBackColor = true;
            this.chkActionize.CheckedChanged += new System.EventHandler(this.chkActionize_CheckedChanged);
            // 
            // dgvPorts
            // 
            this.dgvPorts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPorts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPorts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPorts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmPorts});
            this.dgvPorts.EnableHeadersVisualStyles = false;
            this.dgvPorts.Location = new System.Drawing.Point(12, 158);
            this.dgvPorts.Name = "dgvPorts";
            this.dgvPorts.Size = new System.Drawing.Size(410, 100);
            this.dgvPorts.TabIndex = 1;
            this.dgvPorts.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPorts_CellEndEdit);
            // 
            // clmPorts
            // 
            this.clmPorts.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmPorts.HeaderText = "Ports";
            this.clmPorts.Name = "clmPorts";
            this.clmPorts.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // flpLog
            // 
            this.flpLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flpLog.BackColor = System.Drawing.Color.White;
            this.flpLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpLog.Controls.Add(this.kvpTotalEntries);
            this.flpLog.Controls.Add(this.kvpIgnored);
            this.flpLog.Controls.Add(this.kvpMalformed);
            this.flpLog.Controls.Add(this.kvpDiscarded);
            this.flpLog.Controls.Add(this.kvpInEmptyAction);
            this.flpLog.Location = new System.Drawing.Point(440, 12);
            this.flpLog.Name = "flpLog";
            this.flpLog.Size = new System.Drawing.Size(242, 342);
            this.flpLog.TabIndex = 9;
            // 
            // kvpTotalEntries
            // 
            this.kvpTotalEntries.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.kvpTotalEntries.Key = "Total entries";
            this.kvpTotalEntries.Location = new System.Drawing.Point(3, 3);
            this.kvpTotalEntries.Name = "kvpTotalEntries";
            this.kvpTotalEntries.Size = new System.Drawing.Size(97, 16);
            this.kvpTotalEntries.TabIndex = 0;
            this.kvpTotalEntries.Tooltip = "The total of the recorded log entries.";
            this.kvpTotalEntries.Value = "0";
            // 
            // kvpIgnored
            // 
            this.kvpIgnored.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.kvpIgnored.Key = "Ignored";
            this.kvpIgnored.Location = new System.Drawing.Point(106, 3);
            this.kvpIgnored.Name = "kvpIgnored";
            this.kvpIgnored.Size = new System.Drawing.Size(69, 16);
            this.kvpIgnored.TabIndex = 2;
            this.kvpIgnored.Tooltip = "The entries that are filtered out. If you believe too many items have been filter" +
    "ed, you may need to adapt your filtered destinations.";
            this.kvpIgnored.Value = "0";
            // 
            // kvpMalformed
            // 
            this.kvpMalformed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.kvpMalformed.Key = "Malformed";
            this.kvpMalformed.Location = new System.Drawing.Point(3, 25);
            this.kvpMalformed.Name = "kvpMalformed";
            this.kvpMalformed.Size = new System.Drawing.Size(84, 16);
            this.kvpMalformed.TabIndex = 3;
            this.kvpMalformed.Tooltip = "These are entries that are not conform with the log rule set.\r\nThey are kept anyw" +
    "ay, you can choose what to do with them afterwards.";
            this.kvpMalformed.Value = "0";
            // 
            // kvpDiscarded
            // 
            this.kvpDiscarded.BackColor = System.Drawing.Color.Red;
            this.kvpDiscarded.Key = "Discarded";
            this.kvpDiscarded.Location = new System.Drawing.Point(93, 25);
            this.kvpDiscarded.Name = "kvpDiscarded";
            this.kvpDiscarded.Size = new System.Drawing.Size(83, 16);
            this.kvpDiscarded.TabIndex = 4;
            this.kvpDiscarded.Tooltip = "Broken requests are discarded and are never taken into account.";
            this.kvpDiscarded.Value = "0";
            // 
            // kvpInEmptyAction
            // 
            this.kvpInEmptyAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.kvpInEmptyAction.Key = "In empty action";
            this.kvpInEmptyAction.Location = new System.Drawing.Point(3, 47);
            this.kvpInEmptyAction.Name = "kvpInEmptyAction";
            this.kvpInEmptyAction.Size = new System.Drawing.Size(113, 16);
            this.kvpInEmptyAction.TabIndex = 1;
            this.kvpInEmptyAction.Tooltip = "Loose log entries or log entries in an empty user action.";
            this.kvpInEmptyAction.Value = "0";
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 200;
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 200;
            this.toolTip.ReshowDelay = 40;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.AutoSize = true;
            this.btnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.Color.DarkGreen;
            this.btnOK.Location = new System.Drawing.Point(382, 6);
            this.btnOK.MaximumSize = new System.Drawing.Size(1000, 34);
            this.btnOK.MinimumSize = new System.Drawing.Size(0, 34);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(40, 34);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.toolTip.SetToolTip(this.btnOK, "Close this dialog and import the recorded log entries.");
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnOK);
            this.pnlButtons.Controls.Add(this.btnStart);
            this.pnlButtons.Controls.Add(this.btnStop);
            this.pnlButtons.Controls.Add(this.btnAddAction);
            this.pnlButtons.Location = new System.Drawing.Point(0, 314);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(0);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(695, 47);
            this.pnlButtons.TabIndex = 10;
            // 
            // lblTestProxy
            // 
            this.lblTestProxy.AutoSize = true;
            this.lblTestProxy.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblTestProxy.ForeColor = System.Drawing.Color.DarkGray;
            this.lblTestProxy.Image = global::vApus.Stresstest.Properties.Resources.Wait;
            this.lblTestProxy.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblTestProxy.Location = new System.Drawing.Point(198, 328);
            this.lblTestProxy.Name = "lblTestProxy";
            this.lblTestProxy.Size = new System.Drawing.Size(176, 18);
            this.lblTestProxy.TabIndex = 5;
            this.lblTestProxy.Text = "Testing the Proxy..  ";
            this.lblTestProxy.Visible = false;
            // 
            // Record
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(694, 362);
            this.Controls.Add(this.lblTestProxy);
            this.Controls.Add(this.flpLog);
            this.Controls.Add(this.pnlSettings);
            this.Controls.Add(this.pnlButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Record";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Record HTTP";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Record_FormClosing);
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIPsOrDomainNames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPorts)).EndInit();
            this.flpLog.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnAddAction;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.CheckBox chkActionize;
        private System.Windows.Forms.DataGridView dgvPorts;
        private System.Windows.Forms.FlowLayoutPanel flpLog;
        private vApus.Util.KeyValuePairControl kvpTotalEntries;
        private vApus.Util.KeyValuePairControl kvpInEmptyAction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvIPsOrDomainNames;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmIPsORDomainNames;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPorts;
        private Util.KeyValuePairControl kvpIgnored;
        private Util.KeyValuePairControl kvpMalformed;
        private Util.KeyValuePairControl kvpDiscarded;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Label lblTestProxy;
        private System.Windows.Forms.Label label2;
    }
}