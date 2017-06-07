/*
 * Copyright 2010 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using vApus.Monitor;
using vApus.Results;
using vApus.Communication.Shared;
using vApus.SolutionTree;
using vApus.StressTest;
using vApus.Util;
using vApus.Publish;

namespace vApus.DistributedTest {
    /// <summary>
    /// Handles communication comming from vApus master through SocketListener. Pushes status and metrics to the master.
    /// </summary>
    public static class SlaveSideCommunicationHandler {
        /// <summary>
        ///     Use this for instance to show the test name in the title bar of the main window.
        /// </summary>
        public static event EventHandler<NewTestEventArgs> NewTest;

        private delegate void SendPushMessageDelegate(string tileStressTestIndex, FastStressTestMetricsCache stressTestMetricsCache, StressTestStatus stressTestStatus, DateTime startedAt, TimeSpan measuredRuntime,
                                              TimeSpan estimatedRuntimeLeft, StressTestCore stressTestCore, EventPanelEvent[] events, RunStateChange concurrentUsersStateChange, bool runFinished, bool concurrencyFinished);

        #region Fields
        private static readonly object _lock = new object();

        private static TileStressTestView _tileStressTestView;
        //Get the right socket wrapper to push progress to.
        private static SocketWrapper _masterSocketWrapper;

        private static AutoResetEvent _testInitializedWaitHandle = new AutoResetEvent(false);
        private static Exception _testInitializedException;

        private static readonly SendPushMessageDelegate _sendPushMessageDelegate = SendQueuedPushMessage;
        private static BackgroundWorkQueue _sendQueue;
        #endregion

        #region Functions

        #region Message Handling
        public static Message<Key> HandleMessage(Message<Key> message) {
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
                    case Key.ContinueDivided:
                        return HandleContinueDivided(message);
                    case Key.StopTest:
                        return HandleStopTest(message);
                }
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Communication error.", ex);
            }
            return message;
        }

        private static Message<Key> HandlePoll(Message<Key> message) {
            message.Content = new PollMessage { ProcessID = Process.GetCurrentProcess().Id };
            return message;
        }

        private static Message<Key> HandleInitializeTest(Message<Key> message) {
            try {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                    try {
                        Solution.HideStressTestingSolutionExplorer();
                    }
                    catch (Exception ex) {
                        Loggers.Log(Level.Warning, "Failed hiding solution explorer.", ex, new object[] { message });
                    }
                }, null);

                //init the send queue for push messages.
                _sendQueue = new BackgroundWorkQueue();

                var initializeTestMessage = (InitializeTestMessage)message.Content;
                var stressTestWrapper = initializeTestMessage.StressTestWrapper;

                NamedObjectRegistrar.RegisterOrUpdate("IsMaster", false);
                Publisher.Settings.PublisherEnabled = stressTestWrapper.Publish;
                Publisher.Settings.TcpHost = stressTestWrapper.PublishHost;
                Publisher.Settings.TcpPort = stressTestWrapper.PublishPort;

                stressTestWrapper.StressTest.Connection.ConnectionProxy.ForceSettingChildsParent();
                foreach (var kvp in stressTestWrapper.StressTest.Scenarios) {
                    kvp.Key.ScenarioRuleSet.ForceSettingChildsParent();
                    kvp.Key.ForceSettingChildsParent();
                }
                stressTestWrapper.ValueStore.ForceSettingChildsParent();

                try {
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                        try {
                            SolutionComponentViewManager.DisposeViews();
                        }
                        catch (Exception ex) {
                            Loggers.Log(Level.Warning, "Failed disposing views.", ex, new object[] { message });
                        }
                    }, null);

                    Exception pushIPException = null;
                    //Try to connect to multiple ips to see wich one is reachable.
                    foreach (string pushIP in initializeTestMessage.PushIPs) {
                        try {
                            var address = IPAddress.Parse(pushIP);
                            var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            _masterSocketWrapper = new SocketWrapper(address, initializeTestMessage.PushPort, socket);
                            _masterSocketWrapper.Connect(30000, 3);
                            pushIPException = null;
                            break;
                        }
                        catch (Exception e) {
                            pushIPException = e;
                        }
                    }

                    if (pushIPException != null) throw pushIPException;

                    if (NewTest != null) {
                        var invocationList = NewTest.GetInvocationList();
                        string stressTestToString = stressTestWrapper.StressTest.ToString();
                        Parallel.For(0, invocationList.Length, (i) => {
                            (invocationList[i] as EventHandler<NewTestEventArgs>).Invoke(null, new NewTestEventArgs(stressTestToString));
                        });
                    }

                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                        int done = 1;
                    Retry:
                        try {
                            _tileStressTestView = SolutionComponentViewManager.Show(stressTestWrapper.StressTest, typeof(TileStressTestView)) as TileStressTestView;
                            _tileStressTestView.DistributedTest = stressTestWrapper.DistributedTest;
                            _tileStressTestView.TileStressTest = stressTestWrapper.TileStressTest;
                            _tileStressTestView.TileStressTestIndex = stressTestWrapper.TileStressTestIndex;
                            _tileStressTestView.Monitors = stressTestWrapper.Monitors;
                            _tileStressTestView.ResultSetId = stressTestWrapper.PublishResultSetId;
                            _tileStressTestView.RunSynchronization = stressTestWrapper.RunSynchronization;
                            _tileStressTestView.MaxRerunsBreakOnLast = stressTestWrapper.MaxRerunsBreakOnLast;
                            _tileStressTestView.ValueStore = stressTestWrapper.ValueStore;
                        }
                        catch {
                            if (++done != 4) {
                                Thread.Sleep(1000);

                                try {
                                    SolutionComponentViewManager.DisposeViews();
                                }
                                catch (Exception ex) {
                                    Loggers.Log(Level.Warning, "Failed disposing views.", ex, new object[] { message });
                                }

                                goto Retry;
                            }
                            _tileStressTestView = null;
                            throw;
                        }
                    }, null);

                    //This is threadsafe
                    _tileStressTestView.TestInitialized += _tileStressTestView_TestInitialized;
                    _tileStressTestView.InitializeTest();
                    _testInitializedWaitHandle.WaitOne();

                    if (_testInitializedException != null) throw _testInitializedException;
                }
                catch (Exception ex) {
                    initializeTestMessage.Exception = ex.ToString();
                    Loggers.Log(Level.Error, "Failed initializing test.", ex);
                }

                initializeTestMessage.StressTestWrapper = null;
                message.Content = initializeTestMessage;
            }
            catch (Exception ex) {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }, null);
            }
            return message;
        }
        private static void _tileStressTestView_TestInitialized(object sender, StressTest.TestInitializedEventArgs e) {
            _tileStressTestView.TestInitialized -= _tileStressTestView_TestInitialized;
            _testInitializedException = e.Exception;
            _testInitializedWaitHandle.Set();
        }

        private static Message<Key> HandleStartTest(Message<Key> message) {
            var startMessage = new StartAndStopMessage();
            if (_tileStressTestView == null) startMessage.Exception = "No Tile Stress Test View found!";
            else try {
                    startMessage.TileStressTestIndex = _tileStressTestView.TileStressTestIndex;
                    _tileStressTestView.StartTest();
                }
                catch (Exception ex) { startMessage.Exception = ex.ToString(); }

            message.Content = startMessage;
            return message;
        }

        private static Message<Key> HandleBreak(Message<Key> message) {
            _tileStressTestView.Break();
            return message;
        }

        private static Message<Key> HandleContinue(Message<Key> message) {
            _tileStressTestView.Continue(((ContinueMessage)message.Content).ContinueCounter);
            return message;
        }

        private static Message<Key> HandleContinueDivided(Message<Key> message) {
            _tileStressTestView.ContinueDivided();
            return message;
        }

        private static Message<Key> HandleStopTest(Message<Key> message) {
            var stopMessage = new StartAndStopMessage();
            if (_tileStressTestView == null)
                stopMessage.Exception = "No Tile Stress test View found!";
            else {
                stopMessage.TileStressTestIndex = _tileStressTestView.TileStressTestIndex;
                SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                    _tileStressTestView.PerformStopClick();
                }, null);
            }
            message.Content = stopMessage;
            return message;
        }
        #endregion

        #region Message Sending
        /// <summary>
        ///     Queues the messages to send in queues per stress test (vApus.Util.ActiveObject), thread safe.
        ///     Note: this does not take into account / know if the socket on the other end is ready (to receive) or not.
        /// </summary>
        public static void SendPushMessage(string tileStressTestIndex, FastStressTestMetricsCache stressTestMetricsCache, StressTestStatus stressTestStatus, DateTime startedAt, TimeSpan measuredRuntime,
                                           TimeSpan estimatedRuntimeLeft, StressTestCore stressTestCore, EventPanelEvent[] events, RunStateChange runStateChange, bool runFinished, bool concurrencyFinished) {
            lock (_lock) {
                if (runStateChange != RunStateChange.None)
                    Loggers.Log(Level.Info, "Queueing test progress message for " + tileStressTestIndex + ", run state change " + Enum.GetName(typeof(RunStateChange), runStateChange));
                if (_sendQueue != null) {
                    _sendQueue.EnqueueWorkItem(_sendPushMessageDelegate, tileStressTestIndex, stressTestMetricsCache,
                        stressTestStatus, startedAt, measuredRuntime, estimatedRuntimeLeft, stressTestCore, events, runStateChange, runFinished, concurrencyFinished);
                }
            }
        }

        private static void SendQueuedPushMessage(string tileStressTestIndex, FastStressTestMetricsCache stressTestMetricsCache, StressTestStatus stressTestStatus, DateTime startedAt, TimeSpan measuredRuntime,
                                                  TimeSpan estimatedRuntimeLeft, StressTestCore stressTestCore, EventPanelEvent[] events, RunStateChange runStateChange, bool runFinished, bool concurrencyFinished) {
            if (_masterSocketWrapper != null)
                try {
                    var tpm = new TestProgressMessage();
                    tpm.TileStressTestIndex = tileStressTestIndex;

                    tpm.ThreadsInUse = stressTestCore != null && !stressTestCore.IsDisposed ? stressTestCore.BusyThreadCount : 0;
                    try {
                        tpm.CPUUsage = LocalMonitor.CPUUsage;
                        tpm.MemoryUsage = LocalMonitor.MemoryUsage;
                        tpm.TotalVisibleMemory = LocalMonitor.TotalVisibleMemory;
                        tpm.Nic = LocalMonitor.Nic;
                        tpm.NicSent = LocalMonitor.NicSent;
                        tpm.NicReceived = LocalMonitor.NicReceived;
                        tpm.NicBandwidth = LocalMonitor.NicBandwidth;
                    }
                    catch {
                    } //Exception on false WMI. 


                    tpm.StressTestMetricsCache = stressTestMetricsCache;
                    tpm.Events = events;

                    tpm.StressTestStatus = stressTestStatus;
                    tpm.StartedAt = startedAt;
                    tpm.MeasuredRuntime = measuredRuntime;
                    tpm.EstimatedRuntimeLeft = estimatedRuntimeLeft;
                    tpm.RunStateChange = runStateChange;
                    tpm.RunFinished = runFinished;
                    tpm.ConcurrencyFinished = concurrencyFinished;

                    bool reconnect = false;
                    for (int i = 10; ; i += 10)
                        try {
                            if (!_masterSocketWrapper.Connected || reconnect) {
                                try {
                                    if (_masterSocketWrapper.Socket != null) _masterSocketWrapper.Socket.Dispose();
                                }
                                catch {
                                    //Don't care.
                                }

                                var socket = new Socket(_masterSocketWrapper.IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                                _masterSocketWrapper = new SocketWrapper(_masterSocketWrapper.IP, _masterSocketWrapper.Port, socket);

                                _masterSocketWrapper.Connect(3000, 1);
                            }

                            if (_masterSocketWrapper.Connected) {
                                var message = new Message<Key>(Key.Push, tpm);
                                byte[] buffer = SynchronizeBuffers(message);
                                _masterSocketWrapper.SendTimeout = 1000 * i;
                                _masterSocketWrapper.SendBytes(buffer);

                                if (runStateChange != RunStateChange.None)
                                    Loggers.Log(Level.Info, "Sent test progress message for " + tpm.TileStressTestIndex + ", run state change " + Enum.GetName(typeof(RunStateChange), runStateChange));


                                break;
                            }
                        }
                        catch {
                            reconnect = true;
                            if (i == 100) throw;
                        }
                }
                catch (Exception ex) {
                    //Master not available. Ignore.
                    Loggers.Log(Level.Info, "Master became unavailable. If this happens during a test this is considered an error. Otherwise it works as intended.", ex);
                }
        }
        #endregion

        private static byte[] SynchronizeBuffers(object toSend) {
            byte[] buffer = _masterSocketWrapper.ObjectToByteArray(toSend);
            int bufferSize = buffer.Length;
            if (bufferSize > _masterSocketWrapper.SendBufferSize) {
                Loggers.Log(Level.Info, "Synchronizing buffers from slave to server.");

                _masterSocketWrapper.SendBufferSize = bufferSize;
                _masterSocketWrapper.ReceiveBufferSize = _masterSocketWrapper.SendBufferSize;
                var synchronizeBuffersMessage = new SynchronizeBuffersMessage();
                synchronizeBuffersMessage.BufferSize = _masterSocketWrapper.SendBufferSize;

                _masterSocketWrapper.Send(new Message<Key>(Key.SynchronizeBuffers, synchronizeBuffersMessage), SendType.Binary);
            }
            return buffer;
        }
        #endregion

        public class NewTestEventArgs : EventArgs {
            public string Test { get; private set; }
            public NewTestEventArgs(string test) { Test = test; }
        }
    }
}
