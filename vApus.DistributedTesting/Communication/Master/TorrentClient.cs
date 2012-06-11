/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Glenn Desmadryl
 */

using System;
using System.Diagnostics;
using System.IO;
using MonoTorrent.BEncoding;
using MonoTorrent.Client;
using MonoTorrent.Common;

namespace vApus.DistributedTesting
{
    public delegate void ProgressUpdatedEventHandler(TorrentClient source, TorrentEventArgs e);
    public delegate void DownloadCompletedEventHandler(TorrentClient source, TorrentEventArgs e);
    public delegate void StatusChangedEventHandler(TorrentClient source, string status);

    public class TorrentEventArgs : EventArgs
    {
        public double PercentCompleted;

        public TorrentEventArgs(double percentCompleted = 0.0)
        {
            PercentCompleted = percentCompleted;
        }
    }

    public class TorrentClient
    {
        #region Fields
        protected ClientEngine _engine;
        protected TorrentManager _manager;
        protected int _port = 52138; //Default by MonoTorrent
        public bool ReplaceExistingFile = true;
        #endregion

        #region Events
        public event ProgressUpdatedEventHandler ProgressUpdated;
        public event DownloadCompletedEventHandler DownloadCompleted;
        public event StatusChangedEventHandler StatusChanged;
        #endregion

        #region Properties
        public string CurrentStatus
        {
            get { return _manager.State.ToString(); }
        }
        /// <summary>
        /// The name of the torrent if any.
        /// </summary>
        public string Name
        {
            get
            {

                if (_manager != null)
                    return _manager.Torrent.Name;
                else
                    return "Not found";
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TorrentClient()
        {
            _port = new Random().Next(32000, 56000);
            _engine = new ClientEngine(new EngineSettings(Environment.CurrentDirectory, _port));

            Debug.WriteLine("TorrentClient created with port " + _port);
        }

        #region Logic

        #region Download Torrent
        /// <summary>
        /// Downloads the torrent that can be composed from torrentInfo to given location
        /// </summary>
        /// <param name="torrentInfo"></param>
        /// <param name="saveFolder"></param>
        /// <returns>Filename</returns>
        public string DownloadTorrentFromBytes(byte[] torrentInfo, string location)
        {
            return DownloadTorrent(Torrent.Load(torrentInfo), location);
        }

        /// <summary>
        /// Downloads the given torrent to given physical location.
        /// </summary>
        /// <param name="torrent"></param>
        /// <param name="location"></param>
        public string DownloadTorrent(Torrent torrent, string location)
        {

            // Create the manager which will download the torrent to savePath
            // using the default settings.
            _manager = new TorrentManager(torrent, location, new TorrentSettings());
            _manager.TorrentStateChanged += new EventHandler<TorrentStateChangedEventArgs>(manager_TorrentStateChanged);

            _manager.PieceHashed += delegate(object o, PieceHashedEventArgs e)
            {
                #region PieceInformation
                //int pieceIndex = e.PieceIndex;
                //int totalPieces = e.TorrentManager.Torrent.Pieces.Count;
                //double progress = (double)pieceIndex / totalPieces * 100.0;
                //if (e.HashPassed)
                //Console.WriteLine("Piece {0} of {1} is complete ", pieceIndex, totalPieces);
                //else
                //{
                //    Console.WriteLine("Piece {0} of {1} is corrupt or incomplete ", pieceIndex, totalPieces);
                //}

                //// This shows how complete the hashing is.
                //Console.WriteLine("Total progress is: {0}%", progress);
                #endregion

                if (ProgressUpdated != null && e.TorrentManager.State == TorrentState.Downloading && e.HashPassed)
                    foreach (ProgressUpdatedEventHandler del in ProgressUpdated.GetInvocationList())
                        del.BeginInvoke(this, new TorrentEventArgs(e.TorrentManager.Progress), null, null);
            };

            // Register the manager with the engine
            _engine.Register(_manager);

            // If the torrent is fully downloaded already, calling 'Start' will place
            // the manager in the Seeding state.
            _manager.Start();

            return torrent.Name;
        }
        #endregion

        #region TorrentStateChanged
        protected void manager_TorrentStateChanged(object sender, TorrentStateChangedEventArgs e)
        {
            Console.WriteLine("CLIENT: " + e.TorrentManager.Torrent + " has changed state: " + e.OldState + " --> " + e.NewState);
            if (StatusChanged != null)
                foreach (StatusChangedEventHandler del in StatusChanged.GetInvocationList())
                    del.BeginInvoke(this, CurrentStatus, null, null);

            if (e.NewState == TorrentState.Error)
            {
                e.TorrentManager.Stop();

                if (e.TorrentManager.Error != null)
                    Debug.WriteLine(e.TorrentManager.Error.Exception.Message);
            }

            //to release the file after download
            if (e.OldState == TorrentState.Downloading && e.NewState == TorrentState.Seeding)
            {
                if (DownloadCompleted != null)
                    foreach (DownloadCompletedEventHandler del in DownloadCompleted.GetInvocationList())
                        del.BeginInvoke(this, new TorrentEventArgs(100), null, null);
            }

            if (e.NewState == TorrentState.Stopping)
            {
                Console.WriteLine("Torrent {0} has begun stopping", e.TorrentManager.Torrent.Name);
            }
            else if (e.NewState == TorrentState.Stopped)
            {
                // It is now safe to unregister the torrent from the engine and dispose of it (if required)
                try
                {
                    _engine.Unregister(_manager);
                    _manager.Dispose();
                }
                catch (TorrentException ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                Console.WriteLine("Torrent {0} has stopped", e.TorrentManager.Torrent.Name);
            }
        }
        #endregion

        /// <summary>
        /// Stops the current torrent.
        /// </summary>
        public void StopTorrent()
        {
            // When stopping a torrent, certain cleanup tasks need to be perfomed
            // such as flushing unwritten data to disk and informing the tracker
            // the client is no longer downloading/seeding the torrent. To allow for
            // this, when Stop is called the manager enters a 'Stopping' state. Once
            // all the tasks are completed the manager will enter the 'Stopped' state.

            // Begin the process to stop the torrent
            _manager.Stop();
        }

        #region FastResume
        public byte[] SaveFastResume(byte[] torrentBytes)
        {
            Torrent torrent = null;

            if (!Torrent.TryLoad(torrentBytes, out torrent))
                throw new ArgumentException("torrentBytes not correct", "torrentBytes");


            // Get the fast resume data for the torrent
            FastResume data = _manager.SaveFastResume();

            MemoryStream memStream = new MemoryStream();
            // Encode the FastResume data to a stream.
            data.Encode(memStream);
            memStream.Close();

            // stream to bytes
            return memStream.ToArray();
        }

        /// <summary>
        /// This will load a FastResume for the torrent.
        /// </summary>
        /// <param name="fastResumeData"></param>
        public void LoadFastResume(byte[] fastResumeData)
        {
            // Read the main dictionary from disk and iterate through
            // all the fast resume items
            BEncodedList list = (BEncodedList)BEncodedValue.Decode(fastResumeData);
            foreach (BEncodedDictionary fastResume in list)
            {
                // Decode the FastResume data from the BEncodedDictionary
                FastResume data = new FastResume(fastResume);

                // Find the torrentmanager that the fastresume belongs to
                // and then load it
                if (_manager.InfoHash == data.Infohash)
                {
                    _manager.LoadFastResume(data);
                    Console.WriteLine("FastResumeData added for " + _manager.Torrent);
                }
                else
                    throw new ArgumentException("FastResumeData is not intended for this client", "fastResumeData");
            }
        }
        #endregion

        #endregion
    }
}