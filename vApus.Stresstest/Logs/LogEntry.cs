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
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// Contains a captured request to a server app.
    /// </summary>
    [DisplayName("Log Entry"), Serializable]
    public class LogEntry : LabeledBaseItem, ISerializable {

        #region Fields
        private static readonly char[] _beginParameterTokenDelimiterCanditates = new[] { '{', '<', '[', '(', '\\', '#', '$', '£', '€', '§', '%', '*', '²', '³', '°' };
        private static readonly char[] _endParameterTokenDelimiterCanditates = new[] { '}', '>', ']', ')', '/', '#', '$', '£', '€', '§', '%', '*', '²', '³', '°' };

        private readonly object _lock = new object();

        private string _logEntryString = string.Empty;

        private bool _useDelay = false;

        private bool _executeInParallelWithPrevious; //For a special not yet used feature.
        private int _parallelOffsetInMs;

        private LogEntry _sameAs;

        private ASTNode _lexedLogEntry;
        private LexicalResult _lexicalResult = LexicalResult.Error;

        private static Parameters _parameters;
        #endregion

        #region Properties
        /// <summary>
        ///     Call ApplyLogRuleSet after setting this.
        /// </summary>
        [ReadOnly(true)]
        [SavableCloneable]
        [DisplayName("Log Entry String")]
        public string LogEntryString {
            get { return _logEntryString; }
            set { _logEntryString = value; }
        }

        /// <summary>
        ///     Is valid after calling ApplyLogRuleSet.
        /// </summary>
        [Description("Specifies if the entry is valid or not."), DisplayName("Lexical Result")]
        public LexicalResult LexicalResult {
            get { return _lexicalResult; }
        }

        /// <summary>
        ///     Is valid after calling ApplyLogRuleSet.
        /// </summary>
        internal ASTNode LexedLogEntry { get { return _lexedLogEntry; } }

        [ReadOnly(true)]
        [Description("When true the determined delay (stresstest properties) will take place after this log entry."), DisplayName("Ignore Delay")]
        public bool UseDelay {
            get { return _useDelay; }
            set { _useDelay = value; }
        }

        /// <summary>
        ///     Only possible if in a user action.
        /// </summary>
        [ReadOnly(true)]
        [SavableCloneable]
        [Description("You can parallel execute this with an immediate previous or next log entry where this is enabled too. For the first one in a group the connection proxy for the executing thread (user) is used, therefore only that one is able to make data available for the rest of a stresstest for a certain user (eg login data)."), DisplayName("Execute in Parallel with Previous")]
        public bool ExecuteInParallelWithPrevious {
            get { return _executeInParallelWithPrevious; }
            set { _executeInParallelWithPrevious = value; }
        }

        [ReadOnly(true)]
        [SavableCloneable]
        [Description("The offset in ms before this 'parallel log entry' is executed (this simulates what browsers do)."), DisplayName("Parallel Offset")]
        public int ParallelOffsetInMs {
            get { return _parallelOffsetInMs; }
            set { _parallelOffsetInMs = value; }
        }

        /// <summary>
        ///     The maximum index of the token delimiters that can be chosen.
        /// </summary>
        public static int MaxTokenDelimiterIndex {
            get { return (_beginParameterTokenDelimiterCanditates.Length * 3) - 1; }
        }

        /// <summary>
        /// Only happens in stresstest core when determining test patterns.
        /// </summary>
        public LogEntry SameAs {
            get { return _sameAs; }
            set { _sameAs = value; }
        }


        /// <summary>
        /// For a distributed test.
        /// </summary>
        internal static Parameters Parameters {
            set { _parameters = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Contains a captured request to a server app.
        /// </summary>
        public LogEntry() { ShowInGui = false; }
        /// <summary>
        /// Contains a captured request to a server app.
        /// </summary>
        /// <param name="logEntryString">Log entry string as imported will get this value also.</param>
        public LogEntry(string logEntryString)
            : this() {
            LogEntryString = logEntryString;
        }

        public LogEntry(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                _logEntryString = sr.ReadString();
                _useDelay = sr.ReadBoolean();
                _executeInParallelWithPrevious = sr.ReadBoolean();
                _parallelOffsetInMs = sr.ReadInt32();
            }
            sr = null;
        }
        #endregion

        #region Functions
        /// <summary>
        ///     This will apply the ruleset (lexing).
        ///     The lexed log entry will be filled in.
        /// </summary>
        public void ApplyLogRuleSet(LogRuleSet logRuleSet) {
            //For cleaning old solutions
            //ClearWithoutInvokingEvent();

            if (_lexedLogEntry != null) {
                _lexedLogEntry.Dispose();
                _lexedLogEntry = null;
            }

            _lexicalResult = (logRuleSet == null) ? LexicalResult.Error : logRuleSet.TryLexicalAnalysis(_logEntryString, _parameters, out _lexedLogEntry);
        }

        /// <summary>
        ///     (Re)determines and gets a unique begin and end parameter token delimiter for the combined value (CombineValues()) of this node.
        /// </summary>
        /// <param name="beginTokenDelimiter"></param>
        /// <param name="endTokenDelimiter"></param>
        /// <param name="logEntryStringContainsTokens">if not unique in the log entry</param>
        /// <param name="offset">The offset</param>
        /// <returns>
        ///     A unique index of the chosen strings to cross check with other ast nodes.
        ///     If is is greater than for a previous node they are also unique for that node, if it is smalller they are not unique for the previous node
        /// </returns>
        internal int GetParameterTokenDelimiters(bool autoNextOnLogEntryContainsTokens, out string beginTokenDelimiter, out string endTokenDelimiter, out bool logEntryStringContainsTokens, int offset = 0) {
            beginTokenDelimiter = string.Empty;
            endTokenDelimiter = string.Empty;
            logEntryStringContainsTokens = false;

            var parameterTokenDelimiterIndices = new[] { 0, -1, -1 };

            int uniqueCombinedIndex = 0;
            for (int i = 0; i < parameterTokenDelimiterIndices.Length; i++)
                for (int j = 0; j < _beginParameterTokenDelimiterCanditates.Length; j++) {
                    if (j > parameterTokenDelimiterIndices[i])
                        parameterTokenDelimiterIndices[i] = j;

                    if (uniqueCombinedIndex >= offset) {
                        BuildParameterTokenDelimiter(parameterTokenDelimiterIndices, out beginTokenDelimiter, out endTokenDelimiter);

                        logEntryStringContainsTokens = (_logEntryString.Contains(beginTokenDelimiter) || _logEntryString.Contains(endTokenDelimiter));
                        if (!(logEntryStringContainsTokens && autoNextOnLogEntryContainsTokens))
                            return uniqueCombinedIndex;
                    }
                    ++uniqueCombinedIndex;
                }

            throw new Exception("No unique delimiters could be chosen!");
        }

        /// <summary>
        ///     Just builds.
        /// </summary>
        /// <param name="beginDelimiter"></param>
        /// <param name="endDelimiter"></param>
        private void BuildParameterTokenDelimiter(int[] parameterTokenDelimiterIndices, out string beginDelimiter, out string endDelimiter) {
            beginDelimiter = string.Empty;
            endDelimiter = string.Empty;
            foreach (int i in parameterTokenDelimiterIndices) {
                if (i == -1)
                    break;

                beginDelimiter += _beginParameterTokenDelimiterCanditates[i];
                endDelimiter += _endParameterTokenDelimiterCanditates[i];
            }

            endDelimiter = endDelimiter.Reverse();
        }

        /// <summary>
        /// Apply the log rule set before doing this.
        /// </summary>
        /// <param name="beginTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="endTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="chosenNextValueParametersForLScope">Can be an empty hash set but may not be null, used to store all these values for the right scope.</param>
        /// <param name="chosenNextValueParametersForUAScope">Can be an empty hash set but may not be null, used to store all these values for the right scope. If the log entry is not in a user action this should be an empty hash set.</param>
        /// <returns></returns>
        internal StringTree GetParameterizedStructure(Dictionary<string, BaseParameter> parameterTokens, HashSet<BaseParameter> chosenNextValueParametersForLScope, HashSet<BaseParameter> chosenNextValueParametersForUAScope) {

            HashSet<BaseParameter> chosenNextValueParametersForLEScope = parameterTokens == null ? null : new HashSet<BaseParameter>();

            return _lexedLogEntry.GetParameterizedStructure(parameterTokens, chosenNextValueParametersForLScope, chosenNextValueParametersForUAScope, chosenNextValueParametersForLEScope);
        }

        /// <summary>
        /// Clones and applies the log rule set.
        /// </summary>
        /// <param name="logRuleSet"></param>
        /// <param name="applyRuleSet">Not needed in a distributed test.</param>
        /// <param name="cloneLabelAndLogEntryStringByRef">Set to true to leverage memory usage, should only be used in a distributed test otherwise strange things will happen.</param>
        /// <returns></returns>
        public LogEntry Clone(LogRuleSet logRuleSet, bool applyRuleSet, bool cloneLogEntryStringByRef) {
            LogEntry logEntry = new LogEntry();
            logEntry.SetParent(Parent);

            if (cloneLogEntryStringByRef)
                SetLogEntryStringByRef(logEntry, ref _logEntryString);
            else
                logEntry._logEntryString = _logEntryString;

            if (applyRuleSet)
                logEntry.ApplyLogRuleSet(logRuleSet);

            return logEntry;
        }
        private void SetLogEntryStringByRef(LogEntry logEntry, ref string logEntryString) {
            logEntry._logEntryString = logEntryString;
        }

        /// <summary>
        /// Log Entry #: LogEntryString.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return (base.ToString() == null ? string.Empty : base.ToString() + ": ") + _logEntryString;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(Label);
                sw.Write(_logEntryString);
                sw.Write(_useDelay);
                sw.Write(_executeInParallelWithPrevious);
                sw.Write(_parallelOffsetInMs);
                sw.AddToInfo(info);
            }
            sw = null;
        }


        public new void Dispose() {
            _parameters = null;
            if (_lexedLogEntry != null) {
                _lexedLogEntry.Dispose();
                _lexedLogEntry = null;
            }
            base.Dispose();
        }
        #endregion
    }
}
