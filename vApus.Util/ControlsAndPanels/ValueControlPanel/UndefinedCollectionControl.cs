/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Util {
    [ToolboxItem(false)]
    public partial class UndefinedCollectionControl : UserControl {
        private Type _elementType;
        private IEnumerable _value;

        private int _rowCount;

        private bool Truncated { get { return _rowCount > 1000; } }


        /// <summary>
        ///     Designer time constructor.
        /// </summary>
        public UndefinedCollectionControl() {
            InitializeComponent();
        }

        public UndefinedCollectionControl(Type elementType) {
            InitializeComponent();

            SetColumn(elementType);
        }

        public IEnumerable Value {
            get { return _value; }
        }

        public DataGridViewRowCollection Rows {
            get { return dataGridView.Rows; }
        }

        public event EventHandler ValueChanged;

        /// <summary>
        ///     If the input is not in the correct format.
        /// </summary>
        public event EventHandler Failed;

        private void SetColumn(Type elementType) {
            _elementType = elementType;

            DataGridViewColumn column = null;
            if (elementType == typeof(bool))
                column = new DataGridViewCheckBoxColumn();
            else if (elementType == typeof(char)) {
                column = new DataGridViewTextBoxColumn();
                (column as DataGridViewTextBoxColumn).MaxInputLength = 1;
            }
            else if (elementType.BaseType == typeof(Enum)) {
                column = new DataGridViewComboBoxColumn();
                (column as DataGridViewComboBoxColumn).DataSource = Enum.GetValues(elementType);
            } else
                column = new DataGridViewTextBoxColumn();

            dataGridView.Columns.Add(column);
            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        public void SetValue(IEnumerable value) {
            _value = value;

            dataGridView.CellValueChanged -= dataGridView_CellValueChanged;
            dataGridView.RowsRemoved -= dataGridView_RowsRemoved;
            dataGridView.Rows.Clear();
            _rowCount = 0;

            dataGridView.SuspendLayout();
            IEnumerator enumerator = value.GetEnumerator();
            while (enumerator.MoveNext())
                if (enumerator.Current != null) {
                    ++_rowCount;
                    if (!Truncated) {
                        var row = new DataGridViewRow();
                        row.Cells.Add(CreateDataGridViewCell(enumerator.Current));
                        dataGridView.Rows.Add(row);
                    }
                }
            dataGridView.ReadOnly = Truncated;

            btnEdit.Text = (Truncated) ? "[Truncated] Edit..." : "Edit...";

            dataGridView.ResumeLayout();

            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            dataGridView.RowsRemoved += dataGridView_RowsRemoved;
        }

        private DataGridViewCell CreateDataGridViewCell(object value) {
            DataGridViewCell cell;
            if (value is bool)
                cell = new DataGridViewCheckBoxCell();
            else if (value is char)
                cell = new DataGridViewTextBoxCell();
            else if (value is Enum) {
                var cboCell = new DataGridViewComboBoxCell();
                Type valueType = value.GetType();
                foreach (Enum e in Enum.GetValues(valueType)) {
                    var attr =
                        valueType.GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as
                        DescriptionAttribute[];
                    cboCell.Items.Add(attr.Length > 0 ? attr[0].Description : e.ToString());
                }
                cell = cboCell;
            } else
                cell = new DataGridViewTextBoxCell();

            cell.Value = value;
            return cell;
        }

        private void btnEdit_Click(object sender, EventArgs e) {
            var fromTextDialog = new FromTextDialog() { AutoRemoveEmptyLines = _elementType != typeof(string) };

            var sb = new StringBuilder();
            if (Truncated)
                foreach (object value in _value)
                    sb.AppendLine(value.ToString());
            else
                for (int i = 0; i < Rows.Count - 1; i++)
                    if (Rows[i].Cells[0].Value != null)
                        sb.AppendLine(Rows[i].Cells[0].Value.ToString());

            fromTextDialog.SetText(sb.ToString().Trim());

            if (fromTextDialog.ShowDialog() == DialogResult.OK) {
                dataGridView.CellValueChanged -= dataGridView_CellValueChanged;
                dataGridView.RowsRemoved -= dataGridView_RowsRemoved;
                dataGridView.Rows.Clear();
                try {
                    foreach (string entry in fromTextDialog.Entries) {
                        if (_elementType == typeof(bool))
                            dataGridView.Rows.Add(bool.Parse(entry));
                        else if (_elementType == typeof(char))
                            dataGridView.Rows.Add(char.Parse(entry));
                        else if (_elementType.BaseType == typeof(Enum))
                            dataGridView.Rows.Add(Enum.Parse(_elementType, entry));
                        else
                            dataGridView.Rows.Add(entry);
                    }
                }
                catch {
                }
                dataGridView.CellValueChanged += dataGridView_CellValueChanged;
                dataGridView.RowsRemoved += dataGridView_RowsRemoved;
                HandleValueChanged();
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            HandleValueChanged();
        }

        private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
            HandleValueChanged();
        }

        private void HandleValueChanged() {
            try {
                var arrayList = new ArrayList(dataGridView.Rows.Count - 1);
                Type elementType = (_value).AsQueryable().ElementType;

                if (elementType == typeof(bool))
                    for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                        arrayList.Add((bool)dataGridView.Rows[i].Cells[0].Value);
                else if (elementType == typeof(char))
                    for (int i = 0; i < dataGridView.Rows.Count - 1; i++) {
                        string s = dataGridView.Rows[i].Cells[0].Value.ToString();
                        if (s.Length == 0) arrayList.Add('\0');
                        else arrayList.Add(s[0]);
                    }
                else if (elementType == typeof(string))
                    for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                        arrayList.Add(dataGridView.Rows[i].Cells[0].Value as string);
                else if (elementType.BaseType == typeof(Enum))
                    for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                        arrayList.Add(Enum.Parse(elementType, dataGridView.Rows[i].Cells[0].Value.ToString()));
                else
                    try {
                        for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                            arrayList.Add(ConvertToNumericValue(elementType, dataGridView.Rows[i].Cells[0].Value.ToString()));
                    }
                    catch { }

                if (_value is Array) {
                    _value = arrayList.ToArray(elementType);
                }
                else if (_value is IList) {
                    var list = _value as IList;
                    list.Clear();
                    for (int i = 0; i < arrayList.Count; i++)
                        list.Add(arrayList[i]);
                    _value = list;
                }

                if (ValueChanged != null)
                    ValueChanged(this, null);
            }
            catch {
                if (Failed != null)
                    Failed(this, null);
            }
        }

        private object ConvertToNumericValue(Type numericValueType, string s) {
            return ConvertToNumericValue(numericValueType, double.Parse(s));
        }

        private object ConvertToNumericValue(Type numericValueType, object o) {
            if (numericValueType == typeof(short))
                return Convert.ToInt16(o);
            else if (numericValueType == typeof(int))
                return Convert.ToInt32(o);
            else if (numericValueType == typeof(long))
                return Convert.ToInt64(o);
            else if (numericValueType == typeof(ushort))
                return Convert.ToUInt16(o);
            else if (numericValueType == typeof(uint))
                return Convert.ToUInt32(o);
            else if (numericValueType == typeof(ulong))
                return Convert.ToUInt64(o);
            else if (numericValueType == typeof(float))
                return Convert.ToSingle(o);
            else if (numericValueType == typeof(double))
                return Convert.ToDouble(o);
            else if (numericValueType == typeof(decimal))
                return Convert.ToDecimal(o);
            throw new InvalidCastException("numericValueType");
        }
    }
}