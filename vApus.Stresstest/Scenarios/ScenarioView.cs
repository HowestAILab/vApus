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

namespace vApus.StressTest {
    public partial class ScenarioView : BaseSolutionComponentView {

        #region Fields
        private const string VBLRn = "<16 0C 02 12n>";
        private const string VBLRr = "<16 0C 02 12r>";

        private readonly Scenario _scenario;

        //For the find.
        private List<int> _foundUserActions = new List<int>();
        private List<int> _foundRequests = new List<int>();
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
        public ScenarioView() { InitializeComponent(); }
        public ScenarioView(SolutionComponent solutionComponent)
            : base(solutionComponent) {
            InitializeComponent();

            _scenario = solutionComponent as Scenario;
            if (IsHandleCreated)
                SetScenario();
            else
                HandleCreated += _HandleCreated;

            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (!IsDisposed && _scenario != null && IsHandleCreated)
                if (sender == _scenario || sender.GetParent() == _scenario || sender.GetParent().GetParent() == _scenario) {
                    _foundUserActions.Clear();
                    _foundRequests.Clear();
                    _foundColumns.Clear();
                    _foundMatchLengths.Clear();
                    _findIndex = 0;
                }
        }

        private void _HandleCreated(object sender, EventArgs e) {
            HandleCreated -= _HandleCreated;
            _scenario.ScenarioRuleSet.ScenarioRuleSetChanged += ScenarioRuleSet_ScenarioRuleSetChanged;
            SetScenario();
        }
        private void editScenario_ScenarioImported(object sender, EventArgs e) {
            SetScenario();
            //It is possible that the parameter token delimiters changed.
            editUserActionPanel.SetParameters();
            editUserActionPanel.SetCodeStyle();
        }
        private void editScenario_RevertedToAsImported(object sender, EventArgs e) {
            SetScenario();
            editUserActionPanel.SetParameters();
            editUserActionPanel.SetCodeStyle();
        }
        private void editScenario_RedeterminedTokens(object sender, EventArgs e) {
            SetScenario();
            editUserActionPanel.SetParameters();
            editUserActionPanel.SetCodeStyle();
        }
        private void ScenarioRuleSet_ScenarioRuleSetChanged(object sender, EventArgs e) {
            SetScenario();
            editUserActionPanel.SetParameters();
            editUserActionPanel.SetCodeStyle();
        }
        private void SetScenario() {
            scenarioTreeView.SetScenario(_scenario);
            editScenarioPanel.SetScenario(_scenario);
            editUserActionPanel.SetScenario(_scenario);
            _scenario.ApplyScenarioRuleSet();
        }

        private void scenarioTreeView_AfterSelect(object sender, EventArgs e) {
            if (sender is ScenarioTreeViewItem) {
                editScenarioPanel.Visible = true;
                editUserActionPanel.Visible = false;
            } else {
                editScenarioPanel.Visible = false;
                editUserActionPanel.Visible = true;

                editUserActionPanel.SetScenarioAndUserAction(_scenario, sender as UserActionTreeViewItem);
            }
        }
        private void editUserAction_UserActionMoved(object sender, EventArgs e) {
            tmrRefreshGui.Stop();
            scenarioTreeView.SetScenario(_scenario, (sender as UserActionTreeViewItem).UserAction);
            tmrRefreshGui.Start();
        }
        private void editUserAction_SplitClicked(object sender, EventArgs e) {
            tmrRefreshGui.Stop();
            scenarioTreeView.SetScenario(_scenario);
            tmrRefreshGui.Start();
        }
        private void editUserAction_LinkedChanged(object sender, EventArgs e) {
            tmrRefreshGui.Stop();
            scenarioTreeView.SetScenario(_scenario, (sender as EditUserActionPanel).UserActionTreeViewItem.UserAction);
            tmrRefreshGui.Start();
        }
        private void editUserAction_MergeClicked(object sender, EventArgs e) {
            tmrRefreshGui.Stop();
            scenarioTreeView.SetScenario(_scenario, (sender as EditUserActionPanel).UserActionTreeViewItem.UserAction);
            tmrRefreshGui.Start();
        }
        private void tmrRefreshGui_Tick(object sender, EventArgs e) {
            scenarioTreeView.SetGui();
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
                for (int i = 0; i != _scenario.Count; i++) {
                    UserAction userAction = _scenario[i] as UserAction;
                    for (int j = 0; j != userAction.Count; j++) {
                        var request = userAction[j] as Request;
                        List<int> r, c, ml;
                        vApus.Util.FindAndReplace.Find(_find, request.RequestString.Replace("\n", VBLRn).Replace("\r", VBLRr), out r, out c, out ml, wholeWords, ignoreCase);

                        for (int k = 0; k != r.Count; k++) {
                            _foundUserActions.Add(i);
                            _foundRequests.Add(j);
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
                    scenarioTreeView.SelectUserActionTreeViewItem(_selectedUserAction);
                }
                editUserActionPanel.SelectFound(_foundRequests[_findIndex], _foundColumns[_findIndex], _foundMatchLengths[_findIndex]);
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
                        var ua = _scenario[_foundUserActions[index]] as UserAction;
                        var re = ua[_foundRequests[index]] as Request;
                        re.RequestString = vApus.Util.FindAndReplace.Replace(0, _foundColumns[index], _foundMatchLengths[index], re.RequestString.Replace("\n", VBLRn).Replace("\r", VBLRr), with);
                        re.RequestString.Replace(VBLRn, "\n").Replace(VBLRr, "\r");
                        ++index;
                    }
                    _findIndex = 0;

                } else {
                    var ua = _scenario[_foundUserActions[_findIndex]] as UserAction;
                    var re = ua[_foundRequests[_findIndex]] as Request;
                    re.RequestString = vApus.Util.FindAndReplace.Replace(0, _foundColumns[_findIndex], _foundMatchLengths[_findIndex], re.RequestString.Replace("\n", VBLRn).Replace("\r", VBLRr), with);
                    re.RequestString.Replace(VBLRn, "\n").Replace(VBLRr, "\r");
                }

                SetScenario();
                _scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                int selectedUserAction = _foundUserActions[_findIndex];
                if (_selectedUserAction != selectedUserAction) {
                    _selectedUserAction = selectedUserAction;
                    scenarioTreeView.SelectUserActionTreeViewItem(_selectedUserAction);
                }
                _foundMatchLengths[_findIndex] = with.Length;
                editUserActionPanel.SelectFound(_foundRequests[_findIndex], _foundColumns[_findIndex], _foundMatchLengths[_findIndex]);
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
            _foundRequests.Clear();
            _foundColumns.Clear();
            _foundMatchLengths.Clear();
            _findIndex = 0;
        }
        #endregion
    }
}
