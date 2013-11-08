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
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting {
    /// <summary>
    /// Advanced section of a TileStresstest
    /// </summary>
    public class AdvancedTileStresstest : BaseItem {

        #region Fields
        private int[] _concurrencies = { 5, 5, 10, 25, 50, 100 };
        private Stresstest.Stresstest _defaultSettingsTo;
        private bool _actionDistribution;
        private int _maximumNumberOfUserActions;

        private Logs _allLogs;
        private int[] _logIndices = { };
        private uint[] _logWeights = { };

        private KeyValuePair<Log, uint>[] _logs = { };

        private int _runs = 1, _minimumDelay = 900, _maximumDelay = 1100, _monitorAfter, _monitorBefore;
        private bool _shuffle = true;
        #endregion

        #region Properties
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
                    _logs.SetParent(_allLogs, false);
                }
            }
        }
        [Description("The logs used to test the application. Maximum 5 allowed and they must have the same log rule set. Change the weights to define the percentage distribution of users using a certain log.")]
        [PropertyControl(0)]
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
                    _logs.SetParent(_allLogs, false);

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
        [SavableCloneable, PropertyControl(2)]
        public int Runs {
            get { return _runs; }
            set {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than 1.");
                _runs = value;
            }
        }

        [Description("The minimum delay in milliseconds between the execution of log entries per user. Keep this and the maximum delay zero to have an ASAP test."), DisplayName("Minimum Delay")]
        [PropertyControl(3)]
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
        [PropertyControl(4)]
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
        [SavableCloneable, PropertyControl(5)]
        public bool Shuffle {
            get { return _shuffle; }
            set { _shuffle = value; }
        }

        [Description("When this is used, user actions are executed X times its occurance. You can use 'Shuffle' and 'Maximum Number of User Actions' in combination with this to define unique test patterns for each user."),
        DisplayName("Action Distribution")]
        [SavableCloneable, PropertyControl(6)]
        public bool ActionDistribution {
            get { return _actionDistribution; }
            set { _actionDistribution = value; }
        }

        [Description("This sets the maximum number of user actions that a test pattern for a user can contain. Pinned actions however are always picked. Set this to zero to not use this."),
        DisplayName("Maximum Number of User Actions")]
        [SavableCloneable, PropertyControl(7)]
        public int MaximumNumberOfUserActions {
            get { return _maximumNumberOfUserActions; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot be smaller than zero.");
                _maximumNumberOfUserActions = value;
            }
        }

        [Description("Start monitoring before the test starts, expressed in minutes with a max of 60. The largest value for all tile stresstests is used."), DisplayName("Monitor Before")]
        [SavableCloneable, PropertyControl(8)]
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

        [Description("Continue monitoring after the test is finished, expressed in minutes with a max of 60. The largest value for all tile stresstests is used."), DisplayName("Monitor After")]
        [SavableCloneable, PropertyControl(9)]
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

        #endregion

        #region Constructors
        /// <summary>
        /// Advanced section of a TileStresstest
        /// </summary>
        public AdvancedTileStresstest() {
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
            _logs.SetParent(_allLogs, false);

            if (_allLogs != null && _allLogs.Count > 1 && _logIndices.Length == 0) {
                _logWeights = new uint[] { 1 };
                LogIndices = new int[] { 1 };
            }
            SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e) {
            try {
                if (sender != null && Parent != null &&
                    (sender == Parent.GetParent().GetParent().GetParent() ||
                     sender == Parent || sender == (Parent as TileStresstest).DefaultAdvancedSettingsTo)) {
                    var parent = Parent as TileStresstest;
                    if (parent.AutomaticDefaultAdvancedSettings)
                        DefaultTo(parent.DefaultAdvancedSettingsTo);
                }
            } catch {
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

        internal void DefaultTo(Stresstest.Stresstest stresstest) {
            _defaultSettingsTo = stresstest;

            var logs = new KeyValuePair<Log, uint>[_defaultSettingsTo.Logs.Length];
            _defaultSettingsTo.Logs.CopyTo(logs, 0);
            logs.SetParent(_allLogs, false);
            Logs = logs;

            _concurrencies = new int[_defaultSettingsTo.Concurrencies.Length];
            _defaultSettingsTo.Concurrencies.CopyTo(_concurrencies, 0);

            _runs = _defaultSettingsTo.Runs;
            _minimumDelay = _defaultSettingsTo.MinimumDelay;
            _maximumDelay = _defaultSettingsTo.MaximumDelay;
            _shuffle = _defaultSettingsTo.Shuffle;
            _actionDistribution = _defaultSettingsTo.ActionDistribution;
            _maximumNumberOfUserActions = _defaultSettingsTo.MaximumNumberOfUserActions;
            _monitorBefore = _defaultSettingsTo.MonitorBefore;
            _monitorAfter = _defaultSettingsTo.MonitorAfter;

            if (Solution.ActiveSolution != null)
                InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        /// <summary>
        ///     Create a clone of this.
        /// </summary>
        /// <returns></returns>
        public AdvancedTileStresstest Clone() {
            var clone = new AdvancedTileStresstest();

            clone._logs = new KeyValuePair<Log, uint>[_defaultSettingsTo.Logs.Length];
            _logs.CopyTo(clone._logs, 0);
            clone._logs.SetParent(_allLogs, false);


            clone._concurrencies = new int[_concurrencies.Length];
            _concurrencies.CopyTo(clone._concurrencies, 0);
            clone.Runs = _runs;
            clone.MinimumDelayOverride = _minimumDelay;
            clone.MaximumDelayOverride = _maximumDelay;
            clone.Shuffle = _shuffle;
            clone.ActionDistribution = _actionDistribution;
            clone.MaximumNumberOfUserActions = _maximumNumberOfUserActions;
            clone.MonitorBefore = _monitorBefore;
            clone.MonitorAfter = _monitorAfter;
            return clone;
        }
        #endregion
    }
}