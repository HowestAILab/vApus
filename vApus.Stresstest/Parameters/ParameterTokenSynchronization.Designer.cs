namespace vApus.Stresstest
{
    partial class ParameterTokenSynchronization
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
            this.label1 = new System.Windows.Forms.Label();
            this.lvw = new System.Windows.Forms.ListView();
            this.clmParameter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmOld = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmNew = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(636, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Because one or more parameters where added or removed, tokens used in the log ent" +
    "ries where updated.\r\nThe numeric parts of those tokens where synchronized with t" +
    "he indices of the parameters.";
            // 
            // lvw
            // 
            this.lvw.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvw.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvw.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmParameter,
            this.clmOld,
            this.clmNew});
            this.lvw.FullRowSelect = true;
            this.lvw.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvw.Location = new System.Drawing.Point(0, 99);
            this.lvw.Name = "lvw";
            this.lvw.Size = new System.Drawing.Size(666, 395);
            this.lvw.TabIndex = 3;
            this.lvw.UseCompatibleStateImageBehavior = false;
            this.lvw.View = System.Windows.Forms.View.Details;
            // 
            // clmParameter
            // 
            this.clmParameter.Text = "Parameter";
            this.clmParameter.Width = 557;
            // 
            // clmOld
            // 
            this.clmOld.Text = "Old --";
            this.clmOld.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.clmOld.Width = 37;
            // 
            // clmNew
            // 
            this.clmNew.Text = "-> New";
            this.clmNew.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.clmNew.Width = 46;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(125, 60);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(3);
            this.label2.Size = new System.Drawing.Size(432, 32);
            this.label2.TabIndex = 4;
            this.label2.Text = "Please double-check the log entries that contain parameter tokens.\r\nIn some cases" +
    " they cannot be synchronized! (e.g. {1} cannot become {0})";
            // 
            // ParameterTokenSynchronization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 494);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lvw);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ParameterTokenSynchronization";
            this.Text = "ParameterTokenSynchronization";
            this.Resize += new System.EventHandler(this.ParameterTokenSynchronization_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvw;
        private System.Windows.Forms.ColumnHeader clmParameter;
        private System.Windows.Forms.ColumnHeader clmOld;
        private System.Windows.Forms.ColumnHeader clmNew;
        private System.Windows.Forms.Label label2;
    }
}