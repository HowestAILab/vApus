/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest
{
    /// <summary>
    /// </summary>
    public partial class ConnectionProxyRuleSetSyntaxItemPanel : UserControl
    {
        #region Events
        public event EventHandler InputChanged;
        #endregion

        #region Fields
        private ConnectionProxyRuleSet _ruleSet;
        private string _input;
        private List<string> _splitInput;
        #endregion

        #region Properties
        public ConnectionProxyRuleSet RuleSet
        {
            get { return _ruleSet; }
        }
        public string Input
        {
            get { return _input; }
        }

        public FlowDirection FlowDirection
        {
            get { return flp.FlowDirection; }
            set { flp.FlowDirection = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// </summary>
        public ConnectionProxyRuleSetSyntaxItemPanel()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        private void RuleSetSyntaxItemPanel_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= RuleSetSyntaxItemPanel_HandleCreated;
            SetGui();
        }
        /// <summary>
        /// This serves also as a refresh of the Gui, when the input and ruleset are unchanged this does nothing.
        /// </summary>
        /// <param name="ruleSet"></param>
        /// <param name="input"></param>
        public void SetRuleSetAndInput(ConnectionProxyRuleSet ruleSet, string input)
        {
            if (ruleSet == null)
                throw new ArgumentNullException("ruleSet");
            if (input == null)
                throw new ArgumentNullException("input");

            _ruleSet = ruleSet;
            _input = input;
            _splitInput = new List<string>(_input.Split(new string[] { _ruleSet.ChildDelimiter }, StringSplitOptions.None));

            if (IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(RuleSetSyntaxItemPanel_HandleCreated);
        }
        private void SetGui()
        {
            this.SuspendLayout();
            flp.Controls.Clear();

            if (_input != null && _ruleSet != null)
            {
                if (_input.Length == 0)
                    for (int i = 0; i < _ruleSet.Count; i++)
                    {
                        SyntaxItem syntaxItem = _ruleSet[i] as SyntaxItem;
                        flp.Controls.Add(new SyntaxItemControl(syntaxItem, string.Empty));
                    }
                else
                {
                    int indexModifier = 0;
                    for (int i = 0; i < _ruleSet.Count; i++)
                    {
                        SyntaxItem syntaxItem = _ruleSet[i] as SyntaxItem;
                        if (indexModifier == _splitInput.Count)
                        {
                            flp.Controls.Add(new SyntaxItemControl(syntaxItem, string.Empty));
                        }
                        else
                        {
                            flp.Controls.Add(new SyntaxItemControl(syntaxItem, _splitInput[indexModifier]));
                            ++indexModifier;
                        }
                    }
                }
            }
            if (flp.Controls.Count > 0)
                flp.Controls[0].VisibleChanged += new EventHandler(RuleSetSyntaxItemPanel_VisibleChanged);
            foreach (Control control in flp.Controls)
                (control as SyntaxItemControl).InputChanged += new EventHandler(RuleSetSyntaxItemPanel_InputChanged);
            flp.AutoScroll = true;
            this.ResumeLayout(true);
        }
        private void RuleSetSyntaxItemPanel_VisibleChanged(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control.Visible)
            {
                control.VisibleChanged -= RuleSetSyntaxItemPanel_VisibleChanged;
                control.Focus();
            }
        }
        private void RuleSetSyntaxItemPanel_InputChanged(object sender, EventArgs e)
        {
            _input = string.Empty;
            _splitInput = new List<string> { };
            for (int i = 0; i < flp.Controls.Count; i++)
            {
                SyntaxItemControl syntaxItemControl = flp.Controls[i] as SyntaxItemControl;
                _splitInput.Add(syntaxItemControl.Value);
                if (i == 0)
                    _input = syntaxItemControl.Value;
                else
                    _input = string.Format("{0}{1}{2}", _input, _ruleSet.ChildDelimiter, syntaxItemControl.Value);
            }
            if (InputChanged != null)
                InputChanged(this, null);
        }
        #endregion
    }
}
