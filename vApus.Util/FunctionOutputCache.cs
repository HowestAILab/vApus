using RandomUtils;
/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
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
