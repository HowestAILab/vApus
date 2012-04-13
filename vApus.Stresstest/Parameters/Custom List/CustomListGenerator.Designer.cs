namespace vApus.Stresstest
{
    partial class CustomListGenerator
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
            this.components = new System.ComponentModel.Container();
            this.nudGenerate = new System.Windows.Forms.NumericUpDown();
            this.cboParameterType = new System.Windows.Forms.ComboBox();
            this.parameterTypeSolutionComponentPropertyPanel = new vApus.SolutionTree.SolutionComponentPropertyPanel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudGenerate)).BeginInit();
            this.SuspendLayout();
            // 
            // nudGenerate
            // 
            this.nudGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudGenerate.Location = new System.Drawing.Point(198, 534);
            this.nudGenerate.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.nudGenerate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudGenerate.Name = "nudGenerate";
            this.nudGenerate.Size = new System.Drawing.Size(50, 20);
            this.nudGenerate.TabIndex = 2;
            this.nudGenerate.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cboParameterType
            // 
            this.cboParameterType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboParameterType.BackColor = System.Drawing.Color.White;
            this.cboParameterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboParameterType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboParameterType.FormattingEnabled = true;
            this.cboParameterType.Items.AddRange(new object[] {
            "Numeric",
            "Text"});
            this.cboParameterType.Location = new System.Drawing.Point(12, 533);
            this.cboParameterType.Name = "cboParameterType";
            this.cboParameterType.Size = new System.Drawing.Size(180, 21);
            this.cboParameterType.TabIndex = 1;
            this.cboParameterType.SelectedIndexChanged += new System.EventHandler(this.cboParameterType_SelectedIndexChanged);
            // 
            // parameterTypeSolutionComponentPropertyPanel
            // 
            this.parameterTypeSolutionComponentPropertyPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.parameterTypeSolutionComponentPropertyPanel.BackColor = System.Drawing.Color.White;
            this.parameterTypeSolutionComponentPropertyPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.parameterTypeSolutionComponentPropertyPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.parameterTypeSolutionComponentPropertyPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.parameterTypeSolutionComponentPropertyPanel.Location = new System.Drawing.Point(0, 0);
            this.parameterTypeSolutionComponentPropertyPanel.Name = "parameterTypeSolutionComponentPropertyPanel";
            this.parameterTypeSolutionComponentPropertyPanel.Size = new System.Drawing.Size(784, 524);
            this.parameterTypeSolutionComponentPropertyPanel.SolutionComponent = null;
            this.parameterTypeSolutionComponentPropertyPanel.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.AutoSize = true;
            this.btnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(616, 530);
            this.btnOK.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnOK.MinimumSize = new System.Drawing.Size(75, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 24);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(697, 530);
            this.btnCancel.MaximumSize = new System.Drawing.Size(1000, 24);
            this.btnCancel.MinimumSize = new System.Drawing.Size(75, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 24);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // timer
            // 
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // CustomListGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.parameterTypeSolutionComponentPropertyPanel);
            this.Controls.Add(this.nudGenerate);
            this.Controls.Add(this.cboParameterType);
            this.MinimizeBox = false;
            this.Name = "CustomListGenerator";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Generate from type...";
            ((System.ComponentModel.ISupportInitialize)(this.nudGenerate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SolutionTree.SolutionComponentPropertyPanel parameterTypeSolutionComponentPropertyPanel;
        private System.Windows.Forms.NumericUpDown nudGenerate;
        private System.Windows.Forms.ComboBox cboParameterType;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Timer timer;

    }
}