/**
 * @Name SortableListviewColumnHeader.cs
 * @Purpose A columnheader with support for sorting
 * @Date 19 April 2005, 07:57:06
 * @Author S.Deckers 
 * @Description
 */

using System;
using System.Collections;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary date="16-04-2005, 21:04:26" author="S.Deckers">
    ///     A columnheader with sorting
    /// </summary>
    public class SortableListviewColumnHeader : ColumnHeader
                                                , IComparer {
        #region -- Properties --

        #region -- Column property --

        public int Column { get; set; }

        #endregion

        #region -- ListviewSorter property --

        /// <summary>
        ///     The sorter for this column, standard text
        /// </summary>
        private ListViewSorter _listviewSorter = new ListViewTextCaseInsensitiveSorter();

        public ListViewSorter ListviewSorter {
            get { return (_listviewSorter); }
            set { _listviewSorter = value; }
        }

        #endregion

        #endregion

        #region -- PreviousWidth property --

        public int PreviousWidth { get; set; }

        #endregion

        #region -- LargestSize property --

        /// <summary>
        ///     Size largest item
        /// </summary>
        private float _largestSize = -1.00F;

        public float LargestSize {
            get { return (_largestSize); }
            set { _largestSize = value; }
        }

        #endregion

        /// <summary>
        ///     Delegate sorting to ListviewSorter
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        int IComparer.Compare(object x, object y) {
            try {
                if (ListviewSorter == null) {
                    throw new NotImplementedException("ListviewSorter is null");
                }

                // --- Get items over here ??

                object left = ((ListViewItem)x).SubItems[Column];
                object right = ((ListViewItem)y).SubItems[Column];

                ListView listView = ListView;
                return (ListviewSorter.OnSort(left, right, ListView.Sorting));
            } catch {
                return 0;
            }
        }

        /// <summary date="18-04-2005, 07:04:23" author="S.Deckers">
        ///     ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return
                (string.Format(
                    "Index={0}, Column={1}, SortOrder={2}, Width={3}, PreviousWidth={4}, LargestWidth={5:F3}", Index,
                    Column, ListView.Sorting, Width, PreviousWidth, LargestSize));
        }
    }
}