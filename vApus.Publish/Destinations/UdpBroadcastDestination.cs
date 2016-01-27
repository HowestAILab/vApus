/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using System;
using System.Net;
using System.Net.Sockets;

namespace vApus.Publish {
    internal class UdpBroadcastDestination : BaseDestination, IDisposable {
        private Socket _socketV4 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private Socket _socketV6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);

        private IPEndPoint _broadCastIPv4, _broadCastIPv6;

        public override bool AllowMultipleInstances { get { return true; } }

        private UdpBroadcastDestination() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        public UdpBroadcastDestination(int port) {
            _socketV4.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            _broadCastIPv4 = new IPEndPoint(IPAddress.Broadcast, port);
            _broadCastIPv6 = new IPEndPoint(IPAddress.Parse("ff02::1"), port);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void Post(object message) {
            byte[] buffer = SerializationHelper.Encode(FormatMessage(message) as string, SerializationHelper.TextEncoding.UTF8);
            _socketV4.SendTo(buffer, _broadCastIPv4);
            _socketV6.SendTo(buffer, _broadCastIPv6);
        }

        public void Dispose() {
            if (_socketV4 != null) {
                _socketV4.Dispose();
                _socketV4 = null;
            }
            if (_socketV6 != null) {
                _socketV6.Dispose();
                _socketV6 = null;
            }
        }
    }
}
