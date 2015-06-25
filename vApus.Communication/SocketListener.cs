/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.DistributedTest;
using vApus.Communication.Properties;
using vApus.Communication.Shared;
using vApus.Util;

namespace vApus.Communication {
    /// <summary>
    ///     Handles communication comming from vApus master or a vApus API Test Client.
    ///     Built using the singleton design pattern so a reference must not be made in the Gui class.
    /// </summary>
    public class SocketListener {

        #region Events
        /// <summary>
        ///     Use this for instance to show the test name in the title bar of the main window.
        /// </summary>
        public event EventHandler<SlaveSideCommunicationHandler.NewTestEventArgs> NewTest;
        public event EventHandler<ListeningErrorEventArgs> ListeningError;
        #endregion

        #region Fields
        public const int DEFAULTPORT = 1337;

        private int _maximumStartTries = 3, _startTries;

        private static SocketListener _socketListener;
        private Socket _serverSocketV4, _serverSocketV6;
        private int _port;

        private readonly HashSet<SocketWrapper> _connectedMasters = new HashSet<SocketWrapper>();

        public AsyncCallback _onReceiveCallBack;
        #endregion

        #region Properties
        /// <summary>
        /// </summary>
        public bool IsRunning { get { return (_serverSocketV4 != null || _serverSocketV6 != null); } }

        /// <summary>
        ///     Setting an invalid port will throw an exception.
        /// </summary>
        public int Port { get { return _port; } }

        public int PreferredPort { get { return Settings.Default.PreferredPort; } }
        #endregion

        #region Constructor
        /// <summary>
        ///     Handles communication comming from vApus master.
        ///     Built using the singleton design pattern so a reference must not be made in the Gui class.
        /// </summary>
        private SocketListener() { SlaveSideCommunicationHandler.NewTest += SlaveSideCommunicationHandler_NewTest; }
        #endregion

        #region Functions
        public static SocketListener GetInstance() {
            if (_socketListener == null)
                _socketListener = new SocketListener();
            return _socketListener;
        }

        private void SlaveSideCommunicationHandler_NewTest(object sender, SlaveSideCommunicationHandler.NewTestEventArgs e) {
            if (NewTest != null) {
                var invocationList = NewTest.GetInvocationList();
                Parallel.For(0, invocationList.Length, (i) => {
                    (invocationList[i] as EventHandler<SlaveSideCommunicationHandler.NewTestEventArgs>).Invoke(this, e);
                });

            }
        }

        /// <summary>
        /// Check if the given port is the same as the preferred one.
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool CheckAgainstPreferred(int port) { return Settings.Default.PreferredPort == port; }

        #region Start & Stop
        /// <summary>
        /// Restarts listening when setting this.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="preferred"></param>
        public void SetPort(int port, bool preferred = false) {
            Stop();
            try {
                _port = port;
                var address = IPAddress.Any;
                _serverSocketV4 = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _serverSocketV4.Bind(new IPEndPoint(address, _port));
                _serverSocketV4.Listen(int.MaxValue);
                _serverSocketV4.BeginAccept(OnAcceptV4, null);

                address = IPAddress.IPv6Any;
                _serverSocketV6 = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _serverSocketV6.Bind(new IPEndPoint(address, _port));
                _serverSocketV6.Listen(int.MaxValue);
                _serverSocketV6.BeginAccept(OnAcceptV6, null);

                if (preferred) {
                    Settings.Default.PreferredPort = _port;
                    Settings.Default.Save();
                }

                NamedObjectRegistrar.RegisterOrUpdate("Port", _port);
            } catch {
                Stop();
                throw;
            }
        }

        /// <summary>
        ///     Will determine it's port from the DEFAULTPORT (1337) to int.MaxValue.
        ///     The maximum number of connections is 100.
        /// </summary>
        public void Start() {
            try {
                _port = Settings.Default.PreferredPort;
                try {
                    var address = IPAddress.Any;
                    _serverSocketV4 = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    _serverSocketV4.Bind(new IPEndPoint(address, _port));
                } catch {
                    for (int port = DEFAULTPORT; port <= int.MaxValue; port++)
                        try {
                            var address = IPAddress.Any;
                            _serverSocketV4 = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            _serverSocketV4.Bind(new IPEndPoint(address, port));

                            _port = port;
                            break;
                        } catch {
                        }
                }

                _serverSocketV4.Listen(int.MaxValue);
                _serverSocketV4.BeginAccept(OnAcceptV4, null);
                _startTries = 0;

                try {
                    var address = IPAddress.IPv6Any;
                    _serverSocketV6 = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    _serverSocketV6.Bind(new IPEndPoint(address, _port));
                } catch {
                    for (int port = DEFAULTPORT; port <= int.MaxValue; port++)
                        try {
                            var address = IPAddress.Any;
                            _serverSocketV6 = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            _serverSocketV6.Bind(new IPEndPoint(address, port));

                            _port = port;
                            break;
                        } catch {
                        }
                }

                _serverSocketV6.Listen(int.MaxValue);
                _serverSocketV6.BeginAccept(OnAcceptV6, null);

                _startTries = 0;

                NamedObjectRegistrar.RegisterOrUpdate("Port", _port);
            } catch {
                _startTries++;
                if (_startTries <= _maximumStartTries)
                    Start();
                else
                    throw;
            }
        }

        /// <summary>
        /// </summary>
        public void Stop() {
            try {
                if (_serverSocketV4 != null)
                    try { _serverSocketV4.Close(); } catch { }
                _serverSocketV4 = null;

                if (_serverSocketV6 != null)
                    try { _serverSocketV6.Close(); } catch { }
                _serverSocketV6 = null;

                DisconnectMasters();
            } catch {
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="minimumPort"></param>
        /// <param name="maximumPort"></param>
        public void Restart() {
            Stop();
            Start();
        }

        #endregion

        #region Communication

        private void ConnectMaster(string ip, int port, int connectTimeout, out Exception exception) {
            try {
                exception = null;
                SocketWrapper socketWrapper = Get(ip, port);
                if (socketWrapper == null) {
                    var address = IPAddress.Parse(ip);
                    var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socketWrapper = new SocketWrapper(address, port, socket, SocketFlags.None, SocketFlags.None);
                }

                if (!socketWrapper.Connected)
                    socketWrapper.Connect(connectTimeout);
            } catch (Exception ex) {
                exception = ex;
            }
        }
        private SocketWrapper Get(string ip, int port) {
            foreach (SocketWrapper socketWrapper in _connectedMasters)
                if (socketWrapper.IP.ToString() == ip && socketWrapper.Port == port)
                    return socketWrapper;
            return null;
        }

        private void DisconnectMasters() {
            foreach (SocketWrapper socketWrapper in _connectedMasters)
                if (socketWrapper != null && socketWrapper.Connected)
                    socketWrapper.Close();
            _connectedMasters.Clear();
        }
        private void DisconnectMaster(SocketWrapper masterSocketWrapper) {
            foreach (SocketWrapper socketWrapper in _connectedMasters)
                if (socketWrapper == masterSocketWrapper) {
                    _connectedMasters.Remove(socketWrapper);
                    break;
                }
            if (masterSocketWrapper != null && masterSocketWrapper.Connected)
                masterSocketWrapper.Close();
        }

        private void OnAcceptV4(IAsyncResult ar) {
            try {
                Socket socket = _serverSocketV4.EndAccept(ar);
                var socketWrapper = new SocketWrapper(IPAddress.Any, 1234, socket, SocketFlags.None, SocketFlags.None);
                _connectedMasters.Add(socketWrapper);
                BeginReceive(socketWrapper);
                _serverSocketV4.BeginAccept(OnAcceptV4, null);
            } catch {
            }
        }
        private void OnAcceptV6(IAsyncResult ar) {
            try {
                Socket socket = _serverSocketV6.EndAccept(ar);
                var socketWrapper = new SocketWrapper(IPAddress.IPv6Any, 1234, socket, SocketFlags.None, SocketFlags.None);
                _connectedMasters.Add(socketWrapper);
                BeginReceive(socketWrapper);
                _serverSocketV6.BeginAccept(OnAcceptV6, null);
            } catch {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="canReconnect">Should be false for RPC communication</param>
        private void BeginReceive(SocketWrapper socketWrapper, bool canReconnect = true) {
            try {
                if (_onReceiveCallBack == null)
                    _onReceiveCallBack = OnReceive;
                socketWrapper.Buffer = new byte[socketWrapper.ReceiveBufferSize];
                socketWrapper.Socket.BeginReceive(socketWrapper.Buffer, 0, socketWrapper.ReceiveBufferSize,
                                                  SocketFlags.None, _onReceiveCallBack, socketWrapper);
            } catch (Exception ex) {
                Exception exception = ex;
                //Reconnect on network hiccup.
                if (canReconnect) {
                    ConnectMaster(socketWrapper.IP.ToString(), socketWrapper.Port, 1000, out exception);
                    if (exception == null) {
                        BeginReceive(socketWrapper);
                    } else {
                        DisconnectMaster(socketWrapper);
                        CommunicationHandler.HandleMessage(new Message<Key>(Key.StopTest, null));
                        //The test cannot be valid without a master, stop the test if any.
                        Loggers.Log(Level.Warning, "Lost connection with vApus master at " + socketWrapper.IP + ":" + socketWrapper.Port + ".", exception);
                        if (ListeningError != null)
                            ListeningError(null, new ListeningErrorEventArgs(socketWrapper.IP.ToString(), socketWrapper.Port, exception));
                    }
                } else {
                    DisconnectMaster(socketWrapper);
                }
            }
        }

        /// <summary>
        /// Handles synchronization of the send and receive buffers, the rest is handled by CommunicationHandler.
        /// </summary>
        /// <param name="result"></param>
        private void OnReceive(IAsyncResult result) {
            var socketWrapper = (SocketWrapper)result.AsyncState;
            object message;
            try {
                socketWrapper.Socket.EndReceive(result);

                try {
                    message = socketWrapper.ByteArrayToObject(socketWrapper.Buffer);
                    var msg = (Message<Key>)message;
                    if (msg.Key == Key.SynchronizeBuffers) {
                        socketWrapper.Socket.ReceiveBufferSize = ((SynchronizeBuffersMessage)msg.Content).BufferSize;
                        BeginReceive(socketWrapper);
                        socketWrapper.Socket.SendBufferSize = socketWrapper.ReceiveBufferSize;
                    } else {
                        BeginReceive(socketWrapper);
                        message = CommunicationHandler.HandleMessage(message);
                    }

                    socketWrapper.Send(message, SendType.Binary);
                } catch {
                    message = socketWrapper.Decode(socketWrapper.Buffer, Encoding.UTF8); //In most times the deserialization will be valid. This is for starting vApus via the commandline + args.
                    if ((message as string).Length == 0) {
                        DisconnectMaster(socketWrapper);
                    } else {
                        BeginReceive(socketWrapper, false);
                        message = CommunicationHandler.HandleMessage(message);

                        //Do not send an answer back to avoid deadlocking when vApus is run from the commandline.
                        //socketWrapper.Send(message, SendType.Text, Encoding.UTF8);
                    }
                }

            } catch (Exception exception) {
                DisconnectMaster(socketWrapper);
                CommunicationHandler.HandleMessage(new Message<Key>(Key.StopTest, null));
                //The test cannot be valid without a master, stop the test if any.
                //Loggers.Log(Level.Warning, "Lost connection with vApus master at " + socketWrapper.IP + ":" + socketWrapper.Port + ".", exception);
                if (ListeningError != null)
                    ListeningError(null, new ListeningErrorEventArgs(socketWrapper.IP.ToString(), socketWrapper.Port, exception));
            }
        }
        #endregion

        #endregion
    }
}