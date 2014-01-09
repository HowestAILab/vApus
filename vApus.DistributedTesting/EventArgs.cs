/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using vApus.Server.Shared;

namespace vApus.DistributedTesting {
    public class ListeningErrorEventArgs : EventArgs {

        #region Properties
        public Exception Exception { get; private set; }
        public string SlaveIP { get; private set; }
        public int SlavePort { get; private set; }
        #endregion

        #region Constructor
        public ListeningErrorEventArgs(string slaveIP, int slavePort, Exception exception) {
            Exception = exception;
            SlaveIP = slaveIP;
            SlavePort = slavePort;
        }
        #endregion

        #region Functions
        public override string ToString() { return "A listening error occured for slave " + SlaveIP + ":" + SlavePort + " threw following exception: " + Exception; }
        #endregion
    }

    public class TestInitializedEventArgs : EventArgs {

        #region Properties
        public Exception Exception { get; private set; }
        public TileStresstest TileStresstest { get; private set; }
        #endregion

        #region Constructor
        public TestInitializedEventArgs(TileStresstest tileStresstest, Exception exception) {
            TileStresstest = tileStresstest;
            Exception = exception;
        }
        #endregion
    }

    public class TestProgressMessageReceivedEventArgs : EventArgs {

        #region Properties
        /// <summary>
        /// Link to the right TileStresstest using the TileStresstestIndex field.
        /// </summary>
        public TestProgressMessage TestProgressMessage { get; private set; }
        #endregion

        #region Constructor
        public TestProgressMessageReceivedEventArgs(TestProgressMessage testProgressMessage) { TestProgressMessage = testProgressMessage; }
        #endregion
    }

    public class TestFinishedEventArgs : EventArgs {

        #region Properties
        public int Cancelled { get; private set; }
        public int Error { get; private set; }
        public int OK { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ok"># of tile stresstests that finished succesfully.</param>
        /// <param name="cancelled"># of tile stresstests that where cancelled.</param>
        /// <param name="error"># of tile stresstests that failed.</param>
        public TestFinishedEventArgs(int ok, int cancelled, int error) {
            OK = ok;
            Cancelled = cancelled;
            Error = error;
        }
        #endregion
    }
}