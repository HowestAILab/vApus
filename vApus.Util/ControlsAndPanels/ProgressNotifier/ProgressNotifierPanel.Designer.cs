namespace vApus.Util
{
    partial class ProgressNotifierPanel
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
            this.grp = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEmailAddress = new vApus.Util.LabeledTextBox();
            this.txtSmtp = new vApus.Util.LabeledTextBox();
            this.txtPassword = new vApus.Util.LabeledTextBox();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.chkWhenTestFinished = new System.Windows.Forms.CheckBox();
            this.ChkWhenError = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkAfterConcurrency = new System.Windows.Forms.CheckBox();
            this.chkAfterRun = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnEnableDisable = new System.Windows.Forms.Button();
            this.grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.SuspendLayout();
            // 
            // grp
            // 
            this.grp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grp.Controls.Add(this.label1);
            this.grp.Controls.Add(this.txtEmailAddress);
            this.grp.Controls.Add(this.txtSmtp);
            this.grp.Controls.Add(this.txtPassword);
            this.grp.Controls.Add(this.nudPort);
            this.grp.Controls.Add(this.chkWhenTestFinished);
            this.grp.Controls.Add(this.ChkWhenError);
            this.grp.Controls.Add(this.label3);
            this.grp.Controls.Add(this.chkAfterConcurrency);
            this.grp.Controls.Add(this.chkAfterRun);
            this.grp.Controls.Add(this.label2);
            this.grp.Enabled = false;
            this.grp.Location = new System.Drawing.Point(12, 12);
            this.grp.Name = "grp";
            this.grp.Size = new System.Drawing.Size(391, 239);
            this.grp.TabIndex = 1;
            this.grp.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(216, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "(port)";
            // 
            // txtEmailAddress
            // 
            this.txtEmailAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmailAddress.EmptyTextBoxLabel = "E-Mail Address";
            this.txtEmailAddress.ForeColor = System.Drawing.Color.Black;
            this.txtEmailAddress.Location = new System.Drawing.Point(29, 39);
            this.txtEmailAddress.Name = "txtEmailAddress";
            this.txtEmailAddress.Size = new System.Drawing.Size(120, 20);
            this.txtEmailAddress.TabIndex = 16;
            this.txtEmailAddress.Text = "E-Mail Address";
            this.txtEmailAddress.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtSmtp
            // 
            this.txtSmtp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSmtp.EmptyTextBoxLabel = "SMTP Server";
            this.txtSmtp.ForeColor = System.Drawing.Color.Black;
            this.txtSmtp.Location = new System.Drawing.Point(29, 65);
            this.txtSmtp.Name = "txtSmtp";
            this.txtSmtp.Size = new System.Drawing.Size(120, 20);
            this.txtSmtp.TabIndex = 17;
            this.txtSmtp.Text = "SMTP Server";
            this.txtSmtp.TextChanged += new System.EventHandler(this.txt_TextChanged);
            this.txtSmtp.Enter += new System.EventHandler(this.txtPassword_Enter);
            this.txtSmtp.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.EmptyTextBoxLabel = "Password";
            this.txtPassword.ForeColor = System.Drawing.Color.Black;
            this.txtPassword.Location = new System.Drawing.Point(155, 39);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(120, 20);
            this.txtPassword.TabIndex = 18;
            this.txtPassword.Text = "Password";
            this.txtPassword.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // nudPort
            // 
            this.nudPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPort.Location = new System.Drawing.Point(155, 65);
            this.nudPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(50, 20);
            this.nudPort.TabIndex = 19;
            this.nudPort.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // chkWhenTestFinished
            // 
            this.chkWhenTestFinished.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.chkWhenTestFinished.AutoSize = true;
            this.chkWhenTestFinished.Location = new System.Drawing.Point(155, 189);
            this.chkWhenTestFinished.Name = "chkWhenTestFinished";
            this.chkWhenTestFinished.Size = new System.Drawing.Size(104, 17);
            this.chkWhenTestFinished.TabIndex = 4;
            this.chkWhenTestFinished.Text = "The test finished";
            this.chkWhenTestFinished.UseVisualStyleBackColor = true;
            this.chkWhenTestFinished.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // ChkWhenError
            // 
            this.ChkWhenError.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ChkWhenError.AutoSize = true;
            this.ChkWhenError.Location = new System.Drawing.Point(155, 166);
            this.ChkWhenError.Name = "ChkWhenError";
            this.ChkWhenError.Size = new System.Drawing.Size(105, 17);
            this.ChkWhenError.TabIndex = 3;
            this.ChkWhenError.Text = "An error occured";
            this.ChkWhenError.UseVisualStyleBackColor = true;
            this.ChkWhenError.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(71, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "When:";
            // 
            // chkAfterConcurrency
            // 
            this.chkAfterConcurrency.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.chkAfterConcurrency.AutoSize = true;
            this.chkAfterConcurrency.Location = new System.Drawing.Point(155, 136);
            this.chkAfterConcurrency.Name = "chkAfterConcurrency";
            this.chkAfterConcurrency.Size = new System.Drawing.Size(113, 17);
            this.chkAfterConcurrency.TabIndex = 2;
            this.chkAfterConcurrency.Text = "Each concurrency";
            this.chkAfterConcurrency.UseVisualStyleBackColor = true;
            this.chkAfterConcurrency.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // chkAfterRun
            // 
            this.chkAfterRun.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.chkAfterRun.AutoSize = true;
            this.chkAfterRun.Location = new System.Drawing.Point(155, 113);
            this.chkAfterRun.Name = "chkAfterRun";
            this.chkAfterRun.Size = new System.Drawing.Size(69, 17);
            this.chkAfterRun.TabIndex = 1;
            this.chkAfterRun.Text = "Each run";
            this.chkAfterRun.UseVisualStyleBackColor = true;
            this.chkAfterRun.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(71, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "After:";
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.BackColor = System.Drawing.Color.White;
            this.btnDelete.Enabled = false;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Location = new System.Drawing.Point(328, 266);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.White;
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(247, 266);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // btnEnableDisable
            // 
            this.btnEnableDisable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEnableDisable.BackColor = System.Drawing.Color.White;
            this.btnEnableDisable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnableDisable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnableDisable.Location = new System.Drawing.Point(12, 266);
            this.btnEnableDisable.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnEnableDisable.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnEnableDisable.Name = "btnEnableDisable";
            this.btnEnableDisable.Size = new System.Drawing.Size(61, 24);
            this.btnEnableDisable.TabIndex = 10;
            this.btnEnableDisable.Text = "Enable";
            this.btnEnableDisable.UseVisualStyleBackColor = false;
            this.btnEnableDisable.Click += new System.EventHandler(this.btnEnableDisable_Click);
            // 
            // ProgressNotifierPanel
            // 
            this.ClientSize = new System.Drawing.Size(415, 301);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnEnableDisable);
            this.Controls.Add(this.grp);
            this.Name = "ProgressNotifierPanel";
            this.grp.ResumeLayout(false);
            this.grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grp;
        private System.Windows.Forms.CheckBox chkWhenTestFinished;
        private System.Windows.Forms.CheckBox ChkWhenError;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkAfterConcurrency;
        private System.Windows.Forms.CheckBox chkAfterRun;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private LabeledTextBox txtEmailAddress;
        private LabeledTextBox txtSmtp;
        private LabeledTextBox txtPassword;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnEnableDisable;

    }
}