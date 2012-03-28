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
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Monitor;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public static class SlaveSideCommunicationHandler
    {
        private static object _lock = new object();

        #region Message Handling
        private static ManualResetEvent _handleMessageWaitHandle = new ManualResetEvent(true);
        private static HashSet<TileStresstestView> _tileStresstestViews = new HashSet<TileStresstestView>();

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
            message.Content = new PollMessage(Process.GetCurrentProcess().Id);
            return message;
        }
        private static Message<Key> HandleInitializeTest(Message<Key> message)
        {
            CleanTileStresstestViews();
            CleanSendQueues();

            InitializeTestMessage initializeTestMessage = (InitializeTestMessage)message.Content;
            TileStresstest tileStresstest = initializeTestMessage.TileStresstest;

            //Set if possible.
            IntPtr originalProcessorAffinity = Process.GetCurrentProcess().ProcessorAffinity;
            try
            {
                int[] formattedPA = null;
                if (tileStresstest.ProcessorAffinity.Length != 0)
                {
                    formattedPA = new int[tileStresstest.ProcessorAffinity.Length];
                    for (int i = 0; i < tileStresstest.ProcessorAffinity.Length; i++)
                        formattedPA[i] = tileStresstest.ProcessorAffinity[i] - 1;
                }
                else
                {
                    formattedPA = new int[Environment.ProcessorCount];
                    for (int i = 0; i < Environment.ProcessorCount; i++)
                        formattedPA[i] = i;
                }

                Process.GetCurrentProcess().ProcessorAffinity = ProcessorAffinityCalculator.FromArrayToBitmask(formattedPA);

                TileStresstestView tileStresstestView = GetTileStresstestView(tileStresstest.OriginalHashCode);
                if (tileStresstestView != null)
                    _tileStresstestViews.Remove(tileStresstestView);

                //Get the right socket wrapper to push progress to.
                SocketWrapper masterSocketWrapper = null;

                foreach (TileStresstestView tvw in _tileStresstestViews)
                    if (tvw.MasterSocketWrapper != null)
                    {
                        masterSocketWrapper = tvw.MasterSocketWrapper;
                        break;
                    }

                if (masterSocketWrapper == null)
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    masterSocketWrapper = new SocketWrapper(initializeTestMessage.PushIP, initializeTestMessage.PushPort, socket);
                    masterSocketWrapper.Connect(1000, 3);
                }

                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    tileStresstestView = SolutionComponentViewManager.Show(tileStresstest) as TileStresstestView;
                });

                _tileStresstestViews.Add(tileStresstestView);

                tileStresstestView.MasterSocketWrapper = masterSocketWrapper;

                //This is threadsafe
                tileStresstestView.InitializeTest();

                initializeTestMessage.TileStresstest = null;

                message.Content = initializeTestMessage;
            }
            catch (Exception ex)
            {
                Process.GetCurrentProcess().ProcessorAffinity = originalProcessorAffinity;
                initializeTestMessage.TileStresstest = null;
                initializeTestMessage.Exception = ex.ToString();
                message.Content = initializeTestMessage;
            }
            return message;
        }
        private static Message<Key> HandleStartTest(Message<Key> message)
        {
            StartAndStopMessage startMessage = (StartAndStopMessage)message.Content;
            Parallel.ForEach(startMessage.TileStresstestHashCodes, delegate(int tileStresstestOriginalHashCode, ParallelLoopState state)
            {
                TileStresstestView tileStresstestView = GetTileStresstestView(tileStresstestOriginalHashCode);
                if (tileStresstestView == null)
                {
                    startMessage.Exception = "No Tile Stresstest View found!";
                    state.Stop();
                }
                else
                {
                    try
                    {
                        tileStresstestView.StartTest();
                    }
                    catch (Exception ex)
                    {
                        startMessage.Exception = ex.ToString();
                        state.Stop();
                    }
                }
            });
            message.Content = startMessage;
            return message;
        }
        private static Message<Key> HandleBreak(Message<Key> message)
        {
            Parallel.ForEach(_tileStresstestViews, delegate(TileStresstestView tsv)
            {
                tsv.Break();
            });
            return message;
        }
        private static Message<Key> HandleContinue(Message<Key> message)
        {
            var continueMessage = (ContinueMessage)message.Content;
            Parallel.ForEach(_tileStresstestViews, delegate(TileStresstestView tsv)
            {
                tsv.Continue(continueMessage.ContinueCounter);
            });
            return message;
        }
        private static Message<Key> HandleStopTest(Message<Key> message)
        {
            var stopMessage = (StartAndStopMessage)message.Content;

            Parallel.ForEach(stopMessage.TileStresstestHashCodes, delegate(int tileStresstestOriginalHashCode)
            {
                TileStresstestView tileStresstestView = GetTileStresstestView(tileStresstestOriginalHashCode);
                if (tileStresstestView == null)
                    stopMessage.Exception = "No Tile Stresstest View found!";
                else
                    tileStresstestView.PerformStopClick();
            });

            message.Content = stopMessage;
            return message;
        }
        private static Message<Key> HandleResults(SocketWrapper receiver, Message<Key> message)
        {
            ResultsMessage resultsMessage = (ResultsMessage)message.Content;

            Parallel.ForEach(resultsMessage.TileStresstestHashCodes, delegate(int tileStresstestOriginalHashCode)
            {
                TileStresstestView tileStresstestView = GetTileStresstestView(tileStresstestOriginalHashCode);
                if (tileStresstestView != null && tileStresstestView.StresstestResults != null)
                {
                    //Needed?
                    tileStresstestView.MasterSocketWrapper = receiver;

                    string slaveSideResultsDir = Path.Combine(Application.StartupPath, "SlaveSideResults");
                    string file = Path.Combine(slaveSideResultsDir, "PID_" + Process.GetCurrentProcess().Id.ToString() + "_" +
                        tileStresstestView.StresstestResults.Stresstest.Replace(' ', '_').ReplaceInvalidWindowsFilenameChars('_') + ".r");

                    int j = 0;
                    while (File.Exists(Path.Combine(slaveSideResultsDir, "PID_" + Process.GetCurrentProcess().Id.ToString() + "_" +
                        tileStresstestView.StresstestResults.Stresstest.Replace(' ', '_').ReplaceInvalidWindowsFilenameChars('_') + new string('_', ++j) + ".r")))
                    {
                        file = Path.Combine(slaveSideResultsDir, "PID_" + Process.GetCurrentProcess().Id.ToString() + "_" +
                        tileStresstestView.StresstestResults.Stresstest.Replace(' ', '_').ReplaceInvalidWindowsFilenameChars('_') + new string('_', j) + ".r");
                    }

                    lock (_lock)
                        resultsMessage.TorrentInfo.Add(CreateTorrent(file, slaveSideResultsDir));
                }
            });

            message.Content = resultsMessage;
            SynchronizeBuffers(receiver, message);

            return message;
        }
        private static Message<Key> HandleStopSeedingResults(Message<Key> message)
        {
            if (_torrentServer != null)
            {
                StopSeedingResultsMessage stopSeedingResultsMessage = (StopSeedingResultsMessage)message.Content;
                _torrentServer.StopTorrent(stopSeedingResultsMessage.TorrentName);
            }
            message.Content = null;
            return message;
        }

        private static void CleanTileStresstestViews()
        {
            HashSet<TileStresstestView> tileStresstestViews = new HashSet<TileStresstestView>();
            foreach (TileStresstestView tileStresstestView in _tileStresstestViews)
                if (tileStresstestView != null && !tileStresstestView.IsDisposed)
                    tileStresstestViews.Add(tileStresstestView);
            _tileStresstestViews = tileStresstestViews;
        }
        /// <summary>
        /// Thread safe
        /// </summary>
        /// <param name="originalHashCode"></param>
        /// <returns></returns>
        private static TileStresstestView GetTileStresstestView(int originalHashCode)
        {
            lock (_lock)
            {
                foreach (TileStresstestView tileStresstestView in _tileStresstestViews)
                    if (tileStresstestView.TileStresstest.OriginalHashCode == originalHashCode)
                        return tileStresstestView;
                return null;
            }
        }
        #endregion

        private static void SynchronizeBuffers(SocketWrapper socketWrapper, object toSend)
        {
            byte[] buffer = socketWrapper.ObjectToByteArray(toSend);
            int bufferSize = buffer.Length;
            if (bufferSize > socketWrapper.SendBufferSize)
            {
                socketWrapper.SendBufferSize = bufferSize;
                socketWrapper.ReceiveBufferSize = socketWrapper.SendBufferSize;
                SynchronizeBuffersMessage synchronizeBuffersMessage = new SynchronizeBuffersMessage();
                synchronizeBuffersMessage.BufferSize = socketWrapper.SendBufferSize;

                Message<Key> message = new Message<Key>();
                message.Key = Key.SynchronizeBuffers;
                message.Content = synchronizeBuffersMessage;
                socketWrapper.Send(message, SendType.Binary);
            }
        }

        #region Message Sending
        private delegate void SendPushMessageDelegate(SocketWrapper socketWrapper,
                TileStresstest tileStresstest,
                TileStresstestProgressResults tileStresstestProgressResults,
                StresstestResult stresstestResult,
                StresstestCore stresstestCore,
                List<EventPanelEvent> events,
                RunStateChange concurrentUsersStateChange);

        private static SendPushMessageDelegate _sendPushMessageDelegate = new SendPushMessageDelegate(SendQueuedPushMessage);
        private static Dictionary<int, ActiveObject> _sendQueues = new Dictionary<int, ActiveObject>();

        /// <summary>
        /// This is called when the tile stresstest are initialized.
        /// </summary>
        private static void CleanSendQueues()
        {
            lock (_lock)
            {
                foreach (ActiveObject sendQueue in _sendQueues.Values)
                    sendQueue.Dispose();
                _sendQueues = new Dictionary<int, ActiveObject>();
            }
        }

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
        public static void SendPushMessage(SocketWrapper socketWrapper,
                TileStresstest tileStresstest,
                TileStresstestProgressResults tileStresstestProgressResults,
                StresstestResult stresstestResult,
                StresstestCore stresstestCore,
                List<EventPanelEvent> events,
                RunStateChange concurrentUsersStateChange)
        {
            lock (_lock)
            {
                if (!_sendQueues.ContainsKey(tileStresstest.OriginalHashCode))
                    _sendQueues.Add(tileStresstest.OriginalHashCode, new ActiveObject());

                ActiveObject sendQueue = _sendQueues[tileStresstest.OriginalHashCode];
                sendQueue.Send(_sendPushMessageDelegate, socketWrapper, tileStresstest, tileStresstestProgressResults, stresstestResult, stresstestCore, events, concurrentUsersStateChange);
            }
        }

        private static void SendQueuedPushMessage(SocketWrapper socketWrapper,
            TileStresstest tileStresstest,
            TileStresstestProgressResults tileStresstestProgressResults,
            StresstestResult stresstestResult,
            StresstestCore stresstestCore,
            List<EventPanelEvent> events,
            RunStateChange concurrentUsersStateChange)
        {
            try
            {
                PushMessage pushMessage = new PushMessage();
                pushMessage.TileStresstestOriginalHashCode = tileStresstest.OriginalHashCode;

                pushMessage.ThreadsInUse = stresstestCore != null && !stresstestCore.IsDisposed ? stresstestCore.BusyThreadCount : 0;
                pushMessage.CPUUsage = LocalMonitor.CPUUsage;
                pushMessage.MemoryUsage = LocalMonitor.MemoryUsage;
                pushMessage.TotalVisibleMemory = LocalMonitor.TotalVisibleMemory;
                pushMessage.ContextSwitchesPerSecond = LocalMonitor.ContextSwitchesPerSecond;
                pushMessage.NicsSent = LocalMonitor.NicsSent;
                pushMessage.NicsReceived = LocalMonitor.NicsReceived;

                if (tileStresstestProgressResults != null)
                    tileStresstestProgressResults.Refresh();
                pushMessage.TileStresstestProgressResults = tileStresstestProgressResults;
                pushMessage.Events = events;
                pushMessage.StresstestResult = stresstestResult;
                pushMessage.RunStateChange = concurrentUsersStateChange;

                if (!socketWrapper.Connected)
                {
                    try
                    {
                        if (socketWrapper.Socket != null)
                            socketWrapper.Socket.Dispose();
                    }
                    catch { }

                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    socketWrapper = new SocketWrapper(socketWrapper.IP, socketWrapper.Port, socket);

                    try
                    {
                        socketWrapper.Connect(1000, 3);
                    }
                    catch { }
                }

                if (socketWrapper.Connected)
                {
                    var message = new Message<Key>(Key.Push, pushMessage);
                    try
                    {
                        SynchronizeBuffers(socketWrapper, message);
                        socketWrapper.Send(message, SendType.Binary);
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
    }
}
