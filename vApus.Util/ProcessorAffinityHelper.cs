/*
 * Copyright 2008 (c) Sizing Servers Lab 
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace vApus.Util {
    /// <summary>
    /// </summary>
    public static class ProcessorAffinityHelper {
        /// <summary>
        ///     to know how much cores a group contains
        /// </summary>
        /// <param name="groupNumber">the group number if any, or ALL_PROCESSOR_GROUPS (0xffff) for every group</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetActiveProcessorCount(ushort groupNumber);

        /// <summary>
        /// </summary>
        /// <param name="threadHandle">handle of the specific thread</param>
        /// <param name="groupAffinity">GROUP_AFFINITY struct who will be applied</param>
        /// <param name="previousGroupAffinity">pointer to a GROUP_AFFINITY struct who will contain the previous group affinity, may be null.</param>
        /// <returns>nonzero on success, zero on failure. Use Marshal.GetLastWin32Error to get the errorcode</returns>
        [DllImport("kernel32.dll")]
        private static extern bool SetThreadGroupAffinity(IntPtr threadHandle, GROUP_AFFINITY groupAffinity, out GROUP_AFFINITY previousGroupAffinity);

        /// <summary>
        /// Duplicates an object handle
        /// </summary>
        /// <param name="hSourceProcessHandle">source process handle</param>
        /// <param name="hSourceHandle">handle to be duplicated</param>
        /// <param name="hTargetProcessHandle">process who will receive the new handle</param>
        /// <param name="lpTargetHandle">this pointer will receive the new handle</param>
        /// <param name="dwDesiredAccess">access parameters for the new handle</param>
        /// <param name="bInheritHandle">if true, the handle can be inherited by new childprocesses of the target</param>
        /// <param name="dwOptions">optional actions, can be zero</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle, uint dwDesiredAccess, bool bInheritHandle, uint dwOptions);

        /// <summary>
        /// Closes the previously created handle
        /// </summary>
        /// <param name="handle">the handle that should be closed</param>
        /// <returns>true if succesful, false if otherwise</returns>
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        ///     Converts from a hex bitmask to an array of cpu's this process its affinity is set to.
        /// </summary>
        /// <param name="bitmask">e.g. Process.GetCurrentProcess().ProcessorAffinity</param>
        /// <returns>Zero-based core indices.</returns>
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
        ///     Does not work for more than 63 cores.
        /// </summary>
        /// <param name="array">Zero-based core indices.</param>
        /// <returns></returns>
        public static IntPtr FromArrayToBitmask(int[] array) {
            //Hex = 2^(n)
            long l = 0;
            for (int j = 0; j < array.Length; j++)
                l += (long)Math.Pow(2, array[j]);
            return new IntPtr(l);
        }

        public static void SetCoreAffinity(int core) {

            #region tryout
            uint logicalCoresPerGroup = GetActiveProcessorCount(0); //The number should be the same for each group.
            ushort group = 0;
            while (core > logicalCoresPerGroup) {
                group++; //maximum of 4 groups on Win7
                core -= (int)logicalCoresPerGroup;
            }

            GROUP_AFFINITY aff = new GROUP_AFFINITY() {
                Group = group,
                Mask = new IntPtr((long)Math.Pow(2, core)),
                Reserved = new ushort[] { 0, 0, 0 }
            };

            IntPtr handle = Process.GetCurrentProcess().Handle;
            IntPtr newHandle;
            if (!DuplicateHandle(handle, handle, handle, out newHandle, 0x0200, true, 0)) {

            }

            //if (!GetThreadGroupAffinity(Process.GetCurrentProcess().MainWindowHandle, out aff2))
            //    Console.WriteLine("meh");


            GROUP_AFFINITY aff2;
            if (!SetThreadGroupAffinity(handle, aff, out aff2)) {

            }


            if (!CloseHandle(newHandle)) {

            }
            #endregion

            //uncomment to check if the affinity has done correctly
            //Debug.WriteLine("Affinity is set to: " + Process.GetCurrentProcess().ProcessorAffinity);
        }

        public struct GROUP_AFFINITY {
            public IntPtr Mask; // KAFFINITY is a INTPTR which contains the normal thread affinity mask http://msdn.microsoft.com/en-us/library/windows/hardware/ff551830(v=vs.85).aspx
            public ushort Group;
            public ushort[] Reserved; //initialize with 3 zeros
        }
    }
}