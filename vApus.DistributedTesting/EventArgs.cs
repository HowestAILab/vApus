﻿/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using vApus.Communication.Shared;

namespace vApus.DistributedTest {
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
        public TileStressTest TileStressTest { get; private set; }
        #endregion

        #region Constructor
        public TestInitializedEventArgs(TileStressTest tileStressTest, Exception exception) {
            TileStressTest = tileStressTest;
            Exception = exception;
        }
        #endregion
    }

    public class TestProgressMessageReceivedEventArgs : EventArgs {

        #region Properties
        /// <summary>
        /// Link to the right TileStressTest using the TileStressTestIndex field.
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
        /// <param name="ok"># of tile stress tests that finished succesfully.</param>
        /// <param name="cancelled"># of tile stress tests that where cancelled.</param>
        /// <param name="error"># of tile stress tests that failed.</param>
        public TestFinishedEventArgs(int ok, int cancelled, int error) {
            OK = ok;
            Cancelled = cancelled;
            Error = error;
        }
        #endregion
    }
}