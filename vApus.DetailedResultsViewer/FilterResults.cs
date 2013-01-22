/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.DetailedResultsViewer {
    public partial class FilterResults : UserControl {
        public event EventHandler FilterChanged;

        private Timer _filterChangedDelayedTimer;

        public string[] Filter { get { return txtFilter.Text.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries); } }

        public FilterResults() {
            InitializeComponent();
            ClearAvailableTags();
        }
        /// <summary>
        /// Duplicate tags are ignored.
        /// </summary>
        /// <param name="tags"></param>
        public void SetAvailableTags(DatabaseActions databaseActions) {
            List<string> tags = new List<string>();
            var dbs = databaseActions.GetDataTable("Show Databases like 'vapus%';");
            foreach (DataRow rrDB in dbs.Rows) {
                var t = databaseActions.GetDataTable("Select Tag from " + rrDB.ItemArray[0] + ".Tags;");
                foreach (DataRow row in t.Rows) {
                    string tag = (row.ItemArray[0] as string).Trim();
                    if (tag.Length != 0 && !tags.Contains(tag)) tags.Add(tag);
                }
            }
            SetAvailableTags(tags);
        }
        private void SetAvailableTags(List<string> tags) {
            ClearAvailableTags();
            if (tags.Count != 0) {
                foreach (string tag in tags) {
                    var kvpTag = new KeyValuePairControl(tag, string.Empty) { BackColor = SystemColors.Control };
                    kvpTag.Tooltip = "Click to add this tag to the filter.";
                    kvpTag.Cursor = Cursors.Hand;
                    kvpTag.MouseDown += kvpTag_MouseDown;
                    flpTags.Controls.Add(kvpTag);
                }
            }
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
            if (_filterChangedDelayedTimer == null) {
                _filterChangedDelayedTimer = new Timer() { Interval = 500 };
                _filterChangedDelayedTimer.Tick += _filterChangedTimer_Tick;
                _filterChangedDelayedTimer.Start();
            }
        }
        private void _filterChangedTimer_Tick(object sender, EventArgs e) {
            _filterChangedDelayedTimer.Dispose();
            _filterChangedDelayedTimer = null;
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
    }
}
