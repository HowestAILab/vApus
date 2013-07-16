/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Runtime.Serialization;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    [Serializable]
    [ContextMenu(new[] { "Activate_Click", "Remove_Click", "Export_Click", "ExportLogAndUsedParameters_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { "Edit/Import", "Remove", "Export Data Structure", "Export log and Used Parameter Data Structures", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    public class Log : LabeledBaseItem, ISerializable {

        #region Events

        /// <summary>
        ///     This event is used in a control, this makes sure that trying to serialize the control where this event is used will not happen.
        /// </summary>
        [field: NonSerialized] //This makes sure that trying to serialize the control where this event is used will not happen.
        internal event EventHandler<LexicalResultsChangedEventArgs> LexicalResultChanged;

        #endregion

        #region Fields

        private static readonly object _lock = new object();

        private LexicalResult _lexicalResult;
        private LogRuleSet _logRuleSet;

        private Parameters _parameters;
        private int _preferredTokenDelimiterIndex;

        //Capture settings
        private string[] _allow = new string[] { };
        private string[] _deny = new string[] { };
        #endregion

        #region Properties

        [SavableCloneable, PropertyControl(1)]
        [DisplayName("Log Rule Set"),
         Description("You must define a rule set to validate if the log file(s) are correctly formated to be able to stresstest.")]
        public LogRuleSet LogRuleSet {
            get {
                if (_logRuleSet.IsEmpty)
                    LogRuleSet = GetNextOrEmptyChild(typeof(LogRuleSet), Solution.ActiveSolution.GetSolutionComponent(typeof(LogRuleSets))) as LogRuleSet;

                return _logRuleSet;
            }
            set {
                if (value == null)
                    return;
                value.ParentIsNull -= _logRuleSet_ParentIsNull;
                _logRuleSet = value;
                _logRuleSet.ParentIsNull += _logRuleSet_ParentIsNull;
            }
        }

        /// <summary>
        ///     Used for getting the right token delimiters, this value is updated if the tokens are manually redetermined.
        ///     Set: if it is outside boundaries this will be corrected by going to the last or first possible index.
        /// </summary>
        [SavableCloneable]
        public int PreferredTokenDelimiterIndex {
            get { return _preferredTokenDelimiterIndex; }
            set {
                if (value < 0)
                    value = LogEntry.MaxTokenDelimiterIndex;
                else if (value > LogEntry.MaxTokenDelimiterIndex)
                    value = 0;

                _preferredTokenDelimiterIndex = value;
            }
        }

        public LexicalResult LexicalResult {
            get { return _lexicalResult; }
        }

        [SavableCloneable]
        public bool UseAllow { get; set; }
        [SavableCloneable]
        public string[] Allow {
            get { return _allow; }
            set { _allow = value; }
        }
        [SavableCloneable]
        public bool AllowIncludeReferer { get; set; }
        [SavableCloneable]
        public bool UseDeny { get; set; }
        [SavableCloneable]
        public string[] Deny {
            get { return _deny; }
            set { _deny = value; }
        }

        #endregion

        #region Constructors

        public Log() {
            if (Solution.ActiveSolution == null) {
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
            } else {
                LogRuleSet = GetNextOrEmptyChild(typeof(LogRuleSet), Solution.ActiveSolution.GetSolutionComponent(typeof(LogRuleSets))) as LogRuleSet;
                _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
            }
        }

        /// <summary>
        ///     Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public Log(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                Label = sr.ReadString();
                _logRuleSet = sr.ReadObject() as LogRuleSet;
                _preferredTokenDelimiterIndex = sr.ReadInt32();
                _parameters = sr.ReadObject() as Parameters;

                AddRangeWithoutInvokingEvent(sr.ReadCollection<BaseItem>(new List<BaseItem>()), false);
            }
            sr = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(Label);
                sw.WriteObject(_logRuleSet);
                sw.Write(_preferredTokenDelimiterIndex);
                sw.WriteObject(_parameters);

                sw.Write(this);
                sw.AddToInfo(info);
            }
            sw = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }

        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            LogRuleSet = GetNextOrEmptyChild(typeof(LogRuleSet), Solution.ActiveSolution.GetSolutionComponent(typeof(LogRuleSets))) as LogRuleSet;
            _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
        }

        private void _logRuleSet_ParentIsNull(object sender, EventArgs e) {
            if (_logRuleSet == sender)
                LogRuleSet = GetNextOrEmptyChild(typeof(LogRuleSet), Solution.ActiveSolution.GetSolutionComponent(typeof(LogRuleSets))) as LogRuleSet;
        }

        public override void Activate() { SolutionComponentViewManager.Show(this); }

        /// <summary>
        ///     This will apply the ruleset (lexing).
        ///     The lexed log entry will be filled in for the log entries.
        /// </summary>
        public void ApplyLogRuleSet() {
            _lexicalResult = LexicalResult.OK;
            var logEntriesWithErrors = new List<LogEntry>();
            foreach (LogEntry logEntry in GetAllLogEntries()) {
                logEntry.ApplyLogRuleSet();
                if (logEntry.LexicalResult == LexicalResult.Error) {
                    _lexicalResult = LexicalResult.Error;
                    logEntriesWithErrors.Add(logEntry);
                }
            }

            if (LexicalResultChanged != null) LexicalResultChanged(this, new LexicalResultsChangedEventArgs(logEntriesWithErrors));
        }

        /// <summary>
        /// </summary>
        /// <param name="beginTokenDelimiter"></param>
        /// <param name="endTokenDelimiter"></param>
        /// <param name="logEntryContainsTokens">True if one of the delimiters is in the log entry string.</param>
        public void GetParameterTokenDelimiters(out string beginTokenDelimiter, out string endTokenDelimiter, out bool logEntryContainsTokens, bool autoNextOnLogEntryContainsTokens) {
            beginTokenDelimiter = string.Empty;
            endTokenDelimiter = string.Empty;
            logEntryContainsTokens = false;

            string b, e;
            bool bln;
            int tokenIndex = -1;

            if (this.CountOf(typeof(UserAction)) == 0) {
                tokenIndex = (new LogEntry()).GetParameterTokenDelimiters(autoNextOnLogEntryContainsTokens, out b, out e, out bln, _preferredTokenDelimiterIndex);
                if (bln) logEntryContainsTokens = true;

                beginTokenDelimiter = b;
                endTokenDelimiter = e;

                _preferredTokenDelimiterIndex = tokenIndex;
            } else {
                foreach (LogEntry logEntry in GetAllLogEntries()) {
                    tokenIndex = logEntry.GetParameterTokenDelimiters(autoNextOnLogEntryContainsTokens, out b, out e, out bln, _preferredTokenDelimiterIndex);

                    if (tokenIndex >= _preferredTokenDelimiterIndex) {
                        beginTokenDelimiter = b;
                        endTokenDelimiter = e;
                        if (bln) logEntryContainsTokens = true;

                        _preferredTokenDelimiterIndex = tokenIndex;
                    }
                }
            }
        }

        /// <summary>
        ///     Get a list of string trees, these are used in the connection proxy code.
        /// </summary>
        /// <returns></returns>
        public List<StringTree> GetParameterizedStructure() {
            var parameterizedStructure = new List<StringTree>(Count);
            var chosenNextValueParametersForLScope = new HashSet<BaseParameter>();

            string b, e;
            bool logEntryContainsTokens;
            GetParameterTokenDelimiters(out b, out e, out logEntryContainsTokens, false);

            foreach (UserAction userAction in this)
                foreach (StringTree ps in userAction.GetParameterizedStructure(b, e, chosenNextValueParametersForLScope)) parameterizedStructure.Add(ps);

            return parameterizedStructure;
        }

        /// <summary>
        ///     Get the log entries even if they are in a user action.
        ///     Threadsafe.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LogEntry> GetAllLogEntries() {
            lock (_lock) {
                foreach (BaseItem item in this) {
                    if (item is LogEntry)
                        yield return (item as LogEntry);
                    else
                        foreach (LogEntry childItem in item)
                            yield return childItem;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="oldAndNewIndices"></param>
        /// <param name="oldAndNewBeginTokenDelimiter"></param>
        /// <param name="oldAndNewEndTokenDelimiter"></param>
        public void SynchronizeTokens(Dictionary<BaseParameter, KeyValuePair<int, int>> oldAndNewIndices, KeyValuePair<string, string> oldAndNewBeginTokenDelimiter,
            KeyValuePair<string, string> oldAndNewEndTokenDelimiter) {
            //Synchronize only if needed.
            if (oldAndNewIndices.Count == 0)
                return;

            var oldAndNewTokens = new Dictionary<string, string>();

            var scopeIdentifiers = new[] { ASTNode.LOG_PARAMETER_SCOPE, ASTNode.USER_ACTION_PARAMETER_SCOPE, 
                ASTNode.LOG_ENTRY_PARAMETER_SCOPE, ASTNode.LEAF_NODE_PARAMETER_SCOPE, ASTNode.ALWAYS_PARAMETER_SCOPE };

            foreach (string scopeIdentifier in scopeIdentifiers)
                foreach (BaseParameter parameter in oldAndNewIndices.Keys) {
                    KeyValuePair<int, int> kvp = oldAndNewIndices[parameter];
                    string oldToken = oldAndNewBeginTokenDelimiter.Key + scopeIdentifier + kvp.Key + oldAndNewEndTokenDelimiter.Key;
                    string newToken = oldAndNewBeginTokenDelimiter.Value + scopeIdentifier + kvp.Value + oldAndNewEndTokenDelimiter.Value;

                    oldAndNewTokens.Add(oldToken, newToken);
                }

            foreach (LogEntry entry in GetAllLogEntries())
                foreach (string oldToken in oldAndNewTokens.Keys)
                    entry.LogEntryString = entry.LogEntryString.Replace(oldToken, oldAndNewTokens[oldToken]);

            InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        public Log Clone(bool cloneChildren = true) {
            var log = new Log();
            log.Parent = Parent;
            log.Label = Label;
            log.LogRuleSet = _logRuleSet;
            log._lexicalResult = _lexicalResult;
            log.PreferredTokenDelimiterIndex = _preferredTokenDelimiterIndex;
            log._parameters = _parameters;

            if (cloneChildren)
                foreach (BaseItem item in this)
                    if (item is UserAction)
                        log.AddWithoutInvokingEvent((item as UserAction).Clone(), false);
                    else
                        log.AddWithoutInvokingEvent((item as LogEntry).Clone(), false);

            return log;
        }

        private void ExportLogAndUsedParameters_Click(object sender, EventArgs e) {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Zip Files (*.zip) | *.zip";
            sfd.Title = "Export log and used parameters to...";
            sfd.FileName = Label.ReplaceInvalidWindowsFilenameChars('_');
            if (sfd.ShowDialog() == DialogResult.OK) {
                Package package = null;

                try {
                    package = Package.Open(sfd.FileName, FileMode.Create, FileAccess.ReadWrite);

                    var uri = new Uri("/" + Name, UriKind.Relative);
                    var part = package.CreatePart(uri, string.Empty, CompressionOption.Maximum);
                    using (var sw = new StreamWriter(part.GetStream(FileMode.Create, FileAccess.Write)))
                        GetXmlStructure().Save(sw);

                    //Get the parameters used in the log
                    string begin, end;
                    bool logEntryContainsTokens;
                    GetParameterTokenDelimiters(out begin, out end, out logEntryContainsTokens, false);

                    var usedParameters = new List<BaseParameter>();
                    var allParameterTokens = ASTNode.GetParameterTokens(begin, end, _parameters);

                    foreach (UserAction userAction in this)
                        foreach (LogEntry logEntry in userAction)
                            foreach (string token in allParameterTokens.Keys) {
                                var parameter = allParameterTokens[token];
                                if (!usedParameters.Contains(parameter) && logEntry.LogEntryString.Contains(token))
                                    usedParameters.Add(allParameterTokens[token]);
                            }

                    //Save thenm to the package.
                    foreach (var parameter in usedParameters) {
                        uri = new Uri("/" + parameter.Name.Replace(' ', '_') + "_0" + parameter.Index, UriKind.Relative);
                        part = package.CreatePart(uri, string.Empty, CompressionOption.Maximum);
                        using (var sw = new StreamWriter(part.GetStream(FileMode.Create, FileAccess.Write)))
                            parameter.GetXmlStructure().Save(sw);
                    }

                    package.Flush();
                } catch (Exception ex) {
                    LogWrapper.LogByLevel("Failed to export the log + parameters.\n" + ex.ToString(), LogLevel.Error);
                }

                try {
                    if (package != null)
                        package.Close();
                } catch { }
            }
        }

        #endregion

        public class LexicalResultsChangedEventArgs : EventArgs {
            public List<LogEntry> LogEntriesWithErrors { get; private set; }
            public LexicalResultsChangedEventArgs(List<LogEntry> logEntriesWithErrors) {
                LogEntriesWithErrors = logEntriesWithErrors;
            }
        }
    }
}