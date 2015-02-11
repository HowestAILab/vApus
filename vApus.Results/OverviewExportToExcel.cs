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
                rowOffset = Do(doc, rowOffset, databaseName, token);

            try { doc.SelectWorksheet(worksheet); } catch { }
            try { doc.DeleteWorksheet("Sheet1"); } catch { }

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
                foreach (int stresstestId in stresstestIds) {
                    DataTable avgConcurrencyResults = resultsHelper.GetAverageConcurrencyResults(token, stresstestId);
                    SetCellValues(doc, row, column, avgConcurrencyResults);
                    column += avgConcurrencyResults.Columns.Count + 1;

                    Dictionary<int, string> monitors = resultsHelper.GetMonitors(new int[] { stresstestId });
                    foreach (int monitorId in monitors.Keys) {
                        DataTable avgMonitorResults = resultsHelper.GetAverageMonitorResultsByMonitorId(token, monitorId);
                        SetCellValues(doc, row, column, avgMonitorResults);
                        column += avgMonitorResults.Columns.Count + 1;
                    }
                    row += avgConcurrencyResults.Rows.Count + 4;
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
                    doc.SetCellValue(1, clmIndex + 1, dt.Columns[clmIndex].ColumnName);
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
                    if (value is string) {
                        string s = value as string;
                        if (s.IsNumeric()) doc.SetCellValue(r, c, double.Parse(s)); else doc.SetCellValue(r, c, s); //Overcoming wrong datatable conversions.
                    } else if (value is int) {
                        doc.SetCellValue(r, c, (int)value);
                    } else if (value is long) {
                        doc.SetCellValue(r, c, (long)value);
                    } else if (value is float) {
                        doc.SetCellValue(r, c, (double)(decimal)(float)value); //Ensures no formatting garbage when changing types.
                    } else if (value is double) {
                        doc.SetCellValue(r, c, (double)value);
                    } else if (value is DateTime) {
                        doc.SetCellValue(r, c, ((DateTime)value).ToString("yyyy'-'MM'-'dd HH':'mm':'ss'.'fff"));
                    } else {
                        doc.SetCellValue(r, c, value.ToString());
                    }

                    if (c == 1) doc.SetCellStyle(r, c, boldStyle);
                }
            }

            if (autoFilter) doc.Filter(1, 1, dt.Rows.Count, dt.Columns.Count);
            if (autoFitColumns) doc.AutoFitColumn(1, dt.Columns.Count, 60d);

            return name;
        }

    }
}
