/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace vApus.Util
{
    /// <summary>
    /// This is a very basic chart, just intended to be used in vApus. it can display series using the chosen chart type. 
    /// Series and their XYValues can not be removed or inserted just added.
    /// 
    /// This control has 2 childcontrols: ChartArea and Legend.
    /// </summary>
    public partial class Chart : UserControl
    {
        #region Events
        public event EventHandler<HighlightedSeriesChangedEventArgs> HighlightedSeriesChanged;
        #endregion

        #region Fields
        private Random random = new Random();
        private HashSet<Color> _seriesColors = new HashSet<Color>();
        private LegendViewState _legendViewState = LegendViewState.Expanded;
        #endregion

        #region Properties
        [Category("vApus Direct2D Chart")]
        public ChartViewState ChartViewState
        {
            get { return chartArea.ChartViewState; }
        }
        [Description("This will automatically scroll the x axis when in expanded view mode, this can be set also using the linklabels in this control."), DefaultValue(typeof(AutoScrollXAxis), "KeepAtEnd"), Category("vApus Direct2D Chart")]
        public AutoScrollXAxis AutoScrollXAxis
        {
            get { return chartArea.AutoScrollXAxis; }
            set { chartArea.AutoScrollXAxis = value; }
        }
        [Category("vApus Direct2D Chart")]
        public string ChartAreaTitleText
        {
            get { return chartArea.ChartAreaTitleText; }
            set { chartArea.ChartAreaTitleText = value; }
        }
        [Category("vApus Direct2D Chart")]
        public string XTitleText
        {
            get { return chartArea.XTitleText; }
            set { chartArea.XTitleText = value; }
        }
        [Category("vApus Direct2D Chart")]
        public string YTitleText
        {
            get { return chartArea.YTitleText; }
            set { chartArea.YTitleText = value; }
        }
        [Category("vApus Direct2D Chart")]
        public bool ChartAreaTitleTextVisible
        {
            get { return chartArea.ChartAreaTitleTextVisible; }
            set { chartArea.ChartAreaTitleTextVisible = value; }
        }
        [Category("vApus Direct2D Chart")]
        public bool XTitleVisible
        {
            get { return chartArea.XTitleVisible; }
            set { chartArea.XTitleVisible = value; }
        }
        [Category("vApus Direct2D Chart")]
        public bool YTitleVisible
        {
            get { return chartArea.YTitleVisible; }
            set { chartArea.YTitleVisible = value; }
        }
        [Description("The way the y axis is drawn."), Category("vApus Direct2D Chart")]
        public YAxisViewState YAxisViewState
        {
            get { return chartArea.YAxisViewState; }
            set { chartArea.YAxisViewState = value; }
        }
        [Category("vApus Direct2D Chart")]
        public ChartType ChartType
        {
            get { return chartArea.ChartType; }
            set { chartArea.ChartType = value; }
        }
        [Category("vApus Direct2D Chart")]
        public LegendViewState LegendViewState
        {
            get { return _legendViewState; }
            set
            {
                if (value == LegendViewState.Collapsed)
                    CollapseLegend();
                else
                    ExpandLegend();
            }
        }
        /// <summary>
        /// The number of series.
        /// </summary>
        [Category("vApus Direct2D Chart")]
        public int Count
        {
            get { return legend.Count; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// This is a very basic chart, just intended to be used in vApus. it can display series using the chosen chart type. 
        /// Series and their XYValues can not be removed or inserted just added.
        /// 
        /// This control has 2 childcontrols: ChartArea and Legend.
        /// </summary>
        public Chart()
        {
            InitializeComponent();
            chartArea.HighlightedSeriesChanged += new EventHandler<HighlightedSeriesChangedEventArgs>(chartArea_HighlightedSeriesChanged);
        }
        #endregion

        #region Functions
        private void Chart_SizeChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                if (_legendViewState == LegendViewState.Collapsed)
                {
                    LegendViewState = Util.LegendViewState.Expanded;
                    LegendViewState = Util.LegendViewState.Collapsed;
                }
                else
                {
                    LegendViewState = Util.LegendViewState.Collapsed;
                    LegendViewState = Util.LegendViewState.Expanded;
                }
            }
        }
        private void chartArea_HighlightedSeriesChanged(object sender, HighlightedSeriesChangedEventArgs e)
        {
            if (HighlightedSeriesChanged != null)
                HighlightedSeriesChanged(this, e);
        }
        private void llblCollapseExpand_Click(object sender, EventArgs e)
        {
            if (_legendViewState == LegendViewState.Expanded)
                CollapseLegend();
            else
                ExpandLegend();
        }
        private void CollapseLegend()
        {
            _legendViewState = LegendViewState.Collapsed;
            toolTip.Active = false;
            split.Panel2Collapsed = true;
            llblCollapseExpand.Text = "<<";
            toolTip.SetToolTip(llblCollapseExpand, "Show Legend");
            toolTip.Active = true;
            this.Invalidate(true);
        }
        private void ExpandLegend()
        {
            _legendViewState = LegendViewState.Expanded;
            toolTip.Active = false;
            split.Panel2Collapsed = false;
            llblCollapseExpand.Text = ">>";
            toolTip.SetToolTip(llblCollapseExpand, "Hide Legend");
            toolTip.Active = true;
            this.Invalidate(true);
        }
        /// <summary>
        /// Adds a new series using the given label.
        /// If a series with the same label is already present it will not be added.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public Series AddNewSeries(string label, string instance = "")
        {
            Series series = null;
            for (int i = 0; i < Count; i++)
            {
                series = GetSeriesAt(i);
                if (series.Label == label && series.Instance == instance)
                    return series;
            }

            Color color;
            do
                color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            while (!_seriesColors.Add(color));
            series = new Series(label, instance);
            series.Color = color;
            chartArea.AddSeries(series);
            legend.AddSeries(series);
            return series;
        }
        public void AddSeriesXYValue(int seriesIndex, string xValue, float yValue)
        {
            chartArea.AddSeriesXYValue(seriesIndex, xValue, yValue);
        }
        public void AddSeriesXYValue(Series series, string xValue, float yValue)
        {
            chartArea.AddSeriesXYValue(series, xValue, yValue);
        }
        public Series GetSeriesAt(int index)
        {
            return chartArea.GetSeriesAt(index);
        }
        #endregion
    }
}
