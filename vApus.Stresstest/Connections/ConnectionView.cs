/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.CodeDom.Compiler;
using System.Text;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;
using System;
using System.Threading;

namespace vApus.Stresstest
{
    public partial class ConnectionView : BaseSolutionComponentView
    {
        #region Fields
        private delegate void TestConnectionDel();
        private TestConnectionDel _testConnectionDel;

        private Connection _connection;
        /// <summary>
        /// For endless loops.
        /// </summary>
        private bool _canUpdateGui = true;

       // private AutoResetEvent _tracertWaitHandle = new AutoResetEvent(false);
        #endregion

        #region Constructors
        public ConnectionView()
        {
            InitializeComponent();
        }
        public ConnectionView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();
            _connection = solutionComponent as Connection;

            _testConnectionDel = new TestConnectionDel(TestConnection);

            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new System.EventHandler(ConnectionView_HandleCreated);
        }
        #endregion

        #region Functions
        private void ConnectionView_HandleCreated(object sender, System.EventArgs e)
        {
            HandleCreated -= ConnectionView_HandleCreated;
            SetGui();
        }
        private void SetGui()
        {
            solutionComponentPropertyPanel.SolutionComponent = _connection;
            ruleSetSyntaxItemPanel.SetRuleSetAndInput(_connection.ConnectionProxy.ConnectionProxyRuleSet, _connection.ConnectionString);
            ruleSetSyntaxItemPanel.InputChanged += new System.EventHandler(ruleSetSyntaxItemPanel_InputChanged);
        }

        private void ruleSetSyntaxItemPanel_InputChanged(object sender, System.EventArgs e)
        {
            _connection.ConnectionString = ruleSetSyntaxItemPanel.Input;
            _canUpdateGui = false;
            _connection.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }

        private void btnTestConnection_Click(object sender, System.EventArgs e)
        {
            split.Enabled = false;
            btnTestConnection.Enabled = false;
            btnTestConnection.Text = "Testing...";

            Tracert();

            StaticActiveObjectWrapper.ActiveObject.Send(_testConnectionDel);
        }
        private void Tracert()
        {
            tracertControl.Visible = true;

            string[] split = _connection.ConnectionString.Split(new string[] { _connection.ConnectionProxy.ConnectionProxyRuleSet.ChildDelimiter }, StringSplitOptions.None);
            string tracertField = split[_connection.ConnectionProxy.ConnectionProxyRuleSet.TracertField - 1];

            if (tracertField.ContainsChars('/'))
            {
                if (tracertField.StartsWith("http://"))
                    tracertField = tracertField.Substring("http://".Length);

                tracertField = tracertField.Split('/')[0];
            }

            tracertControl.Trace(tracertField);
        }
        private void tracertControl_Done(object sender, EventArgs e)
        {
            //_tracertWaitHandle.Set();
        }
        private void TestConnection()
        {
            if (_connection.ConnectionProxy.IsEmpty)
            {
                //_tracertWaitHandle.WaitOne();
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    btnTestConnection.Text = "Test Connection";
                    split.Enabled = true;
                    btnTestConnection.Enabled = true;

                    MessageBox.Show(this, "This connection has no connection proxy assigned to!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                });
                return;
            }

            ConnectionProxyPool connectionProxyPool = new ConnectionProxyPool(_connection);
            CompilerResults compilerResults = connectionProxyPool.CompileConnectionProxyClass(false);

            if (compilerResults.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder("Failed at compiling the connection proxy class: ");
                sb.AppendLine();
                foreach (CompilerError error in compilerResults.Errors)
                {
                    sb.AppendFormat("Error number {0}, Line {1}, Column {2}: {3}", error.ErrorNumber, error.Line, error.Column, error.ErrorText);
                    sb.AppendLine();
                }

               // _tracertWaitHandle.WaitOne();
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    btnTestConnection.Text = "Test Connection";
                    split.Enabled = true;
                    btnTestConnection.Enabled = true;

                    MessageBox.Show(this, sb.ToString(), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                });
            }
            else
            {
                string error;
                connectionProxyPool.TestConnection(out error);
                
               // _tracertWaitHandle.WaitOne();
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    btnTestConnection.Text = "Test Connection";
                    split.Enabled = true;
                    btnTestConnection.Enabled = true;

                    if (error == null)
                        MessageBox.Show(this, "The connection has been established! and closed again successfully.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    else
                        MessageBox.Show(this, "The connection could not be made, please make sure everything is filled in correctly.\nException: " + error, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                });
            }
            connectionProxyPool.Dispose();
            connectionProxyPool = null;
        }
        public override void Refresh()
        {
            if (_canUpdateGui)
            {
                base.Refresh();
                solutionComponentPropertyPanel.Refresh();
                ruleSetSyntaxItemPanel.SetRuleSetAndInput(_connection.ConnectionProxy.ConnectionProxyRuleSet, _connection.ConnectionString);
            }
            _canUpdateGui = true;
        }
        #endregion
    }
}