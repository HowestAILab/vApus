namespace vApus.Stresstest
{
    partial class AddReferences
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
            this.lvwGac = new System.Windows.Forms.ListView();
            this.clmAssemblyName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.clmCulture = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmPublicKeyToken = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvwGac
            // 
            this.lvwGac.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwGac.CheckBoxes = true;
            this.lvwGac.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmAssemblyName,
            this.clmVersion,
            this.clmCulture,
            this.clmPublicKeyToken});
            this.lvwGac.FullRowSelect = true;
            this.lvwGac.Location = new System.Drawing.Point(12, 12);
            this.lvwGac.Name = "lvwGac";
            this.lvwGac.Size = new System.Drawing.Size(560, 509);
            this.lvwGac.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwGac.TabIndex = 0;
            this.lvwGac.UseCompatibleStateImageBehavior = false;
            this.lvwGac.View = System.Windows.Forms.View.Details;
            this.lvwGac.VisibleChanged += new System.EventHandler(this.lvwGac_VisibleChanged);
            // 
            // clmAssemblyName
            // 
            this.clmAssemblyName.Text = "Assembly Name";
            this.clmAssemblyName.Width = 87;
            // 
            // clmVersion
            // 
            this.clmVersion.Text = "Version";
            this.clmVersion.Width = 47;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(416, 527);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(497, 527);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // clmCulture
            // 
            this.clmCulture.Text = "Culture";
            this.clmCulture.Width = 45;
            // 
            // clmPublicKeyToken
            // 
            this.clmPublicKeyToken.Text = "Public Key Token";
            this.clmPublicKeyToken.Width = 96;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefresh.Location = new System.Drawing.Point(12, 527);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // AddReferences
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(584, 562);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lvwGac);
            this.MinimizeBox = false;
            this.Name = "AddReferences";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add References from Gac";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvwGac;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ColumnHeader clmAssemblyName;
        private System.Windows.Forms.ColumnHeader clmVersion;
        private System.Windows.Forms.ColumnHeader clmCulture;
        private System.Windows.Forms.ColumnHeader clmPublicKeyToken;
        private System.Windows.Forms.Button btnRefresh;
    }
}