/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.DistributedTest.Properties;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTest {
    [ToolboxItem(false)]
    public partial class TileTreeViewItem : UserControl, ITreeViewItem {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        #region Events

        /// <summary>
        ///     Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;

        public event EventHandler AddTileStressTestClicked;
        public event EventHandler DuplicateClicked;
        public event EventHandler DeleteClicked;

        #endregion

        #region Fields

        private readonly List<Control> _childControls = new List<Control>();
        private readonly Tile _tile = new Tile();
        private bool _collapsed;

        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;

        private DistributedTestMode _distributedTestMode;

        #endregion

        #region Properties

        public Tile Tile {
            get { return _tile; }
        }

        public bool Collapsed {
            get { return _collapsed; }
            set {
                if (_collapsed != value) {
                    _collapsed = value;
                    picCollapseExpand.Image = _collapsed ? Resources.Expand_small : Resources.Collapse_small;

                    //Cannot be null!
                    var largeList = this.GetParent() as LargeList;
                    LockWindowUpdate(largeList.Handle);

                    if (_collapsed) {
                        largeList.RemoveRange(new List<Control>(_childControls.ToArray()));
                    } else {
                        //Correcting on wich view the controls must be inserted.
                        KeyValuePair<int, int> index = largeList.IndexOf(this);
                        if (index.Value == largeList[index.Key].Count - 1)
                            index = new KeyValuePair<int, int>(index.Key + 1, 0);
                        else
                            index = new KeyValuePair<int, int>(index.Key, index.Value + 1);

                        if (index.Key == largeList.ViewCount || index.Value == largeList[index.Key].Count)
                            largeList.AddRange(_childControls);
                        else
                            largeList.InsertRange(_childControls, index);
                    }
                    LockWindowUpdate(IntPtr.Zero);
                }
            }
        }

        public List<Control> ChildControls {
            get { return _childControls; }
        }

        private int UsedTileStressTestCount {
            get {
                int count = 0;
                foreach (TileStressTest ts in _tile)
                    if (ts.Use) ++count;
                return count;
            }
        }

        #endregion

        #region Constructors

        public TileTreeViewItem() {
            InitializeComponent();
        }

        public TileTreeViewItem(Tile tile)
            : this() {
            _tile = tile;
            RefreshGui();

            chk.CheckedChanged -= chk_CheckedChanged;
            chk.Checked = _tile.Use;
            chk.CheckedChanged += chk_CheckedChanged;

            //To check if the use has changed of the child controls.
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

        #endregion

        #region Functions

        public void Unfocus() { BackColor = Color.Transparent; }

        public void SetVisibleControls(bool visible) {
            if (_distributedTestMode == DistributedTestMode.Edit)
                picAddTileStressTest.Visible = picDuplicate.Visible = picDelete.Visible = visible;
        }

        public void SetVisibleControls() {
            if (IsDisposed) return;

            if (BackColor == SystemColors.Control)
                SetVisibleControls(true);
            else
                SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        }

        public void RefreshGui() {
            string label = _tile + " (" + UsedTileStressTestCount + "/" + _tile.Count + ")";

            if (lblTile.Text != label)
                lblTile.Text = label;

            _tile.Use = UsedTileStressTestCount != 0;
            if (_tile.Use != chk.Checked) {
                chk.CheckedChanged -= chk_CheckedChanged;
                chk.Checked = _tile.Use;
                chk.CheckedChanged += chk_CheckedChanged;
            }
        }

        public void SetDistributedTestMode(DistributedTestMode distributedTestMode) {
            _distributedTestMode = distributedTestMode;
            if (_distributedTestMode == DistributedTestMode.Edit) {
                if (_tile.Use) chk.Visible = true; else Visible = true;
            } else {
                if (_tile.Use)
                    chk.Visible = picAddTileStressTest.Visible = picDelete.Visible = picDuplicate.Visible = false;
                else
                    Visible = false;
            }
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            //To set if the tile is used or not.
            if (sender is TileStressTest) {
                var tileStressTest = sender as TileStressTest;
                if (_tile.Contains(tileStressTest)) {
                    _tile.Use = false;
                    foreach (TileStressTest ts in _tile)
                        if (ts.Use) {
                            _tile.Use = true;
                            break;
                        }
                    if (chk.Checked != _tile.Use) {
                        chk.CheckedChanged -= chk_CheckedChanged;
                        chk.Checked = _tile.Use;
                        chk.CheckedChanged += chk_CheckedChanged;
                    }
                }
            }
        }

        private void _Enter(object sender, EventArgs e) {
            BackColor = SystemColors.Control;
            SetVisibleControls();

            if (AfterSelect != null)
                AfterSelect(this, null);
        }

        private void _MouseEnter(object sender, EventArgs e) {
            SetVisibleControls();
        }

        private void _MouseLeave(object sender, EventArgs e) {
            SetVisibleControls();
        }

        private void _KeyUp(object sender, KeyEventArgs e) {
            if (_distributedTestMode == DistributedTestMode.Test) {
                _ctrl = false;
                return;
            }

            if (e.KeyCode == Keys.ControlKey)
                _ctrl = false;
            else if (_ctrl)
                if (e.KeyCode == Keys.I && AddTileStressTestClicked != null)
                    AddTileStressTestClicked(this, null);
                else if (e.KeyCode == Keys.R && DeleteClicked != null)
                    DeleteClicked(this, null);
                else if (e.KeyCode == Keys.D && DuplicateClicked != null)
                    DuplicateClicked(this, null);
                else if (e.KeyCode == Keys.U)
                    chk.Checked = !chk.Checked;
        }

        private void _KeyDown(object sender, KeyEventArgs e) {
            if (_distributedTestMode == DistributedTestMode.Test)
                return;

            if (e.KeyCode == Keys.ControlKey)
                _ctrl = true;
        }

        private void picAddTileStressTest_Click(object sender, EventArgs e) {
            Focus();
            if (AddTileStressTestClicked != null)
                AddTileStressTestClicked(this, null);
        }

        private void picDuplicate_Click(object sender, EventArgs e) {
            if (DuplicateClicked != null)
                DuplicateClicked(this, null);
        }

        private void picDelete_Click(object sender, EventArgs e) {
            if (DeleteClicked != null)
                DeleteClicked(this, null);
        }

        private void picCollapseExpand_Click(object sender, EventArgs e) {
            Focus();
            Collapsed = !Collapsed;
        }

        private void chk_CheckedChanged(object sender, EventArgs e) {
            if (_tile.Use != chk.Checked) {
                _tile.Use = chk.Checked;
                foreach (TileStressTest ts in _tile)
                    ts.Use = _tile.Use;

                _tile.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }

        #endregion
    }
}