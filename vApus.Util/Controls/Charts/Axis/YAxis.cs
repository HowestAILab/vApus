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
    /// GDI+ is used for drawing the y values, Direct2D would be a bit overkill, if not this will be changed later.
    /// </summary>
    [ToolboxItem(false)]
    public class YAxis : PictureBox
    {
        #region Fields
        private ToolTip _toolTip;
        private ContextMenuStrip _cmnu;
        private ToolStripComboBox _cboView;

        private YAxisViewState _yAxisViewState;

        private Series _highlightedSeries;
        private List<Series> _seriesCollection = new List<Series>();
        private float _textWidth;
        private float _drawYOffset;

        private Font _highlightedSeriesFont;
        private SolidBrush _brush = new SolidBrush(Color.DarkGray);
        private Pen _pen = new Pen(Color.LightGray, 1.0F);

        const int NUMBEROFVALUES = 5;
        const float STARTINGYOFFSET = 25F;
        #endregion

        #region Properties
        /// <summary>
        /// The way the y axis is drawn.
        /// </summary>
        public YAxisViewState YAxisViewState
        {
            get { return _yAxisViewState; }
            set
            {
                if (_yAxisViewState != value)
                {
                    _yAxisViewState = value;
                    if (this.IsHandleCreated)
                        _cboView.SelectedIndex = (int)_yAxisViewState;
                    else
                        this.HandleCreated += new EventHandler(YAxis_HandleCreated);
                }
            }
        }
        #endregion

        #region Constructor
        public YAxis()
        {
            InitializeComponent();
            _highlightedSeriesFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _yAxisViewState = Util.YAxisViewState.OnlyShowSelected;
        }
        #endregion

        #region Functions
        private void InitializeComponent()
        {
            this._toolTip = new ToolTip();
            this._cmnu = new System.Windows.Forms.ContextMenuStrip();
            this._cmnu.SuspendLayout();
            this._cboView = new ToolStripComboBox();
            this.SuspendLayout();
            this.BackColor = Color.White;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "YAxisLabel";
            this.MinimumSize = new Size(5, 100);
            this.Size = this.MinimumSize;

            //
            // toolTip
            //
            this._toolTip.SetToolTip(this, "Right-click to set wich y-axises are shown.");

            // 
            // cmnu
            // 
            this._cmnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._cboView});
            this._cmnu.Name = "_cmnu";
            this._cmnu.Size = new System.Drawing.Size(182, 31);
            // 
            // cboView
            // 
            this._cboView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboView.Items.AddRange(new object[] {
            "Only Show Selected Series",
            "Show Selected Series and Series with Equal X Values"});
            this._cboView.Name = "cboView";
            this._cboView.Size = new System.Drawing.Size(300, 23);
            this._cboView.SelectedIndex = (int)_yAxisViewState;
            this._cboView.SelectedIndexChanged += new EventHandler(_cboView_SelectedIndexChanged);
            this.ContextMenuStrip = _cmnu;
            this._cmnu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            this.Resize += new System.EventHandler(YAxis_Resize);
        }
        private void YAxis_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= YAxis_HandleCreated;
            _cboView.SelectedIndexChanged -= _cboView_SelectedIndexChanged;
            _cboView.SelectedIndex = (int)_yAxisViewState;
            _cboView.SelectedIndexChanged += _cboView_SelectedIndexChanged;
        }
        private void _cboView_SelectedIndexChanged(object sender, EventArgs e)
        {
            _yAxisViewState = (YAxisViewState)_cboView.SelectedIndex;
            _cmnu.Hide();
            Invalidate();
        }
        private void YAxis_Resize(object sender, System.EventArgs e)
        {
            Invalidate();
        }
        internal void AddSeries(Series series)
        {
            if (!_seriesCollection.Contains(series))
            {
                _seriesCollection.Add(series);
                series.SeriesPropertiesChanged += new EventHandler(series_SeriesPropertiesChanged);
                if (_highlightedSeries == null)
                {
                    series.IsHighlighted = true;
                    series.InvokeSeriesPropertiesChanged();
                }
                this.Invalidate();
            }
        }
        private void series_SeriesPropertiesChanged(object sender, EventArgs e)
        {
            if (sender != null)
            {
                Series series = sender as Series;
                if (series.IsHighlighted)
                    _highlightedSeries = series;
            }
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_seriesCollection.Count == 0)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _drawYOffset = ((ClientRectangle.Height - (STARTINGYOFFSET * 2)) / NUMBEROFVALUES) - (this.Font.Size / 8);
            float startingYPoint = STARTINGYOFFSET - this.Font.Size / 2;
            float currentYValue;
            float differentialYValue;
            float tempTextWidth;

            _textWidth = 0F;

            List<Series> toDraw = _yAxisViewState == YAxisViewState.OnlyShowSelected ? new List<Series>(new Series[] { _highlightedSeries }) : GetEqualVisibleSeries();
            for (int i = 0; i < toDraw.Count; i++)
            {
                Series series = toDraw[i];

                currentYValue = series.MaximumY;
                differentialYValue = series.MaximumY / NUMBEROFVALUES;
                tempTextWidth = 0F;
                float width;

                string label = GetUniqueSeriesLabel(series);
                if (series.IsHighlighted)
                {
                    width = g.MeasureString(label, _highlightedSeriesFont).Width;
                    if (width > tempTextWidth)
                        tempTextWidth = width;
                    _brush.Color = series.Color;
                    g.DrawString(label, _highlightedSeriesFont, _brush, _textWidth, 3);
                }
                else
                {
                    width = g.MeasureString(label, Font).Width;
                    if (width > tempTextWidth)
                        tempTextWidth = width;
                    _brush.Color = series.Color;
                    g.DrawString(label, Font, _brush, _textWidth, 3);
                }

                _brush.Color = Color.DarkGray;
                for (float f = startingYPoint; f < ClientRectangle.Height - startingYPoint; f += _drawYOffset)
                    if (ClientRectangle.Height - startingYPoint - f < _drawYOffset)
                    {
                        width = g.MeasureString("0", Font).Width;
                        if (width > tempTextWidth)
                            tempTextWidth = width;

                        g.DrawString("0", Font, _brush, _textWidth, f);
                        break;
                    }
                    else
                    {
                        width = g.MeasureString(currentYValue.ToString(), Font).Width;
                        if (width > tempTextWidth)
                            tempTextWidth = width;

                        g.DrawString(currentYValue.ToString(), Font, _brush, _textWidth, f);
                        if (currentYValue <= 0)
                            break;
                        currentYValue -= differentialYValue;
                    }

                _textWidth += tempTextWidth + 5;
                if (i < toDraw.Count - 1)
                {
                    g.DrawLine(_pen, _textWidth, startingYPoint, _textWidth, Height - startingYPoint);
                    _textWidth += 7;
                }
            }
            _textWidth = (float)Math.Round(_textWidth, MidpointRounding.AwayFromZero);
            if (_textWidth != Width)
            {
                this.Resize -= YAxis_Resize;
                Width = (int)_textWidth;
                this.Resize += YAxis_Resize;
            }
        }
        private string GetUniqueSeriesLabel(Series series)
        {
            int occurance = 0, matches = 0;
            foreach (Series s in _seriesCollection)
            {
                if (s.Label == series.Label)
                    ++matches;
                if (s == series)
                    occurance = matches;
            }

            if (matches == 1)
                return series.Label;
            return series.Label + "/" + occurance;
        }
        private List<Series> GetEqualVisibleSeries()
        {
            bool equal = true;
            int i;
            List<Series> equalVisibleSeries = new List<Series>();
            foreach (Series series in _seriesCollection)
                if (series.Visible)
                {
                    equal = true;
                    if (series != _highlightedSeries)
                        if (series.Count == _highlightedSeries.Count)
                        {
                            i = 0;
                            foreach (string xValue1 in _highlightedSeries.Keys)
                                if (xValue1 != series.GetKeyAt(i++))
                                {
                                    equal = false;
                                    break;
                                }
                        }
                        else
                        {
                            equal = false;
                        }
                    if (equal)
                        equalVisibleSeries.Add(series);
                }
            return equalVisibleSeries;
        }
        #endregion
    }
}
