/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public partial class TestTreeView : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        /// <summary>
        /// The selected item is the sender
        /// </summary>
        public event EventHandler AfterSelect;

        #region Fields
        private DistributedTestMode _distributedTestMode;
        #endregion

        #region Properties
        public DistributedTestMode DistributedTestMode
        {
            get { return _distributedTestMode; }
        }
        public IEnumerable<ITreeViewItem> Items
        {
            get
            {
                foreach (ITreeViewItem control in largeList.AllControls)
                    yield return control;
            }
        }
        #endregion

        #region Constructors
        public TestTreeView()
        {
            InitializeComponent();
            // TestGui();
        }
        #endregion

        #region Functions
        public void TestGui()
        {

            DistributedTest test = new DistributedTest();
            test.ClearWithoutInvokingEvent();

            Tile tile = new Tile();
            tile.ClearWithoutInvokingEvent();
            test.AddWithoutInvokingEvent(tile);

            TileStresstest ts = new TileStresstest();
            ts.Label = "Benchdb";
            tile.AddWithoutInvokingEvent(ts);
            tile.AddWithoutInvokingEvent(new TileStresstest());
            tile.AddWithoutInvokingEvent(new TileStresstest());

            tile = new Tile();
            tile.ClearWithoutInvokingEvent();
            test.AddWithoutInvokingEvent(tile);

            ts = new TileStresstest();
            ts.Label = "Benchdb";
            tile.AddWithoutInvokingEvent(ts);
            tile.AddWithoutInvokingEvent(new TileStresstest());

            SetDistributedTest(test);
        }

        public void SetMode(DistributedTestMode distributedTestMode)
        {
            if (_distributedTestMode != distributedTestMode)
            {
                _distributedTestMode = distributedTestMode;
                foreach (ITreeViewItem item in largeList.AllControls)
                    item.SetDistributedTestMode(_distributedTestMode);
            }
        }

        public void SetDistributedTest(DistributedTest distributedTest)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            largeList.Clear();
            var dttvi = new DistributedTestTreeViewItem(distributedTest);
            dttvi.AfterSelect += new EventHandler(_AfterSelect);
            dttvi.AddTileClicked += new EventHandler(dttvi_AddTileClicked);
            largeList.Add(dttvi);

            foreach (Tile tile in distributedTest.Tiles)
                CreateAndAddTileTreeViewItem(tile);

            SetGui();

            //select the first stresstest tvi if any
            bool selected = false;
            foreach(Control control in largeList.AllControls)
                if (control is TileStresstestTreeViewItem)
                {
                    control.Select();
                    selected = true;
                    break;
                }

            if (!selected)
                dttvi.Select();
            
            LockWindowUpdate(0);
        }

        private void dttvi_AddTileClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            DistributedTestTreeViewItem dttvi = sender as DistributedTestTreeViewItem;

            Tile tile = new Tile();
            CreateAndAddTileTreeViewItem(tile);

            dttvi.DistributedTest.Tiles.Add(tile);

            dttvi.DistributedTest.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Added, true);
            LockWindowUpdate(0);
        }

        private void CreateAndAddTileTreeViewItem(Tile tile)
        {
            var tvi = new TileTreeViewItem(tile);
            //Used for handling collapsing and expanding.
            tvi.SetParent(largeList);
            tvi.AfterSelect += new EventHandler(_AfterSelect);
            tvi.AddTileStresstestClicked += new EventHandler(tvi_AddTileStresstestClicked);
            tvi.DuplicateClicked += new EventHandler(tvi_DuplicateClicked);
            tvi.DeleteClicked += new EventHandler(tvi_DeleteClicked);

            largeList.Add(tvi);
            foreach (TileStresstest tileStresstest in tile)
            {
                var tsvi = CreateTileStresstestTreeViewItem(tvi, tileStresstest);
                tvi.ChildControls.Add(tsvi);
                largeList.Add(tsvi);
            }
        }
        private void CreateAndInsertTileTreeViewItem(Tile tile, KeyValuePair<int,int> index)
        {
            var tvi = new TileTreeViewItem(tile);
            //Used for handling collapsing and expanding.
            tvi.SetParent(largeList);
            tvi.AfterSelect += new EventHandler(_AfterSelect);
            tvi.AddTileStresstestClicked += new EventHandler(tvi_AddTileStresstestClicked);
            tvi.DuplicateClicked += new EventHandler(tvi_DuplicateClicked);
            tvi.DeleteClicked += new EventHandler(tvi_DeleteClicked);

            for(int i = tile.Count - 1; i != -1; i--)
            {
                var tsvi = CreateTileStresstestTreeViewItem(tvi, tile[i] as TileStresstest);
                tvi.ChildControls.Add(tsvi);
                largeList.Insert(tsvi, index, false);
            }
            largeList.Insert(tvi, index);

            tvi.Select();
        }
        private void tvi_AddTileStresstestClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            TileTreeViewItem tvi = sender as TileTreeViewItem;

            TileStresstest ts = new TileStresstest();
            var tsvi = CreateTileStresstestTreeViewItem(tvi, ts);
            tvi.ChildControls.Add(tsvi);

            TileTreeViewItem closestNextTileTreeViewItem = GetClosestNextTileTreeViewItem(tvi);
            if (closestNextTileTreeViewItem == null)
                largeList.Add(tsvi);
            else
                largeList.Insert(tsvi, largeList.IndexOf(closestNextTileTreeViewItem));


            tvi.Tile.Add(ts);
            tvi.RefreshGui();

            tvi.Tile.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Added, true);
           
            LockWindowUpdate(0);
        }
        private TileTreeViewItem GetClosestNextTileTreeViewItem(Control control)
        {
            KeyValuePair<int, int> index = largeList.IndexOf(control);
            for (int i = index.Key; i < largeList.ViewCount; i++)
                if (i == index.Key)
                {
                    for (int j = index.Value + 1; j < largeList[i].Count; j++)
                        if (largeList[i][j] is TileTreeViewItem)
                            return largeList[i][j] as TileTreeViewItem;
                }
                else
                {
                    for (int j = 0; j < largeList[i].Count; j++)
                        if (largeList[i][j] is TileTreeViewItem)
                            return largeList[i][j] as TileTreeViewItem;
                }
            return null;
        }
        private void tvi_DeleteClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            TileTreeViewItem tvi = sender as TileTreeViewItem;
            if (tvi.Tile.Parent != null)
                tvi.Tile.Parent.Remove(tvi.Tile);

            foreach (var child in tvi.ChildControls)
                largeList.Remove(child, false);

            largeList.Remove(tvi);

            largeList.Select();

            LockWindowUpdate(0);
        }
        private void tvi_DuplicateClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            TileTreeViewItem tvi = sender as TileTreeViewItem;
            if (tvi.Tile.Parent != null)
            {
                var clone = tvi.Tile.Clone();

                var parent = tvi.Tile.Parent as Tiles;
                parent.InsertWithoutInvokingEvent(parent.IndexOf(tvi.Tile), clone);
                
                CreateAndInsertTileTreeViewItem(clone, largeList.IndexOf(tvi));
                
                parent.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Added, true);
            }

            LockWindowUpdate(0);
        }

        private TileStresstestTreeViewItem CreateTileStresstestTreeViewItem(TileTreeViewItem parent, TileStresstest tileStresstest)
        {
            var tsvi = new TileStresstestTreeViewItem(tileStresstest);
            //To be able to delete the control.
            tsvi.SetParent(parent);
            tsvi.AfterSelect += new EventHandler(_AfterSelect);
            tsvi.DuplicateClicked += new EventHandler(tsvi_DuplicateClicked);
            tsvi.DeleteClicked += new EventHandler(tsvi_DeleteClicked);
            return tsvi;
        }
        private void tsvi_DeleteClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            TileStresstestTreeViewItem tsvi = sender as TileStresstestTreeViewItem;
            if (tsvi.GetParent() != null)
                (tsvi.GetParent() as TileTreeViewItem).ChildControls.Remove(tsvi);
            if (tsvi.TileStresstest.Parent != null)
                tsvi.TileStresstest.Parent.Remove(tsvi.TileStresstest);

            largeList.Remove(tsvi);

            largeList.Select();

            LockWindowUpdate(0);
        }
        private void tsvi_DuplicateClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            TileStresstestTreeViewItem tsvi = sender as TileStresstestTreeViewItem;
            if (tsvi.TileStresstest.Parent != null)
            {
                var clone = tsvi.TileStresstest.Clone();
                var parent = tsvi.TileStresstest.Parent as Tile;
                parent.InsertWithoutInvokingEvent(parent.IndexOf(tsvi.TileStresstest), clone);

                var cloneTsvi = CreateTileStresstestTreeViewItem(tsvi.GetParent() as TileTreeViewItem, clone);
                //Make it appear that the clone is the original one, it is the most simple thing to do and the user doesn't notice.
                largeList.Insert(cloneTsvi, largeList.IndexOf(tsvi));
                tsvi.Select();

                parent.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Added, true);
            }

            LockWindowUpdate(0);
        }

        private void _AfterSelect(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            foreach (ITreeViewItem item in largeList.AllControls)
                if (item != sender)
                    item.Unfocus();

            if (AfterSelect != null)
                AfterSelect(sender, null);

            LockWindowUpdate(0);
        }

        public void SetGui()
        {
            foreach (ITreeViewItem ctrl in largeList.AllControls)
            {
                ctrl.SetVisibleControls();
                //To determine what add tile stresstest control can be visible
                ctrl.RefreshGui();
            }
        }
        #endregion
    }
}
