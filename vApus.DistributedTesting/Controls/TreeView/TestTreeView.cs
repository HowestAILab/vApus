using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace vApus.DistributedTesting
{
    public partial class TestTreeView : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public TestTreeView()
        {
            InitializeComponent();
            TestGui();
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
            largeList.Clear();
            largeList.Add(new DistributedTestTreeViewItem(distributedTest));

            bool addControlsVisible = ClientRectangle.Contains(PointToClient(Cursor.Position));
            foreach (Tile tile in distributedTest)
            {
                var tvi = CreateTreeViewItem(tile);
                largeList.Add(tvi);
                foreach (TileStresstest tileStresstest in tile)
                {
                    var tsvi = new TileStresstestTreeViewItem(tileStresstest);
                    tvi.ChildControls.Add(tsvi);
                    largeList.Add(tsvi);
                }
                var atstvi = new AddTileStresstestTreeViewItem();
                tvi.ChildControls.Add(atstvi);
                largeList.Add(atstvi);
            }
            largeList.Add(new AddTileTreeViewItem());

            SetGui();
        }
        private TileTreeViewItem CreateTreeViewItem(Tile tile)
        {
            var tvi = new TileTreeViewItem(tile);
            tvi.CollapsedChanged += new EventHandler(tvi_CollapsedChanged);
            return tvi;
        }

        private void tvi_CollapsedChanged(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            var tvi = sender as TileTreeViewItem;
            if (tvi.Collapsed)
            {
                largeList.RemoveRange(new List<Control>(tvi.ChildControls.ToArray()));
            }
            else
            {
                KeyValuePair<int, int> index = largeList.IndexOf(GetClosestNextSibling(tvi));
                if (index.Value == -1)
                    largeList.AddRange(new List<Control>(tvi.ChildControls.ToArray()));
                else
                    largeList.InsertRange(new List<Control>(tvi.ChildControls.ToArray()), index);
            }
            LockWindowUpdate(0);
        }
        /// <summary>Gets the closest next sibling.</summary>
        /// <returns></returns>
        private Control GetClosestNextSibling(Control control)
        {
            Type type = control.GetType();
            KeyValuePair<int, int> index = largeList.IndexOf(control);
            for (int i = index.Key; i < largeList.ViewCount; i++)
                if (i == index.Key)
                {
                    for (int j = index.Value + 1; j < largeList[i].Count; j++)
                        if (largeList[i][j].GetType() == type)
                            return largeList[i][j];
                }
                else
                {
                    for (int j = 0; j < largeList[i].Count; j++)
                        if (largeList[i][j].GetType() == type)
                            return largeList[i][j];
                }
            return largeList[largeList.ViewCount - 1][largeList[largeList.ViewCount - 1].Count - 1];
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            SetGui();
        }
        private void SetGui()
        {
            foreach (ITestTreeViewItem ctrl in largeList.AllControls)
            {
                ctrl.SetVisibleControls();

                if (ctrl is AddTileStresstestTreeViewItem || ctrl is AddTileTreeViewItem)
                    ctrl.SetVisibleControls(ClientRectangle.Contains(PointToClient(Cursor.Position)));

                ctrl.RefreshLabel();
            }
        }
    }
}
