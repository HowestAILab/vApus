/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Generic;
using System.Reflection;

namespace vApus.SolutionTree {
    /// <summary>
    ///     Compares the display index of property control attributes. When gui for properties is generated they can be sorted using this.
    /// </summary>
    internal class PropertyControlAttributeDisplayIndexComparer : IComparer<PropertyInfo> {

        #region Fields
        private static readonly PropertyControlAttributeDisplayIndexComparer _propertyControlAttributeDisplayIndexComparer;
        #endregion

        #region Constructors
        /// <summary>
        ///     Compares the display index of property control attributes. When gui for properties is generated they can be sorted using this.
        /// </summary>
        static PropertyControlAttributeDisplayIndexComparer() { _propertyControlAttributeDisplayIndexComparer = new PropertyControlAttributeDisplayIndexComparer(); }
        private PropertyControlAttributeDisplayIndexComparer() { }
        #endregion

        #region Functions
        /// <summary>
        ///     Compares the display index of property control attributes. When gui for properties is generated they can be sorted using this.
        /// </summary>
        public static PropertyControlAttributeDisplayIndexComparer GetInstance() { return _propertyControlAttributeDisplayIndexComparer; }
        public int Compare(PropertyInfo x, PropertyInfo y) {
            var xPropertyControlAttribute = GetPropertyControlAttribute(x);
            var yPropertyControlAttribute = GetPropertyControlAttribute(y);
            if (xPropertyControlAttribute == null && yPropertyControlAttribute == null)
                return 0;
            else if (xPropertyControlAttribute == null)
                return (-1).CompareTo(yPropertyControlAttribute.DisplayIndex);
            else if (yPropertyControlAttribute == null)
                return xPropertyControlAttribute.DisplayIndex.CompareTo(-1);
            return xPropertyControlAttribute.DisplayIndex.CompareTo(yPropertyControlAttribute.DisplayIndex);
        }
        private PropertyControlAttribute GetPropertyControlAttribute(PropertyInfo info) {
            object[] attributes = info.GetCustomAttributes(typeof(PropertyControlAttribute), true);
            return (attributes.Length == 0) ? null : (attributes[0] as PropertyControlAttribute);
        }
        #endregion
    }
}