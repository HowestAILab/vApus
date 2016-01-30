/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace vApus.Publish {
    internal class MulticastBlock {
        private HashSet<string> _destinationGroupIds = new HashSet<string>();
        protected ConcurrentDictionary<string, HashSet<IDestination>> _internalStore = new ConcurrentDictionary<string, HashSet<IDestination>>();

        private AutoResetEvent _idleWaithandle = new AutoResetEvent(true);
        private int _busyPosting = 0;

        /// <summary>
        /// Check this to detmine if one or more destinations can be added to the group.
        /// </summary>
        /// <param name="destinationGroupId"></param>
        /// <returns></returns>
        public bool Contains(string destinationGroupId) { return _internalStore.ContainsKey(destinationGroupId) && _destinationGroupIds.Contains(destinationGroupId); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationGroupId">To post one message to multiple destinations</param>
        /// <param name="item"></param>
        public void Add(string destinationGroupId, IDestination item) {
            _internalStore.TryAdd(destinationGroupId, new HashSet<IDestination>());
            _destinationGroupIds.Add(destinationGroupId);

            _internalStore[destinationGroupId].Add(item);
        }
  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationGroupId"></param>
        /// <returns></returns>
        public bool Remove(string destinationGroupId) {
            bool removed = true;
            HashSet<IDestination> value;
            if (!_internalStore.TryRemove(destinationGroupId, out value)) removed = false;
            if (!_destinationGroupIds.Remove(destinationGroupId)) removed = false;
            return removed;
        }

        /// <summary>
        /// Async post. Only if the destination group id was found.
        /// </summary>
        /// <param name="destinationGroupId">To post one message to multiple destinations</param>
        /// <param name="message"></param>
        public void Post(string destinationGroupId, object message) {
            if (!_internalStore.ContainsKey(destinationGroupId)) return;

            _idleWaithandle.Reset();
            Interlocked.Add(ref _busyPosting, _internalStore[destinationGroupId].Count);

            Parallel.ForEach(_internalStore[destinationGroupId],
                (dest) => {
                    try {
                        dest.Post(message);
                    } catch (Exception ex) {
                        //Do not use the application logger. This will result in a circular error mess.
                        Debug.WriteLine("PUBLISHER " + dest + " " + ex);
                    }

                    if (Interlocked.Decrement(ref _busyPosting) <= 0) {
                        Interlocked.Exchange(ref _busyPosting, 0);
                        _idleWaithandle.Set();
                    }
                });
        }

        public void Clear() {
            Interlocked.Exchange(ref _busyPosting, 0);
            _idleWaithandle.Set();

            _destinationGroupIds.Clear();
            _internalStore.Clear();
        }

        /// <summary>
        /// Wait until the post is idle. Can be handy for when posting stuff when the application gets closed.
        /// </summary>
        public void WaitUntilIdle() {
            while (_busyPosting != 0)
                _idleWaithandle.WaitOne();
        }
    }
}
