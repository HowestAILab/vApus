/*
 * Copyright 2012 (c) Sizing Servers Lab
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
    ///     Serves at combining a label control and a text box.
    /// </summary>
    public class LabeledTextBox : TextBox
    {
        private readonly ToolTip _toolTip = new ToolTip();
        private bool _canInvokeTextChanged = true;
        private string _emptyTextBoxLabel = "Label";
        private bool _settingLabel;

        public LabeledTextBox()
        {
            ForeColor = Color.DimGray;
        }

        public string EmptyTextBoxLabel
        {
            get { return _emptyTextBoxLabel; }
            set
            {
                _settingLabel = true;
                _emptyTextBoxLabel = value;
                _toolTip.SetToolTip(this, _emptyTextBoxLabel);
                if (Text.Length == 0)
                {
                    ForeColor = Color.DimGray;
                    Text = EmptyTextBoxLabel;
                }
                _settingLabel = false;
            }
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (!_settingLabel) ForeColor = Color.Black;
                base.Text = value;
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            _canInvokeTextChanged = false;
            if (ForeColor == Color.DimGray)
                Text = string.Empty;

            base.OnEnter(e);
            _canInvokeTextChanged = true;
        }

        public void InvokeOnLeave()
        {
            OnLeave(new EventArgs());
        }

        protected override void OnLeave(EventArgs e)
        {
            _canInvokeTextChanged = false;
            Text = Text.Trim();
            EmptyTextBoxLabel = _emptyTextBoxLabel;
            base.OnLeave(e);
            _canInvokeTextChanged = true;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (_canInvokeTextChanged && ForeColor != Color.DimGray)
                base.OnTextChanged(e);
        }
    }
}