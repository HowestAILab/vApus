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
        private ulong[] _stresstestIds = new ulong[0];
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource(); //Cancel refreshing the report.

        public SaveChartsDialog() {
            InitializeComponent();
        }
        public void Init(ResultsHelper resultsHelper, params ulong[] stresstestIds) {
            _resultsHelper = resultsHelper;
            _stresstestIds = stresstestIds;
        }

        async private void btnSaveCharts_Click(object sender, EventArgs e) {
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                btnSaveCharts.Enabled = false;
                btnSaveCharts.Text = "Saving, can take a while...";

                await Task.Run(() => {
                    try {
                        var doc = new SLDocument();

                        //For some strange reason the doubles are changed to string.
                        var overview = _resultsHelper.GetCummulativeResponseTimesVsAchievedThroughput(_cancellationTokenSource.Token, _stresstestIds);

                        MakeCummulativeResponseTimesVsAchievedThroughputChart(doc, overview);
                        MakeTop5HeaviestUserActionsChart(doc, overview);

                        doc.SelectWorksheet("Overview");
                        try { doc.DeleteWorksheet("Sheet1"); } catch { }
                        try { doc.SaveAs(saveFileDialog.FileName); } catch {
                            MessageBox.Show("Failed to save the charts because the Excel file is in use.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } catch {
                        MessageBox.Show("Failed to get data from the database.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }, _cancellationTokenSource.Token);

                btnSaveCharts.Text = "Save Charts...";
                btnSaveCharts.Enabled = true;
            }
        }
        private void MakeCummulativeResponseTimesVsAchievedThroughputChart(SLDocument doc, DataTable dt) {
            doc.AddWorksheet("Overview");

            //Add data to the worksheet
            int rangeWidth = dt.Columns.Count, rangeHeight = dt.Rows.Count + 1;
            for (int clmIndex = 0; clmIndex != dt.Columns.Count; clmIndex++)
                doc.SetCellValue(1, clmIndex + 1, dt.Columns[clmIndex].ColumnName);

            for (int rowIndex = 0; rowIndex != dt.Rows.Count; rowIndex++) {
                var row = dt.Rows[rowIndex].ItemArray;
                for (int clmIndex = 0; clmIndex != row.Length; clmIndex++) {
                    var value = row[clmIndex];
                    if (value is string) {
                        string s = value as string;
                        if (s.IsNumeric())
                            doc.SetCellValue(rowIndex + 2, clmIndex + 1, double.Parse(s));
                        else
                            doc.SetCellValue(rowIndex + 2, clmIndex + 1, s);
                    } else if (value is int) {
                        doc.SetCellValue(rowIndex + 2, clmIndex + 1, (int)value);
                    } else {
                        doc.SetCellValue(rowIndex + 2, clmIndex + 1, (double)value);
                    }
                }
            }

            //Plot the response times
            var chart = doc.CreateChart(1, 2, rangeHeight, rangeWidth, false, false);
            chart.SetChartType(SLColumnChartType.StackedColumn);
            chart.Legend.LegendPosition = DocumentFormat.OpenXml.Drawing.Charts.LegendPositionValues.Bottom;
            chart.SetChartPosition(rangeHeight + 1, 0, 35, 20);

            //Plot the throughput
            chart.PlotDataSeriesAsSecondaryLineChart(rangeWidth - 2, SLChartDataDisplayType.Normal, false);

            //Set the titles
            chart.Title.SetTitle("Cummulative Response Times vs Achieved Throughput");
            chart.ShowChartTitle(false);
            chart.PrimaryTextAxis.Title.SetTitle("Concurrency");
            chart.PrimaryTextAxis.ShowTitle = true;
            chart.PrimaryValueAxis.Title.SetTitle("Cummulative Response Time (ms)");
            chart.PrimaryValueAxis.ShowTitle = true;
            chart.SecondaryValueAxis.Title.SetTitle("Throughput (responses / s)");
            chart.SecondaryValueAxis.ShowTitle = true;

            doc.InsertChart(chart);
        }
        private void MakeTop5HeaviestUserActionsChart(SLDocument doc, DataTable dt) {
            doc.AddWorksheet("Heaviest User Actions");

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
                chart.SetChartPosition(rangeHeight + 1, 0, 35, 20);

                //Set the titles
                chart.Title.SetTitle("Top 5 Heaviest User Actions");
                chart.ShowChartTitle(false);
                chart.PrimaryTextAxis.Title.SetTitle("Concurrency");
                chart.PrimaryTextAxis.ShowTitle = true;
                chart.PrimaryValueAxis.Title.SetTitle("Response Time (ms)");
                chart.PrimaryValueAxis.ShowTitle = true;


                doc.InsertChart(chart);
            }
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
