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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.JumpStartStructures;
using vApus.Util;

namespace vApus.JumpStart {
    public static class CommunicationHandler {
        [ThreadStatic]
        private static HandleJumpStartWorkItem _handleJumpStartWorkItem;

        #region Message Handling

        public static Message<Key> HandleMessage(SocketWrapper receiver, Message<Key> message) {
            try {
                switch (message.Key) {
                    case Key.JumpStart:
                        return HandleJumpStart(message);
                    case Key.Kill:
                        return HandleKill(message);
                    case Key.CpuCoreCount:
                        return HandleCpuCoreCount(message);
                }
            } catch { }
            return message;
        }

        private static Message<Key> HandleJumpStart(Message<Key> message) {
            var jumpStartMessage = (JumpStartMessage)message.Content;
            string[] ports = jumpStartMessage.Port.Split(',');
            string[] processorAffinity = jumpStartMessage.ProcessorAffinity.Split(',');

            var waithandle = new AutoResetEvent(false);
            int j = 0;
            for (int i = 0; i != ports.Length; i++) {
                var t = new Thread(delegate(object state) {
                    _handleJumpStartWorkItem = new HandleJumpStartWorkItem();
                    _handleJumpStartWorkItem.HandleJumpStart(jumpStartMessage.IP, int.Parse(ports[(int)state]),
                                                             processorAffinity[(int)state]);
                    if (Interlocked.Increment(ref j) == ports.Length)
                        waithandle.Set();
                });
                t.IsBackground = true;
                t.Start(i);
            }

            waithandle.WaitOne();

            return message;
        }

        private static Message<Key> HandleKill(Message<Key> message) {
            var killMessage = (KillMessage)message.Content;
            Kill(killMessage.ExcludeProcessID);
            return message;
        }

        private static void Kill(int excludeProcessID) {
            Process[] processes = Process.GetProcessesByName("vApus");
            Parallel.ForEach(processes, delegate(Process p) {
                if (excludeProcessID == -1 || p.Id != excludeProcessID)
                    KillProcess(p);
            });
        }

        private static void KillProcess(Process p) {
            try {
                if (!p.HasExited) {
                    p.Kill();
                    p.WaitForExit(10000);
                }
            } catch {
            }
        }

        private static Message<Key> HandleCpuCoreCount(Message<Key> message) {
            var cpuCoreCountMessage = new CpuCoreCountMessage(Environment.ProcessorCount);
            message.Content = cpuCoreCountMessage;
            return message;
        }

        #endregion

        private class HandleJumpStartWorkItem {
            public void HandleJumpStart(string ip, int port, string processorAffinity) {
                var p = new Process();
                try {
                    string vApusLocation = Path.Combine(Application.StartupPath, "vApus.exe");

                    if (processorAffinity.Length == 0)
                        p.StartInfo = new ProcessStartInfo(vApusLocation, "-ipp " + ip + ":" + port);
                    else
                        p.StartInfo = new ProcessStartInfo(vApusLocation,
                                                           "-ipp " + ip + ":" + port + " -pa " + processorAffinity);

                    p.Start();
                    if (!p.WaitForInputIdle(10000))
                        throw new TimeoutException("The process did not start.");
                } catch {
                    try {
                        if (!p.HasExited) {
                            p.Kill();
                            p.WaitForExit(10000);
                        }
                    } catch {
                    }
                    p = null;
                }
            }
        }
    }
}