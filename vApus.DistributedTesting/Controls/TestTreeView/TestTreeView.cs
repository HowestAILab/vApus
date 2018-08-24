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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTest {
    public partial class TestTreeView : UserControl {

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        /// <summary>
        ///     The selected item is the sender
        /// </summary>
        public event EventHandler AfterSelect;
        public event EventHandler TileStressTestTreeViewItemDoubleClicked;

        /// <summary>
        ///     Event of the test clicked.
        /// </summary>
        public event EventHandler<EventProgressChart.ProgressEventEventArgs> EventClicked;

        #region Fields

        private DistributedTestMode _distributedTestMode;

        #endregion

        #region Properties

        public DistributedTestMode DistributedTestMode {
            get { return _distributedTestMode; }
        }

        /// <summary>
        ///     get all tree view items.
        /// </summary>
        public IEnumerable<ITreeViewItem> Items {
            get {
                foreach (ITreeViewItem control in largeList.AllControls)
                    yield return control;
            }
        }
        /// <summary>
        ///     To check if the test can start (if false).
        /// </summary>
        public bool Exclamation {
            get {
                int usedCount = 0;
                foreach (Control ctrl in largeList.AllControls)
                    if (ctrl is TileStressTestTreeViewItem) {
                        var tstvi = ctrl as TileStressTestTreeViewItem;
                        if (tstvi.Exclamation)
                            return true;

                        if (tstvi.TileStressTest.Use)
                            ++usedCount;
                    }
                return usedCount == 0;
            }
        }
        #endregion

        #region Constructors

        public TestTreeView() {
            InitializeComponent();
            // TestGui();
        }

        #endregion

        #region Functions

        public void SetMode(DistributedTestMode distributedTestMode) {
            if (_distributedTestMode != distributedTestMode) {
                LockWindowUpdate(Handle);
                _distributedTestMode = distributedTestMode;
                foreach (ITreeViewItem item in largeList.AllControls)
                    item.SetDistributedTestMode(_distributedTestMode);
                LockWindowUpdate(IntPtr.Zero);

                largeList.RefreshControls();

                //Otherwise the gui freezes, stupid winforms.
                System.Timers.Timer tmr = new System.Timers.Timer(500);
                tmr.Elapsed += tmr_Elapsed;
                tmr.Start();
            }
        }
        public void SetMonitorBeforeCancelled() {
            foreach (ITreeViewItem item in largeList.AllControls)
                if (item is TileStressTestTreeViewItem) {
                    var ts = item as TileStressTestTreeViewItem;
                    if (ts.TileStressTest.Use && ts.StressTestStatus == StressTest.StressTestStatus.Busy)
                        ts.SetStressTestStatus(StressTest.StressTestStatus.Cancelled);
                }
        }
        /// <summary>
        /// This is automatically unset when the mode becomes Edit again. (SetMode fx)
        /// Use this in the monitor before function in the distributed test view.
        /// </summary>
        public void SetMonitoringBeforeAfter() {
            foreach (ITreeViewItem item in largeList.AllControls)
                if (item is DistributedTestTreeViewItem) {
                    var ds = item as DistributedTestTreeViewItem;
                    ds.SetMonitoringBeforeAfter();
                } else if (item is TileStressTestTreeViewItem) {
                    var ts = item as TileStressTestTreeViewItem;
                    if (ts.TileStressTest.Use && ts.TileStressTest.BasicTileStressTest.MonitorIndices.Length != 0)
                        ts.SetMonitoringBeforeAfter();
                }
        }

        private void tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            try {
                (sender as System.Timers.Timer).Stop();
                SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                    if (_distributedTestMode == DistributedTestMode.Test)
                        largeList[0][0].Focus();
                }, null);
            } catch {
                //Not important. Only on gui closed.
            }
        }

        public void SetDistributedTest(DistributedTest distributedTest) {
            if (IsDisposed) return;
            LockWindowUpdate(Handle);
            largeList.Clear();
            var dttvi = new DistributedTestTreeViewItem(distributedTest);
            dttvi.AfterSelect += _AfterSelect;
            dttvi.AddTileClicked += dttvi_AddTileClicked;
            largeList.Add(dttvi);

            foreach (Tile tile in distributedTest.Tiles) CreateAndAddTileTreeViewItem(tile);

            SetGui();

            //select the first stress test tvi if any
            //bool selected = false;
            //foreach (Control control in largeList.AllControls)
            //    if (control is TileStressTestTreeViewItem) {
            //        control.Select();
            //        selected = true;
            //        break;
            //    }

            //if (!selected) dttvi.Select();

            LockWindowUpdate(IntPtr.Zero);
        }

        public void SelectDistributedTestTreeViewItem() {
            foreach (Control ctrl in largeList.AllControls) {
                ctrl.Select();
                break;
            }
        }
        /// <summary>
        ///     Select a tile stress test tvi.
        /// </summary>
        /// <param name="tileStressTest"></param>
        public void SelectTileStressTest(TileStressTest tileStressTest) {
            foreach (Control ctrl in largeList.AllControls)
                if (ctrl is TileStressTestTreeViewItem)
                    if ((ctrl as TileStressTestTreeViewItem).TileStressTest == tileStressTest) {
                        ctrl.Select();
                        break;
                    }
        }

        private void dttvi_AddTileClicked(object sender, EventArgs e) {
            LockWindowUpdate(Handle);

            var dttvi = sender as DistributedTestTreeViewItem;

            var tile = new Tile();
            tile.AddWithoutInvokingEvent(new TileStressTest());

            dttvi.DistributedTest.Tiles.Add(tile);

            foreach (TileStressTest tileStressTest in tile) tileStressTest.SelectAvailableSlave();

            CreateAndAddTileTreeViewItem(tile);

            dttvi.DistributedTest.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);

            LockWindowUpdate(IntPtr.Zero);
        }

        private void CreateAndAddTileTreeViewItem(Tile tile) {
            var tvi = new TileTreeViewItem(tile);
            //Used for handling collapsing and expanding.
            tvi.SetParent(largeList);
            tvi.AfterSelect += _AfterSelect;
            tvi.AddTileStressTestClicked += tvi_AddTileStressTestClicked;
            tvi.DuplicateClicked += tvi_DuplicateClicked;
            tvi.DeleteClicked += tvi_DeleteClicked;

            largeList.Add(tvi, false);
            foreach (TileStressTest tileStressTest in tile) {
                TileStressTestTreeViewItem tsvi = CreateTileStressTestTreeViewItem(tvi, tileStressTest);
                tvi.ChildControls.Add(tsvi);
                largeList.Add(tsvi, false);
            }
            largeList.RefreshControls();
        }

        private void CreateAndInsertTileTreeViewItem(Tile tile, KeyValuePair<int, int> index) {
            var tvi = new TileTreeViewItem(tile);
            //Used for handling collapsing and expanding.
            tvi.SetParent(largeList);
            tvi.AfterSelect += _AfterSelect;
            tvi.AddTileStressTestClicked += tvi_AddTileStressTestClicked;
            tvi.DuplicateClicked += tvi_DuplicateClicked;
            tvi.DeleteClicked += tvi_DeleteClicked;

            //Just add if the index is invalid
            if (index.Key == -1) {
                largeList.Add(tvi, false);
                for (int i = 0; i != tile.Count; i++) {
                    TileStressTestTreeViewItem tsvi = CreateTileStressTestTreeViewItem(tvi, tile[i] as TileStressTest);
                    tvi.ChildControls.Add(tsvi);
                    largeList.Add(tsvi, false);
                }
            } else {
                for (int i = tile.Count - 1; i != -1; i--) {
                    TileStressTestTreeViewItem tsvi = CreateTileStressTestTreeViewItem(tvi, tile[i] as TileStressTest);
                    tvi.ChildControls.Add(tsvi);
                    largeList.Insert(tsvi, index, false);
                }
                largeList.Insert(tvi, index, false);
            }
            largeList.RefreshControls();
        }

        private void tvi_AddTileStressTestClicked(object sender, EventArgs e) {
            LockWindowUpdate(Handle);

            var tvi = sender as TileTreeViewItem;

            var ts = new TileStressTest();
            tvi.Tile.AddWithoutInvokingEvent(ts);

            ts.ForceDefaultTo();

            TileStressTestTreeViewItem tsvi = CreateTileStressTestTreeViewItem(tvi, ts);
            tvi.ChildControls.Add(tsvi);

            TileTreeViewItem closestNextTileTreeViewItem = GetClosestNextTileTreeViewItem(tvi);
            if (closestNextTileTreeViewItem == null)
                largeList.Add(tsvi);
            else
                largeList.Insert(tsvi, largeList.IndexOf(closestNextTileTreeViewItem));


            tvi.RefreshGui();
            ts.SelectAvailableSlave();

            tvi.Tile.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);

            LockWindowUpdate(IntPtr.Zero);
        }

        private TileTreeViewItem GetClosestNextTileTreeViewItem(Control control) {
            KeyValuePair<int, int> index = largeList.IndexOf(control);
            for (int i = index.Key; i < largeList.ViewCount; i++)
                if (i == index.Key) {
                    for (int j = index.Value + 1; j < largeList[i].Count; j++)
                        if (largeList[i][j] is TileTreeViewItem)
                            return largeList[i][j] as TileTreeViewItem;
                } else {
                    for (int j = 0; j < largeList[i].Count; j++)
                        if (largeList[i][j] is TileTreeViewItem)
                            return largeList[i][j] as TileTreeViewItem;
                }
            return null;
        }

        private void tvi_DeleteClicked(object sender, EventArgs e) {
            LockWindowUpdate(Handle);

            var tvi = sender as TileTreeViewItem;
            if (tvi.Tile.Parent != null)
                tvi.Tile.Parent.Remove(tvi.Tile);

            foreach (Control child in tvi.ChildControls)
                largeList.Remove(child, false);

            KeyValuePair<int, int> previousIndex = largeList.ParseFlatIndex(largeList.FlatIndexOf(tvi) - 1);
            largeList.Remove(tvi);

            largeList[previousIndex.Key][previousIndex.Value].Select();

            LockWindowUpdate(IntPtr.Zero);
        }

        private void tvi_DuplicateClicked(object sender, EventArgs e) {
            LockWindowUpdate(Handle);

            var tvi = sender as TileTreeViewItem;
            if (tvi.Tile.Parent != null) {
                Tile clone = tvi.Tile.Clone();
                var parent = tvi.Tile.Parent as Tiles;
                int cloneIndex = parent.IndexOf(tvi.Tile) + 1;

                if (parent.Count == cloneIndex)
                    parent.AddWithoutInvokingEvent(clone);
                else
                    parent.InsertWithoutInvokingEvent(cloneIndex, clone);

                TileTreeViewItem closestNextTileTreeViewItem = GetClosestNextTileTreeViewItem(tvi);
                KeyValuePair<int, int> cloneIndexForLargeList = closestNextTileTreeViewItem == null
                                                                    ? new KeyValuePair<int, int>(-1, -1)
                                                                    : largeList.IndexOf(closestNextTileTreeViewItem);
                CreateAndInsertTileTreeViewItem(clone, cloneIndexForLargeList);

                foreach (TileStressTest tileStressTest in clone) tileStressTest.SelectAvailableSlave();

                parent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);
            }

            LockWindowUpdate(IntPtr.Zero);
        }

        private TileStressTestTreeViewItem CreateTileStressTestTreeViewItem(TileTreeViewItem parent, TileStressTest tileStressTest) {
            var tsvi = new TileStressTestTreeViewItem(tileStressTest);
            //To be able to delete the control.
            tsvi.SetParent(parent);
            tsvi.AfterSelect += _AfterSelect;
            tsvi.DoubleClicked += tsvi_DoubleClicked;
            tsvi.DuplicateClicked += tsvi_DuplicateClicked;
            tsvi.DeleteClicked += tsvi_DeleteClicked;

            tsvi.EventClicked += tsvi_EventClicked;
            return tsvi;
        }

        private void tsvi_DoubleClicked(object sender, EventArgs e) {
            if (TileStressTestTreeViewItemDoubleClicked != null) TileStressTestTreeViewItemDoubleClicked.Invoke(this, null);
        }

        private void tsvi_DeleteClicked(object sender, EventArgs e) {
            LockWindowUpdate(Handle);

            var tsvi = sender as TileStressTestTreeViewItem;
            if (tsvi.GetParent() != null)
                (tsvi.GetParent() as TileTreeViewItem).ChildControls.Remove(tsvi);
            if (tsvi.TileStressTest.Parent != null)
                tsvi.TileStressTest.Parent.Remove(tsvi.TileStressTest);

            KeyValuePair<int, int> previousIndex = largeList.ParseFlatIndex(largeList.FlatIndexOf(tsvi) - 1);
            largeList.Remove(tsvi);

            largeList[previousIndex.Key][previousIndex.Value].Select();

            LockWindowUpdate(IntPtr.Zero);
        }

        private void tsvi_DuplicateClicked(object sender, EventArgs e) {
            LockWindowUpdate(Handle);

            var tsvi = sender as TileStressTestTreeViewItem;
            if (tsvi.TileStressTest.Parent != null) {
                //In memory
                TileStressTest clone = tsvi.TileStressTest.Clone();
                var parent = tsvi.TileStressTest.Parent as Tile;
                int cloneIndex = parent.IndexOf(tsvi.TileStressTest) + 1;

                if (parent.Count == cloneIndex)
                    parent.AddWithoutInvokingEvent(clone);
                else
                    parent.InsertWithoutInvokingEvent(cloneIndex, clone);

                //In Largelist
                TileStressTestTreeViewItem cloneTsvi =
                    CreateTileStressTestTreeViewItem(tsvi.GetParent() as TileTreeViewItem, clone);
                KeyValuePair<int, int> cloneIndexForLargeList =
                    largeList.ParseFlatIndex(largeList.ParseIndex(largeList.IndexOf(tsvi)) + 1);
                if (cloneIndexForLargeList.Key == -1)
                    largeList.Add(cloneTsvi);
                else
                    largeList.Insert(cloneTsvi, cloneIndexForLargeList);

                clone.SelectAvailableSlave();

                parent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);
            }

            LockWindowUpdate(IntPtr.Zero);
        }

        private void tsvi_EventClicked(object sender, EventProgressChart.ProgressEventEventArgs e) {
            if (EventClicked != null)
                EventClicked(sender, e);
        }

        private void _AfterSelect(object sender, EventArgs e) {
            LockWindowUpdate(Handle);

            foreach (ITreeViewItem item in largeList.AllControls)
                if (item != sender)
                    item.Unfocus();

            if (AfterSelect != null)
                AfterSelect(sender, null);

            LockWindowUpdate(IntPtr.Zero);
        }

        public void SetGui() {
            foreach (ITreeViewItem ctrl in largeList.AllControls) {
                ctrl.SetVisibleControls();
                //To determine what add tile stress test control can be visible
                ctrl.RefreshGui();
            }
        }

        #endregion
    }
}