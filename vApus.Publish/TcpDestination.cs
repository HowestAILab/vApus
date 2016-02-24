/*
 * Copyright 2016 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace vApus.Publish {
    internal class TcpDestination : IDisposable {
        private TcpClient _tcpClient;
        private StreamWriter _sw;
        private string _host;
        private int _port;

        private bool _connectionErrorLogged = false;

        private IFormatter _formatter;

        public TcpDestination() { }

        /// <summary>
        /// Inits if needed.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns>False if already inited.</returns>
        public void Init(string host, int port, IFormatter formatter) {
            try {
                _formatter = formatter;
                if (Connected && _host == host && _port == port) return;

                _host = host;
                _port = port;

                if (_sw != null) {
                    _sw.Dispose();
                    _sw = null;
                }
                if (_tcpClient != null) {
                    _tcpClient.Dispose();
                    _tcpClient = null;
                }

                _tcpClient = new TcpClient(Dns.GetHostEntry(_host).HostName, _port);
                _sw = new StreamWriter(_tcpClient.GetStream(), Encoding.UTF8);
                _connectionErrorLogged = false;
            }
            catch {
                if (!_connectionErrorLogged) { //Log only once.
                    Loggers.Log(Level.Error, "Failed to connect to the publisher's TCP endpoint.");
                    _connectionErrorLogged = true;
                }
            }
        }
        private bool Connected { get { return _tcpClient != null && _tcpClient.Connected; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public bool Send(object message) {
            if (Connected) {
                try {
                    _sw.WriteLine(FormatMessage(message));
                    _sw.Flush();
                    return true;
                }
                catch {
                }
            }
            return false;
        }
        private object FormatMessage(object message) { return (_formatter == null) ? message : _formatter.Format(message); }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose() {
            if (_sw != null) {
                _sw.Dispose();
                _sw = null;
            }
            if (_tcpClient != null) {
                _tcpClient.Dispose();
                _tcpClient = null;
            }
        }
    }
}
