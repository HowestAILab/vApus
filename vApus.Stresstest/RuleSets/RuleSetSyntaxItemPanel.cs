/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using vApus.Util;

namespace vApus.StressTest {
    public partial class RuleSetSyntaxItemPanel : ValueControlPanel {
        #region Events

        public event EventHandler InputChanged;

        #endregion

        #region Fields

        private string _input;
        private BaseRuleSet _ruleSet;
        private List<string> _splitInput;

        #endregion

        #region Properties

        public BaseRuleSet RuleSet {
            get { return _ruleSet; }
        }

        public string Input {
            get { return _input; }
        }

        #endregion

        public RuleSetSyntaxItemPanel() {
            InitializeComponent();

            ValueChanged += RuleSetSyntaxItemPanel_ValueChanged;
        }

        private void RuleSetSyntaxItemPanel_ValueChanged(object sender, ValueChangedEventArgs e) {
            _input = string.Empty;
            _splitInput = new List<string> { };
            for (int i = 0; i < base.ValueControls.Count; i++) {
                var valueControl = base.ValueControls[i] as BaseValueControl;
                string value = valueControl.__Value.__Value.ToString();

                _splitInput.Add(value);
                _input = (i == 0) ? value : string.Format("{0}{1}{2}", _input, _ruleSet.ChildDelimiter, value);
            }
            if (InputChanged != null)
                InputChanged(this, null);
        }

        public void SetRuleSetAndInput(BaseRuleSet ruleSet, string input) {
            if (ruleSet == null)
                throw new ArgumentNullException("ruleSet");
            if (input == null)
                throw new ArgumentNullException("input");

            ValueChanged -= RuleSetSyntaxItemPanel_ValueChanged;

            _ruleSet = ruleSet;
            _input = input;
            _splitInput = new List<string>(_input.Split(new[] { _ruleSet.ChildDelimiter }, StringSplitOptions.None));

            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += RuleSetSyntaxItemPanel_HandleCreated;

            ValueChanged += RuleSetSyntaxItemPanel_ValueChanged;
        }

        private void RuleSetSyntaxItemPanel_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= RuleSetSyntaxItemPanel_HandleCreated;
            SetGui();
        }

        private void SetGui() {
            var values = new List<BaseValueControl.Value>(_ruleSet.Count);
            if (_input != null && _ruleSet != null) {
                if (_input.Length == 0) {
                    for (int i = 0; i < _ruleSet.Count; i++)
                        values.Add(CreateValue(_ruleSet[i] as SyntaxItem, string.Empty));
                } else {
                    int indexModifier = 0;
                    for (int i = 0; i < _ruleSet.Count; i++) {
                        var syntaxItem = _ruleSet[i] as SyntaxItem;
                        if (indexModifier >= _splitInput.Count)
                            values.Add(CreateValue(syntaxItem, string.Empty));
                        else
                            values.Add(CreateValue(syntaxItem, _splitInput[indexModifier]));
                        ++indexModifier;
                    }
                }
            }
            base.SetValues(false, values.ToArray());
        }

        private BaseValueControl.Value CreateValue(SyntaxItem syntaxItem, string input) {
            //Revert to the default value if need be.
            if (string.IsNullOrEmpty(input)) input = syntaxItem.DefaultValue;

            object value = input;
            object defaultValue = syntaxItem.DefaultValue;
            bool isEncrypted = false;
            if (syntaxItem.Count != 0 && syntaxItem[0] is Rule) {
                var rule = syntaxItem[0] as Rule;
                isEncrypted = rule.DisplayAsPassword;

                switch (rule.ValueType) {
                    case Rule.ValueTypes.boolType:
                        bool b;
                        value = bool.TryParse(input, out b) ? b : false;
                        defaultValue = bool.TryParse(syntaxItem.DefaultValue, out b) ? b : false;
                        break;
                    case Rule.ValueTypes.charType:
                        value = input[0];
                        defaultValue = syntaxItem.DefaultValue[0];
                        break;
                    case Rule.ValueTypes.decimalType:
                        decimal dec = 0;
                        decimal.TryParse(input, out dec);
                        value = dec;
                        decimal.TryParse(syntaxItem.DefaultValue, out dec);
                        defaultValue = dec;
                        break;
                    case Rule.ValueTypes.doubleType:
                        double d = 0;
                        double.TryParse(input, out d);
                        value = d;
                        double.TryParse(syntaxItem.DefaultValue, out d);
                        defaultValue = d;
                        break;
                    case Rule.ValueTypes.floatType:
                        float f = 0;
                        float.TryParse(input, out f);
                        value = f;
                        float.TryParse(syntaxItem.DefaultValue, out f);
                        defaultValue = f;
                        break;
                    case Rule.ValueTypes.intType:
                        int i = 0;
                        int.TryParse(input, out i);
                        value = i;
                        int.TryParse(syntaxItem.DefaultValue, out i);
                        defaultValue = i;
                        break;
                    case Rule.ValueTypes.longType:
                        long l = 0;
                        long.TryParse(input, out l);
                        value = l;
                        long.TryParse(syntaxItem.DefaultValue, out l);
                        defaultValue = l;
                        break;
                    case Rule.ValueTypes.shortType:
                        short s = 0;
                        short.TryParse(input, out s);
                        value = s;
                        short.TryParse(syntaxItem.DefaultValue, out s);
                        defaultValue = s;
                        break;
                    case Rule.ValueTypes.uintType:
                        uint ui = 0;
                        uint.TryParse(input, out ui);
                        value = ui;
                        uint.TryParse(syntaxItem.DefaultValue, out ui);
                        defaultValue = ui;
                        break;
                    case Rule.ValueTypes.ulongType:
                        ulong ul = 0;
                        ulong.TryParse(input, out ul);
                        value = ul;
                        ulong.TryParse(syntaxItem.DefaultValue, out ul);
                        defaultValue = ul;
                        break;
                    case Rule.ValueTypes.ushortType:
                        ushort us = 0;
                        ushort.TryParse(input, out us);
                        value = us;
                        ushort.TryParse(syntaxItem.DefaultValue, out us);
                        defaultValue = us;
                        break;
                }
            }

            return new BaseValueControl.Value {
                __Value = value,
                DefaultValue = defaultValue,
                Description = syntaxItem.Description,
                IsEncrypted = isEncrypted,
                IsReadOnly = false,
                Label = syntaxItem.Label,
                AllowedMaximum = int.MaxValue,
                AllowedMinimum = int.MinValue
            };
        }
    }
}