/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using vApus.Util;
using System.Drawing;

namespace vApus.Stresstest
{
    public partial class CodeBlockTextBox : UserControl
    {
        public new event EventHandler TextChanged;
        /// <summary>
        /// TextChangedDelayed event. It occurs after insert, delete, clear, undo and redo operations. This event occurs with a delay relative to TextChanged, and fires only once.
        /// </summary>
        public event EventHandler TextChangedDelayed;

        /// <summary>
        /// Use this event to decide when to set the line numbers.
        /// </summary>
        public event EventHandler LineCountChanged;
        /// <summary>
        /// Only occurs when focussed and the left, right, up, down, home or end was last pressed.
        /// </summary>
        public event EventHandler CaretPositionChangedUsingKeyboard;

        public event EventHandler EnterTextBox, LeaveTextBox;

        #region Fields
        private CSharpTextStyle _csharpTextStyle;

        private int _previousLineCount;
        private Keys _lastPressedKeys = Keys.None;

        //Workaround for when we don't want this event launched.
        private bool _canFireTextChangeDelayed = true;
        #endregion

        #region Properties
        public bool ShowLineNumbers
        {
            get { return fastColoredTextBox.ShowLineNumbers; }
            set 
            {
                fastColoredTextBox.ShowLineNumbers = value;
                fastColoredTextBox.ServiceLinesColor = fastColoredTextBox.ShowLineNumbers ? Color.Silver : fastColoredTextBox.BackColor;
            }
        }
        public int LineNumberOffset
        {
            get { return (int)fastColoredTextBox.LineNumberStartValue; }
        }
        public int CodeLineCount
        {
            get { return fastColoredTextBox.LinesCount; }
        }
        public bool ReadOnly
        {
            get { return fastColoredTextBox.ReadOnly; }
            set
            {
                if (fastColoredTextBox.ReadOnly != value)
                {
                    fastColoredTextBox.ReadOnly = value;
                    fastColoredTextBox.BackColor = fastColoredTextBox.ReadOnly ? Color.FromArgb(0xFFFFFE) : Color.White;
                }
            }
        }
        public override string Text
        {
            get { return fastColoredTextBox.Text; }
            set
            {
                if (fastColoredTextBox.Text != value)
                {
                    _canFireTextChangeDelayed = false;
                    fastColoredTextBox.TextChangedDelayed -= fastColoredTextBox_TextChangedDelayed;
                    fastColoredTextBox.SelectionChanged -= fastColoredTextBox_SelectionChanged;

                    fastColoredTextBox.Text = value;
                    fastColoredTextBox.SelectAll();
                    fastColoredTextBox.DoAutoIndent();
                    fastColoredTextBox.Selection.Start = Place.Empty;
                    fastColoredTextBox.Selection.End = Place.Empty;

                    fastColoredTextBox.SelectionChanged += fastColoredTextBox_SelectionChanged;
                    fastColoredTextBox.TextChangedDelayed += fastColoredTextBox_TextChangedDelayed;

                    fastColoredTextBox_SelectionChanged(this, null);
                }
            }
        }
        #endregion

        public CodeBlockTextBox()
        {
            InitializeComponent();
            _csharpTextStyle = new CSharpTextStyle(fastColoredTextBox);
        }

        #region Functions
        private void fastColoredTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_previousLineCount != CodeLineCount)
            {
                _previousLineCount = CodeLineCount;
                if (LineCountChanged != null)
                    LineCountChanged(this, e);
            }

            if (TextChanged != null)
                TextChanged(this, e);
        }
        private void fastColoredTextBox_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            if (_canFireTextChangeDelayed && TextChangedDelayed != null)
                TextChangedDelayed(this, e);

            _canFireTextChangeDelayed = true;
        }
        private void fastColoredTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            _lastPressedKeys = e.KeyCode;
        }
        //The only event fired before the selection changed
        private void fastColoredTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            _lastPressedKeys = Keys.None;
        }
        private void fastColoredTextBox_SelectionChanged(object sender, EventArgs e)
        {
            if (fastColoredTextBox.Focused && _lastPressedKeys != Keys.None && CaretPositionChangedUsingKeyboard != null)
                CaretPositionChangedUsingKeyboard(this, null);
            else
                _lastPressedKeys = Keys.None;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineNumberOffset">Cannot be smaller than 1.</param>
        public void SetLineNumbers(int lineNumberOffset = 1)
        {
            if (lineNumberOffset < 1)
                throw new ArgumentOutOfRangeException("lineNumberOffset cannot be smaller than 1.");
            _previousLineCount = CodeLineCount;
            fastColoredTextBox.LineNumberStartValue = (uint)lineNumberOffset;
        }
        public bool ContainsLine(int absoluteLineNumber)
        {
            return (absoluteLineNumber >= LineNumberOffset && absoluteLineNumber <= CodeLineCount + LineNumberOffset);
        }
        /// <summary>
        /// Will select the line if it has it.
        /// </summary>
        /// <param name="absoluteLineNumber"></param>
        public void SelectLine(int absoluteLineNumber)
        {
            if (ContainsLine(absoluteLineNumber))
            {
                int relativeLineNumber = absoluteLineNumber - LineNumberOffset;
                int line = 0, start = 0, stop = 0;
                foreach (char c in fastColoredTextBox.Text)
                {
                    ++stop;
                    if (line < relativeLineNumber)
                        ++start;
                    if (c == '\n' && ++line >= relativeLineNumber && stop - start > 0)
                        break;
                }

                fastColoredTextBox.SelectionStart = start;
                fastColoredTextBox.SelectionLength = stop - start;
                
                fastColoredTextBox.DoSelectionVisible();
                fastColoredTextBox.ForceCreateCaret();
            }
        }
        public void ClearSelection()
        {
            fastColoredTextBox.Selection.Start = Place.Empty;
            fastColoredTextBox.DoCaretVisible();
        }

        /// <summary>
        /// Returns the lines of text where the given string was found and the absolute line numbers.
        /// If it cannot be editted "[READ ONLY]\0" is added.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="wholeWords"></param>
        /// <param name="matchCase"></param>
        /// <returns></returns>
        public Dictionary<int, string> Find(string text, bool wholeWords = false, bool matchCase = false)
        {
            var found = new Dictionary<int, string>();

            text = Regex.Escape(text);
            if (wholeWords)
                text = "\\b" + text + "\\b";
            RegexOptions options = matchCase ? RegexOptions.Singleline : RegexOptions.Singleline | RegexOptions.IgnoreCase;

            string[] arr = fastColoredTextBox.Text.Split('\n');
            for (int i = 0; i != arr.Length; i++)
            {
                string line = arr[i];
                if (Regex.IsMatch(line, text, options))
                    if (ReadOnly)
                        found.Add(LineNumberOffset + i, line + " [READ ONLY]\0");
                    else
                        found.Add(LineNumberOffset + i, line);

            }
            return found;
        }
        /// <summary>
        /// Returns the replaced lines (formatted) and the absolute line numbers.
        /// The found lines are also returned.
        /// </summary>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        /// <param name="atLine">-1 for all (absolute line number)</param>
        /// <param name="wholeWords"></param>
        /// <param name="matchCase"></param>
        /// <returns></returns>
        public Dictionary<int, string> Replace(string oldText, string newText, int atLine, bool wholeWords = false, bool matchCase = false)
        {
            var replacedOrFound = new Dictionary<int, string>();
            var found = Find(oldText, wholeWords, matchCase);

            oldText = Regex.Escape(oldText);
            if (wholeWords)
                oldText = "\\b" + oldText + "\\b";
            RegexOptions options = matchCase ? RegexOptions.Singleline : RegexOptions.Singleline | RegexOptions.IgnoreCase;

            bool didReplace = false;
            StringBuilder sb = new StringBuilder();
            string[] arr = fastColoredTextBox.Text.Split('\n');
            for (int i = 0; i != arr.Length; i++)
            {
                int absoluteLineNumber = LineNumberOffset + i;
                string line = arr[i];
                if (found.ContainsKey(absoluteLineNumber))
                {
                    if (ReadOnly)
                    {
                        line = found[absoluteLineNumber];
                        if (i == arr.Length - 1)
                            sb.Append(line);
                        else
                            sb.AppendLine(line);
                    }
                    else if (atLine == -1 || absoluteLineNumber == atLine)
                    {
                        string newLine = Regex.Replace(line, oldText, newText, options);

                        if (i == arr.Length - 1)
                            sb.Append(newLine);
                        else
                            sb.AppendLine(newLine);

                        line += " > " + newLine;
                        didReplace = true;
                    }
                    else
                    {
                        if (i == arr.Length - 1)
                            sb.Append(line);
                        else
                            sb.AppendLine(line);
                    }
                    replacedOrFound.Add(absoluteLineNumber, line);
                }
                else
                {
                    if (i == arr.Length - 1)
                        sb.Append(line);
                    else
                        sb.AppendLine(line);
                }
            }
            if (didReplace)
                this.Text = sb.ToString();

            return replacedOrFound;
        }
        private void fastColoredTextBox_Enter(object sender, EventArgs e)
        {
            _lastPressedKeys = Keys.None;
            if (EnterTextBox != null)
                EnterTextBox(this, e);
        }
        private void fastColoredTextBox_Leave(object sender, EventArgs e)
        {
            _lastPressedKeys = Keys.None;
            if (LeaveTextBox != null)
                LeaveTextBox(this, e);
        }
        #endregion
    }
}