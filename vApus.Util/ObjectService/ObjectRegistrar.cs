/*
 * Copyright 2011 (c) Sizing Servers Lab 
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;

namespace vApus.Util {
    public static class ObjectRegistrar {
        private static readonly object _lock = new object();
        private static int _maxRegistered = int.MaxValue;
        private static List<object> _register = new List<object>();

        /// <summary>
        ///     You can set a maximum value if you like, thread safe.
        /// </summary>
        public static int MaxRegistered {
            get { return _maxRegistered; }
            set {
                lock (_lock) {
                    if (value < _register.Count)
                        throw new ArgumentOutOfRangeException("The number of suscribers is already greater than the given value.");
                    _maxRegistered = value;
                }
            }
        }

        /// <summary>
        ///     Retrieve all suscribed objects, thread safe.
        /// </summary>
        public static object[] Registered { get { lock (_lock) return _register.ToArray(); } }

        /// <summary>
        ///     If the object is already suscribed nothing will happen, thread safe.
        /// </summary>
        /// <param name="obj"></param>
        public static void Register(object obj) {
            lock (_lock)
                if (!_register.Contains(obj)) {
                    if (_register.Count == _maxRegistered)
                        throw new ArgumentOutOfRangeException("Cannot add the suscriber (MaxSuscribers).");
                    _register.Add(obj);
                }
        }

        /// <summary>
        ///     Thread safe.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Unregister(object obj) { lock (_lock) return _register.Remove(obj); }

        /// <summary>
        ///     Thread safe.
        /// </summary>
        public static void Clear() { lock (_lock) _register.Clear(); }

        /// <summary>
        ///     Remove all objects that are null.
        /// </summary>
        public static void Clean() {
            lock (_lock) {
                var suscribers = new List<object>(_register.Count);
                foreach (object suscriber in _register)
                    if (suscriber != null) suscribers.Add(suscriber);
                _register = suscribers;
            }
        }
    }
}