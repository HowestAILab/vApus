/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class EditUserAction : UserControl {

        public event EventHandler UserActionMoved;

        #region Fields
        private Log _log;
        private UserActionTreeViewItem _userActionTreeViewItem;

        /// <summary>
        /// Show the log entries structured.
        /// </summary>
        private DataTable _cache = new DataTable("Cache");

        private Rectangle _dragBoxFromMouseDown;
        private int _rowIndexFromMouseDown;
        private int _rowIndexOfItemUnderMouseToDrop;

        private string _beginTokenDelimiter;
        private string _endTokenDelimiter;

        private Parameters _parameters;

        private ParameterTokenTextStyle _parameterTokenTextStyle;

        private System.Timers.Timer _labelChanged = new System.Timers.Timer(500);
        #endregion

        public EditUserAction() {
            InitializeComponent();
            try {
                _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
                SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
            } catch { }
        }

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender is CustomListParameters || sender is CustomListParameter || sender is CustomRandomParameters || sender is CustomRandomParameter
                || sender is NumericParameters || sender is NumericParameter || sender is TextParameters || sender is TextParameter) {
                SetCodeStyle();
                SetParameters();
            }
        }
        internal void SetLogAndUserAction(Log log, UserActionTreeViewItem userActionTreeViewItem) {
            _log = log;
            _userActionTreeViewItem = userActionTreeViewItem;

            bool warning, error;
            _log.GetUniqueParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out warning, out error);

            cboParameterScope.SelectedIndex = 5;

            txtLabel.TextChanged -= txtLabel_TextChanged;
            txtLabel.Text = userActionTreeViewItem.UserAction.Label;
            txtLabel.TextChanged += txtLabel_TextChanged;

            _labelChanged.Elapsed += _labelChanged_Elapsed;

            if (_parameterTokenTextStyle == null) SetCodeStyle();
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
            MoveUserAction(false);
            SetMove();
        }
        private void picMoveDown_Click(object sender, EventArgs e) {
            MoveUserAction(true);
            SetMove();
        }
        private void MoveUserAction(bool down) {
            if (nudMoveSteps.Value == 0) return;

            int move = (int)nudMoveSteps.Value;
            if (!down) move *= -1;

            var userAction = _userActionTreeViewItem.UserAction;
            int newIndex = userAction.Index - 1 + move;

            bool moved = false;
            if (newIndex > -1 && newIndex < _log.Count) {
                _log.RemoveWithoutInvokingEvent(userAction);
                _log.InsertWithoutInvokingEvent(newIndex, userAction);

                moved = true;
            }
            if (newIndex >= _log.Count) {
                _log.RemoveWithoutInvokingEvent(userAction);
                _log.AddWithoutInvokingEvent(userAction);

                moved = true;
            }

            if (moved) {
                _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);

                if (UserActionMoved != null) UserActionMoved(_userActionTreeViewItem, null);
            }
        }
        private void picCopy_Click(object sender, EventArgs e) {
            ClipboardWrapper.SetDataObject(_userActionTreeViewItem.UserAction.Clone());
        }

        private void lbtn_ActiveChanged(object sender, EventArgs e) {
            SetLogEntries();
        }
        private void SetLogEntries() {
            _cache = new DataTable("Cache");

            dgvLogEntries.RowCount = 1;
            dgvLogEntries.Columns.Clear();

            _cache.Columns.Add("imageClm");
            if (_log.LogRuleSet.Count == 0)
                _cache.Columns.Add(_log.LogRuleSet.Label);
            else
                foreach (SyntaxItem item in _log.LogRuleSet)
                    _cache.Columns.Add(CheckOptionalSyntaxItem(item) ? item.Label : "*" + item.Label);

            var plainText = new StringBuilder();

            string[] splitter = new string[] { _log.LogRuleSet.ChildDelimiter };
            if (lbtnEditable.Active) {
                foreach (LogEntry logEntry in _userActionTreeViewItem.UserAction) {
                    var row = new List<string>();
                    row.Add(string.Empty);
                    row.AddRange(logEntry.LogEntryString.Split(splitter, StringSplitOptions.None));
                    _cache.Rows.Add(row.ToArray());
                    plainText.AppendLine(logEntry.LogEntryString);
                }
            } else {
                foreach (string s in _userActionTreeViewItem.UserAction.LogEntryStringsAsImported) {
                    var row = new List<string>();
                    row.Add(string.Empty);
                    row.AddRange(s.Split(splitter, StringSplitOptions.None));
                    _cache.Rows.Add(row.ToArray());
                    plainText.AppendLine(s);
                }
            }



            var imageColumn = new DataGridViewImageColumn();
            imageColumn.HeaderText = string.Empty;
            imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            imageColumn.CellTemplate = new DataGridViewImageCellBlank();
            imageColumn.DefaultCellStyle.NullValue = null;

            dgvLogEntries.Columns.Add(imageColumn);

            for (int i = 1; i != _cache.Columns.Count; i++) {
                var clm = new DataGridViewTextBoxColumn();
                clm.HeaderText = _cache.Columns[i].ColumnName;
                clm.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvLogEntries.Columns.Add(clm);
            }

            dgvLogEntries.RowCount = dgvLogEntries.ReadOnly ? _cache.Rows.Count : _cache.Rows.Count + 1;

            fctxtxPlainText.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);
            fctxtxPlainText.Range.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);

            fctxtxPlainText.TextChanged -= fctxtxPlainText_TextChanged;
            fctxtxPlainText.Text = plainText.ToString().TrimEnd();
            fctxtxPlainText.ClearUndo();
            fctxtxPlainText.TextChanged += fctxtxPlainText_TextChanged;

            SetEditableOrAsImported();
        }
        private bool CheckOptionalSyntaxItem(SyntaxItem item) {
            bool optional = item.Optional;
            foreach (var subItem in item)
                if (subItem is SyntaxItem)
                    if (item.Optional) {
                        optional = true;
                        break;
                    } else {
                        optional = CheckOptionalSyntaxItem(subItem as SyntaxItem);
                        if (optional) break;
                    }
            return optional;
        }
        private void tc_SelectedIndexChanged(object sender, EventArgs e) {
            SetEditableOrAsImported();
        }
        private void SetEditableOrAsImported() {
            if (lbtnEditable.Active) {
                btnRevertToImported.Visible = true;
                btnApply.Visible = tc.SelectedIndex == 1;
                btnApply.Enabled = false;
                dgvLogEntries.ReadOnly = fctxtxPlainText.ReadOnly = false;
                dgvLogEntries.AllowDrop = true;

                dgvLogEntries.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
            } else {
                btnApply.Visible = btnRevertToImported.Visible = false;
                dgvLogEntries.ReadOnly = fctxtxPlainText.ReadOnly = true;
                dgvLogEntries.AllowDrop = false;

                dgvLogEntries.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlDarkDark;
            }
        }

        private void dgvLogEntries_MouseMove(object sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left) {
                // If the mouse moves outside the rectangle, start the drag.
                if (_dragBoxFromMouseDown != Rectangle.Empty && !_dragBoxFromMouseDown.Contains(e.X, e.Y)) {

                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dgvLogEntries.DoDragDrop(
                    dgvLogEntries.Rows[_rowIndexFromMouseDown],
                    DragDropEffects.Move);
                }
            }
        }
        private void dgvLogEntries_MouseDown(object sender, MouseEventArgs e) {
            // Get the index of the item the mouse is below.
            _rowIndexFromMouseDown = dgvLogEntries.HitTest(e.X, e.Y).RowIndex;
            if (_rowIndexFromMouseDown != -1) {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                _dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            } else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                _dragBoxFromMouseDown = Rectangle.Empty;
        }
        private void dgvLogEntries_DragOver(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.Move;
        }
        private void dgvLogEntries_DragDrop(object sender, DragEventArgs e) {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = dgvLogEntries.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            _rowIndexOfItemUnderMouseToDrop = dgvLogEntries.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move) {
                var userAction = _userActionTreeViewItem.UserAction;
                var logEntry = userAction[_rowIndexFromMouseDown];

                userAction.RemoveWithoutInvokingEvent(logEntry);
                if (_rowIndexOfItemUnderMouseToDrop >= userAction.Count)
                    userAction.AddWithoutInvokingEvent(logEntry);
                else
                    userAction.InsertWithoutInvokingEvent(_rowIndexOfItemUnderMouseToDrop, logEntry);

                userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);

                SetLogEntries();

                foreach (DataGridViewRow row in dgvLogEntries.Rows)
                    row.Selected = false;
                dgvLogEntries.Rows[_rowIndexOfItemUnderMouseToDrop].Selected = true;
            }
        }
        private void dgvLogEntries_CellEndEdit(object sender, DataGridViewCellEventArgs e) {

        }
        private void dgvLogEntries_CellValuePushed(object sender, DataGridViewCellValueEventArgs e) {
            var userAction = _userActionTreeViewItem.UserAction;

            if (e.RowIndex >= userAction.Count) {
                var sb = new StringBuilder();
                for (int i = 1; i < _cache.Columns.Count - 1; i++) {
                    if (i == e.ColumnIndex) sb.Append(e.Value);
                    sb.Append(_log.LogRuleSet.ChildDelimiter);
                }
                if (e.ColumnIndex == _cache.Columns.Count - 1) sb.Append(e.Value);

                userAction.AddWithoutInvokingEvent(new LogEntry(sb.ToString()));
            } else {
                var row = _cache.Rows[e.RowIndex].ItemArray;
                row[e.ColumnIndex] = e.Value;

                var sb = new StringBuilder();
                for (int i = 1; i < row.Length - 1; i++) {
                    sb.Append(row[i]);
                    sb.Append(_log.LogRuleSet.ChildDelimiter);
                }
                sb.Append(row[row.Length - 1]);

                (userAction[e.RowIndex] as LogEntry).LogEntryString = sb.ToString();
            }

            _log.ApplyLogRuleSet();

            userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited, true);

            SetLogEntries();
        }
        private void dgvLogEntries_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete && dgvLogEntries.SelectedRows.Count != 0) {
                var userAction = _userActionTreeViewItem.UserAction;
                var toRemove = new List<BaseItem>();
                foreach (DataGridViewRow row in dgvLogEntries.SelectedRows) {
                    int index = dgvLogEntries.Rows.IndexOf(row);
                    if (index != userAction.Count) toRemove.Add(userAction[index]);
                }
                userAction.RemoveRangeWithoutInvokingEvent(toRemove);

                userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited, true);

                _log.ApplyLogRuleSet();
                SetLogEntries();
            }
        }
        private void dgvLogEntries_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            try {
                if (e.RowIndex < _cache.Rows.Count && e.ColumnIndex < _cache.Columns.Count) {
                    var userAction = _userActionTreeViewItem.UserAction;
                    if (e.ColumnIndex == 0) {
                        var logEntry = userAction[e.RowIndex] as LogEntry;
                        if (logEntry.LexicalResult == LexicalResult.OK) {
                            e.Value = null;
                            dgvLogEntries.Rows[e.RowIndex].Cells[0].ToolTipText = null;
                        } else {
                            if (logEntry.LexedLogEntry == null) {
                                e.Value = null;
                                dgvLogEntries.Rows[e.RowIndex].Cells[0].ToolTipText = null;
                            } else {
                                e.Value = global::vApus.Stresstest.Properties.Resources.LogEntryError;
                                int column = logEntry.LexedLogEntry.Count;
                                if (column == 0) column = 1;
                                dgvLogEntries.Rows[e.RowIndex].Cells[0].ToolTipText = logEntry.LexedLogEntry.Error + " See column " + column + ".";
                            }
                        }
                    } else {
                        e.Value = _cache.Rows[e.RowIndex][e.ColumnIndex];
                    }
                }
            } catch {
            }
        }

        private void btnRevertToImported_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to do this?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                var userAction = _userActionTreeViewItem.UserAction;
                userAction.ClearWithoutInvokingEvent(false);

                foreach (string s in userAction.LogEntryStringsAsImported)
                    userAction.AddWithoutInvokingEvent(new LogEntry(s), false);

                userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                _log.ApplyLogRuleSet();
                SetLogEntries();
            }
        }
        private void btnShowHideParameterTokens_Click(object sender, EventArgs e) {
            if (btnShowHideParameterTokens.Text == "Show Parameter Tokens") {
                btnShowHideParameterTokens.Text = "Hide Parameter Tokens";

                split.Panel2Collapsed = false;

                pnlBorderTokens.Width = split.Panel2.Width - pnlBorderTokens.Left - 9;
                pnlBorderTokens.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

                flpTokens.Width = split.Panel2.Width - flpTokens.Left - 9;
                flpTokens.Height = split.Panel2.Height - flpTokens.Top - 43;
                flpTokens.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            } else {
                btnShowHideParameterTokens.Text = "Show Parameter Tokens";
                pnlBorderTokens.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                flpTokens.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                split.Panel2Collapsed = true;
            }
        }

        private void fctxtxPlainText_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
            btnApply.Enabled = true;
        }
        private void btnApply_Click(object sender, EventArgs e) {
            var userAction = _userActionTreeViewItem.UserAction;
            bool changed = false;

            int i = 0;
            foreach (string s in fctxtxPlainText.Lines) {
                if (i < userAction.Count) {
                    var logEntry = userAction[i] as LogEntry;
                    if (logEntry.LogEntryString != s) {
                        logEntry.LogEntryString = s;
                        changed = true;
                    }
                } else {
                    userAction.AddWithoutInvokingEvent(new LogEntry(s));
                    changed = true;
                }
                ++i;
            }

            while (userAction.Count > i) {
                userAction.RemoveWithoutInvokingEvent(userAction[userAction.Count - 1]);
                changed = true;
            }

            if (changed) {
                _log.ApplyLogRuleSet();
                SetLogEntries();

                userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
            btnApply.Enabled = false;
        }

        private void cboParameterScope_SelectedIndexChanged(object sender, EventArgs e) {
            SetParameters();
        }
        private void SetParameters() {
            string scopeIdentifier = null;

            flpTokens.Controls.Clear();

            switch (cboParameterScope.SelectedIndex) {
                case 1:
                    scopeIdentifier = ASTNode.LOG_PARAMETER_SCOPE;
                    break;
                case 2:
                    scopeIdentifier = ASTNode.USER_ACTION_PARAMETER_SCOPE;
                    break;
                case 3:
                    scopeIdentifier = ASTNode.LOG_ENTRY_PARAMETER_SCOPE;
                    break;
                case 4:
                    scopeIdentifier = ASTNode.LEAF_NODE_PARAMETER_SCOPE;
                    break;
                case 5:
                    scopeIdentifier = ASTNode.ALWAYS_PARAMETER_SCOPE;
                    break;
            }

            if (scopeIdentifier == null) {
                AddKvpsToFlps(ASTNode.LOG_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.USER_ACTION_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.LOG_ENTRY_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.LEAF_NODE_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.ALWAYS_PARAMETER_SCOPE);
            } else {
                AddKvpsToFlps(scopeIdentifier);
            }
        }
        private void AddKvpsToFlps(string scopeIdentifier) {
            BaseItem customListParameters = _parameters[0];
            BaseItem numericParameters = _parameters[1];
            BaseItem textParameters = _parameters[2];
            BaseItem customRandomParameters = _parameters[3];

            int j = 1;
            for (int i = 0; i < customListParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             customListParameters[i].ToString(), Color.LightPink);
            for (int i = 0; i < numericParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             numericParameters[i].ToString(), Color.LightGreen);
            for (int i = 0; i < textParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             textParameters[i].ToString(), Color.LightBlue);
            for (int i = 0; i < customRandomParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             customRandomParameters[i].ToString(), Color.Yellow);
        }
        private void AddKvpToFlps(string key, string value, Color backColor) {
            var kvp = new KeyValuePairControl(key, value);
            kvp.BackColor = backColor;
            flpTokens.Controls.Add(kvp);
        }

        private void SetCodeStyle() {
            BaseItem customListParameters = _parameters[0];
            BaseItem numericParameters = _parameters[1];
            BaseItem textParameters = _parameters[2];
            BaseItem customRandomParameters = _parameters[3];

            var scopeIdentifiers = new[] { ASTNode.ALWAYS_PARAMETER_SCOPE, ASTNode.LEAF_NODE_PARAMETER_SCOPE, ASTNode.LOG_ENTRY_PARAMETER_SCOPE, 
                ASTNode.USER_ACTION_PARAMETER_SCOPE, ASTNode.LOG_PARAMETER_SCOPE };


            int index;
            List<string> clp = new List<string>(),
                         np = new List<string>(),
                         tp = new List<string>(),
                         crp = new List<string>();
            foreach (string scopeIdentifier in scopeIdentifiers) {
                index = 1;
                for (int i = 0; i < customListParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    clp.Add(token);
                }
                for (int i = 0; i < numericParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    np.Add(token);
                }
                for (int i = 0; i < textParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    tp.Add(token);
                }
                for (int i = 0; i < customRandomParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    crp.Add(token);
                }
            }
            fctxtxPlainText.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);
            fctxtxPlainText.Range.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);

            if (_parameterTokenTextStyle != null) {
                _parameterTokenTextStyle.Dispose();
                _parameterTokenTextStyle = null;
            }
            _parameterTokenTextStyle = new ParameterTokenTextStyle(fctxtxPlainText, GetDelimiters(_log.LogRuleSet), clp, np, tp, crp, true);
        }
        private string[] GetDelimiters(LogRuleSet logRuleSet) {
            var hs = new HashSet<string>();
            if (logRuleSet.ChildDelimiter.Length != 0)
                hs.Add(logRuleSet.ChildDelimiter);

            foreach (BaseItem item in logRuleSet)
                if (item is LogSyntaxItem)
                    foreach (string delimiter in GetDelimiters(item as LogSyntaxItem))
                        hs.Add(delimiter);

            var delimiters = new string[hs.Count];
            hs.CopyTo(delimiters);
            hs = null;

            return delimiters;
        }
        private IEnumerable<string> GetDelimiters(LogSyntaxItem logSyntaxItem) {
            if (logSyntaxItem.ChildDelimiter.Length != 0)
                yield return logSyntaxItem.ChildDelimiter;
            foreach (BaseItem item in logSyntaxItem)
                if (item is LogSyntaxItem)
                    foreach (string delimiter in GetDelimiters(item as LogSyntaxItem))
                        yield return delimiter;
        }
        #endregion
    }
}
