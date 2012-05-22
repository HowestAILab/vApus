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
    public partial class SlaveTreeView : UserControl
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

        #region Constructors
        public SlaveTreeView()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        public void SetMode(DistributedTestMode distributedTestMode)
        {
            if (_distributedTestMode != distributedTestMode)
            {
                _distributedTestMode = distributedTestMode;
            }
        }

        public void SetDistributedTest(DistributedTest distributedTest)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            largeList.Clear();
            var sctvi = new SlaveCollectionTreeViewItem();
            sctvi.AfterSelect += new EventHandler(_AfterSelect);
            sctvi.AddClientClicked += new EventHandler(sctvi_AddClientClicked);
            largeList.Add(sctvi);

            //bool addControlsVisible = ClientRectangle.Contains(PointToClient(Cursor.Position));
            //foreach (Tile tile in distributedTest.Tiles)
            //    CreateAndAddTileTreeViewItem(tile);

           // SetGui();

            sctvi.Select();
            LockWindowUpdate(0);
        }

        private void sctvi_AddClientClicked(object sender, EventArgs e)
        {
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
        #endregion
    }
}
