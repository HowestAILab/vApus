/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class TestAllConnections : BaseSolutionComponentView
    {
        /// <summary>
        ///     Test multithreaded.
        /// </summary>
        [ThreadStatic] private static TestWorkItem _testWorkItem;

        private static object _lock = new object();
        private readonly Connections _connections;
        private readonly TestAllConnectionsDel _testAllConnectionsDel;

        private AutoResetEvent _testAutoResetEvent = new AutoResetEvent(false);
        private int _testedConnections;
        private int _totalNumberOfConnections;

        public TestAllConnections()
        {
            InitializeComponent();
        }

        public TestAllConnections(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();

            _connections = solutionComponent as Connections;

            _testAllConnectionsDel = TestConnections;

            if (IsHandleCreated)
            {
                SetGui();
                Test();
            }
            else
            {
                HandleCreated += TestAllConnections_HandleCreated;
            }
        }

        private void TestAllConnections_HandleCreated(object sender, EventArgs e)
        {
            HandleCreated -= TestAllConnections_HandleCreated;
            SetGui();
            Test();
        }

        private void SetGui()
        {
            clmSuccess.Width = 400;
            foreach (BaseItem item in _connections)
                if (item is Connection)
                {
                    var lvwi = new ListViewItem(item.ToString());
                    lvwi.SubItems.Add("");
                    lvwi.Tag = item;
                    lvw.Items.Add(lvwi);
                }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Test();
        }

        private void Test()
        {
            Cursor = Cursors.WaitCursor;
            btnTest.Enabled = false;
            btnTest.Text = "Testing...";

            var items = new List<ListViewItem>(lvw.Items.Count);
            foreach (ListViewItem item in lvw.Items)
                items.Add(item);

            //Do this on another message loop.
            StaticActiveObjectWrapper.ActiveObject.Send(_testAllConnectionsDel, items);
        }

        private void TestConnections(List<ListViewItem> items)
        {
            try
            {
                _totalNumberOfConnections = items.Count;
                _testedConnections = 0;

                foreach (ListViewItem item in items)
                {
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                        {
                            item.BackColor = Color.White;
                            item.SubItems[1].Text = "Testing...";
                        }, null);
                    //Use the state object, otherwise there will be a reference mismatch.
                    var t = new Thread(delegate(object state)
                        {
                            try
                            {
                                _testWorkItem = new TestWorkItem();
                                _testWorkItem.Message += _testWorkItem_Message;
                                _testWorkItem.TestConnection(state as ListViewItem);
                                if (Interlocked.Increment(ref _testedConnections) == _totalNumberOfConnections)
                                    _testAutoResetEvent.Set();
                            }
                            catch
                            {
                            }
                        });
                    t.IsBackground = true;
                    t.Start(item);
                }
                _testAutoResetEvent.WaitOne();

                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                    {
                        btnTest.Text = "Test";
                        btnTest.Enabled = true;
                        Cursor = Cursors.Default;
                        Invalidate(true);
                    }, null);
            }
            catch
            {
            }
        }

        private void _testWorkItem_Message(object sender, TestWorkItem.MessageEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    e.Item.BackColor = e.Succes ? Color.LawnGreen : Color.Red;
                    e.Item.SubItems[1].Text = e.Message;
                }, null);
        }

        private void lvw_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnCopyErrorMessage.Enabled = (lvw.SelectedItems.Count != 0 && lvw.SelectedItems[0].BackColor == Color.Red);
        }

        private void btnCopyErrorMessage_Click(object sender, EventArgs e)
        {
            ClipboardWrapper.SetDataObject(lvw.SelectedItems[0].SubItems[1].Text);
        }

        private delegate void TestAllConnectionsDel(List<ListViewItem> items);

        private class TestWorkItem
        {
            public event EventHandler<MessageEventArgs> Message;

            public void TestConnection(ListViewItem item)
            {
                var connection = item.Tag as Connection;

                if (connection.ConnectionProxy.IsEmpty)
                {
                    if (Message != null)
                        Message(this,
                                new MessageEventArgs(item, false, "This connection has no connection proxy assigned to!"));
                    return;
                }

                var connectionProxyPool = new ConnectionProxyPool(connection);
                CompilerResults compilerResults = connectionProxyPool.CompileConnectionProxyClass(false);

                if (compilerResults.Errors.HasErrors)
                {
                    var sb = new StringBuilder("Failed at compiling the connection proxy class: ");
                    sb.AppendLine();
                    foreach (CompilerError error in compilerResults.Errors)
                    {
                        sb.AppendFormat("Error number {0}, Line {1}, Column {2}: {3}", error.ErrorNumber, error.Line,
                                        error.Column, error.ErrorText);
                        sb.AppendLine();
                    }

                    if (Message != null)
                        Message(this, new MessageEventArgs(item, false, sb.ToString()));
                }
                else
                {
                    string error;
                    connectionProxyPool.TestConnection(out error);
                    SynchronizationContextWrapper.SynchronizationContext.Post(delegate
                        {
                            if (error == null)
                            {
                                if (Message != null)
                                    Message(this, new MessageEventArgs(item, true, "OK"));
                            }
                            else
                            {
                                if (Message != null)
                                    Message(this,
                                            new MessageEventArgs(item, false,
                                                                 "Failed to connect with the given credentials: " +
                                                                 error));
                            }
                        }, null);
                }
                connectionProxyPool.Dispose();
                connectionProxyPool = null;
            }

            public class MessageEventArgs : EventArgs
            {
                public readonly ListViewItem Item;
                public readonly string Message;
                public readonly bool Succes;

                public MessageEventArgs(ListViewItem item, bool succes, string message)
                {
                    Item = item;
                    Succes = succes;
                    Message = message;
                }
            }
        }
    }
}