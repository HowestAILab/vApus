/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.Collections.Generic;
namespace vApus.Util {
    /// <summary>
    /// The root has a label (string, int, ...) and a ObjectTree for a leaf. The deepest leaf should be a label and a struct.
    /// This is not a real dictionary in the sence that it can have duplicate keys. However, when serializing to JSON (Newtonsoft.Json), the output is clean for this (dictionary) type.
    /// </summary>
    public class JSONObjectTree : IDictionary<object, object> {
        private List<KeyValuePair<object, object>> _cache = new List<KeyValuePair<object, object>>();

        public void Add(object key, object value) { _cache.Add(new KeyValuePair<object, object>(key, value)); }

        public bool ContainsKey(object key) {
            foreach (var kvp in _cache)
                if (kvp.Key == key)
                    return true;
            return false;
        }

        public ICollection<object> Keys {
            get {
                var keys = new List<object>(_cache.Count);
                foreach (var kvp in _cache) keys.Add(kvp.Key);
                return keys;
            }
        }
        /// <summary>
        /// Removes all entries with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(object key) {
            if (ContainsKey(key)) {
                var newCache = new List<KeyValuePair<object, object>>(_cache.Count - 1);
                foreach (var kvp in _cache)
                    if (kvp.Key != key) newCache.Add(kvp);

                _cache = newCache;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries getting the first value with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(object key, out object value) {
            foreach (var kvp in _cache)
                if (kvp.Key == key) {
                    value = kvp.Value;
                    return true;
                }
            value = null;
            return false;
        }

        public ICollection<object> Values {
            get {
                var values = new List<object>(_cache.Count);
                foreach (var kvp in _cache) values.Add(kvp.Value);
                return values;
            }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[object key] { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }

        public void Add(KeyValuePair<object, object> item) { _cache.Add(item); }

        public void Clear() { _cache.Clear(); }

        public bool Contains(KeyValuePair<object, object> item) { return _cache.Contains(item); }

        public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex) { _cache.CopyTo(array, arrayIndex); }

        public int Count { get { return _cache.Count; } }

        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<object, object> item) { return _cache.Remove(item); }

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator() { return _cache.GetEnumerator(); }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return _cache.GetEnumerator(); }
    }
}
