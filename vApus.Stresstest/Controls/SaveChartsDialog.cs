/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Threading;
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

        private void btnSaveCharts_Click(object sender, EventArgs e) {
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    var doc = new SLDocument();
                    MakeCummulativeResponseTimesVsAchievedThroughputChart(doc);

                    doc.SelectWorksheet("Overview");
                    try { doc.DeleteWorksheet("Sheet1"); } catch { }

                    try {
                        doc.SaveAs(saveFileDialog.FileName);

                        this.Close();
                    } catch {
                        MessageBox.Show("Failed to save the charts because the Excel file is in use.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch {
                    MessageBox.Show("Failed to get data from the database.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void MakeCummulativeResponseTimesVsAchievedThroughputChart(SLDocument doc) {

            var dt = _resultsHelper.GetCummulativeResponseTimesVsAchievedThroughput(_cancellationTokenSource.Token, _stresstestIds); //For some strange reason the doubles are changed to string.

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
            chart.SetChartPosition(rangeHeight + 1, 0, 35, 25);
            chart.Legend.LegendPosition = DocumentFormat.OpenXml.Drawing.Charts.LegendPositionValues.Bottom;

            //Plot the throughput
            chart.PlotDataSeriesAsSecondaryLineChart(rangeWidth - 2, SLChartDataDisplayType.Normal, false);

            //Set the tiles
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

        private void picOverview_Click(object sender, EventArgs e) {
            var dialog = new ChartDialog(picOverview.Image);
            dialog.ShowDialog();
        }
    }
}
