namespace vApus.Stresstest {
    partial class NewLogView {
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
            this.split = new System.Windows.Forms.SplitContainer();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.picFind = new System.Windows.Forms.PictureBox();
            this.llblFindAndReplace = new System.Windows.Forms.LinkLabel();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.logTreeView = new vApus.Stresstest.LogTreeView();
            this.editLog = new vApus.Stresstest.EditLog();
            this.editUserAction = new vApus.Stresstest.EditUserAction();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tmrRefreshGui = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFind)).BeginInit();
            this.SuspendLayout();
            // 
            // split
            // 
            this.split.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.Location = new System.Drawing.Point(0, 0);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.BackColor = System.Drawing.Color.White;
            this.split.Panel1.Controls.Add(this.pnlFilter);
            this.split.Panel1.Controls.Add(this.logTreeView);
            // 
            // split.Panel2
            // 
            this.split.Panel2.BackColor = System.Drawing.Color.White;
            this.split.Panel2.Controls.Add(this.editLog);
            this.split.Panel2.Controls.Add(this.editUserAction);
            this.split.Size = new System.Drawing.Size(1046, 595);
            this.split.SplitterDistance = 348;
            this.split.TabIndex = 0;
            // 
            // pnlFilter
            // 
            this.pnlFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFilter.Controls.Add(this.picFind);
            this.pnlFilter.Controls.Add(this.llblFindAndReplace);
            this.pnlFilter.Controls.Add(this.txtFind);
            this.pnlFilter.Location = new System.Drawing.Point(0, 568);
            this.pnlFilter.MinimumSize = new System.Drawing.Size(227, 21);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(348, 21);
            this.pnlFilter.TabIndex = 2;
            // 
            // picFind
            // 
            this.picFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picFind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picFind.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picFind.Image = global::vApus.Stresstest.Properties.Resources.find;
            this.picFind.Location = new System.Drawing.Point(307, 1);
            this.picFind.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.picFind.Name = "picFind";
            this.picFind.Size = new System.Drawing.Size(20, 20);
            this.picFind.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFind.TabIndex = 8;
            this.picFind.TabStop = false;
            this.toolTip.SetToolTip(this.picFind, "Find Next.");
            this.picFind.Click += new System.EventHandler(this.picFind_Click);
            // 
            // llblFindAndReplace
            // 
            this.llblFindAndReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llblFindAndReplace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.llblFindAndReplace.DisabledLinkColor = System.Drawing.Color.Black;
            this.llblFindAndReplace.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.llblFindAndReplace.LinkColor = System.Drawing.Color.Black;
            this.llblFindAndReplace.Location = new System.Drawing.Point(328, 1);
            this.llblFindAndReplace.Name = "llblFindAndReplace";
            this.llblFindAndReplace.Size = new System.Drawing.Size(20, 20);
            this.llblFindAndReplace.TabIndex = 9;
            this.llblFindAndReplace.TabStop = true;
            this.llblFindAndReplace.Text = "...";
            this.llblFindAndReplace.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip.SetToolTip(this.llblFindAndReplace, "Find and Replace...");
            this.llblFindAndReplace.VisitedLinkColor = System.Drawing.Color.Black;
            this.llblFindAndReplace.Click += new System.EventHandler(this.llblFindAndReplace_Click);
            // 
            // txtFind
            // 
            this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFind.HideSelection = false;
            this.txtFind.Location = new System.Drawing.Point(3, 1);
            this.txtFind.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.txtFind.MinimumSize = new System.Drawing.Size(100, 4);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(304, 20);
            this.txtFind.TabIndex = 0;
            this.txtFind.TabStop = false;
            this.toolTip.SetToolTip(this.txtFind, "Wild cards * + - \"\" can be used. Not case sensitive.");
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFind_KeyDown);
            // 
            // logTreeView
            // 
            this.logTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTreeView.Location = new System.Drawing.Point(0, 3);
            this.logTreeView.Name = "logTreeView";
            this.logTreeView.Size = new System.Drawing.Size(348, 560);
            this.logTreeView.TabIndex = 0;
            this.logTreeView.AfterSelect += new System.EventHandler(this.logTreeView_AfterSelect);
            // 
            // editLog
            // 
            this.editLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editLog.Location = new System.Drawing.Point(0, 0);
            this.editLog.Name = "editLog";
            this.editLog.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.editLog.Size = new System.Drawing.Size(694, 595);
            this.editLog.TabIndex = 0;
            this.editLog.LogImported += new System.EventHandler(this.editLog_LogImported);
            this.editLog.RedeterminedTokens += new System.EventHandler(this.editLog_RedeterminedTokens);
            // 
            // editUserAction
            // 
            this.editUserAction.BackColor = System.Drawing.Color.White;
            this.editUserAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editUserAction.Location = new System.Drawing.Point(0, 0);
            this.editUserAction.Name = "editUserAction";
            this.editUserAction.Size = new System.Drawing.Size(694, 595);
            this.editUserAction.TabIndex = 1;
            this.editUserAction.Visible = false;
            this.editUserAction.UserActionMoved += new System.EventHandler(this.editUserAction_UserActionMoved);
            this.editUserAction.SplitClicked += new System.EventHandler(this.editUserAction_SplitClicked);
            this.editUserAction.MergeClicked += new System.EventHandler(this.editUserAction_MergeClicked);
            this.editUserAction.LinkedChanged += new System.EventHandler(this.editUserAction_LinkedChanged);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 20;
            // 
            // tmrRefreshGui
            // 
            this.tmrRefreshGui.Enabled = true;
            this.tmrRefreshGui.Tick += new System.EventHandler(this.tmrRefreshGui_Tick);
            // 
            // NewLogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1046, 595);
            this.Controls.Add(this.split);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "NewLogView";
            this.Text = "NewLogView";
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFind)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer split;
        private LogTreeView logTreeView;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.PictureBox picFind;
        private System.Windows.Forms.TextBox txtFind;
        private EditLog editLog;
        private EditUserAction editUserAction;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer tmrRefreshGui;
        private System.Windows.Forms.LinkLabel llblFindAndReplace;
    }
}