using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using vApus.Publish;

namespace vApus.ResultsHandler {
    public static class QueuedListener {
        private static TcpListener _listener;
        private static bool _isListening;
        private static MessageQueue _queue;

        static QueuedListener() { }

        public static void Start(int port = 4337) {
            _isListening = true;
            _queue = new MessageQueue();
            _queue.OnDequeue += _queue_OnDequeue;

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start(int.MaxValue);

            ThreadPool.QueueUserWorkItem((state) => {
                while (_isListening)
                    try {
                        var client = _listener.AcceptTcpClient();
                        HandleRead(new StreamReader(client.GetStream(), Encoding.UTF8));
                    } catch {

                    }
            });
        }

        public static void Stop() {
            _isListening = false;
        }

        private static void HandleRead(StreamReader sr) {
            ThreadPool.QueueUserWorkItem((state) => {
            try {
                while (_isListening) {
                    string msg = sr.ReadLine();
                    dynamic intermediate = JObject.Parse(msg);
                    //A validating step.
                    var item = intermediate.ToObject(Assembly.GetExecutingAssembly().GetType("vApus.Publish." + intermediate.PublishItemType));
                    _queue.Enqueue(item);
                    }
                } catch {

                }
            });
        }

        private static void _queue_OnDequeue(object sender, MessageQueue.OnDequeueEventArgs e) { PublishItemHandler.Handle(e.Messages as PublishItem[]); }
    }
}
