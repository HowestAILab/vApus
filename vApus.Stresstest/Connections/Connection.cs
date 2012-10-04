/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    [Serializable]
    [ContextMenu(new string[] { "Activate_Click", "Remove_Click", "Export_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new string[] { "Edit", "Remove", "Export", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new string[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new Keys[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    public class Connection : LabeledBaseItem, ISerializable
    {
        #region Fields
        private ConnectionProxy _connectionProxy;
        private string _connectionString = string.Empty;

        private Parameters _parameters;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl(1)]
        [Description("To be able to connect to the application-to-test."), DisplayName("Connection Proxy")]
        public ConnectionProxy ConnectionProxy
        {
            get { return _connectionProxy; }
            set
            {
                if (value == null)
                    return;
                value.ParentIsNull -= _connectionProxy_ParentIsNull;
                _connectionProxy = value;
                _connectionProxy.ParentIsNull += _connectionProxy_ParentIsNull;
            }
        }

        [SavableCloneable(true)]
        [DisplayName("Connection String")]
        [ReadOnly(true), Browsable(false)]
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                value = value.Trim();
                LexicalResult lexicalResult = LexicalResult.OK;
                ASTNode output = null;
                if (_connectionProxy != null && !_connectionProxy.IsEmpty && !_connectionProxy.ConnectionProxyRuleSet.IsEmpty)
                    lexicalResult = _connectionProxy.ConnectionProxyRuleSet.TryLexicalAnalysis(value, _parameters, out output);

                if (lexicalResult == LexicalResult.OK)
                    _connectionString = value;
                else
                    throw new Exception(output.Error);
            }
        }
        #endregion

        #region Constructors
        public Connection()
        {
            if (Solution.ActiveSolution != null)
            {
                ConnectionProxy = SolutionComponent.GetNextOrEmptyChild(typeof(ConnectionProxy), Solution.ActiveSolution.GetSolutionComponent(typeof(ConnectionProxies))) as ConnectionProxy;
                if (_parameters == null)
                    _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
            }
            else
            {
                Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);
            }
        }
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            ConnectionProxy = SolutionComponent.GetNextOrEmptyChild(typeof(ConnectionProxy), Solution.ActiveSolution.GetSolutionComponent(typeof(ConnectionProxies))) as ConnectionProxy;
            if (_parameters == null)
                _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
        }
        public override void Activate()
        {
            SolutionComponentViewManager.Show(this);
        }
        /// <summary>
        /// Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public Connection(SerializationInfo info, StreamingContext ctxt)
        {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info))
            {
                Label = sr.ReadString();
                _connectionProxy = sr.ReadObject() as ConnectionProxy;
                _connectionString = sr.ReadString();
                _parameters = sr.ReadObject() as Parameters;
            }
            sr = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }
        #endregion

        #region Functions
        private void _connectionProxy_ParentIsNull(object sender, EventArgs e)
        {
            if (_connectionProxy == sender)
                ConnectionProxy = SolutionComponent.GetNextOrEmptyChild(typeof(ConnectionProxy), Solution.ActiveSolution.GetSolutionComponent(typeof(ConnectionProxies))) as ConnectionProxy;
        }

        /// <summary>
        /// Build and returns a new connection proxy class. 
        /// </summary>
        /// <returns></returns>
        public string BuildConnectionProxyClass()
        {
            return _connectionProxy.BuildConnectionProxyClass(_connectionString);
        }
       
        /// <summary>
        /// Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter())
            {
                sw.Write(Label);
                sw.WriteObject(_connectionProxy);
                sw.Write(_connectionString);
                sw.WriteObject(_parameters);
                sw.AddToInfo(info);
            }
            sw = null;
            //Not pretty, but helps against mem saturation.
            GC.Collect();
        }
        #endregion
    }
}
