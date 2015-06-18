/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Util;
using System.Linq;
using System.Diagnostics;

namespace vApus.Results {
    /// <summary>
    /// Uses ResultsHelper to gather all results.
    /// </summary>
    public partial class RichExportToExcelDialog : Form {

        #region Fields
        private ResultsHelper _resultsHelper;
        private Form _parentToClose;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource(); //To Cancel refreshing the report.
        /// <summary>
        /// A color pallete of 40 colors to be able to visualy match overiew and 5 heaviest user actions charts.
        /// (Filled later on)
        /// </summary>
        private List<Color> _colorPalette = new List<Color>(34);

        private IEnumerable<string> _toExport;

        private string _autoExportFolder = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Uses ResultsHelper to gather all results.
        /// </summary>
        public RichExportToExcelDialog() {
            InitializeComponent();
            FillColorPalette();

            tvw.VisibleChanged += tvw_VisibleChanged;
        }

        private void tvw_VisibleChanged(object sender, EventArgs e) {
            if (tvw.Visible) {
                tvw.VisibleChanged -= tvw_VisibleChanged;
                foreach (TreeNode node in tvw.Nodes)
                    RefreshTreeNode(node);

                if (_autoExportFolder.Length != 0 && cboStressTest.SelectedIndex == 0) {
                    foreach (TreeNode node in tvw.Nodes)
                        node.Checked = true;

                    Export(_autoExportFolder);
                }
            }
        }

        private bool RefreshTreeNode(TreeNode node) {
            if (node != null) {
                bool succes = true;
                string text = node.Text;

                node.Text = "";
                var font = new Font(tvw.Font, FontStyle.Regular);
                if (node.NodeFont != null)
                    font = new Font(node.NodeFont, node.NodeFont.Bold ? FontStyle.Bold : FontStyle.Regular);

                node.NodeFont = font;
                node.Text = text;

                if (!node.IsExpanded && node.Nodes.Count != 0) node.Expand();
                foreach (TreeNode childNode in node.Nodes)
                    if (!RefreshTreeNode(childNode))
                        succes = false;
                return succes;
            }
            return false;
        }

        #endregion

        #region Funtions
        /// <summary>
        /// Colors for different series in Excel charts.
        /// </summary>
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

        public void Init(ResultsHelper resultsHelper, Form parentToClose) {
            _parentToClose = parentToClose;
            Init(resultsHelper);
            this.VisibleChanged += RichExportToExcelDialog_VisibleChanged;
        }

        private void RichExportToExcelDialog_VisibleChanged(object sender, EventArgs e) {
            if (this.Visible) {
                this.VisibleChanged -= RichExportToExcelDialog_VisibleChanged;
                _parentToClose.Hide();
                _parentToClose.Close();
            }
        }
        public void Init(ResultsHelper resultsHelper) {
            StringCollection selectedGoals = Properties.Settings.Default.ExportToExcelSelectedGoals as StringCollection;
            if (selectedGoals != null) {
                string[] toExport = new string[selectedGoals.Count];
                selectedGoals.CopyTo(toExport, 0);
                _toExport = new List<string>(toExport);
            }

            _resultsHelper = resultsHelper;

            var stressTests = _resultsHelper.GetStressTests();
            if (stressTests != null && stressTests.Rows.Count == 0) {
                this.Enabled = false;
            } else {
                if (stressTests.Rows.Count > 1)
                    cboStressTest.Items.Add("<All>");
                foreach (DataRow stressTestRow in stressTests.Rows)
                    cboStressTest.Items.Add((string)stressTestRow.ItemArray[1] + " " + stressTestRow.ItemArray[2]);

                cboStressTest.SelectedIndex = 0;
            }
        }

        private void cboStressTest_SelectedIndexChanged(object sender, EventArgs e) {
            tvw.Nodes.Clear();

            var stressTestIds = new int[1];
            if (cboStressTest.Items.Count == 1)
                stressTestIds[0] = cboStressTest.SelectedIndex + 1;
            else
                stressTestIds[0] = cboStressTest.SelectedIndex == 0 ? -1 : cboStressTest.SelectedIndex;

            tvw.Nodes.AddRange(RichExportToExcel.GetTreeNodes(tvw.Font, _resultsHelper, stressTestIds));

            if (tvw.Visible)
                foreach (TreeNode node in tvw.Nodes)
                    RefreshTreeNode(node);

            //Load settings in gui.
            if (_toExport != null) {
                tvw.AfterCheck -= tvw_AfterCheck;

                var paths = new List<string>();
                foreach (string fullPath in _toExport) {
                    string[] splitted = fullPath.Split(new string[] { "'/'" }, StringSplitOptions.None);
                    string path = string.Empty;
                    foreach (string s in splitted) {
                        path = path.Length == 0 ? s : path + "'/'" + s;
                        if (!paths.Contains(path)) paths.Add(path);
                    }
                }

                UncheckAllTreeNodes(tvw.Nodes);
                foreach (TreeNode node in GetAllNodes(tvw.Nodes))
                    if (paths.Contains(node.FullPath))
                        node.Checked = true;

				//Double check for dynamic added nodes if they can be checked (is at least one child node checked?)
                foreach (TreeNode node in GetAllNodes(tvw.Nodes))
                    if (node.Checked && node.Nodes.Count != 0) {
                        bool canBeChecked = false;
                        foreach (TreeNode child in node.Nodes)
                            if (child.Checked) {
                                canBeChecked = true;
                                break;
                            }

                        if (!canBeChecked) node.Checked = false;
                    }

                tvw.AfterCheck += tvw_AfterCheck;
            }
        }
        private static void UncheckAllTreeNodes(TreeNodeCollection nodes) {
            foreach (TreeNode node in nodes) {
                node.Checked = false;
                UncheckAllTreeNodes(node.Nodes);
            }
        }
        private static IEnumerable<TreeNode> GetAllNodes(TreeNodeCollection nodes) {
            foreach (TreeNode node in nodes) {
                yield return node;
                foreach (TreeNode childnode in GetAllNodes(node.Nodes))
                    yield return childnode;
            }
        }

        private void tvw_AfterCheck(object sender, TreeViewEventArgs e) {
            tvw.AfterCheck -= tvw_AfterCheck;
            RichExportToExcel.HandleTreeNodeChecked(e.Node);

            ExtractToExport();

            btnExportToExcel.Enabled = _toExport.Count() != 0;

            tvw.AfterCheck += tvw_AfterCheck;
        }
        private void ExtractToExport() {
            _toExport = RichExportToExcel.ExtractToExport(tvw.Nodes, tvw.PathSeparator);

            //Save settings.
            Properties.Settings.Default.ExportToExcelSelectedGoals = new StringCollection();

            foreach (string s in _toExport)
                Properties.Settings.Default.ExportToExcelSelectedGoals.Add(s);

            Properties.Settings.Default.Save();
        }

        private void btnExportToExcel_Click(object sender, EventArgs e) { Export(); }

        async private void Export(string autoExportFolder = "") {
            saveFileDialog.FileName = Path.Combine(autoExportFolder, _resultsHelper.DatabaseName.ReplaceInvalidWindowsFilenameChars('_'));
            if (autoExportFolder.Length != 0 || saveFileDialog.ShowDialog() == DialogResult.OK) {
                btnExportToExcel.Enabled = btnOverviewExport.Enabled = cboStressTest.Enabled = tvw.Enabled = false;
                btnExportToExcel.Text = "Exporting...";

                string zipPath = saveFileDialog.FileName;
                if (!zipPath.EndsWith(".zip")) zipPath += ".zip";

                if (File.Exists(zipPath))
                    try {
                        File.Delete(zipPath);
                    } catch (Exception ex) {
                        Loggers.Log(Level.Warning, "Failed deleting zipped Excel results.", ex, new object[] { zipPath });
                    }

                var stressTestIds = new int[1];
                if (cboStressTest.Items.Count == 1)
                    stressTestIds[0] = cboStressTest.SelectedIndex + 1;
                else
                    stressTestIds[0] = cboStressTest.SelectedIndex == 0 ? -1 : cboStressTest.SelectedIndex;

                ExtractToExport();

                bool exceptionThrown = false;
                var cultureInfo = Thread.CurrentThread.CurrentCulture;
                await Task.Run(() => {
                    try {
                        Thread.CurrentThread.CurrentCulture = cultureInfo;

                        RichExportToExcel.Do(zipPath, stressTestIds, _resultsHelper, _toExport, _cancellationTokenSource.Token);
                    } catch (Exception ex) {
                        exceptionThrown = true;
                        Loggers.Log(Level.Error, "Failed to export results to Excel.", ex);
                        MessageBox.Show("Failed to export results to Excel.\nCheck the error log for details.");
                    }
                }, _cancellationTokenSource.Token);

                btnExportToExcel.Text = "Export to Excel...";
                btnExportToExcel.Enabled = btnOverviewExport.Enabled = cboStressTest.Enabled = tvw.Enabled = true;

                GC.WaitForPendingFinalizers();
                GC.Collect();

                if (!exceptionThrown) {
                    if (MessageBox.Show("Results where exported to " + zipPath + ".\nDo you want to browse them?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        Process.Start(zipPath);
                    this.Close();
                }

                //Save specialized stuff
                //----------
                //if (specialized) {
                //    var doc = new SLDocument();

                //    //int[] stressTestIds = new int[stressTests.Count];
                //    stressTests.Keys.CopyTo(stressTestIds, 0);
                //    Dictionary<string, List<string>> concurrencyAndRuns;
                //    DataTable runsOverTimeDt = _resultsHelper.GetRunsOverTime(_cancellationTokenSource.Token, out concurrencyAndRuns, stressTestIds); //This one is special, it is for multiple tests by default.

                //    string firstWorksheet = MakeRunsOverTimeSheet(doc, runsOverTimeDt, concurrencyAndRuns);

                //    try { doc.SelectWorksheet(firstWorksheet); } catch { }
                //    try { doc.DeleteWorksheet("Sheet1"); } catch { }
                //    try {
                //        string docPath = Path.Combine(directory, "RunsOverTime.xlsx");
                //        doc.SaveAs(docPath);

                //        AddFileToZip(zipPath, docPath);

                //        File.Delete(docPath);
                //    } catch {
                //        MessageBox.Show("Failed to export.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    }
                //}
            }
        }

        //private string MakeRunsOverTimeSheet(SLDocument doc, DataTable dt, Dictionary<string, List<string>> concurrencyAndRuns) {
        //    string title = "Runs over time in minutes";
        //    doc.AddWorksheet(title);

        //    int rangeOffset = 1;
        //    int rangeWidth = dt.Columns.Count - 1;
        //    int rangeHeight = dt.Rows.Count;

        //    var boldStyle = new SLStyle();
        //    boldStyle.Font.Bold = true;

        //    doc.SetCellValue(rangeOffset, 1, dt.Columns[0].ColumnName);
        //    doc.SetCellStyle(rangeOffset, 1, boldStyle);
        //    for (int clmIndex = 1; clmIndex < dt.Columns.Count; clmIndex++) {
        //        string clmName = dt.Columns[clmIndex].ColumnName;
        //        int i;
        //        if (int.TryParse(clmName, out i)) {
        //            doc.SetCellValue(rangeOffset, clmIndex + 1, i);
        //            doc.SetCellStyle(rangeOffset, clmIndex + 1, boldStyle);
        //        } else {
        //            doc.SetCellValue(rangeOffset, clmIndex + 1, clmName);
        //        }
        //    }

        //    var formattedValues = new List<List<string>>();
        //    int rowOffset = 2;
        //    for (int rowIndex = 0; rowIndex != dt.Rows.Count; rowIndex++) {
        //        var row = dt.Rows[rowIndex].ItemArray;
        //        formattedValues.Add(new List<string>());
        //        for (int clmIndex = 1; clmIndex <= row.Length; clmIndex++) {
        //            var value = row[clmIndex - 1];

        //            if (value is System.DBNull)
        //                break;

        //            int rowInSheet = rowIndex + rowOffset;
        //            TimeSpan ts = new TimeSpan(0);
        //            if (value is string) {
        //                string s = value as string;
        //                if (!TimeSpan.TryParse(s, out ts)) {
        //                    int conIndex = s.IndexOf("Connection");
        //                    if (conIndex != -1) s = s.Substring(0, conIndex) + "\n" + s.Substring(conIndex);

        //                    doc.SetCellValue(rowInSheet, clmIndex, s);
        //                }
        //            } else if (value is TimeSpan) {
        //                ts = (TimeSpan)value;
        //            }

        //            if (ts.Ticks != 0) {
        //                doc.SetCellValue(rowInSheet, clmIndex, Convert.ToDouble(ts.Ticks) / TimeSpan.TicksPerMinute);
        //                formattedValues[rowIndex].Add(ts.ToShortFormattedString());
        //            }

        //            if (clmIndex == 1) doc.SetCellStyle(rowInSheet, clmIndex, boldStyle);
        //        }
        //    }

        //    doc.AutoFitColumn(rangeOffset, rangeOffset + rangeWidth, 60d);

        //    var chart = doc.CreateChart(rangeOffset, 1, rangeHeight + rangeOffset, rangeWidth + rangeOffset, new SLCreateChartOptions() { RowsAsDataSeries = false, ShowHiddenData = false });
        //    chart.Title.SetTitle("Runs over time");
        //    chart.ShowChartTitle(false);
        //    chart.HideChartLegend();

        //    chart.SetChartType(SLBarChartType.StackedBar);
        //    chart.PrimaryValueAxis.MajorUnit = 1;
        //    chart.PrimaryValueAxis.MinorUnit = 1.0d / 6;
        //    chart.PrimaryValueAxis.ShowMinorGridlines = true;
        //    chart.PrimaryValueAxis.Title.SetTitle("Run duration in minutes");
        //    chart.PrimaryValueAxis.ShowTitle = true;

        //    chart.PrimaryTextAxis.InReverseOrder = true;
        //    chart.PrimaryTextAxis.Title.SetTitle("StressTests");
        //    chart.PrimaryTextAxis.ShowTitle = true;

        //    var runTimeOptions = new SLDataSeriesOptions();
        //    runTimeOptions.Fill.SetSolidFill(Color.OrangeRed, 0);
        //    runTimeOptions.Line.SetSolidLine(Color.LightSteelBlue, 0);



        //    var gapOptions = new SLDataSeriesOptions();
        //    gapOptions.Fill.SetNoFill();

        //    for (int clmIndex = 1; clmIndex <= dt.Columns.Count; clmIndex++) {
        //        if (Convert.ToDouble(clmIndex) % 2 == 0) {
        //            chart.SetDataSeriesOptions(clmIndex, gapOptions);
        //        } else {
        //            chart.SetDataSeriesOptions(clmIndex, runTimeOptions);

        //            for (int rowIndex = 0; rowIndex != dt.Rows.Count; rowIndex++) {
        //                List<string> l = concurrencyAndRuns[dt.Rows[rowIndex].ItemArray[0] as string];
        //                int labelIndex = clmIndex / 2;
        //                if (labelIndex >= l.Count)
        //                    continue;

        //                var dataLabelOptions = chart.CreateDataLabelOptions();
        //                dataLabelOptions.ShowValue = dataLabelOptions.ShowPercentage = dataLabelOptions.ShowSeriesName = false;
        //                chart.SetDataLabelOptions(clmIndex, rowIndex + 1, dataLabelOptions);
        //            }
        //        }
        //    }

        //    chart.SetChartPosition(rangeHeight + 2, 0, rangeHeight + 45, 21);

        //    doc.InsertChart(chart);

        //    return title;
        //}

        private void ExportToExcelDialog_FormClosing(object sender, FormClosingEventArgs e) { try { _cancellationTokenSource.Cancel(); } catch { } }

        /// <summary>
        /// Call Init(...) first.
        /// </summary>
        public void AutoExportToExcel(string folder) {
            _autoExportFolder = folder;
            this.ShowDialog();
        }

        private void btnOverviewExport_Click(object sender, EventArgs e) {
            if (_resultsHelper != null)
                try {
                    var overviewExportDialog = new OverviewExportToExcelDialog();
                    overviewExportDialog.Init(_resultsHelper, new List<string>(new string[] { _resultsHelper.DatabaseName }), this);
                    overviewExportDialog.ShowDialog();
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed exporting overview to Excel.", ex);
                    MessageBox.Show(string.Empty, "Failed exporting overview to Excel.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }
        #endregion

    }
}
