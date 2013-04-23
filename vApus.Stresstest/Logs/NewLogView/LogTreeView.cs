/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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

        private UserActionTreeViewItem _focussedUserAction = null;


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
        public void SetLog(Log log, UserAction focus = null) {
            if (IsDisposed) return;
            LockWindowUpdate(Handle.ToInt32());
            var selection = largeList.BeginOfSelection;
            if (selection.Key == -1 || selection.Value == -1)
                selection = new KeyValuePair<int, int>(0, 0);

            largeList.Clear();
            _log = log;
            _logTreeViewItem = new LogTreeViewItem(_log);
            _logTreeViewItem.AfterSelect += _AfterSelect;
            _logTreeViewItem.AddPasteUserActionClicked += _logTreeViewItem_AddPasteUserActionClicked;
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
                } else {
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

            _focussedUserAction = null;
            foreach (UserAction userAction in log) {
                var uatvi = CreateAndAddUserActionTreeViewItem(userAction);

                if (uatvi.UserAction == focus) _focussedUserAction = uatvi;
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

            if (largeList.ViewCount <= selection.Key || largeList[selection.Key].Count <= selection.Value)
                selection = new KeyValuePair<int, int>(0, 0);
            largeList.Select(selection);

            var selected = largeList.Selection[0];
            if (selected is LogTreeViewItem)
                (selected as LogTreeViewItem).Focus();
            else
                (selected as UserActionTreeViewItem).Focus();

            LockWindowUpdate(0);
        }

        private void _logTreeViewItem_AddPasteUserActionClicked(object sender, LogTreeView.AddUserActionEventArgs e) {
            CreateAndAddUserActionTreeViewItem(e.UserAction);
        }
        private void _logTreeViewItem_ClearUserActionsClicked(object sender, EventArgs e) {
            largeList.Clear();
            largeList.Add(_logTreeViewItem);
        }

        private UserActionTreeViewItem CreateAndAddUserActionTreeViewItem(UserAction userAction) {
            var uatvi = new UserActionTreeViewItem(_log, userAction);
            uatvi.AfterSelect += _AfterSelect;
            uatvi.DuplicateClicked += uatvi_DuplicateClicked;
            uatvi.DeleteClicked += uatvi_DeleteClicked;
            largeList.Add(uatvi);
            return uatvi;
        }

        private void uatvi_DuplicateClicked(object sender, LogTreeView.AddUserActionEventArgs e) {
            CreateAndAddUserActionTreeViewItem(e.UserAction);
        }
        void uatvi_DeleteClicked(object sender, EventArgs e) {
            largeList.Select(_logTreeViewItem);
            largeList.Remove(sender as Control);
        }

        private void _AfterSelect(object sender, EventArgs e) {
            _focussedUserAction = null;
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
            largeList.Select(sender as Control);
            if (AfterSelect != null)
                AfterSelect(sender, e);
        }
        public void SetGui() {
            if (_focussedUserAction != null) _focussedUserAction.Focus();

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
