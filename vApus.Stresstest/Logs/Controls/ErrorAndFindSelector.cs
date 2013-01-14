/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace vApus.Stresstest
{
    public partial class ErrorAndFindSelector : UserControl
    {
        private readonly List<LogEntryControl> _errors = new List<LogEntryControl>();
        private int _errorIndex;
        private LogChildControlBase _found;

        public ErrorAndFindSelector()
        {
            InitializeComponent();
        }

        public LogChildControlBase Found
        {
            get { return _found; }
            set { _found = value; }
        }

        public event EventHandler<SelectErrorEventArgs> SelectError;
        public event EventHandler<FindEventArgs> Find;

        /// <summary>
        ///     Clears and sets invisible.
        /// </summary>
        public void ClearErrors()
        {
            _errorIndex = 0;
            _errors.Clear();
            btnSelectError.Text = "1 / ?";
            btnPreviousError.Enabled = false;
            btnNextError.Enabled = true;

            btnSelectError.Visible = false;
            btnPreviousError.Visible = false;
            btnNextError.Visible = false;
        }

        /// <summary>
        ///     Adds and sets visible.
        /// </summary>
        /// <param name="error"></param>
        public void AddError(LogEntryControl error)
        {
            if (!_errors.Contains(error))
            {
                _errors.Add(error);
                btnSelectError.Text = (_errorIndex + 1) + " / " + _errors.Count;
                btnNextError.Enabled = (_errorIndex < _errors.Count - 1);
                btnPreviousError.Enabled = (_errorIndex > 0);
                if (_errors.Count == 1)
                {
                    btnSelectError.Visible = true;
                    btnPreviousError.Visible = true;
                    btnNextError.Visible = true;
                    if (SelectError != null)
                        SelectError(this, new SelectErrorEventArgs(_errors[_errorIndex]));
                }
            }
        }

        public void RemoveError(LogEntryControl error)
        {
            _errors.Remove(error);
            if (_errors.Count == 0)
            {
                ClearErrors();
            }
            else
            {
                if (_errorIndex > 0)
                    --_errorIndex;
                btnSelectError.Text = (_errorIndex + 1) + " / " + _errors.Count;
                btnNextError.Enabled = (_errorIndex < _errors.Count - 1);
                btnPreviousError.Enabled = (_errorIndex > 0);
            }
        }

        private void btnPreviousError_Click(object sender, EventArgs e)
        {
            --_errorIndex;
            IndexChanged();
        }

        private void btnNextError_Click(object sender, EventArgs e)
        {
            ++_errorIndex;
            IndexChanged();
        }

        private void IndexChanged()
        {
            btnSelectError.Text = (_errorIndex + 1) + " / " + _errors.Count;
            btnNextError.Enabled = (_errorIndex < _errors.Count - 1);
            btnPreviousError.Enabled = (_errorIndex > 0);
            if (SelectError != null)
                SelectError(this, new SelectErrorEventArgs(_errors[_errorIndex]));
        }

        private void btnSelectError_Click(object sender, EventArgs e)
        {
            if (SelectError != null)
                SelectError(this, new SelectErrorEventArgs(_errors[_errorIndex]));
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (Find != null)
                Find(this, new FindEventArgs(txtFind.Text));
        }

        private void txtFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && Find != null)
                Find(this, new FindEventArgs(txtFind.Text));
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            _found = null;
        }
    }

    public class SelectErrorEventArgs : EventArgs
    {
        public readonly LogEntryControl Error;

        public SelectErrorEventArgs(LogEntryControl error)
        {
            Error = error;
        }
    }

    public class FindEventArgs : EventArgs
    {
        public readonly string Find;

        public FindEventArgs(string find)
        {
            Find = find;
        }
    }
}