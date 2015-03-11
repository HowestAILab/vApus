namespace vApus.APITool {
    partial class CheatSheet {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheatSheet));
            this.fctxtScript = new vApus.Util.CodeTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.fctxtScript)).BeginInit();
            this.SuspendLayout();
            // 
            // fctxtScript
            // 
            this.fctxtScript.AutoScrollMinSize = new System.Drawing.Size(0, 784);
            this.fctxtScript.BackBrush = null;
            this.fctxtScript.CharHeight = 14;
            this.fctxtScript.CharWidth = 8;
            this.fctxtScript.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctxtScript.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctxtScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fctxtScript.IsReplaceMode = false;
            this.fctxtScript.LeftBracket = '(';
            this.fctxtScript.Location = new System.Drawing.Point(0, 0);
            this.fctxtScript.Name = "fctxtScript";
            this.fctxtScript.Paddings = new System.Windows.Forms.Padding(0);
            this.fctxtScript.PreferredLineWidth = 65536;
            this.fctxtScript.ReadOnly = true;
            this.fctxtScript.RightBracket = ')';
            this.fctxtScript.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctxtScript.Size = new System.Drawing.Size(784, 562);
            this.fctxtScript.TabIndex = 3;
            this.fctxtScript.Text = resources.GetString("fctxtScript.Text");
            this.fctxtScript.WordWrap = true;
            this.fctxtScript.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.CharWrapControlWidth;
            this.fctxtScript.Zoom = 100;
            // 
            // CheatSheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.fctxtScript);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CheatSheet";
            this.Text = "Cheat Sheet";
            ((System.ComponentModel.ISupportInitialize)(this.fctxtScript)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private vApus.Util.CodeTextBox fctxtScript;
    }
}