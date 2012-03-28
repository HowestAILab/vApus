/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, department PIH
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
    public class PatternGenerator : IDisposable
    {
        #region Fields
        private bool _isDisposed;
        private bool _shuffleActionsAndLooseLogEntries;
        private ActionAndLogEntryDistribution _actionAndLogEntryDistribution;
        private int _actionCount, _patternLength, _minimumDelay, _maximumDelay;

        private List<List<int>> _actions;

        //For shuffle
        private HashSet<int> _chosenSeeds = new HashSet<int>();
        //For fast action distribution
        private WeightedRandom _wrAction = new WeightedRandom();
        private List<WeightedRandom> _wrsLogEntries;
        #endregion

        public int PatternLength
        {
            get { return _patternLength; }
        }
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
        #region Con-/Destructor
        public PatternGenerator(LogEntry[] logEntries, bool shuffleActionsAndLooseLogEntries, ActionAndLogEntryDistribution actionAndLogEntryDistribution, int minimumDelay, int maximumDelay)
        {
            _shuffleActionsAndLooseLogEntries = shuffleActionsAndLooseLogEntries;
            _actionAndLogEntryDistribution = actionAndLogEntryDistribution;
            _minimumDelay = minimumDelay;
            _maximumDelay = maximumDelay;

            switch (_actionAndLogEntryDistribution)
            {
                case ActionAndLogEntryDistribution.None:
                    InitForNone(logEntries);
                    break;
                case ActionAndLogEntryDistribution.Fast:
                    InitForFast(logEntries);
                    break;
                case ActionAndLogEntryDistribution.Full:
                    InitForFull(logEntries);
                    break;
            }
        }
        ~PatternGenerator()
        {
            Dispose();
        }
        #endregion

        #region Functions

        private void InitForNone(LogEntry[] logEntries)
        {
            SolutionComponent currentParent = null;
            _actions = new List<List<int>>(logEntries.Length);
            List<int> action = null;

            for (int i = 0; i < logEntries.Length; i++)
            {
                LogEntry logEntry = logEntries[i];
                if (currentParent != logEntry.Parent || currentParent is Log)
                {
                    action = new List<int>();
                    _actions.Add(action);
                    currentParent = logEntry.Parent;
                }
                action.Add(i);
            }
            _actionCount = _actions.Count;
            _patternLength = logEntries.Length;
        }
        private void InitForFast(LogEntry[] logEntries)
        {
            SolutionComponent currentParent = null;
            _actions = new List<List<int>>(logEntries.Length);
            List<List<int>> actions = new List<List<int>>(logEntries.Length);
            List<int> action = null;

            _wrsLogEntries = new List<WeightedRandom>(logEntries.Length);
            WeightedRandom wrlogEntry = null;

            for (int i = 0; i < logEntries.Length; i++)
            {
                LogEntry logEntry = logEntries[i];
                if (currentParent != logEntry.Parent || currentParent is Log)
                {
                    action = new List<int>();
                    actions.Add(action);

                    wrlogEntry = new WeightedRandom();
                    _wrsLogEntries.Add(wrlogEntry);
                    currentParent = logEntry.Parent;

                    if (currentParent is UserAction)
                        _wrAction.AddWeight((currentParent as UserAction).Occurance, 1);
                }
                if (currentParent is UserAction)
                {
                    wrlogEntry.AddWeight(logEntry.Occurance, 1);
                }
                else
                {
                    _wrAction.AddWeight(logEntry.Occurance, 1);
                    wrlogEntry.AddWeight(1, 1);
                }
                action.Add(i);
            }

            _actions = actions;
            _actionCount = _actions.Count;
            _patternLength = logEntries.Length;
        }
        private void InitForFull(LogEntry[] logEntries)
        {
            SolutionComponent currentParent = null;
            _actions = new List<List<int>>(logEntries.Length);
            List<List<int>> actions = new List<List<int>>(logEntries.Length), logEntryOccurances = new List<List<int>>(logEntries.Length);
            List<int> action = null, occuranceInAnAction = null, actionOccurances = new List<int>(logEntries.Length);

            for (int i = 0; i < logEntries.Length; i++)
            {
                LogEntry logEntry = logEntries[i];
                if (currentParent != logEntry.Parent || currentParent is Log)
                {
                    action = new List<int>();
                    actions.Add(action);
                    occuranceInAnAction = new List<int>();
                    logEntryOccurances.Add(occuranceInAnAction);

                    currentParent = logEntry.Parent;
                }
                if (currentParent is UserAction)
                {
                    actionOccurances.Add((currentParent as UserAction).Occurance);
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
                List<int> child = actions[i], fullAction = new List<int>(child.Count);
                for (int j = 0; j < child.Count; j++)
                    for (int k = 0; k < logEntryOccurances[i][j]; k++)
                        fullAction.Add(child[j]);

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

            if (_shuffleActionsAndLooseLogEntries && _actionAndLogEntryDistribution != ActionAndLogEntryDistribution.Fast)
                Shuffle(random);

            testPattern = GetTestPattern();
            delayPattern = GetDelayPattern(random);
        }

        private void Shuffle(Random random)
        {
            for (int i = 0; i < _actionCount; i++)
            {
                int j = random.Next(0, _actionCount);
                List<int> temp = _actions[i];
                _actions[i] = _actions[j];
                _actions[j] = temp;
            }
        }
        private int[] GetTestPattern()
        {
            List<int> testPattern = new List<int>(_patternLength);
            if (_actionAndLogEntryDistribution == ActionAndLogEntryDistribution.Fast)
                for (int i = 0; i < _patternLength; i++)
                {
                    int actionIndex = _wrAction.Next(true);
                    List<int> action = _actions[actionIndex];
                    int logEntryIndex = _wrsLogEntries[actionIndex].Next(true);
                    for (int j = 0; j < action.Count; j++)
                        if (j == logEntryIndex)
                        {
                            testPattern.Add(action[j]);
                            break;
                        }
                }
            else
                foreach (List<int> action in _actions)
                    foreach (int j in action)
                        testPattern.Add(j);
            return testPattern.ToArray();
        }
        private int[] GetDelayPattern(Random random)
        {
            List<int> delayPattern = new List<int>(_patternLength);
            foreach (List<int> action in _actions)
                foreach (int i in action)
                    delayPattern.Add(random.Next(_minimumDelay, _maximumDelay + 1));
            return delayPattern.ToArray();
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _actions = null;
                _chosenSeeds = null;
                _wrAction = null;
                _wrsLogEntries = null;
            }
        }

        #endregion
    }
}