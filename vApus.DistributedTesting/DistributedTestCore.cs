/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public class DistributedTestCore : IDisposable
    {
        #region Fields
        private object _communicationLock = new object(),
            _testProgressMessagesLock = new object(),
            _usedTileStresstestsLock = new object(),
            _resultsLock = new object();

        private Stopwatch _sw = new Stopwatch();

        private DistributedTest _distributedTest;
        private MasterSideCommunicationHandler _masterCommunication = new MasterSideCommunicationHandler();
        private List<TileStresstest> _usedTileStresstests = new List<TileStresstest>();
        /// <summary>
        /// The messages pusht from the slaves.
        /// </summary>
        private Dictionary<TileStresstest, TestProgressMessage> _testProgressMessages = new Dictionary<TileStresstest, TestProgressMessage>();
        /// <summary>
        /// The paths to the results.
        /// </summary>
        private Dictionary<TileStresstest, string> _results = new Dictionary<TileStresstest, string>();
        private bool _isDisposed;

        private volatile string[] _ok = new string[] { };
        private volatile string[] _cancelled = new string[] { };
        private volatile string[] _failed = new string[] { };

        private volatile string[] _runInitialized = new string[] { };
        private volatile string[] _runDoneOnce = new string[] { };

        private int _resultsReceived;
        //Invoke only once
        private bool _finishedHandled;
        private bool _hasResults;

        /// <summary>
        /// The full result path where results are saved.
        /// </summary>
        private string _resultPath;
        #endregion

        #region Properties
        public IEnumerable<TileStresstest> UsedTileStresstests
        {
            get
            {
                foreach (TileStresstest tileStresstest in _usedTileStresstests)
                    yield return tileStresstest;
            }
        }
        /// <summary>
        /// Key= index of the tile stresstest. Value = pusht progress message from slave.
        /// </summary>
        public Dictionary<TileStresstest, TestProgressMessage> TestProgressMessages
        {
            get
            {
                lock (_testProgressMessagesLock)
                    return _testProgressMessages;
            }
        }
        /// <summary>
        /// The paths to the results.
        /// </summary>
        public Dictionary<TileStresstest, string> Results
        {
            get
            {
                lock (_resultsLock)
                    return _results;
            }
        }
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        public int OK
        {
            get { return _ok.Length; }
        }
        public int Cancelled
        {
            get { return _cancelled.Length; }
        }
        public int Failed
        {
            get { return _failed.Length; }
        }

        public int RunInitializedCount
        {
            get { return _runInitialized.Length; }
        }
        public int RunDoneOnceCount
        {
            get { return _runDoneOnce.Length; }
        }
        /// <summary>
        /// The stresstests that are not finished.
        /// </summary>
        public int Running
        {
            get { return _usedTileStresstests.Count - Finished; }
        }
        public int Finished
        {
            get { return OK + Cancelled + Failed; }
        }

        /// <summary>
        /// To check wheter you can close the distributed test view without a warning or not.
        /// </summary>
        public bool HasResults
        {
            get { return _hasResults; }
        }
        #endregion

        #region Con-/Destructor
        //Only one test can run at the same time, if this is called and another test (stresstest core or distributed test core) exists (not disposed) an argument out of range exception will be thrown.
        public DistributedTestCore(DistributedTest distributedTest)
        {
            StaticObjectServiceWrapper.ObjectService.MaxSuscribers = 1;
            StaticObjectServiceWrapper.ObjectService.Suscribe(this);

            _distributedTest = distributedTest;
            _masterCommunication.ListeningError += new EventHandler<ListeningErrorEventArgs>(_masterCommunication_ListeningError);
            _masterCommunication.OnTestProgressMessageReceived += new EventHandler<TestProgressMessageReceivedEventArgs>(_masterCommunication_OnTestProgressMessageReceived);

            //The path where results are stored.
            string subResultDir = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            _resultPath = Path.Combine(_distributedTest.ResultPath, subResultDir);
        }
        ~DistributedTestCore()
        {
            Dispose();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Connects + sends tests
        /// </summary>
        public void Initialize()
        {
            InvokeMessage("Initializing the Test.");

            _usedTileStresstests.Clear();

            lock (_testProgressMessagesLock)
                _testProgressMessages.Clear();
            _results.Clear();

            _ok = new string[] { };
            _cancelled = new string[] { };
            _failed = new string[] { };
            _runInitialized = new string[] { };
            _runDoneOnce = new string[] { };

            _finishedHandled = false;
            _hasResults = false;

            Connect();
            SendAndReceiveInitializeTest();
        }
        private void Connect()
        {
            InvokeMessage("Connecting slaves...");
            _sw.Start();

            foreach (BaseItem item in _distributedTest.Tiles)
            {
                Tile tile = item as Tile;
                if (tile.Use)
                    foreach (BaseItem childItem in tile)
                    {
                        TileStresstest tileStresstest = childItem as TileStresstest;
                        if (tileStresstest.Use)
                        {
                            Exception exception;
#warning Allow multiple slaves to be able to distribute work.
                            int processID;
                            _masterCommunication.ConnectSlave(tileStresstest.BasicTileStresstest.Slaves[0].IP, tileStresstest.BasicTileStresstest.Slaves[0].Port, out processID, out exception);
                            if (exception == null)
                            {
                                _usedTileStresstests.Add(tileStresstest);
                                lock (_testProgressMessagesLock)
                                    _testProgressMessages.Add(tileStresstest, new TestProgressMessage());
                                InvokeMessage(string.Format("|->Connected {0} - {1}", tileStresstest.Parent, tileStresstest));
                            }
                            else
                            {
                                Dispose();
                                Exception ex = new Exception(string.Format("Could not connect to one of the slaves ({0} - {1})!{2}{3}", tileStresstest.Parent, tileStresstest, Environment.NewLine, exception.ToString()));
                                InvokeMessage(ex.ToString(), LogLevel.Error);
                                throw ex;
                            }
                        }
                    }
            }
            if (_usedTileStresstests.Count == 0)
            {
                Exception ex = new Exception("Please use at least one test!");
                InvokeMessage(ex.ToString(), LogLevel.Warning);
                throw ex;
            }
            _sw.Stop();
            InvokeMessage(string.Format(" ...Connected slaves in {0}", _sw.Elapsed.ToLongFormattedString()));
            _sw.Reset();
        }
        private void SendAndReceiveInitializeTest()
        {
            InvokeMessage("Initializing test on slaves...");
            _sw.Start();
            Parallel.ForEach(_usedTileStresstests, delegate(TileStresstest tileStresstest)
            {
                Exception exception;
                _masterCommunication.InitializeTest(tileStresstest, _distributedTest.RunSynchronization, out exception);
                if (exception == null)
                {
                    InvokeMessage(string.Format("|->Initialized {0} - {1}", tileStresstest.Parent, tileStresstest));
                }
                else
                {
                    Dispose();
                    Exception ex = new Exception(string.Format("Could not initialize {0} - {1}.{2}{3}", tileStresstest.Parent, tileStresstest, Environment.NewLine, exception.ToString()));
                    InvokeMessage(ex.ToString(), LogLevel.Error);
                    throw ex;
                }
            });
            _sw.Stop();
            InvokeMessage(string.Format(" ...Test initialized in {0}", _sw.Elapsed.ToLongFormattedString()));
            _sw.Reset();
        }
        public void Start()
        {
            InvokeMessage("Starting the test...");
            Exception exception;
            _masterCommunication.StartTest(out exception);

            if (exception == null)
            {
                InvokeMessage(" ...Started!", Color.LightGreen);
            }
            else
            {
                Dispose();
                InvokeMessage(exception.ToString(), LogLevel.Error);
                throw exception;
            }

            if (exception != null)
            {
                Dispose();
                Exception ex = new Exception(string.Format("Could not start the Distributed Test.{0}{1}", Environment.NewLine, exception.ToString()));
                InvokeMessage(ex.ToString(), LogLevel.Error);
                throw ex;
            }
        }

        private void _masterCommunication_ListeningError(object sender, ListeningErrorEventArgs e)
        {
            lock (_communicationLock)
            {
                try
                {
                    if (!_isDisposed && Finished < _usedTileStresstests.Count)
                    {
#warning allow multiple slaves yadda yadda
                        foreach (TileStresstest tileStresstest in _usedTileStresstests)
                            if (tileStresstest.BasicTileStresstest.Slaves[0].IP == e.SlaveIP && tileStresstest.BasicTileStresstest.Slaves[0].Port == e.SlavePort)
                                _failed = AddUniqueToStringArray(_failed, tileStresstest.TileStresstestIndex);

                        InvokeOnListeningError(e);

                        if (RunInitializedCount == Running)
                        {
                            _runInitialized = new string[] { };
                            _masterCommunication.SendContinue();
                        }
                        else if (RunDoneOnceCount == Running)
                        {
                            _runDoneOnce = new string[] { };
                            _masterCommunication.SendContinue();
                        }
                        else if (_distributedTest.RunSynchronization ==
                            RunSynchronization.BreakOnFirstFinished && RunDoneOnceCount == Finished + 1)
                        {
                            _masterCommunication.SendBreak();
                        }


                        if (Finished == _usedTileStresstests.Count)
                            HandleFinished();
                    }
                }
                catch { }
            }
        }
        private string[] AddUniqueToStringArray(string[] arr, string item)
        {
            foreach (string s in arr)
                if (s == item)
                    return arr;

            string[] newArr = new string[arr.Length + 1];
            for (int index = 0; index != arr.Length; index++)
                newArr[index] = arr[index];
            newArr[arr.Length] = item;

            return newArr;
        }
        private void _masterCommunication_OnTestProgressMessageReceived(object sender, TestProgressMessageReceivedEventArgs e)
        {
            lock (_communicationLock)
            {
                try
                {
                    _hasResults = true;

                    var tpm = e.TestProgressMessage;
                    lock (_testProgressMessagesLock)
                        _testProgressMessages[GetTileStresstest(e.TestProgressMessage.TileStresstestIndex)] = tpm;

                    bool okCancelError = true;
                    switch (tpm.StresstestResult)
                    {
                        case StresstestResult.Busy:
                            okCancelError = false;
                            break;
                        case StresstestResult.Ok:
                            _ok = AddUniqueToStringArray(_ok, tpm.TileStresstestIndex);
                            break;
                        case StresstestResult.Cancelled:
                            _cancelled = AddUniqueToStringArray(_cancelled, tpm.TileStresstestIndex);
                            break;
                        case StresstestResult.Error:
                            _failed = AddUniqueToStringArray(_failed, tpm.TileStresstestIndex);
                            break;
                    }


                    //Check if it is needed to do anything
                    if (Running != 0)
                    {
                        //Thread this as stopped.
                        if (okCancelError)
                            _runDoneOnce = AddUniqueToStringArray(_runDoneOnce, tpm.TileStresstestIndex);

                        switch (_distributedTest.RunSynchronization)
                        {
                            case RunSynchronization.None:
                                break;

                            //Send Break, wait for all stopped, send continue, wait for all started, send continue
                            case RunSynchronization.BreakOnFirstFinished:
                                if (tpm.RunStateChange == RunStateChange.ToRunInitializedFirstTime)
                                {
                                    _runInitialized = AddUniqueToStringArray(_runInitialized, tpm.TileStresstestIndex);
                                    if (RunInitializedCount == Running)
                                    {
                                        _runInitialized = new string[] { };
                                        _masterCommunication.SendContinue();
                                    }
                                }
                                else if (tpm.RunStateChange == RunStateChange.ToRunDoneOnce)
                                {
                                    _runDoneOnce = AddUniqueToStringArray(_runDoneOnce, tpm.TileStresstestIndex);
                                    if (RunDoneOnceCount == _usedTileStresstests.Count)
                                    {
                                        _runDoneOnce = new string[] { };
                                        //Increment the index here to be able to continue to the next run.
                                        _masterCommunication.SendContinue();
                                    }
                                    else if (RunDoneOnceCount == 1)
                                    {
                                        //Break the tests that are still in the previous run
                                        _masterCommunication.SendBreak();
                                    }
                                }
                                break;

                            //Wait for all stopped, send continue, wait for all started, send continue
                            case RunSynchronization.BreakOnLastFinished:
                                if (tpm.RunStateChange == RunStateChange.ToRunInitializedFirstTime)
                                {
                                    _runInitialized = AddUniqueToStringArray(_runInitialized, tpm.TileStresstestIndex);
                                    if (RunInitializedCount == Running)
                                    {
                                        _runInitialized = new string[] { };
                                        _masterCommunication.SendContinue();
                                    }
                                }
                                else if (tpm.RunStateChange == RunStateChange.ToRunDoneOnce)
                                {
                                    _runDoneOnce = AddUniqueToStringArray(_runDoneOnce, tpm.TileStresstestIndex);
                                    if (RunDoneOnceCount == Running)
                                    {
                                        _runDoneOnce = new string[] { };
                                        _masterCommunication.SendBreak();
                                    }
                                }
                                break;
                        }
                    }
                    InvokeOnTestProgressMessageReceived(GetTileStresstest(tpm.TileStresstestIndex), tpm);

                    if (Finished == _usedTileStresstests.Count)
                        HandleFinished();
                }
                catch { }
            }
        }
        private TileStresstest GetTileStresstest(string tileStresstestIndex)
        {
            lock (_usedTileStresstestsLock)
            {
                foreach (TileStresstest ts in _usedTileStresstests)
                    if (ts.TileStresstestIndex == tileStresstestIndex)
                        return ts;
                return null;
            }
        }
        public void Stop()
        {
#warning Handle exception
            Exception ex;
            _masterCommunication.StopTest(out ex);
        }

        private void HandleFinished()
        {
            if (!_finishedHandled)
            {
                _finishedHandled = true;

                StaticObjectServiceWrapper.ObjectService.Unsuscribe(this);

                InvokeMessage("Getting results (it can take a minute or two until transmission begins)...");
                Exception exception = null;

                try
                {
                    if (!Directory.Exists(_distributedTest.ResultPath))
                        Directory.CreateDirectory(_distributedTest.ResultPath);
                    if (!Directory.Exists(_resultPath))
                        Directory.CreateDirectory(_resultPath);
                }
                catch (Exception ex) { exception = ex; }

                if (exception == null)
                    foreach (ResultsMessage rm in _masterCommunication.GetResults(out exception))
                        try
                        {
                            TorrentClient torrentClient = new TorrentClient();
                            torrentClient.SetTag(rm.TileStresstestIndex);
                            torrentClient.ProgressUpdated += new ProgressUpdatedEventHandler(torrentClient_ProgressUpdated);
                            torrentClient.DownloadCompleted += new DownloadCompletedEventHandler(torrentClient_DownloadCompleted);

                            torrentClient.DownloadTorrentFromBytes(rm.TorrentInfo, _resultPath);
                        }
                        catch (Exception ex) { exception = ex; }

                if (exception != null)
                {
                    ++_resultsReceived;
                    InvokeMessage("Could not receive one or more resuls!\n" + exception.ToString(), LogLevel.Error);
                }
                if (_resultsReceived == Finished)
                    InvokeOnFinished();
            }
        }

        private void torrentClient_ProgressUpdated(TorrentClient source, TorrentEventArgs e)
        {
            string tileStresstestIndex = source.GetTag() as string;
            InvokeResultsDownloadProgressUpdated(tileStresstestIndex, e.PercentCompleted);
        }

        private void torrentClient_DownloadCompleted(TorrentClient source, TorrentEventArgs e)
        {
            source.StopTorrent();
            string tileStresstestIndex = source.GetTag() as string;

            InvokeResultsDownloadCompleted(tileStresstestIndex, source.Name);

            source = null;

            if (++_resultsReceived == Finished)
                InvokeOnFinished();
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                try
                {
                    _isDisposed = true;
                    _hasResults = false;
                    _masterCommunication.Dispose();
                    _masterCommunication = null;
                    _usedTileStresstests = null;
                    _testProgressMessages = null;
                    _sw = null;
                }
                catch { }

                StaticObjectServiceWrapper.ObjectService.Unsuscribe(this);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Messages to the user on the message textbox
        /// </summary>
        public event EventHandler<MessageEventArgs> Message;
        public event EventHandler<TestProgressMessageReceivedEventArgs> OnTestProgressMessageReceived;
        public event EventHandler<ListeningErrorEventArgs> OnListeningError;
        public event EventHandler<ResultsDownloadProgressUpdatedEventArgs> ResultsDownloadProgressUpdated;
        public event EventHandler<ResultsDownloadCompletedEventArgs> ResultsDownloadCompleted;
        public event EventHandler<FinishedEventArgs> OnFinished;

        private void InvokeMessage(string message, LogLevel logLevel = LogLevel.Info)
        {
            InvokeMessage(message, Color.Empty, logLevel);
        }
        private void InvokeMessage(string message, Color color, LogLevel logLevel = LogLevel.Info)
        {
            LogWrapper.LogByLevel(message, logLevel);
            if (Message != null)
            {
                if (logLevel == LogLevel.Error)
                {
                    string[] split = message.Split(new char[] { '\n', '\r' }, StringSplitOptions.None);
                    message = split[0] + "\n See " + Logger.DEFAULT_LOCATION + " for the stack trace.";
                }
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { Message(this, new MessageEventArgs(message, color, logLevel)); });
            }
        }
        private void InvokeOnTestProgressMessageReceived(TileStresstest tileStresstest, TestProgressMessage testProgressMessage)
        {
            if (OnTestProgressMessageReceived != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { OnTestProgressMessageReceived(this, new TestProgressMessageReceivedEventArgs(tileStresstest, testProgressMessage)); });
        }
        private void InvokeOnListeningError(ListeningErrorEventArgs listeningErrorEventArgs)
        {
            LogWrapper.LogByLevel(listeningErrorEventArgs, LogLevel.Error);
            if (OnListeningError != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { OnListeningError(this, listeningErrorEventArgs); });
        }

        private void InvokeResultsDownloadProgressUpdated(string tileStresstestIndex, double percentCompleted)
        {
            lock (this)
            {
                TileStresstest ts = GetTileStresstest(tileStresstestIndex);

                if (ResultsDownloadProgressUpdated != null)
                    foreach (EventHandler<ResultsDownloadProgressUpdatedEventArgs> del in ResultsDownloadProgressUpdated.GetInvocationList())
                        del.BeginInvoke(this, new ResultsDownloadProgressUpdatedEventArgs(ts, (int)percentCompleted), null, null);
            }
        }
        private void InvokeResultsDownloadCompleted(string tileStresstestIndex, string subResultDir)
        {
            lock (this)
            {
                TileStresstest ts = GetTileStresstest(tileStresstestIndex);
                string resultPath = Path.Combine(_resultPath, subResultDir);

                StopSeedingResults(ts);

                lock (_resultsLock)
                    _results.Add(ts, resultPath);
                if (ResultsDownloadCompleted != null)
                    foreach (EventHandler<ResultsDownloadCompletedEventArgs> del in ResultsDownloadCompleted.GetInvocationList())
                        del.BeginInvoke(this, new ResultsDownloadCompletedEventArgs(ts, resultPath), null, null);
            }
        }
        /// <summary>
        /// Stops the seeding at the torrent server. (slave side)
        /// </summary>
        private void StopSeedingResults(TileStresstest tileStresstest)
        {
#warning Allow multiple slaves for work distribution
            Slave slave = tileStresstest.BasicTileStresstest.Slaves[0];

            Exception ex;
            _masterCommunication.StopSeedingResults(tileStresstest, out ex);
            _masterCommunication.DisconnectSlave(slave.IP, slave.Port);
        }

        private void InvokeOnFinished()
        {
            InvokeMessage("Test finished!");
            if (OnFinished != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { OnFinished(this, new FinishedEventArgs(OK, Cancelled, Failed)); });
        }
        #endregion
    }
}
