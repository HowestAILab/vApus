namespace vApus.Stresstest
{
    partial class ErrorAndFindSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorAndFindSelector));
            this.ts = new System.Windows.Forms.ToolStrip();
            this.btnNextError = new System.Windows.Forms.ToolStripButton();
            this.btnSelectError = new System.Windows.Forms.ToolStripButton();
            this.btnPreviousError = new System.Windows.Forms.ToolStripButton();
            this.btnFind = new System.Windows.Forms.ToolStripButton();
            this.txtFind = new System.Windows.Forms.ToolStripTextBox();
            this.ts.SuspendLayout();
            this.SuspendLayout();
            // 
            // ts
            // 
            this.ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNextError,
            this.btnSelectError,
            this.btnPreviousError,
            this.btnFind,
            this.txtFind});
            this.ts.Location = new System.Drawing.Point(0, 0);
            this.ts.Name = "ts";
            this.ts.Size = new System.Drawing.Size(450, 25);
            this.ts.TabIndex = 4;
            // 
            // btnNextError
            // 
            this.btnNextError.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnNextError.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNextError.Enabled = false;
            this.btnNextError.Image = ((System.Drawing.Image)(resources.GetObject("btnNextError.Image")));
            this.btnNextError.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNextError.Name = "btnNextError";
            this.btnNextError.Size = new System.Drawing.Size(23, 22);
            this.btnNextError.Text = "Next Error";
            this.btnNextError.ToolTipText = "Next Error";
            this.btnNextError.Click += new System.EventHandler(this.btnNextError_Click);
            // 
            // btnSelectError
            // 
            this.btnSelectError.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnSelectError.Image = global::vApus.Stresstest.Properties.Resources.LogEntryError;
            this.btnSelectError.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectError.Name = "btnSelectError";
            this.btnSelectError.Size = new System.Drawing.Size(49, 22);
            this.btnSelectError.Text = "1 / ?";
            this.btnSelectError.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnSelectError.ToolTipText = "Go to Error";
            this.btnSelectError.Click += new System.EventHandler(this.btnSelectError_Click);
            // 
            // btnPreviousError
            // 
            this.btnPreviousError.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnPreviousError.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPreviousError.Enabled = false;
            this.btnPreviousError.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousError.Image")));
            this.btnPreviousError.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPreviousError.Name = "btnPreviousError";
            this.btnPreviousError.Size = new System.Drawing.Size(23, 22);
            this.btnPreviousError.Text = "Previous Error";
            this.btnPreviousError.ToolTipText = "Previous Error";
            this.btnPreviousError.Click += new System.EventHandler(this.btnPreviousError_Click);
            // 
            // btnFind
            // 
            this.btnFind.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(23, 22);
            this.btnFind.Text = "Find";
            this.btnFind.ToolTipText = "Find Next";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtFind
            // 
            this.txtFind.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.txtFind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(300, 25);
            this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFind_KeyDown);
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            // 
            // ErrorAndFindSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ts);
            this.Name = "ErrorAndFindSelector";
            this.Size = new System.Drawing.Size(450, 25);
            this.ts.ResumeLayout(false);
            this.ts.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip ts;
        private System.Windows.Forms.ToolStripButton btnNextError;
        private System.Windows.Forms.ToolStripButton btnPreviousError;
        private System.Windows.Forms.ToolStripButton btnSelectError;
        private System.Windows.Forms.ToolStripButton btnFind;
        private System.Windows.Forms.ToolStripTextBox txtFind;

    }
}
