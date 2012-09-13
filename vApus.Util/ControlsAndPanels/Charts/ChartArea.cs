/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util
{
    [ToolboxItem(false)]
    public partial class ChartArea : UserControl
    {
        #region Events
        public event EventHandler<HighlightedSeriesChangedEventArgs> HighlightedSeriesChanged;
        #endregion

        #region Fields
        private ChartType _chartType;
        private BaseChart _chartControl = null;
        #endregion

        #region Properties

        //The designer cannot handle internal properties, which I find silly, therefore they are public. *Sigh*

        public ChartViewState ChartViewState
        {
            get { return _chartControl .ChartViewState; }
            private set
            {
                xAxis.ChartViewState = value;
                _chartControl .ChartViewState = value;
            }
        }
        public AutoScrollXAxis AutoScrollXAxis
        {
            get { return xAxis.AutoScrollXAxis; }
            set { xAxis.AutoScrollXAxis = value; }
        }
        public string ChartAreaTitleText
        {
            get { return lblChartAreaTitle.Text; }
            set { lblChartAreaTitle.Text = value; }
        }
        public string XTitleText
        {
            get { return lblXTitle.Text; }
            set { lblXTitle.Text = value; }
        }
        public string YTitleText
        {
            get { return lblYTitle.Text; }
            set { lblYTitle.Text = value; }
        }
        public bool ChartAreaTitleTextVisible
        {
            get { return lblChartAreaTitle.Visible; }
            set { lblChartAreaTitle.Visible = value; }
        }
        public bool XTitleVisible
        {
            get { return lblXTitle.Visible; }
            set { lblXTitle.Visible = value; }
        }
        public bool YTitleVisible
        {
            get { return lblYTitle.Visible; }
            set { lblYTitle.Visible = value; }
        }
        /// <summary>
        /// The way the y axis is drawn.
        /// </summary>
        public YAxisViewState YAxisViewState
        {
            get { return yAxis.YAxisViewState; }
            set { yAxis.YAxisViewState = value; }
        }
        public ChartType ChartType
        {
            get { return _chartType; }
            set
            {
                if (_chartControl == null || _chartType != value)
                {
                    _chartType = value;

                    switch (_chartType)
                    {
                        case ChartType.LineChart:
                            _chartControl = new GDIPlusLineChart();

                            //Not used, some real issues (device lost and such) and line drawing is not all that
                            //it looks not good to say the least.
                            //_chartControl = new D3D9LineChart();
                            break;
                    }
                    
                    cboView.SelectedIndexChanged -= cboView_SelectedIndexChanged;
                    cboView.SelectedIndex = (int)ChartViewState;
                    cboView.SelectedIndexChanged += cboView_SelectedIndexChanged;
                    _chartControl.MouseEnter += new System.EventHandler(_chartControl_MouseEnter);
                    _chartControl.MouseLeave += new System.EventHandler(_chartControl_MouseLeave);
                }
                _chartControl.Size = pnlChartControlHolder.ClientRectangle.Size;
                _chartControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
                _chartControl.BackColor = Color.White;
            }
        }
        #endregion

        #region Constructor
        public ChartArea()
        {
            InitializeComponent();
            this.HandleCreated += new System.EventHandler(ChartArea_HandleCreated);
        }
        #endregion

        #region Functions
        private void ChartArea_SizeChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                splitContainer.Panel2Collapsed = true;
                splitContainer.Panel2Collapsed = false;
            }
        }
        private void ChartArea_HandleCreated(object sender, System.EventArgs e)
        {
            ChartType = _chartType;
            pnlChartControlHolder.Controls.Add(_chartControl);
            //Transform dimensions if the yAxis width changes.
            yAxis.Resize += new EventHandler(yAxis_Resize);
            xAxis.DrawValuesCalculated += new EventHandler<DrawValuesCalculatedEventArgs>(xAxis_DrawOffsetCalculated);
        }
        private void yAxis_Resize(object sender, EventArgs e)
        {
            //A SplitContainer is used for resizing correctly, using a High level solution for a high level way of programming works,
            //setting the dimensions of the other controls right if the width of the y axis changes can sadly enough not be done manual.
            splitContainer.SplitterDistance = yAxis.Right;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            splitContainer.SplitterDistance = yAxis.Right;
        }
        //Rendering the chart occurs when the x axis calculated the draw offsets.
        private void xAxis_DrawOffsetCalculated(object sender, DrawValuesCalculatedEventArgs e)
        {
            _chartControl.XDrawOffset = e.XDrawOffset;
            _chartControl.ViewDrawOffset = e.ViewDrawOffset;
            _chartControl.Invalidate();
        }

        #region Mnu & cbo view
        private void pnlChartControlHolder_MouseEnter(object sender, EventArgs e)
        {
            pnlCBOView.Visible = true;
        }
        private void pnlChartControlHolder_MouseLeave(object sender, EventArgs e)
        {
            pnlCBOView.Visible = false;
        }
        private void _chartControl_MouseEnter(object sender, System.EventArgs e)
        {
            pnlCBOView.Visible = true;
        }
        private void _chartControl_MouseLeave(object sender, System.EventArgs e)
        {
            if (HidePnlCBOView())
                pnlCBOView.Visible = false;
        }
        private void mnu_MouseLeave(object sender, System.EventArgs e)
        {
            if (HidePnlCBOView())
                pnlCBOView.Visible = false;
        }
        private bool HidePnlCBOView()
        {
            Point cursorPosition = PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));
            return cursorPosition.X <= splitContainer.Left + splitContainer.Panel2.Left ||
                    cursorPosition.X >= splitContainer.Right - 1 ||
                    cursorPosition.Y <= splitContainer.Top + pnlChartControlHolder.Top + 1 ||
                    cursorPosition.Y >= splitContainer.Top + pnlChartControlHolder.Bottom;
        }
        private void cboView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChartViewState chartViewState = (ChartViewState)cboView.SelectedIndex;
            ChartViewState = chartViewState;
            _chartControl.Left = xAxis.XAxisLabelLeft;
            _chartControl.Width = xAxis.XAxisLabelWidth;
        }
        #endregion

        internal void AddSeries(Series series)
        {
            series.SeriesPropertiesChanged += new System.EventHandler(series_SeriesPropertiesChanged);
            _chartControl .AddSeries(series);
            yAxis.AddSeries(series);
        }
        private void series_SeriesPropertiesChanged(object sender, System.EventArgs e)
        {
            Series series = sender as Series;
            xAxis.SetSeries(series);
            if (series != null && series.IsHighlighted && HighlightedSeriesChanged != null)
                HighlightedSeriesChanged(this, new HighlightedSeriesChangedEventArgs(series));
        }
        internal void AddSeriesXYValue(int seriesIndex, string xValue, float yValue)
        {
            Series series = _chartControl .AddSeriesXYValue(seriesIndex, xValue, yValue);
            xAxis.Invalidate(true);
            yAxis.Invalidate();
            if (series.IsHighlighted && HighlightedSeriesChanged != null)
                HighlightedSeriesChanged(this, new HighlightedSeriesChangedEventArgs(series));
        }
        internal void AddSeriesXYValue(Series series, string xValue, float yValue)
        {
            _chartControl .AddSeriesXYValue(series, xValue, yValue);
            xAxis.Invalidate(true);
            yAxis.Invalidate();
            if (series.IsHighlighted && HighlightedSeriesChanged != null)
                HighlightedSeriesChanged(this, new HighlightedSeriesChangedEventArgs(series));
        }
        internal Series GetSeriesAt(int index)
        {
            return _chartControl .GetSeriesAt(index);
        }
        #endregion

        private void ChartArea_Validated(object sender, EventArgs e)
        {
            _chartControl.Invalidate(true);
        }
    }
    public class HighlightedSeriesChangedEventArgs : EventArgs
    {
        public readonly Series HighlightedSeries;
        public HighlightedSeriesChangedEventArgs(Series highlightedSeries)
        {
            HighlightedSeries = highlightedSeries;
        }
    }
}
