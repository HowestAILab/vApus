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
    /// <summary>
    /// Lists a LogTreeViewItem and zero or more UserActionTreeViewItems.
    /// Call SetLog first.
    /// </summary>
    public partial class LogTreeView : UserControl {
        /// <summary>
        ///     The selected item is the sender
        /// </summary>
        public event EventHandler AfterSelect;

        #region Fields
        private Log _log;
        private LogTreeViewItem _logTreeViewItem;

        //Keep this focussed if possible, otherwise the LogTreeViewItem will be focussed.
        private UserActionTreeViewItem _focussedUserActionTreeViewItem;
        #endregion

        #region Properties
        /// <summary>
        ///     Yield returns all tree view items: a LogTreeViewItem and zero or more UserActionTreeViewItems.
        /// </summary>
        public IEnumerable<Control> Items {
            get {
                foreach (Control control in largeList.AllControls)
                    yield return control;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Lists a LogTreeViewItem and zero or more UserActionTreeViewItems.
        /// Call SetLog first.
        /// </summary>
        public LogTreeView() { InitializeComponent(); }
        #endregion

        #region Functions
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        public void SetLog(Log log, UserAction focus = null) {
            if (IsDisposed) return;
            LockWindowUpdate(Handle);
            //Try to select the same control as before, this wil be overriden if focus != null.
            var selection = largeList.BeginOfSelection;
            if (selection.Key == -1 || selection.Value == -1)
                selection = new KeyValuePair<int, int>(0, 0);

            _log = log;
            _logTreeViewItem = new LogTreeViewItem(_log);
            _logTreeViewItem.AfterSelect += _AfterSelect;
            _logTreeViewItem.AddPasteUserActionClicked += _logTreeViewItem_AddPasteUserActionClicked;
            _logTreeViewItem.ClearUserActionsClicked += _logTreeViewItem_ClearUserActionsClicked;

            //For backwards compatibility, all loose log entries are put into a user action.
            var newLog = new List<BaseItem>(_log.Count);
            bool newlogNeeded = false;
            foreach (BaseItem item in _log) {
                UserAction ua = null;
                if (item is UserAction) {
                    ua = item as UserAction;
                } else {
                    newlogNeeded = true;
                    var logEntry = item as LogEntry;
                    ua = new UserAction(logEntry.LogEntryString.Length < 101 ? logEntry.LogEntryString : logEntry.LogEntryString.Substring(0, 100) + "...");
                    ua.AddWithoutInvokingEvent(logEntry);
                }
                newLog.Add(ua);
            }

            if (newlogNeeded) {
                _log.ClearWithoutInvokingEvent();
                _log.AddRangeWithoutInvokingEvent(newLog);
            }

            //Add al to a list, and add the list to the largelist afterwards, this is faster.
            var rangeToAdd = new List<Control>(_log.Count + 1);
            rangeToAdd.Add(_logTreeViewItem);

            foreach (UserAction userAction in _log) {
                var uatvi = CreateUserActionTreeViewItem(userAction);
                rangeToAdd.Add(uatvi);
            }

            largeList.Clear();
            largeList.AddRange(rangeToAdd);

            if (focus != null) {
                foreach (Control ctrl in largeList.AllControls) {
                    var uatvi = ctrl as UserActionTreeViewItem;
                    if (uatvi != null && uatvi.UserAction == focus)
                        selection = largeList.IndexOf(uatvi);
                }
            }

            if (largeList.ViewCount <= selection.Key || largeList[selection.Key].Count <= selection.Value || selection.Key == -1 || selection.Value == -1)
                selection = new KeyValuePair<int, int>(0, 0);
            largeList.Select(selection);

            _focussedUserActionTreeViewItem = (selection.Key == 0 && selection.Value == 0) ? null : largeList.Selection[0] as UserActionTreeViewItem;
            if (_focussedUserActionTreeViewItem == null) _logTreeViewItem.Focus(); else _focussedUserActionTreeViewItem.Focus();

            LockWindowUpdate(IntPtr.Zero);
        }

        private void _logTreeViewItem_AddPasteUserActionClicked(object sender, LogTreeView.AddUserActionEventArgs e) { CreateAndAddUserActionTreeViewItem(e.UserAction); }
        private void _logTreeViewItem_ClearUserActionsClicked(object sender, EventArgs e) {
            largeList.Clear();
            largeList.Add(_logTreeViewItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userAction"></param>
        /// <param name="atIndex">Insert the item at a certain index, if == -1 it will be added tot the end of the collection.</param>
        /// <returns></returns>
        private UserActionTreeViewItem CreateAndAddUserActionTreeViewItem(UserAction userAction, int atIndex = -1) {
            var uatvi = CreateUserActionTreeViewItem(userAction);
            if (atIndex == -1 || atIndex >= largeList.ControlCount)
                largeList.Add(uatvi);
            else
                largeList.Insert(uatvi, new KeyValuePair<int, int>(largeList.CurrentView, atIndex));
            return uatvi;
        }
        private UserActionTreeViewItem CreateUserActionTreeViewItem(UserAction userAction) {
            var uatvi = new UserActionTreeViewItem(_log, userAction);
            uatvi.AfterSelect += _AfterSelect;
            uatvi.DuplicateClicked += uatvi_DuplicateClicked;
            uatvi.DeleteClicked += uatvi_DeleteClicked;
            return uatvi;
        }
        private void uatvi_DuplicateClicked(object sender, LogTreeView.AddUserActionEventArgs e) {
            CreateAndAddUserActionTreeViewItem(e.UserAction, e.UserAction.Index);
        }
        private void uatvi_DeleteClicked(object sender, EventArgs e) {
            largeList.Remove(sender as Control);
            largeList.Select(_logTreeViewItem);
            _logTreeViewItem.Focus();
        }

        /// <summary>
        /// Use this in for instance find and replace functionality.
        /// </summary>
        /// <param name="userActionIndex">Zero-based index in the log.</param>
        public void SelectUserActionTreeViewItem(int userActionIndex) {
            //+ 1 because there is a LogTreeViewItem.
            ++userActionIndex;

            int i = 0;
            foreach (Control control in largeList.AllControls)
                if (i++ == userActionIndex) {
                    var uatvi = control as UserActionTreeViewItem;
                    uatvi.Select();
                    uatvi.Focus();
                    break;
                }
        }

        private void _AfterSelect(object sender, EventArgs e) {
            _focussedUserActionTreeViewItem = null;
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

        internal void SetGui() {
            if (_focussedUserActionTreeViewItem != null) _focussedUserActionTreeViewItem.Focus();

            for (int v = 0; v != largeList.ViewCount; v++)
                for (int i = 0; i != largeList[v].Count; i++) {
                    var ctrl = largeList[v][i];
                    if (ctrl != _logTreeViewItem && ctrl != null && ctrl is UserActionTreeViewItem)
                        try {
                            (ctrl as UserActionTreeViewItem).SetVisibleControls();
                        } catch {
                            //In case of timer issues, occured one time only.
                        }
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
