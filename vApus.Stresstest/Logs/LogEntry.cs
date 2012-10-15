/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using vApus.SolutionTree;
using vApus.Util;
using System.Collections.Generic;

namespace vApus.Stresstest
{
    [DisplayName("Log Entry"), Serializable]
    public class LogEntry : LabeledBaseItem
    {
        #region Events
        /// <summary>
        /// This event is used in a control, this makes sure that trying to serialize the control where this event is used will not happen.
        /// </summary>
        [field: NonSerialized]
        internal event EventHandler LexicalResultChanged;
        #endregion

        #region Fields
        private int _occurance = 1;
        private string _logEntryString = string.Empty;
        private string _logEntryStringAsImported = string.Empty;
        private bool _pinned;
        private bool _ignoreDelay;
        private bool _executeInParallelWithPrevious;
        private int _parallelOffsetInMs;
        private ASTNode _lexedLogEntry;
        private LexicalResult _lexicalResult = LexicalResult.Error;

        private Parameters _parameters;

        private static char[] _beginParameterTokenDelimiterCanditates = new char[] { '{', '<', '[', '(', '\\', '#', '$', '£', '€', '§', '%', '*', '²', '³', '°' },
                        _endParameterTokenDelimiterCanditates = new char[] { '}', '>', ']', ')', '/', '#', '$', '£', '€', '§', '%', '*', '²', '³', '°' };
        #endregion

        #region Properties
        /// <summary>
        /// Call ApplyLogRuleSet after setting this.
        /// </summary>
        [ReadOnly(true)]
        [SavableCloneable]
        [DisplayName("Log Entry String")]
        public string LogEntryString
        {
            get { return _logEntryString; }
            set { _logEntryString = value; }
        }
        [ReadOnly(true)]
        [SavableCloneable]
        [DisplayName("Log Entry String as Imported")]
        public string LogEntryStringAsImported
        {
            get { return _logEntryStringAsImported; }
            set { _logEntryStringAsImported = value; }
        }
        /// <summary>
        /// Is valid after calling ApplyLogRuleSet.
        /// </summary>

        [Description("Specifies if the entry is valid or not."), DisplayName("Lexical Result")]
        public LexicalResult LexicalResult
        {
            get { return _lexicalResult; }
        }
        [ReadOnly(true)]
        [SavableCloneable]
        [Description("How many times this entry occures in the log or parent user action. Action and Log Entry Distribution in the stresstest determines how this value will be used.")]
        public int Occurance
        {
            get { return _occurance; }
            set
            {
                if (_occurance < 0)
                    throw new ArgumentOutOfRangeException("occurance");
                _occurance = value;
            }
        }
        /// <summary>
        /// Is valid after calling ApplyLogRuleSet.
        /// </summary>

        public ASTNode LexedLogEntry
        {
            get { return _lexedLogEntry; }
        }

        public LogRuleSet LogRuleSet
        {
            get
            {
                SolutionComponent parent = Parent;
                if (parent == null)
                    return null;
                while (!(parent is Log))
                {
                    if (parent == null)
                        return null;

                    parent = (parent as BaseItem).Parent;
                }
                return (parent as Log).LogRuleSet;
            }
        }
        [ReadOnly(true)]
        [SavableCloneable]
        [Description("To pin this log entry in place.")]
        public bool Pinned
        {
            get { return _pinned; }
            set { _pinned = value; }
        }
        [ReadOnly(true)]
        [SavableCloneable]
        [Description("When true the determined delay (stresstest properties) will not take place after this log entry."), DisplayName("Ignore Delay")]
        public bool IgnoreDelay
        {
            get { return _ignoreDelay; }
            set { _ignoreDelay = value; }
        }
        /// <summary>
        /// Only possible if in a user action.
        /// </summary>
        [ReadOnly(true)]
        [SavableCloneable]
        [Description("You can parallel execute this with an immediate previous or next log entry where this is enabled too. For the first one in a group the connection proxy for the executing thread (user) is used, therefore only that one is able to make data available for the rest of a stresstest for a certain user (eg login data)."), DisplayName("Execute in Parallel with Previous")]
        public bool ExecuteInParallelWithPrevious
        {
            get { return _executeInParallelWithPrevious; }
            set { _executeInParallelWithPrevious = value; }
        }
        [ReadOnly(true)]
        [SavableCloneable]
        [Description("The offset in ms before this 'parallel log entry' is executed (this simulates what browsers do)."), DisplayName("Parallel Offset")]
        public int ParallelOffsetInMs
        {
            get { return _parallelOffsetInMs; }
            set { _parallelOffsetInMs = value; }
        }
        /// <summary>
        /// The maximum index of the token delimiters that can be chosen.
        /// </summary>
        public static int MaxTokenDelimiterIndex
        {
            get
            {
                return (_beginParameterTokenDelimiterCanditates.Length * 3) - 1;
            }
        }
        #endregion

        #region Constructors
        public LogEntry()
            : base()
        {
            ShowInGui = false;
            if (_parameters == null && Solution.ActiveSolution != null)
                try
                {
                    _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
                }
                catch { }

            Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logEntryString">Log entry string as imported will get this value also.</param>
        public LogEntry(string logEntryString)
            : this()
        {
            LogEntryString = logEntryString;
            LogEntryStringAsImported = logEntryString;
        }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
        }
        /// <summary>
        /// This will apply the ruleset (lexing).
        /// The lexed log entry will be filled in.
        /// </summary>
        public void ApplyLogRuleSet()
        {
            //For cleaning old solutions
            this.ClearWithoutInvokingEvent();

            if (LogRuleSet == null)
            {
                _lexicalResult = LexicalResult.Error;
            }
            else
            {
                _lexicalResult = LogRuleSet.TryLexicalAnalysis(_logEntryString, _parameters, out _lexedLogEntry);
                _logEntryString = _lexedLogEntry.CombineValues();
            }

            if (LexicalResultChanged != null)
                LexicalResultChanged.Invoke(this, null);
        }

        /// <summary>
        /// (Re)determines and gets a unique begin and end parameter token delimiter for the combined value (CombineValues()) of this node.
        /// </summary>
        /// <param name="beginTokenDelimiter"></param>
        /// <param name="endTokenDelimiter"></param>
        /// <param name="warning">if not unique in the editable log entry</param>
        /// <param name="error">if not unique in the imported log entry</param>
        /// <param name="offset">The offset</param>
        /// <returns>A unique index of the chosen strings to cross check with other ast nodes.
        /// If is is greater than for a previous node they are also unique for that node, if it is smalller they are not unique for the previous node</returns>
        internal int GetUniqueParameterTokenDelimiters(bool autoNextOnError, out string beginTokenDelimiter, out string endTokenDelimiter, out bool warning, out bool error, int offset = 0)
        {
            beginTokenDelimiter = string.Empty;
            endTokenDelimiter = string.Empty;
            warning = false;
            error = false;

            int[] parameterTokenDelimiterIndices = new int[] { 0, -1, -1 };

            int uniqueCombinedIndex = 0; ;
            for (int i = 0; i < parameterTokenDelimiterIndices.Length; i++)
                for (int j = 0; j < _beginParameterTokenDelimiterCanditates.Length; j++)
                {
                    if (j > parameterTokenDelimiterIndices[i])
                        parameterTokenDelimiterIndices[i] = j;

                    if (uniqueCombinedIndex >= offset)
                    {
                        BuildParameterTokenDelimiter(parameterTokenDelimiterIndices, out beginTokenDelimiter, out endTokenDelimiter);

                        error = (_logEntryStringAsImported.Contains(beginTokenDelimiter) || _logEntryStringAsImported.Contains(endTokenDelimiter));
                        warning = (_logEntryString.Contains(beginTokenDelimiter) || _logEntryString.Contains(endTokenDelimiter));
                        if (!(error && autoNextOnError))
                            return uniqueCombinedIndex;
                    }
                    ++uniqueCombinedIndex;
                }

            throw new Exception("No unique delimiters could be chosen!");
        }
        /// <summary>
        /// Just builds.
        /// </summary>
        /// <param name="beginDelimiter"></param>
        /// <param name="endDelimiter"></param>
        private void BuildParameterTokenDelimiter(int[] parameterTokenDelimiterIndices, out string beginDelimiter, out string endDelimiter)
        {
            beginDelimiter = string.Empty;
            endDelimiter = string.Empty;
            foreach (int i in parameterTokenDelimiterIndices)
            {
                if (i == -1)
                    break;

                beginDelimiter += _beginParameterTokenDelimiterCanditates[i];
                endDelimiter += _endParameterTokenDelimiterCanditates[i];
            }

            endDelimiter = endDelimiter.Reverse();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="beginTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="endTokenDelimiter">Needed to dermine parameter tokens</param>
        /// <param name="chosenNextValueParametersForLScope">Can be an empty hash set but may not be null, used to store all these values for the right scope.</param>
        /// <param name="chosenNextValueParametersForUAScope">Can be an empty hash set but may not be null, used to store all these values for the right scope. If the log entry is not in a user action this should be an empty hash set.</param>
        /// <returns></returns>
        internal StringTree GetParameterizedStructure(string beginTokenDelimiter, string endTokenDelimiter,
                                                  HashSet<BaseParameter> chosenNextValueParametersForLScope,
                                                  HashSet<BaseParameter> chosenNextValueParametersForUAScope)
        {
            if (chosenNextValueParametersForUAScope == null)
                chosenNextValueParametersForUAScope = new HashSet<BaseParameter>();
            HashSet<BaseParameter> chosenNextValueParametersForLEScope = new HashSet<BaseParameter>();

            return _lexedLogEntry.GetParameterizedStructure(beginTokenDelimiter, endTokenDelimiter,
                chosenNextValueParametersForLScope, chosenNextValueParametersForUAScope, chosenNextValueParametersForLEScope);
        }

        public LogEntry Clone(bool setParent = true)
        {
            LogEntry logEntry = new LogEntry();
            if (setParent)
                logEntry.Parent = Parent;
            logEntry.Occurance = _occurance;
            logEntry.LogEntryString = _logEntryString;
            logEntry.LogEntryStringAsImported = _logEntryStringAsImported;
            logEntry.Pinned = _pinned;
            logEntry.IgnoreDelay = _ignoreDelay;

            logEntry.ApplyLogRuleSet();

            return logEntry;
        }
        public override string ToString()
        {
            return (base.ToString() == null ? string.Empty : base.ToString() + ": ") + _logEntryString;
        }
        #endregion
    }
}
