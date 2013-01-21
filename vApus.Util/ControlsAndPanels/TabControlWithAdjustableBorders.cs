/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vApus.Util
{
    public class TabControlWithAdjustableBorders : TabControl
    {
        private const int TCM_ADJUSTRECT = 0X1328;
        private bool _bottomVisible, _leftVisible, _rightVisible, _topVisible;

        public bool LeftVisible
        {
            get { return _leftVisible; }
            set { _leftVisible = value; }
        }

        public bool RightVisible
        {
            get { return _rightVisible; }
            set { _rightVisible = value; }
        }

        public bool TopVisible
        {
            get { return _topVisible; }
            set { _topVisible = value; }
        }

        public bool BottomVisible
        {
            get { return _bottomVisible; }
            set { _bottomVisible = value; }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == TCM_ADJUSTRECT)
            {
                var rc = (RECT)m.GetLParam(typeof(RECT));

                if (!_leftVisible) rc.Left -= 4;
                if (!_rightVisible) rc.Right += 3;
                if (!_topVisible) rc.Top -= 3;
                if (!_bottomVisible) rc.Bottom += 3;

                Marshal.StructureToPtr(rc, m.LParam, true);
            }
            base.WndProc(ref m);
        }

        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }
    }
}