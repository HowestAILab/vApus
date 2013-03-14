namespace vApus.Util
{
    partial class LogPanel
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
            this.cboLogLevel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.llblPath = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnWarning = new System.Windows.Forms.Button();
            this.llblLatestLog = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // cboLogLevel
            // 
            this.cboLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLogLevel.BackColor = System.Drawing.Color.White;
            this.cboLogLevel.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLogLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboLogLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboLogLevel.FormattingEnabled = true;
            this.cboLogLevel.Items.AddRange(new object[] {
            "Info",
            "Warning",
            "Error",
            "Fatal"});
            this.cboLogLevel.Location = new System.Drawing.Point(80, 13);
            this.cboLogLevel.Name = "cboLogLevel";
            this.cboLogLevel.Size = new System.Drawing.Size(313, 21);
            this.cboLogLevel.TabIndex = 0;
            this.toolTip.SetToolTip(this.cboLogLevel, "Selecting another item will change the log level immediately.");
            this.cboLogLevel.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cboLogLevel_DrawItem);
            this.cboLogLevel.SelectedIndexChanged += new System.EventHandler(this.cboLogLevel_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Log Level:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Path:";
            // 
            // llblPath
            // 
            this.llblPath.AutoSize = true;
            this.llblPath.Location = new System.Drawing.Point(77, 47);
            this.llblPath.Name = "llblPath";
            this.llblPath.Size = new System.Drawing.Size(16, 13);
            this.llblPath.TabIndex = 2;
            this.llblPath.TabStop = true;
            this.llblPath.Text = "...";
            this.llblPath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblPath_LinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dgv);
            this.groupBox1.Controls.Add(this.btnWarning);
            this.groupBox1.Controls.Add(this.llblLatestLog);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboLogLevel);
            this.groupBox1.Controls.Add(this.llblPath);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(428, 312);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.ColumnHeadersVisible = false;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.dgv.Location = new System.Drawing.Point(6, 85);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgv.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(416, 221);
            this.dgv.TabIndex = 7;
            this.dgv.VirtualMode = true;
            this.dgv.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellDoubleClick);
            this.dgv.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgv_CellFormatting);
            // 
            // btnWarning
            // 
            this.btnWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWarning.AutoSize = true;
            this.btnWarning.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnWarning.BackColor = System.Drawing.Color.Yellow;
            this.btnWarning.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnWarning.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWarning.Location = new System.Drawing.Point(399, 11);
            this.btnWarning.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.btnWarning.MaximumSize = new System.Drawing.Size(176, 24);
            this.btnWarning.Name = "btnWarning";
            this.btnWarning.Size = new System.Drawing.Size(23, 24);
            this.btnWarning.TabIndex = 1;
            this.btnWarning.Text = "!";
            this.btnWarning.UseVisualStyleBackColor = false;
            this.btnWarning.Click += new System.EventHandler(this.btnWarning_Click);
            // 
            // llblLatestLog
            // 
            this.llblLatestLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.llblLatestLog.AutoSize = true;
            this.llblLatestLog.Location = new System.Drawing.Point(77, 69);
            this.llblLatestLog.Name = "llblLatestLog";
            this.llblLatestLog.Size = new System.Drawing.Size(19, 13);
            this.llblLatestLog.TabIndex = 3;
            this.llblLatestLog.TabStop = true;
            this.llblLatestLog.Text = "<>";
            this.llblLatestLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblLatestLog_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Current Log:";
            // 
            // LogPanel
            // 
            this.ClientSize = new System.Drawing.Size(452, 336);
            this.Controls.Add(this.groupBox1);
            this.Name = "LogPanel";
            this.Text = "LogPanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboLogLevel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel llblPath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel llblLatestLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnWarning;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.DataGridView dgv;
    }
}