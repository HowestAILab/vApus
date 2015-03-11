using RandomUtils;
/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    ///     Used for stresstesting, the size of the pool must equal or be greater than the concurrent users count of Stresstest.
    ///     Note, CompileConnectionProxyClass must be called to be able to use this pool.
    /// </summary>
    public class ConnectionProxyPool : IDisposable {

        #region Fields
        private CompilerUnit _compilerUnit = new CompilerUnit();
        private Assembly _connectionProxyAssembly;
        private Type _connectionProxyType;

        private Connection _connection;

        private IConnectionProxy[] _connectionProxies;
        private ParallelConnectionProxy[][] _parallelConnectionProxies;
        private int _usedConnectionProxies;

        private bool _isShutdown, _isDisposed;
        #endregion

        #region Properties
        public IConnectionProxy this[int index] { get { return _connectionProxies[index]; } }
        /// <summary>
        ///     The connection proxies for parallely executed log entries are kept in this array.
        ///     The index of the connection proxy for the log entry that is on the start of the parallel executed range
        ///     is used to get (and set) the right connection proxies
        /// </summary>
        public ParallelConnectionProxy[][] ParallelConnectionProxies {
            get { return _parallelConnectionProxies; }
            set { _parallelConnectionProxies = value; }
        }

        /// <summary>
        /// Sometimes needed in connection proxy code, MaxConnectionLimit for HttpWebRequests for instance.
        /// </summary>
        public int PoolSize { get { return (_connectionProxies == null) ? 0 : _connectionProxies.Length; } }

        public bool IsDisposed {
            get { return _isDisposed; }
        }
        #endregion

        #region Con-/Destructor
        /// <summary>
        ///     Used for stresstesting, the size of the pool must equal or be greater than the concurrent users count of Stresstest.
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

        /// <summary>
        ///     Use this before using everything else.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="deleteTempFiles">To delete the temp files if compiled with debug == true (generates temp files). (Fx can be called afterwards)</param>
        /// <returns></returns>
        public CompilerResults CompileConnectionProxyClass(bool debug, bool deleteTempFiles = true) {
            //Otherwise probing privatePath will not work --> monitorsources and ConnectionProxyPrerequisites sub folder.
            Directory.SetCurrentDirectory(Application.StartupPath);

            CompilerResults compilerResults = null;
            if (deleteTempFiles)
                _compilerUnit.DeleteTempFiles();

            _connectionProxyType = null;
            _connectionProxyAssembly = _compilerUnit.Compile(_connection.BuildConnectionProxyClass(), debug, out compilerResults);
            if (!compilerResults.Errors.HasErrors)
                _connectionProxyType = _connectionProxyAssembly.GetType("vApus.Stresstest.ConnectionProxy");
            return compilerResults;
        }

        /// <summary>
        ///     Tests if a single connection can be establisht.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public void TestConnection(out string error) {
            error = null;
            IConnectionProxy connectionProxy = null;
            try {
                connectionProxy = FastObjectCreator.CreateInstance<IConnectionProxy>(_connectionProxyType);
                connectionProxy.SetParent(this);
                connectionProxy.TestConnection(out error);
            } catch (Exception ex) {
                error = ex.ToString();
            }
            try {
                if (connectionProxy != null)
                    connectionProxy.Dispose();
            } catch {
                //Not important. Ignore.
            }
        }

        /// <summary>
        ///     Will dispose the current connection proxies and open new ones.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="parallelConnectionsCount">The count of how many log entries are executed in parallel and need a unique connection proxy</param>
        public void SetAndConnectConnectionProxies(int count, int parallelConnectionsCount = 0) {
            DisposeConnectionProxies();

            _usedConnectionProxies = count;
            _connectionProxies = new IConnectionProxy[_usedConnectionProxies];
            _parallelConnectionProxies = new ParallelConnectionProxy[_usedConnectionProxies][];

            Exception exception;
            for (int i = 0; i != _usedConnectionProxies; i++) {
                if (_isDisposed || _isShutdown)
                    return;

                var connectionProxy = FastObjectCreator.CreateInstance<IConnectionProxy>(_connectionProxyType);
                connectionProxy.SetParent(this);
                _connectionProxies[i] = connectionProxy;

                exception = Connect(connectionProxy, i);
                if (exception != null)
                    throw (exception);

                if (parallelConnectionsCount != 0) {
                    var pcps = new ParallelConnectionProxy[parallelConnectionsCount];
                    for (int pi = 0; pi != parallelConnectionsCount; pi++) {
                        if (_isDisposed || _isShutdown)
                            return;

                        var cp = FastObjectCreator.CreateInstance<IConnectionProxy>(_connectionProxyType);
                        cp.SetParent(this);

                        var pcp = new ParallelConnectionProxy(cp);
                        pcps[pi] = pcp;

                        exception = Connect(cp, i);
                        if (exception != null)
                            throw (exception);
                    }
                    _parallelConnectionProxies[i] = pcps;
                }
            }
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
            } catch (Exception ex) {
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
            } catch (Exception e) {
                Loggers.Log(Level.Error, "Reconnecting failed for connection proxy " + index + "!", e, new object[] { connectionProxy, index });
                return ex;
            }
            return null;
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
                    } catch (Exception ex) {
                        Loggers.Log(Level.Error, "Failed disposing connection proxy.", ex, new object[] { _connectionProxies, cp });
                    }
                });

            //Dispose multi threaded.
            if (_parallelConnectionProxies != null)
                Parallel.ForEach(_parallelConnectionProxies, (pcps) => {
                    if (pcps != null)
                        foreach (var pcp in pcps)
                            try {
                                if (pcp.ConnectionProxy != null && !pcp.ConnectionProxy.IsDisposed)
                                    pcp.ConnectionProxy.Dispose();
                            } catch (Exception ex) {
                                Loggers.Log(Level.Error, "Failed disposing parallel connection proxy.", ex, new object[] { _parallelConnectionProxies, pcps, pcp });
                            }
                });

            _connectionProxies = null;
            _parallelConnectionProxies = null;
        }
        #endregion
    }
}