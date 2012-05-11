/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.Util;
using System.ComponentModel;

namespace vApus.SolutionTree
{
    /// <summary>
    /// This is a standard panel to edit a property of the types: string, char, bool, all numeric types and array or list of those.
    /// Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
    /// The type of the property must be one of the above, or else an exception will be thrown. 
    /// Or else you can always make your own control derived from "BaseSolutionComponentPropertyControl".
    /// The value of the property may not be null or an exception will be thrown.
    /// </summary>
    public partial class SolutionComponentPropertyPanel : ValueControlPanel
    {

        #region Fields
        private SolutionComponent _solutionComponent;
        private bool _solutionComponentTypeChanged;
        //Can be sorted, kept here.
        private List<PropertyInfo> _properties;

        //For restoring the parents --> temp solution.
        // private Dictionary<Type, SolutionComponent> _parentCache = new Dictionary<Type, SolutionComponent>();
        #endregion

        /// <summary>
        /// Set the gui if the panel is empty.
        /// </summary>
        public SolutionComponent SolutionComponent
        {
            get { return _solutionComponent; }
            set
            {
                if (_solutionComponent != value)
                {
                    this.ValueChanged -= SolutionComponentPropertyPanel_ValueChanged;

                    _solutionComponentTypeChanged = _solutionComponent == null || _solutionComponent.GetType() != value.GetType();
                    _solutionComponent = value;
                    SetGui();
                    _solutionComponentTypeChanged = false;

                    this.ValueChanged += SolutionComponentPropertyPanel_ValueChanged;
                }
            }
        }

        #region Constructors
        /// <summary>
        /// This is a standard panel to edit a property of the types: string, char, bool, all numeric types and array or list of those.
        /// Furthermore, a single object having a parent of the type IEnumerable (GetParent() in vApus.Util.ObjectExtension), can be displayed also.
        /// The type of the property must be one of the above, or else an exception will be thrown. 
        /// Or else you can always make your own control derived from "BaseSolutionComponentPropertyControl".
        /// The value of the property may not be null or an exception will be thrown.
        /// </summary>
        public SolutionComponentPropertyPanel()
        {
            InitializeComponent();
            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(SolutionComponentPropertyPanel_HandleCreated);

            this.ValueChanged += new EventHandler<ValueChangedEventArgs>(SolutionComponentPropertyPanel_ValueChanged);
        }

        #endregion

        #region Functions
        private void SolutionComponentPropertyPanel_ValueChanged(object sender, ValueControlPanel.ValueChangedEventArgs e)
        {
            if (Solution.ActiveSolution != null)
                SetValue(e.Index, e.NewValue, e.OldValue, true);
        }
        private void SetValue(int index, object newValue, object oldValue, bool invokeEvent)
        {
            //Nothing can be null, this is solved this way.
            if (oldValue is BaseItem && newValue == null)
            {
                if ((oldValue as BaseItem).IsEmpty)
                    return;
                var empty = BaseItem.Empty(oldValue.GetType(), oldValue.GetParent() as SolutionComponent);
                empty.SetParent(oldValue.GetParent());
                _properties[index].SetValue(_solutionComponent, empty, null);
            }
            else
            {
                _properties[index].SetValue(_solutionComponent, newValue, null);
            }
            //Very needed, for when leaving when disposed, or key up == enter while creating.
            if (invokeEvent)
                try
                {
                    //var attributes = _propertyInfo.GetCustomAttributes(typeof(SavableCloneableAttribute), true);
                    //if (attributes.Length != 0)
                    _solutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                }
                catch { }
        }
        private void SolutionComponentPropertyPanel_HandleCreated(object sender, EventArgs e)
        {
            SetGui();
        }
        private void SetGui()
        {
            if (_solutionComponent != null && IsHandleCreated)
            {
                if (_solutionComponentTypeChanged || _properties == null)
                {
                    _properties = new List<PropertyInfo>();
                    foreach (PropertyInfo propertyInfo in _solutionComponent.GetType().GetProperties())
                    {
                        object[] attributes = propertyInfo.GetCustomAttributes(typeof(PropertyControlAttribute), true);
                        PropertyControlAttribute propertyControlAttribute = (attributes.Length == 0) ? null : (attributes[0] as PropertyControlAttribute);
                        if (propertyControlAttribute != null)
                            _properties.Add(propertyInfo);
                    }
                    _properties.Sort(PropertyInfoComparer.GetInstance());
                    _properties.Sort(PropertyInfoDisplayIndexComparer.GetInstance());

                    BaseValueControl.Value[] values = new BaseValueControl.Value[_properties.Count];
                    for (int i = 0; i != values.Length; i++)
                    {
                        PropertyInfo propertyInfo = _properties[i];

                        object value = _properties[i].GetValue(_solutionComponent, null);

                        object[] attributes = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                        string label = (attributes.Length != 0) ? (attributes[0] as DisplayNameAttribute).DisplayName : propertyInfo.Name;

                        attributes = propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                        string description = (attributes.Length != 0) ? (attributes[0] as DescriptionAttribute).Description : string.Empty;

                        attributes = propertyInfo.GetCustomAttributes(typeof(ReadOnlyAttribute), true);
                        bool isReadOnly = !propertyInfo.CanWrite || (attributes.Length > 0 && (attributes[0] as ReadOnlyAttribute).IsReadOnly);

                        attributes = propertyInfo.GetCustomAttributes(typeof(SavableCloneableAttribute), true);
                        bool isEncrypted = (attributes.Length != 0 && (attributes[0] as SavableCloneableAttribute).Encrypt);


                        values[i] = new BaseValueControl.Value { __Value = value, Description = description, IsEncrypted = isEncrypted, IsReadOnly = isReadOnly, Label = label };
                    }

                    base.SetValues(values);
                }
                else //Recycle controls
                {
                    object[] values = new object[_properties.Count];
                    for (int i = 0; i != values.Length; i++)
                        values[i] = _properties[i].GetValue(_solutionComponent, null);

                    base.Set__Values(values);
                }
            }
        }
        /// <summary>
        /// This is used in the solution component view manager, please implement this always.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            SetGui();
        }
        #endregion
    }
}
