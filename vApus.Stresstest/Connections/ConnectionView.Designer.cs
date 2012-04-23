namespace vApus.Stresstest
{
    partial class ConnectionView
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
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.split = new System.Windows.Forms.SplitContainer();
            this.solutionComponentPropertyPanel = new vApus.SolutionTree.SolutionComponentPropertyPanel();
            this.ruleSetSyntaxItemPanel = new vApus.Stresstest.ConnectionProxyRuleSetSyntaxItemPanel();
            this.tracertControl = new vApus.Util.TracertControl();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestConnection.AutoSize = true;
            this.btnTestConnection.BackColor = System.Drawing.Color.White;
            this.btnTestConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTestConnection.Location = new System.Drawing.Point(668, 536);
            this.btnTestConnection.MaximumSize = new System.Drawing.Size(112, 24);
            this.btnTestConnection.MinimumSize = new System.Drawing.Size(112, 24);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(112, 24);
            this.btnTestConnection.TabIndex = 2;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = false;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // split
            // 
            this.split.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.split.Location = new System.Drawing.Point(12, 12);
            this.split.Name = "split";
            this.split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split.Panel1
            // 
            this.split.Panel1.BackColor = System.Drawing.Color.White;
            this.split.Panel1.Controls.Add(this.solutionComponentPropertyPanel);
            this.split.Panel1MinSize = 130;
            // 
            // split.Panel2
            // 
            this.split.Panel2.BackColor = System.Drawing.Color.White;
            this.split.Panel2.Controls.Add(this.ruleSetSyntaxItemPanel);
            this.split.Size = new System.Drawing.Size(768, 520);
            this.split.SplitterDistance = 130;
            this.split.TabIndex = 0;
            // 
            // solutionComponentPropertyPanel
            // 
            this.solutionComponentPropertyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.solutionComponentPropertyPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.solutionComponentPropertyPanel.Location = new System.Drawing.Point(0, 0);
            this.solutionComponentPropertyPanel.Name = "solutionComponentPropertyPanel";
            this.solutionComponentPropertyPanel.Size = new System.Drawing.Size(766, 128);
            this.solutionComponentPropertyPanel.SolutionComponent = null;
            this.solutionComponentPropertyPanel.TabIndex = 0;
            // 
            // ruleSetSyntaxItemPanel
            // 
            this.ruleSetSyntaxItemPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ruleSetSyntaxItemPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ruleSetSyntaxItemPanel.Location = new System.Drawing.Point(0, 0);
            this.ruleSetSyntaxItemPanel.Name = "ruleSetSyntaxItemPanel";
            this.ruleSetSyntaxItemPanel.Size = new System.Drawing.Size(766, 384);
            this.ruleSetSyntaxItemPanel.TabIndex = 0;
            // 
            // tracertControl
            // 
            this.tracertControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tracertControl.Location = new System.Drawing.Point(6, 532);
            this.tracertControl.MaximumSize = new System.Drawing.Size(9999, 35);
            this.tracertControl.MinimumSize = new System.Drawing.Size(0, 35);
            this.tracertControl.Name = "tracertControl";
            this.tracertControl.Size = new System.Drawing.Size(656, 35);
            this.tracertControl.TabIndex = 1;
            this.tracertControl.Visible = false;
            this.tracertControl.Done += new System.EventHandler(this.tracertControl_Done);
            // 
            // ConnectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.tracertControl);
            this.Controls.Add(this.split);
            this.Controls.Add(this.btnTestConnection);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ConnectionView";
            this.Text = "ConnectionView";
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTestConnection;
        private ConnectionProxyRuleSetSyntaxItemPanel ruleSetSyntaxItemPanel;
        private System.Windows.Forms.SplitContainer split;
        private SolutionTree.SolutionComponentPropertyPanel solutionComponentPropertyPanel;
        private Util.TracertControl tracertControl;
    }
}