/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using vApus.Util;

namespace vApus.Stresstest {
    public class TestPatternsAndDelaysGenerator : IDisposable {

        #region Fields

        private readonly LogEntry[] _logEntries;
        private readonly int _maxActionCount;
        private readonly int _maximumDelay;
        private readonly int _minimumDelay;
        private readonly bool _shuffleUserActions;
        private UserActionDistribution _userActionDistribution;

        private List<List<int>> _actions;

        //For shuffle
        private HashSet<int> _chosenSeeds = new HashSet<int>();
        private bool _isDisposed;

        #endregion

        #region Properties

        public int PatternLength {
            get { return _logEntries.Length; }
        }

        public int UserActionsInPattern {
            get { return _actions.Count; }
        }

        public bool IsDisposed {
            get { return _isDisposed; }
        }

        #endregion

        #region Con-/Destructor

        /// <summary>
        /// </summary>
        /// <param name="logEntries"></param>
        /// <param name="maxActionCount">Pinned actions are always chosen, non pinned are if this value allows it.</param>
        /// <param name="shuffleUserActions"></param>
        /// <param name="userActionDistribution"></param>
        /// <param name="minimumDelay"></param>
        /// <param name="maximumDelay"></param>
        public TestPatternsAndDelaysGenerator(LogEntry[] logEntries, int maxActionCount, bool shuffleUserActions,
                                              UserActionDistribution userActionDistribution, int minimumDelay, int maximumDelay) {
            _logEntries = logEntries;
            _maxActionCount = maxActionCount;
            _shuffleUserActions = shuffleUserActions;
            _userActionDistribution = userActionDistribution;
            _minimumDelay = minimumDelay;
            _maximumDelay = maximumDelay;

            Init();
        }

        ~TestPatternsAndDelaysGenerator() {
            Dispose();
        }

        #endregion

        #region Functions

        public void Dispose() {
            if (!_isDisposed) {
                _isDisposed = true;
                _actions = null;
                _chosenSeeds = null;
            }
        }

        private void Init() {
            UserAction currentParent = null;
            var unlinkedActions = new List<List<int>>(_logEntries.Length);
            List<int> action = null;

            var linkedUserActions = new Dictionary<int, List<int>>();

            for (int i = 0; i != _logEntries.Length; i++) {
                LogEntry logEntry = _logEntries[i];
                if (currentParent != logEntry.Parent) {
                    //Set the use delay if the currentparent != null
                    SetUseDelay(currentParent);

                    currentParent = logEntry.Parent as UserAction;
                    //Linked user actions will be merged to 1 afterwards. We want to set delays first.
                    if (currentParent.LinkedToUserActionIndices.Count != 0) {
                        var l = new List<int>();
                        foreach (int index in currentParent.LinkedToUserActionIndices)
                            l.Add(index - 1);
                        linkedUserActions.Add(unlinkedActions.Count, l);
                    }
                    action = new List<int>();

                    //To pin log entries and user actions in place --> to not shuffle them.
                    action.SetTag(currentParent.Pinned);

                    unlinkedActions.Add(action);
                }
                action.Add(i);
            }

            SetUseDelay(currentParent);

            _actions = new List<List<int>>();
            for (int i = 0; i < unlinkedActions.Count; i++) {
                var unlinkedAction = unlinkedActions[i];
                var linkedAction = new List<int>();
                linkedAction.SetTag(unlinkedAction.GetTag()); //Preserve pinned

                foreach (int j in unlinkedAction)
                    linkedAction.Add(j);

                if (linkedUserActions.ContainsKey(i)) {
                    int add = 0;
                    foreach (int j in linkedUserActions[i]) {
                        foreach (int k in unlinkedActions[j])
                            linkedAction.Add(k);
                        ++add;
                    }

                    i += add;
                }
                _actions.Add(linkedAction);
            }

        }
        private void SetUseDelay(UserAction userAction) {
            if (userAction != null) {
                LogEntry lastLogEntry = null;
                foreach (LogEntry logEntry in userAction) {
                    logEntry.UseDelay = false;
                    lastLogEntry = logEntry;
                }
                if (userAction.UseDelay && lastLogEntry != null)
                    lastLogEntry.UseDelay = true;
            }
        }

        public void GetPatterns(out int[] testPattern, out int[] delayPattern) {
            int seed = DateTime.Now.Millisecond;
            var random = new Random(seed);
            while (!_chosenSeeds.Add(seed)) {
                seed = random.Next();
                random = new Random(seed);
            }

            var tp = new List<int>();
            var dp = new List<int>();

            if (_shuffleUserActions)
                Shuffle(random);

            //Pinned actions are always chosen, non pinned can be chosen if the max allows it.
            int notPinnedToChoose = _maxActionCount - PinnedActionCount();
            if (notPinnedToChoose < 0)
                notPinnedToChoose = 0;

            foreach (var action in _actions)
                if ((bool)action.GetTag() || notPinnedToChoose-- > 0) {
                    foreach (int j in action) {
                        tp.Add(j);
                        dp.Add(_logEntries[j].UseDelay ? random.Next(_minimumDelay, _maximumDelay + 1) : 0);
                    }
                }

            testPattern = tp.ToArray();
            delayPattern = dp.ToArray();
        }

        private void Shuffle(Random random) {
            int actionCount = _actions.Count;
            for (int i = 0; i < actionCount; i++) {
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

        private int PinnedActionCount() {
            int actionCount = _actions.Count;
            int pinnedActionCount = 0;
            for (int i = 0; i < actionCount; i++)
                if ((bool)_actions[i].GetTag())
                    pinnedActionCount++;
            return pinnedActionCount;
        }

        #endregion
    }
}