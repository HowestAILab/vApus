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
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// Is mainly a configuration file that bring all the pieces of a stresstest together split into basic and advanced properties
    /// </summary>
    [Serializable]
    [ContextMenu(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { "Edit", "Remove", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    public class Stresstest : LabeledBaseItem, ISerializable {

        #region Fields
        private int _runs = 1, _minimumDelay = 900, _maximumDelay = 1100, _monitorBefore, _monitorAfter;
        private int[] _concurrencies = { 5, 5, 10, 25, 50, 100 };
        private bool _shuffle = true;
        private bool _actionDistribution;
        private int _maximumNumberOfUserActions;
        private Connection _connection;

        [NonSerialized]
        private Logs _allLogs;
        [NonSerialized]
        private int[] _logIndices = { };
        [NonSerialized]
        private uint[] _logWeights = { };

        private KeyValuePair<Log, uint>[] _logs = { };

        //This will be saved, I don't want to extend the save logic so I hack around it.
        [NonSerialized]
        private MonitorProject _monitorProject;
        [NonSerialized]
        private int[] _monitorIndices = { };
        [NonSerialized]
        private Monitor.Monitor[] _monitors = { };

        //private bool _useParallelExecutionOfLogEntries;

        //For in the results database
        private string _description = string.Empty;
        private string[] _tags = new string[0];

        /// <summary>
        ///     Let for instance the gui behave differently if this is true.
        /// </summary>
        private bool _forDistributedTest;

        private bool _useParallelExecutionOfLogEntries;

        private Parameters _parameters; //Kept here for a distributed test
        #endregion

        #region Properties
        [Description("The connection to the application to test.")]
        [SavableCloneable, PropertyControl(0)]
        public Connection Connection {
            get {
                if (Solution.ActiveSolution != null && (_connection.IsEmpty || _connection.Parent == null))
                    _connection = GetNextOrEmptyChild(typeof(Connection), SolutionTree.Solution.ActiveSolution.GetSolutionComponent(typeof(Connections))) as Connection;

                if (_connection != null)
                    _connection.SetDescription("The connection to the application to test. [" + ConnectionProxy + "]");

                return _connection;
            }
            set {
                if (value == null)
                    return;
                _connection = value;
            }
        }

        [ReadOnly(true)]
        [DisplayName("Connection Proxy")]
        public string ConnectionProxy {
            get {
                if (_connection == null || _connection.IsEmpty || _connection.ConnectionProxy.IsEmpty)
                    return "Connection Proxy: <none>";
                return _connection.ConnectionProxy.ToString();
            }
        }

        [SavableCloneable]
        public uint[] LogWeights {
            get { return _logWeights; }
            set {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");
                _logWeights = value;
            }
        }
        [SavableCloneable]
        public int[] LogIndices {
            get { return _logIndices; }
            set {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");
                _logIndices = value;

                if (_allLogs != null) {
                    if (_logIndices.Length == 0 && _allLogs.Count > 1) {
                        _logWeights = new uint[] { 1 };
                        _logIndices = new int[] { 1 };
                    }
                    var l = new List<KeyValuePair<Log, uint>>(_logIndices.Length);
                    int weightIndex = 0;
                    foreach (int index in _logIndices) {
                        if (index < _allLogs.Count) {
                            var log = _allLogs[index] as Log;

                            bool added = false;
                            foreach (var addedKvp in l)
                                if (addedKvp.Key == log) {
                                    added = true;
                                    break;
                                }
                            if (!added) {
                                uint weight = weightIndex < _logWeights.Length ? _logWeights[weightIndex] : 0;
                                l.Add(new KeyValuePair<Log, uint>(log, weight));
                            }
                        }
                        ++weightIndex;
                    }
                    _logs = l.ToArray();
                    _logs.SetParent(_allLogs);
                }
            }
        }
        [Description("The logs used to test the application. Maximum 5 allowed and they must have the same log rule set. Change the weights to define the percentage distribution of users using a certain log.")]
        [PropertyControl(1)]
        public KeyValuePair<Log, uint>[] Logs {
            get { return _logs; }
            set {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");
                if (value.Length > 5)
                    throw new ArgumentOutOfRangeException("Maximum 5 allowed.");

                if (_allLogs != null && _allLogs.Count > 1 && value.Length == 0) {
                    _logWeights = new uint[] { 1 };
                    LogIndices = new int[] { 1 };
                    return;
                }

                if (value.Length != 0) {
                    var logRuleSet = value[0].Key.LogRuleSet;
                    for (int i = 1; i < value.Length; i++)
                        if (value[i].Key.LogRuleSet != logRuleSet)
                            throw new Exception("Only logs having the same log rule set are allowed.");

                    //New entries should have a weight of 1.
                    for (int i = _logs.Length; i < value.Length; i++)
                        value[i] = new KeyValuePair<vApus.Stresstest.Log, uint>(value[i].Key, 1);

                    //1 should not be 0 :).
                    bool allZeros = true;
                    for (int i = 0; i != value.Length; i++)
                        if (value[i].Value != 0) {
                            allZeros = false;
                            break;
                        }
                    if (allZeros) value[0] = new KeyValuePair<vApus.Stresstest.Log, uint>(value[0].Key, 1);
                }

                _logs = value;

                if (_allLogs != null) {
                    _logs.SetParent(_allLogs);

                    var logIndices = new List<int>(_logs.Length);
                    var logWeights = new List<uint>(_logs.Length);
                    for (int allLogsIndex = 1; allLogsIndex < _allLogs.Count; allLogsIndex++) {
                        KeyValuePair<Log, uint> kvp = new KeyValuePair<Log, uint>();
                        for (int logIndex = 0; logIndex != _logs.Length; logIndex++)
                            if (_logs[logIndex].Key == _allLogs[allLogsIndex]) {
                                kvp = _logs[logIndex];
                                break;
                            }

                        if (kvp.Key != null) {
                            logIndices.Add(allLogsIndex);
                            logWeights.Add(kvp.Value);
                        }
                    }

                    _logIndices = logIndices.ToArray();
                    _logWeights = logWeights.ToArray();
                }
            }
        }

        /// <summary>
        /// Does not validate and does not set the parent.
        /// </summary>
        public KeyValuePair<Log, uint>[] LogsOverride { set { _logs = value; } }

        [ReadOnly(true)]
        [DisplayName("Log Rule Set")]
        public string LogRuleSet {
            get {
                if (_logs.Length == 0)
                    return "Log Rule Set: <none>";
                var log = _logs[0].Key;
                if (log == null || log.IsEmpty || log.LogRuleSet.IsEmpty)
                    return "Log Rule Set: <none>";
                return log.LogRuleSet.ToString();
            }
        }

        [SavableCloneable]
        public int[] MonitorIndices {
            get { return _monitorIndices; }
            set {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");

                _monitorIndices = value;
                if (_monitorProject != null) {
                    var l = new List<Monitor.Monitor>(_monitorIndices.Length);
                    foreach (int index in _monitorIndices) {
                        var monitor = _monitorProject[index] as Monitor.Monitor;
                        if (index < _monitorProject.Count && !l.Contains(monitor))
                            l.Add(monitor);
                    }
                    _monitors = l.ToArray();
                    _monitors.SetParent(_monitorProject);
                }
            }
        }

        [Description("The monitors used to link stresstest results to performance counters. Maximum 5 allowed.")]
        [PropertyControl(2)]
        public Monitor.Monitor[] Monitors {
            get { return _monitors; }
            set {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");
                if (value.Length > 5)
                    throw new ArgumentOutOfRangeException("Maximum 5 allowed.");

                _monitors = value;
                if (_monitorProject != null) {
                    _monitors.SetParent(_monitorProject);


                    var l = new List<int>(_monitors.Length);
                    for (int index = 0; index != _monitorProject.Count; index++)
                        if (_monitors.Contains(_monitorProject[index]) && !l.Contains(index))
                            l.Add(index);

                    _monitorIndices = l.ToArray();
                }
            }
        }

        [Description("The count(s) of the concurrent users generated, the minimum given value equals one.")]
        [SavableCloneable, PropertyControl(3)]
        public int[] Concurrencies {
            get { return _concurrencies; }
            set {
                if (value.Length == 0)
                    throw new ArgumentException();
                foreach (int i in value)
                    if (i < 1)
                        throw new ArgumentOutOfRangeException("A concurrency in cannot be smaller then one.");
                _concurrencies = value;
            }
        }

        [Description("A static multiplier of the runtime for each concurrency. Must be greater than zero.")]
        [SavableCloneable, PropertyControl(4, 1, int.MaxValue)]
        public int Runs {
            get { return _runs; }
            set {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than 1.");
                _runs = value;
            }
        }

        [Description("The minimum delay in milliseconds between the execution of log entries per user. Keep this and the maximum delay zero to have an ASAP test."), DisplayName("Minimum Delay")]
        [PropertyControl(5, true, 0)]
        public int MinimumDelay {
            get { return _minimumDelay; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                if (value > _maximumDelay)
                    _maximumDelay = value;
                _minimumDelay = value;
            }
        }

        /// <summary>
        ///     Only for saving and loading, should not be used.
        /// </summary>
        [SavableCloneable]
        public int MinimumDelayOverride {
            get { return _minimumDelay; }
            set { _minimumDelay = value; }
        }

        [Description("The maximum delay in milliseconds between the execution of log entries per user. Keep this and the minimum delay zero to have an ASAP test."), DisplayName("Maximum Delay")]
        [PropertyControl(6, true, 0)]
        public int MaximumDelay {
            get { return _maximumDelay; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                if (value < _minimumDelay)
                    _minimumDelay = value;
                _maximumDelay = value;
            }
        }

        /// <summary>
        ///     Only for saving and loading, should not be used.
        /// </summary>
        [SavableCloneable]
        public int MaximumDelayOverride {
            get { return _maximumDelay; }
            set { _maximumDelay = value; }
        }

        [Description("The user actions will be shuffled for each concurrent user when testing.")]
        [SavableCloneable, PropertyControl(7)]
        public bool Shuffle {
            get { return _shuffle; }
            set { _shuffle = value; }
        }

        [Description("When this is used, user actions are executed X times its occurance. You can use 'Shuffle' and 'Maximum Number of User Actions' in combination with this to define unique test patterns for each user."),
        DisplayName("Action Distribution")]
        [SavableCloneable, PropertyControl(8, true)]
        public bool ActionDistribution {
            get { return _actionDistribution; }
            set { _actionDistribution = value; }
        }

        [Description("The maximum number of user actions that a test pattern for a user can contain. Pinned and linked actions however are always picked. Set this to zero to not use this."),
        DisplayName("Maximum Number of User Actions")]
        [SavableCloneable, PropertyControl(9, true, 0)]
        public int MaximumNumberOfUserActions {
            get { return _maximumNumberOfUserActions; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                _maximumNumberOfUserActions = value;
            }
        }

        [Description("Start monitoring before the test starts, expressed in minutes with a max of 60."),
         DisplayName("Monitor Before")]
        [SavableCloneable, PropertyControl(10, true, 0)]
        public int MonitorBefore {
            get { return _monitorBefore; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                if (value > 60)
                    value = 60;
                _monitorBefore = value;
            }
        }

        [Description("Continue monitoring after the test is finished, expressed in minutes with a max of 60."),
         DisplayName("Monitor After")]
        [SavableCloneable, PropertyControl(11, true, 0)]
        public int MonitorAfter {
            get { return _monitorAfter; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                if (value > 60)
                    value = 60;
                _monitorAfter = value;
            }
        }

        [SavableCloneable]
        public string Description {
            get { return _description; }
            set { _description = value; }
        }

        [SavableCloneable]
        public string[] Tags {
            get { return _tags; }
            set { _tags = value; }
        }

#if EnableBetaFeature
        [Description("If this equals false then the parallel switch for log entries is ignored."),
         DisplayName("Use Parallel Execution of Log Entries")]
        [SavableCloneable, PropertyControl(int.MaxValue, true)]
        public bool UseParallelExecutionOfLogEntries {
            get { return _useParallelExecutionOfLogEntries; }
            set { _useParallelExecutionOfLogEntries = value; }
        }
#else
#warning Parallel executions temp not available
        public bool UseParallelExecutionOfLogEntries {
            get { return false; }
            set { //_useParallelExecutionOfLogEntries = false; 
            }
        }
#endif
        /// <summary>
        ///     Let for instance the gui behave differently if this is true.
        /// </summary>
        public bool ForDistributedTest {
            get { return _forDistributedTest; }
            set { _forDistributedTest = value; }
        }
        /// <summary>
        /// Is this a part of a stresstest divided over multiple slaves?
        /// </summary>
        public bool IsDividedStresstest { get; set; }
        #endregion

        #region Constructors
        public Stresstest() {
            if (SolutionTree.Solution.ActiveSolution != null)
                Init();
            else
                SolutionTree.Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
        }
        /// <summary>
        /// Only used for deserializing
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public Stresstest(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                _runs = sr.ReadInt32();
                _minimumDelay = sr.ReadInt32();
                _maximumDelay = sr.ReadInt32();
                _monitorBefore = sr.ReadInt32();
                _monitorAfter = sr.ReadInt32();
                _concurrencies = sr.ReadArray(typeof(int)) as int[];
                _shuffle = sr.ReadBoolean();
                _actionDistribution = sr.ReadBoolean();
                _maximumNumberOfUserActions = sr.ReadInt32();
                _connection = sr.ReadObject() as Connection;
                _logs = sr.ReadObject() as KeyValuePair<Log, uint>[];
                _forDistributedTest = sr.ReadBoolean();
                _useParallelExecutionOfLogEntries = sr.ReadBoolean();

                _parameters = sr.ReadObject() as Parameters;
                _parameters.ForceSettingChildsParent();
                
                Connection.Parameters = _parameters;
                Log.Parameters = _parameters;
            }
            sr = null;

            GC.WaitForPendingFinalizers();
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
        }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            SolutionTree.Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            Init();
        }
        private void Init() {
            _allLogs = SolutionTree.Solution.ActiveSolution.GetSolutionComponent(typeof(Logs)) as Logs;

            var logs = new List<KeyValuePair<Log, uint>>(_logIndices.Length);
            int weightIndex = 0;
            foreach (int index in _logIndices) {
                if (index < _allLogs.Count) {
                    var log = _allLogs[index] as Log;
                    uint weight = weightIndex < _logWeights.Length ? _logWeights[weightIndex] : 0;

                    logs.Add(new KeyValuePair<Log, uint>(log, weight));
                }
                ++weightIndex;
            }

            _logs = logs.ToArray();
            _logs.SetParent(_allLogs);

            if (_allLogs != null && _allLogs.Count > 1 && _logIndices.Length == 0) {
                _logWeights = new uint[] { 1 };
                LogIndices = new int[] { 1 };
            }

            Connection = GetNextOrEmptyChild(typeof(Connection), SolutionTree.Solution.ActiveSolution.GetSolutionComponent(typeof(Connections))) as Connection;
            _monitorProject = SolutionTree.Solution.ActiveSolution.GetSolutionComponent(typeof(MonitorProject)) as MonitorProject;

            var monitors = new List<Monitor.Monitor>(_monitorIndices.Length);
            foreach (int index in _monitorIndices)
                if (index < _monitorProject.Count)
                    monitors.Add(_monitorProject[index] as Monitor.Monitor);

            _monitors = monitors.ToArray();
            _monitors.SetParent(_monitorProject);

            SolutionComponentChanged += SolutionComponentChanged_SolutionComponentChanged;
        }

        private void SolutionComponentChanged_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            //Cleanup _monitors/_logs if _monitorProject Changed
            if (sender == _monitorProject || sender is Monitor.Monitor) {
                var l = new List<Monitor.Monitor>(_monitorProject.Count);
                foreach (Monitor.Monitor monitor in _monitors)
                    if (!l.Contains(monitor) && _monitorProject.Contains(monitor))
                        l.Add(monitor);

                Monitors = l.ToArray();
            }

            if (sender == _allLogs || sender is Log) {
                var l = new List<KeyValuePair<Log, uint>>(_allLogs.Count);
                foreach (var kvp in _logs) {
                    bool added = false;
                    foreach (var addedKvp in l)
                        if (addedKvp.Key == kvp.Key) {
                            added = true;
                            break;
                        }

                    if (!added && _allLogs.Contains(kvp.Key)) {
                        var newKvp = new KeyValuePair<Log, uint>(kvp.Key, kvp.Value);
                        l.Add(newKvp);
                    }
                }

                Logs = l.ToArray();
            }
        }

        public override void Activate() { SolutionComponentViewManager.Show(this); }
        public override string ToString() {
            if (_forDistributedTest)
                return Label;
            return base.ToString();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(Label);
                sw.Write(_runs);
                sw.Write(_minimumDelay);
                sw.Write(_maximumDelay);
                sw.Write(_monitorBefore);
                sw.Write(_monitorAfter);
                sw.Write(_concurrencies);
                sw.Write(_shuffle);
                sw.Write(_actionDistribution);
                sw.Write(_maximumNumberOfUserActions);
                sw.WriteObject(Connection);
                sw.WriteObject(_logs);
                sw.Write(_forDistributedTest);
                sw.Write(_useParallelExecutionOfLogEntries);

                //Parameters will be pushed in the child objects when deserializing, this is faster and way less memory consuming then serializing this for each object (each log entry has a reference to this object)

                GC.WaitForPendingFinalizers();
                sw.WriteObject(Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters);
                sw.AddToInfo(info);
            }
            sw = null;
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
        }
        #endregion
    }
}