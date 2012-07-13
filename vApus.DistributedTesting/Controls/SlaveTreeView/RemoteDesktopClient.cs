/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Drawing;
using System.Windows.Forms;
using AxMSTSCLib;
using MSTSCLib;
using System.Threading;
using System;

namespace vApus.DistributedTesting
{
    public partial class RemoteDesktopClient : Form
    {
        public static event EventHandler<RdpExceptionEventArgs> RdpException;

        #region Fields
        private static RemoteDesktopClient _remoteDesktopClient;
        private Form _parentForm;
        private bool _canClose = false;
        #endregion

        #region Properties
        private Form __ParentForm
        {
            get { return _parentForm; }
            set
            {
                if (_parentForm != null)
                    _parentForm.FormClosing -= ParentForm_FormClosing;

                _parentForm = value;
                if (_parentForm == null)
                    _canClose = true;
                else
                    _parentForm.FormClosing += ParentForm_FormClosing;
            }
        }
        #endregion

        private RemoteDesktopClient()
        {
            InitializeComponent();
        }

        #region Functions
        private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _canClose = true;
            this.Close();
        }
        private void RemoteDesktopClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_canClose)
            {
                ClearRemoteDesktops();
            }
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        public static RemoteDesktopClient GetInstance(Form parentForm = null)
        {
            if (_remoteDesktopClient == null || _remoteDesktopClient.IsDisposed)
                _remoteDesktopClient = new RemoteDesktopClient();
            else
                _remoteDesktopClient.Hide();

            _remoteDesktopClient.__ParentForm = parentForm;
            return _remoteDesktopClient;
        }

        public void ShowRemoteDesktop(string hostName, string ip, string userName, string password, string domain)
        {
            TabPage tp = DoesRdcExist(ip);
            AxMsTscAxNotSafeForScripting rdc = null;
            if (tp == null)
            {
                tp = new TabPage(hostName);
                tp.Padding = new Padding(0);
                rdc = new AxMsTscAxNotSafeForScripting();
                rdc.OnLogonError += new AxMSTSCLib.IMsTscAxEvents_OnLogonErrorEventHandler(rdc_OnLogonError);
                rdc.OnFatalError += new AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEventHandler(rdc_OnFatalError);

                rdc.Dock = DockStyle.Fill;
                tp.Controls.Add(rdc);
                tc.TabPages.Add(tp);
            }
            else
            {
                rdc = tp.Controls[0] as AxMsTscAxNotSafeForScripting;
            }

            try
            {
                if (rdc.Connected.ToString() != "1")
                {
                    rdc.Server = ip;
                    rdc.Domain = domain;
                    rdc.UserName = userName;

                    rdc.DesktopWidth = 1280;
                    rdc.DesktopHeight = 800;

                    var comObject = (IMsRdpClient)rdc.GetOcx();
                    comObject.AdvancedSettings2.ClearTextPassword = password;

                    rdc.Connect();
                }
            }
            catch
            {
                tp.Text += " - Connection Failed";
            }

            tp.Select();
        }

        private void rdc_OnFatalError(object sender, IMsTscAxEvents_OnFatalErrorEvent e)
        {
            AxMsTscAxNotSafeForScripting rdc = sender as AxMsTscAxNotSafeForScripting;
            TabPage tp = rdc.Parent as TabPage;
            tp.Text += " - Connection Failed";

            if (RdpException != null)
                RdpException(this, new RdpExceptionEventArgs(rdc.Server, e.errorCode));
        }
        private void rdc_OnLogonError(object sender, IMsTscAxEvents_OnLogonErrorEvent e)
        {
            AxMsTscAxNotSafeForScripting rdc = sender as AxMsTscAxNotSafeForScripting;
            TabPage tp = rdc.Parent as TabPage;
            tp.Text += " - Connection Failed";

            if (RdpException != null)
                RdpException(this, new RdpExceptionEventArgs(rdc.Server, e.lError));
        }
        public TabPage DoesRdcExist(string ip)
        {
            foreach (TabPage tp in tc.TabPages)
            {
                AxMsTscAxNotSafeForScripting rdp = tp.Controls[0] as AxMsTscAxNotSafeForScripting;
                if (rdp.Server == ip)
                    return tp;
            }
            return null;
        }

        public void ClearRemoteDesktops()
        {
            try
            {
                foreach (TabPage tp in tc.TabPages)
                {
                    AxMsTscAxNotSafeForScripting rdp = tp.Controls[0] as AxMsTscAxNotSafeForScripting;
                    try
                    {
                        if (rdp.Connected.ToString() == "1")
                            rdp.Disconnect();
                    }
                    catch { }
                }
                tc.TabPages.Clear();
            }
            catch { }
        }

        private void tc_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TabPage tp = GetTabPage(e.Location);
                if (tp != null)
                {
                    RemoveRemoteDesktop(tp);

                    if (tc.TabPages.Count == 0)
                    {
                        _canClose = true;
                        this.Close();
                    }
                }
            }
        }
        private TabPage GetTabPage(Point mouseLocation)
        {
            for (int i = 0; i != tc.Controls.Count; i++)
                if (tc.GetTabRect(i).Contains(mouseLocation))
                    return tc.TabPages[i];
            return null;
        }
        private void RemoveRemoteDesktop(TabPage tabPage)
        {
            AxMsTscAxNotSafeForScripting rdp = tabPage.Controls[0] as AxMsTscAxNotSafeForScripting;
            try
            {
                if (rdp.Connected.ToString() == "1")
                    rdp.Disconnect();
            }
            catch { }
            tc.TabPages.Remove(tabPage);
        }
        #endregion

        public class RdpExceptionEventArgs : EventArgs
        {
            public readonly string IP;
            public readonly int ErrorCode;
            public RdpExceptionEventArgs(string ip, int errorCode)
            {
                IP = ip;
                ErrorCode = errorCode;
            }
        }
    }
}
