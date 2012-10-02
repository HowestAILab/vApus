/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Glenn Desmadryl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MonoTorrent.Common;
using MonoTorrent.Tracker;
using MonoTorrent.Tracker.Listeners;

namespace vApus.DistributedTesting
{
    public delegate void TorrentCreatedEventHandler(object source, TorrentServerEventArgs e);
    public delegate void TorrentSeededEventHandler(object source, TorrentServerEventArgs e);

    public class TorrentServerEventArgs : EventArgs
    {
        public byte[] TorrentData;
        public TorrentServerEventArgs(byte[] torrentData)
        {
            TorrentData = torrentData;
        }
    }

    public class TorrentServer
    {
        #region Fields
        private ManualResetEvent _torrentStateChangedToStoppedWaitHandle = new ManualResetEvent(true);
        private ManualResetEvent _locationUnlockedWaitHandle = new ManualResetEvent(true);

        private Tracker _tracker;
        private HttpListener _listener;
        private string _ip;
        private int _port;

        //private HashSet<TorrentManager> _managers;
        //private ClientEngine _engine;
        //private Dictionary<Torrent, byte[]> _torrentQueu;
        private Dictionary<string, byte[]> _torrentQueu;

        HashSet<TorrentClient> _clients;
        #endregion

        #region Properties
        public bool TrackerStarted
        {
            get { return _tracker != null && !_tracker.Disposed; }
        }

        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public List<string> CurrentStatus
        {
            get
            {
                List<string> answers = new List<string>();

                foreach (TorrentClient client in _clients)
                    answers.Add(client.Name + " : " + client.CurrentStatus);

                return answers;
            }
        }
        #endregion

        #region Events
        public event TorrentSeededEventHandler TorrentSeeded;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TorrentServer(string ip, int port)
        {
            _torrentQueu = new Dictionary<string, byte[]>();
            _clients = new HashSet<TorrentClient>();

            StartTracker(ip, port);
        }

        #region Logic

        #region Tracker
        /// <summary>
        /// Starts the tracker with given IP and port. Tracker can only be started once.
        /// </summary>
        public void StartTracker(string ip, int port)
        {

            if (_tracker == null)
            {
                StopTracker();
                _tracker = new Tracker();
                // When unregistered torrents are allowed, if a client requests peers for
                // a torrent which is not currently monitored by the tracker, that torrent
                // will be added automatically. This is useful for creating a 'public' tracker.
                // Normally, if this is false, announce requests for torrents not already
                // registered with the engine are sent an error message.

                _tracker.AllowUnregisteredTorrents = false;
                _tracker.AllowScrape = true;
                //_tracker.PeerAnnounced += new EventHandler<AnnounceEventArgs>(_tracker_PeerAnnounced);
                _tracker.AnnounceInterval = new TimeSpan(0, 0, 5); //set the timespan

                // Create a listener which will respond to scrape/announce requests
                _ip = ip;
                _port = port;

                while (!CreateListener(_ip, _port, out _listener))
                {
                    _port++;
                }

                _tracker.RegisterListener(_listener);

                _listener.Start();

                Debug.WriteLine("Listener created on ip " + _ip + " with port " + _port);
            }
        }

        private bool CreateListener(string ip, int port, out HttpListener listener)
        {
            try
            {
                listener = new HttpListener("http://" + _ip + ":" + _port + "/announce/");
                return true;
            }
            catch
            {
                listener = null;
                return false;
            }
        }

        void _tracker_PeerAnnounced(object sender, AnnounceEventArgs e)
        {
            Debug.WriteLine(e.Peer.ClientAddress + " has connected.");
        }
        public void StopTracker()
        {
            if (TrackerStarted)
            {
                _tracker.Dispose();
                _tracker = null;
                if (_listener != null && _listener.Running)
                    //Stop pauses, not quits! (with normal HTTPListener, but this is a own MonoTorrent implementation so I don't know the effect of this)
                    _listener.Stop();
                _listener = null;
            }
        }
        #endregion

        /// <summary>
        /// Loads a given .torrent filelocation and gives a MonoTorrent.Common.Torrent object back.
        /// </summary>
        /// <param name="torrentLocation">Location of .torrent file</param>
        /// <returns>Torrent if can be loaded, null when not</returns>
        public object LoadTorrent(string torrentLocation)
        {
            Torrent torrent = null;
            Torrent.TryLoad(torrentLocation, out torrent);
            return torrent;
        }

        /// <summary>
        /// Stops the manager who watches over the given Torrent
        /// </summary>
        /// <param name="torrent"></param>
        public void StopTorrent(string torrentName)
        {
            foreach (TorrentClient client in _clients)
                if (client.Name == torrentName)
                    client.StopTorrent();
        }

        /// <summary>
        /// Closes down all current torrents. Its basically a reset for the server.
        /// </summary>
        public void CloseAllTorrents()
        {
            foreach (TorrentClient client in _clients)
                client.StopTorrent();

            _clients.Clear();
        }
        public void EnforceReleaseFile(string filename)
        {
            Debug.WriteLine("Have to wait for door");
            _torrentStateChangedToStoppedWaitHandle.WaitOne();

            //I've added it on Wednesday 10-11-2010 (FileSystemWatcher) but doesnt work, keeps everything hanging
            if (!FileUnlocked(new FileInfo(filename)))
            {    //Thread.Sleep(2);
                _locationUnlockedWaitHandle.Reset();
                FileSystemWatcher watcher = new FileSystemWatcher(new FileInfo(filename).DirectoryName, "*" + new FileInfo(filename).Extension);

                watcher.NotifyFilter = NotifyFilters.LastAccess;

                watcher.Changed += new FileSystemEventHandler(watcher_Changed);

                watcher.EnableRaisingEvents = true;
            }
            _locationUnlockedWaitHandle.WaitOne();

            Debug.WriteLine(filename + " released.");
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("something changed");
            _locationUnlockedWaitHandle.Set();
        }

        private bool FileUnlocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return true;
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        #region StartSeeding
        /// <summary>
        /// Main method, not public.
        /// </summary>
        private void StartSeeding(Torrent torrent, string parentDirectory)
        {
            //Register this torrent as tracked by our tracker
            InfoHashTrackable trackable = new InfoHashTrackable(torrent);
            if (_tracker.Contains(trackable))
            {
                _tracker.Remove(trackable);
                Debug.WriteLine("Tracker has removed trackable " + trackable.Name);
            }

            _tracker.Add(trackable);
            Debug.WriteLine(torrent + " added to tracker.");

            #region MonoTorrentDefaults

            //int DefaultDownloadSpeed = 0;
            // int DefaultMaxConnections = 60;
            // int DefaultUploadSlots = 4;
            // int DefaultUploadSpeed = 0;

            #endregion

            #region New way of seeding
            //trying to move the logic using a TorrentClient
            TorrentClient client = new TorrentClient();
            _clients.Add(client);
            //in this message there's the needed information to setup a torrent connection
            //client.ProgressUpdated += new ProgressUpdatedEventHandler(client_ProgressUpdated);
            //client.DownloadCompleted += new DownloadCompletedEventHandler(client_DownloadCompleted);
            client.StatusChanged += new StatusChangedEventHandler(client_StatusChanged);
            client.DownloadTorrent(torrent, parentDirectory); //by giving the parentDirectory of the input the torrent will start Seeding

            #endregion
        }

        void client_StatusChanged(TorrentClient source, string status)
        {
            if (status == "Seeding")
            {
                if (TorrentSeeded != null)
                    foreach (TorrentSeededEventHandler del in TorrentSeeded.GetInvocationList())
                        del.BeginInvoke(this, new TorrentServerEventArgs(_torrentQueu[source.Name]), null, null);
            }
            else if (status == "Stopping")
            {
                Debug.WriteLine("SERVERCLIENT Door closed for " + source.Name);
                _torrentStateChangedToStoppedWaitHandle.Reset();
            }
            else if (status == "Stopped")
            {
                Debug.WriteLine("SERVERCLIENT Door open for " + source.Name);
                _torrentStateChangedToStoppedWaitHandle.Set();
            }

            Debug.WriteLine("SERVERCLIENT changed to " + status + "(" + source.Name + ")");
        }
        /// <summary>
        /// Starts seeding a torrent composed from the given torrentBytes. Specify the location on your file system with location.
        /// </summary>
        public void StartSeeding(byte[] torrentBytes, string parentDirectory)
        {
            StartSeeding(Torrent.Load(torrentBytes), parentDirectory);
        }
        /// <summary>
        /// Starts seeding a given torrent. Specify the location on your file system with location.
        /// </summary>
        public void StartSeeding(object torrent, string parentDirectory)
        {
            if (torrent is Torrent)
                StartSeeding(torrent as Torrent, parentDirectory);
            else
                throw new ArgumentException("torrent is not a Torrent", "torrent");
        }
        #endregion

        #region CreateTorrent
        /// <summary>
        /// Main method to create torrents (file and/or stream). This is private.
        /// </summary>
        private void CreateTorrent(string inputLocation, string torrentOutputLocation = null, System.IO.Stream stream = null)
        {
            if (!TrackerStarted)
                throw new Exception("Please start the tracker first.");

            // The class used for creating the torrent
            TorrentCreator torrentCreator = new TorrentCreator();

            // Add one tier which contains one tracker
            List<string> tier = new List<string>();
            tier.Add("http://" + _ip + ":" + _port + "/announce/");
            torrentCreator.Announces.Add(new MonoTorrent.RawTrackerTier(tier));

            torrentCreator.Comment = "Package with stresstest results";
            torrentCreator.CreatedBy = "vApus using MonoTorrent " + VersionInfo.ClientVersion;
            torrentCreator.Publisher = "www.sizingservers.be";

            // Set the torrent as private so it will not use DHT or peer exchange
            // Generally you will not want to set this.
            torrentCreator.Private = false;

            //By setting the PieceLength to zero the torrentCreator will calculate the best PieceLength (not setting it at all will cause an exception)
            torrentCreator.PieceLength = 0;

            //Create a filesource, path can be either a directory *or* a file.
            TorrentFileSource source = new TorrentFileSource(inputLocation, false);

            // Create the torrent file and save it directly to the specified path
            // Different overloads of 'Create' can be used to save the data to a Stream
            // or just return it as a BEncodedDictionary (its native format) so it can be
            // processed in memory
            if (torrentOutputLocation != null && torrentOutputLocation != string.Empty)
                //torrentCreator.Create(torrentOutputLocation);
                torrentCreator.Create(source, torrentOutputLocation);

            if (stream != null)
                torrentCreator.Create(source, stream);

            //Extra creation to add this one to the queu
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            torrentCreator.Create(source, memStream);
            memStream.Close();
            if (!_torrentQueu.ContainsKey(Torrent.Load(memStream.ToArray()).Name))
                _torrentQueu.Add(Torrent.Load(memStream.ToArray()).Name, memStream.ToArray());
            memStream.Dispose();

            torrentCreator = null;
        }

        /// <summary>
        /// Use this function to create a .torrent file.
        /// </summary>
        /// <param name="trackerIp">Ip of the tracker you want to use.</param>
        /// <param name="trackerPort">Port of the tracker you want to use.</param>
        /// <param name="fileOrFolder">The input, can be a file or directory.</param>
        /// <param name="outputFileLocation">Location where the .torrent file will be saved.</param>
        public void CreateTorrentToFile(string fileOrFolder, string outputFileLocation)
        {
            CreateTorrent(fileOrFolder, outputFileLocation);
        }

        /// <summary>
        /// Use this function to create a torrent in byte[]. Can be used to send across sockets.
        /// </summary>
        /// <param name="trackerIp">Ip of the tracker you want to use.</param>
        /// <param name="trackerPort">Port of the tracker you want to use.</param>
        /// <param name="fileOrFolder">The input, can be a file or directory.</param>
        /// <returns></returns>
        public byte[] CreateTorrentInBytes(string fileOrFolder)
        {
            MemoryStream memStream = new MemoryStream();
            try
            {
                CreateTorrent(fileOrFolder, null, memStream);
            }
            finally
            {
                memStream.Close();
            }
            return memStream.ToArray();
        }
        #endregion

        #endregion

    }
}