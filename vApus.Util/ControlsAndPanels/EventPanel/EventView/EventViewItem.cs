/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vApus.Util {
    [ToolboxItem(false)]
    public class EventViewItem : Label {

        #region Fields
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        private readonly LargeList _parent;
        private readonly ToolTip _toolTip = new ToolTip();
        private DateTime _at;
        private int _defaultHeight;
        private bool _entered;
        private EventViewEventType _eventType;
        private List<int> _lineBreaks = new List<int>();
        private string _message;
        #endregion

        #region Properties
        public EventViewEventType EventType {
            get { return _eventType; }
            set {
                _eventType = value;
                if (_eventType == EventViewEventType.Info) {
                    _toolTip.SetToolTip(this, "Right-click to copy.");
                    ForeColor = Color.DimGray;
                } else if (_eventType == EventViewEventType.Warning) {
                    _toolTip.SetToolTip(this, "Right-click to copy.");
                    ForeColor = Color.Orange;
                } else {
                    _toolTip.SetToolTip(this, "The given stack trace, if any, is for the developer. Right-click to copy.");
                    ForeColor = Color.Red;
                }
            }
        }

        public string Message {
            get { return _message; }
            set {
                _message = value;
                SetText();
            }
        }

        public DateTime At {
            get { return _at; }
            set {
                _at = value;
                SetText();
            }
        }
        #endregion

        public EventViewItem(LargeList parent, EventViewEventType eventType, string message, DateTime at) {
            AutoSize = true;
            AutoEllipsis = true;
            Cursor = Cursors.Hand;
            Padding = new Padding(0, 1, 0, 1);
            TextAlign = ContentAlignment.MiddleLeft;

            _parent = parent;

            EventType = eventType;

            _message = message;
            _at = at;
            SetText();
        }

        #region Functions
        private void SetText() {
            string message;
            if (_entered) {
                message = _message;
            } else {
                message = string.Empty;
                for (int i = 0; i != _message.Length; i++) {
                    char c = _message[i];
                    if (c == '\n' || c == '\r')
                        message += ' ';
                    else
                        message += _message[i];
                }
            }

            Text = message + " [" + _at + "]";
        }

        protected override void OnMouseHover(EventArgs e) {
            LockWindowUpdate(Parent.Handle);

            base.OnMouseHover(e);
            PerformMouseEnter();

            LockWindowUpdate(IntPtr.Zero);
        }

        public void PerformMouseEnter() {
            foreach (EventViewItem item in _parent.AllControls)
                item.PerformLeave();

            _defaultHeight = Height;
            MinimumSize = new Size(Width, 0);
            MaximumSize = new Size(Width, 0);

            BackColor = Color.Gainsboro;
            _entered = true;

            SetText();
        }

        private void PerformLeave() {
            if (_entered) {
                MinimumSize = new Size(Width, _defaultHeight);
                MaximumSize = new Size(Width, _defaultHeight);

                BackColor = Color.White;
                _entered = false;

                SetText();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Right)
                ClipboardWrapper.SetDataObject(Text);
        }
        #endregion
    }
}