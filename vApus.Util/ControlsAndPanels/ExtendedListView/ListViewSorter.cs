/**
 * @Name ListViewSorter.cs
 * @Purpose Abstract base class for sorting
 * @Date 19 April 2005, 08:00:50
 * @Author S.Deckers 
 * @Description This class defines an interface for the the 'OnSort'-method which should
 * be implemented by specialized classes. It also has implementations for some helper methods
 */
using System;
using System.Windows.Forms;

namespace vApus.Util
{
	/// <summary>
	/// A sorter
	/// </summary>
	abstract public class ListViewSorter
	{
		/// <summary>
		/// Internal sorting. Specialized objects should provide an implementation for this
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public virtual int OnSort( object x, object y, System.Windows.Forms.SortOrder sortOrder)
		{
			throw new System.NotImplementedException( "ListviewSorter is sorting");
		}

		/// <summary date="18-04-2005, 07:04:42" author="S.Deckers">
		/// Get left element for sorting
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public System.Windows.Forms.ListViewItem.ListViewSubItem GetLeft( Object x)
		{
			System.Windows.Forms.ListViewItem.ListViewSubItem item = x as System.Windows.Forms.ListViewItem.ListViewSubItem;
			return( item);
		}

		/// <summary date="18-04-2005, 07:04:38" author="S.Deckers">
		/// Get right element for sorting
		/// </summary>
		/// <param name="y"></param>
		/// <returns></returns>
		public System.Windows.Forms.ListViewItem.ListViewSubItem GetRight( Object y)
		{
			System.Windows.Forms.ListViewItem.ListViewSubItem item = y as System.Windows.Forms.ListViewItem.ListViewSubItem;
			return( item);
		}
	
		/// <summary date="18-04-2005, 10:04:12" author="S.Deckers">
		/// Handle emptystrings
		/// </summary>
		/// <param name="left">Left object</param>
		/// <param name="right">Right object</param>
		/// <param name="sortOrder">Sortorder</param>
		/// <returns>0 if no empty string was found, otherwise an indication which item was greater</returns>
		protected int HandleEmptyStrings
			( 
			System.Windows.Forms.ListViewItem.ListViewSubItem left, 
			System.Windows.Forms.ListViewItem.ListViewSubItem right,
			System.Windows.Forms.SortOrder sortOrder
			)
		{
			if( sortOrder == SortOrder.Ascending)
			{
				if( left.Text == string.Empty )
				{
					return( -1);
				}

				if( right.Text == string.Empty)
				{
					return( 1);
				}
			}

			if( sortOrder == SortOrder.Descending)
			{
				if( left.Text == string.Empty )
				{
					return( 1);
				}

				if( right.Text == string.Empty)
				{
					return( -1);
				}
			}

			// --- No empty strings found (20050418 SDE)

			return( 0);
		}
	}
}
