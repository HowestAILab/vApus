/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.Windows.Forms;

namespace vApus.Util {
    public class DataGridViewImageCellBlank : DataGridViewImageCell {
        public DataGridViewImageCellBlank() : base() { }
        public DataGridViewImageCellBlank(bool valueIsIcon) : base() { }
        public override object DefaultNewRowValue { get { return null; } }
    }
}
