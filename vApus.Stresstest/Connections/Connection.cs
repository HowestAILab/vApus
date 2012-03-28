/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest
{
    [Serializable]
    [ContextMenu(new string[] { "Activate_Click", "Remove_Click", "Export_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new string[] { "Edit", "Remove", "Export", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new string[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new Keys[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    public class Connection : LabeledBaseItem
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
            set { _connectionProxy = value; }
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
                _connectionProxy = BaseItem.Empty(typeof(ConnectionProxy), Solution.ActiveSolution.GetSolutionComponent(typeof(ConnectionProxies))) as ConnectionProxy;
            else
                Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);
        }
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            _connectionProxy = BaseItem.Empty(typeof(ConnectionProxy), Solution.ActiveSolution.GetSolutionComponent(typeof(ConnectionProxies))) as ConnectionProxy;
            if (_parameters == null)
                _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
        }
        public override void Activate()
        {
            SolutionComponentViewManager.Show(this);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Build and returns a new connection proxy class. 
        /// </summary>
        /// <returns></returns>
        public string BuildConnectionProxyClass()
        {
            return _connectionProxy.BuildConnectionProxyClass(_connectionString);
        }
        #endregion
    }
}
