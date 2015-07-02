/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using System.Net;
using System.Net.Sockets;

namespace vApus.Publish {
    internal class BroadcastDestination : BaseDestination {
        private Socket _socketV4 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private Socket _socketV6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);

        private IPEndPoint _broadCastIPv4, _broadCastIPv6;

        private BroadcastDestination() { }
        public BroadcastDestination(int port) {
            _socketV4.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            _broadCastIPv4 = new IPEndPoint(IPAddress.Broadcast, port);
            _broadCastIPv6 = new IPEndPoint(IPAddress.Parse("ff02::1"), port);
        }

        public override void Post(object message) {
            byte[] buffer = SerializationHelper.Encode(FormatMessage(message) as string, SerializationHelper.TextEncoding.UTF8);
            _socketV4.SendTo(buffer, _broadCastIPv4);
            _socketV6.SendTo(buffer, _broadCastIPv6);
        }
    }
}
