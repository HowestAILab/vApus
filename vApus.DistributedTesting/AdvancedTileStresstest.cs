/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting {
    public class AdvancedTileStresstest : BaseItem {
        #region Fields

        private int[] _concurrencies = { 5, 5, 10, 25, 50, 100 };
        private Stresstest.Stresstest _defaultSettingsTo;
        private UserActionDistribution _distribute;
        protected internal Log _log;
        private int _maximumDelay = 1100;
        private int _minimumDelay = 900;
        private int _monitorAfter;
        private int _monitorBefore;
        private int _runs = 1;
        private bool _shuffle = true;

        #endregion

        #region Properties

        [Description("The log used to test the application.")]
        [SavableCloneable, PropertyControl(0)]
        public Log Log {
            get {
                if (_log != null) {
                    if (_log.IsEmpty)
                        Log =
                            GetNextOrEmptyChild(typeof(Log),
                                                Solution.ActiveSolution.GetSolutionComponent(typeof(Logs))) as Log;

                    _log.SetDescription("The log used to test the application. [" + LogRuleSet + "]");
                }

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

        [Description(
            "The minimum delay in milliseconds between the execution of log entries per user. Keep this and the maximum delay zero to have an ASAP test."
            ), DisplayName("Minimum Delay")]
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

        [Description(
            "The maximum delay in milliseconds between the execution of log entries per user. Keep this and the minimum delay zero to have an ASAP test."
            ), DisplayName("Maximum Delay")]
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

        [Description(
            "The user actions will be shuffled for each concurrent user when testing, creating unique usage patterns."
            )]
        [SavableCloneable, PropertyControl(5)]
        public bool Shuffle {
            get { return _shuffle; }
            set { _shuffle = value; }
        }

        [Description(
            "Fast: The length of the log stays the same, user actions are picked by chance based on its occurance, Full: user actions are executed X times its occurance."
            )]
        [SavableCloneable, PropertyControl(6)]
        public UserActionDistribution Distribute {
            get { return _distribute; }
            set { _distribute = value; }
        }

        [Description(
            "Start monitoring before the test starts, expressed in minutes with a max of 60. The largest value for all tile stresstests is used."
            ), DisplayName("Monitor Before")]
        [SavableCloneable, PropertyControl(7)]
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

        [Description(
            "Continue monitoring after the test is finished, expressed in minutes with a max of 60. The largest value for all tile stresstests is used."
            ), DisplayName("Monitor After")]
        [SavableCloneable, PropertyControl(8)]
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
            Log = GetNextOrEmptyChild(typeof(Log), Solution.ActiveSolution.GetSolutionComponent(typeof(Logs))) as Log;

            SolutionComponentChanged += SolutionComponent_SolutionComponentChanged;
        }

        private void _log_ParentIsNull(object sender, EventArgs e) {
            if (_log == sender)
                Log =
                    GetNextOrEmptyChild(typeof(Log), Solution.ActiveSolution.GetSolutionComponent(typeof(Logs))) as
                    Log;
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
        }

        internal void DefaultTo(Stresstest.Stresstest stresstest) {
            _defaultSettingsTo = stresstest;
            Log = _defaultSettingsTo.Log;
            _concurrencies = new int[_defaultSettingsTo.Concurrencies.Length];
            _defaultSettingsTo.Concurrencies.CopyTo(_concurrencies, 0);
            _runs = _defaultSettingsTo.Runs;
            _minimumDelay = _defaultSettingsTo.MinimumDelay;
            _maximumDelay = _defaultSettingsTo.MaximumDelay;
            _shuffle = _defaultSettingsTo.Shuffle;
            _distribute = _defaultSettingsTo.Distribute;
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
            clone.Log = _log;
            clone._concurrencies = new int[_concurrencies.Length];
            _concurrencies.CopyTo(clone._concurrencies, 0);
            clone.Runs = _runs;
            clone.MinimumDelayOverride = _minimumDelay;
            clone.MaximumDelayOverride = _maximumDelay;
            clone.Shuffle = _shuffle;
            clone.Distribute = _distribute;
            clone.MonitorBefore = _monitorBefore;
            clone.MonitorAfter = _monitorAfter;
            return clone;
        }

        #endregion
    }
}