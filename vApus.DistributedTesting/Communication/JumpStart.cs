/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public static class JumpStart
    {
        public static event EventHandler Done;

        private static object _lock = new object();
        [ThreadStatic]
        private static WorkItem _workItem;

        /// <summary>
        /// Jump start the used slaves in the test. Kill all slaves on the used clients first.
        /// </summary>
        /// <param name="distributedTest"></param>
        public static void Do(DistributedTest distributedTest)
        {
            List<Slave> slaves = new List<Slave>();
            foreach (Tile t in distributedTest.Tiles)
                if (t.Use)
                    foreach (TileStresstest ts in t)
                        if (ts.Use)
                            slaves.AddRange(ts.BasicTileStresstest.Slaves);

            Do(slaves);
        }
        private static void Do(List<Slave> slaves)
        {
            Hashtable toKill = RegisterForKill(slaves);

            Hashtable toJumpStart = new Hashtable(slaves.Count);
            foreach (Slave slave in slaves)
                RegisterForJumpStart(toJumpStart, slave.IP, slave.Port);

            Do(toKill, toJumpStart);
        }

        private static void RegisterForJumpStart(Hashtable toJumpStart, string ip, int port)
        {
            string s = port.ToString();
            if (toJumpStart.ContainsKey(ip))
            {
                string value = toJumpStart[ip] as string;
                if (!value.Contains(s))
                    value += "," + s;
                toJumpStart[ip] = value;
            }
            else
            {
                toJumpStart.Add(ip, s);
            }
        }
        private static Hashtable RegisterForKill(List<Slave> slaves)
        {
            Hashtable toKill = new Hashtable(slaves.Count);
            foreach (Slave slave in slaves)
                RegisterForKill(toKill, slave.IP);
            return toKill;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="processID">-1 tot kill all on the client</param>
        private static void RegisterForKill(Hashtable toKill, string ip)
        {
            if (!toKill.ContainsKey(ip))
                toKill.Add(ip, SocketListener.GetInstance().IP == ip ? Process.GetCurrentProcess().Id : -1);
        }
        /// <summary>
        /// Use only this Do.
        /// Use the Done event and mind when calling this (on another thread).
        /// </summary>
        private static void Do(Hashtable toKill, Hashtable toJumpStart)
        {
            Thread worker = new Thread(delegate()
            {
                DoKill(toKill);
                DoJumpStart(toJumpStart);

                if (Done != null)
                    Done(null, null);
            });
            worker.IsBackground = true;
            worker.Start();
        }
        private static void DoKill(Hashtable toKill)
        {
            Do(toKill, false);
        }
        private static void DoJumpStart(Hashtable toJumpStart)
        {
            Do(toJumpStart, true);
        }
        /// <summary>
        /// Error handling happens afterwards.
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="jumpStart">true for jumpstart false for kill</param>
        private static void Do(Hashtable ht, bool jumpStart)
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

                        if (jumpStart)
                            _workItem.DoJumpStart(kvp.Key as string, kvp.Value as string);
                        else
                            _workItem.DoKill(kvp.Key as string, (int)kvp.Value);

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
            public void DoKill(string ip, int excludeProcessID)
            {
                SocketWrapper socketWrapper = null;
                try
                {
                    socketWrapper = Connect(ip);
                    if (socketWrapper == null)
                        throw new Exception("Could not connect to the vApus Jump Start Service!");

                    var killMessage = new vApus.JumpStartStructures.KillMessage(excludeProcessID);
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
