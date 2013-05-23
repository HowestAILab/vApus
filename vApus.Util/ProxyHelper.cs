//Thx to random dude at codeproject

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace vApus.Util {
    public class ProxyHelper {
        /// <summary>
        /// This is not unset correctly by the fiddler-proxy api, that's why we are using our own implementation.
        /// </summary>
        /// <returns></returns>
        public static bool UnsetProxy() {
            return SetProxy(null, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="strProxy">ip:port</param>
        /// <returns></returns>
        private static bool SetProxy(string strProxy) {
            return SetProxy(strProxy, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="strProxy">ip:port</param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        private static bool SetProxy(string strProxy, string exceptions) {
            int retried = 0;

        Retry:
            try {
                var list = new InternetPerConnOptionList();

                int optionCount = string.IsNullOrEmpty(strProxy) ? 1 : (string.IsNullOrEmpty(exceptions) ? 2 : 3);
                var options = new InternetConnectionOption[optionCount];
                // USE a proxy server ...
                options[0].m_Option = PerConnOption.INTERNET_PER_CONN_FLAGS;
                options[0].m_Value.m_Int =
                    (int)
                    ((optionCount < 2)
                         ? PerConnFlags.PROXY_TYPE_DIRECT
                         : (PerConnFlags.PROXY_TYPE_DIRECT | PerConnFlags.PROXY_TYPE_PROXY));
                // use THIS proxy server
                if (optionCount > 1) {
                    options[1].m_Option = PerConnOption.INTERNET_PER_CONN_PROXY_SERVER;
                    options[1].m_Value.m_StringPtr = Marshal.StringToHGlobalAuto(strProxy);
                    // except for these addresses ...
                    if (optionCount > 2) {
                        options[2].m_Option = PerConnOption.INTERNET_PER_CONN_PROXY_BYPASS;
                        options[2].m_Value.m_StringPtr = Marshal.StringToHGlobalAuto(exceptions);
                    }
                }

                // default stuff
                list.dwSize = Marshal.SizeOf(list);
                list.szConnection = IntPtr.Zero;
                list.dwOptionCount = options.Length;
                list.dwOptionError = 0;


                int optSize = Marshal.SizeOf(typeof(InternetConnectionOption));
                // make a pointer out of all that ...
                IntPtr optionsPtr = Marshal.AllocCoTaskMem(optSize * options.Length);
                // copy the array over into that spot in memory ...
                for (int i = 0; i < options.Length; ++i) {
                    var opt = new IntPtr(optionsPtr.ToInt32() + (i * optSize));
                    Marshal.StructureToPtr(options[i], opt, false);
                }

                list.options = optionsPtr;

                // and then make a pointer out of the whole list
                IntPtr ipcoListPtr = Marshal.AllocCoTaskMem(list.dwSize);
                Marshal.StructureToPtr(list, ipcoListPtr, false);

                // and finally, call the API method!
                int returnvalue = NativeMethods.InternetSetOption(IntPtr.Zero,
                                                                  InternetOption.INTERNET_OPTION_PER_CONNECTION_OPTION,
                                                                  ipcoListPtr, list.dwSize)
                                      ? -1
                                      : 0;
                if (returnvalue == 0) {
                    // get the error codes, they might be helpful
                    returnvalue = Marshal.GetLastWin32Error();
                }
                // FREE the data ASAP
                Marshal.FreeCoTaskMem(optionsPtr);
                Marshal.FreeCoTaskMem(ipcoListPtr);
                if (returnvalue > 0) {
                    // throw the error codes, they might be helpful
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            } catch (Exception ex) {
                if (++retried != 3) {
                    Thread.Sleep(100 * retried);
                    goto Retry;
                }

                throw ex;
            }
            return true;
        }
        public static void ClearProxyCache() {
            int retried = 0;

        Retry:
            try {

                // Indicates that all of the cache groups in the user's system should be enumerated
                const int CACHEGROUP_SEARCH_ALL = 0x0;
                // Indicates that all the cache entries that are associated with the cache group
                // should be deleted, unless the entry belongs to another cache group.
                const int CACHEGROUP_FLAG_FLUSHURL_ONDELETE = 0x2;
                // File not found.
                const int ERROR_FILE_NOT_FOUND = 0x2;
                // No more items have been found.
                const int ERROR_NO_MORE_ITEMS = 259;
                // Pointer to a GROUPID variable
                long groupId = 0;

                // Local variables
                int cacheEntryInfoBufferSizeInitial = 0;
                int cacheEntryInfoBufferSize = 0;
                IntPtr cacheEntryInfoBuffer = IntPtr.Zero;
                INTERNET_CACHE_ENTRY_INFOA internetCacheEntry;
                IntPtr enumHandle = IntPtr.Zero;
                bool returnValue = false;

                // Delete the groups first.
                // Groups may not always exist on the system.
                // For more information, visit the following Microsoft Web site:
                // http://msdn.microsoft.com/library/?url=/workshop/networking/wininet/overview/cache.asp			
                // By default, a URL does not belong to any group. Therefore, that cache may become
                // empty even when the CacheGroup APIs are not used because the existing URL does not belong to any group.			
                enumHandle = NativeMethods.FindFirstUrlCacheGroup(0, CACHEGROUP_SEARCH_ALL, IntPtr.Zero, 0, ref groupId, IntPtr.Zero);
                // If there are no items in the Cache, you are finished.
                if (enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                    return;

                // Loop through Cache Group, and then delete entries.
                while (true) {
                    // Delete a particular Cache Group.
                    returnValue = NativeMethods.DeleteUrlCacheGroup(groupId, CACHEGROUP_FLAG_FLUSHURL_ONDELETE, IntPtr.Zero);
                    if (!returnValue && ERROR_FILE_NOT_FOUND == Marshal.GetLastWin32Error())
                        returnValue = NativeMethods.FindNextUrlCacheGroup(enumHandle, ref groupId, IntPtr.Zero);


                    if (!returnValue && (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error() || ERROR_FILE_NOT_FOUND == Marshal.GetLastWin32Error()))
                        break;

                    long prevGroupId = groupId;
                    enumHandle = NativeMethods.FindFirstUrlCacheGroup(0, CACHEGROUP_SEARCH_ALL, IntPtr.Zero, 0, ref groupId, IntPtr.Zero);
                    // If there are no items in the Cache, you are finished.
                    if ((enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error()) || groupId == prevGroupId)
                        break;
                }

                // Start to delete URLs that do not belong to any group.
                enumHandle = NativeMethods.FindFirstUrlCacheEntry(null, IntPtr.Zero, ref cacheEntryInfoBufferSizeInitial);
                if (enumHandle == IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                    return;

                cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
                cacheEntryInfoBuffer = Marshal.AllocHGlobal(cacheEntryInfoBufferSize);
                enumHandle = NativeMethods.FindFirstUrlCacheEntry(null, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);

                while (true) {
                    internetCacheEntry = (INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(cacheEntryInfoBuffer, typeof(INTERNET_CACHE_ENTRY_INFOA));

                    cacheEntryInfoBufferSizeInitial = cacheEntryInfoBufferSize;
                    returnValue = NativeMethods.DeleteUrlCacheEntry(internetCacheEntry.lpszLocalFileName);
                    if (!returnValue)
                        returnValue = NativeMethods.FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);

                    if (!returnValue && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                        break;

                    if (!returnValue && cacheEntryInfoBufferSizeInitial > cacheEntryInfoBufferSize) {
                        cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
                        cacheEntryInfoBuffer = Marshal.ReAllocHGlobal(cacheEntryInfoBuffer, (IntPtr)cacheEntryInfoBufferSize);
                        returnValue = NativeMethods.FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                    }
                }
                Marshal.FreeHGlobal(cacheEntryInfoBuffer);
            } catch (Exception ex) {
                if (++retried != 3) {
                    Thread.Sleep(100 * retried);
                    goto Retry;
                }

                throw ex;
            }
        }
    }
    #region WinInet structures

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct InternetPerConnOptionList {
        public int dwSize; // size of the INTERNET_PER_CONN_OPTION_LIST struct
        public IntPtr szConnection; // connection name to set/query options
        public int dwOptionCount; // number of options to set/query
        public int dwOptionError; // on error, which option failed
        //[MarshalAs(UnmanagedType.)]
        public IntPtr options;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct InternetConnectionOption {
        private static readonly int Size;
        public PerConnOption m_Option;
        public InternetConnectionOptionValue m_Value;

        static InternetConnectionOption() {
            Size = Marshal.SizeOf(typeof(InternetConnectionOption));
        }

        // Nested Types
        [StructLayout(LayoutKind.Explicit)]
        public struct InternetConnectionOptionValue {
            // Fields
            [FieldOffset(0)]
            public FILETIME m_FileTime;
            [FieldOffset(0)]
            public int m_Int;
            [FieldOffset(0)]
            public IntPtr m_StringPtr;
        }
    }

    // For PInvoke: Contains information about an entry in the Internet cache
    [StructLayout(LayoutKind.Explicit, Size = 80)]
    public struct INTERNET_CACHE_ENTRY_INFOA {
        [FieldOffset(0)]
        public uint dwStructSize;
        [FieldOffset(4)]
        public IntPtr lpszSourceUrlName;
        [FieldOffset(8)]
        public IntPtr lpszLocalFileName;
        [FieldOffset(12)]
        public uint CacheEntryType;
        [FieldOffset(16)]
        public uint dwUseCount;
        [FieldOffset(20)]
        public uint dwHitRate;
        [FieldOffset(24)]
        public uint dwSizeLow;
        [FieldOffset(28)]
        public uint dwSizeHigh;
        [FieldOffset(32)]
        public FILETIME LastModifiedTime;
        [FieldOffset(40)]
        public FILETIME ExpireTime;
        [FieldOffset(48)]
        public FILETIME LastAccessTime;
        [FieldOffset(56)]
        public FILETIME LastSyncTime;
        [FieldOffset(64)]
        public IntPtr lpHeaderInfo;
        [FieldOffset(68)]
        public uint dwHeaderInfoSize;
        [FieldOffset(72)]
        public IntPtr lpszFileExtension;
        [FieldOffset(76)]
        public uint dwReserved;
        [FieldOffset(76)]
        public uint dwExemptDelta;
    }
    #endregion

    #region WinInet enums

    //
    // options manifests for Internet{Query|Set}Option
    //
    public enum InternetOption : uint {
        INTERNET_OPTION_PER_CONNECTION_OPTION = 75
    }

    //
    // Options used in INTERNET_PER_CONN_OPTON struct
    //
    public enum PerConnOption {
        INTERNET_PER_CONN_FLAGS = 1,
        // Sets or retrieves the connection type. The Value member will contain one or more of the values from PerConnFlags 
        INTERNET_PER_CONN_PROXY_SERVER = 2, // Sets or retrieves a string containing the proxy servers.  
        INTERNET_PER_CONN_PROXY_BYPASS = 3,
        // Sets or retrieves a string containing the URLs that do not use the proxy server.  
        INTERNET_PER_CONN_AUTOCONFIG_URL = 4
        //, // Sets or retrieves a string containing the URL to the automatic configuration script.  
    }

    //
    // PER_CONN_FLAGS
    //
    [Flags]
    public enum PerConnFlags {
        PROXY_TYPE_DIRECT = 0x00000001, // direct to net
        PROXY_TYPE_PROXY = 0x00000002, // via named proxy
        PROXY_TYPE_AUTO_PROXY_URL = 0x00000004, // autoproxy URL
        PROXY_TYPE_AUTO_DETECT = 0x00000008 // use autoproxy detection
    }

    #endregion

    internal static class NativeMethods {
        [DllImport("WinInet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InternetSetOption(IntPtr hInternet, InternetOption dwOption, IntPtr lpBuffer, int dwBufferLength);

        // For PInvoke: Initiates the enumeration of the cache groups in the Internet cache
        [DllImport(@"wininet", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FindFirstUrlCacheGroup", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstUrlCacheGroup(int dwFlags, int dwFilter, IntPtr lpSearchCondition, int dwSearchCondition, ref long lpGroupId, IntPtr lpReserved);

        // For PInvoke: Retrieves the next cache group in a cache group enumeration
        [DllImport(@"wininet", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FindNextUrlCacheGroup", CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextUrlCacheGroup(IntPtr hFind, ref long lpGroupId, IntPtr lpReserved);

        // For PInvoke: Releases the specified GROUPID and any associated state in the cache index file
        [DllImport(@"wininet", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DeleteUrlCacheGroup", CallingConvention = CallingConvention.StdCall)]
        public static extern bool DeleteUrlCacheGroup(long GroupId, int dwFlags, IntPtr lpReserved);

        // For PInvoke: Begins the enumeration of the Internet cache
        [DllImport(@"wininet", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FindFirstUrlCacheEntry", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstUrlCacheEntry([MarshalAs(UnmanagedType.LPTStr)] string lpszUrlSearchPattern, IntPtr lpFirstCacheEntryInfo, ref int lpdwFirstCacheEntryInfoBufferSize);

        // For PInvoke: Retrieves the next entry in the Internet cache
        [DllImport(@"wininet", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FindNextUrlCacheEntry", CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextUrlCacheEntry(IntPtr hFind, IntPtr lpNextCacheEntryInfo, ref int lpdwNextCacheEntryInfoBufferSize);

        // For PInvoke: Removes the file that is associated with the source name from the cache, if the file exists
        [DllImport(@"wininet", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DeleteUrlCacheEntry", CallingConvention = CallingConvention.StdCall)]
        public static extern bool DeleteUrlCacheEntry(IntPtr lpszUrlName);
    }
}