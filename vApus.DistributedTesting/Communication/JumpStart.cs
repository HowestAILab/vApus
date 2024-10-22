﻿/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using vApus.JumpStartStructures;
using vApus.Util;

namespace vApus.DistributedTest {
    /// <summary>
    /// Starts, kills and smart updates vApus slaves.
    /// </summary>
    public static class JumpStart {
        public static event EventHandler<DoneEventArgs> Done;

        #region Fields
        [ThreadStatic]
        private static WorkItem _workItem;
        private static readonly object _lock = new object();
        #endregion

        #region Functions
        /// <summary>
        /// The update notifier must be refreshed before calling this.
        /// Use await keyword when calling this.
        /// </summary>
        /// <param name="distributedTest"></param>
        /// <returns></returns>
        public static Task<Exception[]> SmartUpdate(DistributedTest distributedTest) {
            return Task<Exception[]>.Run(() => {
                lock (_lock) {
                    var ips = new List<string>();
                    foreach (Tile t in distributedTest.Tiles)
                        if (t.Use)
                            foreach (TileStressTest ts in t)
                                if (ts.Use)
                                    foreach (var slave in ts.BasicTileStressTest.Slaves) {
                                        string ip = slave.IP;
                                        if (!ips.Contains(ip))
                                            ips.Add(ip);
                                    }

                    string version, host, username, privateRSAKeyPath;
                    int port, channel;
                    bool smartUpdate;
                    version = UpdateNotifier.CurrentVersion;
                    UpdateNotifier.GetCredentials(out host, out port, out username, out privateRSAKeyPath, out channel, out smartUpdate);

                    var exs = new ConcurrentBag<Exception>();
                    int count = ips.Count;
                    if (count != 0) {
                        int i = 0;
                        var waithandle = new AutoResetEvent(false);

                        foreach (string ip in ips) {
                            var t = new Thread(delegate(object state) {
                                _workItem = new WorkItem();
                                exs.Add(_workItem.DoSmartUpdate(state as string, version, host, username, privateRSAKeyPath, port, channel));

                                if (Interlocked.Increment(ref i) == count) waithandle.Set();
                            });
                            t.IsBackground = true;
                            t.Start(ip);
                        }
                        waithandle.WaitOne();
                        waithandle.Dispose();
                        waithandle = null;
                    }
                    var l = new List<Exception>();
                    foreach (Exception ex in exs)
                        if (ex != null) l.Add(ex);

                    return l.ToArray();
                }
            });
        }

        /// <summary>
        ///     Jump start the used slaves in the test. Kill all slaves on the used clients first.
        /// </summary>
        /// <param name="distributedTest"></param>
        public static void Do(DistributedTest distributedTest) {
            var slaves = new List<Slave>();
            foreach (Tile t in distributedTest.Tiles)
                if (t.Use)
                    foreach (TileStressTest ts in t)
                        if (ts.Use) slaves.AddRange(ts.BasicTileStressTest.Slaves);

            Do(slaves);
        }
        private static void Do(List<Slave> slaves) {
            Hashtable toKill = RegisterForKill(slaves);

            var toJumpStart = new Hashtable(slaves.Count);
            foreach (Slave slave in slaves)
                RegisterForJumpStart(toJumpStart, slave.IP, slave.Port);

            Do(toKill, toJumpStart);
        }

        private static void RegisterForJumpStart(Hashtable toJumpStart, string ip, int port) {
            string s = port.ToString();
            if (toJumpStart.ContainsKey(ip)) {
                var ports = toJumpStart[ip] as string;
                if (!ports.Contains(s))
                    ports += "," + s;
                toJumpStart[ip] = ports;
            } else {
                toJumpStart.Add(ip, port.ToString());
            }
        }

        private static Hashtable RegisterForKill(List<Slave> slaves) {
            var toKill = new Hashtable(slaves.Count);
            foreach (Slave slave in slaves) RegisterForKill(toKill, slave.IP);

            return toKill;
        }
        /// <summary>
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="processID">-1 tot kill all on the client</param>
        private static void RegisterForKill(Hashtable toKill, string ip) {
            if (!toKill.ContainsKey(ip)) {
                int exclude = -1;
                bool alreadyExcludingMaster = false;
                foreach (object value in toKill.Values)
                    if (value != null && (int)value != -1) {
                        alreadyExcludingMaster = true;
                        break;
                    }

                if (!alreadyExcludingMaster) {
                    // Dns.GetHostName() does not always work. It gets the local host name, but not the name returned from the DNS server.
                    string masterHostName = Dns.GetHostEntry("127.0.0.1").HostName.Trim().Split('.')[0].ToLowerInvariant();
                    try {
                        string slaveHostName = Dns.GetHostEntry(ip).HostName.Trim().Split('.')[0].ToLowerInvariant();

                        if (masterHostName == slaveHostName) exclude = Process.GetCurrentProcess().Id;
                    } catch {
                        var ipAddresses = Dns.GetHostAddresses(masterHostName);
                        foreach (var ipAddress in ipAddresses)
                            if (ipAddress.ToString() == ip) {
                                exclude = Process.GetCurrentProcess().Id;
                                break;
                            }
                    }
                }
                toKill.Add(ip, exclude);
            }
        }

        /// <summary>
        ///     Use only this Do.
        ///     Use the Done event and mind when calling this (on another thread).
        /// </summary>
        private static void Do(Hashtable toKill, Hashtable toJumpStart) {
            var worker = new Thread(delegate() {
                Do(toKill, false);
                Exception[] exceptions = Do(toJumpStart, true);

                if (Done != null) {
                    var invocationList = Done.GetInvocationList();
                    Parallel.For(0, invocationList.Length, (i) => {
                        (invocationList[i] as EventHandler<DoneEventArgs>).Invoke(null, new DoneEventArgs(exceptions));
                    });
                }
            });
            worker.IsBackground = true;
            worker.Start();
        }
        /// <summary>
        ///     Error handling happens afterwards.
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="jumpStart">true for jumpstart false for kill</param>
        private static Exception[] Do(Hashtable ht, bool jumpStart) {
            lock (_lock) {
                var exceptions = new ConcurrentBag<Exception>();
                int count = ht.Count;
                if (count != 0) {
                    int i = 0;
                    var waithandle = new AutoResetEvent(false);

                    foreach (object keyvalue in ht) {
                        var t = new Thread(delegate(object state) {
                            var dictionaryEntry = (DictionaryEntry)state;
                            _workItem = new WorkItem();

                            if (jumpStart)
                                exceptions.Add(_workItem.DoJumpStart(dictionaryEntry.Key as string, dictionaryEntry.Value as string));
                            else
                                _workItem.DoKill(dictionaryEntry.Key as string, (int)dictionaryEntry.Value);

                            if (Interlocked.Increment(ref i) == count) waithandle.Set();
                        });
                        t.IsBackground = true;
                        t.Start(keyvalue);
                    }
                    waithandle.WaitOne();
                    waithandle.Dispose();
                    waithandle = null;
                }
                ht.Clear();

                var l = new List<Exception>();
                foreach (Exception ex in exceptions)
                    if (ex != null) l.Add(ex);

                return l.ToArray();
            }
        }
        #endregion

        public class DoneEventArgs : EventArgs {
            /// <summary>
            ///     One exception per slave.
            /// </summary>
            public readonly Exception[] Exceptions;
            public DoneEventArgs(Exception[] exceptions) { Exceptions = exceptions; }
        }

        private class WorkItem {
            /// <summary>
            /// Ip is the ip of the client, the rest are update credentials.
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="version"></param>
            /// <param name="host"></param>
            /// <param name="username"></param>
            /// <param name="privateRSAKeyPath"></param>
            /// <param name="port"></param>
            /// <param name="channel"></param>
            /// <returns></returns>
            public Exception DoSmartUpdate(string ip, string version, string host, string username, string privateRSAKeyPath, int port, int channel) {
                SocketWrapper socketWrapper = null;
                Exception exception = null;
                try {
                    socketWrapper = Connect(ip);
                    if (socketWrapper == null) throw new Exception("Could not connect to the vApus Jump Start Service!");
                    var smartUpdateMessage = new SmartUpdateMessage() { Version = version, Host = host, Username = username, PrivateRSAKeyPath = privateRSAKeyPath, Channel = channel, Port = port };
                    var message = new Message<JumpStartStructures.Key>(JumpStartStructures.Key.SmartUpdate, smartUpdateMessage);

                    socketWrapper.Send(message, SendType.Binary);
                    message = (Message<JumpStartStructures.Key>)socketWrapper.Receive(SendType.Binary);

                    if (message.Content == null)
                        throw new Exception();
                } catch (SocketException se) {
                    if (se.ErrorCode == 10054) { //Connection closed because updating.
                        int retry = 0;
                        do {
                            socketWrapper = Connect(ip);
                        } while (socketWrapper == null && retry++ != 4);

                        if (socketWrapper == null)
                            exception = new Exception(string.Concat("Failed to update vApus@", ip));
                    } else {
                        exception = new Exception(string.Concat("Failed to update vApus@", ip));
                    }
                } catch {
                    exception = new Exception(string.Concat("Failed to update vApus@", ip));
                }

                if (socketWrapper != null) {
                    try {
                        if (socketWrapper.Connected) socketWrapper.Close();
                    } catch {
                        //Ignore.
                    }
                    socketWrapper = null;
                }

                return exception;
            }

            /// <summary>
            ///     Error handling happens afterwards.
            /// </summary>
            /// <param name="slaveProcessID">if -1 all will be killed</param>
            public void DoKill(string ip, int excludeProcessID) {
                SocketWrapper socketWrapper = null;
                try {
                    socketWrapper = Connect(ip);
                    if (socketWrapper == null) throw new Exception("Could not connect to the vApus Jump Start Service!");

                    var killMessage = new KillMessage(excludeProcessID);
                    var message = new Message<JumpStartStructures.Key>(JumpStartStructures.Key.Kill, killMessage);

                    socketWrapper.Send(message, SendType.Binary);
                    message = (Message<JumpStartStructures.Key>)socketWrapper.Receive(SendType.Binary);
                } catch (Exception ex) {
                    Loggers.Log(Level.Warning, "Failed killing slaves.", ex);
                }

                if (socketWrapper != null) {
                    try {
                        if (socketWrapper.Connected) socketWrapper.Close();
                    } catch {
                        //Ignore.
                    }
                    socketWrapper = null;
                }
            }

            /// <summary>
            ///     Error handling happens afterwards.
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="port"></param>
            public Exception DoJumpStart(string ip, string port) {
                Exception exception = null;
                SocketWrapper socketWrapper = null;
                try {
                    socketWrapper = Connect(ip);
                    if (socketWrapper == null) throw new Exception("Could not connect to the vApus Jump Start Service!");

                    var jumpStartMessage = new JumpStartMessage(port);
                    var message = new Message<JumpStartStructures.Key>(JumpStartStructures.Key.JumpStart, jumpStartMessage);

                    socketWrapper.Send(message, SendType.Binary);
                    message = (Message<JumpStartStructures.Key>)socketWrapper.Receive(SendType.Binary);

                    if (message.Content == null) throw new Exception("The vApus process won't start, take a look client side.");

                    jumpStartMessage = (JumpStartMessage)message.Content;
                } catch (Exception ex) {
                    exception = new Exception("JumpStart failed for " + ip + ":" + port + "\n" + ex);
                }

                if (socketWrapper != null) {
                    try {
                        if (socketWrapper.Connected)
                            socketWrapper.Close();
                    } catch {
                        //Ignore.
                    }
                    socketWrapper = null;
                }

                return exception;
            }

            private SocketWrapper Connect(string ip) {
                var address = IPAddress.Parse(ip);
                var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                int port = 1314;

                var socketWrapper = new SocketWrapper(address, port, socket);
                socketWrapper.SendTimeout = 3000;
                socketWrapper.ReceiveTimeout = 300000;

                try { socketWrapper.Connect(5000, 3); } catch { }

                if (socketWrapper.Connected) return socketWrapper;

                return null;
            }
        }
    }
}