/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    ///     This is a standard control to edit a property of the types: string, char, bool, all numeric types and array or list of those.
    ///     Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
    ///     The type of the property must be one of the above, or else an exception will be thrown.
    ///     Or else you can always make your own control derived from "BaseSolutionComponentPropertyControl".
    ///     The value of the property may not be null or an exception will be thrown.
    /// </summary>
    [ToolboxItem(false)]
    public abstract partial class BaseValueControl : UserControl {
        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        #region Enums

        public enum ToggleState {
            Collapse = 0,
            Expand
        }

        #endregion

        #region Fields

        private TextBox _collapsedTextBox;
        private bool _locked;
        private ToggleState _toggleState = ToggleState.Collapse;
        private Value _value;

        #endregion

        #region Properties

        public string Label {
            get { return _value.Label; }
        }

        public string Description {
            get { return _value.Description; }
        }

        public bool Locked {
            get { return _locked; }
        }

        public bool IsReadOnly {
            get { return _value.IsReadOnly; }
        }

        public bool IsEncrypted {
            get { return _value.IsEncrypted; }
        }

        /// <summary>
        ///     This will not fire the value changed event, this event is invoked through user actions.
        /// </summary>
        public Value __Value {
            get { return _value; }
            set {
                _value = value;

                lblLabel.Text = _value.Label == null ? string.Empty : _value.Label;
                ;
                rtxtDescription.Text = string.IsNullOrEmpty(_value.Description) ? string.Empty : _value.Description;
            }
        }

        protected internal object ValueParent {
            get {
                object value = _value.__Value;
                if (value != null) {
                    object p = value.GetParent();
                    if (p != null)
                        return p;
                }
                return null;
            }
        }

        public Control ValueControl {
            get { return split.Panel1.Controls.Count == 0 ? null : split.Panel1.Controls[0]; }
            set {
                split.Panel1.Controls.Clear();
                split.Panel1.Controls.Add(value);

                if (IsEncrypted && _value.__Value is string) {
                    if (value is TextBox)
                        (value as TextBox).UseSystemPasswordChar = true;
                } else {
                    _value.IsEncrypted = false;
                }

                value.Enabled = !IsReadOnly;

                Toggle(_toggleState);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        public BaseValueControl() {
            InitializeComponent();
        }

        #endregion

        #region Functions

        public void HandleValueChanged(object value) {
            SetValue(value);
        }

        public bool HandleKeyUp(Keys key, object value) {
            if (key == Keys.Enter) {
                SetValue(value);
                return true;
            }
            return false;
        }

        private void SetValue(object value) {
            if (value == null && _value.__Value == null)
                return;

            //Revert to the default value if need be.
            if (value.ToString().Length == 0 && _value.DefaultValue != null && _value.DefaultValue.ToString().Length != 0) {
                value = _value.DefaultValue;
                RevertToDefaultValueOnGui();
                SetCollapsedTextBoxText();
            }

            //Equals is used instead of  ==  because == results in a shallow check (just handles (pointers)).
            if (!_value.__Value.Equals(value)) {
                object oldValue = _value.__Value;
                _value.__Value = value;
                try {
                    if (ValueChanged != null)
                        ValueChanged(this, new ValueChangedEventArgs(oldValue, value));
                } catch {
                    if (ValueChanged != null)
                        ValueChanged(this, new ValueChangedEventArgs(oldValue, oldValue));
                }
            }
        }

        /// <summary>
        /// If the tostring of the value is empty, the value is defaulted. This should be present in the gui also, without invoking events to prevent endless loops. 
        /// </summary>
        protected abstract void RevertToDefaultValueOnGui();

        private void _collapsedTextBox_GotFocus(object sender, EventArgs e) {
            if (Parent != null)
                foreach (Control control in Parent.Controls)
                    if (control != this && control is BaseValueControl)
                        (control as BaseValueControl).Toggle(ToggleState.Collapse);
            Toggle(ToggleState.Expand);
        }

        #region Toggle

        /// <summary>
        ///     Toggle Expand or collapse.
        /// </summary>
        /// <param name="toggleState"></param>
        public void Toggle(ToggleState toggleState) {
            int splitterDistance;
            _toggleState = toggleState;
            if (_toggleState == ToggleState.Expand) {
                BackColor = Color.LightBlue;
                if (_locked) {
                    split.Panel2Collapsed = (rtxtDescription.Text.Length == 0);
                    ValueControl.Visible = false;
                    SetCollapsedTextBoxText();
                    if (!split.Panel1.Controls.Contains(_collapsedTextBox))
                        split.Panel1.Controls.Add(_collapsedTextBox);
                    splitterDistance = _collapsedTextBox.Height + split.Panel1.Padding.Top + split.Panel1.Padding.Bottom +
                                       _collapsedTextBox.Margin.Bottom + _collapsedTextBox.Margin.Top;
                } else {
                    split.Panel1.Controls.Remove(_collapsedTextBox);
                    split.Panel2Collapsed = (rtxtDescription.Text.Length == 0);
                    ValueControl.Visible = true;
                    splitterDistance = ValueControl.Height + split.Panel1.Padding.Top + split.Panel1.Padding.Bottom +
                                       ValueControl.Margin.Bottom + ValueControl.Margin.Top;

                    //Ugly but works.
                    if (ParentForm != null)
                        try {
                            if (ParentForm.MdiParent == null) {
                                ParentForm.TopMost = true;
                                ParentForm.TopMost = false;
                                ParentForm.Activate();
                            } else {
                                ParentForm.MdiParent.TopMost = true;
                                ParentForm.MdiParent.TopMost = false;
                                ParentForm.MdiParent.Activate();
                            }
                        } catch {
                        }

                    lblLabel.Select();
                    Application.DoEvents();
                    ValueControl.Select();
                }
            } else {
                BackColor = Color.FromArgb(240, 240, 240);
                ValueControl.Visible = false;
                SetCollapsedTextBoxText();
                split.Panel2Collapsed = true;
                split.Panel1.Controls.Add(_collapsedTextBox);
                splitterDistance = _collapsedTextBox.Height + split.Panel1.Padding.Top + split.Panel1.Padding.Bottom +
                                   _collapsedTextBox.Margin.Bottom + _collapsedTextBox.Margin.Top;
            }
            split.Height = split.Panel2Collapsed
                               ? splitterDistance
                               : splitterDistance + split.SplitterWidth + 52 +
                                 rtxtDescription.Margin.Top + rtxtDescription.Margin.Bottom;
            split.SplitterDistance = splitterDistance;
            Height = split.Top + split.Height;
        }

        private void SetCollapsedTextBoxText() {
            if (_collapsedTextBox == null) {
                _collapsedTextBox = new TextBox();
                _collapsedTextBox.MaxLength = int.MaxValue;

                _collapsedTextBox.Dock = DockStyle.Fill;

                _collapsedTextBox.ReadOnly = true;

                if (IsEncrypted)
                    _collapsedTextBox.UseSystemPasswordChar = true;

                _collapsedTextBox.GotFocus += _collapsedTextBox_GotFocus;
            }
            if (IsEncrypted)
                _collapsedTextBox.UseSystemPasswordChar = true;

            _collapsedTextBox.BackColor = _locked ? Color.FromArgb(240, 240, 240) : Color.White;


            object value = _value.__Value;
            if (value == null) value = string.Empty;

            string text = string.Empty;
            if (value is Enum) {
                var attr =
                    value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false)
                    as DescriptionAttribute[];
                text = attr.Length != 0 ? attr[0].Description : value.ToString();
            } else if (ValueControl is ComboBox) {
                try {
                    text = value.ToString();
                } catch {
                }
            } else if (value is string) {
                text = value as string;
            } else if (value is IEnumerable) {
                var collection = value as IEnumerable;
                IEnumerator enumerator = collection.GetEnumerator();

                var sb = new StringBuilder();
                while (enumerator.MoveNext())
                    if (enumerator.Current != null) {
                        sb.Append(enumerator.Current);
                        sb.Append(", ");
                    }
                text = sb.ToString();
                if (text.Length != 0)
                    text = text.Substring(0, text.Length - 2);
            } else {
                text = value.ToString();
            }
            _collapsedTextBox.Text = text;
        }

        #endregion

        #region Lock/Unlock

        /// <summary>
        ///     Lock this.
        /// </summary>
        public void Lock() {
            _locked = true;
            Toggle(_toggleState);
        }

        /// <summary>
        ///     Unlock this.
        /// </summary>
        public void Unlock() {
            _locked = false;
            Toggle(_toggleState);
        }

        #endregion

        #endregion

        public struct Value {
            public string Description;

            /// <summary>
            ///     Only applicable for strings.
            /// </summary>
            public bool IsEncrypted;

            public bool IsReadOnly;
            public string Label;
            public object __Value;
            public object DefaultValue;

            /// <summary>
            /// Only for integer values.
            /// </summary>
            public int AllowedMinimum, AllowedMaximum;

            public override string ToString() {
                return __Value.ToString();
            }
        }

        public class ValueChangedEventArgs : EventArgs {
            public readonly object NewValue;
            public readonly object OldValue;

            public ValueChangedEventArgs(object oldValue, object newValue) {
                OldValue = oldValue;
                NewValue = newValue;
            }
        }
    }
}