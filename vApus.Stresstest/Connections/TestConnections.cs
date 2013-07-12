using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using vApus.Util;

namespace vApus.Stresstest {
    public class TestConnections {
        public event EventHandler<TestWorkItem.MessageEventArgs> Message;

        [ThreadStatic]
        private TestWorkItem _testWorkItem;

        private static object _lock = new object();
        private TestConnectionsDel _testConnectionsDel;

        private AutoResetEvent _testAutoResetEvent = new AutoResetEvent(false);
        private int _testedConnections;
        private int _totalNumberOfConnections;

        private delegate void TestConnectionsDel(List<Connection> connections);

        public void Test(Connection connection1, params Connection[] connections) {
            var l = new List<Connection>(connections.Length + 1);
            l.Add(connection1);
            l.AddRange(connections);
            Test(l);
        }
        public void Test(List<Connection> connections) {
            _testConnectionsDel = __TestConnections;
            //Do this on another message loop.
            StaticActiveObjectWrapper.ActiveObject.Send(_testConnectionsDel, connections);
        }

        private void __TestConnections(List<Connection> connections) {
            try {
                _totalNumberOfConnections = connections.Count;
                _testedConnections = 0;

                foreach (Connection connection in connections) {
                    //Use the state object, otherwise there will be a reference mismatch.
                    var t = new Thread(delegate(object state) {
                        try {
                            _testWorkItem = new TestWorkItem();
                            _testWorkItem.Message += _testWorkItem_Message;
                            _testWorkItem.TestConnection(state as Connection);
                            if (Interlocked.Increment(ref _testedConnections) == _totalNumberOfConnections)
                                _testAutoResetEvent.Set();
                        } catch {
                        }
                    });
                    t.IsBackground = true;
                    t.Start(connection);
                }
                _testAutoResetEvent.WaitOne();
            } catch {
            }
        }

        private void _testWorkItem_Message(object sender, TestWorkItem.MessageEventArgs e) {
            if (Message != null) Message(sender, e);
        }

        public class TestWorkItem {
            public event EventHandler<MessageEventArgs> Message;

            public void TestConnection(Connection connection) {

                if (connection.ConnectionProxy.IsEmpty) {
                    if (Message != null)
                        Message(this,
                                new MessageEventArgs(connection, false, "This connection has no connection proxy assigned to!"));
                    return;
                }

                var connectionProxyPool = new ConnectionProxyPool(connection);
                CompilerResults compilerResults = connectionProxyPool.CompileConnectionProxyClass(false);

                if (compilerResults.Errors.HasErrors) {
                    var sb = new StringBuilder("Failed at compiling the connection proxy class: ");
                    sb.AppendLine();
                    foreach (CompilerError error in compilerResults.Errors) {
                        sb.AppendFormat("Error number {0}, Line {1}, Column {2}: {3}", error.ErrorNumber, error.Line,
                                        error.Column, error.ErrorText);
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
                                Message(this,
                                        new MessageEventArgs(connection, false, "Failed to connect with the given credentials: " + error));
                        }
                    }, null);
                }
                connectionProxyPool.Dispose();
                connectionProxyPool = null;
            }

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
        }

    }
}
