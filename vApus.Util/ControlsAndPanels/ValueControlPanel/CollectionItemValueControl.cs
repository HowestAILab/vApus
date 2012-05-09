/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util
{
    public partial class CollectionItemValueControl : BaseValueControl, IValueControl
    {
        public CollectionItemValueControl()
        {
            InitializeComponent();
        }

        public void Init(BaseValueControl.Value value)
        {
            base.__Value = value;

            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            ComboBox cbo = null;
            if (base.ValueControl == null)
            {
                cbo = new ComboBox();
                cbo.DropDownStyle = ComboBoxStyle.DropDownList;
                cbo.FlatStyle = FlatStyle.Flat;
                cbo.BackColor = Color.White;

                cbo.Dock = DockStyle.Fill;

                cbo.DropDown += new EventHandler(cbo_DropDown);
                cbo.SelectedIndexChanged += new EventHandler(cbo_SelectedIndexChanged);
                cbo.Leave += new EventHandler(cbo_Leave);
                cbo.KeyUp += new KeyEventHandler(cbo_KeyUp);
            }
            else
            {
                cbo = base.ValueControl as ComboBox;
            }

            SetCBO(cbo);

            base.ValueControl = cbo;
        }
        private void SetCBO(ComboBox cbo)
        {
            cbo.SelectedIndexChanged -= cbo_SelectedIndexChanged;

            cbo.Items.Clear();

            if (base.ValueParent != null)
                foreach (object childItem in (base.ValueParent as IEnumerable))
                    if (childItem.GetType() == base.__Value.__Value.GetType())
                        cbo.Items.Add(childItem);

            cbo.SelectedItem = base.__Value.__Value;

            cbo.SelectedIndexChanged += cbo_SelectedIndexChanged;

            //Revert to the first one available if the item is not found (handy when using an Item.Empty static property for instance, it must still have the correct parent!).
            if (cbo.Items.Count != 0 && cbo.SelectedIndex == -1)
                cbo.SelectedIndex = 0;
        }
        private void cbo_DropDown(object sender, EventArgs e)
        {
            SetCBO(ValueControl as ComboBox);
        }
        private void cbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Use sender here, it can change before the ValueControlisknown(see SetCBO).
            ComboBox cbo = sender as ComboBox;
            if (cbo.SelectedIndex != -1)
                base.HandleValueChanged(cbo.SelectedItem);
        }
        private void cbo_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox cbo = ValueControl as ComboBox;
            if (cbo.SelectedIndex != -1)
                base.HandleKeyUp(e.KeyCode, cbo.SelectedItem);
        }
        private void cbo_Leave(object sender, EventArgs e)
        {
            ComboBox cbo = ValueControl as ComboBox;
            if (cbo.SelectedIndex != -1)
                base.HandleValueChanged(cbo.SelectedItem);
        }
    }
}
