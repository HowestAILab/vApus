/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Windows.Forms;
using System;
using System.ComponentModel;

namespace vApus.Util
{
    [ToolboxItem(false)]
    public partial class XAxis : UserControl
    {
        #region Events
        internal event EventHandler<DrawValuesCalculatedEventArgs> DrawValuesCalculated;
        #endregion

        #region Properties
        internal ChartViewState ChartViewState
        {
            get { return xAxisLabel.ChartViewState; }
            set
            {
                if (xAxisLabel.ChartViewState != value)
                {
                    xAxisLabel.ChartViewState = value;
                    switch (xAxisLabel.ChartViewState)
                    {
                        case ChartViewState.Collapsed:
                            xAxisLabel.Left = -1;
                            xAxisLabel.Width = this.Width + 1;
                            break;
                        default:
                            xAxisLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                            xAxisLabel.Left = llblScrollBack.Right;
                            xAxisLabel.Width = this.Width - xAxisLabel.Left - (this.Width - llblScrollForth.Left);
                            break;
                    }
                }
            }
        }
        internal AutoScrollXAxis AutoScrollXAxis
        {
            get { return xAxisLabel.AutoScrollXAxis; }
            set { xAxisLabel.AutoScrollXAxis = value; }
        }
        internal int XAxisLabelLeft
        {
            get { return xAxisLabel.Left; }
        }
        internal int XAxisLabelWidth
        {
            get { return xAxisLabel.Width; }
        }
        #endregion

        #region Constructor
        public XAxis()
        {
            InitializeComponent();
            xAxisLabel.DrawValuesCalculated += new EventHandler<DrawValuesCalculatedEventArgs>(xAxisLabel_DrawValuesCalculated);
        }
        #endregion

        #region Functions
        internal void SetSeries(Series series)
        {
            xAxisLabel.SetSeries(series);
        }
        /// <summary>
        /// Linklabels are enabled or disabled accordanly, this is invoked to chart area,
        /// whereupon chartarea will send these values to chart control and forces it to render.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xAxisLabel_DrawValuesCalculated(object sender, DrawValuesCalculatedEventArgs e)
        {
            SetLLBLS();
            if (DrawValuesCalculated != null)
                DrawValuesCalculated.Invoke(sender, e);
        }
        /// <summary>
        /// This is set on invalidate, aka ExpandedXDrawOffsetCalculated, so this must not be done in the functions for the button clicks.
        /// </summary>
        private void SetLLBLS()
        {
            float totalXValueWidth = xAxisLabel.TotalXValueWidth;
            float viewDrawOffset = xAxisLabel.ViewDrawOffset;
            if (totalXValueWidth <= xAxisLabel.Width)
            {
                llblScrollToBeginning.Enabled = false;
                llblScrollBack.Enabled = false;
                llblScrollForth.Enabled = false;
                llblScrollToEnd.Enabled = false;
            }
            else if (viewDrawOffset <= totalXValueWidth - xAxisLabel.Width && viewDrawOffset > 0F)
            {
                llblScrollToBeginning.Enabled = true;
                llblScrollBack.Enabled = true;
                llblScrollForth.Enabled = true;
                llblScrollToEnd.Enabled = true;
            }
            else if (viewDrawOffset <= totalXValueWidth - xAxisLabel.Width)
            {
                llblScrollToBeginning.Enabled = false;
                llblScrollBack.Enabled = false;
                llblScrollForth.Enabled = true;
                llblScrollToEnd.Enabled = true;
            }
            else
            {
                llblScrollToBeginning.Enabled = true;
                llblScrollBack.Enabled = true;
                llblScrollForth.Enabled = false;
                llblScrollToEnd.Enabled = false;
            }
        }
        private void llblScrollToBeginning_Click(object sender, EventArgs e)
        {
            xAxisLabel.ScrollToBeginning();
            xAxisLabel.Invalidate();
        }
        private void llblScrollBack_MouseDown(object sender, MouseEventArgs e)
        {
            xAxisLabel.ScrollBack();
            xAxisLabel.Invalidate();
        }
        private void llblScrollForth_MouseDown(object sender, MouseEventArgs e)
        {
            xAxisLabel.ScrollForth();
            xAxisLabel.Invalidate();
        }
        private void llblScrollToEnd_Click(object sender, EventArgs e)
        {
            xAxisLabel.ScrollToEnd();
            xAxisLabel.Invalidate();
        }
        #endregion
    }
}
