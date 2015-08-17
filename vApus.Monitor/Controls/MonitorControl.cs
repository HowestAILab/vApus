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
        private MonitorResult _monitorResultCache;
        private List<object[]> _toDisplayRows = new List<object[]>();
        private readonly List<int> _filteredColumnIndices = new List<int>();
        private string[] _filter = new string[0];
        private bool _keepAtEnd = true;

        private readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private long _deltaTimestamp = 0L;
        #endregion

        #region Properties
        public MonitorResult MonitorResultCache { get { lock (_lock) return _monitorResultCache; } }
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
            _monitorResultCache = new MonitorResult();
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
                    value = -1d;

                if (value is double) {
                    var dou = (double)value;
                    string s = null;

                    try { s = StringUtil.DoubleToLongString(dou, false); } catch { dou = -1d; }

                    if (Double.IsNaN(dou) || Double.IsPositiveInfinity(dou) || Double.IsNegativeInfinity(dou)) dou = -1d;
                    if (dou == -1d) Columns[e.ColumnIndex].HeaderCell.Style.BackColor = Color.Yellow;

                    value = s;
                } else if (value is DateTime) {
                    value = ((DateTime)value).ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
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
        public void Init(Monitor monitor, Entities wdyh) {
            Rows.Clear();
            Columns.Clear();

            _toDisplayRows.Clear();

            _monitorResultCache = new MonitorResult() { Monitor = monitor.ToString() };
            _filteredColumnIndices.Clear();

            RowCount = 0;

            var lHeaders = new List<string>();
            lHeaders.Add(string.Empty);

            if (wdyh.GetSubs().Count == 1) {
                foreach (Entity entity in monitor.Wiw.GetSubs())
                    foreach (CounterInfo counterInfo in entity.GetSubs()) {
                        string counterInfoName = counterInfo.GetName();
                        if (counterInfo.GetSubs().Count == 0 || (counterInfo.GetSubs().Count == 1 && wdyh.GetCounterInfo(1, counterInfoName).GetSubs().Count == 1)) //__Total__ for instance
                            lHeaders.Add(counterInfoName);
                        else
                            foreach (CounterInfo instance in counterInfo.GetSubs())
                                lHeaders.Add(counterInfoName + "/" + instance.GetName());

                    }
            } else {
                foreach (Entity entity in monitor.Wiw.GetSubs())
                    foreach (CounterInfo counterInfo in entity.GetSubs()) {
                        string counterInfoName = counterInfo.GetName();
                        if (counterInfo.GetSubs().Count == 0 || (counterInfo.GetSubs().Count == 1 && wdyh.GetCounterInfo(1, counterInfoName).GetSubs().Count == 1))
                            lHeaders.Add(entity.GetName() + "/" + counterInfoName);
                        else
                            foreach (CounterInfo instance in counterInfo.GetSubs())
                                lHeaders.Add(entity.GetName() + "/" + counterInfoName + "/" + instance.GetName());
                    }
            }

            _monitorResultCache.Headers = lHeaders.ToArray();

            //Add to columns, fillweight 1 --> to enable max 65 535 columns.
            var clms = new DataGridViewColumn[_monitorResultCache.Headers.Length];
            string clmPrefix = ToString() + "clm";
            for (int headerIndex = 0; headerIndex != _monitorResultCache.Headers.Length; headerIndex++) {
                string header = _monitorResultCache.Headers[headerIndex];
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

                object[] row = new object[ColumnCount];

                //Null is given back for dropped counters. We need to be able to make correct averages.
                if (counters == null) {
                    row[0] = DateTime.Now.ToLocalTime();
                    for (int i = 1; i != row.Length; i++)
                        row[i] = 0d;
                } else {
                    row[0] = GetTimestamp(counters.GetTimestamp(), RowCount == 0);
                    List<string> counterValues = counters.GetCountersAtLastLevel();

                    for (int i = 0; i != counterValues.Count; i++) {
                        if (i >= ColumnCount) break;

                        string counterValue = counterValues[i];
                        object parsedValue = null;
                        bool boolValue = false;
                        if (counterValue.IsNumeric()) {
                            double dou = double.Parse(counterValue);
                            if (Double.IsNaN(dou) || Double.IsPositiveInfinity(dou) || Double.IsNegativeInfinity(dou))
                                dou = -1d;
                            parsedValue = Math.Round(dou, 3, MidpointRounding.AwayFromZero);
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
                    _monitorResultCache.Rows.Add(row);

                    if (row.Length != _monitorResultCache.Headers.Length)
                        Loggers.Log(Level.Error, "The number of monitor values is not the same as the number of headers!\nThis is a serious problem.", null, row);
                }

            } catch (Exception ex) {
                var parent = FindForm() as MonitorView;
                if (parent == null)
                    Loggers.Log(Level.Error, "Failed adding monitor values.", ex);
                else
                    Loggers.Log(Level.Error, "Failed adding monitor values.", ex, new object[] { (Parent as MonitorView).Monitor.ToString() });
            }
        }

        /// <summary>
        ///     Save all monitor values.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName) {
            try {
                List<string[]> newCache = GetSaveableCache();
                using (var sw = new StreamWriter(fileName, false)) {
                    sw.WriteLine(_monitorResultCache.Headers.Combine("\t"));
                    foreach (var row in newCache) sw.WriteLine(row.Combine("\t"));
                    sw.Flush();
                }
            } catch { MessageBox.Show("Cannot access '" + fileName + "' because it is in use!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        public void SaveFiltered(string fileName) {
            try {
                List<string[]> newCache = GetSaveableCache();
                using (var sw = new StreamWriter(fileName, false)) {
                    sw.WriteLine(FilterArray(_monitorResultCache.Headers).Combine("\t"));
                    foreach (var row in newCache) sw.WriteLine(FilterArray(row).Combine("\t"));
                    sw.Flush();
                }
            } catch { MessageBox.Show("Cannot access '" + fileName + "' because it is in use!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private List<string[]> GetSaveableCache() {
            lock (_lock) {
                var newCache = new List<string[]>(_monitorResultCache.Rows.Count);
                foreach (var row in _monitorResultCache.Rows) {
                    var newRow = new string[row.Length];
                    for (int i = 0; i != row.Length; i++) {
                        object o = row[i];
                        string s = string.Empty;
                        if (o is double)
                            s = StringUtil.DoubleToLongString((double)o);
                        else if (o is DateTime)
                            s = ((DateTime)o).ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
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

        /// <summary>
        /// Network latency independant timestamp.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="determineFirstTimestamp">Must happen once to be able to determine the other timestamps</param>
        /// <returns></returns>
        private DateTime GetTimestamp(long timestamp, bool determineFirstTimestamp) {
            if (determineFirstTimestamp) {
                long now = (long)(DateTime.UtcNow - _epoch).TotalMilliseconds;
                _deltaTimestamp = now - timestamp;
            }
            timestamp += _deltaTimestamp;

            return _epoch.AddMilliseconds(timestamp).ToLocalTime();
        }
        #endregion
    }
}
