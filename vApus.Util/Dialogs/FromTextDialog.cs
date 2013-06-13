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

namespace vApus.Util {
    public partial class FromTextDialog : Form {
        #region Fields
        private string[] _entries = new string[] { };
        #endregion

        #region Properties

        public string[] Entries {
            get { return _entries; }
        }
        public string Description {
            get { return rtxtDescription.Text; }
            set {
                rtxtDescription.Text = value.Trim();
                split.Panel1Collapsed = rtxtDescription.Text.Length == 0;
            }
        }
        [DefaultValue(true)]
        public bool WarnForEndingWithNewLine { get; set; }
        #endregion

        #region Constructor

        public FromTextDialog() {
            InitializeComponent();
            WarnForEndingWithNewLine = true;
        }

        #endregion

        #region Functions

        public void SetText(string text) {
            rtxt.Text = text;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            if (WarnForEndingWithNewLine &&
                (rtxt.Text.EndsWith("\r") || rtxt.Text.EndsWith("\n")) &&
                MessageBox.Show("The text ends with one ore more new line characters, do you want to trim these?",
                                string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                rtxt.Text = rtxt.Text.TrimEnd();

            _entries = rtxt.Text.Split('\n');
            DialogResult = DialogResult.OK;
            Close();
        }

        private void rtxt_KeyPress(object sender, KeyPressEventArgs e) {
            //Paste
            if (e.KeyChar == (char)22) {
                string text = rtxt.Text;
                rtxt.Rtf = string.Empty;
                rtxt.Text = text;
            }
        }

        #endregion
    }
}