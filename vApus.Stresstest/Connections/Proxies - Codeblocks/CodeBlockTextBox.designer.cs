using vApus.Util;
namespace vApus.Stresstest
{
    partial class CodeBlockTextBox
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
            this.fastColoredTextBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.SuspendLayout();
            // 
            // fastColoredTextBox
            // 
            this.fastColoredTextBox.AutoScrollMinSize = new System.Drawing.Size(0, 15);
            this.fastColoredTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fastColoredTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastColoredTextBox.Location = new System.Drawing.Point(0, 0);
            this.fastColoredTextBox.Name = "fastColoredTextBox";
            this.fastColoredTextBox.PreferredLineWidth = 65536;
            this.fastColoredTextBox.Size = new System.Drawing.Size(867, 150);
            this.fastColoredTextBox.TabIndex = 0;
            this.fastColoredTextBox.WordWrap = true;
            this.fastColoredTextBox.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.CharWrapControlWidth;
            this.fastColoredTextBox.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fastColoredTextBox_TextChanged);
            this.fastColoredTextBox.SelectionChanged += new System.EventHandler(this.fastColoredTextBox_SelectionChanged);
            this.fastColoredTextBox.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fastColoredTextBox_TextChangedDelayed);
            this.fastColoredTextBox.Enter += new System.EventHandler(this.fastColoredTextBox_Enter);
            this.fastColoredTextBox.Leave += new System.EventHandler(this.fastColoredTextBox_Leave);
            this.fastColoredTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fastColoredTextBox_MouseDown);
            this.fastColoredTextBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.fastColoredTextBox_PreviewKeyDown);
            // 
            // CodeBlockTextBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.fastColoredTextBox);
            this.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.Name = "CodeBlockTextBox";
            this.Size = new System.Drawing.Size(867, 150);
            this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBox;



    }
}
