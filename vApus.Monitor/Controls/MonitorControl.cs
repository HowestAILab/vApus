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
using System.Linq;
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
            ReadOnly = VirtualMode = DoubleBuffered = true;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            this.CellValueNeeded += new DataGridViewCellValueEventHandler(MonitorControl_CellValueNeeded);
            this.Scroll += new ScrollEventHandler(MonitorControl_Scroll);
        }
        private void MonitorControl_Scroll(object sender, ScrollEventArgs e)
        {
            _keepAtEnd = (VerticalScrollBar.Value + VerticalScrollBar.LargeChange + 1) >= VerticalScrollBar.Maximum;
        }

        private void MonitorControl_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                object value = null;
                lock (_lock)
                    value = _cache[e.RowIndex][e.ColumnIndex];

                if (value is float)
                {
                    float f = (float)value;
                    string s = null;
                    if (f == -1f)
                    {
                        var headerCell = this.Columns[e.ColumnIndex].HeaderCell;
                        if (headerCell.Style.BackColor != Color.Yellow)
                            headerCell.Style.BackColor = Color.Yellow;

                        s = f.ToString();
                    }
                    else
                    {
                        s = StringUtil.FloatToLongString(f, false);
                    }
                    value = s;
                }
                else
                {
                    value = ((DateTime)value).ToString("dd/MM/yyyy HH:mm:ss.fff");
                }

                e.Value = value;
            }
            catch
            {
                //index out of range exception, the user is notified about this on receiving the monitor values
            }
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
        //disabling resizing --> faster adding
        public new DataGridViewColumnHeadersHeightSizeMode ColumnHeadersHeightSizeMode
        {
            get { return base.ColumnHeadersHeightSizeMode; }
            set { base.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing; }
        }
        /// <summary>
        /// Must always happen before the first value was added.
        /// </summary>
        public void Init(Dictionary<Entity, List<CounterInfo>> wiw, string[] units)
        {
            this.Rows.Clear();
            this.Columns.Clear();

            _cache.Clear();
            _headers = new string[0];

            _filteredColumnIndices.Clear();

            this.RowCount = 0;

            List<string> lHeaders = new List<string>();
            lHeaders.Add(string.Empty);

            int unitIndex = 0;
            foreach (Entity entity in wiw.Keys)
                foreach (CounterInfo counterInfo in wiw[entity])
                    if (counterInfo.Instances.Count == 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(entity.Name);
                        sb.Append("/");
                        sb.Append(counterInfo.Counter);
                        string unit = units[unitIndex++];
                        if (!string.IsNullOrEmpty(unit))
                        {
                            sb.Append(" [");
                            sb.Append(unit);
                            sb.Append("]");
                        }

                        lHeaders.Add(sb.ToString());
                    }
                    else
                    {
                        foreach (string instance in counterInfo.Instances)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(entity.Name);
                            sb.Append("/");
                            sb.Append(counterInfo.Counter);

                            string unit = units[unitIndex++];
                            if (!string.IsNullOrEmpty(unit))
                            {
                                sb.Append(" [");
                                sb.Append(unit);
                                sb.Append("]");
                            }

                            if (instance != String.Empty)
                            {
                                sb.Append("/");
                                sb.Append(instance);
                            }
                            lHeaders.Add(sb.ToString());
                        }
                    }

            _headers = lHeaders.ToArray();

            //Add to columns, fillweight 1 --> to enable max 65 535 columns.
            DataGridViewColumn[] clms = new DataGridViewColumn[_headers.Length];
            string clmPrefix = this.ToString() + "clm";
            for (int headerIndex = 0; headerIndex != _headers.Length; headerIndex++)
            {
                string header = _headers[headerIndex];
                DataGridViewColumn clm = new DataGridViewTextBoxColumn();
                clm.Name = clmPrefix + header;
                clm.HeaderText = header;

                clm.SortMode = DataGridViewColumnSortMode.NotSortable;
                clm.FillWeight = 1;

                clms[headerIndex] = clm;
            }

            this.Columns.AddRange(clms);

            if (this.Visible)
                SetColumns();
            else
                this.VisibleChanged += new EventHandler(MonitorControl_VisibleChanged);
        }

        private void MonitorControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.VisibleChanged -= MonitorControl_VisibleChanged;
                SetColumns();
            }
        }
        private void SetColumns()
        {
            this.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            this.Columns[0].Width = 200;

            Filter(_filter);
        }
        /// <summary>
        /// This will add values to the collection and will update the Gui.
        /// </summary>
        /// <param name="monitorValues"></param>
        public void AddMonitorValues(object[] monitorValues)
        {
            if (this.ColumnCount != 0)
                lock (_lock)
                {
                    _cache.Add(monitorValues);
                    ++this.RowCount;

                    if (monitorValues.Length != _headers.Length)
                        LogWrapper.LogByLevel("[Monitoring] The number of monitor values is not the same as the number of headers!\nThis is a serious problem.", LogLevel.Error);
                }
            if (_keepAtEnd)
            {
                this.Scroll -= MonitorControl_Scroll;
                FirstDisplayedScrollingRowIndex = RowCount - 1;
                this.Scroll += MonitorControl_Scroll;
            }
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
            if (_headers == null)
                return string.Empty;

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
            if (_cache != null)
                lock (_lock)
                    foreach (var r in _cache)
                    {
                        if (r == null) continue;
                        var row = r as object[];
                        if (RowContainsNull(row)) continue;
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
        private bool RowContainsNull(object[] row)
        {
            foreach (var v in (row as object[]))
                if (v == null) return true;
            return false;
        }
    }
}
