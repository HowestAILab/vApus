/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class LogView : BaseSolutionComponentView {

        #region Fields
        private const string VBLRn = "<16 0C 02 12n>";
        private const string VBLRr = "<16 0C 02 12r>";

        private readonly Log _log;

        //For the find.
        private List<int> _foundUserActions = new List<int>();
        private List<int> _foundLogEntries = new List<int>();
        private List<int> _foundColumns = new List<int>();
        private List<int> _foundMatchLengths = new List<int>();
        private int _findIndex = 0, _selectedUserAction = -1;
        private string _find = string.Empty;
        private bool _findWholeWords = false;
        private bool _findIgnoreCase = true;

        private FindAndReplaceDialog _findAndReplaceDialog;
        #endregion

        #region Constructors
        /// <summary>
        /// Design time constructor.
        /// </summary>
        public LogView() { InitializeComponent(); }
        public LogView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            InitializeComponent();

            _log = solutionComponent as Log;
            if (IsHandleCreated)
                SetLog();
            else
                HandleCreated += LogView_HandleCreated;

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (!IsDisposed && _log != null && IsHandleCreated)
                if (sender == _log || sender.GetParent() == _log || sender.GetParent().GetParent() == _log) {
                    _foundUserActions.Clear();
                    _foundLogEntries.Clear();
                    _foundColumns.Clear();
                    _foundMatchLengths.Clear();
                    _findIndex = 0;
                }
        }

        private void LogView_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= LogView_HandleCreated;
            _log.LogRuleSet.LogRuleSetChanged += LogRuleSet_LogRuleSetChanged;
            SetLog();
        }
        private void editLog_LogImported(object sender, EventArgs e) {
            SetLog();
            //It is possible that the parameter token delimiters changed.
            editUserActionPanel.SetParameters();
            editUserActionPanel.SetCodeStyle();
        }
        private void editLog_RevertedToAsImported(object sender, EventArgs e) {
            SetLog();
            editUserActionPanel.SetParameters();
            editUserActionPanel.SetCodeStyle();
        }
        private void editLog_RedeterminedTokens(object sender, EventArgs e) {
            SetLog();
            editUserActionPanel.SetParameters();
            editUserActionPanel.SetCodeStyle();
        }
        private void LogRuleSet_LogRuleSetChanged(object sender, EventArgs e) {
            SetLog();
            editUserActionPanel.SetParameters();
            editUserActionPanel.SetCodeStyle();
        }
        private void SetLog() {
            logTreeView.SetLog(_log);
            editLogPanel.SetLog(_log);
            editUserActionPanel.SetLog(_log);
            _log.ApplyLogRuleSet();
        }

        private void logTreeView_AfterSelect(object sender, EventArgs e) {
            if (sender is LogTreeViewItem) {
                editLogPanel.Visible = true;
                editUserActionPanel.Visible = false;
            } else {
                editLogPanel.Visible = false;
                editUserActionPanel.Visible = true;

                editUserActionPanel.SetLogAndUserAction(_log, sender as UserActionTreeViewItem);
            }
        }
        private void editUserAction_UserActionMoved(object sender, EventArgs e) {
            tmrRefreshGui.Stop();
            logTreeView.SetLog(_log, (sender as UserActionTreeViewItem).UserAction);
            tmrRefreshGui.Start();
        }
        private void editUserAction_SplitClicked(object sender, EventArgs e) {
            tmrRefreshGui.Stop();
            logTreeView.SetLog(_log);
            tmrRefreshGui.Start();
        }
        private void editUserAction_LinkedChanged(object sender, EventArgs e) {
            tmrRefreshGui.Stop();
            logTreeView.SetLog(_log, (sender as EditUserActionPanel).UserActionTreeViewItem.UserAction);
            tmrRefreshGui.Start();
        }
        private void editUserAction_MergeClicked(object sender, EventArgs e) {
            tmrRefreshGui.Stop();
            logTreeView.SetLog(_log, (sender as EditUserActionPanel).UserActionTreeViewItem.UserAction);
            tmrRefreshGui.Start();
        }
        private void tmrRefreshGui_Tick(object sender, EventArgs e) {
            logTreeView.SetGui();
        }

        private void txtFind_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter && txtFind.Text.Length != 0) Find(false, true); }
        private void picFind_Click(object sender, EventArgs e) { Find(false, true); }
        private void _findAndReplaceDialog_FindClicked(object sender, FindAndReplaceDialog.FindEventArgs e) {
            if (txtFind.Text != e.Find)
                txtFind.Text = e.Find;
            else if (_findWholeWords != e.WholeWords || _findIgnoreCase != e.IgnoreCase)
                ResetFindCache();

            //The above checked variables will be set here.
            Find(e.WholeWords, e.IgnoreCase);
        }
        private void Find(bool wholeWords, bool ignoreCase) {
            _findWholeWords = wholeWords;
            _findIgnoreCase = ignoreCase;

            if (_foundUserActions.Count == 0) {
                _find = txtFind.Text.TrimEnd();
                for (int i = 0; i != _log.Count; i++) {
                    UserAction userAction = _log[i] as UserAction;
                    for (int j = 0; j != userAction.Count; j++) {
                        var logEntry = userAction[j] as LogEntry;
                        List<int> r, c, ml;
                        vApus.Util.FindAndReplace.Find(_find, logEntry.LogEntryString.Replace("\n", VBLRn).Replace("\r", VBLRr), out r, out c, out ml, wholeWords, ignoreCase);

                        for (int k = 0; k != r.Count; k++) {
                            _foundUserActions.Add(i);
                            _foundLogEntries.Add(j);
                        }
                        _foundColumns.AddRange(c);
                        _foundMatchLengths.AddRange(ml);
                    }
                }
            }
            if (_foundUserActions.Count != 0) {
                int selectedUserAction = _foundUserActions[_findIndex];
                if (_selectedUserAction != selectedUserAction) {
                    _selectedUserAction = selectedUserAction;
                    logTreeView.SelectUserActionTreeViewItem(_selectedUserAction);
                }
                editUserActionPanel.SelectFound(_foundLogEntries[_findIndex], _foundColumns[_findIndex], _foundMatchLengths[_findIndex]);
                if (++_findIndex >= _foundUserActions.Count) _findIndex = 0;
            }
            txtFind.Select();
            txtFind.Focus();
        }

        private void txtFind_TextChanged(object sender, EventArgs e) {
            if (_find != txtFind.Text) {
                ResetFindCache();
                if (_findAndReplaceDialog == null || _findAndReplaceDialog.IsDisposed) {
                    _findAndReplaceDialog = new FindAndReplaceDialog();
                    _findAndReplaceDialog.FindClicked += _findAndReplaceDialog_FindClicked;
                    _findAndReplaceDialog.ReplaceClicked += _findAndReplaceDialog_ReplaceClicked;
                }
                _findAndReplaceDialog.SetFind(_find);
            }
            picFind.Enabled = (txtFind.Text.Length != 0);
        }
   
        private void _findAndReplaceDialog_ReplaceClicked(object sender, FindAndReplaceDialog.ReplaceEventArgs e) {
            txtFind.Text = e.Find;
            ResetFindCache();

            Replace(e.WholeWords, e.IgnoreCase, e.With, e.All);
        }
        private void Replace(bool wholeWords, bool ignoreCase, string with, bool all) {
            if (_foundUserActions.Count == 0) {
                Find(wholeWords, ignoreCase);
                if (--_findIndex < 0) _findIndex = 0;
            }
            if (_foundUserActions.Count != 0) {
                if (all) {
                    int index = 0;
                    while (index != _foundUserActions.Count) {
                        var ua = _log[_foundUserActions[index]] as UserAction;
                        var le = ua[_foundLogEntries[index]] as LogEntry;
                        le.LogEntryString = vApus.Util.FindAndReplace.Replace(0, _foundColumns[index], _foundMatchLengths[index], le.LogEntryString.Replace("\n", VBLRn).Replace("\r", VBLRr), with);
                        le.LogEntryString = le.LogEntryString.Replace(VBLRn, "\n").Replace(VBLRr, "\r");
                        ++index;
                    }
                    _findIndex = 0;

                } else {
                    var ua = _log[_foundUserActions[_findIndex]] as UserAction;
                    var le = ua[_foundLogEntries[_findIndex]] as LogEntry;
                    le.LogEntryString = vApus.Util.FindAndReplace.Replace(0, _foundColumns[_findIndex], _foundMatchLengths[_findIndex], le.LogEntryString.Replace("\n", VBLRn).Replace("\r", VBLRr), with);
                    le.LogEntryString = le.LogEntryString.Replace(VBLRn, "\n").Replace(VBLRr, "\r");
                }

                SetLog();
                _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                int selectedUserAction = _foundUserActions[_findIndex];
                if (_selectedUserAction != selectedUserAction) {
                    _selectedUserAction = selectedUserAction;
                    logTreeView.SelectUserActionTreeViewItem(_selectedUserAction);
                }
                _foundMatchLengths[_findIndex] = with.Length;
                editUserActionPanel.SelectFound(_foundLogEntries[_findIndex], _foundColumns[_findIndex], _foundMatchLengths[_findIndex]);
                if (++_findIndex >= _foundUserActions.Count) _findIndex = 0;
            }
        }

        private void llblFindAndReplace_Click(object sender, EventArgs e) {
            if (_findAndReplaceDialog == null || _findAndReplaceDialog.IsDisposed) {
                _findAndReplaceDialog = new FindAndReplaceDialog();
                _findAndReplaceDialog.FindClicked += _findAndReplaceDialog_FindClicked;
                _findAndReplaceDialog.ReplaceClicked += _findAndReplaceDialog_ReplaceClicked;
            }
            _findAndReplaceDialog.SetFind(_find);
            if (!_findAndReplaceDialog.Visible) _findAndReplaceDialog.Show();
        }

        private void ResetFindCache() {
            _find = txtFind.Text;
            _foundUserActions.Clear();
            _foundLogEntries.Clear();
            _foundColumns.Clear();
            _foundMatchLengths.Clear();
            _findIndex = 0;
        }
        #endregion
    }
}
