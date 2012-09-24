/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace vApus.Stresstest
{
    public partial class FindAndReplace : UserControl
    {
        public event EventHandler<FoundReplacedButtonClickedEventArgs> FoundReplacedButtonClicked;

        #region Fields
        public CodeTextBox CodeTextBox;
        #endregion

        public FindAndReplace()
        {
            InitializeComponent();
        }

        #region Functions

        #region Find
        private void btnFind_Click(object sender, EventArgs e)
        {
            Find(txtFind.Text, chkWholeWords.Checked, chkMatchCase.Checked);
        }
        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            btnFind.Enabled = txtFind.Text.Length > 0;
            Button btn = SelectedButton();
            if (chkReplaceAll.Checked)
                btnReplaceWith.Enabled = btnFind.Enabled;
            else
                btnReplaceWith.Enabled = btnFind.Enabled && ((btn == null) ? true : !btn.Text.EndsWith("[READ ONLY]\0"));

            btnSwitchValues.Enabled = txtFind.Text.Length != 0 && txtReplace.Text.Length != 0;
        }
        private void txtFind_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnFind.Enabled)
                Find(txtFind.Text, chkWholeWords.Checked, chkMatchCase.Checked);
        }
        public void Find(string text, bool wholeWords = false, bool matchCase = false)
        {
            this.Cursor = Cursors.WaitCursor;

            flpFoundReplaced.Controls.Clear();

            txtFind.Text = text;
            txtFind.SelectAll();
            chkWholeWords.Checked = wholeWords;
            chkMatchCase.Checked = matchCase;

            var found = CodeTextBox.Find(text, wholeWords, matchCase);
            foreach (int replaceLineNumber in found.Keys)
                AddFoundReplacedButton(replaceLineNumber, found[replaceLineNumber]);

            if (flpFoundReplaced.Controls.Count > 0)
                SelectButton(flpFoundReplaced.Controls[0] as Button);

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Replace
        private void btnReplaceWith_Click(object sender, EventArgs e)
        {
            Button btn = SelectedButton();
            if (btn == null)
                chkReplaceAll.Checked = true;
            int atLine = chkReplaceAll.Checked ? -1 : (int)btn.Tag;
            Replace(txtFind.Text, txtReplace.Text, atLine, chkWholeWords.Checked, chkMatchCase.Checked);
        }
        private void txtReplace_TextChanged(object sender, EventArgs e)
        {
            btnSwitchValues.Enabled = txtFind.Text.Length != 0 && txtReplace.Text.Length != 0;
        }
        private void txtReplace_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Button btn = SelectedButton();
                if (btn == null)
                    chkReplaceAll.Checked = true;
                int atLine = chkReplaceAll.Checked ? -1 : (int)btn.Tag;
                Replace(txtFind.Text, txtReplace.Text, atLine, chkWholeWords.Checked, chkMatchCase.Checked);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        /// <param name="atLine">-1 = all</param>
        /// <param name="wholeWords"></param>
        /// <param name="matchCase"></param>
        public void Replace(string oldText, string newText, int atLine, bool wholeWords = false, bool matchCase = false)
        {
            this.Cursor = Cursors.WaitCursor;

            flpFoundReplaced.Controls.Clear();

            txtFind.Text = oldText;
            txtFind.SelectAll();

            txtReplace.Text = newText;
            txtReplace.SelectAll();

            chkWholeWords.Checked = wholeWords;
            chkMatchCase.Checked = matchCase;
            chkReplaceAll.Checked = atLine == -1;

            var replaced = CodeTextBox.Replace(oldText, newText, atLine, wholeWords, matchCase);
            foreach (int relativeLineNumber in replaced.Keys)
                AddFoundReplacedButton(relativeLineNumber, replaced[relativeLineNumber]);

            if (flpFoundReplaced.Controls.Count > 0)
                SelectButton(flpFoundReplaced.Controls[0] as Button);

            this.Cursor = Cursors.Default;
        }
        private void chkReplaceAll_CheckedChanged(object sender, EventArgs e)
        {
            Button btn = SelectedButton();
            if (chkReplaceAll.Checked)
                btnReplaceWith.Enabled = btnFind.Enabled;
            else
                btnReplaceWith.Enabled = btnFind.Enabled && ((btn == null) ? true : !btn.Text.EndsWith("[READ ONLY]\0"));
        }
        #endregion

        private void AddFoundReplacedButton(int lineNumber, string foundReplaced)
        {
            Button btn = new Button();
            btn.AutoSize = true;
            btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn.Font = flpFoundReplaced.Font;
            btn.TextAlign = ContentAlignment.TopLeft;

            btn.FlatStyle = FlatStyle.Flat;

            btn.Text = (lineNumber + 1) + ") " + foundReplaced;
            btn.Tag = lineNumber;
            btn.Click += new EventHandler(btn_Click);

            flpFoundReplaced.Controls.Add(btn);

            btn.Width = flpFoundReplaced.ClientSize.Width - 18;
            int height = btn.Height;
            btn.AutoSize = false;
            btn.Height = height;
        }
        private void btn_Click(object sender, EventArgs e)
        {
            SelectButton(sender as Button);
        }
        private void GotoLine(int lineNumber)
        {
            if (FoundReplacedButtonClicked != null)
               FoundReplacedButtonClicked(this, new FoundReplacedButtonClickedEventArgs(lineNumber));
        }
        private void SelectButton(Button btn)
        {
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
        private Button SelectedButton()
        {
            foreach (Button btn in flpFoundReplaced.Controls)
                if (btn.BackColor == SystemColors.GradientActiveCaption)
                    return btn;
            return null;
        }

        private void btnSwitchValues_Click(object sender, EventArgs e)
        {
            if (txtReplace.Text == string.Empty)
                return;
            string find = txtFind.Text;
            txtFind.Text = txtReplace.Text;
            txtReplace.Text = find;
        }

        private void flpFoundReplaced_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control control in flpFoundReplaced.Controls)
                control.Width = flpFoundReplaced.ClientSize.Width - 18;
        }
        #endregion

        public class FoundReplacedButtonClickedEventArgs : EventArgs
        {
            public readonly int LineNumber;
            public FoundReplacedButtonClickedEventArgs(int lineNumber)
            {
                LineNumber = lineNumber;
            }
        }
    }
}
