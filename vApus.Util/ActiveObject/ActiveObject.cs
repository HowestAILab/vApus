/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Threading;

namespace vApus.Util
{
    /// <summary>
    ///     To offload work to a background thread keeping that work in a synchronized order.
    /// </summary>
    public class ActiveObject : IDisposable
    {
        /// <summary>
        ///     To offload work to a background thread keeping that work in a synchronized order.
        /// </summary>
        public ActiveObject()
        {
            _sendQueue = new Queue<KeyValuePair<Delegate, object[]>>();
            _sendWaitHandle = new AutoResetEvent(false);
            _sendWorkerThread = new Thread(HandleSendQueue);
            _sendWorkerThread.IsBackground = true;
            _sendWorkerThread.Start();
        }

        public event EventHandler<OnResultEventArgs> OnResult;

        ~ActiveObject()
        {
            Dispose(0);
        }

        #region Functions

        #region IDisposable Members

        /// <summary>
        ///     Wait indefinetly until the work is done before disposing.
        /// </summary>
        public void Dispose()
        {
            Dispose(-1);
        }

        /// <summary>
        ///     Wait the given timeout before disposing, if the work is not done it will be aborted.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        public void Dispose(int millisecondsTimeout)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _sendWaitHandle.Set();

                if (millisecondsTimeout > 0)
                    _sendWorkerThread.Join(millisecondsTimeout);
                else
                    _sendWorkerThread.Join();

                _sendWaitHandle.Close();
                _sendWaitHandle.Dispose();
                _sendWorkerThread.Abort();
                _sendWorkerThread = null;
                _sendQueue = null;
            }
        }

        #endregion

        #region Send

        /// <summary>
        ///     Define your delegate like so:
        ///     delegate T del(T, out T);
        ///     then pass your function using this signature:
        ///     ActiveObject.Send(new del(Func), value, value);
        ///     The return type may be void and args are not obligatory.
        ///     Make sure you provide the right amount of args, even if it are out parameters (yes that is possible too).
        /// </summary>
        /// <param name="del"></param>
        /// <param name="args"></param>
        public void Send(Delegate del, params object[] args)
        {
            try
            {
                _sendQueue.Enqueue(new KeyValuePair<Delegate, object[]>(del, args));
                _sendWaitHandle.Set();
            }
            catch
            {
                //Exception on dispose if any.
            }
        }

        private void HandleSendQueue()
        {
            try
            {
                while (!_isDisposed)
                {
                    while (_sendQueue.Count != 0)
                    {
                        KeyValuePair<Delegate, object[]> kvp = _sendQueue.Dequeue();
                        object returned = null;
                        Exception exception = null;
                        try
                        {
                            returned = kvp.Key.DynamicInvoke(kvp.Value);
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                        finally
                        {
                            if (OnResult != null)
                                foreach (EventHandler<OnResultEventArgs> del in OnResult.GetInvocationList())
                                    del.BeginInvoke(this, new OnResultEventArgs(kvp.Key, kvp.Value, returned, exception),
                                                    OnResultCallback, null);
                        }
                    }
                    _sendWaitHandle.WaitOne();
                }
            }
            catch
            {
                //Exception on dispose if any.
            }
        }

        #endregion

        private void OnResultCallback(IAsyncResult ar)
        {
        }

        #endregion

        #region EventArgs

        public class OnResultEventArgs : EventArgs
        {
            /// <summary>
            ///     Out parameters are stored here too.
            /// </summary>
            public readonly object[] Arguments;

            public readonly Delegate Delegate;

            public readonly Exception Exception;
            public readonly object Returned;

            public OnResultEventArgs(Delegate del, object[] args, object returned, Exception exception)
            {
                Delegate = del;
                Arguments = args;
                Returned = returned;
                Exception = exception;
            }
        }

        #endregion

        #region Fields

        private readonly AutoResetEvent _sendWaitHandle;

        private bool _isDisposed;
        private Queue<KeyValuePair<Delegate, object[]>> _sendQueue;
        private Thread _sendWorkerThread;

        #endregion
    }
}