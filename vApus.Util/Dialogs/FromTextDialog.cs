/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

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
        /// <summary>
        /// Auto remove the empty lines when OK is clicked.
        /// </summary>
        public bool AutoRemoveEmptyLines { get; set; }

        public bool ShowLineNumbers {
            get { return fctxt.ShowLineNumbers; }
            set { fctxt.ShowLineNumbers = value; }
        }
        #endregion

        #region Constructor

        /// Entries can be given one per line, those can be returned using the Entries property.
        public FromTextDialog() {
            InitializeComponent();
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
            if (AutoRemoveEmptyLines) {
                var sb = new StringBuilder();
                foreach (string line in fctxt.Lines)
                    if (!string.IsNullOrWhiteSpace(line))
                        sb.AppendLine(line);
                fctxt.Text = sb.ToString().Trim();
            }

            _entries = fctxt.Lines;
            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion
    }
}