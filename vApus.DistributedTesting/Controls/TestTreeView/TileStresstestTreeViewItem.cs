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
using System.Drawing;
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

        #endregion

        #region Properties

        public TileStresstest TileStresstest { get { return _tileStresstest; } }

        public StresstestStatus StresstestResult { get { return _stresstestStatus; } }

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
                             _tileStresstest.BasicTileStresstest.Connection.IsEmpty)
                                ? string.Empty
                                : _tileStresstest.BasicTileStresstest.Connection.ToString());

            if (lblTileStresstest.Text != label)
                lblTileStresstest.Text = label;
        }

        public void SetDistributedTestMode(DistributedTestMode distributedTestMode) {
            _distributedTestMode = distributedTestMode;
            if (_distributedTestMode == DistributedTestMode.Edit) {
                if (_tileStresstest.Use) chk.Visible = true; else Visible = true;
            }
            else {
                if (_tileStresstest.Use) {
                    chk.Visible = picDelete.Visible = picDuplicate.Visible = false;

                    eventProgressChart.BeginOfTimeFrame = DateTime.MinValue;

                    picStresstestStatus.Image = null;
                    toolTip.SetToolTip(picStresstestStatus, string.Empty);
                }
                else Visible = false;
            }
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
            }
            else if (sender == _tileStresstest.BasicTileStresstest || sender == _tileStresstest.AdvancedTileStresstest) {
                CheckIfTestCanStart();
            }
        }

        private void _Enter(object sender, EventArgs e) {
            BackColor = SystemColors.Control;
            SetVisibleControls();

            if (AfterSelect != null) AfterSelect(this, null);
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

        private void eventProgressBar_EventClick(object sender, EventProgressChart.ProgressEventEventArgs e) {
            _Enter(this, e);
            if (EventClicked != null) EventClicked(this, e);
        }

        public void ClearEvents() { eventProgressChart.ClearEvents(); }

        public void SetEvents(List<EventPanelEvent> events) {
            ClearEvents();
            foreach (EventPanelEvent epe in events)
                eventProgressChart.AddEvent(epe.EventProgressBarEventColor, epe.Message, epe.At);
        }

        public void SetStresstestStarted(DateTime start) { eventProgressChart.BeginOfTimeFrame = start; }

        public void SetEstimatedRunTimeLeft(TimeSpan estimatedRuntimeLeft) {
            eventProgressChart.SetEndOfTimeFrameTo(eventProgressChart.BeginOfTimeFrame + estimatedRuntimeLeft);
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
                if (_tileStresstest.BasicTileStresstest.Slaves.Length == 0) sb.AppendLine("No slaves have been assigned.");
                if (_tileStresstest.AdvancedTileStresstest.Log.IsEmpty) sb.AppendLine("The log is not filled in. [Advanced Settings]");
            }

            string exclamation = sb.ToString();

            if (exclamation.Length != 0) {
                if (toolTip.GetToolTip(lblExclamation) != exclamation) toolTip.SetToolTip(lblExclamation, exclamation);
                _exclamation = lblExclamation.Visible = true;
            }
            else {
                _exclamation = lblExclamation.Visible = false;
            }
        }

        #endregion
    }
}