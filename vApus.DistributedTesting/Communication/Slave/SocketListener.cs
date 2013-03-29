/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;
using vApus.DistributedTesting.Properties;
using vApus.Util;

namespace vApus.DistributedTesting {
    /// <summary>
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
        private static SocketListener _socketListener;

        private readonly HashSet<SocketWrapper> _connectedMasters = new HashSet<SocketWrapper>();

        private int _maximumStartTries = 3;
        public AsyncCallback _onReceiveCallBack;
        private int _port;
        private Socket _serverSocketV4;
        private Socket _serverSocketV6;
        private int _startTries;

        #endregion

        #region Properties
        /// <summary>
        ///     Setting an invalid port will throw an exception.
        /// </summary>
        public int Port { get { return _port; } }

        /// <summary>
        /// </summary>
        public int ConnectedMastersCount { get { return _connectedMasters.Count; } }

        /// <summary>
        /// </summary>
        public bool IsRunning { get { return (_serverSocketV4 != null || _serverSocketV6 != null); } }

        public int PreferredPort { get { return Settings.Default.PreferredPort; } }

        #endregion

        #region Constructor

        private SocketListener() {
            //NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            SlaveSideCommunicationHandler.NewTest += SlaveSideCommunicationHandler_NewTest;
        }

        #endregion

        #region Functions

        public static SocketListener GetInstance() {
            if (_socketListener == null)
                _socketListener = new SocketListener();
            return _socketListener;
        }

        private void SlaveSideCommunicationHandler_NewTest(object sender,
                                                           SlaveSideCommunicationHandler.NewTestEventArgs e) {
            if (NewTest != null)
                foreach (EventHandler<SlaveSideCommunicationHandler.NewTestEventArgs> del in NewTest.GetInvocationList()
                    )
                    del.BeginInvoke(this, e, null, null);
        }

        public bool CheckAgainstPreferred(int port) {
            return Settings.Default.PreferredPort == port;
        }

        #region Start & Stop

        //private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e) {
        //try {
        //    SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
        //        SetPort(_port, false);
        //    }, null);
        //} catch { }
        //}

        public void SetPort(int port, bool preferred = false) {
            Stop();
            try {
                _port = port;
                var address = IPAddress.Any;
                _serverSocketV4 = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _serverSocketV4.Bind(new IPEndPoint(address, _port));
                _serverSocketV4.Listen(100);
                _serverSocketV4.BeginAccept(OnAcceptV4, null);

                address = IPAddress.IPv6Any;
                _serverSocketV6 = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _serverSocketV6.Bind(new IPEndPoint(address, _port));
                _serverSocketV6.Listen(100);
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

                _serverSocketV4.Listen(100);
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

                _serverSocketV6.Listen(100);
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

        private SocketWrapper Get(string ip, int port) {
            foreach (SocketWrapper socketWrapper in _connectedMasters)
                if (socketWrapper.IP.ToString() == ip && socketWrapper.Port == port)
                    return socketWrapper;
            return null;
        }

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
                MessageBox.Show(ex.ToString());
                exception = ex;
            }
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
                    if (socketWrapper != null && socketWrapper.Connected)
                        socketWrapper.Close();
                    _connectedMasters.Remove(socketWrapper);
                    break;
                }
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

        private void BeginReceive(SocketWrapper socketWrapper) {
            try {
                if (_onReceiveCallBack == null)
                    _onReceiveCallBack = OnReceive;
                socketWrapper.Buffer = new byte[socketWrapper.ReceiveBufferSize];
                socketWrapper.Socket.BeginReceive(socketWrapper.Buffer, 0, socketWrapper.ReceiveBufferSize,
                                                  SocketFlags.None, _onReceiveCallBack, socketWrapper);
            } catch (Exception ex) {
                Exception exception = ex;
                //Reconnect on network hiccup.
                ConnectMaster(socketWrapper.IP.ToString(), socketWrapper.Port, 1000, out exception);
                if (exception == null) {
                    BeginReceive(socketWrapper);
                } else {
                    DisconnectMaster(socketWrapper);
                    SlaveSideCommunicationHandler.HandleMessage(socketWrapper, new Message<Key>(Key.StopTest, null));
                    //The test cannot be valid without a master, stop the test if any.
                    LogWrapper.LogByLevel("Lost connection with vApus master at " + socketWrapper.IP + ":" + socketWrapper.Port + ".\n" + exception, LogLevel.Warning);
                    if (ListeningError != null)
                        ListeningError(null, new ListeningErrorEventArgs(socketWrapper.IP.ToString(), socketWrapper.Port, exception));
                }
            }
        }

        private void OnReceive(IAsyncResult result) {
            var socketWrapper = (SocketWrapper)result.AsyncState;
            var message = new Message<Key>();
            try {
                socketWrapper.Socket.EndReceive(result);
                message = (Message<Key>)socketWrapper.ByteArrayToObject(socketWrapper.Buffer);

                if (message.Key == Key.SynchronizeBuffers) {
                    socketWrapper.Socket.ReceiveBufferSize = ((SynchronizeBuffersMessage)message.Content).BufferSize;
                    BeginReceive(socketWrapper);
                    socketWrapper.Socket.SendBufferSize = socketWrapper.ReceiveBufferSize;
                } else {
                    BeginReceive(socketWrapper);
                    message = SlaveSideCommunicationHandler.HandleMessage(socketWrapper, message);
                }
                socketWrapper.Send(message, SendType.Binary);
            } catch (Exception exception) {
                //MessageBox.Show(exception.ToString());
                DisconnectMaster(socketWrapper);
                SlaveSideCommunicationHandler.HandleMessage(socketWrapper, new Message<Key>(Key.StopTest, null));
                //The test cannot be valid without a master, stop the test if any.
                LogWrapper.LogByLevel("Lost connection with vApus master at " + socketWrapper.IP + ":" + socketWrapper.Port + ".\n" + exception, LogLevel.Warning);
                if (ListeningError != null)
                    ListeningError(null, new ListeningErrorEventArgs(socketWrapper.IP.ToString(), socketWrapper.Port, exception));
            }
        }

        #endregion

        #endregion
    }
}