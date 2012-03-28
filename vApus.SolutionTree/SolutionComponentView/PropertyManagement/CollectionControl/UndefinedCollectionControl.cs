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
using System.Text;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.SolutionTree
{
    public partial class UndefinedCollectionControl : UserControl
    {
        public event EventHandler ValueChanged;

        private Type _elementType;
        private IEnumerable _value;

        public IEnumerable Value
        {
            get { return _value; }
        }
        public DataGridViewRowCollection Rows
        {
            get { return dataGridView.Rows; }
        }
        /// <summary>
        /// Designer time constructor.
        /// </summary>
        public UndefinedCollectionControl()
        {
            InitializeComponent();
        }
        public UndefinedCollectionControl(Type elementType)
        {
            InitializeComponent();

            SetColumn(elementType);
        }
        private void SetColumn(Type elementType)
        {
            _elementType = elementType;

            DataGridViewColumn column = null;
            if (elementType == typeof(bool))
                column = new DataGridViewCheckBoxColumn();
            else if (elementType == typeof(char))
            {
                column = new DataGridViewTextBoxColumn();
                (column as DataGridViewTextBoxColumn).MaxInputLength = 1;
            }
            else if (elementType.BaseType == typeof(Enum))
            {
                column = new DataGridViewComboBoxColumn();
                (column as DataGridViewComboBoxColumn).DataSource = Enum.GetValues(elementType);
            }
            else if (elementType == typeof(string) || StringUtil.IsNumericType(elementType))
                column = new DataGridViewTextBoxColumn();

            dataGridView.Columns.Add(column);
            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        public void SetValue(IEnumerable value)
        {
            _value = value;

            dataGridView.CellValueChanged -= dataGridView_CellValueChanged;
            dataGridView.RowsRemoved -= dataGridView_RowsRemoved;
            dataGridView.Rows.Clear();

            IEnumerator enumerator = value.GetEnumerator();
            while (enumerator.MoveNext())
                if (enumerator.Current != null)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Cells.Add(CreateDataGridViewCell(enumerator.Current));
                    dataGridView.Rows.Add(row);
                }
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            dataGridView.RowsRemoved += new DataGridViewRowsRemovedEventHandler(dataGridView_RowsRemoved);
        }
        private DataGridViewCell CreateDataGridViewCell(object value)
        {
            DataGridViewCell cell;
            if (value is bool)
                cell = new DataGridViewCheckBoxCell();
            else if (value is char)
                cell = new DataGridViewTextBoxCell();
            else if (value is Enum)
            {
                DataGridViewComboBoxCell cboCell = new DataGridViewComboBoxCell();
                Type valueType = value.GetType();
                foreach (Enum e in Enum.GetValues(valueType))
                {
                    DescriptionAttribute[] attr = valueType.GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                    cboCell.Items.Add(attr.Length > 0 ? attr[0].Description : e.ToString());
                }
                cell = cboCell;
            }
            else if (value is string || StringUtil.IsNumeric(value))
                cell = new DataGridViewTextBoxCell();
            else
                throw new InvalidCastException("elementType");
            cell.Value = value;
            return cell;
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            FromTextDialog fromTextDialog = new FromTextDialog();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Rows.Count - 1; i++)
                if (Rows[i].Cells[0].Value != null)
                    sb.AppendLine(Rows[i].Cells[0].Value.ToString());

            fromTextDialog.SetText(sb.ToString().Trim());

            if (fromTextDialog.ShowDialog() == DialogResult.OK)
            {
                dataGridView.CellValueChanged -= dataGridView_CellValueChanged;
                dataGridView.RowsRemoved -= dataGridView_RowsRemoved;
                dataGridView.Rows.Clear();
                try
                {
                    foreach (string entry in fromTextDialog.Entries)
                    {
                        if (_elementType == typeof(bool))
                            dataGridView.Rows.Add(bool.Parse(entry));
                        else if (_elementType == typeof(char))
                            dataGridView.Rows.Add(char.Parse(entry));
                        else if (_elementType.BaseType == typeof(Enum))
                            dataGridView.Rows.Add(Enum.Parse(_elementType, entry));
                        else if (_elementType == typeof(string) || StringUtil.IsNumericType(_elementType))
                            dataGridView.Rows.Add(entry);
                    }
                }
                catch { }
                dataGridView.CellValueChanged += dataGridView_CellValueChanged;
                dataGridView.RowsRemoved += dataGridView_RowsRemoved;
                if (ValueChanged != null)
                    ValueChanged(this, null);
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, null);
        }
        private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, null);
        }
    }
}
