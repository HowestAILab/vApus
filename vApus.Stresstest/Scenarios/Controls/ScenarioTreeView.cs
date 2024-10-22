﻿/*
 * 2013 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.StressTest {
    /// <summary>
    /// Lists a ScenarioTreeViewItem and zero or more UserActionTreeViewItems.
    /// Call SetScenario first.
    /// </summary>
    public partial class ScenarioTreeView : UserControl {
        /// <summary>
        ///     The selected item is the sender
        /// </summary>
        public event EventHandler AfterSelect;

        #region Fields
        private Scenario _scenario;
        private ScenarioTreeViewItem _scenarioTreeViewItem;

        //Keep this focussed if possible, otherwise the ScenarioTreeViewItem will be focussed.
        private UserActionTreeViewItem _focussedUserActionTreeViewItem;
        #endregion

        #region Properties
        /// <summary>
        ///     Yield returns all tree view items: a ScenarioTreeViewItem and zero or more UserActionTreeViewItems.
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
        /// Lists a ScenarioTreeViewItem and zero or more UserActionTreeViewItems.
        /// Call SetScenario first.
        /// </summary>
        public ScenarioTreeView() { InitializeComponent(); }
        #endregion

        #region Functions
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        public void SetScenario(Scenario scenario, UserAction focus = null) {
            if (IsDisposed) return;
            LockWindowUpdate(Handle);
            //Try to select the same control as before, this wil be overriden if focus != null.
            var selection = largeList.BeginOfSelection;
            if (selection.Key == -1 || selection.Value == -1)
                selection = new KeyValuePair<int, int>(0, 0);

            _scenario = scenario;
            _scenarioTreeViewItem = new ScenarioTreeViewItem(_scenario);
            _scenarioTreeViewItem.AfterSelect += _AfterSelect;
            _scenarioTreeViewItem.AddPasteUserActionClicked += _scenarioTreeViewItem_AddPasteUserActionClicked;
            _scenarioTreeViewItem.ClearUserActionsClicked += _scenarioTreeViewItem_ClearUserActionsClicked;

            //For backwards compatibility, all loose requests are put into a user action.
            var newScenario = new List<BaseItem>(_scenario.Count);
            bool newScenarioNeeded = false;
            foreach (BaseItem item in _scenario) {
                UserAction ua = null;
                if (item is UserAction) {
                    ua = item as UserAction;
                } else {
                    newScenarioNeeded = true;
                    var request = item as Request;
                    ua = new UserAction(request.RequestString.Length < 101 ? request.RequestString : request.RequestString.Substring(0, 100) + "...");
                    ua.AddWithoutInvokingEvent(request);
                }
                newScenario.Add(ua);
            }

            if (newScenarioNeeded) {
                _scenario.ClearWithoutInvokingEvent();
                _scenario.AddRangeWithoutInvokingEvent(newScenario);
            }

            //Add al to a list, and add the list to the largelist afterwards, this is faster.
            var rangeToAdd = new List<Control>(_scenario.Count + 1);
            rangeToAdd.Add(_scenarioTreeViewItem);

            foreach (UserAction userAction in _scenario) {
                var uatvi = CreateUserActionTreeViewItem(userAction);
                rangeToAdd.Add(uatvi);
            }

            largeList.Clear();
            largeList.AddRange(rangeToAdd);

            LockWindowUpdate(IntPtr.Zero);

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
            largeList.ScrollTo(selection.Key);

            _focussedUserActionTreeViewItem = (selection.Key == 0 && selection.Value == 0) ? null : largeList.Selection[0] as UserActionTreeViewItem;
            if (_focussedUserActionTreeViewItem == null) _scenarioTreeViewItem.Focus(); else _focussedUserActionTreeViewItem.Focus();
        }

        private void _scenarioTreeViewItem_AddPasteUserActionClicked(object sender, ScenarioTreeView.AddUserActionEventArgs e) { CreateAndAddUserActionTreeViewItem(e.UserAction); }
        private void _scenarioTreeViewItem_ClearUserActionsClicked(object sender, EventArgs e) {
            largeList.Clear();
            largeList.Add(_scenarioTreeViewItem);
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
                largeList.Insert(uatvi, largeList.ParseFlatIndex(atIndex));
            return uatvi;
        }
        private UserActionTreeViewItem CreateUserActionTreeViewItem(UserAction userAction) {
            var uatvi = new UserActionTreeViewItem(_scenario, userAction);
            uatvi.AfterSelect += _AfterSelect;
            uatvi.DuplicateClicked += uatvi_DuplicateClicked;
            uatvi.DeleteClicked += uatvi_DeleteClicked;
            return uatvi;
        }
        private void uatvi_DuplicateClicked(object sender, ScenarioTreeView.AddUserActionEventArgs e) {
            CreateAndAddUserActionTreeViewItem(e.UserAction, e.UserAction.Index);
        }
        private void uatvi_DeleteClicked(object sender, EventArgs e) {
            largeList.Remove(sender as Control);
            largeList.Select(_scenarioTreeViewItem);
            _scenarioTreeViewItem.Focus();
        }

        /// <summary>
        /// Use this in for instance find and replace functionality.
        /// </summary>
        /// <param name="userActionIndex">Zero-based index in the scenario.</param>
        public void SelectUserActionTreeViewItem(int userActionIndex) {
            //+ 1 because there is a ScenarioTreeViewItem.
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
                        if (ctrl == _scenarioTreeViewItem)
                            _scenarioTreeViewItem.Unfocus();
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
                    if (ctrl != _scenarioTreeViewItem && ctrl != null && ctrl is UserActionTreeViewItem)
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
