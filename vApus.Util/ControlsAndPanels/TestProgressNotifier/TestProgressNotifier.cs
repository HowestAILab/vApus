/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using vApus.Util.Properties;

namespace vApus.Util {
    /// <summary>
    /// Notifies test progress.
    /// </summary>
    public static class TestProgressNotifier {
        public enum What {
            RunFinished = 0,
            ConcurrencyFinished,
            TestFinished
        }

        #region Fields
        internal const string PasswordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";
        internal static readonly byte[] Salt =
            {
                0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03,
                0x62
            };
        private static readonly object _lock = new object();
        private static readonly System.Timers.Timer _tmrNotify = new System.Timers.Timer(30000);
        private static ConcurrentBag<string> _messages = new ConcurrentBag<string>();
        #endregion

        static TestProgressNotifier() {
            _tmrNotify.Elapsed += _tmrNotify_Elapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="what">Used to check if a notification for this is wanted.</param>
        /// <param name="message"></param>
        /// <param name="vApusIP"></param>
        /// <param name="vApusPort"></param>
        public static void Notify(What what, string message, Exception exception = null) {
            if (vApus.Util.Properties.Settings.Default.PNEnabled) {
                if ((what == What.RunFinished && Settings.Default.PNAfterEachRun) || (what == What.ConcurrencyFinished && Settings.Default.PNAfterEachConcurrency) ||
                    (what == What.TestFinished && Settings.Default.PNWhenTheTestIsFinished))
                    Notify(message, exception);
            }
        }
        private static void Notify(string message, Exception exception = null) {
            _messages.Add(string.Concat(message, Environment.NewLine, exception));
            lock (_lock) {
                if (_tmrNotify != null) {
                    _tmrNotify.Stop();
                    _tmrNotify.Start();
                }
            }
        }
        static void _tmrNotify_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            lock (_lock) {
                try {
                    if (_tmrNotify != null) _tmrNotify.Stop();

                    if (_messages.Count == 0) return;

                    string message = _messages.ToArray().Combine("<br>");
                    _messages = new ConcurrentBag<string>();

                    var client = new SmtpClient(Settings.Default.PNSMTP, Settings.Default.PNPort);
                    client.EnableSsl = Settings.Default.PNSecure;
                    client.Timeout = 10000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    string username = Settings.Default.PNUsername;
                    if (!string.IsNullOrWhiteSpace(username)) username = Settings.Default.PNEMailAddress;

                    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(Settings.Default.PNPassword)) {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(Settings.Default.PNUsername, Settings.Default.PNPassword.Decrypt(PasswordGUID, Salt));
                    }

                    var msg = new MailMessage("vapus@sizingservers.be", Settings.Default.PNEMailAddress, string.Concat("vApus@", NamedObjectRegistrar.Get<string>("IP"), ":", NamedObjectRegistrar.Get<int>("Port"), " Test Progress Notification"), message);
                    msg.SubjectEncoding = msg.BodyEncoding = UTF8Encoding.UTF8;
                    msg.IsBodyHtml = true;
                    msg.Priority = MailPriority.High;
                    msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    client.Send(msg);
                } catch {
                    LogWrapper.LogByLevel("A progress nofification mail could not be sent, please check the settings.", LogLevel.Error);
                }
            }
        }
    }
}
