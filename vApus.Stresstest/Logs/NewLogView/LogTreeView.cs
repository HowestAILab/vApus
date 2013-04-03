using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using vApus.SolutionTree;

namespace vApus.Stresstest {
    public partial class LogTreeView : UserControl {

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        /// <summary>
        ///     The selected item is the sender
        /// </summary>
        public event EventHandler AfterSelect;

        LogTreeViewItem _logTreeViewItem;
        #region Properties
        /// <summary>
        ///     get all tree view items.
        /// </summary>
        public IEnumerable<Control> Items {
            get {
                foreach (Control control in largeList.AllControls)
                    yield return control;
            }
        }

        #endregion

        public LogTreeView() {
            InitializeComponent();
        }

        #region Functions
        public void SetLog(Log log) {
            if (IsDisposed) return;
            LockWindowUpdate(Handle.ToInt32());
            largeList.Clear();
            _logTreeViewItem = new LogTreeViewItem(log);
            _logTreeViewItem.AfterSelect += _AfterSelect;
            //dttvi.AddTileClicked += dttvi_AddTileClicked;
            largeList.Add(_logTreeViewItem);

            //foreach (Tile tile in distributedTest.Tiles) CreateAndAddTileTreeViewItem(tile);
            foreach (UserAction userAction in log) {
                CreateAndAddUserActionTreeViewItem(userAction);
            }

            //SetGui();

            //select the first stresstest tvi if any
            //bool selected = false;
            //foreach (Control control in largeList.AllControls)
            //    if (control is TileStresstestTreeViewItem) {
            //        control.Select();
            //        selected = true;
            //        break;
            //    }

            //if (!selected) dttvi.Select();

            LockWindowUpdate(0);
        }
        private void CreateAndAddUserActionTreeViewItem(UserAction userAction) {
            var uatvi = new UserActionTreeViewItem(userAction);
            uatvi.AfterSelect += _AfterSelect;
            largeList.Add(uatvi);
        }
        private void _AfterSelect(object sender, EventArgs e) {
            for (int v = 0; v != largeList.ViewCount; v++)
                for (int i = 0; i != largeList[v].Count; i++) {
                    var ctrl = largeList[v][i];
                    if (ctrl != sender) {
                        if (ctrl == _logTreeViewItem)
                            _logTreeViewItem.Unfocus();
                        else
                            (ctrl as UserActionTreeViewItem).Unfocus();
                    }
                }

            if (AfterSelect != null) 
                AfterSelect(sender, e);
        }
        public void SetGui() {
            for (int v = 0; v != largeList.ViewCount; v++)
                for (int i = 0; i != largeList[v].Count; i++) {
                    var ctrl = largeList[v][i];
                        if (ctrl != _logTreeViewItem)
                            (ctrl as UserActionTreeViewItem).SetVisibleControls();
                }
        }
        #endregion
    }
}
