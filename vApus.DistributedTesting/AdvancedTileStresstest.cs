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

namespace vApus.DistributedTesting
{
    public class AdvancedTileStresstest : BaseItem
    {
        #region Fields
        private Stresstest.Stresstest _defaultSettingsTo;

        private int _precision = 1, _dynamicRunMultiplier = 1, _minimumDelay = 900, _maximumDelay = 1100;
        private int[] _concurrentUsers = { 5, 5, 10, 25, 50, 100 };
        private bool _shuffle = true;
        private ActionAndLogEntryDistribution _distribute;
        protected internal Log _log;
        #endregion

        #region Properties
        [Description("The log used to test the application.")]
        [SavableCloneable, PropertyControl(0)]
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

        [Description("The count(s) of the concurrent users generated, the minimum given value equals one."), DisplayName("Concurrent Users")]
        [SavableCloneable, PropertyControl(1)]
        public int[] ConcurrentUsers
        {
            get { return _concurrentUsers; }
            set
            {
                if (value.Length == 0)
                    throw new ArgumentException();
                foreach (int i in value)
                    if (i < 1)
                        throw new ArgumentOutOfRangeException("A value in Concurrent Users cannot be smaller than one.");
                _concurrentUsers = value;
            }
        }

        [Description("A static multiplier of the runtime for each concurrency level. Must be greater than zero.")]
        [SavableCloneable, PropertyControl(2)]
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
        [SavableCloneable, PropertyControl(3)]
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
        [PropertyControl(4)]
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
        [PropertyControl(5)]
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
        [SavableCloneable, PropertyControl(6)]
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
        [SavableCloneable, PropertyControl(7)]
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
        #endregion

        #region Constructors
        public AdvancedTileStresstest()
        {
            ShowInGui = false;
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

            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
        }
        private void _log_ParentIsNull(object sender, EventArgs e)
        {
            Log = SolutionComponent.GetNextOrEmptyChild(typeof(Log), Solution.ActiveSolution.GetSolutionComponent(typeof(Logs))) as Log;
        }
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            try
            {
                if (sender != null && this.Parent != null &&
                    (sender == this.Parent || sender == (this.Parent as TileStresstest).DefaultSettingsTo))
                {
                    TileStresstest parent = this.Parent as TileStresstest;
                    if (parent.AutomaticDefaultAdvancedSettings)
                        DefaultTo(parent.DefaultSettingsTo);
                }
            }
            catch { }
        }
        private void DefaultTo(Stresstest.Stresstest stresstest)
        {
            _defaultSettingsTo = stresstest;
            Log = _defaultSettingsTo.Log;
            _concurrentUsers = new int[_defaultSettingsTo.ConcurrentUsers.Length];
            _defaultSettingsTo.ConcurrentUsers.CopyTo(_concurrentUsers, 0);
            _precision = _defaultSettingsTo.Precision;
            _dynamicRunMultiplier = _defaultSettingsTo.DynamicRunMultiplier;
            _minimumDelay = _defaultSettingsTo.MinimumDelay;
            _maximumDelay = _defaultSettingsTo.MaximumDelay;
            _shuffle = _defaultSettingsTo.Shuffle;
            _distribute = _defaultSettingsTo.Distribute;

            if (Solution.ActiveSolution != null)
                this.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        /// <summary>
        /// Create a clone of this.
        /// </summary>
        /// <returns></returns>
        public AdvancedTileStresstest Clone()
        {
            var clone = new AdvancedTileStresstest();
            clone.Log = _log;
            clone._concurrentUsers = new int[_concurrentUsers.Length];
            _concurrentUsers.CopyTo(clone._concurrentUsers, 0);
            clone.Precision = _precision;
            clone.DynamicRunMultiplier = _dynamicRunMultiplier;
            clone.MinimumDelayOverride = _minimumDelay;
            clone.MaximumDelayOverride = _maximumDelay;
            clone.Shuffle = _shuffle;
            clone.Distribute = _distribute;
            return clone;
        }
        #endregion
    }
}
