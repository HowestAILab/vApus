namespace vApus.LogFixer
{
    partial class FixTab
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FixTab));
            this.largeList = new vApus.Util.LargeList();
            this.flp1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tbtnOK = new vApus.Util.ToggleButton();
            this.tbtnError = new vApus.Util.ToggleButton();
            this.btnApplyFix = new System.Windows.Forms.Button();
            this.btnRestoreAll = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.flp2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flp1.SuspendLayout();
            this.flp2.SuspendLayout();
            this.SuspendLayout();
            // 
            // largeList
            // 
            this.largeList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.largeList.BackColor = System.Drawing.SystemColors.Control;
            this.largeList.Location = new System.Drawing.Point(0, 35);
            this.largeList.Name = "largeList";
            this.largeList.Size = new System.Drawing.Size(600, 445);
            this.largeList.SizeMode = vApus.Util.SizeMode.StretchHorizontal;
            this.largeList.TabIndex = 2;
            // 
            // flp1
            // 
            this.flp1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flp1.Controls.Add(this.tbtnOK);
            this.flp1.Controls.Add(this.tbtnError);
            this.flp1.Location = new System.Drawing.Point(3, 3);
            this.flp1.Name = "flp1";
            this.flp1.Size = new System.Drawing.Size(250, 29);
            this.flp1.TabIndex = 0;
            this.flp1.TabStop = true;
            // 
            // tbtnOK
            // 
            this.tbtnOK.Appearance = System.Windows.Forms.Appearance.Button;
            this.tbtnOK.AutoSize = true;
            this.tbtnOK.BackColor = System.Drawing.SystemColors.Control;
            this.tbtnOK.Checked = true;
            this.tbtnOK.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tbtnOK.Image = ((System.Drawing.Image)(resources.GetObject("tbtnOK.Image")));
            this.tbtnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tbtnOK.Location = new System.Drawing.Point(3, 3);
            this.tbtnOK.Name = "tbtnOK";
            this.tbtnOK.Size = new System.Drawing.Size(38, 23);
            this.tbtnOK.TabIndex = 0;
            this.tbtnOK.Tag = "";
            this.tbtnOK.Text = "     0";
            this.tbtnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tbtnOK.UseVisualStyleBackColor = false;
            this.tbtnOK.Visible = false;
            this.tbtnOK.CheckedChanged += new System.EventHandler(this.tbtnOK_CheckedChanged);
            // 
            // tbtnError
            // 
            this.tbtnError.Appearance = System.Windows.Forms.Appearance.Button;
            this.tbtnError.AutoSize = true;
            this.tbtnError.BackColor = System.Drawing.SystemColors.Control;
            this.tbtnError.Checked = true;
            this.tbtnError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tbtnError.Image = ((System.Drawing.Image)(resources.GetObject("tbtnError.Image")));
            this.tbtnError.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tbtnError.Location = new System.Drawing.Point(47, 3);
            this.tbtnError.Name = "tbtnError";
            this.tbtnError.Size = new System.Drawing.Size(38, 23);
            this.tbtnError.TabIndex = 2;
            this.tbtnError.Tag = "";
            this.tbtnError.Text = "     0";
            this.tbtnError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tbtnError.UseVisualStyleBackColor = false;
            this.tbtnError.Visible = false;
            this.tbtnError.CheckedChanged += new System.EventHandler(this.tbtnError_CheckedChanged);
            // 
            // btnApplyFix
            // 
            this.btnApplyFix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyFix.AutoSize = true;
            this.btnApplyFix.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnApplyFix.BackColor = System.Drawing.Color.LightGray;
            this.btnApplyFix.Location = new System.Drawing.Point(125, 3);
            this.btnApplyFix.Name = "btnApplyFix";
            this.btnApplyFix.Size = new System.Drawing.Size(68, 23);
            this.btnApplyFix.TabIndex = 0;
            this.btnApplyFix.Text = "Apply Fix...";
            this.btnApplyFix.UseVisualStyleBackColor = false;
            this.btnApplyFix.Click += new System.EventHandler(this.btnApplyFix_Click);
            // 
            // btnRestoreAll
            // 
            this.btnRestoreAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestoreAll.AutoSize = true;
            this.btnRestoreAll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRestoreAll.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnRestoreAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestoreAll.Location = new System.Drawing.Point(199, 3);
            this.btnRestoreAll.Name = "btnRestoreAll";
            this.btnRestoreAll.Size = new System.Drawing.Size(79, 23);
            this.btnRestoreAll.TabIndex = 1;
            this.btnRestoreAll.Text = "Restore All";
            this.btnRestoreAll.UseVisualStyleBackColor = false;
            this.btnRestoreAll.Visible = false;
            this.btnRestoreAll.Click += new System.EventHandler(this.btnRestoreAll_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.BackColor = System.Drawing.Color.LightGray;
            this.btnSave.Location = new System.Drawing.Point(284, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(51, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // sfd
            // 
            this.sfd.Filter = "TXT Files|*.txt";
            // 
            // flp2
            // 
            this.flp2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flp2.Controls.Add(this.btnSave);
            this.flp2.Controls.Add(this.btnRestoreAll);
            this.flp2.Controls.Add(this.btnApplyFix);
            this.flp2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flp2.Location = new System.Drawing.Point(259, 3);
            this.flp2.Name = "flp2";
            this.flp2.Size = new System.Drawing.Size(338, 29);
            this.flp2.TabIndex = 1;
            this.flp2.TabStop = true;
            // 
            // FixTab
            // 
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.flp2);
            this.Controls.Add(this.largeList);
            this.Controls.Add(this.flp1);
            this.Name = "FixTab";
            this.Size = new System.Drawing.Size(600, 480);
            this.flp1.ResumeLayout(false);
            this.flp1.PerformLayout();
            this.flp2.ResumeLayout(false);
            this.flp2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.FlowLayoutPanel flp1;
        private vApus.Util.ToggleButton tbtnError;
        private vApus.Util.ToggleButton tbtnOK;
        private System.Windows.Forms.Button btnApplyFix;
        private vApus.Util.LargeList largeList;
        private System.Windows.Forms.Button btnRestoreAll;
        private System.Windows.Forms.FlowLayoutPanel flp2;
    }
}
