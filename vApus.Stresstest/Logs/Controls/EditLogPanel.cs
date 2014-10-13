/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// Capture HTTP(S) traffic, import a log from text and extra tools.
    /// Call SetLog before doing anything else.
    /// </summary>
    public partial class EditLogPanel : UserControl {
        public event EventHandler LogImported, RevertedToAsImported, RedeterminedTokens;

        #region Fields
        private Log _log;
        private string _beginTokenDelimiter;
        private string _endTokenDelimiter;

        private Parameters _parameters;
        private ParameterTokenTextStyle _parameterTokenTextStyle;

        private const string VBLRn = "<16 0C 02 12n>";
        private const string VBLRr = "<16 0C 02 12r>";
        private static string _multilineComment = string.Empty;

        #endregion

        #region Constructors
        /// <summary>
        /// Capture HTTP(S) traffic, import a log from text and extra tools.
        /// Call SetLog before doing anything else.
        /// </summary>
        public EditLogPanel() {
            InitializeComponent();
            try {
                _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
                SolutionComponent.SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
            } catch {
                //Should / can never happen.
            }
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            if (sender is CustomListParameters || sender is CustomListParameter || sender is CustomRandomParameters || sender is CustomRandomParameter
                || sender is NumericParameters || sender is NumericParameter || sender is TextParameters || sender is TextParameter) {
                SetCodeStyle();
            }
        }

        internal void SetLog(Log log) {
            _log = log;

            bool logEntryContainsTokens;
            _log.GetParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out logEntryContainsTokens, false);

            SetCodeStyle();

            captureControl.UseAllow = _log.UseAllow;
            captureControl.UseDeny = _log.UseDeny;
            captureControl.AllowIncludeReferer = _log.AllowIncludeReferer;

            captureControl.Allow = _log.Allow;
            captureControl.Deny = _log.Deny;
        }

        #region Capture HTTP(S)
        private void captureControl_StartClicked(object sender, EventArgs e) {
            if (chkClearLogBeforeCapture.Checked && _log.Count != 0)
                if (MessageBox.Show("Are you sure you want to clear the log?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                    _log.ClearWithoutInvokingEvent();
                } else {
                    captureControl.CancelStart();
                    return;
                }
            SaveCaptureSettings();
        }
        private void captureControl_StopClicked(object sender, EventArgs e) {
            SaveCaptureSettings();
            Import(captureControl.ParsedLog, false);
            try {
                ProxyHelper.UnsetProxy();
            } catch (Exception ex) {
                Loggers.Log(Level.Warning, "Failed to unset the proxy.", ex);
            }
        }
        private void SaveCaptureSettings() {
            try {
                if (_log != null) {
                    bool editted = false;
                    if (_log.UseAllow != captureControl.UseAllow) {
                        _log.UseAllow = captureControl.UseAllow;
                        editted = true;
                    }
                    if (_log.Allow.Length != captureControl.Allow.Length) {
                        _log.Allow = captureControl.Allow;
                        editted = true;
                    }
                    if (_log.AllowIncludeReferer != captureControl.AllowIncludeReferer) {
                        _log.AllowIncludeReferer = captureControl.AllowIncludeReferer;
                        editted = true;
                    }
                    if (_log.UseDeny != captureControl.UseDeny) {
                        _log.UseDeny = captureControl.UseDeny;
                        editted = true;
                    }
                    if (_log.Deny.Length != captureControl.Deny.Length) {
                        _log.Deny = captureControl.Deny;
                        editted = true;
                    }
                    if (editted)
                        _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed saving capture settings.", ex);
            }
        }
        #endregion

        #region Import from Text
        private void fctxtxImport_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) { btnImport.Enabled = fctxtxImport.Text.Trim().Length != 0; }

        private void btnBrowse_Click(object sender, EventArgs e) {
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                fctxtxImport.Clear();
                var sb = new StringBuilder();
                foreach (string fileName in openFileDialog.FileNames)
                    using (var sr = new StreamReader(fileName))
                        sb.AppendLine(sr.ReadToEnd());

                fctxtxImport.Text = sb.ToString().Trim();

                try {
                    bool logEntryContainsTokens;
                    _log.GetParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out logEntryContainsTokens, false);
                    SetCodeStyle();
                } catch(Exception ex) {
                    Loggers.Log(Level.Error, "Failed getting parameter token delimiters + setting code style.", ex, new object[] { sender, e });
                }
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
            List<string> clp = new List<string>(), np = new List<string>(), tp = new List<string>(), crp = new List<string>();
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

        private void btnImport_Click(object sender, EventArgs e) { Import(fctxtxImport.Text, chkClearLogBeforeImport.Checked); }
        private void Import(string text, bool clearLog) {
            //Clone and add to the clone to redetermine the tokens if needed.
            Log toAdd = _log.Clone(false, false, false, false);

            if (clearLog && _log.Count != 0)
                if (MessageBox.Show("Are you sure you want to clear the log?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    _log.ClearWithoutInvokingEvent();
                else
                    return;

            UserAction userAction = null;

            string[] splitter = new string[] { "\n", "\r" };
            foreach (string line in text.Split(splitter, StringSplitOptions.RemoveEmptyEntries)) {
                if (line.Trim().Length == 0)
                    continue;

                string output;
                if (DetermineComment(line, out output)) {
                    if (toAdd.LogRuleSet.ActionizeOnComment) {
                        userAction = new UserAction(output);
                        toAdd.AddWithoutInvokingEvent(userAction);
                    }
                } else if (userAction == null) {
                    var logEntry = new LogEntry(output.Replace(VBLRn, "\n").Replace(VBLRr, "\r"));
                    var ua = new UserAction(logEntry.LogEntryString.Length < 101 ? logEntry.LogEntryString : logEntry.LogEntryString.Substring(0, 100) + "...");
                    ua.LogEntryStringsAsImported.Add(logEntry.LogEntryString);
                    ua.AddWithoutInvokingEvent(logEntry);
                    toAdd.AddWithoutInvokingEvent(ua);
                } else {
                    var logEntry = new LogEntry(output.Replace(VBLRn, "\n").Replace(VBLRr, "\r"));
                    userAction.AddWithoutInvokingEvent(logEntry);
                    userAction.LogEntryStringsAsImported.Add(logEntry.LogEntryString);
                }
            }

            string beginTokenDelimiter, endTokenDelimiter;
            bool logEntryContainsTokens;
            toAdd.GetParameterTokenDelimiters(out beginTokenDelimiter, out endTokenDelimiter, out logEntryContainsTokens, false);

            if (logEntryContainsTokens) {
                var dialog = new RedetermineTokens(_log, toAdd);
                if (dialog.ShowDialog() == DialogResult.Cancel) return;
            }

            _log.AddRangeWithoutInvokingEvent(toAdd);
            toAdd = null;

            RemoveEmptyUserActions();

            //#if EnableBetaFeature
            //            bool successfullyParallized = SetParallelExecutions();
            //#else
            //#warning Parallel executions temp not available
            bool successfullyParallized = true;
            //#endif
            //SetIgnoreDelays();
            // FillLargeList();

            if (!successfullyParallized) {
                string message = Text + ": Could not determine the begin- and end timestamps for one or more log entries in the different user actions, are they correctly formatted?";
                Loggers.Log(Level.Error, message, null, new object[] { text, clearLog });
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
        #endregion

        #region Extra Tools
        private void btnExportToTextFile_Click(object sender, EventArgs e) {
            saveFileDialog.FileName = _log.ToString().ReplaceInvalidWindowsFilenameChars('_').Replace(' ', '_');
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                var sb = new StringBuilder();
                if (_log.LogRuleSet.BeginCommentString.Length != 0 && _log.LogRuleSet.EndCommentString.Length != 0)
                    foreach (UserAction ua in _log) {
                        sb.Append(_log.LogRuleSet.BeginCommentString);
                        sb.Append(ua.Label);
                        sb.AppendLine(_log.LogRuleSet.EndCommentString);
                        foreach (LogEntry le in ua)
                            sb.AppendLine(le.LogEntryString);
                    } else
                    foreach (UserAction ua in _log) {
                        foreach (LogEntry le in ua)
                            sb.AppendLine(le.LogEntryString);
                    }

                using (var sw = new StreamWriter(saveFileDialog.FileName))
                    sw.Write(sb.ToString().TrimEnd());
            }
        }
        private void btnRevertToImported_Click(object sender, EventArgs e) {
            if (_log != null && MessageBox.Show("Are you sure you want to do this?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                foreach (UserAction userAction in _log) {
                    userAction.ClearWithoutInvokingEvent();

                    foreach (string s in userAction.LogEntryStringsAsImported)
                        userAction.AddWithoutInvokingEvent(new LogEntry(s));
                }
                _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

                _log.ApplyLogRuleSet();
                if (RevertedToAsImported != null)
                    RevertedToAsImported(this, null);
            }
        }
        private void btnRedetermineParameterTokens_Click(object sender, EventArgs e) {
            if ((new RedetermineTokens(_log)).ShowDialog() == DialogResult.OK && RedeterminedTokens != null)
                RedeterminedTokens(this, null);
        }
        #endregion

        #endregion
    }
}
