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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class EditUserAction : UserControl {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler UserActionMoved, SplitClicked, MergeClicked, LinkedChanged;

        #region Fields
        private Log _log;
        private UserActionTreeViewItem _userActionTreeViewItem;

        private static int[] _linkColors = { 0x00FF00, 0x0000FF, 0xFF0000, 0x01FFFE, 0xFFA6FE, 0xFFDB66, 0x006401, 0x010067, 0x95003A, 0x007DB5, 0xFF00F6, 0xFFEEE8, 0x774D00, 0x90FB92, 0x0076FF, 0xD5FF00, 
                                             0xFF937E, 0x6A826C, 0xFF029D, 0xFE8900, 0x7A4782, 0x7E2DD2, 0x85A900, 0xFF0056, 0xA42400, 0x00AE7E, 0x683D3B, 0xBDC6FF, 0x263400, 0xBDD393, 0x00B917, 0x9E008E,
                                             0x001544, 0xC28C9F, 0xFF74A3, 0x01D0FF, 0x004754, 0xE56FFE, 0x788231, 0x0E4CA1, 0x91D0CB, 0xBE9970, 0x968AE8, 0xBB8800, 0x43002C, 0xDEFF74, 0x00FFC6, 0xFFE502,
                                             0x620E00, 0x008F9C, 0x98FF52, 0x7544B1, 0xB500FF, 0x00FF78, 0xFF6E41, 0x005F39, 0x6B6882, 0x5FAD4E, 0xA75740, 0xA5FFD2, 0xFFB167, 0x009BFF, 0xE85EBE };

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

        public UserActionTreeViewItem UserActionTreeViewItem {
            get { return _userActionTreeViewItem; }
        }

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
                SetParameters();
                SetCodeStyle();
            }
        }
        internal void SetLogAndUserAction(Log log, UserActionTreeViewItem userActionTreeViewItem) {
            LockWindowUpdate(this.Handle.ToInt32());
            _log = log;
            _userActionTreeViewItem = userActionTreeViewItem;

            cboParameterScope.SelectedIndex = 5;

            txtLabel.TextChanged -= txtLabel_TextChanged;
            txtLabel.Text = userActionTreeViewItem.UserAction.Label;
            txtLabel.TextChanged += txtLabel_TextChanged;

            _labelChanged.Elapsed += _labelChanged_Elapsed;

            if (_parameterTokenTextStyle == null) SetCodeStyle();
            SetMove();
            SetPicDelay();
            SetBtnSplit();
            SetLinked();
            SetLogEntries();
            LockWindowUpdate(0);
        }

        private void _labelChanged_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (_labelChanged != null) _labelChanged.Stop();
            if (!IsDisposed)
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
            var userAction = _userActionTreeViewItem.UserAction;
            int index, count;

            GetOneBasedIndexAndCount(userAction, out index, out count);

            picMoveUp.Enabled = index != 1;
            picMoveDown.Enabled = index != count;

            decimal value = nudMoveSteps.Value;
            //Move down
            nudMoveSteps.Maximum = count - index;

            //Move up
            int candidate = count - Math.Abs(index - count) - 1;
            if (candidate > nudMoveSteps.Maximum) nudMoveSteps.Maximum = candidate;

            if (nudMoveSteps.Maximum < 1) nudMoveSteps.Maximum = 1;

            if (value > nudMoveSteps.Maximum) value = nudMoveSteps.Maximum;
            if (value < 1) value = 1;

            nudMoveSteps.Minimum = 1;
            nudMoveSteps.Value = value;
        }
        /// <summary>
        /// Takes linked user actions into account.
        /// </summary>
        private void GetOneBasedIndexAndCount(UserAction userAction, out int index, out int count) {
            index = -1;
            count = -1;

            UserAction linkUserAction;
            var linkUserActions = userAction.LinkedToUserActions;
            if (userAction.IsLinked(out linkUserAction))
                if (userAction != linkUserAction) {
                    index = linkUserAction.LinkedToUserActionIndices.IndexOf(userAction.Index) + 1;
                    count = linkUserAction.LinkedToUserActionIndices.Count;
                    return;
                }

            var l = new List<UserAction>(_log.Count);
            foreach (UserAction ua in _log) {
                if (!ua.IsLinked(out linkUserAction) || ua == linkUserAction)
                    l.Add(ua);

                index = l.IndexOf(userAction) + 1;
                count = l.Count;
            }
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

            var userAction = _userActionTreeViewItem.UserAction;
            int index = userAction.Index - 1;

            UserAction linkUserAction;
            userAction.IsLinked(out linkUserAction);

            //Step over link to useractions
            int move = (int)nudMoveSteps.Value;
            if (!down) move *= -1;
            if (!userAction.IsLinked(out linkUserAction) || userAction == linkUserAction) {
                int moveIndex = index;
                if (down) {
                    for (int i = 0; i < nudMoveSteps.Value; i++) {
                        if (++moveIndex == _log.Count) break;

                        var ua = _log[moveIndex] as UserAction;
                        moveIndex += ua.LinkedToUserActionIndices.Count;
                    }

                } else {
                    for (int i = 0; i < nudMoveSteps.Value; i++) {
                        if (--moveIndex == 0) break;

                        var ua = _log[moveIndex] as UserAction;
                        while (ua.IsLinked(out linkUserAction) && ua != linkUserAction) {
                            if (--moveIndex == 0) break;
                            ua = _log[moveIndex] as UserAction;
                        }
                    }
                }

                move = moveIndex - index;
            }

            int newIndex = index + move;

            var toMove = new List<UserAction>();
            toMove.Add(userAction);
            foreach (var ua in userAction.LinkedToUserActions) toMove.Add(ua);

            if (toMove.Count != 0) {
                toMove.Reverse();
                _log.RemoveRangeWithoutInvokingEvent(toMove);
                foreach (var ua in toMove) {
                    if (newIndex > -1 && newIndex < _log.Count)
                        _log.InsertWithoutInvokingEvent(newIndex, ua);
                    else
                        _log.AddWithoutInvokingEvent(ua);
                }

                userAction.LinkedToUserActionIndices.Clear();
                for (int i = 0; i < toMove.Count - 1; i++)
                    userAction.LinkedToUserActionIndices.Add(toMove[i].Index);

                userAction.LinkedToUserActionIndices.Sort();

                _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);

                if (UserActionMoved != null) UserActionMoved(_userActionTreeViewItem, null);
            }
        }
        private void picDelay_Click(object sender, EventArgs e) {
            _userActionTreeViewItem.UserAction.UseDelay = !_userActionTreeViewItem.UserAction.UseDelay;
            _userActionTreeViewItem.UserAction.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            SetPicDelay();
        }
        private void SetPicDelay() {
            if (_userActionTreeViewItem.UserAction.UseDelay) {
                picDelay.Image = global::vApus.Stresstest.Properties.Resources.Delay;
                toolTip.SetToolTip(picDelay, "Click to NOT use delay after this user action.\nDelay is determined in the stresstest settings.");
            } else {
                picDelay.Image = global::vApus.Stresstest.Properties.Resources.IgnoreDelay;
                toolTip.SetToolTip(picDelay, "Click to use delay after this user action.\nDelay is determined in the stresstest settings.");
            }
        }
        private void picCopy_Click(object sender, EventArgs e) {
            ClipboardWrapper.SetDataObject(_userActionTreeViewItem.UserAction.Clone());
        }
        private void btnSplit_Click(object sender, EventArgs e) {
            _userActionTreeViewItem.UserAction.Split();
            if (SplitClicked != null) SplitClicked(this, null);
        }
        private void SetBtnSplit() {
            if (_userActionTreeViewItem == null)
                btnSplit.Enabled = false;
            else
                btnSplit.Enabled = _userActionTreeViewItem.UserAction.Count > 1;
        }
        private void SetLinked() {
            var userAction = _userActionTreeViewItem.UserAction;

            //Check if the user action is not part of a chain of user actions.
            UserAction linkedUserAction;
            _userActionTreeViewItem.UserAction.IsLinked(out linkedUserAction);

            while (flpLink.Controls.Count != 1) {
                var ctrl = flpLink.Controls[0];
                (ctrl.Controls[0] as ComboBox).SelectedIndexChanged -= cboLinkTo_SelectedIndexChanged;
                flpLink.Controls.Remove(ctrl);
            }

            btnMerge.Enabled = userAction.LinkedToUserActionIndices.Count != 0;

            if (linkedUserAction != null && linkedUserAction != userAction) {
                var bogus = GetLinkToCombobox(new UserAction[0]);
                bogus.Enabled = false;
                flpLink.Controls.Add(bogus);
                flpLink.Controls.SetChildIndex(bogus, 0);
                return;
            }

            var canUse = new List<UserAction>();
            var cannotUse = new List<UserAction>();
            cannotUse.Add(userAction);

            foreach (UserAction ua in _log) {
                if (ua.LinkedToUserActionIndices.Count != 0) {
                    foreach (int index in ua.LinkedToUserActionIndices) {
                        var linked = _log[index - 1] as UserAction;
                        if (!cannotUse.Contains(linked))
                            cannotUse.Add(linked);
                    }
                }
            }
            foreach (UserAction ua in _log)
                if (!cannotUse.Contains(ua)) canUse.Add(ua);

            Control cbo = null;
            var arr = canUse.ToArray();
            foreach (int index in userAction.LinkedToUserActionIndices) {
                cbo = GetLinkToCombobox(arr, _log[index - 1] as UserAction);
                flpLink.Controls.Add(cbo);
                flpLink.Controls.SetChildIndex(cbo, flpLink.Controls.Count - 2);
            }
            cbo = GetLinkToCombobox(arr);
            flpLink.Controls.Add(cbo);
            flpLink.Controls.SetChildIndex(cbo, flpLink.Controls.Count - 2);

        }
        private Control GetLinkToCombobox(UserAction[] userActions, UserAction selected = null) {
            var pnl = new Panel();
            pnl.BackColor = Color.Silver;
            pnl.Width = 200;
            pnl.Height = 23;

            var cbo = new ComboBox();
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbo.FlatStyle = FlatStyle.Flat;
            cbo.Font = new Font(this.Font, FontStyle.Bold);
            cbo.Width = 198;
            cbo.Height = 21;
            cbo.Items.Add("<none>");
            if (selected != null && !userActions.Contains(selected))
                cbo.Items.Add(selected);

            cbo.Items.AddRange(userActions);

            if (selected == null) {
                cbo.SelectedIndex = 0;
            } else {
                cbo.SelectedItem = selected;
                cbo.Tag = selected;
            }

            pnl.Controls.Add(cbo);
            cbo.Left = cbo.Top = 1;

            cbo.SelectedIndexChanged += cboLinkTo_SelectedIndexChanged;

            return pnl;
        }
        private void cboLinkTo_SelectedIndexChanged(object sender, EventArgs e) {
            var cbo = sender as ComboBox;
            var ua = cbo.Tag as UserAction;
            if (ua != null)
                _userActionTreeViewItem.UserAction.RemoveFromLink(ua);

            if (cbo.SelectedIndex != 0)
                _userActionTreeViewItem.UserAction.AddToLink(cbo.SelectedItem as UserAction, _linkColors);

            if (LinkedChanged != null) LinkedChanged(this, null);
        }
        private void btnMerge_Click(object sender, EventArgs e) {
            _userActionTreeViewItem.UserAction.MergeLinked();
            if (MergeClicked != null) MergeClicked(this, null);
        }

        private void lbtn_ActiveChanged(object sender, EventArgs e) {
            SetLogEntries();
        }
        private void SetLogEntries() {
            dgvLogEntries.CellValuePushed -= dgvLogEntries_CellValuePushed;
            _cache = new DataTable("Cache");

            dgvLogEntries.Rows.Clear();
            dgvLogEntries.RowCount = 1;
            dgvLogEntries.Columns.Clear();

            _cache.Columns.Add("imageClm");
            if (_log.LogRuleSet.Count == 0)
                _cache.Columns.Add(_log.LogRuleSet.Label);
            else
                foreach (SyntaxItem item in _log.LogRuleSet)
                    _cache.Columns.Add(CheckOptionalSyntaxItem(item) ? item.Label : "*" + item.Label);

            var plainText = new StringBuilder();

            var userAction = _userActionTreeViewItem.UserAction;
            if (lbtnEditable.Active)
                foreach (LogEntry logEntry in userAction) {
                    AddRowToDgv(logEntry.LogEntryString);
                    plainText.AppendLine(logEntry.LogEntryString);
                } else
                foreach (string s in userAction.LogEntryStringsAsImported) {
                    AddRowToDgv(s);
                    plainText.AppendLine(s);
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

            dgvLogEntries.CellValuePushed += dgvLogEntries_CellValuePushed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logEntryString"></param>
        /// <returns>True if has tail.</returns>
        private bool AddRowToDgv(string logEntryString) {
            string delimiter = _log.LogRuleSet.ChildDelimiter;
            var row = new List<string>();
            row.Add(string.Empty);
            while (row.Count != _cache.Columns.Count - 1) {
                int delimiterIndex = logEntryString.IndexOf(delimiter);
                if (delimiterIndex == -1) {
                    row.Add(string.Empty);
                } else {
                    row.Add(logEntryString.Substring(0, delimiterIndex));
                    logEntryString = logEntryString.Substring(delimiterIndex + delimiter.Length);
                }
            }
            row.Add(logEntryString);
            _cache.Rows.Add(row.ToArray());

            return logEntryString.Length != 0;
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

            Point clientPoint = dgvLogEntries.PointToClient(new Point(e.X, e.Y));
            _rowIndexOfItemUnderMouseToDrop = dgvLogEntries.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
            DataGridViewRow row = dgvLogEntries.Rows[_rowIndexOfItemUnderMouseToDrop];

            Color backColor = row.Cells[0].Style.BackColor;
            Color dragOverBackColor = Color.FromArgb(51, 153, 255);
            if (backColor != dragOverBackColor) row.Tag = backColor;
            foreach (DataGridViewCell cell in row.Cells)
                cell.Style.BackColor = dragOverBackColor;

            foreach (DataGridViewRow otherRow in dgvLogEntries.Rows)
                if (otherRow != row && otherRow.Tag != null)
                    foreach (DataGridViewCell cell in otherRow.Cells)
                        cell.Style.BackColor = (Color)otherRow.Tag;
        }
        private void dgvLogEntries_DragDrop(object sender, DragEventArgs e) {
            Point clientPoint = dgvLogEntries.PointToClient(new Point(e.X, e.Y));
            _rowIndexOfItemUnderMouseToDrop = dgvLogEntries.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

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

            SetBtnSplit();
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

                SetBtnSplit();
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
            SetBtnSplit();
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
        public void SetParameters() {
            bool logEntryContainsTokens;
            _log.GetParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out logEntryContainsTokens, false);

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

        public void SetCodeStyle() {
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
