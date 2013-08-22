/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.Collections.Generic;
using System.Reflection;

namespace vApus.SolutionTree
{
    internal class PropertyInfoDisplayIndexComparer : IComparer<PropertyInfo>
    {
        private static readonly PropertyInfoDisplayIndexComparer _propertyInfoDisplayIndexComparer;

        /// <summary>
        ///     Compares the name of the property info's.
        /// </summary>
        static PropertyInfoDisplayIndexComparer()
        {
            _propertyInfoDisplayIndexComparer = new PropertyInfoDisplayIndexComparer();
        }

        private PropertyInfoDisplayIndexComparer()
        {
        }

        public int Compare(PropertyInfo x, PropertyInfo y)
        {
            PropertyControlAttribute xPropertyControlAttribute = GetPropertyControlAttribute(x);
            PropertyControlAttribute yPropertyControlAttribute = GetPropertyControlAttribute(y);
            if (xPropertyControlAttribute == null && yPropertyControlAttribute == null)
                return 0;
            else if (xPropertyControlAttribute == null)
                return (-1).CompareTo(yPropertyControlAttribute.DisplayIndex);
            else if (yPropertyControlAttribute == null)
                return xPropertyControlAttribute.DisplayIndex.CompareTo(-1);
            return xPropertyControlAttribute.DisplayIndex.CompareTo(yPropertyControlAttribute.DisplayIndex);
        }

        /// <summary>
        ///     Compares the name of the property info's.
        /// </summary>
        public static PropertyInfoDisplayIndexComparer GetInstance()
        {
            return _propertyInfoDisplayIndexComparer;
        }

        private PropertyControlAttribute GetPropertyControlAttribute(PropertyInfo info)
        {
            object[] attributes = info.GetCustomAttributes(typeof (PropertyControlAttribute), true);
            return (attributes.Length == 0) ? null : (attributes[0] as PropertyControlAttribute);
        }
    }
}