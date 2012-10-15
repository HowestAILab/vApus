using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FastColoredTextBoxNS;

namespace vApus.Stresstest
{
    public class CodeTextBox : FastColoredTextBox
    {
        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (base.Text != value)
                {
                    base.Text = value;
                    SelectAll();
                    DoAutoIndent();
                    Selection.Start = Selection.End = Place.Empty;
                }
            }
        }

        public void ClearSelection()
        {
            this.Selection.Start = Place.Empty;
            this.DoCaretVisible();
        }
        /// <summary>
        /// Will select the line if it has it.
        /// </summary>
        /// <param name="lineNumber"></param>
        public void SelectLine(int lineNumber)
        {
            if (lineNumber < this.LinesCount)
            {
                int line = 0, start = 0, stop = 0;
                foreach (char c in this.Text)
                {
                    ++stop;
                    if (line < lineNumber)
                        ++start;
                    if (c == '\n' && ++line >= lineNumber && stop - start > 0)
                        break;
                }

                this.SelectionStart = start;
                this.SelectionLength = stop - start;

                this.DoSelectionVisible();
                this.ForceCreateCaret();
            }
        }

        /// <summary>
        /// Returns the lines of text where the given string was found and the absolute line numbers.
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

            string[] arr = this.Text.Split('\n');
            for (int i = 0; i != arr.Length; i++)
            {
                string line = arr[i];
                if (Regex.IsMatch(line, text, options))
                    found.Add(i, line.Trim());

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
            string[] arr = this.Text.Split('\n');
            for (int i = 0; i != arr.Length; i++)
            {
                string line = arr[i];
                if (found.ContainsKey(i))
                {
                    if (atLine == -1 || i == atLine)
                    {
                        string newLine = Regex.Replace(line, oldText, newText, options);

                        if (i == arr.Length - 1)
                            sb.Append(newLine);
                        else
                            sb.AppendLine(newLine);

                        line = line.Trim();
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
                    replacedOrFound.Add(i, line.Trim());
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

    }
}
