using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class EditUserAction : UserControl {
        #region Fields
        private Log _log;
        private UserActionTreeViewItem _userActionTreeViewItem;

        private System.Timers.Timer _labelChanged = new System.Timers.Timer(500);
        #endregion

        public EditUserAction() { InitializeComponent(); }

        #region Functions
        internal void SetLogAndUserAction(Log log, UserActionTreeViewItem userActionTreeViewItem) {
            _log = log;
            _userActionTreeViewItem = userActionTreeViewItem;

            txtLabel.TextChanged -= txtLabel_TextChanged;
            txtLabel.Text = userActionTreeViewItem.UserAction.Label;
            txtLabel.TextChanged += txtLabel_TextChanged;

            _labelChanged.Elapsed += _labelChanged_Elapsed;

            SetMove();

            SetLogEntries();
        }

        private void _labelChanged_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                _userActionTreeViewItem.UserAction.Label = txtLabel.Text;
                _userActionTreeViewItem.SetLabel();
                _userActionTreeViewItem.UserAction.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            }, null);
        }

        private void txtLabel_TextChanged(object sender, EventArgs e) {
            if (_labelChanged != null) {
                _labelChanged.Stop();
                _labelChanged.Start();
            }
        }
        private void SetMove() {
            int index = _userActionTreeViewItem.UserAction.Index;
            int count = _log.CountOf(typeof(UserAction));
            picMoveUp.Enabled = index != 1;
            picMoveDown.Enabled = index != count;

            decimal value = nudMoveSteps.Value;
            nudMoveSteps.Maximum = count - index;

            int candidate = count - Math.Abs(index - count) - 1;
            if (candidate > nudMoveSteps.Maximum) nudMoveSteps.Maximum = candidate;

            if (nudMoveSteps.Maximum == 0)
                nudMoveSteps.Minimum = nudMoveSteps.Maximum = 1;

            if (value > nudMoveSteps.Maximum) value = nudMoveSteps.Maximum;
            nudMoveSteps.Value = value;
        }

        private void picMoveUp_Click(object sender, EventArgs e) {
            SetMove();
        }
        private void picMoveDown_Click(object sender, EventArgs e) {
            SetMove();
        }
        #endregion

        private void lbtn_ActiveChanged(object sender, EventArgs e) {
            SetLogEntries();
        }
        private void SetLogEntries() {
            dgvLogEntries.DataSource = null;

            var structured = new DataTable("Structured");
            if (_log.LogRuleSet.Count == 0)
                structured.Columns.Add(_log.LogRuleSet.Label);
            else
                foreach (SyntaxItem item in _log.LogRuleSet) structured.Columns.Add(item.Label);

            var plainText = new StringBuilder();

            if (lbtnEditable.Active) {
                foreach (LogEntry logEntry in _userActionTreeViewItem.UserAction) {
                    var row = new List<string>();
                    if (logEntry.LexedLogEntry.Count == 0)
                        row.Add(logEntry.LexedLogEntry.Value);
                    else
                        foreach (ASTNode node in logEntry.LexedLogEntry)
                            if (node.Count == 0) row.Add(node.Value); else row.Add(node.CombineValues());

                    structured.Rows.Add(row.ToArray());
                    plainText.AppendLine(logEntry.LogEntryString);
                }
            } else {
                string[] splitter = new string[] { _log.LogRuleSet.ChildDelimiter };
                foreach (LogEntry logEntry in _userActionTreeViewItem.UserAction) {
                    structured.Rows.Add(logEntry.LogEntryStringAsImported.Split(splitter, StringSplitOptions.None));
                    plainText.AppendLine(logEntry.LogEntryStringAsImported);
                }
            }

            dgvLogEntries.DataSource = structured;
            foreach (DataGridViewColumn clm in dgvLogEntries.Columns) clm.SortMode = DataGridViewColumnSortMode.NotSortable;

            fctxtxPlainText.Text = plainText.ToString().TrimEnd();

            SetEditableOrAsImported();
        }
        private void tc_SelectedIndexChanged(object sender, EventArgs e) {
            SetEditableOrAsImported();
        }
        private void SetEditableOrAsImported() {
            if (lbtnEditable.Active) {
                btnRevertToImported.Visible = btnUndo.Visible = true;
                btnApply.Visible = (tc.SelectedIndex == 1);
                dgvLogEntries.ReadOnly = fctxtxPlainText.ReadOnly = false;

                dgvLogEntries.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
            } else {
                btnApply.Visible = btnRevertToImported.Visible = btnUndo.Visible = false;
                dgvLogEntries.ReadOnly = fctxtxPlainText.ReadOnly = true;

                dgvLogEntries.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlDarkDark;
            }
        }
    }
}
