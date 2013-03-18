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
    public class ConverterCollection : List<ConverterKVP> {
    }
    public class Bla : IDictionary<object, object> {
        private ConverterCollection _cache = new ConverterCollection();
        public void Add(object key, object value) {
           // _cache.Add(new ConverterKVP(key, value);
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
            throw new System.NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new System.NotImplementedException();
        }
    }
    public class ConverterKVP : Dictionary<object, object> {
        private KeyValuePair<object, object> _kvp;

        public ConverterKVP()
            : base(1) { }
        public ConverterKVP(int capacity)
            : base(1) { }
        public ConverterKVP(object key, object value) {
            _kvp = new KeyValuePair<object, object>(key, value);
            Clear();
            Add(_kvp.Key, _kvp.Value);
        }
        [JsonIgnore]
        public object Key {
            get { return _kvp.Key; }
            set {
                _kvp = new KeyValuePair<object, object>(value, _kvp.Value);
                Clear();
                Add(_kvp.Key, _kvp.Value);
            }
        }
        [JsonIgnore]
        public object Value {
            get { return _kvp.Value; }
            set {
                _kvp = new KeyValuePair<object, object>(_kvp.Key, value);
                Clear();
                Add(_kvp.Key, _kvp.Value);
            }
        }
    }
}
