namespace vApus.Stresstest {
    partial class EditLog {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditLog));
            this.tc = new vApus.Util.TabControlWithAdjustableBorders();
            this.tpCapture = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddAction = new System.Windows.Forms.Button();
            this.btnPauseContinue = new System.Windows.Forms.Button();
            this.btnStartStopAndExport = new System.Windows.Forms.Button();
            this.tpImport = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.fctxtxImport = new FastColoredTextBoxNS.FastColoredTextBox();
            this.chkDeny = new System.Windows.Forms.CheckBox();
            this.chkAllow = new System.Windows.Forms.CheckBox();
            this.dgvDeny = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvAllow = new System.Windows.Forms.DataGridView();
            this.clmIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tpMisc = new System.Windows.Forms.TabPage();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tc.SuspendLayout();
            this.tpCapture.SuspendLayout();
            this.tpImport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeny)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllow)).BeginInit();
            this.SuspendLayout();
            // 
            // tc
            // 
            this.tc.BottomVisible = false;
            this.tc.Controls.Add(this.tpCapture);
            this.tc.Controls.Add(this.tpImport);
            this.tc.Controls.Add(this.tpMisc);
            this.tc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tc.ImageList = this.imageList;
            this.tc.LeftVisible = true;
            this.tc.Location = new System.Drawing.Point(0, 3);
            this.tc.Name = "tc";
            this.tc.RightVisible = false;
            this.tc.SelectedIndex = 0;
            this.tc.Size = new System.Drawing.Size(748, 606);
            this.tc.TabIndex = 0;
            this.tc.TopVisible = true;
            // 
            // tpCapture
            // 
            this.tpCapture.Controls.Add(this.chkDeny);
            this.tpCapture.Controls.Add(this.chkAllow);
            this.tpCapture.Controls.Add(this.dgvDeny);
            this.tpCapture.Controls.Add(this.dgvAllow);
            this.tpCapture.Controls.Add(this.label1);
            this.tpCapture.Controls.Add(this.btnAddAction);
            this.tpCapture.Controls.Add(this.btnPauseContinue);
            this.tpCapture.Controls.Add(this.btnStartStopAndExport);
            this.tpCapture.ImageIndex = 0;
            this.tpCapture.Location = new System.Drawing.Point(4, 23);
            this.tpCapture.Name = "tpCapture";
            this.tpCapture.Padding = new System.Windows.Forms.Padding(3);
            this.tpCapture.Size = new System.Drawing.Size(743, 582);
            this.tpCapture.TabIndex = 0;
            this.tpCapture.Text = "Capture HTTP(S)";
            this.tpCapture.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(731, 46);
            this.label1.TabIndex = 29;
            this.label1.Text = "Start Internet Explorer (!) and do typical actions (e.g. logging in or browsing a" +
    " specific page) at a web application of choice.\r\nAll captured requests can be gr" +
    "ouped using \'Add User Action\'.";
            // 
            // btnAddAction
            // 
            this.btnAddAction.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAddAction.AutoSize = true;
            this.btnAddAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddAction.BackColor = System.Drawing.Color.White;
            this.btnAddAction.Enabled = false;
            this.btnAddAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddAction.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddAction.Location = new System.Drawing.Point(372, 552);
            this.btnAddAction.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnAddAction.Name = "btnAddAction";
            this.btnAddAction.Size = new System.Drawing.Size(111, 24);
            this.btnAddAction.TabIndex = 6;
            this.btnAddAction.Text = "Add User Action";
            this.btnAddAction.UseVisualStyleBackColor = false;
            // 
            // btnPauseContinue
            // 
            this.btnPauseContinue.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnPauseContinue.AutoSize = true;
            this.btnPauseContinue.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPauseContinue.BackColor = System.Drawing.Color.White;
            this.btnPauseContinue.Enabled = false;
            this.btnPauseContinue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPauseContinue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPauseContinue.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPauseContinue.Location = new System.Drawing.Point(312, 552);
            this.btnPauseContinue.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnPauseContinue.Name = "btnPauseContinue";
            this.btnPauseContinue.Size = new System.Drawing.Size(54, 24);
            this.btnPauseContinue.TabIndex = 5;
            this.btnPauseContinue.Text = "Pause";
            this.btnPauseContinue.UseVisualStyleBackColor = false;
            // 
            // btnStartStopAndExport
            // 
            this.btnStartStopAndExport.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStartStopAndExport.AutoSize = true;
            this.btnStartStopAndExport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnStartStopAndExport.BackColor = System.Drawing.Color.White;
            this.btnStartStopAndExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartStopAndExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartStopAndExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStartStopAndExport.Location = new System.Drawing.Point(260, 552);
            this.btnStartStopAndExport.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnStartStopAndExport.Name = "btnStartStopAndExport";
            this.btnStartStopAndExport.Size = new System.Drawing.Size(46, 24);
            this.btnStartStopAndExport.TabIndex = 4;
            this.btnStartStopAndExport.Text = "Start";
            this.btnStartStopAndExport.UseVisualStyleBackColor = false;
            // 
            // tpImport
            // 
            this.tpImport.Controls.Add(this.button1);
            this.tpImport.Controls.Add(this.btnBrowse);
            this.tpImport.Controls.Add(this.fctxtxImport);
            this.tpImport.ImageIndex = 1;
            this.tpImport.Location = new System.Drawing.Point(4, 23);
            this.tpImport.Name = "tpImport";
            this.tpImport.Padding = new System.Windows.Forms.Padding(3);
            this.tpImport.Size = new System.Drawing.Size(743, 585);
            this.tpImport.TabIndex = 1;
            this.tpImport.Text = "Import from Text";
            this.tpImport.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.AutoSize = true;
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.Enabled = false;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(305, 555);
            this.button1.MaximumSize = new System.Drawing.Size(9999, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(54, 24);
            this.button1.TabIndex = 1;
            this.button1.Text = "Import";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnBrowse.AutoSize = true;
            this.btnBrowse.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBrowse.BackColor = System.Drawing.Color.White;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrowse.Location = new System.Drawing.Point(365, 555);
            this.btnBrowse.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(72, 24);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = false;
            // 
            // fctxtxImport
            // 
            this.fctxtxImport.AllowDrop = true;
            this.fctxtxImport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fctxtxImport.AutoScrollMinSize = new System.Drawing.Size(0, 14);
            this.fctxtxImport.BackBrush = null;
            this.fctxtxImport.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctxtxImport.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctxtxImport.IsReplaceMode = false;
            this.fctxtxImport.Location = new System.Drawing.Point(3, 3);
            this.fctxtxImport.Name = "fctxtxImport";
            this.fctxtxImport.Paddings = new System.Windows.Forms.Padding(0);
            this.fctxtxImport.PreferredLineWidth = 65536;
            this.fctxtxImport.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctxtxImport.Size = new System.Drawing.Size(737, 546);
            this.fctxtxImport.TabIndex = 0;
            this.fctxtxImport.WordWrap = true;
            // 
            // chkDeny
            // 
            this.chkDeny.AutoSize = true;
            this.chkDeny.Location = new System.Drawing.Point(9, 279);
            this.chkDeny.Name = "chkDeny";
            this.chkDeny.Size = new System.Drawing.Size(54, 17);
            this.chkDeny.TabIndex = 2;
            this.chkDeny.Text = "Deny:";
            this.toolTip.SetToolTip(this.chkDeny, "Check to use this filter.\r\nEven if an entry of the \'allow list\' is in a referer y" +
        "ou want entries from for instance a CDN but you don\'t want entries from for inst" +
        "ance google analytics.\r\n");
            this.chkDeny.UseVisualStyleBackColor = true;
            // 
            // chkAllow
            // 
            this.chkAllow.AutoSize = true;
            this.chkAllow.Location = new System.Drawing.Point(9, 52);
            this.chkAllow.Name = "chkAllow";
            this.chkAllow.Size = new System.Drawing.Size(54, 17);
            this.chkAllow.TabIndex = 0;
            this.chkAllow.Text = "Allow:";
            this.toolTip.SetToolTip(this.chkAllow, "Check to use this filter.");
            this.chkAllow.UseVisualStyleBackColor = true;
            // 
            // dgvDeny
            // 
            this.dgvDeny.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDeny.BackgroundColor = System.Drawing.Color.White;
            this.dgvDeny.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDeny.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvDeny.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDeny.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDeny.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvDeny.Enabled = false;
            this.dgvDeny.EnableHeadersVisualStyles = false;
            this.dgvDeny.Location = new System.Drawing.Point(9, 302);
            this.dgvDeny.Name = "dgvDeny";
            this.dgvDeny.Size = new System.Drawing.Size(728, 195);
            this.dgvDeny.TabIndex = 3;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "IP / (Part of a) Destination Host";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dgvAllow
            // 
            this.dgvAllow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAllow.BackgroundColor = System.Drawing.Color.White;
            this.dgvAllow.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAllow.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvAllow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAllow.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmIP});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAllow.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvAllow.Enabled = false;
            this.dgvAllow.EnableHeadersVisualStyles = false;
            this.dgvAllow.Location = new System.Drawing.Point(9, 75);
            this.dgvAllow.Name = "dgvAllow";
            this.dgvAllow.Size = new System.Drawing.Size(728, 195);
            this.dgvAllow.TabIndex = 1;
            // 
            // clmIP
            // 
            this.clmIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmIP.HeaderText = "IP / (Part of a) Destination Host / (Part of a) Referer";
            this.clmIP.Name = "clmIP";
            this.clmIP.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // tpMisc
            // 
            this.tpMisc.Location = new System.Drawing.Point(4, 23);
            this.tpMisc.Name = "tpMisc";
            this.tpMisc.Padding = new System.Windows.Forms.Padding(3);
            this.tpMisc.Size = new System.Drawing.Size(743, 585);
            this.tpMisc.TabIndex = 2;
            this.tpMisc.Text = "Misc Tasks";
            this.tpMisc.UseVisualStyleBackColor = true;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "capture.png");
            this.imageList.Images.SetKeyName(1, "import.png");
            // 
            // EditLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tc);
            this.Name = "EditLog";
            this.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.Size = new System.Drawing.Size(748, 609);
            this.tc.ResumeLayout(false);
            this.tpCapture.ResumeLayout(false);
            this.tpCapture.PerformLayout();
            this.tpImport.ResumeLayout(false);
            this.tpImport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeny)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Util.TabControlWithAdjustableBorders tc;
        private System.Windows.Forms.TabPage tpCapture;
        private System.Windows.Forms.TabPage tpImport;
        private System.Windows.Forms.Button btnAddAction;
        private System.Windows.Forms.Button btnPauseContinue;
        private System.Windows.Forms.Button btnStartStopAndExport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnBrowse;
        private FastColoredTextBoxNS.FastColoredTextBox fctxtxImport;
        private System.Windows.Forms.CheckBox chkDeny;
        private System.Windows.Forms.CheckBox chkAllow;
        private System.Windows.Forms.DataGridView dgvDeny;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridView dgvAllow;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmIP;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TabPage tpMisc;
        private System.Windows.Forms.ImageList imageList;
    }
}
