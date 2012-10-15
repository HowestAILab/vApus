/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    public class TestPatternsAndDelaysGenerator : IDisposable
    {
        #region Fields
        private LogEntry[] _logEntries;
        private bool _isDisposed;
        private bool _shuffleActionsAndLooseLogEntries;
        private ActionAndLogEntryDistribution _actionAndLogEntryDistribution;
        private int _maxActionCount, _minimumDelay, _maximumDelay;

        private List<List<int>> _actions;

        //For shuffle
        private HashSet<int> _chosenSeeds = new HashSet<int>();
        #endregion

        #region Properties
        public int PatternLength
        {
            get { return _logEntries.Length; }
        }
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
        #endregion

        #region Con-/Destructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logEntries"></param>
        /// <param name="maxActionCount">Pinned actions are always chosen, non pinned are if this value allows it.</param>
        /// <param name="shuffleActionsAndLooseLogEntries"></param>
        /// <param name="actionAndLogEntryDistribution"></param>
        /// <param name="minimumDelay"></param>
        /// <param name="maximumDelay"></param>
        public TestPatternsAndDelaysGenerator(LogEntry[] logEntries, int maxActionCount, bool shuffleActionsAndLooseLogEntries, ActionAndLogEntryDistribution actionAndLogEntryDistribution, int minimumDelay, int maximumDelay)
        {
            _logEntries = logEntries;
            _maxActionCount = maxActionCount;
            _shuffleActionsAndLooseLogEntries = shuffleActionsAndLooseLogEntries;
            _actionAndLogEntryDistribution = actionAndLogEntryDistribution;
            _minimumDelay = minimumDelay;
            _maximumDelay = maximumDelay;

            Init();
        }
        ~TestPatternsAndDelaysGenerator()
        {
            Dispose();
        }
        #endregion

        #region Functions

        private void Init()
        {
            SolutionComponent currentParent = null;
            _actions = new List<List<int>>(_logEntries.Length);
            List<int> action = null;

            for (int i = 0; i != _logEntries.Length; i++)
            {
                LogEntry logEntry = _logEntries[i];
                if (currentParent != logEntry.Parent || currentParent is Log)
                {
                    currentParent = logEntry.Parent;
                    action = new List<int>();

                    //To pin log entries and user actions in place --> to not shuffle them.
                    if (currentParent is Log)
                        action.SetTag(logEntry.Pinned);
                    else
                        action.SetTag((currentParent as UserAction).Pinned);

                    _actions.Add(action);

                }
                action.Add(i);
            }
        }
        public void GetPatterns(out int[] testPattern, out int[] delayPattern)
        {
            int seed = DateTime.Now.Millisecond;
            Random random = new Random(seed);
            while (!_chosenSeeds.Add(seed))
            {
                seed = random.Next();
                random = new Random(seed);
            }

            var tp = new List<int>();
            var dp = new List<int>();

            if (_shuffleActionsAndLooseLogEntries)
                Shuffle(random);

            //Pinned actions are always chosen, non pinned can be chosen if the max allows it.
            int notPinnedToChoose = _maxActionCount - PinnedActionCount();
            if (notPinnedToChoose < 0)
                notPinnedToChoose = 0;

            foreach (var action in _actions)
                if ((bool)action.GetTag() || notPinnedToChoose-- > 0)
                    foreach (int j in action)
                    {
                        tp.Add(j);
                        dp.Add(_logEntries[j].IgnoreDelay ? 0 : random.Next(_minimumDelay, _maximumDelay + 1));
                    }

            testPattern = tp.ToArray();
            delayPattern = dp.ToArray();
        }
        private void Shuffle(Random random)
        {
            int actionCount = _actions.Count;
            for (int i = 0; i < actionCount; i++)
            {
                if ((bool)_actions[i].GetTag())
                    continue;

                int j = random.Next(0, actionCount);
                while ((bool)_actions[j].GetTag()) //Do not shuffle pinned actions
                    j = random.Next(0, actionCount);

                List<int> temp = _actions[i];
                _actions[i] = _actions[j];
                _actions[j] = temp;
            }
        }
        private int PinnedActionCount()
        {
            int actionCount = _actions.Count;
            int pinnedActionCount = 0;
            for (int i = 0; i < actionCount; i++)
                if ((bool)_actions[i].GetTag())
                    pinnedActionCount++;
            return pinnedActionCount;
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _actions = null;
                _chosenSeeds = null;
            }
        }
        #endregion
    }
}