﻿/*
 * 2011 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.StressTest {
    /// <summary>
    /// To generate a list of parameters of a given type. Those are added to the given CustomListParameter.
    /// </summary>
    public partial class CustomListGeneratorDialog : Form {

        #region Fields
        private readonly CustomListParameter _customListParameter;
        private readonly CustomRandomParameterPanel _customRandomParameterPanel = new CustomRandomParameterPanel();

        private readonly SolutionComponentPropertyPanel _parameterTypeSolutionComponentPropertyPanel = new SolutionComponentPropertyPanel();
        private BaseParameter _generateFromParameter;
        #endregion

        #region Constructors
        /// <summary>
        /// Design time constructor
        /// </summary>
        public CustomListGeneratorDialog() {
            InitializeComponent();
            _parameterTypeSolutionComponentPropertyPanel.AllowInvokingSolutionComponentChangedEvent = false;
            _parameterTypeSolutionComponentPropertyPanel.ValueChanged += (s,e) => { timer.Start(); }; //Workaround for refreshing the property values on the GUI. For some reason the default sync way does not work.
        }

        /// <summary>
        /// To generate a list of parameters of a given type. Those are added to the given CustomListParameter.
        /// </summary>
        /// <param name="customListParameter"></param>
        public CustomListGeneratorDialog(CustomListParameter customListParameter)
            : this() {
            _customListParameter = customListParameter;

            if (IsHandleCreated) {
                if (cboParameterType.SelectedIndex == -1) {
                    if (_customListParameter.GenerateFromParameter is NumericParameter)
                        cboParameterType.SelectedIndex = 0;
                    else if (_customListParameter.GenerateFromParameter is TextParameter)
                        cboParameterType.SelectedIndex = 1;
                    else
                        cboParameterType.SelectedIndex = 2;
                } 
            } else {
                HandleCreated += CustomListGenerator_HandleCreated;
            }
        }
        #endregion

        #region Functions
        private void CustomListGenerator_HandleCreated(object sender, EventArgs e) {
            if (cboParameterType.SelectedIndex == -1) {
                if (_customListParameter.GenerateFromParameter is NumericParameter)
                    cboParameterType.SelectedIndex = 0;
                else if (_customListParameter.GenerateFromParameter is TextParameter)
                    cboParameterType.SelectedIndex = 1;
                else
                    cboParameterType.SelectedIndex = 2;
            }
        }

        private void cboParameterType_SelectedIndexChanged(object sender, EventArgs e) {
            timer.Stop();
            if (cboParameterType.SelectedIndex == 0)
                _generateFromParameter = new NumericParameter();
            else if (cboParameterType.SelectedIndex == 1)
                _generateFromParameter = new TextParameter();
            else
                _generateFromParameter = new CustomRandomParameter();

            _customListParameter.GenerateFromParameter = _generateFromParameter;

            if (_generateFromParameter is CustomRandomParameter) {
                if (pnlPlaceHolder.Controls.Count == 0 || pnlPlaceHolder.Controls[0] != _customRandomParameterPanel) {
                    pnlPlaceHolder.Controls.Clear();
                    pnlPlaceHolder.Controls.Add(_customRandomParameterPanel);
                    _customRandomParameterPanel.Dock = DockStyle.Fill;
                }
                _customRandomParameterPanel.Init(_generateFromParameter);
            } else {
                if (pnlPlaceHolder.Controls.Count == 0 ||
                    pnlPlaceHolder.Controls[0] != _parameterTypeSolutionComponentPropertyPanel) {
                    pnlPlaceHolder.Controls.Clear();
                    pnlPlaceHolder.Controls.Add(_parameterTypeSolutionComponentPropertyPanel);
                    _parameterTypeSolutionComponentPropertyPanel.Dock = DockStyle.Fill;
                }
                _parameterTypeSolutionComponentPropertyPanel.SolutionComponent = _customListParameter.GenerateFromParameter;
                timer.Start();
            }
        }
        private void timer_Tick(object sender, EventArgs e) {
            if (!IsDisposed && IsHandleCreated && Visible) {
                _parameterTypeSolutionComponentPropertyPanel.Refresh();
                timer.Stop();
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            var entries = new List<string>(_customListParameter.CustomList);
            bool customRandomParameterException = false;

            if (cboParameterType.SelectedIndex == 2)
                _generateFromParameter = _customRandomParameterPanel.Parameter;
            else
                _generateFromParameter = _parameterTypeSolutionComponentPropertyPanel.SolutionComponent as BaseParameter;

            if (_generateFromParameter is TextParameter ||
                (_generateFromParameter is NumericParameter && (_generateFromParameter as NumericParameter).Random))
                for (int i = 0; i != nudGenerate.Value; i++) {
                    _generateFromParameter.Next();
                    entries.Add(_generateFromParameter.Value);
                } else if (_generateFromParameter is CustomRandomParameter)
                try {
                    Exception exception;
                    _customRandomParameterPanel.TryCompileAndTestCode(out exception);
                    if (exception != null)
                        throw exception;

                    for (int i = 0; i != nudGenerate.Value; i++) {
                        _generateFromParameter.Next();
                        entries.Add(_generateFromParameter.Value);
                    }
                } catch {
                    customRandomParameterException = true;
                } else
                for (int i = 0; i != nudGenerate.Value; i++) {
                    entries.Add(_generateFromParameter.Value);
                    _generateFromParameter.Next();
                }
            _generateFromParameter.ResetValue();

            Cursor = Cursors.Default;

            if (!customRandomParameterException) {
                _customListParameter.CustomList = entries.ToArray();

                DialogResult = DialogResult.OK;
                Close();
            }
        }
        private void btnCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        #endregion
    }
}