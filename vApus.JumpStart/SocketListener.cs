/*
 * Copyright 2012 (c) Sizing Servers Lab
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
using vApus.JumpStartStructures;
using vApus.Util;

namespace vApus.JumpStart {
    /// <summary>
    ///     Built using the singleton design pattern so a reference must not be made in the Gui class.
    /// </summary>
    public class SocketListener {
        #region Fields

        public const int PORT = 1314;
        private static SocketListener _socketListener;

        //many to many communication
        private readonly HashSet<SocketWrapper> _connectedMasters = new HashSet<SocketWrapper>();

        //To queue the communication
        private readonly object _lock = new object();
        private int _maximumStartTries = 3;
        public AsyncCallback _onReceiveCallBack;
        private Socket _serverSocket;
        private int _startTries;

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public int ConnectedMastersCount {
            get { return _connectedMasters.Count; }
        }

        /// <summary>
        /// </summary>
        public bool IsRunning {
            get { return (_serverSocket != null); }
        }

        #endregion

        #region Constructor

        private SocketListener() {
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        #endregion

        #region Functions

        public static SocketListener GetInstance() {
            if (_socketListener == null)
                _socketListener = new SocketListener();
            return _socketListener;
        }

        #region Start & Stop

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e) {
            try {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { Start(); }, null);
            } catch {
            }
        }

        /// <summary>
        /// </summary>
        public void Start() {
            Stop();
            try {
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));

                _serverSocket.Listen(100);
                _serverSocket.BeginAccept(OnAccept, null);
                _startTries = 0;
            } catch {
                _startTries++;
                if (_startTries <= _maximumStartTries) {
                    Start();
                } else {
                    Stop();
                    throw;
                }
            }
        }

        /// <summary>
        /// </summary>
        public void Stop() {
            try {
                if (_serverSocket != null)
                    _serverSocket.Close();
                _serverSocket = null;
                DisconnectMasters();
            } catch {
            }
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
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketWrapper = new SocketWrapper(ip, port, socket, SocketFlags.None, SocketFlags.None);
                }

                if (!socketWrapper.Connected)
                    socketWrapper.Connect(connectTimeout);
            } catch (Exception ex) {
                exception = ex;
            }
        }

        private void DisconnectMasters() {
            foreach (SocketWrapper socketWrapper in _connectedMasters)
                if (socketWrapper != null && socketWrapper.Connected)
                    socketWrapper.Close();
            _connectedMasters.Clear();
        }

        private void DisconnectMaster(SocketWrapper slaveSocketWrapper) {
            foreach (SocketWrapper socketWrapper in _connectedMasters)
                if (socketWrapper == slaveSocketWrapper) {
                    if (socketWrapper != null && socketWrapper.Connected)
                        socketWrapper.Close();
                    _connectedMasters.Remove(socketWrapper);
                    break;
                }
        }

        private void OnAccept(IAsyncResult ar) {
            try {
                Socket socket = _serverSocket.EndAccept(ar);
                var socketWrapper = new SocketWrapper(IPAddress.Any, 1234, socket, SocketFlags.None, SocketFlags.None);
                _connectedMasters.Add(socketWrapper);
                BeginReceive(socketWrapper);
                _serverSocket.BeginAccept(OnAccept, null);
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
                if (exception == null)
                    BeginReceive(socketWrapper);
                else
                    DisconnectMaster(socketWrapper);
            }
        }

        private void OnReceive(IAsyncResult result) {
            lock (_lock) {
                var socketWrapper = (SocketWrapper)result.AsyncState;
                var message = new Message<Key>();
                try {
                    socketWrapper.Socket.EndReceive(result);
                    message = (Message<Key>)socketWrapper.ByteArrayToObject(socketWrapper.Buffer);


                    BeginReceive(socketWrapper);
                    message = CommunicationHandler.HandleMessage(socketWrapper, message);

                    socketWrapper.Send(message, SendType.Binary);
                } catch {
                    DisconnectMaster(socketWrapper);
                }
            }
        }

        #endregion

        #endregion
    }
}