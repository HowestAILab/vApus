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
    public partial class StringValueControl : BaseValueControl, IValueControl {
        private PictureBox pic;

        public StringValueControl() {
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
                txt.Dock = DockStyle.Fill;

                txt.Leave += txt_Leave;
                txt.KeyUp += txt_KeyUp;
                txt.VisibleChanged += txt_VisibleChanged;
                
                base.ValueControl = txt;
            }
            else {
                txt = base.ValueControl as TextBox;
            }

            txt.Text = value.__Value as string;


            if (IsEncrypted) {
                pic = new PictureBox();
                pic.SizeMode = PictureBoxSizeMode.AutoSize;
                pic.Image = vApus.Util.Properties.Resources.Eye_black_16;

                split.Panel1.Controls.Add(pic);
                pic.Left = split.Panel1.Width - 21;
                pic.Top = 5;
                pic.Visible = txt.Visible;

                pic.Click += pic_Click;

                txt.Dock = DockStyle.None;
                txt.Width = pic.Left - 9;
            }
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
            base.HandleKeyUp(e.KeyCode, txt.Text);
        }

        private void txt_Leave(object sender, EventArgs e) {
            try {
                var txt = sender as TextBox;
                if (txt.Text.Length != 0 || (!ParentForm.IsDisposed && !ParentForm.Disposing)) 
                    base.HandleValueChanged(txt.Text);
            } catch {
            }
        }

        private void txt_VisibleChanged(object sender, EventArgs e) {
            if (pic != null) pic.Visible = (sender as TextBox).Visible;
        }

        private void pic_Click(object sender, EventArgs e) {
            var txt = base.ValueControl as TextBox;
            txt.UseSystemPasswordChar = !txt.UseSystemPasswordChar;
        }

        protected override void RevertToDefaultValueOnGui() {
            var txt = base.ValueControl as TextBox;
            txt.Text = base.__Value.DefaultValue as string;
        }
    }
}