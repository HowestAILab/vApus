namespace vApus.Stresstest
{
    partial class Execute
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Execute));
            this.tvw = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.flpTest = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPlay = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nudThreads = new System.Windows.Forms.NumericUpDown();
            this.btnStop = new System.Windows.Forms.Button();
            this.pb = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.cboThread = new System.Windows.Forms.ComboBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tpTvw = new System.Windows.Forms.TabPage();
            this.tpSelectedNode = new System.Windows.Forms.TabPage();
            this.rtxtSelectedNode = new System.Windows.Forms.RichTextBox();
            this.btnPreviousError = new System.Windows.Forms.ToolStripButton();
            this.btnSelectError = new System.Windows.Forms.ToolStripButton();
            this.btnNextError = new System.Windows.Forms.ToolStripButton();
            this.split = new System.Windows.Forms.SplitContainer();
            this.flp = new System.Windows.Forms.FlowLayoutPanel();
            this.ruleSetSyntaxItemPanel = new vApus.Stresstest.ConnectionProxyRuleSetSyntaxItemPanel();
            this.flpTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreads)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tpTvw.SuspendLayout();
            this.tpSelectedNode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvw
            // 
            this.tvw.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvw.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.tvw.HideSelection = false;
            this.tvw.ImageIndex = 0;
            this.tvw.ImageList = this.imageList;
            this.tvw.Location = new System.Drawing.Point(3, 3);
            this.tvw.Name = "tvw";
            this.tvw.SelectedImageIndex = 0;
            this.tvw.Size = new System.Drawing.Size(471, 193);
            this.tvw.TabIndex = 5;
            this.tvw.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvw_AfterSelect);
            this.tvw.DoubleClick += new System.EventHandler(this.tvw_DoubleClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Empty.png");
            this.imageList.Images.SetKeyName(1, "OK.png");
            this.imageList.Images.SetKeyName(2, "Error.png");
            // 
            // flpTest
            // 
            this.flpTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.flpTest.Controls.Add(this.btnPlay);
            this.flpTest.Controls.Add(this.label1);
            this.flpTest.Controls.Add(this.nudThreads);
            this.flpTest.Controls.Add(this.btnStop);
            this.flpTest.Location = new System.Drawing.Point(12, 12);
            this.flpTest.Name = "flpTest";
            this.flpTest.Size = new System.Drawing.Size(483, 35);
            this.flpTest.TabIndex = 8;
            // 
            // btnPlay
            // 
            this.btnPlay.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnPlay.AutoSize = true;
            this.btnPlay.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.Image = global::vApus.Stresstest.Properties.Resources.Play;
            this.btnPlay.Location = new System.Drawing.Point(3, 3);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(22, 22);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Threads:";
            // 
            // nudThreads
            // 
            this.nudThreads.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudThreads.Location = new System.Drawing.Point(86, 4);
            this.nudThreads.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.nudThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudThreads.Name = "nudThreads";
            this.nudThreads.Size = new System.Drawing.Size(50, 20);
            this.nudThreads.TabIndex = 1;
            this.nudThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnStop.AutoSize = true;
            this.btnStop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnStop.Enabled = false;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.Location = new System.Drawing.Point(142, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(22, 22);
            this.btnStop.TabIndex = 0;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // pb
            // 
            this.pb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb.Location = new System.Drawing.Point(284, 22);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(196, 5);
            this.pb.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Thread:";
            // 
            // cboThread
            // 
            this.cboThread.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboThread.FormattingEnabled = true;
            this.cboThread.Location = new System.Drawing.Point(86, 7);
            this.cboThread.Name = "cboThread";
            this.cboThread.Size = new System.Drawing.Size(192, 21);
            this.cboThread.TabIndex = 0;
            this.cboThread.SelectedIndexChanged += new System.EventHandler(this.cboThread_SelectedIndexChanged);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(281, 7);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(30, 13);
            this.lblProgress.TabIndex = 9;
            this.lblProgress.Text = "0 / ?";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblProgress);
            this.panel1.Controls.Add(this.pb);
            this.panel1.Controls.Add(this.cboThread);
            this.panel1.Controls.Add(this.tabControl);
            this.panel1.Location = new System.Drawing.Point(12, 53);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(483, 259);
            this.panel1.TabIndex = 13;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tpTvw);
            this.tabControl.Controls.Add(this.tpSelectedNode);
            this.tabControl.Location = new System.Drawing.Point(0, 34);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(485, 225);
            this.tabControl.TabIndex = 6;
            // 
            // tpTvw
            // 
            this.tpTvw.Controls.Add(this.tvw);
            this.tpTvw.Location = new System.Drawing.Point(4, 22);
            this.tpTvw.Name = "tpTvw";
            this.tpTvw.Padding = new System.Windows.Forms.Padding(3);
            this.tpTvw.Size = new System.Drawing.Size(477, 199);
            this.tpTvw.TabIndex = 0;
            this.tpTvw.Text = "Results";
            this.tpTvw.UseVisualStyleBackColor = true;
            // 
            // tpSelectedNode
            // 
            this.tpSelectedNode.Controls.Add(this.rtxtSelectedNode);
            this.tpSelectedNode.Location = new System.Drawing.Point(4, 22);
            this.tpSelectedNode.Name = "tpSelectedNode";
            this.tpSelectedNode.Padding = new System.Windows.Forms.Padding(3);
            this.tpSelectedNode.Size = new System.Drawing.Size(475, 197);
            this.tpSelectedNode.TabIndex = 1;
            this.tpSelectedNode.Text = "Selected Node";
            this.tpSelectedNode.UseVisualStyleBackColor = true;
            // 
            // rtxtSelectedNode
            // 
            this.rtxtSelectedNode.BackColor = System.Drawing.Color.White;
            this.rtxtSelectedNode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtSelectedNode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtSelectedNode.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.rtxtSelectedNode.Location = new System.Drawing.Point(3, 3);
            this.rtxtSelectedNode.Name = "rtxtSelectedNode";
            this.rtxtSelectedNode.ReadOnly = true;
            this.rtxtSelectedNode.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxtSelectedNode.Size = new System.Drawing.Size(469, 191);
            this.rtxtSelectedNode.TabIndex = 0;
            this.rtxtSelectedNode.Text = "";
            // 
            // btnPreviousError
            // 
            this.btnPreviousError.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnPreviousError.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPreviousError.Enabled = false;
            this.btnPreviousError.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPreviousError.Name = "btnPreviousError";
            this.btnPreviousError.Size = new System.Drawing.Size(23, 22);
            this.btnPreviousError.Text = "Previous Error";
            this.btnPreviousError.ToolTipText = "Previous Error";
            // 
            // btnSelectError
            // 
            this.btnSelectError.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnSelectError.Image = global::vApus.Stresstest.Properties.Resources.LogEntryError;
            this.btnSelectError.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectError.Name = "btnSelectError";
            this.btnSelectError.Size = new System.Drawing.Size(49, 22);
            this.btnSelectError.Text = "1 / ?";
            this.btnSelectError.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnSelectError.ToolTipText = "Go to Error";
            // 
            // btnNextError
            // 
            this.btnNextError.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnNextError.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNextError.Enabled = false;
            this.btnNextError.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNextError.Name = "btnNextError";
            this.btnNextError.Size = new System.Drawing.Size(23, 22);
            this.btnNextError.Text = "Next Error";
            this.btnNextError.ToolTipText = "Next Error";
            // 
            // split
            // 
            this.split.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.split.Location = new System.Drawing.Point(500, 0);
            this.split.Name = "split";
            this.split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.flp);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.ruleSetSyntaxItemPanel);
            this.split.Size = new System.Drawing.Size(400, 324);
            this.split.SplitterDistance = 160;
            this.split.SplitterWidth = 1;
            this.split.TabIndex = 12;
            // 
            // flp
            // 
            this.flp.AutoScroll = true;
            this.flp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flp.Location = new System.Drawing.Point(0, 0);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(400, 160);
            this.flp.TabIndex = 10;
            this.flp.Layout += new System.Windows.Forms.LayoutEventHandler(this.flp_Layout);
            // 
            // ruleSetSyntaxItemPanel
            // 
            this.ruleSetSyntaxItemPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ruleSetSyntaxItemPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.ruleSetSyntaxItemPanel.Location = new System.Drawing.Point(0, 0);
            this.ruleSetSyntaxItemPanel.Name = "ruleSetSyntaxItemPanel";
            this.ruleSetSyntaxItemPanel.Size = new System.Drawing.Size(400, 163);
            this.ruleSetSyntaxItemPanel.TabIndex = 11;
            // 
            // Execute
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.split);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.flpTest);
            this.Name = "Execute";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.Size = new System.Drawing.Size(900, 324);
            this.flpTest.ResumeLayout(false);
            this.flpTest.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreads)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tpTvw.ResumeLayout(false);
            this.tpSelectedNode.ResumeLayout(false);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvw;
        private System.Windows.Forms.FlowLayoutPanel flpTest;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.NumericUpDown nudThreads;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboThread;
        private System.Windows.Forms.FlowLayoutPanel flp;
        private ConnectionProxyRuleSetSyntaxItemPanel ruleSetSyntaxItemPanel;
        private System.Windows.Forms.ProgressBar pb;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton btnPreviousError;
        private System.Windows.Forms.ToolStripButton btnSelectError;
        private System.Windows.Forms.ToolStripButton btnNextError;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tpTvw;
        private System.Windows.Forms.TabPage tpSelectedNode;
        private System.Windows.Forms.RichTextBox rtxtSelectedNode;
        private System.Windows.Forms.SplitContainer split;
    }
}
