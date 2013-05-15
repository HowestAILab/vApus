/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting {
    public partial class ConfigureTileStresstest : UserControl {
        #region Fields
        private LinkButton _selectNextAvailableSlave;
        private int _currentSlaveIndex = -1;

        private DistributedTestMode _distributedTestMode;
        private TileStresstest _tileStresstest;

        #endregion

        #region Properties

        public TileStresstest TileStresstest {
            get { return _tileStresstest; }
        }

        #endregion

        #region Constructor

        public ConfigureTileStresstest() {
            InitializeComponent();


            //For refreshing the property panels.
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

        #endregion

        #region Functions

        public void SetTileStresstest(TileStresstest tileStresstest) {
            lblRunSync.Visible =
                lblUseRDP.Visible =
                lblUsage.Visible = false;

            defaultAdvancedSettingsToControl.Visible =
                solutionComponentPropertyPanelBasic.Visible =
                llblShowHideAdvancedSettings.Visible = true;

            if (_tileStresstest != tileStresstest) {
                _tileStresstest = tileStresstest;
                defaultAdvancedSettingsToControl.Init(tileStresstest);
                solutionComponentPropertyPanelBasic.SolutionComponent = _tileStresstest.BasicTileStresstest;
                AddSelectNextAvailableSlaveButtonIfNeeded();
                solutionComponentPropertyPanelAdvanced.SolutionComponent = _tileStresstest.AdvancedTileStresstest;

                Handle_DefaultToChecked();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="showDescriptions">Should only be shown if the run sync cbo is focused</param>
        public void ClearTileStresstest(bool showDescriptions) {
            defaultAdvancedSettingsToControl.Visible =
                solutionComponentPropertyPanelBasic.Visible =
                solutionComponentPropertyPanelAdvanced.Visible =
                llblShowHideAdvancedSettings.Visible = false;

            lblUseRDP.Visible = lblRunSync.Visible = showDescriptions;
            lblUsage.Visible = true;
        }

        private void defaultAdvancedSettingsToControl_CheckChanged(object sender, EventArgs e) {
            Handle_DefaultToChecked();
        }

        private void Handle_DefaultToChecked() {
            if (defaultAdvancedSettingsToControl.DefaultToChecked)
                solutionComponentPropertyPanelAdvanced.Lock();
            else
                solutionComponentPropertyPanelAdvanced.Unlock();

            if (_tileStresstest.AutomaticDefaultAdvancedSettings != defaultAdvancedSettingsToControl.DefaultToChecked) {
                solutionComponentPropertyPanelAdvanced.Visible = true;
                _tileStresstest.AutomaticDefaultAdvancedSettings = defaultAdvancedSettingsToControl.DefaultToChecked;
                _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }

        private void llblAdvancedSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            solutionComponentPropertyPanelAdvanced.Visible = !solutionComponentPropertyPanelAdvanced.Visible;
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender is TileStresstest || sender is Stresstest.Stresstest || sender is StresstestProject) {
                if (_tileStresstest != null) {
                    if (sender is StresstestProject || sender == _tileStresstest.DefaultAdvancedSettingsTo) {
                        defaultAdvancedSettingsToControl.Init(_tileStresstest);
                    }
                    if (sender is StresstestProject || sender == _tileStresstest || sender == _tileStresstest.DefaultAdvancedSettingsTo) {
                        solutionComponentPropertyPanelBasic.Refresh();
                        AddSelectNextAvailableSlaveButtonIfNeeded();
                        solutionComponentPropertyPanelAdvanced.Refresh();
                    } else if (sender is AdvancedTileStresstest) {
                        solutionComponentPropertyPanelAdvanced.Refresh();
                    }
                }
            } else if (sender is Clients || sender is Client || sender is Slave || sender is MonitorProject || sender is Monitor.Monitor) {
                solutionComponentPropertyPanelBasic.Refresh();
                AddSelectNextAvailableSlaveButtonIfNeeded();
            }
        }

        public void SetMode(DistributedTestMode distributedTestMode) {
            if (_distributedTestMode != distributedTestMode) {
                _distributedTestMode = distributedTestMode;
                if (_distributedTestMode == DistributedTestMode.Edit) {
                    defaultAdvancedSettingsToControl.Enabled = true;
                    solutionComponentPropertyPanelBasic.Unlock();
                    if (_selectNextAvailableSlave != null) _selectNextAvailableSlave.Enabled = true;
                    if (!defaultAdvancedSettingsToControl.DefaultToChecked) solutionComponentPropertyPanelAdvanced.Unlock();
                } else {
                    defaultAdvancedSettingsToControl.Enabled = false;
                    solutionComponentPropertyPanelBasic.Lock();
                    if (_selectNextAvailableSlave != null) _selectNextAvailableSlave.Enabled = false;
                    solutionComponentPropertyPanelAdvanced.Lock();
                }
            }
        }

        private void AddSelectNextAvailableSlaveButtonIfNeeded() {
            try {
                if (_selectNextAvailableSlave != null && solutionComponentPropertyPanelBasic.Controls.Contains(_selectNextAvailableSlave))
                    return;

                if (_selectNextAvailableSlave != null)
                    try {
                        _selectNextAvailableSlave.Click -= _selectNextAvailableSlave_Click;
                        _selectNextAvailableSlave = null;
                    } catch { }

                _selectNextAvailableSlave = new LinkButton();
                _selectNextAvailableSlave.Text = "Select Next Available Slave";
                _selectNextAvailableSlave.LinkColor = _selectNextAvailableSlave.ActiveLinkColor =
                    _selectNextAvailableSlave.VisitedLinkColor = _selectNextAvailableSlave.ForeColor = Color.DimGray;
                _selectNextAvailableSlave.LinkBehavior = LinkBehavior.AlwaysUnderline;
                _selectNextAvailableSlave.TextAlign = ContentAlignment.TopCenter;
                _selectNextAvailableSlave.Padding = new Padding(3, 4, 3, 3);
                _selectNextAvailableSlave.AutoSize = true;
                _selectNextAvailableSlave.Click += _selectNextAvailableSlave_Click;
                solutionComponentPropertyPanelBasic.Controls.Add(_selectNextAvailableSlave);
            } catch { }
        }

        private void _selectNextAvailableSlave_Click(object sender, EventArgs e) {
            try {
                var slaveIndices = TileStresstest.BasicTileStresstest.SlaveIndices;
                if (slaveIndices.Length != 0) {
                    //Get the ones that are available
                    var availableSlaves = new List<Slave>();

                    var distributedTest = _tileStresstest.Parent.GetParent().GetParent() as DistributedTest;
                    if (distributedTest != null) {
                        foreach (Client client in distributedTest.Clients)
                            foreach (Slave slave in client)
                                availableSlaves.Add(slave);

                        foreach (Tile tile in distributedTest.Tiles)
                            if (tile.Use)
                                foreach (TileStresstest tileStresstest in tile)
                                    if (tileStresstest.Use && tileStresstest.BasicTileStresstest.SlaveIndices.Length != 0)
                                        availableSlaves.Remove(tileStresstest.BasicTileStresstest.Slaves[0]);

                        if (availableSlaves.Count != 0) {
                            if (++_currentSlaveIndex >= availableSlaves.Count) _currentSlaveIndex = 0;
                            _tileStresstest.BasicTileStresstest.Slaves = new Slave[] { availableSlaves[_currentSlaveIndex] };
                            _tileStresstest.Use = true;
                            _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                        }
                    }
                }
            } catch {
            }
        }

        /// <summary>
        ///     Refresh some properties that are overriden in code.
        /// </summary>
        public override void Refresh() {
            base.Refresh();
            solutionComponentPropertyPanelAdvanced.Refresh();
        }

        #endregion
    }
}