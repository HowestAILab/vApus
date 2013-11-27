/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections;
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
    public class FunctionOutputCache : IDisposable {
        private bool _isDisposed;
        private List<CacheEntry> _cache = new List<CacheEntry>();

        public bool IsDisposed { get { return _isDisposed; } }

        /// <summary>
        /// If the data table in the returned entry is null, you need to set it afterwards. The same with output arguments.
        /// Put this call in each function getting data from the resultsserver (if you like): ResultsHelperCache(MethodInfo.GetCurrentMethod, arg1, arg2,...). 
        /// </summary>
        /// <param name="methodBase"></param>
        /// <param name="inputArguments">Exclude cancellationToken and databaseActions; primary datatypes, arrays and ILists are supported. If you pass arrays or list, preserve the order of the entries everywhere to be able to have as much hits as possible.</param>
        /// <returns></returns>
        public CacheEntry GetOrAdd(MethodBase methodBase, params object[] inputArguments) {
            foreach (var entry in _cache)
                if (entry.MethodBase == methodBase && InspectArgumentEquality(entry.InputArguments, inputArguments))
                    return entry;

            var newEntry = new CacheEntry(methodBase, inputArguments);
            _cache.Add(newEntry);
            _cache.Sort(CacheEntryComparer.GetInstance);
            return newEntry;
        }
        private bool InspectArgumentEquality(object a, object b) {
            if ((a == null && b != null) || (a != null && b == null)) return false;
            if ((a == null && b == null) || a.Equals(b)) return true;
            if (a is Array && b is Array) {
                var aArr = a as Array;
                var bArr = b as Array;

                if (aArr.Length == bArr.Length) {
                    for (int i = 0; i != aArr.Length; i++) {
                        if (!InspectArgumentEquality(aArr.GetValue(i), bArr.GetValue(i)))
                            return false;
                    }
                    return true;
                }
                return false;
            }
            if (a is IList && b is IList) {
                var aL = a as IList;
                var bL = b as IList;

                if (aL.Count == bL.Count) {
                    for (int i = 0; i != aL.Count; i++) {
                        if (!InspectArgumentEquality(aL[i], bL[i]))
                            return false;
                    }
                    return true;
                }
                return false;
            }

            return false;
        }
        public void Dispose() {
            if (!_isDisposed) {
                _isDisposed = true;
                _cache = null;
                GC.Collect();
            }
        }
        /// <summary>
        /// Contains the results (DataTable), the identifier (MethodBase) and the meta data (Arguments of the method) for the entry.
        /// </summary>
        public class CacheEntry {
            public MethodBase MethodBase { get; private set; }
            public object[] InputArguments { get; private set; }
            public object[] OutputArguments { get; set; }
            public object ReturnValue { get; set; }

            public CacheEntry(MethodBase methodBase, object[] inputArguments) {
                MethodBase = methodBase;
                InputArguments = inputArguments;
            }
        }
        private class CacheEntryComparer : IComparer<CacheEntry> {
            private static CacheEntryComparer _cacheEntryComparer = new CacheEntryComparer();

            private CacheEntryComparer() { }
            public int Compare(CacheEntry x, CacheEntry y) {
                return x.MethodBase.Name.CompareTo(y.MethodBase.Name);
            }

            public static CacheEntryComparer GetInstance { get { return _cacheEntryComparer; } }
        }
    }
}
