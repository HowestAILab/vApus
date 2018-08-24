/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace vApus.Util {
    [ToolboxItem(false)]
    public partial class CharValueControl : BaseValueControl, IValueControl {
        public CharValueControl() {
            InitializeComponent();
            base.SyncGuiWithValueRequested += _SyncGuiWithValueRequested;
        }

        /// <summary>
        ///     This inits the control with event handling.
        /// </summary>
        /// <param name="value"></param>
        public void Init(Value value) {
            base.__Value = value;

            //Only take the value into account, the other properties are taken care off.
            //Keep control recycling in mind.
            TextBox txt = null;

            if (base.ValueControl == null) {
                txt = new TextBox();

                txt.MaxLength = 1;
                txt.Dock = DockStyle.Fill;

                txt.Leave += txt_Leave;
                txt.KeyUp += txt_KeyUp;

                base.ValueControl = txt;
            }
            else {
                txt = base.ValueControl as TextBox;
            }

            txt.Text = value.__Value.ToString();
        }
        private void _SyncGuiWithValueRequested(object sender, EventArgs e) {
            if (base.ValueControl != null) {
                string value = base.__Value.__Value.ToString();
                var txt = base.ValueControl as TextBox;
                if (txt.Text != value) txt.Text = base.__Value.__Value.ToString();
            }
        }
        private void txt_KeyUp(object sender, KeyEventArgs e) {
            var txt = sender as TextBox;
            if (txt.Text.Length != 0)
                base.HandleKeyUp(e.KeyCode, txt.Text[0]);
        }

        private void txt_Leave(object sender, EventArgs e) {
            try {
                var txt = sender as TextBox;
                if (txt.Text.Length == 0 && !ParentForm.IsDisposed && !ParentForm.Disposing)
                    txt.Text = base.__Value.__Value.ToString();
                else
                    base.HandleValueChanged(txt.Text[0]);
            }
            catch {
            }
        }

        protected override void RevertToDefaultValueOnGui() {
            var txt = ValueControl as TextBox;
            txt.Text = base.__Value.DefaultValue.ToString();
        }
    }
}