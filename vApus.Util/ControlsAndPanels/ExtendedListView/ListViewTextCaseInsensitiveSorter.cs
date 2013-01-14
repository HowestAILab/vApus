/**
 * @Name ListViewTextCaseInsensitiveSorter.cs
 * @Purpose Implements text case-insensitive sorting
 * @Date 19 April 2005, 08:03:20
 * @Author S.Deckers 
 * @Description
 */

using System;
using System.Windows.Forms;

namespace vApus.Util
{
    /// <summary date="16-04-2005, 22:04:24" author="S.Deckers">
    ///     Implements Text case-insensitive sorting
    /// </summary>
    public class ListViewTextCaseInsensitiveSorter : ListViewSorter
    {
        /// <summary>
        ///     Sorting implementation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int OnSort(object x, object y, SortOrder sortOrder)
        {
            ListViewItem.ListViewSubItem left = GetLeft(x);
            ListViewItem.ListViewSubItem right = GetLeft(y);

            int res = String.Compare(left.Text, right.Text, true);

            if (sortOrder == SortOrder.Descending)
            {
                res *= -1;
            }
            return (res);
        }
    }
}