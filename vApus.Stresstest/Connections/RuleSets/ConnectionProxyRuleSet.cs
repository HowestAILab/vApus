/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest
{
    [ContextMenu(new string[] { "Activate_Click", "Add_Click", "Clear_Click", "Paste_Click" }, new string[] { "Edit", "Add Syntax Item", "Clear", "Paste" })]
    [Hotkeys(new string[] { "Activate_Click", "Add_Click", "Paste_Click" }, new Keys[] { Keys.Enter, Keys.Insert, (Keys.Control | Keys.V) })]
    [DisplayName("Connection Proxy Rule Set"), Serializable]
    public class ConnectionProxyRuleSet : BaseItem
    {
        #region Fields
        private string _childDelimiter = string.Empty, _description = string.Empty;
        private bool _connected = true;
        private uint _tracertField = 1;
        #endregion

        #region Properties
        //[SavableCloneable, PropertyControl(1)]
        //[Description("A value to specify that different rule sets are compatible with each other, for example: log and connection rule sets")]
        //public string Category
        //{
        //    get { return _catagory; }
        //    set { _catagory = value; }
        //}
        [SavableCloneable, PropertyControl(2)]
        [Description("If the length of the delimiter is zero, the given string will not be splitted into parts (space = valid)."), DisplayName("Child Delimiter")]
        public virtual string ChildDelimiter
        {
            get { return _childDelimiter; }
            set { _childDelimiter = value; }
        }
        [SavableCloneable, PropertyControl(int.MaxValue)]
        [Description("Describes this rule set.")]
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [SavableCloneable, PropertyControl(3)]
        [Description("Is it a connected or connectionless protocol that is used?")]
        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; }
        }
        [SavableCloneable, PropertyControl(3)]
        [Description("The one-base index of the syntax item that is used for tracing the route of communication."), DisplayName("Trace Route Field")]
        public uint TracertField
        {
            get { return _tracertField; }
            set { _tracertField = value; }
        }
        #endregion

        public ConnectionProxyRuleSet()
        { }

        #region Functions
        protected void Add_Click(object sender, EventArgs e)
        {
            Add(new ConnectionProxySyntaxItem());
        }
        /// <summary>
        /// Lexes the input if possible and builds an AST.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parameters">Can be null.</param>
        /// <param name="output"></param>
        /// <returns></returns>
        public LexicalResult TryLexicalAnalysis(string input, Parameters parameters, out ASTNode output)
        {
            return RuleSetLexer.TryLexicalAnalysis(input, this, _childDelimiter, parameters, out output);
        }
        public IEnumerable<Control> GetControls()
        {
            foreach (BaseItem item in this)
                yield return (item as SyntaxItem).GetControl();
        }
        #endregion
    }
}
