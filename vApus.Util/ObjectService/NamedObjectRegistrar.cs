/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Concurrent;

namespace vApus.Util
{
    /// <summary>
    /// Serves at registring an object so you can get the properties anywhere in the application.
    /// If you don't need a name please use ObjectRegistrar, you can do more with it and it is strongly typed making it less error prone.
    /// </summary>
    public static class NamedObjectRegistrar
    {
        private static ConcurrentDictionary<string, object> register = new ConcurrentDictionary<string, object>();
        public static void RegisterOrUpdate(string name, object obj)
        {
            register.AddOrUpdate(name, obj, (key, oldValue) => obj);
        }
        public static bool Unregister(string name)
        {
            object obj;
            return register.TryRemove(name, out obj);
        }
        public static T Get<T>(string name)
        {
            object obj;
            register.TryGetValue(name, out obj);
            return (T)obj;
        }
    }
}
