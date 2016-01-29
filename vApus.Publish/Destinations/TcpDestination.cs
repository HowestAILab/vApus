/*
 * Copyright 2016 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace vApus.Publish {
    internal class TcpDestination : BaseDestination, IDisposable {
        private static TcpDestination _instance;

        private TcpClient _tcpClient;
        private StreamWriter _sw;
        private string _host;
        private int _port;

        public override bool AllowMultipleInstances { get { return false; } }

        private TcpDestination() { }

        public static TcpDestination GetInstance() {
            if (_instance == null) _instance = new TcpDestination();
            return _instance;
        }

        /// <summary>
        /// Inits if needed.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns>False if already inited.</returns>
        public bool Init(string host, int port) {
            if (Connected && _host == host && _port == port) return false;

            Connect(host, port);
            _host = Dns.GetHostEntry(host).HostName;
            _port = port;
            return true;
        }
        private bool Connected { get { return _tcpClient != null && _tcpClient.Connected; } }
        private void Connect(string host, int port) {
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
            _tcpClient = new TcpClient(_host, _port);
            _sw = new StreamWriter(_tcpClient.GetStream(), Encoding.UTF8);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void Post(object message) {
            _sw.WriteLine(FormatMessage(message));
            _sw.Flush();
        }
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
