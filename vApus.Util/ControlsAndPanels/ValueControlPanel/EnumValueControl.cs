/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util {
    public partial class EnumValueControl : BaseValueControl, IValueControl {
        [ToolboxItem(false)]
        public EnumValueControl() {
            InitializeComponent();
        }

        /// <summary>
        ///     This inits the control with event handling.
        /// </summary>
        /// <param name="value"></param>
        public void Init(Value value) {
            base.__Value = value;

            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            ComboBox cbo = null;

            if (base.ValueControl == null) {
                cbo = new ComboBox();
                cbo.Dock = DockStyle.Fill;
                cbo.DropDownStyle = ComboBoxStyle.DropDownList;
                cbo.FlatStyle = FlatStyle.Flat;
                cbo.BackColor = Color.White;

                cbo.SelectedIndexChanged += cbo_SelectedIndexChanged;
                cbo.Leave += cbo_Leave;
                cbo.KeyUp += cbo_KeyUp;
            } else {
                cbo = base.ValueControl as ComboBox;
            }

            cbo.SelectedIndexChanged -= cbo_SelectedIndexChanged;
            cbo.Items.Clear();
            //Extract all the values.
            Type valueType = value.__Value.GetType();
            foreach (Enum e in Enum.GetValues(valueType)) {
                //The description value will be used instead of the tostring of the enum, if any.
                var attr =
                    valueType.GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as
                    DescriptionAttribute[];
                cbo.Items.Add(attr.Length != 0 ? attr[0].Description : e.ToString());
            }

            var attr2 =
                valueType.GetField(value.__Value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false)
                as DescriptionAttribute[];
            cbo.SelectedItem = attr2.Length > 0 ? attr2[0].Description : value.__Value.ToString();

            cbo.SelectedIndexChanged += cbo_SelectedIndexChanged;

            base.ValueControl = cbo;
        }

        private void cbo_SelectedIndexChanged(object sender, EventArgs e) {
            var cbo = base.ValueControl as ComboBox;
            base.HandleValueChanged(ExtractValue(cbo));
        }

        private void cbo_KeyUp(object sender, KeyEventArgs e) {
            var cbo = base.ValueControl as ComboBox;
            base.HandleKeyUp(e.KeyCode, ExtractValue(cbo));
        }

        private void cbo_Leave(object sender, EventArgs e) {
            try {
                var cbo = base.ValueControl as ComboBox;
                base.HandleValueChanged(ExtractValue(cbo));
            } catch {
            }
        }

        private object ExtractValue(ComboBox cbo) {
            Type valueType = base.__Value.__Value.GetType();
            foreach (Enum e in Enum.GetValues(valueType)) {
                var attr =
                    valueType.GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as
                    DescriptionAttribute[];
                if (cbo.SelectedItem.ToString() == (attr.Length != 0 ? attr[0].Description : e.ToString()))
                    return e;
            }
            return null;
        }
    }
}