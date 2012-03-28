/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Threading;

namespace vApus.Util
{
    public static class SynchronizationContextWrapper
    {
        private static SynchronizationContext _synchronizationContext;
        /// <summary>
        /// To synchronize to the main thread (SynchronizationContext.Send/.Post).
        /// </summary>
        public static SynchronizationContext SynchronizationContext
        {
            get
            {
                if (_synchronizationContext == null)
                    _synchronizationContext = SynchronizationContext.Current;
                return _synchronizationContext;
            }
            set
            {
                if (value == null)
                    throw new NullReferenceException("Try assigning SynchronizationContext.Current to this value, one is automaticaly created together with the handle of the first form. AKA, set this when the HandleCreated event of your main form is invoked.");
                _synchronizationContext = value;
            }
        }
        /// <summary>
        /// When overridden in a derived class, dispatches a synchronous message to a synchronization context.
        /// </summary>
        /// <param name="synchronizationContext"></param>
        /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
        public static void Send(this SynchronizationContext synchronizationContext, SendOrPostCallback d)
        {
            try
            {
                synchronizationContext.Send(d, null);
            }
            catch { }
        }
        /// <summary>
        /// When overridden in a derived class, dispatches an asynchronous message to a synchronization context.
        /// </summary>
        /// <param name="synchronizationContext"></param>
        /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
        public static void Post(this SynchronizationContext synchronizationContext, SendOrPostCallback d)
        {
            try
            {
                synchronizationContext.Post(d, null);
            }
            catch { }
        }
    }
}
