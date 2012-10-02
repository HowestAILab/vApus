/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    /// <summary>
    /// </summary>
    public partial class SyntaxItemControl : UserControl
    {
        #region Events
        public event EventHandler InputChanged;
        #endregion

        #region Enums
        private enum ToggleState
        {
            Collapsed = 0,
            Expanded
        }
        #endregion

        #region Fields
        private Control _control;
        private TextBox _collapsedTextBox;
        private SyntaxItem _syntaxItem;
        private string _input;
        private string[] _arrayInput;
        #endregion

        #region Properties
        public string Input
        {
            get { return _input; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Only use while designing.
        /// </summary>
        public SyntaxItemControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyInfo"></param>
        public SyntaxItemControl(SyntaxItem syntaxItem, string input)
        {
            InitializeComponent();
            _syntaxItem = syntaxItem;
            _input = input;
            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(BaseSolutionComponentStandardPropertyControl_HandleCreated);
        }
        public SyntaxItemControl(SyntaxItem syntaxItem, string[] input)
        {
            InitializeComponent();
            _syntaxItem = syntaxItem;
            _arrayInput = input;
            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(BaseSolutionComponentStandardPropertyControl_HandleCreated);
        }
        #endregion

        #region Functions
        #region Make Control
        private void BaseSolutionComponentStandardPropertyControl_HandleCreated(object sender, EventArgs e)
        {
            SetGui();
        }
        private void SetGui()
        {
            lblSyntaxItemLabel.Text = _syntaxItem.Label + ':';
            txtDescription.Text = _syntaxItem.Description;
            toolTip.SetToolTip(this, txtDescription.Text);
            foreach (Control control in this.Controls)
                toolTip.SetToolTip(control, txtDescription.Text);

            //if (_input == null)
            //{

            //}
            //else
            //{ 
            //No deeper syntax items, just for rules
            //AND + OR stuff
            //equals or rules --> cbo
            //And equals will not work --> should always be an invalid rule check
            //maybe add input length and do not equal

            //to check what is a rule and what not --> no child delimiter.

            //different value types should also fail

            //if (_syntaxItem.Count == 0 || _syntaxItem.ChildDelimiter.Length != 0)
            //{
            StringControl(_input);
            //}
            //else
            //{

            //}
            //}
            tglCollapse.Checked = false;

            //object value = Value;
            //try
            //{
            //    if (value == null)
            //        throw new ArgumentNullException("value");
            //    else if (value is bool)
            //        BoolControl((bool)value);
            //    else if (value is char)
            //        CharControl((char)value);
            //    else if (value is string)
            //        StringControl(value as string);
            //    else if (StringUtil.IsNumeric(value))
            //        NumericControl(value);
            //    else if (value is IEnumerable)
            //        CollectionControl(value as IEnumerable);
            //    else
            //        throw new ArgumentException("Invalid property type");
            //    tglCollapse.Checked = false;
            //}
            //catch { }
        }
        private void BoolControl(bool value)
        {
            CheckBox chk = new CheckBox();
            chk.Checked = value;
            chk.Text = chk.Checked.ToString();
            chk.Dock = DockStyle.Top;
            _control = chk;
            chk.CheckedChanged += new EventHandler(_ValueChanged);
            chk.Leave += new EventHandler(_commonPropertyControl_Leave);
            chk.KeyUp += new KeyEventHandler(_commonPropertyControl_KeyUp);
        }
        private void CharControl(char value)
        {
            TextBox txt = new TextBox();
            txt.MaxLength = 1;
            txt.Text = value.ToString();
            txt.Dock = DockStyle.Fill;
            _control = txt;
            txt.TextChanged += new EventHandler(_ValueChanged);
            txt.Leave += new EventHandler(_commonPropertyControl_Leave);
            txt.KeyUp += new KeyEventHandler(_commonPropertyControl_KeyUp);
        }
        private void StringControl(string value)
        {
            TextBox txt = new TextBox();
            txt.Text = value as string;
            txt.Dock = DockStyle.Fill;
            _control = txt;
            txt.TextChanged += new EventHandler(_ValueChanged);
            txt.Leave += new EventHandler(_commonPropertyControl_Leave);
            txt.KeyUp += new KeyEventHandler(_commonPropertyControl_KeyUp);
        }
        private void NumericControl(object value)
        {
            NumericUpDown nud = new NumericUpDown();
            string[] split = value.ToString().Split(new char[] { '.', ',' });
            if (split.Length == 2)
                nud.DecimalPlaces = split[1].Length;

            if (value is short)
            {
                nud.Minimum = short.MinValue;
                nud.Maximum = short.MaxValue;
            }
            else if (value is int)
            {
                nud.Minimum = int.MinValue;
                nud.Maximum = int.MaxValue;
            }
            else if (value is long)
            {
                nud.Minimum = long.MinValue;
                nud.Maximum = long.MaxValue;
            }
            else if (value is ushort)
            {
                nud.Minimum = ushort.MinValue;
                nud.Maximum = ushort.MaxValue;
            }
            else if (value is uint)
            {
                nud.Minimum = uint.MinValue;
                nud.Maximum = uint.MaxValue;
            }
            else if (value is ulong)
            {
                nud.Minimum = ulong.MinValue;
                nud.Maximum = ulong.MaxValue;
            }
            else
            {
                nud.Minimum = decimal.MinValue;
                nud.Maximum = decimal.MaxValue;
            }

            nud.Value = Convert.ToDecimal(value);
            nud.Dock = DockStyle.Fill;
            _control = nud;
            //Use text changed rather then value changed, value changed is not invoked when the text is changed.
            nud.TextChanged += new EventHandler(_ValueChanged);
            nud.Leave += new EventHandler(_commonPropertyControl_Leave);
            nud.KeyUp += new KeyEventHandler(_commonPropertyControl_KeyUp);
        }
        private void CollectionControl(IEnumerable value)
        {
            Type elementType = value.AsQueryable().ElementType;

            DataGridView dataGridView = (_control == null) ? CreateDataGridView(elementType) : _control as DataGridView;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.CellValueChanged -= dataGridView_CellValueChanged;
            dataGridView.RowsRemoved -= dataGridView_RowsRemoved;
            dataGridView.Rows.Clear();

            IEnumerator enumerator = value.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.Add(CreateDataGridViewCell(enumerator.Current));
                dataGridView.Rows.Add(row);
            }
            dataGridView.Dock = DockStyle.Top;
            _control = dataGridView;
            //Hard coded for the purpose of simplicity.
            _control.Height = 170;
            dataGridView.BackgroundColor = split.BackColor;

            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            dataGridView.RowsRemoved += new DataGridViewRowsRemovedEventHandler(dataGridView_RowsRemoved);
        }
        private DataGridView CreateDataGridView(Type elementType)
        {
            DataGridView dataGridView = new DataGridView();
            dataGridView.ColumnHeadersVisible = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DataGridViewColumn column = null;
            if (elementType == typeof(bool))
                column = new DataGridViewCheckBoxColumn();
            else if (elementType == typeof(char))
            {
                column = new DataGridViewTextBoxColumn();
                (column as DataGridViewTextBoxColumn).MaxInputLength = 1;
            }
            else if (elementType.BaseType == typeof(Enum))
            {
                column = new DataGridViewComboBoxColumn();
                (column as DataGridViewComboBoxColumn).DataSource = Enum.GetValues(elementType);
            }
            else if (elementType == typeof(string) || StringUtil.IsNumericType(elementType))
                column = new DataGridViewTextBoxColumn();

            dataGridView.Columns.Add(column);
            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            return dataGridView;
        }
        private DataGridViewCell CreateDataGridViewCell(object value)
        {
            DataGridViewCell cell;
            if (value is bool)
                cell = new DataGridViewCheckBoxCell();
            else if (value is char)
                cell = new DataGridViewTextBoxCell();
            else if (value is Enum)
            {
                DataGridViewComboBoxCell cboCell = new DataGridViewComboBoxCell();
                Type valueType = value.GetType();
                foreach (Enum e in Enum.GetValues(valueType))
                {
                    DescriptionAttribute[] attr = valueType.GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                    cboCell.Items.Add(attr.Length > 0 ? attr[0].Description : e.ToString());
                }
                cell = cboCell;
            }
            else if (value is string || StringUtil.IsNumeric(value))
                cell = new DataGridViewTextBoxCell();
            else
                throw new InvalidCastException("elementType");
            cell.Value = value;
            return cell;
        }
        #endregion

        #region Notify value change
        private void _ValueChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox)
            {
                CheckBox chk = sender as CheckBox;
                chk.Text = chk.Checked.ToString();
            }
            lblSyntaxItemLabel.Text = '*' + _syntaxItem.Label + ':';
        }
        #endregion

        #region Set value
        private void _commonPropertyControl_Leave(object sender, EventArgs e)
        {
            SetValue();
        }
        private void _commonPropertyControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SetValue();
            else if (e.KeyCode == Keys.Escape)
                Refresh();
        }
        private void cbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetValue();
        }
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            SetValue();
        }
        private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            SetValue();
        }
        private void SetValue()
        {
            if (!this.ParentForm.Disposing)
            {
                if (!(_control is DataGridView))
                {
                    _control.Leave -= _commonPropertyControl_Leave;
                    _control.KeyUp -= _commonPropertyControl_KeyUp;
                }
                try
                {
                    if (_input != (_control as TextBox).Text.Trim())
                    {
                        _input = (_control as TextBox).Text.Trim();
                        if (InputChanged != null)
                            InputChanged(this, null);
                    }
                    //object value = Value;
                    //if (value is bool)
                    //{
                    //    Value = (_control as CheckBox).Checked;
                    //}
                    //else if (value is char)
                    //{
                    //    Value = (_control.Text.Length == 0) ? Value = '\0' : (char)_control.Text[0];
                    //}
                    //else if (value is string)
                    //{
                    //    Value = _control.Text;
                    //}
                    //else if (StringUtil.IsNumeric(value))
                    //{
                    //    NumericUpDown nud = _control as NumericUpDown;
                    //    Value = ConvertToNumericValue(value.GetType(), nud.Value);
                    //}
                    //else if (value is IEnumerable)
                    //{
                    //    DataGridView dataGridView = _control as DataGridView;
                    //    ArrayList arrayList = new ArrayList(dataGridView.Rows.Count - 1);
                    //    Type elementType = (value as IEnumerable).AsQueryable().ElementType;

                    //    if (elementType == typeof(bool))
                    //        for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                    //            arrayList.Add((bool)dataGridView.Rows[i].Cells[0].Value);
                    //    else if (elementType == typeof(char))
                    //        for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                    //        {
                    //            string s = dataGridView.Rows[i].Cells[0].Value.ToString();
                    //            if (s.Length == 0) arrayList.Add('\0'); else arrayList.Add(s[0]);
                    //        }
                    //    else if (elementType == typeof(string))
                    //        for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                    //            arrayList.Add(dataGridView.Rows[i].Cells[0].Value as string);
                    //    else if (elementType.BaseType == typeof(Enum))
                    //        for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                    //            arrayList.Add(Enum.Parse(elementType, dataGridView.Rows[i].Cells[0].Value.ToString()));
                    //    else if (StringUtil.IsNumericType(elementType))
                    //        for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                    //            arrayList.Add(ConvertToNumericValue(elementType, dataGridView.Rows[i].Cells[0].Value.ToString()));

                    //    if (value is Array)
                    //        Value = arrayList.ToArray(elementType);
                    //    else if (value is IList)
                    //    {
                    //        IList list = value as IList;
                    //        list.Clear();
                    //        for (int i = 0; i < arrayList.Count; i++)
                    //            list.Add(arrayList[i]);
                    //        Value = list;
                    //    }
                    //}
                }
                catch
                {
                    Refresh();
                }
                if (!(_control is DataGridView))
                {
                    _control.Leave += _commonPropertyControl_Leave;
                    _control.KeyUp += _commonPropertyControl_KeyUp;
                }
            }
        }
        private object ConvertToNumericValue(Type numericValueType, string s)
        {
            return ConvertToNumericValue(numericValueType, double.Parse(s));
        }
        private object ConvertToNumericValue(Type numericValueType, object o)
        {
            if (numericValueType == typeof(short))
                return Convert.ToInt16(o);
            else if (numericValueType == typeof(int))
                return Convert.ToInt32(o);
            else if (numericValueType == typeof(long))
                return Convert.ToInt64(o);
            else if (numericValueType == typeof(ushort))
                return Convert.ToUInt16(o);
            else if (numericValueType == typeof(uint))
                return Convert.ToUInt32(o);
            else if (numericValueType == typeof(ulong))
                return Convert.ToUInt64(o);
            else if (numericValueType == typeof(float))
                return Convert.ToSingle(o);
            else if (numericValueType == typeof(double))
                return Convert.ToDouble(o);
            else if (numericValueType == typeof(decimal))
                return Convert.ToDecimal(o);
            throw new InvalidCastException("numericValueType");
        }
        #endregion

        #region Toggle
        private void tglCollapse_CheckedChanged(object sender, EventArgs e)
        {
            int splitterDistance;
            split.Panel1.Controls.Clear();
            if (tglCollapse.Checked)
            {
                tglCollapse.BackColor = Color.LightGray;
                split.Panel2Collapsed = (txtDescription.Text == null);
                txtDescription.Height = (txtDescription.Text == null) ? 0 : txtDescription.Height;
                split.Panel1.Controls.Add(_control);
                splitterDistance = _control.Height + split.Panel1.Padding.Top + split.Panel1.Padding.Bottom + _control.Margin.Bottom + _control.Margin.Top;
                _control.Select();
            }
            else
            {
                tglCollapse.BackColor = tglCollapse.Parent.BackColor;
                SetCollapsedTextBoxText();
                split.Panel2Collapsed = true;
                split.Panel1.Controls.Add(_collapsedTextBox);
                splitterDistance = _collapsedTextBox.Height + split.Panel1.Padding.Top + split.Panel1.Padding.Bottom + _collapsedTextBox.Margin.Bottom + _collapsedTextBox.Margin.Top;
                _collapsedTextBox.Select();
            }
            split.Height = split.Panel2Collapsed ? splitterDistance
                         : splitterDistance + split.SplitterWidth + txtDescription.Height + txtDescription.Margin.Top + txtDescription.Margin.Bottom;
            split.SplitterDistance = splitterDistance;
            this.Height = split.Top + split.Height;
        }
        private void SetCollapsedTextBoxText()
        {
            if (_collapsedTextBox == null)
            {
                _collapsedTextBox = new TextBox();
                _collapsedTextBox.ReadOnly = true;
                _collapsedTextBox.BackColor = SystemColors.Info;
                _collapsedTextBox.Dock = DockStyle.Fill;
            }
            _collapsedTextBox.Text = _input;
            //object value = Value;
            //if (value is string)
            //{
            //    _collapsedTextBox.Text = value as string;
            //}
            //else if (value is IEnumerable)
            //{
            //    IEnumerable collection = value as IEnumerable;
            //    IEnumerator enumerator = collection.GetEnumerator();
            //    StringBuilder sb = new StringBuilder();
            //    if (collection.AsQueryable().ElementType.BaseType == typeof(Enum))
            //        while (enumerator.MoveNext())
            //        {
            //            sb.Append(enumerator.Current.ToString());
            //            sb.Append(';');
            //        }
            //    else
            //        while (enumerator.MoveNext())
            //        {
            //            sb.Append(enumerator.Current.ToString());
            //            sb.Append(';');
            //        }
            //    _collapsedTextBox.Text = sb.ToString();
            //    if (_collapsedTextBox.Text.Length != 0)
            //        _collapsedTextBox.Text = _collapsedTextBox.Text.Substring(0, _collapsedTextBox.Text.Length - 1);
            //}
            //else
            //{
            //    _collapsedTextBox.Text = value.ToString();
            //}
        }
        #endregion

        #region Lock/Unlock
        /// <summary>
        /// Lock this.
        /// </summary>
        public void Lock()
        {
            tglCollapse.Checked = false;
            tglCollapse.Enabled = false;
        }
        /// <summary>
        /// Unlock this.
        /// </summary>
        public void Unlock()
        {
            tglCollapse.Enabled = true;
        }
        #endregion

        public override void Refresh()
        {
            base.Refresh();
            lblSyntaxItemLabel.Text = _syntaxItem.Label + ':';
            txtDescription.Text = _syntaxItem.Description;
            toolTip.SetToolTip(this, txtDescription.Text);
            foreach (Control control in this.Controls)
                toolTip.SetToolTip(control, txtDescription.Text);
        }
        #endregion
    }
}
