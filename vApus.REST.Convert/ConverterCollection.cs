/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.Collections.Generic;
namespace vApus.REST.Convert {
    public class ConverterCollection : IDictionary<object, object> {
        private List<KeyValuePair<object, object>> _cache = new List<KeyValuePair<object, object>>();
        public void Add(object key, object value) {
            _cache.Add(new KeyValuePair<object, object>(key, value));
        }

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

        public bool Remove(object key) {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(object key, out object value) {
            throw new System.NotImplementedException();
        }

        public ICollection<object> Values {
            get { throw new System.NotImplementedException(); }
        }

        public object this[object key] {
            get {
                throw new System.NotImplementedException();
            }
            set {
                throw new System.NotImplementedException();
            }
        }

        public void Add(KeyValuePair<object, object> item) {
            throw new System.NotImplementedException();
        }

        public void Clear() {
            _cache.Clear();
        }

        public bool Contains(KeyValuePair<object, object> item) {
            return _cache.Contains(item);
        }

        public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex) {
            throw new System.NotImplementedException();
        }

        public int Count {
            get { return _cache.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(KeyValuePair<object, object> item) {
            throw new System.NotImplementedException();
        }

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator() {
            return _cache.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _cache.GetEnumerator();
        }
    }
}
