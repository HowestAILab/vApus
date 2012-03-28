/*
 * Copyright 2011 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System.Windows.Forms;

namespace vApus.Util
{
    /// <summary>
    /// Fixes scroll to control reset.
    /// </summary>
    public class ScrollablePanel : Panel
    {
        //Finally solved, thank you Anonymous at http://seewinapp.blogspot.com/2005/09/is-your-autoscroll-too-auto.html!
        protected override System.Drawing.Point ScrollToControl(Control activeControl)
        {
            return DisplayRectangle.Location;
        }
    }
}
