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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    ///     Makes it able to show events in time while changing the value of the progress chart.
    /// </summary>
    public class EventProgressChart : Panel {

        [Description("Occurs when the cursor enters a progress event.")]
        public event EventHandler<ProgressEventEventArgs> EventMouseEnter;

        [Description("Occurs when the cursor leaves a progress event.")]
        public event EventHandler<ProgressEventEventArgs> EventMouseLeave;

        [Description("Occurs when a progress event is clicked.")]
        public event EventHandler<ProgressEventEventArgs> EventClick;

        #region Fields

        private readonly SolidBrush _brush = new SolidBrush(Color.LightSteelBlue);
        private readonly List<ChartProgressEvent> _progressEvents = new List<ChartProgressEvent>();

        private readonly ToolTip toolTip = new ToolTip();

        private DateTime _beginOfTimeFrame = DateTime.MinValue, _endOfTimeFrame = DateTime.MaxValue;
        private bool _eventToolTip = true;

        /// <summary>
        ///     To set the progressbar to 'now'.
        /// </summary>
        private ChartProgressEvent _nowProgressEvent;

        private ChartProgressEvent _previouslyHovered;

        private List<ChartProgressEvent> _sortedProgressEvents = new List<ChartProgressEvent>();
        //Sorted on importance, to draw the lines.

        private bool _toolTipThisShown;

        private bool _behaveAsBar = false;

        /// <summary>
        /// If true, when the end of time frame is set the current time is remembered, thus creating a progress bar.
        /// (SetEndOfTimeFrameTo(DateTime))
        /// </summary>
        public bool BehaveAsBar {
            get { return _behaveAsBar; }
            set { _behaveAsBar = value; }
        }

        #endregion

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

        #endregion

        public class ProgressEventEventArgs : EventArgs {
            public readonly ChartProgressEvent ProgressEvent;

            public ProgressEventEventArgs(ChartProgressEvent progressEvent) {
                ProgressEvent = progressEvent;
            }
        }

        private ChartProgressEvent GetEventAt(int index) {
            return _progressEvents[index];
        }

        /// <summary>
        ///     Always thinks the events are chronlologically ordered.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        /// <param name="at"></param>
        public ChartProgressEvent AddEvent(Color color, string message, DateTime at) {
            var pe = new ChartProgressEvent(this, color, message, at);
            pe.MouseEnter += pe_MouseEnter;
            pe.MouseLeave += pe_MouseLeave;
            pe.Click += pe_Click;
            _progressEvents.Add(pe);

            _sortedProgressEvents = Sort(_progressEvents);

            Invalidate();

            return pe;
        }

        /// <summary>
        ///     Sort on color, the smallest counts first
        /// </summary>
        /// <param name="progressEvents"></param>
        /// <returns></returns>
        private List<ChartProgressEvent> Sort(List<ChartProgressEvent> progressEvents) {
            var pes = new List<ChartProgressEvent>(progressEvents.Count);
            var sorter = new Dictionary<Color, List<ChartProgressEvent>>();

            foreach (ChartProgressEvent pe in progressEvents)
                if (sorter.ContainsKey(pe.Color)) {
                    sorter[pe.Color].Add(pe);
                } else {
                    var value = new List<ChartProgressEvent>();
                    value.Add(pe);
                    sorter.Add(pe.Color, value);
                }

            var sorted = new Dictionary<Color, List<ChartProgressEvent>>(sorter.Count);

            while (sorter.Count != 0) {
                Color smallestCount = Color.Empty;
                foreach (Color key in sorter.Keys)
                    if (smallestCount == Color.Empty)
                        smallestCount = key;
                    else if (sorter[key].Count < sorter[smallestCount].Count)
                        smallestCount = key;

                sorted.Add(smallestCount, sorter[smallestCount]);
                sorter.Remove(smallestCount);
            }

            foreach (Color key in sorted.Keys)
                pes.AddRange(sorted[key]);

            return pes;
        }

        public List<ChartProgressEvent> GetEvents() {
            return _progressEvents;
        }

        public void ClearEvents() {
            _progressEvents.Clear();
            Invalidate();
        }

        private void pe_MouseEnter(object sender, EventArgs e) {
            if (EventMouseEnter != null)
                EventMouseEnter(this, new ProgressEventEventArgs(sender as ChartProgressEvent));
        }

        private void pe_MouseLeave(object sender, EventArgs e) {
            if (EventMouseLeave != null)
                EventMouseLeave(this, new ProgressEventEventArgs(sender as ChartProgressEvent));
        }

        private void pe_Click(object sender, EventArgs e) {
            if (EventClick != null)
                EventClick(this, new ProgressEventEventArgs(sender as ChartProgressEvent));
        }

        protected override void OnPaint(PaintEventArgs e) {
            try {
                base.OnPaint(e);

                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;

                DrawBackground(g);
                ChartProgressEvent entered = null;

                //Do this in reversed order --> the most important are drawn last.
                for (int i = _sortedProgressEvents.Count - 1; i != -1; i--) {
                    ChartProgressEvent pe = _sortedProgressEvents[i];
                    if (pe.Entered)
                        entered = pe;
                    else
                        pe.Draw(g);
                }
                if (entered != null)
                    entered.Draw(g);
            } catch {
            }
        }
        /// <summary>
        /// Fill a rectangle to the now progress event location.
        /// </summary>
        /// <param name="g"></param>
        private void DrawBackground(Graphics g) {
            ChartProgressEvent pe = null;
            if (_sortedProgressEvents.Count != 0) {
                pe = _sortedProgressEvents[_sortedProgressEvents.Count - 1];
                if (_nowProgressEvent == null || _nowProgressEvent.At < pe.At)
                    _nowProgressEvent = pe;
            }

            if (_nowProgressEvent != null)
                g.FillRectangle(_brush, 0, 0, _nowProgressEvent.Location.X + ChartProgressEvent.WIDTH, Bounds.Height);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            ChartProgressEvent pe = GetHoveredProgressEvent();
            PerformMouseEnter(pe, _eventToolTip);
        }

        public void PerformMouseEnter(DateTime at, bool showToolTip) {
            foreach (ChartProgressEvent pe in _sortedProgressEvents)
                if (pe.At == at) {
                    PerformMouseEnter(pe, showToolTip);
                    break;
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
            foreach (ChartProgressEvent pe in _sortedProgressEvents) {
                Point location = pe.Location;
                if (p.X >= location.X &&
                    p.X <= location.X + ChartProgressEvent.WIDTH)
                    return pe;
            }
            return null;
        }

        public void PerformMouseLeave(bool invalidate = true) {
            if (_previouslyHovered != null)
                _previouslyHovered.PerformMouseLeave();

            if (invalidate)
                Invalidate();
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
    }
}