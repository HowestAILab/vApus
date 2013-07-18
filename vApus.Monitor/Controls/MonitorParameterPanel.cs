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
using vApusSMT.Base;

namespace vApus.Monitor {
    public partial class MonitorParameterPanel : ValueControlPanel {
        private Dictionary<Parameter, object> _parametersWithValues;

        public MonitorParameterPanel() {
            InitializeComponent();
            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += ParameterPanel_HandleCreated;

            ValueChanged += ParameterPanel_ValueChanged;
        }

        /// <summary>
        ///     Sets the Gui if the panel is empty.
        /// </summary>
        public Dictionary<Parameter, object> ParametersWithValues {
            //Get is not used, but it is here when needed.
            get { return _parametersWithValues; }
            set {
                _parametersWithValues = value;
                SetGui();
            }
        }

        /// <summary>
        ///     All the solution component property controls.
        /// </summary>
        public ControlCollection ParameterControls {
            get { return base.Controls; }
        }

        public event EventHandler ParameterValueChanged;

        private void ParameterPanel_ValueChanged(object sender, ValueChangedEventArgs e) {
            int i = 0;
            foreach (Parameter parameter in _parametersWithValues.Keys)
                if (i++ == e.Index) {
                    _parametersWithValues[parameter] = e.NewValue;
                    break;
                }
            if (ParameterValueChanged != null)
                ParameterValueChanged(this, null);
        }

        private void ParameterPanel_HandleCreated(object sender, EventArgs e) {
            SetGui();
        }

        private void SetGui() {
            if (_parametersWithValues != null) {
                var values = new List<BaseValueControl.Value>(_parametersWithValues.Count);
                foreach (Parameter parameter in _parametersWithValues.Keys) {
                    object value = _parametersWithValues[parameter];
                    if (value == null)
                        value = parameter.DefaultValue;

                    string description = parameter.Description;
                    if (parameter.DefaultValue.ToString().Length != 0)
                        description += "[Default Value: '" + parameter.DefaultValue + "']";
                    if (!parameter.Optional)
                        description += " [Obligatory]";

                    values.Add(new BaseValueControl.Value {
                        __Value = value,
                        Description = description,
                        IsEncrypted = parameter.Encrypted,
                        IsReadOnly = false,
                        Label = parameter.Name
                    });
                }
                base.SetValues(values.ToArray());
            }
        }
    }
}