/*
 * Copyright 2012 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.JumpStartStructures;
using vApus.Util;
using System.Threading;
using System.Collections.Generic;

namespace vApus.JumpStart
{
    public static class CommunicationHandler
    {
        private static object _lock = new object();
        [ThreadStatic]
        private static HandleJumpStartWorkItem _handleJumpStartWorkItem;

        #region Message Handling

        public static Message<Key> HandleMessage(SocketWrapper receiver, Message<Key> message)
        {
            try
            {
                switch (message.Key)
                {
                    case Key.JumpStart:
                        return HandleJumpStart(message);
                    case Key.Kill:
                        return HandleKill(message);
                }
            }
            catch { }
            return message;
        }
        private static Message<Key> HandleJumpStart(Message<Key> message)
        {
            JumpStartMessage jumpStartMessage = (JumpStartMessage)message.Content;
            string[] ports = jumpStartMessage.Port.Split(',');

            AutoResetEvent waithandle = new AutoResetEvent(false);
            int j = 0;
            for (int i = 0; i != ports.Length; i++)
            {
                Thread t = new Thread(delegate(object state)
                {
                    _handleJumpStartWorkItem = new HandleJumpStartWorkItem();
                    _handleJumpStartWorkItem.HandleJumpStart(jumpStartMessage.IP, int.Parse(ports[(int)state]));
                    if (Interlocked.Increment(ref j) == ports.Length)
                        waithandle.Set();
                });
                t.IsBackground = true;
                t.Start(i);
            }

            waithandle.WaitOne();

            return message;
        }

        private static Process Launch_vApus(string ip, int port)
        {
            Process process = new Process();
            try
            {
                string vApusLocation = Path.Combine(Application.StartupPath, "vApus.exe");

                process.StartInfo = new ProcessStartInfo(vApusLocation, "-ipp " + ip + ':' + port);
                process.Start();
                if (!process.WaitForInputIdle(10000))
                    throw new TimeoutException("The process did not start.");
            }
            catch
            {
                try
                {
                    if (!process.HasExited)
                        process.Kill();
                }
                catch { }
                process = null;
            }
            return process;
        }

        private static Message<Key> HandleKill(Message<Key> message)
        {
            KillMessage killMessage = (KillMessage)message.Content;
            Kill(killMessage.ExcludeProcessID);
            return message;
        }
        private static void Kill(int excludeProcessID)
        {
            Process[] processes = Process.GetProcessesByName("vApus");
            Parallel.ForEach(processes, delegate(Process p)
            {
                if (excludeProcessID == -1 || p.Id != excludeProcessID)
                    KillProcess(p);
            });
        }
        private static void KillProcess(Process p)
        {
            try
            {
                if (!p.HasExited)
                {
                    p.Kill();
                    p.WaitForExit(10000);
                }
            }
            catch { }
        }
        #endregion

        private class HandleJumpStartWorkItem
        {
            public void HandleJumpStart(string ip, int port)
            {
                int processID = PollvApus(ip, port);
                lock (_lock)
                    if (processID == -1)
                        Launch_vApus(ip, port);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="port"></param>
            /// <returns>process id</returns>
            private int PollvApus(string ip, int port)
            {
                int processID = -1;
                SocketWrapper socketWrapper = null;
                try
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketWrapper = new SocketWrapper(ip, port, socket);
                    socketWrapper.Connect(2000, 3);
                    if (socketWrapper.Connected)
                    {
                        Message<vApus.DistributedTesting.Key> message = new Message<vApus.DistributedTesting.Key>(vApus.DistributedTesting.Key.Poll, null);
                        socketWrapper.SendTimeout = 3000;
                        socketWrapper.ReceiveTimeout = 3000;

                        socketWrapper.Send(message, SendType.Binary);
                        message = (Message<vApus.DistributedTesting.Key>)socketWrapper.Receive(SendType.Binary);

                        if (message.Content != null)
                        {
                            vApus.DistributedTesting.PollMessage pollMessage = (vApus.DistributedTesting.PollMessage)message.Content;
                            processID = pollMessage.ProcessID;
                        }
                    }
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

                return processID;
            }
        }
    }
}