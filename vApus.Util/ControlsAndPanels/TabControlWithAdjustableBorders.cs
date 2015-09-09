/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vApus.Util {
    public class TabControlWithAdjustableBorders : TabControl {

        private const int TCM_ADJUSTRECT = 0X1328;

        public bool BottomVisible { get; set; }
        public bool LeftVisible { get; set; }
        public bool RightVisible { get; set; }
        public bool TopVisible { get; set; }

        protected override void WndProc(ref Message m) {
            try {
                if (m.Msg == TCM_ADJUSTRECT) {
                    var rc = (RECT)m.GetLParam(typeof(RECT));

                    if (!LeftVisible) rc.Left -= 4;
                    if (!RightVisible) rc.Right += 3;
                    if (!TopVisible) rc.Top -= 3;
                    if (!BottomVisible) rc.Bottom += 3;

                    Marshal.StructureToPtr(rc, m.LParam, true);
                }
                base.WndProc(ref m);
            } catch {
                //Whatever.
            }
        }

        private struct RECT { public int Left, Top, Right, Bottom;  }
    }
}