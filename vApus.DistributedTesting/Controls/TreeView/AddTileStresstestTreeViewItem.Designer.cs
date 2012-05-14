namespace vApus.DistributedTesting
{
    partial class AddTileStresstestTreeViewItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTileStresstestTreeViewItem));
            this.picAdd = new System.Windows.Forms.PictureBox();
            this.lbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picAdd)).BeginInit();
            this.SuspendLayout();
            // 
            // picAdd
            // 
            this.picAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAdd.Image = ((System.Drawing.Image)(resources.GetObject("picAdd.Image")));
            this.picAdd.Location = new System.Drawing.Point(20, 4);
            this.picAdd.Name = "picAdd";
            this.picAdd.Size = new System.Drawing.Size(16, 16);
            this.picAdd.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picAdd.TabIndex = 16;
            this.picAdd.TabStop = false;
            this.picAdd.Click += new System.EventHandler(this._Click);
            this.picAdd.MouseEnter += new System.EventHandler(this._MouseEnter);
            this.picAdd.MouseLeave += new System.EventHandler(this._MouseLeave);
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lbl.Location = new System.Drawing.Point(42, 6);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(118, 13);
            this.lbl.TabIndex = 17;
            this.lbl.Text = "Add new Tile Stresstest";
            this.lbl.Click += new System.EventHandler(this._Click);
            this.lbl.MouseEnter += new System.EventHandler(this._MouseEnter);
            this.lbl.MouseLeave += new System.EventHandler(this._MouseLeave);
            // 
            // AddTileStresstestTreeViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl);
            this.Controls.Add(this.picAdd);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "AddTileStresstestTreeViewItem";
            this.Size = new System.Drawing.Size(189, 25);
            this.Click += new System.EventHandler(this._Click);
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
