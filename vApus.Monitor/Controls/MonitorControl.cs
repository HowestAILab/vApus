/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using vApus.Util;
using vApusSMT.Base;

namespace vApus.Monitor
{
    public class MonitorControl : DataGridView
    {
        private object _lock = new object();

        private string[] _headers = new string[0];
        private List<object[]> _cache = new List<object[]>();

        private string[] _filter = new string[0];
        private List<int> _filteredColumnIndices = new List<int>();

        private bool _keepAtEnd = true;

        public MonitorControl()
        {
            AllowUserToAddRows = AllowUserToDeleteRows = AllowUserToResizeRows = AllowUserToOrderColumns = false;
            ReadOnly = true;
            VirtualMode = true;
            DoubleBuffered = true;

            this.CellValueNeeded += new DataGridViewCellValueEventHandler(MonitorControl_CellValueNeeded);
            this.Scroll += new ScrollEventHandler(MonitorControl_Scroll);
        }
        private void MonitorControl_Scroll(object sender, ScrollEventArgs e)
        {
            _keepAtEnd = (VerticalScrollBar.Value + VerticalScrollBar.LargeChange + 1) >= VerticalScrollBar.Maximum;
        }

        private void MonitorControl_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            object value = null;
            lock (_lock)
                value = _cache[e.RowIndex][e.ColumnIndex];

            if (value is float)
            {
                float f = (float)value;
                if (f == -1f)
                {
                    var headerCell = this.Columns[e.ColumnIndex].HeaderCell;
                    if (headerCell.Style.BackColor != Color.Yellow)
                        headerCell.Style.BackColor = Color.Yellow;
                }
                value = StringUtil.FloatToLongString(f);
            }
            else
            {
                value = ((DateTime)value).ToString("dd/MM/yyyy HH:mm:ss.fff");
            }

            e.Value = value;
        }
        public new bool AllowUserToAddRows
        {
            get { return base.AllowUserToAddRows; }
            set { base.AllowUserToAddRows = false; }
        }
        public new bool AllowUserToDeleteRows
        {
            get { return base.AllowUserToDeleteRows; }
            set { base.AllowUserToDeleteRows = false; }
        }
        public new bool AllowUserToResizeRows
        {
            get { return base.AllowUserToResizeRows; }
            set { base.AllowUserToResizeRows = false; }
        }
        public new bool AllowUserToOrderColumns
        {
            get { return base.AllowUserToOrderColumns; }
            set { base.AllowUserToOrderColumns = false; }
        }
        public new bool ReadOnly
        {
            get { return base.ReadOnly; }
            set { base.ReadOnly = true; }
        }
        public new bool VirtualMode
        {
            get { return base.VirtualMode; }
            set { base.VirtualMode = true; }
        }
        public new bool DoubleBuffered
        {
            get { return base.DoubleBuffered; }
            set { base.DoubleBuffered = true; }
        }
        /// <summary>
        /// This will add values to the collection and will update the Gui.
        /// </summary>
        /// <param name="monitorValues"></param>
        public void AddMonitorValues(Dictionary<string, HashSet<MonitorValueCollection>> monitorValues)
        {
            SetHeaders(monitorValues);
            AddRow(monitorValues);

            if (_keepAtEnd)
            {
                this.Scroll -= MonitorControl_Scroll;
                FirstDisplayedScrollingRowIndex = RowCount - 1;
                this.Scroll += MonitorControl_Scroll;
            }
        }
        private void SetHeaders(Dictionary<string, HashSet<MonitorValueCollection>> monitorValues)
        {
            if (this.ColumnCount == 0)
            {
                _headers = ExtractHeaders(monitorValues);
                string clmPrefix = this.ToString() + "clm";
                foreach (string header in _headers)
                    this.Columns[this.Columns.Add(clmPrefix + header, header)].SortMode = DataGridViewColumnSortMode.NotSortable;

                this.RowCount = 0;

                this.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
                this.Columns[0].Width = 200;

                Filter(_filter);
            }
        }
        private string[] ExtractHeaders(Dictionary<string, HashSet<MonitorValueCollection>> monitorValues)
        {
            List<string> l = new List<string>();
            l.Add(string.Empty);
            foreach (string entity in monitorValues.Keys)
                foreach (MonitorValueCollection monitorValueCollection in monitorValues[entity])
                {
                    StringBuilder sb = new StringBuilder();
                    if (monitorValueCollection.Instance == String.Empty)
                        sb.Append(entity);
                    else
                        sb.Append(entity + "/" + monitorValueCollection.Instance);
                    sb.Append("//");
                    sb.Append(monitorValueCollection.Counter);
                    if (!string.IsNullOrEmpty(monitorValueCollection.Unit))
                    {
                        sb.Append(" [");
                        sb.Append(monitorValueCollection.Unit);
                        sb.Append("]");
                    }
                    l.Add(sb.ToString());
                }
            return l.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monitorValues"></param>
        private void AddRow(Dictionary<string, HashSet<MonitorValueCollection>> monitorValues)
        {
            if (monitorValues.Count != 0 && this.ColumnCount != 0)
            {
                DateTime timestamp = DateTime.MinValue;

                object[] row = new object[this.ColumnCount];
                int i = 1;
                foreach (string entity in monitorValues.Keys)
                    foreach (MonitorValueCollection monitorValueCollection in monitorValues[entity])
                        foreach (MonitorValue monitorValue in monitorValueCollection)
                        {
                            row[i++] = monitorValue.Value;
                            if (timestamp == DateTime.MinValue)
                                timestamp = monitorValue.TimeStamp;
                        }

                row[0] = timestamp;
                lock (_lock)
                {
                    _cache.Add(row);
                    ++this.RowCount;
                }
            }
        }
        /// <summary>
        /// This will clear all the values and will remove the accordeon controls.
        /// </summary>
        public void ClearMonitorValues()
        {
            this.Rows.Clear();
            this.Columns.Clear();

            _cache.Clear();
            _headers = new string[0];

            _filteredColumnIndices.Clear();

            this.RowCount = 0;
        }
        /// <summary>
        /// Save all monitor values.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            try
            {
                List<string[]> newCache = GetSaveableCache();
                using (StreamWriter sw = new StreamWriter(fileName, false))
                {
                    sw.WriteLine(GetHeaders().Combine("\t"));
                    foreach (string[] row in newCache)
                        sw.WriteLine(row.Combine("\t"));
                    sw.Flush();
                }
            }
            catch
            {
                MessageBox.Show("Cannot access '" + fileName + "' because it is in use!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void SaveFiltered(string fileName)
        {
            try
            {
                List<string[]> newCache = GetSaveableCache();
                using (StreamWriter sw = new StreamWriter(fileName, false))
                {
                    sw.WriteLine(FilterArray(GetHeaders()).Combine("\t"));
                    foreach (string[] row in newCache)
                        sw.WriteLine(FilterArray(row).Combine("\t"));
                    sw.Flush();
                }
            }
            catch
            {
                MessageBox.Show("Cannot access '" + fileName + "' because it is in use!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private List<string[]> GetSaveableCache()
        {
            lock (_lock)
            {
                var newCache = new List<string[]>(_cache.Count);
                foreach (object[] row in _cache)
                {
                    string[] newRow = new string[row.Length];
                    for (int i = 0; i != row.Length; i++)
                    {
                        object o = row[i];
                        if (o is float)
                            newRow[i] = StringUtil.FloatToLongString((float)o);
                        else
                            newRow[i] = ((DateTime)o).ToString("dd/MM/yyyy HH:mm:ss.fff");
                    }
                    newCache.Add(newRow);
                }
                return newCache;
            }
        }
        private string[] FilterArray(string[] array)
        {
            int j = 0;
            string[] filtered = new string[_filteredColumnIndices.Count];
            for (int i = 0; i != array.Length; i++)
                if (_filteredColumnIndices.Contains(i))
                    filtered[j++] = array[i];
            return filtered;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>The indices of the visible columns.</returns>
        public void Filter(string[] filter)
        {
            _filter = filter;
            _filteredColumnIndices.Clear();
            if (this.Columns.Count != 0)
            {
                if (filter.Length == 0)
                {
                    int i = 0;
                    foreach (DataGridViewColumn clm in this.Columns)
                    {
                        clm.Visible = true;
                        _filteredColumnIndices.Add(i++);
                    }
                }
                else
                {
                    List<DataGridViewColumn> visibleColumns = new List<DataGridViewColumn>();
                    visibleColumns.Add(this.Columns[0]);

                    foreach (string s in _filter)
                        foreach (DataGridViewColumn clm in Find(s))
                            if (!visibleColumns.Contains(clm))
                                visibleColumns.Add(clm);

                    int i = 0;
                    foreach (DataGridViewColumn clm in this.Columns)
                    {
                        if (visibleColumns.Contains(clm))
                        {
                            clm.Visible = true;
                            _filteredColumnIndices.Add(i);
                        }
                        else
                        {
                            clm.Visible = false;
                        }
                        ++i;
                    }
                }
            }
        }
        private IEnumerable<DataGridViewColumn> Find(string text)
        {
            text = Regex.Escape(text);
            text = text.Replace("\\*", ".*");
            text = "\\b" + text + "\\b";
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;

            foreach (DataGridViewColumn clm in this.Columns)
                if (Regex.IsMatch(clm.HeaderText, text, options))
                    yield return clm;
        }

        public string[] GetHeaders()
        {
            return _headers;
        }
        /// <summary>
        /// Returns the header of all monitor values.
        /// </summary>
        /// <returns></returns>
        public string GetHeaders(string separator)
        {
            return _headers.Combine(separator);
        }
        /// <summary>
        /// Returns all monitor values.
        /// </summary>
        /// <returns></returns>
        public Dictionary<DateTime, float[]> GetMonitorValues()
        {
            return GetMonitorValues(DateTime.MinValue, DateTime.MaxValue);
        }
        /// <summary>
        /// Returns monitor values filtered.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public Dictionary<DateTime, float[]> GetMonitorValues(DateTime from, DateTime to)
        {
            var monitorValues = new Dictionary<DateTime, float[]>();
            foreach (object[] row in _cache)
            {
                DateTime timestamp = (DateTime)row[0];
                if (timestamp >= from && timestamp <= to)
                {
                    float[] values = new float[row.Length - 1];
                    for (int i = 0; i != values.Length; i++)
                        values[i] = (float)row[i + 1];

                    if (!monitorValues.ContainsKey(timestamp))
                        monitorValues.Add(timestamp, values);
                }
            }
            return monitorValues;
        }
        /// <summary>
        /// Returns all monitor values.
        /// </summary>
        /// <returns></returns>
    }
}
