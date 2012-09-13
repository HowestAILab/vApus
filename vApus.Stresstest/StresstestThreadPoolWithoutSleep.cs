/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using vApus.Util;

namespace vApus.Stresstest
{
    public class StresstestThreadPool : IDisposable
    {
        #region Events
        public event EventHandler<MessageEventArgs> Message;
        public event EventHandler<MessageEventArgs> ThreadWorkException;
        #endregion

        #region Fields
        private Thread[] _threads;

        private int _waitingThreadCount, _busyThreadCount;
        private volatile int _usedThreadCount;

        private ManualResetEvent _doWorkWaitHandle = new ManualResetEvent(false), _poolIdleWaitHandle = new ManualResetEvent(false);
        private AutoResetEvent _poolInitializedWaitHandle = new AutoResetEvent(false);

        //0 == false, 1 == true
        private int _isDisposed;
        /// <summary>To be able to run to the same code with different threads while this code is unique for every thread so the callback can be threadsafe invoked without locking.</summary>
        [ThreadStatic]
        private static WorkItem _workItem;
        #endregion

        #region Properties
        public bool IsDisposed
        {
            get { return _isDisposed == 1; }
        }
        /// <summary>
        /// The number of threads that are actually doing something.
        /// </summary>
        public int BusyThreadCount
        {
            get { return _busyThreadCount; }
        }
        public int UsedThreadCount
        {
            get { return _usedThreadCount; }
        }
        public int PoolSize
        {
            get { return (_threads == null) ? 0 : _threads.Length; }
        }
        public int IndexOf(Thread thread)
        {
            if (_threads != null)
                lock (_threads.SyncRoot)
                {
                    for (int i = 0; i != _threads.Length; i++)
                        if (_threads[i] == thread)
                            return i;
                }
            return -1;
        }
        #endregion

        #region Constructor
        public StresstestThreadPool(WorkItemCallback workItemCallback)
        {
            if (workItemCallback == null)
                throw new ArgumentNullException("workCallback");
            WorkItem.Init(this, workItemCallback);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Set the number of threads, start them and let them wait for a continue. (DoWorkAndWaitForFinish)
        /// </summary>
        /// <param name="count"></param>
        public void SetThreads(int count)
        {
            WriteMessage("Initializing Thread Pool...");
            Console.WriteLine(count);

            _usedThreadCount = count;
            HashSet<Thread> threads = new HashSet<Thread>();

            //Create threads and thread pool recycling
            if (_threads != null)
                foreach (Thread thread in _threads)
                    threads.Add(thread);

            count -= threads.Count;

            for (int i = 0; i < count; i++)
            {
                Thread t = new Thread(DoWork);
                t.Name = "vApus Thread Pool Thread #" + (threads.Count + 1);
                t.IsBackground = true;
                threads.Add(t);
            }

            int originalPoolSize = PoolSize;
            _threads = new Thread[threads.Count];
            threads.CopyTo(_threads);

            //Initialize
            //Start the threads who aren't started yet.
            for (int i = originalPoolSize; i < _usedThreadCount; i++)
                _threads[i].Start();

            if (originalPoolSize < _usedThreadCount)
                _poolInitializedWaitHandle.WaitOne();

            Console.WriteLine("Initialized");
            WriteMessage("Initialized!");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patternIndices">The patterns for delays and randomized log entries.</param>
        public void DoWorkAndWaitForIdle()
        {
            if (IsDisposed)
                throw new Exception("Disposed");

            WriteMessage("Executing work items...");
            try
            {
                _busyThreadCount = _usedThreadCount;
                //Let the threads start.
                _doWorkWaitHandle.Set();
                Console.WriteLine("Waiting for Idle");
                _poolIdleWaitHandle.WaitOne();

                Console.WriteLine("IDLE!\n\n\n\n\n");
            }
            catch
            {
                if (!IsDisposed)
                    throw;
            }
            WriteMessage("Finished!");
        }

        private void DoWork()
        {
            try
            {
                //The constructor will find the index of the thread, this is thread safe (this is still initialisation so it does not influence the test).
                if (_workItem == null)
                    _workItem = new WorkItem();

                if (Interlocked.Increment(ref _waitingThreadCount) == _usedThreadCount && !IsDisposed)
                {
                    Console.WriteLine("Waiting thread count == used thread count (thread " + _workItem.ThreadIndex + ")");
                    _poolIdleWaitHandle.Reset();
                    _poolInitializedWaitHandle.Set();
                }

                if (!IsDisposed)
                {
                    Console.WriteLine("Waiting for work (thread " + _workItem.ThreadIndex + ")");
                    _doWorkWaitHandle.WaitOne();
                }

                Interlocked.Decrement(ref _waitingThreadCount);

                Console.WriteLine("Start work (thread " + _workItem.ThreadIndex + ")"); 

                //Check for disposed in this function.
                _workItem.Execute();

                Console.WriteLine("Work executed for thread " + _workItem.ThreadIndex);
            }
            catch (Exception ex)
            {
                //Reports this to the gui.
                WriteThreadWorkException("Unhandled exception in " + Thread.CurrentThread.Name + ':' + Environment.NewLine + ex);
            }
            finally
            {
                //Let only the used threads do this, the rest must wait until idle.
                if (_workItem.ThreadIndex < _usedThreadCount && !IsDisposed && Interlocked.Decrement(ref _busyThreadCount) == 0)
                {
                    Console.WriteLine("Busy Thread Count == 0 (thread "+ _workItem.ThreadIndex + ")");
                    _doWorkWaitHandle.Reset();
                    _poolIdleWaitHandle.Set();
                }
                else if (!IsDisposed)
                {
                    Console.WriteLine("Waiting for idle (thread " + _workItem.ThreadIndex + ")");

                    //Wait for idle to continue
                    _poolIdleWaitHandle.WaitOne();
                }
                if (!IsDisposed)
                    DoWork();
            }
        }

        public void Dispose()
        {
            Dispose(0);
        }
        public void Dispose(int timeout)
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
            {
                _doWorkWaitHandle.Set();
                _poolInitializedWaitHandle.Set();
                if (_threads != null)
                {
                    //Join
                    foreach (Thread t in _threads)
                    {
                        try
                        {
                            if (t != null)
                                t.Join(timeout);
                        }
                        catch { }
                    }
                    _usedThreadCount = 0;

                    //Abort
                    foreach (Thread t in _threads)
                    {
                        try
                        {
                            if (t != null)
                                t.Abort();
                        }
                        catch { }
                    }
                    _poolIdleWaitHandle.Set();
                }
                try
                {
                    if (_poolInitializedWaitHandle != null)
                        _poolInitializedWaitHandle.Close();
                }
                catch { }
                try
                {
                    if (_doWorkWaitHandle != null)
                        _doWorkWaitHandle.Close();
                }
                catch { }
                try
                {
                    if (_poolIdleWaitHandle != null)
                        _poolIdleWaitHandle.Close();
                }
                catch { }
                _poolInitializedWaitHandle = null;
                _doWorkWaitHandle = null;
                _poolIdleWaitHandle = null;
                _threads = null;
                WriteMessage("Disposed.");
            }
        }
        private void WriteMessage(string message, LogLevel logLevel = LogLevel.Info)
        {
            Debug.WriteLine(message, ToString());
            if (Message != null)
                Message(this, new MessageEventArgs(message, logLevel));
        }
        private void WriteThreadWorkException(string message)
        {
            Debug.WriteLine(message, ToString());
            if (ThreadWorkException != null)
                ThreadWorkException(this, new MessageEventArgs(message, LogLevel.Error));
        }
        #endregion

        private class WorkItem
        {
            #region Fields
            private static StresstestThreadPool _threadPool;
            private static WorkItemCallback _callback;

            private volatile int _threadIndex;
            #endregion

            #region Properties
            /// <summary>
            /// The index of the current thread in the thread pool. (volatile)
            /// </summary>
            public int ThreadIndex
            {
                get { return _threadIndex; }
            }
            #endregion

            #region Constructor
            public WorkItem()
            {
                if (!_threadPool.IsDisposed)
                    _threadIndex = _threadPool.IndexOf(Thread.CurrentThread);
            }
            #endregion

            #region Functions
            public static void Init(StresstestThreadPool threadPool, WorkItemCallback callback)
            {
                _threadPool = threadPool;
                _callback = callback;
            }
            public void Execute()
            {
                // Make sure that only a used thread can be executed.
                if (!_threadPool.IsDisposed && ThreadIndex < _threadPool.UsedThreadCount)
                    _callback(_threadIndex);
            }
            #endregion
        }
    }
}