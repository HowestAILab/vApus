/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    /// The connection proxy code class implements this interface. This for easy function access in the ConnectionProxyPool and StressTestCore.
    /// </summary>
    public interface IConnectionProxy : IDisposable {
        bool IsConnectionOpen { get; }
        bool IsDisposed { get; }
        void TestConnection(out string error);
        void OpenConnection();
        void CloseConnection();

        void SendAndReceive(StringTree parameterizedRequest, out DateTime sentAt, out TimeSpan timeToLastByte, out string meta, out Exception exception);
    }
}