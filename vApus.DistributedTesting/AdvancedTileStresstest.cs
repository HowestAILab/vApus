/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using vApus.SolutionTree;
using vApus.StressTest;
using vApus.Util;

namespace vApus.DistributedTest {
    /// <summary>
    /// Advanced section of a TileStressTest
    /// </summary>
    public class AdvancedTileStressTest : BaseItem {

        #region Fields
        private int[] _concurrencies = { 5, 5, 10, 25, 50, 100 };
        private bool _actionDistribution;
        private int _maximumNumberOfUserActions;

        private Scenarios _allScenarios;
        private int[] _scenarioIndices = { };
        private uint[] _scenarioWeights = { };

        private KeyValuePair<Scenario, uint>[] _scenarios = { };

        private int _runs = 1, _initialMinimumDelay = 900, _initialMaximumDelay = 1100, _minimumDelay = 900, _maximumDelay = 1100, _monitorAfter, _monitorBefore;
        private bool _shuffle = true;

        private bool _useParallelExecutionOfRequests;
        private int _maximumPersistentConnections = 0;
        private int _persistentConnectionsPerHostname = 6; //Default for most browsers.

        private bool _simplifiedFastResults;
        #endregion

        #region Properties
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
        [PropertyControl(0)]
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

                    //New entries should have a weight of 1.
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


        [Description("The count(s) of the concurrent users generated, the minimum given value equals one.")]
        [SavableCloneable, PropertyControl(1)]
        public int[] Concurrencies {
            get { return _concurrencies; }
            set {
                if (value.Length == 0)
                    throw new ArgumentException();
                foreach (int i in value)
                    if (i < 1)
                        throw new ArgumentOutOfRangeException("A value in Concurrency cannot be smaller than one.");
                _concurrencies = value;
            }
        }

        [Description("A static multiplier of the runtime for each concurrency level. Must be greater than zero.")]
        [SavableCloneable, PropertyControl(2, 1, int.MaxValue)]
        public int Runs {
            get { return _runs; }
            set {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than 1.");
                _runs = value;
            }
        }

        [Description("The minimum delay in milliseconds before the execution of the first requests per user. This is not used in result calculations, but rather to spread the requests at the start of the test."), DisplayName("Initial minimum delay")]
        [PropertyControl(3, 0, int.MaxValue)]
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
        [PropertyControl(4, 0, int.MaxValue)]
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
        [PropertyControl(5, 0, int.MaxValue)]
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
        [PropertyControl(6, 0, int.MaxValue)]
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
        DisplayName("Action distribution")]
        [SavableCloneable, PropertyControl(8)]
        public bool ActionDistribution {
            get { return _actionDistribution; }
            set { _actionDistribution = value; }
        }

        [Description("This sets the maximum number of user actions that a test pattern for a user can contain. Pinned and linked actions however are always picked. Set this to zero to not use this."),
        DisplayName("Maximum number of user actions")]
        [SavableCloneable, PropertyControl(9, 0, int.MaxValue)]
        public int MaximumNumberOfUserActions {
            get { return _maximumNumberOfUserActions; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                _maximumNumberOfUserActions = value;
            }
        }

        [Description("Start monitoring before the test starts, expressed in minutes with a max of 60. The largest value for all tile stress tests is used."), DisplayName("Monitor before")]
        [SavableCloneable, PropertyControl(10, 0, int.MaxValue)]
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

        [Description("Continue monitoring after the test is finished, expressed in minutes with a max of 60. The largest value for all tile stress tests is used."), DisplayName("Monitor after")]
        [SavableCloneable, PropertyControl(11, 0, int.MaxValue)]
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

        [Description("Use this to enable the settings below. Should only be enabled for web tests."),
        DisplayName("Use parallel execution of requests")]
        [SavableCloneable, PropertyControl(12)]
        public bool UseParallelExecutionOfRequests {
            get { return _useParallelExecutionOfRequests; }
            set { _useParallelExecutionOfRequests = value; }
        }
        [Description("Fill in the maximum persistent connections that a browser allows. As was researched (May 2015): IE - infinite / on demand, Chrome & Opera - 10, Firefox - 256, Safari - > 59 (unknown). (0 == infinite)"),
         DisplayName("Maximum persistent connections")]
        [SavableCloneable, PropertyControl(13)]
        public int MaximumPersistentConnections {
            get { return _maximumPersistentConnections; }
            set { _maximumPersistentConnections = value; }
        }
        [Description("Fill in the maximum persistent connections per hostname that a browser allows. As was researched (May 2015): IE - infinite / on demand, others - 6. (0 == infinite)"),
         DisplayName("Persistent connections per hostname")]
        [SavableCloneable, PropertyControl(14)]
        public int PersistentConnectionsPerHostname {
            get { return _persistentConnectionsPerHostname; }
            set { _persistentConnectionsPerHostname = value; }
        }
        [Description("Set this to true for heavy tests (when using parallel requests). Nonetheless, Each run the full fast results will be calculated."),
DisplayName("Simplified fast results")]
        [SavableCloneable, PropertyControl(15)]
        public bool SimplifiedFastResults {
            get { return _simplifiedFastResults; }
            set { _simplifiedFastResults = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Advanced section of a TileStressTest
        /// </summary>
        public AdvancedTileStressTest() {
            ShowInGui = false;
            if (Solution.ActiveSolution != null)
                Init();
            else
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
        }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
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
            SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            try {
                if (sender != null && sender != this && Parent != null &&
                    (sender == Parent.GetParent().GetParent().GetParent() ||
                     sender == Parent || sender == (Parent as TileStressTest).DefaultAdvancedSettingsTo)) {
                    var parent = Parent as TileStressTest;
                    if (parent.AutomaticDefaultAdvancedSettings)
                        DefaultTo(parent.DefaultAdvancedSettingsTo);
                }
            } catch {
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

        internal void DefaultTo(StressTest.StressTest stressTest) {
            var scenarios = new KeyValuePair<Scenario, uint>[stressTest.Scenarios.Length];
            stressTest.Scenarios.CopyTo(scenarios, 0);
            scenarios.SetParent(_allScenarios);
            Scenarios = scenarios;

            _concurrencies = new int[stressTest.Concurrencies.Length];
            stressTest.Concurrencies.CopyTo(_concurrencies, 0);

            _runs = stressTest.Runs;
            _initialMinimumDelay = stressTest.InitialMinimumDelay;
            _initialMaximumDelay = stressTest.InitialMaximumDelay;
            _minimumDelay = stressTest.MinimumDelay;
            _maximumDelay = stressTest.MaximumDelay;
            _shuffle = stressTest.Shuffle;
            _actionDistribution = stressTest.ActionDistribution;
            _maximumNumberOfUserActions = stressTest.MaximumNumberOfUserActions;
            _monitorBefore = stressTest.MonitorBefore;
            _monitorAfter = stressTest.MonitorAfter;

            _useParallelExecutionOfRequests = stressTest.UseParallelExecutionOfRequests;
            _maximumPersistentConnections = stressTest.MaximumPersistentConnections;
            _persistentConnectionsPerHostname = stressTest.PersistentConnectionsPerHostname;

            _simplifiedFastResults = stressTest.SimplifiedFastResults;

            if (Solution.ActiveSolution != null)
                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        /// <summary>
        ///     Create a clone of this.
        /// </summary>
        /// <returns></returns>
        public AdvancedTileStressTest Clone() {
            var clone = new AdvancedTileStressTest();

            var defaultSettingsTo = (Parent as TileStressTest).DefaultAdvancedSettingsTo;

            clone._scenarios = new KeyValuePair<Scenario, uint>[defaultSettingsTo.Scenarios.Length];
            _scenarios.CopyTo(clone._scenarios, 0);
            clone._scenarios.SetParent(_allScenarios);


            clone._concurrencies = new int[_concurrencies.Length];
            _concurrencies.CopyTo(clone._concurrencies, 0);
            clone.Runs = _runs;
            clone.InitialMinimumDelayOverride = _initialMinimumDelay;
            clone.InitialMaximumDelayOverride = _initialMaximumDelay;
            clone.MinimumDelayOverride = _minimumDelay;
            clone.MaximumDelayOverride = _maximumDelay;
            clone.Shuffle = _shuffle;
            clone.ActionDistribution = _actionDistribution;
            clone.MaximumNumberOfUserActions = _maximumNumberOfUserActions;
            clone.MonitorBefore = _monitorBefore;
            clone.MonitorAfter = _monitorAfter;

            clone.UseParallelExecutionOfRequests = _useParallelExecutionOfRequests;
            clone.MaximumPersistentConnections = _maximumPersistentConnections;
            clone.PersistentConnectionsPerHostname = _persistentConnectionsPerHostname;

            clone.SimplifiedFastResults = _simplifiedFastResults;

            return clone;
        }
        #endregion
    }
}