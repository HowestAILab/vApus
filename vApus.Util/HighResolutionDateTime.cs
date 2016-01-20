﻿/*
http://manski.net/2014/07/high-resolution-clock-in-csharp/
Thx Manski
*/
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Use this instead of DateTime.Now.
/// </summary>
public static class HighResolutionDateTime {
    public static bool IsAvailable { get; private set; }

    [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
    private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

    public static DateTime UtcNow {
        get {
            if (!IsAvailable)
                throw new InvalidOperationException("High resolution clock isn't available.");

            long filetime;
            GetSystemTimePreciseAsFileTime(out filetime);

            return DateTime.FromFileTimeUtc(filetime);
        }
    }

    static HighResolutionDateTime() {
        try {
            long filetime;
            GetSystemTimePreciseAsFileTime(out filetime);
            IsAvailable = true;
        } catch (EntryPointNotFoundException) {
            // Not running Windows 8 or higher.
            IsAvailable = false;
        }
    }
}