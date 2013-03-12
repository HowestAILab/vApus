﻿using System;
using System.Collections.Generic;
using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SpreadsheetLight
{
    public partial class SLDocument
    {
        internal bool Calculate(TotalsRowFunctionValues Function, List<SLCellPoint> Points, out string ResultText)
        {
            bool result = false;
            ResultText = string.Empty;

            int i;
            int iCount = 0;
            double fTemp = 0;
            double fValue = 0;
            double fMean = 0;
            List<double> listMean = new List<double>();
            bool bFound = false;

            SLCell c;

            switch (Function)
            {
                case TotalsRowFunctionValues.None:
                    result = true;
                    ResultText = string.Empty;
                    break;
                case TotalsRowFunctionValues.Average:
                    iCount = 0;
                    fTemp = 0.0;
                    foreach (SLCellPoint pt in Points)
                    {
                        c = slws.Cells[pt];
                        if (c.DataType == CellValues.Number)
                        {
                            if (c.CellText != null)
                            {
                                if (double.TryParse(c.CellText, out fValue))
                                {
                                    ++iCount;
                                    fTemp += fValue;
                                }
                            }
                            else
                            {
                                fValue = c.NumericValue;
                                ++iCount;
                                fTemp += fValue;
                            }
                        }
                    }

                    if (iCount == 0)
                    {
                        result = false;
                        ResultText = SLConstants.ErrorDivisionByZero;
                    }
                    else
                    {
                        result = true;
                        fTemp = fTemp / iCount;
                        ResultText = fTemp.ToString(CultureInfo.InvariantCulture);
                    }
                    break;
                case TotalsRowFunctionValues.Count:
                    iCount = 0;
                    foreach (SLCellPoint pt in Points)
                    {
                        c = slws.Cells[pt];
                        if (c.CellText != null)
                        {
                            ++iCount;
                        }
                        else
                        {
                            if (c.DataType == CellValues.Number || c.DataType == CellValues.SharedString || c.DataType == CellValues.Boolean)
                            {
                                ++iCount;
                            }
                        }
                    }

                    result = true;
                    ResultText = iCount.ToString(CultureInfo.InvariantCulture);
                    break;
                case TotalsRowFunctionValues.CountNumbers:
                    iCount = 0;
                    foreach (SLCellPoint pt in Points)
                    {
                        c = slws.Cells[pt];
                        // we're not going to check the cell value itself...
                        if (c.DataType == CellValues.Number) ++iCount;
                    }

                    result = true;
                    ResultText = iCount.ToString(CultureInfo.InvariantCulture);
                    break;
                case TotalsRowFunctionValues.Maximum:
                    bFound = false;
                    fTemp = double.NegativeInfinity;
                    foreach (SLCellPoint pt in Points)
                    {
                        c = slws.Cells[pt];
                        if (c.DataType == CellValues.Number)
                        {
                            if (c.CellText != null)
                            {
                                if (double.TryParse(c.CellText, out fValue))
                                {
                                    bFound = true;
                                    if (fValue > fTemp) fTemp = fValue;
                                }
                            }
                            else
                            {
                                bFound = true;
                                if (c.NumericValue > fTemp) fTemp = c.NumericValue;
                            }
                        }
                    }

                    result = true;
                    ResultText = bFound ? fTemp.ToString(CultureInfo.InvariantCulture) : "0";
                    break;
                case TotalsRowFunctionValues.Minimum:
                    bFound = false;
                    fTemp = double.PositiveInfinity;
                    foreach (SLCellPoint pt in Points)
                    {
                        c = slws.Cells[pt];
                        if (c.DataType == CellValues.Number)
                        {
                            if (c.CellText != null)
                            {
                                if (double.TryParse(c.CellText, out fValue))
                                {
                                    bFound = true;
                                    if (fValue < fTemp) fTemp = fValue;
                                }
                            }
                            else
                            {
                                bFound = true;
                                if (c.NumericValue < fTemp) fTemp = c.NumericValue;
                            }
                        }
                    }

                    result = true;
                    ResultText = bFound ? fTemp.ToString(CultureInfo.InvariantCulture) : "0";
                    break;
                case TotalsRowFunctionValues.StandardDeviation:
                    iCount = 0;
                    fTemp = 0.0;
                    listMean = new List<double>();
                    foreach (SLCellPoint pt in Points)
                    {
                        c = slws.Cells[pt];
                        if (c.DataType == CellValues.Number)
                        {
                            if (c.CellText != null)
                            {
                                if (double.TryParse(c.CellText, out fValue))
                                {
                                    ++iCount;
                                    fTemp += fValue;
                                    listMean.Add(fValue);
                                }
                            }
                            else
                            {
                                ++iCount;
                                fTemp += c.NumericValue;
                                listMean.Add(c.NumericValue);
                            }
                        }
                    }

                    if (iCount > 0)
                    {
                        fMean = fTemp / iCount;
                        fTemp = 0.0;
                        for (i = 0; i < listMean.Count; ++i)
                        {
                            fTemp += ((fMean - listMean[i]) * (fMean - listMean[i]));
                        }
                        fTemp = Math.Sqrt(fTemp / iCount);

                        result = true;
                        ResultText = fTemp.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        result = false;
                        ResultText = SLConstants.ErrorDivisionByZero;
                    }
                    break;
                case TotalsRowFunctionValues.Sum:
                    fTemp = 0.0;
                    foreach (SLCellPoint pt in Points)
                    {
                        c = slws.Cells[pt];
                        if (c.DataType == CellValues.Number)
                        {
                            if (c.CellText != null)
                            {
                                if (double.TryParse(c.CellText, out fValue))
                                {
                                    fTemp += fValue;
                                }
                            }
                            else
                            {
                                fTemp += c.NumericValue;
                            }
                        }
                    }

                    result = true;
                    ResultText = fTemp.ToString(CultureInfo.InvariantCulture);
                    break;
                case TotalsRowFunctionValues.Variance:
                    iCount = 0;
                    fTemp = 0.0;
                    fMean = 0.0;
                    listMean = new List<double>();
                    foreach (SLCellPoint pt in Points)
                    {
                        c = slws.Cells[pt];
                        if (c.DataType == CellValues.Number)
                        {
                            if (c.CellText != null)
                            {
                                if (double.TryParse(c.CellText, out fValue))
                                {
                                    ++iCount;
                                    fMean += fValue;
                                    fTemp += (fValue * fValue);
                                }
                            }
                            else
                            {
                                ++iCount;
                                fMean += c.NumericValue;
                                fTemp += (c.NumericValue * c.NumericValue);
                            }
                        }
                    }

                    if (iCount <= 1)
                    {
                        result = false;
                        ResultText = SLConstants.ErrorDivisionByZero;
                    }
                    else
                    {
                        result = true;
                        --iCount;
                        fTemp = (fMean / iCount) - ((fTemp / iCount) * (fTemp / iCount));
                        ResultText = fTemp.ToString(CultureInfo.InvariantCulture);
                    }
                    break;
            }

            return result;
        }

        internal int GetFunctionNumber(TotalsRowFunctionValues Function)
        {
            int result = 0;
            switch (Function)
            {
                case TotalsRowFunctionValues.Average:
                    result = 101;
                    break;
                case TotalsRowFunctionValues.Count:
                    result = 103;
                    break;
                case TotalsRowFunctionValues.CountNumbers:
                    result = 102;
                    break;
                case TotalsRowFunctionValues.Maximum:
                    result = 104;
                    break;
                case TotalsRowFunctionValues.Minimum:
                    result = 105;
                    break;
                case TotalsRowFunctionValues.StandardDeviation:
                    result = 107;
                    break;
                case TotalsRowFunctionValues.Sum:
                    result = 109;
                    break;
                case TotalsRowFunctionValues.Variance:
                    result = 110;
                    break;
            }

            return result;
        }
    }
}