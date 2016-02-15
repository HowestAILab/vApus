/*
 * Copyright 2016 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
 using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace vApus.PublishItemsHandler {
    public static class QueuedListener {
        private static TcpListener _listener;
        private static bool _isListening;
        private static SimpleMessageQueue _queue;

        static QueuedListener() { }

        public static void Start(int port) {
            _isListening = true;
            _queue = new SimpleMessageQueue();
            _queue.OnDequeue += _queue_OnDequeue;

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start(int.MaxValue);

            ThreadPool.QueueUserWorkItem((state) => {
                while (_isListening)
                    try {
                        var client = _listener.AcceptTcpClient();
                        HandleRead(new StreamReader(client.GetStream(), Encoding.UTF8));
                    }
                    catch {

                    }
            });
        }

        public static void Stop() {
            _isListening = false;
        }

        private static void HandleRead(StreamReader sr) {
            ThreadPool.QueueUserWorkItem((state) => {
                try {
                    string msg;
                    while (_isListening) {
                        msg = sr.ReadLine();
                        try {
                            dynamic intermediate = JObject.Parse(msg);
                            //A validating step.
                            var item = intermediate.ToObject(Assembly.GetExecutingAssembly().GetType("vApus.Publish." + intermediate.PublishItemType));
                            _queue.Enqueue(item);
                        }
                        catch (Exception ex) {
                            Debug.WriteLine("Faled parsing msg " + msg + " " + ex.ToString());
                        }
                    }
                }
                catch {
                    Debug.WriteLine("Faled reading line, connection was closed.");
                }
            });
        }

        private static void _queue_OnDequeue(object sender, SimpleMessageQueue.OnDequeueEventArgs e) { PublishItemHandler.Handle(e.Messages); }
    }
}
