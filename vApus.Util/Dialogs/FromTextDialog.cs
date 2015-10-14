/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

namespace vApus.Util {
    /// <summary>
    /// Entries can be given one per line, those can be returned using the Entries property.
    /// </summary>
    public partial class FromTextDialog : Form {

        #region Fields
        private IList<string> _entries = new List<string>();
        #endregion

        #region Properties

        public IList<string> Entries { get { return _entries; } }

        public string Description {
            get { return rtxtDescription.Text; }
            set {
                rtxtDescription.Text = value.Trim();
                split.Panel1Collapsed = rtxtDescription.Text.Length == 0;
                fctxt.Focus();
                fctxt.Select();
                fctxt.SelectionStart = fctxt.Text.Length;
            }
        }
        [DefaultValue(true)]
        public bool WarnForEndingWithNewLine { get; set; }
        #endregion

        #region Constructor

        /// Entries can be given one per line, those can be returned using the Entries property.
        public FromTextDialog() {
            InitializeComponent();
            WarnForEndingWithNewLine = true;

            fctxt.DefaultContextMenu(true);
        }

        #endregion

        #region Functions

        public void SetText(string text) {
            fctxt.Text = text;
            fctxt.Focus();
            fctxt.Select();
            fctxt.SelectionStart = fctxt.Text.Length;
        }
        public string GetText() {
            return fctxt.Text.Trim();
        }

        private void btnOK_Click(object sender, EventArgs e) {
            if (WarnForEndingWithNewLine &&
                (fctxt.Text.EndsWith("\r") || fctxt.Text.EndsWith("\n")) &&
                MessageBox.Show("The text ends with one ore more new line characters, do you want to trim these?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                fctxt.Text = fctxt.Text.TrimEnd();

            _entries = fctxt.Lines;
            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion
    }
}