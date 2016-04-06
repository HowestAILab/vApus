using RandomUtils;
using RandomUtils.Log;
/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    /// Here you can choose the rule set, add or paste user actions or delete them all.
    /// </summary>
    [ToolboxItem(false)]
    public partial class ScenarioTreeViewItem : UserControl {

        #region Events
        /// <summary>
        ///     Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;

        public event EventHandler<ScenarioTreeView.AddUserActionEventArgs> AddPasteUserActionClicked;
        public event EventHandler ClearUserActionsClicked;
        #endregion

        #region Fields
        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrlKeyPressed;
        private readonly Scenario _scenario;

        private ScenarioRuleSets _scenarioRuleSets;

        private static Color _selectBackColor = Color.FromArgb(255, 240, 240, 240);
        #endregion

        #region Constructor
        /// <summary>
        /// Design time constructor, for testing.
        /// </summary>
        public ScenarioTreeViewItem() { InitializeComponent(); }
        /// <summary>
        /// Here you can choose the rule set, add or paste user actions or delete them all.
        /// </summary>
        /// <param name="scenario"></param>
        public ScenarioTreeViewItem(Scenario scenario)
            : this() {
            _scenario = scenario;
            if (Solution.ActiveSolution == null)
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
            else
                SetGui();
        }
        #endregion

        #region Functions
        public void Unfocus() {
            BackColor = Color.Transparent;
        }
        public new void Focus() {
            base.Focus();
            BackColor = _selectBackColor;
            if (AfterSelect != null) AfterSelect(this, null);
        }
        private void _Enter(object sender, EventArgs e) {
            Focus();
        }
        private void SetGui() {
            _scenarioRuleSets = Solution.ActiveSolution.GetSolutionComponent(typeof(ScenarioRuleSets)) as ScenarioRuleSets;
            FillCboRuleSet();
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
            picValid.Image = _scenario.LexicalResult == LexicalResult.OK ? null : global::vApus.StressTest.Properties.Resources.RequestError;
            _scenario.LexicalResultChanged += _scenario_LexicalResultChanged;
        }

        private void _scenario_LexicalResultChanged(object sender, Scenario.LexicalResultsChangedEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                picValid.Image = _scenario.LexicalResult == LexicalResult.OK ? null : global::vApus.StressTest.Properties.Resources.RequestError;
            }, null);
        }
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            SetGui();
        }
        //Update the scenario rule set cbo
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _scenarioRuleSets || sender is ScenarioRuleSet)
                FillCboRuleSet();
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
            } catch(Exception ex) {
                Loggers.Log(Level.Error, "Failed filling cbo rule set.", ex);
            }
        }
        private void cboRuleSet_SelectedIndexChanged(object sender, EventArgs e) {
            if (!IsDisposed && cboRuleSet.Items.Count != 0 && _scenarioRuleSets != null)
                try {
                    _scenario.ScenarioRuleSet = _scenarioRuleSets[cboRuleSet.SelectedIndex] as ScenarioRuleSet;
                    _scenario.ApplyScenarioRuleSet();
                    _scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                } catch (Exception ex){
                    Loggers.Log(Level.Error, "Failed applying scenario rule set.", ex, new object[] { sender, e });
                }
        }

        private void picAddUserAction_Click(object sender, EventArgs e) {
            var ua = new UserAction();
            _scenario.Add(ua);
            _scenario.ApplyScenarioRuleSet();
            _scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

            if (AddPasteUserActionClicked != null)
                AddPasteUserActionClicked(this, new ScenarioTreeView.AddUserActionEventArgs(ua));
        }
        private void picPasteUserAction_Click(object sender, EventArgs e) {
            IDataObject dataObject = ClipboardWrapper.GetDataObject();
            if (dataObject.GetDataPresent(typeof(UserAction))) {
                var ua = dataObject.GetData(typeof(UserAction)) as UserAction;
                if (ua != null) {
                    _scenario.Add(ua);
                    _scenario.ApplyScenarioRuleSet();
                    _scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                    if (AddPasteUserActionClicked != null)
                        AddPasteUserActionClicked(this, new ScenarioTreeView.AddUserActionEventArgs(ua));
                }
            }
        }
        private void picClearUserActions_Click(object sender, EventArgs e) {
            Focus();
            _scenario.Clear();
            if (ClearUserActionsClicked != null)
                ClearUserActionsClicked(this, null);
            _scenario.ApplyScenarioRuleSet();
        }

        private void _KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey)
                _ctrlKeyPressed = true;
        }
        private void _KeyUp(object sender, KeyEventArgs e) {
            if (_ctrlKeyPressed)
                if (e.KeyCode == Keys.I)
                    picAddUserAction_Click(picAddUserAction, null);
                else if (e.KeyCode == Keys.I)
                    picPasteUserAction_Click(picPasteUserAction, null);
        }
        #endregion
    }
}