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
    /// <summary>
    /// Jumpstarting amongst others is handled here.
    /// </summary>
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
                    case Key.SmartUpdate:
                        return HandleSmartUpdate(message);
                }
            } catch {
                //Handled later.
            }
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
                    _handleJumpStartWorkItem.HandleJumpStart(int.Parse(ports[(int)state]), processorAffinity[(int)state]);
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
                //Don't care.
            }
        }

        /// <summary>
        /// If the content of the returned message is empty it is failed.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static Message<Key> HandleSmartUpdate(Message<Key> message) {
            var smartUpdateMessage = (SmartUpdateMessage)message.Content;
            UpdateNotifier.Refresh();
            string currentVersion = UpdateNotifier.CurrentVersion;
            string currentChannel = UpdateNotifier.CurrentChannel.ToLower();

            string channel = smartUpdateMessage.Channel == 0 ? "stable" : "nightly";
            if (currentVersion != smartUpdateMessage.Version || currentChannel != channel) {
                if (smartUpdateMessage.Host != null) {
                    string path = Path.Combine(Application.StartupPath, "vApus.UpdateToolLoader.exe");
                    if (File.Exists(path)) {
                        var process = new Process();
                        process.EnableRaisingEvents = true;
                        string password = smartUpdateMessage.Password.Decrypt("{A84E447C-3734-4afd-B383-149A7CC68A32}",
                                                               new byte[]
                                                                   {
                                                                       0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76
                                                                       , 0x65, 0x64, 0x65, 0x76
                                                                   });
                        process.StartInfo = new ProcessStartInfo(path, "{A84E447C-3734-4afd-B383-149A7CC68A32} " + smartUpdateMessage.Host + " " +
                                                                 smartUpdateMessage.Port + " " + smartUpdateMessage.Username + " " + password + " " + smartUpdateMessage.Channel +
                                                                 " " + false + " " + true);

                        process.Start();
                        process.WaitForExit();
                    } else {
                        message.Content = null;
                    }
                }
            }
            return message;
        }
        #endregion

        private class HandleJumpStartWorkItem {
            public void HandleJumpStart(int port, string processorAffinity) {
                var p = new Process();
                try {
                    string vApusLocation = Path.Combine(Application.StartupPath, "vApus.exe");

                    if (processorAffinity.Length == 0)
                        p.StartInfo = new ProcessStartInfo(vApusLocation, string.Concat("-p ", port));
                    else
                        p.StartInfo = new ProcessStartInfo(vApusLocation, string.Concat("-p ", port, " -pa ", processorAffinity));

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
                        //Ignore.
                    }
                    p = null;
                }
            }
        }
    }
}