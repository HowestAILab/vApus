/**
 * @Name SortableListviewColumnHeader.cs
 * @Purpose A columnheader with support for sorting
 * @Date 19 April 2005, 07:57:06
 * @Author S.Deckers 
 * @Description
 */
using System.Collections;
using System.Windows.Forms;

namespace vApus.Util
{
	/// <summary date="16-04-2005, 21:04:26" author="S.Deckers">
	/// A columnheader with sorting
	/// </summary>
	public class SortableListviewColumnHeader : ColumnHeader
		, IComparer
	{
		#region -- Properties --
		#region -- Column property --
		/// <summary>
		/// The column to sort
		/// </summary>
		private int _column;
		public int Column
		{
			get{	return( _column);  }
			set{	_column = value;   }
		}
		#endregion

		#region -- ListviewSorter property --
		/// <summary>
		/// The sorter for this column, standard text
		/// </summary>
		private ListViewSorter _listviewSorter = new ListViewTextCaseInsensitiveSorter ();
		public ListViewSorter ListviewSorter
		{
			get{	return( _listviewSorter);  }
			set{	_listviewSorter = value;   }
		}
		#endregion
		#endregion

		#region -- PreviousWidth property --
		private int _previousWidth = 0;
		public int PreviousWidth
		{
			get{	return( _previousWidth);  }
			set{	_previousWidth = value;   }
		}
		#endregion

		#region -- LargestSize property --
		/// <summary>
		/// Size largest item
		/// </summary>
		private float _largestSize = -1.00F;
		public float LargestSize
		{
			get{	return( _largestSize);  }
			set{	_largestSize = value;   }
		}
		#endregion

		/// <summary>
		/// Delegate sorting to ListviewSorter
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		int System.Collections.IComparer.Compare( object x, object y)
		{
            try
            {
                if (this.ListviewSorter == null)
                {
                    throw new System.NotImplementedException("ListviewSorter is null");
                }

                // --- Get items over here ??

                object left = ((ListViewItem)x).SubItems[this.Column];
                object right = ((ListViewItem)y).SubItems[this.Column];

                System.Windows.Forms.ListView listView = this.ListView;
                return (this.ListviewSorter.OnSort(left, right, this.ListView.Sorting));
            }
            catch 
            {
                return 0;
            }
		}

		/// <summary date="18-04-2005, 07:04:23" author="S.Deckers">
		/// ToString override
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return( string.Format( "Index={0}, Column={1}, SortOrder={2}, Width={3}, PreviousWidth={4}, LargestWidth={5:F3}",  this.Index, this.Column, this.ListView.Sorting, this.Width, this.PreviousWidth, this.LargestSize));
		}
	}
}
