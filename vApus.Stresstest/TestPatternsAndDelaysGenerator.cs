/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Threading;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    /// Generates test patterns with delays for the different simulated users in StressTestCore. Delays and the way a test pattern is build is determined in stress test (Min- max delay, shuffle, ActionDistribution).
    /// </summary>
    internal class TestPatternsAndDelaysGenerator { //: IDisposable {

        #region Fields
        private readonly Request[] _requests;
        private readonly int _maxActionCount, _initialMaximumDelay, _initialMinimumDelay, _maximumDelay, _minimumDelay;
        private readonly bool _shuffleUserActions;

        //Representation of the user actions (List<int>) containing request indices.
        private List<List<int>> _actions;

        //For shuffle, multiple generators are used in a test, therefore this is static.
        private static HashSet<int> _chosenSeeds = new HashSet<int>();
        private bool _isDisposed;
        #endregion

        #region Properties
        public int PatternLength { get { return _requests.Length; } }
        #endregion

        #region Con-/Destructor
        /// <summary>
        /// Generates test patterns with delays for the different simulated users in StressTestCore. Delays and the way a test pattern is build is determined in stress test (Min- max delay, UserActionDistribution).
        /// </summary>
        /// <param name="requests">Predetermined in StressTestCore for difficulty reasons (depends on UserActionDistribution).</param>
        /// <param name="maxActionCount">Pinned actions are always chosen, non pinned are if this value allows it. This value depends on UserActionDistribution but is determined in StressTestCore for difficulty reasons.</param>
        /// <param name="shuffleUserActions"></param>
        /// <param name="userActionDistribution"></param>
        /// <param name="initialMinimumDelay"></param>
        /// <param name="initialMaximumDelay"></param>
        /// <param name="minimumDelay"></param>
        /// <param name="maximumDelay"></param>
        public TestPatternsAndDelaysGenerator(Request[] requests, int maxActionCount, bool shuffleUserActions, int initialMinimumDelay, int initialMaximumDelay, int minimumDelay, int maximumDelay) {
            _requests = requests;
            _maxActionCount = maxActionCount;
            _shuffleUserActions = shuffleUserActions;
            _initialMinimumDelay = initialMinimumDelay;
            _initialMaximumDelay = initialMaximumDelay;
            _minimumDelay = minimumDelay;
            _maximumDelay = maximumDelay;

            Init();
        }
        ~TestPatternsAndDelaysGenerator() { Dispose(); }
        #endregion

        #region Functions
        private void Init() {
            UserAction currentParent = null;
            var unlinkedActions = new List<List<int>>(_requests.Length);
            List<int> action = null;

            var linkedUserActions = new Dictionary<int, List<int>>();

            for (int i = 0; i != _requests.Length; i++) {
                Request request = _requests[i];
                if (currentParent != request.Parent) {
                    //Set the use delay if the currentparent != null
                    SetUseDelay(currentParent);

                    currentParent = request.Parent as UserAction;
                    //Linked user actions will be merged to 1 afterwards. We want to set delays first.
                    if (currentParent.LinkedToUserActionIndices.Count != 0) {
                        var l = new List<int>();
                        foreach (int index in currentParent.LinkedToUserActionIndices)
                            l.Add(index - 1);
                        linkedUserActions.Add(unlinkedActions.Count, l);
                    }
                    action = new List<int>();

                    //To pin requests and user actions in place --> to not shuffle them.
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
                Request lastRequest = null;
                foreach (Request request in userAction) {
                    request.UseDelay = false;
                    lastRequest = request;
                }
                if (userAction.UseDelay && lastRequest != null)
                    lastRequest.UseDelay = true;
            }
        }

        public void GetPatterns(out int[] testPattern, out int initialDelayInMilliseconds, out int[] delayPattern) {
            int seed = 0;

            do {
                try {
                    seed = Guid.NewGuid().GetHashCode(); //Not using DateTime.Now or Environement.Ticks because this is updated every few milliseconds and we need a Thread.Sleep.
                } catch {
                    //To many items.
                    _chosenSeeds = new HashSet<int>();
                }
            } while (!_chosenSeeds.Add(seed));


            var random = new Random(seed);

            initialDelayInMilliseconds = random.Next(_initialMinimumDelay, _initialMaximumDelay + 1);

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
                        dp.Add(_requests[j].UseDelay ? random.Next(_minimumDelay, _maximumDelay + 1) : 0);
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

        public void Dispose() {
            if (!_isDisposed) {
                _isDisposed = true;
                _actions = null;
            }
        }
        #endregion
    }
}