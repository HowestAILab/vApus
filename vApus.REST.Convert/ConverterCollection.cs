/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.Collections.Generic;
using Newtonsoft.Json;
namespace vApus.REST.Convert {
    //public class ConverterCollection : List<ConverterKVP> {
    //}
    public class ConverterCollection : IDictionary<object, object> {
        private List<KeyValuePair<object, object>> _cache = new List<KeyValuePair<object, object>>();
        public void Add(object key, object value) {
            _cache.Add(new KeyValuePair<object, object>(key, value));
        }

        public bool ContainsKey(object key) {
            throw new System.NotImplementedException();
        }

        public ICollection<object> Keys {
            get { throw new System.NotImplementedException(); }
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
            throw new System.NotImplementedException();
        }

        public bool Contains(KeyValuePair<object, object> item) {
            throw new System.NotImplementedException();
        }

        public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex) {
            throw new System.NotImplementedException();
        }

        public int Count {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsReadOnly {
            get { throw new System.NotImplementedException(); }
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
