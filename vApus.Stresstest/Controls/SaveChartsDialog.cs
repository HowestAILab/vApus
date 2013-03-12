/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetLight;
using SpreadsheetLight.Charts;
using vApus.Results;
using vApus.Util;

namespace vApus.Stresstest {
    public partial class SaveChartsDialog : Form {
        private ResultsHelper _resultsHelper;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource(); //Cancel refreshing the report.

        public SaveChartsDialog() {
            InitializeComponent();
        }
        public void Init(ResultsHelper resultsHelper) {
            _resultsHelper = resultsHelper;

            var stresstests = _resultsHelper.GetStresstests();
            if (stresstests.Rows.Count == 0) {
                btnSaveCharts.Enabled = false;
            } else {
                cboStresstest.Items.Add("<All>");
                foreach (DataRow stresstestRow in stresstests.Rows)
                    cboStresstest.Items.Add((string)stresstestRow.ItemArray[1] + " " + stresstestRow.ItemArray[2]);

                cboStresstest.SelectedIndex = 1;
            }
        }

        async private void btnSaveCharts_Click(object sender, EventArgs e) {
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                btnSaveCharts.Enabled = cboStresstest.Enabled = false;
                btnSaveCharts.Text = "Saving, can take a while...";
                ulong selectedIndex = (ulong)cboStresstest.SelectedIndex;

                await Task.Run(() => {
                    try {
                        var doc = new SLDocument();

                        //Make different sheets per test.
                        var stresstests = new Dictionary<ulong, string>();
                        var stresstestsDt = _resultsHelper.GetStresstests();
                        if (selectedIndex == 0) {
                            foreach (DataRow row in stresstestsDt.Rows) {
                                string stresstest = row.ItemArray[1] as string;
                                if (stresstest.Contains(": ")) stresstest = stresstest.Split(':')[1];
                                stresstest += " " + (row.ItemArray[2] as string);
                                stresstests.Add(Convert.ToUInt64(row.ItemArray[0]), stresstest);
                            }
                        } else {
                            foreach (DataRow row in stresstestsDt.Rows) {
                                ulong ul = Convert.ToUInt64(row.ItemArray[0]);
                                if (selectedIndex == ul) {
                                    string stresstest = row.ItemArray[1] as string;
                                    if (stresstest.Contains(": ")) stresstest = stresstest.Split(':')[1].TrimStart();
                                    stresstest += " " + (row.ItemArray[2] as string);
                                    stresstests.Add(ul, stresstest);
                                    break;
                                }
                            }
                        }

                        string firstWorksheet = null;
                        int worksheetIndex = 1; //Just for a unique sheet name
                        foreach (ulong stresstestId in stresstests.Keys) {
                            //For some strange reason the doubles are changed to string.
                            var overview = _resultsHelper.GetCummulativeResponseTimesVsAchievedThroughput(_cancellationTokenSource.Token, stresstestId);
                            var monitors = _resultsHelper.GetMonitorResults(_cancellationTokenSource.Token, stresstestId);

                            string stresstest = stresstests[stresstestId];
                            string fws = MakeCummulativeResponseTimesVsAchievedThroughputChart(doc, overview, worksheetIndex++, stresstest);
                            if (firstWorksheet == null) firstWorksheet = fws;

                            MakeTop5HeaviestUserActionsChart(doc, overview, worksheetIndex++, stresstest);

                            foreach (DataTable monitorDt in monitors) MakeMonitorChart(doc, monitorDt, worksheetIndex++, stresstest);
                        }

                        try { doc.SelectWorksheet(firstWorksheet); } catch { }
                        try { doc.DeleteWorksheet("Sheet1"); } catch { }
                        try { doc.SaveAs(saveFileDialog.FileName); } catch {
                            MessageBox.Show("Failed to save the charts because the Excel file is in use.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } catch {
                        MessageBox.Show("Failed to get data from the database.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }, _cancellationTokenSource.Token);

                btnSaveCharts.Text = "Save Charts...";
                btnSaveCharts.Enabled = cboStresstest.Enabled = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="dt"></param>
        /// <param name="stresstest"></param>
        /// <returns>the worksheet name</returns>
        private string MakeCummulativeResponseTimesVsAchievedThroughputChart(SLDocument doc, DataTable dt, int worksheetIndex, string stresstest) {
            //max 31 chars
            string worksheet = worksheetIndex + ") " + stresstest.ReplaceInvalidWindowsFilenameChars(' ').Replace('/', ' ').Replace('[', ' ').Replace(']', ' ').Trim();
            if (worksheet.Length > 31) worksheet = worksheet.Substring(0, 31);
            doc.AddWorksheet(worksheet);

            int rangeWidth = dt.Columns.Count, rangeHeight = dt.Rows.Count + 1;

            //Add data to the worksheet
            for (int clmIndex = 0; clmIndex != dt.Columns.Count; clmIndex++)
                doc.SetCellValue(1, clmIndex + 1, dt.Columns[clmIndex].ColumnName);

            for (int rowIndex = 0; rowIndex != dt.Rows.Count; rowIndex++) {
                var row = dt.Rows[rowIndex].ItemArray;
                for (int clmIndex = 0; clmIndex != row.Length; clmIndex++) {
                    var value = row[clmIndex];

                    int rowInSheet = rowIndex + 2;
                    int clmInSheet = clmIndex + 1;
                    if (value is string) {
                        string s = value as string;
                        if (s.IsNumeric())
                            doc.SetCellValue(rowInSheet, clmInSheet, double.Parse(s));
                        else
                            doc.SetCellValue(rowInSheet, clmInSheet, s);
                    } else if (value is int) {
                        doc.SetCellValue(rowInSheet, clmInSheet, (int)value);
                    } else {
                        doc.SetCellValue(rowInSheet, clmInSheet, (double)value);
                    }
                }
            }

            //Plot the response times
            var chart = doc.CreateChart(1, 2, rangeHeight, rangeWidth, false, false);
            chart.SetChartType(SLColumnChartType.StackedColumn);
            chart.Legend.LegendPosition = DocumentFormat.OpenXml.Drawing.Charts.LegendPositionValues.Bottom;
            chart.SetChartPosition(rangeHeight + 1, 0, rangeHeight + 30, 20);

            //Plot the throughput
            chart.PlotDataSeriesAsSecondaryLineChart(rangeWidth - 2, SLChartDataDisplayType.Normal, false);

            //Set the titles
            chart.Title.SetTitle(stresstest + " Cummulative Response Times vs Achieved Throughput");
            chart.ShowChartTitle(false);
            chart.PrimaryTextAxis.Title.SetTitle("Concurrency");
            chart.PrimaryTextAxis.ShowTitle = true;
            chart.PrimaryValueAxis.Title.SetTitle("Cummulative Response Time (ms)");
            chart.PrimaryValueAxis.ShowTitle = true;
            chart.PrimaryValueAxis.ShowMinorGridlines = true;
            chart.SecondaryValueAxis.Title.SetTitle("Throughput (responses / s)");
            chart.SecondaryValueAxis.ShowTitle = true;

            doc.InsertChart(chart);

            return worksheet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="dt"></param>
        /// <param name="stresstest"></param>
        /// <returns>the worksheet name</returns>
        private string MakeTop5HeaviestUserActionsChart(SLDocument doc, DataTable dt, int worksheetIndex, string stresstest) {
            //max 31 chars
            string worksheet = worksheetIndex + ") " + stresstest.ReplaceInvalidWindowsFilenameChars(' ').Replace('/', ' ').Replace('[', ' ').Replace(']', ' ').Trim();
            if (worksheet.Length > 31) worksheet = worksheet.Substring(0, 31);
            doc.AddWorksheet(worksheet);

            //Sort the acions, only the last row is used for this
            var sortedColumns = new List<int>();
            var responseTimes = new List<double>();
            if (dt.Rows.Count != 0) {
                DataRow dr = dt.Rows[dt.Rows.Count - 1];

                //0 = stresstest, 1 = concurrency, last = throughput
                for (int i = 2; i < dt.Columns.Count - 2; i++) {
                    var o = dr.ItemArray[i];
                    double value = (o is double ? (double)o : double.Parse(o as string));

                    //Sort the columns by response time, we need the indices.
                    bool inserted = false;
                    for (int j = 0; j != responseTimes.Count; j++)
                        if (responseTimes[j] < value) {
                            responseTimes.Insert(j, value);
                            sortedColumns.Insert(j, i);
                            inserted = true;
                            break;
                        }
                    if (!inserted) {
                        responseTimes.Add(value);
                        sortedColumns.Add(i);
                    }
                }
                while (responseTimes.Count > 5) {
                    responseTimes.RemoveAt(4);
                    sortedColumns.RemoveAt(4);
                }
                if (dt.Columns.Count > 1) {
                    sortedColumns.Insert(0, 1);
                    sortedColumns.Insert(0, 0);
                }

                //Add data to the worksheet, only the first two columns and the 5 heaviest actions
                int rangeWidth = sortedColumns.Count, rangeHeight = dt.Rows.Count + 1;

                for (int i = 0; i < sortedColumns.Count; i++)
                    doc.SetCellValue(1, i + 1, dt.Columns[sortedColumns[i]].ColumnName);

                for (int rowIndex = 0; rowIndex != dt.Rows.Count; rowIndex++) {
                    var row = dt.Rows[rowIndex].ItemArray;
                    for (int i = 0; i < sortedColumns.Count; i++) {
                        var value = row[sortedColumns[i]];
                        if (value is string) {
                            string s = value as string;
                            if (s.IsNumeric())
                                doc.SetCellValue(rowIndex + 2, i + 1, double.Parse(s));
                            else
                                doc.SetCellValue(rowIndex + 2, i + 1, s);
                        } else if (value is int) {
                            doc.SetCellValue(rowIndex + 2, i + 1, (int)value);
                        } else {
                            doc.SetCellValue(rowIndex + 2, i + 1, (double)value);
                        }
                    }
                }

                //Plot the response times
                var chart = doc.CreateChart(1, 2, rangeHeight, rangeWidth, false, false);
                chart.SetChartType(SLColumnChartType.ClusteredColumn);
                chart.Legend.LegendPosition = DocumentFormat.OpenXml.Drawing.Charts.LegendPositionValues.Bottom;
                chart.SetChartPosition(rangeHeight + 1, 0, rangeHeight + 30, 20);

                //Set the titles
                chart.Title.SetTitle(stresstest + " - Top 5 Heaviest User Actions");
                chart.ShowChartTitle(false);
                chart.PrimaryTextAxis.Title.SetTitle("Concurrency");
                chart.PrimaryTextAxis.ShowTitle = true;
                chart.PrimaryValueAxis.Title.SetTitle("Response Time (ms)");
                chart.PrimaryValueAxis.ShowTitle = true;
                chart.PrimaryValueAxis.ShowMinorGridlines = true;

                doc.InsertChart(chart);
            }

            return worksheet;
        }

        private string MakeMonitorChart(SLDocument doc, DataTable dt, int worksheetIndex, string stresstest) {
            //max 31 chars
            string worksheet = worksheetIndex + ") " + stresstest.ReplaceInvalidWindowsFilenameChars(' ').Replace('/', ' ').Replace('[', ' ').Replace(']', ' ').Trim();
            if (worksheet.Length > 31) worksheet = worksheet.Substring(0, 31);
            doc.AddWorksheet(worksheet);

            int rangeWidth = dt.Columns.Count, rangeHeight = dt.Rows.Count + 1;

            //Add data to the worksheet
            for (int clmIndex = 0; clmIndex != dt.Columns.Count; clmIndex++)
                doc.SetCellValue(1, clmIndex + 1, dt.Columns[clmIndex].ColumnName);

            string monitor = null;
            for (int rowIndex = 0; rowIndex != dt.Rows.Count; rowIndex++) {
                var row = dt.Rows[rowIndex].ItemArray;
                for (int clmIndex = 0; clmIndex != row.Length; clmIndex++) {
                    var value = row[clmIndex];

                    int rowInSheet = rowIndex + 2;
                    int clmInSheet = clmIndex + 1;
                    if (value is string) {
                        string s = value as string;
                        if (s.IsNumeric())
                            doc.SetCellValue(rowInSheet, clmInSheet, double.Parse(s));
                        else {
                            doc.SetCellValue(rowInSheet, clmInSheet, s);

                            if (clmInSheet == 2 && monitor == null)
                                monitor = s;
                        }
                    } else if (value is int) {
                        doc.SetCellValue(rowInSheet, clmInSheet, (int)value);
                    } else if (value is double) {
                        doc.SetCellValue(rowInSheet, clmInSheet, (double)value);
                    } else if (value is DateTime) {
                        doc.SetCellValue(rowInSheet, clmInSheet, ((DateTime)value).ToString("yyyy'-'MM'-'dd HH':'mm':'ss'.'fff"));
                    } else {
                        doc.SetCellValue(rowInSheet, clmInSheet, value.ToString());
                    }
                }
            }

            //Since monitor charts are not particulary usefull (heterogeneous data) those are not added.
            ////Plot the monitor values
            //var chart = doc.CreateChart(1, 3, rangeHeight, rangeWidth, false, false);
            //chart.SetChartType(SLLineChartType.Line);
            //chart.Legend.LegendPosition = DocumentFormat.OpenXml.Drawing.Charts.LegendPositionValues.Bottom;
            //chart.SetChartPosition(rangeHeight + 1, 0, rangeHeight + 30, 20);

            ////Set the titles
            //chart.Title.SetTitle(stresstest + " - " + monitor);
            //chart.ShowChartTitle(false);
            
            //chart.PrimaryTextAxis.Title.SetTitle("Timestamp");
            //chart.PrimaryTextAxis.ShowTitle = true;
            ////chart.PrimaryValueAxis.Title.SetTitle("Cummulative Response Time (ms)");
            ////chart.PrimaryValueAxis.ShowTitle = true;
            //chart.PrimaryValueAxis.ShowMinorGridlines = true;

            //doc.InsertChart(chart);

            return worksheet;
        }

        /// <summary>
        /// string = stresstest, int1 = start row index, int2 = number of rows
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private Dictionary<string, KeyValuePair<int, int>> GetRowRangesPerStresstest(DataTable dt) {
            var rowRangesPerStresstest = new Dictionary<string, KeyValuePair<int, int>>(); //string = stresstest, int1 = start row index, int2 = number of rows
            foreach (DataRow row in dt.Rows) {
                string stresstest = row.ItemArray[0] as string;
                if (rowRangesPerStresstest.ContainsKey(stresstest)) {
                    var kvp = rowRangesPerStresstest[stresstest];
                    rowRangesPerStresstest[stresstest] = new KeyValuePair<int, int>(kvp.Key, kvp.Value + 1);
                } else {
                    rowRangesPerStresstest.Add(stresstest, new KeyValuePair<int, int>(rowRangesPerStresstest.Count, 1));
                }
            }
            return rowRangesPerStresstest;
        }
        private void pic_Click(object sender, EventArgs e) {
            var dialog = new ChartDialog((sender as PictureBox).Image);
            dialog.ShowDialog();
        }

        private void SaveChartsDialog_FormClosing(object sender, FormClosingEventArgs e) {
            _cancellationTokenSource.Cancel();
        }
    }
}
