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
    }
}
