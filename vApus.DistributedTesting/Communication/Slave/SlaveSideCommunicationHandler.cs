/*
 * Copyright 2010 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public static class SlaveSideCommunicationHandler
    {
        /// <summary>
        /// Use this for instance to show the test name in the title bar of the main window.
        /// </summary>
        public static event EventHandler<NewTestEventArgs> NewTest;

        private static object _lock = new object();

        #region Message Handling
        private static ManualResetEvent _handleMessageWaitHandle = new ManualResetEvent(true);
        private static TileStresstestView _tileStresstestView;
        //Get the right socket wrapper to push progress to.
        private static SocketWrapper _masterSocketWrapper;

        public static Message<Key> HandleMessage(SocketWrapper receiver, Message<Key> message)
        {
            try
            {
                switch (message.Key)
                {
                    case Key.Poll:
                        return HandlePoll(message);
                    case Key.InitializeTest:
                        return HandleInitializeTest(message);
                    case Key.StartTest:
                        return HandleStartTest(message);
                    case Key.Break:
                        return HandleBreak(message);
                    case Key.Continue:
                        return HandleContinue(message);
                    case Key.StopTest:
                        return HandleStopTest(message);
                    case Key.Results:
                        return HandleResults(receiver, message);
                    case Key.StopSeedingResults:
                        return HandleStopSeedingResults(message);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "Communication error");
            }
            //'Communication error' in here?
            return message;
        }
        private static Message<Key> HandlePoll(Message<Key> message)
        {
            message.Content = new PollMessage { ProcessID = Process.GetCurrentProcess().Id };
            return message;
        }
        private static Message<Key> HandleInitializeTest(Message<Key> message)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                Solution.HideStresstestingSolutionExplorer();
            }, null);
            //init the send queue for push messages.
            _sendQueue = new ActiveObject();

            InitializeTestMessage initializeTestMessage = (InitializeTestMessage)message.Content;
            StresstestWrapper stresstestWrapper = initializeTestMessage.StresstestWrapper;

            try
            {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    SolutionComponentViewManager.DisposeViews();

                    if (_tileStresstestView != null)
                        try { _tileStresstestView.Close(); }
                        catch { }
                    try { _tileStresstestView.Dispose(); }
                    catch { }
                    _tileStresstestView = null;
                }, null);

                if (_masterSocketWrapper == null)
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    _masterSocketWrapper = new SocketWrapper(initializeTestMessage.PushIP, initializeTestMessage.PushPort, socket);
                    _masterSocketWrapper.Connect(1000, 3);
                }

                foreach(EventHandler<NewTestEventArgs> del in NewTest.GetInvocationList())
                    del.BeginInvoke(null, new NewTestEventArgs(stresstestWrapper.Stresstest.ToString()), null, null);

                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    _tileStresstestView = SolutionComponentViewManager.Show(stresstestWrapper.Stresstest, typeof(TileStresstestView)) as TileStresstestView;
                    _tileStresstestView.TileStresstestIndex = stresstestWrapper.TileStresstestIndex;
                    _tileStresstestView.RunSynchronization = stresstestWrapper.RunSynchronization;
                }, null);


                //This is threadsafe
                _tileStresstestView.InitializeTest();

            }
            catch (Exception ex)
            {
                initializeTestMessage.Exception = ex.ToString();
            }

            initializeTestMessage.StresstestWrapper = new StresstestWrapper();
            message.Content = initializeTestMessage;

            return message;
        }
        private static Message<Key> HandleStartTest(Message<Key> message)
        {
            StartAndStopMessage startMessage = new StartAndStopMessage();
            if (_tileStresstestView == null)
                startMessage.Exception = "No Tile Stresstest View found!";
            else
                try
                {
                    startMessage.TileStresstestIndex = _tileStresstestView.TileStresstestIndex;
                    _tileStresstestView.StartTest();
                }
                catch (Exception ex)
                {
                    startMessage.Exception = ex.ToString();
                }

            message.Content = startMessage;
            return message;
        }
        private static Message<Key> HandleBreak(Message<Key> message)
        {
            _tileStresstestView.Break();
            return message;
        }
        private static Message<Key> HandleContinue(Message<Key> message)
        {
            _tileStresstestView.Continue(((ContinueMessage)message.Content).ContinueCounter);
            return message;
        }
        private static Message<Key> HandleStopTest(Message<Key> message)
        {
            StartAndStopMessage stopMessage = new StartAndStopMessage();
            if (_tileStresstestView == null)
            {
                stopMessage.Exception = "No Tile Stresstest View found!";
            }
            else
            {
                stopMessage.TileStresstestIndex = _tileStresstestView.TileStresstestIndex;
                _tileStresstestView.PerformStopClick();
            }
            message.Content = stopMessage;
            return message;
        }
        private static Message<Key> HandleResults(SocketWrapper receiver, Message<Key> message)
        {
            ResultsMessage resultsMessage = new ResultsMessage();

            if (_tileStresstestView != null && _tileStresstestView.StresstestResults != null)
            {
                resultsMessage.TileStresstestIndex = _tileStresstestView.TileStresstestIndex;

                try
                {
                    string slaveSideResultsDir = Path.Combine(Application.StartupPath, "SlaveSideResults");
                    string file = Path.Combine(slaveSideResultsDir,
                        _tileStresstestView.StresstestResults.Stresstest.Replace(' ', '_').ReplaceInvalidWindowsFilenameChars('_') + ".r");

                    int j = 0;
                    while (File.Exists(Path.Combine(slaveSideResultsDir,
                        _tileStresstestView.StresstestResults.Stresstest.Replace(' ', '_').ReplaceInvalidWindowsFilenameChars('_') + new string('_', ++j) + ".r")))
                    {
                        file = Path.Combine(slaveSideResultsDir,
                            _tileStresstestView.StresstestResults.Stresstest.Replace(' ', '_').ReplaceInvalidWindowsFilenameChars('_') + new string('_', j) + ".r");
                    }

                    resultsMessage.TorrentInfo = CreateTorrent(file, slaveSideResultsDir);
                }
                catch (Exception ex)
                {
                    resultsMessage.Exception = ex.ToString();
                }
            }

            message.Content = resultsMessage;
            SynchronizeBuffers(message);

            return message;
        }
        private static Message<Key> HandleStopSeedingResults(Message<Key> message)
        {
            if (_torrentServer != null)
            {
                _torrentServer.CloseAllTorrents();
                _torrentServer = null;
            }
            message.Content = null;
            return message;
        }
        #endregion

        private static void SynchronizeBuffers(object toSend)
        {
            byte[] buffer = _masterSocketWrapper.ObjectToByteArray(toSend);
            int bufferSize = buffer.Length;
            if (bufferSize > _masterSocketWrapper.SendBufferSize)
            {
                _masterSocketWrapper.SendBufferSize = bufferSize;
                _masterSocketWrapper.ReceiveBufferSize = _masterSocketWrapper.SendBufferSize;
                SynchronizeBuffersMessage synchronizeBuffersMessage = new SynchronizeBuffersMessage();
                synchronizeBuffersMessage.BufferSize = _masterSocketWrapper.SendBufferSize;

                Message<Key> message = new Message<Key>();
                message.Key = Key.SynchronizeBuffers;
                message.Content = synchronizeBuffersMessage;
                _masterSocketWrapper.Send(message, SendType.Binary);
            }
        }

        #region Message Sending
        private delegate void SendPushMessageDelegate(string tileStresstestIndex,
                TileStresstestProgressResults tileStresstestProgressResults,
                StresstestResult stresstestResult,
                StresstestCore stresstestCore,
                List<EventPanelEvent> events,
                RunStateChange concurrentUsersStateChange);

        private static SendPushMessageDelegate _sendPushMessageDelegate = new SendPushMessageDelegate(SendQueuedPushMessage);
        private static ActiveObject _sendQueue;


        /// <summary>
        /// Queues the messages to send in queues per stresstest (vApus.Util.ActiveObject), thread safe.
        /// Note: this does not take into account / know if the socket on the other end is ready (to receive) or not.
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="tileStresstest"></param>
        /// <param name="tileStresstestProgressResults"></param>
        /// <param name="stresstestResult"></param>
        /// <param name="stresstestCore"></param>
        /// <param name="events"></param>
        /// <param name="concurrentUsersStateChange"></param>
        public static void SendPushMessage(string tileStresstestIndex,
                TileStresstestProgressResults tileStresstestProgressResults,
                StresstestResult stresstestResult,
                StresstestCore stresstestCore,
                List<EventPanelEvent> events,
                RunStateChange concurrentUsersStateChange)
        {
            lock (_lock)
            {
                _sendQueue.Send(_sendPushMessageDelegate, tileStresstestIndex, tileStresstestProgressResults, stresstestResult, stresstestCore, events, concurrentUsersStateChange);
            }
        }

        private static void SendQueuedPushMessage(string tileStresstestIndex,
            TileStresstestProgressResults tileStresstestProgressResults,
            StresstestResult stresstestResult,
            StresstestCore stresstestCore,
            List<EventPanelEvent> events,
            RunStateChange concurrentUsersStateChange)
        {
            try
            {
                TestProgressMessage tpm = new TestProgressMessage();
                tpm.TileStresstestIndex = tileStresstestIndex;

                tpm.ThreadsInUse = stresstestCore != null && !stresstestCore.IsDisposed ? stresstestCore.BusyThreadCount : 0;
                try
                {
                    tpm.CPUUsage = LocalMonitor.CPUUsage;
                    tpm.MemoryUsage = LocalMonitor.MemoryUsage;
                    tpm.TotalVisibleMemory = LocalMonitor.TotalVisibleMemory;
                    tpm.ContextSwitchesPerSecond = LocalMonitor.ContextSwitchesPerSecond;
                    tpm.NicsSent = LocalMonitor.NicsSent;
                    tpm.NicsReceived = LocalMonitor.NicsReceived;
                }
                catch { } //Exception on false WMI. 

                if (tileStresstestProgressResults != null)
                    tileStresstestProgressResults.Refresh();
                tpm.TileStresstestProgressResults = tileStresstestProgressResults;
                tpm.Events = events;
                tpm.StresstestResult = stresstestResult;
                tpm.RunStateChange = concurrentUsersStateChange;

                if (!_masterSocketWrapper.Connected)
                {
                    try
                    {
                        if (_masterSocketWrapper.Socket != null)
                            _masterSocketWrapper.Socket.Dispose();
                    }
                    catch { }

                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    _masterSocketWrapper = new SocketWrapper(_masterSocketWrapper.IP, _masterSocketWrapper.Port, socket);

                    try
                    {
                        _masterSocketWrapper.Connect(1000, 3);
                    }
                    catch { }
                }

                if (_masterSocketWrapper.Connected)
                {
                    var message = new Message<Key>(Key.Push, tpm);
                    try
                    {
                        SynchronizeBuffers(message);
                        _masterSocketWrapper.Send(message, SendType.Binary);
                    }
                    catch { }
                }
            }
            catch { }
        }
        #endregion

        #region Torrent (Result Sending)
        private static TorrentServer _torrentServer;
        private static SocketListener _socketListener = SocketListener.GetInstance();
        private static AutoResetEvent _torrentSeededWaitHandle = new AutoResetEvent(false);

        private static byte[] CreateTorrent(string file, string parentFolder)
        {
            try
            {
                //Set up and seed the torrent
                if (_torrentServer != null && _torrentServer.IP != _socketListener.IP && _torrentServer.Port != _socketListener.Port + 1000)
                {
                    _torrentServer.CloseAllTorrents();
                    _torrentServer = null;
                }

                //we'll only start the server and his tracker once
                if (_torrentServer == null)
                {
                    int i = 1000;
                    int max = 1500;
                    while (_torrentServer == null)
                        try
                        {
                            _torrentServer = new TorrentServer(_socketListener.IP, _socketListener.Port + (i++));
                        }
                        catch
                        {
                            if (i == max)
                                throw;
                        }
                    _torrentServer.TorrentSeeded += new TorrentSeededEventHandler(_torrentServer_TorrentSeeded);
                }

                //we start seeding this torrent (given through the bytes again) with given inputlocation.
                byte[] torrentInfo = _torrentServer.CreateTorrentInBytes(file);
                _torrentServer.StartSeeding(torrentInfo, parentFolder);
                _torrentSeededWaitHandle.WaitOne();

                return torrentInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed seeding torrent.\n" + ex.ToString());
            }
            return null;
        }
        private static void _torrentServer_TorrentSeeded(object source, TorrentServerEventArgs e)
        {
            _torrentSeededWaitHandle.Set();
        }
        #endregion

        public class NewTestEventArgs : EventArgs
        {
            public readonly string Test;
            public NewTestEventArgs(string test)
            {
                Test = test;
            }
        }
    }
}
