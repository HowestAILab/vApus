using vApus.Util;
namespace vApus.Stresstest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionProxyCodeView));
            this.splitCode = new System.Windows.Forms.SplitContainer();
            this.scrollablePanel = new vApus.Util.ScrollablePanel();
            this.document = new vApus.Stresstest.CodeBlock();
            this.btnFoldUnfold = new System.Windows.Forms.Button();
            this.btnCollapseExpand = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.tcTools = new System.Windows.Forms.TabControl();
            this.tpReferences = new System.Windows.Forms.TabPage();
            this.references = new vApus.Stresstest.References();
            this.tpFind = new System.Windows.Forms.TabPage();
            this.find = new vApus.Stresstest.FindAndReplace();
            this.tpCompile = new System.Windows.Forms.TabPage();
            this.compile = new vApus.Stresstest.Compile();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitCode)).BeginInit();
            this.splitCode.Panel1.SuspendLayout();
            this.splitCode.Panel2.SuspendLayout();
            this.splitCode.SuspendLayout();
            this.scrollablePanel.SuspendLayout();
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
            this.splitCode.Panel1.Controls.Add(this.scrollablePanel);
            this.splitCode.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // splitCode.Panel2
            // 
            this.splitCode.Panel2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.splitCode.Panel2.Controls.Add(this.btnFoldUnfold);
            this.splitCode.Panel2.Controls.Add(this.btnCollapseExpand);
            this.splitCode.Panel2.Controls.Add(this.btnExport);
            this.splitCode.Panel2.Controls.Add(this.tcTools);
            this.splitCode.Size = new System.Drawing.Size(805, 541);
            this.splitCode.SplitterDistance = 350;
            this.splitCode.TabIndex = 4;
            // 
            // scrollablePanel
            // 
            this.scrollablePanel.AutoScroll = true;
            this.scrollablePanel.Controls.Add(this.document);
            this.scrollablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollablePanel.Location = new System.Drawing.Point(3, 3);
            this.scrollablePanel.Name = "scrollablePanel";
            this.scrollablePanel.Size = new System.Drawing.Size(799, 344);
            this.scrollablePanel.TabIndex = 0;
            this.scrollablePanel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollablePanel_Scroll);
            // 
            // document
            // 
            this.document.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.document.BackColor = System.Drawing.Color.White;
            this.document.CanCollapse = false;
            this.document.Collapsed = false;
            this.document.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.document.Footer = "";
            this.document.FooterVisible = false;
            this.document.Header = "//Connection Proxy Document";
            this.document.HeaderVisible = false;
            this.document.LineNumberOffset = 1;
            this.document.Location = new System.Drawing.Point(0, 0);
            this.document.Margin = new System.Windows.Forms.Padding(0);
            this.document.Name = "document";
            this.document.ParentLevelControl = true;
            this.document.ReadOnly = false;
            this.document.ShowLineNumbers = false;
            this.document.Size = new System.Drawing.Size(787, 76);
            this.document.TabIndex = 0;
            this.document.SizeChanged += new System.EventHandler(this.document_SizeChanged);
            // 
            // btnFoldUnfold
            // 
            this.btnFoldUnfold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFoldUnfold.BackColor = System.Drawing.Color.White;
            this.btnFoldUnfold.FlatAppearance.BorderSize = 0;
            this.btnFoldUnfold.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFoldUnfold.Image = ((System.Drawing.Image)(resources.GetObject("btnFoldUnfold.Image")));
            this.btnFoldUnfold.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFoldUnfold.Location = new System.Drawing.Point(628, 1);
            this.btnFoldUnfold.Name = "btnFoldUnfold";
            this.btnFoldUnfold.Size = new System.Drawing.Size(152, 23);
            this.btnFoldUnfold.TabIndex = 7;
            this.btnFoldUnfold.TabStop = false;
            this.btnFoldUnfold.Text = "Fold Code Text Blocks";
            this.btnFoldUnfold.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFoldUnfold.UseVisualStyleBackColor = false;
            this.btnFoldUnfold.Click += new System.EventHandler(this.btnFoldUnfold_Click);
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
            this.btnExport.Location = new System.Drawing.Point(483, 1);
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
            this.tpReferences.Controls.Add(this.references);
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
            this.references.Dock = System.Windows.Forms.DockStyle.Fill;
            this.references.Location = new System.Drawing.Point(3, 3);
            this.references.Name = "references";
            this.references.Padding = new System.Windows.Forms.Padding(9);
            this.references.Size = new System.Drawing.Size(783, 147);
            this.references.TabIndex = 0;
            this.references.ReferencesChanged += new System.EventHandler(this.references_ReferencesChanged);
            // 
            // tpFind
            // 
            this.tpFind.Controls.Add(this.find);
            this.tpFind.Location = new System.Drawing.Point(4, 22);
            this.tpFind.Name = "tpFind";
            this.tpFind.Padding = new System.Windows.Forms.Padding(3);
            this.tpFind.Size = new System.Drawing.Size(789, 153);
            this.tpFind.TabIndex = 5;
            this.tpFind.Text = "Find and Replace";
            this.tpFind.UseVisualStyleBackColor = true;
            // 
            // find
            // 
            this.find.Dock = System.Windows.Forms.DockStyle.Fill;
            this.find.Location = new System.Drawing.Point(3, 3);
            this.find.Name = "find";
            this.find.Padding = new System.Windows.Forms.Padding(9);
            this.find.Size = new System.Drawing.Size(783, 147);
            this.find.TabIndex = 0;
            this.find.FoundReplacedButtonClicked += new System.EventHandler<vApus.Stresstest.FindAndReplace.FoundReplacedButtonClickedEventArgs>(this.find_FoundButtonClicked);
            // 
            // tpCompile
            // 
            this.tpCompile.Controls.Add(this.compile);
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
            this.compile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compile.Location = new System.Drawing.Point(3, 3);
            this.compile.Name = "compile";
            this.compile.Padding = new System.Windows.Forms.Padding(9);
            this.compile.Size = new System.Drawing.Size(783, 147);
            this.compile.TabIndex = 0;
            this.compile.CompileError += new System.EventHandler(this.compile_CompileError);
            this.compile.CompileErrorButtonClicked += new System.EventHandler<vApus.Stresstest.Compile.CompileErrorButtonClickedEventArgs>(this.compile_CompileErrorButtonClicked);
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
            ((System.ComponentModel.ISupportInitialize)(this.splitCode)).EndInit();
            this.splitCode.ResumeLayout(false);
            this.scrollablePanel.ResumeLayout(false);
            this.tcTools.ResumeLayout(false);
            this.tpReferences.ResumeLayout(false);
            this.tpFind.ResumeLayout(false);
            this.tpCompile.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CodeBlock document;
        private System.Windows.Forms.SplitContainer splitCode;
        private System.Windows.Forms.TabControl tcTools;
        private System.Windows.Forms.TabPage tpReferences;
        private System.Windows.Forms.TabPage tpCompile;
        private References references;
        private Compile compile;
        private System.Windows.Forms.TabPage tpFind;
        private FindAndReplace find;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.SaveFileDialog sfd;
        private vApus.Util.ScrollablePanel scrollablePanel;
        private System.Windows.Forms.Button btnCollapseExpand;
        private System.Windows.Forms.Button btnFoldUnfold;
    }
}

