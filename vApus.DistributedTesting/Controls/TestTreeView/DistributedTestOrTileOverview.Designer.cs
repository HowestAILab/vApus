namespace vApus.DistributedTesting.Controls.TestTreeView {
    partial class DistributedTestOrTileOverview {
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
            this.label2 = new System.Windows.Forms.Label();
            this.tlvw = new BrightIdeasSoftware.TreeListView();
            this.clmName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clmConnectionString = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.chkShowConnectionStrings = new System.Windows.Forms.CheckBox();
            this.chkShowOnlyChecked = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tlvw)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(373, 16);
            this.label2.TabIndex = 72;
            this.label2.Text = "A compact overview of the connections and monitors:";
            // 
            // tlvw
            // 
            this.tlvw.AllColumns.Add(this.clmName);
            this.tlvw.AllColumns.Add(this.clmConnectionString);
            this.tlvw.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tlvw.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlvw.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tlvw.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmName,
            this.clmConnectionString});
            this.tlvw.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tlvw.FullRowSelect = true;
            this.tlvw.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.tlvw.HideSelection = false;
            this.tlvw.Location = new System.Drawing.Point(3, 52);
            this.tlvw.Name = "tlvw";
            this.tlvw.OwnerDraw = true;
            this.tlvw.ShowGroups = false;
            this.tlvw.Size = new System.Drawing.Size(858, 331);
            this.tlvw.TabIndex = 2;
            this.tlvw.UseAlternatingBackColors = true;
            this.tlvw.UseCompatibleStateImageBehavior = false;
            this.tlvw.View = System.Windows.Forms.View.Details;
            this.tlvw.VirtualMode = true;
            this.tlvw.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.tlvw_FormatRow);
            // 
            // clmName
            // 
            this.clmName.AspectName = "Name";
            this.clmName.IsEditable = false;
            this.clmName.Sortable = false;
            this.clmName.Text = "Name";
            this.clmName.Width = 200;
            // 
            // clmConnectionString
            // 
            this.clmConnectionString.AspectName = "ConnectionString";
            this.clmConnectionString.IsEditable = false;
            this.clmConnectionString.Sortable = false;
            this.clmConnectionString.Text = "Connection string";
            // 
            // chkShowConnectionStrings
            // 
            this.chkShowConnectionStrings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShowConnectionStrings.AutoSize = true;
            this.chkShowConnectionStrings.Location = new System.Drawing.Point(722, 5);
            this.chkShowConnectionStrings.Name = "chkShowConnectionStrings";
            this.chkShowConnectionStrings.Size = new System.Drawing.Size(142, 17);
            this.chkShowConnectionStrings.TabIndex = 1;
            this.chkShowConnectionStrings.Text = "Show connection strings";
            this.chkShowConnectionStrings.UseVisualStyleBackColor = true;
            this.chkShowConnectionStrings.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // chkShowOnlyChecked
            // 
            this.chkShowOnlyChecked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShowOnlyChecked.AutoSize = true;
            this.chkShowOnlyChecked.Checked = true;
            this.chkShowOnlyChecked.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowOnlyChecked.Location = new System.Drawing.Point(596, 5);
            this.chkShowOnlyChecked.Name = "chkShowOnlyChecked";
            this.chkShowOnlyChecked.Size = new System.Drawing.Size(120, 17);
            this.chkShowOnlyChecked.TabIndex = 0;
            this.chkShowOnlyChecked.Text = "Show only checked";
            this.chkShowOnlyChecked.UseVisualStyleBackColor = true;
            this.chkShowOnlyChecked.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(345, 13);
            this.label1.TabIndex = 73;
            this.label1.Text = "Duplicate connection strings and monitor parameters are marked yellow.";
            // 
            // DistributedTestOrTileOverview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkShowOnlyChecked);
            this.Controls.Add(this.chkShowConnectionStrings);
            this.Controls.Add(this.tlvw);
            this.Controls.Add(this.label2);
            this.Name = "DistributedTestOrTileOverview";
            this.Size = new System.Drawing.Size(864, 383);
            ((System.ComponentModel.ISupportInitialize)(this.tlvw)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private BrightIdeasSoftware.TreeListView tlvw;
        private BrightIdeasSoftware.OLVColumn clmName;
        private BrightIdeasSoftware.OLVColumn clmConnectionString;
        private System.Windows.Forms.CheckBox chkShowConnectionStrings;
        private System.Windows.Forms.CheckBox chkShowOnlyChecked;
        private System.Windows.Forms.Label label1;
    }
}
