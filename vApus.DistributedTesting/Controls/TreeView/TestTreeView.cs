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

        public TestTreeView()
        {
            InitializeComponent();
            //TestGui();
        }

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
        public void SetDistributedTest(DistributedTest distributedTest)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            largeList.Clear();
            var dttvi = new DistributedTestTreeViewItem(distributedTest);
            largeList.Add(dttvi);

            bool addControlsVisible = ClientRectangle.Contains(PointToClient(Cursor.Position));
            foreach (Tile tile in distributedTest)
                AddAndCreateTileTreeViewItem(tile);

            largeList.Add(CreateAddTileTreeViewItem(dttvi));

            SetGui();
            LockWindowUpdate(0);
        }

        private void AddAndCreateTileTreeViewItem(Tile tile)
        {
            var tvi = new TileTreeViewItem(tile);
            //Used for handling collapsing and expanding.
            tvi.SetParent(largeList);
            tvi.DuplicateClicked += new EventHandler(tvi_DuplicateClicked);
            tvi.DeleteClicked += new EventHandler(tvi_DeleteClicked);

            largeList.Add(tvi);
            foreach (TileStresstest tileStresstest in tile)
            {
                var tsvi = CreateTileStresstestTreeViewItem(tvi, tileStresstest);
                tvi.ChildControls.Add(tsvi);
                largeList.Add(tsvi);
            }
            var atstvi = CreateAddTileStresstestTreeViewItem(tvi);
            tvi.ChildControls.Add(atstvi);
            largeList.Add(atstvi);
        }
        private void tvi_DeleteClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            TileTreeViewItem tvi = sender as TileTreeViewItem;
            largeList.Remove(tvi);
            largeList.RemoveRange(tvi.ChildControls);
            if (tvi.Tile.Parent != null)
                tvi.Tile.Parent.Remove(tvi.Tile);

            LockWindowUpdate(0);
        }
        private void tvi_DuplicateClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            TileTreeViewItem tvi = sender as TileTreeViewItem;
            if (tvi.Tile.Parent != null)
            {
                Tile clone = new Tile();
                //foreach(TileStresstest ts in tvi.Tile)
                //    clone.AddWithoutInvokingEvent(ts.

                tvi.Tile.Parent.Add(clone);
            }

            LockWindowUpdate(0);
        }

        private TileStresstestTreeViewItem CreateTileStresstestTreeViewItem(TileTreeViewItem parent, TileStresstest tileStresstest)
        {
            var tsvi = new TileStresstestTreeViewItem(tileStresstest);
            //To be able to delete the control.
            tsvi.SetParent(parent);
            tsvi.DuplicateClicked += new EventHandler(tsvi_DuplicateClicked);
            tsvi.DeleteClicked += new EventHandler(tsvi_DeleteClicked);
            return tsvi;
        }
        private void tsvi_DeleteClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            TileStresstestTreeViewItem tsvi = sender as TileStresstestTreeViewItem;
            largeList.Remove(tsvi);
            if (tsvi.GetParent() != null)
                (tsvi.GetParent() as TileTreeViewItem).ChildControls.Remove(tsvi);
            if (tsvi.TileStresstest.Parent != null)
                tsvi.TileStresstest.Parent.Remove(tsvi.TileStresstest);

            LockWindowUpdate(0);
        }
        private void tsvi_DuplicateClicked(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            TileStresstestTreeViewItem tsvi = sender as TileStresstestTreeViewItem;
            if (tsvi.TileStresstest.Parent != null)
            { }

            LockWindowUpdate(0);
        }

        private AddTileStresstestTreeViewItem CreateAddTileStresstestTreeViewItem(TileTreeViewItem tileTreeViewItem)
        {
            var atstvi = new AddTileStresstestTreeViewItem();
            //For adding to the right tile
            atstvi.SetParent(tileTreeViewItem);
            atstvi.AddClick += new EventHandler(atstvi_AddClick);
            return atstvi;
        }
        private void atstvi_AddClick(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            AddTileStresstestTreeViewItem atstvi = sender as AddTileStresstestTreeViewItem;
            if (atstvi.GetParent() != null)
            {
                var parent = atstvi.GetParent() as TileTreeViewItem;
                TileStresstest ts = new TileStresstest();
                var tsvi = CreateTileStresstestTreeViewItem(parent, ts);
                parent.ChildControls.Insert(parent.ChildControls.Count - 1, tsvi);

                largeList.Insert(tsvi, largeList.IndexOf(parent.ChildControls[parent.ChildControls.Count - 1]));

                parent.Tile.Add(ts);

                tsvi.Select();
            }
            LockWindowUpdate(0);
        }

        private AddTileTreeViewItem CreateAddTileTreeViewItem(DistributedTestTreeViewItem distributedTestTreeViewItem)
        {
            var attvi = new AddTileTreeViewItem();
            //For adding to the right test
            attvi.SetParent(distributedTestTreeViewItem);
            attvi.AddClick += new EventHandler(attvi_AddClick);
            return attvi;
        }
        private void attvi_AddClick(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            AddTileTreeViewItem attvi = sender as AddTileTreeViewItem;
            var parent = attvi.GetParent() as DistributedTestTreeViewItem;

            Tile tile = new Tile();
            parent.DistributedTest.Add(tile);

            largeList.Remove(attvi);

            AddAndCreateTileTreeViewItem(tile);

            largeList.Add(attvi);

            LockWindowUpdate(0);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            SetGui();
        }
        private void SetGui()
        {
            Control hoveredControl = null;
            foreach (ITestTreeViewItem ctrl in largeList.AllControls)
            {
                ctrl.SetVisibleControls();

                //To determine what add tile stresstest control can be visible
                Control control = ctrl as Control;
                if (control.ClientRectangle.Contains(control.PointToClient(Cursor.Position)))
                    hoveredControl = control;

                if (ctrl is AddTileTreeViewItem)
                    ctrl.SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));

                ctrl.RefreshGui();
            }

            //To determine what add tile stresstest control can be visible
            if (hoveredControl == null)
            {
                foreach (ITestTreeViewItem ctrl in largeList.AllControls)
                    if (ctrl is AddTileStresstestTreeViewItem)
                        ctrl.SetVisibleControls(false);
            }
            else
            {
                var addControl = GetClosestAddControl(hoveredControl);
                foreach (ITestTreeViewItem ctrl in largeList.AllControls)
                    if (ctrl is AddTileStresstestTreeViewItem)
                        ctrl.SetVisibleControls(ctrl == addControl);
            }
        }
        /// <summary>Gets the closest add control.</summary>
        /// <returns></returns>
        private AddTileStresstestTreeViewItem GetClosestAddControl(Control control)
        {
            KeyValuePair<int, int> index = largeList.IndexOf(control);
            for (int i = index.Key; i < largeList.ViewCount; i++)
                if (i == index.Key)
                {
                    for (int j = index.Value; j < largeList[i].Count; j++)
                        if (largeList[i][j] is AddTileStresstestTreeViewItem)
                            return largeList[i][j] as AddTileStresstestTreeViewItem;
                }
                else
                {
                    for (int j = 0; j < largeList[i].Count; j++)
                        if (largeList[i][j] is AddTileStresstestTreeViewItem)
                            return largeList[i][j] as AddTileStresstestTreeViewItem;
                }
            return null;
        }
    }
}
