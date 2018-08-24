/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;

namespace vApus.Util {
    /// <summary>
    ///     For selecting multiple items with the same parent.
    /// </summary>
    [ToolboxItem(false)]
    public partial class DefinedKVPCollectionControl : UserControl {
        private IEnumerable _value;
        private Type _elementType;

        /// <summary>
        ///     For selecting multiple items with the same parent.
        /// </summary>
        public DefinedKVPCollectionControl() {
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
            var column = new DataGridViewTextBoxColumn();
            dataGridView.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            dataGridView.Columns.Add(column);

            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        public void SetValue(IEnumerable value) {
            try {
                var parent = (IEnumerable)value.GetParent();
            } catch (Exception ex) {
                throw new Exception("value must be of type IEnumerable (direct or indirect) and must have a parent of the type IEnumerable (also direct or indirect type).", ex);
            }
            _value = value;

            dataGridView.CellValueChanged -= dataGridView_CellValueChanged;
            dataGridView.RowsRemoved -= dataGridView_RowsRemoved;
            dataGridView.Rows.Clear();

            _elementType = value.AsQueryable().ElementType;
            IEnumerator enumerator = value.GetEnumerator();
            enumerator.Reset();
            while (enumerator.MoveNext())
                if (enumerator.Current != null) {
                    object[] kvp = { _elementType.GetProperty("Key").GetValue(enumerator.Current, null), _elementType.GetProperty("Value").GetValue(enumerator.Current, null) };

                    var row = new DataGridViewRow();
                    var keyCell = CreateDataGridViewCell(kvp[0]);
                    row.Cells.Add(keyCell);
                    keyCell.ReadOnly = true;

                    var valueCell = CreateDataGridViewCell(kvp[1]);
                    row.Cells.Add(valueCell);

                    dataGridView.Rows.Add(row);
                }
            enumerator.Reset();

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
                if (ValueChanged != null) ValueChanged(this, null);
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            try {
                if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.ColumnIndex == 1)
                    ValueEditted();
                if (ValueChanged != null)
                    ValueChanged(this, null);
            } catch { }
        }

        private void ValueEditted() {
            var arrayList = new ArrayList();
            var valueType = _elementType.GetGenericArguments()[1];

            int rowIndex = 0;

            IEnumerator enumerator = _value.GetEnumerator();
            enumerator.Reset();
            while (enumerator.MoveNext()) {
                var key = _elementType.GetProperty("Key").GetValue(enumerator.Current, null);
                var value = _elementType.GetProperty("Value").GetValue(enumerator.Current, null);

                if (rowIndex == dataGridView.CurrentCell.RowIndex)
                    try {
                        if (valueType.IsPrimitive) {
                            var value2 = Convert.ChangeType(dataGridView.CurrentCell.Value, valueType);
                            if (value == value2) return;
                            value = value2;
                        } else if (valueType.IsEnum) {
                            var value2 = Enum.Parse(valueType, value.ToString());
                            if (value == value2) return;
                            value = value2;
                        } else {
                            var value2 = dataGridView.CurrentCell.Value;
                            if (value == value2) return;
                            value = value2;
                        }
                    } catch {
                        dataGridView.CellValueChanged -= dataGridView_CellValueChanged;
                        dataGridView.CurrentCell.Value = value.ToString();
                        dataGridView.CellValueChanged += dataGridView_CellValueChanged;
                        throw;
                    }

                ++rowIndex;

                var kvp = Activator.CreateInstance(_elementType, new object[] { key, value });
                arrayList.Add(kvp);
            }
            enumerator.Reset();

            IEnumerable newValue = null;
            if (_value is Array) {
                newValue = arrayList.ToArray(_elementType);
            } else if (_value is IList) {
                newValue = Activator.CreateInstance(_value.GetType()) as IEnumerable;
                var list = newValue as IList;
                for (int i = 0; i < arrayList.Count; i++)
                    list.Add(arrayList[i]);
            }
            newValue.SetParent(_value.GetParent());
            _value = newValue;
        }

        private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) { if (ValueChanged != null)   ValueChanged(this, null); }
    }
}