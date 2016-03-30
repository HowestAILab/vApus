/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
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

namespace vApus.StressTest {
    /// <summary>
    /// Says what it does, do not forget to call SetScenario or SetScenarioAndUserAction.
    /// </summary>
    public partial class EditUserActionPanel : UserControl {
        public event EventHandler UserActionMoved, SplitClicked, MergeClicked, LinkedChanged;

        #region Fields
        private static readonly object _lock = new object();

        private const string VBLRn = "<16 0C 02 12n>";
        private const string VBLRr = "<16 0C 02 12r>";

        private Scenario _scenario;

        //This list should be long enough to color user actions grouped together in a 'link'.
        private static int[] _linkColors = { 0x00FF00, 0x0000FF, 0xFF0000, 0x01FFFE, 0xFFA6FE, 0xFFDB66, 0x006401, 0x010067, 0x95003A, 0x007DB5, 0xFF00F6, 0xFFEEE8, 0x774D00, 0x90FB92, 0x0076FF, 0xD5FF00,
                                             0xFF937E, 0x6A826C, 0xFF029D, 0xFE8900, 0x7A4782, 0x7E2DD2, 0x85A900, 0xFF0056, 0xA42400, 0x00AE7E, 0x683D3B, 0xBDC6FF, 0x263400, 0xBDD393, 0x00B917, 0x9E008E,
                                             0x001544, 0xC28C9F, 0xFF74A3, 0x01D0FF, 0x004754, 0xE56FFE, 0x788231, 0x0E4CA1, 0x91D0CB, 0xBE9970, 0x968AE8, 0xBB8800, 0x43002C, 0xDEFF74, 0x00FFC6, 0xFFE502,
                                             0x620E00, 0x008F9C, 0x98FF52, 0x7544B1, 0xB500FF, 0x00FF78, 0xFF6E41, 0x005F39, 0x6B6882, 0x5FAD4E, 0xA75740, 0xA5FFD2, 0xFFB167, 0x009BFF, 0xE85EBE };

        /// <summary>
        /// Show the requests structured.
        /// </summary>
        private DataTable _cache = new DataTable("Cache");
        private System.Timers.Timer _tmr = new System.Timers.Timer(500); //Size columns

        private Rectangle _dragBoxFromMouseDown;
        private int _rowIndexFromMouseDown;
        private int _rowIndexOfItemUnderMouseToDrop;

        private string _beginTokenDelimiter;
        private string _endTokenDelimiter;

        private Parameters _parameters;

        private ParameterTokenTextStyle _plainTextParameterTokenTextStyle, _editViewParameterTokenTextStyle;

        // private System.Timers.Timer _labelChanged = new System.Timers.Timer(500);
        #endregion

        #region Properties
        public UserActionTreeViewItem UserActionTreeViewItem { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Says what it does, do not forget to call SetScenario.
        /// </summary>
        public EditUserActionPanel() {
            InitializeComponent();
            try {
                _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
                _tmr.Elapsed += _tmr_Elapsed;
                SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;

                fctxteditView.DefaultContextMenu(true);
                fctxtxPlainText.DefaultContextMenu(true);

            }
            catch {
                //Should / can never happen.
            }
            this.HandleCreated += EditUserActionPanel_HandleCreated;
        }
        #endregion

        #region Functions
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender is CustomListParameters || sender is CustomListParameter || sender is CustomRandomParameters || sender is CustomRandomParameter
                || sender is NumericParameters || sender is NumericParameter || sender is TextParameters || sender is TextParameter) {
                SetParameters();
                SetCodeStyle();
            }
        }

        private void EditUserActionPanel_HandleCreated(object sender, EventArgs e) {
            this.HandleCreated -= EditUserActionPanel_HandleCreated;
            ParentForm.FormClosing += ParentForm_FormClosing;
            ParentForm.Leave += ParentForm_Leave;

            //Stupid workaround.
            dgvRequests.ColumnHeadersDefaultCellStyle.Font = new Font(dgvRequests.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
        }

        private void ParentForm_FormClosing(object sender, FormClosingEventArgs e) {
            txtLabel.Leave -= txtLabel_Leave;
            ParentForm.FormClosing -= ParentForm_FormClosing;
            ParentForm.Leave -= ParentForm_Leave;
            SetLabelChanged();
        }

        private void ParentForm_Leave(object sender, EventArgs e) { SetLabelChanged(); }

        internal void SetScenario(Scenario scenario) { _scenario = scenario; }
        internal void SetScenarioAndUserAction(Scenario scenario, UserActionTreeViewItem userActionTreeViewItem) {
            LockWindowUpdate(this.Handle);
            _scenario = scenario;
            UserActionTreeViewItem = userActionTreeViewItem;

            cboParameterScope.SelectedIndex = 5;

            txtLabel.Text = userActionTreeViewItem.UserAction.Label;

            if (_plainTextParameterTokenTextStyle == null) SetCodeStyle();
            SetMove();
            SetPicDelay();
            SetBtnSplit();
            SetLinked();
            SetRequests();
            LockWindowUpdate(IntPtr.Zero);
        }

        private void txtLabel_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) SetLabelChanged();
        }
        private void txtLabel_Leave(object sender, EventArgs e) {
            SetLabelChanged();
        }
        private void SetLabelChanged() {
            UserActionTreeViewItem.UserAction.Label = txtLabel.Text;
            UserActionTreeViewItem.SetLabel();
            if (UserActionTreeViewItem.UserAction.Label != txtLabel.Text)
                UserActionTreeViewItem.UserAction.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
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
            MoveUserAction(UserActionTreeViewItem.UserAction, down, (int)nudMoveSteps.Value);
        }
        private void MoveUserAction(UserAction userAction, bool down, int moveSteps) {
            if (moveSteps == 0) return;

            //use the zero based index.
            int index = userAction.Index - 1;

            if (down) {
                for (int i = 0; i < moveSteps; i++)
                    if (++index + userAction.LinkedToUserActionIndices.Count < _scenario.Count)
                        MoveDownOneStep(userAction);
            }
            else {
                //We move the previous user action(s) down, this makes the following logic easier (we don't need a 'move up' logic)
                //linked user actions are taken into account
                UserAction linkUserAction;
                var toMoveDown = new List<UserAction>();
                if (!userAction.IsLinked(out linkUserAction) || userAction == linkUserAction) {
                    for (int i = 0; i < moveSteps; i++) {
                        if (--index == 0) {
                            toMoveDown.Add(_scenario[index] as UserAction);
                            break;
                        }

                        userAction = _scenario[index] as UserAction;

                        while (userAction.IsLinked(out linkUserAction) && userAction != linkUserAction) {
                            if (--index == 0) break;
                            userAction = _scenario[index] as UserAction;
                        }
                        userAction = _scenario[index] as UserAction;
                        toMoveDown.Add(userAction);

                        if (index == 0) break;
                    }
                }
                else if (index != 0) {
                    toMoveDown.Add(_scenario[index - 1] as UserAction);
                }

                foreach (var ua in toMoveDown) MoveDownOneStep(ua);
            }

            _scenario.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);
            if (UserActionMoved != null) UserActionMoved(UserActionTreeViewItem, null);
        }
        private void MoveDownOneStep(UserAction userAction) {
            UserAction linkUserAction;
            UserAction nextUserAction = null;

            //use the zero based index.
            int index = userAction.Index - 1;

            //Step over link to useractions
            int moveIndex = index;
            if (!userAction.IsLinked(out linkUserAction) || userAction == linkUserAction) {
                if (++moveIndex < _scenario.Count) {
                    nextUserAction = _scenario[moveIndex] as UserAction;
                    while (nextUserAction.IsLinked(out linkUserAction) && nextUserAction != linkUserAction) {
                        if (++moveIndex == _scenario.Count) break;
                        nextUserAction = _scenario[moveIndex] as UserAction;
                    }
                }
            }
            else if (++moveIndex < _scenario.Count) {
                nextUserAction = _scenario[moveIndex] as UserAction;
            }
            if (nextUserAction == null) return;

            //Because these indices are one-based this will work out fo the move
            int newIndex = (nextUserAction.Index + nextUserAction.LinkedToUserActionIndices.Count) - (userAction.LinkedToUserActionIndices.Count + 1);

            var toMove = new List<UserAction>();
            toMove.Add(userAction);
            foreach (var ua in userAction.LinkedToUserActions) toMove.Add(ua);

            var toMoveNextUserAction = new List<UserAction>();
            toMoveNextUserAction.Add(nextUserAction);
            foreach (var ua in nextUserAction.LinkedToUserActions) toMoveNextUserAction.Add(ua);

            //Add reversed, no index needs to be updated this way.
            toMove.Reverse();
            _scenario.RemoveRangeWithoutInvokingEvent(toMove);
            foreach (var ua in toMove)
                if (newIndex > -1 && newIndex < _scenario.Count)
                    _scenario.InsertWithoutInvokingEvent(newIndex, ua);
                else
                    _scenario.AddWithoutInvokingEvent(ua);

            //Update the linked indices
            int add = toMoveNextUserAction.Count;
            var linkedIndices = userAction.LinkedToUserActionIndices.ToArray();
            for (int j = 0; j != linkedIndices.Length; j++)
                userAction.LinkedToUserActionIndices[j] = linkedIndices[j] + add;

            int subtract = toMove.Count;
            linkedIndices = nextUserAction.LinkedToUserActionIndices.ToArray();
            for (int j = 0; j != linkedIndices.Length; j++)
                nextUserAction.LinkedToUserActionIndices[j] = linkedIndices[j] - subtract;
        }

        private void SetMove() {
            var userAction = UserActionTreeViewItem.UserAction;
            int index, count;

            GetOneBasedIndexAndCount(userAction, out index, out count);

            picMoveUp.Enabled = index != 1;
            picMoveDown.Enabled = index != count;

            picMoveUp.Image = picMoveUp.Enabled ? global::vApus.StressTest.Properties.Resources.MoveUp : global::vApus.StressTest.Properties.Resources.MoveUpGreyedOut;
            picMoveDown.Image = picMoveDown.Enabled ? global::vApus.StressTest.Properties.Resources.MoveDown : global::vApus.StressTest.Properties.Resources.MoveDownGreyedOut;

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

            var l = new List<UserAction>(_scenario.Count);
            foreach (UserAction ua in _scenario) {
                if (!ua.IsLinked(out linkUserAction) || ua == linkUserAction)
                    l.Add(ua);

                index = l.IndexOf(userAction) + 1;
                count = l.Count;
            }
        }

        private void picCopy_Click(object sender, EventArgs e) { ClipboardWrapper.SetDataObject(UserActionTreeViewItem.UserAction.Clone(_scenario.ScenarioRuleSet, true, false, true, false)); }

        private void SetPicDelay() {
            if (UserActionTreeViewItem.UserAction.UseDelay) {
                picDelay.Image = global::vApus.StressTest.Properties.Resources.Delay;
                toolTip.SetToolTip(picDelay, "Click to NOT use delay after this user action.\nDelay is determined in the stress test settings.");
            }
            else {
                picDelay.Image = global::vApus.StressTest.Properties.Resources.IgnoreDelay;
                toolTip.SetToolTip(picDelay, "Click to use delay after this user action.\nDelay is determined in the stress test settings.");
            }
        }
        private void picDelay_Click(object sender, EventArgs e) {
            UserActionTreeViewItem.UserAction.UseDelay = !UserActionTreeViewItem.UserAction.UseDelay;
            UserActionTreeViewItem.UserAction.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            SetPicDelay();
        }

        private void SetBtnSplit() {
            bool enabled = UserActionTreeViewItem != null;
            if (enabled) {
                enabled = UserActionTreeViewItem.UserAction.Count > 1;
                if (enabled) {
                    UserAction linkUserAction;
                    if (UserActionTreeViewItem.UserAction.IsLinked(out linkUserAction))
                        enabled = false;
                }
            }
            btnSplit.Enabled = enabled;
        }
        private void btnSplit_Click(object sender, EventArgs e) {
            UserActionTreeViewItem.UserAction.Split();
            if (SplitClicked != null) SplitClicked(this, null);
        }

        private void SetLinked() {
            var userAction = UserActionTreeViewItem.UserAction;

            //Check if the user action is not part of a chain of user actions.
            UserAction linkedUserAction;
            UserActionTreeViewItem.UserAction.IsLinked(out linkedUserAction);

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

            foreach (UserAction ua in _scenario) {
                if (ua.LinkedToUserActionIndices.Count != 0) {
                    foreach (int index in ua.LinkedToUserActionIndices) {
                        var linked = _scenario[index - 1] as UserAction;
                        if (!cannotUse.Contains(linked))
                            cannotUse.Add(linked);
                    }
                }
            }
            foreach (UserAction ua in _scenario)
                if (!cannotUse.Contains(ua)) canUse.Add(ua);

            Control cbo = null;
            var arr = canUse.ToArray();
            foreach (int index in userAction.LinkedToUserActionIndices) {
                cbo = GetLinkToCombobox(arr, _scenario[index - 1] as UserAction);
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
            }
            else {
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
                UserActionTreeViewItem.UserAction.RemoveFromLink(ua);

            if (cbo.SelectedIndex != 0)
                UserActionTreeViewItem.UserAction.AddToLink(cbo.SelectedItem as UserAction, _linkColors);

            if (LinkedChanged != null) LinkedChanged(this, null);
        }
        private void btnMerge_Click(object sender, EventArgs e) {
            UserActionTreeViewItem.UserAction.MergeLinked();
            if (MergeClicked != null) MergeClicked(this, null);
        }

        /// <summary>
        /// Editable or As Imported  toggled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtn_ActiveChanged(object sender, EventArgs e) {
            SetRequests();
            SetParameters();
            SetCodeStyle();
        }

        private void SetRequests(bool fillEditView = true) {
            dgvRequests.CellValuePushed -= dgvRequests_CellValuePushed;
            _cache = new DataTable("Cache");

            dgvRequests.Rows.Clear();
            dgvRequests.RowCount = 1;
            dgvRequests.Columns.Clear();

            _cache.Columns.Add("imageClm");
            if (_scenario.ScenarioRuleSet.Count == 0)
                _cache.Columns.Add(_scenario.ScenarioRuleSet.Label);
            else
                foreach (SyntaxItem item in _scenario.ScenarioRuleSet)
                    _cache.Columns.Add(CheckOptionalSyntaxItem(item) ? item.Label : "*" + item.Label);

            var plainText = new StringBuilder();

            var userAction = UserActionTreeViewItem.UserAction;
            if (lbtnEditable.Active)
                foreach (Request request in userAction) {
                    string formattedS = request.RequestString.Replace("\n", VBLRn).Replace("\r", VBLRr);
                    AddRowToDgv(request.RequestString);
                    plainText.AppendLine(formattedS);
                }
            else
                foreach (string s in userAction.RequestStringsAsImported) {
                    string formattedS = s.Replace("\n", VBLRn).Replace("\r", VBLRr);
                    AddRowToDgv(s);
                    plainText.AppendLine(formattedS);
                }

            var imageColumn = new DataGridViewImageColumn();
            imageColumn.HeaderText = string.Empty;
            imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            imageColumn.CellTemplate = new DataGridViewImageCellBlank();
            imageColumn.DefaultCellStyle.NullValue = null;

            dgvRequests.Columns.Add(imageColumn);

            for (int i = 1; i != _cache.Columns.Count; i++) {
                var clm = new DataGridViewTextBoxColumn();
                clm.HeaderText = _cache.Columns[i].ColumnName;
                clm.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRequests.Columns.Add(clm);
            }

            SizeColumns();

            fctxtxPlainText.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);
            fctxtxPlainText.Range.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);

            fctxtxPlainText.TextChanged -= fctxtxPlainText_TextChanged;
            fctxtxPlainText.Text = plainText.ToString().TrimEnd();
            fctxtxPlainText.ClearUndo();
            fctxtxPlainText.TextChanged += fctxtxPlainText_TextChanged;

            btnApply.BackColor = Color.White;
            btnApply.Enabled = false;

            SetEditableOrAsImported();

            dgvRequests.RowCount = dgvRequests.ReadOnly ? _cache.Rows.Count : _cache.Rows.Count + 1;

            lblRequestCount.Text = "[" + UserActionTreeViewItem.UserAction.Count + "]";

            dgvRequests.CellValuePushed += dgvRequests_CellValuePushed;

            if (fillEditView)
                FillEditView();
        }
        private void SizeColumns() {
            if (_tmr != null) {
                _tmr.Stop();
                dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                if (dgvRequests.Columns.Count > 2)
                    _tmr.Start();
            }
        }
        private void _tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            lock (_lock) {
                _tmr.Stop();
                SynchronizationContextWrapper.SynchronizationContext.Send((x) => {
                    int[] widths = new int[dgvRequests.ColumnCount];
                    for (int i = 0; i != widths.Length; i++) {
                        int width = dgvRequests.Columns[i].Width;
                        widths[i] = width > 500 ? 500 : width;
                    }
                    dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    for (int i = 0; i != widths.Length; i++)
                        dgvRequests.Columns[i].Width = widths[i];
                }, null);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestString"></param>
        /// <returns>True if has tail.</returns>
        private bool AddRowToDgv(string requestString) {
            string delimiter = _scenario.ScenarioRuleSet.ChildDelimiter;
            var row = new List<string>();
            row.Add(string.Empty);

            row.AddRange(requestString.Split(new string[] { delimiter }, StringSplitOptions.None));

            while (row.Count < _cache.Columns.Count) row.Add(string.Empty);

            _cache.Rows.Add(row.ToArray());

            return requestString.Length != 0;
        }
        private bool CheckOptionalSyntaxItem(SyntaxItem item) {
            bool optional = item.Optional;
            foreach (var subItem in item)
                if (subItem is SyntaxItem)
                    if (item.Optional) {
                        optional = true;
                        break;
                    }
                    else {
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
                dgvRequests.ReadOnly = fctxtxPlainText.ReadOnly = false;
                dgvRequests.AllowDrop = true;
                dgvRequests.AllowUserToAddRows = true;

                dgvRequests.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
            }
            else {
                btnApply.Visible = btnRevertToImported.Visible = false;
                dgvRequests.ReadOnly = fctxtxPlainText.ReadOnly = true;
                dgvRequests.AllowDrop = false;
                dgvRequests.AllowUserToAddRows = false;

                dgvRequests.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlDarkDark;
            }
        }

        private void dgvRequests_MouseMove(object sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left) {
                // If the mouse moves outside the rectangle, start the drag.
                if (_dragBoxFromMouseDown != Rectangle.Empty && !_dragBoxFromMouseDown.Contains(e.X, e.Y)) {

                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dgvRequests.DoDragDrop(
                    dgvRequests.Rows[_rowIndexFromMouseDown],
                    DragDropEffects.Move);
                }
            }
        }
        private void dgvRequests_MouseDown(object sender, MouseEventArgs e) {
            // Get the index of the item the mouse is below.
            _rowIndexFromMouseDown = dgvRequests.HitTest(e.X, e.Y).RowIndex;
            if (_rowIndexFromMouseDown != -1) {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                _dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                _dragBoxFromMouseDown = Rectangle.Empty;
        }
        private void dgvRequests_DragOver(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.Move;

            Point clientPoint = dgvRequests.PointToClient(new Point(e.X, e.Y));
            _rowIndexOfItemUnderMouseToDrop = dgvRequests.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
            DataGridViewRow row = dgvRequests.Rows[_rowIndexOfItemUnderMouseToDrop];

            Color backColor = row.Cells[0].Style.BackColor;
            Color dragOverBackColor = Color.FromArgb(51, 153, 255);
            if (backColor != dragOverBackColor) row.Tag = backColor;
            foreach (DataGridViewCell cell in row.Cells)
                cell.Style.BackColor = dragOverBackColor;

            foreach (DataGridViewRow otherRow in dgvRequests.Rows)
                if (otherRow != row && otherRow.Tag != null)
                    foreach (DataGridViewCell cell in otherRow.Cells)
                        cell.Style.BackColor = (Color)otherRow.Tag;
        }
        private void dgvRequests_DragDrop(object sender, DragEventArgs e) {
            Point clientPoint = dgvRequests.PointToClient(new Point(e.X, e.Y));
            _rowIndexOfItemUnderMouseToDrop = dgvRequests.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            if (e.Effect == DragDropEffects.Move) {
                var userAction = UserActionTreeViewItem.UserAction;
                var request = userAction[_rowIndexFromMouseDown];

                userAction.RemoveWithoutInvokingEvent(request);
                if (_rowIndexOfItemUnderMouseToDrop >= userAction.Count)
                    userAction.AddWithoutInvokingEvent(request);
                else
                    userAction.InsertWithoutInvokingEvent(_rowIndexOfItemUnderMouseToDrop, request);

                userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Added, true);

                SetRequests();

                foreach (DataGridViewRow row in dgvRequests.Rows)
                    row.Selected = false;
                dgvRequests.Rows[_rowIndexOfItemUnderMouseToDrop].Selected = true;
            }
        }
        private void dgvRequests_CellValuePushed(object sender, DataGridViewCellValueEventArgs e) {
            PushCellValueToRequest(e.RowIndex, e.ColumnIndex, e.Value);
        }
        private void PushCellValueToRequest(int cellRowIndex, int cellColumnIndex, object cellValue) {
            var userAction = UserActionTreeViewItem.UserAction;

            if (cellRowIndex >= userAction.Count) {
                var sb = new StringBuilder();
                for (int i = 1; i < _cache.Columns.Count - 1; i++) {
                    if (i == cellColumnIndex) sb.Append(cellValue);
                    sb.Append(_scenario.ScenarioRuleSet.ChildDelimiter);
                }
                if (cellColumnIndex == _cache.Columns.Count - 1) sb.Append(cellValue);

                string formattedS = sb.ToString().Replace(VBLRn, "\n").Replace(VBLRr, "\r");
                userAction.AddWithoutInvokingEvent(new Request(formattedS));
            }
            else {
                var row = _cache.Rows[cellRowIndex].ItemArray;
                row[cellColumnIndex] = cellValue;

                var sb = new StringBuilder();
                for (int i = 1; i < row.Length - 1; i++) {
                    sb.Append(row[i]);
                    sb.Append(_scenario.ScenarioRuleSet.ChildDelimiter);
                }
                sb.Append(row[row.Length - 1]);

                string formattedS = sb.ToString().Replace(VBLRn, "\n").Replace(VBLRr, "\r");
                (userAction[cellRowIndex] as Request).RequestString = formattedS;
            }

            _scenario.ApplyScenarioRuleSet();

            userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited, true);

            SetBtnSplit();
            SetRequests(false);

            var selectedCells = new List<DataGridViewCell>(dgvRequests.SelectedCells.Count);
            foreach (DataGridViewCell cell in dgvRequests.SelectedCells) selectedCells.Add(cell);
            foreach (var cell in selectedCells) cell.Selected = false;

            dgvRequests.Rows[cellRowIndex].Cells[cellColumnIndex].Selected = true;
            FillEditView();
        }

        private void dgvRequests_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete && dgvRequests.SelectedRows.Count != 0) {
                var userAction = UserActionTreeViewItem.UserAction;
                var toRemove = new List<BaseItem>();
                foreach (DataGridViewRow row in dgvRequests.SelectedRows) {
                    int index = dgvRequests.Rows.IndexOf(row);
                    if (index != userAction.Count) toRemove.Add(userAction[index]);
                }
                userAction.RemoveRangeWithoutInvokingEvent(toRemove);

                userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited, true);

                _scenario.ApplyScenarioRuleSet();

                SetBtnSplit();
                SetRequests();
            }
        }
        private void dgvRequests_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            try {
                if (e.RowIndex < _cache.Rows.Count && e.ColumnIndex < _cache.Columns.Count) {
                    var userAction = UserActionTreeViewItem.UserAction;
                    if (e.ColumnIndex == 0) {
                        var request = userAction[e.RowIndex] as Request;
                        if (request.LexicalResult == LexicalResult.OK) {
                            e.Value = null;
                            dgvRequests.Rows[e.RowIndex].Cells[0].ToolTipText = null;
                        }
                        else {
                            if (request.LexedRequest == null) {
                                e.Value = null;
                                dgvRequests.Rows[e.RowIndex].Cells[0].ToolTipText = null;
                            }
                            else {
                                e.Value = global::vApus.StressTest.Properties.Resources.RequestError;
                                int column = request.LexedRequest.Count;
                                if (column == 0) column = 1;
                                dgvRequests.Rows[e.RowIndex].Cells[0].ToolTipText = request.LexedRequest.Error + " See column " + column + ".";
                            }
                        }
                    }
                    else {
                        e.Value = _cache.Rows[e.RowIndex][e.ColumnIndex];
                    }
                }
            }
            catch {
                //Cell is probably deleted. Logged and tested this.
            }
        }

        private void btnRevertToImported_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to do this?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                var userAction = UserActionTreeViewItem.UserAction;
                userAction.ClearWithoutInvokingEvent();

                foreach (string s in userAction.RequestStringsAsImported)
                    userAction.AddWithoutInvokingEvent(new Request(s));

                userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                _scenario.ApplyScenarioRuleSet();
                SetRequests();
            }
        }
        private void btnShowHideParameterTokens_Click(object sender, EventArgs e) {
            if (btnShowHideParameterTokens.Text == "Show parameter tokens") {
                btnShowHideParameterTokens.Text = "Hide parameter tokens";
                toolTip.SetToolTip(btnShowHideParameterTokens, btnShowHideParameterTokens.Text);

                splitParameterTokens.Panel2Collapsed = false;

                pnlBorderTokens.Width = splitParameterTokens.Panel2.Width - pnlBorderTokens.Left - 9;
                pnlBorderTokens.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

                flpTokens.Width = splitParameterTokens.Panel2.Width - flpTokens.Left - 9;
                flpTokens.Height = splitParameterTokens.Panel2.Height - flpTokens.Top - 43;
                flpTokens.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

                cboParameterScope.Refresh();

            }
            else {
                btnShowHideParameterTokens.Text = "Show Parameter Tokens";
                pnlBorderTokens.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                flpTokens.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                splitParameterTokens.Panel2Collapsed = true;
            }
            toolTip.SetToolTip(btnShowHideParameterTokens, btnShowHideParameterTokens.Text);
        }
        private void split_SplitterMoved(object sender, SplitterEventArgs e) {
            cboParameterScope.Refresh();
        }

        private void fctxtxPlainText_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
            btnApply.BackColor = Color.LawnGreen;
            btnApply.Enabled = true;
        }
        private void btnApply_Click(object sender, EventArgs e) {
            var userAction = UserActionTreeViewItem.UserAction;
            bool changed = false;

            int i = 0;
            foreach (string s in fctxtxPlainText.Lines) {
                string formattedS = s.Replace(VBLRn, "\n").Replace(VBLRr, "\r");
                if (i < userAction.Count) {
                    var request = userAction[i] as Request;
                    if (request.RequestString != formattedS) {
                        request.RequestString = formattedS;
                        changed = true;
                    }
                }
                else {
                    userAction.AddWithoutInvokingEvent(new Request(formattedS));
                    changed = true;
                }
                ++i;
            }

            while (userAction.Count > i) {
                userAction.RemoveWithoutInvokingEvent(userAction[userAction.Count - 1]);
                changed = true;
            }

            if (changed) {
                _scenario.ApplyScenarioRuleSet();
                SetRequests();
                SetCodeStyle();
                SetBtnSplit();
                userAction.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
            btnApply.BackColor = Color.White;
            btnApply.Enabled = false;
        }

        private void cboParameterScope_SelectedIndexChanged(object sender, EventArgs e) {
            SetParameters();
        }
        public void SetParameters() {
            bool requestContainsTokens;
            _scenario.GetParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out requestContainsTokens, false);

            string scopeIdentifier = null;

            flpTokens.Controls.Clear();

            switch (cboParameterScope.SelectedIndex) {
                case 1:
                    scopeIdentifier = ASTNode.SCENARIO_PARAMETER_SCOPE;
                    break;
                case 2:
                    scopeIdentifier = ASTNode.USER_ACTION_PARAMETER_SCOPE;
                    break;
                case 3:
                    scopeIdentifier = ASTNode.REQUEST_PARAMETER_SCOPE;
                    break;
                case 4:
                    scopeIdentifier = ASTNode.LEAF_NODE_PARAMETER_SCOPE;
                    break;
                case 5:
                    scopeIdentifier = ASTNode.ALWAYS_PARAMETER_SCOPE;
                    break;
            }

            if (scopeIdentifier == null) {
                AddKvpsToFlps(ASTNode.SCENARIO_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.USER_ACTION_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.REQUEST_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.LEAF_NODE_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.ALWAYS_PARAMETER_SCOPE);
            }
            else {
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

            var scopeIdentifiers = new[] { ASTNode.ALWAYS_PARAMETER_SCOPE, ASTNode.LEAF_NODE_PARAMETER_SCOPE, ASTNode.REQUEST_PARAMETER_SCOPE,
                ASTNode.USER_ACTION_PARAMETER_SCOPE, ASTNode.SCENARIO_PARAMETER_SCOPE };


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

            fctxteditView.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);
            fctxteditView.Range.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);

            if (_editViewParameterTokenTextStyle != null) {
                _editViewParameterTokenTextStyle.Dispose();
                _editViewParameterTokenTextStyle = null;
            }

            if (_plainTextParameterTokenTextStyle != null) {
                _plainTextParameterTokenTextStyle.Dispose();
                _plainTextParameterTokenTextStyle = null;
            }

            _plainTextParameterTokenTextStyle = new ParameterTokenTextStyle(fctxtxPlainText, GetDelimiters(_scenario.ScenarioRuleSet), clp, np, tp, crp, true);
            _editViewParameterTokenTextStyle = new ParameterTokenTextStyle(fctxteditView, GetDelimiters(_scenario.ScenarioRuleSet), clp, np, tp, crp, true);
        }
        private string[] GetDelimiters(ScenarioRuleSet scenarioRuleSet) {
            var hs = new HashSet<string>();
            if (scenarioRuleSet.ChildDelimiter.Length != 0)
                hs.Add(scenarioRuleSet.ChildDelimiter);

            foreach (BaseItem item in scenarioRuleSet)
                if (item is ScenarioSyntaxItem)
                    foreach (string delimiter in GetDelimiters(item as ScenarioSyntaxItem))
                        hs.Add(delimiter);

            var delimiters = new string[hs.Count];
            hs.CopyTo(delimiters);
            hs = null;

            return delimiters;
        }
        private IEnumerable<string> GetDelimiters(ScenarioSyntaxItem scenarioSyntaxItem) {
            if (scenarioSyntaxItem.ChildDelimiter.Length != 0)
                yield return scenarioSyntaxItem.ChildDelimiter;
            foreach (BaseItem item in scenarioSyntaxItem)
                if (item is ScenarioSyntaxItem)
                    foreach (string delimiter in GetDelimiters(item as ScenarioSyntaxItem))
                        yield return delimiter;
        }

        public void SelectFound(int request, int column, int matchLength) {
            if (!lbtnEditable.Active) lbtnEditable.PerformClick();
            tc.SelectedIndex = 1;
            if (request < fctxtxPlainText.LinesCount) {
                int line = 0, start = 0;
                foreach (char c in fctxtxPlainText.Text) {
                    if (line < request)
                        ++start;
                    if (c == '\n' && ++line >= request)
                        break;
                }

                start += column;
                if (start + matchLength < fctxtxPlainText.Text.Length) {
                    fctxtxPlainText.SelectionStart = start;
                    fctxtxPlainText.SelectionLength = matchLength;

                    fctxtxPlainText.DoSelectionVisible();
                }
                Focus();

                SetParameters();
                SetCodeStyle();
            }
        }

        #region Edit View
        private void chkUseEditView_CheckedChanged(object sender, EventArgs e) { FillEditView(); }

        //Draw the row index
        private void dgvRequests_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e) {
            var dgv = sender as DataGridView;
            int rowIndex = e.RowIndex + 1;
            if (rowIndex != dgv.Rows.Count) {
                var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, dgv.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString(rowIndex.ToString(), this.Font, SystemBrushes.ControlText, headerBounds, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void dgvRequests_CellEnter(object sender, DataGridViewCellEventArgs e) { FillEditView(); }
        private void fctxteditView_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
            if (dgvRequests.CurrentCell != null) {
                btnApplyEditView.BackColor = Color.LawnGreen;
                btnApplyEditView.Enabled = true;
                if (dgvRequests.CurrentCell.ColumnIndex != dgvRequests.Columns.Count - 1 && fctxteditView.Text.Contains(_scenario.ScenarioRuleSet.ChildDelimiter))
                    toolTip.SetToolTip(btnApplyEditView, "Be careful to insert the request delimiter (green-colored '" + _scenario.ScenarioRuleSet.ChildDelimiter + "'), doing this can make requests invalid!\nIf this happens you can fix it in the 'Plain Text'-tab page.");
                else
                    toolTip.SetToolTip(btnApplyEditView, null);
            }
        }
        private void btnApplyEditView_Click(object sender, EventArgs e) { PushCellValueToRequest(dgvRequests.CurrentCell.RowIndex, dgvRequests.CurrentCell.ColumnIndex, fctxteditView.Text); }

        private void FillEditView() {
            try {
                //Sadly enough DIY control composition due to dodgy Winforms/Weifenluo.
                if (chkUseEditView.Checked) {
                    splitStructured.Panel2Collapsed = false;

                    fctxteditView.Width = splitStructured.Panel2.Width - fctxteditView.Left - 6;
                    fctxteditView.Height = splitStructured.Panel2.Height - fctxteditView.Top - 33;
                    fctxteditView.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

                    btnApplyEditView.Top = fctxteditView.Bottom + 3;
                    btnApplyEditView.Left = splitStructured.Panel2.Width / 2 - btnApplyEditView.Width / 2;
                    btnApplyEditView.Anchor = AnchorStyles.Bottom;

                    //Set the text and style.
                    fctxteditView.TextChanged -= fctxteditView_TextChanged;

                    if (dgvRequests.SelectedCells.Count == 1) dgvRequests.CurrentCell = dgvRequests.SelectedCells[0];
                    fctxteditView.Enabled = dgvRequests.CurrentCell != null && dgvRequests.CurrentCell.Value != null && dgvRequests.CurrentCell.ColumnIndex != 0 && dgvRequests.SelectedCells.Count == 1;
                    if (fctxteditView.Enabled) {
                        SetParameters();
                        SetCodeStyle();
                        fctxteditView.Text = dgvRequests.CurrentCell.Value.ToString();
                    }
                    else {
                        toolTip.SetToolTip(btnApplyEditView, null);
                        fctxteditView.Text = string.Empty;
                    }
                    btnApplyEditView.BackColor = Color.White;
                    btnApplyEditView.Enabled = false;

                    fctxteditView.TextChanged += fctxteditView_TextChanged;
                }
                else {
                    fctxteditView.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    splitStructured.Panel2Collapsed = true;
                }
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed filling edit view.", ex);
            }
        }
        #endregion

        #endregion

    }
}
