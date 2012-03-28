/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using vApus.SolutionTree;
using System.Windows.Forms;

namespace vApus.Stresstest
{
    [ContextMenu(new string[] { "Remove_Click", "Export_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new string[] { "Remove", "Export", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new string[] { "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new Keys[] { Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    [DisplayName("Connection Proxy"), Serializable]
    public class ConnectionProxy : LabeledBaseItem
    {
        #region Properties
        public ConnectionProxyRuleSet ConnectionProxyRuleSet
        {
            get { return this[0] as ConnectionProxyRuleSet; }
        }
        public ConnectionProxyCode ConnectionProxyCode
        {
            get { return this[1] as ConnectionProxyCode; }
        }
        #endregion

        #region Constructor
        public ConnectionProxy()
        {
            AddAsDefaultItem(new ConnectionProxyRuleSet());
            AddAsDefaultItem(new ConnectionProxyCode());
        }
        #endregion

        #region Functions
        /// <summary>
        /// Build and returns a new connection proxy class. 
        /// </summary>
        /// <returns></returns>
        public string BuildConnectionProxyClass(string connectionString = "")
        {
            return ConnectionProxyCode.BuildConnectionProxyClass(ConnectionProxyRuleSet, connectionString);
        }
        #endregion
    }
}
