//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Threading.Tasks.Dataflow;

//namespace vApus.Stresstest {
//    public class ActorPool : IDisposable {

//        #region Events
//        /// <summary>
//        /// Notifie if an error occured that was not caught in the workload of an actor. This actor will stop working immediately; carefully designing the 'workload function' is advised (WorkItemCallback).
//        /// </summary>
//        public event EventHandler<MessageEventArgs> ActorWorkException;
//        #endregion

//        /// <summary>
//        /// The delegate to the function each actor will execute.
//        /// </summary>
//        /// <param name="threadIndex">The index of the actor in the actorpool will be given with.</param>
//        public delegate void WorkItemCallback(int actorIndex);

//        #region Fields
//        private readonly WorkItemCallback _workItemCallback;

//        private int _busyActorCount;

//        private ManualResetEvent _doWorkWaitHandle = new ManualResetEvent(false);

//        //0 == false, 1 == true
//        private int _isDisposed;
//        private AutoResetEvent _poolIdleWaitHandle = new AutoResetEvent(false), _poolInitializedWaitHandle = new AutoResetEvent(false);
//        private ActionBlock<WorkItemCallback>[] _actors;
//        private volatile int _size;
//        private int _waitingActorCount;
//        #endregion

//        #region Properties
//        public bool IsDisposed { get { return _isDisposed == 1; } }

//        /// <summary>
//        ///     The number of threads that are actually doing something. This will become 0 if the work is finished.
//        /// </summary>
//        public int BusyActorCount { get { return _busyActorCount; } }

//        /// <summary>
//        /// The number of threads in the pool.
//        /// </summary>
//        public int Size { get { return _size; } }

//        public int IndexOf(ActionBlock<WorkItemCallback> actor) {
//            if (_actors != null)
//                lock (_actors.SyncRoot)
//                    for (int i = 0; i != _actors.Length; i++)
//                        if (_actors[i] == actor)
//                            return i;

//            return -1;
//        }

//        #endregion

//        #region Con-/Destructor
//        /// <summary>
//        /// This is a single purpose actorpool, it is specifically designed to be able to execute one type of workload: An actor must communicate to a server app through the means of a connection proxy; time to last byte and delay must be added to (vApus,Results.)StresstestResult.
//        /// The CLR threadpool did not suffice due to the lack of control.
//        /// </summary>
//        /// <param name="workItemCallback">The delegate to the function each thread will execute. The index of the thread in the threadpool will be given with.</param>
//        public ActorPool(WorkItemCallback workItemCallback) {
//            if (workItemCallback == null)
//                throw new ArgumentNullException("workItemCallback");
//            _workItemCallback = workItemCallback;
//        }
//        ~ActorPool() { Dispose(); }
//        #endregion


//        //#region Functions
//        ///// <summary>
//        /////     Set the number of threads (size), start them and let them wait for a continue. (DoWorkAndWaitForFinish)
//        /////     Do this only when the pool is not doing any work.
//        ///// </summary>
//        ///// <param name="size"></param>
//        //public void SetThreads(int size) {
//        //    Debug.WriteLine("Initializing Actor Pool...", ToString());

//        //    _size = size;
//        //    //Already contained threads are ready to be garbage collected.
//        //    _actors = new ActionBlock<WorkItemCallback>[_size];

//        //    for (int i = 0; i < _size; i++) {
//        //        var a = new ActionBlock<WorkItemCallback>((work) => DoWork());
//        //        //a.Name = "vApus Thread Pool Thread #" + (i + 1);
//        //        _actors[i] = a;
//        //        a.Post(_workItemCallback);
//        //    }
//        //    _poolInitializedWaitHandle.WaitOne();

//        //    Debug.WriteLine("Initialized!", ToString());
//        //}

//        ///// <summary>
//        /////  After SetThreads is called, execute this to do the actual stresstest. When this returns SetThreads must be called again to fill the threadpool with threads that are alive.
//        ///// </summary>
//        ///// <param name="patternIndices">The patterns for delays and randomized log entries.</param>
//        //public void DoWorkAndWaitForIdle() {
//        //    if (IsDisposed)
//        //        throw new Exception("Disposed");

//        //    Debug.WriteLine("Executing work items...", ToString());
//        //    try {
//        //        _busyActorCount = _size;
//        //        //Let the threads start.
//        //        _doWorkWaitHandle.Set();
//        //        _poolIdleWaitHandle.WaitOne();

//        //        //Clean out the pool to be sure no thread is hanging. (Maybe a bit to much, but it doesn't hurt)
//        //        Interlocked.Exchange(ref _busyActorCount, 0);
//        //        _doWorkWaitHandle.Reset();
//        //    } catch {
//        //        if (!IsDisposed)
//        //            throw;
//        //    }
//        //    Debug.WriteLine("Finished!", ToString());
//        //}

//        ///// <summary>
//        ///// Each thread calls this method, it contains some boilerplate code to make sure that the WorkItemCallback is executed properly without threads interfering eachother.
//        ///// </summary>
//        //private void DoWork() {
//        //    try {
//        //        //The constructor will find the index of the thread, this is thread safe (this is still initialization so it does not influence the test).
//        //        if (_workItem == null) _workItem = new WorkItem(this, _workItemCallback);

//        //        if (Interlocked.Increment(ref _waitingThreadCount) == _size) _poolInitializedWaitHandle.Set();
//        //        //Do not execute if disposed.
//        //        if (!IsDisposed) _doWorkWaitHandle.WaitOne();
//        //        Interlocked.Decrement(ref _waitingThreadCount);

//        //        //Check for disposed in this function.
//        //        _workItem.Execute();
//        //    } catch (Exception ex) {
//        //        //Reports this to the gui.
//        //        if (!IsDisposed) WriteThreadWorkException("Exception for " + Thread.CurrentThread.Name + ":" + Environment.NewLine + ex);
//        //    } finally {
//        //        //Let only the used threads do this, the rest may sleep. When all threads are finished they will be purged.
//        //        if (_workItem.ThreadIndex < _size && Interlocked.Decrement(ref _busyThreadCount) <= 0 && !IsDisposed) {
//        //            try {
//        //                Interlocked.Exchange(ref _busyThreadCount, 0);
//        //                _doWorkWaitHandle.Reset();
//        //                _poolIdleWaitHandle.Set();
//        //            } catch {
//        //            }
//        //        }
//        //    }
//        //}

//        ///// <summary>
//        ///// Async firing the event to the different subscribers.
//        ///// </summary>
//        ///// <param name="message"></param>
//        //private void WriteThreadWorkException(string message) {
//        //    if (ActorWorkException != null) {
//        //        var invocationList = ActorWorkException.GetInvocationList();
//        //        Parallel.For(0, invocationList.Length, (i) => {
//        //            (invocationList[i] as EventHandler<MessageEventArgs>).Invoke(this, new MessageEventArgs(message, Color.Empty, Level.Error));
//        //        });
//        //    }
//        //}

//        //public void Dispose() { Dispose(0); }
//        //public void Dispose(int timeout) {
//        //    if (!IsDisposed) {
//        //        Interlocked.Exchange(ref _isDisposed, 1);

//        //        _doWorkWaitHandle.Set();
//        //        _poolInitializedWaitHandle.Set();
//        //        if (_actors != null) {
//        //            _size = 0;

//        //            ////Abort
//        //            //Parallel.ForEach(_threads, delegate(Thread t) { try { if (t != null) t.Abort(); } catch { } });

//        //            ////Join
//        //            //Parallel.ForEach(_threads, delegate(Thread t) { try { if (t != null) t.Join(timeout); } catch { } });

//        //            _poolIdleWaitHandle.Set();
//        //        }
//        //        try { if (_poolInitializedWaitHandle != null) _poolInitializedWaitHandle.Close(); } catch { }
//        //        try { if (_doWorkWaitHandle != null) _doWorkWaitHandle.Close(); } catch { }
//        //        try { if (_poolIdleWaitHandle != null) _poolIdleWaitHandle.Close(); } catch { }

//        //        _poolInitializedWaitHandle = null;
//        //        _doWorkWaitHandle = null;
//        //        _poolIdleWaitHandle = null;
//        //        _actors = null;
//        //        _busyActorCount = 0;

//        //        Debug.WriteLine("Disposed.", ToString());
//        //    }
//        //}
//        //#endregion


//        public void Dispose() {
//            throw new NotImplementedException();
//        }
//    }
//}
