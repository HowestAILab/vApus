namespace vApus.DistributedTesting
{
    partial class ConfigureTileStresstest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureTileStresstest));
            this.solutionComponentPropertyPanelBasic = new vApus.SolutionTree.SolutionComponentPropertyPanel();
            this.solutionComponentPropertyPanelAdvanced = new vApus.SolutionTree.SolutionComponentPropertyPanel();
            this.llblShowHideAdvancedSettings = new System.Windows.Forms.LinkLabel();
            this.lblUsage = new System.Windows.Forms.Label();
            this.lblUseRDP = new System.Windows.Forms.Label();
            this.lblRunSync = new System.Windows.Forms.Label();
            this.defaultAdvancedSettingsToControl = new vApus.DistributedTesting.DefaultAdvancedSettingsToControl();
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
            this.solutionComponentPropertyPanelBasic.Size = new System.Drawing.Size(360, 489);
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
            this.solutionComponentPropertyPanelAdvanced.MaximumSize = new System.Drawing.Size(360, 99999);
            this.solutionComponentPropertyPanelAdvanced.Name = "solutionComponentPropertyPanelAdvanced";
            this.solutionComponentPropertyPanelAdvanced.Size = new System.Drawing.Size(360, 409);
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
            // lblUsage
            // 
            this.lblUsage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblUsage.AutoSize = true;
            this.lblUsage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsage.ForeColor = System.Drawing.Color.DimGray;
            this.lblUsage.Location = new System.Drawing.Point(117, 237);
            this.lblUsage.Name = "lblUsage";
            this.lblUsage.Size = new System.Drawing.Size(489, 26);
            this.lblUsage.TabIndex = 3;
            this.lblUsage.Text = "Add Tiles to the Distributed Test and Tile Stresstests to a Tile clicking the \'+ " +
                "button\'.\r\nSelect a Tile Stresstest to configure it.";
            this.lblUsage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblUseRDP
            // 
            this.lblUseRDP.AutoSize = true;
            this.lblUseRDP.BackColor = System.Drawing.SystemColors.Control;
            this.lblUseRDP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUseRDP.ForeColor = System.Drawing.Color.DimGray;
            this.lblUseRDP.Location = new System.Drawing.Point(0, 11);
            this.lblUseRDP.Name = "lblUseRDP";
            this.lblUseRDP.Size = new System.Drawing.Size(553, 26);
            this.lblUseRDP.TabIndex = 4;
            this.lblUseRDP.Text = "Check \'Use RDP\' if you want vApus to open remote desktop connections to the used " +
                "clients.\r\nRegardless if you check it or not, you need to be logged into the clie" +
                "nts to be able to stresstest.";
            this.lblUseRDP.Visible = false;
            // 
            // lblRunSync
            // 
            this.lblRunSync.AutoSize = true;
            this.lblRunSync.BackColor = System.Drawing.SystemColors.Control;
            this.lblRunSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRunSync.ForeColor = System.Drawing.Color.DimGray;
            this.lblRunSync.Location = new System.Drawing.Point(0, 47);
            this.lblRunSync.Name = "lblRunSync";
            this.lblRunSync.Size = new System.Drawing.Size(504, 78);
            this.lblRunSync.TabIndex = 5;
            this.lblRunSync.Text = resources.GetString("lblRunSync.Text");
            this.lblRunSync.Visible = false;
            // 
            // defaultAdvancedSettingsToControl
            // 
            this.defaultAdvancedSettingsToControl.BackColor = System.Drawing.SystemColors.Control;
            this.defaultAdvancedSettingsToControl.Location = new System.Drawing.Point(366, 14);
            this.defaultAdvancedSettingsToControl.Name = "defaultAdvancedSettingsToControl";
            this.defaultAdvancedSettingsToControl.Size = new System.Drawing.Size(350, 50);
            this.defaultAdvancedSettingsToControl.TabIndex = 0;
            this.defaultAdvancedSettingsToControl.CheckChanged += new System.EventHandler(this.defaultAdvancedSettingsToControl_CheckChanged);
            // 
            // ConfigureTileStresstest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblRunSync);
            this.Controls.Add(this.lblUseRDP);
            this.Controls.Add(this.lblUsage);
            this.Controls.Add(this.solutionComponentPropertyPanelAdvanced);
            this.Controls.Add(this.solutionComponentPropertyPanelBasic);
            this.Controls.Add(this.llblShowHideAdvancedSettings);
            this.Controls.Add(this.defaultAdvancedSettingsToControl);
            this.Name = "ConfigureTileStresstest";
            this.Size = new System.Drawing.Size(724, 500);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SolutionTree.SolutionComponentPropertyPanel solutionComponentPropertyPanelBasic;
        private SolutionTree.SolutionComponentPropertyPanel solutionComponentPropertyPanelAdvanced;
        private System.Windows.Forms.LinkLabel llblShowHideAdvancedSettings;
        private System.Windows.Forms.Label lblUsage;
        private System.Windows.Forms.Label lblUseRDP;
        private System.Windows.Forms.Label lblRunSync;
        private DefaultAdvancedSettingsToControl defaultAdvancedSettingsToControl;
    }
}
