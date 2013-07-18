//http://www.codeproject.com/cs/miscctrl/ListViewEmbeddedControls.asp

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vApus.Util {
    public class ExtendedListView : ListView {
        private readonly List<EmbeddedControl> _embeddedControls = new List<EmbeddedControl>();
        private bool _sorting;

        #region Interop-Defines

        // ListView messages
        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59);

        // Windows Messages
        private const int WM_PAINT = 0x000F;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wPar, IntPtr lPar);

        #endregion

        public ExtendedListView() {
            ColumnClick += ExtendedListView_ColumnClick;
        }

        /// <summary>Return the list of al the controls embedded in this listview.</summary>
        public List<Control> EmbeddedControls {
            get {
                var controls = new List<Control>(_embeddedControls.Count);
                foreach (EmbeddedControl embeddedControl in _embeddedControls)
                    controls.Add(embeddedControl.Control);
                return controls;
            }
        }

        [DefaultValue(View.LargeIcon)]
        public new View View {
            get { return base.View; }
            set {
                // Embedded controls are rendered only when we're in Details mode
                foreach (EmbeddedControl ec in _embeddedControls)
                    ec.Control.Visible = (value == View.Details);

                base.View = value;
            }
        }

        /// <summary>
        ///     Adds a control to the ListView.
        /// </summary>
        /// <param name="control">Control to be added.</param>
        /// <param name="column">Column index.</param>
        /// <param name="row">Row index.</param>
        public void AddEmbeddedControl(Control control, int column, int row) {
            AddEmbeddedControl(control, column, row, DockStyle.Fill);
        }

        /// <summary>
        ///     Adds a control to the ListView.
        /// </summary>
        /// <param name="control">Control to be added.</param>
        /// <param name="column">Column index.</param>
        /// <param name="row">Row index.</param>
        /// <param name="dock">Location and resize behavior of embedded control.</param>
        public void AddEmbeddedControl(Control control, int column, int row, DockStyle dock) {
            if (control == null)
                throw new ArgumentNullException();
            if (column >= Columns.Count || row >= Items.Count)
                throw new ArgumentOutOfRangeException();

            EmbeddedControl embeddedControl;
            embeddedControl.Control = control;
            embeddedControl.Column = column;
            embeddedControl.Row = row;
            embeddedControl.Dock = dock;
            embeddedControl.Item = Items[row];

            _embeddedControls.Add(embeddedControl);

            // Add a Click event handler to select the ListView row when an embedded control is clicked
            control.Click += EmbeddedControl_Click;
            control.SizeChanged += control_SizeChanged;

            Controls.Add(control);
        }

        /// <summary>
        ///     Removes a control from the ListView.
        /// </summary>
        /// <param name="control">
        ///     Control to be removed.
        ///     <./param>
        public void RemoveEmbeddedControl(Control control) {
            if (control == null)
                throw new ArgumentNullException();

            for (int i = 0; i < _embeddedControls.Count; i++) {
                EmbeddedControl embeddedControl = _embeddedControls[i];
                if (embeddedControl.Control == control) {
                    control.Click -= EmbeddedControl_Click;
                    Controls.Remove(control);
                    _embeddedControls.RemoveAt(i);
                    break;
                }
            }
            throw new Exception("Control not found!");
        }

        /// <summary>
        /// </summary>
        public void ClearEmbeddedControls() {
            for (int i = 0; i < _embeddedControls.Count; i++) {
                EmbeddedControl embeddedControl = _embeddedControls[i];

                embeddedControl.Control.Click -= EmbeddedControl_Click;
                Controls.Remove(embeddedControl.Control);
            }
            _embeddedControls.Clear();
        }

        /// <summary>
        ///     Retrieve the control embedded at a given location.
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="row">Row index.</param>
        /// <returns>Control found at given location or null if none assigned.</returns>
        public Control GetEmbeddedControl(int column, int row) {
            foreach (EmbeddedControl embeddedControl in _embeddedControls)
                if (embeddedControl.Row == row && embeddedControl.Column == column)
                    return embeddedControl.Control;

            return null;
        }

        /// <summary>
        ///     Retrieves the bounds of a ListViewSubItem.
        /// </summary>
        /// <param name="item">The Item containing the subItem.</param>
        /// <param name="subItem">Index of the subItem.</param>
        /// <returns>Subitem's bounds.</returns>
        protected Rectangle GetSubItemBounds(ListViewItem item, int subItem) {
            Rectangle subItemRectangle = Rectangle.Empty;

            if (item == null)
                throw new ArgumentNullException("Item");

            int[] order = GetColumnOrder();
            if (order == null) // No Columns
                return subItemRectangle;

            if (subItem >= order.Length)
                throw new IndexOutOfRangeException("SubItem " + subItem + " out of range");

            // Retrieve the bounds of the entire ListViewItem (all subitems)
            Rectangle lviBounds = item.GetBounds(ItemBoundsPortion.Entire);
            int subItemX = lviBounds.Left;

            // Calculate the X position of subItem.
            // Because the columns can be reordered we have to use Columns[order[i]] instead of Columns[i] !
            ColumnHeader col;
            int i;
            for (i = 0; i < order.Length; i++) {
                col = Columns[order[i]];
                if (col.Index == subItem)
                    break;
                subItemX += col.Width;
            }

            subItemRectangle = new Rectangle(subItemX, lviBounds.Top, Columns[order[i]].Width, lviBounds.Height);

            return subItemRectangle;
        }

        /// <summary>
        ///     Retrieve the order in which columns appear.
        /// </summary>
        /// <returns>Current display order of column indices.</returns>
        protected int[] GetColumnOrder() {
            IntPtr lPar = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * Columns.Count);

            IntPtr res = SendMessage(Handle, LVM_GETCOLUMNORDERARRAY, new IntPtr(Columns.Count), lPar);
            if (res.ToInt32() == 0) // Something went wrong
            {
                Marshal.FreeHGlobal(lPar);
                return null;
            }

            var order = new int[Columns.Count];
            Marshal.Copy(lPar, order, 0, Columns.Count);

            Marshal.FreeHGlobal(lPar);

            return order;
        }

        protected override void WndProc(ref Message message) {
            switch (message.Msg) {
                case WM_PAINT:
                    if (View != View.Details)
                        break;

                    // Calculate the position of all embedded controls
                    foreach (EmbeddedControl ec in _embeddedControls) {
                        Rectangle rectangle = GetSubItemBounds(ec.Item, ec.Column);

                        if ((HeaderStyle != ColumnHeaderStyle.None) &&
                            (rectangle.Top < Font.Height)) // Control overlaps ColumnHeader
                        {
                            ec.Control.Visible = false;
                            continue;
                        } else {
                            ec.Control.Visible = true;
                        }

                        switch (ec.Dock) {
                            case DockStyle.Fill:
                                break;
                            case DockStyle.Top:
                                rectangle.Height = ec.Control.Height;
                                break;
                            case DockStyle.Left:
                                rectangle.Width = ec.Control.Width;
                                rectangle.Height = ec.Control.Height;
                                break;
                            case DockStyle.Bottom:
                                rectangle.Offset(0, rectangle.Height - ec.Control.Height);
                                break;
                            case DockStyle.Right:
                                rectangle.Offset(rectangle.Width - ec.Control.Width, 0);
                                rectangle.Width = ec.Control.Width;
                                rectangle.Height = ec.Control.Height;
                                break;
                            case DockStyle.None:
                                rectangle.Size = ec.Control.Size;
                                break;
                        }

                        // Set embedded control's bounds
                        ec.Control.Bounds = rectangle;
                    }
                    break;
            }
            base.WndProc(ref message);
        }

        private void EmbeddedControl_Click(object sender, EventArgs e) {
            // When a control is clicked the ListViewItem holding it is selected
            foreach (EmbeddedControl embeddedControl in _embeddedControls) {
                if (embeddedControl.Control == sender) {
                    SelectedItems.Clear();
                    embeddedControl.Item.Selected = true;
                }
            }
        }

        private void control_SizeChanged(object sender, EventArgs e) {
            //this.Refresh();
        }

        private void InitializeComponent() {
            SuspendLayout();
            ResumeLayout(false);
        }

        //To sort
        private void ExtendedListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            _sorting = true;
            // --- Perform sorting
            var sorter = Columns[e.Column] as SortableListviewColumnHeader;

            if (sorter == null)
                return;

            if (Sorting == SortOrder.None)
                Sorting = SortOrder.Ascending;
            else if (Sorting == SortOrder.Ascending)
                Sorting = SortOrder.Descending;
            else
                Sorting = SortOrder.Ascending;

            sorter.Column = e.Column;
            ListViewItemSorter = sorter;
            _sorting = false;
        }

        protected override void OnSelectedIndexChanged(EventArgs e) {
            if (!_sorting)
                base.OnSelectedIndexChanged(e);
        }

        /// <summary>
        ///     Structure to hold an embedded control's info.
        /// </summary>
        private struct EmbeddedControl {
            public int Column;
            public Control Control;
            public DockStyle Dock;
            public ListViewItem Item;
            public int Row;
        }
    }
}