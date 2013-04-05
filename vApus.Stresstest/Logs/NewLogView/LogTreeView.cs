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

        private Log _log;
        private LogTreeViewItem _logTreeViewItem;

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
            _log = log;
            _logTreeViewItem = new LogTreeViewItem(_log);
            _logTreeViewItem.AfterSelect += _AfterSelect;
            _logTreeViewItem.AddUserActionClicked += _logTreeViewItem_AddUserActionClicked;
            _logTreeViewItem.ClearUserActionsClicked += _logTreeViewItem_ClearUserActionsClicked;
            //dttvi.AddTileClicked += dttvi_AddTileClicked;
            largeList.Add(_logTreeViewItem);

            //For backwards compatibility, all loose log entries are put into a user action.
            var newLog = new List<BaseItem>(log.Count);
            bool newlogNeeded = false;
            foreach (BaseItem item in log) {
                UserAction ua = null;
                if (item is UserAction) {
                    ua = item as UserAction;
                }
                else {
                    newlogNeeded = true;
                    var logEntry = item as LogEntry;
                    ua = new UserAction(logEntry.LogEntryString.Length < 101 ? logEntry.LogEntryString : logEntry.LogEntryString.Substring(0, 100) + "...");
                    ua.AddWithoutInvokingEvent(logEntry, false);
                }
                newLog.Add(ua);
            }

            if (newlogNeeded) {
                log.ClearWithoutInvokingEvent(false);
                log.AddRangeWithoutInvokingEvent(newLog, false);
            }
          
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

        private void _logTreeViewItem_AddUserActionClicked(object sender, LogTreeView.AddUserActionEventArgs e) {
            CreateAndAddUserActionTreeViewItem(e.UserAction);
        }
        private void _logTreeViewItem_ClearUserActionsClicked(object sender, EventArgs e) {
            largeList.Clear();
            largeList.Add(_logTreeViewItem);
        }

        private void CreateAndAddUserActionTreeViewItem(UserAction userAction) {
            var uatvi = new UserActionTreeViewItem(_log, userAction);
            uatvi.AfterSelect += _AfterSelect;
            uatvi.DuplicateClicked += uatvi_DuplicateClicked;
            uatvi.DeleteClicked += uatvi_DeleteClicked;
            largeList.Add(uatvi);
        }

        private void uatvi_DuplicateClicked(object sender, LogTreeView.AddUserActionEventArgs e) {
            CreateAndAddUserActionTreeViewItem(e.UserAction);
        }
        void uatvi_DeleteClicked(object sender, EventArgs e) {
            largeList.Select(_logTreeViewItem);
            largeList.Remove(sender as Control);
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

        public class AddUserActionEventArgs : EventArgs {
            public UserAction UserAction { get; private set; }
            public AddUserActionEventArgs(UserAction userAction) {
                UserAction = userAction;
            }
        }
    }
}
