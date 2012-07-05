/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public static class JumpStart
    {
        public static event EventHandler<DoneEventArgs> Done;

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
                RegisterForJumpStart(toJumpStart, slave.IP, slave.Port, slave.ProcessorAffinity);

            Do(toKill, toJumpStart);
        }

        private static void RegisterForJumpStart(Hashtable toJumpStart, string ip, int port, int[] processorAffinity)
        {
            string s = port.ToString();
            if (toJumpStart.ContainsKey(ip))
            {
                var kvp = (KeyValuePair<string, string>)toJumpStart[ip];
                if (!kvp.Key.Contains(s))
                    kvp = new KeyValuePair<string, string>(kvp.Key + "," + s, kvp.Value + "," + GetZeroBasedPA(processorAffinity));
                toJumpStart[ip] = kvp;
            }
            else
            {
                var kvp = new KeyValuePair<string, string>(port.ToString(), GetZeroBasedPA(processorAffinity));
                toJumpStart.Add(ip, kvp);
            }
        }
        private static string GetZeroBasedPA(int[] processorAffinity)
        {
            int[] pa = new int[processorAffinity.Length];

            for (int i = 0; i != pa.Length; i++)
                pa[i] = processorAffinity[i] - 1;

            return pa.Combine(" ");
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
                Exception[] exceptions = DoJumpStart(toJumpStart);

                if (Done != null)
                    foreach (EventHandler<DoneEventArgs> del in Done.GetInvocationList())
                        del.BeginInvoke(null, new DoneEventArgs(exceptions), null, null);
            });
            worker.IsBackground = true;
            worker.Start();
        }
        private static void DoKill(Hashtable toKill)
        {
            Do(toKill, false);
        }
        private static Exception[] DoJumpStart(Hashtable toJumpStart)
        {
            return Do(toJumpStart, true);
        }
        /// <summary>
        /// Error handling happens afterwards.
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="jumpStart">true for jumpstart false for kill</param>
        private static Exception[] Do(Hashtable ht, bool jumpStart)
        {
            lock (_lock)
            {
                ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();
                int count = ht.Count;
                if (count != 0)
                {
                    int i = 0;
                    AutoResetEvent waithandle = new AutoResetEvent(false);

                    foreach (var keyvalue in ht)
                    {
                        Thread t = new Thread(delegate(object state)
                        {
                            var dictionaryEntry = (DictionaryEntry)state;
                            _workItem = new WorkItem();

                            if (jumpStart)
                            {
                                string ip = dictionaryEntry.Key as string;
                                var kvp = (KeyValuePair<string, string>)ht[ip];

                                exceptions.Add(
                                    _workItem.DoJumpStart(ip, kvp.Key, kvp.Value)
                                );
                            }
                            else
                            {
                                _workItem.DoKill(dictionaryEntry.Key as string, (int)dictionaryEntry.Value);
                            }

                            if (Interlocked.Increment(ref i) == count)
                                waithandle.Set();
                        });
                        t.IsBackground = true;
                        t.Start(keyvalue);
                    }
                    waithandle.WaitOne();
                    waithandle.Dispose();
                    waithandle = null;

                    //Be sure they are jump started before trying to connect to them.
                    Thread.Sleep(3000);
                }
                ht.Clear();

                List<Exception> l = new List<Exception>();
                foreach (Exception ex in exceptions)
                    if (ex != null)
                        l.Add(ex);

                return l.ToArray();
            }
        }

        private class WorkItem
        {
            /// <summary>
            /// Error handling happens afterwards.
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="port"></param>
            public Exception DoJumpStart(string ip, string port, string processorAffinity)
            {
                Exception exception = null;
                SocketWrapper socketWrapper = null;
                try
                {
                    socketWrapper = Connect(ip);
                    if (socketWrapper == null)
                        throw new Exception("Could not connect to the vApus Jump Start Service!");

                    var jumpStartMessage = new vApus.JumpStartStructures.JumpStartMessage(ip, port, processorAffinity);
                    var message = new Message<vApus.JumpStartStructures.Key>(vApus.JumpStartStructures.Key.JumpStart, jumpStartMessage);

                    socketWrapper.Send(message, SendType.Binary);
                    message = (Message<vApus.JumpStartStructures.Key>)socketWrapper.Receive(SendType.Binary);

                    if (message.Content == null)
                        throw new Exception("The vApus process won't start, take a look client side.");

                    jumpStartMessage = (vApus.JumpStartStructures.JumpStartMessage)message.Content;
                }
                catch (Exception ex)
                {
                    exception = new Exception("JumpStart failed for " + ip + ":" + port + "\n" + ex.ToString());
                }

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

                return exception;
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
                catch { }

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

                int port = 1314;

                var socketWrapper = new SocketWrapper(ip, port, socket);
                socketWrapper.SendTimeout = 3000;
                socketWrapper.ReceiveTimeout = 120000;

                try
                {
                    socketWrapper.Connect(3000, 2);
                }
                catch { }

                if (socketWrapper.Connected)
                    return socketWrapper;

                return null;
            }
        }
        public class DoneEventArgs : EventArgs
        {
            /// <summary>
            /// One exception per slave.
            /// </summary>
            public readonly Exception[] Exceptions;
            public DoneEventArgs(Exception[] exceptions)
            {
                Exceptions = exceptions;
            }
        }
    }
}
