/*
 * Copyright 2013 (c) Sizing Servers Lab
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Results;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// Uses ResultsHelper to gather all results.
    /// </summary>
    public partial class ExportToExcelDialog : Form {

        #region Fields
        private ResultsHelper _resultsHelper;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource(); //To Cancel refreshing the report.
        /// <summary>
        /// A color pallete of 40 colors to be able to visualy match overiew and 5 heaviest user actions charts.
        /// (Filled later on)
        /// </summary>
        private List<Color> _colorPalette = new List<Color>(25);
        #endregion

        #region Constructor
        /// <summary>
        /// Uses ResultsHelper to gather all results.
        /// </summary>
        public ExportToExcelDialog() {
            InitializeComponent();
            FillColorPalette();
            saveFileDialog.FileName = "results" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
        }
        #endregion

        #region Funtions
        private void FillColorPalette() {
            _colorPalette.Add(Color.FromArgb(50, 85, 126));
            _colorPalette.Add(Color.FromArgb(128, 51, 49));
            _colorPalette.Add(Color.FromArgb(103, 125, 57));
            _colorPalette.Add(Color.FromArgb(84, 65, 107));
            _colorPalette.Add(Color.FromArgb(47, 114, 132));
            _colorPalette.Add(Color.FromArgb(166, 99, 44));

            _colorPalette.Add(Color.FromArgb(64, 105, 156));
            _colorPalette.Add(Color.FromArgb(158, 65, 62));
            _colorPalette.Add(Color.FromArgb(127, 154, 72));
            _colorPalette.Add(Color.FromArgb(105, 81, 133));
            _colorPalette.Add(Color.FromArgb(60, 141, 163));
            _colorPalette.Add(Color.FromArgb(204, 123, 56));

            _colorPalette.Add(Color.FromArgb(74, 122, 178));
            _colorPalette.Add(Color.FromArgb(181, 75, 72));
            _colorPalette.Add(Color.FromArgb(146, 177, 84));
            _colorPalette.Add(Color.FromArgb(121, 94, 153));
            _colorPalette.Add(Color.FromArgb(70, 162, 187));
            _colorPalette.Add(Color.FromArgb(233, 141, 66));

            _colorPalette.Add(Color.FromArgb(118, 150, 198));
            _colorPalette.Add(Color.FromArgb(200, 118, 116));
            _colorPalette.Add(Color.FromArgb(170, 196, 123));
            _colorPalette.Add(Color.FromArgb(149, 130, 176));
            _colorPalette.Add(Color.FromArgb(115, 184, 205));
            _colorPalette.Add(Color.FromArgb(248, 166, 113));

            _colorPalette.Add(Color.FromArgb(170, 186, 215));
            _colorPalette.Add(Color.FromArgb(217, 170, 169));
            _colorPalette.Add(Color.FromArgb(198, 214, 172));
            _colorPalette.Add(Color.FromArgb(187, 176, 201));
            _colorPalette.Add(Color.FromArgb(169, 206, 220));
            _colorPalette.Add(Color.FromArgb(250, 195, 168));

            _colorPalette.Add(Color.FromArgb(205, 214, 230));
            _colorPalette.Add(Color.FromArgb(231, 205, 205));
            _colorPalette.Add(Color.FromArgb(220, 230, 207));
            _colorPalette.Add(Color.FromArgb(214, 208, 222));
        }
        public void Init(ResultsHelper resultsHelper) {
            _resultsHelper = resultsHelper;

            var stresstests = _resultsHelper.GetStresstests();
            if (stresstests.Rows.Count == 0) {
                this.Enabled = false;
            } else {
                cboStresstest.Items.Add("<All>");
                foreach (DataRow stresstestRow in stresstests.Rows)
                    cboStresstest.Items.Add((string)stresstestRow.ItemArray[1] + " " + stresstestRow.ItemArray[2]);

                cboStresstest.SelectedIndex = 1;
            }
        }

        private void chkCharts_CheckedChanged(object sender, EventArgs e) { btnExportToExcel.Enabled = chkGeneral.Checked || chkMonitorData.Checked || chkSpecialized.Checked; }
        async private void btnExportToExcel_Click(object sender, EventArgs e) {
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                btnExportToExcel.Enabled = cboStresstest.Enabled = chkGeneral.Enabled = chkMonitorData.Enabled = chkSpecialized.Enabled = chkMonitorDataToDifferentFiles.Enabled = false;
                btnExportToExcel.Text = "Saving, can take a while...";
                int selectedIndex = cboStresstest.SelectedIndex;

                bool general = chkGeneral.Checked;
                bool monitorData = chkMonitorData.Checked;
                bool monitorDataToDifferentFiles = chkMonitorDataToDifferentFiles.Checked;
                bool specialized = chkSpecialized.Checked;

                string fileNameWithoutExtension = saveFileDialog.FileName;
                if (fileNameWithoutExtension.EndsWith(".xlsx"))
                    fileNameWithoutExtension = fileNameWithoutExtension.Substring(0, fileNameWithoutExtension.Length - 5);

                var cultureInfo = Thread.CurrentThread.CurrentCulture;
                await Task.Run(() => {
                    try {
                        Thread.CurrentThread.CurrentCulture = cultureInfo;

                        var doc = new SLDocument();

                        //Make different sheets per test.
                        var stresstests = new Dictionary<int, string>();
                        var stresstestsDt = _resultsHelper.GetStresstests();
                        if (selectedIndex == 0) {
                            foreach (DataRow row in stresstestsDt.Rows) {
                                string stresstest = row.ItemArray[1] as string;
                                if (stresstest.Contains(": ")) stresstest = stresstest.Split(':')[1];
                                stresstest += " " + (row.ItemArray[2] as string);
                                stresstests.Add((int)row.ItemArray[0], stresstest);
                            }
                        } else {
                            foreach (DataRow row in stresstestsDt.Rows) {
                                int i = (int)row.ItemArray[0];
                                if (selectedIndex == i) {
                                    string stresstest = row.ItemArray[1] as string;
                                    if (stresstest.Contains(": ")) stresstest = stresstest.Split(':')[1].TrimStart();
                                    stresstest += " " + (row.ItemArray[2] as string);
                                    stresstests.Add(i, stresstest);
                                    break;
                                }
                            }
                        }
                        stresstestsDt = null;

                        string firstWorksheet = null;
                        int worksheetIndex = 1; //Just for a unique sheet name
                        foreach (int stresstestId in stresstests.Keys) {
                            string stresstest = stresstests[stresstestId];

                            //Save general stuff
                            //----------
                            if (general) {
                                //For some strange reason the doubles are changed to string.
                                var overview = _resultsHelper.GetOverview(_cancellationTokenSource.Token, stresstestId);
                                var avgUserActions = _resultsHelper.GetAverageUserActionResults(_cancellationTokenSource.Token, stresstestId);
                                var errors = _resultsHelper.GetErrors(_cancellationTokenSource.Token, stresstestId);
                                var userActionComposition = _resultsHelper.GetUserActionComposition(_cancellationTokenSource.Token, stresstestId);


                                string worksheet = MakeCumulativeResponseTimesVsAchievedThroughputSheet(doc, overview, worksheetIndex++, stresstest + " - Overview: Response Times, Throughput & Errors");
                                if (firstWorksheet == null) firstWorksheet = worksheet;

                                MakeTop5HeaviestUserActionsSheet(doc, overview, worksheetIndex++, stresstest + " - Top 5 Heaviest User Actions");

                                int rangeWidth, rangeOffset, rangeHeight;
                                MakeWorksheet(doc, avgUserActions, worksheetIndex++, stresstest + " - Average User Actions", out rangeWidth, out rangeOffset, out rangeHeight);
                                MakeWorksheet(doc, errors, worksheetIndex++, stresstest + " - Errors", out rangeWidth, out rangeOffset, out rangeHeight);
                                MakeUserActionCompositionSheet(doc, userActionComposition, worksheetIndex++, stresstest + " - User Action Composition");

                                overview = null;
                                avgUserActions = null;
                                errors = null;
                                userActionComposition = null;
                            }

                            //Save monitor stuff
                            //----------
                            if (monitorData) {
                                var monitors = _resultsHelper.GetMonitorResults(_cancellationTokenSource.Token, stresstestId);
                                //An ugly piece of code to make 'export monitor data to different excel files' work.
                                try {
                                    foreach (DataTable monitorDt in monitors)
                                        if (monitorDt.Rows.Count != 0) {
                                            var monitor = monitorDt.Rows[0].ItemArray[1];

                                            if (monitorDataToDifferentFiles) {
                                                SLDocument monitorDoc = new SLDocument();
                                                var monitorSheet = MakeMonitorSheet(monitorDoc, monitorDt, 1, stresstest + " - " + monitor);
                                                try { monitorDoc.SelectWorksheet(monitorSheet); } catch { }
                                                try { monitorDoc.DeleteWorksheet("Sheet1"); } catch { }

                                                string monitorFileName = fileNameWithoutExtension + "_" + monitor.ToString().ReplaceInvalidWindowsFilenameChars('_') + ".xlsx";
                                                monitorDoc.SaveAs(monitorFileName);

                                            } else {
                                                string worksheet = MakeMonitorSheet(doc, monitorDt, worksheetIndex++, stresstest + " - " + monitor);
                                                if (firstWorksheet == null) firstWorksheet = worksheet;
                                            }
                                        }
                                } catch {
                                    MessageBox.Show("Failed to export because the Excel file is in use.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                                monitors = null;
                            }
                        }

                        //Save specialized stuff
                        //----------
                        if (specialized) {
                            int[] stresstestIds = new int[stresstests.Count];
                            stresstests.Keys.CopyTo(stresstestIds, 0);
                            Dictionary<string, List<string>> concurrencyAndRuns;
                            var runsOverTimeDt = _resultsHelper.GetRunsOverTime(_cancellationTokenSource.Token, out concurrencyAndRuns, stresstestIds); //This one is special, it is for multiple tests by default.

                            string worksheet = MakeRunsOverTimeSheet(doc, runsOverTimeDt, concurrencyAndRuns);
                            if (firstWorksheet == null) firstWorksheet = worksheet;
                            runsOverTimeDt = null;
                        }

                        if (firstWorksheet != null) {
                            try { doc.SelectWorksheet(firstWorksheet); } catch { }
                            try { doc.DeleteWorksheet("Sheet1"); } catch { }
                        }
                        try { doc.SaveAs(saveFileDialog.FileName); } catch {
                            MessageBox.Show("Failed to export because the Excel file is in use.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } catch {
                        MessageBox.Show("Failed to get data from the database.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }, _cancellationTokenSource.Token);

                btnExportToExcel.Text = "Export to Excel...";
                btnExportToExcel.Enabled = cboStresstest.Enabled = chkGeneral.Enabled = chkMonitorData.Enabled = chkSpecialized.Enabled = chkMonitorDataToDifferentFiles.Enabled = true;

                GC.Collect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="dt"></param>
        /// <param name="title"></param>
        /// <returns>the worksheet name</returns>
        private string MakeCumulativeResponseTimesVsAchievedThroughputSheet(SLDocument doc, DataTable dt, int worksheetIndex, string title) {
            int rangeWidth, rangeOffset, rangeHeight;
            string worksheet = MakeWorksheet(doc, dt, worksheetIndex, title, out rangeWidth, out rangeOffset, out rangeHeight);

            //Don't use the bonus column "Errors"
            --rangeWidth;
            //Plot the response times
            var chart = doc.CreateChart(rangeOffset, 1, rangeHeight + rangeOffset, rangeWidth, false, false);
            chart.SetChartType(SLColumnChartType.StackedColumn);
            chart.Legend.LegendPosition = DocumentFormat.OpenXml.Drawing.Charts.LegendPositionValues.Bottom;
            chart.SetChartPosition(rangeHeight + rangeOffset + 1, 0, rangeHeight + 45, 20);

            //Plot the throughput
            chart.PlotDataSeriesAsSecondaryLineChart(rangeWidth - 1, SLChartDataDisplayType.Normal, false);

            //Set the titles
            chart.Title.SetTitle(title.Replace("Overview: Response Times, Throughput & Errors", "Cumulative Response Times vs Achieved Throughput"));
            chart.ShowChartTitle(false);
            chart.PrimaryTextAxis.Title.SetTitle("Concurrency");
            chart.PrimaryTextAxis.ShowTitle = true;
            chart.PrimaryValueAxis.Title.SetTitle("Cumulative Response Time (ms)");
            chart.PrimaryValueAxis.ShowTitle = true;
            chart.PrimaryValueAxis.ShowMinorGridlines = true;
            chart.SecondaryValueAxis.Title.SetTitle("Throughput (responses / s)");
            chart.SecondaryValueAxis.ShowTitle = true;

            SetDataSeriesColors(chart, rangeWidth - 2, _colorPalette);

            var dso = chart.GetDataSeriesOptions(rangeWidth - 1);
            dso.Line.SetSolidLine(Color.LimeGreen, 0);
            chart.SetDataSeriesOptions(rangeWidth - 1, dso);

            doc.InsertChart(chart);

            return worksheet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="dt"></param>
        /// <param name="title"></param>
        /// <returns>the worksheet name</returns>
        private string MakeTop5HeaviestUserActionsSheet(SLDocument doc, DataTable dt, int worksheetIndex, string title) {
            //max 31 chars
            string worksheet = worksheetIndex + ") " + title.ReplaceInvalidWindowsFilenameChars(' ').Replace('/', ' ').Replace('[', ' ').Replace(']', ' ').Trim();
            if (worksheet.Length > 31) worksheet = worksheet.Substring(0, 31);
            doc.AddWorksheet(worksheet);

            if (dt.Rows.Count != 0) {
                //Make an average of all response times and use this to determine the heaviest actions.
                var averageResponseTimes = new double[dt.Columns.Count - 4];
                int rowCount = dt.Rows.Count;
                foreach (DataRow dr in dt.Rows)
                    for (int i = 2; i < dt.Columns.Count - 2; i++) {
                        var o = dr.ItemArray[i];
                        double value = (o is double ? (double)o : double.Parse(o as string));
                        averageResponseTimes[i - 2] += (value / rowCount);
                    }

                var avgRow = new List<object>(dt.Columns.Count);
                avgRow.Add("");
                avgRow.Add("");
                foreach (double value in averageResponseTimes)
                    avgRow.Add(value);
                avgRow.Add("");
                avgRow.Add("");

                //Sort the acions
                var sortedColumns = new List<int>();
                var responseTimes = new List<double>();

                //0 = stresstest, 1 = concurrency, second to last = throughput, last = errors
                for (int i = 2; i < dt.Columns.Count - 2; i++) {
                    var o = avgRow[i];
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
                    responseTimes.RemoveAt(5);
                    sortedColumns.RemoveAt(5);
                }
                if (dt.Columns.Count > 1) sortedColumns.Insert(0, 1);

                //Add data to the worksheet, only the second column and the 5 heaviest actions
                int rangeWidth = sortedColumns.Count, rangeOffset = 2, rangeHeight = dt.Rows.Count;

                //Add the title
                var titleStyle = new SLStyle();
                titleStyle.Font.Bold = true;
                titleStyle.Font.FontSize = 12d;
                doc.SetCellValue(1, 1, title);
                doc.SetCellStyle(1, 1, titleStyle);

                var colorPalette = new List<Color>(5);
                var boldStyle = new SLStyle();
                boldStyle.Font.Bold = true;
                //Add the headers and determine the colors.
                for (int i = 0; i < rangeWidth; i++) {
                    string columnName = dt.Columns[sortedColumns[i]].ColumnName;
                    doc.SetCellValue(rangeOffset, i + 1, columnName);
                    doc.SetCellStyle(rangeOffset, i + 1, boldStyle);

                    if (columnName.Contains(":")) {
                        int index = int.Parse(columnName.Split(':')[0]) - 1;
                        while (index >= _colorPalette.Count)
                            index -= _colorPalette.Count;

                        colorPalette.Add(_colorPalette[index]);
                    }
                }

                for (int rowIndex = 0; rowIndex != rangeHeight; rowIndex++) {
                    var row = dt.Rows[rowIndex].ItemArray;
                    for (int i = 0; i < sortedColumns.Count; i++) {
                        var value = row[sortedColumns[i]];
                        if (value is string) {
                            string s = value as string;
                            if (s.IsNumeric())
                                doc.SetCellValue(rowIndex + 3, i + 1, double.Parse(s));
                            else
                                doc.SetCellValue(rowIndex + 3, i + 1, s);
                        } else if (value is int) {
                            doc.SetCellValue(rowIndex + 3, i + 1, (int)value);
                        } else {
                            doc.SetCellValue(rowIndex + 3, i + 1, (double)value);
                        }

                        if (i == 0) doc.SetCellStyle(rowIndex + 3, i + 1, boldStyle);
                    }
                }

                //Plot the response times
                var chart = doc.CreateChart(rangeOffset, 1, rangeHeight + rangeOffset, rangeWidth, false, false);
                chart.SetChartType(SLColumnChartType.ClusteredColumn);
                chart.Legend.LegendPosition = DocumentFormat.OpenXml.Drawing.Charts.LegendPositionValues.Bottom;
                chart.SetChartPosition(rangeHeight + rangeOffset + 1, 0, rangeHeight + 45, 20);

                //Set the titles
                chart.Title.SetTitle(title);
                chart.ShowChartTitle(false);
                chart.PrimaryTextAxis.Title.SetTitle("Concurrency");
                chart.PrimaryTextAxis.ShowTitle = true;
                chart.PrimaryValueAxis.Title.SetTitle("Response Time (ms)");
                chart.PrimaryValueAxis.ShowTitle = true;
                chart.PrimaryValueAxis.ShowMinorGridlines = true;

                SetDataSeriesColors(chart, rangeWidth - 1, colorPalette);

                doc.InsertChart(chart);
            }

            return worksheet;
        }
        /// <summary>
        /// Format the user action comosition differently so it is more readable for customers.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="dt"></param>
        /// <param name="worksheetIndex"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private string MakeUserActionCompositionSheet(SLDocument doc, DataTable dt, int worksheetIndex, string title) {
            var userActionComposition = new DataTable("UserActionComposition");
            userActionComposition.Columns.Add("stubClm");
            userActionComposition.Columns.Add();
            userActionComposition.Columns.Add();

            var userActions = new Dictionary<string, List<string>>();
            foreach (DataRow row in dt.Rows) {
                string userAction = row.ItemArray[1] as string;
                string logEntry = (row.ItemArray[2] as string).Replace("<16 0C 02 12$>", "•");
                if (!userActions.ContainsKey(userAction)) userActions.Add(userAction, new List<string>());
                userActions[userAction].Add(logEntry);
            }

            foreach (string userAction in userActions.Keys) {
                userActionComposition.Rows.Add(string.Empty, userAction, string.Empty);
                foreach (string logEntry in userActions[userAction])
                    userActionComposition.Rows.Add(string.Empty, string.Empty, logEntry);
            }

            int rangeWidth, rangeOffset, rangeHeight;
            return MakeWorksheet(doc, userActionComposition, worksheetIndex, title, out rangeWidth, out rangeOffset, out rangeHeight, true);
        }
        private string MakeMonitorSheet(SLDocument doc, DataTable dt, int worksheetIndex, string title) {
            dt.Columns.RemoveAt(1);

            int rangeWidth, rangeOffset, rangeHeight;
            return MakeWorksheet(doc, dt, worksheetIndex++, title, out rangeWidth, out rangeOffset, out rangeHeight);
        }

        private string MakeRunsOverTimeSheet(SLDocument doc, DataTable dt, Dictionary<string, List<string>> concurrencyAndRuns) {
            string title = "Runs over Time in Minutes";
            doc.AddWorksheet(title);

            int rangeOffset = 2;
            int rangeWidth = dt.Columns.Count - 1;
            int rangeHeight = dt.Rows.Count;

            //Add the title
            var titleStyle = new SLStyle();
            titleStyle.Font.Bold = true;
            titleStyle.Font.FontSize = 12d;
            doc.SetCellValue(1, 1, title);
            doc.SetCellStyle(1, 1, titleStyle);

            var boldStyle = new SLStyle();
            boldStyle.Font.Bold = true;

            doc.SetCellValue(rangeOffset, 1, dt.Columns[0].ColumnName);
            doc.SetCellStyle(rangeOffset, 1, boldStyle);
            for (int clmIndex = 1; clmIndex < dt.Columns.Count; clmIndex++) {
                string clmName = dt.Columns[clmIndex].ColumnName;
                int i;
                if (int.TryParse(clmName, out i)) {
                    doc.SetCellValue(rangeOffset, clmIndex + 1, i);
                    doc.SetCellStyle(rangeOffset, clmIndex + 1, boldStyle);
                } else {
                    doc.SetCellValue(rangeOffset, clmIndex + 1, clmName);
                }
            }

            var formattedValues = new List<List<string>>();
            int rowOffset = 3;
            for (int rowIndex = 0; rowIndex != dt.Rows.Count; rowIndex++) {
                var row = dt.Rows[rowIndex].ItemArray;
                formattedValues.Add(new List<string>());
                for (int clmIndex = 1; clmIndex <= row.Length; clmIndex++) {
                    var value = row[clmIndex - 1];

                    if (value is System.DBNull)
                        break;

                    int rowInSheet = rowIndex + rowOffset;
                    TimeSpan ts = new TimeSpan(0);
                    if (value is string) {
                        string s = value as string;
                        if (!TimeSpan.TryParse(s, out ts)) {
                            int conIndex = s.IndexOf("Connection");
                            if (conIndex != -1) s = s.Substring(0, conIndex) + "\n" + s.Substring(conIndex);

                            doc.SetCellValue(rowInSheet, clmIndex, s);
                        }
                    } else if (value is TimeSpan) {
                        ts = (TimeSpan)value;
                    }

                    if (ts.Ticks != 0) {
                        doc.SetCellValue(rowInSheet, clmIndex, Convert.ToDouble(ts.Ticks) / TimeSpan.TicksPerMinute);
                        formattedValues[rowIndex].Add(ts.ToShortFormattedString());
                    }

                    if (clmIndex == 1) doc.SetCellStyle(rowInSheet, clmIndex, boldStyle);
                }
            }

            var chart = doc.CreateChart(rangeOffset, 1, rangeHeight + rangeOffset, rangeWidth + rangeOffset, false, false);
            chart.Title.SetTitle("Runs over Time");
            chart.ShowChartTitle(false);
            chart.HideChartLegend();

            chart.SetChartType(SLBarChartType.StackedBar);
            chart.PrimaryValueAxis.MajorUnit = 1;
            chart.PrimaryValueAxis.MinorUnit = 1.0d / 6;
            chart.PrimaryValueAxis.ShowMinorGridlines = true;
            chart.PrimaryValueAxis.Title.SetTitle("Concurrency.Run Duration in Minutes");
            chart.PrimaryValueAxis.ShowTitle = true;

            chart.PrimaryTextAxis.InReverseOrder = true;
            chart.PrimaryTextAxis.Title.SetTitle("Stresstests");
            chart.PrimaryTextAxis.ShowTitle = true;

            var runTimeOptions = new SLDataSeriesOptions();
            runTimeOptions.Fill.SetSolidFill(Color.OrangeRed, 0);
            runTimeOptions.Line.SetSolidLine(Color.LightSteelBlue, 0);



            var gapOptions = new SLDataSeriesOptions();
            gapOptions.Fill.SetNoFill();

            for (int clmIndex = 1; clmIndex <= dt.Columns.Count; clmIndex++) {
                if (Convert.ToDouble(clmIndex) % 2 == 0) {
                    chart.SetDataSeriesOptions(clmIndex, gapOptions);
                } else {
                    chart.SetDataSeriesOptions(clmIndex, runTimeOptions);

                    for (int rowIndex = 0; rowIndex != dt.Rows.Count; rowIndex++) {
                        List<string> l = concurrencyAndRuns[dt.Rows[rowIndex].ItemArray[0] as string];
                        int labelIndex = clmIndex / 2;
                        if (labelIndex >= l.Count)
                            continue;

                        var dataLabelOptions = chart.CreateDataLabelOptions();
                        dataLabelOptions.ShowValue = dataLabelOptions.ShowPercentage = dataLabelOptions.ShowSeriesName = false;
                        dataLabelOptions.SetLabelText(l[labelIndex] + "\n" + formattedValues[rowIndex][clmIndex - 1]);
                        chart.SetDataLabelOptions(clmIndex, rowIndex + 1, dataLabelOptions);
                    }
                }
            }

            chart.SetChartPosition(rangeHeight + rangeOffset + 1, 0, rangeHeight + 45, 20);

            doc.InsertChart(chart);

            return title;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="dt"></param>
        /// <param name="worksheetIndex"></param>
        /// <param name="title"></param>
        /// <param name="rangeWidth"></param>
        /// <param name="rangeHeight"></param>
        /// <returns>worksheet name</returns>
        private string MakeWorksheet(SLDocument doc, DataTable dt, int worksheetIndex, string title, out int rangeWidth, out int rangeOffset, out int rangeHeight, bool doNotAddHeaders = false) {
            //max 31 chars
            string worksheet = worksheetIndex + ") " + title.ReplaceInvalidWindowsFilenameChars(' ').Replace('/', ' ').Replace('[', ' ').Replace(']', ' ').Trim();
            if (worksheet.Length > 31) worksheet = worksheet.Substring(0, 31);
            doc.AddWorksheet(worksheet);

            rangeOffset = 2;
            rangeWidth = dt.Columns.Count - 1;
            rangeHeight = dt.Rows.Count;

            //Add the title
            var titleStyle = new SLStyle();
            titleStyle.Font.Bold = true;
            titleStyle.Font.FontSize = 12d;
            doc.SetCellValue(1, 1, title);
            doc.SetCellStyle(1, 1, titleStyle);

            var boldStyle = new SLStyle();
            boldStyle.Font.Bold = true;
            //Add the headers
            if (!doNotAddHeaders)
                for (int clmIndex = 1; clmIndex < dt.Columns.Count; clmIndex++) {
                    doc.SetCellValue(rangeOffset, clmIndex, dt.Columns[clmIndex].ColumnName);
                    doc.SetCellStyle(rangeOffset, clmIndex, boldStyle);
                }

            int rowOffset = doNotAddHeaders ? 2 : 3;
            for (int rowIndex = 0; rowIndex != dt.Rows.Count; rowIndex++) {
                var row = dt.Rows[rowIndex].ItemArray;
                for (int clmIndex = 1; clmIndex < row.Length; clmIndex++) {
                    var value = row[clmIndex];

                    int rowInSheet = rowIndex + rowOffset;
                    if (value is string) {
                        string s = value as string;
                        if (s.IsNumeric())
                            doc.SetCellValue(rowInSheet, clmIndex, double.Parse(s));
                        else
                            doc.SetCellValue(rowInSheet, clmIndex, s);
                    } else if (value is int) {
                        doc.SetCellValue(rowInSheet, clmIndex, (int)value);
                    } else if (value is long) {
                        doc.SetCellValue(rowInSheet, clmIndex, (long)value);
                    } else if (value is double) {
                        doc.SetCellValue(rowInSheet, clmIndex, (double)value);
                    } else if (value is DateTime) {
                        doc.SetCellValue(rowInSheet, clmIndex, ((DateTime)value).ToString("yyyy'-'MM'-'dd HH':'mm':'ss'.'fff"));
                    } else {
                        doc.SetCellValue(rowInSheet, clmIndex, value.ToString());
                    }

                    if (clmIndex == 1) doc.SetCellStyle(rowInSheet, clmIndex, boldStyle);
                }
            }
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

        private void SetDataSeriesColors(SLChart chart, int numberOfSeries, List<Color> colorPalette) {
            if (colorPalette.Count == 0)
                return;

            SLDataSeriesOptions dso = null;

            for (int i = 1; i <= numberOfSeries; i++) {
                dso = chart.GetDataSeriesOptions(i);

                int j = i - 1;
                while (j >= colorPalette.Count)
                    j -= colorPalette.Count;
                dso.Fill.SetSolidFill(colorPalette[j], 0);

                chart.SetDataSeriesOptions(i, dso);
            }
        }

        private void pic_Click(object sender, EventArgs e) {
            var dialog = new ChartDialog((sender as PictureBox).Image);
            dialog.ShowDialog();
        }

        private void SaveChartsDialog_FormClosing(object sender, FormClosingEventArgs e) { _cancellationTokenSource.Cancel(); }
        #endregion
    }
}
