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
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// Can list events in a user friendly manner. Is encapsulated in the EventPanel together with the EventProgressChart.
    /// </summary>
    public partial class EventView : UserControl {

        #region Fields
        private List<EventPanelEvent> _events = new List<EventPanelEvent>();

        private Queue<string> _backlog = new Queue<string>();
        private Timer _tmr = new Timer() { Interval = 10 };

        private readonly object _lock = new object();

        private EventViewEventType _filter = EventViewEventType.Info;
        #endregion

        #region Properties

        public EventViewEventType Filter {
            get {
                return _filter;
            }
            set {
                _filter = value;
                SetEvents();
            }
        }

        #endregion

        /// <summary>
        /// Can list events in a user friendly manner. Is encapsulated in the EventPanel together with the EventProgressBar.
        /// </summary>
        public EventView() {
            InitializeComponent();
            _tmr.Tick += _tmr_Tick;
            fctb.DefaultContextMenu(true);
        }

        private void _tmr_Tick(object sender, EventArgs e) {
            lock (_lock)
                if (_tmr != null) {
                    _tmr.Stop();

                    bool startTimer = false;
                    int count = _backlog.Count;
                    if (count > 1000) {
                        count = 1000;
                        startTimer = true;
                    }

                    var sb = new StringBuilder();
                    for (int i = 0; i != count; i++) sb.AppendLine(_backlog.Dequeue());

                    fctb.AppendText(sb.ToString());
                    fctb.VerticalScroll.Value = fctb.VerticalScroll.Maximum;

                    if (startTimer) _tmr.Start();
                }
        }

        #region Functions
        public void AddEvent(string message) { AddEvent(EventViewEventType.Info, message); }

        public void AddEvent(EventViewEventType eventType, string message) { AddEvent(eventType, message, DateTime.Now, Color.DarkGray); }

        public void AddEvent(EventViewEventType eventType, string message, DateTime at, Color color) {
            lock (_lock)
                if (_tmr != null) {
                    var item = new EventPanelEvent { EventType = eventType, Message = message, At = at };

                    _events.Add(item);

                    if (eventType >= Filter) {
                        _tmr.Stop();
                        _backlog.Enqueue(item.ToString());
                        _tmr.Start();
                    }
                }
        }

        public void CancelAddingEventsToGui() {
            if (_tmr != null)
                _tmr.Stop();
        }


        private void SetEvents() {
            lock (_lock)
                if (_tmr != null) {
                    _tmr.Stop();
                    fctb.Clear();

                    _backlog.Clear();
                    foreach (var item in _events)
                        if (item.EventType >= Filter)
                            _backlog.Enqueue(item.ToString());

                    _tmr.Start();
                }
        }

        public EventPanelEvent[] GetEvents() { lock (_lock) return _events.ToArray(); }

        public void ClearEvents() {
            lock (_lock) {
                if (_tmr != null)
                    _tmr.Stop();

                _events.Clear();
                fctb.Clear();
                _backlog.Clear();
            }
        }

        public void PerformMouseEnter(DateTime at) {
            var events = GetEvents();
            foreach (EventPanelEvent item in events)
                if (item.At == at) {
                    List<int> lines = fctb.FindLines("(\\b" + Regex.Escape(item.At.ToString("yyyy'-'MM'-'dd HH':'mm':'ss") + " " + item.EventType) + "\\b)", RegexOptions.Singleline);

                    if (lines.Count != 0)
                        SelectLine(lines[0]);
                    break;
                }
        }

        private void SelectLine(int lineNumber) {
            if (lineNumber < fctb.LinesCount) {
                int line = 0, start = 0, stop = 0;
                foreach (char c in fctb.Text) {
                    ++stop;
                    if (line < lineNumber)
                        ++start;
                    if (c == '\n' && ++line >= lineNumber && stop - start > 0)
                        break;
                }

                fctb.SelectionStart = start;
                fctb.SelectionLength = stop - start;

                fctb.DoSelectionVisible();
            }
        }
        public void Export() {
            if (sfd.ShowDialog() == DialogResult.OK)
                try {
                    using (var sw = new StreamWriter(sfd.FileName)) {
                        var events = GetEvents();

                        sw.WriteLine("Timestamp\tLevel\tMessage");
                        foreach (EventPanelEvent item in events) {
                            sw.Write(item.At);
                            sw.Write("\t");
                            sw.Write(item.EventType);
                            sw.Write("\t");
                            sw.WriteLine(item.Message);
                        }
                        sw.Flush();
                    }
                }
                catch {
                    Loggers.Log(Level.Error, "Could not export event panel messages to '" + sfd.FileName + "'!");
                }
        }
        #endregion
    }
}