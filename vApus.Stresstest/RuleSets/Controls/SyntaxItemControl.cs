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
using System.Text;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Stresstest
{
    /// <summary>
    /// </summary>
    public partial class OldSyntaxItemControl : UserControl
    {
        #region Events
        public event EventHandler InputChanged;
        #endregion

        #region Enums
        public enum ToggleState
        {
            Collapse = 0,
            Expand
        }
        #endregion

        #region Fields
        private Control _control;
        private TextBox _collapsedTextBox;
        private SyntaxItem _syntaxItem;
        private string _value;
        #endregion

        #region Properties
        public string Value
        {
            get { return _value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Only use while designing.
        /// </summary>
        public OldSyntaxItemControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyInfo"></param>
        public OldSyntaxItemControl(SyntaxItem syntaxItem, string value)
        {
            InitializeComponent();
            _syntaxItem = syntaxItem;
            _value = value;
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
            lblSyntaxItemLabel.Text = _syntaxItem.Label;
            rtxtDescription.Text = _syntaxItem.Description;
            toolTip.SetToolTip(this, rtxtDescription.Text);
            foreach (Control control in this.Controls)
                toolTip.SetToolTip(control, rtxtDescription.Text);

            if (_syntaxItem.Count > 0 && _syntaxItem[0] is Rule)
            {
                Rule rule = _syntaxItem[0] as Rule;
                switch (rule.ValueType)
                {
                    case Rule.ValueTypes.boolType:
                        bool b;
                        BoolControl(bool.TryParse(_value, out b) ? b : false);
                        break;
                    case Rule.ValueTypes.charType:
                        CharControl(char.Parse(_value));
                        break;
                    case Rule.ValueTypes.decimalType:
                        decimal dec;
                        NumericControl(decimal.TryParse(_value, out dec) ? dec : 0);
                        break;
                    case Rule.ValueTypes.doubleType:
                        double d;
                        NumericControl(double.TryParse(_value, out d) ? d : 0);
                        break;
                    case Rule.ValueTypes.floatType:
                        float f;
                        NumericControl(float.TryParse(_value, out f) ? f : 0);
                        break;
                    case Rule.ValueTypes.intType:
                        int i;
                        NumericControl(int.TryParse(_value, out i) ? i : 0);
                        break;
                    case Rule.ValueTypes.longType:
                        long l;
                        NumericControl(long.TryParse(_value, out l) ? l : 0);
                        break;
                    case Rule.ValueTypes.shortType:
                        short s;
                        NumericControl(short.TryParse(_value, out s) ? s : 0);
                        break;
                    case Rule.ValueTypes.uintType:
                        uint ui;
                        NumericControl(uint.TryParse(_value, out ui) ? ui : 0);
                        break;
                    case Rule.ValueTypes.ulongType:
                        ulong ul;
                        NumericControl(ulong.TryParse(_value, out ul) ? ul : 0);
                        break;
                    case Rule.ValueTypes.ushortType:
                        ushort us;
                        NumericControl(ushort.TryParse(_value, out us) ? us : 0);
                        break;
                    default:
                        StringControl(_value, rule.UsePasswordChar);
                        break;
                }
            }
            else
            {
                StringControl(_value, false);
            }
            Toggle(ToggleState.Collapse);
        }
        private void BoolControl(bool value)
        {
            CheckBox chk = new CheckBox();
            chk.Checked = value;
            chk.Text = "[" + (chk.Checked ? "Checked " : "Unchecked ") + "equals " + chk.Checked + "]";
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
        private void StringControl(string value, bool usePasswordChar)
        {
            TextBox txt = new TextBox();
            txt.UseSystemPasswordChar = usePasswordChar;
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
        #endregion

        #region Notify value change
        private void _ValueChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox)
            {
                CheckBox chk = sender as CheckBox;
                chk.Text = "[" + (chk.Checked ? "Checked " : "Unchecked ") + "equals " + chk.Checked + "]";
                SetValue();
            }
            else
            {
                lblSyntaxItemLabel.Text = '*' + _syntaxItem.Label;
            }
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
        private void SetValue()
        {
            if (_control.Text != _value && !this.ParentForm.Disposing)
            {
                _control.Leave -= _commonPropertyControl_Leave;
                _control.KeyUp -= _commonPropertyControl_KeyUp;
                lblSyntaxItemLabel.Text = _syntaxItem.Label;
                string oldValue = _value;

                try
                {
                    string newValue = string.Empty;
                    if (_syntaxItem.Count > 0 && _syntaxItem[0] is Rule)
                        switch ((_syntaxItem[0] as Rule).ValueType)
                        {
                            case Rule.ValueTypes.boolType:
                                newValue = ((_control as CheckBox).Checked).ToString();
                                break;
                            case Rule.ValueTypes.charType:
                                newValue = (_control.Text.Length == 0) ? string.Empty : _control.Text[0].ToString();
                                break;
                            case Rule.ValueTypes.decimalType:
                            case Rule.ValueTypes.doubleType:
                            case Rule.ValueTypes.floatType:
                            case Rule.ValueTypes.intType:
                            case Rule.ValueTypes.longType:
                            case Rule.ValueTypes.shortType:
                            case Rule.ValueTypes.uintType:
                            case Rule.ValueTypes.ulongType:
                            case Rule.ValueTypes.ushortType:
                                newValue = (_control as NumericUpDown).Value.ToString();
                                break;
                            default:
                                newValue = _control.Text;
                                break;
                        }
                    else
                        newValue = _control.Text;

                    if (_value != newValue)
                    {
                        _value = newValue;
                        if (InputChanged != null)
                            InputChanged(this, null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _value = oldValue;
                    Refresh();
                }
                _control.Leave += _commonPropertyControl_Leave;
                _control.KeyUp += _commonPropertyControl_KeyUp;
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
        /// <summary>
        /// Toggle Expand or collapse.
        /// </summary>
        /// <param name="toggleState"></param>
        public void Toggle(ToggleState toggleState)
        {
            int splitterDistance;
            if (toggleState == ToggleState.Expand)
            {
                split.Panel1.Controls.Remove(_collapsedTextBox);
                split.Panel2Collapsed = (rtxtDescription.Text.Length == 0);
                rtxtDescription.Height = (rtxtDescription.Text.Length == 0) ? 0 : rtxtDescription.Height;
                split.Panel1.Controls.Add(_control);
                splitterDistance = _control.Height + split.Panel1.Padding.Top + split.Panel1.Padding.Bottom + _control.Margin.Bottom + _control.Margin.Top;

                //Ugly but works.
                if (ParentForm.MdiParent == null)
                {
                    ParentForm.TopMost = true;
                    ParentForm.TopMost = false;
                    ParentForm.Activate();
                }
                else
                {
                    ParentForm.MdiParent.TopMost = true;
                    ParentForm.MdiParent.TopMost = false;
                    ParentForm.MdiParent.Activate();
                }

                lblSyntaxItemLabel.Select();
                Application.DoEvents();
                _control.Select();
            }
            else
            {
                split.Panel1.Controls.Remove(_control);
                SetCollapsedTextBoxText();
                split.Panel2Collapsed = true;
                split.Panel1.Controls.Add(_collapsedTextBox);
                splitterDistance = _collapsedTextBox.Height + split.Panel1.Padding.Top + split.Panel1.Padding.Bottom + _collapsedTextBox.Margin.Bottom + _collapsedTextBox.Margin.Top;
            }
            split.Height = split.Panel2Collapsed ? splitterDistance
                         : splitterDistance + split.SplitterWidth + rtxtDescription.Height + rtxtDescription.Margin.Top + rtxtDescription.Margin.Bottom;
            split.SplitterDistance = splitterDistance;
            this.Height = split.Top + split.Height;
        }
        private void SetCollapsedTextBoxText()
        {
            if (_collapsedTextBox == null)
            {
                _collapsedTextBox = new TextBox();
                if (_syntaxItem.Count > 0 && _syntaxItem[0] is Rule)
                    _collapsedTextBox.UseSystemPasswordChar = (_syntaxItem[0] as Rule).UsePasswordChar;

                _collapsedTextBox.ReadOnly = true;
                _collapsedTextBox.BackColor = Color.White;
                _collapsedTextBox.Dock = DockStyle.Fill;
                _collapsedTextBox.GotFocus += new EventHandler(_collapsedTextBox_GotFocus);
            }
            object value = Value;
            if (value.GetParent() != null && value.GetParent() is IEnumerable)
            {
                try
                {
                    _collapsedTextBox.Text = (_control as ComboBox).SelectedItem.ToString();
                }
                catch { }
            }
            else if (value is Enum)
            {
                DescriptionAttribute[] attr = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                _collapsedTextBox.Text = attr.Length > 0 ? attr[0].Description : value.ToString();
            }
            else if (value is string)
            {
                _collapsedTextBox.Text = value as string;
            }
            else if (value is IEnumerable)
            {
                IEnumerable collection = value as IEnumerable;
                IEnumerator enumerator = collection.GetEnumerator();
                StringBuilder sb = new StringBuilder();
                while (enumerator.MoveNext())
                    if (enumerator.Current != null)
                    {
                        sb.Append(enumerator.Current.ToString());
                        sb.Append(", ");
                    }
                _collapsedTextBox.Text = sb.ToString();
                if (_collapsedTextBox.Text.Length != 0)
                    _collapsedTextBox.Text = _collapsedTextBox.Text.Substring(0, _collapsedTextBox.Text.Length - 2);
            }
            else
            {
                _collapsedTextBox.Text = value.ToString();
            }
        }
        #endregion

        public override void Refresh()
        {
            base.Refresh();
            lblSyntaxItemLabel.Text = _syntaxItem.Label;
            rtxtDescription.Text = _syntaxItem.Description;
            toolTip.SetToolTip(this, rtxtDescription.Text);
            foreach (Control control in this.Controls)
                toolTip.SetToolTip(control, rtxtDescription.Text);
            if (_control != null)
            {
                if (_syntaxItem.Count > 0 && _syntaxItem[0] is Rule)
                    switch ((_syntaxItem[0] as Rule).ValueType)
                    {
                        case Rule.ValueTypes.boolType:
                            CheckBox chk = _control as CheckBox;
                            chk.Checked = bool.Parse(_value);
                            chk.Text = "[" + (chk.Checked ? "Checked " : "Unchecked ") + "equals " + chk.Checked + "]";
                            break;
                        case Rule.ValueTypes.charType:
                        case Rule.ValueTypes.decimalType:
                        case Rule.ValueTypes.doubleType:
                        case Rule.ValueTypes.floatType:
                        case Rule.ValueTypes.intType:
                        case Rule.ValueTypes.longType:
                        case Rule.ValueTypes.shortType:
                        case Rule.ValueTypes.uintType:
                        case Rule.ValueTypes.ulongType:
                        case Rule.ValueTypes.ushortType:
                            NumericUpDown nud = _control as NumericUpDown;
                            nud.TextChanged -= _ValueChanged;
                            nud.Value = Convert.ToDecimal(_value);

                            string[] split = nud.Value.ToString().Split(new char[] { '.', ',' });
                            nud.DecimalPlaces = (split.Length == 2) ? split[1].Length : 0;
                            //Stupid .net bug
                            nud.Text = nud.Value.ToString();
                            nud.TextChanged += _ValueChanged;
                            break;
                        default:
                            _control.TextChanged -= _ValueChanged;
                            _control.Text = _value;
                            _control.TextChanged += _ValueChanged;
                            break;
                    }
                else
                    _control.Text = _value;
                SetCollapsedTextBoxText();
            }
        }
        private void _collapsedTextBox_GotFocus(object sender, EventArgs e)
        {
            if (Parent != null)
                foreach (Control control in Parent.Controls)
                    if (control != this && control is OldSyntaxItemControl)
                        (control as OldSyntaxItemControl).Toggle(ToggleState.Collapse);
            Toggle(ToggleState.Expand);
        }
        #endregion
    }
}
