﻿/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using RandomUtils.Log;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace vApus.StressTest {
    /// <summary>
    /// This is a single purpose threadpool, it is specifically designed to be able to execute one type of workload: A thread must communicate to a server app through the means of a connection proxy; time to last byte and delay must be added to (vApus,Results.)StressTestResult.
    /// The CLR threadpool did not suffice due to the lack of control.
    /// </summary>
    internal class StressTestThreadPool : IDisposable {

        #region Events
        /// <summary>
        /// Notifie if an error occured that was not caught in the workload of a thread. This thread will stop working immediately; carefully designing the 'workload function' is advised (WorkItemCallback).
        /// </summary>
        public event EventHandler<MessageEventArgs> ThreadWorkException;
        #endregion

        /// <summary>
        /// The delegate to the function each thread will execute.
        /// </summary>
        /// <param name="threadIndex">The index of the thread in the threadpool will be given with.</param>
        public delegate void WorkItemCallback(int threadIndex);

        #region Fields
        /// <summary>To be able to run to the same code with different threads while this code is unique for every thread so the callback can be threadsafe invoked without locking.</summary>
        [ThreadStatic]
        private static WorkItem _workItem;

        private readonly WorkItemCallback _workItemCallback;
        private int _busyThreadCount;
        private int _waitingThreadCount;

        private ManualResetEvent _doWorkWaitHandle = new ManualResetEvent(false);

        //0 == false, 1 == true
        private int _isDisposed;
        private AutoResetEvent _poolIdleWaitHandle = new AutoResetEvent(false), _poolInitializedWaitHandle = new AutoResetEvent(false);
        private Thread[] _threads;
        private ConcurrentQueue<Thread> _parallelThreads;
        private ConcurrentBag<Thread> _toDisposeParallelThreads;

        private volatile int _count, _parallelThreadCount;
        #endregion

        #region Properties
        public bool IsDisposed { get { return _isDisposed == 1; } }

        /// <summary>
        ///     The number of threads that are actually doing something. This will become 0 if the work is finished.
        /// </summary>
        public int BusyThreadCount { get { return _busyThreadCount + _parallelThreadCount; } }

        /// <summary>
        /// The number of threads in the pool.
        /// </summary>
        public int PoolSize {
            get { return _threads == null ? 0 : _count + _parallelThreadCount; }
        }

        #endregion

        #region Con-/Destructor
        /// <summary>
        /// This is a single purpose threadpool, it is specifically designed to be able to execute one type of workload: A thread must communicate to a server app through the means of a connection proxy; time to last byte and delay must be added to (vApus,Results.)StressTestResult.
        /// The CLR threadpool did not suffice due to the lack of control.
        /// </summary>
        /// <param name="workItemCallback">The delegate to the function each thread will execute. The index of the thread in the threadpool will be given with.</param>
        public StressTestThreadPool(WorkItemCallback workItemCallback) {
            if (workItemCallback == null)
                throw new ArgumentNullException("workItemCallback");
            _workItemCallback = workItemCallback;
        }
        ~StressTestThreadPool() { Dispose(); }
        #endregion

        #region Functions
        /// <summary>
        ///     Set the number of threads (size), start them and let them wait for a continue. (DoWorkAndWaitForFinish)
        ///     Do this only when the pool is not doing any work.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="parallelThreads">To enable testing parallel requests, for instance fo web sites. Must be dequeued and started ad hoc.</param>
        public void SetThreads(int count, int parallelThreadCount = 0) {
            Debug.WriteLine("Initializing Thread Pool...", ToString());

            //Already contained threads are ready to be garbage collected.
            _threads = new Thread[count];
            _parallelThreads = new ConcurrentQueue<Thread>();
            _toDisposeParallelThreads = new ConcurrentBag<Thread>();

            _count = count;
            _parallelThreadCount = parallelThreadCount;

            for (int i = 0; i < count; i++) {
                var t = new Thread(DoWork);
                t.Name = "vApus Thread Pool Thread #" + (i + 1);
                t.IsBackground = true;
                _threads[i] = t;
                t.Start(i);
            }

            for (int i = 0; i < parallelThreadCount; i++) {
                var t = new Thread(DoParallelWork);
                t.Name = "vApus Thread Pool Thread #" + (count + i + 1);
                t.IsBackground = true;
                _parallelThreads.Enqueue(t);
            }

            _poolInitializedWaitHandle.WaitOne();

            Debug.WriteLine("Initialized!", ToString());
        }

        /// <summary>
        ///  After SetThreads is called, execute this to do the actual stress test. When this returns SetThreads must be called again to fill the threadpool with threads that are alive.
        /// </summary>
        /// <param name="patternIndices">The patterns for delays and randomized requests.</param>
        public void DoWorkAndWaitForIdle() {
            if (IsDisposed)
                throw new Exception("Disposed");

            Debug.WriteLine("Executing work items...", ToString());
            try {
                _busyThreadCount = _count;
                //Let the threads start.
                _doWorkWaitHandle.Set();
                _poolIdleWaitHandle.WaitOne();

                //Clean out the pool to be sure no thread is hanging. (Maybe a bit to much, but it doesn't hurt)
                Interlocked.Exchange(ref _busyThreadCount, 0);
                _doWorkWaitHandle.Reset();
            } catch {
                if (!IsDisposed)
                    throw;
            }
            Debug.WriteLine("Finished!", ToString());
        }

        /// <summary>
        /// Each thread calls this method, it contains some boilerplate code to make sure that the WorkItemCallback is executed properly without threads interfering eachother.
        /// </summary>
        private void DoWork(object threadIndex) {
            try {
                if (_workItem == null) _workItem = new WorkItem(this, (int)threadIndex, _workItemCallback);

                if (Interlocked.Increment(ref _waitingThreadCount) == _count) _poolInitializedWaitHandle.Set();
                //Do not execute if disposed.
                if (!IsDisposed) _doWorkWaitHandle.WaitOne();
                Interlocked.Decrement(ref _waitingThreadCount);

                //Check for disposed in this function.
                _workItem.Execute();
            } catch (Exception ex) {
                //Reports this to the gui.
                if (!IsDisposed) WriteThreadWorkException("Exception for " + Thread.CurrentThread.Name + ":\n" + ex);
            } finally {
                //Let only the used threads do this, the rest may sleep. When all threads are finished they will be purged.
                if (_workItem.ThreadIndex < _count && Interlocked.Decrement(ref _busyThreadCount) <= 0 && !IsDisposed) {
                    try {
                        Interlocked.Exchange(ref _busyThreadCount, 0);
                        _doWorkWaitHandle.Reset();
                        _poolIdleWaitHandle.Set();
                    } catch {
                        //Ignore. Happens if stuff becomes null.
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallelCallback">object[] { callback, request index }</param>
        private void DoParallelWork(object parallelCallback) {
            var arr = parallelCallback as object[];
            var fx = arr[0] as WorkItemCallback;
            if (IsDisposed) return;
            fx((int)arr[1]);
        }

        /// <summary>
        /// Async firing the event to the different subscribers.
        /// </summary>
        /// <param name="message"></param>
        private void WriteThreadWorkException(string message) {
            if (ThreadWorkException != null) {
                var invocationList = ThreadWorkException.GetInvocationList();
                Parallel.For(0, invocationList.Length, (i) => {
                    (invocationList[i] as EventHandler<MessageEventArgs>).Invoke(this, new MessageEventArgs(message, Color.Empty, Level.Error));
                });
            }
        }

        /// <summary>
        /// Dequeue the threads you want. Start with a ParallelWorkItemCallback() as an argument.
        /// </summary>
        /// <returns></returns>
        public Thread DequeueParallelThread() {
            Thread t;
            _parallelThreads.TryDequeue(out t);
            _toDisposeParallelThreads.Add(t);
            return t;
        }

        public void Dispose() {
            if (!IsDisposed) {
                Interlocked.Exchange(ref _isDisposed, 1);

                _doWorkWaitHandle.Set();
                _poolInitializedWaitHandle.Set();
                if (_threads != null) {
                    _count = 0;

                    //Abort
                    Parallel.ForEach(_threads, delegate (Thread t) {
                        try { if (t != null) t.Abort(); } catch {
                            //Ignore.
                        }
                    });

                    //Join
                    Parallel.ForEach(_threads, delegate (Thread t) {
                        try { if (t != null && (t.ThreadState & System.Threading.ThreadState.Unstarted) != System.Threading.ThreadState.Unstarted) t.Join(1000); } catch {
                            //Ignore.
                        }
                    });

                    //Abort
                    Parallel.ForEach(_parallelThreads, delegate (Thread t) {
                        try { if (t != null) t.Abort(); } catch {
                            //Ignore.
                        }
                    });

                    //Join
                    Parallel.ForEach(_parallelThreads, delegate (Thread t) {
                        try { if (t != null && (t.ThreadState & System.Threading.ThreadState.Unstarted) != System.Threading.ThreadState.Unstarted) t.Join(1000); } catch {
                            //Ignore.
                        }
                    });

                    //Abort
                    Parallel.ForEach(_toDisposeParallelThreads, delegate (Thread t) {
                        try { if (t != null) t.Abort(); } catch {
                            //Ignore.
                        }
                    });

                    //Join
                    Parallel.ForEach(_toDisposeParallelThreads, delegate (Thread t) {
                        try { if (t != null && (t.ThreadState & System.Threading.ThreadState.Unstarted) != System.Threading.ThreadState.Unstarted) t.Join(1000); } catch {
                            //Ignore.
                        }
                    });

                    _poolIdleWaitHandle.Set();
                }
                try { if (_poolInitializedWaitHandle != null) _poolInitializedWaitHandle.Close(); } catch {
                    //Ignore.
                }
                try { if (_doWorkWaitHandle != null) _doWorkWaitHandle.Close(); } catch {
                    //Ignore.
                }
                try { if (_poolIdleWaitHandle != null) _poolIdleWaitHandle.Close(); } catch {
                    //Ignore.
                }

                _poolInitializedWaitHandle = null;
                _doWorkWaitHandle = null;
                _poolIdleWaitHandle = null;
                _threads = null;
                _busyThreadCount = 0;

                Debug.WriteLine("Disposed.", ToString());
            }
        }
        #endregion

        private class WorkItem {

            #region Fields
            private readonly WorkItemCallback _callback;
            private readonly StressTestThreadPool _threadPool;

            private volatile int _threadIndex;
            #endregion

            #region Properties
            /// <summary>
            ///     The index of the current thread in the thread pool. (volatile)
            /// </summary>
            public int ThreadIndex { get { return _threadIndex; } }
            #endregion

            #region Constructor
            public WorkItem(StressTestThreadPool threadPool, int threadIndex, WorkItemCallback callback) {
                _threadPool = threadPool;
                _callback = callback;

                _threadIndex = threadIndex;
            }
            #endregion

            #region Functions
            public void Execute() {
                // Make sure that only a used thread can execute this. (Extra check for rogue threads)
                if (!_threadPool.IsDisposed && _threadIndex < _threadPool._count) _callback(_threadIndex);
            }
            #endregion
        }
    }
}