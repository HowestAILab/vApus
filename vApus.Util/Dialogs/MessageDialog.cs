/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;

namespace vApus.Util
{
    public partial class MessageDialog : Form
    {
        public override string Text
        {
            get { return rtxt == null ? string.Empty : rtxt.Text; }
            set { rtxt.Text = value; }
        }
        public string Title
        {
            get { return base.Text; }
            set { base.Text = value; }
        }
        #region Constructor
        public MessageDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        private void btnCopy_Click(object sender, EventArgs e)
        {
            ClipboardWrapper.SetDataObject(rtxt.Text);
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion
    }
}
