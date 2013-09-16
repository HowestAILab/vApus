/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class PlaintTextLogView : BaseSolutionComponentView {

        #region Fields

        private readonly Log _log;
        private LogRuleSets _logRuleSets;

        //For the find.
        private List<int> _foundRows = new List<int>();
        private List<int> _foundColumns = new List<int>();
        private List<int> _foundMatchLengths = new List<int>();
        private int _findIndex = 0;
        private string _find = string.Empty;
        private bool _findWholeWords = false;
        private bool _findIgnoreCase = true;

        private FindAndReplaceDialog _findAndReplaceDialog;
        #endregion

        #region Constructors
        /// <summary>
        /// Design time constructor.
        /// </summary>
        public PlaintTextLogView() { InitializeComponent(); }
        public PlaintTextLogView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            InitializeComponent();

            _log = solutionComponent as Log;
            _logRuleSets = Solution.ActiveSolution.GetSolutionComponent(typeof(LogRuleSets)) as LogRuleSets;

            if (IsHandleCreated) {
                FillCboRuleSet();
                SetLog();
            } else {
                HandleCreated += NewLogView_HandleCreated;
            }

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (!IsDisposed && _log != null && IsHandleCreated) {
                if (sender == _log || sender.GetParent() == _log || sender.GetParent().GetParent() == _log) {
                    _foundRows.Clear();
                    _foundColumns.Clear();
                    _foundMatchLengths.Clear();
                    _findIndex = 0;
                }

                if (sender == _log.LogRuleSet || sender == _logRuleSets || sender is LogRuleSet)
                    FillCboRuleSet();
            }

        }
        private void NewLogView_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= NewLogView_HandleCreated;
            FillCboRuleSet();
            SetLog();
        }

        private void SetLog() {
            var sb = new StringBuilder();
            foreach (UserAction userAction in _log) {
                sb.Append("<!--");
                sb.Append(userAction.Label);
                sb.AppendLine("-->");
                foreach (LogEntry logEntry in userAction)
                    sb.AppendLine(logEntry.LogEntryString);
            }

            fctxt.Text = sb.ToString();
            fctxt.ClearUndo();

            fctxt.TextChanged += fctxt_TextChanged;
        }

        private void FillCboRuleSet() {
            try {
                if (!IsDisposed) {
                    cboRuleSet.SelectedIndexChanged -= cboRuleSet_SelectedIndexChanged;

                    cboRuleSet.Items.Clear();
                    if (_logRuleSets != null)
                        foreach (LogRuleSet ruleSet in _logRuleSets)
                            cboRuleSet.Items.Add(ruleSet);

                    if (_log != null && _log.LogRuleSet != null && cboRuleSet.Items.Contains(_log.LogRuleSet))
                        cboRuleSet.SelectedItem = _log.LogRuleSet;
                    else if (cboRuleSet.Items.Count != 0) cboRuleSet.SelectedIndex = 0;

                    cboRuleSet.SelectedIndexChanged += cboRuleSet_SelectedIndexChanged;
                }
            } catch { }
        }

        private void cboRuleSet_SelectedIndexChanged(object sender, EventArgs e) { btnApply.Enabled = true; }

        private void fctxt_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) { btnUndo.Enabled = btnApply.Enabled = true; }

        private void txtFind_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && txtFind.Text.Length != 0) Find(false, true);
        }
        private void picFind_Click(object sender, EventArgs e) {
            Find(false, true);
        }
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

            if (_foundMatchLengths.Count == 0) {
                _findIndex = 0;
                _find = txtFind.Text.TrimEnd();
                vApus.Util.FindAndReplace.Find(_find, fctxt.Text, out _foundRows, out _foundColumns, out _foundMatchLengths, wholeWords, ignoreCase);
            }

            if (_foundMatchLengths.Count != 0) {
                SelectFound(_foundRows[_findIndex], _foundColumns[_findIndex], _foundMatchLengths[_findIndex]);

                if (++_findIndex >= _foundMatchLengths.Count) _findIndex = 0;
            }

            txtFind.Select();
            txtFind.Focus();
        }
        public void SelectFound(int row, int column, int matchLength) {
            if (row < fctxt.LinesCount) {
                int line = 0, start = 0;
                foreach (char c in fctxt.Text) {
                    if (line < row)
                        ++start;
                    if (c == '\n' && ++line >= row)
                        break;
                }

                start += column;
                if (start + matchLength < fctxt.Text.Length) {
                    fctxt.SelectionStart = start;
                    fctxt.SelectionLength = matchLength;

                    fctxt.DoSelectionVisible();
                }
                Focus();
            }
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
            if (_foundMatchLengths.Count == 0) {
                Find(wholeWords, ignoreCase);
                if (--_findIndex < 0) _findIndex = 0;
            }

            if (all) {
                fctxt.Text = vApus.Util.FindAndReplace.Replace(_foundRows, _foundColumns, _foundMatchLengths, fctxt.Text, with);
            } else {
                fctxt.Text = vApus.Util.FindAndReplace.Replace(_foundRows[_findIndex], _foundColumns[_findIndex], _foundMatchLengths[_findIndex], fctxt.Text, with);
            }

            _foundMatchLengths[_findIndex] = with.Length;
            SelectFound(_foundRows[_findIndex], _foundColumns[_findIndex], _foundMatchLengths[_findIndex]);

            ResetFindCache();
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
            _foundRows.Clear();
            _foundColumns.Clear();
            _foundMatchLengths.Clear();
            _findIndex = 0;
        }

        private void btnUndo_Click(object sender, EventArgs e) {
            fctxt.Undo();
        }

        private void btnApply_Click(object sender, EventArgs e) {
            btnApply.Enabled = false;

            _log.ClearWithoutInvokingEvent(false);

            if (!IsDisposed && cboRuleSet.Items.Count != 0 && _logRuleSets != null)
                try {
                    _log.LogRuleSet = _logRuleSets[cboRuleSet.SelectedIndex] as LogRuleSet;
                } catch { }

            UserAction currentUserAction = null;
            string userActionBegin = "<!--", userActionEnd = "-->";
            foreach (string s in fctxt.Lines)
                if (s.StartsWith(userActionBegin) && s.EndsWith(userActionEnd)) {
                    currentUserAction = new UserAction(s.Substring(userActionBegin.Length, s.Length - 7));
                    _log.AddWithoutInvokingEvent(currentUserAction);
                } else if (currentUserAction != null) {
                    currentUserAction.AddWithoutInvokingEvent(new LogEntry(s), false);
                }

            _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        #endregion
    }
}
