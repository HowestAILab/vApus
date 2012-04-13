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
            BaseParameter generateFromParameter = _customListParameter.GenerateFromParameter;

            List<string> entries = new List<string>(_customListParameter.CustomList);

            if (generateFromParameter is TextParameter || (generateFromParameter as NumericParameter).Random)
                for (int i = 0; i < nudGenerate.Value; i++)
                {
                    generateFromParameter.Next();
                    entries.Add(generateFromParameter.Value);
                }
            else
                for (int i = 0; i < nudGenerate.Value; i++)
                {
                    entries.Add(generateFromParameter.Value);
                    generateFromParameter.Next();
                }
            generateFromParameter.ResetValue();

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
            BaseParameter p = null;
            if (cboParameterType.SelectedIndex == 0)
                p = new NumericParameter();
            else if (cboParameterType.SelectedIndex == 1)
                p = new TextParameter();
            else
                p = new CustomRandomParameter();

            _customListParameter.GenerateFromParameter = p;

            if (p is CustomRandomParameter)
            {
                if (pnlPlaceHolder.Controls.Count == 0 || pnlPlaceHolder.Controls[0] != _customRandomParameterPanel)
                {
                    pnlPlaceHolder.Controls.Clear();
                    pnlPlaceHolder.Controls.Add(_customRandomParameterPanel);
                    _parameterTypeSolutionComponentPropertyPanel.Dock = DockStyle.Fill;
                }
                _customRandomParameterPanel.Init(p);
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
