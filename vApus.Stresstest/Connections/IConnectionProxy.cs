/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
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