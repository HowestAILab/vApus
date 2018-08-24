using RandomUtils;
/*
 * 2013 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace vApus.Util {
    public static class FunctionOutputCacheWrapper {
        private static FunctionOutputCache _functionOutputCache;
        public static FunctionOutputCache FunctionOutputCache {
            get {
                if (_functionOutputCache == null || _functionOutputCache.IsDisposed) _functionOutputCache = new FunctionOutputCache();
                return _functionOutputCache;
            }
        }

    }
}
