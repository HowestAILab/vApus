namespace vApus.Util
{
    partial class LargeList
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
            this.tlp = new System.Windows.Forms.TableLayoutPanel();
            this.flpnl = new System.Windows.Forms.FlowLayoutPanel();
            this.pnl = new System.Windows.Forms.Panel();
            this.txtView = new System.Windows.Forms.TextBox();
            this.lblTotalViews = new System.Windows.Forms.Label();
            this.scrollbar = new System.Windows.Forms.HScrollBar();
            this.lblSlash = new System.Windows.Forms.Label();
            this.tlp.SuspendLayout();
            this.pnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp
            // 
            this.tlp.ColumnCount = 1;
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp.Controls.Add(this.flpnl, 0, 0);
            this.tlp.Controls.Add(this.pnl, 0, 1);
            this.tlp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp.Location = new System.Drawing.Point(0, 0);
            this.tlp.Name = "tlp";
            this.tlp.RowCount = 2;
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlp.Size = new System.Drawing.Size(150, 150);
            this.tlp.TabIndex = 0;
            // 
            // flpnl
            // 
            this.flpnl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpnl.AutoScroll = true;
            this.flpnl.BackColor = System.Drawing.Color.White;
            this.flpnl.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpnl.Location = new System.Drawing.Point(0, 0);
            this.flpnl.Margin = new System.Windows.Forms.Padding(0);
            this.flpnl.MinimumSize = new System.Drawing.Size(20, 20);
            this.flpnl.Name = "flpnl";
            this.flpnl.Size = new System.Drawing.Size(150, 133);
            this.flpnl.TabIndex = 0;
            this.flpnl.WrapContents = false;
            // 
            // pnl
            // 
            this.pnl.Controls.Add(this.txtView);
            this.pnl.Controls.Add(this.lblTotalViews);
            this.pnl.Controls.Add(this.scrollbar);
            this.pnl.Controls.Add(this.lblSlash);
            this.pnl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnl.Location = new System.Drawing.Point(0, 133);
            this.pnl.Margin = new System.Windows.Forms.Padding(0);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(150, 17);
            this.pnl.TabIndex = 4;
            // 
            // txtView
            // 
            this.txtView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtView.Location = new System.Drawing.Point(114, 1);
            this.txtView.Margin = new System.Windows.Forms.Padding(0);
            this.txtView.MaxLength = 10;
            this.txtView.Name = "txtView";
            this.txtView.Size = new System.Drawing.Size(14, 20);
            this.txtView.TabIndex = 4;
            this.txtView.Text = "1";
            this.txtView.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtView_KeyPress);
            // 
            // lblTotalViews
            // 
            this.lblTotalViews.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalViews.AutoSize = true;
            this.lblTotalViews.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalViews.Location = new System.Drawing.Point(136, 3);
            this.lblTotalViews.Margin = new System.Windows.Forms.Padding(0);
            this.lblTotalViews.Name = "lblTotalViews";
            this.lblTotalViews.Size = new System.Drawing.Size(13, 13);
            this.lblTotalViews.TabIndex = 6;
            this.lblTotalViews.Text = "1";
            this.lblTotalViews.TextChanged += new System.EventHandler(this.lblTotalViews_TextChanged);
            // 
            // scrollbar
            // 
            this.scrollbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollbar.LargeChange = 1;
            this.scrollbar.Location = new System.Drawing.Point(0, 0);
            this.scrollbar.Maximum = 0;
            this.scrollbar.Name = "scrollbar";
            this.scrollbar.Size = new System.Drawing.Size(114, 17);
            this.scrollbar.TabIndex = 1;
            this.scrollbar.ValueChanged += new System.EventHandler(this.scrollbar_ValueChanged);
            // 
            // lblSlash
            // 
            this.lblSlash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSlash.AutoSize = true;
            this.lblSlash.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSlash.Location = new System.Drawing.Point(128, 3);
            this.lblSlash.Margin = new System.Windows.Forms.Padding(0);
            this.lblSlash.Name = "lblSlash";
            this.lblSlash.Size = new System.Drawing.Size(12, 13);
            this.lblSlash.TabIndex = 5;
            this.lblSlash.Text = "/";
            // 
            // LargeList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlp);
            this.DoubleBuffered = true;
            this.Name = "LargeList";
            this.tlp.ResumeLayout(false);
            this.pnl.ResumeLayout(false);
            this.pnl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp;
        private System.Windows.Forms.HScrollBar scrollbar;
        private System.Windows.Forms.FlowLayoutPanel flpnl;
        private System.Windows.Forms.Panel pnl;
        private System.Windows.Forms.TextBox txtView;
        private System.Windows.Forms.Label lblTotalViews;
        private System.Windows.Forms.Label lblSlash;
    }
}
