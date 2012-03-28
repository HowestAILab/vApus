/*
 * Copyright 2011 (c) Sizing Servers Lab 
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;

namespace vApus.Util
{
    public class ObjectService
    {
        private object _lock = new object();
        private List<object> _suscribers = new List<object>();
        private int _maxSuscribers = int.MaxValue;

        /// <summary>
        /// You can set a maximum value if you like, thread safe.
        /// </summary>
        public int MaxSuscribers
        {
            get { return _maxSuscribers; }
            set
            {
                lock (_lock)
                {
                    if (value < _suscribers.Count)
                        throw new ArgumentOutOfRangeException("The number of suscribers is already greater than the given value.");
                    _maxSuscribers = value;
                }
            }
        }

        /// <summary>
        /// Retrieve all suscribed objects, thread safe.
        /// </summary>
        public object[] Suscribers
        {
            get
            {
                lock (_lock)
                    return _suscribers.ToArray();
            }
        }
        /// <summary>
        /// If the object is already suscribed nothing will happen, thread safe.
        /// </summary>
        /// <param name="suscriber"></param>
        public void Suscribe(object suscriber)
        {
            lock (_lock)
                if (!_suscribers.Contains(suscriber))
                {
                    if (_suscribers.Count == _maxSuscribers)
                        throw new ArgumentOutOfRangeException("Cannot add the suscriber (MaxSuscribers).");
                    _suscribers.Add(suscriber);
                }
        }
        /// <summary>
        /// Thread safe.
        /// </summary>
        /// <param name="suscriber"></param>
        /// <returns></returns>
        public bool Unsuscribe(object suscriber)
        {
            lock (_lock)
                return _suscribers.Remove(suscriber);
        }
        /// <summary>
        /// Thread safe.
        /// </summary>
        public void UnsuscribeAll()
        {
            lock (_lock)
                _suscribers.Clear();
        }
        /// <summary>
        /// Remove all objects that are null.
        /// </summary>
        public void Clean()
        {
            lock (_lock)
            {
                var suscribers = new List<object>(_suscribers.Count);
                foreach (object suscriber in _suscribers)
                    if (suscriber != null)
                        suscribers.Add(suscriber);
                _suscribers = suscribers;
            }
        }
    }
}