/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace vApus.Util {
    [ToolboxItem(false)]
    public partial class BoolValueControl : BaseValueControl, IValueControl {
        public BoolValueControl() {
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
            CheckBox chk = null;
            if (base.ValueControl == null) {
                chk = new CheckBox();
                chk.Dock = DockStyle.Top;
                chk.CheckedChanged += chk_CheckedChanged;
                chk.Leave += chk_Leave;
                chk.KeyUp += chk_KeyUp;

                base.ValueControl = chk;
            }
            else {
                chk = base.ValueControl as CheckBox;
            }

            chk.CheckedChanged -= chk_CheckedChanged;
            chk.Checked = (bool)value.__Value;
            SetChkText(chk);
            chk.CheckedChanged += chk_CheckedChanged;
        }

        private void chk_CheckedChanged(object sender, EventArgs e) {
            var chk = ValueControl as CheckBox;
            SetChkText(chk);
            base.HandleValueChanged(chk.Checked);
        }

        private void chk_KeyUp(object sender, KeyEventArgs e) {
            var chk = ValueControl as CheckBox;
            SetChkText(chk);
            base.HandleKeyUp(e.KeyCode, chk.Checked);
        }

        private void chk_Leave(object sender, EventArgs e) {
            try {
                if (!ParentForm.IsDisposed && !ParentForm.IsDisposed) {
                    var chk = ValueControl as CheckBox;
                    SetChkText(chk);
                    base.HandleValueChanged(chk.Checked);
                }
            } catch {
            }
        }

        private void SetChkText(CheckBox chk) {
            chk.Text = "[" + (chk.Checked ? "Checked " : "Unchecked ") + "equals " + chk.Checked + "]";
        }

        protected override void RevertToDefaultValueOnGui() {
            var chk = ValueControl as CheckBox;
            chk.CheckedChanged -= chk_CheckedChanged;
            chk.Checked = (bool)base.__Value.DefaultValue;
            SetChkText(chk);
            chk.CheckedChanged += chk_CheckedChanged;
        }
    }
}