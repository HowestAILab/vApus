/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// Encapsulates EventProgressChar and EventView.
    /// </summary>
    public partial class EventPanel : UserControl {

        #region Static
        //All this to be able to add events from anywhere (for example the connection proxy code) for debugging purposes.
        private static readonly object _staticLock = new object();
        private static List<EventPanel> _eventPanels = new List<EventPanel>();

        private static volatile bool _cancelAddingStaticEvents;

        private static void CleanEventPanels() {
            var l = new List<EventPanel>();
            foreach (var ep in _eventPanels)
                if (ep != null && !ep.IsDisposed)
                    l.Add(ep);
            _eventPanels = l;
        }
        private static void RegisterEventPanel(EventPanel eventPanel) {
            lock (_staticLock) {
                if (!_eventPanels.Contains(eventPanel))
                    _eventPanels.Add(eventPanel);
                CleanEventPanels();
            }
        }

        /// <summary>
        /// This allows us to add events from within the connection proxy code, events will be added to all available event panels.
        /// </summary>
        /// <param name="message"></param>
        public static void AddEvent(string message) {
            if (_cancelAddingStaticEvents) return;

            lock (_staticLock) {
                CleanEventPanels();

                try {
                    SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                        foreach (var ep in _eventPanels)
                            ep.AddEvent(EventViewEventType.Info, Color.Black, message);
                    }, null);
                }
                catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed to add events to an event panel from within connection proxy code.", ex, new object[] { message });
                }
            }
        }
        #endregion

        public event EventHandler CollapsedChanged;

        #region Fields
        private readonly object _lock = new object();

        private bool _expandOnErrorEvent;
        private int _preferredHeight = 150;
        #endregion

        #region Properties
        [Description("The begin of the time frame when the events occured ('at').")]
        /// </summary>
        public DateTime BeginOfTimeFrame {
            set { eventProgressBar.BeginOfTimeFrame = value; }
            get { return eventProgressBar.BeginOfTimeFrame; }
        }

        /// <summary>
        ///     It is set through SetProgressBarToNow().
        /// </summary>
        [Description("The end of the time frame.")]
        public DateTime EndOfTimeFrame {
            get { return eventProgressBar.EndOfTimeFrame; }
        }

        public Color ProgressBarColor {
            get { return eventProgressBar.ProgressBarColor; }
            set { eventProgressBar.ProgressBarColor = value; }
        }

        public int EventCount {
            get { return eventProgressBar.EventCount; }
        }

        [Description("Collapse or Expand.")]
        public bool Collapsed {
            get { return splitContainer.Panel2Collapsed; }
            set {
                if (splitContainer.Panel2Collapsed != value) {
                    splitContainer.Panel2Collapsed = value;
                    if (splitContainer.Panel2Collapsed) {
                        btnCollapseExpand.Text = "+";
                        Height = 25;

                        MinimumSize = new Size(0, Height);
                        MaximumSize = new Size(int.MaxValue, Height);

                        eventProgressBar.Width += (cboFilter.Margin.Left + cboFilter.Width);
                        cboFilter.Visible = false;
                    }
                    else {
                        btnCollapseExpand.Text = "-";
                        MinimumSize = DefaultMinimumSize;
                        MaximumSize = DefaultMaximumSize;

                        Height = _preferredHeight;

                        eventProgressBar.Width -= (cboFilter.Margin.Left + cboFilter.Width);
                        cboFilter.Visible = true;
                    }

                    if (CollapsedChanged != null)
                        CollapsedChanged(this, null);
                }
            }
        }

        public bool ExpandOnErrorEvent {
            get { return _expandOnErrorEvent; }
            set { _expandOnErrorEvent = value; }
        }

        [DefaultValue(typeof(EventViewEventType), "Info")]
        public EventViewEventType Filter {
            get { return (EventViewEventType)cboFilter.SelectedIndex; }
            set {
                cboFilter.SelectedIndex = (int)value;
                eventView.Filter = value;
            }
        }
        #endregion

        #region Constructor
        public EventPanel() {
            InitializeComponent();
            Filter = EventViewEventType.Info;

            RegisterEventPanel(this);
        }
        #endregion

        #region Functions
        /// <summary>
        ///     Thread safe.
        /// </summary>
        /// <returns></returns>
        public EventPanelEvent[] GetEvents() {
            lock (_lock) {
                int tried = 0;
                Retry:
                try {
                    return eventView.GetEvents();
                }
                catch {
                    if (++tried != 3) {
                        Thread.Sleep(tried * 100);
                        goto Retry;
                    }
                }
                return new EventPanelEvent[0];
            }
        }

        public void AddEvent(EventViewEventType eventType, Color eventPrograssBarEventColor, string message) {
            AddEvent(eventType, eventPrograssBarEventColor, message, DateTime.Now);
        }

        public void AddEvent(EventViewEventType eventType, Color eventPrograssBarEventColor, string message, DateTime at) {
            lock (_lock)
                AddEvent(eventType, eventPrograssBarEventColor, message, at, true);
        }
        private void AddEvent(EventViewEventType eventType, Color eventPrograssBarEventColor, string message, DateTime at, bool refreshGui) {
            if (eventType > EventViewEventType.Info)
                eventProgressBar.AddEvent(eventPrograssBarEventColor, message, at, refreshGui);
            eventView.AddEvent(eventType, message, at, eventPrograssBarEventColor);


            if (eventType == EventViewEventType.Error && _expandOnErrorEvent) {
                Collapsed = false;

                eventProgressBar.PerformMouseEnter(at, false);
            }
        }
        public void AddEvents(List<EventPanelEvent> events) {
            lock (_lock) {
                int count = events.Count;
                if (count != 0) {
                    EventPanelEvent epe;
                    for (int i = 0; i < count - 1; i++) {
                        epe = events[i];
                        AddEvent(epe.EventType, epe.EventProgressBarEventColor, epe.Message, epe.At, false);
                    }
                    epe = events[count - 1];
                    AddEvent(epe.EventType, epe.EventProgressBarEventColor, epe.Message, epe.At, true);
                }
            }
        }
        public void SetEvents(EventPanelEvent[] events) {
            lock (_lock) {
                ClearEvents();

                if (events.Length != 0) {
                    EventPanelEvent epe;
                    for (int i = 0; i < events.Length - 1; i++) {
                        epe = events[i];
                        AddEvent(epe.EventType, epe.EventProgressBarEventColor, epe.Message, epe.At, false);
                    }
                    epe = events[events.Length - 1];
                    AddEvent(epe.EventType, epe.EventProgressBarEventColor, epe.Message, epe.At, true);
                }
            }
        }

        public void SetEndOfTimeFrameToNow() {
            eventProgressBar.SetEndOfTimeFrameToNow();
        }
        public void SetEndOfTimeFrameTo(DateTime dateTime) {
            if (eventProgressBar.EndOfTimeFrame != dateTime) eventProgressBar.SetEndOfTimeFrameTo(dateTime);
        }
        public void ClearEvents() {
            _cancelAddingStaticEvents = false;
            eventProgressBar.ClearEvents();
            eventView.ClearEvents();
        }

        /// <summary>
        /// Call ClearEvents() to allow events to be added again.
        /// </summary>
        public void CancelAddingStaticEventsToGui() {
            _cancelAddingStaticEvents = true;
            eventProgressBar.CancelAddingEventsToGui();
            eventView.CancelAddingEventsToGui();
        }

        public void Export() {
            eventView.Export();
        }

        private void eventProgressBar_EventClick(object sender, EventProgressChart.ProgressEventEventArgs e) {
            ShowEvent(e.ProgressEvent.At);
        }

        /// <summary>
        ///     Show / scroll to event message at the right date time, use this if you have an external event progress bar.
        /// </summary>
        /// <param name="at"></param>
        public void ShowEvent(DateTime at) {
            Collapsed = false;
            eventView.PerformMouseEnter(at);
        }

        private void btnCollapseExpand_Click(object sender, EventArgs e) {
            Collapsed = btnCollapseExpand.Text == "-";
        }

        private void EventPanel_Resize(object sender, EventArgs e) {
            if (!Collapsed) _preferredHeight = Height;
        }

        private void cboFilter_SelectedIndexChanged(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            Filter = (EventViewEventType)cboFilter.SelectedIndex;
            Cursor = Cursors.Default;
        }
        #endregion
    }
}