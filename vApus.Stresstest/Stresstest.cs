/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
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

namespace vApus.StressTest {
    /// <summary>
    /// Is mainly a configuration file that bring all the pieces of a stress test together split into basic and advanced properties
    /// </summary>
    [Serializable, DisplayName("Stress test")]
    [ContextMenu(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { "Edit", "Remove", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    public class StressTest : LabeledBaseItem, ISerializable {

        #region Fields
        private int _runs = 2, _initialMinimumDelay = 0, _initialMaximumDelay = 20000, _minimumDelay = 900, _maximumDelay = 1100, _monitorBefore = 1, _monitorAfter = 1;
        private int[] _concurrencies = { 5, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
        private bool _shuffle = true;
        private bool _actionDistribution;
        private int _maximumNumberOfUserActions;
        private Connection _connection;

        [NonSerialized]
        private Scenarios _allScenarios;
        [NonSerialized]
        private int[] _scenarioIndices = { };
        [NonSerialized]
        private uint[] _scenarioWeights = { };

        private KeyValuePair<Scenario, uint>[] _scenarios = { };

        //This will be saved, I don't want to extend the save logic so I hack around it.
        [NonSerialized]
        private MonitorProject _monitorProject;
        [NonSerialized]
        private int[] _monitorIndices = { };
        [NonSerialized]
        private Monitor.Monitor[] _monitors = { };

        //private bool _useParallelExecutionOfRequests;

        //For in the results database
        private string _description = string.Empty;
        private string[] _tags = new string[0];

        /// <summary>
        ///     Let for instance the gui behave differently if this is true.
        /// </summary>
        private bool _forDistributedTest;

        private bool _useParallelExecutionOfRequests;
        private int _maximumPersistentConnections = 0;
        private int _persistentConnectionsPerHostname = 6; //Default for most browsers.

        private Parameters _parameters; //Kept here for a distributed test

        private bool _simplifiedFastResults = true;
        #endregion

        #region Properties
        [Description("The description for this test that will be stored in the results database for easy finding.")]
        [SavableCloneable, PropertyControl(0)]
        public string Description {
            get { return _description; }
            set { _description = value; }
        }

        [Description("The tags for this test that will be stored in the results database for easy finding.")]
        [SavableCloneable, PropertyControl(1)]
        public string[] Tags {
            get { return _tags; }
            set {
                if (value == null) {
                    _tags = null;
                }
                else {
                    _tags = new string[value.Length];
                    var ss = new SortedSet<string>();
                    for (int i = 0; i != value.Length; i++) 
                        ss.Add(value[i].ToLowerInvariant());

                    ss.CopyTo(_tags);                       
                }
            }
        }

        [Description("The connection to the application to test.")]
        [SavableCloneable, PropertyControl(2)]
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
        [DisplayName("Connection proxy")]
        public string ConnectionProxy {
            get {
                if (_connection == null || _connection.IsEmpty || _connection.ConnectionProxy.IsEmpty)
                    return "Connection proxy: <none>";
                return _connection.ConnectionProxy.ToString();
            }
        }

        [SavableCloneable]
        public uint[] ScenarioWeights {
            get { return _scenarioWeights; }
            set {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");
                _scenarioWeights = value;
            }
        }
        [SavableCloneable]
        public int[] ScenarioIndices {
            get { return _scenarioIndices; }
            set {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");
                _scenarioIndices = value;

                if (_allScenarios != null) {
                    if (_scenarioIndices.Length == 0 && _allScenarios.Count > 1) {
                        _scenarioWeights = new uint[] { 1 };
                        _scenarioIndices = new int[] { 1 };
                    }
                    var l = new List<KeyValuePair<Scenario, uint>>(_scenarioIndices.Length);
                    int weightIndex = 0;
                    foreach (int index in _scenarioIndices) {
                        if (index < _allScenarios.Count) {
                            var scenario = _allScenarios[index] as Scenario;

                            bool added = false;
                            foreach (var addedKvp in l)
                                if (addedKvp.Key == scenario) {
                                    added = true;
                                    break;
                                }
                            if (!added) {
                                uint weight = weightIndex < _scenarioWeights.Length ? _scenarioWeights[weightIndex] : 0;
                                l.Add(new KeyValuePair<Scenario, uint>(scenario, weight));
                            }
                        }
                        ++weightIndex;
                    }
                    _scenarios = l.ToArray();
                    _scenarios.SetParent(_allScenarios);
                }
            }
        }
        [Description("The scenarios used to test the application. They must have the same scenario rule set. Change the weights to define the percentage distribution of users using a certain scenario.")]
        [PropertyControl(3)]
        public KeyValuePair<Scenario, uint>[] Scenarios {
            get { return _scenarios; }
            set {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");

                if (_allScenarios != null && _allScenarios.Count > 1 && value.Length == 0) {
                    _scenarioWeights = new uint[] { 1 };
                    ScenarioIndices = new int[] { 1 };
                    return;
                }

                if (value.Length != 0) {
                    var scenarioRuleSet = value[0].Key.ScenarioRuleSet;
                    for (int i = 1; i < value.Length; i++)
                        if (value[i].Key.ScenarioRuleSet != scenarioRuleSet)
                            throw new Exception("Only scenarios having the same scenario rule set are allowed.");

                    //New requests should have a weight of 1.
                    for (int i = _scenarios.Length; i < value.Length; i++)
                        value[i] = new KeyValuePair<vApus.StressTest.Scenario, uint>(value[i].Key, 1);

                    //1 should not be 0 :).
                    bool allZeros = true;
                    for (int i = 0; i != value.Length; i++)
                        if (value[i].Value != 0) {
                            allZeros = false;
                            break;
                        }
                    if (allZeros) value[0] = new KeyValuePair<vApus.StressTest.Scenario, uint>(value[0].Key, 1);
                }

                _scenarios = value;

                if (_allScenarios != null) {
                    _scenarios.SetParent(_allScenarios);

                    var scenarioIndices = new List<int>(_scenarios.Length);
                    var scenarioWeights = new List<uint>(_scenarios.Length);
                    for (int allScenariosIndex = 1; allScenariosIndex < _allScenarios.Count; allScenariosIndex++) {
                        KeyValuePair<Scenario, uint> kvp = new KeyValuePair<Scenario, uint>();
                        for (int scenarioIndex = 0; scenarioIndex != _scenarios.Length; scenarioIndex++)
                            if (_scenarios[scenarioIndex].Key == _allScenarios[allScenariosIndex]) {
                                kvp = _scenarios[scenarioIndex];
                                break;
                            }

                        if (kvp.Key != null) {
                            scenarioIndices.Add(allScenariosIndex);
                            scenarioWeights.Add(kvp.Value);
                        }
                    }

                    _scenarioIndices = scenarioIndices.ToArray();
                    _scenarioWeights = scenarioWeights.ToArray();
                }
            }
        }

        /// <summary>
        /// Does not validate and does not set the parent.
        /// </summary>
        public KeyValuePair<Scenario, uint>[] ScenariosOverride { set { _scenarios = value; } }

        [ReadOnly(true)]
        [DisplayName("Scenario rule set")]
        public string ScenarioRuleSet {
            get {
                if (_scenarios.Length == 0)
                    return "Scenario rule set: <none>";
                var scenario = _scenarios[0].Key;
                if (scenario == null || scenario.IsEmpty || scenario.ScenarioRuleSet.IsEmpty)
                    return "Scenario rule set: <none>";
                return scenario.ScenarioRuleSet.ToString();
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

        [Description("The monitors used to link stress test results to performance counters.")]
        [PropertyControl(4)]
        public Monitor.Monitor[] Monitors {
            get { return _monitors; }
            set {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");

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
        [SavableCloneable, PropertyControl(5)]
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
        [SavableCloneable, PropertyControl(6, 1, int.MaxValue)]
        public int Runs {
            get { return _runs; }
            set {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than 1.");
                _runs = value;
            }
        }

        [Description("The minimum delay in milliseconds before the execution of the first requests per user. This is not used in result calculations, but rather to spread the requests at the start of the test."), DisplayName("Initial minimum delay")]
        [PropertyControl(7, true, 0, int.MaxValue)]
        public int InitialMinimumDelay {
            get { return _initialMinimumDelay; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                if (value > _initialMaximumDelay)
                    _initialMaximumDelay = value;
                _initialMinimumDelay = value;
            }
        }
        /// <summary>
        ///     Only for saving and loading, should not be used.
        /// </summary>
        [SavableCloneable]
        public int InitialMinimumDelayOverride {
            get { return _initialMinimumDelay; }
            set { _initialMinimumDelay = value; }
        }

        [Description("The maximum delay in milliseconds before the execution of the first requests per user. This is not used in result calculations, but rather to spread the requests at the start of the test."), DisplayName("Initial maximum delay")]
        [PropertyControl(8, true, 0, int.MaxValue)]
        public int InitialMaximumDelay {
            get { return _initialMaximumDelay; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                if (value < _initialMinimumDelay)
                    _initialMinimumDelay = value;
                _initialMaximumDelay = value;
            }
        }

        /// <summary>
        ///     Only for saving and loading, should not be used.
        /// </summary>
        [SavableCloneable]
        public int InitialMaximumDelayOverride {
            get { return _initialMaximumDelay; }
            set { _initialMaximumDelay = value; }
        }

        [Description("The minimum delay in milliseconds between the execution of requests per user. Keep this and the maximum delay zero to have an ASAP test."), DisplayName("Minimum delay")]
        [PropertyControl(9, true, 0, int.MaxValue)]
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

        [Description("The maximum delay in milliseconds between the execution of requests per user. Keep this and the minimum delay zero to have an ASAP test."), DisplayName("Maximum delay")]
        [PropertyControl(10, true, 0, int.MaxValue)]
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
        [SavableCloneable, PropertyControl(11)]
        public bool Shuffle {
            get { return _shuffle; }
            set { _shuffle = value; }
        }

        [Description("When this is used, user actions are executed X times its occurance. You can use 'Shuffle' and 'Maximum Number of User Actions' in combination with this to define unique test patterns for each user."),
        DisplayName("Action distribution")]
        [SavableCloneable, PropertyControl(12, false, 0)]
        public bool ActionDistribution {
            get { return _actionDistribution; }
            set { _actionDistribution = value; }
        }

        [Description("The maximum number of user actions that a test pattern for a user can contain. Pinned and linked actions however are always picked. Set this to zero to not use this."),
        DisplayName("Maximum number of user actions")]
        [SavableCloneable, PropertyControl(13, false, 0)]
        public int MaximumNumberOfUserActions {
            get { return _maximumNumberOfUserActions; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                _maximumNumberOfUserActions = value;
            }
        }

        [Description("Start monitoring before the test starts, expressed in minutes with a max of 60."),
         DisplayName("Monitor before")]
        [SavableCloneable, PropertyControl(14, false, 0)]
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
         DisplayName("Monitor after")]
        [SavableCloneable, PropertyControl(15, false, 0)]
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

        [Description("Use this to enable the settings below. Should only be enabled for web tests. IMPORTANT: If you parameterize the Hostname- and/or Redirects field of a request, parallelisation might not be correct. For genereral performance reasons only the fields' string value as it is is checked to determine this."),
         DisplayName("[Browser] Use parallel execution of requests")]
        [SavableCloneable, PropertyControl(16, true)]
        public bool UseParallelExecutionOfRequests {
            get { return _useParallelExecutionOfRequests; }
            set { _useParallelExecutionOfRequests = value; }
        }
        [Description("Fill in the maximum persistent connections that a browser allows. As was researched (May 2015): IE - infinite / on demand, Chrome & Opera - 10, Firefox - 256, Safari - > 59 (unknown). (0 == infinite)"),
         DisplayName("[Browser] Maximum persistent connections")]
        [SavableCloneable, PropertyControl(17, true)]
        public int MaximumPersistentConnections {
            get { return _maximumPersistentConnections; }
            set { _maximumPersistentConnections = value; }
        }
        [Description("Fill in the maximum persistent connections per hostname that a browser allows. As was researched (May 2015): IE - infinite / on demand, others - 6. (0 == infinite)"),
         DisplayName("[Browser] Persistent connections per hostname")]
        [SavableCloneable, PropertyControl(18, true)]
        public int PersistentConnectionsPerHostname {
            get { return _persistentConnectionsPerHostname; }
            set { _persistentConnectionsPerHostname = value; }
        }
        [Description("Set this to true for heavy tests (when using parallel requests). Nonetheless, Each run the full fast results will be calculated."),
         DisplayName("Simplified fast results")]
        [SavableCloneable, PropertyControl(19, true)]
        public bool SimplifiedFastResults {
            get { return _simplifiedFastResults; }
            set { _simplifiedFastResults = value; }
        }
        /// <summary>
        ///     Let for instance the gui behave differently if this is true.
        /// </summary>
        public bool ForDistributedTest {
            get { return _forDistributedTest; }
            set { _forDistributedTest = value; }
        }
        /// <summary>
        /// Is this a part of a stress test divided over multiple slaves?
        /// </summary>
        public bool IsDividedStressTest { get; set; }
        #endregion

        #region Constructors
        public StressTest() {
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
        public StressTest(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                _runs = sr.ReadInt32();
                _initialMinimumDelay = sr.ReadInt32();
                _initialMaximumDelay = sr.ReadInt32();
                _minimumDelay = sr.ReadInt32();
                _maximumDelay = sr.ReadInt32();
                _monitorBefore = sr.ReadInt32();
                _monitorAfter = sr.ReadInt32();
                _concurrencies = sr.ReadArray(typeof(int)) as int[];
                _shuffle = sr.ReadBoolean();
                _actionDistribution = sr.ReadBoolean();
                _maximumNumberOfUserActions = sr.ReadInt32();
                _connection = sr.ReadObject() as Connection;
                _scenarios = sr.ReadObject() as KeyValuePair<Scenario, uint>[];
                _forDistributedTest = sr.ReadBoolean();
                _useParallelExecutionOfRequests = sr.ReadBoolean();
                _simplifiedFastResults = sr.ReadBoolean();

                _parameters = sr.ReadObject() as Parameters;
                _parameters.ForceSettingChildsParent();

                Connection.Parameters = _parameters;
                Scenario.Parameters = _parameters;
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
            _allScenarios = SolutionTree.Solution.ActiveSolution.GetSolutionComponent(typeof(Scenarios)) as Scenarios;

            var scenarios = new List<KeyValuePair<Scenario, uint>>(_scenarioIndices.Length);
            int weightIndex = 0;
            foreach (int index in _scenarioIndices) {
                if (index < _allScenarios.Count) {
                    var scenario = _allScenarios[index] as Scenario;
                    uint weight = weightIndex < _scenarioWeights.Length ? _scenarioWeights[weightIndex] : 0;

                    scenarios.Add(new KeyValuePair<Scenario, uint>(scenario, weight));
                }
                ++weightIndex;
            }

            _scenarios = scenarios.ToArray();
            _scenarios.SetParent(_allScenarios);

            if (_allScenarios != null && _allScenarios.Count > 1 && _scenarioIndices.Length == 0) {
                _scenarioWeights = new uint[] { 1 };
                ScenarioIndices = new int[] { 1 };
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
            //Cleanup _monitors/_scenarios if _monitorProject Changed
            if (sender == _monitorProject || sender is Monitor.Monitor) {
                var l = new List<Monitor.Monitor>(_monitorProject.Count);
                foreach (Monitor.Monitor monitor in _monitors)
                    if (!l.Contains(monitor) && _monitorProject.Contains(monitor))
                        l.Add(monitor);

                Monitors = l.ToArray();
            }

            if (sender == _allScenarios || sender is Scenario) {
                var l = new List<KeyValuePair<Scenario, uint>>(_allScenarios.Count);
                foreach (var kvp in _scenarios) {
                    bool added = false;
                    foreach (var addedKvp in l)
                        if (addedKvp.Key == kvp.Key) {
                            added = true;
                            break;
                        }

                    if (!added && _allScenarios.Contains(kvp.Key)) {
                        var newKvp = new KeyValuePair<Scenario, uint>(kvp.Key, kvp.Value);
                        l.Add(newKvp);
                    }
                }

                Scenarios = l.ToArray();
            }
        }

        public override BaseSolutionComponentView Activate() { return SolutionComponentViewManager.Show(this); }
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
                sw.Write(_initialMinimumDelay);
                sw.Write(_initialMaximumDelay);
                sw.Write(_minimumDelay);
                sw.Write(_maximumDelay);
                sw.Write(_monitorBefore);
                sw.Write(_monitorAfter);
                sw.Write(_concurrencies);
                sw.Write(_shuffle);
                sw.Write(_actionDistribution);
                sw.Write(_maximumNumberOfUserActions);
                sw.WriteObject(Connection);
                sw.WriteObject(_scenarios);
                sw.Write(_forDistributedTest);
                sw.Write(_useParallelExecutionOfRequests);
                sw.Write(_simplifiedFastResults);

                //Parameters will be pushed in the child objects when deserializing, this is faster and way less memory consuming then serializing this for each object (each request has a reference to this object)

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