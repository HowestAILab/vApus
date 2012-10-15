namespace vApus.Stresstest
{
    partial class TestAllConnections
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
            if (_testAutoResetEvent != null)
                try
                {
                    _testAutoResetEvent.Set();
                    _testAutoResetEvent.Dispose();
                }
                catch { }
            _testAutoResetEvent = null;

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
            this.btnTest = new System.Windows.Forms.Button();
            this.lvw = new System.Windows.Forms.ListView();
            this.clmConnection = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmSuccess = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCopyErrorMessage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.AutoSize = true;
            this.btnTest.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnTest.BackColor = System.Drawing.Color.White;
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTest.Location = new System.Drawing.Point(561, 526);
            this.btnTest.MaximumSize = new System.Drawing.Size(73, 24);
            this.btnTest.MinimumSize = new System.Drawing.Size(73, 24);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(73, 24);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // lvw
            // 
            this.lvw.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvw.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmConnection,
            this.clmSuccess});
            this.lvw.FullRowSelect = true;
            this.lvw.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvw.Location = new System.Drawing.Point(12, 12);
            this.lvw.MultiSelect = false;
            this.lvw.Name = "lvw";
            this.lvw.Size = new System.Drawing.Size(760, 508);
            this.lvw.TabIndex = 0;
            this.lvw.UseCompatibleStateImageBehavior = false;
            this.lvw.View = System.Windows.Forms.View.Details;
            this.lvw.SelectedIndexChanged += new System.EventHandler(this.lvw_SelectedIndexChanged);
            // 
            // clmConnection
            // 
            this.clmConnection.Text = "Connection";
            this.clmConnection.Width = 345;
            // 
            // clmSuccess
            // 
            this.clmSuccess.Text = "";
            this.clmSuccess.Width = 400;
            // 
            // btnCopyErrorMessage
            // 
            this.btnCopyErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyErrorMessage.AutoSize = true;
            this.btnCopyErrorMessage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCopyErrorMessage.BackColor = System.Drawing.Color.White;
            this.btnCopyErrorMessage.Enabled = false;
            this.btnCopyErrorMessage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopyErrorMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopyErrorMessage.Location = new System.Drawing.Point(640, 525);
            this.btnCopyErrorMessage.MaximumSize = new System.Drawing.Size(132, 24);
            this.btnCopyErrorMessage.Name = "btnCopyErrorMessage";
            this.btnCopyErrorMessage.Size = new System.Drawing.Size(132, 24);
            this.btnCopyErrorMessage.TabIndex = 2;
            this.btnCopyErrorMessage.Text = "Copy Error Message";
            this.btnCopyErrorMessage.UseVisualStyleBackColor = false;
            this.btnCopyErrorMessage.Click += new System.EventHandler(this.btnCopyErrorMessage_Click);
            // 
            // TestAllConnections
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.btnCopyErrorMessage);
            this.Controls.Add(this.lvw);
            this.Controls.Add(this.btnTest);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TestAllConnections";
            this.Text = "Test All Connections";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.ListView lvw;
        private System.Windows.Forms.ColumnHeader clmConnection;
        private System.Windows.Forms.ColumnHeader clmSuccess;
        private System.Windows.Forms.Button btnCopyErrorMessage;
    }
}