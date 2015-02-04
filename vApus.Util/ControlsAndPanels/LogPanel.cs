/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using Newtonsoft.Json;
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;
using System.Xml;

namespace vApus.Util {
    public partial class LogPanel : Panel {
        private int _reporting = 0;
        private string[] _selectedLogEntries = { };

        public LogPanel() {
            InitializeComponent();
            NewIssue.Done += NewIssue_Done;
            this.ParentChanged += LogPanel_ParentChanged;
        }

        private void LogPanel_ParentChanged(object sender, EventArgs e) {
            if (Parent != null) {
                Form form = FindForm();
                if (form != null) {
                    this.ParentChanged -= LogPanel_ParentChanged;
                    tmr.Start();
                }
            }
        }
        
        private void NewIssue_Done(object sender, BackgroundWorkQueue.OnWorkItemProcessedEventArgs e) {
            if (e.Exception == null) {
                llblBug.Text = e.ReturnValue == null ? string.Empty : e.ReturnValue.ToString();
            } else {
                llblBug.Text = string.Empty;
                Loggers.Log(Level.Error, "Failed posting new issue.", e.Exception, new object[] { sender, e });
            }
            btnReport.Width = 72;
            btnReport.Text = "Report";

            --_reporting;
        }

        private void btnReport_Click(object sender, EventArgs e) {
            _reporting = _selectedLogEntries.Length;

            btnReport.Text = "Reporting...";
            btnReport.Width = 103;
            btnReport.Enabled = false;
            llblBug.Text = string.Empty;

            foreach (string json in _selectedLogEntries)
                NewIssue.Post(json);
        }

        private void llblBug_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(llblBug.Text);
        }

        private void tmr_Tick(object sender, EventArgs e) {
            if (IsHandleCreated && Visible && _reporting == 0) {
                _selectedLogEntries = fileLoggerPanel.SelectedLogEntries;
                btnReport.Enabled = _selectedLogEntries.Length != 0;
            }
        }

        public override string ToString() {
            return "Application logging";
        }

        /// <summary>
        /// Post a new issue to Redmine using the Post function.
        /// </summary>
        public static class NewIssue {
            public static event EventHandler<BackgroundWorkQueue.OnWorkItemProcessedEventArgs> Done;

            private delegate string PostDelegate(string applicationLogEntry);

            private static readonly PostDelegate _postDelegate;
            private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            /// <summary>
            /// Post a new issue to Redmine using the Post function. Used in the LogMessageDialog.
            /// </summary>
            static NewIssue() {
                _postDelegate = PostCallback;
                BackgroundWorkQueueWrapper.BackgroundWorkQueue.OnWorkItemProcessed += BackgroundWorkQueue_OnWorkItemProcessed;
            }

            private static void BackgroundWorkQueue_OnWorkItemProcessed(object sender, BackgroundWorkQueue.OnWorkItemProcessedEventArgs e) {
                if (Done != null)
                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate { Done(null, e); }, null);
            }

            /// <summary>
            ///     Posts bugs to redmine.sizingservers.be to the @Tickets project.
            ///     This wil throw an exception if any.
            /// </summary>
            /// <param name="applicationLogEntry"></param>
            /// <returns>Issue url</returns>
            public static void Post(string applicationLogEntry) {
                BackgroundWorkQueueWrapper.BackgroundWorkQueue.EnqueueWorkItem(_postDelegate, applicationLogEntry);
            }

            private static string PostCallback(string applicationLogEntry) {
                Entry entry = JsonConvert.DeserializeObject<Entry>(applicationLogEntry, _jsonSerializerSettings);

                string host = "redmine.sizingservers.be";
                string apiKey = "01bd5e9eb150a91e12641ba164014bc2c7245a64";

                var doc = new XmlDocument();
                XmlElement escape = doc.CreateElement("escape");
                escape.InnerText = entry.Level + " " + entry.Description;
                string subject = escape.InnerXml;

                escape.InnerText = applicationLogEntry;
                string description = escape.InnerXml;

                var httpWebRequest = WebRequest.Create(new Uri("http://" + host + "/issues.xml?key=" + apiKey)) as HttpWebRequest;
                httpWebRequest.UserAgent = "vApus";
                httpWebRequest.ServicePoint.Expect100Continue = true;

                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "text/xml";

                string postData = "<?xml version=\"1.0\"? encoding=\"UTF-8\"?><issue><subject>" + subject + 
                    "</subject><description>" + description + 
                    "</description><custom_fields type=\"array\"><custom_field name=\"Software version\" id=\"2\"><value>N/A</value></custom_field></custom_fields><project_id>18</project_id><tracker_id>1</tracker_id></issue>";

                ApplyPostData(httpWebRequest, postData);

                var httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;

                if (httpWebResponse.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Path not found to report to.");
                else if (httpWebResponse.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("You are not authorized to report bugs.");


                //Read the response and get the issue id
                string response, issueUrl = null;
                if (httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
                    using (var streamReader = new StreamReader(new GZipStream(httpWebResponse.GetResponseStream(), CompressionMode.Decompress)))
                        response = streamReader.ReadToEnd();
                else
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream(), (httpWebResponse.ContentEncoding.Length != 0)
                                                                ? System.Text.Encoding.GetEncoding(httpWebResponse.ContentEncoding) : System.Text.Encoding.GetEncoding(1252)))
                        response = streamReader.ReadToEnd();

                doc = new XmlDocument();
                doc.LoadXml(response);

                issueUrl = "http://" + host + "/issues/" + doc.ChildNodes[1].ChildNodes[0].InnerText;

                doc = null;
                return issueUrl;
            }
            private static void ApplyPostData(HttpWebRequest httpWebRequest, string postData) {
                httpWebRequest.ContentLength = postData.Length;
                Stream postStream = httpWebRequest.GetRequestStream();
                var postStreamWriter = new StreamWriter(postStream);
                postStreamWriter.Write(postData);
                postStreamWriter.Flush();

                postStreamWriter.Close();
                postStreamWriter.Dispose();
                postStreamWriter = null;

                postStream.Close();
            }
        }
    }
}
