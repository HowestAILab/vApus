﻿/*
 * 2011 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util {
    public class ChartProgressEvent {
        public event EventHandler MouseEnter, MouseLeave, Click;

        #region Fields
        public const int WIDTH = 2;

        private readonly EventProgressChart _parent;
        private readonly Pen _selectedPen = new Pen(Color.Yellow, WIDTH);
        private DateTime _at;
        private bool _entered;

        private string _message;
        private Pen _pen;
        #endregion

        #region Constructors
        /// <summary>
        /// </summary>
        /// <param name="parent">Where it must be drawn upon.</param>
        /// <param name="color"></param>
        /// <param name="message"></param>
        /// <param name="at">The occured event at a certain date/time.</param>
        public ChartProgressEvent(EventProgressChart parent, Color color, string message, DateTime at) {
            _parent = parent;
            Color = color;
            _message = message;
            _at = at;
        }
        #endregion

        #region Properties
        public Color Color {
            get { return _pen == null ? Color.Transparent : _pen.Color; }
            set { _pen = new Pen(value, WIDTH); }
        }

        public string Message {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        ///     The occured event at a certain date/time.
        /// </summary>
        public DateTime At {
            get { return _at; }
            set { _at = value; }
        }

        /// <summary>
        ///     The shown tool tip text (if enabled in the parent).
        /// </summary>
        public string ToolTipText { get { return _message + "\n" + _at; } }

        /// <summary>
        ///     The location where it is drawn (only x changes).
        /// </summary>
      //  public Point Location { get { return new Point(X, 0); } }

        public int X {
            get {
                //Rule of 3
                double at = (_at - _parent.BeginOfTimeFrame).Ticks;
                long endOfTimeFrame = (_parent.EndOfTimeFrame - _parent.BeginOfTimeFrame).Ticks;

                var x = (int)((at / endOfTimeFrame) * _parent.Width);
                x -= (WIDTH / 2);

                //Correct out of bounds
                if (x < 0)
                    x = 0;
                else if (x + WIDTH > _parent.Width) {
                    x = _parent.Width - WIDTH;
                    if (_parent.BorderStyle == BorderStyle.None)
                        x -= 1; //-1 for the border
                }

                return x;
            }
        }

        public object Tag { get; set; }

        /// <summary>
        ///     Mouse over
        /// </summary>
        public bool Entered { get { return _entered; } }
        #endregion

        #region Functions
        /// <summary>
        ///     Draw at the calculated x (location)
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g) {
            try {
                int x = X;
                if (x < 1073741952) //Max value possible, Google it if you want
                    g.DrawLine(_entered ? _selectedPen : _pen, x, 0, x, _parent.Bounds.Height);
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed drawing line.", ex, new object[] { g, X, _parent.Bounds.Height });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="toolTip">Show e tool tip</param>
        public void PerformMouseEnter(ToolTip toolTip = null) {
            if (toolTip != null) {
                Point p = _parent.PointToClient(Cursor.Position);
                toolTip.Show(ToolTipText, _parent, p.X + 12, p.Y);
            }
            _entered = true;

            if (MouseEnter != null)
                MouseEnter(this, new EventArgs());
        }

        public void PerformMouseLeave() {
            _entered = false;

            if (MouseLeave != null)
                MouseLeave(this, new EventArgs());
        }

        public void PerformClick() { if (Click != null)  Click(this, new EventArgs()); }
        #endregion
    }
}