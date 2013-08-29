/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// This is a single purpose threadpool, it is specifically designed to be able to execute one type of workload: A thread must communicate to a server app through the means of a connection proxy; time to last byte and delay must be added to (vApus,Results.)StresstestResult.
    /// The CLR threadpool did not suffice due to the lack of control.
    /// </summary>
    internal class StresstestThreadPool : IDisposable {

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

        private ManualResetEvent _doWorkWaitHandle = new ManualResetEvent(false);

        //0 == false, 1 == true
        private int _isDisposed;
        private AutoResetEvent _poolIdleWaitHandle = new AutoResetEvent(false), _poolInitializedWaitHandle = new AutoResetEvent(false);
        private Thread[] _threads;
        private volatile int _size;
        private int _waitingThreadCount;
        #endregion

        #region Properties
        public bool IsDisposed { get { return _isDisposed == 1; } }

        /// <summary>
        ///     The number of threads that are actually doing something. This will become 0 if the work is finished.
        /// </summary>
        public int BusyThreadCount { get { return _busyThreadCount; } }

        /// <summary>
        /// The number of threads in the pool.
        /// </summary>
        public int Size { get { return _size; } }

        public int IndexOf(Thread thread) {
            if (_threads != null)
                lock (_threads.SyncRoot)
                    for (int i = 0; i != _threads.Length; i++)
                        if (_threads[i] == thread)
                            return i;

            return -1;
        }

        #endregion

        #region Con-/Destructor
        /// <summary>
        /// This is a single purpose threadpool, it is specifically designed to be able to execute one type of workload: A thread must communicate to a server app through the means of a connection proxy; time to last byte and delay must be added to (vApus,Results.)StresstestResult.
        /// The CLR threadpool did not suffice due to the lack of control.
        /// </summary>
        /// <param name="workItemCallback">The delegate to the function each thread will execute. The index of the thread in the threadpool will be given with.</param>
        public StresstestThreadPool(WorkItemCallback workItemCallback) {
            if (workItemCallback == null)
                throw new ArgumentNullException("workCallback");
            _workItemCallback = workItemCallback;
        }
        ~StresstestThreadPool() { Dispose(); }
        #endregion

        #region Functions
        /// <summary>
        ///     Set the number of threads (size), start them and let them wait for a continue. (DoWorkAndWaitForFinish)
        ///     Do this only when the pool is not doing any work.
        /// </summary>
        /// <param name="size"></param>
        public void SetThreads(int size) {
            Debug.WriteLine("Initializing Thread Pool...", ToString());

            _size = size;
            //Already contained threads are ready to be garbage collected.
            _threads = new Thread[_size];

            for (int i = 0; i < _size; i++) {
                var t = new Thread(DoWork);
                t.Name = "vApus Thread Pool Thread #" + (i + 1);
                t.IsBackground = true;
                _threads[i] = t;
                t.Start();
            }
            _poolInitializedWaitHandle.WaitOne();

            Debug.WriteLine("Initialized!", ToString());
        }

        /// <summary>
        ///  After SetThreads is called, execute this to do the actual stresstest. When this returns SetThreads must be called again to fill the threadpool with threads that are alive.
        /// </summary>
        /// <param name="patternIndices">The patterns for delays and randomized log entries.</param>
        public void DoWorkAndWaitForIdle() {
            if (IsDisposed)
                throw new Exception("Disposed");

            Debug.WriteLine("Executing work items...", ToString());
            try {
                _busyThreadCount = _size;
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
        private void DoWork() {
            try {
                //The constructor will find the index of the thread, this is thread safe (this is still initialization so it does not influence the test).
                if (_workItem == null) _workItem = new WorkItem(this, _workItemCallback);

                if (Interlocked.Increment(ref _waitingThreadCount) == _size) _poolInitializedWaitHandle.Set();
                //Do not execute if disposed.
                if (!IsDisposed) _doWorkWaitHandle.WaitOne();
                Interlocked.Decrement(ref _waitingThreadCount);

                //Check for disposed in this function.
                _workItem.Execute();
            } catch (Exception ex) {
                //Reports this to the gui.
                if (!IsDisposed) WriteThreadWorkException("Exception for " + Thread.CurrentThread.Name + ":" + Environment.NewLine + ex);
            } finally {
                //Let only the used threads do this, the rest may sleep. When all threads are finished they will be purged.
                if (_workItem.ThreadIndex < _size && Interlocked.Decrement(ref _busyThreadCount) <= 0 && !IsDisposed) {
                    try {
                        Interlocked.Exchange(ref _busyThreadCount, 0);
                        _doWorkWaitHandle.Reset();
                        _poolIdleWaitHandle.Set();
                    } catch {
                    }
                }
            }
        }

        /// <summary>
        /// Async firing the event to the different subscribers.
        /// </summary>
        /// <param name="message"></param>
        private void WriteThreadWorkException(string message) {
            if (ThreadWorkException != null)
                foreach (EventHandler<MessageEventArgs> del in ThreadWorkException.GetInvocationList())
                    del.BeginInvoke(this, new MessageEventArgs(message, Color.Empty, LogLevel.Error), null, null);
        }

        public void Dispose() { Dispose(0); }
        public void Dispose(int timeout) {
            if (!IsDisposed) {
                Interlocked.Exchange(ref _isDisposed, 1);

                _doWorkWaitHandle.Set();
                _poolInitializedWaitHandle.Set();
                if (_threads != null) {
                    //Join
                    Parallel.ForEach(_threads, delegate(Thread t) { try { if (t != null) t.Join(timeout); } catch { } });

                    _size = 0;

                    //Abort
                    Parallel.ForEach(_threads, delegate(Thread t) { try { if (t != null) t.Abort(); } catch { } });

                    _poolIdleWaitHandle.Set();
                }
                try { if (_poolInitializedWaitHandle != null) _poolInitializedWaitHandle.Close(); } catch { }
                try { if (_doWorkWaitHandle != null) _doWorkWaitHandle.Close(); } catch { }
                try { if (_poolIdleWaitHandle != null) _poolIdleWaitHandle.Close(); } catch { }

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
            private readonly StresstestThreadPool _threadPool;

            private volatile int _threadIndex;
            #endregion

            #region Properties
            /// <summary>
            ///     The index of the current thread in the thread pool. (volatile)
            /// </summary>
            public int ThreadIndex { get { return _threadIndex; } }
            #endregion

            #region Constructor
            public WorkItem(StresstestThreadPool threadPool, WorkItemCallback callback) {
                _threadPool = threadPool;
                _callback = callback;

                if (!_threadPool.IsDisposed) _threadIndex = _threadPool.IndexOf(Thread.CurrentThread);
            }
            #endregion

            #region Functions
            public void Execute() {
                // Make sure that only a used thread can execute this. (Extra check for rogue threads)
                if (!_threadPool.IsDisposed && _threadIndex < _threadPool.Size) _callback(_threadIndex);
            }
            #endregion
        }
    }
}