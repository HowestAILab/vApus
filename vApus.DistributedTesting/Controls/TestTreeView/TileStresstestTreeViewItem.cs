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
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;
using System.Text;

namespace vApus.DistributedTesting
{
    [ToolboxItem(false)]
    public partial class TileStresstestTreeViewItem : UserControl, ITreeViewItem
    {
        #region Events
        /// <summary>
        /// Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;
        public event EventHandler DuplicateClicked;
        public event EventHandler DeleteClicked;

        /// <summary>
        /// Event of the test clicked.
        /// </summary>
        public event EventHandler<EventProgressBar.ProgressEventEventArgs> EventClicked;
        #endregion

        #region Fields
        private TileStresstest _tileStresstest = new TileStresstest();
        /// <summary>
        /// Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;

        private DistributedTestMode _distributedTestMode;

        private StresstestResult _stresstestResult;
        private bool _downloadResultsFinished;

        private bool _exclemation;
        #endregion

        #region Properties
        public TileStresstest TileStresstest
        {
            get { return _tileStresstest; }
        }
        public StresstestResult StresstestResult
        {
            get { return _stresstestResult; }
        }
        /// <summary>
        /// true if the test can't start.
        /// </summary>
        public bool Exclemation
        {
            get { return _exclemation; }
        }
        #endregion

        #region Constructors
        public TileStresstestTreeViewItem()
        {
            InitializeComponent();
        }

        public TileStresstestTreeViewItem(TileStresstest tileStresstest)
            : this()
        {
            _tileStresstest = tileStresstest;
            RefreshGui();

            chk.CheckedChanged -= chk_CheckedChanged;
            chk.Checked = _tileStresstest.Use;
            chk.CheckedChanged += chk_CheckedChanged;

            //Use if the parent is used explicitely.
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);

            eventProgressBar.BeginOfTimeFrame = DateTime.MinValue;
            eventProgressBar.EndOfTimeFrame = DateTime.MaxValue;

            CheckIfTestCanStart();
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            if (sender == _tileStresstest.Parent && e.__DoneAction == SolutionComponentChangedEventArgs.DoneAction.Edited)
            {
                Tile parent = sender as Tile;
                _tileStresstest.Use = parent.Use;
                if (chk.Checked != _tileStresstest.Use)
                {
                    chk.CheckedChanged -= chk_CheckedChanged;
                    chk.Checked = _tileStresstest.Use;
                    chk.CheckedChanged += chk_CheckedChanged;

                    CheckIfTestCanStart();
                }
            }
            else if (sender == _tileStresstest.BasicTileStresstest || sender == _tileStresstest.AdvancedTileStresstest)
            {
                CheckIfTestCanStart();
            }
        }

        private void _Enter(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.Control;
            SetVisibleControls();

            if (AfterSelect != null)
                AfterSelect(this, null);
        }
        public void Unfocus()
        {
            this.BackColor = Color.Transparent;
            SetVisibleControls();
        }

        private void _MouseEnter(object sender, EventArgs e)
        {
            SetVisibleControls();
        }
        private void _MouseLeave(object sender, EventArgs e)
        {
            SetVisibleControls();
        }
        public void SetVisibleControls(bool visible)
        {
            if (_distributedTestMode == DistributedTestMode.Edit)
            {
                picDuplicate.Visible = picDelete.Visible = visible;
                CheckIfTestCanStart();
            }
        }

        public void SetVisibleControls()
        {
            if (this.IsDisposed)
                return;

            if (this.BackColor == SystemColors.Control)
                SetVisibleControls(true);
            else
                SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }

        public void RefreshGui()
        {
            string label = _tileStresstest.Index + ") " +
                ((_tileStresstest.BasicTileStresstest.Connection == null || _tileStresstest.BasicTileStresstest.Connection.IsEmpty) ?
                    string.Empty : _tileStresstest.BasicTileStresstest.Connection.ToString());

            if (lblTileStresstest.Text != label)
                lblTileStresstest.Text = label;
        }

        private void _KeyUp(object sender, KeyEventArgs e)
        {
            if (_distributedTestMode == DistributedTestMode.TestAndReport)
            {
                _ctrl = false;
                return;
            }

            if (e.KeyCode == Keys.ControlKey)
                _ctrl = false;
            else if (_ctrl)
                if (e.KeyCode == Keys.R && DeleteClicked != null)
                    DeleteClicked(this, null);
                else if (e.KeyCode == Keys.D && DuplicateClicked != null)
                    DuplicateClicked(this, null);
                else if (e.KeyCode == Keys.U)
                    chk.Checked = !chk.Checked;
        }
        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (_distributedTestMode == DistributedTestMode.TestAndReport)
                return;

            if (e.KeyCode == Keys.ControlKey)
                _ctrl = true;
        }
        private void picDuplicate_Click(object sender, EventArgs e)
        {
            if (DuplicateClicked != null)
                DuplicateClicked(this, null);
        }
        private void picDelete_Click(object sender, EventArgs e)
        {
            if (DeleteClicked != null)
                DeleteClicked(this, null);
        }
        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            _tileStresstest.Use = chk.Checked;
            CheckIfTestCanStart();
            _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        public void SetDistributedTestMode(DistributedTestMode distributedTestMode)
        {
            _distributedTestMode = distributedTestMode;
            if (_distributedTestMode == DistributedTestMode.Edit)
            {
                if (_tileStresstest.Use)
                    chk.Visible = true;
                else
                    this.Visible = true;
            }
            else
            {
                if (_tileStresstest.Use)
                {
                    chk.Visible =
                    picDelete.Visible =
                    picDuplicate.Visible = false;

                    eventProgressBar.BeginOfTimeFrame = DateTime.MinValue;
                    eventProgressBar.EndOfTimeFrame = DateTime.MaxValue;

                    picStresstestStatus.Image = null;
                    toolTip.SetToolTip(picStresstestStatus, string.Empty);

                    _downloadResultsFinished = false;
                }
                else
                {
                    this.Visible = false;
                }
            }
        }

        private void eventProgressBar_EventClick(object sender, EventProgressBar.ProgressEventEventArgs e)
        {
            _Enter(this, e);
            if (EventClicked != null)
                EventClicked(this, e);
        }
        public void ClearEvents()
        {
            eventProgressBar.ClearEvents();
        }
        public void SetEvents(List<EventPanelEvent> events)
        {
            ClearEvents();
            foreach (var epe in events)
                eventProgressBar.AddEvent(epe.EventProgressBarEventColor, epe.Message, epe.At);
        }
        public void SetStresstestStarted(DateTime start)
        {
            eventProgressBar.BeginOfTimeFrame = start;
        }
        public void SetMeasuredRunTime(TimeSpan estimatedRuntimeLeft)
        {
            eventProgressBar.EndOfTimeFrame = DateTime.Now + estimatedRuntimeLeft;
            eventProgressBar.SetProgressBarToNow();
        }

        public void SetStresstestResult(StresstestResult stresstestResult, int downloadResultsProgress)
        {
            _stresstestResult = stresstestResult;
            if (downloadResultsProgress == 100)
                _downloadResultsFinished = true;

            if (downloadResultsProgress == 0 || downloadResultsProgress == 100)
            {
                switch (stresstestResult)
                {
                    case StresstestResult.Ok:
                        picStresstestStatus.Image = vApus.DistributedTesting.Properties.Resources.OK;
                        toolTip.SetToolTip(picStresstestStatus, "Finished");
                        break;
                    case StresstestResult.Cancelled:
                        picStresstestStatus.Image = vApus.DistributedTesting.Properties.Resources.Cancelled;
                        toolTip.SetToolTip(picStresstestStatus, "Cancelled");
                        break;
                    case StresstestResult.Error:
                        picStresstestStatus.Image = vApus.DistributedTesting.Properties.Resources.Error;
                        toolTip.SetToolTip(picStresstestStatus, "Failed");
                        break;
                }
            }
            else if (!_downloadResultsFinished)
            {
                picStresstestStatus.Image = vApus.DistributedTesting.Properties.Resources.Busy;
                toolTip.SetToolTip(picStresstestStatus, "Downloading Results " + downloadResultsProgress + "%");
            }
        }

        private void CheckIfTestCanStart()
        {
            StringBuilder sb = new StringBuilder();
            if (_tileStresstest.Use)
            {
                if (_tileStresstest.BasicTileStresstest.Connection.IsEmpty)
                    sb.AppendLine("The connection is not filled in.");
                if (_tileStresstest.BasicTileStresstest.Slaves.Length == 0)
                    sb.AppendLine("No slaves have been assigned.");
                if (_tileStresstest.AdvancedTileStresstest.Log.IsEmpty)
                    sb.AppendLine("The log is not filled in. [Advanced Settings]");
            }

            string exclemation = sb.ToString();

            if (exclemation.Length != 0)
            {
                if (toolTip.GetToolTip(lblExclemation) != exclemation)
                    toolTip.SetToolTip(lblExclemation, exclemation);
                _exclemation = lblExclemation.Visible = true;
            }
            else
            {
                _exclemation = lblExclemation.Visible = false;
            }
        }
        #endregion
    }
}
