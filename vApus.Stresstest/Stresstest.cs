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
    public class Stresstest : LabeledBaseItem {

        #region Fields
        private string _solution; //For the results.
        private int _runs = 1, _minimumDelay = 900, _maximumDelay = 1100, _monitorBefore, _monitorAfter;
        private int[] _concurrencies = { 5, 5, 10, 25, 50, 100 };
        private bool _shuffle = true;
        private UserActionDistribution _distribute;
        private Connection _connection;
        private Log _log;

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
        #endregion

        #region Properties
        /// <summary>
        ///     For the stresstest results.
        /// </summary>
        public string Solution {
            get {
                if (_solution == null && SolutionTree.Solution.ActiveSolution != null)
                    _solution = SolutionTree.Solution.ActiveSolution.FileName;
                return _solution;
            }
        }

        [Description("The connection to the application to test.")]
        [SavableCloneable, PropertyControl(0)]
        public Connection Connection {
            get {
                if (_connection.IsEmpty)
                    Connection =
                        GetNextOrEmptyChild(typeof(Connection),
                                            SolutionTree.Solution.ActiveSolution.GetSolutionComponent(
                                                typeof(Connections))) as Connection;

                if (_connection != null)
                    _connection.SetDescription("The connection to the application to test. [" + ConnectionProxy + "]");

                return _connection;
            }
            set {
                if (value == null)
                    return;
                value.ParentIsNull -= _connection_ParentIsNull;
                _connection = value;
                _connection.ParentIsNull += _connection_ParentIsNull;
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

        [Description("The log used to test the application.")]
        [SavableCloneable, PropertyControl(1)]
        public Log Log {
            get {
                if (_log.IsEmpty)
                    Log =
                        GetNextOrEmptyChild(typeof(Log),
                                            SolutionTree.Solution.ActiveSolution.GetSolutionComponent(typeof(Logs))) as
                        Log;

                if (_log != null)
                    _log.SetDescription("The log used to test the application. [" + LogRuleSet + "]");

                return _log;
            }
            set {
                if (value == null)
                    return;
                value.ParentIsNull -= _log_ParentIsNull;
                _log = value;
                _log.ParentIsNull += _log_ParentIsNull;
            }
        }

        [ReadOnly(true)]
        [DisplayName("Log Rule Set")]
        public string LogRuleSet {
            get {
                if (_log == null || _log.IsEmpty || _log.LogRuleSet.IsEmpty)
                    return "Log Rule Set: <none>";
                return _log.LogRuleSet.ToString();
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
        [SavableCloneable, PropertyControl(4)]
        public int Runs {
            get { return _runs; }
            set {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than 1.");
                _runs = value;
            }
        }

        [Description("The minimum delay in milliseconds between the execution of log entries per user. Keep this and the maximum delay zero to have an ASAP test."), DisplayName("Minimum Delay")]
        [PropertyControl(5, true)]
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
        [PropertyControl(6, true)]
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

        [Description("The user actions will be shuffled for each concurrent user when testing, creating unique usage patterns.")]
        [SavableCloneable, PropertyControl(7)]
        public bool Shuffle {
            get { return _shuffle; }
            set { _shuffle = value; }
        }

        [Description("Fast: The length of the log stays the same, user actions are picked by chance based on its occurance, Full: user actions are executed X times its occurance.")]
        [SavableCloneable, PropertyControl(8, true)]
        public UserActionDistribution Distribute {
            get { return _distribute; }
            set { _distribute = value; }
        }

        [Description("Start monitoring before the test starts, expressed in minutes with a max of 60."),
         DisplayName("Monitor Before")]
        [SavableCloneable, PropertyControl(9, true)]
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
        [SavableCloneable, PropertyControl(10, true)]
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
        #endregion

        #region Constructors
        public Stresstest() { if (SolutionTree.Solution.ActiveSolution != null)  Init(); else  SolutionTree.Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged; }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            SolutionTree.Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            Init();
        }
        private void Init() {
            Log =
                GetNextOrEmptyChild(typeof(Log),
                                    SolutionTree.Solution.ActiveSolution.GetSolutionComponent(typeof(Logs))) as Log;
            Connection =
                GetNextOrEmptyChild(typeof(Connection),
                                    SolutionTree.Solution.ActiveSolution.GetSolutionComponent(typeof(Connections))) as
                Connection;
            _monitorProject =
                SolutionTree.Solution.ActiveSolution.GetSolutionComponent(typeof(MonitorProject)) as MonitorProject;

            var l = new List<Monitor.Monitor>(_monitorIndices.Length);
            foreach (int index in _monitorIndices)
                if (index < _monitorProject.Count)
                    l.Add(_monitorProject[index] as Monitor.Monitor);

            _monitors = l.ToArray();
            _monitors.SetParent(_monitorProject);

            SolutionComponentChanged += SolutionComponentChanged_SolutionComponentChanged;
        }

        private void _connection_ParentIsNull(object sender, EventArgs e) {
            if (_connection == sender)
                Connection =
                    GetNextOrEmptyChild(typeof(Connection),
                                        SolutionTree.Solution.ActiveSolution.GetSolutionComponent(typeof(Connections)))
                    as Connection;
        }
        private void _log_ParentIsNull(object sender, EventArgs e) {
            if (_log == sender)
                Log =
                    GetNextOrEmptyChild(typeof(Log),
                                        SolutionTree.Solution.ActiveSolution.GetSolutionComponent(typeof(Logs))) as Log;
        }
        private void SolutionComponentChanged_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            //Cleanup _monitors if _monitorProject Changed
            if (sender == _monitorProject || sender is Monitor.Monitor) {
                var l = new List<Monitor.Monitor>(_monitorProject.Count);
                foreach (Monitor.Monitor monitor in _monitors)
                    if (!l.Contains(monitor) && _monitorProject.Contains(monitor))
                        l.Add(monitor);

                Monitors = l.ToArray();
            }
        }

        /// <summary>
        ///     For the stresstest results.
        ///     Sets the solution field to the active one.
        /// </summary>
        public void SetSolution() {
            if (SolutionTree.Solution.ActiveSolution != null)
                _solution = SolutionTree.Solution.ActiveSolution.FileName;
        }

        public override void Activate() { SolutionComponentViewManager.Show(this); }
        public override string ToString() {
            if (_forDistributedTest)
                return Label;
            return base.ToString();
        }
        #endregion
    }
}