namespace vApus.Util
{
    partial class TracertControl
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
            this.flp = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.kvpHops = new vApus.Util.KeyValuePairControl();
            this.kvpRoundtripTime = new vApus.Util.KeyValuePairControl();
            this.btnStatus = new System.Windows.Forms.Button();
            this.flp.SuspendLayout();
            this.SuspendLayout();
            // 
            // flp
            // 
            this.flp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flp.Controls.Add(this.label4);
            this.flp.Controls.Add(this.kvpHops);
            this.flp.Controls.Add(this.kvpRoundtripTime);
            this.flp.Controls.Add(this.btnStatus);
            this.flp.Location = new System.Drawing.Point(0, 0);
            this.flp.MaximumSize = new System.Drawing.Size(9999, 35);
            this.flp.MinimumSize = new System.Drawing.Size(0, 35);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(357, 35);
            this.flp.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(3, 6);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 6, 6, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "Trace Route";
            // 
            // kvpHops
            // 
            this.kvpHops.BackColor = System.Drawing.Color.LightBlue;
            this.kvpHops.Key = "? Hops";
            this.kvpHops.Location = new System.Drawing.Point(109, 6);
            this.kvpHops.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpHops.Name = "kvpHops";
            this.kvpHops.Size = new System.Drawing.Size(53, 24);
            this.kvpHops.TabIndex = 18;
            this.kvpHops.TabStop = false;
            this.kvpHops.Tooltip = "";
            this.kvpHops.Value = "";
            // 
            // kvpRoundtripTime
            // 
            this.kvpRoundtripTime.BackColor = System.Drawing.Color.LightBlue;
            this.kvpRoundtripTime.Key = "? Roundtrip Time";
            this.kvpRoundtripTime.Location = new System.Drawing.Point(165, 6);
            this.kvpRoundtripTime.Margin = new System.Windows.Forms.Padding(3, 6, 0, 3);
            this.kvpRoundtripTime.Name = "kvpRoundtripTime";
            this.kvpRoundtripTime.Size = new System.Drawing.Size(110, 24);
            this.kvpRoundtripTime.TabIndex = 19;
            this.kvpRoundtripTime.TabStop = false;
            this.kvpRoundtripTime.Tooltip = "";
            this.kvpRoundtripTime.Value = "";
            // 
            // btnStatus
            // 
            this.btnStatus.AutoSize = true;
            this.btnStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnStatus.BackColor = System.Drawing.Color.LightBlue;
            this.btnStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStatus.Location = new System.Drawing.Point(278, 6);
            this.btnStatus.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnStatus.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnStatus.Name = "btnStatus";
            this.btnStatus.Size = new System.Drawing.Size(74, 24);
            this.btnStatus.TabIndex = 20;
            this.btnStatus.Text = "Tracing...";
            this.btnStatus.UseVisualStyleBackColor = false;
            // 
            // TracertControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flp);
            this.MaximumSize = new System.Drawing.Size(9999, 35);
            this.MinimumSize = new System.Drawing.Size(0, 35);
            this.Name = "TracertControl";
            this.Size = new System.Drawing.Size(357, 35);
            this.flp.ResumeLayout(false);
            this.flp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flp;
        private System.Windows.Forms.Label label4;
        private KeyValuePairControl kvpHops;
        private KeyValuePairControl kvpRoundtripTime;
        private System.Windows.Forms.Button btnStatus;
    }
}
