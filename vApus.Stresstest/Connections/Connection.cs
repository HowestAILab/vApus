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

namespace vApus.Stresstest {
    /// <summary>
    /// Brings connection string, proxy and parameters together in one config file.
    /// </summary>
    [Serializable]
    [ContextMenu(new[] { "Activate_Click", "Remove_Click", "Export_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { "Edit", "Remove", "Export", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    public class Connection : LabeledBaseItem, ISerializable {

        #region Fields
        private ConnectionProxy _connectionProxy;
        private string _connectionString = string.Empty;

        private static Parameters _parameters;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl(1)]
        [Description("To be able to connect to the application-to-test."), DisplayName("Connection Proxy")]
        public ConnectionProxy ConnectionProxy {
            get {
                if (Solution.ActiveSolution != null && (_connectionProxy.IsEmpty || _connectionProxy.Parent == null))
                    _connectionProxy = GetNextOrEmptyChild(typeof(ConnectionProxy), Solution.ActiveSolution.GetSolutionComponent(typeof(ConnectionProxies))) as ConnectionProxy;

                return _connectionProxy;
            }
            set {
                if (value == null)
                    return;
                _connectionProxy = value;
            }
        }

        [SavableCloneable(true)]
        [DisplayName("Connection String")]
        [ReadOnly(true), Browsable(false)]
        public string ConnectionString {
            get { return _connectionString; }
            set {
                value = value.Trim();
                var lexicalResult = LexicalResult.OK;
                ASTNode output = null;

                if (Solution.ActiveSolution != null)
                    try {
                        var connectionProxy = ConnectionProxy;
                        if (value.Length != 0 && connectionProxy != null && !connectionProxy.IsEmpty && !connectionProxy.ConnectionProxyRuleSet.IsEmpty) {
                            lexicalResult = connectionProxy.ConnectionProxyRuleSet.TryLexicalAnalysis(value, _parameters, out output);

                            if (output != null) {
                                output.Dispose();
                                output = null;
                            }
                        }
                    } catch {
                        //While loading. Ignore.
                    }

                if (lexicalResult == LexicalResult.OK)
                    _connectionString = value;
                else
                    throw new Exception(output.Error);
            }
        }

        /// <summary>
        /// For a distributed test.
        /// </summary>
        internal static Parameters Parameters {
            set { _parameters = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Brings connection string, proxy and parameters together in one config file.
        /// </summary>
        public Connection() {
            if (Solution.ActiveSolution != null) {
                ConnectionProxy = GetNextOrEmptyChild(typeof(ConnectionProxy), Solution.ActiveSolution.GetSolutionComponent(typeof(ConnectionProxies))) as ConnectionProxy;

                if (_parameters == null)
                    _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
            } else {
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
            }
        }

        /// <summary>
        ///     Only for sending from master to slave. (Serialization)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public Connection(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                _connectionProxy = sr.ReadObject() as ConnectionProxy;
                _connectionString = sr.ReadString();
            }
            sr = null;
        }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            ConnectionProxy = GetNextOrEmptyChild(typeof(ConnectionProxy), Solution.ActiveSolution.GetSolutionComponent(typeof(ConnectionProxies))) as ConnectionProxy;

            if (_parameters == null)
                _parameters = Solution.ActiveSolution.GetSolutionComponent(typeof(Parameters)) as Parameters;
        }
        /// <summary>
        ///     Only for sending from master to slave.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(Label);
                sw.WriteObject(ConnectionProxy);
                sw.Write(ConnectionString);
                sw.AddToInfo(info);
            }
            sw = null;
        }

        /// <summary>
        ///     Build and returns a new connection proxy class as string.
        ///     This is further used in vApus.Util.CompilerUnit to compile. 
        ///     Each simulated user in StresstestCore has an instance of this class (in ConnectionProxyPool) to be able to connect to a server app and communicate with it.
        /// </summary>
        /// <returns></returns>
        public string BuildConnectionProxyClass() { return ConnectionProxy.BuildConnectionProxyClass(ConnectionString); }

        public Connection Clone() {
            var clone = new Connection();
            clone._connectionProxy = ConnectionProxy;
            clone._connectionString = ConnectionString;
            return clone;
        }
        public override BaseSolutionComponentView Activate() { return SolutionComponentViewManager.Show(this); }

        public new void Dispose() {
            _parameters = null;
            base.Dispose();
        }
        #endregion
    }
}
