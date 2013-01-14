/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace vApus.Stresstest
{
    /// <summary>
    ///     This is just a wrapper class, it adds an extra variable that can be set (Used).
    ///     The reason for this is because all parallel connection proxies that are needed are made and destroyed before and after the test, but
    ///     we need a way to flag the ones that where chosen because they could not be used again, reusing connection proxies can be problematic (field data that stays behind, a crashed connection is best to not be reused).
    ///     We could use .SetTag from the extension methods in vApus.Util, but this is way faster (shared collection otherwise --> locking for reader threads).
    /// </summary>
    public struct ParallelConnectionProxy
    {
        public IConnectionProxy ConnectionProxy;

        /// <summary>
        ///     Set this to true when used.
        /// </summary>
        public bool Used;

        /// <summary>
        ///     This is just a wrapper class, it adds an extra variable that can be set (Used).
        ///     The reason for this is because all parallel connection proxies that are needed are made and destroyed before and after the test, but
        ///     we need a way to flag the ones that where chosen because they could not be used again, reusing connection proxies can be problematic (field data that stays behind, a crashed connection is best to not be reused).
        ///     We could use .SetTag from the extension methods in vApus.Util, but this is way faster (shared collection otherwise --> locking for reader threads).
        /// </summary>
        public ParallelConnectionProxy(IConnectionProxy connectionProxy)
        {
            ConnectionProxy = connectionProxy;
            Used = false;
        }
    }
}