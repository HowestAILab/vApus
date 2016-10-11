/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vApus.Publish;
using vApus.Results;
using vApus.Util;

namespace vApus.StressTest {
    public class StressTestCore : IDisposable {

        #region Events
        public event EventHandler<TestInitializedEventArgs> TestInitialized;
        public event EventHandler<StressTestResultEventArgs> StressTestStarted;
        public event EventHandler<ConcurrencyResultEventArgs> ConcurrencyStarted, ConcurrencyStopped;
        public event EventHandler<RunResultEventArgs> RunInitializedFirstTime, RunStarted, RunStopped;
        public event EventHandler<RunResultEventArgs> RunDoneOnce, RerunStarted, RerunDone;
        public event EventHandler<MessageEventArgs> Message;
        /// <summary>
        /// Be carefull when you use this. Only to output results to be handled elsewhere (other process).
        /// </summary>
        public event EventHandler<OnRequestEventArgs> OnRequest;
        #endregion

        #region Fields
        /// <summary>
        ///     To be able to execute requests parallel.
        /// </summary>
        [ThreadStatic]
        private static SyncAndAsyncWorkItem _syncAndAsyncWorkItem;

        private readonly StressTest _stressTest;
        private ValueStore _valueStore;
        private Scenarios _scenariosStub = new Scenarios(); //For distributed tests.

        /// <summary>
        /// Multiple scenarios can occur in one test, the (incremental) percentage division is kept here also.
        /// </summary>
        private KeyValuePair<Scenario, KeyValuePair<Request[], float>>[] _requests;
        private TestableRequest[][] _testableRequests; //First index: user. Second: the request

        private StressTestResult _stressTestResult;
        /// <summary>
        /// For publisher.
        /// </summary>
        internal volatile int _concurrencyId = -1;
        /// <summary>
        /// The result of the current executing concurrency.
        /// </summary>
        internal RunResult _runResult;
        /// <summary>
        /// The result of the current executing concurrency.
        /// </summary>
        internal ConcurrencyResult _concurrencyResult;

        private Dictionary<Scenario, TestPatternsAndDelaysGenerator> _testPatternsAndDelaysGenerators;
        private int[] _initialDelaysInMilliseconds;
        private int[][] _delaysInMilliseconds;
        private AutoResetEvent _sleepWaitHandle = new AutoResetEvent(false); //Better than Thread.Sleep to wait a delay after sending to / receiving from the server app.

        private StressTestThreadPool _threadPool;
        private ConnectionProxyPool _connectionProxyPool;

        /// <summary> Needed for break on last. </summary>
        private volatile bool _runDoneOnce;
        private volatile int _rerun = 0;
        /// <summary> Every time the execution is broken (Break) the continue counter is incremented by one. This to be sure that if a continue is send it is reacted on it the right way. </summary>
        private int _continueCounter;

        private AutoResetEvent _runSynchronizationContinueWaitHandle = new AutoResetEvent(false), _manyToOneWaitHandle = new AutoResetEvent(false);

        private volatile bool _break, _cancel, _completed, _isFailed, _isDisposed;
        private volatile bool _waitWhenInitializedTheFirstRun = false;

        /// <summary> The number of all parallel connections per user. Those are put in the pool in a queue. When needed one is dequeued.</summary>
        private int _parallelConnections;
        /// <summary> The number of all parallel threads per user. Those are put in the pool in a queue. When needed one is dequeued.</summary>
        private int _parallelThreads;
        #endregion

        #region Properties

        public StressTestResult StressTestResult { get { return _stressTestResult; } }

        public RunSynchronization RunSynchronization { get; set; }

        public int MaxRerunsBreakOnLast { get; set; }

        public int BusyThreadCount { get { return _threadPool == null ? 0 : _threadPool.BusyThreadCount; } }

        public bool IsDisposed { get { return _isDisposed; } }

        /// <summary>
        /// Set to true when distributed testing.
        /// </summary>
        public bool WaitWhenInitializedTheFirstRun { set { _waitWhenInitializedTheFirstRun = value; } }
        #endregion

        #region Con-/Destructor
        /// <summary>
        /// Don't forget to set the resultshelper if need be
        /// </summary>
        /// <param name="stressTest"></param>
        public StressTestCore(StressTest stressTest, ValueStore valueStore) {
            ObjectRegistrar.MaxRegistered = 1;
            ObjectRegistrar.Register(this);

            _stressTest = stressTest;
            _valueStore = valueStore;
        }
        ~StressTestCore() {
            Dispose();
        }
        #endregion

        #region Functions

        #region Eventing
        private void SetStressTestStarted() {
            _stressTestResult = new StressTestResult();
            InvokeMessage("Starting the stress test...");

            if (!_cancel && StressTestStarted != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { StressTestStarted(this, new StressTestResultEventArgs(_stressTestResult)); }, null);
        }

        private void SetConcurrencyStarted(int concurrentUsersIndex) {
            int concurrentUsers = _stressTest.Concurrencies[concurrentUsersIndex];
            _concurrencyResult = new ConcurrencyResult(concurrentUsersIndex, concurrentUsers, _stressTest.Runs);
            _stressTestResult.ConcurrencyResults.Add(_concurrencyResult);
            InvokeMessage(
                string.Format("|-> {0} Concurrent Users... (Initializing the first run, please be patient)", concurrentUsers),
                Color.MediumPurple);

            if (!_cancel && ConcurrencyStarted != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { ConcurrencyStarted(this, new ConcurrencyResultEventArgs(_concurrencyResult)); }, null);
        }
        private void SetConcurrencyStopped() {
            _concurrencyResult.StoppedAt = DateTime.Now;
            if (!_cancel && ConcurrencyStopped != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { ConcurrencyStopped(this, new ConcurrencyResultEventArgs(_concurrencyResult)); }, null);
        }

        /// <summary>
        ///     For monitoring --> to know the time offset of the counters so a range can be linked to a run.
        ///     The current run result (_runResult) is also given with in the event's event args.
        /// </summary>
        private void SetRunStarted() {
            if (!_cancel && RunStarted != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { RunStarted(this, new RunResultEventArgs(_runResult)); }, null);
        }

        /// <summary>
        ///     For monitoring --> to know the time offset of the counters so a range can be linked to a run.
        /// </summary>
        private void SetRunStopped() {
            _runResult.StoppedAt = DateTime.Now;
            InvokeMessage("|----> |Run Finished in " + (_runResult.StoppedAt - _runResult.StartedAt) + "!", Color.MediumPurple);

            if (!_cancel && RunStopped != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { RunStopped(this, new RunResultEventArgs(_runResult)); }, null);
        }

        /// <summary>
        ///     For run sync
        /// </summary>
        /// <param name="concurrentUsersIndex"></param>
        /// <param name="run"></param>
        private void SetRunInitializedFirstTime(int concurrentUsersIndex, int run) {
            InvokeMessage(string.Format("|----> |Run {0}...", run), Color.MediumPurple);

            int concurrentUsers = _stressTest.Concurrencies[concurrentUsersIndex];

            _runResult = new RunResult(concurrentUsersIndex, run, concurrentUsers);
            for (int user = 0; user != concurrentUsers; user++) {
                //Find the right scenario based on the weight given in the stress test view config.
                float percentage = ((float)(user + 1)) / concurrentUsers;

                Scenario scenario = null;
                float previousValue = 0f;
                foreach (var kvp in _requests) {
                    Scenario candidate = kvp.Key;
                    if (percentage > previousValue && percentage <= kvp.Value.Value)
                        scenario = candidate;
                    previousValue = kvp.Value.Value;
                }

                _runResult.VirtualUserResults[user] = new VirtualUserResult(_testPatternsAndDelaysGenerators[scenario].PatternLength);
            }

            _concurrencyResult.RunResults.Add(_runResult);

            if (!_cancel && RunInitializedFirstTime != null) {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { RunInitializedFirstTime(this, new RunResultEventArgs(_runResult)); }, null);
            }
        }

        /// <summary>
        ///     For run sync (break on first and last finished)
        ///     Returns true if run done once event invoked
        /// </summary>
        private bool SetRunDoneOnce() {
            if (!_runDoneOnce) {
                _runDoneOnce = true;
                if (!_cancel && RunDoneOnce != null) {
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate { RunDoneOnce(this, new RunResultEventArgs(_runResult)); }, null);
                }
                return true;
            }
            return false;
        }

        public void SetRerunStarted() {
            if (!_cancel && RerunStarted != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { RerunStarted(this, new RunResultEventArgs(_runResult)); }, null);
        }
        /// <summary>
        ///     For run sync (break on last finished)
        ///     Do not use this in combination with run done once.
        /// </summary>
        private void SetRerunDone() {
            if (!_cancel && RerunDone != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { RerunDone(this, new RunResultEventArgs(_runResult)); }, null);
        }

        private void _threadPool_ThreadWorkException(object sender, MessageEventArgs e) { InvokeMessage(e.Message, e.Color, e.Level); }

        private void InvokeMessage(string message, Level logLevel = Level.Info) { InvokeMessage(message, Color.Empty, logLevel); }
        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color">can be Color.Empty</param>
        /// <param name="logLevel"></param>
        private void InvokeMessage(string message, Color color, Level logLevel = Level.Info) {
            try {
                if (Message != null)
                    SynchronizationContextWrapper.SynchronizationContext.Send(
                        delegate {
                            try {
                                Message(this, new MessageEventArgs(message, color, logLevel));
                            }
                            catch (Exception ex) {
                                Debug.WriteLine("Failed invoking message: " + message + " at log level: " + logLevel + ".\n" + ex);
                            }
                        }, null);

                Loggers.Log(logLevel, message);
            }
            catch (Exception ex) {
                Debug.WriteLine("Failed invoking message: " + message + " at log level: " + logLevel + ".\n" + ex);
            }
        }
        #endregion

        #region Init
        /// <summary>
        ///     Do this before everything else.
        /// </summary>
        public void InitializeTest() {
            Exception ex = null;
            try {
                InvokeMessage("Initializing the Test.");
                InitializeScenarios();
                if (_cancel) return;
                InitializeConnectionProxyPool();
                if (_cancel) return;
            }
            catch (Exception e) {
                _isFailed = true;
                ex = e;
            }
            if (TestInitialized != null) TestInitialized(this, new TestInitializedEventArgs(ex));
        }

        /// <summary>
        ///     Will apply the scenario rule set.
        ///     Parallel connections will be determined here also.
        /// </summary>
        private void InitializeScenarios() {
            if (_cancel) return;

            InvokeMessage("Initializing Scenario(s)...");

            if (_stressTest.Scenarios[0].Key.ScenarioRuleSet.IsEmpty) {
                var ex = new Exception("No rule set has been assigned to the selected scenario(s).");
                Loggers.Log(Level.Error, ex.ToString());
                throw ex;
            }

            _scenariosStub = new Scenarios();
            foreach (var kvp in _stressTest.Scenarios) {
                if (_cancel) return;

                if (kvp.Value != 0 && kvp.Key.Count == 0) {
                    var ex = new Exception("There are no user actions in a selected scenario(s).");
                    Loggers.Log(Level.Error, ex.ToString());
                    throw ex;
                }

                if (kvp.Key.GetParent() == null) _scenariosStub.Add(kvp.Key); // Only the case when distributed testing.
            }


            var allWeights = new List<float>();//To determine if all scenarios will be used in the test --> if the concurrency is big enough.
            float totalScenarioWeight = 0; //To calculate the percentage distribution
            var scenariosSortedByWeight = new List<KeyValuePair<Scenario, uint>>(); //for easy determining incremental percentages
            foreach (var kvp in _stressTest.Scenarios) {
                if (_cancel) return;

                float weight = Convert.ToSingle(kvp.Value);
                if (weight > 0) allWeights.Add(weight);

                totalScenarioWeight += weight;

                bool added = false;
                for (int i = 0; i != scenariosSortedByWeight.Count; i++) {
                    if (_cancel) return;

                    if (scenariosSortedByWeight[i].Value > kvp.Value) {
                        scenariosSortedByWeight.Insert(i, kvp);
                        added = true;
                        break;
                    }
                }
                if (!added)
                    scenariosSortedByWeight.Add(kvp);
            }

            if (totalScenarioWeight == 0) {
                var ex = new Exception("At least one scenario should have a weight greater then 0.");
                Loggers.Log(Level.Error, ex.ToString());
                throw ex;
            }

            //Find the greatest common divisor tot determine the smallest allowed concurrency.
            float smallestAllowedConcurrency = totalScenarioWeight / GCD.Get(allWeights.ToArray());

            foreach (int concurrency in _stressTest.Concurrencies)
                if (Convert.ToSingle(concurrency) < smallestAllowedConcurrency) {
                    var ex = new Exception("Make sure that all scenarios can be used by all concurrencies.\nThe smallest concurrency must be greater than or equal to the total scenario wheight divided by the greatest common divisor of the weights: " + smallestAllowedConcurrency + ".");
                    Loggers.Log(Level.Error, ex.ToString());
                    throw ex;
                }

            uint incrementedWeight = 0;
            var scenarios = new List<KeyValuePair<Scenario, KeyValuePair<Request[], float>>>();
            _testPatternsAndDelaysGenerators = new Dictionary<Scenario, TestPatternsAndDelaysGenerator>();

            _parallelConnections = 0;
            _parallelThreads = 0;
            foreach (var kvp in scenariosSortedByWeight) {
                if (_cancel) return;

                if (kvp.Value != 0) {
                    Scenario scenario = ScenarioTimesOccurancies(kvp.Key);

                    if (_cancel) return;

                    //Parallel connections, check per user action. Execute with previous is set correctly according to the stress test settings.
                    var connectionsPerHostname = new Dictionary<string, int>();
                    int parallelConnections = 0;

                    var l = new List<Request>();

                    foreach (UserAction ua in scenario) {

                        int validateRequestIndex = 0; //Only from the third request. After that the second after the previous parallel group.
                        foreach (Request request in ua) {
                            if (_cancel) return;

                            if (_stressTest.UseParallelExecutionOfRequests)
                                ++_parallelThreads;

                            l.Add(request);

                            request.ExecuteInParallelWithPrevious = false;
                            if (request.Redirects) {
                                continue;
                            }

                            if (validateRequestIndex > 1 && _stressTest.UseParallelExecutionOfRequests && (_stressTest.MaximumPersistentConnections == 0 || parallelConnections <= _stressTest.MaximumPersistentConnections)) {

                                if (_stressTest.PersistentConnectionsPerHostname == 0) {
                                    request.ExecuteInParallelWithPrevious = true;
                                    ++parallelConnections;
                                }
                                else {
                                    if (string.IsNullOrWhiteSpace(request.Hostname))
                                        throw new Exception("You try to run a a test with parallel connections (specialized web test setting) but a hostname to be able to parallize correctly was not found in one or more requests.\nIf you want to use parallel connections, please make sure a hostname is present in all requests and the index of that field is defined in the Scenario Rule Set of the given scenario (double-click on it).\nWhile you are at it, check the parallel offset in ms index field and the redirects index field.\nOtherwise uncheck this feature in the advanced settings of this stress test and press 'play' again.");
                                    if (!connectionsPerHostname.ContainsKey(request.Hostname)) connectionsPerHostname.Add(request.Hostname, 0);

                                    connectionsPerHostname[request.Hostname] += 1;

                                    if (connectionsPerHostname[request.Hostname] < _stressTest.PersistentConnectionsPerHostname) {
                                        request.ExecuteInParallelWithPrevious = true;
                                        ++parallelConnections;
                                    }
                                    else {
                                        connectionsPerHostname[request.Hostname] = 0;
                                        validateRequestIndex = 1;
                                    }

                                }
                            }

                            ++validateRequestIndex;
                        }

                        if (_stressTest.UseParallelExecutionOfRequests)
                            _parallelConnections += parallelConnections;

                        parallelConnections = 0;

                        //Cps and threads are dequeued when used. Make sure that we have enough.
                        connectionsPerHostname.Clear();
                    }

                    var requestArr = l.ToArray();
                    l = null;

                    incrementedWeight += kvp.Value;

                    scenarios.Add(new KeyValuePair<Scenario, KeyValuePair<Request[], float>>(scenario, new KeyValuePair<Request[], float>(requestArr, Convert.ToSingle(incrementedWeight) / totalScenarioWeight)));

                    int actionCount = _stressTest.MaximumNumberOfUserActions == 0 ? scenario.Count : _stressTest.MaximumNumberOfUserActions;
                    var testPatternsAndDelaysGenerators = new TestPatternsAndDelaysGenerator(requestArr, actionCount, _stressTest.Shuffle, _stressTest.InitialMinimumDelay, _stressTest.InitialMaximumDelay, _stressTest.MinimumDelay, _stressTest.MaximumDelay);
                    _testPatternsAndDelaysGenerators.Add(scenario, testPatternsAndDelaysGenerators);
                }
            }

            _requests = scenarios.ToArray();

            scenarios = null;
            scenariosSortedByWeight = null;

            GC.WaitForPendingFinalizers();
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
        }
        /// <summary>
        ///     Expands the scenario (into a new one) times the occurance.
        ///     This also applies the scenario rule set.
        /// </summary>
        /// <returns></returns>
        private Scenario ScenarioTimesOccurancies(Scenario scenario) {
            //Check if following logic is needed, if not return immediatly.
            bool doScenarioTimesOccurancies = false;
            if (_stressTest.ActionDistribution)
                foreach (UserAction action in scenario)
                    if (action.Occurance != 1) {
                        doScenarioTimesOccurancies = true;
                        break;
                    }

            if (!doScenarioTimesOccurancies) {
                scenario.ApplyScenarioRuleSet();
                return scenario;
            }

            var newScenario = scenario.Clone(false, false, false, false);
            newScenario.SetTag(scenario.Index); //Workaround for the wrong numbering in the user action in requestresults.
            var linkCloned = new Dictionary<UserAction, UserAction>(); //To add the right user actions to the link.
            foreach (UserAction action in scenario) {
                if (_cancel) return null;

                var firstRequestClones = new List<Request>(); //This is a complicated structure to be able to get averages when using action distribution.
                for (int i = 0; i != action.Occurance; i++) {
                    if (_cancel) return null;

                    var actionClone = new UserAction(action.Label);
                    actionClone.Occurance = 1; //Must be one now, this value doesn't matter anymore.
                    actionClone.Pinned = action.Pinned;

                    bool canAddClones = firstRequestClones.Count == 0;
                    int requestIndex = 0;
                    foreach (Request child in action) {
                        if (_cancel) return null;

                        Request childClone = child.Clone(scenario.ScenarioRuleSet, true, false);

                        if (canAddClones)
                            firstRequestClones.Add(childClone);
                        else
                            childClone.SameAs = firstRequestClones[requestIndex];

                        actionClone.AddWithoutInvokingEvent(childClone);
                        ++requestIndex;
                    }

                    newScenario.AddWithoutInvokingEvent(actionClone);

                    if (action.LinkedToUserActionIndices.Count != 0 && !linkCloned.ContainsKey(action)) {
                        linkCloned.Add(action, actionClone);
                    }
                    else if (linkCloned.Count != 0) { //We can avoid the looping logic until a linkUserAction is found.
                        UserAction linkUserAction;
                        if (action.IsLinked(scenario, out linkUserAction) && linkCloned.ContainsKey(linkUserAction))
                            linkCloned[linkUserAction].LinkedToUserActionIndices.Add(newScenario.Count);
                    }
                }
            }
            return newScenario;
        }

        private void InitializeConnectionProxyPool() {
            if (_cancel) return;

            InvokeMessage("Initialize Connection Proxy Pool...");

            _connectionProxyPool = new ConnectionProxyPool(_stressTest.Connection);
            CompilerResults compilerResults = _connectionProxyPool.CompileConnectionProxyClass(true);
            if (compilerResults.Errors.HasErrors) {
                var sb = new StringBuilder("Failed at compiling the connection proxy class:");
                sb.AppendLine();
                foreach (CompilerError compileError in compilerResults.Errors) {
                    sb.AppendFormat("Error number {0}, Line {1}, Column {2}: {3}", compileError.ErrorNumber, compileError.Line, compileError.Column, compileError.ErrorText);
                    sb.AppendLine();
                }
                _connectionProxyPool.Dispose();
                _connectionProxyPool = null;
                var ex = new Exception(sb.ToString());
                Loggers.Log(Level.Error, ex.ToString());
                // This handles notification to the user.
                throw ex;
            }

            InvokeMessage("|->Testing the connection to the server application...");
            string error = _connectionProxyPool.TestConnection();

            if (error == null) {
                InvokeMessage("|-> ...Success!");
            }
            if (error != null) {
                _connectionProxyPool.Dispose();
                _connectionProxyPool = null;
                var ex = new Exception("Failed at testing the connection!\n" + error);
                throw ex;
            }
        }

        //Following init functions happen right before a run starts (ExecuteStressTest).
        private void DetermineTestableRequestsAndDelays(int concurrentUsers) {
            try {
                InvokeMessage(string.Format("       |Determining Test Patterns and Delays for {0} Concurrent Users...", concurrentUsers));

                var testableRequests = new ConcurrentDictionary<int, TestableRequest[]>(); //Keep the user to preserve the order.
                var initialDelaysInMilliseconds = new List<int>();
                var delaysInMilliseconds = new List<int[]>();

                //Get parameterized structures and patterns first sync first.
                var parameterizedStructures = new ConcurrentDictionary<int, Util.StringTree[]>();
                var testPatterns = new ConcurrentDictionary<int, int[]>();
                var parameterLessStructures = new Dictionary<Scenario, Util.StringTree[]>();
                for (int user = 0; user != concurrentUsers; user++) {
                    if (_cancel) return;

                    //Find the right scenario based on the weight given in the stress test view config.
                    float percentage = ((float)(user + 1)) / concurrentUsers;

                    Scenario scenario = null;
                    float previousValue = 0f;
                    foreach (var kvp in _requests) {
                        Scenario candidate = kvp.Key;
                        if (percentage > previousValue && percentage <= kvp.Value.Value)
                            scenario = candidate;
                        previousValue = kvp.Value.Value;
                    }

                    Util.StringTree[] structure = null;
                    if (parameterLessStructures.ContainsKey(scenario)) {
                        structure = parameterLessStructures[scenario];
                    }
                    else {
                        bool hasParameters;
                        structure = scenario.GetParameterizedStructure(out hasParameters);
                        if (!hasParameters) parameterLessStructures.Add(scenario, structure);
                    }
                    parameterizedStructures.TryAdd(user, structure);

                    int initialDelayInMilliseconds;
                    int[] testPattern, delayPattern;
                    _testPatternsAndDelaysGenerators[scenario].GetPatterns(out testPattern, out initialDelayInMilliseconds, out delayPattern);

                    testPatterns.TryAdd(user, testPattern);
                    initialDelaysInMilliseconds.Add(initialDelayInMilliseconds);
                    delaysInMilliseconds.Add(delayPattern);

                    testableRequests.TryAdd(user, null);
                }

                //Get all this stuff here, otherwise locking will slow down all following code.
                var requestIndices = new ConcurrentDictionary<Request, string>();
                var userActionStrings = new ConcurrentDictionary<Request, string>();
                string dot = ".", empty = " ", colon = ": ";

                foreach (var kvp in _requests) {
                    Scenario scenario = kvp.Key;
                    int scenarioIndex = scenario.GetTag() == null ? scenario.Index : (int)scenario.GetTag();
                    string scenarioString = scenario.Label == string.Empty ? string.Join(" ", scenario.Name, scenarioIndex) : string.Join(": ", string.Join(" ", scenario.Name, scenarioIndex), scenario.Label); //Previously worked around the otherwise 'wrong' scenario index in the tostring. 

                    Parallel.For(0, scenario.Count, (userActionIndex, loopState) => {
                        if (_cancel) loopState.Break();

                        var userAction = scenario[userActionIndex] as UserAction;
                        if (!userAction.IsEmpty) {
                            int oneBasedUserActionIndex = userActionIndex + 1;
                            string userActionString = string.Join(empty, scenarioString, userAction.Name, oneBasedUserActionIndex);
                            if (userAction.Label != string.Empty)
                                userActionString = string.Join(colon, userActionString, userAction.Label);

                            Parallel.For(0, userAction.Count, (requestIndex, loopState2) => {
                                if (_cancel) loopState2.Break();

                                var request = userAction[requestIndex] as Request;

                                requestIndices.TryAdd(request, string.Join(dot, scenarioIndex, oneBasedUserActionIndex, requestIndex + 1));
                                userActionStrings.TryAdd(request, userActionString);
                            });
                        }
                    });
                }
                if (_cancel) return;

                //Get testable requests async.
                Parallel.For(0, concurrentUsers, (user, loopState) => {
                    try {
                        if (_cancel) loopState.Break();

                        Util.StringTree[] parameterizedStructureArr;
                        parameterizedStructures.TryGetValue(user, out parameterizedStructureArr);

                        int[] testPattern;
                        testPatterns.TryGetValue(user, out testPattern);


                        //Find the right scenario based on the weight given in the stress test view config.
                        float percentage = ((float)(user + 1)) / concurrentUsers;

                        Request[] requestsPart = null;
                        float previousValue = 0f;
                        for (int kvp = 0; kvp != _requests.Length; kvp++) {
                            Scenario candidate = _requests[kvp].Key;
                            var candidateValue = _requests[kvp].Value;
                            if (percentage > previousValue && percentage <= candidateValue.Value) {
                                requestsPart = candidateValue.Key;
                            }
                            previousValue = candidateValue.Value;
                        }

                        var tle = new TestableRequest[testPattern.Length];

                        //Get the requests/parameterized structures using the test pattern indices (can be shuffled)
                        Parallel.For(0, testPattern.Length, (i, loopState2) => {
                            try {
                                if (_cancel) loopState2.Break();

                                int testPatternIndex = testPattern[i];
                                var request = requestsPart[testPatternIndex];

                                string requestIndex;
                                requestIndices.TryGetValue(request, out requestIndex);

                                string sameAsRequestIndex = string.Empty;
                                if (request.SameAs != null)
                                    requestIndices.TryGetValue(request.SameAs, out sameAsRequestIndex);

                                string userActionString;
                                userActionStrings.TryGetValue(request, out userActionString);

                                tle[i] = new TestableRequest(requestIndex, sameAsRequestIndex, parameterizedStructureArr[testPatternIndex], userActionString, request.ExecuteInParallelWithPrevious, request.ParallelOffsetInMs, _rerun);
                            }
                            catch (Exception ex2) {
                                Loggers.Log(Level.Error, "Failed at determining test patterns.", ex2);
                                loopState.Break();
                            }
                        });

                        testableRequests.TryUpdate(user, tle, null);
                    }
                    catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed at determining test patterns.", ex);
                        loopState.Break();
                    }
                });
                if (_cancel) return;

                //Box to list.
                var tleList = new List<TestableRequest[]>(testableRequests.Count);
                foreach (var tle in testableRequests.Values)
                    tleList.Add(tle);

                _testableRequests = tleList.ToArray();
                _initialDelaysInMilliseconds = initialDelaysInMilliseconds.ToArray();
                _delaysInMilliseconds = delaysInMilliseconds.ToArray();

                testableRequests = null;
                initialDelaysInMilliseconds = null;
                delaysInMilliseconds = null;

                parameterizedStructures = null;
                testPatterns = null;

                requestIndices = null;
                userActionStrings = null;

                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
            }
            catch {
                if (!_cancel)
                    throw;
            }
        }
        private void SetThreadPool(int concurrentUsers) {
            if (_cancel) return;

            InvokeMessage(string.Format("       |Setting Thread Pool for {0} Concurrent Users...", concurrentUsers));
            if (_threadPool == null) {
                _threadPool = new StressTestThreadPool(Work);
                _threadPool.ThreadWorkException += _threadPool_ThreadWorkException;
            }

            _threadPool.SetThreads(concurrentUsers, _parallelThreads * concurrentUsers);
        }
        private void SetConnectionProxyPool(int concurrentUsers) {
            if (_cancel) return;

            InvokeMessage(string.Format("       |Setting Connections for {0} Concurrent Users...", concurrentUsers));
            try {
                _connectionProxyPool.SetAndConnectConnectionProxies(concurrentUsers, _parallelConnections * concurrentUsers);
            }
            catch {
                throw;
            }
        }
        #endregion

        #region Work
        public StressTestStatus ExecuteStressTest() {
            //No run started yet.
            _continueCounter = -1;
            SetStressTestStarted();
            //Loop all concurrent users
            for (int concurrentUsersIndex = 0; concurrentUsersIndex != _stressTest.Concurrencies.Length; concurrentUsersIndex++) {
                int concurrentUsers = _stressTest.Concurrencies[concurrentUsersIndex];
                _concurrencyId = concurrentUsersIndex;

                if (_cancel) break;

                SetConcurrencyStarted(concurrentUsersIndex);
                //Loop 'run' times for the concurrent users.
                for (int run = 0; run != _stressTest.Runs; run++) {
                    if (_cancel) break;
                    _runDoneOnce = false;
                    _rerun = 0;

                    Rerun:
                    //Initialize all for a new test.
                    if (_cancel) break;
                    _valueStore.InitForTestRun();
                    //Initialize the requests and delays (every time so the tests for different runs are not the same)
                    DetermineTestableRequestsAndDelays(concurrentUsers);
                    if (_cancel) break;
                    SetThreadPool(concurrentUsers);
                    if (_cancel) break;
                    //Initialize the connection proxy pool (every time, so dead or locked connections can't be used anymore)
                    SetConnectionProxyPool(concurrentUsers);
                    if (_cancel) break;

                    if (_runDoneOnce) {
                        InvokeMessage(string.Format("|----> | Rerunning Run {0}...", run + 1), Color.LightPink);
                    }
                    else {
                        ++_continueCounter;

                        //For many-to-one testing, keeping the run shared by divided tile stress test in sync.
                        SetRunInitializedFirstTime(concurrentUsersIndex, run + 1);
                        if (_stressTest.IsDividedStressTest && !_cancel) {
                            InvokeMessage("[Many to One Waithandle before run] Waiting for Continue Message from Master...");
                            _manyToOneWaitHandle.WaitOne();
                            InvokeMessage("Continuing...");
                        }
                        //Wait here untill the master sends continue when using run sync.
                        if ((RunSynchronization != RunSynchronization.None || _waitWhenInitializedTheFirstRun) && !_cancel) { //For distributed testing.
                            _waitWhenInitializedTheFirstRun = false;
                            InvokeMessage("[Run Sync Waithandle before run] Waiting for Continue Message from Master...");
                            _runSynchronizationContinueWaitHandle.WaitOne();
                            InvokeMessage("Continuing...");
                        }

                        //Set this run started, used for for instance monitoring --> the range for this run can be determined this way.
                        SetRunStarted();
                    }

                    if (_cancel) break;

                    //Do the actual work and invoke when the run is finished.
                    try {
                        _threadPool.DoWorkAndWaitForIdle();
                    }
                    catch (Exception ex) {
                        if (!_isDisposed)
                            InvokeMessage("|----> |Run Not Finished Succesfully!\n|Thread Pool Exception:\n" + ex, Color.Red, Level.Error);
                    }

                    //For many-to-one testing, keeping the run shared by divided tile stress test in sync.
                    if (RunSynchronization == RunSynchronization.None) {
                        if (_stressTest.IsDividedStressTest) {
                            SetRunDoneOnce();
                            SetRunStopped();

                            InvokeMessage("[Many to One Waithandle after run] Waiting for Continue Message from Master...");
                            _manyToOneWaitHandle.WaitOne();
                            InvokeMessage("Continuing...");
                        }
                        else {
                            SetRunStopped();
                        }
                    }
                    //Wait here when the run is broken untill the master sends continue when using run sync.
                    else if (RunSynchronization == RunSynchronization.BreakOnFirstFinished) {
                        ++_continueCounter;
                        SetRunDoneOnce();
                        SetRunStopped();

                        if (_stressTest.IsDividedStressTest) {
                            InvokeMessage("[Many to One Waithandle after run] Waiting for Continue Message from Master...");
                            _manyToOneWaitHandle.WaitOne();
                            InvokeMessage("Continuing...");
                        }

                        InvokeMessage("[Run Sync Waithandle after run] Waiting for Continue Message from Master...");
                        _runSynchronizationContinueWaitHandle.WaitOne();
                        InvokeMessage("Continuing...");
                    }
                    //Rerun untill the master sends a break. This is better than recurions --> no stack overflows.
                    //Also breaks after the rerun count == _maxRerunsBreakOnLast.
                    else if (RunSynchronization == RunSynchronization.BreakOnLastFinished && !_break) {
                        if (!SetRunDoneOnce())
                            SetRerunDone();
                        ++_rerun;

                        //Allow one last rerun, then wait for the master for a continue, or rerun nfinite.
                        if (MaxRerunsBreakOnLast == 0 || _rerun <= MaxRerunsBreakOnLast) {
                            InvokeMessage("Initializing Rerun...");
                            //Increase resultset
                            _runResult.PrepareForRerun();
                            SetRerunStarted();
                            goto Rerun;
                        }
                        else {
                            SetRunStopped();
                            if (_stressTest.IsDividedStressTest) {
                                InvokeMessage("[Many to One Waithandle after run] Waiting for Continue Message from Master...");
                                _manyToOneWaitHandle.WaitOne();
                                InvokeMessage("Continuing...");
                            }
                        }
                    }
                    else {
                        SetRunStopped();
                    }
                }
                SetConcurrencyStopped();
            }

            return Completed();
        }


        /// <summary>
        /// Break the current executing run. (Distributed testing)
        /// </summary>
        public void Break() {
            if (!_break && !(_completed | _cancel | _isFailed)) {
                _break = true;
                InvokeMessage("Breaking the execution for the current run...");

                _sleepWaitHandle.Set();
            }
        }
        /// <summary>
        /// Continue to newly inited or the next run. (Distributed Testing)
        /// </summary>
        /// <param name="continueCounter">Every time the execution is paused the continue counter is incremented by one.</param>
        public void Continue(int continueCounter) {
            if (_continueCounter == continueCounter && !(_completed | _cancel | _isFailed)) {
                _break = false;
                _runSynchronizationContinueWaitHandle.Set();
            }
        }

        /// <summary>
        /// Keeping the shared run for a divided tile stress test in sync.
        /// </summary>
        public void ContinueDivided() {
            if (!(_completed | _cancel | _isFailed))
                _manyToOneWaitHandle.Set();
        }

        /// <summary>
        /// WorkItemCallback in StressTestCore, executes all requests for the current running thread. Has some exception handling.
        /// </summary>
        /// <param name="threadIndex"></param>
        public void Work(int threadIndex) {
            try {
                IConnectionProxy connectionProxy = _connectionProxyPool[threadIndex];
                if (!connectionProxy.IsConnectionOpen) connectionProxy.OpenConnection();

                TestableRequest[] testableRequests = _testableRequests[threadIndex];
                int initialDelayInMilliseconds = _initialDelaysInMilliseconds[threadIndex];
                int[] delaysInMilliseconds = _delaysInMilliseconds[threadIndex];

                int testableRequestIndex = 0;
                while (testableRequestIndex != testableRequests.Length) {
                    if (_cancel || _break) {
                        _sleepWaitHandle.Set();
                        return;
                    }

                    int incrementIndex;
                    ExecuteRequest(threadIndex, testableRequestIndex, connectionProxy, testableRequests, testableRequestIndex == 0 ? initialDelayInMilliseconds : 0, delaysInMilliseconds, out incrementIndex);
                    testableRequestIndex += incrementIndex;
                }
            }
            catch (Exception e) {
                if (!_cancel && !_break) InvokeMessage("Work failed for " + Thread.CurrentThread.Name + ".\n" + e, Color.Red, Level.Error);
            }
        }

        /// <summary>
        ///     Decides if the request must be executed in parallel with others or not.
        /// </summary>
        /// <param name="threadIndex"></param>
        /// <param name="testableRequestIndex"></param>
        /// <param name="incrementIndex">Can be greater than 1 if some are parallel executed.</param>
        private void ExecuteRequest(int threadIndex, int testableRequestIndex, IConnectionProxy connectionProxy, TestableRequest[] testableRequests, int initialDelayInMilliseconds, int[] delaysInMilliseconds, out int incrementIndex) {
            incrementIndex = 1;

            if (_cancel || _break) {
                _sleepWaitHandle.Set();
                return;
            }

            TestableRequest testableRequest = testableRequests[testableRequestIndex];

            //Check if this request could be the first of a parallel executed range of entries
            //If so the range will be executed multi threaded
            if (!testableRequest.ExecuteInParallelWithPrevious) {
                int testableRequestsLength = 0, exclusiveEnd = 0;

                // Make the range if following is true.
                if (_stressTest.UseParallelExecutionOfRequests) {
                    testableRequestsLength = testableRequests.Length;
                    exclusiveEnd = testableRequestIndex + 1;
                    //Do not go out of bounds
                    if (exclusiveEnd != testableRequestsLength)
                        while (testableRequests[exclusiveEnd].ExecuteInParallelWithPrevious) {
                            ++exclusiveEnd;
                            ++incrementIndex;
                            if (exclusiveEnd == testableRequestsLength)
                                break;
                        }
                }

                //Do work parallel if needed.
                if (incrementIndex == 1) {
                    if (_syncAndAsyncWorkItem == null)
                        _syncAndAsyncWorkItem = new SyncAndAsyncWorkItem();

                    //_sleepWaitHandle can be given here without a problem, the Set and Wait functions are thread specific. 
                    var result = _syncAndAsyncWorkItem.ExecuteRequest(this, _sleepWaitHandle, _runResult, threadIndex, testableRequestIndex, testableRequest, connectionProxy, initialDelayInMilliseconds, delaysInMilliseconds[testableRequestIndex]);

                    FireOnRequestResult(result);
                }
                else {
                    int parallelCount = exclusiveEnd - testableRequestIndex;
                    //Get the connection proxies, this first one is not a parallel one but the connection proxy for the specific user, this way data (cookies for example) can be saved for other requests.
                    IConnectionProxy[] parallelConnectionProxies = new IConnectionProxy[parallelCount];
                    parallelConnectionProxies[0] = connectionProxy;

                    for (int i = 1; i != parallelCount; i++) {
                        parallelConnectionProxies[i] = _connectionProxyPool.DequeueParallelConnectionProxy();
                    }

                    //Make a mini thread pool (Thread pools in thread pools, what it this madness?! :p)

                    var pThreadsSignalStart = new ManualResetEvent(false);
                    var pThreadsSignalFinished = new AutoResetEvent(false);

                    int pcpIndex = -1;
                    int finished = 0;
                    int delay = 0; //Workaround for the delay after a user action.
                    var results = new ConcurrentDictionary<int, RequestResult>(); //Keep stuff in order.

                    object parentThread = Thread.CurrentThread.Name as object;

                    for (int i = 0; i != parallelCount; i++) {
                        _threadPool.DequeueParallelThread().Start(new object[]{ (StressTestThreadPool.WorkItemCallback)((int index) => {

                            Thread.CurrentThread.SetParent(parentThread);

                            if (_syncAndAsyncWorkItem == null) _syncAndAsyncWorkItem = new SyncAndAsyncWorkItem();

                            if (delaysInMilliseconds[index] > delay) delay = delaysInMilliseconds[index];

                            pThreadsSignalStart.WaitOne();

                            try {
                                //_sleepWaitHandle can be given here without a problem, the Set and Wait functions are thread specific. 
                              results.TryAdd(index, _syncAndAsyncWorkItem.ExecuteRequest(this, _sleepWaitHandle, _runResult, threadIndex, index, testableRequests[index], parallelConnectionProxies[Interlocked.Increment(ref pcpIndex)], 0, 0));
                            } catch {
                                //when stopping a test...
                            }

                            Thread.CurrentThread.RemoveParent();

                            if (Interlocked.Increment(ref finished) == parallelCount) pThreadsSignalFinished.Set();
                        }), testableRequestIndex + i});
                    }

                    //Start them all at the same time
                    pThreadsSignalStart.Set();
                    pThreadsSignalFinished.WaitOne();

                    if (delay != 0 && !(_cancel || _break)) {
                        _sleepWaitHandle.WaitOne(delay);

                        RequestResult resultWithDelay = null; //Only one result can store the delay in the database and fast results.
                        foreach (var result in results.Values) {
                            if (resultWithDelay == null || result.SentAt.AddTicks(result.TimeToLastByteInTicks) > resultWithDelay.SentAt.AddTicks(resultWithDelay.TimeToLastByteInTicks))
                                resultWithDelay = result;
                        }

                        resultWithDelay.DelayInMilliseconds = delay;
                    }

                    for (int i = testableRequestIndex; i != testableRequestIndex + results.Count; i++) FireOnRequestResult(results[i]);


                    pThreadsSignalFinished.Dispose();
                    pThreadsSignalStart.Dispose();
                    parallelConnectionProxies = null;
                }
            }
        }

        public void Cancel() {
            if (!_cancel) {
                _cancel = true;
                _sleepWaitHandle.Set();
                _runSynchronizationContinueWaitHandle.Set();
                _manyToOneWaitHandle.Set();
                if (_connectionProxyPool != null) _connectionProxyPool.ShutDown();
                DisposeThreadPool();
                if (_connectionProxyPool != null) _connectionProxyPool.Dispose();

                _connectionProxyPool = null;

            }
        }
        private StressTestStatus Completed() {
            _completed = true;
            if (_connectionProxyPool != null) _connectionProxyPool.ShutDown();
            DisposeThreadPool();
            if (_connectionProxyPool != null) _connectionProxyPool.Dispose();

            if (_cancel) {
                return StressTestStatus.Cancelled;
            }
            if (_isFailed) {
                return StressTestStatus.Error;
            }
            return StressTestStatus.Ok;
        }
        private void DisposeThreadPool() {
            if (_threadPool != null) try { _threadPool.Dispose(); } catch { }
            _threadPool = null;
        }
        #endregion

        public void Dispose() {
            if (!_isDisposed) {
                try {
                    _isDisposed = true;
                    Cancel();

                    _sleepWaitHandle.Close();
                    _sleepWaitHandle.Dispose();
                    _sleepWaitHandle = null;

                    _runSynchronizationContinueWaitHandle.Close();
                    _runSynchronizationContinueWaitHandle.Dispose();
                    _runSynchronizationContinueWaitHandle = null;

                    _manyToOneWaitHandle.Close();
                    _manyToOneWaitHandle.Dispose();
                    _manyToOneWaitHandle = null;

                    _requests = null;
                    _testableRequests = null;

                    if (_testPatternsAndDelaysGenerators != null) {
                        foreach (var gen in _testPatternsAndDelaysGenerators.Values)
                            gen.Dispose();
                        _testPatternsAndDelaysGenerators = null;
                    }


                    _stressTestResult = null;
                }
                catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed disposing the stress test core.", ex);
                }
            }
            ObjectRegistrar.Unregister(this);

            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        #endregion

        /// <summary>
        ///     Request with metadata.
        /// </summary>
        private class TestableRequest {

            #region Fields
            /// <summary>
            ///     Should be scenario.IndexOf(UserAction) + "." + UserAction.IndexOf(Request); this must be unique.
            /// </summary>
            public readonly string RequestIndex;

            /// <summary>
            /// To be able to calculate averages when using distribute. Cannot be null, string.empty is allowed.
            /// </summary>
            public readonly string SameAsRequestIndex;

            /// <summary>
            /// Used in IConnectionProxy.SendAndReceive.
            /// </summary>
            public readonly Util.StringTree ParameterizedRequest;

            /// <summary>
            ///     Should be scenario.IndexOf(Request) or scenario.IndexOf(UserAction) + "." + UserAction.IndexOf(Request), this must be unique.
            /// </summary>
            public readonly string ParameterizedRequestString;

            /// <summary>
            ///     Cannot be null, string.empty is not allowed.
            /// </summary>
            public readonly string UserAction;

            /// <summary>
            ///     Execute in parallel with next siblings.
            /// </summary>
            public readonly bool ExecuteInParallelWithPrevious;

            /// <summary>
            ///     The offset in ms before this 'parallel request' is executed (this simulates what browsers do).
            /// </summary>
            public readonly int ParallelOffsetInMs;

            /// <summary>
            /// 0 for all but Break on last runs (Distributed Testing).
            /// </summary>
            public readonly int Rerun;
            #endregion

            #region Constructor
            /// <summary>
            ///     Request with metadata.
            /// </summary>
            /// <param name="requestIndex"> Should be scenario.IndexOf(UserAction) + "." + UserAction.IndexOf(Request); this must be unique.</param>
            /// <param name="sameAsRequestString">To be able to calculate averages when using distribute. Cannot be null, string.empty is allowed.</param>
            /// <param name="parameterizedRequest">Used in IConnectionProxy.SendAndReceive.</param>
            /// <param name="userAction">Cannot be null, string.empty is not allowed.</param>
            /// <param name="executeInParallelWithPrevious">Execute parallel with immediate previous sibling. Not used feature atm.</param>
            /// <param name="parallelOffsetInMs">The offset in ms before this 'parallel request' is executed (this simulates what browsers do).</param>
            /// <param name="rerun">0 for all but Break on last runs (Distributed Testing).</param>
            public TestableRequest(string requestIndex, string sameAsRequestString, Util.StringTree parameterizedRequest, string userAction, bool executeInParallelWithPrevious, int parallelOffsetInMs, int rerun) {
                RequestIndex = requestIndex;
                SameAsRequestIndex = sameAsRequestString;

                ParameterizedRequest = parameterizedRequest;
                ParameterizedRequestString = ParameterizedRequest.CombineValues();
                UserAction = userAction;
                ExecuteInParallelWithPrevious = executeInParallelWithPrevious;
                ParallelOffsetInMs = parallelOffsetInMs;

                Rerun = rerun;
            }
            #endregion
        }

        /// <summary>
        /// An extra work item class (ThreadStatic) for executing requests in parallel.
        /// </summary>
        private class SyncAndAsyncWorkItem {
            public RequestResult ExecuteRequest(StressTestCore stressTestCore, AutoResetEvent sleepWaitHandle, RunResult runResult, int threadIndex, int testableRequestIndex, TestableRequest testableRequest, IConnectionProxy connectionProxy, int initialDelayInMilliseconds, int delayInMilliseconds) {
                if (initialDelayInMilliseconds != 0 && !(stressTestCore._cancel || stressTestCore._break))
                    sleepWaitHandle.WaitOne(initialDelayInMilliseconds);

                if (testableRequest.ExecuteInParallelWithPrevious && testableRequest.ParallelOffsetInMs != 0 & !(stressTestCore._cancel || stressTestCore._break))
                    sleepWaitHandle.WaitOne(testableRequest.ParallelOffsetInMs);

                RequestResult requestResult = null;

                DateTime sentAt = DateTime.Now;
                var timeToLastByte = new TimeSpan();
                string meta = null;
                Exception exception = null;

                if (stressTestCore._cancel || stressTestCore._break) {
                    sleepWaitHandle.Set();
                    return requestResult;
                }

                //Sleep the offset of parallel requests.
                if (testableRequest.ExecuteInParallelWithPrevious) Thread.Sleep(testableRequest.ParallelOffsetInMs);

                bool retried = false;
                RetryOnce:
                try {
                    if (connectionProxy == null || connectionProxy.IsDisposed)
                        exception = new Exception("Connectionproxy is disposed. Metrics for this request (" + testableRequest.ParameterizedRequestString + ") are not correct.");
                    else
                        connectionProxy.SendAndReceive(testableRequest.ParameterizedRequest, out sentAt, out timeToLastByte, out meta, out exception);

                }
                catch (Exception ex) {
                    if (!retried && connectionProxy != null && !connectionProxy.IsDisposed) {
                        try {
                            retried = true;
                            connectionProxy.OpenConnection();
                            goto RetryOnce;
                        }
                        catch (Exception e) {
                            exception = new Exception("An error in the connection proxy has occured and is now disposed due to for instance a time out or a bug in the connection proxy code; I tried to reopen the connection so testing could continue for this simulated user including a retry (once) for the current request, but it failed; Metrics for this request (" +
                                    testableRequest.ParameterizedRequestString + ") are not correct:\n" + ex + "\n\nReconnect failure:\n" + e);
                        }
                    }
                    else {
                        try {
                            if (connectionProxy != null) connectionProxy.Dispose();
                        }
                        catch (Exception exc) {
                            Loggers.Log(Level.Warning, "Failed disposing connection proxy.", exc);
                        }
                        connectionProxy = null;
                        exception = new Exception("An error in the connection proxy has occured, after the second try, (and is now disposed) due to for instance a time out or a bug in the connection proxy code; Metrics for this request (" +
                                testableRequest.ParameterizedRequestString + ") are not correct:\n" + ex);
                    }
                    throw (exception);
                }
                finally {
                    VirtualUserResult result = runResult.VirtualUserResults[threadIndex];
                    if (result.VirtualUser == null)
                        result.VirtualUser = Thread.CurrentThread.Name;

                    requestResult = new RequestResult {
                        ConcurrencyId = stressTestCore._concurrencyId,
                        Concurrency = stressTestCore._concurrencyResult.Concurrency,
                        Run = stressTestCore._runResult.Run,
                        VirtualUser = result.VirtualUser,
                        RequestIndex = testableRequest.RequestIndex,
                        SameAsRequestIndex = testableRequest.SameAsRequestIndex,
                        Request = testableRequest.ParameterizedRequestString,
                        InParallelWithPrevious = testableRequest.ExecuteInParallelWithPrevious,
                        UserAction = testableRequest.UserAction,
                        SentAt = sentAt,
                        TimeToLastByteInTicks = timeToLastByte.Ticks,
                        Meta = (meta == null) ? string.Empty : meta,
                        DelayInMilliseconds = delayInMilliseconds,
                        Error = (exception == null) ? string.Empty : exception.ToString(),
                        Rerun = testableRequest.Rerun
                    };
                    result.SetRequestResultAt(testableRequestIndex, requestResult);

                    runResult.VirtualUserResults[threadIndex] = result;

                    if (delayInMilliseconds != 0 && !(stressTestCore._cancel || stressTestCore._break)) sleepWaitHandle.WaitOne(delayInMilliseconds);
                }

                return requestResult;
            }
        }

        /// <summary>
        /// Fire on a seperated thread. In order.
        /// </summary>
        /// <param name="requestResult"></param>
        internal void FireOnRequestResult(RequestResult requestResult) {
            if (OnRequest != null && BackgroundWorkQueueWrapper.BackgroundWorkQueue != null && !BackgroundWorkQueueWrapper.BackgroundWorkQueue.IsDisposed) {
                var publishItem = new RequestResults();
                publishItem.ConcurrencyId = requestResult.ConcurrencyId;
                publishItem.Concurrency = requestResult.Concurrency;
                publishItem.Run = requestResult.Run;
                publishItem.VirtualUser = requestResult.VirtualUser;
                publishItem.UserAction = requestResult.UserAction;
                publishItem.RequestIndex = requestResult.RequestIndex;
                publishItem.SameAsRequestIndex = requestResult.SameAsRequestIndex;
                publishItem.Request = requestResult.Request;
                publishItem.InParallelWithPrevious = requestResult.InParallelWithPrevious;
                publishItem.SentAtInTicksSinceEpochUtc = (long)(requestResult.SentAt - PublishItem.EpochUtc).Ticks;
                publishItem.TimeToLastByteInTicks = requestResult.TimeToLastByteInTicks;
                publishItem.Meta = requestResult.Meta;
                publishItem.DelayInMilliseconds = requestResult.DelayInMilliseconds;
                publishItem.Error = requestResult.Error;
                publishItem.Rerun = requestResult.Rerun;

                BackgroundWorkQueueWrapper.BackgroundWorkQueue.EnqueueWorkItem(OnRequest, this, new OnRequestEventArgs(publishItem));
            }
        }
    }
}