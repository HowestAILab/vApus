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
using System.Runtime.Serialization;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    [Serializable]
    [ContextMenu(new[] {"Activate_Click", "Remove_Click", "Export_Click", "Copy_Click", "Cut_Click", "Duplicate_Click"},
        new[] {"Edit/Import", "Remove", "Export Data Structure", "Copy", "Cut", "Duplicate"})]
    [Hotkeys(new[] {"Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click"},
        new[] {Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D)})]
    public class Log : LabeledBaseItem, ISerializable
    {
        #region Fields

        private static readonly object _lock = new object();

        private LexicalResult _lexicalResult;
        private LogRuleSet _logRuleSet;

        private Parameters _parameters;
        private int _preferredTokenDelimiterIndex;

        //Record settings
        private string[] _recordIps = new string[] {};
        private int[] _recordPorts = new[] {80};

        #endregion

        #region Properties

        [SavableCloneable, PropertyControl(1)]
        [DisplayName("Log Rule Set"),
         Description(
             "You must define a rule set to validate if the log file(s) are correctly formated to be able to stresstest."
             )]
        public LogRuleSet LogRuleSet
        {
            get
            {
                if (_logRuleSet.IsEmpty)
                    LogRuleSet =
                        GetNextOrEmptyChild(typeof (LogRuleSet),
                                            Solution.ActiveSolution.GetSolutionComponent(typeof (LogRuleSets))) as
                        LogRuleSet;

                return _logRuleSet;
            }
            set
            {
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
        public int PreferredTokenDelimiterIndex
        {
            get { return _preferredTokenDelimiterIndex; }
            set
            {
                if (value < 0)
                    value = LogEntry.MaxTokenDelimiterIndex;
                else if (value > LogEntry.MaxTokenDelimiterIndex)
                    value = 0;

                _preferredTokenDelimiterIndex = value;
            }
        }

        public LexicalResult LexicalResult
        {
            get { return _lexicalResult; }
        }

        [SavableCloneable]
        public string[] RecordIps
        {
            get { return _recordIps; }
            set { _recordIps = value; }
        }

        [SavableCloneable]
        public int[] RecordPorts
        {
            get { return _recordPorts; }
            set { _recordPorts = value; }
        }

        #endregion

        #region Constructors

        public Log()
        {
            if (Solution.ActiveSolution != null)
            {
                LogRuleSet =
                    GetNextOrEmptyChild(typeof (LogRuleSet),
                                        Solution.ActiveSolution.GetSolutionComponent(typeof (LogRuleSets))) as
                    LogRuleSet;
                _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof (Parameters)) as Parameters;
            }
            else
            {
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
            }
        }

        /// <summary>
        ///     Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public Log(SerializationInfo info, StreamingContext ctxt)
        {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info))
            {
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
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter())
            {
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

        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            LogRuleSet =
                GetNextOrEmptyChild(typeof (LogRuleSet),
                                    Solution.ActiveSolution.GetSolutionComponent(typeof (LogRuleSets))) as LogRuleSet;
            _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof (Parameters)) as Parameters;
        }

        private void _logRuleSet_ParentIsNull(object sender, EventArgs e)
        {
            if (_logRuleSet == sender)
                LogRuleSet =
                    GetNextOrEmptyChild(typeof (LogRuleSet),
                                        Solution.ActiveSolution.GetSolutionComponent(typeof (LogRuleSets))) as
                    LogRuleSet;
        }

        public override void Activate()
        {
            SolutionComponentViewManager.Show(this);
        }

        /// <summary>
        ///     This will apply the ruleset (lexing).
        ///     The lexed log entry will be filled in for the log entries.
        /// </summary>
        public void ApplyLogRuleSet()
        {
            _lexicalResult = LexicalResult.OK;
            foreach (LogEntry logEntry in GetAllLogEntries())
            {
                logEntry.ApplyLogRuleSet();
                if (logEntry.LexicalResult == LexicalResult.Error)
                    _lexicalResult = LexicalResult.Error;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="beginTokenDelimiter"></param>
        /// <param name="endTokenDelimiter"></param>
        /// <param name="warning">True if one of the delimiters is not contained in the log entry string as imported but is in the log entry string.</param>
        public void GetUniqueParameterTokenDelimiters(out string beginTokenDelimiter, out string endTokenDelimiter,
                                                      out bool warning, out bool error, bool autoNextOnError = true)
        {
            beginTokenDelimiter = string.Empty;
            endTokenDelimiter = string.Empty;
            warning = false;
            error = false;

            foreach (LogEntry logEntry in GetAllLogEntries())
            {
                string b, e;
                bool warn, err;

                int i = logEntry.GetUniqueParameterTokenDelimiters(autoNextOnError, out b, out e, out warn, out err,
                                                                   _preferredTokenDelimiterIndex);

                if (i >= _preferredTokenDelimiterIndex)
                {
                    beginTokenDelimiter = b;
                    endTokenDelimiter = e;
                    if (warn)
                        warning = warn;
                    if (err)
                        error = err;

                    _preferredTokenDelimiterIndex = i;
                }
            }
        }

        /// <summary>
        ///     Get a list of string trees, these are used in the connection proxy code.
        /// </summary>
        /// <returns></returns>
        public List<StringTree> GetParameterizedStructure()
        {
            var parameterizedStructure = new List<StringTree>(Count);
            var chosenNextValueParametersForLScope = new HashSet<BaseParameter>();

            string b, e;
            bool warning, error;
            GetUniqueParameterTokenDelimiters(out b, out e, out warning, out error);

            foreach (BaseItem item in this)
                if (item is UserAction)
                    foreach (
                        StringTree ps in
                            (item as UserAction).GetParameterizedStructure(b, e, chosenNextValueParametersForLScope))
                        parameterizedStructure.Add(ps);
                else
                    parameterizedStructure.Add((item as LogEntry).GetParameterizedStructure(b, e,
                                                                                            chosenNextValueParametersForLScope,
                                                                                            new HashSet<BaseParameter>()));

            return parameterizedStructure;
        }

        /// <summary>
        ///     Get the log entries even if they are in a user action.
        ///     Threadsafe.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LogEntry> GetAllLogEntries()
        {
            lock (_lock)
            {
                foreach (BaseItem item in this)
                {
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
        public void SynchronizeTokens(Dictionary<BaseParameter, KeyValuePair<int, int>> oldAndNewIndices,
                                      KeyValuePair<string, string> oldAndNewBeginTokenDelimiter,
                                      KeyValuePair<string, string> oldAndNewEndTokenDelimiter)
        {
            //Synchronize only if needed.
            if (oldAndNewIndices.Count == 0)
                return;

            var oldAndNewTokens = new Dictionary<string, string>();

            var scopeIdentifiers = new[]
                {
                    ASTNode.LOG_PARAMETER_SCOPE,
                    ASTNode.USER_ACTION_PARAMETER_SCOPE,
                    ASTNode.LOG_ENTRY_PARAMETER_SCOPE,
                    ASTNode.LEAF_NODE_PARAMETER_SCOPE,
                    ASTNode.ALWAYS_PARAMETER_SCOPE
                };

            foreach (string scopeIdentifier in scopeIdentifiers)
                foreach (BaseParameter parameter in oldAndNewIndices.Keys)
                {
                    KeyValuePair<int, int> kvp = oldAndNewIndices[parameter];
                    string oldToken = oldAndNewBeginTokenDelimiter.Key + scopeIdentifier + kvp.Key +
                                      oldAndNewEndTokenDelimiter.Key;
                    string newToken = oldAndNewBeginTokenDelimiter.Value + scopeIdentifier + kvp.Value +
                                      oldAndNewEndTokenDelimiter.Value;

                    oldAndNewTokens.Add(oldToken, newToken);
                }

            foreach (LogEntry entry in GetAllLogEntries())
            {
                foreach (string oldToken in oldAndNewTokens.Keys)
                {
                    entry.LogEntryString = entry.LogEntryString.Replace(oldToken, oldAndNewTokens[oldToken]);
                }
            }

            InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        /// <summary>
        ///     Used for redetermining tokens
        /// </summary>
        /// <param name="beginTokenDelimiter"></param>
        /// <param name="endTokenDelimiter"></param>
        /// <param name="replacement">Will be prefixed with begin and end</param>
        public string ReplaceTokenDelimitersInLogEntryStringAsImported(string beginTokenDelimiter,
                                                                       string endTokenDelimiter)
        {
            string replacement = null;

            Replace:
            replacement = StringUtil.GenerateRandomName(5);
            foreach (LogEntry entry in GetAllLogEntries())
                if (entry.LogEntryStringAsImported.Contains(replacement))
                    goto Replace;

            string begin = "begin" + replacement;
            string end = "end" + replacement;
            foreach (LogEntry entry in GetAllLogEntries())
                entry.LogEntryStringAsImported =
                    entry.LogEntryStringAsImported.Replace(beginTokenDelimiter, begin).Replace(endTokenDelimiter, end);

            InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);

            return replacement;
        }

        public Log Clone(bool cloneChildren = true)
        {
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

        #endregion
    }
}