/**
 * @Name ListViewInt32Sorter.cs
 * @Purpose Sorting for integers
 * @Date 19 April 2005, 08:05:10
 * @Author S.Deckers 
 * @Description
 */
using System;
using System.Globalization;
using System.Windows.Forms;

namespace vApus.Util
{

	/// <summary date="16-04-2005, 22:04:24" author="S.Deckers">
	/// Implements sorting for integers
	/// </summary>
	public class ListViewInt32Sorter : ListViewSorter
	{
		/// <summary>
		/// Sorting
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override int OnSort( object x, object y, System.Windows.Forms.SortOrder sortOrder)
		{
			System.Windows.Forms.ListViewItem.ListViewSubItem left  = GetLeft( x);
			System.Windows.Forms.ListViewItem.ListViewSubItem right = GetLeft( y);

			// --- Handle empty strings

			int res = HandleEmptyStrings( left, right, sortOrder);

			if( res != 0)
			{
				return( res);
			}

			res = Int32.Parse( left.Text, NumberStyles.Number) - Int32.Parse( right.Text, NumberStyles.Number);
			
			if( sortOrder == SortOrder.Descending)
			{
				res *= -1;
			}

			return( res);
		}
	}
}
