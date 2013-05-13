/*
 * Copyright 2008 (c) Sizing Servers Lab 
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace vApus.Util {
    /// <summary>
    /// </summary>
    public static class ProcessorAffinityCalculator {
        /// <summary>
        ///     to know how much cores a group contains
        /// </summary>
        /// <param name="groupNumber">the group number if any, or ALL_PROCESSOR_GROUPS (0xffff) for every group</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetActiveProcessorCount(ushort groupNumber);

        /// <summary>
        ///     Converts from a hex bitmask to an array of cpu's this process its affinity is set to.
        /// </summary>
        /// <param name="bitmask"></param>
        /// <returns></returns>
        public static int[] FromBitmaskToArray(IntPtr bitmask) {
            //To check if the bitmask contains the cpu's a binairy and is used.
            var cpus = new List<int>();
            long lBitmask = bitmask.ToInt64();
            for (int i = 0; i < GetActiveProcessorCount(0xFFFF); i++) //(0xFFFF) to include all processor groups
                if ((lBitmask & (int)Math.Pow(2, i)) > 0)
                    cpus.Add(i);
            return cpus.ToArray();
        }

        /// <summary>
        ///     Converts from an array of cpu's to a hex bitmask this process its affinaty is set to.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IntPtr FromArrayToBitmask(int[] array) {
            //Hex = 2^(n)
            long l = 0;
            for (int j = 0; j < array.Length; j++)
                l += (long)Math.Pow(2, array[j]);
            return new IntPtr(l);
        }
    }
}