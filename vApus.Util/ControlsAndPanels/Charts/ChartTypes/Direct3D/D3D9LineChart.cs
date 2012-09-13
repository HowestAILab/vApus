///*
// * Copyright 2012 (c) Sizing Servers Lab
// * University College of West-Flanders, Department GKG
// * 
// * Author(s):
// *    Dieter Vandroemme
// */
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using SlimDX;
//using SlimDX.Direct3D9;

//namespace vApus.Util
//{
//    public class D3D9LineChart : D3D9BaseChart
//    {
//        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
//        {
//            Render();
//        }
//        protected override void Render()
//        {
//            try
//            {
//                if (!this.IsHandleCreated)
//                    throw new Exception("No windowhandle created.");

//                _device.Clear(ClearFlags.Target, this.BackColor.ToArgb(), 0f, 1);
//                _device.BeginScene();

//                ////Tell the device that it does not need to apply lighting, there is not light present, if this is not done the lines will be black.
//                _device.SetRenderState(RenderState.Lighting, false);
//                ////The expected vertexes have a position and color. 
//                _device.VertexFormat = VertexFormat.Position | VertexFormat.Diffuse;

//                DrawGridLines();

//                Series HighlightedSeries = null;
//                List<Series> toDraw = new List<Series>();
//                _maximumY = 0F;
//                foreach (Series series in SeriesCollection)
//                    if (series.Count > 1)
//                        if (series.Visible)
//                        {
//                            if (series.IsHighlighted)
//                                HighlightedSeries = series;
//                            else
//                                toDraw.Add(series);
//                            if (_maximumY < series.MaximumY)
//                                _maximumY = series.MaximumY;
//                        }
//                foreach (Series series in toDraw)
//                    DrawSeries(series);

//                if (HighlightedSeries != null)
//                    DrawSeries(HighlightedSeries);

//                _device.EndScene();
//                _device.Present();
//            }
//            catch (Direct3D9Exception)
//            {
//                _device.Reset(PresentParameters);
//            }
//        }
//        protected override void DrawSeries(Series series)
//        {
//            var seriesVertexes = new List<LineVertex>();

//            float viewDrawOffset = 0F;
//            if (ChartViewState == ChartViewState.Expanded)
//                viewDrawOffset = ViewDrawOffset;

//            float currentX = 0;
//            float previousY = -1F;
//            foreach (float y in series.Values)
//            {
//                //If the view is set to expanded, all values are drawn in a scrollable pane (panel that is redrawn according to the offset)
//                //This makes sure nothing is drawn that goes out of the right bounds
//                if (currentX - viewDrawOffset > Width)
//                    break;
//                if (previousY > -1F)
//                {
//                    //And here for the left bounds
//                    //So only what is visuale is actually drawn.
//                    if (currentX >= viewDrawOffset - XDrawOffset)
//                    {
//                        //Transform and add the begin point
//                        //unlike drawing with GDI it starts from the lower left corner instead of the lower right.
//                        var beginPoint = TransformToD3DCoordinate(currentX - viewDrawOffset, Height * (previousY / _maximumY));
//                        seriesVertexes.Add(new LineVertex()
//                        {
//                            Color = series.Color.ToArgb(),
//                            Position = new Vector3(beginPoint.X, beginPoint.Y, 0.0f)
//                        });

//                        //Transform and add the end point
//                        var endPoint = TransformToD3DCoordinate((currentX - viewDrawOffset + XDrawOffset), Height * (y / _maximumY));
//                        seriesVertexes.Add(new LineVertex()
//                        {
//                            Color = series.Color.ToArgb(),
//                            Position = new Vector3(endPoint.X, endPoint.Y, 0.0f)
//                        });
//                    }
//                    currentX += XDrawOffset;
//                }
//                previousY = y;
//            }

//            var seriesVertexesArr = seriesVertexes.ToArray();
//            if (seriesVertexesArr.Length != 0)
//                _device.DrawUserPrimitives(PrimitiveType.LineList, seriesVertexesArr.Length, seriesVertexesArr);
//        }
//        /// <summary>
//        /// D3D9 expects a number between -1 and 1 (0 == the middle of the viewport)
//        /// </summary>
//        /// <param name="coordinate"></param>
//        /// <returns></returns>
//        private PointF TransformToD3DCoordinate(float x, float y)
//        {
//            return new PointF()
//            {
//                //The height per two (like percent --> per 100), minus one to have a number that fits in the range.
//                X = ((x * 2) / Width) - 1,
//                Y = ((y * 2) / Height) - 1
//            };
//        }
//    }
//}