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
using vApus.SolutionTree;
using vApus.Util;
using System.Reflection;

namespace vApus.Stresstest
{
    [Serializable]
    [ContextMenu(new string[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new string[] { "Edit", "Remove", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new string[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new Keys[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    public class Stresstest : LabeledBaseItem
    {
        #region Fields
        /// <summary>
        /// In seconds how fast the stresstest progress will be updated.
        /// </summary>
        public const int ProgressUpdateDelay = 5;
        private int _precision = 1, _dynamicRunMultiplier = 1, _minimumDelay = 900, _maximumDelay = 1100;
        private int[] _concurrentUsers = { 5, 5, 10, 25, 50, 100 };
        private bool _shuffle = true;
        private ActionAndLogEntryDistribution _distribute;
        private Connection _connection;
        private Log _log;
        //This will be saved, I don't want to extend the save logic so I hack around it.
        [NonSerialized]
        private Monitor.MonitorProject _monitorProject;
        [NonSerialized]
        private int[] _monitorIndices = { };
        [NonSerialized]
        private Monitor.Monitor[] _monitors = { };
        private bool _useParallelExecutionOfLogEntries;
        #endregion

        #region Properties
        [Description("The connection to the application to test.")]
        [SavableCloneable, PropertyControl(0)]
        public Connection Connection
        {
            get
            {

                if (_connection != null)
                    _connection.SetDescription("The connection to the application to test. [" + ConnectionProxy + "]");

                return _connection;
            }
            set
            {
                if (value == null)
                    return;
                value.ParentIsNull -= _connection_ParentIsNull;
                _connection = value;
                _connection.ParentIsNull += _connection_ParentIsNull;
            }
        }
        [ReadOnly(true)]
        [DisplayName("Connection Proxy")]
        public string ConnectionProxy
        {
            get
            {
                if (_connection == null || _connection.IsEmpty || _connection.ConnectionProxy.IsEmpty)
                    return "Connection Proxy: <none>";
                return _connection.ConnectionProxy.ToString();
            }
        }

        [Description("The log used to test the application.")]
        [SavableCloneable, PropertyControl(1)]
        public Log Log
        {
            get
            {
                if (_log != null)
                    _log.SetDescription("The log used to test the application. [" + LogRuleSet + "]");

                return _log;
            }
            set
            {
                if (value == null)
                    return;
                value.ParentIsNull -= _log_ParentIsNull;
                _log = value;
                _log.ParentIsNull += _log_ParentIsNull;
            }
        }
        [ReadOnly(true)]
        [DisplayName("Log Rule Set")]
        public string LogRuleSet
        {
            get
            {
                if (_log == null || _log.IsEmpty || _log.LogRuleSet.IsEmpty)
                    return "Log Rule Set: <none>";
                return _log.LogRuleSet.ToString();
            }
        }

        [SavableCloneable]
        public int[] MonitorIndices
        {
            get { return _monitorIndices; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");

                _monitorIndices = value;
                if (_monitorProject != null)
                {
                    List<Monitor.Monitor> l = new List<Monitor.Monitor>(_monitorIndices.Length);
                    foreach (int index in _monitorIndices)
                    {
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
        public Monitor.Monitor[] Monitors
        {
            get { return _monitors; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");
                if (value.Length > 5)
                    throw new ArgumentOutOfRangeException("Maximum 5 allowed.");

                _monitors = value;
                if (_monitorProject != null)
                {
                    _monitors.SetParent(_monitorProject);


                    List<int> l = new List<int>(_monitors.Length);
                    for (int index = 0; index != _monitorProject.Count; index++)
                        if (_monitors.Contains(_monitorProject[index]) && !l.Contains(index))
                            l.Add(index);

                    _monitorIndices = l.ToArray();
                }
            }
        }

        [Description("The count(s) of the concurrent users generated, the minimum given value equals one."), DisplayName("Concurrent Users")]
        [SavableCloneable, PropertyControl(3)]
        public int[] ConcurrentUsers
        {
            get { return _concurrentUsers; }
            set
            {
                if (value.Length == 0)
                    throw new ArgumentException();
                foreach (int i in value)
                    if (i < 1)
                        throw new ArgumentOutOfRangeException("A value in Concurrent Users cannot be smaller then one.");
                _concurrentUsers = value;
            }
        }

        [Description("A static multiplier of the runtime for each concurrency level. Must be greater than zero.")]
        [SavableCloneable, PropertyControl(4)]
        public int Precision
        {
            get { return _precision; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than 1.");
                _precision = value;
            }
        }

        [Description("Useful for tests with low or no delay. It appends the runtime for every concurrency level in a way they even out. For example: a minimum request of 10 with 5 concurrent users makes the runtime twice as long, of 15 three times."), DisplayName("Dynamic Run Multiplier")]
        [SavableCloneable, PropertyControl(5)]
        public int DynamicRunMultiplier
        {
            get { return _dynamicRunMultiplier; }
            set
            {
                if (value < 1)
                    throw new Exception("The multiplier cannot be smaller than one.");
                _dynamicRunMultiplier = value;
            }
        }

        [Description("The minimum delay in milliseconds between the execution of log entries per user. Keep this and the maximum delay zero to have an ASAP test."), DisplayName("Minimum Delay")]
        [PropertyControl(6)]
        public int MinimumDelay
        {
            get { return _minimumDelay; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                if (value > _maximumDelay)
                    _maximumDelay = value;
                _minimumDelay = value;
            }
        }

        /// <summary>
        /// Only for saving and loading, should not be used.
        /// </summary>
        [SavableCloneable]
        public int MinimumDelayOverride
        {
            get { return _minimumDelay; }
            set { _minimumDelay = value; }
        }

        [Description("The maximum delay in milliseconds between the execution of log entries per user. Keep this and the minimum delay zero to have an ASAP test."), DisplayName("Maximum Delay")]
        [PropertyControl(7)]
        public int MaximumDelay
        {
            get { return _maximumDelay; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                if (value < _minimumDelay)
                    _minimumDelay = value;
                _maximumDelay = value;
            }
        }

        /// <summary>
        /// Only for saving and loading, should not be used.
        /// </summary>
        [SavableCloneable]
        public int MaximumDelayOverride
        {
            get { return _maximumDelay; }
            set { _maximumDelay = value; }
        }

        [Description("The actions and loose log entries will be shuffled for each concurrent user when testing, creating unique usage patterns; obligatory for Fast Action and Log Entry Distribution.")]
        [SavableCloneable, PropertyControl(8)]
        public bool Shuffle
        {
            get { return _shuffle; }
            set
            {
                if (value == false && _distribute == ActionAndLogEntryDistribution.Fast)
                    throw new Exception("Fast action and log entry distribution cannot happen unshuffled.");
                _shuffle = value;
            }
        }

        [Description("Action and Loose Log Entry Distribution; Fast: The length of the log stays the same, entries are picked by chance based on its occurance, Full: entries are executed X times its occurance. Note:This can't be used in combination with parameters (works but it breaks the parameter refresh logic, giving a wrong result).")]
        [SavableCloneable, PropertyControl(9)]
        public ActionAndLogEntryDistribution Distribute
        {
            get { return _distribute; }
            set
            {
                if (value == ActionAndLogEntryDistribution.Fast && !_shuffle)
                    throw new Exception("For 'Fast Action and Log Entry Distribution' the 'Shuffle Actions and Loose Log Entries' property must be set to 'True'.");
                _distribute = value;
            }
        }

#if EnableBetaFeature
        [Description("If this equals false then the parallel switch for log entries is ignored."), DisplayName("Use Parallel Execution of Log Entries")]
        [SavableCloneable, PropertyControl(10)]
        public bool UseParallelExecutionOfLogEntries
        {
            get { return _useParallelExecutionOfLogEntries; }
            set { _useParallelExecutionOfLogEntries = value; }
        }
#else
#warning Parallel executions temp not available
        public bool UseParallelExecutionOfLogEntries
        {
            get { return false; }
            set { _useParallelExecutionOfLogEntries = false; }
        }
#endif
        #endregion

        #region Constructors
        public Stresstest()
        {
            if (Solution.ActiveSolution != null)
                Init();
            else
                Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);
        }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            Init();
        }
        private void Init()
        {
            Log = SolutionComponent.GetNextOrEmptyChild(typeof(Log), Solution.ActiveSolution.GetSolutionComponent(typeof(Logs))) as Log;
            Connection = SolutionComponent.GetNextOrEmptyChild(typeof(Connection), Solution.ActiveSolution.GetSolutionComponent(typeof(Connections))) as Connection;
            _monitorProject = Solution.ActiveSolution.GetSolutionComponent(typeof(Monitor.MonitorProject)) as Monitor.MonitorProject;

            List<Monitor.Monitor> l = new List<Monitor.Monitor>(_monitorIndices.Length);
            foreach (int index in _monitorIndices)
                if (index < _monitorProject.Count)
                    l.Add(_monitorProject[index] as Monitor.Monitor);

            _monitors = l.ToArray();
            _monitors.SetParent(_monitorProject);

            SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponentChanged_SolutionComponentChanged);
        }
        private void _connection_ParentIsNull(object sender, EventArgs e)
        {
            Connection = SolutionComponent.GetNextOrEmptyChild(typeof(Connection), Solution.ActiveSolution.GetSolutionComponent(typeof(Connections))) as Connection;
        }
        private void _log_ParentIsNull(object sender, EventArgs e)
        {
            Log = SolutionComponent.GetNextOrEmptyChild(typeof(Log), Solution.ActiveSolution.GetSolutionComponent(typeof(Logs))) as Log;
        }
        private void SolutionComponentChanged_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            //Cleanup _monitors if _monitorProject Changed
            if (sender == _monitorProject || sender is Monitor.Monitor)
            {
                List<Monitor.Monitor> l = new List<Monitor.Monitor>(_monitorProject.Count);
                foreach (Monitor.Monitor monitor in _monitors)
                    if (!l.Contains(monitor) && _monitorProject.Contains(monitor))
                        l.Add(monitor);

                Monitors = l.ToArray();
            }
        }
        public override void Activate()
        {
            SolutionComponentViewManager.Show(this);
        }
        #endregion
    }
}
