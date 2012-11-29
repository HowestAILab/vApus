/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace vApus.Util
{
    /// <summary>
    ///     Serves to register a window message to your application.
    ///     For instance, in combination with a named mutex you can bring the already running app to the front.
    ///     (See vApus.UpdateTool.Update)
    /// </summary>
    public class Win32WindowMessageHandler
    {
        public const int HWND_BROADCAST = 0xffff;
        public readonly int WINDOW_MSG;

        /// <summary>
        ///     Win32WindowMessageHandler(Assembly.GetEntryAssembly().FullName + " " + Process.GetCurrentProcess().Id.ToString())
        /// </summary>
        public Win32WindowMessageHandler()
            : this(Assembly.GetEntryAssembly().FullName + " " + Process.GetCurrentProcess().Id.ToString())
        {
        }

        /// <summary>
        ///     Serves to register a window message to your application.
        ///     For instance, in combination with a named mutex you can bring the already running app to the front.
        ///     (See vApus.UpdateTool.Update)
        /// </summary>
        public Win32WindowMessageHandler(string windowMessage)
        {
            WINDOW_MSG = RegisterWindowMessage(windowMessage);
        }

        [DllImport("user32")]
        private static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        private static extern int RegisterWindowMessage(string message);

        /// <summary>
        ///     This will broadcast the message to all the running apps, WndProc should be overridden in your main form to handle the message.
        /// </summary>
        public void PostMessage()
        {
            PostMessage(IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        ///     This will broadcast the message to all the running apps, WndProc should be overridden in your main form to handle the message.
        /// </summary>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        public void PostMessage(IntPtr wparam, IntPtr lparam)
        {
            PostMessage((IntPtr) HWND_BROADCAST, WINDOW_MSG, wparam, lparam);
        }
    }
}