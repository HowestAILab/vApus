/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest {
    [ToolboxItem(false)]
    public partial class LogTreeViewItem : UserControl {

        #region Events

        /// <summary>
        ///     Call unfocus for the other items in the panel.
        /// </summary>
        public event EventHandler AfterSelect;

        public event EventHandler AddUserActionClicked;
        public event EventHandler ClearUserActionsClicked;

        #endregion

        #region Fields


        /// <summary>
        ///     Check if the ctrl key is pressed.
        /// </summary>
        private bool _ctrl;
        private readonly Log _log;
        private bool _testStarted;

        private LogRuleSets _logRuleSets;
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

        private void _Enter(object sender, EventArgs e) {
            BackColor = SystemColors.Control;
            if (AfterSelect != null)
                AfterSelect(this, null);
        }
        private void SetGui() {
            _logRuleSets = Solution.ActiveSolution.GetSolutionComponent(typeof(LogRuleSets)) as LogRuleSets;
            FillCboRuleSet();
            SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            SetGui();
        }
        //Update the log rule set cbo
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender == _log || sender == _log.LogRuleSet || sender == _logRuleSets || sender is LogRuleSet) {

            }
        }
        private void FillCboRuleSet() {
            try {
                if (!IsDisposed) {
                    cboRuleSet.Items.Clear();
                    if (_logRuleSets != null)
                        foreach (LogRuleSet ruleSet in _logRuleSets)
                            cboRuleSet.Items.Add(ruleSet);

                    if (cboRuleSet.Items.Count != 0) cboRuleSet.SelectedIndex = 0;
                }
            } catch { }
        }
        private void cboRuleSet_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void picAddUserAction_Click(object sender, EventArgs e) {
            Focus();
            if (AddUserActionClicked != null)
                AddUserActionClicked(this, null);
        }
        private void picClearUserActions_Click(object sender, EventArgs e) {
            Focus();
            if (ClearUserActionsClicked != null)
                ClearUserActionsClicked(this, null);
        }
        private void _KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey)
                _ctrl = true;
        }

        private void _KeyUp(object sender, KeyEventArgs e) {
            if (_ctrl && e.KeyCode == Keys.I && AddUserActionClicked != null)
                AddUserActionClicked(this, null);
        }

        #endregion
    }
}