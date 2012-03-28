/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;
using System.Linq;

namespace vApus.DistributedTesting
{
    /// <summary>
    /// The configuration of a certain stresstest in a certain tile.
    /// </summary>
    [Serializable]
    public class TileStresstest : LabeledBaseItem
    {
        #region Fields
        private bool _use = false;
        private string _slaveIP = "?";
        private int _slavePort = SocketListener.DEFAULTPORT;
        private int[] _processorAffinity = new int[] { };

        //Stresstest property override
        [NonSerialized]
        private Stresstest.Stresstest _defaultStresstest;
        private int _precision = 1, _dynamicRunMultiplier = 0, _minimumDelay = 800, _maximumDelay = 1000;
        private int[] _concurrentUsers = { 5, 5, 10, 25, 50, 100 };
        private bool _shuffle = true;
        private ActionAndLogEntryDistribution _distribute;
        private Connection _connection;
        private Log _log;
        //This will be saved, I don't want to extend the save logic so I hack around it.
        [NonSerialized]
        private Monitor.MonitorProject _monitorProject;
        //This will be saved, I don't want to extend the save logic so I hack around it.
        [NonSerialized]
        private int[] _monitorIndices = { };
        //This will be saved, I don't want to extend the save logic so I hack around it.
        [NonSerialized]
        private Monitor.Monitor[] _monitors = { };

        private bool _useParallelExecutionOfLogEntries;

        private int _originalHashCode;
        private RunSynchronization _runSynchronization;
        #endregion

        #region Properties
        /// <summary>
        ///The name of this item.
        /// </summary>
        public new string Name
        {
            get { return "Test"; }
        }
        [SavableCloneable, PropertyControl(0)]
        public new string Label
        {
            get { return base.Label; }
            set { base.Label = value; }
        }
        
        [SavableCloneable, PropertyControl(1)]
        [DisplayName("Use this Tile Stresstest")]
        public bool Use
        {
            get { return _use; }
            set { _use = value; }
        }
        
        [SavableCloneable, PropertyControl(2)]
        [Description("The IP of a computer running vApus."), DisplayName("Slave IP")]
        public string SlaveIP
        {
            get { return _slaveIP; }
            set { _slaveIP = value.Length == 0 ? "?" : value; }
        }
        
        [SavableCloneable, PropertyControl(3)]
        [Description("The port the running instance of vApus is listening to."), DisplayName("Slave Port")]
        public int SlavePort
        {
            get { return _slavePort; }
            set
            {
                if (value < SocketListener.DEFAULTPORT)
                    throw new Exception(string.Format("Cannot be smaller than {0}.", SocketListener.DEFAULTPORT));
                _slavePort = value;
            }
        }
        
        [SavableCloneable, PropertyControl(4)]
        [Description("The one-based indices of the logical processing units to where the slave process should be affinated to (if possible). Leave empty to affinate to all PU's."), DisplayName("Processor Affinity")]
        public int[] ProcessorAffinity
        {
            get { return _processorAffinity; }
            set
            {
                HashSet<int> duplicatesCheck = new HashSet<int>();
                foreach (int i in value)
                {
                    if (i < 1)
                        throw new Exception("Cannot be smaller than one.");
                    if (!duplicatesCheck.Add(i))
                        throw new Exception("Duplicates are not allowed.");
                }
                _processorAffinity = value;
            }
        }
        //Stresstest property override
        
        [SavableCloneable]
        public Stresstest.Stresstest DefaultStresstest
        {
            get { return _defaultStresstest; }
            set { _defaultStresstest = value; }
        }
        
        [Description("The connection to the application to test.")]
        [SavableCloneable, PropertyControl(5)]
        public Connection Connection
        {
            get { return _connection; }
            set
            {
                if (value == null)
                    return;

                _connection = value;
            }
        }
        [Description("This is used in and defines the connection."), DisplayName("Connection Proxy")]
        [PropertyControl(6)]
        public string ConnectionProxy
        {
            get
            {
                if (_connection == null || _connection.IsEmpty || _connection.ConnectionProxy.IsEmpty)
                    return "<none>";
                return _connection.ConnectionProxy.ToString();
            }
        }
        
        [Description("The log used to test the application.")]
        [SavableCloneable, PropertyControl(7)]
        public Log Log
        {
            get { return _log; }
            set
            {
                if (value == null)
                    return;
                _log = value;
            }
        }
        [ReadOnly(true)]
        [Description("This is used in and defines the log entries."), DisplayName("Log Rule Set")]
        [PropertyControl(8)]
        public string LogRuleSet
        {
            get
            {
                if (_log == null || _log.IsEmpty || _log.LogRuleSet.IsEmpty)
                    return "<none>";
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
        [PropertyControl(9)]
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
        [SavableCloneable, PropertyControl(10)]
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
        [SavableCloneable, PropertyControl(11)]
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
        
        [Description("Useful for test with low or no delay. It appends the runtime for every concurrency level in a way they even out. For example: a minimimum request of 10 with 5 concurrent users makes the runtime twice as long, of 15 three times."), DisplayName("Dynamic Run Multiplier")]
        [SavableCloneable, PropertyControl(12)]
        public int DynamicRunMultiplier
        {
            get { return _dynamicRunMultiplier; }
            set
            {
                if (value < 0)
                    throw new Exception("The multiplier cannot be smaller than zero.");
                _dynamicRunMultiplier = value;
            }
        }
        
        [Description("The minimum delay in milliseconds between the execution of log entries per user. Keep this and the maximum delay zero to have an ASAP test."), DisplayName("Minimum Delay")]
        [PropertyControl(13)]
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
        [PropertyControl(14)]
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
        
        [Description("The actions and loose log entries will be shuffled for each concurrent user when testing, creating unique usage patterns, obligatory for Fast Action and Log Entry Distribution.")]
        [SavableCloneable, PropertyControl(15)]
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
        [SavableCloneable, PropertyControl(16)]
        public ActionAndLogEntryDistribution Distribute
        {
            get { return _distribute; }
            set
            {
                if (value == ActionAndLogEntryDistribution.Fast)
                    _shuffle = true;
                _distribute = value;
            }
        }
        [ReadOnly(true)]
        [Description("Saves the results slave-side in the directory \"\\SlaveSideResults\". The name of the sub folder is \"PID \" + the process Id of the vApus instance."), DisplayName("Slave-Side Result Saving")]
        [PropertyControl(17)]
        public bool SlaveSideResultSaving
        {
            get { return true; }
        }

        #if EnableBetaFeature
        [Description("If this equals false then the parallel switch for log entries is ignored."), DisplayName("Use Parallel Execution of Log Entries")]
        [SavableCloneable, PropertyControl(17)]
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
        /// <summary>
        /// A unique identifier to match results received from slaves to the right tile stresstest.
        /// </summary>
        public int OriginalHashCode
        {
            get { return _originalHashCode; }
            set { _originalHashCode = value; }
        }
        
        public RunSynchronization RunSynchronization
        {
            get { return _runSynchronization; }
            set { _runSynchronization = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// The configuration of a certain stresstest in a certain tile.
        /// </summary>
        public TileStresstest()
        {
            //To update the monitor settings
            vApus.SolutionTree.SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);

            if (Solution.ActiveSolution != null)
            {
                _defaultStresstest = BaseItem.Empty(typeof(Stresstest.Stresstest), Solution.ActiveSolution.GetSolutionComponent(typeof(StresstestProject))) as Stresstest.Stresstest;
                _connection = BaseItem.Empty(typeof(Connection), Solution.ActiveSolution.GetSolutionComponent(typeof(Connections))) as Connection;
                _log = BaseItem.Empty(typeof(Log), Solution.ActiveSolution.GetSolutionComponent(typeof(Logs))) as Log;
                _monitorProject = Solution.ActiveSolution.GetSolutionComponent(typeof(Monitor.MonitorProject)) as Monitor.MonitorProject;

                List<Monitor.Monitor> l = new List<Monitor.Monitor>(_monitorIndices.Length);
                foreach (int index in _monitorIndices)
                    if (index < _monitorProject.Count)
                        l.Add(_monitorProject[index] as Monitor.Monitor);

                _monitors = l.ToArray();
                _monitors.SetParent(_monitorProject);
            }
            else
            {
                Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);
            }
        }
        /// <summary>
        /// The configuration of a certain stresstest in a certain tile.
        /// This will copy the properties of the default stresstest and also store this default stresstest.
        /// </summary>
        /// <param name="defaultStresstest"></param>
        public TileStresstest(Stresstest.Stresstest defaultStresstest)
            : this()
        {
            _defaultStresstest = defaultStresstest;
            SetDefaults();
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
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
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            _defaultStresstest = BaseItem.Empty(typeof(Stresstest.Stresstest), Solution.ActiveSolution.GetSolutionComponent(typeof(StresstestProject))) as Stresstest.Stresstest;
            _connection = BaseItem.Empty(typeof(Connection), Solution.ActiveSolution.GetSolutionComponent(typeof(Connections))) as Connection;
            _log = BaseItem.Empty(typeof(Log), Solution.ActiveSolution.GetSolutionComponent(typeof(Logs))) as Log;
            _monitorProject = Solution.ActiveSolution.GetSolutionComponent(typeof(Monitor.MonitorProject)) as Monitor.MonitorProject;

            List<Monitor.Monitor> l = new List<Monitor.Monitor>(_monitorIndices.Length);
            foreach (int index in _monitorIndices)
                if (index < _monitorProject.Count)
                    l.Add(_monitorProject[index] as Monitor.Monitor);

            _monitors = l.ToArray();
            _monitors.SetParent(_monitorProject);
        }
        /// <summary>
        /// Sets the defaults from the containing default stresstest.
        /// </summary>
        public void SetDefaults()
        {
            _maximumDelay = int.MaxValue;
            foreach (PropertyInfo info in _defaultStresstest.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                if (info.GetCustomAttributes(typeof(SavableCloneableAttribute), true).Length > 0)
                    try
                    {
                        object o = info.GetValue(_defaultStresstest, null);
                        if (o != null)
                            this.GetType().GetProperty(info.Name).SetValue(this, o, null);
                    }
                    catch { }

            _connection.Parent = Solution.ActiveSolution.GetSolutionComponent(typeof(Connections));
            _log.Parent = Solution.ActiveSolution.GetSolutionComponent(typeof(Logs));
        }
        /// <summary>
        /// Get a new stresstest based on the made settings.
        /// </summary>
        /// <param name="monitor">null if none</param>
        /// <returns></returns>
        public Stresstest.Stresstest GetNewStresstest()
        {
            Stresstest.Stresstest stresstest = new Stresstest.Stresstest();
            stresstest.ShowInGui = false;
            stresstest.Distribute = _distribute;
            stresstest.ConcurrentUsers = _concurrentUsers;

            Connections connections = new Connections();
            connections.AddWithoutInvokingEvent(_connection);
            _connection.ForceSettingChildsParent();

            stresstest.Connection = _connection;
            stresstest.DynamicRunMultiplier = _dynamicRunMultiplier;
            stresstest.Label = Label + "[" + OriginalHashCode + "]";

            Logs logs = new Logs();
            logs.AddWithoutInvokingEvent(_log);
            _log.ForceSettingChildsParent();

            stresstest.Log = _log;

            //Nothing happens with this on the other side
            //var monitors = Monitors;
            //stresstest.Monitors = new Monitor.Monitor[monitors.Length];
            //for (int i = 0; i != monitors.Length; i++)
            //    stresstest.Monitors[i] = monitors[i];

            stresstest.MinimumDelayOverride = _minimumDelay;
            stresstest.MaximumDelayOverride = _maximumDelay;
            stresstest.Precision = _precision;
            stresstest.Shuffle = _shuffle;
            stresstest.UseParallelExecutionOfLogEntries = _useParallelExecutionOfLogEntries;

            stresstest.ForceSettingChildsParent();

            return stresstest;
        }
        public override string ToString()
        {
            if (IsEmpty)
                return null;
            return Label == string.Empty ? string.Format("{0} {1} ({2}:{3})", Name, Index, _slaveIP, _slavePort) : string.Format("{0} {1} ({2}:{3}): {4}", Name, Index, _slaveIP, _slavePort, Label);
        }
        #endregion
    }
}