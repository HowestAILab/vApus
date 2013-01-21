/*
 * Copyright 2010 (c) Sizing Servers Lab
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
    [ContextMenu(new[] {"Activate_Click", "Add_Click", "Clear_Click", "Paste_Click"},
        new[] {"Edit", "Add Syntax Item", "Clear", "Paste"})]
    [Hotkeys(new[] {"Activate_Click", "Add_Click", "Paste_Click"},
        new[] {Keys.Enter, Keys.Insert, (Keys.Control | Keys.V)})]
    [DisplayName("Connection Proxy Rule Set"), Serializable]
    public class ConnectionProxyRuleSet : BaseRuleSet
    {
        #region Fields

        private bool _connected = true;
        private uint _tracertField = 1;

        #endregion

        #region Properties

        public new string Label
        {
            get { return string.Empty; }
            set { }
        }

        [SavableCloneable, PropertyControl(3)]
        [Description("Is it a connected or connectionless protocol that is used?")]
        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; }
        }

        [SavableCloneable, PropertyControl(3)]
        [Description("The one-base index of the syntax item that is used for tracing the route of communication."),
         DisplayName("Trace Route Field")]
        public uint TracertField
        {
            get { return _tracertField; }
            set { _tracertField = value; }
        }

        #endregion

        #region Functions

        protected new void Add_Click(object sender, EventArgs e)
        {
            Add(new ConnectionProxySyntaxItem());
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}