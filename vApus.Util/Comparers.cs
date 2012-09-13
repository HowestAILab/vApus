/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace vApus.Util
{   
    public enum SortBy
    {
        Text = 1,
        ToString = 2,
        Name = 4,
        FullName = 8
    }
    /// <summary>
    /// Compares the name of the property info's.
    /// </summary>
    public class PropertyInfoComparer : IComparer<PropertyInfo>
    {
        private static PropertyInfoComparer _propertyInfoComparer;
        private PropertyInfoComparer()
        { }
        /// <summary>
        /// Compares the name of the property info's.
        /// </summary>
        static PropertyInfoComparer()
        {
            _propertyInfoComparer = new PropertyInfoComparer();
        }
        /// <summary>
        /// Compares the name of the property info's.
        /// </summary>
        public static PropertyInfoComparer GetInstance()
        {
            return _propertyInfoComparer;
        }
        public int Compare(PropertyInfo x, PropertyInfo y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }

    public class ControlComparer : IComparer<Control>
    {
        private SortBy _sortBy;
        private SortOrder _sortOrder;
        public ControlComparer(SortBy sortBy, SortOrder sortOrder)
        {
            _sortBy = sortBy;
            _sortOrder = sortOrder;
        }
        /// <summary>
        /// Compares by text, to string, name, full name or a combination of these. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(Control x, Control y)
        {
            int output = 0;
            for (int i = 0; i < 4; i++)
            {
                //Using a 'binairy and' to see what needs to be compared.
                int containsSortBy = (int)Math.Pow(2, i);
                if (((int)_sortBy & containsSortBy) > 0)
                {
                    switch (containsSortBy)
                    {
                        //Text
                        case 1:
                            output += x.Text.CompareTo(y.Text);
                            break;
                        //ToString
                        case 2:
                            output += x.ToString().CompareTo(y.ToString());
                            break;
                        //Name
                        case 4:
                            output += x.GetType().Name.CompareTo(y.GetType().Name);
                            break;
                        //FullName
                        case 8:
                            output += x.GetType().FullName.CompareTo(y.GetType().FullName);
                            break;
                    }
                }
            }
            if (_sortOrder == SortOrder.Descending)
                output *= -1;
            return output;
        }
    }
}
