/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using vApus.Util;
namespace vApus.Stresstest
{
    public interface IConnectionProxy: IDisposable
    {
        bool IsConnectionOpen { get; }
        bool IsDisposed { get; }
        void TestConnection(out string error);
        void OpenConnection();
        void CloseConnection();
        void SendAndReceive(StringTree lexedLogEntry, out DateTime sentAt, out TimeSpan timeToLastByte, out Exception exception);
        /// <summary>
        /// For testing the code in the connection proxy code view.
        /// </summary>
        /// <param name="lexedLogEntry"></param>
        /// <param name="sentAt"></param>
        /// <param name="timeToLastByte"></param>
        /// <param name="exception"></param>
        void TestSendAndReceive(StringTree lexedLogEntry, out DateTime sentAt, out TimeSpan timeToLastByte, out Exception exception);
    }
}
