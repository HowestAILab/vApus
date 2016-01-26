/*
 * Copyright 2014 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using Newtonsoft.Json.Linq;
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
    /// <summary>
    /// <para>An export to Excel enabler that can easily be expanded.</para>
    /// <para>Export to Excel includes worksheets over different Excel files as charts, made possible by SpreadsheetLight (Do(...)).</para>
    /// <para>An array of tree nodes can be generated for you from the available export goals (GetTreeNodes(...)).</para>
    /// </summary>
    public static class RichExportToExcel {
        /*
         * An export goal is defined as a '/' seperated path and must be added to _toExport in the InitToExport function.
         * Each goal represents at least one worksheet and chart if you like: this is defined in the attached function, see the code for an example (Datasets region).
         * 
         * If you want a new Excel file, add a new group to GROUPS. A group is the root node of a path, therefore a path must always start with a defined group.
         * 
         * Notice that a group/path can end with a *. That is for dynamic export goals, in this case we export a file per monitor for a certain test.
         */
        private enum ChartLocation {
            RightOfData = 0,
            BelowData
        }
        private enum ChartType {
            StackedBar = 0,
            StackedColumnAndLine,
            Column,
            TwoLines
        }

        #region Fields
        private static string[] GROUPS = { "General", "Meta", "Monitor data'/'*", "Specialized" };

        private delegate string Del(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token);
        private static Dictionary<string, Del> _toExport;
        /// <summary>
        /// A color palette of 34 colors to be able to visualy match overiew and 5 heaviest user actions charts.
        /// (Filled later on)
        /// </summary>
        private static List<Color> _colorPalette = new List<Color>(34);
        #endregion

        static RichExportToExcel() {
            InitToExport();
            FillColorPalette();
        }

        /// <summary>
        /// Fill _toExport with the available export goals.
        /// </summary>
        private static void InitToExport() {
            _toExport = new Dictionary<string, Del>();
            _toExport.Add("General'/'Response time vs throughput'/'with throughput in responses / s", GeneralResponseTimeVSThroughputWithThroughputInResponsesPerS);
            _toExport.Add("General'/'Response time vs throughput'/'with throughput in user actions / s", GeneralResponseTimeVSThroughputWithThroughputInUserActionsPerS);

            _toExport.Add("General'/'Errors vs throughput", GeneralErrorsVSThroughput);

            _toExport.Add("General'/'Top 5 heaviest user actions'/'for average response times", GeneralTop5HeaviestUserActionsForAverageResponseTimes);
            _toExport.Add("General'/'Top 5 heaviest user actions'/'for 95th percentile for the response times", GeneralTop5HeaviestUserActionsFor95thPercentileForTheResponseTimes);
            _toExport.Add("General'/'Top 5 heaviest user actions'/'for 99th percentile for the response times", GeneralTop5HeaviestUserActionsFor99thPercentileForTheResponseTimes);
            _toExport.Add("General'/'Top 5 heaviest user actions'/'for average top 5 the response times", GeneralTop5HeaviestUserActionsForAverageTop5ResponseTimes);

            _toExport.Add("General'/'Results per concurrency", GeneralResultsPerConcurrency);
            _toExport.Add("General'/'Results per user action", GeneralResultsPerUserAction);
            _toExport.Add("General'/'Results per request", GeneralResultsPerRequest);

            _toExport.Add("General'/'Errors", GeneralErrors);

            _toExport.Add("General'/'User action composition", GeneralUserActionComposisiton);

            _toExport.Add("Meta'/'Export meta results if any", Meta);

            _toExport.Add("Monitor data'/'*", MonitorData);

            _toExport.Add("Specialized'/'Response time distribution'/'for requests per concurrency", SpecializedResponseTimeDistributionForRequestsPerConcurrency);
            _toExport.Add("Specialized'/'Response time distribution'/'for user actions per concurrency", SpecializedResponseTimeDistributionForUserActionsPerConcurrency);
        }
        /// <summary>
        /// Handy for color matching series over different charts.
        /// </summary>
        private static void FillColorPalette() {
            _colorPalette = new List<Color>(34);

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

        #region Generated UI
        public static TreeNode[] GetTreeNodes(Font prototype, ResultsHelper resultsHelper, params int[] stressTestIds) {
            if (stressTestIds.Length == 0 || stressTestIds[0] < 1)
                stressTestIds = resultsHelper.GetStressTestIds().ToArray();

            var root = new TreeNode();

            var toExport = new List<string>(_toExport.Count);
            //First search the to be dynamically added nodes.
            foreach (string path in _toExport.Keys) {
                if (path.EndsWith("'/'*"))
                    foreach (string dynPath in GetDynamicPaths(path, resultsHelper, stressTestIds))
                        toExport.Add(dynPath);
                else
                    toExport.Add(path);
            }

            foreach (string path in toExport) {
                TreeNode parent = root;
                string[] splitted = path.Split(new string[] { "'/'" }, StringSplitOptions.None);

                for (int i = 0; i != splitted.Length; i++) {
                    string label = splitted[i];
                    if (!parent.Nodes.ContainsKey(label)) {
                        var node = new TreeNode(label);
                        node.Name = label;
                        node.Checked = true;
                        if (i == 0) node.NodeFont = new Font(prototype, FontStyle.Bold);
                        parent.Nodes.Add(node);
                    }

                    parent = parent.Nodes[label];
                }
            }

            var nodes = new TreeNode[root.Nodes.Count];
            root.Nodes.CopyTo(nodes, 0);
            return nodes;
        }
        /// <summary>
        /// Filters and returns the appropriate dynamic paths.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resultsHelper"></param>
        /// <param name="stressTestIds"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetDynamicPaths(string path, ResultsHelper resultsHelper, params int[] stressTestIds) {
            string parentPath = path.Substring(0, path.Length - 1); //Trim the *

            //Expand filters here.

            var childPaths = new string[0];
            if (parentPath == "Monitor data'/'") childPaths = GetMonitorDataChildPaths(resultsHelper, stressTestIds);

            foreach (string childPath in childPaths) yield return parentPath + childPath;
        }
        private static string[] GetMonitorDataChildPaths(ResultsHelper resultsHelper, params int[] stressTestIds) {
            Dictionary<int, string> monitorIdsAndNames = resultsHelper.GetMonitors(stressTestIds);

            var childPaths = new string[monitorIdsAndNames.Count];
            monitorIdsAndNames.Values.CopyTo(childPaths, 0);
            return childPaths;
        }

        /// <summary>
        /// Handles check state of the parent node and the child nodes if the given node's check state changes.
        /// 
        /// Do not forget to unsuscribe from the tree view after check event!
        /// </summary>
        /// <param name="node"></param>
        public static void HandleTreeNodeChecked(TreeNode node) {
            HandleTreeNodeChecked(node, true);
        }
        private static void HandleTreeNodeChecked(TreeNode node, bool checkParent) {
            if (checkParent && node.Parent != null) {
                bool check = false;
                foreach (TreeNode sibling in node.Parent.Nodes)
                    if (sibling.Checked) {
                        check = true;
                        break;
                    }

                CheckParent(node, check);
            }
            foreach (TreeNode childNode in node.Nodes) {
                childNode.Checked = node.Checked;
                HandleTreeNodeChecked(childNode, false);
            }
        }
        private static void CheckParent(TreeNode node, bool check) {
            if (node.Parent != null) {
                node.Parent.Checked = check;
                CheckParent(node.Parent, check);
            }
        }
        #endregion

        /// <summary>
        /// Extract paths for the checked nodes.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="pathSeperator">The seperator fo the nodes' full paths.</param>
        /// <returns></returns>
        public static IEnumerable<string> ExtractToExport(TreeNodeCollection nodes, string pathSeperator) {
            var toExport = new List<string>();
            foreach (string path in GetDeepestPaths(nodes))
                toExport.Add(path.Replace(pathSeperator, "'/'"));
            return toExport;
        }

        private static List<string> GetDeepestPaths(TreeNodeCollection nodes) {
            var deepestPaths = new List<string>();
            foreach (TreeNode node in nodes)
                if (node.Checked && node.Nodes.Count == 0)
                    deepestPaths.Add(node.FullPath);
                else
                    deepestPaths.AddRange(GetDeepestPaths(node.Nodes));
            return deepestPaths;
        }

        /// <summary>
        /// Do the export using all stress test ids.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="resultsHelper"></param>
        /// <param name="toExport"></param>
        /// <param name="token"></param>
        public static void Do(string fullExportPath, ResultsHelper resultsHelper, IEnumerable<string> toExport, CancellationToken token) {
            Do(fullExportPath, new int[] { -1 }, resultsHelper, toExport, token);
        }
        /// <summary>
        /// Do the export certain stress test ids. This should be either all or one.
        /// </summary>
        /// <param name="fullExportPath"></param>
        /// <param name="stressTestIds"></param>
        /// <param name="resultsHelper"></param>
        /// <param name="toExport"></param>
        /// <param name="token"></param>
        public static void Do(string fullExportPath, int[] stressTestIds, ResultsHelper resultsHelper, IEnumerable<string> toExport, CancellationToken token) {
            string directory = Path.GetDirectoryName(fullExportPath);

            if (stressTestIds.Length == 0 || stressTestIds[0] < 1)
                stressTestIds = resultsHelper.GetStressTestIds().ToArray();

            Dictionary<string, List<Del>> dataSetStructure = ParseToDataSetStructure(toExport);

            foreach (string dataSet in dataSetStructure.Keys) {
                foreach (int stressTestId in stressTestIds) {
                    var doc = new SLDocument();

                    DataTable stressTests = resultsHelper.GetStressTests(stressTestId);
                    if (stressTests == null || stressTests.Rows.Count == 0) continue;

                    DataRow stressTestsRow = stressTests.Rows[0];
                    string stressTest = string.Format("{0} {1}", stressTestsRow["StressTest"], stressTestsRow["Connection"]);

                    string docFileName = (stressTest + "_" + dataSet).Replace(' ', '_').ReplaceInvalidWindowsFilenameChars('_') + ".xlsx";
                    string firstWorksheet = null;

                    foreach (Del del in dataSetStructure[dataSet]) {
                        string worksheet = del.Invoke(dataSet, doc, stressTestId, resultsHelper, token);
                        if (firstWorksheet == null) firstWorksheet = worksheet;
                    }

                    if (firstWorksheet == null) continue;

                    try { doc.SelectWorksheet(firstWorksheet); } catch { }
                    try { doc.DeleteWorksheet("Sheet1"); } catch { }

                    string docPath = Path.Combine(directory, docFileName);

                    doc.SaveAs(docPath);

                    AddFileToZip(fullExportPath, docPath);

                    File.Delete(docPath);
                }
            }

        }

        #region Datasets
        private static Dictionary<string, List<Del>> ParseToDataSetStructure(IEnumerable<string> toExport) {
            var d = new Dictionary<string, List<Del>>();

            foreach (string s in toExport) {
                bool added = false;
                foreach (string group in GROUPS)
                    if (s.StartsWith(group + "'/'")) {
                        if (!d.ContainsKey(group)) d.Add(group, new List<Del>());
                        d[group].Add(_toExport[s]);
                        added = true;
                        break;
                    }

                if (!added) {
                    string[] splittedS = s.Split(new string[] { "'/'" }, StringSplitOptions.None);

                    foreach (string group in GROUPS) {
                        string[] splittedGroup = group.Split(new string[] { "'/'" }, StringSplitOptions.None);
                        if (splittedS[0] == splittedGroup[0] && splittedGroup[1] == "*") {
                            string newGroup = splittedS[1];
                            d.Add(newGroup, new List<Del>());
                            d[newGroup].Add(_toExport[group]);
                            break;
                        }
                    }
                }
            }

            return d;
        }

        //---
        //Do not forget to prep the datatables coming from the results helper. Prep(...) makes a copy and removes the "Stress test" column and replaces "<16 0C 02 12$>" by "•"
        //---

        private static string GeneralResponseTimeVSThroughputWithThroughputInResponsesPerS(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            DataTable dt = Prep(resultsHelper.GetOverview(token, stressTestId));
            dt.Columns.Remove("User actions / s");
            string title = "Response time vs throughput";
            string workSheet = MakeWorksheet(doc, dt, title, false, true);
            AddChart(doc, dt.Columns.Count - 1, dt.Rows.Count + 1, title, "Concurrency", "Cumulative response time (ms)", "Throughput (responses / s)", ChartType.StackedColumnAndLine, ChartLocation.RightOfData, true);

            return workSheet;
        }
        private static string GeneralResponseTimeVSThroughputWithThroughputInUserActionsPerS(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            DataTable dt = Prep(resultsHelper.GetOverview(token, stressTestId));
            dt.Columns.Remove("Throughput");
            string title = "Response time vs user actions / s";
            string workSheet = MakeWorksheet(doc, dt, title, false, true);
            AddChart(doc, dt.Columns.Count - 1, dt.Rows.Count + 1, title, "Concurrency", "Cumulative response time (ms)", "User actions / s", ChartType.StackedColumnAndLine, ChartLocation.RightOfData, true);

            return workSheet;
        }

        private static string GeneralErrorsVSThroughput(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            DataTable dt = Prep(resultsHelper.GetOverviewErrors(token, stressTestId));
            string title = "Errors vs throughput";
            string workSheet = MakeWorksheet(doc, dt, title, false, true);
            AddChart(doc, dt.Columns.Count, dt.Rows.Count + 1, title, "Concurrency", "Errors", "Throughput (responses / s)", ChartType.TwoLines);

            return workSheet;
        }

        private static string GeneralTop5HeaviestUserActionsForAverageResponseTimes(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            return GeneralTop5HeaviestUserActions(doc, resultsHelper.GetTop5HeaviestUserActions(token, stressTestId), stressTestId, resultsHelper, token, "", " (averages)");
        }
        private static string GeneralTop5HeaviestUserActionsFor95thPercentileForTheResponseTimes(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            return GeneralTop5HeaviestUserActions(doc, resultsHelper.GetTop5HeaviestUserActions95thPercentile(token, stressTestId), stressTestId, resultsHelper, token, "_", " (95th percentiles)");
        }
        private static string GeneralTop5HeaviestUserActionsFor99thPercentileForTheResponseTimes(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            return GeneralTop5HeaviestUserActions(doc, resultsHelper.GetTop5HeaviestUserActions99thPercentile(token, stressTestId), stressTestId, resultsHelper, token, "__", " (99th percentiles)");
        }
        private static string GeneralTop5HeaviestUserActionsForAverageTop5ResponseTimes(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            return GeneralTop5HeaviestUserActions(doc, resultsHelper.GetTop5HeaviestUserActionsAverageTop5(token, stressTestId), stressTestId, resultsHelper, token, "___", " (top 5 averages)");
        }

        private static string GeneralTop5HeaviestUserActions(SLDocument doc, DataTable dt, int stressTestId, ResultsHelper resultsHelper, CancellationToken token, string worksheetSuffix, string chartTitleSuffix) {
            dt = Prep(dt);
            string title = "Top 5 heaviest user actions";

            string workSheet = MakeWorksheet(doc, dt, title + worksheetSuffix, false, true);

            List<Color> colorPalette = GetGeneralTop5HeaviestUserActionsColors(dt, stressTestId, resultsHelper, token);

            AddChart(doc, dt.Columns.Count, dt.Rows.Count + 1, title + chartTitleSuffix, "Concurrency", "Response time (ms)", ChartType.Column, ChartLocation.RightOfData, true, colorPalette);

            return workSheet;
        }

        /// <summary>
        /// Match colors in the charts
        /// </summary>
        /// <param name="top5Heaviest"></param>
        /// <param name="chart"></param>
        /// <param name="stressTestId"></param>
        /// <param name="resultsHelper"></param>
        /// <param name="token"></param>
        private static List<Color> GetGeneralTop5HeaviestUserActionsColors(DataTable top5Heaviest, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            var clone = top5Heaviest.Copy();
            clone.Columns.Remove("Concurrency");

            DataTable overview = Prep(resultsHelper.GetOverview(token, stressTestId));
            overview.Columns.Remove("Concurrency");

            var colorPalette = new List<Color>(5);

            foreach (DataColumn column in clone.Columns) {
                int index = overview.Columns.IndexOf(column.ColumnName);
                while (index >= _colorPalette.Count) index -= _colorPalette.Count;
                colorPalette.Add(_colorPalette[index]);
            }

            return colorPalette;
        }

        private static string GeneralResultsPerConcurrency(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            DataTable dt = Prep(resultsHelper.GetAverageConcurrencyResults(token, stressTestId));
            string title = "Results per concurrency";
            return MakeWorksheet(doc, dt, title, true, true);
        }
        private static string GeneralResultsPerUserAction(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            DataTable dt = Prep(resultsHelper.GetAverageUserActionResults(token, stressTestId));
            string title = "Results per user action";
            return MakeWorksheet(doc, dt, title, true, true);
        }
        private static string GeneralResultsPerRequest(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            DataTable dt = Prep(resultsHelper.GetAverageRequestResults(token, stressTestId));
            string title = "Results per request";
            return MakeWorksheet(doc, dt, title, true, true);
        }

        private static string GeneralErrors(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            DataTable dt = Prep(resultsHelper.GetErrors(token, stressTestId));
            string title = "Errors";
            return MakeWorksheet(doc, dt, title, true, true);
        }

        private static string Meta(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            DataTable dt = resultsHelper.GetMeta(token, stressTestId);
            string firstWorksheet = null;

            int i = 0;
            foreach (DataRow row in dt.Rows) {
                dynamic data = JObject.Parse(row["Meta"] as string);
                if (data.type == "WebPageTest") {
                    var colorPalette = new List<Color>(7);

                    colorPalette.Add(Color.FromArgb(255, 255, 255));
                    colorPalette.Add(Color.FromArgb(50, 85, 126));
                    colorPalette.Add(Color.FromArgb(128, 51, 49));
                    colorPalette.Add(Color.FromArgb(103, 125, 57));
                    colorPalette.Add(Color.FromArgb(84, 65, 107));
                    colorPalette.Add(Color.FromArgb(47, 114, 132));
                    colorPalette.Add(Color.FromArgb(166, 99, 44));

                    DataTable waterfallDt;
                    string worksheet = CreateWaterfallWorksheet(doc, data.requests, (++i).ToString(), out waterfallDt);
                    AddChart(doc, waterfallDt.Columns.Count, waterfallDt.Rows.Count + 1, "Waterfall (ms)", string.Empty, string.Empty, ChartType.StackedBar, ChartLocation.BelowData, true, colorPalette);
                    CreateWaterfallWorksheet(doc, data.cachedRequests, i + " cached", out waterfallDt);
                    AddChart(doc, waterfallDt.Columns.Count, waterfallDt.Rows.Count + 1, "Waterfall cached (ms)", string.Empty, string.Empty, ChartType.StackedBar, ChartLocation.BelowData, true, colorPalette);

                    if (firstWorksheet == null) firstWorksheet = worksheet;
                }
            }

            return firstWorksheet;
        }

        private static string CreateWaterfallWorksheet(SLDocument doc, JArray requests, string title, out DataTable waterfallDt) {
            waterfallDt = CreateEmptyDataTable("Waterfall", "Url", "empty", "DNS", "Connect", "SSL", "Time to first byte", "Time to last byte");

            foreach (dynamic r in requests)
                waterfallDt.Rows.Add(r.method + " " + r.result + " " + r.host + r.url, (long)r.requestOffsetInMs, (long)r.dnsInMs,
                    (long)r.connectInMs, (long)r.sslInMs, (long)r.timeToFirstByteInMs, (long)r.timeToLastByteInMs);

            string worksheet = MakeWorksheet(doc, waterfallDt, title, false, true);         
            
            return worksheet;
        }

        private static DataTable CreateEmptyDataTable(string name, params string[] columnNames) {
            var objectType = typeof(object);
            var dataTable = new DataTable(name);
            foreach (string columnName in columnNames) dataTable.Columns.Add(columnName, objectType);
            return dataTable;
        }

        private static string GeneralUserActionComposisiton(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            DataTable dt = Prep(resultsHelper.GetUserActionComposition(token, stressTestId));

            var userActionComposition = new DataTable("UserActionComposition");
            userActionComposition.Columns.Add();
            userActionComposition.Columns.Add();

            var userActions = new Dictionary<string, List<string>>();
            foreach (DataRow row in dt.Rows) {
                string userAction = row["User action"] as string;
                string request = row["Request"] as string;
                if (!userActions.ContainsKey(userAction)) userActions.Add(userAction, new List<string>());
                userActions[userAction].Add(request);
            }

            foreach (string userAction in userActions.Keys) {
                userActionComposition.Rows.Add(userAction, string.Empty);
                foreach (string request in userActions[userAction])
                    userActionComposition.Rows.Add(string.Empty, request);
            }

            string title = "User action composition";
            return MakeWorksheet(doc, userActionComposition, title, false, true, false);
        }

        private static string MonitorData(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            Dictionary<int, string> monitorIdsAndNames = resultsHelper.GetMonitors(new int[] { stressTestId });

            int monitorId;
            if (monitorIdsAndNames.TryGetKey(dataset, out monitorId)) {
                DataTable dt = Prep(resultsHelper.GetMonitorResultsByMonitorId(token, monitorId));
                dt.Columns.Remove("Monitor");

                string firstWorksheet = MakeWorksheet(doc, dt, dataset, false, true);

                dt = Prep(resultsHelper.GetAverageMonitorResultsByMonitorId(token, monitorId));
                dt.Columns.Remove("Monitor");

                MakeWorksheet(doc, dt, "Metrics per concurrency", false, true);

                return firstWorksheet;
            }
            return null;
        }

        private static string SpecializedResponseTimeDistributionForRequestsPerConcurrency(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            return SpecializedResponseTimeDistributionPerConcurrency(doc, resultsHelper.GetResponseTimeDistributionForRequestsPerConcurrency(token, stressTestId), " RE");
        }
        private static string SpecializedResponseTimeDistributionForUserActionsPerConcurrency(string dataset, SLDocument doc, int stressTestId, ResultsHelper resultsHelper, CancellationToken token) {
            return SpecializedResponseTimeDistributionPerConcurrency(doc, resultsHelper.GetResponseTimeDistributionForUserActionsPerConcurrency(token, stressTestId), " UA");
        }

        private static string SpecializedResponseTimeDistributionPerConcurrency(SLDocument doc, DataTable dt, string worksheetSuffix) {
            dt = Prep(dt);
            var rowsPerConcurrency = new Dictionary<int, DataTable>();

            foreach (DataRow row in dt.Rows) {
                int concurrencyResultId = (int)row["ConcurrencyResultId"];
                if (!rowsPerConcurrency.ContainsKey(concurrencyResultId))
                    rowsPerConcurrency.Add(concurrencyResultId, dt.Clone());

                rowsPerConcurrency[concurrencyResultId].Rows.Add(row.ItemArray);
            }

            string firstWorksheet = null;

            int i = 0;
            foreach (DataTable part in rowsPerConcurrency.Values) {
                if (part.Rows.Count == 0) continue;

                part.Columns.Remove("ConcurrencyResultId");

                int concurrency = (int)part.Rows[0]["Concurrency"];

                part.Columns.Remove("Concurrency");

                string workSheet = MakeWorksheet(doc, part, (++i) + ") concurrency " + concurrency + worksheetSuffix, false, true);
                AddChart(doc, part.Columns.Count, part.Rows.Count + 1, "Response time distribution for concurrency " + concurrency, "Time to last byte (s)", "Count", ChartType.Column);

                if (firstWorksheet == null) firstWorksheet = workSheet;
            }

            return firstWorksheet;
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
                    string columnName = dt.Columns[clmIndex].ColumnName;
                    if (columnName == "empty") columnName = string.Empty;
                    doc.SetCellValue(1, clmIndex + 1, columnName);
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
        /// Makes a new data table without the 'Stresst est' column (if any) and "<16 0C 02 12$>" replaced by "•".
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static DataTable Prep(DataTable dt) {
            DataTable copy = dt.Copy();

            if (copy.Columns.Count != 0) {
                if (copy.Columns[0].ColumnName == "StressTest" || copy.Columns[0].ColumnName == "Stress test")
                    copy.Columns.RemoveAt(0);

                DataTable clone = copy.Clone();

                foreach (DataRow row in copy.Rows) {
                    object[] arr = row.ItemArray;
                    for (int i = 0; i != arr.Length; i++)
                        if (arr[i] is string)
                            arr[i] = (arr[i] as string).Replace("<16 0C 02 12$>", "•");

                    clone.Rows.Add(arr);
                }

                copy = clone;
            }

            return copy;
        }
        #endregion

        #region Charts
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="rangeWidth"></param>
        /// <param name="rangeHeight"></param>
        /// <param name="title"></param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="type"></param>
        /// <param name="location"></param>
        /// <param name="setDataSeriesColors">Where available</param>
        /// <param name="dataSeriesColors">Default (_colorPalette) used if null</param>
        /// <returns></returns>
        private static SLChart AddChart(SLDocument doc, int rangeWidth, int rangeHeight, string title, string xAxis, string yAxis, ChartType type, ChartLocation location = ChartLocation.RightOfData, bool setDataSeriesColors = false, List<Color> dataSeriesColors = null) {
            return AddChart(doc, rangeWidth, rangeHeight, title, xAxis, yAxis, string.Empty, type, location, setDataSeriesColors, dataSeriesColors);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="rangeWidth"></param>
        /// <param name="rangeHeight"></param>
        /// <param name="title"></param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="secondaryYAxis"></param>
        /// <param name="type"></param>
        /// <param name="location"></param>
        /// <param name="setDataSeriesColors">Where available</param>
        /// <param name="dataSeriesColors">Default (_colorPalette) used if null</param>
        /// <returns></returns>
        private static SLChart AddChart(SLDocument doc, int rangeWidth, int rangeHeight, string title, string xAxis, string yAxis, string secondaryYAxis, ChartType type, ChartLocation location = ChartLocation.RightOfData, bool setDataSeriesColors = false, List<Color> dataSeriesColors = null) {
            SLChart chart = doc.CreateChart(1, 1, rangeHeight, rangeWidth, new SLCreateChartOptions() { RowsAsDataSeries = false, ShowHiddenData = false });

            if (dataSeriesColors == null) dataSeriesColors = _colorPalette;

            if (type == ChartType.StackedBar)
                chart = MakeStackedBar(chart, rangeWidth, setDataSeriesColors, dataSeriesColors);
            else if (type == ChartType.StackedColumnAndLine)
                chart = MakeStackedColumnAndLineChart(chart, rangeWidth, setDataSeriesColors, dataSeriesColors);
            else if (type == ChartType.Column)
                chart = MakeColumnChart(chart, rangeWidth, setDataSeriesColors, dataSeriesColors);
            else if (type == ChartType.TwoLines)
                chart = MakeTwoLinesChart(chart);

            if (location == ChartLocation.RightOfData)
                chart.SetChartPosition(0, rangeWidth + 2, 45, rangeWidth + 21);
            else
                chart.SetChartPosition(rangeHeight + 2, 0, rangeHeight + 45, 21);

            //Set the tiles
            chart.Title.SetTitle(title);
            chart.ShowChartTitle(false);
            chart.PrimaryTextAxis.Title.SetTitle(xAxis);
            chart.PrimaryTextAxis.ShowTitle = !string.IsNullOrEmpty(xAxis);
            chart.PrimaryValueAxis.Title.SetTitle(yAxis);
            chart.PrimaryValueAxis.ShowTitle = !string.IsNullOrEmpty(xAxis);
            chart.PrimaryValueAxis.ShowMinorGridlines = true;

            if (!string.IsNullOrWhiteSpace(secondaryYAxis)) {
                chart.SecondaryValueAxis.Title.SetTitle(secondaryYAxis);
                chart.SecondaryValueAxis.ShowTitle = true;
            }

            doc.InsertChart(chart);

            return chart;
        }

        private static SLChart MakeStackedBar(SLChart chart, int rangeWidth, bool setDataSeriesColors, List<Color> dataSeriesColors) {
            chart.SetChartType(SLBarChartType.StackedBar);
            chart.Legend.LegendPosition = DocumentFormat.OpenXml.Drawing.Charts.LegendPositionValues.Bottom;
            chart.PrimaryTextAxis.InReverseOrder = true;

            if (setDataSeriesColors)
                SetDataSeriesColors(chart, rangeWidth, dataSeriesColors);

            return chart;
        }
        private static SLChart MakeStackedColumnAndLineChart(SLChart chart, int rangeWidth, bool setDataSeriesColors, List<Color> dataSeriesColors) {
            chart.SetChartType(SLColumnChartType.StackedColumn);
            chart.Legend.LegendPosition = DocumentFormat.OpenXml.Drawing.Charts.LegendPositionValues.Bottom;

            int primaryRange = rangeWidth - 1;
            int secondaryDataSeriesIndex = primaryRange;
            chart.PlotDataSeriesAsSecondaryLineChart(secondaryDataSeriesIndex, SLChartDataDisplayType.Normal, false);

            if (setDataSeriesColors)
                SetDataSeriesColors(chart, primaryRange, dataSeriesColors);

            var dso = chart.GetDataSeriesOptions(secondaryDataSeriesIndex);
            dso.Line.SetSolidLine(Color.LimeGreen, 0);
            chart.SetDataSeriesOptions(secondaryDataSeriesIndex, dso);

            return chart;
        }
        private static SLChart MakeColumnChart(SLChart chart, int rangeWidth, bool setDataSeriesColors, List<Color> dataSeriesColors) {
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.Legend.LegendPosition = DocumentFormat.OpenXml.Drawing.Charts.LegendPositionValues.Bottom;

            if (setDataSeriesColors)
                SetDataSeriesColors(chart, rangeWidth, dataSeriesColors);

            return chart;
        }
        private static SLChart MakeTwoLinesChart(SLChart chart) {
            chart.SetChartType(SLLineChartType.Line);
            chart.Legend.LegendPosition = DocumentFormat.OpenXml.Drawing.Charts.LegendPositionValues.Bottom;

            int primaryDataSeriesIndex = 1;
            int secondaryDataSeriesIndex = 2;

            chart.PlotDataSeriesAsSecondaryLineChart(secondaryDataSeriesIndex, SLChartDataDisplayType.Normal, false);

            var dso1 = chart.GetDataSeriesOptions(primaryDataSeriesIndex);
            dso1.Line.SetSolidLine(Color.Red, 0);
            chart.SetDataSeriesOptions(primaryDataSeriesIndex, dso1);

            var dso2 = chart.GetDataSeriesOptions(secondaryDataSeriesIndex);
            dso2.Line.SetSolidLine(Color.LimeGreen, 0);
            chart.SetDataSeriesOptions(secondaryDataSeriesIndex, dso2);

            return chart;
        }

        private static void SetDataSeriesColors(SLChart chart, int numberOfSeries, List<Color> colorPalette) {
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
        #endregion

        private static void AddFileToZip(string zipFilename, string fileToAdd, CompressionOption compressionOption = CompressionOption.Normal) {
            using (Package zip = System.IO.Packaging.Package.Open(zipFilename, FileMode.OpenOrCreate)) {
                string destFilename = ".\\" + Path.GetFileName(fileToAdd);
                Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                if (zip.PartExists(uri))
                    zip.DeletePart(uri);

                PackagePart part = zip.CreatePart(uri, "", compressionOption);
                using (var fileStream = new FileStream(fileToAdd, FileMode.Open, FileAccess.Read))
                using (Stream dest = part.GetStream())
                    CopyStream(fileStream, dest);
            }
        }

        private static void CopyStream(FileStream inputStream, Stream outputStream) {
            long bufferSize = inputStream.Length < 4096 ? inputStream.Length : 4096;
            var buffer = new byte[bufferSize];
            int bytesRead = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
                outputStream.Write(buffer, 0, bytesRead);
        }
    }
}
