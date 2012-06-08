/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;
using System.Collections.Generic;

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
        #endregion

        #region Properties
        public TileStresstest TileStresstest
        {
            get { return _tileStresstest; }
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
                }
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
        private void txtTileStresstest_Leave(object sender, EventArgs e)
        {
            txtTileStresstest.Text = lblTileStresstest.Text = txtTileStresstest.Text.Trim();
            if (_tileStresstest.Label != txtTileStresstest.Text)
            {
                _tileStresstest.Label = txtTileStresstest.Text;
                _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
            lblTileStresstest.Text = _tileStresstest.Label;
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
                txtTileStresstest.Visible = picDuplicate.Visible = picDelete.Visible = visible;
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
            string label = string.Empty;
            if (_tileStresstest.Label == string.Empty)
                label = (_tileStresstest.BasicTileStresstest.Connection == null || _tileStresstest.BasicTileStresstest.Connection.IsEmpty) ?
                    string.Empty : _tileStresstest.BasicTileStresstest.Connection.ToString();
            else
                label = _tileStresstest.Label;

            if (lblTileStresstest.Text != label)
            {
                lblTileStresstest.Text = label;
                if (!txtTileStresstest.Focused)
                    txtTileStresstest.Text = label;
            }
        }

        private void _KeyUp(object sender, KeyEventArgs e)
        {
            if (_distributedTestMode == DistributedTestMode.TestAndReport)
            {
                _ctrl = false;
                return;
            }
            if (sender == txtTileStresstest)
                if (e.KeyCode == Keys.Enter && _tileStresstest.Label != txtTileStresstest.Text)
                {
                    txtTileStresstest.Text = txtTileStresstest.Text.Trim();
                    if (_tileStresstest.Label != txtTileStresstest.Text)
                    {
                        _tileStresstest.Label = txtTileStresstest.Text;
                        if (_tileStresstest.Label == string.Empty)
                            lblTileStresstest.Text = (_tileStresstest.BasicTileStresstest.Connection == null || _tileStresstest.BasicTileStresstest.Connection.IsEmpty) ? string.Empty : _tileStresstest.BasicTileStresstest.Connection.ToString();
                        else
                            lblTileStresstest.Text = _tileStresstest.Label;

                        _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                    }
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
            _tileStresstest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        public void SetDistributedTestMode(DistributedTestMode distributedTestMode)
        {
            _distributedTestMode = distributedTestMode;
            if (_distributedTestMode == DistributedTestMode.Edit)
                if (_tileStresstest.Use)
                {
                    chk.Visible =
                    txtTileStresstest.Visible =
                    picDelete.Visible =
                    picDuplicate.Visible = true;
                }
                else
                {
                    this.Visible = true;
                }
            else
                if (_tileStresstest.Use)
                {
                    chk.Visible =
                    txtTileStresstest.Visible =
                    picDelete.Visible =
                    picDuplicate.Visible = false;

                    eventProgressBar.BeginOfTimeFrame = DateTime.MinValue;
                    eventProgressBar.EndOfTimeFrame = DateTime.MaxValue;
                }
                else
                {
                    this.Visible = false;
                }
        }
        #endregion

        private void picStresstestStatus_Click(object sender, EventArgs e)
        {

        }

        private void eventProgressBar_EventClick(object sender, EventProgressBar.ProgressEventEventArgs e)
        {
            this.Select();
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

        internal void SetStresstestStarted(DateTime start)
        {
            eventProgressBar.BeginOfTimeFrame = start;
        }

        internal void SetMeasuredRunTime(TimeSpan estimatedRuntimeLeft)
        {
            eventProgressBar.EndOfTimeFrame = DateTime.Now + estimatedRuntimeLeft;
            eventProgressBar.SetProgressBarToNow();
        }
    }
}
