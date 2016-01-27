/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using vApus.Util;

namespace vApus.Publish {
    /// <summary>
    /// Do not forget to call Init() as early as possible to enable capturing application logging.
    /// </summary>
    public static class Publisher {
        private static Properties.Settings _settings = Properties.Settings.Default;
        private static bool _inited = false;

        /// <summary>
        /// Use these settings to determine if a value can be published, where you want to call Post(string id, object message).
        /// </summary>
        public static Properties.Settings Settings { get { return _settings; } }

        private static MulticastBlock _destinations = new MulticastBlock();

        /// <summary>
        /// Must be called as early as possible to enable capturing application logging.
        /// </summary>
        public static void Init() {
            if (!_inited) {
                Loggers.GetLogger<FileLogger>().LogEntryWritten += Publisher_LogEntryWritten;
                _inited = true;
            }
        }

        /// <summary>
        /// Will only post if Settings.PublisherEnabled.
        /// </summary>
        /// <param name="id">e.g. The tostring of a stress test. Does not have to be unique persee. Only useful for you own logic later on.</param>
        /// <param name="message"></param>
        /// <returns>The fully qualified id of the destination group. (one message to multiple destinations)</returns>
        public static string Post(string id, PublishItem message) {
            if (Settings.PublisherEnabled) {
                message.PublishItemId = id;

                string destinationGroupId;
                TryAddDestination(message, out destinationGroupId);

                _destinations.Post(destinationGroupId, message);

                return destinationGroupId;
            }
            return null;
        }

        private static bool TryAddDestination(PublishItem message, out string destinationGroupId) {
            destinationGroupId = message.PublishItemId.ReplaceInvalidWindowsFilenameChars('_').Replace(" ", "_") + "_" + message.PublishItemType + "_" + message.vApusPID;

            if (_destinations.Contains(destinationGroupId)) return false;

            if (_settings.TcpOutput && !string.IsNullOrWhiteSpace(_settings.TcpHost)) {
                var tcpDestination = TcpDestination.GetInstance();
                if (tcpDestination.Init(_settings.TcpHost, _settings.TcpPort)) {
                    tcpDestination.Formatter = new JSONFormatter();
                }
                _destinations.Add(destinationGroupId, tcpDestination);
            }
            if (_settings.UdpBroadcastOutput) {
                _destinations.Add(destinationGroupId, new UdpBroadcastDestination(_settings.UdpBroadcastPort) { Formatter = new JSONFormatter() });
            }
            return true;
        }

        internal static void Clear() { _destinations.Clear(); }

        private static void Publisher_LogEntryWritten(object sender, WriteLogEntryEventArgs e) {
            if (Settings.PublisherEnabled && Settings.PublishApplicationLogs && (ushort)e.Entry.Level >= Settings.LogLevel) {
                var publishItem = new ApplicationLogEntry();
                publishItem.Init();
                publishItem.Level = (int)e.Entry.Level;
                publishItem.Description = e.Entry.Description;
                publishItem.Exception = e.Entry.Exception.ToString();
                publishItem.Parameters = e.Entry.Parameters;
                publishItem.Member = e.Entry.Member;
                publishItem.SourceFile = e.Entry.SourceFile;
                publishItem.Line = e.Entry.Line;

                Post("vApus", publishItem);
            }
        }

        /// Wait until the post is idle. Can be handy for when posting stuff when the application gets closed.
        public static void WaitUntilIdle() { _destinations.WaitUntilIdle(); }
    }
}
