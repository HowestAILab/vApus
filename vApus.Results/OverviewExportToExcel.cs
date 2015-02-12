/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using SpreadsheetLight;
using SpreadsheetLight.Charts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.Results {
    public static class OverviewExportToExcel {
        public static void Do(string fileName, IEnumerable<string> databaseNames, CancellationToken token) {
            var doc = new SLDocument();
            string worksheet = "Overview";
            doc.AddWorksheet(worksheet);

            int rowOffset = 0;

            foreach (string databaseName in databaseNames)
                if (!token.IsCancellationRequested)
                    rowOffset = Do(doc, rowOffset, databaseName, token);

            try { doc.SelectWorksheet(worksheet); } catch { }
            try { doc.DeleteWorksheet("Sheet1"); } catch { }

            if (!token.IsCancellationRequested)
                doc.SaveAs(fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="rowOffset"></param>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="token"></param>
        /// <returns>The row offset</returns>
        private static int Do(SLDocument doc, int rowOffset, string databaseName, CancellationToken token) {
            int row = 1 + rowOffset;
            int column = 1;
            string connectionString = ConnectionStringManager.GetCurrentConnectionString(databaseName);

            using (var databaseActions = new DatabaseActions() { ConnectionString = connectionString }) {
                var resultsHelper = new ResultsHelper();
                resultsHelper.ConnectToExistingDatabase(databaseActions, databaseName);

                SetCellValue(doc, row++, column, "Description:");
                SetCellValue(doc, row++, column, resultsHelper.GetDescription());
                ++row;
                SetCellValue(doc, row++, column, "Tags:");
                SetCellValue(doc, row++, column, resultsHelper.GetTags().Combine(" "));
                ++row;

                List<int> stresstestIds = resultsHelper.GetStresstestIds();
                foreach (int stresstestId in stresstestIds)
                    if (!token.IsCancellationRequested) {
                        DataTable avgConcurrencyResults = resultsHelper.GetAverageConcurrencyResults(token, stresstestId);

                        if (avgConcurrencyResults.Rows.Count == 0) continue;
                        avgConcurrencyResults.Columns.Remove("Stresstest");

                        List<KeyValuePair<string, string>> configuration = resultsHelper.GetStresstestConfigurations(stresstestId);

                        var sb = new StringBuilder();
                        for (int i = 0; i != configuration.Count; i++) {
                            var kvp = configuration[i];
                            sb.Append(kvp.Key);
                            if (!string.IsNullOrWhiteSpace(kvp.Value)) {
                                sb.Append(": ");
                                sb.Append(kvp.Value);
                            }
                            if (i != configuration.Count - 1)
                                sb.Append(", ");
                        }

                        SetCellValue(doc, row, column, sb.ToString().Trim());

                        Dictionary<int, string> monitors = resultsHelper.GetMonitors(new int[] { stresstestId });

                        if (monitors.Count != 0)
                            foreach (var kvp in configuration)
                                if (kvp.Key == "Monitor Before" && kvp.Value != "0 minutes")
                                    avgConcurrencyResults.Rows.InsertAt(avgConcurrencyResults.NewRow(), 0);
                                else if (kvp.Key == "Monitor After" && kvp.Value != "0 minutes")
                                    avgConcurrencyResults.Rows.Add(avgConcurrencyResults.NewRow());



                        SetCellValues(doc, ++row, column, avgConcurrencyResults);
                        column += avgConcurrencyResults.Columns.Count + 1;

                        foreach (int monitorId in monitors.Keys)
                            if (!token.IsCancellationRequested) {
                                DataTable avgMonitorResults = resultsHelper.GetAverageMonitorResultsByMonitorId(token, monitorId);
                                if (avgMonitorResults.Rows.Count == 0) continue;

                                avgMonitorResults.Columns.Remove("Stresstest");
                                avgMonitorResults.Columns.Remove("Started At");
                                avgMonitorResults.Columns.Remove("Measured Time");
                                avgMonitorResults.Columns.Remove("Measured Time (ms)");
                                avgMonitorResults.Columns.Remove("Concurrency");

                                string monitor = avgMonitorResults.Rows[0].ItemArray[0] as string;
                                SetCellValue(doc, row - 1, column, monitor);

                                avgMonitorResults.Columns.Remove("Monitor");

                                SetCellValues(doc, row, column, avgMonitorResults);
                                column += avgMonitorResults.Columns.Count + 1;
                            }
                        row += avgConcurrencyResults.Rows.Count + 2;
                        column = 1;
                    }

                resultsHelper.KillConnection();
                resultsHelper = null;
            }
            return ++row;
        }

        private static void SetCellValues(SLDocument doc, int rowOffset, int columnOffset, DataTable dt) {
            var boldStyle = new SLStyle();
            boldStyle.Font.Bold = true;

            for (int c = 0; c != dt.Columns.Count; c++) {
                int column = columnOffset + c;
                doc.SetCellValue(rowOffset, column, dt.Columns[c].ColumnName);
                doc.SetCellStyle(rowOffset, column, boldStyle);
            }
            ++rowOffset;

            for (int rowIndex = 0; rowIndex != dt.Rows.Count; rowIndex++) {
                var row = dt.Rows[rowIndex].ItemArray;
                for (int columnIndex = 0; columnIndex != row.Length; columnIndex++) {
                    var value = row[columnIndex];

                    int r = rowIndex + rowOffset;
                    int c = columnIndex + columnOffset;

                    SetCellValue(doc, r, c, value);
                }
            }
        }
        private static void SetCellValue(SLDocument doc, int row, int column, object value) {
            if (value is string) {
                string s = value as string;
                if (s.IsNumeric()) doc.SetCellValue(row, column, double.Parse(s)); else doc.SetCellValue(row, column, s); //Overcoming wrong datatable conversions.
            } else if (value is int) {
                doc.SetCellValue(row, column, (int)value);
            } else if (value is long) {
                doc.SetCellValue(row, column, (long)value);
            } else if (value is float) {
                doc.SetCellValue(row, column, (double)(decimal)(float)value); //Ensures no formatting garbage when changing types.
            } else if (value is double) {
                doc.SetCellValue(row, column, (double)value);
            } else if (value is DateTime) {
                doc.SetCellValue(row, column, ((DateTime)value).ToString("yyyy'-'MM'-'dd HH':'mm':'ss'.'fff"));
            } else {
                doc.SetCellValue(row, column, value.ToString());
            }
        }
    }
}
