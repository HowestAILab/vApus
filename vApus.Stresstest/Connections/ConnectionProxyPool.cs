/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using RandomUtils;
using RandomUtils.Log;
using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    ///     Used for stress testing, the size of the pool must equal or be greater than the concurrent users count of Stress test.
    ///     Note, CompileConnectionProxyClass must be called to be able to use this pool.
    /// </summary>
    public class ConnectionProxyPool : IDisposable {

        #region Fields
        private CompilerUnit _compilerUnit = new CompilerUnit();
        private Assembly _connectionProxyAssembly;
        private Type _connectionProxyType;

        private Connection _connection;

        private IConnectionProxy[] _connectionProxies;
        private ConcurrentQueue<IConnectionProxy> _parallelConnectionProxies;
        private ConcurrentBag<IConnectionProxy> _toDisposeParallelConnectionProxies;

        private bool _isShutdown, _isDisposed;

        private volatile int _poolSize;
        #endregion

        #region Properties
        public IConnectionProxy this[int index] { get { return _connectionProxies[index]; } }
        /// <summary>
        /// Sometimes needed in connection proxy code, MaxConnectionLimit for HttpWebRequests for instance.
        /// </summary>
        public int PoolSize {
            get { return _connectionProxies == null ? 0 : _poolSize; }
        }

        public bool IsDisposed {
            get { return _isDisposed; }
        }
        #endregion

        #region Con-/Destructor
        /// <summary>
        ///     Used for stress testing, the size of the pool must equal or be greater than the concurrent users count of Stress test.
        ///     Note, CompileConnectionProxyClass must be called to be able to use this pool.
        /// </summary>
        /// <param name="connection"></param>
        public ConnectionProxyPool(Connection connection) {
            if (connection == null)
                throw new ArgumentNullException("connection");
            _connection = connection;
        }
        ~ConnectionProxyPool() { Dispose(); }
        #endregion

        #region Functions

        /// <summary>
        ///     Use this before using everything else.
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public CompilerResults CompileConnectionProxyClass(bool debug) {
            //Otherwise probing privatePath will not work --> monitorsources and ConnectionProxyPrerequisites sub folder.
            Directory.SetCurrentDirectory(Application.StartupPath);

            CompilerResults compilerResults = null;
            _connectionProxyType = null;
            _connectionProxyAssembly = _compilerUnit.Compile(_connection.BuildConnectionProxyClass(), debug, out compilerResults);
            if (!compilerResults.Errors.HasErrors)
                _connectionProxyType = _connectionProxyAssembly.GetType("vApus.StressTest.ConnectionProxy");
            return compilerResults;
        }

        /// <summary>
        ///     Tests if a single connection can be established.
        /// </summary>
        /// <returns>Error or null.</returns>
        public string TestConnection() {
            string error = null;
            IConnectionProxy connectionProxy = null;
            try {
                connectionProxy = FastObjectCreator.CreateInstance<IConnectionProxy>(_connectionProxyType);
                connectionProxy.SetParent(this);
                connectionProxy.TestConnection(out error);
            }
            catch (Exception ex) {
                error = ex.ToString();
            }
            try {
                if (connectionProxy != null)
                    connectionProxy.Dispose();
            }
            catch {
                //Not important. Ignore.
            }
            if (string.IsNullOrWhiteSpace(error)) error = null;
            return error;
        }



        /// <summary>
        ///     Will dispose the current connection proxies and open new ones.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="parallelConnectionCount">The count of how many requests are executed in parallel and need a unique connection proxy (e.g. web sited). Must be dequeued ad hoc.</param>
        public void SetAndConnectConnectionProxies(int count, int parallelConnectionCount = 0) {
            DisposeConnectionProxies();

            _connectionProxies = new IConnectionProxy[count];
            _parallelConnectionProxies = new ConcurrentQueue<IConnectionProxy>();
            _toDisposeParallelConnectionProxies = new ConcurrentBag<IConnectionProxy>();

            Exception exception;
            for (int i = 0; i != count; i++) {
                if (_isDisposed || _isShutdown)
                    return;

                var connectionProxy = FastObjectCreator.CreateInstance<IConnectionProxy>(_connectionProxyType);
                connectionProxy.SetParent(this);
                _connectionProxies[i] = connectionProxy;

                exception = Connect(connectionProxy, i);
                if (exception != null)
                    throw (exception);
            }

            for (int pi = 0; pi != parallelConnectionCount; pi++) {
                if (_isDisposed || _isShutdown)
                    return;

                var connectionProxy = FastObjectCreator.CreateInstance<IConnectionProxy>(_connectionProxyType);
                connectionProxy.SetParent(this);
                _parallelConnectionProxies.Enqueue(connectionProxy);

                exception = Connect(connectionProxy, count + pi);
                if (exception != null)
                    throw (exception);
            }

            _poolSize = count + parallelConnectionCount;
        }

        /// <summary>
        /// </summary>
        /// <param name="connectionProxy"></param>
        /// <param name="index">Only used for messages, can be any value.</param>
        /// <returns></returns>
        private Exception Connect(IConnectionProxy connectionProxy, int index) {
            try {
                connectionProxy.OpenConnection();
                if (!connectionProxy.IsConnectionOpen)
                    return ReconnectOnce(connectionProxy, index, null);
            }
            catch (Exception ex) {
                return ReconnectOnce(connectionProxy, index, ex);
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="connectionProxy"></param>
        /// <param name="index">Only used for messages, can be any value.</param>
        /// <returns></returns>
        private Exception ReconnectOnce(IConnectionProxy connectionProxy, int index, Exception ex) {
            Loggers.Log(Level.Warning, "Connection for connection proxy #" + index +
                    " could not be opened, trying to make a new one. (Expensive operation!)", ex, new object[] { connectionProxy, index });
            try {
                connectionProxy = FastObjectCreator.CreateInstance<IConnectionProxy>(_connectionProxyType);
                connectionProxy.SetParent(this);
                connectionProxy.OpenConnection();
                if (!connectionProxy.IsConnectionOpen) {
                    ex = new Exception("Reconnecting failed for connection proxy " + index + "!");
                    Loggers.Log(Level.Error, "Reconnecting failed for connection proxy " + index + "!", null, new object[] { connectionProxy, index });
                    return ex;
                }
            }
            catch (Exception e) {
                Loggers.Log(Level.Error, "Reconnecting failed for connection proxy " + index + "!", e, new object[] { connectionProxy, index });
                return ex;
            }
            return null;
        }

        public IConnectionProxy DequeueParallelConnectionProxy() {
            IConnectionProxy cp;
            _parallelConnectionProxies.TryDequeue(out cp);
            _toDisposeParallelConnectionProxies.Add(cp);
            return cp;
        }

        /// <summary>
        ///     Dispose all connection proxies and clears the pool.
        /// </summary>
        public void ShutDown() {
            if (!_isShutdown) {
                _isShutdown = true;
                DisposeConnectionProxies();
            }
        }

        /// <summary>
        ///     Multi threaded solution
        /// </summary>
        private void DisposeConnectionProxies() {
            //Dispose multi threaded.
            if (_connectionProxies != null)
                Parallel.ForEach(_connectionProxies, (cp) => {
                    try {
                        if (cp != null && !cp.IsDisposed)
                            cp.Dispose();
                    }
                    catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed disposing connection proxy.", ex, new object[] { _connectionProxies, cp });
                    }
                });

            //Dispose multi threaded.
            if (_parallelConnectionProxies != null)
                Parallel.ForEach(_parallelConnectionProxies, (cp) => {
                    try {
                        if (cp != null && !cp.IsDisposed)
                            cp.Dispose();
                    }
                    catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed disposing parallel connection proxy.", ex, new object[] { _parallelConnectionProxies, cp });
                    }
                });

            //Dispose multi threaded.
            if (_toDisposeParallelConnectionProxies != null)
                Parallel.ForEach(_toDisposeParallelConnectionProxies, (cp) => {
                    try {
                        if (cp != null && !cp.IsDisposed)
                            cp.Dispose();
                    }
                    catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed disposing parallel connection proxy.", ex, new object[] { _toDisposeParallelConnectionProxies, cp });
                    }
                });

            _connectionProxies = null;
            _parallelConnectionProxies = null;
            _toDisposeParallelConnectionProxies = null;
        }

        public void Dispose() {
            if (!_isDisposed) {
                _isDisposed = true;
                ShutDown();
                _compilerUnit.DeleteTempFiles();
                _compilerUnit = null;

                _connection = null;
                _connectionProxyAssembly = null;
                _connectionProxyType = null;
            }
        }
        #endregion
    }
}