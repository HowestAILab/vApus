/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using vApus.SolutionTree;

namespace vApus.Stresstest
{
    public partial class CustomListGenerator : Form
    {
        private SolutionComponentPropertyPanel _parameterTypeSolutionComponentPropertyPanel = new SolutionComponentPropertyPanel();
        private CustomRandomParameterPanel _customRandomParameterPanel = new CustomRandomParameterPanel();
        private CustomListParameter _customListParameter;
        private BaseParameter _generateFromParameter;

        public CustomListGenerator()
        {
            InitializeComponent();
        }

        public CustomListGenerator(CustomListParameter customListParameter)
            : this()
        {
            _customListParameter = customListParameter;

            if (this.IsHandleCreated)
            {
                if (cboParameterType.SelectedIndex == -1)
                    cboParameterType.SelectedIndex = _customListParameter.GenerateFromParameter is NumericParameter ? 0 : 1;
                else
                    timer.Start();
            }
            else
            {
                this.HandleCreated += new EventHandler(CustomListGenerator_HandleCreated);
            }
        }

        private void CustomListGenerator_HandleCreated(object sender, EventArgs e)
        {
            if (cboParameterType.SelectedIndex == -1)
                cboParameterType.SelectedIndex = _customListParameter.GenerateFromParameter is NumericParameter ? 0 : 1;
            else
                timer.Start();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            List<string> entries = new List<string>(_customListParameter.CustomList);

            if (_generateFromParameter is TextParameter ||
                (_generateFromParameter is NumericParameter && (_generateFromParameter as NumericParameter).Random))
                for (int i = 0; i != nudGenerate.Value; i++)
                {
                    _generateFromParameter.Next();
                    entries.Add(_generateFromParameter.Value);
                }
            else if (_generateFromParameter is CustomRandomParameter)
                for (int i = 0; i != nudGenerate.Value; i++)
                {
                    _generateFromParameter.Next();
                    entries.Add(_generateFromParameter.Value);
                }
            else
                for (int i = 0; i != nudGenerate.Value; i++)
                {
                    entries.Add(_generateFromParameter.Value);
                    _generateFromParameter.Next();
                }
            _generateFromParameter.ResetValue();

            _customListParameter.CustomList = entries.ToArray();
            Cursor = Cursors.Default;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cboParameterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer.Stop();
            if (cboParameterType.SelectedIndex == 0)
                _generateFromParameter = new NumericParameter();
            else if (cboParameterType.SelectedIndex == 1)
                _generateFromParameter = new TextParameter();
            else
                _generateFromParameter = new CustomRandomParameter();

            _customListParameter.GenerateFromParameter = _generateFromParameter;

            if (_generateFromParameter is CustomRandomParameter)
            {
                if (pnlPlaceHolder.Controls.Count == 0 || pnlPlaceHolder.Controls[0] != _customRandomParameterPanel)
                {
                    pnlPlaceHolder.Controls.Clear();
                    pnlPlaceHolder.Controls.Add(_customRandomParameterPanel);
                    _customRandomParameterPanel.Dock = DockStyle.Fill;
                }
                _customRandomParameterPanel.Init(_generateFromParameter);
            }
            else
            {
                if (pnlPlaceHolder.Controls.Count == 0 || pnlPlaceHolder.Controls[0] != _parameterTypeSolutionComponentPropertyPanel)
                {
                    pnlPlaceHolder.Controls.Clear();
                    pnlPlaceHolder.Controls.Add(_parameterTypeSolutionComponentPropertyPanel);
                    _parameterTypeSolutionComponentPropertyPanel.Dock = DockStyle.Fill;
                }
                _parameterTypeSolutionComponentPropertyPanel.SolutionComponent = _customListParameter.GenerateFromParameter;
                timer.Start();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!this.IsDisposed && this.IsHandleCreated && this.Visible)
                foreach (SolutionComponentCommonPropertyControl ctrl in _parameterTypeSolutionComponentPropertyPanel.SolutionComponentPropertyControls)
                    if (ctrl.Label == "Label")
                    {
                        ctrl.Visible = false;
                        timer.Stop();
                        break;
                    }
        }
    }
}
