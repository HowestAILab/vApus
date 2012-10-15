/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace vApus.Util
{
    public partial class TracertControl : UserControl
    {
        /// <summary>
        /// You can call SetToTrace here last minute.
        /// </summary>
        public event EventHandler BeforeTrace;
        public event EventHandler Done;

        #region Fields
        private Tracert _tracert;
        private int _hops;
        private IPStatus _lastStatus;

        private string _hostNameOrIP;
        private int _maxHops, _timeout;

        private TracertDialog _tracertDialog = new TracertDialog();
        #endregion

        public TracertControl()
        {
            InitializeComponent();
            _tracertDialog.CancelTraceRoute += new EventHandler(_tracertDialog_CancelTraceRoute);
        }
        #region Functions
        public void SetToTrace(string hostNameOrIP, int maxHops = 100, int timeout = 5000)
        {
            _hostNameOrIP = hostNameOrIP;
            _maxHops = maxHops;
            _timeout = timeout;
        }
        private void _tracertDialog_CancelTraceRoute(object sender, EventArgs e)
        {
            btnStatus.Text = "Idle";
            btnStatus.BackColor = Color.Transparent;
            btnTraceRoute.Enabled = true;

            _tracert.Dispose();
            _tracert = null;

            if (Done != null)
                Done(this, null);
        }
        private void Trace()
        {
            _lastStatus = IPStatus.Unknown;
            _hops = 0;

            kvpHops.Key = "0 Hops";
            kvpRoundtripTime.Key = "Roundtrip Time N/A";
            btnStatus.Text = "Tracing...";
            btnStatus.BackColor = Color.LightBlue;

            _tracert = new Tracert();
            _tracert.Hop += new EventHandler<Tracert.HopEventArgs>(_tracert_Hop);
            _tracert.Done += new EventHandler(_tracert_Done);

            _tracert.Trace(_hostNameOrIP, _maxHops, _timeout);
        }
        private void btnTraceRoute_MouseDown(object sender, MouseEventArgs e)
        {
            if (BeforeTrace != null)
                BeforeTrace(this, null);
        }
        private void btnTraceRoute_Click(object sender, EventArgs e)
        {
            _tracertDialog.ClearHops();
            btnTraceRoute.Enabled = false;
            Trace();

            _tracertDialog.ShowDialog();
        }
        private void _tracert_Hop(object sender, Tracert.HopEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                kvpHops.Key = ((++_hops) == 1 ?  "1 Hop" : _hops + " Hop");
                string roundtripTime = "N/A";
                if (e.Status == IPStatus.Success)
                {
                    TimeSpan ts = TimeSpan.FromMilliseconds(e.RoundTripTime);
                    roundtripTime = ts.ToShortFormattedString();
                    kvpRoundtripTime.Key = roundtripTime + " Roundtrip Time";
                }
                else
                {
                    kvpRoundtripTime.Key = "Roundtrip Time N/A";
                }
                _lastStatus = e.Status;

                _tracertDialog.AddHop(e.IP, e.HostName, roundtripTime);
            }, null);
        }
        private void _tracert_Done(object sender, EventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                if (_lastStatus == IPStatus.Success)
                {
                    btnStatus.Text = "Success...";
                    btnStatus.BackColor = Color.LightGreen;
                }
                else
                {
                    btnStatus.Text = "Failed...";
                    btnStatus.BackColor = Color.Orange;
                }
                btnTraceRoute.Enabled = true;

                _tracert.Dispose();
                _tracert = null;

                if (Done != null)
                    Done(this, null);
            }, null);
        }

        private void btnStatus_Click(object sender, EventArgs e)
        {
            _tracertDialog.ShowDialog();
        }
        #endregion
    }
}
