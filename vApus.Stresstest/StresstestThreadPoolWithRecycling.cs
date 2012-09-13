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
using System.Drawing;
using System.Threading;
using vApus.Util;

namespace vApus.Stresstest
{
    public class StresstestThreadPoolWithRecycling : IDisposable
    {
        #region Events
        public event EventHandler<MessageEventArgs> Message;
        public event EventHandler<MessageEventArgs> ThreadWorkException;
        #endregion

        #region Fields
        /// <summary>
        /// Only for setting disposed
        /// </summary>
        private object _lock = new object();

        private Thread[] _threads;

        private int _waitingThreadCount, _busyThreadCount;
        private volatile int _usedThreadCount;

        private WorkItemCallback _workItemCallback;

        private ManualResetEvent _doWorkWaitHandle = new ManualResetEvent(false);
        private AutoResetEvent _poolInitializedWaitHandle = new AutoResetEvent(false), _poolIdleWaitHandle = new AutoResetEvent(false);

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
        public StresstestThreadPoolWithRecycling(WorkItemCallback workItemCallback)
        {
            if (workItemCallback == null)
                throw new ArgumentNullException("workCallback");
            _workItemCallback = workItemCallback;
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
                _poolIdleWaitHandle.WaitOne();

                ForceSetIdle();
            }
            catch
            {
                if (!IsDisposed)
                    throw;
            }
            WriteMessage("Finished!");
        }
        private void ForceSetIdle()
        {
            Interlocked.Exchange(ref  _busyThreadCount, 0);
            _doWorkWaitHandle.Reset();
        }

        private void DoWork()
        {
            try
            {
                //The constructor will find the index of the thread, this is thread safe (this is still initialisation so it does not influence the test).
                if (_workItem == null)
                    _workItem = new WorkItem(this, _workItemCallback);

                if (Interlocked.Increment(ref _waitingThreadCount) == _usedThreadCount)
                    _poolInitializedWaitHandle.Set();

                if (!IsDisposed)
                    _doWorkWaitHandle.WaitOne();

                Interlocked.Decrement(ref _waitingThreadCount);

                //Check for disposed in this function.
                _workItem.Execute();
            }
            catch (Exception ex)
            {
                //Reports this to the gui.
                if (!IsDisposed)
                    WriteThreadWorkException("Exception for " + Thread.CurrentThread.Name + ":" + Environment.NewLine + ex);
            }
            finally
            {
                //Let only the used threads do this, the rest may sleep.
                if (_workItem.ThreadIndex < _usedThreadCount && Interlocked.Decrement(ref _busyThreadCount) <= 0 && !IsDisposed)
                {
                    try
                    {
                        Interlocked.Exchange(ref _busyThreadCount, 0);
                        _doWorkWaitHandle.Reset();
                        _poolIdleWaitHandle.Set();
                    }
                    catch { }
                }
                else
                {
                    while (_busyThreadCount > 0 && !IsDisposed)
                        Thread.Sleep(1);
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
            if (!IsDisposed)
            {
                lock (_lock)
                    _isDisposed = 1;

                _doWorkWaitHandle.Set();
                _poolInitializedWaitHandle.Set();
                if (_threads != null)
                {
                    //Join
                    foreach (Thread t in _threads)
                        try
                        {
                            if (t != null)
                                t.Join(timeout);
                        }
                        catch { }

                    _usedThreadCount = 0;

                    //Abort
                    foreach (Thread t in _threads)
                        try
                        {
                            if (t != null)
                                t.Abort();
                        }
                        catch { }

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
                _busyThreadCount = 0;
                WriteMessage("Disposed.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color">Can be Color.Empty</param>
        /// <param name="logLevel"></param>
        private void WriteMessage(string message, LogLevel logLevel = LogLevel.Info)
        {
            Debug.WriteLine(message, ToString());
            if (Message != null)
                Message(this, new MessageEventArgs(message, Color.Empty, logLevel));
        }
        private void WriteThreadWorkException(string message)
        {
            Debug.WriteLine(message, ToString());
            if (ThreadWorkException != null)
                ThreadWorkException(this, new MessageEventArgs(message, Color.Empty, LogLevel.Error));
        }
        #endregion

        private class WorkItem
        {
            #region Fields
            private StresstestThreadPoolWithRecycling _threadPool;
            private WorkItemCallback _callback;

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
            public WorkItem(StresstestThreadPoolWithRecycling threadPool, WorkItemCallback callback)
            {
                _threadPool = threadPool;
                _callback = callback;

                if (!_threadPool.IsDisposed)
                    _threadIndex = _threadPool.IndexOf(Thread.CurrentThread);
            }
            #endregion

            #region Functions
            public static void Init()
            {
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