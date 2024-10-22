﻿/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    ///     For selecting multiple items with the same parent.
    /// </summary>
    [ToolboxItem(false)]
    public partial class DefinedCollectionControl : UserControl {
        private IEnumerable _value;

        /// <summary>
        ///     For selecting multiple items with the same parent.
        /// </summary>
        public DefinedCollectionControl() {
            InitializeComponent();

            SetColumn();
        }

        public IEnumerable Value {
            get { return _value; }
        }

        public DataGridViewRowCollection Rows {
            get { return dataGridView.Rows; }
        }

        public event EventHandler ValueChanged;

        private void SetColumn() {
            DataGridViewColumn column = new DataGridViewTextBoxColumn();

            dataGridView.Columns.Add(column);
            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        public void SetValue(IEnumerable value) {
            try {
                var parent = (IEnumerable)value.GetParent();
            } catch (Exception ex) {
                throw new Exception(
                    "value must be of type IEnumerable (direct or indirect) and must have a parent of the type IEnumerable (also direct or indirect type).",
                    ex);
            }
            _value = value;

            dataGridView.CellValueChanged -= dataGridView_CellValueChanged;
            dataGridView.RowsRemoved -= dataGridView_RowsRemoved;
            dataGridView.Rows.Clear();

            IEnumerator enumerator = value.GetEnumerator();
            while (enumerator.MoveNext())
                if (enumerator.Current != null) {
                    var row = new DataGridViewRow();
                    row.Cells.Add(CreateDataGridViewCell(enumerator.Current));
                    dataGridView.Rows.Add(row);
                }
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            dataGridView.RowsRemoved += dataGridView_RowsRemoved;
        }

        private DataGridViewCell CreateDataGridViewCell(object value) {
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            cell.Value = value.ToString();
            return cell;
        }

        private void btnEdit_Click(object sender, EventArgs e) {
            var selectBaseItemsDialog = new SelectCollectionItemsDialog();
            selectBaseItemsDialog.SetValue(_value);

            if (selectBaseItemsDialog.ShowDialog() == DialogResult.OK) {
                try {
                    SetValue(selectBaseItemsDialog.NewValue);
                } catch {
                }
                if (ValueChanged != null)
                    ValueChanged(this, null);
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (ValueChanged != null)
                ValueChanged(this, null);
        }

        private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
            if (ValueChanged != null)
                ValueChanged(this, null);
        }
    }
}