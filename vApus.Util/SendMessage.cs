/*
 * 2013 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
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
