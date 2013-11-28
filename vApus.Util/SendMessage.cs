/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace vApus.Util {
    public static class SendMessageWrapper {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, string lp);

        private const int WM_SETREDRAW = 0xB;
        private const int CB_SETCUEMESSAGE = 0x1703;

        public static void SetWindowRedraw(IntPtr hwnd, bool allowRedraw) {
            SendMessage(hwnd, WM_SETREDRAW, allowRedraw ? 1 : 0, 0);
        }

        public static void SetComboboxCueBar(IntPtr hwnd, string cueText) {
            SendMessage(hwnd, CB_SETCUEMESSAGE, (IntPtr)1, cueText);
        }
    }
}
