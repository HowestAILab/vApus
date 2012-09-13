/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;

namespace vApus.Util
{
    public static class DictionaryUtil
    {
        public static TKey GetKeyAt<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, int index)
        {
            if (index < 0)
                throw new IndexOutOfRangeException("index < 0");
            else if (index >= dictionary.Count)
                throw new IndexOutOfRangeException("Index is larger or equals the count of dictionary kvps.");

            IEnumerator<TKey> enumerator = dictionary.Keys.GetEnumerator();
            enumerator.Reset();
            int currentIndex = -1;
            while (currentIndex++ < index)
                enumerator.MoveNext();
            return enumerator.Current;
        }
        public static bool TryGetKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value, out TKey key)
        {
            foreach (TKey k in dictionary.Keys)
                if (dictionary[k].Equals(value))
                {
                    key = k;
                    return true;
                }
            key = default(TKey);
            return false;
        }
    }
}
