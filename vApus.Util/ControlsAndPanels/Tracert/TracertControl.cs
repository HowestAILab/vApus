﻿/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// Do do a trace route, same functionality as Windows CMD tool tracert.
    /// </summary>
    public partial class TracertControl : UserControl {
        /// <summary>
        ///     You can call SetToTrace here last minute.
        /// </summary>
        public event EventHandler BeforeTrace;
        public event EventHandler Done;

        #region Fields
        private readonly TracertDialog _tracertDialog = new TracertDialog();
        private int _hops;

        private string _hostNameOrIP;
        private IPStatus _lastStatus;
        private int _maxHops, _timeout;
        private Tracert _tracert;
        #endregion

        #region Constructor
        /// <summary>
        /// Do do a trace route, same functionality as Windows CMD tool tracert.
        /// </summary>
        public TracertControl() {
            InitializeComponent();
            _tracertDialog.CancelTraceRoute += _tracertDialog_CancelTraceRoute;
        }
        #endregion

        #region Functions
        public void SetToTrace(string hostNameOrIP, int maxHops = 100, int timeout = 5000) {
            _hostNameOrIP = hostNameOrIP;
            _maxHops = maxHops;
            _timeout = timeout;
        }

        private void _tracertDialog_CancelTraceRoute(object sender, EventArgs e) {
            btnStatus.Text = "Idle";
            btnStatus.BackColor = Color.Transparent;
            btnTraceRoute.Enabled = true;

            _tracert.Dispose();
            _tracert = null;

            if (Done != null)
                Done(this, null);
        }

        private void Trace() {
            _lastStatus = IPStatus.Unknown;
            _hops = 0;

            kvpHops.Key = "0 hops";
            kvpRoundtripTime.Key = "Roundtrip time N/A";
            btnStatus.Text = "Tracing...";
            btnStatus.BackColor = Color.LightBlue;

            _tracert = new Tracert();
            _tracert.Hop += _tracert_Hop;
            _tracert.Done += _tracert_Done;

            _tracert.Trace(_hostNameOrIP, _maxHops, _timeout);
        }

        private void btnTraceRoute_MouseDown(object sender, MouseEventArgs e) {
            if (BeforeTrace != null)
                BeforeTrace(this, null);
        }

        private void btnTraceRoute_Click(object sender, EventArgs e) {
            _tracertDialog.SetStarted();
            btnTraceRoute.Enabled = false;
            Trace();

            _tracertDialog.ShowDialog();
        }

        private void _tracert_Hop(object sender, Tracert.HopEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                kvpHops.Key = ((++_hops) == 1 ? "1 hop" : _hops + " hop");
                string roundtripTime = "N/A";
                if (e.Status == IPStatus.Success) {
                    TimeSpan ts = TimeSpan.FromMilliseconds(e.RoundTripTime);
                    roundtripTime = ts.ToShortFormattedString(false);
                    kvpRoundtripTime.Key = roundtripTime + " Roundtrip time";
                } else {
                    kvpRoundtripTime.Key = "Roundtrip time N/A";
                }
                _lastStatus = e.Status;

                _tracertDialog.AddHop(e.IP, e.HostName, roundtripTime);
            }, null);
        }

        private void _tracert_Done(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                if (_lastStatus == IPStatus.Success) {
                    btnStatus.Text = "Success...";
                    btnStatus.BackColor = Color.LightGreen;
                } else {
                    btnStatus.Text = "Failed...";
                    btnStatus.BackColor = Color.Orange;
                }
                btnTraceRoute.Enabled = true;

                _tracert.Dispose();
                _tracert = null;

                _tracertDialog.SetCompleted();

                if (Done != null)
                    Done(this, null);
            }, null);
        }

        private void btnStatus_Click(object sender, EventArgs e) {
            _tracertDialog.ShowDialog();
        }
        #endregion
    }
}