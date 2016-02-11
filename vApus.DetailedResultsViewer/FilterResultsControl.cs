/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.DetailedResultsViewer {
    public partial class FilterResultsControl : UserControl {
        public event EventHandler FilterChanged;

        private System.Windows.Forms.Timer _filterChangedDelayedTimer = new System.Windows.Forms.Timer() { Interval = 1000 };
        [ThreadStatic]
        private static GetTagsWorkItem _getTagsWorkItem;

        private AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private readonly object _lock = new object();

        public string Filter { get { return txtFilter.Text.Trim(); } }

        public FilterResultsControl() {
            InitializeComponent();
            ClearAvailableTags();
            _filterChangedDelayedTimer.Tick += _filterChangedDelayedTimer_Tick;
        }

        ~FilterResultsControl() {
            try { _waitHandle.Dispose(); } catch {
                //Ignore.
            }
        }

        /// <summary>
        /// Duplicate tags are ignored.
        /// </summary>
        /// <param name="tags"></param>
        public void SetAvailableTags(DatabaseActions databaseActions) {
            Cursor = Cursors.WaitCursor;
            try {
                var tags = new SortedSet<string>();

                DataTable dbs = new DataTable("dbs");
                dbs.Columns.Add("db");

                var temp = databaseActions.GetDataTable("Show Databases;");
                foreach (DataRow rrDB in temp.Rows) {
                    string db = rrDB.ItemArray[0] as string;
                    if (db.StartsWith("vapus", StringComparison.OrdinalIgnoreCase)) dbs.Rows.Add(db);
                }

                int count = dbs.Rows.Count;
                int done = 0;
                foreach (DataRow rrDB in dbs.Rows) {
                    string database = rrDB.ItemArray[0] as string;
                    ThreadPool.QueueUserWorkItem((object state) => {
                        if (done < count) {
                            try {
                                if (_getTagsWorkItem == null) _getTagsWorkItem = new GetTagsWorkItem();

                                lock (_lock) {
                                    var dba = new DatabaseActions() { ConnectionString = databaseActions.ConnectionString };
                                    foreach (string t in _getTagsWorkItem.GetTags(dba, state as string))
                                        if (t.Length != 0 && !tags.Contains(t)) tags.Add(t);
                                    dba.ReleaseConnection();
                                    ++done;
                                }
                                if (done == count) _waitHandle.Set();
                            } catch {
                                try {
                                    lock (_lock) done = int.MaxValue;
                                    _waitHandle.Set();
                                } catch {
                                    //Ignore.
                                }
                            }
                        }
                    }, database);

                }
                if (count != 0) _waitHandle.WaitOne();
                SetAvailableTags(tags);
            } catch {
                //Ignore.
            }
            try { if (!Disposing && !IsDisposed) Cursor = Cursors.Arrow; } catch { }
        }
        private void SetAvailableTags(SortedSet<string> tags) {
            flpTags.AutoScroll = false;
            flpTags.SuspendLayout();
            ClearAvailableTags();
            foreach (string tag in tags) {
                var kvpTag = new KeyValuePairControl(tag, string.Empty) { BackColor = SystemColors.Control };
                kvpTag.Tooltip = "Click to add this tag to the filter.";
                kvpTag.Cursor = Cursors.Hand;
                kvpTag.MouseDown += kvpTag_MouseDown;
                flpTags.Controls.Add(kvpTag);
            }
            flpTags.ResumeLayout();
            flpTags.AutoScroll = true;
        }
        private void kvpTag_MouseDown(object sender, MouseEventArgs e) {
            var kvpTag = sender as KeyValuePairControl;
            string tag = "\\b" + Regex.Escape(kvpTag.Key) + "\\b";

            if (!Regex.IsMatch(txtFilter.Text, tag, RegexOptions.IgnoreCase)) {
                if (txtFilter.Text.Length != 0) txtFilter.Text += " ";
                txtFilter.Text += kvpTag.Key;
                txtFilter.Focus();
                txtFilter.Select(txtFilter.Text.Length, 0);
            }
        }
        public void ClearAvailableTags() { flpTags.Controls.Clear(); }
        private void txtFilter_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) e.Handled = true; }
        private void txtFilter_KeyPress(object sender, KeyPressEventArgs e) { if (e.KeyChar == '\r') e.Handled = true; }

        private void txtFilter_TextChanged(object sender, EventArgs e) {
                _filterChangedDelayedTimer.Stop();
                _filterChangedDelayedTimer.Start();
        }
        private void _filterChangedDelayedTimer_Tick(object sender, EventArgs e) {
                _filterChangedDelayedTimer.Stop();
                InvokeFilterChanged();
        }
        private void InvokeFilterChanged() {
            if (FilterChanged != null) {
                int caretPosition = txtFilter.SelectionStart;
                FilterChanged(this, null);
                txtFilter.Focus();
                txtFilter.Select(caretPosition, 0);
            }
        }

        private class GetTagsWorkItem {
            public List<string> GetTags(DatabaseActions databaseActions, string database) {
                List<string> tags = new List<string>();
                var t = databaseActions.GetDataTable("Select Tag from " + database + ".tags;");
                foreach (DataRow row in t.Rows) tags.Add((row.ItemArray[0] as string).Trim().ToLowerInvariant());
                return tags;
            }
        }

        private void FilterResults_Resize(object sender, EventArgs e) {
            flpTags.PerformLayout();
        }

        private void flpTags_Scroll(object sender, ScrollEventArgs e) {
            flpTags.PerformLayout();
        }
    }
}
