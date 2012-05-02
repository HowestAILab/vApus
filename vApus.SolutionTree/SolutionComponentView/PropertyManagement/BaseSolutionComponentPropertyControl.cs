/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.SolutionTree
{
    [ToolboxItem(false)]
    public class BaseSolutionComponentPropertyControl : BaseSolutionComponentControl
    {
        private PropertyInfo _propertyInfo;
        private bool _isReadOnly;
        private bool _isEncrypted;
        private object _existingParent = null;

        protected internal object Value
        {
            get { return _propertyInfo.GetValue(_target, null); }
            set
            {
                //Equals is used instead of  ==  because == results in a shallow check (just handles (pointers)).
                if (!Value.Equals(value))
                {
                    _propertyInfo.SetValue(_target, value, null);
                    //Very needed, for when leaving when disposed, or key up == enter while creating.
                    try
                    {
                        //var attributes = _propertyInfo.GetCustomAttributes(typeof(SavableCloneableAttribute), true);
                        //if (attributes.Length != 0)
                            InvokeSolutionComponentEdited();
                    }
                    catch { }
                }
            }
        }
        /// <summary>
        /// A parent that is not null (if any), to be able to display also when the parent was removed from the value.
        /// </summary>
        protected internal object ExistingParent
        {
            get
            {
                object value = Value;
                if (value != null)
                {
                    object p = value.GetParent();
                    if (p != null)
                        _existingParent = p;
                }

                return _existingParent;
            }
        }
        protected internal string DisplayName
        {
            get
            {
                object[] attributes = _propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                return (attributes.Length > 0) ? (attributes[0] as DisplayNameAttribute).DisplayName : _propertyInfo.Name;
            }
        }
        /// <summary>
        /// Returns null if no description is supplied.
        /// </summary>
        protected internal string Description
        {
            get
            {
                object[] attributes = _propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                return (attributes.Length > 0) ? (attributes[0] as DescriptionAttribute).Description : null;
            }
        }
        protected internal bool IsReadOnly
        {
            get { return _isReadOnly; }
        }
        protected internal bool IsEncrypted
        {
            get { return _isEncrypted; }
        }
        public BaseSolutionComponentPropertyControl() { }
        public BaseSolutionComponentPropertyControl(SolutionComponent target, PropertyInfo propertyInfo)
            : base(target)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            _target = target;
            _propertyInfo = propertyInfo;

            _existingParent = Value.GetParent();

            object[] attributes = _propertyInfo.GetCustomAttributes(typeof(ReadOnlyAttribute), true);
            _isReadOnly = !_propertyInfo.CanWrite || (attributes.Length > 0 && (attributes[0] as ReadOnlyAttribute).IsReadOnly);

            attributes = _propertyInfo.GetCustomAttributes(typeof(SavableCloneableAttribute), true);
            _isEncrypted = (attributes.Length > 0 && (attributes[0] as SavableCloneableAttribute).Encrypt);
        }
        /// <summary>
        /// If the new solution component is of the same type as the old one, this control can be recycled.
        /// </summary>
        /// <param name="newTarget"></param>
        /// <param name="propertyInfo"></param>
        public virtual void Recycle(SolutionComponent newTarget, PropertyInfo propertyInfo)
        {
            base.Recycle(newTarget);
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            _target = newTarget;
            _propertyInfo = propertyInfo;

            object[] attributes = _propertyInfo.GetCustomAttributes(typeof(ReadOnlyAttribute), true);
            _isReadOnly = !_propertyInfo.CanWrite || (attributes.Length > 0 && (attributes[0] as ReadOnlyAttribute).IsReadOnly);

            attributes = _propertyInfo.GetCustomAttributes(typeof(SavableCloneableAttribute), true);
            _isEncrypted = (attributes.Length > 0 && (attributes[0] as SavableCloneableAttribute).Encrypt);
        }
        /// <summary>
        /// Lock this.
        /// </summary>
        public virtual void Lock() { throw new Exception("Use this in a derived class"); }
        /// <summary>
        /// Unlock this.
        /// </summary>
        public virtual void Unlock() { throw new Exception("Use this in a derived class"); }
    }
}
