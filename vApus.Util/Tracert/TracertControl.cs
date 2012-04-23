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
        public event EventHandler Done;

        #region Fields
        private Tracert _tracert;
        private int _hops;
        private IPStatus _lastStatus;
        #endregion

        public TracertControl()
        {
            InitializeComponent();
        }

        #region Functions
        public void Trace(string hostNameOrIP, int maxHops = 100, int timeout = 5000)
        {
            kvpHops.Key = "? Hops";
            kvpRoundtripTime.Key = "? Roundtrip Time";
            btnStatus.Text = "Tracing...";
            btnStatus.BackColor = Color.LightBlue;

            _tracert = new Tracert();
            _tracert.TracedNode += new EventHandler<Tracert.TracedNodeEventArgs>(_tracert_TracedNode);
            _tracert.Done += new EventHandler(_tracert_Done);

            _tracert.Trace(hostNameOrIP, maxHops, timeout);
        }
        private void _tracert_TracedNode(object sender, Tracert.TracedNodeEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                kvpHops.Key = (++_hops) + " Hops";
                if (e.Status == IPStatus.Success)
                {
                    TimeSpan ts = TimeSpan.FromMilliseconds(e.RoundTripTime);
                    kvpRoundtripTime.Key = ts.ToShortFormattedString() + " Roundtrip Time";
                }
                else 
                {
                    kvpRoundtripTime.Key = "\\ Roundtrip Time";
                }
                _lastStatus = e.Status;
            });
        }
        private void _tracert_Done(object sender, EventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                if (_lastStatus == IPStatus.Success)
                {
                    btnStatus.Text = "Success...";
                    btnStatus.BackColor = Color.Green;
                }
                else
                {
                    btnStatus.Text = "Failed...";
                    btnStatus.BackColor = Color.Orange;
                }
            });
            _tracert.Dispose();
            _tracert = null;

            if (Done != null)
                Done(this, null);
        }
        #endregion
    }
}
