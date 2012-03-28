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
using System.IO;
using System.Net.Sockets;
using System.Threading;
using vApus.Util;
using vApusSMT.Base;
using vApusSMT.Communication;

namespace vApusSMT.Proxy.Remote
{
    public class MonitorProxy : IMonitorProxy
    {
        #region Events
        public event EventHandler<ErrorEventArgs> OnHandledException;
        public event EventHandler<ErrorEventArgs> OnUnhandledException;
        public event EventHandler<OnMonitorEventArgs> OnMonitor;
        #endregion

        #region Fields
        private bool _stop;
        
        private SocketWrapper _connected;
        private AsyncCallback _onReceiveCallBack;
        private IAsyncResult _asyncResult;
        private bool _isDisposed;

        private int _monitorViewOriginalHashCode;
        #endregion

        #region Properties
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
        #endregion

        #region Private
        /// <summary>
        /// Will begin listening after initializing the monitor.
        /// </summary>
        /// <param name="socketWrapper"></param>
        /// <param name="connectTimeout"></param>
        /// <param name="exception"></param>
        private void Connect(SocketWrapper socketWrapper, int connectTimeout, out Exception exception)
        {
            try
            {
                _stop = false;

                exception = null;
                if (!socketWrapper.Connected)
                    socketWrapper.Connect(connectTimeout);
                if (socketWrapper.Connected)
                {
                    //Close the previously connected socket.
                    if (_connected != socketWrapper && _connected != null)
                    {
                        try
                        {
                            _connected.Close();
                        }
                        catch { }

                        _connected = null;
                    }

                    _connected = socketWrapper;
                    _connected.ReceiveTimeout = 10000;
                    SendAndReceive(Key.Poll, null, out exception);
                    _connected.ReceiveTimeout = 120000;
                    if (exception != null)
                        _connected = null;
                }
                else
                {
                    exception = new Exception(string.Format("Could not connect {0}:{1}.", socketWrapper.IP.ToString(), socketWrapper.Port.ToString()));
                }
            }
            catch (Exception ex)
            {
                _connected = null;
                exception = ex;
            }
        }
        private void Send(Key key, object content, out Exception exception)
        {
            Send(new Message<Key>(key, content), out exception);
        }
        private void Send(Message<Key> message, out Exception exception)
        {
            exception = null;
            try
            {
                _connected.Send(message, SendType.Binary);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }
        private Message<Key> Receive(out Exception exception)
        {
            Message<Key> message = new Message<Key>();
            exception = null;
            try
            {
                message = (Message<Key>)_connected.Receive(SendType.Binary);
                if (message.Key == Key.SynchronizeBuffers)
                {
                    _connected.Socket.ReceiveBufferSize = ((SynchronizeBuffersMessage)message.Content).BufferSize;
                    message = Receive(out exception);
                    if (exception == null)
                        _connected.Socket.SendBufferSize = _connected.ReceiveBufferSize;
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            return message;
        }
        private Message<Key> SendAndReceive(Key key, object content, out Exception exception)
        {
            return SendAndReceive(new Message<Key>(key, content), out exception);
        }
        private Message<Key> SendAndReceive(Message<Key> message, out Exception exception)
        {
            exception = null;
            Send(message, out exception);
            if (exception == null)
                message = Receive(out exception);
            return message;
        }
        private void SynchronizeBuffers(object toSend, out Exception exception)
        {
            exception = null;
            try
            {
                byte[] buffer = _connected.ObjectToByteArray(toSend);
                int bufferSize = buffer.Length;
                if (bufferSize > _connected.SendBufferSize)
                {
                    _connected.SendBufferSize = bufferSize;
                    _connected.ReceiveBufferSize = _connected.SendBufferSize;
                    SynchronizeBuffersMessage synchronizeBuffersMessage = new SynchronizeBuffersMessage();
                    synchronizeBuffersMessage.BufferSize = _connected.SendBufferSize;
                    SendAndReceive(Key.SynchronizeBuffers, synchronizeBuffersMessage, out exception);
                }
            }
            catch (Exception ex) { exception = ex; }
        }
        private void StartListening(out Exception exception)
        {
            exception = null;
            try
            {
                BeginReceive();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }
        private void BeginReceive()
        {
            Thread t = new Thread(delegate()
            {
                BeginReceiveThreaded();
            });
            t.IsBackground = true;
            t.Start();
        }
        private void BeginReceiveThreaded()
        {
            try
            {
                if (_onReceiveCallBack == null)
                    _onReceiveCallBack = new AsyncCallback(OnReceive);
                _connected.Buffer = new byte[_connected.ReceiveBufferSize];
                _connected.Socket.BeginReceive(_connected.Buffer, 0, _connected.ReceiveBufferSize, SocketFlags.None, _onReceiveCallBack, _connected);
            }
            catch (Exception ex)
            {
                if (_stop)
                {
                    _stop = false;
                }
                else
                {
                    Exception exception = ex;
                    //Reconnect on network hiccup.
                    Connect(_connected, 2000, out exception);
                    if (exception == null)
                        BeginReceiveThreaded();
                    else
                        HandleOnUnhandledException(exception);
                }
            }
        }
        private void OnReceive(IAsyncResult result)
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
                    BeginReceiveThreaded();
                    socketWrapper.Socket.SendBufferSize = socketWrapper.ReceiveBufferSize;
                }
                else
                {
                    BeginReceiveThreaded();
                    HandleOnMonitor(message.Content as Dictionary<string, HashSet<MonitorValueCollection>>);
                }
            }
            catch (Exception ex)
            {
                if (_stop)
                {
                    _stop = false;
                }
                else
                {
                    Exception exception = ex;
                    Debug.WriteLine("Network hiccup: " + ex, "MasterSideCommunicationHandler.OnReceive(IAsyncResult)");
                    //Reconnect on network hiccup.
                    Connect(socketWrapper, 2000, out exception);
                    if (exception == null)
                        BeginReceiveThreaded();
                    else
                        HandleOnUnhandledException(exception);
                }
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// Connects, sends a poll and adds to the collection (if not there already).
        /// 
        /// There is a connect timeout of 2 seconds with a poll receive timeout of 10 seconds.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="beginPort">inclusive</param>
        /// <param name="endPort">exclusive</param>
        public void ConnectSMT(out Exception exception, string ip, int beginPort = 1327, int endPort = 1337)
        {
            try
            {
                exception = null;
                for (int port = beginPort; port != endPort; port++)
                {
                    //Just poll.
                    if (_connected != null && _connected.Port == port && _connected.Connected)
                    {
                        Connect(_connected, 2000, out exception);
                    }
                    else
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        SocketWrapper socketWrapper = new SocketWrapper(ip, port, socket, SocketFlags.None, SocketFlags.None);
                        Connect(socketWrapper, 2000, out exception);
                    }
                    if (exception == null)
                        break;
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }
        /// <summary>
        /// Get the monitor sources.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public string[] GetMonitorSources(out Exception exception)
        {
            Message<Key> message = new Message<Key>(Key.GetMonitorSources, null);
            SynchronizeBuffers(message, out exception);

            string[] monitorSources = new string[] { };
            if (exception == null)
                message = SendAndReceive(message, out exception);
            if (exception == null)
                try
                {
                    MonitorSourcesMessage msm = (MonitorSourcesMessage)message.Content;
                    monitorSources = msm.MonitorSources;
                    _monitorViewOriginalHashCode = msm.MonitorViewOriginalHashCode;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            return monitorSources;
        }
        private void Disconnect()
        {
            try
            {
                try
                {
                    StopMonitoring();
                }
                catch { }
                try
                {
                    Exception exception;
                    Send(Key.Disconnect, _monitorViewOriginalHashCode, out exception);
                }
                catch
                {
                }
                if (_connected != null && _connected.Connected)
                    _connected.Close();
                _connected = null;
            }
            catch { }
        }

        /// <summary>
        /// Returns the entities and their counters.
        /// </summary>
        /// <param name="monitorSource"></param>
        /// <param name="ip"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Dictionary<Entity, List<CounterInfo>> ConnectToMonitorSource(string monitorSource, out Exception exception, params object[] args)
        {
            ConnectToMonitorSourceMessage connectToMonitorSourceMessage = new ConnectToMonitorSourceMessage(monitorSource, _monitorViewOriginalHashCode, args);
            Message<Key> message = new Message<Key>(Key.ConnectToMonitorSource, connectToMonitorSourceMessage);
            SynchronizeBuffers(message, out exception);

            WDYHMessage wdyhMessage = new WDYHMessage();
            if (exception == null)
                message = SendAndReceive(message, out exception);

            if (exception == null)
                try
                {
                    wdyhMessage = (WDYHMessage)message.Content;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            return wdyhMessage.WhatIHave;
        }
        public Parameter[] GetParameters(string monitorSource, out Exception exception)
        {
            GetSingleValueOfSourceMessage getParametersOfSourceMessage = new GetSingleValueOfSourceMessage(monitorSource, _monitorViewOriginalHashCode);
            Message<Key> message = new Message<Key>(Key.GetParametersOfMonitorSource, getParametersOfSourceMessage);
            SynchronizeBuffers(message, out exception);

            Parameter[] parameters = null;
            if (exception == null)
                message = SendAndReceive(message, out exception);
            if (exception == null)
                try
                {
                    parameters = (Parameter[])message.Content;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            return parameters;
        }
        public int GetRefreshRateInMs(string monitorSource, out Exception exception)
        {
#warning Implement RefreshRateInMs
            exception = null;
            return 1000;
        }

        public void StartMonitoring(Dictionary<string, List<CounterInfo>> wiw, out Exception exception)
        {
            Message<Key> message = new Message<Key>(Key.Start, new StartMessage(_monitorViewOriginalHashCode, wiw));
            SynchronizeBuffers(message, out exception);
            if (exception == null)
            {
                SendAndReceive(message, out exception);
                if (exception == null)
                    StartListening(out exception);
            }
        }

        public void HandleOnMonitor(Dictionary<string, HashSet<MonitorValueCollection>> monitorValues)
        {
            if (OnMonitor != null)
                OnMonitor(this, new OnMonitorEventArgs(monitorValues));
        }
        public void HandleOnHandledException(Exception exception)
        {
#warning Implement HandleOnHandledException
        }
        public void HandleOnUnhandledException(Exception exception)
        {
            if (OnUnhandledException != null)
                OnUnhandledException(this, new ErrorEventArgs(exception));
        }

        /// <summary>
        /// Send a request to close SMT (if there is only one suscriber left it will be granted)
        /// </summary>
        public void StopMonitoring()
        {
            if (!_stop)
            {
                Exception exception;
                int retry = 0;
            Retry:
                _stop = true;
                try
                {
                    _connected.SendTimeout = 2000;
                    _connected.ReceiveTimeout = 2000;
                    SendAndReceive(Key.Stop, _monitorViewOriginalHashCode, out exception);
                    _connected.SendTimeout = 0;
                    _connected.ReceiveTimeout = 0;
                }
                catch
                {
                    if (++retry != 4)
                        goto Retry;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns>null on exception.</returns>
        public string GetConfiguration(out Exception exception)
        {
            Message<Key> message = new Message<Key>(Key.Configuration, _monitorViewOriginalHashCode);
            SynchronizeBuffers(message, out exception);
            if (exception == null)
                message = SendAndReceive(message, out exception);
            return message.Content as string;
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                try
                {
                    _isDisposed = true;
                    Disconnect();
                    _onReceiveCallBack = null;
                    _asyncResult = null;
                }
                catch { }
            }
        }
        #endregion
    }
}