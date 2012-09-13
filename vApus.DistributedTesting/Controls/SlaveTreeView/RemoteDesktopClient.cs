/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using vApus.SolutionTree;
using System.Windows.Forms;

namespace vApus.DistributedTesting
{
    public partial class RemoteDesktopClient : BaseSolutionComponentView
    {
        public static event EventHandler<vApus.Util.RemoteDesktopClient.RdpExceptionEventArgs> RdpException;

        public RemoteDesktopClient()
        {
            InitializeComponent();
        }
        public RemoteDesktopClient(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();
        }

        #region Functions
        public void ShowRemoteDesktop(string hostName, string ip, string userName, string password, string domain)
        {
            rdc.ShowRemoteDesktop(hostName, ip, userName, password, domain);
        }

        public void ClearRemoteDesktops()
        {
            rdc.ClearRemoteDesktops();
        }
        private void rdc_AllConnectionsClosed(object sender, EventArgs e)
        {
            this.Close();
        }
        private void rdc_RdpException(object sender, Util.RemoteDesktopClient.RdpExceptionEventArgs e)
        {
            if (RdpException != null)
                RdpException(this, e);
        }
        #endregion
    }
}
