/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using vApus.SolutionTree;

namespace vApus.DistributedTesting {
    public partial class RemoteDesktopClient : BaseSolutionComponentView {
        public RemoteDesktopClient() {
            InitializeComponent();
        }

        public RemoteDesktopClient(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args) {
            InitializeComponent();
        }

        #region Functions

        public void ShowRemoteDesktop(string hostName, string ip, string userName, string password, string domain) {
            rdc.ShowRemoteDesktop(hostName, ip, userName, password, domain);
        }

        public void ClearRemoteDesktops() {
            rdc.ClearRemoteDesktops();
        }

        private void rdc_AllConnectionsClosed(object sender, EventArgs e) {
            Close();
        }

        private void rdc_RdpException(object sender, Util.RemoteDesktopClient.RdpExceptionEventArgs e) {
            if (RdpException != null)
                RdpException(this, e);
        }

        #endregion

        public static event EventHandler<Util.RemoteDesktopClient.RdpExceptionEventArgs> RdpException;
    }
}