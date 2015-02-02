namespace vApus.DistributedTesting.Controls.TestTreeView {
    partial class TileOverview {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileOverview));
            this.fctxt = new FastColoredTextBoxNS.FastColoredTextBox();
            this.flpConfiguration = new System.Windows.Forms.FlowLayoutPanel();
            this.lbtnOverview = new vApus.Util.LinkButton();
            this.lbtnOverride = new vApus.Util.LinkButton();
            this.btnApply = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.fctxt)).BeginInit();
            this.flpConfiguration.SuspendLayout();
            this.SuspendLayout();
            // 
            // fctxt
            // 
            this.fctxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fctxt.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.fctxt.BackBrush = null;
            this.fctxt.CharHeight = 14;
            this.fctxt.CharWidth = 8;
            this.fctxt.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctxt.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctxt.IsReplaceMode = false;
            this.fctxt.Location = new System.Drawing.Point(6, 118);
            this.fctxt.Name = "fctxt";
            this.fctxt.Paddings = new System.Windows.Forms.Padding(0);
            this.fctxt.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctxt.Size = new System.Drawing.Size(568, 232);
            this.fctxt.TabIndex = 6;
            this.fctxt.Zoom = 100;
            this.fctxt.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fctxt_TextChanged);
            // 
            // flpConfiguration
            // 
            this.flpConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpConfiguration.Controls.Add(this.lbtnOverview);
            this.flpConfiguration.Controls.Add(this.lbtnOverride);
            this.flpConfiguration.Location = new System.Drawing.Point(3, 81);
            this.flpConfiguration.Name = "flpConfiguration";
            this.flpConfiguration.Size = new System.Drawing.Size(571, 31);
            this.flpConfiguration.TabIndex = 49;
            // 
            // lbtnOverview
            // 
            this.lbtnOverview.Active = true;
            this.lbtnOverview.ActiveLinkColor = System.Drawing.Color.Black;
            this.lbtnOverview.AutoSize = true;
            this.lbtnOverview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbtnOverview.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnOverview.ForeColor = System.Drawing.Color.Black;
            this.lbtnOverview.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbtnOverview.LinkColor = System.Drawing.Color.Black;
            this.lbtnOverview.Location = new System.Drawing.Point(3, 6);
            this.lbtnOverview.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnOverview.Name = "lbtnOverview";
            this.lbtnOverview.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnOverview.RadioButtonBehavior = true;
            this.lbtnOverview.Size = new System.Drawing.Size(68, 22);
            this.lbtnOverview.TabIndex = 34;
            this.lbtnOverview.TabStop = true;
            this.lbtnOverview.Text = "Overview";
            this.lbtnOverview.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnOverview.VisitedLinkColor = System.Drawing.Color.Black;
            this.lbtnOverview.ActiveChanged += new System.EventHandler(this.lbtnOverview_ActiveChanged);
            // 
            // lbtnOverride
            // 
            this.lbtnOverride.Active = false;
            this.lbtnOverride.ActiveLinkColor = System.Drawing.Color.DimGray;
            this.lbtnOverride.AutoSize = true;
            this.lbtnOverride.BackColor = System.Drawing.Color.Transparent;
            this.lbtnOverride.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbtnOverride.ForeColor = System.Drawing.Color.DimGray;
            this.lbtnOverride.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lbtnOverride.LinkColor = System.Drawing.Color.DimGray;
            this.lbtnOverride.Location = new System.Drawing.Point(74, 6);
            this.lbtnOverride.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.lbtnOverride.Name = "lbtnOverride";
            this.lbtnOverride.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.lbtnOverride.RadioButtonBehavior = true;
            this.lbtnOverride.Size = new System.Drawing.Size(61, 20);
            this.lbtnOverride.TabIndex = 35;
            this.lbtnOverride.TabStop = true;
            this.lbtnOverride.Text = "Override";
            this.lbtnOverride.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbtnOverride.VisitedLinkColor = System.Drawing.Color.DimGray;
            this.lbtnOverride.ActiveChanged += new System.EventHandler(this.lbtnOverride_ActiveChanged);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.AutoSize = true;
            this.btnApply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnApply.BackColor = System.Drawing.Color.White;
            this.btnApply.Enabled = false;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.Location = new System.Drawing.Point(437, 356);
            this.btnApply.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnApply.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(137, 24);
            this.btnApply.TabIndex = 71;
            this.btnApply.Text = "Use selected / apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(593, 64);
            this.label2.TabIndex = 72;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // TileOverview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.flpConfiguration);
            this.Controls.Add(this.fctxt);
            this.Name = "TileOverview";
            this.Size = new System.Drawing.Size(584, 383);
            ((System.ComponentModel.ISupportInitialize)(this.fctxt)).EndInit();
            this.flpConfiguration.ResumeLayout(false);
            this.flpConfiguration.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox fctxt;
        private System.Windows.Forms.FlowLayoutPanel flpConfiguration;
        private Util.LinkButton lbtnOverview;
        private Util.LinkButton lbtnOverride;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label2;
    }
}
