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
using vApus.Util;

namespace vApus.DistributedTest {
    public partial class SlaveTile : UserControl {
        #region Events

        public event EventHandler DuplicateClicked;
        public event EventHandler DeleteClicked;

        public event EventHandler GoToAssignedTest;

        #endregion

        #region Fields

        private readonly DistributedTest _distributedTest;
        private bool _clientOnline = false;
        private DistributedTestMode _distributedTestMode;
        private Slave _slave;

        #endregion

        #region Properties

        public Slave Slave {
            get { return _slave; }
        }

        public bool ClientOnline {
            get { return _clientOnline; }
        }

        #endregion

        #region Constructor

        public SlaveTile() {
            InitializeComponent();
        }

        public SlaveTile(DistributedTest distributedTest)
            : this() {
            _distributedTest = distributedTest;
        }

        #endregion

        #region Functions

        public void SetSlave(Slave slave) {
            if (_slave != slave)
                _slave = slave;

            SetGui();
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _slave)
                SetGui();
        }

        private void SetGui() {
            if (nudPort.Value != _slave.Port) {
                nudPort.ValueChanged -= nudPort_ValueChanged;
                nudPort.Value = _slave.Port;
                nudPort.ValueChanged += nudPort_ValueChanged;
            }

            string ts = _slave.TileStressTest == null ? "..." : _slave.TileStressTest.ToString();
            if (llblTest.Text != ts) {
                llblTest.Text = ts;
                if (ts == "...") {
                    toolTip.SetToolTip(llblTest, null);
                } else //Show the full name in the tooltip
                {
                    string label = _slave.TileStressTest.Parent + " -> " + _slave.TileStressTest.Index + ") " +
                                   ((_slave.TileStressTest.BasicTileStressTest.Connection == null ||
                                     _slave.TileStressTest.BasicTileStressTest.Connection.IsEmpty)
                                        ? string.Empty
                                        : _slave.TileStressTest.BasicTileStressTest.Connection.ToString());


                    toolTip.SetToolTip(llblTest, label);
                }
            }
        }

        private void nudPort_ValueChanged(object sender, EventArgs e) {
            //Check if the port is not already used. Don't allow duplicates.
            var port = (int)nudPort.Value;
            foreach (Slave slave in _slave.Parent)
                if (slave.Port == port && slave != _slave) {
                    MessageBox.Show(this, "Cannot use the same port more than once.", string.Empty, MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    nudPort.ValueChanged -= nudPort_ValueChanged;
                    nudPort.Value = _slave.Port;
                    nudPort.ValueChanged += nudPort_ValueChanged;

                    return;
                }

            _slave.Port = port;
            _slave.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void llblTest_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var assignTest = new AssignTest(_distributedTest, _slave.TileStressTest);
            if (assignTest.ShowDialog() == DialogResult.OK) {
                try {
                    _slave.AssignTileStressTest(assignTest.AssignedTest);

                    string ts = _slave.TileStressTest == null ? "..." : _slave.TileStressTest.ToString();
                    if (llblTest.Text != ts) {
                        llblTest.Text = ts;
                        if (ts == "...") {
                            toolTip.SetToolTip(llblTest, null);
                        } else //Show the full name in the tooltip
                        {
                            string label = _slave.TileStressTest.Parent + " -> " + _slave.TileStressTest.Index + ") " +
                                           ((_slave.TileStressTest.BasicTileStressTest.Connection == null ||
                                             _slave.TileStressTest.BasicTileStressTest.Connection.IsEmpty)
                                                ? string.Empty
                                                : _slave.TileStressTest.BasicTileStressTest.Connection.ToString());


                            toolTip.SetToolTip(llblTest, label);
                        }
                    }

                    _slave.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                    if (assignTest.GoToAssignedTest && GoToAssignedTest != null)
                        GoToAssignedTest(this, null);
                } catch (Exception ex) {
                    MessageBox.Show(ex.ToString(), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void picDuplicate_Click(object sender, EventArgs e) {
            if (DuplicateClicked != null)
                DuplicateClicked(this, null);
        }

        private void picDelete_Click(object sender, EventArgs e) {
            if (DeleteClicked != null)
                DeleteClicked(this, null);
        }

        public void SetMode(DistributedTestMode distributedTestMode) {
            if (_distributedTestMode != distributedTestMode) {
                _distributedTestMode = distributedTestMode;
                if (_distributedTestMode == DistributedTestMode.Edit)
                    if (_slave.TileStressTest == null) {
                        Visible = true;
                    } else {
                        picDuplicate.Visible =
                            picDelete.Visible =
                            nudPort.Enabled =
                            llblTest.Enabled = true;
                    } else if (_slave.TileStressTest == null) {
                    Visible = false;
                } else {
                    picDuplicate.Visible =
                        picDelete.Visible =
                        nudPort.Enabled =
                        llblTest.Enabled = false;
                }
            }
        }

        #endregion
    }
}