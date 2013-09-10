/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// Draws a blank image for replacing the ugly, unneeded 'no image' image if no image is set.
    /// Put an instance of this in the CellTemplate of a DataGridViewImageColumn.
    /// </summary>
    public class DataGridViewImageCellBlank : DataGridViewImageCell {
        public DataGridViewImageCellBlank() : base() { }
        public DataGridViewImageCellBlank(bool valueIsIcon) : base() { }
        public override object DefaultNewRowValue { get { return null; } }
    }
}
