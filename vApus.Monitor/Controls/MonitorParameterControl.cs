/*
 * Copyright 2009 (c) Sizing Servers Lab
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
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;
using vApusSMT.Base;

namespace vApus.Monitor
{
    /// <summary>
    /// This is a standard control to edit a property of the types: string, char, bool, all numeric types and array or list of those.
    /// Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
    /// The type of the property must be one of the above, or else an exception will be thrown. 
    /// Or else you can always make your own control derived from "BaseSolutionComponentPropertyControl".
    /// The value of the property may not be null or an exception will be thrown.
    /// </summary>
    public partial class ParameterControl : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler ValueChanged;

        #region Enums
        public enum ToggleState
        {
            Collapse = 0,
            Expand
        }
        #endregion

        #region Fields
        private Control _commonPropertyControl;
        private TextBox _collapsedTextBox;
        private bool _lock;

        private Parameter _parameter;
        private object _value;
        #endregion

        #region Properties
        /// <summary>
        /// The label given when this control was made.
        /// </summary>
        public string Label
        {
            get { return lblPropertyName.Text; }
        }
        public Parameter Parameter
        {
            get { return _parameter; }
        }
        protected internal object Value
        {
            get { return _value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Only use while designing.
        /// </summary>
        public ParameterControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This is a standard control to edit a property of the types: string, char, bool, all numeric types and array or list of those.
        /// Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
        /// The type of the property must be one of the above, or else an exception will be thrown. 
        /// Or else you can always make your own control derived from "BaseSolutionComponentPropertyControl".
        /// The value of the property may not be null or an exception will be thrown.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyInfo"></param>
        public ParameterControl(Parameter parameter, object value = null)
        {
            InitializeComponent();
            _parameter = parameter;
            _value = value == null ? _parameter.DefaultValue : value;

            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(BaseSolutionComponentStandardPropertyControl_HandleCreated);
        }
        #endregion

        #region Functions
        #region Make CommonPropertyControl
        private void BaseSolutionComponentStandardPropertyControl_HandleCreated(object sender, EventArgs e)
        {
            SetGui();
        }
        private void SetGui()
        {
            lblPropertyName.Text = _parameter.Name;

            rtxtDescription.Text = _parameter.Description;
            if (_parameter.DefaultValue.ToString().Length != 0)
                rtxtDescription.Text += "[Default Value: '" + _parameter.DefaultValue + "']";
            if (!_parameter.Optional)
                rtxtDescription.Text += " [Obligatory]";
            toolTip.SetToolTip(this, rtxtDescription.Text);
            foreach (Control control in this.Controls)
                toolTip.SetToolTip(control, rtxtDescription.Text);
            object value = Value;
            try
            {
                if (value is bool)
                    BoolControl((bool)value);
                else if (value is char)
                    CharControl((char)value);
                else if (value is string)
                    StringControl(value as string, _parameter.Encrypted);
                else if (value is Enum)
                    EnumControl(value as Enum);
                else if (StringUtil.IsNumeric(value))
                    NumericControl(value);
                else if (value is IEnumerable)
                    CollectionControl(value as IEnumerable);
                else
                    throw new ArgumentException("Invalid property type");
                Toggle(ToggleState.Collapse);
                _commonPropertyControl.Enabled = true;
            }
            catch { }
        }
        private void BoolControl(bool value)
        {
            CheckBox chk = new CheckBox();
            chk.Checked = value;
            chk.Text = "[" + (chk.Checked ? "Checked " : "Unchecked ") + "equals " + chk.Checked + "]";
            chk.Dock = DockStyle.Top;
            _commonPropertyControl = chk;
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
            _commonPropertyControl = txt;
            txt.TextChanged += new EventHandler(_ValueChanged);
            txt.Leave += new EventHandler(_commonPropertyControl_Leave);
            txt.KeyUp += new KeyEventHandler(_commonPropertyControl_KeyUp);
        }
        private void StringControl(string value, bool encrypted = false)
        {
            TextBox txt = new TextBox();
            txt.Text = value as string;
            txt.Dock = DockStyle.Fill;
            if (encrypted)
                txt.UseSystemPasswordChar = true;
            _commonPropertyControl = txt;
            txt.TextChanged += new EventHandler(_ValueChanged);
            txt.Leave += new EventHandler(_commonPropertyControl_Leave);
            txt.KeyUp += new KeyEventHandler(_commonPropertyControl_KeyUp);
        }
        private void EnumControl(Enum value)
        {
            ComboBox cbo = new ComboBox();
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbo.FlatStyle = FlatStyle.Flat;
            cbo.BackColor = Color.White;

            Type valueType = value.GetType();
            foreach (Enum e in Enum.GetValues(valueType))
            {
                DescriptionAttribute[] attr = valueType.GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                cbo.Items.Add(attr.Length > 0 ? attr[0].Description : e.ToString());
            }

            DescriptionAttribute[] attr2 = valueType.GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            cbo.SelectedItem = attr2.Length > 0 ? attr2[0].Description : value.ToString();

            cbo.Dock = DockStyle.Fill;
            _commonPropertyControl = cbo;
            cbo.SelectedIndexChanged += new EventHandler(cbo_SelectedIndexChanged);
            cbo.Leave += new EventHandler(_commonPropertyControl_Leave);
            cbo.KeyUp += new KeyEventHandler(_commonPropertyControl_KeyUp);
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
            _commonPropertyControl = nud;
            //Use text changed rather then value changed, value changed is not invoked when the text is changed.
            nud.TextChanged += new EventHandler(_ValueChanged);
            nud.Leave += new EventHandler(_commonPropertyControl_Leave);
            nud.KeyUp += new KeyEventHandler(_commonPropertyControl_KeyUp);
        }
        private void CollectionControl(IEnumerable value)
        {
            Type elementType = value.AsQueryable().ElementType;

            _commonPropertyControl = GetUndefinedCollectionControl(elementType, value);

            //Hard coded for the purpose of simplicity.
            _commonPropertyControl.Height = 170;
            _commonPropertyControl.Dock = DockStyle.Top;
        }
        private Control GetUndefinedCollectionControl(Type elementType, IEnumerable value)
        {
            UndefinedCollectionControl collectionControl = null;
            if (_commonPropertyControl == null || !(_commonPropertyControl is UndefinedCollectionControl))
            {
                collectionControl = new UndefinedCollectionControl(elementType);
                collectionControl.ValueChanged += new EventHandler(collectionControl_ValueChanged);
            }
            else
            {
                collectionControl = _commonPropertyControl as UndefinedCollectionControl;
            }
            collectionControl.SetValue(value);
            return collectionControl;
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
                lblPropertyName.Text = '*' + _parameter.Name;
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
            ComboBox cbo = sender as ComboBox;
            if (cbo.Tag == null || cbo.SelectedIndex != (int)cbo.Tag)
            {
                cbo.Tag = cbo.SelectedIndex;
                SetValue();
            }
        }
        private void collectionControl_ValueChanged(object sender, EventArgs e)
        {
            SetValue();
        }
        private void SetValue()
        {
            if (this.ParentForm != null && !this.ParentForm.Disposing)
            {
                if (!(_commonPropertyControl is UndefinedCollectionControl))
                {
                    _commonPropertyControl.Leave -= _commonPropertyControl_Leave;
                    _commonPropertyControl.KeyUp -= _commonPropertyControl_KeyUp;
                }
                try
                {
                    if (_value is bool)
                    {
                        _value = (_commonPropertyControl as CheckBox).Checked;
                    }
                    else if (_value is char)
                    {
                        _value = (_commonPropertyControl.Text.Length == 0) ? _value = '\0' : (char)_commonPropertyControl.Text[0];
                    }
                    else if (_value is string)
                    {
                        _value = _commonPropertyControl.Text;
                    }
                    else if (_value is Enum)
                    {
                        ComboBox cbo = _commonPropertyControl as ComboBox;
                        Type valueType = _value.GetType();
                        foreach (Enum e in Enum.GetValues(valueType))
                        {
                            DescriptionAttribute[] attr = valueType.GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                            if (cbo.SelectedItem.ToString() == (attr.Length > 0 ? attr[0].Description : e.ToString()))
                            {
                                _value = e;
                                break;
                            }
                        }
                    }
                    else if (StringUtil.IsNumeric(_value))
                    {
                        NumericUpDown nud = _commonPropertyControl as NumericUpDown;
                        _value = ConvertToNumericValue(_value.GetType(), nud.Value);
                    }
                    else if (_value is IEnumerable)
                    {
                        UndefinedCollectionControl collectionControl = _commonPropertyControl as UndefinedCollectionControl;
                        ArrayList arrayList = new ArrayList(collectionControl.Rows.Count - 1);
                        Type elementType = (_value as IEnumerable).AsQueryable().ElementType;

                        if (elementType == typeof(bool))
                            for (int i = 0; i < collectionControl.Rows.Count - 1; i++)
                                arrayList.Add((bool)collectionControl.Rows[i].Cells[0].Value);
                        else if (elementType == typeof(char))
                            for (int i = 0; i < collectionControl.Rows.Count - 1; i++)
                            {
                                string s = collectionControl.Rows[i].Cells[0].Value.ToString();
                                if (s.Length == 0) arrayList.Add('\0'); else arrayList.Add(s[0]);
                            }
                        else if (elementType == typeof(string))
                            for (int i = 0; i < collectionControl.Rows.Count - 1; i++)
                                arrayList.Add(collectionControl.Rows[i].Cells[0].Value as string);
                        else if (elementType.BaseType == typeof(Enum))
                            for (int i = 0; i < collectionControl.Rows.Count - 1; i++)
                                arrayList.Add(Enum.Parse(elementType, collectionControl.Rows[i].Cells[0].Value.ToString()));
                        else if (StringUtil.IsNumericType(elementType))
                            for (int i = 0; i < collectionControl.Rows.Count - 1; i++)
                                arrayList.Add(ConvertToNumericValue(elementType, collectionControl.Rows[i].Cells[0].Value.ToString()));

                        if (_value is Array)
                        {
                            _value = arrayList.ToArray(elementType);
                        }
                        else if (_value is IList)
                        {
                            IList list = _value as IList;
                            list.Clear();
                            for (int i = 0; i < arrayList.Count; i++)
                                list.Add(arrayList[i]);
                            _value = list;
                        }
                    }

                    lblPropertyName.Text = _parameter.Name;
                }
                catch
                {
                    if (_commonPropertyControl is UndefinedCollectionControl)
                        MessageBox.Show("Only items of the right datatype can be added.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Refresh();
                }
                if (!(_commonPropertyControl is UndefinedCollectionControl))
                {
                    _commonPropertyControl.Leave += _commonPropertyControl_Leave;
                    _commonPropertyControl.KeyUp += _commonPropertyControl_KeyUp;
                }

                if (ValueChanged != null)
                    ValueChanged(this, null);
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
            if (_lock)
                return;
            int splitterDistance;
            if (toggleState == ToggleState.Expand)
            {
                this.BackColor = Color.LightBlue;
                split.Panel1.Controls.Remove(_collapsedTextBox);
                split.Panel2Collapsed = (rtxtDescription.Text.Length == 0);
                rtxtDescription.Height = (rtxtDescription.Text.Length == 0) ? 0 : rtxtDescription.Height;
                split.Panel1.Controls.Add(_commonPropertyControl);
                splitterDistance = _commonPropertyControl.Height + split.Panel1.Padding.Top + split.Panel1.Padding.Bottom + _commonPropertyControl.Margin.Bottom + _commonPropertyControl.Margin.Top;

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

                lblPropertyName.Select();
                Application.DoEvents();
                _commonPropertyControl.Select();
            }
            else
            {
                this.BackColor = Color.FromArgb(240, 240, 240);
                split.Panel1.Controls.Remove(_commonPropertyControl);
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
                _collapsedTextBox.MaxLength = int.MaxValue;

                _collapsedTextBox.Dock = DockStyle.Fill;

                _collapsedTextBox.ReadOnly = true;
                _collapsedTextBox.BackColor = Color.White;

                _collapsedTextBox.GotFocus += new EventHandler(_collapsedTextBox_GotFocus);
            }
            if (_parameter.Encrypted)
                _collapsedTextBox.UseSystemPasswordChar = true;

            object value = Value;
            if (value == null)
                value = string.Empty;
            if (value is Enum)
            {
                DescriptionAttribute[] attr = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                _collapsedTextBox.Text = attr.Length != 0 ? attr[0].Description : value.ToString();
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

        #region Lock/Unlock
        /// <summary>
        /// Lock this.
        /// </summary>
        public void Lock()
        {
            Toggle(ToggleState.Collapse);
            _lock = true;
        }
        /// <summary>
        /// Unlock this.
        /// </summary>
        public void Unlock()
        {
            _lock = false;
        }
        #endregion

        private void _collapsedTextBox_GotFocus(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            if (Parent != null)
                foreach (Control control in Parent.Controls)
                    if (control != this && control is ParameterControl)
                        (control as ParameterControl).Toggle(ToggleState.Collapse);
            Toggle(ToggleState.Expand);

            LockWindowUpdate(0);
        }

        #endregion
    }
}
