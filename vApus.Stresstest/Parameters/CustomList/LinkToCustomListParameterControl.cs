﻿/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    [ToolboxItem(false)]
    public partial class LinkToCustomListParameterControl : BaseValueControl, IValueControl {
        public LinkToCustomListParameterControl() {
            InitializeComponent();
            base.SyncGuiWithValueRequested += _SyncGuiWithValueRequested;
        }

        public void Init(Value value) {
            base.__Value = value;

            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            ComboBox cbo = null;
            if (base.ValueControl == null) {
                cbo = new ComboBox();
                cbo.DropDownStyle = ComboBoxStyle.DropDownList;
                cbo.FlatStyle = FlatStyle.Flat;
                cbo.BackColor = Color.White;

                cbo.Dock = DockStyle.Fill;

                cbo.DropDown += cbo_DropDown;
                cbo.SelectedIndexChanged += cbo_SelectedIndexChanged;
                cbo.Leave += cbo_Leave;
                cbo.KeyUp += cbo_KeyUp;

                base.ValueControl = cbo;
            }
            else {
                cbo = base.ValueControl as ComboBox;
            }

            SetCBO(cbo);
        }
        private void _SyncGuiWithValueRequested(object sender, EventArgs e) {
            //Probably not needed. Not implemented --> too much overhead.
        }
        private void SetCBO(ComboBox cbo) {
            cbo.SelectedIndexChanged -= cbo_SelectedIndexChanged;

            cbo.Items.Clear();

            var value = base.__Value.__Value as CustomListParameter;

            if (base.ValueParent != null) {
                var parent = value.Parent as CustomListParameters;
                var tag = value.GetTag();

                BaseItem empty = null;
                if (value.IsEmpty) {
                    empty = value;
                } else {
                    empty = BaseItem.GetEmpty(typeof(CustomListParameter), parent);
                    empty.SetParent(base.ValueParent);
                    empty.SetTag(tag);
                }

                cbo.Items.Add(empty);
                foreach (CustomListParameter childItem in parent)
                    if (childItem != tag)
                        cbo.Items.Add(childItem);
            }

            cbo.SelectedItem = value;

            cbo.SelectedIndexChanged += cbo_SelectedIndexChanged;

            //Revert to the first one available if the item is not found (handy when using an Item.Empty static property for instance, it must still have the correct parent!).
            if (cbo.Items.Count != 0 && cbo.SelectedIndex == -1)
                cbo.SelectedIndex = 0;
        }

        private void cbo_DropDown(object sender, EventArgs e) {
            SetCBO(ValueControl as ComboBox);
        }

        private void cbo_SelectedIndexChanged(object sender, EventArgs e) {
            //Use sender here, it can change before the ValueControl is known(see SetCBO).
            var cbo = sender as ComboBox;
            if (cbo.SelectedIndex != -1)
                base.HandleValueChanged(cbo.SelectedItem);
        }

        private void cbo_KeyUp(object sender, KeyEventArgs e) {
            var cbo = ValueControl as ComboBox;
            if (cbo.SelectedIndex != -1)
                base.HandleKeyUp(e.KeyCode, cbo.SelectedItem);
        }

        private void cbo_Leave(object sender, EventArgs e) {
            try {
                var cbo = ValueControl as ComboBox;
                if (cbo.SelectedIndex != -1)
                    base.HandleValueChanged(cbo.SelectedItem);
            } catch {
                //Ignore. Not important.
            }
        }

        protected override void RevertToDefaultValueOnGui() {
            //Not supported
        }
    }
}