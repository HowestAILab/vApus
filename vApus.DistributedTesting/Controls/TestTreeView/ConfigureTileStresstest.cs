/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.SolutionTree;
using vApus.StressTest;
using vApus.Util;

namespace vApus.DistributedTest {
    public partial class ConfigureTileStressTest : UserControl {
        #region Fields
        private LinkButton _selectNextAvailableSlave;
        private int _currentSlaveIndex = -1;

        private DistributedTestMode _distributedTestMode;
        private TileStressTest _tileStressTest;

        #endregion

        #region Properties

        public TileStressTest TileStressTest {
            get { return _tileStressTest; }
        }

        #endregion

        #region Constructor

        public ConfigureTileStressTest() {
            InitializeComponent();


            //For refreshing the property panels.
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

        #endregion

        #region Functions

        public void SetTileStressTest(TileStressTest tileStressTest) {
            defaultAdvancedSettingsToControl.Visible = solutionComponentPropertyPanelBasic.Visible = llblShowHideAdvancedSettings.Visible = true;

            if (_tileStressTest != tileStressTest) {
                _tileStressTest = tileStressTest;
                defaultAdvancedSettingsToControl.Init(tileStressTest);
                solutionComponentPropertyPanelBasic.SolutionComponent = _tileStressTest.BasicTileStressTest;
                AddSelectNextAvailableSlaveButtonIfNeeded();
                solutionComponentPropertyPanelAdvanced.SolutionComponent = _tileStressTest.AdvancedTileStressTest;

                Handle_DefaultToChecked();
            }
        }

        /// <summary>
        /// </summary>
        public void ClearTileStressTest() {
            defaultAdvancedSettingsToControl.Visible =
                solutionComponentPropertyPanelBasic.Visible =
                solutionComponentPropertyPanelAdvanced.Visible =
                llblShowHideAdvancedSettings.Visible = false;
        }

        private void defaultAdvancedSettingsToControl_CheckChanged(object sender, EventArgs e) {
            Handle_DefaultToChecked();
        }

        private void Handle_DefaultToChecked() {
            if (defaultAdvancedSettingsToControl.DefaultToChecked)
                solutionComponentPropertyPanelAdvanced.Lock();
            else
                solutionComponentPropertyPanelAdvanced.Unlock();

            if (_tileStressTest.AutomaticDefaultAdvancedSettings != defaultAdvancedSettingsToControl.DefaultToChecked) {
                solutionComponentPropertyPanelAdvanced.Visible = true;
                _tileStressTest.AutomaticDefaultAdvancedSettings = defaultAdvancedSettingsToControl.DefaultToChecked;
                _tileStressTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }

        private void llblAdvancedSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            solutionComponentPropertyPanelAdvanced.Visible = !solutionComponentPropertyPanelAdvanced.Visible;
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender is TileStressTest || sender is StressTest.StressTest || sender is StressTestProject) {
                if (_tileStressTest != null) {
                    if (sender is StressTestProject || sender == _tileStressTest.DefaultAdvancedSettingsTo) {
                        defaultAdvancedSettingsToControl.Init(_tileStressTest);
                    }
                    if (sender is StressTestProject || sender == _tileStressTest || sender == _tileStressTest.DefaultAdvancedSettingsTo) {
                        solutionComponentPropertyPanelBasic.Refresh();
                        AddSelectNextAvailableSlaveButtonIfNeeded();
                        solutionComponentPropertyPanelAdvanced.Refresh();
                    } else if (sender is AdvancedTileStressTest) {
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
                    } catch {
                        //Should / can not happen.
                    }

                _selectNextAvailableSlave = new LinkButton();
                _selectNextAvailableSlave.Text = "Select next available slave";
                _selectNextAvailableSlave.LinkColor = _selectNextAvailableSlave.ActiveLinkColor = _selectNextAvailableSlave.VisitedLinkColor = _selectNextAvailableSlave.ForeColor = Color.DimGray;
                _selectNextAvailableSlave.LinkBehavior = LinkBehavior.AlwaysUnderline;
                _selectNextAvailableSlave.TextAlign = ContentAlignment.TopCenter;
                _selectNextAvailableSlave.Padding = new Padding(3, 4, 3, 3);
                _selectNextAvailableSlave.AutoSize = true;
                toolTip.SetToolTip(_selectNextAvailableSlave, "Slaves in unused tile stress tests can also be selected.");
                _selectNextAvailableSlave.Click += _selectNextAvailableSlave_Click;
                solutionComponentPropertyPanelBasic.Controls.Add(_selectNextAvailableSlave);
            } catch {
                //Only on gui closed.
            }
        }

        private void _selectNextAvailableSlave_Click(object sender, EventArgs e) {
            try {
                //Get the ones that are available
                var availableSlaves = new List<Slave>();

                var distributedTest = _tileStressTest.Parent.GetParent().GetParent() as DistributedTest;
                if (distributedTest != null) {
                    foreach (Client client in distributedTest.Clients)
                        foreach (Slave slave in client)
                            availableSlaves.Add(slave);

                    foreach (Tile tile in distributedTest.Tiles)
                        if (tile.Use)
                            foreach (TileStressTest tileStressTest in tile)
                                if (tileStressTest.Use)
                                    foreach (var slave in tileStressTest.BasicTileStressTest.Slaves)
                                        availableSlaves.Remove(slave);

                    if (availableSlaves.Count != 0) {
                        if (++_currentSlaveIndex >= availableSlaves.Count) _currentSlaveIndex = 0;
                        _tileStressTest.BasicTileStressTest.Slaves = new Slave[] { availableSlaves[_currentSlaveIndex] };
                        _tileStressTest.Use = true;
                        _tileStressTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                    }
                }
            } catch {
                //Only on gui closed.
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