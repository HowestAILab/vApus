namespace vApus.Util
{
    partial class TestProgressNotifierPanel
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
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtEmailaddress = new vApus.Util.LabeledTextBox();
            this.chkSecure = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUsername = new vApus.Util.LabeledTextBox();
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
            this.label4 = new System.Windows.Forms.Label();
            this.grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.SuspendLayout();
            // 
            // grp
            // 
            this.grp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grp.Controls.Add(this.label6);
            this.grp.Controls.Add(this.label5);
            this.grp.Controls.Add(this.txtEmailaddress);
            this.grp.Controls.Add(this.chkSecure);
            this.grp.Controls.Add(this.label1);
            this.grp.Controls.Add(this.txtUsername);
            this.grp.Controls.Add(this.txtSmtp);
            this.grp.Controls.Add(this.txtPassword);
            this.grp.Controls.Add(this.nudPort);
            this.grp.Controls.Add(this.chkWhenTestFinished);
            this.grp.Controls.Add(this.label3);
            this.grp.Controls.Add(this.chkAfterConcurrency);
            this.grp.Controls.Add(this.chkAfterRun);
            this.grp.Controls.Add(this.label2);
            this.grp.Enabled = false;
            this.grp.Location = new System.Drawing.Point(12, 38);
            this.grp.Name = "grp";
            this.grp.Size = new System.Drawing.Size(486, 277);
            this.grp.TabIndex = 0;
            this.grp.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 79);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Mailserver";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Recepient";
            // 
            // txtEmailaddress
            // 
            this.txtEmailaddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmailaddress.EmptyTextBoxLabel = "E-Mail Address";
            this.txtEmailaddress.ForeColor = System.Drawing.Color.Black;
            this.txtEmailaddress.Location = new System.Drawing.Point(14, 36);
            this.txtEmailaddress.Name = "txtEmailaddress";
            this.txtEmailaddress.Size = new System.Drawing.Size(267, 20);
            this.txtEmailaddress.TabIndex = 0;
            this.txtEmailaddress.Text = "E-Mail Address";
            this.txtEmailaddress.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // chkSecure
            // 
            this.chkSecure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSecure.AutoSize = true;
            this.chkSecure.Location = new System.Drawing.Point(380, 97);
            this.chkSecure.Name = "chkSecure";
            this.chkSecure.Size = new System.Drawing.Size(71, 17);
            this.chkSecure.TabIndex = 3;
            this.chkSecure.Text = "TLS/SSL";
            this.chkSecure.UseVisualStyleBackColor = true;
            this.chkSecure.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(343, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "(port)";
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.EmptyTextBoxLabel = "Username (optional)";
            this.txtUsername.ForeColor = System.Drawing.Color.Black;
            this.txtUsername.Location = new System.Drawing.Point(14, 121);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(267, 20);
            this.txtUsername.TabIndex = 4;
            this.txtUsername.Text = "Username (optional)";
            this.txtUsername.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtSmtp
            // 
            this.txtSmtp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSmtp.EmptyTextBoxLabel = "smtp.foo.bar";
            this.txtSmtp.ForeColor = System.Drawing.Color.Black;
            this.txtSmtp.Location = new System.Drawing.Point(14, 95);
            this.txtSmtp.Name = "txtSmtp";
            this.txtSmtp.Size = new System.Drawing.Size(267, 20);
            this.txtSmtp.TabIndex = 1;
            this.txtSmtp.Text = "smtp.foo.bar";
            this.txtSmtp.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.EmptyTextBoxLabel = "Password (optional)";
            this.txtPassword.ForeColor = System.Drawing.Color.Black;
            this.txtPassword.Location = new System.Drawing.Point(287, 121);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(120, 20);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.Text = "Password (optional)";
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.TextChanged += new System.EventHandler(this.txt_TextChanged);
            this.txtPassword.Enter += new System.EventHandler(this.txtPassword_Enter);
            this.txtPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // nudPort
            // 
            this.nudPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPort.Location = new System.Drawing.Point(287, 95);
            this.nudPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(50, 20);
            this.nudPort.TabIndex = 2;
            this.nudPort.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudPort.ValueChanged += new System.EventHandler(this.nudPort_ValueChanged);
            // 
            // chkWhenTestFinished
            // 
            this.chkWhenTestFinished.AutoSize = true;
            this.chkWhenTestFinished.Location = new System.Drawing.Point(92, 226);
            this.chkWhenTestFinished.Name = "chkWhenTestFinished";
            this.chkWhenTestFinished.Size = new System.Drawing.Size(207, 17);
            this.chkWhenTestFinished.TabIndex = 8;
            this.chkWhenTestFinished.Text = "The test is finished or an error occured";
            this.chkWhenTestFinished.UseVisualStyleBackColor = true;
            this.chkWhenTestFinished.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 227);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "When:";
            // 
            // chkAfterConcurrency
            // 
            this.chkAfterConcurrency.AutoSize = true;
            this.chkAfterConcurrency.Location = new System.Drawing.Point(92, 197);
            this.chkAfterConcurrency.Name = "chkAfterConcurrency";
            this.chkAfterConcurrency.Size = new System.Drawing.Size(113, 17);
            this.chkAfterConcurrency.TabIndex = 7;
            this.chkAfterConcurrency.Text = "Each concurrency";
            this.chkAfterConcurrency.UseVisualStyleBackColor = true;
            this.chkAfterConcurrency.CheckedChanged += new System.EventHandler(this.chkAfterConcurrency_CheckedChanged);
            // 
            // chkAfterRun
            // 
            this.chkAfterRun.AutoSize = true;
            this.chkAfterRun.Location = new System.Drawing.Point(92, 174);
            this.chkAfterRun.Name = "chkAfterRun";
            this.chkAfterRun.Size = new System.Drawing.Size(69, 17);
            this.chkAfterRun.TabIndex = 6;
            this.chkAfterRun.Text = "Each run";
            this.chkAfterRun.UseVisualStyleBackColor = true;
            this.chkAfterRun.CheckedChanged += new System.EventHandler(this.chkAfterRun_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 175);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "After:";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.AutoSize = true;
            this.btnClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClear.BackColor = System.Drawing.Color.White;
            this.btnClear.Enabled = false;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(344, 321);
            this.btnClear.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 24);
            this.btnClear.TabIndex = 3;
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
            this.btnSave.Location = new System.Drawing.Point(398, 321);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 24);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Set";
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
            this.btnEnableDisable.Size = new System.Drawing.Size(75, 24);
            this.btnEnableDisable.TabIndex = 1;
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
            this.btnTest.Location = new System.Drawing.Point(93, 321);
            this.btnTest.MaximumSize = new System.Drawing.Size(9999, 24);
            this.btnTest.MinimumSize = new System.Drawing.Size(0, 24);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 24);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(305, 26);
            this.label4.TabIndex = 14;
            this.label4.Text = "A digest is send for events that happen shortly after each other.\r\nThe minimum la" +
    "tency is 30 seconds.";
            // 
            // TestProgressNotifierPanel
            // 
            this.ClientSize = new System.Drawing.Size(510, 357);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnEnableDisable);
            this.Controls.Add(this.grp);
            this.Name = "TestProgressNotifierPanel";
            this.grp.ResumeLayout(false);
            this.grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grp;
        private System.Windows.Forms.CheckBox chkWhenTestFinished;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkAfterConcurrency;
        private System.Windows.Forms.CheckBox chkAfterRun;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private LabeledTextBox txtUsername;
        private LabeledTextBox txtSmtp;
        private LabeledTextBox txtPassword;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnEnableDisable;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.CheckBox chkSecure;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private LabeledTextBox txtEmailaddress;

    }
}