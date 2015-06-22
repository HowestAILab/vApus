/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using RandomUtils;
using RandomUtils.Log;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Monitor.Sources.Base;

namespace vApus.Monitor {
    public partial class BandwidthDialog : Form {
        private BaseSocketClient<string> _monitorSourceClient;

        /// <summary>
        /// Design time constructor.
        /// </summary>
        public BandwidthDialog() {
            InitializeComponent();
        }

        public BandwidthDialog(BaseSocketClient<string> monitorSourceClient)
            : this() {
            _monitorSourceClient = monitorSourceClient;
            this.HandleCreated += BandwidthDialog_HandleCreated;
        }

        async private void BandwidthDialog_HandleCreated(object sender, EventArgs e) {
            lblDown.Text = "Download speed: ...";
            lblUp.Text = "Upload speed: ...";

            await Task.Run(() => {
                try {
                    this.HandleCreated -= BandwidthDialog_HandleCreated;
                    double downloadSpeedInMbps, uploadSpeedInMbps;
                    _monitorSourceClient.TestBandwidth(out downloadSpeedInMbps, out uploadSpeedInMbps);

                    if (downloadSpeedInMbps == -1 || uploadSpeedInMbps == -1) throw new Exception("Download or upload speed returned -1.");

                    SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                        lblWait.Visible = false;
                        lblDown.Text = "Download speed: " + Math.Round(downloadSpeedInMbps, 0, MidpointRounding.AwayFromZero) + " Mbps";
                        lblUp.Text = "Upload speed: " + Math.Round(uploadSpeedInMbps, 0, MidpointRounding.AwayFromZero) + " Mbps";
                    }, null);
                } catch (Exception ex) {
                    SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                        lblWait.Visible = false;
                        lblDown.Text = "Download speed: ? Mbps";
                        lblUp.Text = "Upload speed: ? Mbps";

                        Loggers.Log(Level.Error, "Determining the bandwidth failed.", ex);
                    }, null);
                }
            });
        }

        private void btnClose_Click(object sender, EventArgs e) {
            _monitorSourceClient.Disconnect();
            _monitorSourceClient.Connect();
            this.Close();
        }
    }
}
