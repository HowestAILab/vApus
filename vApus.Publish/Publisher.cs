/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
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
            try {
                if (Settings.PublisherEnabled) {
                    InitItem(item, resultSetId ?? LastGeneratedResultSetId);

                    //Only handy for real multicast / broadcast destinations like file or udp. Not used atm.
                    //destinationGroupId = message.PublishItemId.ReplaceInvalidWindowsFilenameChars('_').Replace(" ", "_") + "_" + message.PublishItemType + "_" + message.ResultSetId;
                    string destinationGroupId = "stub";
                    TryAddDestination(item, destinationGroupId);
                    _destinations.Post(destinationGroupId, item);
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed posting.", ex);
            }
        }
        private static void InitItem(PublishItem item, string resultSetId) {
            item.PublishItemTimestampInMillisecondsSinceEpochUtc = (long)(HighResolutionDateTime.UtcNow - PublishItem.EpochUtc).TotalMilliseconds;

            item.ResultSetId = resultSetId;

            item.PublishItemType = item.GetType().Name;

            item.vApusHost = NamedObjectRegistrar.Get<string>("Host");
            item.vApusPort = NamedObjectRegistrar.Get<int>("Port");
            item.vApusVersion = NamedObjectRegistrar.Get<string>("Version");
            item.vApusChannel = NamedObjectRegistrar.Get<string>("Channel");
            item.vApusIsMaster = NamedObjectRegistrar.Get<bool>("IsMaster");
        }
        /// <summary>
        /// Posts a publish item of the type Poll with an empty result set id.
        /// </summary>
        /// <returns></returns>
        public static bool TryPost() {
            try {
                if (Settings.PublisherEnabled) {
                    var item = new Poll();
                    InitItem(item, string.Empty);

                    _destinations.Clear();

                    string destinationGroupId = "stub";
                    TryAddDestination(item, destinationGroupId);
                    _destinations.Post(destinationGroupId, item);
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed try posting.", ex);
                return false;
            }
            return true;
        }

        private static bool TryAddDestination(PublishItem message, string destinationGroupId) {
            if (_settings.TcpOutput && !string.IsNullOrWhiteSpace(_settings.TcpHost)) {
                var tcpDestination = TcpDestination.GetInstance();
                if (tcpDestination.Init(_settings.TcpHost, _settings.TcpPort)) {
                    tcpDestination.Formatter = new JSONFormatter();
                }

                if (_destinations.Contains(destinationGroupId)) return false;

                _destinations.Add(destinationGroupId, tcpDestination);
            }

            return true;
        }

        internal static void Clear() { _destinations.Clear(); }

        private static void Publisher_LogEntryWritten(object sender, WriteLogEntryEventArgs e) {
            try {
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
            } catch (Exception ex) {
                //Can fail if not connected. Handle like ths to avoid circular error mess.
                Debug.WriteLine("Failed publishing log entry. Is the publisher connected?" + ex.ToString());
            }
        }

        /// Wait until the post is idle. Can be handy for when posting stuff when the application gets closed.
        public static void WaitUntilIdle() { _destinations.WaitUntilIdle(); }
    }
}
