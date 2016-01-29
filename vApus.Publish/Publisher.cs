/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System.Diagnostics;
using System.Net;
using vApus.Util;

namespace vApus.Publish {
    /// <summary>
    /// Do not forget to call Init() as early as possible to enable capturing application logging.
    /// </summary>
    public static class Publisher {
        private static Properties.Settings _settings = Properties.Settings.Default;
        private static bool _inited = false;
        private static readonly object _lock = new object();

        private static MulticastBlock _destinations = new MulticastBlock();

        private static string _lastGeneratedResultSetId;

        /// <summary>
        /// Use these settings to determine if a value can be published, where you want to call Post(string id, object message).
        /// </summary>
        public static Properties.Settings Settings { get { return _settings; } }

        /// <summary>
        /// Set should only be used for a slave --> a master generated the result set id.
        /// </summary>
        public static string LastGeneratedResultSetId {
            get { lock (_lock) return _lastGeneratedResultSetId; }
            set { lock (_lock) _lastGeneratedResultSetId = value; }
        }

        /// <summary>
        /// <para>Call this when a Test or a standalone monitor is started.</para>
        /// <para>Use this to generate a result set id. It will be set on Post(...) to the publish item.</para>
        /// <para>This value is used to link all publish items that belong to one another. </para>
        /// <para>Call ClearResultSetId when a test or a monitor is stopped.</para>
        /// </summary>
        /// <returns></returns>
        public static string GenerateResultSetId() {
            lock (_lock) {
                _lastGeneratedResultSetId = Dns.GetHostEntry(IPAddress.Loopback).HostName + Process.GetCurrentProcess().Id + HighResolutionDateTime.UtcNow.Ticks;
                return _lastGeneratedResultSetId;
            }
        }

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
        /// <para>Will only post if Settings.PublisherEnabled.</para> 
        /// <para>Following must be vailable in NamedObjectRegistrar: string Host, int Port, string Version, string Channel, bool IsMaster</para> 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="resultSetId">If null, the last generated one will be used.Can be null, not adviced.</param>
        public static void Post(PublishItem item, string resultSetId) {
            if (Settings.PublisherEnabled) {
                item.Init(resultSetId ?? LastGeneratedResultSetId, NamedObjectRegistrar.Get<string>("Host"), NamedObjectRegistrar.Get<int>("Port"),
                    NamedObjectRegistrar.Get<string>("Version"), NamedObjectRegistrar.Get<string>("Channel"),
                    NamedObjectRegistrar.Get<bool>("IsMaster"));

                string destinationGroupId;
                TryAddDestination(item, out destinationGroupId);

                _destinations.Post(destinationGroupId, item);

            }
        }

        private static bool TryAddDestination(PublishItem message, out string destinationGroupId) {
            //Only handy for real multicast / broadcast destinations like file or udp. Not used atm.
            //destinationGroupId = message.PublishItemId.ReplaceInvalidWindowsFilenameChars('_').Replace(" ", "_") + "_" + message.PublishItemType + "_" + message.ResultSetId;
            destinationGroupId = "stub";

            if (_destinations.Contains(destinationGroupId)) return false;

            if (_settings.TcpOutput && !string.IsNullOrWhiteSpace(_settings.TcpHost)) {
                var tcpDestination = TcpDestination.GetInstance();
                if (tcpDestination.Init(_settings.TcpHost, _settings.TcpPort)) {
                    tcpDestination.Formatter = new JSONFormatter();
                }
                _destinations.Add(destinationGroupId, tcpDestination);
            }

            return true;
        }

        internal static void Clear() { _destinations.Clear(); }

        private static void Publisher_LogEntryWritten(object sender, WriteLogEntryEventArgs e) {
            if (Settings.PublisherEnabled && Settings.PublishApplicationLogs && (ushort)e.Entry.Level >= Settings.LogLevel) {
                var publishItem = new ApplicationLogEntry();
                publishItem.Level = (int)e.Entry.Level;
                publishItem.Description = e.Entry.Description;
                publishItem.Exception = e.Entry.Exception.ToString();
                publishItem.Parameters = e.Entry.Parameters;
                publishItem.Member = e.Entry.Member;
                publishItem.SourceFile = e.Entry.SourceFile;
                publishItem.Line = e.Entry.Line;

                Post(publishItem, LastGeneratedResultSetId);
            }
        }

        /// Wait until the post is idle. Can be handy for when posting stuff when the application gets closed.
        public static void WaitUntilIdle() { _destinations.WaitUntilIdle(); }
    }
}
