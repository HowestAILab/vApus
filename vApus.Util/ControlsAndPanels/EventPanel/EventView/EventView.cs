/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace vApus.Util {
    /// <summary>
    /// Can list events in a user friendly manner. Is encapsulated in the EventPanel together with the EventProgressChart.
    /// </summary>
    public partial class EventView : UserControl {

        #region Fields
        private List<EventPanelEvent> _events = new List<EventPanelEvent>();
        //private int _previousHeight, _previousWidth;

        private StringBuilder _sb = new StringBuilder();
        private System.Timers.Timer _tmr = new System.Timers.Timer(100);

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
            _tmr.Elapsed += _tmr_Elapsed;
            fctb.DefaultContextMenu(true);
        }

        private void _tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (_tmr != null)
                lock (_lock) {
                    _tmr.Stop();
                    fctb.AppendText(_sb.ToString());
                    _sb.Clear();
                }
        }

        #region Functions
        public void AddEvent(string message) { AddEvent(EventViewEventType.Info, message); }

        public void AddEvent(EventViewEventType eventType, string message) { AddEvent(eventType, message, DateTime.Now, Color.DarkGray); }

        public void AddEvent(EventViewEventType eventType, string message, DateTime at, Color color) {
            if (_tmr != null)
                lock (_lock) {
                    var item = new EventPanelEvent { EventType = eventType, Message = message, At = at };

                    _events.Add(item);

                    if (eventType >= Filter) {
                        _sb.AppendLine(item.ToString());
                        _tmr.Stop();
                        _tmr.Start();
                    }
                }
        }

        private void SetEvents() {
            lock (_lock) {
                _sb.Clear();
                foreach (var item in _events)
                    if (item.EventType >= Filter)
                        _sb.AppendLine(item.ToString());

                fctb.Text = _sb.ToString();
                _sb.Clear();
            }
        }

        public EventPanelEvent[] GetEvents() {
            lock (_lock) {
                return _events.ToArray();
            }
        }

        public void ClearEvents() {
            lock (_lock) {
                _events.Clear();
                fctb.Clear();
                _sb.Clear();
            }
        }

        public void PerformMouseEnter(DateTime at) {
            foreach (EventPanelEvent item in _events)
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
                foreach (char c in Text) {
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
                        lock (this) {
                            sw.WriteLine("Timestamp\tLevel\tMessage");
                            foreach (EventPanelEvent item in _events) {
                                sw.Write(item.At);
                                sw.Write("\t");
                                sw.Write((int)item.EventType);
                                sw.Write("\t");
                                sw.WriteLine(item.Message);
                            }
                        }
                        sw.Flush();
                    }
                }
                catch {
                    MessageBox.Show("Could not write to '" + sfd.FileName + "'!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }
        #endregion
    }
}