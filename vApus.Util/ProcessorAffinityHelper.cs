//#if !defined(_WIN32_WINNT)
//#define _WIN32_WINNT 0x0600
//#endif

/*
 * Copyright 2008 (c) Sizing Servers Lab 
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace vApus.Util {
    /// <summary>
    /// </summary>
    public static class ProcessorAffinityHelper {
        /// <summary>
        ///  To know how much cores a group contains
        /// </summary>
        /// <param name="groupNumber">the group number if any, or ALL_PROCESSOR_GROUPS (0xffff) for every group</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern uint GetActiveProcessorCount(ushort groupNumber);

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

        /*TRY OUT
        /// <summary>
        /// </summary>
        /// <param name="threadHandle">handle of the specific thread</param>
        /// <param name="groupAffinity">GROUP_AFFINITY struct who will be applied</param>
        /// <param name="previousGroupAffinity">pointer to a GROUP_AFFINITY struct who will contain the previous group affinity, may be null.</param>
        /// <returns>nonzero on success, zero on failure. Use Marshal.GetLastWin32Error to get the errorcode</returns>
        [DllImport("kernel32.dll")]
        private static extern bool SetThreadGroupAffinity(IntPtr threadHandle, GROUP_AFFINITY groupAffinity, out GROUP_AFFINITY previousGroupAffinity);

        /// <summary>
        /// Retrieves the processor group affinity of the specified thread
        /// </summary>
        /// <param name="threadHandle">handle of the thread to check</param>
        /// <param name="groupAffinity">pointer to a GROUP_AFFINITY struct who will contain the information on success</param>
        /// <returns>nonzero on success, zero on failure. Use Marshal.GetLastWin32Error to get the errorcode</returns>
        [DllImport("kernel32.dll")]
        private static extern bool GetThreadGroupAffinity(IntPtr threadHandle, out GROUP_AFFINITY groupAffinity);

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

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern ulong GetLastError();

        public static void SetCoreAffinity(int[] array) {

            int logicalCoresPerGroup = (int)GetActiveProcessorCount(0); //The number should be the same for each group.

            var groupAffinities = new Dictionary<ushort, GROUP_AFFINITY>();

            foreach (int core in array) {
                ushort group = 0;
                int groupCore = core;
                while (core >= logicalCoresPerGroup) {
                    group++; //maximum of 4 groups on Win7
                    groupCore -= logicalCoresPerGroup;
                }
                if (groupAffinities.ContainsKey(group)) {
                    GROUP_AFFINITY aff = groupAffinities[group];
                    aff.Mask = new IntPtr(aff.Mask.ToInt64() + (long)Math.Pow(2, groupCore));
                    groupAffinities[group] = aff;

                } else {
                    GROUP_AFFINITY aff = new GROUP_AFFINITY() {
                        Group = group,
                        Mask = new IntPtr((long)Math.Pow(2, groupCore)),
                        Reserved = new ushort[] { 0, 0, 0 }
                    };
                    groupAffinities.Add(group, aff);
                }
            }

            //while (core > logicalCoresPerGroup) {
            //    group++; //maximum of 4 groups on Win7
            //    core -= (int)logicalCoresPerGroup;
            //}

            //GROUP_AFFINITY aff = new GROUP_AFFINITY() {
            //    Group = group,
            //    Mask = new IntPtr((long)Math.Pow(2, core)),
            //    Reserved = new ushort[] { 0, 0, 0 }
            //};
            GROUP_AFFINITY aff2;

            IntPtr handle = Process.GetCurrentProcess().Handle;
            IntPtr newHandle;
            if (!DuplicateHandle(handle, handle, handle, out newHandle, 0x0040 & 0x0200, true, 0)) {

            }

            if (!GetThreadGroupAffinity(newHandle, out aff2)) {
                var error = new Win32Exception((int)GetLastError());
            }


            //if (!SetThreadGroupAffinity(handle, aff, out aff2)) {

            //}


            //if (!CloseHandle(newHandle)) {

            //}

            //uncomment to check if the affinity has done correctly
            //Debug.WriteLine("Affinity is set to: " + Process.GetCurrentProcess().ProcessorAffinity);
        }

        public struct GROUP_AFFINITY {
            public IntPtr Mask; // KAFFINITY is a INTPTR which contains the normal thread affinity mask http://msdn.microsoft.com/en-us/library/windows/hardware/ff551830(v=vs.85).aspx
            public ushort Group;
            public ushort[] Reserved; //initialize with 3 zeros
        }*/
    }
}