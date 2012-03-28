/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public static class JumpStartOrKill
    {
        public static event EventHandler Done;

        private static object _lock = new object();
        [ThreadStatic]
        private static WorkItem _workItem;

        private static Hashtable _toJumpStart = new Hashtable(),
            _toKill = new Hashtable();

        public static void RegisterForJumpStart(string ip, int port)
        {
            AddToHashtable(_toJumpStart, ip, port);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="processID">-1 tot kill all on the client</param>
        public static void RegisterForKill(string ip, int processID)
        {
            AddToHashtable(_toKill, ip, processID);
        }
        private static void AddToHashtable(Hashtable ht, string ip, int portOrProcessID)
        {
            lock (ht.SyncRoot)
            {
                string s = portOrProcessID.ToString();
                if (ht.ContainsKey(ip))
                {
                    string value = ht[ip] as string;
                    if (!value.Contains(s))
                        value += "," + s;
                    ht[ip] = value;
                }
                else
                {
                    ht.Add(ip, s);
                }
            }
        }
        /// <summary>
        /// Use the Done event and mind when calling this (on another thread).
        /// </summary>
        public static void Do()
        {
            Thread worker = new Thread(delegate()
            {
                DoJumpStart();
                DoKill();

                if (Done != null)
                    Done(null, null);
            });
            worker.IsBackground = true;
            worker.Start();
        }
        private static void DoJumpStart()
        {
            Do(_toJumpStart);
        }
        private static void DoKill()
        {
            Do(_toKill);
        }
        /// <summary>
        /// Error handling happens afterwards.
        /// </summary>
        /// <param name="ht"></param>
        private static void Do(Hashtable ht)
        {
            lock (_lock)
            {
                int count = ht.Count;
                if (count == 0)
                    return;

                int i = 0;
                AutoResetEvent waithandle = new AutoResetEvent(false);

                foreach (var keyvalue in ht)
                {
                    Thread t = new Thread(delegate(object state)
                    {
                        var kvp = (DictionaryEntry)state;
                        _workItem = new WorkItem();

                        if (ht == _toJumpStart)
                            _workItem.DoJumpStart(kvp.Key as string, kvp.Value as string);
                        else
                            _workItem.DoKill(kvp.Key as string, kvp.Value as string);

                        if (Interlocked.Increment(ref i) == count)
                            waithandle.Set();
                    });
                    t.IsBackground = true;
                    t.Start(keyvalue);
                }
                waithandle.WaitOne();

                ht.Clear();
            }
        }

        private class WorkItem
        {
            /// <summary>
            /// Error handling happens afterwards.
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="port"></param>
            public void DoJumpStart(string ip, string port)
            {
                SocketWrapper socketWrapper = null;
                try
                {
                    socketWrapper = Connect(ip);
                    if (socketWrapper == null)
                        throw new Exception("Could not connect to the vApus Jump Start Service!");

                    var jumpStartMessage = new vApus.JumpStartStructures.JumpStartMessage(ip, port);
                    var message = new Message<vApus.JumpStartStructures.Key>(vApus.JumpStartStructures.Key.JumpStart, jumpStartMessage);

                    socketWrapper.Send(message, SendType.Binary);
                    message = (Message<vApus.JumpStartStructures.Key>)socketWrapper.Receive(SendType.Binary);

                    if (message.Content == null)
                        throw new Exception("The vApus process won't start, take a look client side.");

                    jumpStartMessage = (vApus.JumpStartStructures.JumpStartMessage)message.Content;
                }
                catch
                { }

                if (socketWrapper != null)
                {
                    try
                    {
                        if (socketWrapper.Connected)
                            socketWrapper.Close();
                    }
                    catch { }
                    socketWrapper = null;
                }
            }
            /// <summary>
            /// Error handling happens afterwards.
            /// </summary>
            /// <param name="slaveProcessID">if -1 all will be killed</param>
            public void DoKill(string ip, string slaveProcessID)
            {
                SocketWrapper socketWrapper = null;
                try
                {
                    socketWrapper = Connect(ip);
                    if (socketWrapper == null)
                        throw new Exception("Could not connect to the vApus Jump Start Service!");

                    var killMessage = new vApus.JumpStartStructures.KillMessage(slaveProcessID);
                    var message = new Message<vApus.JumpStartStructures.Key>(vApus.JumpStartStructures.Key.Kill, killMessage);

                    socketWrapper.Send(message, SendType.Binary);
                    message = (Message<vApus.JumpStartStructures.Key>)socketWrapper.Receive(SendType.Binary);
                }
                catch
                { }

                if (socketWrapper != null)
                {
                    try
                    {
                        if (socketWrapper.Connected)
                            socketWrapper.Close();
                    }
                    catch { }
                    socketWrapper = null;
                }
            }
            private SocketWrapper Connect(string ip)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Check on these ports
                for (int port = 1314; port <= 1316; port++)
                {
                    var socketWrapper = new SocketWrapper(ip, port, socket);
                    socketWrapper.SendTimeout = 3000;
                    socketWrapper.ReceiveTimeout = 20000;

                    try
                    {
                        socketWrapper.Connect(3000, 3);
                    }
                    catch { }

                    if (socketWrapper.Connected)
                        return socketWrapper;
                }
                return null;
            }
        }
    }
}
