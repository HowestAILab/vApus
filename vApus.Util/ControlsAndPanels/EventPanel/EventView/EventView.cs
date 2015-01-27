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
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// Can list events in a user friendly manner. Is encapsulated in the EventPanel together with the EventProgressChart.
    /// </summary>
    public partial class EventView : UserControl {

        #region Events
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(IntPtr hWnd);

        [Description("Occurs when the cursor enters a progress event.")]
        public event EventHandler<EventViewItemEventArgs> EventViewItemMouseEnter;

        [Description("Occurs when the cursor leaves a progress event.")]
        public event EventHandler<EventViewItemEventArgs> EventViewItemMouseLeave;
        #endregion

        private EventViewItem _userEntered;
        private int _previousHeight, _previousWidth;

        private Size _sizeOfI;

        #region Properties
        public EventViewItem UserEntered { get { return _userEntered; } }

        public int ItemCount { get { return largeList.ControlCount; } }
        #endregion

        /// <summary>
        /// Can list events in a user friendly manner. Is encapsulated in the EventPanel together with the EventProgressBar.
        /// </summary>
        public EventView() { InitializeComponent(); }

        #region Functions
        public EventViewItem AddEvent(string message) {
            return AddEvent(EventViewEventType.Info, message);
        }

        public EventViewItem AddEvent(EventViewEventType eventType, string message) {
            return AddEvent(eventType, message, DateTime.Now);
        }

        public EventViewItem AddEvent(EventViewEventType eventType, string message, DateTime at, bool visible = true, bool performMouseEnter = true, bool refreshGui = true) {
            var item = new EventViewItem(largeList, eventType, message, at);
            item.Visible = visible;

            SetSize(item);

            largeList.Add(item, refreshGui);

            if (visible) {
                //Autoscroll if a user is not viewing a progress event and if the scrollbar is at the end.
                bool autoScroll = _userEntered == null && (largeList.CurrentView == largeList.ViewCount - 1 || largeList.ViewCount == 1);

                if (refreshGui && autoScroll)
                    largeList.ScrollIntoView(item);


                if (eventType == EventViewEventType.Error && _userEntered == null)
                    item.PerformMouseEnter();
            }

            item.MouseHover += item_MouseHover;
            item.MouseLeave += item_MouseLeave;

            return item;
        }

        private void SetSize(EventViewItem item) {
            LockWindowUpdate(Handle);

            int width = largeList.Width - largeList.Padding.Left - largeList.Padding.Right - item.Margin.Left -
                        item.Margin.Right - 21;

            if (_sizeOfI == null)
                _sizeOfI = TextRenderer.MeasureText("I", item.Font);
            int height = _sizeOfI.Height + item.Padding.Top + item.Padding.Bottom;

            item.MinimumSize = item.MaximumSize = new Size(width, height);

            LockWindowUpdate(IntPtr.Zero);
        }

        private void item_MouseHover(object sender, EventArgs e) {
            _userEntered = sender as EventViewItem;

            if (EventViewItemMouseEnter != null)
                EventViewItemMouseEnter(this, new EventViewItemEventArgs(sender as EventViewItem));
        }

        private void item_MouseLeave(object sender, EventArgs e) {
            _userEntered = null;

            if (EventViewItemMouseLeave != null)
                EventViewItemMouseLeave(this, new EventViewItemEventArgs(sender as EventViewItem));
        }

        public List<EventViewItem> GetEvents() {
            var l = new List<EventViewItem>(ItemCount);
            foreach (EventViewItem item in largeList.AllControls)
                l.Add(item);
            return l;
        }

        public void ClearEvents() {
            largeList.RemoveAll();
        }

        protected override void OnResize(EventArgs e) {
            if (this.Height == 0 || (this.Height == _previousHeight && this.Width == _previousWidth)) return;

            LockWindowUpdate(Handle);

            _previousHeight = Height;
            _previousWidth = this.Width;

            bool autoScroll = _userEntered == null &&
                              (largeList.CurrentView == largeList.ViewCount - 1 || largeList.ViewCount == 1);

            base.OnResize(e);

            foreach (EventViewItem item in largeList.AllControls)
                SetSize(item);

            largeList.RefreshControls();

            if (autoScroll && largeList.ControlCount != 0)
                largeList.ScrollIntoView(largeList[largeList.ViewCount - 1][largeList[largeList.ViewCount - 1].Count - 1]);

            LockWindowUpdate(IntPtr.Zero);
        }

        public void PerformLargeListResize() { largeList.PerformResize(true); }
        public void PerformMouseEnter(DateTime at) {
            EventViewItem item = null;
            foreach (EventViewItem evi in largeList.AllControls)
                if (evi.At == at) {
                    item = evi;
                    break;
                }

            if (item != null)
                PerformMouseEnter(item);
        }

        private void PerformMouseEnter(EventViewItem item) {
            LockWindowUpdate(Handle);

            item.PerformMouseEnter();
            largeList.ScrollIntoView(item);

            LockWindowUpdate(IntPtr.Zero);
        }

        public void Export() {
            if (sfd.ShowDialog() == DialogResult.OK)
                try {
                    using (var sw = new StreamWriter(sfd.FileName)) {
                        lock (this)
                            foreach (EventViewItem item in largeList.AllControls) {
                                sw.Write(item.Message);
                                sw.Write(" [");
                                sw.Write(item.At);
                                sw.WriteLine("]");
                            }
                        sw.Flush();
                    }
                } catch {
                    MessageBox.Show("Could not write to '" + sfd.FileName + "'!", string.Empty, MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
        }
        #endregion

        public class EventViewItemEventArgs : EventArgs {
            public readonly EventViewItem EventViewItem;

            public EventViewItemEventArgs(EventViewItem eventViewItem) {
                EventViewItem = eventViewItem;
            }
        }
    }
}