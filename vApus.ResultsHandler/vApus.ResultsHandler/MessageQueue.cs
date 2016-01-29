using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using vApus.Publish;

namespace vApus.ResultsHandler {
    /// <summary>
    /// Dequeues every 5 seconds. FIFO.
    /// </summary>
    public class MessageQueue {
        public event EventHandler<OnDequeueEventArgs> OnDequeue;

        private readonly object _lock = new object();

        private ConcurrentQueue<object> _queue = new ConcurrentQueue<object>();
        private Timer _tmr;

        /// <summary>
        /// Dequeues every 5 seconds. FIFO.
        /// </summary>
        /// <param name="dequeueTimeInMs"></param>
        public MessageQueue(int dequeueTimeInMs = 5000) {
            _tmr = new Timer(dequeueTimeInMs);
            _tmr.Elapsed += _tmr_Elapsed;
        }

        private void _tmr_Elapsed(object sender, ElapsedEventArgs e) {
            if (OnDequeue != null)
                lock (_lock) {
                    long count = _queue.LongCount();
                    for (long l = 0L; l != count; l++) {
                        object message;
                        if (_queue.TryDequeue(out message)) FireDequeue(message);
                    }
                }
        }

        private void FireDequeue(object message) {
            if (OnDequeue != null)
                OnDequeue.BeginInvoke(null, new OnDequeueEventArgs(message), null, null);
        }

        public void Enqueue(PublishItem item) {
            lock (_lock) _queue.Enqueue(item);
        }

        public class OnDequeueEventArgs : EventArgs {
            public object Message { get; private set; }
            public OnDequeueEventArgs(object message) {
                Message = message;
            }
        }
    }
}
