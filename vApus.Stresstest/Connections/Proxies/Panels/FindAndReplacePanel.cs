﻿/*
 * 2011 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.StressTest {
    public partial class FindAndReplacePanel : UserControl {
                public event EventHandler<FoundReplacedButtonClickedEventArgs> FoundReplacedButtonClicked;

        #region Fields

        public CodeTextBox CodeTextBox;

        #endregion


        public FindAndReplacePanel() {
            InitializeComponent();
        }


        #region Functions

        #region Find

        private void btnFind_Click(object sender, EventArgs e) {
            Find(txtFind.Text);
        }

        private void txtFind_TextChanged(object sender, EventArgs e) {
            btnFind.Enabled = txtFind.Text.Length > 0;
            Button btn = SelectedButton();
            if (chkReplaceAll.Checked)
                btnReplaceWith.Enabled = btnFind.Enabled;
            else
                btnReplaceWith.Enabled = btnFind.Enabled && ((btn == null) ? true : !btn.Text.EndsWith("[READ ONLY]\0"));

            btnSwitchValues.Enabled = txtFind.Text.Length != 0 && txtReplace.Text.Length != 0;
        }

        private void txtFind_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && btnFind.Enabled)
                Find(txtFind.Text);
        }

        public void Find(string text) {
            Cursor = Cursors.WaitCursor;

            flpFoundReplaced.Controls.Clear();

            txtFind.Text = text;

            bool wholeWords = chkWholeWords.Checked;
            bool matchCase = chkMatchCase.Checked;


            var buttons = new List<Button>();

            Dictionary<int, string> found = CodeTextBox.Find(text, wholeWords, matchCase);
            foreach (int relativeLineNumber in found.Keys)
               buttons.Add(CreateFoundReplaceButton(relativeLineNumber, found[relativeLineNumber]));

            flpFoundReplaced.Controls.AddRange(buttons.ToArray());

            this.Focus();
            txtFind.Focus();
            txtFind.SelectAll();
            Cursor = Cursors.Default;
        }

        #endregion

        #region Replace

        private void btnReplaceWith_Click(object sender, EventArgs e) {
            Button btn = SelectedButton();
            if (btn == null)
                chkReplaceAll.Checked = true;
            int atLine = chkReplaceAll.Checked ? -1 : (int)btn.Tag;
            Replace(txtFind.Text, txtReplace.Text, atLine, chkWholeWords.Checked, chkMatchCase.Checked);
        }

        private void txtReplace_TextChanged(object sender, EventArgs e) {
            btnSwitchValues.Enabled = txtFind.Text.Length != 0 && txtReplace.Text.Length != 0;
        }

        private void txtReplace_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                Button btn = SelectedButton();
                if (btn == null)
                    chkReplaceAll.Checked = true;
                int atLine = chkReplaceAll.Checked ? -1 : (int)btn.Tag;
                Replace(txtFind.Text, txtReplace.Text, atLine, chkWholeWords.Checked, chkMatchCase.Checked);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        /// <param name="atLine">-1 = all</param>
        /// <param name="wholeWords"></param>
        /// <param name="matchCase"></param>
        public void Replace(string oldText, string newText, int atLine, bool wholeWords = false, bool matchCase = false) {
            Cursor = Cursors.WaitCursor;

            flpFoundReplaced.Controls.Clear();

            txtFind.Text = oldText;
            txtFind.SelectAll();

            txtReplace.Text = newText;

            chkWholeWords.Checked = wholeWords;
            chkMatchCase.Checked = matchCase;
            chkReplaceAll.Checked = atLine == -1;

             var buttons = new List<Button>();

            Dictionary<int, string> replaced = CodeTextBox.Replace(oldText, newText, atLine, wholeWords, matchCase);
            foreach (int relativeLineNumber in replaced.Keys)
                buttons.Add(CreateFoundReplaceButton(relativeLineNumber, replaced[relativeLineNumber]));

            flpFoundReplaced.Controls.AddRange(buttons.ToArray());

            this.Focus();
            txtReplace.Focus();
            txtReplace.SelectAll();

            Cursor = Cursors.Default;
        }

        private void chkReplaceAll_CheckedChanged(object sender, EventArgs e) {
            Button btn = SelectedButton();
            if (chkReplaceAll.Checked)
                btnReplaceWith.Enabled = btnFind.Enabled;
            else
                btnReplaceWith.Enabled = btnFind.Enabled && ((btn == null) ? true : !btn.Text.EndsWith("[READ ONLY]\0"));
        }

        #endregion

        
        private Button CreateFoundReplaceButton(int lineNumber, string foundReplaced) {
            var btn = new Button();
            btn.AutoSize = true;
            btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn.Font = flpFoundReplaced.Font;
            btn.TextAlign = ContentAlignment.TopLeft;

            btn.FlatStyle = FlatStyle.Flat;

            btn.Text = (lineNumber + 1) + ") " + foundReplaced;
            btn.Tag = lineNumber;
            btn.Click += btn_Click;
            
            btn.AutoSize = false;
            btn.Width = flpFoundReplaced.ClientSize.Width - 18;
            btn.Height = 27;

            return btn;
        }

        private void btn_Click(object sender, EventArgs e) {
            SelectButton(sender as Button);
        }

        private void GotoLine(int lineNumber) {
            if (FoundReplacedButtonClicked != null)
                FoundReplacedButtonClicked(this, new FoundReplacedButtonClickedEventArgs(lineNumber));
        }

        private void SelectButton(Button btn) {
            GotoLine((int)btn.Tag);

            if (chkReplaceAll.Checked)
                btnReplaceWith.Enabled = btnFind.Enabled;
            else
                btnReplaceWith.Enabled = btnFind.Enabled && ((btn == null) ? true : !btn.Text.EndsWith("[READ ONLY]\0"));

            if (btn.BackColor == SystemColors.GradientActiveCaption)
                return;
            else
                foreach (Control ctrl in flpFoundReplaced.Controls)
                    ctrl.BackColor = (ctrl == btn) ? SystemColors.GradientActiveCaption : Color.White;
        }

        private Button SelectedButton() {
            foreach (Button btn in flpFoundReplaced.Controls)
                if (btn.BackColor == SystemColors.GradientActiveCaption)
                    return btn;
            return null;
        }

        private void btnSwitchValues_Click(object sender, EventArgs e) {
            if (txtReplace.Text == string.Empty)
                return;
            string find = txtFind.Text;
            txtFind.Text = txtReplace.Text;
            txtReplace.Text = find;
        }

        private void flpFoundReplaced_SizeChanged(object sender, EventArgs e) {
            foreach (Control control in flpFoundReplaced.Controls)
                control.Width = flpFoundReplaced.ClientSize.Width - 18;
        }

        #endregion

        public class FoundReplacedButtonClickedEventArgs : EventArgs {
            public readonly int LineNumber;

            public FoundReplacedButtonClickedEventArgs(int lineNumber) {
                LineNumber = lineNumber;
            }
        }
    }
}