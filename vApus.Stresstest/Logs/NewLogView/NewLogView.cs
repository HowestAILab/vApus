/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class NewLogView : BaseSolutionComponentView {
        private readonly Log _log;

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

    }
}
