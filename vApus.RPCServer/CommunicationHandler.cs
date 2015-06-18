/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.RPCServer {
    internal static class CommunicationHandler {
        private delegate string HandleMessageDelegate(string message);

        /// <summary>
        /// Holds the whole or the significant part of the path as Key. Specifics are handled by the matching delegate.
        /// </summary>
        private static Dictionary<string, HandleMessageDelegate> _delegates;
        private static string _vApusPort = "1337";
        private static string _vApus = Path.Combine(Application.StartupPath, "vApus.exe");

        static CommunicationHandler() {
            // Fill delegates
            _delegates = new Dictionary<string, HandleMessageDelegate>();

            _delegates.Add("/status", Status);

            _delegates.Add("/startvapus", StartVApus);
            _delegates.Add("/killvapus", KillvApus);

            //The rest that starts with "/" is communicated to vApus.
        }

        internal static string HandleMessage(string message) {
            foreach (string path in _delegates.Keys) {
                if (Match(message, path))
                    try {
                        return _delegates[path].Invoke(message);
                    } catch (Exception ex) {
                        return SerializeFailed("500 Internal Server Error. " + message + " Details: " + ex.Message);
                    }
            }
            if (message.StartsWith("/"))
                try {
                    return CommunicateToVApus(message);
                } catch (Exception ex) {
                    return SerializeFailed("500 Internal Server Error. " + message + " Details: " + ex.Message);
                }

            return SerializeFailed("404 Not Found. " + message);
        }
        private static bool Match(string message, string path) {
            if (message.StartsWith(path)) return true;

            //Walk the path in the message and find a match in the given path
            var messageNodes = message.Split('/');
            var pathNodes = path.Split('/');
            if (messageNodes.Length != pathNodes.Length) return false;

            for (int i = 0; i != messageNodes.Length; i++) {
                string messageNode = messageNodes[i];
                string pathNode = pathNodes[i];
                if (messageNode != pathNode) return false;
            }

            return true;
        }

        private static string Status(string message) {
            return SerializeSucces(message);
        }
        private static string StartVApus(string message) {
            var split = message.Split('/');
            string vass = split[2];
            _vApusPort = (split.Length == 4) ? split[3] : "1337";

            KillvApus(message);

            var p = Process.Start(_vApus, vass + " -p " + _vApusPort);
            p.WaitForInputIdle();
            while (p.MainWindowTitle == string.Empty || p.MainWindowTitle == "vApus") {
                Thread.Sleep(100);
                p.Refresh();
            }

            return SerializeSucces(message);
        }
        private static string KillvApus(string message) {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var socketWrapper = new SocketWrapper("127.0.0.1", 1314, socket);
            socketWrapper.Connect();

            var killMessage = new vApus.JumpStartStructures.KillMessage();
            var msg = new Message<vApus.JumpStartStructures.Key>(vApus.JumpStartStructures.Key.Kill, killMessage);

            socketWrapper.Send(msg, SendType.Binary);
            msg = (Message<vApus.JumpStartStructures.Key>)socketWrapper.Receive(SendType.Binary);

            socketWrapper.Close();

            return SerializeSucces(message);
        }
        private static string CommunicateToVApus(string message) {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var socketWrapper = new SocketWrapper("127.0.0.1", int.Parse(_vApusPort), socket);
            socketWrapper.Connect();

            var msg = new Message<vApus.Server.Shared.Key>(vApus.Server.Shared.Key.Other, message);

            socketWrapper.Send(msg, SendType.Binary);
            msg = (Message<vApus.Server.Shared.Key>)socketWrapper.Receive(SendType.Binary);

            socketWrapper.Close();

            return msg.Content as string;
        }

        private static string SerializeSucces(string message) { return SerializeStatusMessage("succes", message); }
        private static string SerializeFailed(string message) { return SerializeStatusMessage("failed", message); }
        private static string SerializeStatusMessage(string status, string message) { return JsonConvert.SerializeObject(new statusmessage() { status = status, message = message }); }

        private struct statusmessage {
            public string status;
            public string message;
        }
    }
}
