﻿namespace vApus.StressTest {
    partial class ScenarioView {
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
            this.scenarioTreeView = new vApus.StressTest.ScenarioTreeView();
            this.editScenarioPanel = new vApus.StressTest.EditScenarioPanel();
            this.editUserActionPanel = new vApus.StressTest.EditUserActionPanel();
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
            this.split.Panel1.Controls.Add(this.scenarioTreeView);
            // 
            // split.Panel2
            // 
            this.split.Panel2.BackColor = System.Drawing.Color.White;
            this.split.Panel2.Controls.Add(this.editScenarioPanel);
            this.split.Panel2.Controls.Add(this.editUserActionPanel);
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
            this.picFind.Enabled = false;
            this.picFind.Image = global::vApus.StressTest.Properties.Resources.find;
            this.picFind.Location = new System.Drawing.Point(307, 1);
            this.picFind.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.picFind.Name = "picFind";
            this.picFind.Size = new System.Drawing.Size(20, 20);
            this.picFind.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFind.TabIndex = 8;
            this.picFind.TabStop = false;
            this.toolTip.SetToolTip(this.picFind, "Find next.");
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
            this.toolTip.SetToolTip(this.llblFindAndReplace, "Find and replace...");
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
            // scenarioTreeView
            // 
            this.scenarioTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scenarioTreeView.Location = new System.Drawing.Point(0, 3);
            this.scenarioTreeView.Name = "scenarioTreeView";
            this.scenarioTreeView.Size = new System.Drawing.Size(348, 560);
            this.scenarioTreeView.TabIndex = 0;
            this.scenarioTreeView.AfterSelect += new System.EventHandler(this.scenarioTreeView_AfterSelect);
            // 
            // editScenarioPanel
            // 
            this.editScenarioPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editScenarioPanel.Location = new System.Drawing.Point(0, 0);
            this.editScenarioPanel.Name = "editScenarioPanel";
            this.editScenarioPanel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.editScenarioPanel.Size = new System.Drawing.Size(694, 595);
            this.editScenarioPanel.TabIndex = 0;
            this.editScenarioPanel.ScenarioImported += new System.EventHandler(this.editScenario_ScenarioImported);
            this.editScenarioPanel.RevertedToAsImported += new System.EventHandler(this.editScenario_RevertedToAsImported);
            this.editScenarioPanel.RedeterminedTokens += new System.EventHandler(this.editScenario_RedeterminedTokens);
            // 
            // editUserActionPanel
            // 
            this.editUserActionPanel.BackColor = System.Drawing.Color.White;
            this.editUserActionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editUserActionPanel.Location = new System.Drawing.Point(0, 0);
            this.editUserActionPanel.Name = "editUserActionPanel";
            this.editUserActionPanel.Size = new System.Drawing.Size(694, 595);
            this.editUserActionPanel.TabIndex = 1;
            this.editUserActionPanel.Visible = false;
            this.editUserActionPanel.UserActionMoved += new System.EventHandler(this.editUserAction_UserActionMoved);
            this.editUserActionPanel.SplitClicked += new System.EventHandler(this.editUserAction_SplitClicked);
            this.editUserActionPanel.MergeClicked += new System.EventHandler(this.editUserAction_MergeClicked);
            this.editUserActionPanel.LinkedChanged += new System.EventHandler(this.editUserAction_LinkedChanged);
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
            // ScenarioView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1046, 595);
            this.Controls.Add(this.split);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ScenarioView";
            this.Text = "ScenarioView";
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
        private ScenarioTreeView scenarioTreeView;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.PictureBox picFind;
        private System.Windows.Forms.TextBox txtFind;
        private EditScenarioPanel editScenarioPanel;
        private EditUserActionPanel editUserActionPanel;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer tmrRefreshGui;
        private System.Windows.Forms.LinkLabel llblFindAndReplace;
    }
}
