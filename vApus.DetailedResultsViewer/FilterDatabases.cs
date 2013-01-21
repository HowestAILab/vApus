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
using System.Windows.Forms;
using vApus.Util;

namespace vApus.DetailedResultsViewer {
    public partial class FilterDatabases : UserControl {
        public event EventHandler CollapsedOrExpandedTags;
        public FilterDatabases() {
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
                    var kvp = new KeyValuePairControl(tag, string.Empty) { BackColor = SystemColors.Control };
                    kvp.Tooltip = "Click to add to filter.";
                    flpTags.Controls.Add(kvp);
                }
                llblShowHide.Text = "Show/Hide Available Tags";
                llblShowHide.Enabled = true;
                Height = (flpTags.Visible ? flpTags.Bottom : flpTags.Top) + 19;
            }
        }
        public void ClearAvailableTags() {
            CollapseTags();
            flpTags.Controls.Clear();
            llblShowHide.Text = "             No Tags Available!";
            llblShowHide.Enabled = false;
            Height = llblShowHide.Bottom + 3;
        }
        public void CollapseTags() {
            flpTags.Visible = flpTags.AutoSize = false;
            Height = flpTags.Top + 19;

            if (CollapsedOrExpandedTags != null) CollapsedOrExpandedTags(this, null);
        }
        public void ExpandTags() {
            flpTags.AutoSize = flpTags.Visible = true;
            Height = flpTags.Bottom + 19;

            if (CollapsedOrExpandedTags != null) CollapsedOrExpandedTags(this, null);
        }

        private void llblShowHide_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (flpTags.Visible) CollapseTags(); else ExpandTags();
        }
    }
}
