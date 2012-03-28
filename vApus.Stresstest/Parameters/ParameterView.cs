/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;
using vApus.SolutionTree;
using System.Collections.Generic;
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class ParameterView : BaseSolutionComponentView
    {
        #region Fields
        private BaseParameter _parameter;
        private CustomListGenerator _customListGenerator;
        #endregion

        #region Constructors
        public ParameterView()
        {
            InitializeComponent();
        }
        public ParameterView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();
            _parameter = solutionComponent as BaseParameter;
            solutionComponentPropertyPanel.SolutionComponent = solutionComponent;
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);

            SetGui();
        }
        #endregion

        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            solutionComponentPropertyPanel.Refresh();
            SetGui();
        }

        private void SetGui()
        {
            if (_parameter is CustomListParameter)
            {
                pnlCustomList.Visible = true;

                CustomListParameter parameter = _parameter as CustomListParameter;
                btnClear.Enabled = parameter.CustomList.Length > 0;
                HashSet<string> entries = new HashSet<string>();
                foreach (string s in parameter.CustomList)
                    if (!entries.Add(s))
                    {
                        btnRemoveDuplicates.Enabled = true;
                        return;
                    }
                btnRemoveDuplicates.Enabled = false;

            }
            else
            {
                pnlCustomList.Visible = false;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (_customListGenerator == null)
                _customListGenerator = new CustomListGenerator(_parameter as CustomListParameter);

            if (_customListGenerator.ShowDialog() == DialogResult.OK)
            {
                SolutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
                solutionComponentPropertyPanel.Refresh();
            }
        }

        private void btnAddFromText_Click(object sender, EventArgs e)
        {
            CustomListParameter parameter = _parameter as CustomListParameter;

            FromTextDialog fromTextDialog = new FromTextDialog();
            fromTextDialog.ShowDialog();
            List<string> entries = new List<string>(parameter.CustomList);
            entries.AddRange(fromTextDialog.Entries);
            parameter.CustomList = entries.ToArray();
            _parameter = parameter;
            SolutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            solutionComponentPropertyPanel.Refresh();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            (_parameter as CustomListParameter).CustomList = new string[] { };
            SolutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            solutionComponentPropertyPanel.Refresh();
        }

        private void btnRemoveDuplicates_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            CustomListParameter parameter = _parameter as CustomListParameter;
            HashSet<string> entries = new HashSet<string>();
            foreach (string value in parameter.CustomList)
                entries.Add(value);
            parameter.CustomList = new string[entries.Count];
            entries.CopyTo(parameter.CustomList);
            _parameter = parameter;
            SolutionComponent.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            solutionComponentPropertyPanel.Refresh();
            btnRemoveDuplicates.Enabled = false;
            Cursor = Cursors.Default;
        }
    }
}
