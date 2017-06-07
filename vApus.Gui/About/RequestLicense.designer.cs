namespace vApus.Gui {
    partial class RequestLicense {
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
            this.btnSendRequest = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ctxtComments = new vApus.Util.CueTextBox();
            this.ctxtPhoneNumber = new vApus.Util.CueTextBox();
            this.ctxtEmailAddress = new vApus.Util.CueTextBox();
            this.ctxtCompany = new vApus.Util.CueTextBox();
            this.ctxtLastName = new vApus.Util.CueTextBox();
            this.ctxtFirstName = new vApus.Util.CueTextBox();
            this.SuspendLayout();
            // 
            // btnSendRequest
            // 
            this.btnSendRequest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendRequest.BackColor = System.Drawing.Color.White;
            this.btnSendRequest.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnSendRequest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendRequest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnSendRequest.Location = new System.Drawing.Point(12, 459);
            this.btnSendRequest.Name = "btnSendRequest";
            this.btnSendRequest.Size = new System.Drawing.Size(416, 29);
            this.btnSendRequest.TabIndex = 7;
            this.btnSendRequest.Text = "Send request";
            this.btnSendRequest.UseVisualStyleBackColor = false;
            this.btnSendRequest.Click += new System.EventHandler(this.btnSendRequest_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(416, 71);
            this.label1.TabIndex = 7;
            this.label1.Text = "Please fill in this form so we can approve the request and e-mail you a license f" +
    "ile. You may be contacted for extra information.\r\n\r\nYour contact information wil" +
    "l never be made public.";
            // 
            // ctxtComments
            // 
            this.ctxtComments.Cue = "Comments (max. 500 chars)";
            this.ctxtComments.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::vApus.Gui.Properties.Settings.Default, "Comments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ctxtComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctxtComments.Location = new System.Drawing.Point(12, 225);
            this.ctxtComments.MaxLength = 500;
            this.ctxtComments.Multiline = true;
            this.ctxtComments.Name = "ctxtComments";
            this.ctxtComments.Size = new System.Drawing.Size(416, 223);
            this.ctxtComments.TabIndex = 6;
            this.ctxtComments.Text = global::vApus.Gui.Properties.Settings.Default.Comments;
            this.ctxtComments.Leave += new System.EventHandler(this.ctxt_Leave);
            // 
            // ctxtPhoneNumber
            // 
            this.ctxtPhoneNumber.Cue = "*Phone number, e.g. +32 50 11 11 11";
            this.ctxtPhoneNumber.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::vApus.Gui.Properties.Settings.Default, "PhoneNumber", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ctxtPhoneNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctxtPhoneNumber.Location = new System.Drawing.Point(12, 158);
            this.ctxtPhoneNumber.MaxLength = 20;
            this.ctxtPhoneNumber.Name = "ctxtPhoneNumber";
            this.ctxtPhoneNumber.Size = new System.Drawing.Size(416, 24);
            this.ctxtPhoneNumber.TabIndex = 4;
            this.ctxtPhoneNumber.Text = global::vApus.Gui.Properties.Settings.Default.PhoneNumber;
            this.ctxtPhoneNumber.Leave += new System.EventHandler(this.ctxt_Leave);
            // 
            // ctxtEmailAddress
            // 
            this.ctxtEmailAddress.Cue = "*E-mail address, to send the license file to";
            this.ctxtEmailAddress.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::vApus.Gui.Properties.Settings.Default, "EmailAddress", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ctxtEmailAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctxtEmailAddress.Location = new System.Drawing.Point(12, 188);
            this.ctxtEmailAddress.MaxLength = 250;
            this.ctxtEmailAddress.Name = "ctxtEmailAddress";
            this.ctxtEmailAddress.Size = new System.Drawing.Size(416, 24);
            this.ctxtEmailAddress.TabIndex = 5;
            this.ctxtEmailAddress.Text = global::vApus.Gui.Properties.Settings.Default.EmailAddress;
            this.ctxtEmailAddress.Leave += new System.EventHandler(this.ctxt_Leave);
            // 
            // ctxtCompany
            // 
            this.ctxtCompany.Cue = "Company";
            this.ctxtCompany.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::vApus.Gui.Properties.Settings.Default, "Company", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ctxtCompany.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctxtCompany.Location = new System.Drawing.Point(12, 123);
            this.ctxtCompany.MaxLength = 250;
            this.ctxtCompany.Name = "ctxtCompany";
            this.ctxtCompany.Size = new System.Drawing.Size(416, 24);
            this.ctxtCompany.TabIndex = 2;
            this.ctxtCompany.Text = global::vApus.Gui.Properties.Settings.Default.Company;
            this.ctxtCompany.Leave += new System.EventHandler(this.ctxt_Leave);
            // 
            // ctxtLastName
            // 
            this.ctxtLastName.Cue = "*Last name";
            this.ctxtLastName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::vApus.Gui.Properties.Settings.Default, "LastName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ctxtLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctxtLastName.Location = new System.Drawing.Point(214, 93);
            this.ctxtLastName.MaxLength = 100;
            this.ctxtLastName.Name = "ctxtLastName";
            this.ctxtLastName.Size = new System.Drawing.Size(214, 24);
            this.ctxtLastName.TabIndex = 1;
            this.ctxtLastName.Text = global::vApus.Gui.Properties.Settings.Default.LastName;
            this.ctxtLastName.Leave += new System.EventHandler(this.ctxt_Leave);
            // 
            // ctxtFirstName
            // 
            this.ctxtFirstName.BackColor = System.Drawing.Color.White;
            this.ctxtFirstName.Cue = "*First name";
            this.ctxtFirstName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::vApus.Gui.Properties.Settings.Default, "FirstName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ctxtFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctxtFirstName.Location = new System.Drawing.Point(12, 93);
            this.ctxtFirstName.MaxLength = 100;
            this.ctxtFirstName.Name = "ctxtFirstName";
            this.ctxtFirstName.Size = new System.Drawing.Size(196, 24);
            this.ctxtFirstName.TabIndex = 0;
            this.ctxtFirstName.Text = global::vApus.Gui.Properties.Settings.Default.FirstName;
            this.ctxtFirstName.Leave += new System.EventHandler(this.ctxt_Leave);
            // 
            // RequestLicense
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 503);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ctxtComments);
            this.Controls.Add(this.btnSendRequest);
            this.Controls.Add(this.ctxtPhoneNumber);
            this.Controls.Add(this.ctxtEmailAddress);
            this.Controls.Add(this.ctxtCompany);
            this.Controls.Add(this.ctxtLastName);
            this.Controls.Add(this.ctxtFirstName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RequestLicense";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Request license";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Util.CueTextBox ctxtFirstName;
        private Util.CueTextBox ctxtLastName;
        private Util.CueTextBox ctxtCompany;
        private Util.CueTextBox ctxtEmailAddress;
        private Util.CueTextBox ctxtPhoneNumber;
        private System.Windows.Forms.Button btnSendRequest;
        private Util.CueTextBox ctxtComments;
        private System.Windows.Forms.Label label1;
    }
}