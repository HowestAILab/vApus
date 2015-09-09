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
using System.Linq;

namespace vApus.Results {
    public static class OverviewExportToExcel {
        public static void Do(string fileName, IEnumerable<string> databaseNames, bool includeFullMonitorResults, CancellationToken token) {
            var doc = new SLDocument();
            string firstWorksheet = "Overview";
            doc.AddWorksheet(firstWorksheet);
            doc.AddWorksheet("Machine configurations");

            int rowOffset1 = 0;
            int rowOffset2 = 0;

            foreach (string databaseName in databaseNames) {
                string connectionString = ConnectionStringManager.GetCurrentConnectionString(databaseName);

                using (var databaseActions = new DatabaseActions() { ConnectionString = connectionString }) {
                    var resultsHelper = new ResultsHelper();
                    resultsHelper.ConnectToExistingDatabase(databaseActions, databaseName);

                    if (!token.IsCancellationRequested)
                        rowOffset1 = MakeOverviewSheet(doc, rowOffset1, resultsHelper, token);
                    if (!token.IsCancellationRequested)
                        rowOffset2 = MakeMachineConfigSheet(doc, rowOffset2, resultsHelper, token);

                    if (includeFullMonitorResults)
                        if (!token.IsCancellationRequested)
                            MakeMonitorSheets(doc, resultsHelper, token);

                    resultsHelper.KillConnection();
                    resultsHelper = null;
                }
            }



            try { doc.SelectWorksheet(firstWorksheet); } catch { }
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
        private static int MakeOverviewSheet(SLDocument doc, int rowOffset, ResultsHelper resultsHelper, CancellationToken token) {
            doc.SelectWorksheet("Overview");

            int row = 1 + rowOffset;
            int column = 1;

            SetCellValue(doc, row++, column, "Description:");
            SetCellValue(doc, row++, column, resultsHelper.GetDescription());
            ++row;
            SetCellValue(doc, row++, column, "Tags:");
            SetCellValue(doc, row++, column, resultsHelper.GetTags().Combine(" "));
            ++row;

            List<int> stressTestIds = resultsHelper.GetStressTestIds();
            foreach (int stressTestId in stressTestIds)
                if (!token.IsCancellationRequested) {
                    DataTable avgConcurrencyResults = resultsHelper.GetAverageConcurrencyResults(token, stressTestId);

                    if (avgConcurrencyResults.Rows.Count == 0) continue;
                    avgConcurrencyResults.Columns.Remove("Stress test");

                    List<KeyValuePair<string, string>> configuration = resultsHelper.GetStressTestConfigurations(stressTestId);

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

                    Dictionary<int, string> monitors = resultsHelper.GetMonitors(new int[] { stressTestId });

                    if (monitors.Count != 0)
                        foreach (var kvp in configuration)
                            if (kvp.Key == "Monitor before" && kvp.Value != "0 minutes")
                                avgConcurrencyResults.Rows.InsertAt(avgConcurrencyResults.NewRow(), 0);
                            else if (kvp.Key == "Monitor after" && kvp.Value != "0 minutes")
                                avgConcurrencyResults.Rows.Add(avgConcurrencyResults.NewRow());



                    SetCellValues(doc, ++row, column, avgConcurrencyResults);
                    column += avgConcurrencyResults.Columns.Count + 1;

                    foreach (int monitorId in monitors.Keys)
                        if (!token.IsCancellationRequested) {
                            DataTable avgMonitorResults = resultsHelper.GetAverageMonitorResultsByMonitorId(token, monitorId);
                            if (avgMonitorResults.Rows.Count == 0) continue;

                            avgMonitorResults.Columns.Remove("Stress test");
                            avgMonitorResults.Columns.Remove("Started at");
                            avgMonitorResults.Columns.Remove("Measured time");
                            avgMonitorResults.Columns.Remove("Measured time (ms)");
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

            return ++row;
        }
        private static int MakeMachineConfigSheet(SLDocument doc, int rowOffset, ResultsHelper resultsHelper, CancellationToken token) {
            doc.SelectWorksheet("Machine configurations");

            int row = 1 + rowOffset;
            int column = 1;

            SetCellValue(doc, row++, column, "Description:");
            SetCellValue(doc, row++, column, resultsHelper.GetDescription());
            ++row;
            SetCellValue(doc, row++, column, "Tags:");
            SetCellValue(doc, row++, column, resultsHelper.GetTags().Combine(" "));
            ++row;

            List<int> stressTestIds = resultsHelper.GetStressTestIds();
            foreach (int stressTestId in stressTestIds)
                if (!token.IsCancellationRequested) {
                    DataTable machineConfigs = resultsHelper.GetMachineConfigurations(token, stressTestId);

                    if (machineConfigs.Rows.Count == 0) continue;
                    machineConfigs = Prep(machineConfigs);

                    List<KeyValuePair<string, string>> configuration = resultsHelper.GetStressTestConfigurations(stressTestId);

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
                    SetCellValues(doc, ++row, column, machineConfigs);

                    row += machineConfigs.Rows.Count + 2;
                }

            return ++row;
        }
        private static void MakeMonitorSheets(SLDocument doc, ResultsHelper resultsHelper, CancellationToken token) {
            List<int> stressTestIds = resultsHelper.GetStressTestIds();
            foreach (int stressTestId in stressTestIds)
                if (!token.IsCancellationRequested) {
                    Dictionary<int, string> monitorIdsAndNames = resultsHelper.GetMonitors(new int[] { stressTestId });
                    foreach (int monitorId in monitorIdsAndNames.Keys)
                        if (!token.IsCancellationRequested) {
                            DataTable dt = Prep(resultsHelper.GetMonitorResultsByMonitorId(token, monitorId));
                            dt.Columns.Remove("Monitor");

                            string firstWorksheet = MakeWorksheet(doc, dt, (doc.GetSheetNames().Count - 2) + " " + monitorIdsAndNames[monitorId], false, true);
                        }
                }
        }

        private static string MakeWorksheet(SLDocument doc, DataTable dt, string name, bool autoFilter, bool autoFitColumns, bool includeHeaders = true) {
            //max 31 chars
            name = name.ReplaceInvalidWindowsFilenameChars(' ').Replace('/', ' ').Replace('[', ' ').Replace(']', ' ').Trim();
            if (name.Length > 31) name = name.Substring(0, 31);
            doc.AddWorksheet(name);

            var boldStyle = new SLStyle();
            boldStyle.Font.Bold = true;

            //Add the headers
            if (includeHeaders)
                for (int clmIndex = 0; clmIndex != dt.Columns.Count; clmIndex++) {
                    SetCellValue(doc, 1, clmIndex + 1, dt.Columns[clmIndex].ColumnName);
                    doc.SetCellStyle(1, clmIndex + 1, boldStyle);
                }

            int rowOffset = includeHeaders ? 2 : 1;
            int columnOffset = 1;
            for (int rowIndex = 0; rowIndex != dt.Rows.Count; rowIndex++) {
                var row = dt.Rows[rowIndex].ItemArray;
                for (int columnIndex = 0; columnIndex != row.Length; columnIndex++) {
                    var value = row[columnIndex];

                    int r = rowIndex + rowOffset;
                    int c = columnIndex + columnOffset;
                    SetCellValue(doc, r, c, value);

                    if (c == 1) doc.SetCellStyle(r, c, boldStyle);
                }
            }

            if (autoFilter) doc.Filter(1, 1, dt.Rows.Count, dt.Columns.Count);
            if (autoFitColumns) doc.AutoFitColumn(1, dt.Columns.Count, 60d);

            return name;
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
                DateTime dt = (DateTime)value;
                TimeSpan ts = new TimeSpan(dt.Ticks);
                string dtString = ts.TotalSeconds < 1d ? dt.ToString("yyyy'-'MM'-'dd HH':'mm':'ss'.'fff") : dt.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");

                doc.SetCellValue(row, column, dtString);
            } else {
                doc.SetCellValue(row, column, value.ToString());
            }
        }
        /// <summary>
        /// Makes a new data table without the 'Stress test' column (if any) and "<16 0C 02 12$>" replaced by "•".
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static DataTable Prep(DataTable dt) {
            DataTable copy = dt.Copy();

            if (copy.Columns.Count != 0) {
                if (copy.Columns[0].ColumnName == "Stress test")
                    copy.Columns.RemoveAt(0);

                var clone = copy.Clone();

                //A whole lot of fuzz to overcome the 32 767 character limit for a cell value in an Excel spreadsheet.
                var addExtraColumns = new Dictionary<int, int>(); //Index, number;
                foreach (DataRow row in copy.Rows) {
                    object[] arr = row.ItemArray;
                    for (int i = 0; i != arr.Length; i++)
                        if (arr[i] is string) {
                            string s = arr[i] as string;
                            s = s.Replace("<16 0C 02 12$>", "•");
                            arr[i] = s;

                            int extraColumns = s.Length / 32767; // Excel cell limit;
                            if (extraColumns != 0) {
                                int atIndex = i + 1;
                                if (!addExtraColumns.ContainsKey(atIndex)) addExtraColumns.Add(atIndex, extraColumns);
                                else if (addExtraColumns[atIndex] < extraColumns) addExtraColumns[atIndex] = extraColumns;
                            }
                        }
                    clone.Rows.Add(arr);
                }

                if (addExtraColumns.Count != 0) {
                    copy = clone;
                    clone = copy.Clone();

                    int[] temp = new int[addExtraColumns.Count];
                    addExtraColumns.Keys.CopyTo(temp, 0);
                    var keys = temp.OrderByDescending(x => x);
                    foreach (int key in keys) {
                        string prefix = copy.Columns[key - 1].ColumnName + "_";
                        for (int j = 0; j != addExtraColumns[key]; j++) {
                            DataColumn column = clone.Columns.Add(prefix + (j + 2));
                            int atIndex = key + j;
                            if (atIndex < clone.Columns.Count - 1)
                                column.SetOrdinal(atIndex);
                        }
                    }

                    foreach (DataRow row in copy.Rows) {
                        object[] arr = row.ItemArray;
                        var l = new List<object>();
                        for (int i = 0; i != arr.Length; i++) {
                            if (arr[i] is string) {
                                int extraColumns = 0;
                                int atIndex = i + 1;
                                if (addExtraColumns.ContainsKey(atIndex)) extraColumns = addExtraColumns[atIndex];

                                string s = arr[i] as string;
                                while (s.Length > 32767) { //Excel cell limit.
                                    --extraColumns; //Correct this.
                                    l.Add(s.Substring(0, 32767));
                                    s = s.Substring(32767);
                                }
                                if (s.Length > 0) l.Add(s);

                                for (int j = 0; j != extraColumns; j++)
                                    l.Add(string.Empty);
                            } else {
                                l.Add(arr[i]);
                            }
                        }

                        clone.Rows.Add(l.ToArray());
                    }

                    copy = clone;
                }
            }

            return copy;
        }
    }
}
