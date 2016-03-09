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
using vApus.DistributedTest.Properties;
using vApus.SolutionTree;
using vApus.StressTest;
using vApus.Util;

namespace vApus.DistributedTest {
    [ToolboxItem(false)]
    public partial class TileStressTestTreeViewItem : UserControl, ITreeViewItem {

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

        private readonly TileStressTest _tileStressTest = new TileStressTest();

        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;

        private DistributedTestMode _distributedTestMode;
        private bool _exclamation;
        private StressTestStatus _stressTestStatus;
        private Stopwatch _timeSinceStartRun = new Stopwatch(); //Decreases the 'jumpyness' of the progresscharts.

        #endregion

        #region Properties

        public TileStressTest TileStressTest { get { return _tileStressTest; } }
        public StressTestStatus StressTestResult { get { return _stressTestStatus; } }
        public StressTestStatus StressTestStatus { get { return _stressTestStatus; } }

        /// <summary>
        ///     true if the test can't start.
        /// </summary>
        public bool Exclamation { get { return _exclamation; } }

        #endregion

        #region Constructors

        public TileStressTestTreeViewItem() { InitializeComponent(); }

        public TileStressTestTreeViewItem(TileStressTest tileStressTest)
            : this() {
            _tileStressTest = tileStressTest;
            RefreshGui();

            chk.CheckedChanged -= chk_CheckedChanged;
            chk.Checked = _tileStressTest.Use;
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
            string label = _tileStressTest.Index + ") " +
                           ((_tileStressTest.BasicTileStressTest.Connection == null ||
                             _tileStressTest.BasicTileStressTest.Connection.IsEmpty) ? string.Empty : _tileStressTest.BasicTileStressTest.Connection.ToString());

            if (_tileStressTest.Use != chk.Checked) {
                chk.CheckedChanged -= chk_CheckedChanged;
                chk.Checked = _tileStressTest.Use;
                chk.CheckedChanged += chk_CheckedChanged;
            }

            if (lblTileStressTest.Text != label)
                lblTileStressTest.Text = label;
        }

        public void SetDistributedTestMode(DistributedTestMode distributedTestMode) {
            _distributedTestMode = distributedTestMode;
            if (_distributedTestMode == DistributedTestMode.Edit) {
                if (_tileStressTest.Use) chk.Visible = true; else Visible = true;
                SetStressTestStatus(_stressTestStatus);
            } else {
                if (_tileStressTest.Use) {
                    chk.Visible = picDelete.Visible = picDuplicate.Visible = false;

                    eventProgressChart.BeginOfTimeFrame = DateTime.MinValue;
                    eventProgressChart.Visible = true;

                    picStressTestStatus.Image = null;
                    toolTip.SetToolTip(picStressTestStatus, string.Empty);
                } else Visible = false;
            }
        }
        /// <summary>
        /// Only call this if the tile stress test has monitors.
        /// </summary>
        public void SetMonitoringBeforeAfter() {
            picStressTestStatus.Image = Resources.Busy;
            toolTip.SetToolTip(picStressTestStatus, "Busy monitoring.");
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _tileStressTest.Parent &&
                e.__DoneAction == SolutionComponentChangedEventArgs.DoneAction.Edited) {
                var parent = sender as Tile;
                _tileStressTest.Use = parent.Use;
                if (chk.Checked != _tileStressTest.Use) {
                    chk.CheckedChanged -= chk_CheckedChanged;
                    chk.Checked = _tileStressTest.Use;
                    chk.CheckedChanged += chk_CheckedChanged;

                    CheckIfTestCanStart();
                }
            } else if (sender == _tileStressTest.BasicTileStressTest || sender == _tileStressTest.AdvancedTileStressTest) {
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
            _tileStressTest.Use = chk.Checked;
            CheckIfTestCanStart();
            _tileStressTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void eventProgressChart_EventClick(object sender, EventProgressChart.ProgressEventEventArgs e) {
            _Enter(this, e);
            if (EventClicked != null) EventClicked(this, e);
        }

        public void ClearEvents() { eventProgressChart.ClearEvents(); }

        public void SetEvents(EventPanelEvent[] events) {
            ClearEvents();
            foreach (EventPanelEvent epe in events)
                if (epe.EventType > EventViewEventType.Info)
                    eventProgressChart.AddEvent(epe.EventProgressBarEventColor, epe.Message, epe.At, false);

            eventProgressChart.Invalidate();

            EventPanelEvent lastEpe = new EventPanelEvent();
            lastEpe.Message = string.Empty;
            if (events.Length != 0) lastEpe = events[events.Length - 1];
            if (lastEpe.Message.Contains("|----> |Run") && !lastEpe.Message.Contains("Finished")) _timeSinceStartRun = Stopwatch.StartNew();
        }

        public void SetStressTestStarted(DateTime start) { eventProgressChart.BeginOfTimeFrame = start; }

        public void SetEstimatedRunTimeLeft(TimeSpan measuredRunTime, TimeSpan estimatedRuntimeLeft) {
            if (_timeSinceStartRun.ElapsedMilliseconds >= 1000L)
                eventProgressChart.SetEndOfTimeFrameTo(eventProgressChart.BeginOfTimeFrame + measuredRunTime + estimatedRuntimeLeft);
        }

        public void SetStressTestStatus(StressTestStatus stressTestStatus) {
            _stressTestStatus = stressTestStatus;

            switch (_stressTestStatus) {
                case StressTestStatus.Ok:
                    picStressTestStatus.Image = Resources.OK;
                    toolTip.SetToolTip(picStressTestStatus, "Finished.");
                    eventProgressChart.SetEndOfTimeFrameToNow();
                    break;
                case StressTestStatus.Cancelled:
                    picStressTestStatus.Image = Resources.Cancelled;
                    toolTip.SetToolTip(picStressTestStatus, "Cancelled.");
                    break;
                case StressTestStatus.Error:
                    picStressTestStatus.Image = Resources.Error;
                    toolTip.SetToolTip(picStressTestStatus, "Failed.");
                    break;
            }
        }

        private void CheckIfTestCanStart() {
            var sb = new StringBuilder();
            if (_tileStressTest.Use) {
                if (_tileStressTest.BasicTileStressTest.Connection.IsEmpty) sb.AppendLine("The connection is not filled in.");
                if (_tileStressTest.BasicTileStressTest.Slaves.Length == 0) {
                    sb.AppendLine("No slave has been selected.");
                } else {
                    //Check of the slave is not already chosen in another tile stress test.
                    var distributedTest = _tileStressTest.Parent.GetParent().GetParent() as DistributedTest;
                    if (distributedTest != null)
                        foreach (Tile tile in distributedTest.Tiles)
                            if (tile.Use)
                                foreach (TileStressTest tileStressTest in tile)
                                    if (SlaveUsedElsewhere(tileStressTest)) {
                                        sb.AppendLine("One or more selected slaves are already chosen in another tile stress test.");
                                        break;
                                    }
                }
                if (_tileStressTest.AdvancedTileStressTest.Scenarios.Length == 0) sb.AppendLine("No scenario has been selected. [Advanced settings]");
            }

            string exclamation = sb.ToString();

            if (exclamation.Length != 0) {
                if (toolTip.GetToolTip(lblExclamation) != exclamation) toolTip.SetToolTip(lblExclamation, exclamation);
                _exclamation = lblExclamation.Visible = true;
            } else {
                _exclamation = lblExclamation.Visible = false;
            }
        }
        private bool SlaveUsedElsewhere(TileStressTest elsewhere) {
            if (elsewhere.Use && elsewhere != _tileStressTest) {
                var slavesA = _tileStressTest.BasicTileStressTest.SlaveIndices;
                var slavesB = elsewhere.BasicTileStressTest.SlaveIndices;
                foreach (int slaveA in slavesA)
                    if (slavesB.Contains(slaveA))
                        return true;
            }
            return false;
        }
        #endregion
    }
}