/*
 * Copyright 2008 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util
{
    /// <summary>
    /// An implementation of the old VB6 input dialog.
    /// </summary>
    public partial class InputDialog : Form
    {
        #region Fields
        private int _minimumInputLength;
        private MessageBoxButtons _messageBoxButtons = MessageBoxButtons.OKCancel;
        private DialogResult _ok = DialogResult.OK, _cancel = DialogResult.Cancel;
        #endregion

        #region Properties
        public int MinimumInputLength
        {
            get { return _minimumInputLength; }
        }
        public int MaximumInputLength
        {
            get { return txtInput.MaxLength; }
        }
        public string Input
        {
            get { return txtInput.Text; }
            set { txtInput.Text = value; }
        }
        /// <summary>
        /// Default: MessageBoxButtons.OKCancel
        /// </summary>
        public MessageBoxButtons MessageBoxButtons
        {
            set
            {
                if (value != _messageBoxButtons)
                {
                    _messageBoxButtons = value;
                    switch (_messageBoxButtons)
                    {
                        case MessageBoxButtons.OK:
                            btnOK.Text = "OK";
                            btnCancel.Text = "Cancel";
                            _ok = DialogResult.OK;
                            _cancel = DialogResult.Cancel;
                            btnCancel.Enabled = false;
                            break;
                        case MessageBoxButtons.OKCancel:
                            btnOK.Text = "OK";
                            btnCancel.Text = "Cancel";
                            _ok = DialogResult.OK;
                            _cancel = DialogResult.Cancel;
                            btnCancel.Enabled = true;
                            break;
                        case MessageBoxButtons.YesNo:
                        case MessageBoxButtons.YesNoCancel:
                            btnOK.Text = "Yes";
                            btnCancel.Text = "No";
                            _ok = DialogResult.Yes;
                            _cancel = DialogResult.No;
                            btnCancel.Enabled = true;
                            break;
                    }
                }
            }
            get { return _messageBoxButtons; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// An implementation of the old VB6 input dialog.
        /// </summary>
        public InputDialog(string question, string caption = "", string defaultValue = "")
        {
            InitializeComponent();
            lblQuestion.Text = question;
            Text = caption;
            txtInput.Text = defaultValue;
            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(InputDialog_HandleCreated);
        }
        #endregion

        #region Functions
        private void InputDialog_HandleCreated(object sender, EventArgs e)
        {
            SetGui();
        }
        private void SetGui()
        {
            Graphics g = lblQuestion.CreateGraphics();
            int difference = txtInput.Height - (int)g.MeasureString(lblQuestion.Text, lblQuestion.Font).Height;
            if (this.Height - difference > this.MinimumSize.Height)
                this.Height -= difference;
        }
        public void SetInputLength(int min, int max = int.MaxValue)
        {
            if (min > max)
                throw new ArgumentException("min cannot be larger than max.");
            _minimumInputLength = min;
            txtInput.MaxLength = max;

            btnOK.Enabled = txtInput.Text.Length >= _minimumInputLength;
        }
        /// <summary></summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = _ok;
            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = _cancel;
            this.Close();
        }
        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = txtInput.Text.Length >= _minimumInputLength;
        }


        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            txtInput.Focus();
        }
        #endregion
    }
}