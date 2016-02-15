/*
 * Copyright 2016 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using vApus.Publish;

namespace vApus.PublishItemsHandler {
    /// <summary>
    /// Dequeues every 5 seconds. FIFO.
    /// </summary>
    public class SimpleMessageQueue {
        public event EventHandler<OnDequeueEventArgs> OnDequeue;

        private ConcurrentQueue<object> _queue = new ConcurrentQueue<object>();
        private Timer _tmr;

        /// <summary>
        /// Dequeues every 5 seconds. FIFO.
        /// </summary>
        /// <param name="dequeueTimeInMs"></param>
        public SimpleMessageQueue(int dequeueTimeInMs = 5000) {
            _tmr = new Timer(dequeueTimeInMs);
            _tmr.Elapsed += _tmr_Elapsed;
            _tmr.Start();
        }

        private void _tmr_Elapsed(object sender, ElapsedEventArgs e) {
            if (OnDequeue != null) {
                var messages = new object[_queue.LongCount()];
                if (messages.LongLength != 0) {
                    for (long l = 0L; l != messages.LongLength; l++) {
                        object message;
                        if (_queue.TryDequeue(out message)) messages[l] = message;
                    }
                    if (OnDequeue != null)
                        OnDequeue.Invoke(null, new OnDequeueEventArgs(messages));
                }
            }
        }

        public void Enqueue(PublishItem item) {
            _queue.Enqueue(item);
        }

        public class OnDequeueEventArgs : EventArgs {
            public object[] Messages { get; private set; }
            public OnDequeueEventArgs(object[] messages) {
                Messages = messages;
            }
        }
    }
}
