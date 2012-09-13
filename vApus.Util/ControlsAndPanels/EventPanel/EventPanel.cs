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

namespace vApus.Util
{
    public partial class EventPanel : UserControl
    {
        public event EventHandler CollapsedChanged;

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        private object _lock = new object();
        private int _preferredHeight = 300;

        private bool _expandOnErrorEvent;

        public EventPanel()
        {
            InitializeComponent();
            cboFilter.SelectedIndex = 0;
        }

        [Description("The begin of the time frame when the events occured ('at').")]
        /// </summary>
        public DateTime BeginOfTimeFrame
        {
            set { eventProgressBar.BeginOfTimeFrame = value; }
            get { return eventProgressBar.BeginOfTimeFrame; }
        }
        [Description("The end of the time frame.")]
        public DateTime EndOfTimeFrame
        {
            set { eventProgressBar.EndOfTimeFrame = value; }
            get { return eventProgressBar.EndOfTimeFrame; }
        }

        public Color ProgressBarColor
        {
            get { return eventProgressBar.ProgressBarColor; }
            set { eventProgressBar.ProgressBarColor = value; }
        }
        public int EventCount
        {
            get { return eventProgressBar.EventCount; }
        }
        [Description("Collapse or Expand.")]
        public bool Collapsed
        {
            get { return splitContainer.Panel2Collapsed; }
            set
            {
                if (splitContainer.Panel2Collapsed != value)
                {
                    splitContainer.Panel2Collapsed = value;
                    if (splitContainer.Panel2Collapsed)
                    {
                        btnCollapseExpand.Text = "+";
                        this.Height = 25;

                        this.MinimumSize = new Size(0, this.Height);
                        this.MaximumSize = new Size(int.MaxValue, this.Height);

                        eventProgressBar.Width += (cboFilter.Margin.Left + cboFilter.Width);
                        cboFilter.Visible = false;
                    }
                    else
                    {
                        btnCollapseExpand.Text = "-";
                        this.MinimumSize = this.DefaultMinimumSize;
                        this.MaximumSize = this.DefaultMaximumSize;

                        this.Height = _preferredHeight;

                        eventProgressBar.Width -= (cboFilter.Margin.Left + cboFilter.Width);
                        cboFilter.Visible = true;
                    }

                    if (CollapsedChanged != null)
                        CollapsedChanged(this, null);
                }
            }
        }
        public bool ExpandOnErrorEvent
        {
            get { return _expandOnErrorEvent; }
            set { _expandOnErrorEvent = value; }
        }

        [DefaultValue(typeof(EventViewEventType), "Info")]
        public EventViewEventType Filter
        {
            get { return (EventViewEventType)cboFilter.SelectedIndex; }
            set { cboFilter.SelectedIndex = (int)value; }
        }
        /// <summary>
        /// Thread safe.
        /// </summary>
        /// <returns></returns>
        public List<EventPanelEvent> GetEvents()
        {
            lock (_lock)
            {
                List<EventPanelEvent> l = new List<EventPanelEvent>(eventProgressBar.EventCount);

                var evEvents = eventView.GetEvents();
                var epbEvents = eventProgressBar.GetEvents();
                for (int i = 0; i != eventProgressBar.EventCount; i++)
                {
                    var type = evEvents[i].EventType;
                    var pe = epbEvents[i];
                    l.Add(new EventPanelEvent(type, pe.Color, pe.Message, pe.At));
                }
                return l;
            }
        }
        public void AddEvent(EventViewEventType eventType, Color eventPrograssBarEventColor, string message)
        {
            AddEvent(eventType, eventPrograssBarEventColor, message, DateTime.Now);
        }
        public void AddEvent(EventViewEventType eventType, Color eventPrograssBarEventColor, string message, DateTime at)
        {
            LockWindowUpdate(this.Handle.ToInt32());

            var pr = eventProgressBar.AddEvent(eventPrograssBarEventColor, message, at);
            var evi = eventView.AddEvent(eventType, message, at, eventType >= Filter);

            if (eventType == EventViewEventType.Error && eventView.UserEntered == null)
            {
                if (_expandOnErrorEvent)
                    Collapsed = false;

                eventProgressBar.PerformMouseEnter(at, false);
            }

            LockWindowUpdate(0);
        }
        public void SetProgressBarToNow()
        {
            eventProgressBar.SetProgressBarToNow();
        }
        public void ClearEvents()
        {
            eventProgressBar.ClearEvents();
            eventView.ClearEvents();
        }
        public void Export()
        {
            eventView.Export();
        }
        private void eventProgressBar_EventClick(object sender, EventProgressBar.ProgressEventEventArgs e)
        {
            ShowEvent(e.ProgressEvent.At);
        }
        /// <summary>
        /// Show event message at the right date time, use this if you have an external event progress bar.
        /// </summary>
        /// <param name="at"></param>
        public void ShowEvent(DateTime at)
        {
            Collapsed = false;
            eventView.PerformMouseEnter(at);
        }
        private void eventView_EventViewItemMouseEnter(object sender, EventView.EventViewItemEventArgs e)
        {
            eventProgressBar.PerformMouseEnter(e.EventViewItem.At, false);
        }

        private void eventView_EventViewItemMouseLeave(object sender, EventView.EventViewItemEventArgs e)
        {
            eventProgressBar.PerformMouseLeave();
        }

        private void btnCollapseExpand_Click(object sender, EventArgs e)
        {
            Collapsed = btnCollapseExpand.Text == "-";
        }

        private void EventPanel_Resize(object sender, EventArgs e)
        {
            if (!Collapsed)
                _preferredHeight = this.Height;
        }

        private void cboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            LockWindowUpdate(this.Handle.ToInt32());

            foreach (var evi in eventView.GetEvents())
                evi.Visible = evi.EventType >= Filter;

            eventView.PerformLargeListResize();

            LockWindowUpdate(0);

            this.Cursor = Cursors.Default;
        }
    }
}
