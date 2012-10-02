/*
 * Copyright 2010 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public static class MasterSideCommunicationHandler
    {
        #region Events
        public static event EventHandler<ListeningErrorEventArgs> ListeningError;
        public static event EventHandler<TestProgressMessageReceivedEventArgs> OnTestProgressMessageReceived;

        public static event EventHandler<TestInitializedEventArgs> TestInitialized;
        #endregion

        #region Fields
        private static object _lock = new object();
        private  static object _stopLock = new object();

        //A slave side and a master side socket wrappers for full duplex communication.
        private static Dictionary<SocketWrapper, SocketWrapper> _connectedSlaves = new Dictionary<SocketWrapper, SocketWrapper>();
        private static AsyncCallback _onReceiveCallBack;
        private static IAsyncResult _asyncResult;

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
        /// The retry count for connecting is 3 and the connect timeout is 30 seconds.
        /// </summary>
        /// <param name="slaveSocketWrapper"></param>
        /// <param name="processID">-1 for already connected.</param>
        /// <param name="exception"></param>
        private static void ConnectSlave(SocketWrapper slaveSocketWrapper, out int processID, out Exception exception)
        {
            processID = -1;
            exception = null;
            try
            {
                exception = null;
                if (!slaveSocketWrapper.Connected)
                {
                    slaveSocketWrapper.Connect(30000, 2);
                    if (slaveSocketWrapper.Connected)
                    {
                        var masterSocketWrapper = GetMasterSocketWrapper(slaveSocketWrapper);

                        Message<Key> message = SendAndReceive(slaveSocketWrapper, Key.Poll, null, 30000);
                        PollMessage pollMessage = (PollMessage)message.Content;
                        processID = pollMessage.ProcessID;

                        lock (_lock)
                            if (!_connectedSlaves.ContainsKey(slaveSocketWrapper))
                                _connectedSlaves.Add(slaveSocketWrapper, masterSocketWrapper);
                    }
                    else
                    {
                        throw (new Exception(string.Format("Could not connect {0}:{1}.", slaveSocketWrapper.IP.ToString(), slaveSocketWrapper.Port.ToString())));
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }
        /// <summary>
        /// Will start listening too.
        /// </summary>
        /// <param name="slaveSocketWrapper"></param>
        /// <returns></returns>
        private static SocketWrapper GetMasterSocketWrapper(SocketWrapper slaveSocketWrapper)
        {
            Exception exception = null;

            SocketWrapper masterSocketWrapper = null;
            lock (_lock)
                if (_connectedSlaves.ContainsKey(slaveSocketWrapper))
                    masterSocketWrapper = _connectedSlaves[slaveSocketWrapper];

            if (masterSocketWrapper == null)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                string ip = SocketListener.GetInstance().IP;

                int start = slaveSocketWrapper.Port * 10;
                int stop = int.MaxValue;

                for (int i = start; i != stop; i++)
                {
                    try
                    {
                        masterSocketWrapper = new SocketWrapper(ip, i, socket);
                        StartListening(masterSocketWrapper);
                        exception = null;
                        break;
                    }
                    catch (Exception ex)
                    {
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
        private static void DisconnectSlave(SocketWrapper slaveSocketWrapper)
        {
            try
            {
                foreach (SocketWrapper socketWrapper in _connectedSlaves.Keys)
                    if (socketWrapper == slaveSocketWrapper)
                    {
                        if (slaveSocketWrapper != null)
                        {
                            try
                            {
                                var masterSocketWrapper = _connectedSlaves[slaveSocketWrapper];
                                masterSocketWrapper.Close();
                                masterSocketWrapper = null;
                            }
                            catch { }

                            try
                            {
                                slaveSocketWrapper.Close();
                            }
                            catch { }
                        }
                        _connectedSlaves.Remove(slaveSocketWrapper);
                        slaveSocketWrapper = null;
                        break;
                    }
            }
            catch { }
        }
        /// <summary>
        /// Gets the slave socket wrapper from the connected slaves. Returns null if not found.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        private static SocketWrapper Get(string ip, int port)
        {
            lock (_lock)
            {
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
        private static SocketWrapper Get(string ip, int port, out Exception exception)
        {
            exception = null;
            int processID;
            SocketWrapper socketWrapper = Get(ip, port);
            if (socketWrapper != null && !socketWrapper.Connected)
                ConnectSlave(socketWrapper, out processID, out exception);

            if (socketWrapper == null || !socketWrapper.Connected)
                exception = new Exception(string.Format("Not connected to {0}:{1}.", ip, port));
            return socketWrapper;
        }
        /// <summary>
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="message"></param>
        /// <param name="tempSendTimeout">A temporarly timeout for the send, it will be reset to -1 afterwards.</param>
        private static void Send(SocketWrapper socketWrapper, Message<Key> message, int tempSendTimeout = -1)
        {
            socketWrapper.SendTimeout = tempSendTimeout;
            socketWrapper.Send(message, SendType.Binary);
            socketWrapper.SendTimeout = -1;
        }
        /// <summary>
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="tempSendTimeout">A temporarly timeout for the send, it will be reset to -1 afterwards.</param>
        private static void Send(SocketWrapper socketWrapper, Key key, object content, int tempSendTimeout = -1)
        {
            Send(socketWrapper, new Message<Key>(key, content), tempSendTimeout);
        }
        /// <summary>
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="exception"></param>
        /// <param name="tempSendTimeout">A temporarly timeout for the send, it will be reset to -1 afterwards.</param>
        private static void Send(string ip, int port, Key key, object content, out Exception exception, int tempSendTimeout = -1)
        {
            exception = null;
            SocketWrapper socketWrapper = null;
            try
            {
                socketWrapper = Get(ip, port, out exception);
                if (exception == null)
                    Send(socketWrapper, key, content, tempSendTimeout);
            }
            catch (Exception ex)
            {
                DisconnectSlave(socketWrapper);
                exception = ex;
            }
        }
        /// <summary>
        /// Send to all slaves.
        /// Will connect all the slaves if not connected.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="exception"></param>
        /// <param name="tempSendTimeout">A temporarly timeout for the send, it will be reset to -1 afterwards.</param>
        private static void Send(Key key, object content, out Exception exception, int tempSendTimeout = -1)
        {
            exception = null;
            int processID;
            List<string> failedFor = new List<string>();
            Dictionary<SocketWrapper, Message<Key>> dictionary = new Dictionary<SocketWrapper, Message<Key>>(_connectedSlaves.Count);
            foreach (SocketWrapper slaveSocketWrapper in _connectedSlaves.Keys)
            {
                try
                {
                    ConnectSlave(slaveSocketWrapper, out processID, out exception);
                    Send(slaveSocketWrapper, key, content, tempSendTimeout);
                }
                catch
                {
                    failedFor.Add(string.Format("{0}:{1}", slaveSocketWrapper.IP, slaveSocketWrapper.Port));
                }
            }

            if (failedFor.Count > 0)
            {
                StringBuilder sb = new StringBuilder("Failed send and receive for ");
                for (int j = 0; j < failedFor.Count - 1; j++)
                {
                    sb.Append(failedFor[j]);
                    sb.Append(", ");
                }
                sb.Append(failedFor[failedFor.Count - 1]);
                sb.Append('.');
                exception = new Exception(sb.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="tempReceiveTimeout">A temporarly timeout for the receive, it will be reset to -1 afterwards.</param>
        /// <returns></returns>
        private static Message<Key> Receive(SocketWrapper socketWrapper, int tempReceiveTimeout = -1)
        {
            socketWrapper.ReceiveTimeout = tempReceiveTimeout;
            var message = (Message<Key>)socketWrapper.Receive(SendType.Binary);
            socketWrapper.ReceiveTimeout = -1;
            return message;
        }
        /// <summary>
        /// Thread safe
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="message"></param>
        /// <param name="tempSendReceiveTimeout">A temporarly timeout for the send and the receive, it will be reset to -1 afterwards.</param>
        /// <returns></returns>
        private static Message<Key> SendAndReceive(SocketWrapper socketWrapper, Message<Key> message, int tempSendReceiveTimeout = -1)
        {
            lock (_lock)
            {
                Send(socketWrapper, message, tempSendReceiveTimeout);
                return Receive(socketWrapper, tempSendReceiveTimeout);
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
        private static Message<Key> SendAndReceive(SocketWrapper socketWrapper, Key key, object content, int tempSendReceiveTimeout = -1)
        {
            lock (_lock)
            {
                Send(socketWrapper, key, content, tempSendReceiveTimeout);
                return Receive(socketWrapper, tempSendReceiveTimeout);
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
        private static Message<Key> SendAndReceive(string ip, int port, Key key, object content, out Exception exception, int tempSendReceiveTimeout = -1)
        {
            exception = null;
            SocketWrapper socketWrapper = null;
            try
            {
                socketWrapper = Get(ip, port, out exception);
                if (exception == null)
                    return SendAndReceive(socketWrapper, key, content, tempSendReceiveTimeout);
            }
            catch (Exception ex)
            {
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
        private static Dictionary<SocketWrapper, Message<Key>> SendAndReceive(Key key, object content, out Exception exception, int tempSendReceiveTimeout = -1)
        {
            exception = null;
            List<string> failedFor = new List<string>();
            Dictionary<SocketWrapper, Message<Key>> dictionary = new Dictionary<SocketWrapper, Message<Key>>(_connectedSlaves.Count);
            foreach (SocketWrapper slaveSocketWrapper in _connectedSlaves.Keys)
            {
                Message<Key> message = new Message<Key>();
                try
                {
                    message = SendAndReceive(slaveSocketWrapper, key, content, tempSendReceiveTimeout);
                }
                catch
                {
                    failedFor.Add(string.Format("{0}:{1}", slaveSocketWrapper.IP, slaveSocketWrapper.Port));
                }
                dictionary.Add(slaveSocketWrapper, message);
            }

            if (failedFor.Count != 0)
            {
                StringBuilder sb = new StringBuilder("Failed send and receive for ");
                for (int j = 0; j < failedFor.Count - 1; j++)
                {
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
        private static void StartListening(SocketWrapper masterSocketWrapper)
        {
            Socket socket = masterSocketWrapper.Socket;
            socket.Bind(new IPEndPoint(masterSocketWrapper.IP, masterSocketWrapper.Port));
            socket.Listen(100);
            socket.BeginAccept(new AsyncCallback(OnAccept), masterSocketWrapper);
        }
        private static void OnAccept(IAsyncResult ar)
        {
            try
            {
                SocketWrapper masterSocketWrapper = ar.AsyncState as SocketWrapper;
                Socket socket = masterSocketWrapper.Socket.EndAccept(ar);

                SocketWrapper socketWrapper = new SocketWrapper(masterSocketWrapper.IP, 1234, socket, SocketFlags.None, SocketFlags.None);
                socketWrapper.SetTag(masterSocketWrapper.Port);
                BeginReceive(socketWrapper);
                masterSocketWrapper.Socket.BeginAccept(new AsyncCallback(OnAccept), masterSocketWrapper);
            }
            catch
            { }
        }
        private static void BeginReceive(SocketWrapper socketWrapper)
        {
            try
            {
                if (_onReceiveCallBack == null)
                    _onReceiveCallBack = new AsyncCallback(OnReceive);
                socketWrapper.Buffer = new byte[socketWrapper.ReceiveBufferSize];
                socketWrapper.Socket.BeginReceive(socketWrapper.Buffer, 0, socketWrapper.ReceiveBufferSize, SocketFlags.None, _onReceiveCallBack, socketWrapper);
            }
            catch (SocketException soe)
            {
                InvokeListeningError(socketWrapper, soe);
            }
            catch
            {
                if (socketWrapper != null && socketWrapper.Socket != null && socketWrapper.Connected)
                    BeginReceive(socketWrapper);
            }
        }
        private static void OnReceive(IAsyncResult result)
        {
            _asyncResult = result;
            SocketWrapper socketWrapper = (SocketWrapper)result.AsyncState;
            try
            {
                socketWrapper.Socket.EndReceive(result);

                Message<Key> message = (Message<Key>)socketWrapper.ByteArrayToObject(socketWrapper.Buffer);

                if (message.Key == Key.SynchronizeBuffers)
                {
                    socketWrapper.Socket.ReceiveBufferSize = ((SynchronizeBuffersMessage)message.Content).BufferSize;
                    BeginReceive(socketWrapper);
                    socketWrapper.Socket.SendBufferSize = socketWrapper.ReceiveBufferSize;
                }
                else if (message.Key == Key.Push)
                {
                    BeginReceive(socketWrapper);
                    if (OnTestProgressMessageReceived != null)
                        OnTestProgressMessageReceived(null, new TestProgressMessageReceivedEventArgs((TestProgressMessage)message.Content));
                }
            }
            catch (SocketException soe)
            {
                InvokeListeningError(socketWrapper, soe);
            }
            catch
            {
                if (socketWrapper != null && socketWrapper.Socket != null && socketWrapper.Connected)
                    BeginReceive(socketWrapper);
            }
        }
        private static void InvokeListeningError(SocketWrapper socketWrapper, Exception ex)
        {
            lock (_lock)
            {
                try
                {
                    SocketWrapper slaveSocketWrapper = null;
                    foreach (SocketWrapper sw in _connectedSlaves.Keys)
                    {
                        SocketWrapper masterSocketWrapper = _connectedSlaves[sw];
                        int masterSocketWrapperPort = (int)socketWrapper.GetTag();

                        if (masterSocketWrapper.IP.Equals(socketWrapper.IP) && masterSocketWrapper.Port == masterSocketWrapperPort)
                        {
                            slaveSocketWrapper = sw;
                            break;
                        }
                    }

                    DisconnectSlave(slaveSocketWrapper);

                    if (ListeningError != null)
                        ListeningError.Invoke(null, new ListeningErrorEventArgs(slaveSocketWrapper.IP.ToString(), slaveSocketWrapper.Port, ex));
                }
                catch { }
            }
        }
        private static void InvokeTestInitialized(TileStresstest tileStresstest, Exception ex)
        {
            lock (_lock)
            {
                if (TestInitialized != null)
                    foreach (EventHandler<TestInitializedEventArgs> del in TestInitialized.GetInvocationList())
                        del.BeginInvoke(null, new TestInitializedEventArgs(tileStresstest, ex), null, null);
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// Before everything else do this first.
        /// </summary>
        public static void Init()
        {
            _lock = null;
            GC.Collect();
            _lock = new object();

            DisconnectSlaves();
            _onReceiveCallBack = null;
            _asyncResult = null;

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
        public static SocketWrapper ConnectSlave(string ip, int port, out int processID, out Exception exception)
        {
            SocketWrapper socketWrapper = null;
            exception = null;
            processID = -1;
            try
            {
                socketWrapper = Get(ip, port);
                if (socketWrapper == null)
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketWrapper = new SocketWrapper(ip, port, socket, SocketFlags.None, SocketFlags.None);
                }
                ConnectSlave(socketWrapper, out processID, out exception);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            return socketWrapper;
        }
        /// <summary>
        /// Disconnects all slaves
        /// </summary>
        public static void DisconnectSlaves()
        {
            foreach (SocketWrapper slaveSocketWrapper in _connectedSlaves.Keys)
                if (slaveSocketWrapper != null)
                {
                    try
                    {
                        var masterSocketWrapper = _connectedSlaves[slaveSocketWrapper];
                        masterSocketWrapper.Close();
                        masterSocketWrapper = null;
                    }
                    catch { }

                    try
                    {
                        slaveSocketWrapper.Close();
                    }
                    catch { }

                }
            _connectedSlaves.Clear();
        }
        /// <summary>
        /// Disconnect a certain slave.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public static void DisconnectSlave(string ip, int port)
        {
            DisconnectSlave(Get(ip, port));
        }
        /// <summary>
        /// Will begin listening after this.
        /// 
        /// No timeouts (except for the synchronization of the buffers (30 seconds)), this can take a while.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="tileStresstest"></param>
        /// <param name="exception"></param>
        public static Exception[] InitializeTests(TileStresstest[] tileStresstests, RunSynchronization runSynchronization)
        {
            ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();
            int length = tileStresstests.Length;

            if (length != 0)
            {
                AutoResetEvent waitHandle = new AutoResetEvent(false);
                int handled = 0;
                for (int i = 0; i != length; i++)
                {
                    Thread t = new Thread(delegate(object parameter)
                    {
                        _initializeTestWorkItem = new InitializeTestWorkItem();
                        exceptions.Add(
                            _initializeTestWorkItem.InitializeTest(tileStresstests[(int)parameter], runSynchronization)
                        );
                        _initializeTestWorkItem = null;

                        if (Interlocked.Increment(ref handled) == tileStresstests.Length)
                            waitHandle.Set();
                    });
                    t.IsBackground = true;
                    t.Start(i);
                }

                waitHandle.WaitOne();
                waitHandle.Dispose();
                waitHandle = null;
            }

            List<Exception> l = new List<Exception>();
            foreach (Exception ex in exceptions)
                if (ex != null)
                    l.Add(ex);

            return l.ToArray();
        }

        /// <summary>
        /// Will start the test on all connected slaves.
        /// 
        /// The retry count is 3 with a send and a receive timeout of 30 seconds.
        /// </summary>
        /// <param name="exception"></param>
        public static void StartTest(out Exception exception)
        {
            Exception e = null;
            Parallel.ForEach(_connectedSlaves.Keys, delegate(SocketWrapper socketWrapper)
            {
                for (int i = 1; i != 4; i++)
                    try
                    {
                        Message<Key> message = SendAndReceive(socketWrapper, Key.StartTest, 30000);

                        StartAndStopMessage startMessage = (StartAndStopMessage)message.Content;
                        if (startMessage.Exception != null)
                            throw new Exception(startMessage.Exception);
                        e = null;
                        break;
                    }
                    catch (Exception ex)
                    {
                        e = new Exception("Failed to start the test on " + socketWrapper.IP + ":" + socketWrapper.Port + ":\n" + ex);
                        Thread.Sleep(i * 500);
                    }
            });

            exception = e;
        }
        public static void SendBreak()
        {
            Exception exception = null;
            SendAndReceive(Key.Break, null, out exception, 30000);
        }
        public static void SendContinue()
        {
            Exception exception = null;
            ContinueMessage continueMessage;
            continueMessage.ContinueCounter = ++_continueCounter;

            SendAndReceive(Key.Continue, continueMessage, out exception, 30000);
        }

        /// <summary>
        /// The retry count is 3 with a send and a receive timeout of 30 seconds.
        /// </summary>
        /// <param name="tileStresstest"></param>
        /// <param name="exception"></param>
        public static Exception[] StopTest()
        {
            lock (_stopLock)
            {
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
                            if (!stopped.Contains(socketWrapper))
                            {
                                Thread t = new Thread(delegate(object parameter)
                                {
                                    _stopTestWorkItem = new StopTestWorkItem();
                                    _stopTestWorkItem.StopTest(parameter as SocketWrapper, ref exceptions, ref stopped);
                                    _stopTestWorkItem = null;

                                    if (Interlocked.Increment(ref handled) == length && waitHandle != null)
                                        waitHandle.Set();
                                });
                                t.IsBackground = true;
                                t.Start(socketWrapper);
                            }

                        waitHandle.WaitOne(5000);
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
        /// <summary>
        /// Only use after the test is stopped.
        /// If the result getting fails for a certain slave an empty message is returned.
        ///
        /// The retry count is 3 with a send and a receive timeout of 30 seconds.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static List<ResultsMessage> GetResults(out Exception exception)
        {
            Exception e = null;
            List<ResultsMessage> l = new List<ResultsMessage>(_connectedSlaves.Count);

            Parallel.ForEach(_connectedSlaves.Keys, delegate(SocketWrapper socketWrapper)
            {
                if (socketWrapper != null && socketWrapper.Connected)
                {
                    ResultsMessage resultsMessage = new ResultsMessage();
                    for (int i = 1; i != 4; i++)
                        try
                        {
                            object data = SendAndReceive(socketWrapper, Key.Results, 30000).Content;
                            while (data is SynchronizeBuffersMessage)
                            {
                                socketWrapper.Socket.ReceiveBufferSize = ((SynchronizeBuffersMessage)data).BufferSize;
                                data = Receive(socketWrapper, 30000).Content;
                            }
                            resultsMessage = (ResultsMessage)data;
                            if (resultsMessage.Exception != null)
                                throw new Exception(resultsMessage.Exception);

                            break;
                        }
                        catch (Exception ex)
                        {
                            e = new Exception("Failed to get the test results from " + socketWrapper.IP + ":" + socketWrapper.Port + ":\n" + ex);
                            Thread.Sleep(i * 500);
                        }

                    lock (_lock)
                        l.Add(resultsMessage);
                }
            });

            exception = e;
            return l;
        }
        /// <summary>
        /// The retry count is 3 with a send timeout of 30 seconds.
        /// </summary>
        /// <param name="tileStresstest"></param>
        /// <param name="torrentName"></param>
        /// <param name="exception"></param>
        public static void StopSeedingResults(TileStresstest tileStresstest, out Exception exception)
        {
#warning Allow multiple slaves for work distribution
            Slave slave = tileStresstest.BasicTileStresstest.Slaves[0];
            SocketWrapper socketWrapper = Get(slave.IP, slave.Port, out exception);
            if (exception == null)
                for (int i = 1; i != 4; i++)
                    try
                    {
                        Send(socketWrapper, Key.StopSeedingResults, 30000);
                        break;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        Thread.Sleep(i * 500);
                    }
        }
        #endregion

        #endregion

        #region Work items
        private class InitializeTestWorkItem
        {
            public Exception InitializeTest(TileStresstest tileStresstest, RunSynchronization runSynchronization)
            {
                Exception exception = null;

#warning Allow multiple slaves for work distribution
                Slave slave = tileStresstest.BasicTileStresstest.Slaves[0];
                SocketWrapper socketWrapper = Get(slave.IP, slave.Port, out exception);
                if (exception == null)
                    try
                    {
                        StresstestWrapper stresstestWrapper = tileStresstest.GetStresstestWrapper(runSynchronization);

                        InitializeTestMessage initializeTestMessage = new InitializeTestMessage();
                        initializeTestMessage.StresstestWrapper = stresstestWrapper;

                        SocketWrapper masterSocketWrapper = _connectedSlaves[socketWrapper];
                        initializeTestMessage.PushIP = masterSocketWrapper.IP.ToString();
                        initializeTestMessage.PushPort = masterSocketWrapper.Port;

                        Message<Key> message = new Message<Key>(Key.InitializeTest, initializeTestMessage);

                        //Increases the buffer size, never decreases it.
                        SynchronizeBuffers(socketWrapper, message);

                        //message = SendAndReceive(socketWrapper, message);
                        socketWrapper.Send(message, SendType.Binary);
                        message = (Message<Key>)socketWrapper.Receive(SendType.Binary);

                        initializeTestMessage = (InitializeTestMessage)message.Content;

                        //Reset the buffers to keep the messages as small as possible.
                        ResetBuffers(socketWrapper);
                        if (initializeTestMessage.Exception != null)
                            throw new Exception(initializeTestMessage.Exception);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                // InvokeTestInitialized(tileStresstest, exception);

                return exception;
            }

            /// <summary>
            /// Increases the buffer size if needed, never decreases it. Use ResetBuffers for that.
            /// 
            /// This has a timout of 30 seconds.
            /// </summary>
            /// <param name="socketWrapper"></param>
            /// <param name="toSend"></param>
            private void SynchronizeBuffers(SocketWrapper socketWrapper, object toSend)
            {
                byte[] buffer = socketWrapper.ObjectToByteArray(toSend);
                int bufferSize = buffer.Length;
                if (bufferSize > socketWrapper.SendBufferSize)
                {
                    socketWrapper.SendBufferSize = bufferSize;
                    socketWrapper.ReceiveBufferSize = socketWrapper.SendBufferSize;
                    SynchronizeBuffersMessage synchronizeBuffersMessage = new SynchronizeBuffersMessage();
                    synchronizeBuffersMessage.BufferSize = socketWrapper.SendBufferSize;

                    //Sync the buffers with a temp receive timeout.
                    int receiveTimeout = socketWrapper.ReceiveTimeout;
                    int tempReceiveTimeout = 30000;
                    socketWrapper.ReceiveTimeout = tempReceiveTimeout;
                    socketWrapper.Send(new Message<Key>(Key.SynchronizeBuffers, synchronizeBuffersMessage), SendType.Binary);
                    socketWrapper.Receive(SendType.Binary);
                    socketWrapper.ReceiveTimeout = receiveTimeout;
                }
            }
            /// <summary>
            /// Set the buffer size to the default buffer size (SocketWrapper.DEFAULTBUFFERSIZE).
            /// This will set the buffer size of the socket on the other end too.
            ///
            /// This has a timout of 30 seconds.
            /// </summary>
            /// <param name="socketWrapper"></param>
            private void ResetBuffers(SocketWrapper socketWrapper)
            {
                socketWrapper.SendBufferSize = SocketWrapper.DEFAULTBUFFERSIZE;
                socketWrapper.ReceiveBufferSize = SocketWrapper.DEFAULTBUFFERSIZE;
                SynchronizeBuffersMessage synchronizeBuffersMessage = new SynchronizeBuffersMessage();
                synchronizeBuffersMessage.BufferSize = SocketWrapper.DEFAULTBUFFERSIZE;


                //Sync the buffers with a temp receive timeout.
                int receiveTimeout = socketWrapper.ReceiveTimeout;
                int tempReceiveTimeout = 30000;
                socketWrapper.ReceiveTimeout = tempReceiveTimeout;
                socketWrapper.Send(new Message<Key>(Key.SynchronizeBuffers, synchronizeBuffersMessage), SendType.Binary);
                socketWrapper.Receive(SendType.Binary);
                socketWrapper.ReceiveTimeout = receiveTimeout;
            }
        }
        private class StopTestWorkItem
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="socketWrapper"></param>
            /// <param name="exceptions">Add to exceptions on exception.</param>
            /// <param name="stopped">Add to stopped on succesful stop.</param>
            public void StopTest(SocketWrapper socketWrapper, ref ConcurrentBag<Exception> exceptions, ref ConcurrentBag<SocketWrapper> stopped)
            {
                if (socketWrapper != null)
                    try
                    {
                        Message<Key> message = SendAndReceive(socketWrapper, Key.StopTest, 30000);

                        StartAndStopMessage stopMessage = (StartAndStopMessage)message.Content;
                        if (stopMessage.Exception != null)
                            throw new Exception(stopMessage.Exception);

                        if (!stopped.Contains(socketWrapper))
                            stopped.Add(socketWrapper);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(new Exception("Failed to stop the test on " + socketWrapper.IP + ":" + socketWrapper.Port + ":\n" + ex));
                    }
            }
        }
        #endregion
    }
}