namespace vApus.Util
{
    partial class ExtendedSchedule
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
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dtpTime = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpDuration = new System.Windows.Forms.DateTimePicker();
            this.rdbNow = new System.Windows.Forms.RadioButton();
            this.rdbLater = new System.Windows.Forms.RadioButton();
            this.chkDuration = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // dtpDate
            // 
            this.dtpDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpDate.Location = new System.Drawing.Point(32, 54);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(290, 20);
            this.dtpDate.TabIndex = 2;
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtp_ValueChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(166, 158);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 24);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(247, 158);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 24);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // dtpTime
            // 
            this.dtpTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpTime.CustomFormat = "HH:mm:ss";
            this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTime.Location = new System.Drawing.Point(32, 80);
            this.dtpTime.Name = "dtpTime";
            this.dtpTime.ShowUpDown = true;
            this.dtpTime.Size = new System.Drawing.Size(290, 20);
            this.dtpTime.TabIndex = 3;
            this.dtpTime.ValueChanged += new System.EventHandler(this.dtp_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Start at:";
            // 
            // dtpDuration
            // 
            this.dtpDuration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpDuration.CustomFormat = "HH:mm:ss";
            this.dtpDuration.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDuration.Location = new System.Drawing.Point(32, 129);
            this.dtpDuration.Name = "dtpDuration";
            this.dtpDuration.ShowUpDown = true;
            this.dtpDuration.Size = new System.Drawing.Size(290, 20);
            this.dtpDuration.TabIndex = 5;
            this.dtpDuration.Value = new System.DateTime(1753, 1, 1, 1, 0, 0, 0);
            this.dtpDuration.ValueChanged += new System.EventHandler(this.dtpDuration_ValueChanged);
            // 
            // rdbNow
            // 
            this.rdbNow.AutoSize = true;
            this.rdbNow.Checked = true;
            this.rdbNow.Location = new System.Drawing.Point(12, 12);
            this.rdbNow.Name = "rdbNow";
            this.rdbNow.Size = new System.Drawing.Size(47, 17);
            this.rdbNow.TabIndex = 0;
            this.rdbNow.TabStop = true;
            this.rdbNow.Text = "Now";
            this.rdbNow.UseVisualStyleBackColor = true;
            // 
            // rdbLater
            // 
            this.rdbLater.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdbLater.AutoSize = true;
            this.rdbLater.Location = new System.Drawing.Point(12, 70);
            this.rdbLater.Name = "rdbLater";
            this.rdbLater.Size = new System.Drawing.Size(14, 13);
            this.rdbLater.TabIndex = 1;
            this.rdbLater.TabStop = true;
            this.rdbLater.UseVisualStyleBackColor = true;
            // 
            // chkDuration
            // 
            this.chkDuration.AutoSize = true;
            this.chkDuration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkDuration.Location = new System.Drawing.Point(12, 106);
            this.chkDuration.Name = "chkDuration";
            this.chkDuration.Size = new System.Drawing.Size(94, 17);
            this.chkDuration.TabIndex = 10;
            this.chkDuration.Text = "Fixed duration:";
            this.chkDuration.UseVisualStyleBackColor = true;
            // 
            // ExtendedSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(334, 194);
            this.Controls.Add(this.chkDuration);
            this.Controls.Add(this.rdbLater);
            this.Controls.Add(this.rdbNow);
            this.Controls.Add(this.dtpDuration);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dtpTime);
            this.Controls.Add(this.dtpDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExtendedSchedule";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Schedule";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DateTimePicker dtpTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpDuration;
        private System.Windows.Forms.RadioButton rdbNow;
        private System.Windows.Forms.RadioButton rdbLater;
        private System.Windows.Forms.CheckBox chkDuration;
    }
}