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
using System.Threading;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Results {
    /// <summary>
    ///     Adds description and tags to the results database.
    /// </summary>
    public partial class DescriptionAndTagsInputDialog : Form {
        private string _description;
        private string[] _tags;
        private ResultsHelper _resultsHelper;

        /// <summary>
        /// Don't forget to set the results helper.
        /// </summary>
        public DescriptionAndTagsInputDialog() {
            InitializeComponent();
        }

        public string Description {
            get { return _description; }
            set {
                if (value.Length == 0)
                    FocusDescription();
                else {
                    _description = value;
                    txtDescription.Text = _description;
                }
            }
        }
        public string[] Tags {
            get { return _tags; }
            set {
                if (value.Length != 0) {
                    _tags = value;
                    txtTags.Text = _tags.Combine(", ");
                }
            }
        }
        public ResultsHelper ResultsHelper { set { _resultsHelper = value; } }
        private void FocusDescription() {
            var t = new Thread(delegate() {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                    try { txtDescription.Focus(); } catch { }
                }, null);
            });
            t.IsBackground = true;
            t.Start();
        }

        private void btnOK_Click(object sender, EventArgs e) {
            _description = txtDescription.ForeColor == Color.DimGray ? string.Empty : txtDescription.Text.Trim();
            string tagstring = txtTags.ForeColor == Color.DimGray ? string.Empty : txtTags.Text.Trim();
            var tags = new List<string>();
            foreach (string tag in tagstring.Split(',')) {
                string t = tag.Trim();
                if (!tags.Contains(t)) tags.Add(t);
            }

            _tags = tags.ToArray();

            try {
                _resultsHelper.SetDescriptionAndTags(Description, Tags);
            } catch (Exception ex) {
                LogWrapper.LogByLevel("Could not add the description and tags to the database.\n" + ex, LogLevel.Error);
            }
            DialogResult = DialogResult.OK;
            try { Close(); } catch { }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}