/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using vApus.DistributedTesting.Properties;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting {
    [ToolboxItem(false)]
    public partial class TileStresstestTreeViewItem : UserControl, ITreeViewItem {
        #region Events

        /// <summary>
        ///     Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;

        public event EventHandler DuplicateClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler DoubleClicked;

        /// <summary>
        ///     Event of the test clicked.
        /// </summary>
        public event EventHandler<EventProgressChart.ProgressEventEventArgs> EventClicked;

        #endregion

        #region Fields

        private readonly TileStresstest _tileStresstest = new TileStresstest();

        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;

        private DistributedTestMode _distributedTestMode;
        private bool _exclamation;
        private StresstestStatus _stresstestStatus;
        private Stopwatch _timeSinceStartRun = new Stopwatch(); //Decreases the 'jumpyness' of the progresscharts.

        #endregion

        #region Properties

        public TileStresstest TileStresstest { get { return _tileStresstest; } }
        public StresstestStatus StresstestResult { get { return _stresstestStatus; } }
        public StresstestStatus StresstestStatus { get { return _stresstestStatus; } }

        /// <summary>
        ///     true if the test can't start.
        /// </summary>
        public bool Exclamation { get { return _exclamation; } }

        #endregion

        #region Constructors

        public TileStresstestTreeViewItem() { InitializeComponent(); }

        public TileStresstestTreeViewItem(TileStresstest tileStresstest)
            : this() {
            _tileStresstest = tileStresstest;
            RefreshGui();

            chk.CheckedChanged -= chk_CheckedChanged;
            chk.Checked = _tileStresstest.Use;
            chk.CheckedChanged += chk_CheckedChanged;

            //Use if the parent is used explicitely.
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;

            eventProgressChart.BeginOfTimeFrame = DateTime.MinValue;
        }

        #endregion

        #region Functions

        public void Unfocus() {
            BackColor = Color.Transparent;
            SetVisibleControls();
        }

        public void SetVisibleControls(bool visible) {
            if (_distributedTestMode == DistributedTestMode.Edit) {
                picDuplicate.Visible = picDelete.Visible = visible;
                CheckIfTestCanStart();
            }
        }

        public void SetVisibleControls() {
            if (IsDisposed) return;

            if (BackColor == SystemColors.Control) SetVisibleControls(true);
            else SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }

        public void RefreshGui() {
            string label = _tileStresstest.Index + ") " +
                           ((_tileStresstest.BasicTileStresstest.Connection == null ||
                             _tileStresstest.BasicTileStresstest.Connection.IsEmpty) ? string.Empty : _tileStresstest.BasicTileStresstest.Connection.ToString());

            if (_tileStresstest.Use != chk.Checked) {
                chk.CheckedChanged -= chk_CheckedChanged;
                chk.Checked = _tileStresstest.Use;
                chk.CheckedChanged += chk_CheckedChanged;
            }

            if (lblTileStresstest.Text != label)
                lblTileStresstest.Text = label;
        }

        public void SetDistributedTestMode(DistributedTestMode distributedTestMode) {
            _distributedTestMode = distributedTestMode;
            if (_distributedTestMode == DistributedTestMode.Edit) {
                if (_tileStresstest.Use) chk.Visible = true; else Visible = true;
                SetStresstestStatus(_stresstestStatus);
            } else {
                if (_tileStresstest.Use) {
                    chk.Visible = picDelete.Visible = picDuplicate.Visible = false;

                    eventProgressChart.BeginOfTimeFrame = DateTime.MinValue;
                    eventProgressChart.Visible = true;

                    picStresstestStatus.Image = null;
                    toolTip.SetToolTip(picStresstestStatus, string.Empty);
                } else Visible = false;
            }
        }
        /// <summary>
        /// Only call this if the tile stresstest has monitors.
        /// </summary>
        public void SetMonitoringBeforeAfter() {
            picStresstestStatus.Image = Resources.Busy;
            toolTip.SetToolTip(picStresstestStatus, "Busy Monitoring");
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _tileStresstest.Parent &&
                e.__DoneAction == SolutionComponentChangedEventArgs.DoneAction.Edited) {
                var parent = sender as Tile;
                _tileStresstest.Use = parent.Use;
                if (chk.Checked != _tileStresstest.Use) {
                    chk.CheckedChanged -= chk_CheckedChanged;
                    chk.Checked = _tileStresstest.Use;
                    chk.CheckedChanged += chk_CheckedChanged;

                    CheckIfTestCanStart();
                }
            } else if (sender == _tileStresstest.BasicTileStresstest || sender == _tileStresstest.AdvancedTileStresstest) {
                CheckIfTestCanStart();
            }
        }

        private void _Enter(object sender, EventArgs e) {
            BackColor = SystemColors.Control;
            SetVisibleControls();

            if (AfterSelect != null) AfterSelect(this, null);
        }

        private void _DoubleClick(object sender, EventArgs e) {
            if (DoubleClicked != null) DoubleClicked.Invoke(this, null);
        }

        private void _MouseEnter(object sender, EventArgs e) { SetVisibleControls(); }

        private void _MouseLeave(object sender, EventArgs e) { SetVisibleControls(); }

        private void _KeyUp(object sender, KeyEventArgs e) {
            if (_distributedTestMode == DistributedTestMode.Test) {
                _ctrl = false;
                return;
            }

            if (e.KeyCode == Keys.ControlKey) _ctrl = false;
            else if (_ctrl)
                if (e.KeyCode == Keys.R && DeleteClicked != null) DeleteClicked(this, null);
                else if (e.KeyCode == Keys.D && DuplicateClicked != null) DuplicateClicked(this, null);
                else if (e.KeyCode == Keys.U) chk.Checked = !chk.Checked;
        }

        private void _KeyDown(object sender, KeyEventArgs e) {
            if (_distributedTestMode == DistributedTestMode.Test) return;

            if (e.KeyCode == Keys.ControlKey) _ctrl = true;
        }

        private void picDuplicate_Click(object sender, EventArgs e) {
            if (DuplicateClicked != null) DuplicateClicked(this, null);
        }

        private void picDelete_Click(object sender, EventArgs e) {
            if (DeleteClicked != null) DeleteClicked(this, null);
        }

        private void chk_CheckedChanged(object sender, EventArgs e) {
            _tileStresstest._canDefaultAdvancedSettingsTo = false;
            _tileStresstest.Use = chk.Checked;
            CheckIfTestCanStart();
            _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            _tileStresstest._canDefaultAdvancedSettingsTo = false;
        }

        private void eventProgressChart_EventClick(object sender, EventProgressChart.ProgressEventEventArgs e) {
            _Enter(this, e);
            if (EventClicked != null) EventClicked(this, e);
        }

        public void ClearEvents() { eventProgressChart.ClearEvents(); }

        public void SetEvents(List<EventPanelEvent> events) {
            ClearEvents();
            foreach (EventPanelEvent epe in events)
                eventProgressChart.AddEvent(epe.EventProgressBarEventColor, epe.Message, epe.At, false);

            eventProgressChart.Invalidate();

            EventPanelEvent lastEpe = new EventPanelEvent();
            lastEpe.Message = string.Empty;
            if (events.Count != 0) lastEpe = events[events.Count - 1];
            if (lastEpe.Message.Contains("|----> |Run") && !lastEpe.Message.Contains("Finished")) _timeSinceStartRun = Stopwatch.StartNew();
        }

        public void SetStresstestStarted(DateTime start) { eventProgressChart.BeginOfTimeFrame = start; }

        public void SetEstimatedRunTimeLeft(TimeSpan measuredRunTime, TimeSpan estimatedRuntimeLeft) {
            if (_timeSinceStartRun.ElapsedMilliseconds >= 1000L)
                eventProgressChart.SetEndOfTimeFrameTo(eventProgressChart.BeginOfTimeFrame + measuredRunTime + estimatedRuntimeLeft);
        }

        public void SetStresstestStatus(StresstestStatus stresstestStatus) {
            _stresstestStatus = stresstestStatus;
            eventProgressChart.SetEndOfTimeFrameToNow();

            switch (_stresstestStatus) {
                case StresstestStatus.Ok:
                    picStresstestStatus.Image = Resources.OK;
                    toolTip.SetToolTip(picStresstestStatus, "Finished");
                    break;
                case StresstestStatus.Cancelled:
                    picStresstestStatus.Image = Resources.Cancelled;
                    toolTip.SetToolTip(picStresstestStatus, "Cancelled");
                    break;
                case StresstestStatus.Error:
                    picStresstestStatus.Image = Resources.Error;
                    toolTip.SetToolTip(picStresstestStatus, "Failed");
                    break;
            }
        }

        private void CheckIfTestCanStart() {
            var sb = new StringBuilder();
            if (_tileStresstest.Use) {
                if (_tileStresstest.BasicTileStresstest.Connection.IsEmpty) sb.AppendLine("The connection is not filled in.");
                if (_tileStresstest.BasicTileStresstest.Slaves.Length == 0) {
                    sb.AppendLine("No slave has been selected.");
                } else {
                    //Check of the slave is not already chosen in another tilestresstest.
                    var distributedTest = _tileStresstest.Parent.GetParent().GetParent() as DistributedTest;
                    if (distributedTest != null)
                        foreach (Tile tile in distributedTest.Tiles)
                            if (tile.Use)
                                foreach (TileStresstest tileStresstest in tile)
                                    if (SlaveUsedElsewhere(tileStresstest)) {
                                        sb.AppendLine("One or more selected slaves are already chosen in another tile stresstest.");
                                        break;
                                    }
                }
                if (_tileStresstest.AdvancedTileStresstest.Logs.Length == 0) sb.AppendLine("No log has been selected. [Advanced Settings]");
            }

            string exclamation = sb.ToString();

            if (exclamation.Length != 0) {
                if (toolTip.GetToolTip(lblExclamation) != exclamation) toolTip.SetToolTip(lblExclamation, exclamation);
                _exclamation = lblExclamation.Visible = true;
            } else {
                _exclamation = lblExclamation.Visible = false;
            }
        }
        private bool SlaveUsedElsewhere(TileStresstest elsewhere) {
            if (elsewhere.Use && elsewhere != _tileStresstest) {
                var slavesA = _tileStresstest.BasicTileStresstest.SlaveIndices;
                var slavesB = elsewhere.BasicTileStresstest.SlaveIndices;
                foreach (int slaveA in slavesA)
                    if (slavesB.Contains(slaveA))
                        return true;
            }
            return false;
        }
        #endregion
    }
}