/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Stresstest;

namespace vApus.DistributedTesting
{
    public partial class ConfigureTileStresstest : UserControl
    {
        #region Fields
        private TileStresstest _tileStresstest;
        private DistributedTestMode _distributedTestMode;
        #endregion

        #region Properties
        public TileStresstest TileStresstest
        {
            get { return _tileStresstest; }
        }
        #endregion

        #region Constructor
        public ConfigureTileStresstest()
        {
            InitializeComponent();

            //For refreshing the property panels.
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }
        #endregion

        #region Functions
        public void SetTileStresstest(TileStresstest tileStresstest)
        {
            lblRunSync.Visible = false;
            lblUsage.Visible = false;

            solutionComponentPropertyPanelDefaultTo.Visible =
            solutionComponentPropertyPanelBasic.Visible =
            llblAdvancedSettings.Visible =
            chkAutomatic.Visible = true;

            if (_tileStresstest != tileStresstest)
            {
                _tileStresstest = tileStresstest;
                solutionComponentPropertyPanelDefaultTo.SolutionComponent = _tileStresstest;
                solutionComponentPropertyPanelBasic.SolutionComponent = _tileStresstest.BasicTileStresstest;
                solutionComponentPropertyPanelAdvanced.SolutionComponent = _tileStresstest.AdvancedTileStresstest;


                chkAutomatic.Checked = _tileStresstest.AutomaticDefaultAdvancedSettings;

                Handle_chkAutomatic_CheckedChanged();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="showRunSyncDescription">Should only be shown if the run sync cbo is focused</param>
        public void ClearTileStresstest(bool showRunSyncDescription)
        {
            solutionComponentPropertyPanelDefaultTo.Visible =
            solutionComponentPropertyPanelBasic.Visible =
            solutionComponentPropertyPanelAdvanced.Visible =
            llblAdvancedSettings.Visible =
            chkAutomatic.Visible = false;

            lblRunSync.Visible = showRunSyncDescription;
            lblUsage.Visible = true;

        }
        private void chkAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            Handle_chkAutomatic_CheckedChanged();
        }

        private void Handle_chkAutomatic_CheckedChanged()
        {
            if (chkAutomatic.Checked)
                solutionComponentPropertyPanelAdvanced.Lock();
            else
                solutionComponentPropertyPanelAdvanced.Unlock();

            if (_tileStresstest.AutomaticDefaultAdvancedSettings != chkAutomatic.Checked)
            {
                solutionComponentPropertyPanelAdvanced.Visible = true;
                _tileStresstest.AutomaticDefaultAdvancedSettings = chkAutomatic.Checked;
                _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }

        private void llblAdvancedSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            solutionComponentPropertyPanelAdvanced.Visible = !solutionComponentPropertyPanelAdvanced.Visible;
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            TileStresstest ts = solutionComponentPropertyPanelDefaultTo.SolutionComponent as TileStresstest;
            if (ts != null)
            {
                if (sender is StresstestProject || sender == ts.DefaultSettingsTo)
                {
                    solutionComponentPropertyPanelDefaultTo.Refresh();
                }
                if (sender is StresstestProject || sender == ts || sender == ts.DefaultSettingsTo)
                {
                    solutionComponentPropertyPanelBasic.Refresh();
                    solutionComponentPropertyPanelAdvanced.Refresh();
                }
            }
            if (sender is ClientsAndSlaves || sender is Client || sender is Slave)
            {
                solutionComponentPropertyPanelBasic.Refresh();
            }
        }

        public void SetMode(DistributedTestMode distributedTestMode)
        {
            if (_distributedTestMode != distributedTestMode)
            {
                _distributedTestMode = distributedTestMode;
                if (_distributedTestMode == DistributedTestMode.Edit)
                {
                    solutionComponentPropertyPanelDefaultTo.Unlock();
                    solutionComponentPropertyPanelBasic.Unlock();
                    solutionComponentPropertyPanelAdvanced.Unlock();

                    chkAutomatic.Enabled = true;
                }
                else
                {
                    solutionComponentPropertyPanelDefaultTo.Lock();
                    solutionComponentPropertyPanelBasic.Lock();
                    solutionComponentPropertyPanelAdvanced.Lock();

                    chkAutomatic.Enabled = false;
                }
            }
        }
        #endregion
    }
}
