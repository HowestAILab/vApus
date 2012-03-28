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
        private int _actionCount, _patternLength, _minimumDelay, _maximumDelay;

        private List<List<int>> _actions;

        //For shuffle
        private HashSet<int> _chosenSeeds = new HashSet<int>();
        //For fast action distribution
        private WeightedRandom _wrAction = new WeightedRandom();
        #endregion

        #region Properties
        public int PatternLength
        {
            get { return _patternLength; }
        }
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
        #endregion

        #region Con-/Destructor
        public TestPatternsAndDelaysGenerator(LogEntry[] logEntries, bool shuffleActionsAndLooseLogEntries, ActionAndLogEntryDistribution actionAndLogEntryDistribution, int minimumDelay, int maximumDelay)
        {
            _logEntries = logEntries;
            _shuffleActionsAndLooseLogEntries = shuffleActionsAndLooseLogEntries;
            _actionAndLogEntryDistribution = actionAndLogEntryDistribution;
            _minimumDelay = minimumDelay;
            _maximumDelay = maximumDelay;

            switch (_actionAndLogEntryDistribution)
            {
                case ActionAndLogEntryDistribution.None:
                    InitForNone();
                    break;
                case ActionAndLogEntryDistribution.Fast:
                    InitForFast();
                    break;
                case ActionAndLogEntryDistribution.Full:
                    InitForFull();
                    break;
            }
        }
        ~TestPatternsAndDelaysGenerator()
        {
            Dispose();
        }
        #endregion

        #region Functions

        private void InitForNone()
        {
            SolutionComponent currentParent = null;
            _actions = new List<List<int>>(_logEntries.Length);
            List<int> action = null;

            for (int i = 0; i < _logEntries.Length; i++)
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
            _actionCount = _actions.Count;
            _patternLength = _logEntries.Length;
        }
        private void InitForFast()
        {
            SolutionComponent currentParent = null;
            _actions = new List<List<int>>(_logEntries.Length);
            List<int> action = null;

            for (int i = 0; i < _logEntries.Length; i++)
            {
                LogEntry logEntry = _logEntries[i];
                //Do not add when it may not occur.
                if (logEntry.Occurance == 0 || (logEntry.Parent is UserAction && (logEntry.Parent as UserAction).Occurance == 0))
                    continue;
                if (currentParent != logEntry.Parent || currentParent is Log)
                {
                    currentParent = logEntry.Parent;
                    action = new List<int>();
                    _actions.Add(action);

                    //To pin log entries and user actions in place --> to not shuffle them.
                    //Add weights to be able to "fast distribute".
                    if (currentParent is Log)
                    {
                        action.SetTag(logEntry.Pinned);
                        _wrAction.AddWeight(logEntry.Occurance, 1);
                    }
                    else
                    {
                        var ua = currentParent as UserAction;
                        action.SetTag(ua.Pinned);
                        _wrAction.AddWeight(ua.Occurance, 1);
                    }
                }
                action.Add(i);
            }
            _actionCount = _actions.Count;

            int largestCount = 0;
            foreach (var a in _actions)
                if (a.Count > largestCount)
                    largestCount = a.Count;

            _patternLength = _actionCount * largestCount;
        }
        private void InitForFull()
        {
            SolutionComponent currentParent = null;
            _actions = new List<List<int>>(_logEntries.Length);
            List<List<int>> actions = new List<List<int>>(_logEntries.Length), logEntryOccurances = new List<List<int>>(_logEntries.Length);
            List<int> action = null, occuranceInAnAction = null, actionOccurances = new List<int>(_logEntries.Length);

            bool actionOccuranceAdded = false;
            for (int i = 0; i < _logEntries.Length; i++)
            {
                LogEntry logEntry = _logEntries[i];
                if (currentParent != logEntry.Parent || currentParent is Log)
                {
                    actionOccuranceAdded = false;
                    currentParent = logEntry.Parent;
                    action = new List<int>();
                    actions.Add(action);
                    occuranceInAnAction = new List<int>();
                    logEntryOccurances.Add(occuranceInAnAction);

                    //To pin log entries and user actions in place --> to not shuffle them.
                    if (currentParent is Log)
                        action.SetTag(logEntry.Pinned);
                    else
                        action.SetTag((currentParent as UserAction).Pinned);
                }
                if (currentParent is UserAction)
                {
                    if (!actionOccuranceAdded)
                    {
                        actionOccuranceAdded = true;
                        actionOccurances.Add((currentParent as UserAction).Occurance);
                    }
                    occuranceInAnAction.Add(logEntry.Occurance);
                }
                else
                {
                    actionOccurances.Add(logEntry.Occurance);
                    occuranceInAnAction.Add(1);
                }
                action.Add(i);
            }

            for (int i = 0; i < actions.Count; i++)
            {
                action = actions[i];
                List<int> fullAction = new List<int>(action.Count);
                for (int j = 0; j < action.Count; j++)
                    for (int k = 0; k < logEntryOccurances[i][j]; k++)
                        fullAction.Add(action[j]);

                fullAction.SetTag(action.GetTag());

                for (int j = 0; j < actionOccurances[i]; j++)
                {
                    _actions.Add(fullAction);
                    _patternLength += fullAction.Count;
                }
            }
            _actionCount = _actions.Count;
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

            if (_actionAndLogEntryDistribution == ActionAndLogEntryDistribution.Fast)
            {
                var tp = new List<int>(_patternLength);
                var dp = new List<int>(_patternLength);

                //'Choose' the pinned actions once.
                List<int> pinnedActions = new List<int>();
                for (int i = 0; i != _actionCount; i++)
                {
                    List<int> action = _actions[i];
                    if ((bool)action.GetTag())
                        pinnedActions.Add(i);
                }

                //Choose actions out of the remaining count.
                int remainingActionCount = _actionCount - pinnedActions.Count;
                int[] chosenNonPinnedActions = new int[remainingActionCount];

                for (int i = 0; i != remainingActionCount; i++)
                {
                    //Make sure a pinned action is not added.
                    int actionIndex = _wrAction.Next(true);
                    while (pinnedActions.Contains(actionIndex))
                        actionIndex = _wrAction.Next(true);

                    chosenNonPinnedActions[i] = actionIndex;
                }


                //Make a new list with the pinned and the non pinned.
                List<int> newChosenActions = new List<int>(_actionCount);

                //Add the non pinned indices
                foreach (int index in chosenNonPinnedActions)
                    newChosenActions.Add(index);

                //Add the pinned indices
                foreach (int index in pinnedActions)
                {
                    //Add or insert it.
                    if (index < newChosenActions.Count)
                        newChosenActions.Insert(index, index);
                    else
                        newChosenActions.Add(index);
                }

                //Now we can finally get the test and delay pattern.
                foreach (int actionIndex in newChosenActions)
                    foreach (int j in _actions[actionIndex])
                    {
                        tp.Add(j);
                        dp.Add(_logEntries[j].IgnoreDelay ? 0 : random.Next(_minimumDelay, _maximumDelay + 1));
                    }

                testPattern = tp.ToArray();
                delayPattern = dp.ToArray();
            }
            else
            {
                int index = 0;

                testPattern = new int[_patternLength];
                delayPattern = new int[_patternLength];

                if (_shuffleActionsAndLooseLogEntries)
                    Shuffle(random);

                foreach (List<int> action in _actions)
                    foreach (int j in action)
                    {
                        testPattern[index] = j;
                        delayPattern[index] = _logEntries[j].IgnoreDelay ? 0 : random.Next(_minimumDelay, _maximumDelay + 1);
                        ++index;
                    }
            }
        }
        private void Shuffle(Random random)
        {
            for (int i = 0; i < _actionCount; i++)
            {
                if ((bool)_actions[i].GetTag())
                    continue;

                int j = random.Next(0, _actionCount);
                while ((bool)_actions[j].GetTag())
                    j = random.Next(0, _actionCount);

                List<int> temp = _actions[i];
                _actions[i] = _actions[j];
                _actions[j] = temp;
            }
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _actions = null;
                _chosenSeeds = null;
                _wrAction = null;
            }
        }

        #endregion
    }
}