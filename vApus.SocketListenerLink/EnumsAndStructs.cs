using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vApus.SocketListenerLink
{
    [Serializable]
    public enum Key
    {
        SynchronizeBuffers,
        /// <summary>
        /// To poll if the slave is online.
        /// If so, this slave will connect back to the master.
        /// </summary>
        Poll,
        /// <summary>
        /// Use this key sending a stresstest project from the master, initiate the stresstest at the slave whereupon the slave must send a message with this key in it back.
        /// </summary>
        InitializeTest,
        StartTest,
        Break,
        Continue,
        StopTest,
        /// <summary>
        /// To push progress to the master (also finished and failed and such).
        /// Pushing progress will be at minimum, just the metrics will be send, getting the results will happen afterwards.
        /// </summary>
        Push,
        /// <summary>
        /// This will return a torrent file in bytes that the torrent client will use on the master-side.
        /// </summary>
        Results,
        /// <summary>
        /// Stops seeding the results and deletes the r file slave side if it was succesfully sent to the master.
        /// </summary>
        StopSeedingResults
    }
    [Serializable]
    public struct SynchronizeBuffersMessage
    {
        public int BufferSize;
        public string Exception;
    }

}
