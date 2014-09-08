/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using vApus.Monitor.Sources.Base;
using vApus.Util;

namespace vApus.Monitor {
    public partial class MonitorParameterPanel : ValueControlPanel {
        private Parameter[] _parameters;

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
        public Parameter[] Parameters {
            //Get is not used, but it is here when needed.
            get { return _parameters; }
            set {
                _parameters = value;
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
            foreach (Parameter parameter in _parameters)
                if (i++ == e.Index) {
                    parameter.Value = e.NewValue;
                    break;
                }
            if (ParameterValueChanged != null)
                ParameterValueChanged(this, null);
        }

        private void ParameterPanel_HandleCreated(object sender, EventArgs e) {
            SetGui();
        }

        private void SetGui() {
            if (_parameters != null) {
                var values = new List<BaseValueControl.Value>(_parameters.Length);
                foreach (Parameter parameter in _parameters) {
                    string description = parameter.Description;
                    if (parameter.DefaultValue.ToString().Length != 0)
                        description += " [Default Value: '" + parameter.DefaultValue + "']";
                    if (!parameter.Optional)
                        description += " [Obligatory]";

                    values.Add(new BaseValueControl.Value {
                        __Value = parameter.Value,
                        Description = description,
                        IsEncrypted = parameter.Encrypted,
                        IsReadOnly = false,
                        Label = parameter.Name,
                        AllowedMaximum = int.MaxValue,
                        AllowedMinimum = int.MinValue
                    });
                }
                base.SetValues(values.ToArray());
            }
        }
    }
}