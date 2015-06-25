using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vApus.Communication {
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

}
