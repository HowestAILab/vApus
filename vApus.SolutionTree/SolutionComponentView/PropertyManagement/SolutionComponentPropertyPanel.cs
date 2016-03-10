using RandomUtils.Log;
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.SolutionTree {
    /// <summary>
    ///     This is a standard panel to edit a property of the types: string, char, bool, all numeric types and array or list of those.
    ///     Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
    ///     The type of the property must be one of the above, or else an exception will be thrown.
    ///     Or else you can always make your own control derived from "BaseSolutionComponentPropertyControl".
    ///     The value of the property may not be null or an exception will be thrown.
    /// </summary>
    public partial class SolutionComponentPropertyPanel : ValueControlPanel {

        #region Fields
        private readonly LinkLabel _showHideAdvancedSettings = new LinkLabel();
        private List<PropertyInfo> _properties;
        private bool _showAdvancedSettings;
        private SolutionComponent _solutionComponent;
        private bool _allowInvokingSolutionComponentChangedEvent = true;
        #endregion

        #region Properties
        [DefaultValue(true)]
        public new bool AutoSelectControl {
            get { return base.AutoSelectControl; }
            set { base.AutoSelectControl = value; }
        }
        /// <summary>
        ///     Set the gui if the panel is empty.
        /// </summary>
        public SolutionComponent SolutionComponent {
            get { return _solutionComponent; }
            set {
                if (_solutionComponent != value) {
                    ValueChanged -= SolutionComponentPropertyPanel_ValueChanged;

                    _solutionComponent = value;

                    LockWindowUpdate(Handle);

                    base.ClearValues();
                    SetGui();

                    LockWindowUpdate(IntPtr.Zero);

                    ValueChanged += SolutionComponentPropertyPanel_ValueChanged;
                }
            }
        }

        /// <summary>
        /// Sometimes you do not want this, for instance with temporary solution components.
        /// </summary>
        [DefaultValue(true)]
        public bool AllowInvokingSolutionComponentChangedEvent {
            get { return _allowInvokingSolutionComponentChangedEvent; }
            set { _allowInvokingSolutionComponentChangedEvent = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     This is a standard panel to edit a property of the types: string, char, bool, all numeric types and array or list of those.
        ///     Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
        ///     The type of the property must be one of the above, or else an exception will be thrown.
        ///     Or else you can always make your own control derived from "BaseSolutionComponentPropertyControl".
        ///     The value of the property may not be null or an exception will be thrown.
        /// </summary>
        public SolutionComponentPropertyPanel() {
            InitializeComponent();

            _showHideAdvancedSettings.Text = "Show/Hide advanced settings";
            _showHideAdvancedSettings.AutoSize = true;
            _showHideAdvancedSettings.Click += _showHideAdvancedSettings_Click;
            _showHideAdvancedSettings.KeyUp += _showHideAdvancedSettings_KeyUp;

            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += SolutionComponentPropertyPanel_HandleCreated;

            ValueChanged += SolutionComponentPropertyPanel_ValueChanged;
        }
        #endregion

        #region Functions
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        private void _showHideAdvancedSettings_Click(object sender, EventArgs e) {
            _showAdvancedSettings = !_showAdvancedSettings;
            Refresh();
        }
        private void _showHideAdvancedSettings_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                _showAdvancedSettings = !_showAdvancedSettings;
                Refresh();
            }
        }

        /// <summary>
        /// Generates gui controls for properties with the PropertyControlAttribute.
        /// </summary>
        /// <param name="collapse">not on refresh</param>
        private void SetGui() {
            if (_solutionComponent != null && IsHandleCreated) {
                bool showHideAdvancedSettingsControl = false;

                //Get and sort all valid properties.
                _properties = new List<PropertyInfo>();
                var minAndMaxs = new Dictionary<PropertyInfo, KeyValuePair<int, int>>();
                foreach (PropertyInfo propertyInfo in _solutionComponent.GetType().GetProperties()) {
                    object[] attributes = propertyInfo.GetCustomAttributes(typeof(PropertyControlAttribute), true);
                    PropertyControlAttribute propertyControlAttribute = (attributes.Length == 0) ? null : (attributes[0] as PropertyControlAttribute);
                    if (propertyControlAttribute != null) {
                        if (propertyControlAttribute.AdvancedProperty) {
                            showHideAdvancedSettingsControl = true;
                            if (_showAdvancedSettings) //Show advanced settings only if chosen to.
                                _properties.Add(propertyInfo);
                        } else {
                            _properties.Add(propertyInfo);
                        }

                        minAndMaxs.Add(propertyInfo, new KeyValuePair<int, int>(propertyControlAttribute.AllowedMinimum, propertyControlAttribute.AllowedMaximum));
                    }
                }
                _properties.Sort(PropertyInfoComparer.GetInstance());
                _properties.Sort(PropertyControlAttributeDisplayIndexComparer.GetInstance());

                //Generate BaseValueControl.Values and generate controls.
                var values = new BaseValueControl.Value[_properties.Count];
                for (int i = 0; i != values.Length; i++) {
                    PropertyInfo propertyInfo = _properties[i];

                    object value = _properties[i].GetValue(_solutionComponent, null);

                    object[] attributes = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    string label = (attributes.Length != 0)
                                       ? (attributes[0] as DisplayNameAttribute).DisplayName
                                       : propertyInfo.Name;

                    //for dynamic descriptions you can choose to call SetDescription however usage of the description attribute is adviced.
                    string description = value.GetDescription();
                    if (description == null) {
                        attributes = propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                        description = (attributes.Length != 0) ? (attributes[0] as DescriptionAttribute).Description : string.Empty;
                    }

                    attributes = propertyInfo.GetCustomAttributes(typeof(ReadOnlyAttribute), true);
                    bool isReadOnly = !propertyInfo.CanWrite || (attributes.Length > 0 && (attributes[0] as ReadOnlyAttribute).IsReadOnly);

                    attributes = propertyInfo.GetCustomAttributes(typeof(SavableCloneableAttribute), true);
                    bool isEncrypted = (attributes.Length != 0 && (attributes[0] as SavableCloneableAttribute).Encrypt);

                    var minAndMax = minAndMaxs[propertyInfo];

                    values[i] = new BaseValueControl.Value {
                        __Value = value,
                        Description = description,
                        IsEncrypted = isEncrypted,
                        IsReadOnly = isReadOnly,
                        Label = label,
                        AllowedMinimum = minAndMax.Key,
                        AllowedMaximum = minAndMax.Value
                    };
                }

                base.SetValues(false, values);

                if (_locked)
                    base.Lock();

                if (showHideAdvancedSettingsControl)
                    Controls.Add(_showHideAdvancedSettings);
            }
        }

        private void SolutionComponentPropertyPanel_HandleCreated(object sender, EventArgs e) { SetGui(); }

        private void SolutionComponentPropertyPanel_ValueChanged(object sender, ValueChangedEventArgs e) {
            if (Solution.ActiveSolution != null) SetValue(e.Index, e.NewValue, e.OldValue, _allowInvokingSolutionComponentChangedEvent);
        }

        private void SetValue(int index, object newValue, object oldValue, bool invokeEvent) {
            if (Parent == null || Parent.IsDisposed || Parent.Disposing) return;

            //Nothing can be null, this is solved this way.
            if (oldValue is BaseItem && newValue == null) {
                if ((oldValue as BaseItem).IsEmpty)
                    return;
                BaseItem empty = BaseItem.GetEmpty(oldValue.GetType(), oldValue.GetParent() as SolutionComponent);
                empty.SetParent(oldValue.GetParent());
                _properties[index].SetValue(_solutionComponent, empty, null);
            } else {
                _properties[index].SetValue(_solutionComponent, newValue, null);
            }
            //Very needed, for when leaving when disposed, or key up == enter while creating.
            if (invokeEvent)
                try {
                    _solutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                } catch(Exception ex) {
                    Loggers.Log(Level.Error, "Failed invoking solution component changed.", ex, new object[] { index, newValue, oldValue, invokeEvent});
                }
        }

        /// <summary>
        ///     This is used in the solution component view manager, please use this always.
        /// </summary>
        public override void Refresh() {
            base.Refresh();
            SetGui();
        }
        #endregion
    }
}