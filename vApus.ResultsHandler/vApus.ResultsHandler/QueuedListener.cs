using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vApus.Publish;

namespace vApus.ResultsHandler {
    public static class QueuedListener {
        private static TcpListener _listener;
        private static bool _isListening;
        private static MessageQueue _queue;

        static QueuedListener() { }

        public static void Start(int port = 3337) {
            _isListening = true;
            _queue = new MessageQueue();
            _queue.OnDequeue += _queue_OnDequeue;

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start(int.MaxValue);

            ThreadPool.QueueUserWorkItem((state) => {
                while (!_isListening) {
                    HandleRead(new StreamReader(_listener.AcceptTcpClient().GetStream(), Encoding.UTF8));
                }
            });
        }

        public static void Stop() {
            _isListening = false;
        }

        private static void HandleRead(StreamReader sr) {
            while (!_isListening) {
                string msg = sr.ReadLine();
                dynamic intermediate = JObject.Parse(msg);
                //A validating step.
                var item = intermediate.ToObject(Assembly.GetExecutingAssembly().GetType("vApus.Publish." + intermediate.PublishItemType));
                _queue.Enqueue(item);
            }
        }

        private static void _queue_OnDequeue(object sender, MessageQueue.OnDequeueEventArgs e) { PublishItemHandler.Handle(e.Message as PublishItem); }
    }
}
