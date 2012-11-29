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

namespace vApus.DistributedTesting
{
    /// <summary>
    ///     Built using the singleton design pattern so a reference must not be made in the Gui class.
    /// </summary>
    public class SocketListener
    {
        #region Events

        /// <summary>
        ///     Use this for instance to show the test name in the title bar of the main window.
        /// </summary>
        public event EventHandler<SlaveSideCommunicationHandler.NewTestEventArgs> NewTest;

        public event EventHandler<IPChangedEventArgs> IPChanged;
        public event EventHandler<ListeningErrorEventArgs> ListeningError;

        #endregion

        #region Fields

        public const int DEFAULTPORT = 1337;
        private static SocketListener _socketListener;

        private readonly List<string> _availableIps = new List<string>();
        private readonly HashSet<SocketWrapper> _connectedMasters = new HashSet<SocketWrapper>();
        private string _ip;

        private int _maximumStartTries = 3;
        public AsyncCallback _onReceiveCallBack;
        private int _port;
        private Socket _serverSocket;
        private int _startTries;

        #endregion

        #region Properties

        /// <summary>
        ///     The currenctly used IP.
        ///     Setting an invalid IP will throw an exception.
        /// </summary>
        public string IP
        {
            get { return _ip; }
        }

        /// <summary>
        ///     All possible IPs.
        /// </summary>
        public string[] AvailableIPs
        {
            get { return _availableIps.ToArray(); }
        }

        /// <summary>
        /// </summary>
        public string Network
        {
            get
            {
                string network = string.Empty;
                string[] parts = _ip.Split(new[] {'.'});
                for (int i = 0; i < 3; i++)
                    network = network + parts[i] + '.';
                return network;
            }
        }

        /// <summary>
        ///     Setting an invalid port will throw an exception.
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

        public string PreferredIP
        {
            get { return Settings.Default.PreferredIP; }
        }

        public int PreferredPort
        {
            get { return Settings.Default.PreferredPort; }
        }

        #endregion

        #region Constructor

        private SocketListener()
        {
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            SlaveSideCommunicationHandler.NewTest += SlaveSideCommunicationHandler_NewTest;
        }

        #endregion

        #region Functions

        public static SocketListener GetInstance()
        {
            if (_socketListener == null)
                _socketListener = new SocketListener();
            return _socketListener;
        }

        private void SlaveSideCommunicationHandler_NewTest(object sender,
                                                           SlaveSideCommunicationHandler.NewTestEventArgs e)
        {
            if (NewTest != null)
                foreach (EventHandler<SlaveSideCommunicationHandler.NewTestEventArgs> del in NewTest.GetInvocationList()
                    )
                    del.BeginInvoke(this, e, null, null);
        }

        public bool CheckAgainstPreferred(string ip, int port)
        {
            return Settings.Default.PreferredIP == ip && Settings.Default.PreferredPort == port;
        }

        #region Start & Stop

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            try
            {
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                    {
                        string ip = _availableIps[FillPossibleIPs()];
                        int port = Settings.Default.PreferredPort;
                        if (!_availableIps.Contains(_ip))
                        {
                            MessageBox.Show(
                                "The IP " + _ip +
                                " is no longer available, therefore the socketlistening will be bound to IP " + ip + ".",
                                string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1);
                            SetIPAndPort(ip, _port, false);
                        }
                    }, null);
            }
            catch
            {
            }
        }

        public void SetIPAndPort(string ip, int port, bool preferred = false)
        {
            Stop();
            try
            {
                _ip = ip;
                _port = port;
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(_ip), _port));
                _serverSocket.Listen(100);
                _serverSocket.BeginAccept(OnAccept, null);
                if (preferred)
                {
                    Settings.Default.PreferredIP = _ip;
                    Settings.Default.PreferredPort = _port;
                    Settings.Default.Save();
                }
                NamedObjectRegistrar.RegisterOrUpdate("IP", _ip);
                NamedObjectRegistrar.RegisterOrUpdate("Port", _port);
                NamedObjectRegistrar.RegisterOrUpdate("HostName", Dns.GetHostEntry(_ip).HostName);
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
        ///     Will determine it's port from the DEFAULTPORT (1337) to int.MaxValue.
        ///     The maximum number of connections is 100.
        /// </summary>
        public void Start()
        {
            try
            {
                _ip = _availableIps[FillPossibleIPs()];
                _port = Settings.Default.PreferredPort;
                try
                {
                    _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(_ip), _port));
                }
                catch
                {
                    for (int port = DEFAULTPORT; port <= int.MaxValue; port++)
                        try
                        {
                            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(_ip), port));
                            _port = port;
                            break;
                        }
                        catch
                        {
                        }
                }

                _serverSocket.Listen(100);
                _serverSocket.BeginAccept(OnAccept, null);
                _startTries = 0;

                NamedObjectRegistrar.RegisterOrUpdate("IP", _ip);
                NamedObjectRegistrar.RegisterOrUpdate("Port", _port);
                NamedObjectRegistrar.RegisterOrUpdate("HostName", Dns.GetHostEntry(_ip).HostName);
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
        ///     Fills the collection of possible valid ips and returns an entryindex suggesting the ip to bind to.
        /// </summary>
        /// <returns></returns>
        private int FillPossibleIPs()
        {
            IPHostEntry entry = Dns.GetHostByName(Dns.GetHostName());
            _availableIps.Clear();
            int entryindex = 0;
            var p = new Ping();

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


            int preferredIPIndex = _availableIps.IndexOf(Settings.Default.PreferredIP);
            if (preferredIPIndex > -1)
                entryindex = preferredIPIndex;
            else if (Settings.Default.PreferredIP == string.Empty)
            {
                Settings.Default.PreferredIP = _availableIps[entryindex];
                Settings.Default.Save();
            }
            return entryindex;
        }

        /// <summary>
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
            catch
            {
            }
        }

        /// <summary>
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
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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

        private void DisconnectMaster(SocketWrapper masterSocketWrapper)
        {
            foreach (SocketWrapper socketWrapper in _connectedMasters)
                if (socketWrapper == masterSocketWrapper)
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
                var socketWrapper = new SocketWrapper(_ip, 1234, socket, SocketFlags.None, SocketFlags.None);
                _connectedMasters.Add(socketWrapper);
                BeginReceive(socketWrapper);
                _serverSocket.BeginAccept(OnAccept, null);
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
                    _onReceiveCallBack = OnReceive;
                socketWrapper.Buffer = new byte[socketWrapper.ReceiveBufferSize];
                socketWrapper.Socket.BeginReceive(socketWrapper.Buffer, 0, socketWrapper.ReceiveBufferSize,
                                                  SocketFlags.None, _onReceiveCallBack, socketWrapper);
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
                    SlaveSideCommunicationHandler.HandleMessage(socketWrapper, new Message<Key>(Key.StopTest, null));
                        //The test cannot be valid without a master, stop the test if any.
                    LogWrapper.LogByLevel(
                        "Lost connection with vApus master at " + socketWrapper.IP + ":" + socketWrapper.Port + ".\n" +
                        exception, LogLevel.Warning);
                    if (ListeningError != null)
                        ListeningError(null,
                                       new ListeningErrorEventArgs(socketWrapper.IP.ToString(), socketWrapper.Port,
                                                                   exception));
                }
            }
        }

        private void OnReceive(IAsyncResult result)
        {
            var socketWrapper = (SocketWrapper) result.AsyncState;
            var message = new Message<Key>();
            try
            {
                socketWrapper.Socket.EndReceive(result);
                message = (Message<Key>) socketWrapper.ByteArrayToObject(socketWrapper.Buffer);

                if (message.Key == Key.SynchronizeBuffers)
                {
                    socketWrapper.Socket.ReceiveBufferSize = ((SynchronizeBuffersMessage) message.Content).BufferSize;
                    BeginReceive(socketWrapper);
                    socketWrapper.Socket.SendBufferSize = socketWrapper.ReceiveBufferSize;
                }
                else
                {
                    BeginReceive(socketWrapper);
                    message = SlaveSideCommunicationHandler.HandleMessage(socketWrapper, message);
                }
                socketWrapper.Send(message, SendType.Binary);
            }
            catch (Exception exception)
            {
                DisconnectMaster(socketWrapper);
                SlaveSideCommunicationHandler.HandleMessage(socketWrapper, new Message<Key>(Key.StopTest, null));
                    //The test cannot be valid without a master, stop the test if any.
                LogWrapper.LogByLevel(
                    "Lost connection with vApus master at " + socketWrapper.IP + ":" + socketWrapper.Port + ".\n" +
                    exception, LogLevel.Warning);
                if (ListeningError != null)
                    ListeningError(null,
                                   new ListeningErrorEventArgs(socketWrapper.IP.ToString(), socketWrapper.Port,
                                                               exception));
            }
        }

        #endregion

        #endregion
    }
}