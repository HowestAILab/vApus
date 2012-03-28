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

namespace vApus.JumpStart
{
    /// <summary>
    /// Built using the singleton design pattern so a reference must not be made in the Gui class.
    /// </summary>
    public class SocketListener
    {
        #region Events
        public event EventHandler<IPChangedEventArgs> IPChanged;
        public event EventHandler<ListeningErrorEventArgs> ListeningError;
        #endregion

        #region Fields
        private static SocketListener _socketListener;

        public const int MINPORT = 1314, MAXPORT = 1316;

        private Socket _serverSocket;

        private List<string> _availableIps = new List<string>();
        private string _ip;
        private int _port;

        private int _maximumStartTries = 3;
        private int _startTries = 0;

        //many to many communication
        private HashSet<SocketWrapper> _connectedMasters = new HashSet<SocketWrapper>();
        public AsyncCallback _onReceiveCallBack;

        #endregion

        #region Properties
        /// <summary>
        /// The currenctly used IP.
        /// Setting an invalid IP will throw an exception.
        /// </summary>
        public string IP
        {
            get { return _ip; }
        }
        /// <summary>
        /// All possible IPs.
        /// </summary>
        public string[] AvailableIPs
        {
            get { return _availableIps.ToArray(); }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Network
        {
            get
            {
                string network = string.Empty;
                string[] parts = _ip.Split(new char[] { '.' });
                for (int i = 0; i < 3; i++)
                    network = network + parts[i] + '.';
                return network;
            }
        }
        /// <summary>
        /// Setting an invalid port will throw an exception.
        /// </summary>
        public int Port
        {
            get { return _port; }
        }
        /// <summary>
        /// </summary>
        public int ConnectedMastersCount
        {
            get { return _connectedMasters.Count; }
        }
        /// <summary>
        /// </summary>
        public bool IsRunning
        {
            get { return (_serverSocket != null); }
        }
        #endregion

        #region Constructor
        private SocketListener()
        {
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
        }
        #endregion

        #region Functions

        public static SocketListener GetInstance()
        {
            if (_socketListener == null)
                _socketListener = new SocketListener();
            return _socketListener;
        }

        #region Start & Stop
        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            try
            {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    string ip = _availableIps[FillPossibleIPs()];
                    if (_port < MINPORT)
                        _port = MINPORT;

                    if (!_availableIps.Contains(_ip))
                        SetIPAndPort(ip, _port);

                });
            }
            catch { }
        }
        public void SetIPAndPort(string ip, int port)
        {
            Stop();
            try
            {
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                _serverSocket.Listen(100);
                _serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
                _ip = ip;
                _port = port;

                if (IPChanged != null)
                    IPChanged.Invoke(null, new IPChangedEventArgs(_ip));
            }
            catch
            {
                Stop();
                throw;
            }
        }
        /// <summary>
        /// Will determine it's port from the MINPORT (1314) to MAXPORT (1316 inclusive).
        /// </summary>
        public void Start()
        {
            try
            {
                _ip = _availableIps[FillPossibleIPs()];
                _port = MINPORT;
                try
                {
                    _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(_ip), _port));
                }
                catch
                {
                    for (int port = MINPORT; port <= MAXPORT; port++)
                        try
                        {
                            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(_ip), port));
                            _port = port;
                            break;
                        }
                        catch
                        { }
                }

                _serverSocket.Listen(100);
                _serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
                _startTries = 0;
            }
            catch
            {
                _startTries++;
                if (_startTries <= _maximumStartTries)
                    Start();
                else
                    throw;
            }
        }
        /// <summary>
        /// Fills the collection of possible valid ips and returns an entryindex suggesting the ip to bind to.
        /// </summary>
        /// <returns></returns>
        private int FillPossibleIPs()
        {
            IPHostEntry entry = Dns.GetHostByName(Dns.GetHostName());
            _availableIps.Clear();
            int entryindex = 0;
            Ping p = new Ping();

            //Ping to make sure it is a connected device you can use.
            for (int i = 0; i < entry.AddressList.Length; i++)
            {
                if ((p.Send(entry.AddressList[i])).Status == IPStatus.Success)
                {
                    string ip = entry.AddressList[i].ToString();
                    if (!_availableIps.Contains(ip))
                        _availableIps.Add(ip);
                }
            }

            if (_availableIps.Count != 0)
                entryindex = 0;
            else
                _availableIps.Add("127.0.0.1");

            return entryindex;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            try
            {
                if (_serverSocket != null)
                    _serverSocket.Close();
                _serverSocket = null;
                DisconnectMasters();
            }
            catch { }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minimumPort"></param>
        /// <param name="maximumPort"></param>
        public void Restart()
        {
            Stop();
            Start();
        }
        #endregion

        #region Communication
        private SocketWrapper Get(string ip, int port)
        {
            foreach (SocketWrapper socketWrapper in _connectedMasters)
                if (socketWrapper.IP.ToString() == ip && socketWrapper.Port == port)
                    return socketWrapper;
            return null;
        }
        private void ConnectMaster(string ip, int port, int connectTimeout, out Exception exception)
        {
            try
            {
                exception = null;
                SocketWrapper socketWrapper = Get(ip, port);
                if (socketWrapper == null)
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketWrapper = new SocketWrapper(ip, port, socket, SocketFlags.None, SocketFlags.None);
                }

                if (!socketWrapper.Connected)
                    socketWrapper.Connect(connectTimeout);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }
        private void DisconnectMasters()
        {
            foreach (SocketWrapper socketWrapper in _connectedMasters)
                if (socketWrapper != null && socketWrapper.Connected)
                    socketWrapper.Close();
            _connectedMasters.Clear();
        }
        private void DisconnectMaster(SocketWrapper slaveSocketWrapper)
        {
            foreach (SocketWrapper socketWrapper in _connectedMasters)
                if (socketWrapper == slaveSocketWrapper)
                {
                    if (socketWrapper != null && socketWrapper.Connected)
                        socketWrapper.Close();
                    _connectedMasters.Remove(socketWrapper);
                    break;
                }
        }
        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket socket = _serverSocket.EndAccept(ar);
                SocketWrapper socketWrapper = new SocketWrapper(_ip, 1234, socket, SocketFlags.None, SocketFlags.None);
                _connectedMasters.Add(socketWrapper);
                BeginReceive(socketWrapper);
                _serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
            }
            catch
            {
            }
        }
        private void BeginReceive(SocketWrapper socketWrapper)
        {
            try
            {
                if (_onReceiveCallBack == null)
                    _onReceiveCallBack = new AsyncCallback(OnReceive);
                socketWrapper.Buffer = new byte[socketWrapper.ReceiveBufferSize];
                socketWrapper.Socket.BeginReceive(socketWrapper.Buffer, 0, socketWrapper.ReceiveBufferSize, SocketFlags.None, _onReceiveCallBack, socketWrapper);
            }
            catch (Exception ex)
            {
                Exception exception = ex;
                //Reconnect on network hiccup.
                ConnectMaster(socketWrapper.IP.ToString(), socketWrapper.Port, 1000, out exception);
                if (exception == null)
                {
                    BeginReceive(socketWrapper);
                }
                else
                {
                    DisconnectMaster(socketWrapper);
                    if (ListeningError != null)
                        ListeningError(null, new ListeningErrorEventArgs(socketWrapper.IP.ToString(), socketWrapper.Port, exception));
                }
            }
        }
        private void OnReceive(IAsyncResult result)
        {
            SocketWrapper socketWrapper = (SocketWrapper)result.AsyncState;
            Message<Key> message = new Message<Key>();
            try
            {
                socketWrapper.Socket.EndReceive(result);
                message = (Message<Key>)socketWrapper.ByteArrayToObject(socketWrapper.Buffer);


                BeginReceive(socketWrapper);
                message = CommunicationHandler.HandleMessage(socketWrapper, message);

                socketWrapper.Send(message, SendType.Binary);
            }
            catch (Exception exception)
            {
                DisconnectMaster(socketWrapper);
                if (ListeningError != null)
                    ListeningError(null, new ListeningErrorEventArgs(socketWrapper.IP.ToString(), socketWrapper.Port, exception));
            }
        }
        #endregion
        #endregion
    }
}