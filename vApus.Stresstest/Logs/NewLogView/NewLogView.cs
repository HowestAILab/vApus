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
    public partial class NewLogView : BaseSolutionComponentView {
        private readonly Log _log;

        //For the find.
        private List<int> _foundUserActions = new List<int>();
        private List<int> _foundLogEntries = new List<int>();
        private List<int> _foundColumns = new List<int>();
        private List<int> _foundMatchLengths = new List<int>();
        private int _findIndex = 0, _selectedUserAction = -1;
        private string _find = string.Empty;
        private FindAndReplaceDialog _findAndReplaceDialog = new FindAndReplaceDialog();

        public NewLogView() {
            InitializeComponent();
        }

        public NewLogView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args) {
            InitializeComponent();

            _log = solutionComponent as Log;
            if (IsHandleCreated)
                SetLog();
            else
                HandleCreated += NewLogView_HandleCreated;

            _findAndReplaceDialog.FindClicked += _findAndReplaceDialog_FindClicked;
            _findAndReplaceDialog.ReplaceClicked += _findAndReplaceDialog_ReplaceClicked;

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

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

        private void NewLogView_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= NewLogView_HandleCreated;
            _log.LogRuleSet.LogRuleSetChanged += LogRuleSet_LogRuleSetChanged;
            SetLog();
        }
        private void editLog_LogImported(object sender, EventArgs e) {
            SetLog();
            //It is possible that the parameter token delimiters changed.
            editUserAction.SetParameters();
            editUserAction.SetCodeStyle();
        }
        private void editLog_RedeterminedTokens(object sender, EventArgs e) {
            SetLog();
            editUserAction.SetParameters();
            editUserAction.SetCodeStyle();
        }
        private void LogRuleSet_LogRuleSetChanged(object sender, EventArgs e) {
            SetLog();
        }
        private void SetLog() {
            logTreeView.SetLog(_log);
            editLog.SetLog(_log);
            _log.ApplyLogRuleSet();
        }

        private void logTreeView_AfterSelect(object sender, EventArgs e) {
            if (sender is LogTreeViewItem) {
                editLog.Visible = true;
                editUserAction.Visible = false;
            } else {
                editLog.Visible = false;
                editUserAction.Visible = true;

                editUserAction.SetLogAndUserAction(_log, sender as UserActionTreeViewItem);
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
            logTreeView.SetLog(_log, (sender as EditUserAction).UserActionTreeViewItem.UserAction);
            tmrRefreshGui.Start();
        }
        private void editUserAction_MergeClicked(object sender, EventArgs e) {
            tmrRefreshGui.Stop();
            logTreeView.SetLog(_log, (sender as EditUserAction).UserActionTreeViewItem.UserAction);
            tmrRefreshGui.Start();
        }
        private void tmrRefreshGui_Tick(object sender, EventArgs e) {
            logTreeView.SetGui();
        }

        private void picFind_Click(object sender, EventArgs e) {
            Find();
        }

        private void txtFind_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) Find();
        }

        private void Find(bool ignoreCase = true) {
            if (_foundUserActions.Count == 0) {
                _find = txtFind.Text.TrimEnd();
                for (int i = 0; i != _log.Count; i++) {
                    UserAction userAction = _log[i] as UserAction;
                    for (int j = 0; j != userAction.Count; j++) {
                        var logEntry = userAction[j] as LogEntry;
                        List<int> r, c, ml;
                        vApus.Util.FindAndReplace.Find(_find, logEntry.LogEntryString, out r, out c, out ml, ignoreCase);

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
                    logTreeView.SelectFound(_selectedUserAction);
                }
                editUserAction.SelectFound(_foundLogEntries[_findIndex], _foundColumns[_findIndex], _foundMatchLengths[_findIndex]);
                if (++_findIndex >= _foundUserActions.Count) _findIndex = 0;
            }
            txtFind.Select();
            txtFind.Focus();
        }
        private void Replace(bool ignoreCase, string with, bool all) {
            if (_foundUserActions.Count == 0) {
                Find(ignoreCase);
                if (--_findIndex < 0) _findIndex = 0;
            }
            if (_foundUserActions.Count != 0) {
                if (all) {
                    int index = 0;
                    while (index != _foundUserActions.Count) {
                        var ua = _log[_foundUserActions[index]] as UserAction;
                        var le = ua[_foundLogEntries[index]] as LogEntry;
                        le.LogEntryString = vApus.Util.FindAndReplace.Replace(0, _foundColumns[index], _foundMatchLengths[index], le.LogEntryString, with);
                        ++index;
                    }
                    _findIndex = 0;

                } else {
                    var ua = _log[_foundUserActions[_findIndex]] as UserAction;
                    var le = ua[_foundLogEntries[_findIndex]] as LogEntry;
                    le.LogEntryString = vApus.Util.FindAndReplace.Replace(0, _foundColumns[_findIndex], _foundMatchLengths[_findIndex], le.LogEntryString, with);
                }

                SetLog();
                _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                int selectedUserAction = _foundUserActions[_findIndex];
                if (_selectedUserAction != selectedUserAction) {
                    _selectedUserAction = selectedUserAction;
                    logTreeView.SelectFound(_selectedUserAction);
                }
                _foundMatchLengths[_findIndex] = with.Length;
                editUserAction.SelectFound(_foundLogEntries[_findIndex], _foundColumns[_findIndex], _foundMatchLengths[_findIndex]);
                if (++_findIndex >= _foundUserActions.Count) _findIndex = 0;
            }
        }

        private void txtFind_TextChanged(object sender, EventArgs e) {
            if (_find != txtFind.Text) {
                _find = txtFind.Text;
                _foundUserActions.Clear();
                _foundLogEntries.Clear();
                _foundColumns.Clear();
                _foundMatchLengths.Clear();
                _findIndex = 0;

                _findAndReplaceDialog.SetFind(_find);
            }
        }
        private void _findAndReplaceDialog_FindClicked(object sender, FindAndReplaceDialog.FindEventArgs e) {
            txtFind.Text = e.Find;
            Find(e.IgnoreCase);
        }
        private void _findAndReplaceDialog_ReplaceClicked(object sender, FindAndReplaceDialog.ReplaceEventArgs e) {
            txtFind.Text = e.Find;
            Replace(e.IgnoreCase, e.With, e.All);
        }

        private void llblFindAndReplace_Click(object sender, EventArgs e) {
            if (!_findAndReplaceDialog.Visible) _findAndReplaceDialog.Show();
        }
    }
}
