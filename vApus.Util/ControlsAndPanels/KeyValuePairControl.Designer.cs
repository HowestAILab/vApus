namespace vApus.Util
{
    partial class KeyValuePairControl
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
            this.components = new System.ComponentModel.Container();
            this.lblKey = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblKey
            // 
            this.lblKey.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblKey.AutoSize = true;
            this.lblKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKey.Location = new System.Drawing.Point(3, 0);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(32, 13);
            this.lblKey.TabIndex = 0;
            this.lblKey.Text = "Key:";
            this.lblKey.SizeChanged += new System.EventHandler(this.lbl_SizeChanged);
            this.lblKey.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbl_MouseDown);
            // 
            // lblValue
            // 
            this.lblValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblValue.AutoEllipsis = true;
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(39, 0);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(34, 13);
            this.lblValue.TabIndex = 0;
            this.lblValue.Text = "Value";
            this.lblValue.SizeChanged += new System.EventHandler(this.lbl_SizeChanged);
            this.lblValue.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbl_MouseDown);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 20;
            // 
            // KeyValuePairControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.lblKey);
            this.Name = "KeyValuePairControl";
            this.Size = new System.Drawing.Size(75, 16);
            this.SizeChanged += new System.EventHandler(this.KeyValuePairControl_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
