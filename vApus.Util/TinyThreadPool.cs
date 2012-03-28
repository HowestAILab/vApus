/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, department PIH
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Diagnostics;
using System.Threading;

namespace vApus.Util
{
    /// <summary>
    /// A simple thread pool implementation, when it's work is over it is disposed, just like a thread.
    /// </summary>
    public class TinyThreadPool : IDisposable
    {
        #region Fields
        private Thread[] _threads;
        private int _waitingWorkItemsCount, _busyWorkItemsCount;
        private ManualResetEvent _poolInitializedWaitHandle = new ManualResetEvent(false),
                                 _executeWorkItemsWaitHandle = new ManualResetEvent(false),
                                 _poolIdleWaitHandle = new ManualResetEvent(false);
        private bool _isDisposed;

        /// <summary>To be able to run to the same code with different threads while this code is unique for every thread so the callback can be threadsafe invoked without locking.</summary>
        [ThreadStatic]
        private static WorkItem _workItem;
        /// <summary>What needs to be executed.</summary>
        private ParameterizedCallback _workItemCallback;
        #endregion

        #region Properties
        public int PoolSize
        {
            get { return (_isDisposed) ? 0 : _threads.Length; }
        }
        public int BusyWorkItemsCount
        {
            get { return _busyWorkItemsCount; }
        }
        public bool IsIdle
        {
            get { return _busyWorkItemsCount == 0; }
        }
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
        #endregion

        #region Con/Destructor
        /// <summary>
        /// A simple thread pool implementation, when it's work is over it is disposed, just like a thread.
        /// Initializes in a blocking manner.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="workItemCallback"></param>
        /// <param name="args">Must not equal size, it will set the index automatically at the start of the array when needed.</param>
        public TinyThreadPool(int size, ParameterizedCallback workItemCallback, params object[] args)
            : this(size, workItemCallback, ThreadPriority.Normal, args)
        { }
        /// <summary>
        /// A simple thread pool implementation, when it's work is over it is disposed, just like a thread.
        /// Initializes in a blocking manner.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="workItemCallback"></param>
        /// <param name="threadPriority"></param>
        /// <param name="args">
        /// One arg af this array will be given with the callback delegate sequentally.
        /// Must not equal size, it will set the index automatically at the start of the array when needed.
        /// </param>
        public TinyThreadPool(int size, ParameterizedCallback workItemCallback, ThreadPriority threadPriority, params object[] args)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("size");
            if (workItemCallback == null)
                throw new ArgumentNullException("workItemCallback");

            Debug.WriteLine(string.Format("Initializing with {0} threads...", size), ToString());
            _workItemCallback = workItemCallback;
            _threads = new Thread[size];

            //Fill the pool.
            for (int i = 0; i < size; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(DoWork));
                t.Name = string.Concat("TinyThreadpool_thread", i);
                t.IsBackground = true;
                t.Priority = threadPriority;
                _threads[i] = t;
            }

            //Start the threads and let them wait.
            if (args == null || args.Length == 0)
                for (int i = 0; i < _threads.Length; i++)
                    _threads[i].Start();
            else
                for (int i = 0; i < _threads.Length; i++)
                {
                    int j = i;
                    while (j >= args.Length)
                        j -= args.Length;
                    _threads[i].Start(args[j]);
                }
            _poolInitializedWaitHandle.WaitOne();
            Debug.WriteLine("Initialized!", ToString());
        }
        ~TinyThreadPool()
        {
            Dispose();
        }
        #endregion

        #region Functions
        //public void ExecuteWorkItemsAndWaitForFinish()
        //{
        //    if (_isDisposed)
        //        throw new Exception("Disposed");

        //    Debug.WriteLine("Executing work items...", ToString());
        //    try
        //    {
        //        _waitingWorkItemsCount = 0;
        //        _poolIdleWaitHandle.Reset();
        //        _busyWorkItemsCount = _threads.Length;

        //        _poolInitializedWaitHandle.WaitOne();

        //        //Let the threads start.
        //        _executeWorkItemsWaitHandle.Set();
        //        _poolIdleWaitHandle.WaitOne();
        //    }
        //    catch { }
        //    Debug.WriteLine("Finished!", ToString());
        //}

        public void ExecuteWorkItemsAndWaitForFinish()
        {
            if (_isDisposed)
                throw new Exception("Disposed");

            Debug.WriteLine("Executing work items...", ToString());
            try
            {
                _busyWorkItemsCount = _threads.Length;
                //Let the threads start.
                _executeWorkItemsWaitHandle.Set();
                _poolIdleWaitHandle.WaitOne();
            }
            catch { }
            Dispose();
            Debug.WriteLine("Finished!", ToString());
        }

        public void Dispose()
        {
            Dispose(0);
        }
        public void Dispose(int timeout)
        {
            _isDisposed = true;
            if (_threads != null)
                lock (_threads.SyncRoot)
                {
                    foreach (Thread t in _threads)
                        try { t.Join(timeout); }
                        catch { }
                    _busyWorkItemsCount = 0;
                    foreach (Thread t in _threads)
                        try { t.Abort(); }
                        catch { }
                }
            try
            {
                if (_poolInitializedWaitHandle != null)
                    _poolInitializedWaitHandle.Close();
            }
            catch { }
            try
            {
                if (_executeWorkItemsWaitHandle != null)
                    _executeWorkItemsWaitHandle.Close();
            }
            catch { }
            try
            {
                if (_poolIdleWaitHandle != null)
                    _poolIdleWaitHandle.Close();
            }
            catch { }
            _poolInitializedWaitHandle = null;
            _executeWorkItemsWaitHandle = null;
            _poolIdleWaitHandle = null;
            _threads = null;
            Debug.WriteLine("Disposed.", ToString());
        }
        /// <summary>
        /// This will invoke the callback and will signal when finished using a ManualResetEvent.
        /// </summary>
        /// <param name="arg"></param>
        //private void DoWork(object arg)
        //{
        //    try
        //    {
        //        //This is threadstatic, so unique for each thread.
        //        _workItem = new WorkItem();
        //    Redo:
        //        try
        //        {
        //            if (!_isDisposed && Interlocked.Increment(ref _waitingWorkItemsCount) == _threads.Length)
        //                _poolInitializedWaitHandle.Set();
        //            if (!_isDisposed)
        //                _executeWorkItemsWaitHandle.WaitOne();
        //            if (!_isDisposed)
        //            {
        //                _workItem.Execute(_workItemCallback, arg);
        //                Debug.WriteLine(string.Format("{0} finished!", Thread.CurrentThread.Name), ToString());
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if (!_isDisposed)
        //            {
        //                Debug.WriteLine(string.Format("Unhandled exception in {0}:", Thread.CurrentThread.Name), ToString());
        //                Debug.WriteLine(ex.ToString(), ToString());
        //            }
        //        }
        //        if (Interlocked.Decrement(ref _busyWorkItemsCount) == 0 && !_isDisposed)
        //            _poolIdleWaitHandle.Set();
        //        if (!_isDisposed)
        //        {
        //            //Reset when the work is done before redoing. That way it will wait untill it gets signaled it may do its job again.
        //            _executeWorkItemsWaitHandle.Reset();
        //            goto Redo;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!_isDisposed)
        //        {
        //            Debug.WriteLine(string.Format("Unhandled exception in {0}:", Thread.CurrentThread.Name), ToString());
        //            Debug.WriteLine(ex.ToString(), ToString());
        //        }
        //    }
        //}

        private void DoWork(object arg)
        {
            try
            {
                //This is threadstatic, so unique for each thread.
                _workItem = new WorkItem();
                if (Interlocked.Increment(ref _waitingWorkItemsCount) == _threads.Length)
                    _poolInitializedWaitHandle.Set();

                if (!_isDisposed)
                    _executeWorkItemsWaitHandle.WaitOne();
                if (!_isDisposed)
                    _workItem.Execute(_workItemCallback, arg);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Unhandled exception in {0}:", Thread.CurrentThread.Name), ToString());
                Debug.WriteLine(ex.ToString(), ToString());
            }
            finally
            {
                if (Interlocked.Decrement(ref _busyWorkItemsCount) == 0 && !_isDisposed)
                    try { _poolIdleWaitHandle.Set(); }
                    catch { }
            }
        }

        #endregion

        private struct WorkItem
        {
            public void Execute(ParameterizedCallback callback, object arg)
            {
                callback(arg);
            }
        }
    }
}