using RandomUtils;
using RandomUtils.Log;
/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    /// To test given connections in parallel. Subscribe to the Message event to log progress. 
    /// </summary>
    public class TestConnections {
        public event EventHandler<TestWorkItem.MessageEventArgs> Message;

        #region Fields
        [ThreadStatic]
        private TestWorkItem _testWorkItem;
        private AutoResetEvent _testAutoResetEvent = new AutoResetEvent(false);
        #endregion

        #region Functions
        public void Test(Connection connection1, params Connection[] connections) {
            var l = new List<Connection>(connections.Length + 1);
            l.Add(connection1);
            l.AddRange(connections);
            Test(l);
        }
        async public void Test(IEnumerable<Connection> connections) {
            //Do this in another message loop.
            await Task.Run(() => {
                try {
                    int totalNumberOfConnections = 0, testedConnections = 0;
                    foreach (Connection connection in connections) ++totalNumberOfConnections;

                    foreach (Connection connection in connections) {
                        //Use the state object, otherwise there will be a reference mismatch.
                        var t = new Thread(delegate(object state) {
                            try {
                                _testWorkItem = new TestWorkItem();
                                _testWorkItem.Message += _testWorkItem_Message;
                                _testWorkItem.TestConnection(state as Connection);
                                if (Interlocked.Increment(ref testedConnections) == totalNumberOfConnections)
                                    _testAutoResetEvent.Set();
                            } catch (Exception ex) {
                                Loggers.Log(Level.Warning, "Failed testing connections.", ex, new object[] { state });
                            }
                        });
                        t.IsBackground = true;
                        t.Start(connection);
                    }
                    _testAutoResetEvent.WaitOne();
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed testing connections.", ex, new object[] { connections });
                }
            });
        }

        private void _testWorkItem_Message(object sender, TestWorkItem.MessageEventArgs e) {
            if (Message != null) Message(sender, e);
        }
        #endregion

        public class TestWorkItem {
            public event EventHandler<MessageEventArgs> Message;

            public class MessageEventArgs : EventArgs {
                public readonly Connection Connection;
                public readonly string Message;
                public readonly bool Succes;

                public MessageEventArgs(Connection connection, bool succes, string message) {
                    Connection = connection;
                    Succes = succes;
                    Message = message;
                }
            }

            /// <summary>
            /// Compiles the connection proxy and calls the TestConnection function.
            /// </summary>
            /// <param name="connection"></param>
            public void TestConnection(Connection connection) {

                if (connection.ConnectionProxy.IsEmpty) {
                    if (Message != null)
                        Message(this, new MessageEventArgs(connection, false, "This connection has no connection proxy assigned to!"));
                    return;
                }

                var connectionProxyPool = new ConnectionProxyPool(connection);
                CompilerResults compilerResults = connectionProxyPool.CompileConnectionProxyClass(true, false);

                if (compilerResults.Errors.HasErrors) {
                    var sb = new StringBuilder("Failed at compiling the connection proxy class: ");
                    sb.AppendLine();
                    foreach (CompilerError error in compilerResults.Errors) {
                        sb.AppendFormat("Error number {0}, Line {1}, Column {2}: {3}", error.ErrorNumber, error.Line, error.Column, error.ErrorText);
                        sb.AppendLine();
                    }

                    if (Message != null)
                        Message(this, new MessageEventArgs(connection, false, sb.ToString()));
                } else {
                    string error;
                    connectionProxyPool.TestConnection(out error);
                    SynchronizationContextWrapper.SynchronizationContext.Post(delegate {
                        if (error == null) {
                            if (Message != null)
                                Message(this, new MessageEventArgs(connection, true, "OK"));
                        } else {
                            if (Message != null)
                                Message(this, new MessageEventArgs(connection, false, "Failed to connect with the given credentials: " + error));
                        }
                    }, null);
                }
                connectionProxyPool.Dispose();
                connectionProxyPool = null;
            }
        }
    }
}
