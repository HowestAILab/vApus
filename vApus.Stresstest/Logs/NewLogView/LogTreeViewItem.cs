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

namespace vApus.Stresstest {
    [ToolboxItem(false)]
    public partial class LogTreeViewItem : UserControl {

        #region Events

        /// <summary>
        ///     Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;

        public event EventHandler<LogTreeView.AddUserActionEventArgs> AddPasteUserActionClicked;
        public event EventHandler ClearUserActionsClicked;

        #endregion

        #region Fields


        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;
        private readonly Log _log;

        private LogRuleSets _logRuleSets;

        private static Color _selectedColor = Color.FromArgb(255, 240, 240, 240);
        #endregion

        #region Constructor

        public LogTreeViewItem() {
            InitializeComponent();
        }

        public LogTreeViewItem(Log log)
            : this() {
            _log = log;
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
            BackColor = _selectedColor;
            if (AfterSelect != null) AfterSelect(this, null);
        }
        private void _Enter(object sender, EventArgs e) {
            Focus();
        }
        private void SetGui() {
            _logRuleSets = Solution.ActiveSolution.GetSolutionComponent(typeof(LogRuleSets)) as LogRuleSets;
            FillCboRuleSet();
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
            picValid.Image = _log.LexicalResult == LexicalResult.OK ? null : global::vApus.Stresstest.Properties.Resources.LogEntryError;
            _log.LexicalResultChanged += _log_LexicalResultChanged;
        }

        private void _log_LexicalResultChanged(object sender, Log.LexicalResultsChangedEventArgs e) {
            picValid.Image = _log.LexicalResult == LexicalResult.OK ? null : global::vApus.Stresstest.Properties.Resources.LogEntryError;
        }
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            SetGui();
        }
        //Update the log rule set cbo
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _log.LogRuleSet || sender == _logRuleSets || sender is LogRuleSet)
                FillCboRuleSet();
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
        private void cboRuleSet_SelectedIndexChanged(object sender, EventArgs e) {
            if (!IsDisposed && cboRuleSet.Items.Count != 0 && _logRuleSets != null)
                try {
                    _log.LogRuleSet = _logRuleSets[cboRuleSet.SelectedIndex] as LogRuleSet;
                    _log.ApplyLogRuleSet();
                    _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                } catch { }
        }

        private void picAddUserAction_Click(object sender, EventArgs e) {
            var ua = new UserAction();
            _log.Add(ua);
            _log.ApplyLogRuleSet();
            _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

            if (AddPasteUserActionClicked != null)
                AddPasteUserActionClicked(this, new LogTreeView.AddUserActionEventArgs(ua));
        }
        private void picPasteUserAction_Click(object sender, EventArgs e) {
            IDataObject dataObject = ClipboardWrapper.GetDataObject();
            if (dataObject.GetDataPresent(typeof(UserAction))) {
                var ua = dataObject.GetData(typeof(UserAction)) as UserAction;
                if (ua != null) {
                    _log.Add(ua);
                    _log.ApplyLogRuleSet();
                    _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                    if (AddPasteUserActionClicked != null)
                        AddPasteUserActionClicked(this, new LogTreeView.AddUserActionEventArgs(ua));
                }
            }
        }
        private void picClearUserActions_Click(object sender, EventArgs e) {
            Focus();
            _log.Clear();
            if (ClearUserActionsClicked != null)
                ClearUserActionsClicked(this, null);
        }
        private void _KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey)
                _ctrl = true;
        }

        private void _KeyUp(object sender, KeyEventArgs e) {
            if (_ctrl)
                if (e.KeyCode == Keys.I)
                    picAddUserAction_Click(picAddUserAction, null);
                else if (e.KeyCode == Keys.I)
                    picPasteUserAction_Click(picPasteUserAction, null);
        }

        #endregion
    }
}