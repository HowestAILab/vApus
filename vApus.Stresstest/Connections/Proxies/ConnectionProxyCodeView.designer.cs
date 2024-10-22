﻿using vApus.Util;
namespace vApus.StressTest
{
    partial class ConnectionProxyCodeView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionProxyCodeView));
            this.splitCode = new System.Windows.Forms.SplitContainer();
            this.codeTextBox = new vApus.Util.CodeTextBox();
            this.txtGoToLine = new vApus.Util.CueTextBox();
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.tcTools = new System.Windows.Forms.TabControl();
            this.tpReferences = new System.Windows.Forms.TabPage();
            this.referencesPanel = new vApus.StressTest.ReferencesPanel();
            this.tpFind = new System.Windows.Forms.TabPage();
            this.findAndReplacePanel = new vApus.StressTest.FindAndReplacePanel();
            this.tpCompile = new System.Windows.Forms.TabPage();
            this.compilePanel = new vApus.StressTest.CompilePanel();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitCode)).BeginInit();
            this.splitCode.Panel1.SuspendLayout();
            this.splitCode.Panel2.SuspendLayout();
            this.splitCode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.codeTextBox)).BeginInit();
            this.tcTools.SuspendLayout();
            this.tpReferences.SuspendLayout();
            this.tpFind.SuspendLayout();
            this.tpCompile.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitCode
            // 
            this.splitCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitCode.Location = new System.Drawing.Point(0, 0);
            this.splitCode.Name = "splitCode";
            this.splitCode.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitCode.Panel1
            // 
            this.splitCode.Panel1.BackColor = System.Drawing.Color.White;
            this.splitCode.Panel1.Controls.Add(this.codeTextBox);
            this.splitCode.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // splitCode.Panel2
            // 
            this.splitCode.Panel2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.splitCode.Panel2.Controls.Add(this.txtGoToLine);
            this.splitCode.Panel2.Controls.Add(this.btnCollapseExpand);
            this.splitCode.Panel2.Controls.Add(this.btnExport);
            this.splitCode.Panel2.Controls.Add(this.tcTools);
            this.splitCode.Size = new System.Drawing.Size(805, 541);
            this.splitCode.SplitterDistance = 350;
            this.splitCode.TabIndex = 4;
            // 
            // codeTextBox
            // 
            this.codeTextBox.AutoScrollMinSize = new System.Drawing.Size(0, 14);
            this.codeTextBox.BackBrush = null;
            this.codeTextBox.CharHeight = 14;
            this.codeTextBox.CharWidth = 8;
            this.codeTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.codeTextBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.codeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeTextBox.IsReplaceMode = false;
            this.codeTextBox.Location = new System.Drawing.Point(3, 3);
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.Paddings = new System.Windows.Forms.Padding(0);
            this.codeTextBox.PreferredLineWidth = 65536;
            this.codeTextBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.codeTextBox.Size = new System.Drawing.Size(799, 344);
            this.codeTextBox.TabIndex = 1;
            this.codeTextBox.WordWrap = false;
            this.codeTextBox.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.CharWrapControlWidth;
            this.codeTextBox.Zoom = 100;
            // 
            // txtGoToLine
            // 
            this.txtGoToLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGoToLine.Cue = "Go to line...";
            this.txtGoToLine.ForeColor = System.Drawing.Color.Black;
            this.txtGoToLine.Location = new System.Drawing.Point(529, 3);
            this.txtGoToLine.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtGoToLine.Name = "txtGoToLine";
            this.txtGoToLine.Size = new System.Drawing.Size(100, 20);
            this.txtGoToLine.TabIndex = 0;
            this.toolTip.SetToolTip(this.txtGoToLine, "Go to line...");
            this.txtGoToLine.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGoToLine_KeyDown);
            this.txtGoToLine.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGoToLine_KeyPress);
            this.txtGoToLine.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtGoToLine_KeyUp);
            // 
            // btnCollapseExpand
            // 
            this.btnCollapseExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseExpand.BackColor = System.Drawing.Color.White;
            this.btnCollapseExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseExpand.Location = new System.Drawing.Point(783, 1);
            this.btnCollapseExpand.Name = "btnCollapseExpand";
            this.btnCollapseExpand.Size = new System.Drawing.Size(20, 23);
            this.btnCollapseExpand.TabIndex = 6;
            this.btnCollapseExpand.TabStop = false;
            this.btnCollapseExpand.Text = "-";
            this.btnCollapseExpand.UseVisualStyleBackColor = false;
            this.btnCollapseExpand.Click += new System.EventHandler(this.btnCollapseExpand_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.Image = ((System.Drawing.Image)(resources.GetObject("btnExport.Image")));
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(635, 1);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(142, 23);
            this.btnExport.TabIndex = 1;
            this.btnExport.TabStop = false;
            this.btnExport.Text = "Export code to cs-file...";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // tcTools
            // 
            this.tcTools.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcTools.Controls.Add(this.tpReferences);
            this.tcTools.Controls.Add(this.tpFind);
            this.tcTools.Controls.Add(this.tpCompile);
            this.tcTools.Location = new System.Drawing.Point(4, 4);
            this.tcTools.Name = "tcTools";
            this.tcTools.SelectedIndex = 0;
            this.tcTools.Size = new System.Drawing.Size(797, 179);
            this.tcTools.TabIndex = 3;
            // 
            // tpReferences
            // 
            this.tpReferences.Controls.Add(this.referencesPanel);
            this.tpReferences.Location = new System.Drawing.Point(4, 22);
            this.tpReferences.Name = "tpReferences";
            this.tpReferences.Padding = new System.Windows.Forms.Padding(3);
            this.tpReferences.Size = new System.Drawing.Size(789, 153);
            this.tpReferences.TabIndex = 0;
            this.tpReferences.Text = "References";
            this.tpReferences.UseVisualStyleBackColor = true;
            // 
            // references
            // 
            this.referencesPanel.CodeTextBox = null;
            this.referencesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.referencesPanel.Location = new System.Drawing.Point(3, 3);
            this.referencesPanel.Name = "references";
            this.referencesPanel.Padding = new System.Windows.Forms.Padding(9);
            this.referencesPanel.Size = new System.Drawing.Size(783, 147);
            this.referencesPanel.TabIndex = 0;
            // 
            // tpFind
            // 
            this.tpFind.Controls.Add(this.findAndReplacePanel);
            this.tpFind.Location = new System.Drawing.Point(4, 22);
            this.tpFind.Name = "tpFind";
            this.tpFind.Padding = new System.Windows.Forms.Padding(3);
            this.tpFind.Size = new System.Drawing.Size(789, 153);
            this.tpFind.TabIndex = 5;
            this.tpFind.Text = "Find and replace";
            this.tpFind.UseVisualStyleBackColor = true;
            // 
            // find
            // 
            this.findAndReplacePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.findAndReplacePanel.Location = new System.Drawing.Point(3, 3);
            this.findAndReplacePanel.Name = "find";
            this.findAndReplacePanel.Padding = new System.Windows.Forms.Padding(9);
            this.findAndReplacePanel.Size = new System.Drawing.Size(783, 147);
            this.findAndReplacePanel.TabIndex = 0;
            this.findAndReplacePanel.FoundReplacedButtonClicked += new System.EventHandler<vApus.StressTest.FindAndReplacePanel.FoundReplacedButtonClickedEventArgs>(this.find_FoundButtonClicked);
            // 
            // tpCompile
            // 
            this.tpCompile.Controls.Add(this.compilePanel);
            this.tpCompile.Location = new System.Drawing.Point(4, 22);
            this.tpCompile.Name = "tpCompile";
            this.tpCompile.Padding = new System.Windows.Forms.Padding(3);
            this.tpCompile.Size = new System.Drawing.Size(789, 153);
            this.tpCompile.TabIndex = 1;
            this.tpCompile.Text = "Compile";
            this.tpCompile.UseVisualStyleBackColor = true;
            // 
            // compile
            // 
            this.compilePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compilePanel.Location = new System.Drawing.Point(3, 3);
            this.compilePanel.Name = "compile";
            this.compilePanel.Padding = new System.Windows.Forms.Padding(9);
            this.compilePanel.Size = new System.Drawing.Size(783, 147);
            this.compilePanel.TabIndex = 0;
            this.compilePanel.CompileError += new System.EventHandler(this.compile_CompileError);
            this.compilePanel.CompileErrorButtonClicked += new System.EventHandler<vApus.StressTest.CompilePanel.CompileErrorButtonClickedEventArgs>(this.compile_CompileErrorButtonClicked);
            // 
            // sfd
            // 
            this.sfd.Filter = "cs-file|*.cs";
            // 
            // ConnectionProxyCodeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 541);
            this.Controls.Add(this.splitCode);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "ConnectionProxyCodeView";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ConnectionProxyCodeView_KeyPress);
            this.splitCode.Panel1.ResumeLayout(false);
            this.splitCode.Panel2.ResumeLayout(false);
            this.splitCode.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitCode)).EndInit();
            this.splitCode.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.codeTextBox)).EndInit();
            this.tcTools.ResumeLayout(false);
            this.tpReferences.ResumeLayout(false);
            this.tpFind.ResumeLayout(false);
            this.tpCompile.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitCode;
        private System.Windows.Forms.TabControl tcTools;
        private System.Windows.Forms.TabPage tpReferences;
        private System.Windows.Forms.TabPage tpCompile;
        private ReferencesPanel referencesPanel;
        private CompilePanel compilePanel;
        private System.Windows.Forms.TabPage tpFind;
        private FindAndReplacePanel findAndReplacePanel;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.Button btnCollapseExpand;
        private CodeTextBox codeTextBox;
        private CueTextBox txtGoToLine;
        private System.Windows.Forms.ToolTip toolTip;
    }
}

