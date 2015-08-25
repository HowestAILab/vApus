/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    public partial class PlaintTextScenarioView : BaseSolutionComponentView {

        #region Fields

        private readonly Scenario _scenario;
        private ScenarioRuleSets _scenarioRuleSets;

        //For the find.
        private List<int> _foundRows = new List<int>();
        private List<int> _foundColumns = new List<int>();
        private List<int> _foundMatchLengths = new List<int>();
        private int _findIndex = 0;
        private string _find = string.Empty;
        private bool _findWholeWords = false;
        private bool _findIgnoreCase = true;

        private FindAndReplaceDialog _findAndReplaceDialog;

        private const string VBLRn = "<16 0C 02 12n>";
        private const string VBLRr = "<16 0C 02 12r>";
        #endregion

        #region Constructors
        /// <summary>
        /// Design time constructor.
        /// </summary>
        public PlaintTextScenarioView() { InitializeComponent(); }
        public PlaintTextScenarioView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            InitializeComponent();

            _scenario = solutionComponent as Scenario;
            _scenarioRuleSets = Solution.ActiveSolution.GetSolutionComponent(typeof(ScenarioRuleSets)) as ScenarioRuleSets;

            if (IsHandleCreated) {
                FillCboRuleSet();
                SetScenario();
            } else {
                HandleCreated += _HandleCreated;
            }

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;

            fctxt.DefaultContextMenu(true);
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (!IsDisposed && _scenario != null && IsHandleCreated) {
                if (sender == _scenario || sender.GetParent() == _scenario || sender.GetParent().GetParent() == _scenario) {
                    _foundRows.Clear();
                    _foundColumns.Clear();
                    _foundMatchLengths.Clear();
                    _findIndex = 0;
                }

                if (sender == _scenario.ScenarioRuleSet || sender == _scenarioRuleSets || sender is ScenarioRuleSet)
                    FillCboRuleSet();
            }

        }
        private void _HandleCreated(object sender, EventArgs e) {
            HandleCreated -= _HandleCreated;
            FillCboRuleSet();
            SetScenario();
        }

        private void SetScenario() {
            var sb = new StringBuilder();
            foreach (UserAction userAction in _scenario) {
                sb.Append("<!--");
                sb.Append(userAction.Label);
                sb.AppendLine("-->");
                foreach (Request request in userAction)
                    sb.AppendLine(request.RequestString.Replace("\n", VBLRn).Replace("\r", VBLRr));
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
                    if (_scenarioRuleSets != null)
                        foreach (ScenarioRuleSet ruleSet in _scenarioRuleSets)
                            cboRuleSet.Items.Add(ruleSet);

                    if (_scenario != null && _scenario.ScenarioRuleSet != null && cboRuleSet.Items.Contains(_scenario.ScenarioRuleSet))
                        cboRuleSet.SelectedItem = _scenario.ScenarioRuleSet;
                    else if (cboRuleSet.Items.Count != 0) cboRuleSet.SelectedIndex = 0;

                    cboRuleSet.SelectedIndexChanged += cboRuleSet_SelectedIndexChanged;
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed filling cbo rule set.", ex);
            }
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

            _scenario.ClearWithoutInvokingEvent();

            if (!IsDisposed && cboRuleSet.Items.Count != 0 && _scenarioRuleSets != null)
                try {
                    _scenario.ScenarioRuleSet = _scenarioRuleSets[cboRuleSet.SelectedIndex] as ScenarioRuleSet;
                } catch(Exception ex) {
                    Loggers.Log(Level.Error, "Failed setting scenario rule set.", ex, new object[] { sender, e });
                }

            UserAction currentUserAction = null;
            string userActionBegin = "<!--", userActionEnd = "-->";
            foreach (string s in fctxt.Lines)
                if (s.StartsWith(userActionBegin) && s.EndsWith(userActionEnd)) {
                    currentUserAction = new UserAction(s.Substring(userActionBegin.Length, s.Length - 7));
                    _scenario.AddWithoutInvokingEvent(currentUserAction);
                } else if (currentUserAction != null) {
                    currentUserAction.AddWithoutInvokingEvent(new Request(s.Replace(VBLRn, "\n").Replace(VBLRr, "\r")));
                }

            _scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        #endregion
    }
}
