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

namespace vApus.Util
{
    [ToolboxItem(false)]
    public partial class BoolValueControl : BaseValueControl, IValueControl
    {
        public BoolValueControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This inits the control with event handling.
        /// </summary>
        /// <param name="value"></param>
        public void Init(BaseValueControl.Value value)
        {
            base.__Value = value;

            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            CheckBox chk = null;
            if (base.ValueControl == null)
            {
                chk = new CheckBox();
                chk.Dock = DockStyle.Top;
                chk.CheckedChanged += new EventHandler(chk_CheckedChanged);
                chk.Leave += new EventHandler(chk_Leave);
                chk.KeyUp += new KeyEventHandler(chk_KeyUp);
            }
            else
            {
                chk = base.ValueControl as CheckBox;
            }

            chk.CheckedChanged -= chk_CheckedChanged;
            chk.Checked = (bool)value.__Value;
            SetChkText(chk);
            chk.CheckedChanged += chk_CheckedChanged;

            base.ValueControl = chk;
        }
        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = ValueControl as CheckBox;
            SetChkText(chk);
            base.HandleValueChanged(chk.Checked);
        }
        private void chk_KeyUp(object sender, KeyEventArgs e)
        {
            CheckBox chk = ValueControl as CheckBox;
            SetChkText(chk);
            base.HandleKeyUp(e.KeyCode, chk.Checked);
        }
        private void chk_Leave(object sender, EventArgs e)
        {
            try
            {
                CheckBox chk = ValueControl as CheckBox;
                SetChkText(chk);
                base.HandleValueChanged(chk.Checked);
            }
            catch { }
        }
        private void SetChkText(CheckBox chk)
        {
            chk.Text = "[" + (chk.Checked ? "Checked " : "Unchecked ") + "equals " + chk.Checked + "]";
        }
    }
}