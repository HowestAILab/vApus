/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class EditLog : UserControl {
        public event EventHandler LogImported;

        #region Fields
        private Log _log;
        private static string[] _defaultDeny = { "addthis.com", "apis.google.com", "cloudflare.com", "facebook.com", "google-analytics.com", "googleapis.com", "linkedin.com", "m.addthisedge.com", 
                                                 "nedstatbasic.net", "plusone.google.com", "ssl.gstatic.com", "twimg.com", "twitter.com", "youtube.com" };

        private string _beginTokenDelimiter;
        private string _endTokenDelimiter;

        private Parameters _parameters;
        private ParameterTokenTextStyle _parameterTokenTextStyle;

        private const string VBLRn = "<16 0C 02 12n>";
        private const string VBLRr = "<16 0C 02 12r>";
        private static string _multilineComment = string.Empty;

        #endregion

        public EditLog() {
            InitializeComponent();
            try {
                _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
                SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
            } catch { }
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender is CustomListParameters || sender is CustomListParameter || sender is CustomRandomParameters || sender is CustomRandomParameter
                || sender is NumericParameters || sender is NumericParameter || sender is TextParameters || sender is TextParameter) {
                SetCodeStyle();
            }
        }

        internal void SetLog(Log log) {
            tmrRemoveEmptyCells.Stop();
            _log = log;

            bool warning, error;
            _log.GetUniqueParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out warning, out error);

            SetCodeStyle();

            chkAllow.Checked = _log.UseAllow;
            chkDeny.Checked = _log.UseDeny;

            foreach (string s in _log.Allow) dgvAllow.Rows.Add(s);

            var deny = _log.Deny.Length == 0 ? _defaultDeny : _log.Deny;
            foreach (string s in deny) dgvDeny.Rows.Add(s);
            tmrRemoveEmptyCells.Start();
        }

        private void chkAllow_CheckedChanged(object sender, EventArgs e) {
            dgvAllow.Enabled = chkAllow.Checked;
            dgvAllow.DefaultCellStyle.ForeColor = dgvAllow.ColumnHeadersDefaultCellStyle.ForeColor = dgvAllow.Enabled ? SystemColors.ControlText : SystemColors.ControlDarkDark;

            if (tmrRemoveEmptyCells.Enabled) {
                _log.UseAllow = chkAllow.Checked;
                _log.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        private void chkDeny_CheckedChanged(object sender, EventArgs e) {
            dgvDeny.Enabled = chkDeny.Checked;
            dgvDeny.DefaultCellStyle.ForeColor = dgvDeny.ColumnHeadersDefaultCellStyle.ForeColor = dgvDeny.Enabled ? SystemColors.ControlText : SystemColors.ControlDarkDark;

            if (tmrRemoveEmptyCells.Enabled) {
                _log.UseDeny = chkDeny.Checked;
                _log.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            var dgv = sender as DataGridView;
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[0];
            IPAddress ip;
            try {
                if (!IPAddress.TryParse(cell.Value.ToString(), out ip))
                    Dns.GetHostEntry(cell.Value.ToString());
            } catch {
                cell.Value = null;
            }

            if (tmrRemoveEmptyCells.Enabled) {
                string[] arr = new string[dgv.Rows.Count - 1];
                for (int i = 0; i != dgv.RowCount - 1; i++)
                    if (dgv.Rows[i].Cells[0].Value != null)
                        arr[i] = dgv.Rows[i].Cells[0].Value as string;

                if (sender == dgvAllow) _log.Allow = arr; else _log.Deny = arr;
                _log.InvokeSolutionComponentChangedEvent(SolutionTree.SolutionComponentChangedEventArgs.DoneAction.Edited);
            }
        }
        private void tmrRemoveEmptyCells_Tick(object sender, EventArgs e) {
            try {
                if (!dgvAllow.IsCurrentCellInEditMode)
                    for (int i = 0; i != dgvAllow.RowCount - 1; i++)
                        if (dgvAllow.Rows[i].Cells[0].Value == null)
                            dgvAllow.Rows.RemoveAt(i);

                if (!dgvDeny.IsCurrentCellInEditMode)
                    for (int i = 0; i != dgvDeny.RowCount - 1; i++)
                        if (dgvDeny.Rows[i].Cells[0].Value == null)
                            dgvDeny.Rows.RemoveAt(i);

                if (dgvDeny.Rows.Count <= 1) {
                    tmrRemoveEmptyCells.Stop();
                    foreach (string s in _defaultDeny) dgvDeny.Rows.Add(s);
                    tmrRemoveEmptyCells.Start();
                }

            } catch { }
        }

        private void btnBrowse_Click(object sender, EventArgs e) {
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                fctxtxImport.Clear();
                var sb = new StringBuilder();
                foreach (string fileName in openFileDialog.FileNames)
                    using (var sr = new StreamReader(fileName))
                        sb.AppendLine(sr.ReadToEnd());

                fctxtxImport.Text = sb.ToString().Trim();
            }
        }

        private void SetCodeStyle() {
            BaseItem customListParameters = _parameters[0];
            BaseItem numericParameters = _parameters[1];
            BaseItem textParameters = _parameters[2];
            BaseItem customRandomParameters = _parameters[3];

            var scopeIdentifiers = new[] { ASTNode.ALWAYS_PARAMETER_SCOPE, ASTNode.LEAF_NODE_PARAMETER_SCOPE, ASTNode.LOG_ENTRY_PARAMETER_SCOPE, 
                ASTNode.USER_ACTION_PARAMETER_SCOPE, ASTNode.LOG_PARAMETER_SCOPE };


            int index;
            List<string> clp = new List<string>(),
                         np = new List<string>(),
                         tp = new List<string>(),
                         crp = new List<string>();
            foreach (string scopeIdentifier in scopeIdentifiers) {
                index = 1;
                for (int i = 0; i < customListParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    clp.Add(token);
                }
                for (int i = 0; i < numericParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    np.Add(token);
                }
                for (int i = 0; i < textParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    tp.Add(token);
                }
                for (int i = 0; i < customRandomParameters.Count; i++) {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    crp.Add(token);
                }
            }
            fctxtxImport.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);
            fctxtxImport.Range.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);

            if (_parameterTokenTextStyle != null) {
                _parameterTokenTextStyle.Dispose();
                _parameterTokenTextStyle = null;
            }
            _parameterTokenTextStyle = new ParameterTokenTextStyle(fctxtxImport, GetDelimiters(_log.LogRuleSet), clp, np, tp, crp, true);
        }
        private string[] GetDelimiters(LogRuleSet logRuleSet) {
            var hs = new HashSet<string>();
            if (logRuleSet.ChildDelimiter.Length != 0)
                hs.Add(logRuleSet.ChildDelimiter);

            foreach (BaseItem item in logRuleSet)
                if (item is LogSyntaxItem)
                    foreach (string delimiter in GetDelimiters(item as LogSyntaxItem))
                        hs.Add(delimiter);

            var delimiters = new string[hs.Count];
            hs.CopyTo(delimiters);
            hs = null;

            return delimiters;
        }
        private IEnumerable<string> GetDelimiters(LogSyntaxItem logSyntaxItem) {
            if (logSyntaxItem.ChildDelimiter.Length != 0)
                yield return logSyntaxItem.ChildDelimiter;
            foreach (BaseItem item in logSyntaxItem)
                if (item is LogSyntaxItem)
                    foreach (string delimiter in GetDelimiters(item as LogSyntaxItem))
                        yield return delimiter;
        }

        private void fctxtxImport_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
            btnImport.Enabled = fctxtxImport.Text.Trim().Length != 0;
        }

        private void btnImport_Click(object sender, EventArgs e) {
            if (chkClearLogBeforeImport.Checked &&
                MessageBox.Show("Are you sure you want to clear and import?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                _log.ClearWithoutInvokingEvent(false);
            else
                return;

            UserAction userAction = null;

            string toImport = fctxtxImport.Text;
            string[] splitter = new string[] { "\n", "\r" };
            foreach (string line in toImport.Split(splitter, StringSplitOptions.RemoveEmptyEntries)) {
                if (line.Trim().Length == 0)
                    continue;

                string output;
                if (DetermineComment(line, out output)) {
                    if (_log.LogRuleSet.ActionizeOnComment) {
                        userAction = new UserAction(output);
                        _log.AddWithoutInvokingEvent(userAction, false);
                    }
                } else if (userAction == null) {
                    var logEntry = new LogEntry(output.Replace(VBLRn, "\n").Replace(VBLRr, "\r"));
                    var ua = new UserAction(logEntry.LogEntryString.Length < 101 ? logEntry.LogEntryString : logEntry.LogEntryString.Substring(0, 100) + "...");
                    ua.LogEntryStringsAsImported.Add(logEntry.LogEntryString);
                    ua.AddWithoutInvokingEvent(logEntry, false);
                    _log.AddWithoutInvokingEvent(ua, false);
                } else {
                    var logEntry = new LogEntry(output.Replace(VBLRn, "\n").Replace(VBLRr, "\r"));
                    userAction.AddWithoutInvokingEvent(logEntry, false);
                    userAction.LogEntryStringsAsImported.Add(logEntry.LogEntryString);
                }
            }

            RemoveEmptyUserActions();

            //#if EnableBetaFeature
            //            bool successfullyParallized = SetParallelExecutions();
            //#else
#warning Parallel executions temp not available
            bool successfullyParallized = true;
            //#endif
            SetIgnoreDelays();
            // FillLargeList();

            if (!successfullyParallized) {
                string message = Text + ": Could not determine the begin- and end timestamps for one or more log entries in the different user actions, are they correctly formatted?";
                LogWrapper.LogByLevel(message, LogLevel.Error);
            }

            _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

            if (LogImported != null)
                LogImported(this, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns>True if is comment</returns>
        private bool DetermineComment(string input, out string output) {
            int singleline = -1;
            int multiline = -1;

            input = input.TrimStart().Trim();
            output = input;

            if (_multilineComment.Length > 0) {
                multiline = input.IndexOf(_log.LogRuleSet.EndCommentString);
                if (multiline > -1) {
                    output = input.Substring(multiline + _log.LogRuleSet.EndCommentString.Length);
                    if (output.Length == 0) {
                        if (input.Length > _log.LogRuleSet.EndCommentString.Length + 1)
                            output =
                                FormatComment(_multilineComment.TrimStart() + ' ' +
                                              input.Substring(0, input.Length - _log.LogRuleSet.EndCommentString.Length));
                        else
                            output = FormatComment(_multilineComment.TrimStart());
                        _multilineComment = string.Empty;
                        return true;
                    }
                    _multilineComment = string.Empty;
                    output = input.Substring(multiline + _log.LogRuleSet.EndCommentString.Length);
                    return true;
                }
                if (input.Length > 0)
                    _multilineComment = _multilineComment + ' ' + input;
                output = string.Empty;
                return true;
            }

            if (_log.LogRuleSet.SingleLineCommentString.Length > 0)
                singleline = input.IndexOf(_log.LogRuleSet.SingleLineCommentString);
            if (_log.LogRuleSet.BeginCommentString.Length > 0)
                multiline = input.IndexOf(_log.LogRuleSet.BeginCommentString);

            if (singleline > -1 && multiline == -1)
                multiline = int.MaxValue;
            else if (multiline > -1 && singleline == -1)
                singleline = int.MaxValue;

            if (singleline > -1 && singleline < multiline) {
                _multilineComment = string.Empty;
                if (singleline == 0)
                    output = FormatComment(input.Substring(_log.LogRuleSet.SingleLineCommentString.Length));
                return true;
            } else if (multiline > -1 && multiline < singleline) {
                int multilineCopy = input.IndexOf(_log.LogRuleSet.EndCommentString);
                if (multilineCopy > -1 && multilineCopy > multiline) {
                    _multilineComment = string.Empty;
                    output = input.TrimStart().Substring(0, multiline) +
                             input.Substring(multilineCopy + _log.LogRuleSet.EndCommentString.Length);
                    if (output.Length == 0)
                        output =
                            FormatComment(input.Substring(multiline + _log.LogRuleSet.BeginCommentString.Length,
                                                          input.Length - _log.LogRuleSet.EndCommentString.Length -
                                                          _log.LogRuleSet.BeginCommentString.Length));
                    return true;
                } else {
                    _multilineComment = input.Substring(multiline + _log.LogRuleSet.BeginCommentString.Length);
                    if (_multilineComment.Length == 0)
                        _multilineComment = " ";
                    output = input.Substring(0, multiline);
                    return true;
                }
            }
            return false;
        }

        private string FormatComment(string input) {
            int i = 0;
            input = input.TrimStart();
            var sb = new StringBuilder(255);
            foreach (char c in input) {
                sb.Append(c);
                if (++i == 255)
                    break;
            }
            return sb.ToString();
        }

        private void RemoveEmptyUserActions() {
            var emptyUserActions = new List<BaseItem>(_log.Count);
            foreach (BaseItem item in _log)
                if (item is UserAction && item.Count == 0)
                    emptyUserActions.Add(item);

            foreach (BaseItem item in emptyUserActions)
                _log.RemoveWithoutInvokingEvent(item);
        }
        private void SetIgnoreDelays() {
            foreach (BaseItem item in _log)
                if (item is UserAction) {
                    var userAction = item as UserAction;
                    //Determine the non parallel log entries, set ignore delay for the other ones (must always be ignored for these)
                    var nonParallelLogEntries = new List<LogEntry>();
                    foreach (LogEntry entry in userAction)
                        if (entry.ExecuteInParallelWithPrevious)
                            entry.IgnoreDelay = true;
                        else
                            nonParallelLogEntries.Add(entry);

                    //Then set ignore delays for all but the last
                    for (int i = 0; i < nonParallelLogEntries.Count - 1; i++)
                        nonParallelLogEntries[i].IgnoreDelay = true;
                }
        }

    }
}
