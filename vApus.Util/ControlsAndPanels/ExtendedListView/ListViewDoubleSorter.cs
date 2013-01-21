/**
 * @Name ListViewDoubleSorter.cs
 * @Purpose Sorting for doubles
 * @Date 19 April 2005, 08:06:45
 * @Author S.Deckers 
 * @Description
 */

using System;
using System.Windows.Forms;

namespace vApus.Util
{
    /// <summary date="18-04-2005, 08:04:33" author="S.Deckers">
    ///     Implements sorting for the 'double'-type
    /// </summary>
    public class ListViewDoubleSorter : ListViewSorter
    {
        /// <summary>
        ///     Sorting
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int OnSort(object x, object y, SortOrder sortOrder)
        {
            ListViewItem.ListViewSubItem left = GetLeft(x);
            ListViewItem.ListViewSubItem right = GetLeft(y);

            int res = HandleEmptyStrings(left, right, sortOrder);

            if (res != 0)
            {
                return (res);
            }

            Double result = Double.Parse(left.Text) - Double.Parse(right.Text);

            if (result > 0)
            {
                res = 1;
            }

            else if (result < 0)
            {
                res = -1;
            }

            else
            {
                res = 0;
            }

            if (sortOrder == SortOrder.Descending)
            {
                res *= -1;
            }

            return (res);
        }
    }
}