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
    public class XAxisLabel : PictureBox
    {
        #region Events
        internal event EventHandler<DrawValuesCalculatedEventArgs> DrawValuesCalculated;
        #endregion

        #region Fields
        private ChartViewState _chartViewState;
        private AutoScrollXAxis _autoScrollXAxis;
        private Series _series;
        private SolidBrush _brush;
        private Pen _pen;

        private float _viewDrawOffset;

        private Graphics _g;
        #endregion

        #region Properties
        public ChartViewState ChartViewState
        {
            get { return _chartViewState; }
            set
            {
                _chartViewState = value;
                Invalidate();
            }
        }
        public AutoScrollXAxis AutoScrollXAxis
        {
            get
            {
                return _autoScrollXAxis;
            }
            set
            {
                if (_autoScrollXAxis != value)
                {
                    _autoScrollXAxis = value;
                    if (_autoScrollXAxis == AutoScrollXAxis.KeepAtEnd)
                        ScrollToEnd();
                    else if (_autoScrollXAxis == AutoScrollXAxis.KeepAtBeginning)
                        ScrollToBeginning();
                }
            }
        }
        public float ViewDrawOffset
        {
            get { return _viewDrawOffset; }
        }
        public float TotalXValueWidth
        {
            get
            {
                float totalXValueWidth = 0F;
                if (_g != null)
                    foreach (string xValue in _series.Keys)
                        totalXValueWidth += _g.MeasureString(xValue, Font).Width + Series.XVALUEMARGIN;
                return totalXValueWidth;
            }
        }
        #endregion

        public XAxisLabel()
        {
            InitializeComponent();
            _g = this.CreateGraphics();
        }

        #region Functions
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.BackColor = Color.White;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "XAxisLabel";
            this.Size = this.MinimumSize;
            this.ResumeLayout(false);

            this.Resize += new EventHandler(XAxisLabel_Resize);
        }

        private void XAxisLabel_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
        //Draws x values for two chart view states.
        //When collapsed overlapping x values will not be drawn so the Gui remains nice.
        //When expanded all values are drawn at an equal distance of eachother, this is determined by the width of the widest x value.
        //
        //When this is done DrawValuesCalculated is invoked to x axis who will invoke it to chart area, 
        //whereupon chartarea will send these values to chart control and forces it to render.
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_series == null || _series.Count == 0)
                return;
            Graphics g = e.Graphics;
            float xOffset, currentX;
            switch (_chartViewState)
            {
                case ChartViewState.Collapsed:
                    float lastItemRight = 0F;
                    int currentXValueIndex = 0;
                    currentX = 0F;
                    xOffset = ((float)this.Width + Series.XVALUEMARGIN - _g.MeasureString(_series.GetKeyAt(_series.Count - 1), Font).Width) / _series.Count;
                    foreach (string xValue in _series.Keys)
                    {
                        if (currentX >= lastItemRight)
                        {
                            g.DrawLine(_pen, currentX, 0, currentX, 5);
                            g.DrawString(xValue, Font, _brush, currentX, ((float)this.Height / 2) - this.Font.Size);
                            lastItemRight = currentX + g.MeasureString(xValue, Font).Width + Series.XVALUEMARGIN;
                        }
                        currentX += xOffset;
                        ++currentXValueIndex;
                    }
                    break;
                default:
                    currentX = 0F;
                    xOffset = 0F;
                    float maximizedXOffset = this.Width / _series.Count;
                    foreach (string xValue in _series.Keys)
                    {
                        float xValueWidth = g.MeasureString(xValue, Font).Width;
                        if (xValueWidth > xOffset)
                            xOffset = xValueWidth;
                    }
                    xOffset += Series.XVALUEMARGIN;
                    if (maximizedXOffset > xOffset)
                    {
                        xOffset = maximizedXOffset;
                        _viewDrawOffset = 0F;
                    }
                    else if (_autoScrollXAxis == AutoScrollXAxis.KeepAtBeginning)
                    {
                        ScrollToBeginning();
                    }
                    else if (_autoScrollXAxis == AutoScrollXAxis.KeepAtEnd || (_viewDrawOffset > 0F && _viewDrawOffset > TotalXValueWidth - this.Width - Series.XVALUEMARGIN + _g.MeasureString(_series.GetKeyAt(_series.Count - 1), Font).Width))
                    {
                        ScrollToEnd();
                    }
                    foreach (string xValue in _series.Keys)
                    {
                        if (currentX - _viewDrawOffset > this.Width)
                            break;
                        //Draw the items in the view.
                        else if (currentX >= _viewDrawOffset - xOffset)
                        {
                            g.DrawLine(_pen, currentX - _viewDrawOffset, 0, currentX - _viewDrawOffset, 5);
                            g.DrawString(xValue, Font, _brush, currentX - _viewDrawOffset, ((float)this.Height / 2) - this.Font.Size);
                        }
                        currentX += xOffset;
                    }
                    break;
            }
            if (DrawValuesCalculated != null)
                DrawValuesCalculated.Invoke(this, new DrawValuesCalculatedEventArgs(xOffset, ViewDrawOffset));
        }
        /// <summary>
        /// Only the xvalues of the highlighted series will be drawn.
        /// </summary>
        /// <param name="series"></param>
        internal void SetSeries(Series series)
        {
            _series = series;
            if (_series != null)
            {
                _brush = new SolidBrush(_series.Color);
                _pen = new Pen(_brush, 3F);
            }
            Invalidate();
        }

        /// <summary>
        /// Only usable for expanded chart view. 
        /// This will set autoscroll to keep at beginning.
        /// </summary>
        internal void ScrollToBeginning()
        {
            _autoScrollXAxis = AutoScrollXAxis.KeepAtBeginning;
            _viewDrawOffset = 0F;
        }
        /// <summary>
        /// Only usable for expanded chart view. 
        /// </summary>
        internal void ScrollBack()
        {
            _autoScrollXAxis = AutoScrollXAxis.None;
            _viewDrawOffset -= (float)this.Width / 2;
            if (_viewDrawOffset < 0)
                ScrollToBeginning();
        }
        /// <summary>
        /// Only usable for expanded chart view. 
        /// </summary>
        internal void ScrollForth()
        {
            _autoScrollXAxis = AutoScrollXAxis.None;
            _viewDrawOffset += (float)this.Width / 2;
            if (_viewDrawOffset > TotalXValueWidth - this.Width - Series.XVALUEMARGIN + _g.MeasureString(_series.GetKeyAt(_series.Count - 1), Font).Width)
                ScrollToEnd();
        }
        /// <summary>
        /// Only usable for expanded chart view.
        /// This will set autoscroll to keep at end.
        /// </summary>
        internal void ScrollToEnd()
        {
            _autoScrollXAxis = AutoScrollXAxis.KeepAtEnd;
            _viewDrawOffset = _series == null ? 0F : TotalXValueWidth - this.Width - Series.XVALUEMARGIN + _g.MeasureString(_series.GetKeyAt(_series.Count - 1), Font).Width;
        }
        #endregion
    }

    internal class DrawValuesCalculatedEventArgs : EventArgs
    {
        internal readonly float XDrawOffset, ViewDrawOffset;
        internal DrawValuesCalculatedEventArgs(float xDrawOffset, float viewDrawOffset)
        {
            XDrawOffset = xDrawOffset;
            ViewDrawOffset = viewDrawOffset;
        }
    }
}
