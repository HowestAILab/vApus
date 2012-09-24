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
            lblRunSync.Visible =
            lblUseRDP.Visible =
            lblUsage.Visible = false;

            defaultAdvancedSettingsToControl.Visible =
            solutionComponentPropertyPanelBasic.Visible =
            llblShowHideAdvancedSettings.Visible = true;

            if (_tileStresstest != tileStresstest)
            {
                _tileStresstest = tileStresstest;
                defaultAdvancedSettingsToControl.Init(tileStresstest);
                solutionComponentPropertyPanelBasic.SolutionComponent = _tileStresstest.BasicTileStresstest;
                solutionComponentPropertyPanelAdvanced.SolutionComponent = _tileStresstest.AdvancedTileStresstest;

                Handle_DefaultToChecked();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="showDescriptions">Should only be shown if the run sync cbo is focused</param>
        public void ClearTileStresstest(bool showDescriptions)
        {
            defaultAdvancedSettingsToControl.Visible =
            solutionComponentPropertyPanelBasic.Visible =
            solutionComponentPropertyPanelAdvanced.Visible =
            llblShowHideAdvancedSettings.Visible = false;

            lblUseRDP.Visible = lblRunSync.Visible = showDescriptions;
            lblUsage.Visible = true;
        }
        private void defaultAdvancedSettingsToControl_CheckChanged(object sender, EventArgs e)
        {
            Handle_DefaultToChecked();
        }
        private void Handle_DefaultToChecked()
        {
            if (defaultAdvancedSettingsToControl.DefaultToChecked)
                solutionComponentPropertyPanelAdvanced.Lock();
            else
                solutionComponentPropertyPanelAdvanced.Unlock();

            if (_tileStresstest.AutomaticDefaultAdvancedSettings != defaultAdvancedSettingsToControl.DefaultToChecked)
            {
                solutionComponentPropertyPanelAdvanced.Visible = true;
                _tileStresstest.AutomaticDefaultAdvancedSettings = defaultAdvancedSettingsToControl.DefaultToChecked;
                _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }

        private void llblAdvancedSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            solutionComponentPropertyPanelAdvanced.Visible = !solutionComponentPropertyPanelAdvanced.Visible;
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (sender is TileStresstest || sender is Stresstest.Stresstest || sender is Stresstest.StresstestProject)
            {
                if (_tileStresstest != null)
                {
                    if (sender is StresstestProject || sender == _tileStresstest.DefaultAdvancedSettingsTo)
                    {
                        defaultAdvancedSettingsToControl.Init(_tileStresstest);
                    }
                    if (sender is StresstestProject || sender == _tileStresstest || sender == _tileStresstest.DefaultAdvancedSettingsTo)
                    {
                        solutionComponentPropertyPanelBasic.Refresh();
                        solutionComponentPropertyPanelAdvanced.Refresh();
                    }
                    else if (sender is AdvancedTileStresstest)
                    {
                        solutionComponentPropertyPanelAdvanced.Refresh();
                    }
                }

            }
            else if (sender is Clients || sender is Client || sender is Slave ||
                    sender is Monitor.MonitorProject || sender is Monitor.Monitor)
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
                    defaultAdvancedSettingsToControl.Enabled = true;
                    solutionComponentPropertyPanelBasic.Unlock();
                    if (!defaultAdvancedSettingsToControl.DefaultToChecked)
                        solutionComponentPropertyPanelAdvanced.Unlock();
                }
                else
                {
                    defaultAdvancedSettingsToControl.Enabled = false;
                    solutionComponentPropertyPanelBasic.Lock();
                    solutionComponentPropertyPanelAdvanced.Lock();
                }
            }
        }
        #endregion
    }
}
