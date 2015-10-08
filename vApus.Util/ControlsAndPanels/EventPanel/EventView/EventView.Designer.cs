using RandomUtils;

namespace vApus.Util
{
    partial class EventView
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
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.largeList = new RandomUtils.LargeList();
            this.SuspendLayout();
            // 
            // sfd
            // 
            this.sfd.Filter = "*.txt|*.txt";
            this.sfd.Title = "Export to...";
            // 
            // largeList
            // 
            this.largeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.largeList.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.largeList.Location = new System.Drawing.Point(0, 0);
            this.largeList.Margin = new System.Windows.Forms.Padding(0);
            this.largeList.Name = "largeList";
            this.largeList.Size = new System.Drawing.Size(200, 200);
            this.largeList.SizeMode = RandomUtils.SizeMode.Normal;
            this.largeList.TabIndex = 2;
            // 
            // EventView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.largeList);
            this.Name = "EventView";
            this.Size = new System.Drawing.Size(200, 200);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog sfd;
        private LargeList largeList;
    }
}
