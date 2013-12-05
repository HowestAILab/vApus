/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace vApus.Util {
    public static class FastObjectCreator {

        private delegate object Ctor();

        /// <summary>
        /// Creates a new instance of a type using an empty public constructor. This should be faster than Activator.CreateInstance(...);
        /// This is not thread safe.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstance(Type type) {
            //if (type.IsValueType)
            //    return Activator.CreateInstance(type); //structs have no explicit parameterless constructors.
            return GetConstructor(type)();
        }

        private static Ctor GetConstructor(Type type) {
            var cacheEntry = FunctionOutputCacheWrapper.FunctionOutputCache.GetOrAdd(MethodBase.GetCurrentMethod(), type);

            if (cacheEntry.ReturnValue == null) {
                DynamicMethod method = new DynamicMethod(string.Empty, type, null);

                ILGenerator gen = method.GetILGenerator();
                gen.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));// new Ctor
                gen.Emit(OpCodes.Ret);

                cacheEntry.ReturnValue = method.CreateDelegate(typeof(Ctor));
            }

            return cacheEntry.ReturnValue as Ctor;
        }
    }
}
