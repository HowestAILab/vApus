/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util {
    [ToolboxItem(false)]
    public partial class CollectionItemValueControl : BaseValueControl, IValueControl {
        public CollectionItemValueControl() {
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

        private void cbo_DropDown(object sender, EventArgs e) {
            SetCBO(ValueControl as ComboBox);
        }

        private void cbo_SelectedIndexChanged(object sender, EventArgs e) {
            //Use sender here, it can change before the ValueControlisknown(see SetCBO).
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
            }
        }

        protected override void RevertToDefaultValueOnGui() {
            var cbo = base.ValueControl as ComboBox;

            cbo.SelectedIndexChanged -= cbo_SelectedIndexChanged;

            cbo.Items.Clear();

            if (base.ValueParent != null)
                foreach (object childItem in (base.ValueParent as IEnumerable))
                    if (childItem.GetType() == base.__Value.__Value.GetType())
                        cbo.Items.Add(childItem);

            cbo.SelectedItem = base.__Value.DefaultValue;

            cbo.SelectedIndexChanged += cbo_SelectedIndexChanged;

            //Revert to the first one available if the item is not found (handy when using an Item.Empty static property for instance, it must still have the correct parent!).
            if (cbo.Items.Count != 0 && cbo.SelectedIndex == -1)
                cbo.SelectedIndex = 0;
        }
    }
}