/*
 * Copyright 2011 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */

using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util
{
    /// <summary>
    ///     Fixes scroll to control reset.
    /// </summary>
    public class ScrollablePanel : Panel
    {
        //Finally solved, thank you Anonymous at http://seewinapp.blogspot.com/2005/09/is-your-autoscroll-too-auto.html!
        protected override Point ScrollToControl(Control activeControl)
        {
            return DisplayRectangle.Location;
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            if (se.ScrollOrientation == ScrollOrientation.VerticalScroll)
                VerticalScroll.Value = se.NewValue;
            else if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                HorizontalScroll.Value = se.NewValue;
        }
    }
}