/**
 * @Name ListViewDateSorter.cs
 * @Purpose Sorting for dates
 * @Date 19 April 2005, 08:05:54
 * @Author S.Deckers 
 * @Description
 */
using System;
using System.Windows.Forms;

namespace vApus.Util
{
	/// <summary date="16-04-2005, 22:04:24" author="S.Deckers">
	/// Implements Date sorting
	/// </summary>
	public class ListViewDateSorter : ListViewSorter
	{
		/// <summary>
		/// Sorting
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override int OnSort(object x, object y, System.Windows.Forms.SortOrder sortOrder)
		{
			System.Windows.Forms.ListViewItem.ListViewSubItem left  = GetLeft( x);
			System.Windows.Forms.ListViewItem.ListViewSubItem right = GetLeft( y);

			int res = HandleEmptyStrings( left, right, sortOrder);

			if( res != 0)
			{
				return( res);
			}

			res  = DateTime.Parse( left.Text).CompareTo( DateTime.Parse( right.Text));

			if( sortOrder == SortOrder.Descending)
			{
				res *= -1;
			}

			return( res);
		}
	}
}
