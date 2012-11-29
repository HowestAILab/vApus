/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class BulkEditLog : Form
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Fields

        private const int MAXSHOWNLOGENTRIES = int.MaxValue;

        private readonly Parameters _parameters;
        private ParameterTokenTextStyle _asImportedParameterTokenTextStyle;

        private string _beginTokenDelimiter;
        private ParameterTokenTextStyle _editParameterTokenTextStyle;
        private string _endTokenDelimiter;
        private Log _log;

        private string[] _logEntryStrings;

        #endregion

        #region Properties

        public Log Log
        {
            get { return _log; }
        }

        #endregion

        #region Constructors

        public BulkEditLog()
        {
            InitializeComponent();
            cboParameterScope.SelectedIndex = 0;
        }

        public BulkEditLog(Log log)
        {
            InitializeComponent();

            _log = log.Clone();
            bool warning;
            bool error;
            _log.GetUniqueParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out warning,
                                                   out error);

            var logEntryStrings = new List<string>();
            foreach (LogEntry entry in _log.GetAllLogEntries())
            {
                string index = string.Empty;
                if (entry.Parent is Log)
                    index = "Log Entry " + entry.Index;
                else
                    index = (entry.Parent as UserAction) + " Log Entry " + entry.Index;

                logEntryStrings.Add(index + ": " + entry.LogEntryString);
            }

            _logEntryStrings = logEntryStrings.ToArray();

            _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof (Parameters)) as Parameters;

            cboParameterScope.SelectedIndex = 0;

            if (IsHandleCreated)
                SetTextAndValidation();
            else
                HandleCreated += BulkEditLog_HandleCreated;
        }

        #endregion

        #region Functions

        private void BulkEditLog_HandleCreated(object sender, EventArgs e)
        {
            SetCodeStyle();
            SetTextAndValidation();
        }

        private void SetCodeStyle()
        {
            BaseItem customListParameters = _parameters[0];
            BaseItem numericParameters = _parameters[1];
            BaseItem textParameters = _parameters[2];
            BaseItem customRandomParameters = _parameters[3];

            var scopeIdentifiers = new[]
                {
                    ASTNode.ALWAYS_PARAMETER_SCOPE,
                    ASTNode.LEAF_NODE_PARAMETER_SCOPE,
                    ASTNode.LOG_ENTRY_PARAMETER_SCOPE,
                    ASTNode.USER_ACTION_PARAMETER_SCOPE,
                    ASTNode.LOG_PARAMETER_SCOPE
                };


            int index;
            List<string> clp = new List<string>(),
                         np = new List<string>(),
                         tp = new List<string>(),
                         crp = new List<string>();
            foreach (string scopeIdentifier in scopeIdentifiers)
            {
                index = 1;
                for (int i = 0; i < customListParameters.Count; i++)
                {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    clp.Add(token);
                }
                for (int i = 0; i < numericParameters.Count; i++)
                {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    np.Add(token);
                }
                for (int i = 0; i < textParameters.Count; i++)
                {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    tp.Add(token);
                }
                for (int i = 0; i < customRandomParameters.Count; i++)
                {
                    string token = _beginTokenDelimiter + scopeIdentifier + (index++) + _endTokenDelimiter;
                    crp.Add(token);
                }
            }
            _editParameterTokenTextStyle = new ParameterTokenTextStyle(fastColoredTextBoxView,
                                                                       GetDelimiters(_log.LogRuleSet), clp, np, tp, crp,
                                                                       false);
            _asImportedParameterTokenTextStyle = new ParameterTokenTextStyle(fastColoredTextBoxApplyFilter,
                                                                             GetDelimiters(_log.LogRuleSet), clp, np, tp,
                                                                             crp, false);
        }

        private string[] GetDelimiters(LogRuleSet logRuleSet)
        {
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

        private IEnumerable<string> GetDelimiters(LogSyntaxItem logSyntaxItem)
        {
            if (logSyntaxItem.ChildDelimiter.Length != 0)
                yield return logSyntaxItem.ChildDelimiter;
            foreach (BaseItem item in logSyntaxItem)
                if (item is LogSyntaxItem)
                    foreach (string delimiter in GetDelimiters(item as LogSyntaxItem))
                        yield return delimiter;
        }

        private void cboParameterScope_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetParameterTokensFlps();
        }

        private void SetParameterTokensFlps()
        {
            string scopeIdentifier = null;

            flpNotUsedTokens.Controls.Clear();
            flpUsedTokens.Controls.Clear();

            switch (cboParameterScope.SelectedIndex)
            {
                case 1:
                    scopeIdentifier = ASTNode.LOG_PARAMETER_SCOPE;
                    break;
                case 2:
                    scopeIdentifier = ASTNode.USER_ACTION_PARAMETER_SCOPE;
                    break;
                case 3:
                    scopeIdentifier = ASTNode.LOG_ENTRY_PARAMETER_SCOPE;
                    break;
                case 4:
                    scopeIdentifier = ASTNode.LEAF_NODE_PARAMETER_SCOPE;
                    break;
                case 5:
                    scopeIdentifier = ASTNode.ALWAYS_PARAMETER_SCOPE;
                    break;
            }

            if (scopeIdentifier == null)
            {
                AddKvpsToFlps(ASTNode.LOG_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.USER_ACTION_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.LOG_ENTRY_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.LEAF_NODE_PARAMETER_SCOPE);
                AddKvpsToFlps(ASTNode.ALWAYS_PARAMETER_SCOPE);
            }
            else
            {
                AddKvpsToFlps(scopeIdentifier);
            }
        }

        private void AddKvpsToFlps(string scopeIdentifier)
        {
            BaseItem customListParameters = _parameters[0];
            BaseItem numericParameters = _parameters[1];
            BaseItem textParameters = _parameters[2];
            BaseItem customRandomParameters = _parameters[3];

            int j = 1;
            for (int i = 0; i < customListParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             customListParameters[i].ToString(), Color.LightPink);
            for (int i = 0; i < numericParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             numericParameters[i].ToString(), Color.LightGreen);
            for (int i = 0; i < textParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             textParameters[i].ToString(), Color.LightBlue);
            for (int i = 0; i < customRandomParameters.Count; i++)
                AddKvpToFlps(_beginTokenDelimiter + scopeIdentifier + (j++) + _endTokenDelimiter,
                             customRandomParameters[i].ToString(), Color.Yellow);
        }

        private void AddKvpToFlps(string key, string value, Color backColor)
        {
            var kvp = new KeyValuePairControl(key, value);
            kvp.BackColor = backColor;
            if (LogContains(kvp.Key))
            {
                flpUsedTokens.Controls.Add(kvp);
            }
            else
            {
                kvp.ForeColor = Color.DimGray;
                flpNotUsedTokens.Controls.Add(kvp);
            }
        }

        private bool LogContains(string s)
        {
            foreach (string logEntryString in _logEntryStrings)
                if (logEntryString.Contains(s))
                    return true;
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool close = _log.LexicalResult == LexicalResult.OK;

            if (!close &&
                MessageBox.Show("Are you sure you want to apply the made changes? There are errors!", string.Empty,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) ==
                DialogResult.Yes)
                close = true;

            if (close)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        #region Filter

        private void fastColoredTextBoxApplyFilter_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            SetTextAndValidation();
        }

        private void chkFilterWholeWords_CheckedChanged(object sender, EventArgs e)
        {
            SetTextAndValidation();
        }

        private void chkFilterMatchCase_CheckedChanged(object sender, EventArgs e)
        {
            SetTextAndValidation();
        }

        private void rdbANDWise_CheckedChanged(object sender, EventArgs e)
        {
            SetTextAndValidation();
        }

        private void SetTextAndValidation()
        {
            Cursor = Cursors.WaitCursor;

            string[] filter = fastColoredTextBoxApplyFilter.Text.Split(new[] {'\n'},
                                                                       StringSplitOptions.RemoveEmptyEntries);

            int i = 0;
            int count = 0;
            var sb = new StringBuilder();
            sb.AppendLine();

            if (filter.Length == 0)
            {
                foreach (string logEntryString in _logEntryStrings)
                {
                    if (i != MAXSHOWNLOGENTRIES)
                    {
                        sb.AppendLine(logEntryString);
                        ++i;
                    }
                    ++count;
                }

                rtxtDescription.Text = count <= MAXSHOWNLOGENTRIES
                                           ? "All " + count + " log entries..."
                                           : "The first " + i + " of " + count + " log entries...";
            }
            else
            {
                RegexOptions options = chkFilterMatchCase.Checked
                                           ? RegexOptions.Singleline
                                           : RegexOptions.Singleline | RegexOptions.IgnoreCase;

                if (chkFilterWholeWords.Checked)
                    for (int k = 0; k < filter.Length; k++)
                        filter[k] = "\\b" + Regex.Escape(filter[k]) + "\\b";
                else
                    for (int k = 0; k < filter.Length; k++)
                        filter[k] = Regex.Escape(filter[k]);

                int j = 0;
                foreach (string logEntryString in _logEntryStrings)
                {
                    if (RegexIsMatch(logEntryString, filter, rdbANDWise.Checked, options))
                    {
                        if (i++ != MAXSHOWNLOGENTRIES)
                            sb.AppendLine(logEntryString);
                        else
                            ++j;
                    }
                    ++count;
                }

                rtxtDescription.Text = count <= MAXSHOWNLOGENTRIES
                                           ? "FILTERED " + i + " of " + count + " log entries..."
                                           : "The first " + i + " of " + j + " FILTERED on a total of " + count +
                                             " log entries...";
            }
            fastColoredTextBoxView.Text = sb.ToString();

            SetValidation();

            Cursor = Cursors.Default;
        }

        private bool RegexIsMatch(string input, string[] patterns, bool andWise, RegexOptions options)
        {
            if (andWise)
            {
                for (int i = 0; i < patterns.Length; i++)
                    if (!Regex.IsMatch(input, patterns[i], options))
                        return false;
                return true;
            }
            else
            {
                for (int i = 0; i < patterns.Length; i++)
                    if (Regex.IsMatch(input, patterns[i], options))
                        return true;
                return false;
            }
        }

        private void chkFilterOnUsedTokens_CheckedChanged(object sender, EventArgs e)
        {
            string[] filter = fastColoredTextBoxApplyFilter.Text.Split(new[] {'\n'},
                                                                       StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = null;

            if (chkFilterOnUsedTokens.Checked)
            {
                sb = new StringBuilder(fastColoredTextBoxApplyFilter.Text);
                foreach (string token in GetParameterTokens())
                    if (!filter.Contains(token) && LogContains(token))
                        sb.AppendLine(token);
            }
            else
            {
                sb = new StringBuilder();
                foreach (string s in filter)
                {
                    bool isToken = false;
                    foreach (string token in GetParameterTokens())
                        if (s == token)
                        {
                            isToken = true;
                            break;
                        }

                    if (!isToken)
                        sb.AppendLine(s);
                }
            }
            fastColoredTextBoxApplyFilter.Text = sb.ToString().Trim();
        }

        private IEnumerable<string> GetParameterTokens()
        {
            var scopeIdentifiers = new[]
                {
                    ASTNode.LOG_PARAMETER_SCOPE,
                    ASTNode.USER_ACTION_PARAMETER_SCOPE,
                    ASTNode.LOG_ENTRY_PARAMETER_SCOPE,
                    ASTNode.LEAF_NODE_PARAMETER_SCOPE,
                    ASTNode.ALWAYS_PARAMETER_SCOPE
                };

            int i;
            foreach (string scopeIdentifier in scopeIdentifiers)
            {
                i = 1;
                foreach (BaseParameter parameter in _parameters.GetAllParameters())
                    yield return (_beginTokenDelimiter + scopeIdentifier + (i++) + _endTokenDelimiter);
            }
        }

        private void tcTools_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcTools.SelectedTab == tpApplyFilter)
                fastColoredTextBoxApplyFilter.Select();
        }

        #endregion

        #region Replace

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            btnReplaceWith.Enabled = txtFind.Text.Length != 0;
        }

        private void btnReplaceWith_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            txtFind.SelectAll();
            txtReplace.SelectAll();

            RegexOptions options = chkReplaceMatchCase.Checked
                                       ? RegexOptions.Singleline
                                       : RegexOptions.Singleline | RegexOptions.IgnoreCase;

            string oldText = Regex.Escape(txtFind.Text);
            if (chkReplaceWholeWords.Checked)
                oldText = "\\b" + oldText + "\\b";

            Log undoLog = null;
            int replacedCounter = 0;

            if (rdbReplaceInAllLogEntries.Checked)
            {
                foreach (LogEntry entry in _log.GetAllLogEntries())
                {
                    string replaced = Regex.Replace(entry.LogEntryString, oldText, txtReplace.Text, options);

                    if (entry.LogEntryString != replaced)
                    {
                        if (replacedCounter == 0)
                            undoLog = _log.Clone();

                        ++replacedCounter;
                        entry.LogEntryString = replaced;
                    }
                }
            }
            else
            {
                string[] filter = fastColoredTextBoxApplyFilter.Text.Split(new[] {'\n'},
                                                                           StringSplitOptions.RemoveEmptyEntries);

                if (chkFilterWholeWords.Checked)
                    for (int k = 0; k < filter.Length; k++)
                        filter[k] = "\\b" + Regex.Escape(filter[k]) + "\\b";
                else
                    for (int k = 0; k < filter.Length; k++)
                        filter[k] = Regex.Escape(filter[k]);

                foreach (LogEntry entry in _log.GetAllLogEntries())
                    if (RegexIsMatch(entry.LogEntryString, filter, rdbANDWise.Checked, options))
                    {
                        string replaced = Regex.Replace(entry.LogEntryString, oldText, txtReplace.Text, options);

                        if (entry.LogEntryString != replaced)
                        {
                            if (replacedCounter == 0)
                                undoLog = _log.Clone();

                            ++replacedCounter;
                            entry.LogEntryString = replaced;
                        }
                    }
            }

            if (replacedCounter != 0)
            {
                _log.ApplyLogRuleSet();
                bool warning, error;
                _log.GetUniqueParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out warning,
                                                       out error);

                var logEntryStrings = new List<string>();
                foreach (LogEntry entry in _log.GetAllLogEntries())
                {
                    string index = string.Empty;
                    if (entry.Parent is Log)
                        index = "Log Entry " + entry.Index;
                    else
                        index = (entry.Parent as UserAction) + " Log Entry " + entry.Index;

                    logEntryStrings.Add(index + ": " + entry.LogEntryString);
                }

                _logEntryStrings = logEntryStrings.ToArray();

                SetTextAndValidation();

                SetParameterTokensFlps();

                btnUndoRedo.Text = "Undo";
                btnUndoRedo.Tag = undoLog;
                btnUndoRedo.Enabled = true;
                btnOK.Enabled = true;
            }

            MessageBox.Show(
                "Replaced '" + txtFind.Text + "' with '" + txtReplace.Text + "' in " + replacedCounter + " log entries.",
                string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);

            Cursor = Cursors.Default;
        }

        private void btnSwitchValues_Click(object sender, EventArgs e)
        {
            if (txtReplace.Text == string.Empty)
                return;
            string find = txtFind.Text;
            txtFind.Text = txtReplace.Text;
            txtReplace.Text = find;
        }

        private void btnUndoRedo_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            Log undoRedoLog = _log.Clone();

            _log = btnUndoRedo.Tag as Log;

            _log.ApplyLogRuleSet();
            bool warning, error;
            _log.GetUniqueParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out warning,
                                                   out error);

            var logEntryStrings = new List<string>();
            foreach (LogEntry entry in _log.GetAllLogEntries())
            {
                string index = string.Empty;
                if (entry.Parent is Log)
                    index = "Log Entry " + entry.Index;
                else
                    index = (entry.Parent as UserAction) + " Log Entry " + entry.Index;

                logEntryStrings.Add(index + ": " + entry.LogEntryString);
            }

            _logEntryStrings = logEntryStrings.ToArray();

            SetTextAndValidation();

            SetParameterTokensFlps();


            btnUndoRedo.Tag = undoRedoLog;
            btnUndoRedo.Text = btnUndoRedo.Text == "Undo" ? "Redo" : "Undo";

            Cursor = Cursors.Default;
        }

        #endregion

        #region Errors

        private void SetValidation()
        {
            if (_log.LexicalResult == LexicalResult.OK)
            {
                if (tcTools.TabPages.Contains(tpErrors))
                    tcTools.TabPages.Remove(tpErrors);
            }
            else
            {
                tvw.Nodes.Clear();

                if (!tcTools.Contains(tpErrors))
                    tcTools.TabPages.Add(tpErrors);

                int index = 0;
                foreach (LogEntry entry in _log.GetAllLogEntries())
                {
                    if (entry.LexicalResult == LexicalResult.Error)
                        AddErrorNode(index, entry);
                    ++index;
                }

                tcTools.SelectedTab = tpErrors;
            }
        }

        private void AddErrorNode(int line, LogEntry entry)
        {
            var node = new TreeNode(line + ") " + entry.LogEntryString);
            node.Tag = entry;

            string index = string.Empty;
            if (entry.Parent is Log)
                index = "Log Entry " + entry.Index;
            else
                index = (entry.Parent as UserAction) + " Log Entry " + entry.Index;

            var indexNode = new TreeNode(index);
            indexNode.Tag = entry;
            node.Nodes.Add(indexNode);

            var errorNode = new TreeNode(entry.LexedLogEntry.Error);
            errorNode.Tag = entry;
            errorNode.ForeColor = Color.DarkRed;
            node.Nodes.Add(errorNode);

            node.Expand();
            tvw.Nodes.Add(node);

            if (tvw.SelectedNode == null)
                tvw.SelectedNode = node;
        }

        private void btnEditSelectedLogEntry_Click(object sender, EventArgs e)
        {
            TreeNode node = tvw.SelectedNode;
            var logEntry = node.Tag as LogEntry;

            var addEditLogEntry = new AddEditLogEntry(logEntry);
            if (addEditLogEntry.ShowDialog() == DialogResult.OK)
            {
                logEntry.LogEntryString = addEditLogEntry.LogEntry.LogEntryString;

                _log.ApplyLogRuleSet();
                bool warning, error;
                _log.GetUniqueParameterTokenDelimiters(out _beginTokenDelimiter, out _endTokenDelimiter, out warning,
                                                       out error);

                var logEntryStrings = new List<string>();
                foreach (LogEntry entry in _log.GetAllLogEntries())
                {
                    string index = string.Empty;
                    if (entry.Parent is Log)
                        index = "Log Entry " + entry.Index;
                    else
                        index = (entry.Parent as UserAction) + " Log Entry " + entry.Index;

                    logEntryStrings.Add(index + ": " + entry.LogEntryString);
                }

                _logEntryStrings = logEntryStrings.ToArray();

                SetTextAndValidation();

                SetParameterTokensFlps();
            }
        }

        #endregion

        #endregion
    }
}