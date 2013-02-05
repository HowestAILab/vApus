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
using System.Net.Sockets;
using System.Threading;
using vApus.Monitor;
using vApus.Results;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting {
    public static class SlaveSideCommunicationHandler {
        private static readonly object _lock = new object();
        //For encrypting the mysql password
        private static string _passwordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";
        private static readonly byte[] _salt = { 0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03, 0x62 };

        #region Message Handling

        private static ManualResetEvent _handleMessageWaitHandle = new ManualResetEvent(true);
        private static TileStresstestView _tileStresstestView;
        //Get the right socket wrapper to push progress to.
        private static SocketWrapper _masterSocketWrapper;

        public static Message<Key> HandleMessage(SocketWrapper receiver, Message<Key> message) {
            try {
                switch (message.Key) {
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
                }
            } catch (Exception ex) {
                LogWrapper.LogByLevel("Communication error:\n" + ex, LogLevel.Error);
            }
            return message;
        }

        private static Message<Key> HandlePoll(Message<Key> message) {
            message.Content = new PollMessage { ProcessID = Process.GetCurrentProcess().Id };
            return message;
        }

        private static Message<Key> HandleInitializeTest(Message<Key> message) {
            try {
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { try { Solution.HideStresstestingSolutionExplorer(); } catch { } }, null);
                //init the send queue for push messages.
                _sendQueue = new ActiveObject();

                var initializeTestMessage = (InitializeTestMessage)message.Content;
                var stresstestWrapper = initializeTestMessage.StresstestWrapper;

                try {
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                        try { SolutionComponentViewManager.DisposeViews(); } catch { }
                    }, null);

                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _masterSocketWrapper = new SocketWrapper(initializeTestMessage.PushIP, initializeTestMessage.PushPort, socket);
                    _masterSocketWrapper.Connect(3000, 3);

                    if (NewTest != null)
                        foreach (EventHandler<NewTestEventArgs> del in NewTest.GetInvocationList())
                            del.BeginInvoke(null, new NewTestEventArgs(stresstestWrapper.Stresstest.ToString()), null, null);

                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                        int done = 1;
                    Retry:
                        try {
                            _tileStresstestView = SolutionComponentViewManager.Show(stresstestWrapper.Stresstest, typeof(TileStresstestView)) as TileStresstestView;
                            _tileStresstestView.TileStresstestIndex = stresstestWrapper.TileStresstestIndex;
                            _tileStresstestView.RunSynchronization = stresstestWrapper.RunSynchronization;

                            if (stresstestWrapper.StresstestIdInDb != -1 && !string.IsNullOrEmpty(stresstestWrapper.MySqlHost)) {
                                _tileStresstestView.ConnectToExistingDatabase(stresstestWrapper.MySqlHost, stresstestWrapper.MySqlPort, stresstestWrapper.MySqlDatabaseName, stresstestWrapper.MySqlUser,
                                    stresstestWrapper.MySqlPassword.Decrypt(_passwordGUID, _salt));
                                _tileStresstestView.StresstestIdInDb = stresstestWrapper.StresstestIdInDb;
                            }
                        } catch {
                            if (done != 4) {
                                Thread.Sleep(1000 * (done++));
                                goto Retry;
                            }
                            _tileStresstestView = null;
                            throw;
                        }
                    }, null);

                    //This is threadsafe
                    _tileStresstestView.InitializeTest();
                } catch (Exception ex) {
                    initializeTestMessage.Exception = ex.ToString();
                }

                initializeTestMessage.StresstestWrapper = null;
                message.Content = initializeTestMessage;
            } catch (Exception ex) {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }, null);
            }
            return message;
        }

        private static Message<Key> HandleStartTest(Message<Key> message) {
            var startMessage = new StartAndStopMessage();
            if (_tileStresstestView == null) startMessage.Exception = "No Tile Stresstest View found!";
            else try {
                    startMessage.TileStresstestIndex = _tileStresstestView.TileStresstestIndex;
                    _tileStresstestView.StartTest();
                } catch (Exception ex) { startMessage.Exception = ex.ToString(); }

            message.Content = startMessage;
            return message;
        }

        private static Message<Key> HandleBreak(Message<Key> message) {
            _tileStresstestView.Break();
            return message;
        }

        private static Message<Key> HandleContinue(Message<Key> message) {
            _tileStresstestView.Continue(((ContinueMessage)message.Content).ContinueCounter);
            return message;
        }

        private static Message<Key> HandleStopTest(Message<Key> message) {
            var stopMessage = new StartAndStopMessage();
            if (_tileStresstestView == null)
                stopMessage.Exception = "No Tile Stresstest View found!";
            else {
                stopMessage.TileStresstestIndex = _tileStresstestView.TileStresstestIndex;
                _tileStresstestView.PerformStopClick();
            }
            message.Content = stopMessage;
            return message;
        }
        #endregion

        /// <summary>
        ///     Use this for instance to show the test name in the title bar of the main window.
        /// </summary>
        public static event EventHandler<NewTestEventArgs> NewTest;

        private static void SynchronizeBuffers(object toSend) {
            byte[] buffer = _masterSocketWrapper.ObjectToByteArray(toSend);
            int bufferSize = buffer.Length;
            if (bufferSize > _masterSocketWrapper.SendBufferSize) {
                _masterSocketWrapper.SendBufferSize = bufferSize;
                _masterSocketWrapper.ReceiveBufferSize = _masterSocketWrapper.SendBufferSize;
                var synchronizeBuffersMessage = new SynchronizeBuffersMessage();
                synchronizeBuffersMessage.BufferSize = _masterSocketWrapper.SendBufferSize;

                var message = new Message<Key>();
                message.Key = Key.SynchronizeBuffers;
                message.Content = synchronizeBuffersMessage;
                _masterSocketWrapper.Send(message, SendType.Binary);
            }
        }

        #region Message Sending

        private static readonly SendPushMessageDelegate _sendPushMessageDelegate = SendQueuedPushMessage;
        private static ActiveObject _sendQueue;

        /// <summary>
        ///     Queues the messages to send in queues per stresstest (vApus.Util.ActiveObject), thread safe.
        ///     Note: this does not take into account / know if the socket on the other end is ready (to receive) or not.
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="tileStresstest"></param>
        /// <param name="tileStresstestProgressResults"></param>
        /// <param name="stresstestStatus"></param>
        /// <param name="stresstestCore"></param>
        /// <param name="events"></param>
        /// <param name="concurrentUsersStateChange"></param>
        public static void SendPushMessage(string tileStresstestIndex, StresstestMetricsCache stresstestMetricsCache, StresstestStatus stresstestStatus,
                                           DateTime startedAt, TimeSpan measuredRuntime, TimeSpan estimatedRuntimeLeft, StresstestCore stresstestCore,
                                           List<EventPanelEvent> events, RunStateChange concurrentUsersStateChange) {
            lock (_lock) {
                if (_sendQueue != null)
                    _sendQueue.Send(_sendPushMessageDelegate, tileStresstestIndex, stresstestMetricsCache,
                                    stresstestStatus, startedAt, measuredRuntime, estimatedRuntimeLeft, stresstestCore, events, concurrentUsersStateChange);
            }
        }

        private static void SendQueuedPushMessage(string tileStresstestIndex,
                                                  StresstestMetricsCache stresstestMetricsCache, StresstestStatus stresstestStatus, DateTime startedAt,
                                                  TimeSpan measuredRuntime, TimeSpan estimatedRuntimeLeft, StresstestCore stresstestCore,
                                                  List<EventPanelEvent> events, RunStateChange concurrentUsersStateChange) {
            if (_masterSocketWrapper != null)
                try {
                    var tpm = new TestProgressMessage();
                    tpm.TileStresstestIndex = tileStresstestIndex;

                    tpm.ThreadsInUse = stresstestCore != null && !stresstestCore.IsDisposed
                                           ? stresstestCore.BusyThreadCount : 0;
                    try {
                        tpm.CPUUsage = LocalMonitor.CPUUsage;
                        tpm.MemoryUsage = LocalMonitor.MemoryUsage;
                        tpm.TotalVisibleMemory = LocalMonitor.TotalVisibleMemory;
                        tpm.ContextSwitchesPerSecond = LocalMonitor.ContextSwitchesPerSecond;
                        tpm.NicsSent = LocalMonitor.NicsSent;
                        tpm.NicsReceived = LocalMonitor.NicsReceived;
                    } catch {
                    } //Exception on false WMI. 


                    tpm.StresstestMetricsCache = stresstestMetricsCache;
                    tpm.Events = events;
                    tpm.StresstestStatus = stresstestStatus;
                    tpm.StartedAt = startedAt;
                    tpm.MeasuredRuntime = measuredRuntime;
                    tpm.EstimatedRuntimeLeft = estimatedRuntimeLeft;
                    tpm.RunStateChange = concurrentUsersStateChange;

                    if (!_masterSocketWrapper.Connected) {
                        try {
                            if (_masterSocketWrapper.Socket != null) _masterSocketWrapper.Socket.Dispose();
                        } catch {
                        }

                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        _masterSocketWrapper = new SocketWrapper(_masterSocketWrapper.IP, _masterSocketWrapper.Port, socket);

                        try { _masterSocketWrapper.Connect(1000, 3); } catch {
                        }
                    }

                    if (_masterSocketWrapper.Connected) {
                        var message = new Message<Key>(Key.Push, tpm);
                        SynchronizeBuffers(message);
                        _masterSocketWrapper.Send(message, SendType.Binary);
                    }
                } catch {
                }
        }

        private delegate void SendPushMessageDelegate(string tileStresstestIndex, StresstestMetricsCache stresstestMetricsCache, StresstestStatus stresstestStatus, DateTime startedAt,
                                                      TimeSpan measuredRuntime, TimeSpan estimatedRuntimeLeft, StresstestCore stresstestCore, List<EventPanelEvent> events, RunStateChange concurrentUsersStateChange);

        #endregion

        public class NewTestEventArgs : EventArgs {
            public string Test { get; private set; }
            public NewTestEventArgs(string test) {
                Test = test;
            }
        }
    }
}
