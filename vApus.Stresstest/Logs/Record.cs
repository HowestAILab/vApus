/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using be.sizingservers.proxy;
using be.sizingservers.proxy.events;
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class Record : Form, ProxyListener
    {
        #region Fields
        private JProxy _proxy;

        private InputDialog _inputDialog = new InputDialog("Give the name of the action you want to put the last recorded entries in.", "Add Action");

        private string _startUnsortedTempLogPath;
        private List<string> _unsortedTempLogPaths;
        //Sort and save to this afterwards based on timestamp.
        private string _tempLogPath;

        private const int LUPUS_PROXY_PORT = 5555;

        private int _total, _lastTotalBeforeActionize, _ignored, _malformed, _discarded;

        private delegate void TestRequestThroughProxyDelegate();
        private TestRequestThroughProxyDelegate _testRequestThroughProxyDelegate;
        private AutoResetEvent _testRequestThroughProxyWaitHandle = new AutoResetEvent(false);
        private bool _testRequestGoneTroughProxy = false;

        #endregion

        /// <summary>
        /// The path where the recorded log file is.
        /// </summary>
        public string LogPath
        {
            get { return _tempLogPath; }
        }

        public Record()
        {
            InitializeComponent();
            _testRequestThroughProxyDelegate = TestRequestThroughProxy;

            _startUnsortedTempLogPath = Path.Combine(Application.StartupPath, "unsortedlupusproxytemplog");
            try
            {
                if (File.Exists(_startUnsortedTempLogPath))
                    File.Delete(_startUnsortedTempLogPath);
            }
            catch { }
            while (File.Exists(_startUnsortedTempLogPath))
                _startUnsortedTempLogPath += "_";

            _unsortedTempLogPaths = new List<string>();

            _tempLogPath = Path.Combine(Application.StartupPath, "lupusproxytemplog");
            try
            {
                if (File.Exists(LogPath))
                    File.Delete(LogPath);
            }
            catch { }
            while (File.Exists(LogPath))
                _tempLogPath += "_";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Note: Using Internet Explorer is recommended for recording. Please restart your browser if it is already running to ensure proper proxy configuration.\n\nWhen performing actions on a website, you can bundle the recorded requests into logical groups by using the \"User Action\"-button.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                try { ProxyHelper.UnsetProxy(); }
                catch { }

                try { JProxy.removeListener(this); }
                catch { }

                _proxy = new JProxy(LUPUS_PROXY_PORT);
                _proxy.setAnalyzeHTTPHeaders(true);

                pnlButtons.Enabled = false;

                btnStart.Enabled = false;
                pnlSettings.Enabled = false;
                chkActionize.Enabled = false;

                btnAddAction.Enabled = chkActionize.Checked;
                btnStop.Enabled = true;

                btnStart.BackColor = Color.Silver;
                btnStop.BackColor = Color.White;
                dgvIPsOrDomainNames.DefaultCellStyle.BackColor = dgvPorts.DefaultCellStyle.BackColor = Color.Silver;
                dgvIPsOrDomainNames.DefaultCellStyle.ForeColor = dgvPorts.DefaultCellStyle.ForeColor = Color.DimGray;
                dgvIPsOrDomainNames.ColumnHeadersDefaultCellStyle.ForeColor = dgvPorts.ColumnHeadersDefaultCellStyle.ForeColor = Color.DimGray;

                string ips = string.Empty;
                if (dgvIPsOrDomainNames.Rows.Count - 1 > 0)
                {
                    for (int i = 0; i < dgvIPsOrDomainNames.Rows.Count - 2; i++)
                        ips += dgvIPsOrDomainNames.Rows[i].Cells[0].Value.ToString() + ',';
                    ips += dgvIPsOrDomainNames.Rows[dgvIPsOrDomainNames.Rows.Count - 2].Cells[0].Value.ToString();
                    _proxy.setFixatedIps(ips);
                }

                string ports = string.Empty;
                if (dgvPorts.Rows.Count - 1 > 0)
                {
                    for (int i = 0; i < dgvPorts.Rows.Count - 2; i++)
                        ports += dgvPorts.Rows[i].Cells[0].Value.ToString() + ',';
                    ports += dgvPorts.Rows[dgvPorts.Rows.Count - 2].Cells[0].Value.ToString();
                    _proxy.setFixatedPorts(ports);
                }

                string path = _unsortedTempLogPaths.Count == 0 ? _startUnsortedTempLogPath
                    : _unsortedTempLogPaths[_unsortedTempLogPaths.Count - 1] + "_";

                while (File.Exists(path))
                    path += "_";

                _unsortedTempLogPaths.Add(path);

                _proxy.setFilename(path);
                _proxy.setActionized(chkActionize.Checked);

                JProxy.addListener(this);
                ProxyHelper.SetProxy("127.0.0.1:" + LUPUS_PROXY_PORT);

                _proxy.start();

                ActiveObject activeObject = new ActiveObject();
                activeObject.OnResult += new EventHandler<ActiveObject.OnResultEventArgs>(activeObject_OnResult);
                activeObject.Send(_testRequestThroughProxyDelegate);
            }
            catch (Exception ex)
            {
                Stop();
                string message = "Could not set the proxy!\n" + ex.ToString();
                LogWrapper.LogByLevel(message, LogLevel.Error);
            }
        }

        /// <summary>
        /// Will just give a warning if it does not work, it will not break the execution.
        /// </summary>
        private void TestRequestThroughProxy()
        {
            try
            {
                _proxy.paused = true;

                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    lblTestProxy.Text = "Testing the Proxy..  ";
                    lblTestProxy.Image = vApus.Stresstest.Properties.Resources.Wait;
                    lblTestProxy.Visible = true;
                }, null);

                IPAddress[] ipAddresses = null;
                try { ipAddresses = Dns.GetHostAddresses("www.google.com"); }
                catch { }

                Exception exception = null;

                //Test all ip's to see if any of them works.
                if (ipAddresses == null || ipAddresses.Length == 0)
                    exception = new Exception("Could not resolve 'www.google.com'.\nIf you are in a network that is not connected to the internet you don't need to worry. Otherwise, recording might or might not work.");
                else
                {
                    _testRequestGoneTroughProxy = false;
                    foreach (IPAddress ipAddress in ipAddresses)
                        try
                        {
                            TestRequestThroughProxy(ipAddress.ToString());
                            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                            {
                                try
                                {
                                    lblTestProxy.Image = null;
                                    lblTestProxy.Text = "OK";
                                }
                                catch { }
                            }, null);
                            //For the 'OK'
                            Thread.Sleep(500);
                            break;
                        }
                        catch (Exception e)
                        {
                            exception = new Exception("The test request did not succeed, the website could not be reached.", e);
                        }
                    if (exception == null && _testRequestGoneTroughProxy == false)
                        exception = new Exception("The test request did succeed, but it went not through the proxy that was set up for recording.\nDo you have another proxy running?");
                }
                if (exception != null)
                    throw exception;
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed)
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                    {
                        try
                        {
                            string message = "I tried to reach 'www.google.com' through the proxy and it did not work!\n\n" + ex.Message;
                            LogWrapper.LogByLevel((ex.InnerException == null ? message : message + "\n" + ex), LogLevel.Warning);

                            MessageBox.Show(this, message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        catch { }
                    }, null);
            }
        }
        private void TestRequestThroughProxy(string ip)
        {
            StreamReader sr = null;
            try
            {
                var request = (HttpWebRequest)System.Net.WebRequest.Create(new Uri("http://" + ip));
                request.UserAgent = "vApus - Test connection function";

                request.Timeout = 20000;
                request.ReadWriteTimeout = request.Timeout;
                request.AllowAutoRedirect = false;
                request.ServicePoint.Expect100Continue = false;

                request.ServicePoint.ConnectionLimit = 1;

                request.Method = "GET";
                request.ContentLength = 0;
                request.ContentType = "text/plain";

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                _testRequestThroughProxyWaitHandle.WaitOne(10000);


                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Page not found!");

                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    sr = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress));
                }
                else
                {
                    sr = new StreamReader(response.GetResponseStream(), (response.ContentEncoding.Length != 0) ?
                    System.Text.Encoding.GetEncoding(response.ContentEncoding) : System.Text.Encoding.GetEncoding(1252));
                }
            }
            catch { throw; }
            finally
            {
                if (sr != null)
                {
                    try
                    {
                        sr.Close();
                        sr.Dispose();
                    }
                    catch { }
                    sr = null;
                }
            }
        }
        private void activeObject_OnResult(object sender, ActiveObject.OnResultEventArgs e)
        {
            ActiveObject activeObject = sender as ActiveObject;
            activeObject.OnResult -= activeObject_OnResult;
            try { activeObject.Dispose(); }
            catch { }
            activeObject = null;

            if (!this.IsDisposed)
                try
                {
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                    {
                        try
                        {
                            lblTestProxy.Visible = false;
                            pnlButtons.Enabled = true;
                        }
                        catch { }
                    }, null);
                    _proxy.paused = false;
                }
                catch { }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }
        private void Stop()
        {
            try { ProxyHelper.UnsetProxy(); }
            catch { }

            try { JProxy.removeListener(this); }
            catch { }

            if (_proxy != null)
            {
                try { _proxy.close(); }
                catch { }
                _proxy = null;
            }
            _proxy = null;

            btnStart.Enabled = true;
            pnlSettings.Enabled = true;
            btnAddAction.Enabled = false;
            btnStop.Enabled = false;

            btnStart.BackColor = Color.White;
            btnStop.BackColor = Color.Silver;
            dgvIPsOrDomainNames.DefaultCellStyle.BackColor = dgvPorts.DefaultCellStyle.BackColor = Color.White;
            dgvIPsOrDomainNames.DefaultCellStyle.ForeColor = dgvPorts.DefaultCellStyle.ForeColor = Color.Black;
            dgvIPsOrDomainNames.ColumnHeadersDefaultCellStyle.ForeColor = dgvPorts.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = _total == 0 ? DialogResult.Cancel : DialogResult.OK;
            this.Close();
        }
        private void Record_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Cleanup
            try { _testRequestThroughProxyWaitHandle.Set(); }
            catch { }
            try { _testRequestThroughProxyWaitHandle.Dispose(); }
            catch { }
            _testRequestThroughProxyWaitHandle = null;

            try { ProxyHelper.UnsetProxy(); }
            catch { }

            try { JProxy.removeListener(this); }
            catch { }

            if (_proxy != null)
            {
                try { _proxy.close(); }
                catch { }
                _proxy = null;
            }

            //Combine all unsorted logs if the dialog result is OK.
            if (_unsortedTempLogPaths.Count != 0)
            {
                if (DialogResult == DialogResult.OK)
                {
                    string path = _unsortedTempLogPaths[_unsortedTempLogPaths.Count - 1] + "_";

                    while (File.Exists(path))
                        path += "_";

                    try
                    {
                        using (var sw = new StreamWriter(path))
                        {
                            foreach (string s in _unsortedTempLogPaths)
                                try
                                {
                                    using (var sr = new StreamReader(s))
                                        sw.Write(sr.ReadToEnd());
                                }
                                catch (Exception ex)
                                {
                                    string message = "Record HTTP: Could not access a temp log file, trying to copy from '" + s + "' to '" + path + "'.\n" + ex.ToString();
                                    LogWrapper.LogByLevel(message, LogLevel.Error);
                                }

                            sw.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = "Record HTTP: Could not access a temp log file:" + path + "\n" + ex.ToString();
                        LogWrapper.LogByLevel(message, LogLevel.Error);
                    }

                    //Sort the log
                    try
                    {
                        JProxy.sortLogByRequestTimestamp(path, _tempLogPath);
                    }
                    catch (Exception ex)
                    {
                        string message = "Record HTTP: Could not sort a temp log file:" + path + "\n" + ex.ToString();
                        LogWrapper.LogByLevel(message, LogLevel.Error);
                    }
                    finally
                    {
                        //Cleanup
                        try { if (File.Exists(path)) File.Delete(path); }
                        catch { }
                    }
                }
                else
                {
                    //Cleanup
                    try { if (File.Exists(_tempLogPath)) File.Delete(_tempLogPath); }
                    catch { }
                }

                //Cleanup
                foreach (string s in _unsortedTempLogPaths)
                    try { if (File.Exists(s)) File.Delete(s); }
                    catch { }
            }
        }
        private void btnAddAction_Click(object sender, EventArgs e)
        {
            _inputDialog.Input = string.Empty;

            _proxy.paused = true;

            if (_inputDialog.ShowDialog() == DialogResult.OK)
            {
                _proxy.setActionTitle(_inputDialog.Input);
                KeyValuePairControl kvp = new KeyValuePairControl(_inputDialog.Input, (_total - _lastTotalBeforeActionize).ToString());
                kvp.BackColor = Color.WhiteSmoke;
                flpLog.Controls.Add(kvp);

                _lastTotalBeforeActionize = _total;
                kvpInEmptyAction.Value = "0";
            }

            _proxy.paused = false;
        }

        private void dgvIPsOrDomainNames_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dgvIPsOrDomainNames.Rows[e.RowIndex].Cells[0];
            if (cell.Value == null)
            {
                try { dgvIPsOrDomainNames.Rows.RemoveAt(e.RowIndex); }
                catch { }
            }
            else
            {
                string value = cell.Value.ToString();
                if (value.ContainsChars('/'))
                {
                    if (value.StartsWith("http://"))
                        value = value.Substring("http://".Length);

                    cell.Value = value.Split('/')[0];
                }
            }
        }
        private void dgvPorts_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dgvPorts.Rows[e.RowIndex].Cells[0];

            int value;
            if (cell.Value == null || !int.TryParse(cell.Value.ToString(), out value))
                try { dgvPorts.Rows.RemoveAt(e.RowIndex); }
                catch { }
        }
        private void chkActionize_CheckedChanged(object sender, EventArgs e)
        {
            kvpInEmptyAction.Key = chkActionize.Checked ? "In empty action" : "Loose entries";
        }
        public void RequestFinished(RequestHappenedEvent rhe)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                //Check if the proxy is being tested, 
                //an ignored entry will get in here but we will set the wait handle and not the ignored kvp value (not logged anyways).
                if (lblTestProxy.Visible)
                {
                    try
                    {
                        _testRequestThroughProxyWaitHandle.Set();
                        _testRequestGoneTroughProxy = true;
                    }
                    catch { }
                }
                else
                {
                    if (rhe._statusCode == RequestHappenedEvent.StatusCode.Handled)
                    {
                        ++_total;
                        kvpTotalEntries.Value = _total.ToString();
                        kvpInEmptyAction.Value = (_total - _lastTotalBeforeActionize).ToString();
                    }
                    else if (rhe._statusCode == RequestHappenedEvent.StatusCode.Ignored)
                    {
                        ++_ignored;
                        kvpIgnored.Value = _ignored.ToString();
                    }
                    else if (rhe._statusCode == RequestHappenedEvent.StatusCode.Malformed)
                    {
                        ++_malformed;
                        kvpMalformed.Value = _malformed.ToString();

                        ++_total;
                        kvpTotalEntries.Value = _total.ToString();
                        kvpInEmptyAction.Value = (_total - _lastTotalBeforeActionize).ToString();
                    }
                    else if (rhe._statusCode == RequestHappenedEvent.StatusCode.Discarded)
                    {
                        ++_discarded;
                        kvpDiscarded.Value = _ignored.ToString();
                    }
                }
            }, null);
        }

        public void SetConfig(string[] ips, int[] ports)
        {
            dgvIPsOrDomainNames.Rows.Clear();
            foreach (string ip in ips)
                dgvIPsOrDomainNames.Rows.Add(ip);

            dgvPorts.Rows.Clear();
            foreach (int port in ports)
                dgvPorts.Rows.Add(port.ToString());
        }
        public void GetConfig(out string[] ips, out int[] ports)
        {
            ips = new string[dgvIPsOrDomainNames.Rows.Count - 1];
            for (int i = 0; i != ips.Length; i++)
                ips[i] = dgvIPsOrDomainNames.Rows[i].Cells[0].Value.ToString();

            ports = new int[dgvPorts.Rows.Count - 1];
            for (int i = 0; i != ports.Length; i++)
                ports[i] = int.Parse(dgvPorts.Rows[i].Cells[0].Value.ToString());
        }
    }
}