using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vApus.SocketListenerLink
{
    public class IPChangedEventArgs : EventArgs
    {
        public readonly string IP;
        /// <summary>
        /// </summary>
        /// <param name="ip"></param>
        public IPChangedEventArgs(string ip)
        {
            IP = ip;
        }
    }
    public class ListeningErrorEventArgs : EventArgs
    {
        public readonly Exception Exception;
        public readonly string SlaveIP;
        public readonly int SlavePort;

        public ListeningErrorEventArgs(string slaveIP, int slavePort, Exception exception)
        {
            Exception = exception;
            SlaveIP = slaveIP;
            SlavePort = slavePort;
        }

        public override string ToString()
        {
            return "Listening error occured for slave " + SlaveIP + ":" + SlavePort + " threw following exception: " + Exception;
        }
    }
    public class NewTestEventArgs : EventArgs
    {
        public readonly string Test;
        public NewTestEventArgs(string test)
        {
            Test = test;
        }
    }
}
