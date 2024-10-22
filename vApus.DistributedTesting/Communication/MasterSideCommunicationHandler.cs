﻿/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vApus.Communication.Shared;
using vApus.StressTest;
using vApus.Util;

namespace vApus.DistributedTest {
    /// <summary>
    /// Handles sending to and receiving from slaves / jumpstart service.
    /// </summary>
    public static class MasterSideCommunicationHandler {

        #region Events
        public static event EventHandler<ListeningErrorEventArgs> ListeningError;
        public static event EventHandler<TestProgressMessageReceivedEventArgs> OnTestProgressMessageReceived;
        #endregion

        #region Fields
        private static object _lock = new object();
        private static object _stopLock = new object();

        //A slave side and a master side socket wrappers for full duplex communication.
        private static Dictionary<SocketWrapper, SocketWrapper> _connectedSlaves = new Dictionary<SocketWrapper, SocketWrapper>();
        private static AsyncCallback _onReceiveCallBack;

        //When sending a continue --> continue to the right run.
        private static int _continueCounter = -1;

        [ThreadStatic]
        private static InitializeTestWorkItem _initializeTestWorkItem;
        [ThreadStatic]
        private static StopTestWorkItem _stopTestWorkItem;
        #endregion

        #region Functions

        #region Private
        /// <summary>
        /// Will begin listening after initializing the test.
        /// 
        /// The retry count for connecting is 9 and the connect timeout is 30 seconds.
        /// </summary>
        /// <param name="slaveSocketWrapper"></param>
        /// <param name="processID">-1 for already connected.</param>
        /// <param name="exception"></param>
        private static void ConnectSlave(SocketWrapper slaveSocketWrapper, out int processID, out Exception exception) {
            processID = -1;
            exception = null;
            int retry = 1;
        Retry:
            try {
                exception = null;
                if (!slaveSocketWrapper.Connected) {
                    slaveSocketWrapper.SendTimeout = slaveSocketWrapper.ReceiveTimeout = 360000;
                    slaveSocketWrapper.Connect(120000, 3);
                    if (slaveSocketWrapper.Connected) {
                        var masterSocketWrapper = GetMasterSocketWrapper(slaveSocketWrapper);

                        Message<Key> message = SendAndReceive(slaveSocketWrapper, Key.Poll, null);
                        PollMessage pollMessage = (PollMessage)message.Content;
                        processID = pollMessage.ProcessID;

                        lock (_lock)
                            if (!_connectedSlaves.ContainsKey(slaveSocketWrapper))
                                _connectedSlaves.Add(slaveSocketWrapper, masterSocketWrapper);
                    }
                    else {
                        throw (new Exception(string.Format("Could not connect {0}:{1}.", slaveSocketWrapper.IP.ToString(), slaveSocketWrapper.Port.ToString())));
                    }
                }
            }
            catch (Exception ex) {
                if (++retry != 4) {
                    goto Retry;
                }
                else {
                    exception = ex;
                }
            }
        }
        /// <summary>
        /// Will start listening too.
        /// </summary>
        /// <param name="slaveSocketWrapper"></param>
        /// <returns></returns>
        private static SocketWrapper GetMasterSocketWrapper(SocketWrapper slaveSocketWrapper) {
            Exception exception = null;

            SocketWrapper masterSocketWrapper = null;
            lock (_lock)
                if (_connectedSlaves.ContainsKey(slaveSocketWrapper))
                    masterSocketWrapper = _connectedSlaves[slaveSocketWrapper];

            if (masterSocketWrapper == null) {
                var address = slaveSocketWrapper.IP.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any;
                var adapters = NetworkInterface.GetAllNetworkInterfaces();

                var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                int start = slaveSocketWrapper.Port * 10;
                int stop = int.MaxValue;

                for (int i = start; i != stop; i++) {
                    try {
                        masterSocketWrapper = new SocketWrapper(address, i, socket);
                        StartListening(masterSocketWrapper);
                        exception = null;
                        break;
                    }
                    catch (Exception ex) {
                        exception = ex;
                    }
                }
            }

            if (exception != null)
                throw exception;
            return masterSocketWrapper;
        }
        /// <summary>
        /// </summary>
        /// <param name="slaveSocketWrapper">Will be removed from the connected slaves collection.</param>
        private static void DisconnectSlave(SocketWrapper slaveSocketWrapper) {
            try {
                foreach (SocketWrapper socketWrapper in _connectedSlaves.Keys)
                    if (socketWrapper == slaveSocketWrapper) {
                        if (slaveSocketWrapper != null) {
                            try {
                                var masterSocketWrapper = _connectedSlaves[slaveSocketWrapper];
                                masterSocketWrapper.Close();
                                masterSocketWrapper = null;
                            }
                            catch {
                                //Don't care.
                            }

                            try {
                                slaveSocketWrapper.Close();
                            }
                            catch {
                                //Don't care.
                            }
                        }
                        _connectedSlaves.Remove(slaveSocketWrapper);
                        slaveSocketWrapper = null;
                        break;
                    }
            }
            catch {
                //Don't care.
            }
        }
        /// <summary>
        /// Gets the slave socket wrapper from the connected slaves. Returns null if not found.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        private static SocketWrapper Get(string ip, int port) {
            lock (_lock) {
                foreach (SocketWrapper socketWrapper in _connectedSlaves.Keys)
                    if (socketWrapper.IP.ToString() == ip && socketWrapper.Port == port)
                        return socketWrapper;
                return null;
            }
        }
        /// <summary>
        /// Will try to reconnect if the connection was lost.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static SocketWrapper Get(string ip, int port, out Exception exception) {
            lock (_lock) {
                exception = null;
                int processID;
                SocketWrapper socketWrapper = Get(ip, port);
                if (socketWrapper != null && !socketWrapper.Connected)
                    ConnectSlave(socketWrapper, out processID, out exception);

                if (socketWrapper == null || !socketWrapper.Connected)
                    exception = new Exception(string.Format("Not connected to {0}:{1}.", ip, port));
                return socketWrapper;
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="message"></param>
        private static void Send(SocketWrapper socketWrapper, Message<Key> message) {
            for (int i = 10; ; i += 10)
                try {
                    socketWrapper.SendTimeout = 1000 * i;
                    socketWrapper.Send(message, SendType.Binary);
                    break;
                }
                catch {
                    if (i == 100) throw;
                }
        }
        /// <summary>
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="tempSendTimeout">A temporarly timeout for the send, it will be reset to -1 afterwards.</param>
        private static void Send(SocketWrapper socketWrapper, Key key, object content) {
            Send(socketWrapper, new Message<Key>(key, content));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="tempReceiveTimeout">A temporarly timeout for the receive, it will be reset to -1 afterwards.</param>
        /// <returns></returns>
        private static Message<Key> Receive(SocketWrapper socketWrapper) {
            for (int i = 10; ; i += 10)
                try {
                    socketWrapper.ReceiveTimeout = 1000 * i;
                    return (Message<Key>)socketWrapper.Receive(SendType.Binary);
                }
                catch {
                    if (i == 100) throw;
                }
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="tempSendReceiveTimeout">A temporarly timeout for the send and the receive, it will be reset to -1 afterwards.</param>
        /// <returns></returns>
        private static Message<Key> SendAndReceive(SocketWrapper socketWrapper, Key key, object content = null) {
            lock (_lock) {
                Send(socketWrapper, key, content);
                return Receive(socketWrapper);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="exception"></param>
        /// <param name="tempSendReceiveTimeout">A temporarly timeout for the send and the receive, it will be reset to -1 afterwards.</param>
        /// <returns></returns>
        private static Message<Key> SendAndReceive(string ip, int port, Key key, object content, out Exception exception) {
            exception = null;
            SocketWrapper socketWrapper = null;
            try {
                socketWrapper = Get(ip, port, out exception);
                if (exception == null)
                    return SendAndReceive(socketWrapper, key, content);
            }
            catch (Exception ex) {
                DisconnectSlave(socketWrapper);
                exception = ex;
            }
            return new Message<Key>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="exception"></param>
        /// <param name="tempSendReceiveTimeout">A temporarly timeout for the send and the receive, it will be reset to -1 afterwards.</param>
        /// <returns></returns>
        private static Dictionary<SocketWrapper, Message<Key>> SendAndReceive(Key key, object content, out Exception exception) {
            exception = null;
            List<string> failedFor = new List<string>();
            Dictionary<SocketWrapper, Message<Key>> dictionary = new Dictionary<SocketWrapper, Message<Key>>(_connectedSlaves.Count);
            foreach (SocketWrapper slaveSocketWrapper in _connectedSlaves.Keys) {
                Message<Key> message = new Message<Key>();
                try {
                    message = SendAndReceive(slaveSocketWrapper, key, content);
                }
                catch (Exception ex) {
                    string s = string.Format("{0}:{1}", slaveSocketWrapper.IP, slaveSocketWrapper.Port);
                    failedFor.Add(s);
                    Loggers.Log(Level.Error, "Failed send and receive for " + s, ex, new object[] { key });
                }
                dictionary.Add(slaveSocketWrapper, message);
            }

            if (failedFor.Count != 0) {
                StringBuilder sb = new StringBuilder("Failed send and receive for ");
                for (int j = 0; j < failedFor.Count - 1; j++) {
                    sb.Append(failedFor[j]);
                    sb.Append(", ");
                }
                sb.Append(failedFor[failedFor.Count - 1]);
                sb.Append('.');
                exception = new Exception(sb.ToString());
            }
            return dictionary;
        }

        /// <summary>
        /// The maximum number of connections is 100.
        /// </summary>
        /// <param name="masterSocketWrapper"></param>
        private static void StartListening(SocketWrapper masterSocketWrapper) {
            Socket socket = masterSocketWrapper.Socket;
            socket.Bind(new IPEndPoint(masterSocketWrapper.IP, masterSocketWrapper.Port));
            socket.Listen(100);
            socket.BeginAccept(new AsyncCallback(OnAccept), masterSocketWrapper);
        }
        private static void OnAccept(IAsyncResult ar) {
            try {
                SocketWrapper masterSocketWrapper = ar.AsyncState as SocketWrapper;
                Socket socket = masterSocketWrapper.Socket.EndAccept(ar);

                SocketWrapper socketWrapper = new SocketWrapper(masterSocketWrapper.IP, 1234, socket, SocketFlags.None, SocketFlags.None);
                socketWrapper.SetTag(masterSocketWrapper.Port);
                BeginReceive(socketWrapper);
                masterSocketWrapper.Socket.BeginAccept(new AsyncCallback(OnAccept), masterSocketWrapper);
            }
            catch {
                //Not important, happens when starting a new test after one was run.
            }
            //(Exception ex) {
            //    Loggers.Log(Level.Warning, "Failed on accept. Maybe you cancelled the test?", ex, new object[] { ar });
            //}
        }
        private static void BeginReceive(SocketWrapper socketWrapper) {
            try {
                if (_onReceiveCallBack == null)
                    _onReceiveCallBack = new AsyncCallback(OnReceive);
                socketWrapper.Buffer = new byte[socketWrapper.ReceiveBufferSize];
                socketWrapper.Socket.BeginReceive(socketWrapper.Buffer, 0, socketWrapper.ReceiveBufferSize, SocketFlags.None, _onReceiveCallBack, socketWrapper);
            }
            catch (SocketException soe) {
                InvokeListeningError(socketWrapper, soe);
            }
            catch {
                if (socketWrapper != null && socketWrapper.Socket != null && socketWrapper.Connected)
                    BeginReceive(socketWrapper);
            }
        }
        private static void OnReceive(IAsyncResult result) {
            SocketWrapper socketWrapper = (SocketWrapper)result.AsyncState;
            try {
                socketWrapper.Socket.EndReceive(result);

                Message<Key> message = (Message<Key>)socketWrapper.ByteArrayToObject(socketWrapper.Buffer);

                if (message.Key == Key.SynchronizeBuffers) {
                    socketWrapper.Socket.ReceiveBufferSize = ((SynchronizeBuffersMessage)message.Content).BufferSize;
                    BeginReceive(socketWrapper);
                    socketWrapper.Socket.SendBufferSize = socketWrapper.ReceiveBufferSize;
                }
                else if (message.Key == Key.Push) {
                    BeginReceive(socketWrapper);
                    if (OnTestProgressMessageReceived != null)
                        OnTestProgressMessageReceived(null, new TestProgressMessageReceivedEventArgs((TestProgressMessage)message.Content));
                }
            }
            catch (SocketException soe) {
                Loggers.Log(Level.Info, "Slave listening error. If this happens during a test this is considered an error. Otherwise it works as intended.", soe);
                InvokeListeningError(socketWrapper, soe);
            }
            catch (Exception ex) {

                dynamic buffer = socketWrapper.Buffer;
                try {
                    buffer = System.Text.Encoding.UTF8.GetString(socketWrapper.Buffer).Trim('\0');
                }
                catch {
                    buffer = socketWrapper.Buffer.Combine(",");
                }

                Loggers.Log(Level.Info, "Slave communication error. If this happens during a test this is considered an error. Otherwise it works as intended.", ex, new object[] { socketWrapper.Buffer.Length, buffer });

                if (socketWrapper != null && socketWrapper.Socket != null && socketWrapper.Connected)
                    BeginReceive(socketWrapper);
            }
        }
        private static void InvokeListeningError(SocketWrapper socketWrapper, Exception ex) {
            lock (_lock) {
                try {
                    SocketWrapper slaveSocketWrapper = null;
                    foreach (SocketWrapper sw in _connectedSlaves.Keys) {
                        SocketWrapper masterSocketWrapper = _connectedSlaves[sw];
                        int masterSocketWrapperPort = (int)socketWrapper.GetTag();

                        if (masterSocketWrapper.IP.Equals(socketWrapper.IP) && masterSocketWrapper.Port == masterSocketWrapperPort) {
                            slaveSocketWrapper = sw;
                            break;
                        }
                    }

                    DisconnectSlave(slaveSocketWrapper);

                    if (ListeningError != null)
                        ListeningError.Invoke(null, new ListeningErrorEventArgs(slaveSocketWrapper.IP.ToString(), slaveSocketWrapper.Port, ex));
                }
                catch {
                    //If not handled later on, the gui was closed.
                }
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// Before everything else do this first.
        /// </summary>
        public static void Init() {
            _lock = null;
            GC.WaitForPendingFinalizers();
            GC.Collect();

            _lock = new object();

            DisconnectSlaves();
            _onReceiveCallBack = null;

            _continueCounter = -1;
        }
        /// <summary>
        /// Connects, sends a poll and adds to the collection (if not there already).
        /// The retry count is 3.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="processID">-1 for already connected.</param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static SocketWrapper ConnectSlave(string ip, int port, out int processID, out Exception exception) {
            SocketWrapper socketWrapper = null;
            exception = null;
            processID = -1;
            try {
                socketWrapper = Get(ip, port);
                if (socketWrapper == null) {
                    var address = IPAddress.Parse(ip);
                    var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socketWrapper = new SocketWrapper(address, port, socket, SocketFlags.None, SocketFlags.None);
                }
                ConnectSlave(socketWrapper, out processID, out exception);
            }
            catch (Exception ex) {
                exception = ex;
            }
            return socketWrapper;
        }
        /// <summary>
        /// Disconnects all slaves
        /// </summary>
        public static void DisconnectSlaves() {
            foreach (SocketWrapper slaveSocketWrapper in _connectedSlaves.Keys)
                if (slaveSocketWrapper != null) {
                    try {
                        var masterSocketWrapper = _connectedSlaves[slaveSocketWrapper];
                        masterSocketWrapper.Close();
                        masterSocketWrapper = null;
                    }
                    catch {
                        //Don't care.
                    }

                    try {
                        slaveSocketWrapper.Close();
                    }
                    catch {
                        //Don't care.
                    }

                }
            _connectedSlaves.Clear();
        }
        /// <summary>
        /// Disconnect a certain slave.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public static void DisconnectSlave(string ip, int port) {
            DisconnectSlave(Get(ip, port));
        }
        /// <summary>
        /// Will begin listening after this.
        /// 
        /// No timeouts (except for the synchronization of the buffers (30 seconds)), this can take a while.
        /// </summary>
        public static Exception InitializeTests(Dictionary<TileStressTest, TileStressTest> dividedAndOriginalTileStressTests, RunSynchronization runSynchronization, int maxRerunsBreakOnLast) {
            Exception exception = null;
            var initializeTestData = new InitializeTestWorkItem.InitializeTestData[dividedAndOriginalTileStressTests.Count];
            var functionOutputCache = new FunctionOutputCache();

            int monitorBefore = 0, monitorAfter = 0;
            foreach (var tileStressTest in dividedAndOriginalTileStressTests.Keys)
                if (tileStressTest.BasicTileStressTest.Monitors.Length != 0) {
                    var atst = tileStressTest.AdvancedTileStressTest;
                    if (atst.MonitorBefore > monitorBefore) monitorBefore = atst.MonitorBefore;
                    if (atst.MonitorAfter > monitorAfter) monitorAfter = atst.MonitorAfter;
                }

            int tileStressTestIndex = 0;
            foreach (var tileStressTest in dividedAndOriginalTileStressTests.Keys) {
                var slave = tileStressTest.BasicTileStressTest.Slaves[0];
                Exception ex;
                var socketWrapper = Get(slave.IP, slave.Port, out ex);
                if (ex != null) {
                    exception = ex;
                    break;
                }
                var masterSocketWrapper = _connectedSlaves[socketWrapper];

                var pushIPs = new List<string>();

                //Dns.GetHostName() does not always work.
                string hostName = Dns.GetHostEntry("127.0.0.1").HostName.Trim().Split('.')[0].ToLowerInvariant();

                foreach (var ipAddress in Dns.GetHostAddresses(hostName)) {
                    if (ipAddress.AddressFamily == masterSocketWrapper.IP.AddressFamily)
                        pushIPs.Add(ipAddress.ToString());
                }
                var initializeTestMessage = new InitializeTestMessage() {
                    StressTestWrapper = tileStressTest.GetStressTestWrapper(functionOutputCache, runSynchronization, maxRerunsBreakOnLast, monitorBefore, monitorAfter),
                    PushIPs = pushIPs.ToArray(),
                    PushPort = masterSocketWrapper.Port
                };

                initializeTestData[tileStressTestIndex] = new InitializeTestWorkItem.InitializeTestData() { SocketWrapper = socketWrapper, InitializeTestMessage = initializeTestMessage };
                ++tileStressTestIndex;
            }
            functionOutputCache.Dispose();
            functionOutputCache = null;

            if (initializeTestData.Length != 0 && exception == null) {
                AutoResetEvent waitHandle = new AutoResetEvent(false);
                int count = initializeTestData.Length;
                int done = 0;
                for (int i = 0; i != count; i++) {
                    Thread t = new Thread(delegate (object parameter) {
                        try {
                            int index = (int)parameter;

                            _initializeTestWorkItem = new InitializeTestWorkItem();
                            var ex = _initializeTestWorkItem.InitializeTest(initializeTestData[index]);

                            lock (_lock) {
                                if (ex == null) {
                                    ++done;
                                }
                                else {
                                    exception = ex;
                                    done = count;
                                }
                            }
                            if (done >= count && waitHandle != null)
                                try { waitHandle.Set(); }
                                catch {
                                    //Handled later on.
                                }
                        }
                        catch {
                            //Handled later on.
                        }
                    });
                    t.IsBackground = true;
                    t.Priority = ThreadPriority.Highest;
                    t.Start(i);
                }

                waitHandle.WaitOne();
                waitHandle.Dispose();
                waitHandle = null;
            }

            return exception;
        }

        /// <summary>
        /// Will start the test on all connected slaves.
        /// 
        /// The retry count is 3 with a send and a receive timeout of 30 seconds.
        /// </summary>
        /// <param name="exception"></param>
        public static void StartTest(out Exception exception) {
            Exception e = null;
            Parallel.ForEach(_connectedSlaves.Keys, delegate (SocketWrapper socketWrapper) {
                for (int i = 1; i != 4; i++)
                    try {
                        Message<Key> message = SendAndReceive(socketWrapper, Key.StartTest, 30000);

                        StartAndStopMessage startMessage = (StartAndStopMessage)message.Content;
                        if (startMessage.Exception != null)
                            throw new Exception(startMessage.Exception);
                        e = null;
                        break;
                    }
                    catch (Exception ex) {
                        e = new Exception("Failed to start the test on " + socketWrapper.IP + ":" + socketWrapper.Port + ":\n" + ex);
                    }
            });

            exception = e;
        }
        public static void SendBreak() {
            Exception exception = null;
            SendAndReceive(Key.Break, null, out exception);
        }
        public static void SendContinue() {
            Exception exception = null;
            ContinueMessage continueMessage;
            continueMessage.ContinueCounter = ++_continueCounter;

            SendAndReceive(Key.Continue, continueMessage, out exception);
        }

        public static void SendDividedContinue(Slave[] slaves) {
            Exception exception = null;
            foreach (var slave in slaves)
                SendAndReceive(slave.IP, slave.Port, Key.ContinueDivided, null, out exception);
        }

        /// <summary>
        /// The retry count is 3 with a send and a receive timeout of 30 seconds.
        /// </summary>
        public static Exception[] StopTest() {
            lock (_stopLock) {
                var exceptions = new ConcurrentBag<Exception>();
                var stopped = new ConcurrentBag<SocketWrapper>();

                int length = _connectedSlaves.Count;

                if (length != 0)
                    for (int i = 0; i != 3; i++) //Retry for the ones that are not stopped
                    {
                        exceptions = new ConcurrentBag<Exception>();

                        if (stopped.Count == length)
                            break;

                        AutoResetEvent waitHandle = new AutoResetEvent(false);
                        int handled = 0;

                        foreach (SocketWrapper socketWrapper in _connectedSlaves.Keys)
                            if (!stopped.Contains(socketWrapper)) {
                                Thread t = new Thread(delegate (object parameter) {
                                    try {
                                        if (_stopTestWorkItem == null) _stopTestWorkItem = new StopTestWorkItem();
                                        _stopTestWorkItem.StopTest(parameter as SocketWrapper, ref exceptions, ref stopped);

                                        if (Interlocked.Increment(ref handled) == length && waitHandle != null)
                                            try { waitHandle.Set(); }
                                            catch {
                                                //Ignore.
                                            }
                                    }
                                    catch (Exception ex) {
                                        Loggers.Log(Level.Error, "Failed stopping test.", ex);
                                    }
                                    finally {
                                        _stopTestWorkItem = null;
                                    }
                                });
                                t.IsBackground = true;
                                t.Start(socketWrapper);
                            }

                        waitHandle.WaitOne(10000);
                        waitHandle.Dispose();
                        waitHandle = null;

                        if (exceptions.Count == 0)
                            break;
                    }

                List<Exception> l = new List<Exception>();
                foreach (Exception ex in exceptions)
                    if (ex != null)
                        l.Add(ex);

                return l.ToArray();
            }
        }
        #endregion

        #endregion

        #region Work items
        public class InitializeTestWorkItem {
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public Exception InitializeTest(InitializeTestWorkItem.InitializeTestData initializeTestData) {
                Exception exception = null;
                try {
                    var socketWrapper = initializeTestData.SocketWrapper;
                    var initializeTestMessage = initializeTestData.InitializeTestMessage;

                    var message = new Message<Key>(Key.InitializeTest, initializeTestMessage);

                    //Increases the buffer size, never decreases it.
                    byte[] buffer = SynchronizeBuffers(socketWrapper, message);

                    socketWrapper.SendBytes(buffer);
                    message = (Message<Key>)socketWrapper.Receive(SendType.Binary);
                    buffer = null;

                    initializeTestMessage = (InitializeTestMessage)message.Content;

                    //Reset the buffers to keep the messages as small as possible.
                    ResetBuffers(socketWrapper);
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    if (initializeTestMessage.Exception != null) throw new Exception(initializeTestMessage.Exception);
                }
                catch (Exception ex) {
                    exception = ex;
                }
                // InvokeTestInitialized(tileStressTest, exception);

                return exception;
            }

            /// <summary>
            /// Increases the buffer size if needed, never decreases it. Use ResetBuffers for that.            /// 
            /// </summary>
            /// <param name="socketWrapper"></param>
            /// <param name="toSend"></param>
            private byte[] SynchronizeBuffers(SocketWrapper socketWrapper, object toSend) {
                byte[] buffer = socketWrapper.ObjectToByteArray(toSend);
                int bufferSize = buffer.Length;
                if (bufferSize > socketWrapper.SendBufferSize) {
                    socketWrapper.SendBufferSize = bufferSize;
                    socketWrapper.ReceiveBufferSize = socketWrapper.SendBufferSize;
                    SynchronizeBuffersMessage synchronizeBuffersMessage = new SynchronizeBuffersMessage();
                    synchronizeBuffersMessage.BufferSize = socketWrapper.SendBufferSize;

                    SendAndReceive(socketWrapper, Key.SynchronizeBuffers, synchronizeBuffersMessage);
                }
                return buffer;
            }
            /// <summary>
            /// Set the buffer size to the default buffer size (SocketWrapper.DEFAULTBUFFERSIZE).
            /// This will set the buffer size of the socket on the other end too.
            /// </summary>
            /// <param name="socketWrapper"></param>
            private void ResetBuffers(SocketWrapper socketWrapper) {
                socketWrapper.SendBufferSize = SocketWrapper.DEFAULTBUFFERSIZE;
                socketWrapper.ReceiveBufferSize = SocketWrapper.DEFAULTBUFFERSIZE;
                SynchronizeBuffersMessage synchronizeBuffersMessage = new SynchronizeBuffersMessage();
                synchronizeBuffersMessage.BufferSize = SocketWrapper.DEFAULTBUFFERSIZE;

                SendAndReceive(socketWrapper, Key.SynchronizeBuffers, synchronizeBuffersMessage);
            }

            public class InitializeTestData {
                public SocketWrapper SocketWrapper;
                public InitializeTestMessage InitializeTestMessage;
            }
        }
        private class StopTestWorkItem {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="socketWrapper"></param>
            /// <param name="exceptions">Add to exceptions on exception.</param>
            /// <param name="stopped">Add to stopped on succesful stop.</param>
            public void StopTest(SocketWrapper socketWrapper, ref ConcurrentBag<Exception> exceptions, ref ConcurrentBag<SocketWrapper> stopped) {
                if (socketWrapper != null)
                    try {
                        Message<Key> message = SendAndReceive(socketWrapper, Key.StopTest, 30000);

                        StartAndStopMessage stopMessage = (StartAndStopMessage)message.Content;
                        if (stopMessage.Exception != null)
                            throw new Exception(stopMessage.Exception);

                        if (!stopped.Contains(socketWrapper))
                            stopped.Add(socketWrapper);
                    }
                    catch (Exception ex) {
                        exceptions.Add(new Exception("Failed to stop the test on " + socketWrapper.IP + ":" + socketWrapper.Port + ":\n" + ex));
                    }
            }
        }
        #endregion
    }
}