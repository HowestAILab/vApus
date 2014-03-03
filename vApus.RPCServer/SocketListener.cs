using RandomUtils.Log;
/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using vApus.Util;

namespace vApus.RPCServer {
    /// <summary>
    ///     Handles communication comming from vApus master or a vApus API Test Client.
    ///     Built using the singleton design pattern so a reference must not be made in the Gui class.
    /// </summary>
    public class SocketListener {

        #region Events
        //public event EventHandler<ListeningErrorEventArgs> ListeningError;
        #endregion

        #region Fields
        public const int DEFAULTPORT = 1537;

        private int _maximumStartTries = 3, _startTries;

        private static SocketListener _socketListener;
        private Socket _serverSocketV4, _serverSocketV6;
        private int _port;

        private readonly HashSet<SocketWrapper> _connectedClients = new HashSet<SocketWrapper>();

        public AsyncCallback _onReceiveCallBack;

        private static readonly byte[] Salt =
            {
                0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03,
                0x62
            };
        #endregion

        #region Properties
        /// <summary>
        /// </summary>
        public bool IsRunning { get { return (_serverSocketV4 != null || _serverSocketV6 != null); } }

        /// <summary>
        ///     Setting an invalid port will throw an exception.
        /// </summary>
        public int Port { get { return _port; } }
        #endregion

        #region Constructor
        /// <summary>
        ///     Handles communication comming from vApus master.
        ///     Built using the singleton design pattern so a reference must not be made in the Gui class.
        /// </summary>
        private SocketListener() { }
        #endregion

        #region Functions
        public static SocketListener GetInstance() {
            if (_socketListener == null)
                _socketListener = new SocketListener();
            return _socketListener;
        }

        #region Start & Stop
        /// <summary>
        /// Restarts listening when setting this.
        /// </summary>
        /// <param name="port"></param>
        public void SetPort(int port) {
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
                _port = DEFAULTPORT;
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

                DisconnectClients();
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

        private void ConnectClient(string ip, int port, int connectTimeout, out Exception exception) {
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
                // MessageBox.Show(ex.ToString());
                exception = ex;
            }
        }
        private SocketWrapper Get(string ip, int port) {
            foreach (SocketWrapper socketWrapper in _connectedClients)
                if (socketWrapper.IP.ToString() == ip && socketWrapper.Port == port)
                    return socketWrapper;
            return null;
        }

        private void DisconnectClients() {
            foreach (SocketWrapper socketWrapper in _connectedClients)
                if (socketWrapper != null && socketWrapper.Connected)
                    socketWrapper.Close();
            _connectedClients.Clear();
        }
        private void DisconnectClient(SocketWrapper clientSocketWrapper) {
            foreach (SocketWrapper socketWrapper in _connectedClients)
                if (socketWrapper == clientSocketWrapper) {
                    if (socketWrapper != null && socketWrapper.Connected)
                        socketWrapper.Close();
                    _connectedClients.Remove(socketWrapper);
                    break;
                }
        }

        private void OnAcceptV4(IAsyncResult ar) {
            try {
                Socket socket = _serverSocketV4.EndAccept(ar);
                var socketWrapper = new SocketWrapper(IPAddress.Any, 1234, socket, SocketFlags.None, SocketFlags.None);
                _connectedClients.Add(socketWrapper);
                BeginReceive(socketWrapper);
                _serverSocketV4.BeginAccept(OnAcceptV4, null);
            } catch {
            }
        }
        private void OnAcceptV6(IAsyncResult ar) {
            try {
                Socket socket = _serverSocketV6.EndAccept(ar);
                var socketWrapper = new SocketWrapper(IPAddress.IPv6Any, 1234, socket, SocketFlags.None, SocketFlags.None);
                _connectedClients.Add(socketWrapper);
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
                ConnectClient(socketWrapper.IP.ToString(), socketWrapper.Port, 1000, out exception);
                if (exception == null) {
                    BeginReceive(socketWrapper);
                } else {
                    DisconnectClient(socketWrapper);
                    //The test cannot be valid without a master, stop the test if any.
                    Loggers.Log(Level.Warning, "Lost connection with vApus master at " + socketWrapper.IP + ":" + socketWrapper.Port + ".", exception);
                    //if (ListeningError != null)
                    //    ListeningError(null, new ListeningErrorEventArgs(socketWrapper.IP.ToString(), socketWrapper.Port, exception));
                }
            }
        }

        /// <summary>
        /// Handles synchronization of the send and receive buffers, the rest is handled by SlaveSideCommunicationHandler.
        /// </summary>
        /// <param name="result"></param>
        private void OnReceive(IAsyncResult result) {
            var socketWrapper = (SocketWrapper)result.AsyncState;
            string message = null;
            try {
                socketWrapper.Socket.EndReceive(result);
                if (socketWrapper.Connected) {
                    message = socketWrapper.Decode(socketWrapper.Buffer, Encoding.UTF8);
                    if (message.Length != 0) {
                        Console.Write("In: " + message);

                        BeginReceive(socketWrapper);
                        message = CommunicationHandler.HandleMessage(socketWrapper, message);
                        Console.WriteLine(" Out: " + message);

                        message = message.Encrypt(vApus.RPCServer.Properties.Settings.Default.apikey.ToString(), Salt);
                        socketWrapper.Send(message, SendType.Text, Encoding.UTF8);
                    }
                }
                if (!socketWrapper.Connected)
                    DisconnectClient(socketWrapper);
            } catch (Exception exception) {
                DisconnectClient(socketWrapper);
                //The test cannot be valid without a master, stop the test if any.
                //LogWrapper.LogByLevel("Lost connection with vApus master at " + socketWrapper.IP + ":" + socketWrapper.Port + ".\n" + exception, Level.Warning);
                //if (ListeningError != null)
                //    ListeningError(null, new ListeningErrorEventArgs(socketWrapper.IP.ToString(), socketWrapper.Port, exception));
            }
        }
        #endregion

        #endregion
    }
}