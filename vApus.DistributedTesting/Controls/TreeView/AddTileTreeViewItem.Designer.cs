namespace vApus.DistributedTesting
{
    partial class AddTileTreeViewItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTileTreeViewItem));
            this.lbl = new System.Windows.Forms.Label();
            this.picAdd = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picAdd)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lbl.Location = new System.Drawing.Point(28, 6);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(69, 13);
            this.lbl.TabIndex = 17;
            this.lbl.Text = "Add new Tile";
            this.lbl.Click += new System.EventHandler(this._Click);
            this.lbl.MouseEnter += new System.EventHandler(this._MouseEnter);
            this.lbl.MouseLeave += new System.EventHandler(this._MouseLeave);
            // 
            // picAdd
            // 
            this.picAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAdd.Image = ((System.Drawing.Image)(resources.GetObject("picAdd.Image")));
            this.picAdd.Location = new System.Drawing.Point(6, 4);
            this.picAdd.Name = "picAdd";
            this.picAdd.Size = new System.Drawing.Size(16, 16);
            this.picAdd.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picAdd.TabIndex = 16;
            this.picAdd.TabStop = false;
            this.picAdd.Click += new System.EventHandler(this._Click);
            this.picAdd.MouseEnter += new System.EventHandler(this._MouseEnter);
            this.picAdd.MouseLeave += new System.EventHandler(this._MouseLeave);
            // 
            // AddTileTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(200)))), ((int)(((byte)(229)))));
            this.Controls.Add(this.lbl);
            this.Controls.Add(this.picAdd);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "AddTileTreeViewItem";
            this.Size = new System.Drawing.Size(150, 25);
            this.Click += new System.EventHandler(this._Click);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this._KeyUp);
            this.MouseEnter += new System.EventHandler(this._MouseEnter);
            this.MouseLeave += new System.EventHandler(this._MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.picAdd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picAdd;
        private System.Windows.Forms.Label lbl;
    }
}
