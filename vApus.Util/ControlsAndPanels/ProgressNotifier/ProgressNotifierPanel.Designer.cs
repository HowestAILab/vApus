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
            this.chkSecure = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEmailAddress = new vApus.Util.LabeledTextBox();
            this.txtSmtp = new vApus.Util.LabeledTextBox();
            this.txtPassword = new vApus.Util.LabeledTextBox();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.chkWhenTestFinished = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkAfterConcurrency = new System.Windows.Forms.CheckBox();
            this.chkAfterRun = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnEnableDisable = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.SuspendLayout();
            // 
            // grp
            // 
            this.grp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grp.Controls.Add(this.chkSecure);
            this.grp.Controls.Add(this.label1);
            this.grp.Controls.Add(this.txtEmailAddress);
            this.grp.Controls.Add(this.txtSmtp);
            this.grp.Controls.Add(this.txtPassword);
            this.grp.Controls.Add(this.nudPort);
            this.grp.Controls.Add(this.chkWhenTestFinished);
            this.grp.Controls.Add(this.label3);
            this.grp.Controls.Add(this.chkAfterConcurrency);
            this.grp.Controls.Add(this.chkAfterRun);
            this.grp.Controls.Add(this.label2);
            this.grp.Enabled = false;
            this.grp.Location = new System.Drawing.Point(12, 12);
            this.grp.Name = "grp";
            this.grp.Size = new System.Drawing.Size(375, 303);
            this.grp.TabIndex = 1;
            this.grp.TabStop = false;
            // 
            // chkSecure
            // 
            this.chkSecure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSecure.AutoSize = true;
            this.chkSecure.Location = new System.Drawing.Point(271, 67);
            this.chkSecure.Name = "chkSecure";
            this.chkSecure.Size = new System.Drawing.Size(60, 17);
            this.chkSecure.TabIndex = 21;
            this.chkSecure.Text = "Secure";
            this.chkSecure.UseVisualStyleBackColor = true;
            this.chkSecure.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(234, 68);
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
            this.txtEmailAddress.Location = new System.Drawing.Point(16, 39);
            this.txtEmailAddress.Name = "txtEmailAddress";
            this.txtEmailAddress.Size = new System.Drawing.Size(156, 20);
            this.txtEmailAddress.TabIndex = 16;
            this.txtEmailAddress.Text = "dieter.vandroemme@gmail.com";
            this.txtEmailAddress.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtSmtp
            // 
            this.txtSmtp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSmtp.EmptyTextBoxLabel = "SMTP Server:";
            this.txtSmtp.ForeColor = System.Drawing.Color.Black;
            this.txtSmtp.Location = new System.Drawing.Point(16, 65);
            this.txtSmtp.Name = "txtSmtp";
            this.txtSmtp.Size = new System.Drawing.Size(156, 20);
            this.txtSmtp.TabIndex = 17;
            this.txtSmtp.Text = "smtp.gmail.com";
            this.txtSmtp.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.EmptyTextBoxLabel = "Password";
            this.txtPassword.ForeColor = System.Drawing.Color.Black;
            this.txtPassword.Location = new System.Drawing.Point(178, 39);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(120, 20);
            this.txtPassword.TabIndex = 18;
            this.txtPassword.Text = "XQCT3q7Sh6eg";
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.TextChanged += new System.EventHandler(this.txt_TextChanged);
            this.txtPassword.Enter += new System.EventHandler(this.txtPassword_Enter);
            this.txtPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // nudPort
            // 
            this.nudPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPort.Location = new System.Drawing.Point(178, 65);
            this.nudPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(50, 20);
            this.nudPort.TabIndex = 19;
            this.nudPort.Value = new decimal(new int[] {
            587,
            0,
            0,
            0});
            this.nudPort.ValueChanged += new System.EventHandler(this.nudPort_ValueChanged);
            // 
            // chkWhenTestFinished
            // 
            this.chkWhenTestFinished.AutoSize = true;
            this.chkWhenTestFinished.Location = new System.Drawing.Point(97, 165);
            this.chkWhenTestFinished.Name = "chkWhenTestFinished";
            this.chkWhenTestFinished.Size = new System.Drawing.Size(207, 17);
            this.chkWhenTestFinished.TabIndex = 4;
            this.chkWhenTestFinished.Text = "The test is finished or an error occured";
            this.chkWhenTestFinished.UseVisualStyleBackColor = true;
            this.chkWhenTestFinished.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "When:";
            // 
            // chkAfterConcurrency
            // 
            this.chkAfterConcurrency.AutoSize = true;
            this.chkAfterConcurrency.Location = new System.Drawing.Point(97, 136);
            this.chkAfterConcurrency.Name = "chkAfterConcurrency";
            this.chkAfterConcurrency.Size = new System.Drawing.Size(113, 17);
            this.chkAfterConcurrency.TabIndex = 2;
            this.chkAfterConcurrency.Text = "Each concurrency";
            this.chkAfterConcurrency.UseVisualStyleBackColor = true;
            this.chkAfterConcurrency.CheckedChanged += new System.EventHandler(this.chkAfterConcurrency_CheckedChanged);
            // 
            // chkAfterRun
            // 
            this.chkAfterRun.AutoSize = true;
            this.chkAfterRun.Location = new System.Drawing.Point(97, 113);
            this.chkAfterRun.Name = "chkAfterRun";
            this.chkAfterRun.Size = new System.Drawing.Size(69, 17);
            this.chkAfterRun.TabIndex = 1;
            this.chkAfterRun.Text = "Each run";
            this.chkAfterRun.UseVisualStyleBackColor = true;
            this.chkAfterRun.CheckedChanged += new System.EventHandler(this.chkAfterRun_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "After:";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.BackColor = System.Drawing.Color.White;
            this.btnClear.Enabled = false;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(312, 322);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.White;
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(231, 322);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnEnableDisable
            // 
            this.btnEnableDisable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEnableDisable.BackColor = System.Drawing.Color.White;
            this.btnEnableDisable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnableDisable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnableDisable.Location = new System.Drawing.Point(12, 321);
            this.btnEnableDisable.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnEnableDisable.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnEnableDisable.Name = "btnEnableDisable";
            this.btnEnableDisable.Size = new System.Drawing.Size(61, 24);
            this.btnEnableDisable.TabIndex = 10;
            this.btnEnableDisable.Text = "Enable";
            this.btnEnableDisable.UseVisualStyleBackColor = false;
            this.btnEnableDisable.Click += new System.EventHandler(this.btnEnableDisable_Click);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest.BackColor = System.Drawing.Color.White;
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTest.Location = new System.Drawing.Point(79, 321);
            this.btnTest.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnTest.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(61, 24);
            this.btnTest.TabIndex = 13;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // ProgressNotifierPanel
            // 
            this.ClientSize = new System.Drawing.Size(399, 357);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnClear);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkAfterConcurrency;
        private System.Windows.Forms.CheckBox chkAfterRun;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private LabeledTextBox txtEmailAddress;
        private LabeledTextBox txtSmtp;
        private LabeledTextBox txtPassword;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnEnableDisable;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.CheckBox chkSecure;

    }
}