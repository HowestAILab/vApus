namespace vApus.Util
{
    partial class TracertDialog
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
            this.lvw = new System.Windows.Forms.ListView();
            this.clmHop = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmIP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmHostName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmRoundtripTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancelTraceRoute = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvw
            // 
            this.lvw.AllowColumnReorder = true;
            this.lvw.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvw.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmHop,
            this.clmIP,
            this.clmHostName,
            this.clmRoundtripTime});
            this.lvw.FullRowSelect = true;
            this.lvw.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvw.HideSelection = false;
            this.lvw.Location = new System.Drawing.Point(0, 0);
            this.lvw.Name = "lvw";
            this.lvw.Size = new System.Drawing.Size(420, 377);
            this.lvw.TabIndex = 0;
            this.lvw.UseCompatibleStateImageBehavior = false;
            this.lvw.View = System.Windows.Forms.View.Details;
            // 
            // clmHop
            // 
            this.clmHop.Text = "";
            this.clmHop.Width = 37;
            // 
            // clmIP
            // 
            this.clmIP.Text = "IP";
            this.clmIP.Width = 119;
            // 
            // clmHostName
            // 
            this.clmHostName.Text = "Host Name";
            this.clmHostName.Width = 141;
            // 
            // clmRoundtripTime
            // 
            this.clmRoundtripTime.Text = "Roundtrip Time";
            this.clmRoundtripTime.Width = 87;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.Black;
            this.btnClose.Location = new System.Drawing.Point(134, 376);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnClose.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnClose.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(286, 24);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // btnCancelTraceRoute
            // 
            this.btnCancelTraceRoute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelTraceRoute.AutoSize = true;
            this.btnCancelTraceRoute.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancelTraceRoute.BackColor = System.Drawing.Color.White;
            this.btnCancelTraceRoute.Enabled = false;
            this.btnCancelTraceRoute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelTraceRoute.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelTraceRoute.Location = new System.Drawing.Point(0, 376);
            this.btnCancelTraceRoute.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
            this.btnCancelTraceRoute.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnCancelTraceRoute.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnCancelTraceRoute.Name = "btnCancelTraceRoute";
            this.btnCancelTraceRoute.Size = new System.Drawing.Size(133, 24);
            this.btnCancelTraceRoute.TabIndex = 3;
            this.btnCancelTraceRoute.Text = "Cancel Trace Route";
            this.btnCancelTraceRoute.UseVisualStyleBackColor = false;
            this.btnCancelTraceRoute.Click += new System.EventHandler(this.btnCancelTraceRoute_Click);
            // 
            // TracertDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(420, 400);
            this.Controls.Add(this.btnCancelTraceRoute);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lvw);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(420, 400);
            this.Name = "TracertDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Trace Route";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvw;
        private System.Windows.Forms.ColumnHeader clmIP;
        private System.Windows.Forms.ColumnHeader clmHop;
        private System.Windows.Forms.ColumnHeader clmHostName;
        private System.Windows.Forms.ColumnHeader clmRoundtripTime;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancelTraceRoute;
    }
}