/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Net;
using System.Net.NetworkInformation;

namespace vApus.Util
{
    public class Tracert : IDisposable
    {
        public event EventHandler<TracedNodeEventArgs> TracedNode;
        public event EventHandler Done;

        #region Fields
        private IPAddress _ip;
        private int _hops, _maxHops, _timeout;

        private Ping _ping;
        private PingOptions _options;
        private static byte[] _buffer;

        private bool _isDone = false;
        #endregion

        private static byte[] Buffer
        {
            get
            {
                if (_buffer == null)
                {
                    _buffer = new byte[32];
                    for (int i = 0; i < Buffer.Length; i++)
                        _buffer[i] = 0x65;
                }
                return _buffer;
            }
        }

        #region Functions
        /// <summary>
        /// </summary>
        /// <param name="hostNameOrIP"></param>
        /// <param name="maxHops"></param>
        /// <param name="timeout">in ms</param>
        public void Trace(string hostNameOrIP, int maxHops = 100, int timeout = 5000)
        {
            if (_ping != null)
                throw new Exception("Another trace is still in route!");

            //Set the fields --> asynchonically handled
            _ip = Dns.GetHostEntry(hostNameOrIP).AddressList[0];
            _hops = 0;
            _maxHops = maxHops;
            _timeout = timeout;

            _isDone = false;

            if (IPAddress.IsLoopback(_ip))
            {
                ProcessTraceNode(_ip, IPStatus.Success);
            }
            else
            {
                _ping = new Ping();

                _ping.PingCompleted += new PingCompletedEventHandler(OnPingCompleted);
                _options = new PingOptions(1, true);
                _ping.SendAsync(_ip, _timeout, Tracert.Buffer, _options, null);
            }
        }
        private void OnPingCompleted(object sender, PingCompletedEventArgs e)
        {
            try
            {
                if (!_isDone)
                {
                    _options.Ttl += 1;
                    //The expectation was that SendAsync will throw an exception
                    if (_ping == null)
                    {
                        ProcessTraceNode(_ip, IPStatus.Unknown);
                    }
                    else
                    {
                        ProcessTraceNode(e.Reply.Address, e.Reply.Status);
                        _ping.SendAsync(_ip, _timeout, Tracert.Buffer, _options, null);
                    }
                }
            }
            catch { }
        }
        protected void ProcessTraceNode(IPAddress address, IPStatus status)
        {
            long roundTripTime = 0;
            if (status == IPStatus.TtlExpired || status == IPStatus.Success)
            {
                Ping pingIntermediate = new Ping();
                try
                {
                    //Compute roundtrip time to the address by pinging it
                    PingReply reply = pingIntermediate.Send(address, _timeout);
                    roundTripTime = reply.RoundtripTime;
                    status = reply.Status;
                }
                catch (PingException e)
                {
                    //Do nothing
                    System.Diagnostics.Trace.WriteLine(e);
                }
                pingIntermediate.Dispose();
            }

            FireTracedNode(address, roundTripTime, status);

            _isDone = address.Equals(_ip) || ++_hops == _maxHops;
            if (_isDone)
                FireDone();
        }
        private void FireTracedNode(IPAddress ip, long roundTripTime, IPStatus status)
        {
            if (TracedNode != null)
            {
                string hostName = string.Empty;
                try { hostName = Dns.GetHostEntry(ip).HostName; }
                catch { }
                TracedNode(this, new TracedNodeEventArgs(ip.ToString(), hostName, roundTripTime, status));
            }
        }
        private void FireDone()
        {
            if (Done != null)
                Done(this, null);
        }
        #endregion

        public class TracedNodeEventArgs : EventArgs
        {
            public readonly string IP, HostName;
            /// <summary>
            /// in ms
            /// </summary>
            public readonly long RoundTripTime;
            public readonly IPStatus Status;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="hostName"></param>
            /// <param name="roundTripTime">in ms</param>
            /// <param name="status"></param>
            public TracedNodeEventArgs(string ip, string hostName, long roundTripTime, IPStatus status)
            {
                IP = ip;
                HostName = hostName;
                RoundTripTime = roundTripTime;
                Status = status;
            }
        }

        public void Dispose()
        {
            if (_ping != null)
                try
                {
                    _ping.Dispose();
                }
                catch { }
            _ping = null;
        }
    }
}
