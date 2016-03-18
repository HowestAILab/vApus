/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
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

        private static TcpDestination _tcpDestination = new TcpDestination();

        private static string _lastGeneratedResultSetId;

        private delegate bool SendDelegate(PublishItem item);
        private static BackgroundWorkQueue _sendQueue = new BackgroundWorkQueue();
        private static SendDelegate _sendDelegate = Send;

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
                _sendQueue.OnWorkItemProcessed += _sendQueue_OnWorkItemProcessed;
                _inited = true;
            }
        }

        /// <summary>
        /// <para>Will only send if Settings.PublisherEnabled.</para> 
        /// <para>Following must be vailable in NamedObjectRegistrar: string Host, int Port, string Version, string Channel, bool IsMaster</para> 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="resultSetId">Can be null, not adviced. You can use, with caution, this.LastGeneratedResultSetId.</param>
        public static void Send(PublishItem item, string resultSetId) {
            try {
                if (Settings.PublisherEnabled && !string.IsNullOrWhiteSpace(_settings.TcpHost))
                    _sendQueue.EnqueueWorkItem(_sendDelegate, InitItem(item, resultSetId));
            }
            catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed sending.", ex);
            }
        }
        private static PublishItem InitItem(PublishItem item, string resultSetId) {
            item.PublishItemTimestampInMillisecondsSinceEpochUtc = (long)(HighResolutionDateTime.UtcNow - PublishItem.EpochUtc).TotalMilliseconds;

            item.ResultSetId = resultSetId;

            item.PublishItemType = item.GetType().Name;

            item.vApusHost = NamedObjectRegistrar.Get<string>("Host");
            item.vApusPort = NamedObjectRegistrar.Get<int>("Port");
            item.vApusVersion = NamedObjectRegistrar.Get<string>("Version");
            item.vApusChannel = NamedObjectRegistrar.Get<string>("Channel");
            item.vApusIsMaster = NamedObjectRegistrar.Get<bool>("IsMaster");

            return item;
        }
        /// <summary>
        /// Sends a publish item of the type Poll with an empty result set id.
        /// </summary>
        /// <returns></returns>
        public static bool Poll() {
            try {
                if (Settings.PublisherEnabled && !string.IsNullOrWhiteSpace(_settings.TcpHost))
                    if (Send(InitItem(new Poll(), string.Empty)))
                        return true;
            }
            catch { }
            return false;
        }

        private static void _sendQueue_OnWorkItemProcessed(object sender, BackgroundWorkQueue.OnWorkItemProcessedEventArgs e) {
            if (e.Exception != null)
                Loggers.Log(Level.Error, "Failed sending.", e.Exception);
        }

        private static bool Send(PublishItem item) {
            _tcpDestination.Init(_settings.TcpHost, _settings.TcpPort, JSONFormatter.GetInstance());
            return _tcpDestination.Send(item);
        }

        private static void Publisher_LogEntryWritten(object sender, WriteLogEntryEventArgs e) {
            try {
                if (Settings.PublisherEnabled) {
                    var publishItem = new ApplicationLogEntry();
                    publishItem.Level = (int)e.Entry.Level;
                    publishItem.Description = e.Entry.Description;
                    publishItem.Exception = e.Entry.Exception.ToString();
                    publishItem.Parameters = e.Entry.Parameters;
                    publishItem.Member = e.Entry.Member;
                    publishItem.SourceFile = e.Entry.SourceFile;
                    publishItem.Line = e.Entry.Line;

                    Send(publishItem, LastGeneratedResultSetId);
                }
            }
            catch (Exception ex) {
                //Can fail if not connected. Handle like ths to avoid circular error mess.
                Debug.WriteLine("Failed publishing log entry. Is the publish items handler connected?" + ex.ToString());
            }
        }
    }
}
