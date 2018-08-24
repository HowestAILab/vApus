/*
 * 2013 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;

namespace vApus.StressTest {
    public partial class FindAndReplaceDialog : Form {
        public event EventHandler<FindEventArgs> FindClicked;
        public event EventHandler<ReplaceEventArgs> ReplaceClicked;

        #region Constructors
        public FindAndReplaceDialog() { InitializeComponent(); }
        #endregion

        #region Functions
        /// <summary>
        /// Update the find textbox if the find string gets updated in ScenarioView.
        /// </summary>
        /// <param name="find"></param>
        public void SetFind(string find) { txtFind.Text = find; }

        private void btnFind_Click(object sender, EventArgs e) { if (FindClicked != null) FindClicked(this, new FindEventArgs(txtFind.Text, chkWholeWords.Checked, !chkMatchCase.Checked)); }
        private void txtFind_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter && btnFind.Enabled) btnFind.PerformClick(); }

        private void btnReplaceWith_Click(object sender, EventArgs e) { if (ReplaceClicked != null) ReplaceClicked(this, new ReplaceEventArgs(txtFind.Text, chkWholeWords.Checked, !chkMatchCase.Checked, txtReplace.Text, chkReplaceAll.Checked)); }
        private void txtFind_TextChanged(object sender, EventArgs e) {
            btnFind.Enabled = txtFind.Text.Length != 0;
            btnReplaceWith.Enabled = txtFind.Text.Length != 0 && txtFind.Text != txtReplace.Text;
        }
        private void txtReplace_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter && btnReplaceWith.Enabled) btnReplaceWith.PerformClick(); }

        private void txtReplace_TextChanged(object sender, EventArgs e) { btnReplaceWith.Enabled = txtFind.Text.Length != 0 && txtFind.Text != txtReplace.Text; }
        #endregion

        public class FindEventArgs : EventArgs {
            public string Find { get; private set; }
            public bool WholeWords { get; private set; }
            public bool IgnoreCase { get; private set; }
            public FindEventArgs(string find, bool wholeWords, bool ignoreCase) {
                Find = find;
                WholeWords = wholeWords;
                IgnoreCase = ignoreCase;
            }
        }
        public class ReplaceEventArgs : EventArgs {
            public string Find { get; private set; }
            public bool WholeWords { get; private set; }
            public bool IgnoreCase { get; private set; }
            public string With { get; private set; }
            public bool All { get; private set; }
            public ReplaceEventArgs(string find, bool wholeWords, bool ignoreCase, string with, bool all) {
                Find = find;
                WholeWords = wholeWords;
                IgnoreCase = ignoreCase;
                With = with;
                All = all;
            }
        }
    }
}
