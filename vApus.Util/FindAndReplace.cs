/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace vApus.Util {
    public static class FindAndReplace {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">You can use *, +, - "" like in Google.</param>
        /// <param name="inText"></param>
        /// <param name="rows">The rows where a match is found.</param>
        /// <param name="columns">The columns where a match is found.</param>
        /// <param name="matchLengths"></param>
        public static void Find(string s, string inText, out List<int> rows, out List<int> columns, out List<int> matchLengths, bool wholeWords, bool ignoreCase) {
            string text = inText.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\r", "\n");
            string[] lines = text.Split('\n');

            string wordDelimiter = wholeWords ? "\\b" : string.Empty;

            var mustHave = new List<string>(); //And relation
            var canHave = new List<string>(); //Or relation
            var cannotHave = new List<string>();

            //Split the string, quates are taken into account.
            //Add the parts to the different lists.
            foreach (string ss in Split(s)) {
                string sss = ss.Trim();
                bool addToMustHave = false;
                bool addToCannotHave = false;
                if (sss.StartsWith("+")) {
                    sss = (sss.Length == 1) ? string.Empty : sss.Substring(1);
                    addToMustHave = true;
                } else if (sss.StartsWith("-")) {
                    sss = (sss.Length == 1) ? string.Empty : sss.Substring(1);
                    addToCannotHave = true;
                }

                if (sss.Length == 0) {
                    continue;
                } else {
                    sss = Regex.Escape(sss);
                    //All characters except white space.
                    sss = sss.Replace("\\*", "([\\S{6,}])*");

                    if (addToMustHave) {
                        if (canHave.Contains(sss) || cannotHave.Contains(sss)) {
                            continue;
                        } else {
                            sss = "(" + wordDelimiter + ".*" + sss + wordDelimiter + ")";
                            if (!mustHave.Contains(sss)) mustHave.Add(sss);
                        }
                    } else if (addToCannotHave) {
                        if (canHave.Contains(sss) | mustHave.Contains(sss)) {
                            continue;
                        } else {
                            sss = "(" + wordDelimiter + sss + wordDelimiter + ")";
                            if (!cannotHave.Contains(sss)) cannotHave.Add(sss);
                        }
                    } else {
                        if (mustHave.Contains(sss) || cannotHave.Contains(sss)) {
                            continue;
                        } else {
                            sss = "(" + wordDelimiter + sss + wordDelimiter + ")";
                            if (!canHave.Contains(sss)) canHave.Add(sss);
                        }
                    }
                }

            }

            //Do the actual find
            string mustHaveRegex = mustHave.Combine(string.Empty);
            string canHaveRegex = canHave.Combine("|");
            string cannotHaveRegex = cannotHave.Combine("|");
            RegexOptions options = ignoreCase ? RegexOptions.Singleline | RegexOptions.IgnoreCase : RegexOptions.Singleline;

            rows = new List<int>();
            columns = new List<int>();
            matchLengths = new List<int>();

            for (int row = 0; row != lines.Length; row++) {
                string line = lines[row];
                //Exclude all rows that do not match.
                if (cannotHaveRegex.Length == 0 || !Regex.IsMatch(line, cannotHaveRegex, options)) {
                    //Include the rows that must match.
                    MatchCollection matches = null;
                    if (mustHaveRegex.Length != 0) {
                        matches = Regex.Matches(line, mustHaveRegex, options);
                        foreach (Match match in matches) {
                            rows.Add(row);
                            columns.Add(match.Index);
                            matchLengths.Add(match.Value.Length);
                        }
                    }
                    //Include the rows that match if they matched the previous regex if any.
                    if (mustHaveRegex.Length == 0 || matches.Count != 0) {
                        if (canHaveRegex.Length == 0) {
                            rows.Add(row);
                            columns.Add(0);
                            matchLengths.Add(line.Length);
                        } else {
                            matches = Regex.Matches(line, canHaveRegex, options);
                            foreach (Match match in matches) {
                                rows.Add(row);
                                columns.Add(match.Index);
                                matchLengths.Add(match.Value.Length);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// This takes double quotes into account
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string[] Split(string s) {
            string[] args = s.Split(' ');
            var argsCorrectSentenced = new List<string>();
            //Check if the array does not contain '"', if so make new sentences.
            bool quote = false;
            foreach (string ss in args) {
                if (ss.StartsWith("\"") && !quote) {
                    if (ss.Length > 1)
                        argsCorrectSentenced.Add(ss.Substring(1));
                    else
                        argsCorrectSentenced.Add("");
                    quote = true;
                } else if (ss.EndsWith("\"") && quote) {
                    if (ss.Length > 1)
                        argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " " + ss.Substring(0, ss.Length - 1);
                    else
                        argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " ";
                    quote = false;
                } else {
                    if (quote)
                        argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " " + ss;
                    else
                        argsCorrectSentenced.Add(ss);
                }
            }
            return argsCorrectSentenced.ToArray();
        }

        /// <summary>
        /// Call Find(...) first.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="matchLength"></param>
        /// <param name="inText"></param>
        /// <param name="with"></param>
        /// <returns></returns>
        public static string Replace(int row, int column, int matchLength, string inText, string with) {
            var rows = new List<int>(1);
            var columns = new List<int>(1);
            var matchLengths = new List<int>(1);

            rows.Add(row);
            columns.Add(column);
            matchLengths.Add(matchLength);

            return Replace(rows, columns, matchLengths, inText, with);
        }

        /// <summary>
        /// Call Find(...) first.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="matchLengths"></param>
        /// <param name="inText"></param>
        /// <param name="with"></param>
        /// <returns></returns>
        public static string Replace(List<int> rows, List<int> columns, List<int> matchLengths, string inText, string with) {
            string text = inText.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\r", "\n");
            string[] lines = text.Split('\n');

            var newLines = new string[lines.Length];
            int rowIndex = 0;
            for (int i = 0; i != lines.Length; i++) {
                var line = lines[i];

                //These will be reverserd, so if the text changes the indies will still be right.
                var partColumns = new List<int>();
                var partMatchLengths = new List<int>();

                while (rowIndex != rows.Count && rows[rowIndex] == i) {
                    partColumns.Add(columns[rowIndex]);
                    partMatchLengths.Add(matchLengths[rowIndex]);
                    ++rowIndex;
                }

                if (partColumns.Count != 0) {
                    partColumns.Sort();
                    partColumns.Reverse();

                    partMatchLengths.Sort();
                    partMatchLengths.Reverse();

                    for (int j = 0; j != partColumns.Count; j++) {
                        int column = partColumns[j];
                        line = line.Substring(0, column) + with + line.Substring(column + partMatchLengths[j]);
                    }
                }
                newLines[i] = line;
            }

            return newLines.Combine(Environment.NewLine);
        }

    }
}
