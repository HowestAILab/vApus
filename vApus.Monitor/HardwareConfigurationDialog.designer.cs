namespace vApus.Monitor {
    partial class HardwareConfigurationDialog {
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
            this.table = new System.Windows.Forms.TableLayoutPanel();
            this.sc = new System.Windows.Forms.SplitContainer();
            this.tv = new System.Windows.Forms.TreeView();
            this.rtxt = new System.Windows.Forms.RichTextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.table.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sc)).BeginInit();
            this.sc.Panel1.SuspendLayout();
            this.sc.Panel2.SuspendLayout();
            this.sc.SuspendLayout();
            this.SuspendLayout();
            // 
            // table
            // 
            this.table.ColumnCount = 1;
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table.Controls.Add(this.sc, 0, 0);
            this.table.Controls.Add(this.btnSave, 0, 1);
            this.table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table.Location = new System.Drawing.Point(0, 0);
            this.table.Name = "table";
            this.table.RowCount = 2;
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.table.Size = new System.Drawing.Size(592, 466);
            this.table.TabIndex = 0;
            // 
            // sc
            // 
            this.sc.BackColor = System.Drawing.SystemColors.Control;
            this.sc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sc.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.sc.Location = new System.Drawing.Point(3, 3);
            this.sc.Name = "sc";
            // 
            // sc.Panel1
            // 
            this.sc.Panel1.Controls.Add(this.tv);
            this.sc.Panel1MinSize = 150;
            // 
            // sc.Panel2
            // 
            this.sc.Panel2.Controls.Add(this.rtxt);
            this.sc.Size = new System.Drawing.Size(586, 431);
            this.sc.SplitterDistance = 150;
            this.sc.TabIndex = 1;
            // 
            // tv
            // 
            this.tv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tv.Location = new System.Drawing.Point(0, 0);
            this.tv.Name = "tv";
            this.tv.Size = new System.Drawing.Size(150, 431);
            this.tv.TabIndex = 0;
            this.tv.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tv_AfterSelect);
            // 
            // rtxt
            // 
            this.rtxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxt.Location = new System.Drawing.Point(0, 0);
            this.rtxt.Name = "rtxt";
            this.rtxt.Size = new System.Drawing.Size(432, 431);
            this.rtxt.TabIndex = 0;
            this.rtxt.Text = "";
            this.rtxt.WordWrap = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.Control;
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(3, 440);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(586, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // sfd
            // 
            this.sfd.Filter = "xml files(*.xml) |*.xml";
            this.sfd.RestoreDirectory = true;
            this.sfd.ShowHelp = true;
            // 
            // ConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(592, 466);
            this.Controls.Add(this.table);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 500);
            this.Name = "ConfigurationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.table.ResumeLayout(false);
            this.sc.Panel1.ResumeLayout(false);
            this.sc.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sc)).EndInit();
            this.sc.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel table;
        private System.Windows.Forms.SplitContainer sc;
        private System.Windows.Forms.TreeView tv;
        private System.Windows.Forms.RichTextBox rtxt;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog sfd;

    }
}