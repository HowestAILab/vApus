namespace vApus.DistributedTest
{
    partial class ConfigureTileStressTest
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
            this.solutionComponentPropertyPanelBasic = new vApus.SolutionTree.SolutionComponentPropertyPanel();
            this.solutionComponentPropertyPanelAdvanced = new vApus.SolutionTree.SolutionComponentPropertyPanel();
            this.llblShowHideAdvancedSettings = new System.Windows.Forms.LinkLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.defaultAdvancedSettingsToControl = new vApus.DistributedTest.DefaultAdvancedSettingsToControl();
            this.SuspendLayout();
            // 
            // solutionComponentPropertyPanelBasic
            // 
            this.solutionComponentPropertyPanelBasic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.solutionComponentPropertyPanelBasic.AutoSelectControl = false;
            this.solutionComponentPropertyPanelBasic.BackColor = System.Drawing.Color.White;
            this.solutionComponentPropertyPanelBasic.Location = new System.Drawing.Point(0, 11);
            this.solutionComponentPropertyPanelBasic.MaximumSize = new System.Drawing.Size(360, 99999);
            this.solutionComponentPropertyPanelBasic.Name = "solutionComponentPropertyPanelBasic";
            this.solutionComponentPropertyPanelBasic.Size = new System.Drawing.Size(360, 472);
            this.solutionComponentPropertyPanelBasic.SolutionComponent = null;
            this.solutionComponentPropertyPanelBasic.TabIndex = 1;
            this.solutionComponentPropertyPanelBasic.Visible = false;
            // 
            // solutionComponentPropertyPanelAdvanced
            // 
            this.solutionComponentPropertyPanelAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.solutionComponentPropertyPanelAdvanced.AutoSelectControl = false;
            this.solutionComponentPropertyPanelAdvanced.Location = new System.Drawing.Point(363, 91);
            this.solutionComponentPropertyPanelAdvanced.Margin = new System.Windows.Forms.Padding(0);
            this.solutionComponentPropertyPanelAdvanced.MaximumSize = new System.Drawing.Size(376, 99999);
            this.solutionComponentPropertyPanelAdvanced.Name = "solutionComponentPropertyPanelAdvanced";
            this.solutionComponentPropertyPanelAdvanced.Size = new System.Drawing.Size(376, 392);
            this.solutionComponentPropertyPanelAdvanced.SolutionComponent = null;
            this.solutionComponentPropertyPanelAdvanced.TabIndex = 0;
            this.solutionComponentPropertyPanelAdvanced.Visible = false;
            // 
            // llblShowHideAdvancedSettings
            // 
            this.llblShowHideAdvancedSettings.AutoSize = true;
            this.llblShowHideAdvancedSettings.Location = new System.Drawing.Point(658, 67);
            this.llblShowHideAdvancedSettings.Name = "llblShowHideAdvancedSettings";
            this.llblShowHideAdvancedSettings.Size = new System.Drawing.Size(61, 13);
            this.llblShowHideAdvancedSettings.TabIndex = 1;
            this.llblShowHideAdvancedSettings.TabStop = true;
            this.llblShowHideAdvancedSettings.Text = "Show/Hide";
            this.llblShowHideAdvancedSettings.Visible = false;
            this.llblShowHideAdvancedSettings.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblAdvancedSettings_LinkClicked);
            // 
            // defaultAdvancedSettingsToControl
            // 
            this.defaultAdvancedSettingsToControl.BackColor = System.Drawing.SystemColors.Control;
            this.defaultAdvancedSettingsToControl.Location = new System.Drawing.Point(366, 14);
            this.defaultAdvancedSettingsToControl.MaximumSize = new System.Drawing.Size(350, 46);
            this.defaultAdvancedSettingsToControl.MinimumSize = new System.Drawing.Size(350, 46);
            this.defaultAdvancedSettingsToControl.Name = "defaultAdvancedSettingsToControl";
            this.defaultAdvancedSettingsToControl.Size = new System.Drawing.Size(350, 46);
            this.defaultAdvancedSettingsToControl.TabIndex = 0;
            this.defaultAdvancedSettingsToControl.Visible = false;
            this.defaultAdvancedSettingsToControl.CheckChanged += new System.EventHandler(this.defaultAdvancedSettingsToControl_CheckChanged);
            // 
            // ConfigureTileStressTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.solutionComponentPropertyPanelAdvanced);
            this.Controls.Add(this.solutionComponentPropertyPanelBasic);
            this.Controls.Add(this.llblShowHideAdvancedSettings);
            this.Controls.Add(this.defaultAdvancedSettingsToControl);
            this.Name = "ConfigureTileStressTest";
            this.Size = new System.Drawing.Size(740, 483);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SolutionTree.SolutionComponentPropertyPanel solutionComponentPropertyPanelBasic;
        private SolutionTree.SolutionComponentPropertyPanel solutionComponentPropertyPanelAdvanced;
        private System.Windows.Forms.LinkLabel llblShowHideAdvancedSettings;
        private DefaultAdvancedSettingsToControl defaultAdvancedSettingsToControl;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
