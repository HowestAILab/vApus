/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using FastColoredTextBoxNS;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace vApus.Util {
    public class CodeTextBox : FastColoredTextBox {
        public CodeTextBox()
            : base() {
            this.DefaultContextMenu(true);
        }
        public override string Text {
            get { return base.Text; }
            set {
                if (base.Text != value) {
                    base.Text = value;
                    SelectAll();
                    DoAutoIndent();
                    Selection.Start = Selection.End = Place.Empty;
                }
            }
        }

        public void ClearSelection() {
            Selection.Start = Place.Empty;
            DoCaretVisible();
        }

        /// <summary>
        ///     Will select the line if it has it.
        /// </summary>
        /// <param name="lineNumber"></param>
        public void SelectLine(int lineNumber) {
            if (lineNumber < LinesCount) {
                int line = 0, start = 0, stop = 0;
                foreach (char c in Text) {
                    ++stop;
                    if (line < lineNumber)
                        ++start;
                    if (c == '\n' && ++line >= lineNumber && stop - start > 0)
                        break;
                }

                SelectionStart = start;
                SelectionLength = stop - start;

                DoSelectionVisible();
                Focus();
            }
        }

        /// <summary>
        ///     Returns the lines of text where the given string was found and the absolute line numbers.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="wholeWords"></param>
        /// <param name="matchCase"></param>
        /// <returns></returns>
        public Dictionary<int, string> Find(string text, bool wholeWords = false, bool matchCase = false) {
            var found = new Dictionary<int, string>();

            text = Regex.Escape(text);
            if (wholeWords)
                text = "\\b" + text + "\\b";
            RegexOptions options = matchCase
                                       ? RegexOptions.Singleline
                                       : RegexOptions.Singleline | RegexOptions.IgnoreCase;

            string[] arr = Text.Split('\n');
            for (int i = 0; i != arr.Length; i++) {
                string line = arr[i];
                if (Regex.IsMatch(line, text, options))
                    found.Add(i, line.Trim());
            }
            return found;
        }

        /// <summary>
        ///     Returns the replaced lines (formatted) and the absolute line numbers.
        ///     The found lines are also returned.
        /// </summary>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        /// <param name="atLine">-1 for all (absolute line number)</param>
        /// <param name="wholeWords"></param>
        /// <param name="matchCase"></param>
        /// <returns></returns>
        public Dictionary<int, string> Replace(string oldText, string newText, int atLine, bool wholeWords = false,
                                               bool matchCase = false) {
            var replacedOrFound = new Dictionary<int, string>();
            Dictionary<int, string> found = Find(oldText, wholeWords, matchCase);

            oldText = Regex.Escape(oldText);
            if (wholeWords)
                oldText = "\\b" + oldText + "\\b";
            RegexOptions options = matchCase
                                       ? RegexOptions.Singleline
                                       : RegexOptions.Singleline | RegexOptions.IgnoreCase;

            bool didReplace = false;
            var sb = new StringBuilder();
            string[] arr = Text.Split('\n');
            for (int i = 0; i != arr.Length; i++) {
                string line = arr[i];
                if (found.ContainsKey(i)) {
                    if (atLine == -1 || i == atLine) {
                        string newLine = Regex.Replace(line, oldText, newText, options);

                        if (i == arr.Length - 1)
                            sb.Append(newLine);
                        else
                            sb.AppendLine(newLine);

                        line = line.Trim();
                        line += " > " + newLine;
                        didReplace = true;
                    } else {
                        if (i == arr.Length - 1)
                            sb.Append(line);
                        else
                            sb.AppendLine(line);
                    }
                    replacedOrFound.Add(i, line.Trim());
                } else {
                    if (i == arr.Length - 1)
                        sb.Append(line);
                    else
                        sb.AppendLine(line);
                }
            }
            if (didReplace)
                Text = sb.ToString();

            return replacedOrFound;
        }
    }
}