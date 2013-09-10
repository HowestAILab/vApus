/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml;

namespace vApus.Util {
    /// <summary>
    /// Post a new issue to Redmine using the Post function. Used in the LogMessageDialog.
    /// </summary>
    public static class NewIssue {
        private static readonly PostDelegate _postDelegate;

        /// <summary>
        /// Post a new issue to Redmine using the Post function. Used in the LogMessageDialog.
        /// </summary>
        static NewIssue() {
            _postDelegate = PostCallback;
            StaticActiveObjectWrapper.ActiveObject.OnResult += ActiveObject_OnResult;
        }

        public static event EventHandler<ActiveObject.OnResultEventArgs> Done;

        /// <summary>
        ///     Posts bugs to redmine.sizingservers.be to the @Tickets project.
        ///     This wil throw an exception if any.
        /// </summary>
        /// <param name="applicationLogEntry"></param>
        /// <returns>Issue url</returns>
        public static void Post(string applicationLogEntry) {
            StaticActiveObjectWrapper.ActiveObject.Send(_postDelegate, applicationLogEntry);
        }

        private static string PostCallback(string applicationLogEntry) {
            var statusCode = HttpStatusCode.OK;
            try {
                string host = "redmine.sizingservers.be";
                string apiKey = "a5a8cbd56a3e66e807b3c80009ca73ad81ebec6e"; //apiKey for the vapususer

                string[] split = applicationLogEntry.Replace("\r\n", "\n").Replace("\n\r", "\n").Split('\n');

                var doc = new XmlDocument();
                XmlElement escape = doc.CreateElement("escape");
                escape.InnerText = split[0];
                string subject = escape.InnerXml;

                escape.InnerText = applicationLogEntry.Substring(split[0].Length).TrimStart();
                string description = escape.InnerXml;

                var httpWebRequest =
                    (HttpWebRequest)WebRequest.Create(new Uri("http://" + host + "/issues.xml?key=" + apiKey));
                httpWebRequest.UserAgent = "vApus v2 - Test connection function";
                httpWebRequest.ServicePoint.Expect100Continue = true;

                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "text/xml";

                string postData = "<?xml version=\"1.0\"? encoding=\"UTF-8\"?>" +
                                  "<issue>" +
                                  "<subject>" + subject + "</subject>" +
                                  "<description>" + description + "</description>" +
                                  "<custom_fields type=\"array\">" +
                                  "<custom_field name=\"Software version\" id=\"2\">" +
                                  "<value>N/A</value>" +
                                  "</custom_field>" +
                                  "</custom_fields>" +
                                  "<project_id>18</project_id>" +
                                  "<tracker_id>1</tracker_id>" +
                                  "</issue>";

                ApplyPostData(httpWebRequest, postData);
                var httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;


                statusCode = httpWebResponse.StatusCode;
                if (statusCode == HttpStatusCode.NotFound)
                    throw new Exception();
                else if (statusCode == HttpStatusCode.Unauthorized)
                    throw new Exception(
                        "You are not authorized to report bugs, please contact Sizing Servers about this problem.");

                //Read the response and get the issue id
                string response, issueUrl = null;
                if (httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
                    using (
                        var streamReader =
                            new StreamReader(new GZipStream(httpWebResponse.GetResponseStream(),
                                                            CompressionMode.Decompress))) {
                        response = streamReader.ReadToEnd();
                    } else
                    using (
                        var streamReader = new StreamReader(httpWebResponse.GetResponseStream(),
                                                            (httpWebResponse.ContentEncoding.Length != 0)
                                                                ? System.Text.Encoding.GetEncoding(
                                                                    httpWebResponse.ContentEncoding)
                                                                : System.Text.Encoding.GetEncoding(1252))) {
                        response = streamReader.ReadToEnd();
                    }
                doc = new XmlDocument();
                doc.LoadXml(response);

                issueUrl = "http://" + host + "/issues/" + doc.ChildNodes[1].ChildNodes[0].InnerText;

                return issueUrl;
            } catch {
                if (statusCode == HttpStatusCode.Unauthorized)
                    throw;
                throw new Exception("The bug report server could not be found, are you connected to the internet?");
            }
        }

        private static void ActiveObject_OnResult(object sender, ActiveObject.OnResultEventArgs e) {
            if (Done != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate { Done(null, e); }, null);
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

        private delegate string PostDelegate(string applicationLogEntry);
    }
}