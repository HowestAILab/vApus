/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using vApus.Monitor.Sources.Base;
using vApus.Results;
using vApus.Util;

namespace vApus.Monitor {
    public class MonitorControl : DataGridView {

        #region Fields
        private readonly object _lock = new object();
        private List<object[]> _toDisplayRows = new List<object[]>();
        private readonly List<int> _filteredColumnIndices = new List<int>();
        private string[] _filter = new string[0];
        private bool _keepAtEnd = true;
        #endregion

        #region Properties
        public MonitorResult MonitorResultCache { get; private set; }
        public new bool AllowUserToAddRows { get { return base.AllowUserToAddRows; } set { base.AllowUserToAddRows = false; } }
        public new bool AllowUserToDeleteRows { get { return base.AllowUserToDeleteRows; } set { base.AllowUserToDeleteRows = false; } }
        public new bool AllowUserToResizeRows { get { return base.AllowUserToResizeRows; } set { base.AllowUserToResizeRows = false; } }
        public new bool AllowUserToOrderColumns { get { return base.AllowUserToOrderColumns; } set { base.AllowUserToOrderColumns = false; } }
        public new bool ReadOnly { get { return base.ReadOnly; } set { base.ReadOnly = true; } }
        public new bool VirtualMode { get { return base.VirtualMode; } set { base.VirtualMode = true; } }
        public new bool DoubleBuffered { get { return base.DoubleBuffered; } set { base.DoubleBuffered = true; } }
        //disabling resizing --> faster adding
        public new DataGridViewColumnHeadersHeightSizeMode ColumnHeadersHeightSizeMode { get { return base.ColumnHeadersHeightSizeMode; } set { base.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing; } }
        #endregion

        #region Constructor
        public MonitorControl() {
            MonitorResultCache = new MonitorResult();
            AllowUserToAddRows = AllowUserToDeleteRows = AllowUserToResizeRows = AllowUserToOrderColumns = false;
            ReadOnly = VirtualMode = DoubleBuffered = true;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            CellValueNeeded += MonitorControl_CellValueNeeded;
            Scroll += MonitorControl_Scroll;
        }
        #endregion

        #region Functions
        private void MonitorControl_Scroll(object sender, ScrollEventArgs e) { _keepAtEnd = (VerticalScrollBar.Value + VerticalScrollBar.LargeChange + 1) >= VerticalScrollBar.Maximum; }
        private void MonitorControl_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            try {
                object value = null;
                if (e.RowIndex < _toDisplayRows.Count) {
                    object[] row = _toDisplayRows[e.RowIndex];
                    if (e.ColumnIndex < row.Length)
                        value = row[e.ColumnIndex];
                }

                if (value == null)
                    if (e.ColumnIndex == 0) value = DateTime.Now;
                    else value = -1f;

                if (value is double) {
                    var dou = (double)value;
                    string s = (-1d).ToString();
                    if (dou != -1d)
                        if (Double.IsNaN(dou) || Double.IsPositiveInfinity(dou) || Double.IsNegativeInfinity(dou))
                            dou = -1d;
                        else
                            try {
                                s = StringUtil.DoubleToLongString(dou, false);
                            } catch {
                                dou = -1d;
                            }

                    if (dou == -1d) {
                        DataGridViewColumnHeaderCell headerCell = Columns[e.ColumnIndex].HeaderCell;
                        if (headerCell.Style.BackColor != Color.Yellow) headerCell.Style.BackColor = Color.Yellow;
                    }

                    value = s;
                } else if (value is DateTime) {
                    value = ((DateTime)value).ToString("dd/MM/yyyy HH:mm:ss.fff");
                }

                e.Value = value;
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed at displaying monitor values.", ex);
            }
        }
        /// <summary>
        ///     Must always happen before the first value was added.
        /// </summary>
        /// <param name="monitor">The Wiw and the to string is used.</param>
        public void Init(Monitor monitor) {
            Rows.Clear();
            Columns.Clear();

            _toDisplayRows.Clear();

            MonitorResultCache = new MonitorResult() { Monitor = monitor.ToString() };
            _filteredColumnIndices.Clear();

            RowCount = 0;

            var lHeaders = new List<string>();
            lHeaders.Add(string.Empty);

            foreach (Entity entity in monitor.Wiw)
                foreach (CounterInfo counterInfo in entity.GetSubs())
                    if (counterInfo.GetSubs().Count == 0) {
                        var sb = new StringBuilder();
                        sb.Append(entity.GetName());
                        sb.Append("/");
                        sb.Append(counterInfo.GetName());

                        lHeaders.Add(sb.ToString());
                    } else
                        foreach (CounterInfo instance in counterInfo.GetSubs()) {
                            var sb = new StringBuilder();
                            sb.Append(entity.GetName());
                            sb.Append("/");
                            sb.Append(counterInfo.GetName());
                            sb.Append("/");
                            sb.Append(instance.GetName());

                            lHeaders.Add(sb.ToString());
                        }

            MonitorResultCache.Headers = lHeaders.ToArray();

            //Add to columns, fillweight 1 --> to enable max 65 535 columns.
            var clms = new DataGridViewColumn[MonitorResultCache.Headers.Length];
            string clmPrefix = ToString() + "clm";
            for (int headerIndex = 0; headerIndex != MonitorResultCache.Headers.Length; headerIndex++) {
                string header = MonitorResultCache.Headers[headerIndex];
                DataGridViewColumn clm = new DataGridViewTextBoxColumn();
                clm.Name = clmPrefix + header;
                clm.HeaderText = header;

                clm.SortMode = DataGridViewColumnSortMode.NotSortable;
                //To allow 2 power 32 columns.
                clm.FillWeight = 1;

                clms[headerIndex] = clm;
            }

            Columns.AddRange(clms);

            if (Visible) SetColumns(); else VisibleChanged += MonitorControl_VisibleChanged;
        }
        private void MonitorControl_VisibleChanged(object sender, EventArgs e) {
            if (Visible) {
                VisibleChanged -= MonitorControl_VisibleChanged;
                SetColumns();
            }
        }
        private void SetColumns() {
            AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            Columns[0].Width = 200;

            Filter(_filter);
        }
        /// <summary>
        ///     This will add values to the collection and will update the Gui.
        /// </summary>
        /// <param name="counters"></param>
        public void AddCounters(Entities counters, string decimalSeparator) {
            try {
                if (ColumnCount == 0) return;

                CultureInfo currentCulture = null;
                if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator != decimalSeparator) {
                    currentCulture = Thread.CurrentThread.CurrentCulture;
                    CultureInfo tempCulture = currentCulture.Clone() as CultureInfo;
                    tempCulture.NumberFormat.NumberDecimalSeparator = decimalSeparator;
                    Thread.CurrentThread.CurrentCulture = tempCulture;
                }

                List<string> counterValues = counters.GetCountersAtLastLevel();
                object[] row = new object[ColumnCount];
                row[0] = DateTime.Now;
                for (int i = 0; i != counterValues.Count; i++) {
                    if (i >= ColumnCount) break;

                    string counterValue = counterValues[i];
                    object parsedValue = null;
                    bool boolValue = false;
                    if (counterValue.IsNumeric()) {
                        double dou = double.Parse(counterValue);
                        if (Double.IsNaN(dou) || Double.IsPositiveInfinity(dou) || Double.IsNegativeInfinity(dou))
                            dou = -1d;
                        parsedValue = dou;
                    } else if (bool.TryParse(counterValue, out boolValue)) {
                        parsedValue = boolValue ? 1d : 0d;
                    } else {
                        DateTime timeStamp;
                        if (DateTime.TryParse(counterValue, out timeStamp))
                            parsedValue = timeStamp;
                        else
                            parsedValue = counterValue;
                    }
                    row[i + 1] = parsedValue;
                }
                _toDisplayRows.Add(row);

                // SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                ++RowCount;

                if (_keepAtEnd) {
                    Scroll -= MonitorControl_Scroll;
                    FirstDisplayedScrollingRowIndex = RowCount - 1;
                    Scroll += MonitorControl_Scroll;
                }

                if (currentCulture != null)
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                //  }, null);

                lock (_lock) {
                    MonitorResultCache.Rows.Add(row);

                    if (row.Length != MonitorResultCache.Headers.Length)
                        Loggers.Log(Level.Error, "The number of monitor values is not the same as the number of headers!\nThis is a serious problem.", null, row);
                }

            } catch (Exception ex) {
                if (Parent != null && Parent is MonitorView)
                    Loggers.Log(Level.Error, "Failed adding monitor values.", ex, new object[] { (Parent as MonitorView).Monitor });
                else
                    Loggers.Log(Level.Error, "Failed adding monitor values.", ex);
            }
        }
        /// <summary>
        ///     Returns all monitor values.
        /// </summary>
        /// <returns></returns>
        public Dictionary<DateTime, double[]> GetMonitorValues() {
            var monitorValues = new Dictionary<DateTime, double[]>();
            lock (_lock)
                foreach (var row in MonitorResultCache.Rows) {
                    if (row == null) continue;
                    if (RowContainsNull(row)) continue;
                    var timestamp = (DateTime)row[0];
                    var values = new double[row.Length - 1];
                    for (int i = 0; i != values.Length; i++) values[i] = (double)row[i + 1];

                    if (!monitorValues.ContainsKey(timestamp)) monitorValues.Add(timestamp, values);
                }
            return monitorValues;
        }
        private bool RowContainsNull(object[] row) {
            foreach (var v in (row as object[]))
                if (v == null) return true;
            return false;
        }
        /// <summary>
        ///     Save all monitor values.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName) {
            try {
                List<string[]> newCache = GetSaveableCache();
                using (var sw = new StreamWriter(fileName, false)) {
                    sw.WriteLine(MonitorResultCache.Headers.Combine("\t"));
                    foreach (var row in newCache) sw.WriteLine(row.Combine("\t"));
                    sw.Flush();
                }
            } catch { MessageBox.Show("Cannot access '" + fileName + "' because it is in use!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        public void SaveFiltered(string fileName) {
            try {
                List<string[]> newCache = GetSaveableCache();
                using (var sw = new StreamWriter(fileName, false)) {
                    sw.WriteLine(FilterArray(MonitorResultCache.Headers).Combine("\t"));
                    foreach (var row in newCache) sw.WriteLine(FilterArray(row).Combine("\t"));
                    sw.Flush();
                }
            } catch { MessageBox.Show("Cannot access '" + fileName + "' because it is in use!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private List<string[]> GetSaveableCache() {
            lock (_lock) {
                var newCache = new List<string[]>(MonitorResultCache.Rows.Count);
                foreach (var row in MonitorResultCache.Rows) {
                    var newRow = new string[row.Length];
                    for (int i = 0; i != row.Length; i++) {
                        object o = row[i];
                        string s = string.Empty;
                        if (o is double)
                            s = StringUtil.DoubleToLongString((double)o);
                        else if (o is DateTime)
                            s = ((DateTime)o).ToString("dd/MM/yyyy HH:mm:ss.fff");
                        else s = o.ToString();

                        newRow[i] = s;
                    }
                    newCache.Add(newRow);
                }
                return newCache;
            }
        }
        private string[] FilterArray(string[] array) {
            int j = 0;
            var filtered = new string[_filteredColumnIndices.Count];
            for (int i = 0; i != array.Length; i++)
                if (_filteredColumnIndices.Contains(i)) filtered[j++] = array[i];
            return filtered;
        }
        /// <summary>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>The indices of the visible columns.</returns>
        public void Filter(string[] filter) {
            _filter = filter;
            _filteredColumnIndices.Clear();
            if (Columns.Count != 0) {
                int i = 0;
                if (filter.Length == 0)
                    foreach (DataGridViewColumn clm in Columns) {
                        clm.Visible = true;
                        _filteredColumnIndices.Add(i++);
                    } else {
                    var visibleColumns = new List<DataGridViewColumn>();
                    visibleColumns.Add(Columns[0]);

                    foreach (string s in _filter)
                        foreach (DataGridViewColumn clm in Find(s))
                            if (!visibleColumns.Contains(clm)) visibleColumns.Add(clm);

                    foreach (DataGridViewColumn clm in Columns) {
                        if (visibleColumns.Contains(clm)) {
                            clm.Visible = true;
                            _filteredColumnIndices.Add(i);
                        } else clm.Visible = false;
                        ++i;
                    }
                }
            }
        }
        private IEnumerable<DataGridViewColumn> Find(string text) {
            text = Regex.Escape(text);
            text = text.Replace("\\*", ".*");
            text = "\\b" + text + "\\b";
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;

            foreach (DataGridViewColumn clm in Columns) if (Regex.IsMatch(clm.HeaderText, text, options)) yield return clm;
        }
        #endregion
    }
}
