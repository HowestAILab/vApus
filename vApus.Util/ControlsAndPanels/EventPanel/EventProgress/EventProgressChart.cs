/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    ///     Makes it able to show events in time while changing the value of the progress chart. You can let this behave as a progress bar also setting the BehaveAsBar property to true.
    /// </summary>
    public class EventProgressChart : Panel {

        #region Events
        [Description("Occurs when the cursor enters a progress event.")]
        public event EventHandler<ProgressEventEventArgs> EventMouseEnter;

        [Description("Occurs when the cursor leaves a progress event.")]
        public event EventHandler<ProgressEventEventArgs> EventMouseLeave;

        [Description("Occurs when a progress event is clicked.")]
        public event EventHandler<ProgressEventEventArgs> EventClick;
        #endregion

        #region Fields
        private readonly SolidBrush _brush = new SolidBrush(Color.LightSteelBlue);
        private readonly List<ChartProgressEvent> _progressEvents = new List<ChartProgressEvent>();
        private readonly List<KeyValuePair<Color, HashSet<ChartProgressEvent>>> _sortedProgressEvents = new List<KeyValuePair<Color, HashSet<ChartProgressEvent>>>();

        private readonly ToolTip toolTip = new ToolTip();

        private DateTime _beginOfTimeFrame = DateTime.MinValue, _endOfTimeFrame = DateTime.MaxValue;
        private bool _eventToolTip = true;

        /// <summary>
        ///     To set the progressbar to 'now'.
        /// </summary>
        private ChartProgressEvent _nowProgressEvent;

        private ChartProgressEvent _previouslyHovered;

        private bool _toolTipThisShown;

        private bool _behaveAsBar = false;
        #endregion

        #region Constructor
        /// <summary>
        ///     Makes it able to show events in time while changing the value of the progress chart.
        /// </summary>
        public EventProgressChart() {
            Height = 15;

            toolTip.UseAnimation = false;
            toolTip.UseFading = false;

            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
        }
        #endregion

        #region Properties

        [Description("Show or do not show a tool tip (label + \"\\n\" + at) when hovering a progress event.")]
        public bool EventToolTip {
            get { return _eventToolTip; }
            set { _eventToolTip = value; }
        }

        [Description("The count of all progress events.")]
        public int EventCount {
            get { return _progressEvents.Count; }
        }

        [Description("The begin of the time frame when the events occured ('at').")]
        /// </summary>
        public DateTime BeginOfTimeFrame {
            set {
                _beginOfTimeFrame = value;
                Invalidate();
            }
            get { return _beginOfTimeFrame; }
        }

        /// <summary>
        ///     It is set through SetProgressBarToNow().
        /// </summary>
        [Description("The end of the time frame.")]
        public DateTime EndOfTimeFrame {
            get { return _endOfTimeFrame; }
        }

        public Color ProgressBarColor {
            get { return _brush.Color; }
            set { _brush.Color = value; }
        }

        [DefaultValue(typeof(BorderStyle), "FixedSingle")]
        public new BorderStyle BorderStyle {
            get { return base.BorderStyle; }
            set { base.BorderStyle = value; }
        }

        /// <summary>
        /// If true, when the end of time frame is set the current time is remembered, thus creating a progress bar.
        /// (SetEndOfTimeFrameTo(DateTime))
        /// </summary>
        public bool BehaveAsBar {
            get { return _behaveAsBar; }
            set { _behaveAsBar = value; }
        }
        #endregion

        #region Functions
        /// <summary>
        ///     Always thinks the events are chronlologically ordered.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        /// <param name="at"></param>
        public ChartProgressEvent AddEvent(Color color, string message, DateTime at, bool refreshGui) {
            var pe = new ChartProgressEvent(this, color, message, at);
            pe.MouseEnter += pe_MouseEnter;
            pe.MouseLeave += pe_MouseLeave;
            pe.Click += pe_Click;

            _progressEvents.Add(pe);

            //Keep categorized for faster sorting.
            bool contains = false;
            foreach (var kvp in _sortedProgressEvents)
                if (kvp.Key == color) {
                    kvp.Value.Add(pe);

                    contains = true;
                    break;
                }
            if (!contains) {
                var kvp = new KeyValuePair<Color, HashSet<ChartProgressEvent>>(color, new HashSet<ChartProgressEvent>());
                kvp.Value.Add(pe);
                _sortedProgressEvents.Add(kvp);
            }

            if (refreshGui) Invalidate();


            return pe;
        }


        public List<ChartProgressEvent> GetEvents() {
            return _progressEvents;
        }

        public void ClearEvents() {
            _progressEvents.Clear();
            _sortedProgressEvents.Clear();
            Invalidate();
        }

        private void pe_MouseEnter(object sender, EventArgs e) { if (EventMouseEnter != null)  EventMouseEnter(this, new ProgressEventEventArgs(sender as ChartProgressEvent)); }

        private void pe_MouseLeave(object sender, EventArgs e) { if (EventMouseLeave != null)   EventMouseLeave(this, new ProgressEventEventArgs(sender as ChartProgressEvent)); }

        private void pe_Click(object sender, EventArgs e) { if (EventClick != null)   EventClick(this, new ProgressEventEventArgs(sender as ChartProgressEvent)); }

        protected override void OnPaint(PaintEventArgs e) {
            try {
                base.OnPaint(e);

                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;

                DrawBackground(g);
                ChartProgressEvent entered = null;

                //Make sure the most important events are drawn, hidden events won't be drawn.
                var Xs = new HashSet<int>();
                _sortedProgressEvents.Sort(ChartProgressEventComparer.GetInstance());

                foreach (var kvp in _sortedProgressEvents)
                    foreach (var pe in kvp.Value) {
                        if (pe.Entered) {
                            entered = pe;
                        } else {
                            int x = pe.X;
                            if (x < 1073741952 && !Xs.Contains(x)) { //Max value possible, Google it if you want
                                Xs.Add(x);
                                pe.Draw(g);
                            }
                        }
                    }
                Xs = null;
                if (entered != null)
                    entered.Draw(g); //Out of bounds check is also done in the fx
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed drawing the chart.", ex, new object[] { e });
            }
        }
        /// <summary>
        /// Fill a rectangle to the now progress event location.
        /// </summary>
        /// <param name="g"></param>
        private void DrawBackground(Graphics g) {
            ChartProgressEvent pe = null;
            if (_progressEvents.Count != 0) {
                pe = _progressEvents[_progressEvents.Count - 1];
                if (_nowProgressEvent == null || _nowProgressEvent.At < pe.At)
                    _nowProgressEvent = pe;
            }

            if (_nowProgressEvent != null)
                g.FillRectangle(_brush, 0, 0, _nowProgressEvent.X + ChartProgressEvent.WIDTH, Bounds.Height);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            ChartProgressEvent pe = GetHoveredProgressEvent();
            PerformMouseEnter(pe, _eventToolTip);
        }

        public void PerformMouseEnter(DateTime at, bool showToolTip) {
            foreach (var kvp in _sortedProgressEvents)
                foreach (ChartProgressEvent pe in kvp.Value)
                    if (pe.At == at) {
                        PerformMouseEnter(pe, showToolTip);
                        return;
                    }
        }

        private void PerformMouseEnter(ChartProgressEvent pe, bool showToolTip) {
            if (pe == null) {
                if (!_toolTipThisShown) {
                    if (_previouslyHovered != null)
                        PerformMouseLeave();

                    if (_beginOfTimeFrame != DateTime.MinValue) {
                        _toolTipThisShown = true;
                        toolTip.Show("Time frame: " + _beginOfTimeFrame + " - " + _endOfTimeFrame, this);
                    }
                }
            } else if (pe != _previouslyHovered) {
                _toolTipThisShown = false;
                PerformMouseLeave(false);

                pe.PerformMouseEnter(showToolTip ? toolTip : null);
                Invalidate();
            }

            _previouslyHovered = pe;
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            _toolTipThisShown = false;
            toolTip.Hide(this);
            PerformMouseLeave();
        }

        protected override void OnClick(EventArgs e) {
            base.OnClick(e);

            ChartProgressEvent pe = GetHoveredProgressEvent();
            if (pe != null)
                pe.PerformClick();
        }

        private ChartProgressEvent GetHoveredProgressEvent() {
            Point p = PointToClient(Cursor.Position);
            foreach (var kvp in _sortedProgressEvents)
                foreach (ChartProgressEvent pe in kvp.Value) {
                    Point location = new Point(pe.X, 0);
                    if (p.X >= location.X &&
                        p.X <= location.X + ChartProgressEvent.WIDTH)
                        return pe;
                }
            return null;
        }

        public void PerformMouseLeave(bool invalidate = true) {
            if (_previouslyHovered != null)
                _previouslyHovered.PerformMouseLeave();

            if (invalidate) Invalidate();
        }

        public void SetEndOfTimeFrameToNow() {
            _endOfTimeFrame = DateTime.Now;
            _nowProgressEvent = new ChartProgressEvent(this, Color.Transparent, string.Empty, _endOfTimeFrame);
        }
        public void SetEndOfTimeFrameTo(DateTime dateTime) {
            _endOfTimeFrame = dateTime;
            if (BehaveAsBar)
                _nowProgressEvent = new ChartProgressEvent(this, Color.Transparent, string.Empty, DateTime.Now);
            else
                _nowProgressEvent = new ChartProgressEvent(this, Color.Transparent, string.Empty, _endOfTimeFrame);
            Invalidate();
        }
        #endregion

        public class ProgressEventEventArgs : EventArgs {
            public readonly ChartProgressEvent ProgressEvent;
            public ProgressEventEventArgs(ChartProgressEvent progressEvent) { ProgressEvent = progressEvent; }
        }

        /// <summary>
        ///     Sort on color, the smallest counts first
        /// </summary>
        private class ChartProgressEventComparer : IComparer<KeyValuePair<Color, HashSet<ChartProgressEvent>>> {
            private static ChartProgressEventComparer _instance = new ChartProgressEventComparer();

            public static ChartProgressEventComparer GetInstance() { return _instance; }

            private ChartProgressEventComparer() { }
            public int Compare(KeyValuePair<Color, HashSet<ChartProgressEvent>> x, KeyValuePair<Color, HashSet<ChartProgressEvent>> y) {
                return x.Value.Count.CompareTo(y.Value.Count);
            }
        }

    }
}