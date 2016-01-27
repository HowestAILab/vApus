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
using System.Linq;
using System.Reflection;
using System.Threading;
using vApus.Util;

namespace vApus.Results {
    /// <summary>
    /// An extra layer that serves to handeling results from many-to-one distributed tests (a stress tests workload divided over multiple slaves). It makes the data look like it is for 1 test instead of multiple; ResultsHelper should use this layer.
    /// Caching combined data is something that does not happen, but this can be put in place if the need is there.
    /// </summary>
    public static class ReaderAndCombiner {
        #region Public
        public static DataTable GetDescription(DatabaseActions databaseActions) {
            return databaseActions.GetDataTable("Select Description FROM description");
        }
        public static DataTable GetTags(DatabaseActions databaseActions) {
            return databaseActions.GetDataTable("Select Tag FROM tags");
        }

        /// <summary>
        /// Get all the vApus instances used, divided stress tests are not taken into account.
        /// </summary>
        public static DataTable GetvApusInstances(DatabaseActions databaseActions) {
            return databaseActions.GetDataTable("Select * FROM vapusinstances");
        }

        public static DataTable GetStressTests(CancellationToken cancellationToken, DatabaseActions databaseActions, params string[] selectColumns) { return GetStressTests(cancellationToken, databaseActions, new int[0], selectColumns); }
        public static DataTable GetStressTests(CancellationToken cancellationToken, DatabaseActions databaseActions, int stressTestId, params string[] selectColumns) { return GetStressTests(cancellationToken, databaseActions, new int[] { stressTestId }, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="stressTestIds">If only one Id is given for a tests divided over multiple stress tests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetStressTests(CancellationToken cancellationToken, DatabaseActions databaseActions, int[] stressTestIds, params string[] selectColumns) {
            //The select columns can place a column at another index.
            int stressTestIndex = 2;

            if (selectColumns.Length != 0) {
                stressTestIndex = selectColumns.IndexOf("StressTest");
                if (!selectColumns.Contains("StressTest")) {
                    var copy = new string[selectColumns.Length + 1];
                    selectColumns.CopyTo(copy, 0);
                    copy[selectColumns.Length] = "StressTest";
                    selectColumns = copy;
                }
            }

            var stressTestsAndDividedRows = GetStressTestsAndDividedStressTestRows(cancellationToken, databaseActions, stressTestIds, selectColumns);
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable combined = null;

            foreach (string stressTest in stressTestsAndDividedRows.Keys) {
                if (cancellationToken.IsCancellationRequested) return null;

                var part = stressTestsAndDividedRows[stressTest];
                object[] row = part.Rows[0].ItemArray;

                var combinedRow = new object[row.Length];
                row.CopyTo(combinedRow, 0);

                if (stressTestIndex != -1)
                    combinedRow[stressTestIndex] = stressTest;

                if (combined == null) combined = part.Clone();
                combined.Rows.Add(combinedRow);
            }
            if (combined != null) combined.TableName = "stresstests";
            return combined;
        }

        public static DataTable GetStressTestResults(CancellationToken cancellationToken, DatabaseActions databaseActions, int stressTestId, params string[] selectColumns) { return GetStressTestResults(cancellationToken, databaseActions, new int[] { stressTestId }, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="stressTestIds">If only one Id is given for a tests divided over multiple stress tests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetStressTestResults(CancellationToken cancellationToken, DatabaseActions databaseActions, int[] stressTestIds, params string[] selectColumns) {
            if (GetStressTestIdsAndSiblings(cancellationToken, databaseActions, stressTestIds).Length == stressTestIds.Length) {  //No divided stress tests, so we can immediatly return results.
                if (stressTestIds.Length == 0)
                    return databaseActions.GetDataTable(string.Format("Select {0} From stresstestresults;", GetValidSelect(selectColumns)));

                return databaseActions.GetDataTable(string.Format("Select {0} From stresstestresults Where StressTestId In({1});", GetValidSelect(selectColumns), stressTestIds.Combine(", ")));
            }

            int startedAtIndex = 2;
            int stoppedAtIndex = 3;
            int statusIndex = 4;
            int statusMessageIndex = 5;

            if (selectColumns.Length != 0) {
                startedAtIndex = selectColumns.IndexOf("StartedAt");
                stoppedAtIndex = selectColumns.IndexOf("StoppedAt");
                statusIndex = selectColumns.IndexOf("Status");
                statusMessageIndex = selectColumns.IndexOf("StatusMessage");
            }

            var stressTestsAndDividedRows = GetStressTestsAndDividedStressTestResultRows(cancellationToken, databaseActions, stressTestIds, selectColumns);
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable combined = null;

            foreach (string stressTest in stressTestsAndDividedRows.Keys) {
                if (cancellationToken.IsCancellationRequested) return null;

                var part = stressTestsAndDividedRows[stressTest];
                object[] row = part.Rows[0].ItemArray;

                var combinedRow = new object[row.Length];
                row.CopyTo(combinedRow, 0);

                for (int i = 1; i != part.Rows.Count; i++) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    row = part.Rows[i].ItemArray;

                    if (startedAtIndex != -1) {
                        var startedAt = (DateTime)row[startedAtIndex];
                        if (startedAt < (DateTime)combinedRow[startedAtIndex]) combinedRow[startedAtIndex] = startedAt;
                    }

                    if (stoppedAtIndex != -1) {
                        var stoppedAt = (DateTime)row[stoppedAtIndex];
                        if (stoppedAt > (DateTime)combinedRow[stoppedAtIndex]) combinedRow[stoppedAtIndex] = stoppedAt;
                    }

                    if (statusIndex != -1) {
                        var status = row[statusIndex] as string;
                        if (status == "Failed")
                            combinedRow[statusIndex] = status;
                        else if (status == "Cancelled" && combinedRow[statusIndex] as string == "OK")
                            combinedRow[statusIndex] = status;
                    }

                    if (statusMessageIndex != -1) {
                        var statusMessage = row[statusMessageIndex] as string;
                        if (statusMessage.Length != 0)
                            combinedRow[statusMessageIndex] = ((combinedRow[statusMessageIndex] as string).Length == 0) ?
                                statusMessage : combinedRow[statusMessageIndex] + "\n" + statusMessage;
                    }
                }

                if (combined == null) combined = part.Clone();
                combined.Rows.Add(combinedRow);
            }
            if (combined != null) combined.TableName = "stresstestresults";
            return combined;
        }

        public static DataTable GetConcurrencyResults(CancellationToken cancellationToken, DatabaseActions databaseActions, int stressTestResultId, params string[] selectColumns) { return GetConcurrencyResults(cancellationToken, databaseActions, null, stressTestResultId, selectColumns); }
        public static DataTable GetConcurrencyResults(CancellationToken cancellationToken, DatabaseActions databaseActions, string where, int stressTestResultId, params string[] selectColumns) { return GetConcurrencyResults(cancellationToken, databaseActions, where, new int[] { stressTestResultId }, selectColumns); }
        public static DataTable GetConcurrencyResults(CancellationToken cancellationToken, DatabaseActions databaseActions, int[] stressTestResultIds, params string[] selectColumns) { return GetConcurrencyResults(cancellationToken, databaseActions, null, stressTestResultIds, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="where"></param>
        /// <param name="stressTestResultIds">If only one Id is given for a tests divided over multiple stress tests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetConcurrencyResults(CancellationToken cancellationToken, DatabaseActions databaseActions, string where, int[] stressTestResultIds, params string[] selectColumns) {
            if (GetStressTestResultIdsAndSiblings(cancellationToken, databaseActions, stressTestResultIds).Length == stressTestResultIds.Length) {  //No divided stress tests, so we can immediatly return results.
                if (stressTestResultIds.Length == 0)
                    return databaseActions.GetDataTable(string.Format("Select {0} From concurrencyresults{1};", GetValidSelect(selectColumns), GetValidWhere(where, true)));

                return databaseActions.GetDataTable(string.Format("Select {0} From concurrencyresults Where StressTestResultId In({1}){2};", GetValidSelect(selectColumns), stressTestResultIds.Combine(", "), GetValidWhere(where, false)));
            }

            int concurrencyIndex = 2;
            int startedAtIndex = 3;
            int stoppedAtIndex = 4;

            if (selectColumns.Length != 0) {
                concurrencyIndex = selectColumns.IndexOf("Concurrency");
                startedAtIndex = selectColumns.IndexOf("StartedAt");
                stoppedAtIndex = selectColumns.IndexOf("StoppedAt");

                if (!selectColumns.Contains("StressTestResultId")) {
                    var copy = new string[selectColumns.Length + 1];
                    selectColumns.CopyTo(copy, 0);
                    copy[selectColumns.Length] = "StressTestResultId";
                    selectColumns = copy;
                }
            }
            var stressTestsAndDividedRows = GetStressTestsAndDividedConcurrencyResultRows(cancellationToken, databaseActions, where, stressTestResultIds, selectColumns);
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable combined = null;

            foreach (string stressTest in stressTestsAndDividedRows.Keys) {
                if (cancellationToken.IsCancellationRequested) return null;

                var dividedParts = stressTestsAndDividedRows[stressTest];

                //Pivot the data table (in a more handleble structure) to be able to do the right merge.
                var pivotedRows = Pivot(cancellationToken, dividedParts);
                if (cancellationToken.IsCancellationRequested) return null;

                //Merge
                foreach (var part in pivotedRows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    object[] row = part[0];

                    var combinedRow = new object[row.Length];
                    row.CopyTo(combinedRow, 0);

                    for (int i = 1; i != part.Count; i++) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        row = part[i];
                        if (concurrencyIndex != -1)
                            combinedRow[concurrencyIndex] = (int)combinedRow[concurrencyIndex] + (int)row[concurrencyIndex];

                        if (startedAtIndex != -1) {
                            var startedAt = (DateTime)row[startedAtIndex];
                            if (startedAt < (DateTime)combinedRow[startedAtIndex]) combinedRow[startedAtIndex] = startedAt;
                        }

                        if (stoppedAtIndex != -1) {
                            var stoppedAt = (DateTime)row[stoppedAtIndex];
                            if (stoppedAt > (DateTime)combinedRow[stoppedAtIndex]) combinedRow[stoppedAtIndex] = stoppedAt;
                        }
                    }

                    //Copy the table and column names.
                    if (combined == null) combined = dividedParts[0].Clone();
                    combined.Rows.Add(combinedRow);
                }
            }
            if (combined != null) combined.TableName = "concurrencyresults";
            return combined;
        }

        public static DataTable GetRunResults(CancellationToken cancellationToken, DatabaseActions databaseActions, int concurrencyResultId, params string[] selectColumns) { return GetRunResults(cancellationToken, databaseActions, null, concurrencyResultId, selectColumns); }
        public static DataTable GetRunResults(CancellationToken cancellationToken, DatabaseActions databaseActions, string where, int concurrencyResultId, params string[] selectColumns) { return GetRunResults(cancellationToken, databaseActions, where, new int[] { concurrencyResultId }, selectColumns); }
        public static DataTable GetRunResults(CancellationToken cancellationToken, DatabaseActions databaseActions, int[] concurrencyResultIds, params string[] selectColumns) { return GetRunResults(cancellationToken, databaseActions, null, concurrencyResultIds, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="where"></param>
        /// <param name="concurrencyResultIds">If only one Id is given for a tests divided over multiple stress tests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetRunResults(CancellationToken cancellationToken, DatabaseActions databaseActions, string where, int[] concurrencyResultIds, params string[] selectColumns) {
            if (GetConcurrencyResultIdsAndSiblings(cancellationToken, databaseActions, concurrencyResultIds).Length == concurrencyResultIds.Length) { //No divided stress tests, so we can immediatly return results.
                if (concurrencyResultIds.Length == 0)
                    return databaseActions.GetDataTable(string.Format("Select {0} From runresults{1};", GetValidSelect(selectColumns), GetValidWhere(where, true)));

                return databaseActions.GetDataTable(string.Format("Select {0} From runresults Where ConcurrencyResultId In({1}){2};", GetValidSelect(selectColumns), concurrencyResultIds.Combine(", "), GetValidWhere(where, false)));
            }

            int totalRequestCountIndex = 3;
            int rerunCountIndex = 4;
            int startedAtIndex = 5;
            int stoppedAtIndex = 6;

            if (selectColumns.Length != 0) {
                totalRequestCountIndex = selectColumns.IndexOf("TotalRequestCount");
                rerunCountIndex = selectColumns.IndexOf("RerunCount");
                startedAtIndex = selectColumns.IndexOf("StartedAt");
                stoppedAtIndex = selectColumns.IndexOf("StoppedAt");

                if (!selectColumns.Contains("ConcurrencyResultId")) {
                    var copy = new string[selectColumns.Length + 1];
                    selectColumns.CopyTo(copy, 0);
                    copy[selectColumns.Length] = "ConcurrencyResultId";
                    selectColumns = copy;
                }
            }
            var stressTestsAndDividedRows = GetStressTestsAndDividedRunResultRows(cancellationToken, databaseActions, where, concurrencyResultIds, selectColumns);
            if (cancellationToken.IsCancellationRequested) return null;

            DataTable combined = null;

            foreach (string stressTest in stressTestsAndDividedRows.Keys) {
                if (cancellationToken.IsCancellationRequested) return null;

                var dividedParts = stressTestsAndDividedRows[stressTest];

                //Pivot the data table (in a more handleble structure) to be able to do the right merge.
                var pivotedRows = Pivot(cancellationToken, dividedParts);
                if (cancellationToken.IsCancellationRequested) return null;

                //Merge
                foreach (var part in pivotedRows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    object[] row = part[0];

                    var combinedRow = new object[row.Length];
                    row.CopyTo(combinedRow, 0);

                    for (int i = 1; i != part.Count; i++) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        row = part[i];

                        if (totalRequestCountIndex != -1)
                            combinedRow[totalRequestCountIndex] = (ulong)combinedRow[totalRequestCountIndex] + (ulong)row[totalRequestCountIndex];

                        if (rerunCountIndex != -1) {
                            int rerunCount = (int)row[rerunCountIndex];
                            if (rerunCount > (int)combinedRow[rerunCountIndex])
                                combinedRow[rerunCountIndex] = rerunCount;
                        }

                        if (startedAtIndex != -1) {
                            var startedAt = (DateTime)row[startedAtIndex];
                            if (startedAt < (DateTime)combinedRow[startedAtIndex]) combinedRow[startedAtIndex] = startedAt;
                        }

                        if (stoppedAtIndex != -1) {
                            var stoppedAt = (DateTime)row[stoppedAtIndex];
                            if (stoppedAt > (DateTime)combinedRow[stoppedAtIndex]) combinedRow[stoppedAtIndex] = stoppedAt;
                        }
                    }

                    //Copy the table and column names.
                    if (combined == null) combined = dividedParts[0].Clone();
                    combined.Rows.Add(combinedRow);
                }
            }
            if (combined != null) combined.TableName = "runresults";
            return combined;
        }

        public static DataTable GetRequestResults(CancellationToken cancellationToken, DatabaseActions databaseActions, int runResultId, params string[] selectColumns) { return GetRequestResults(cancellationToken, databaseActions, null, runResultId, selectColumns); }
        public static DataTable GetRequestResults(CancellationToken cancellationToken, DatabaseActions databaseActions, string where, int runResultId, params string[] selectColumns) { return GetRequestResults(cancellationToken, databaseActions, where, new int[] { runResultId }, selectColumns); }
        public static DataTable GetRequestResults(CancellationToken cancellationToken, DatabaseActions databaseActions, int[] runResultIds, params string[] selectColumns) { return GetRequestResults(cancellationToken, databaseActions, null, runResultIds, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="where"></param>
        /// <param name="runResultIds">If only one Id is given for a tests divided over multiple stress tests those will be found and combined for you.</param>
        /// <param name="selectColumns">If none given all columns are selected. Id and VirtualUser must always be included.</param>
        /// <returns></returns>
        private static DataTable GetRequestResults(CancellationToken cancellationToken, DatabaseActions databaseActions, string where, int[] runResultIds, params string[] selectColumns) {
            if (GetRunResultIdsAndSiblings(cancellationToken, databaseActions, runResultIds).Length == runResultIds.Length) { // No divided stress tests, so we can immediatly return results.
                if (runResultIds.Length == 0)
                    return databaseActions.GetDataTable(string.Format("Select {0} From requestresults{1};", GetValidSelect(selectColumns), GetValidWhere(where, true)));

                //No need for the where if all run result ids are selected.
                DataTable runResults = databaseActions.GetDataTable("Select count(*) from runresults;");
                if((long)runResults.Rows[0].ItemArray[0] == runResultIds.LongLength)
                    return databaseActions.GetDataTable(string.Format("Select {0} From requestresults{1};", GetValidSelect(selectColumns), GetValidWhere(where, true)));

                return databaseActions.GetDataTable(string.Format("Select {0} From requestresults Where RunResultId In({1}){2};", GetValidSelect(selectColumns), runResultIds.Combine(", "), GetValidWhere(where, false)));
            }
            int idIndex = 0;
            int runResultIdIndex = 1;
            int virtualUserIndex = 2;
            if (selectColumns.Length != 0) {
                idIndex = selectColumns.IndexOf("Id");
                virtualUserIndex = selectColumns.IndexOf("VirtualUser");

                if (!selectColumns.Contains("RunResultId")) {
                    var copy = new string[selectColumns.Length + 1];
                    selectColumns.CopyTo(copy, 0);
                    copy[selectColumns.Length] = "RunResultId";
                    selectColumns = copy;
                }
                runResultIdIndex = selectColumns.IndexOf("RunResultId");
            }

            DataTable combined = null;
            string virtualUserFirstPart = "vApus Thread Pool Thread #";

            //Instead of pivotting we need to update the ids and the virtual user names, however we need to do this for each divided run in the right order.
            foreach (int runResultId in runResultIds) {
                if (cancellationToken.IsCancellationRequested) return null;

                DataTable combinedRun = null;

                Dictionary<string, List<DataTable>> stressTestsAndDividedRows = GetStressTestsAndDividedRequestResultRows(cancellationToken, databaseActions, selectColumns, where, runResultId);
                if (cancellationToken.IsCancellationRequested) return null;

                foreach (string stressTest in stressTestsAndDividedRows.Keys) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    List<DataTable> dividedParts = stressTestsAndDividedRows[stressTest];
                    ulong correctId = 0;
                    int correctRunResultId = 0;
                    var linkReplaceVirtualUsers = new Dictionary<string, string>();
                    int linkReplaceVirtualUsersCount = 0;
                    if (combinedRun == null) {
                        combinedRun = dividedParts[0];

                        //Only do this when there are multiple parts to be combined.
                        if (dividedParts.Count != 1) {
                            if (idIndex != -1)
                                correctId = (ulong)combinedRun.Rows.Count;

                            if (virtualUserIndex != -1 || runResultIdIndex != -1)
                                foreach (DataRow row in combinedRun.Rows) {
                                    if (cancellationToken.IsCancellationRequested) return null;

                                    if (virtualUserIndex != -1) {
                                        string virtualUser = row.ItemArray[virtualUserIndex] as string;
                                        if (!linkReplaceVirtualUsers.ContainsKey(virtualUser))
                                            linkReplaceVirtualUsers.Add(virtualUser, null);
                                    }

                                    if (runResultIdIndex != -1 && correctRunResultId == 0)
                                        correctRunResultId = (int)row.ItemArray[runResultIdIndex];
                                }
                        }
                    }

                    //Add instead of merge, just increase the Id and the vApus Thread Pool Thread # if needed. Only do this when there are multiple parts.
                    string[] linkReplaceVirtualUsersKeys = new string[linkReplaceVirtualUsers.Count];
                    linkReplaceVirtualUsers.Keys.CopyTo(linkReplaceVirtualUsersKeys, 0);
                    for (int i = 1; i != dividedParts.Count; i++) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        if (virtualUserIndex != -1) {
                            linkReplaceVirtualUsersCount += linkReplaceVirtualUsers.Count;
                            foreach (var s in linkReplaceVirtualUsersKeys) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                int j = int.Parse(s.Substring(virtualUserFirstPart.Length)) + linkReplaceVirtualUsersCount;
                                linkReplaceVirtualUsers[s] = virtualUserFirstPart + j;
                            }
                        }

                        var dividedPart = dividedParts[i];
                        foreach (DataRow row in dividedPart.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            var itemArray = row.ItemArray;

                            if (idIndex != -1)
                                itemArray[idIndex] = ++correctId;

                            if (runResultIdIndex != -1)
                                itemArray[runResultIdIndex] = correctRunResultId;

                            if (virtualUserIndex != -1)
                                itemArray[virtualUserIndex] = linkReplaceVirtualUsers[itemArray[virtualUserIndex] as string];

                            combinedRun.Rows.Add(itemArray);
                        }
                    }

                    if (combined == null)
                        combined = combinedRun;
                    else
                        combined.Merge(combinedRun, true);
                }
            }
            if (combined != null) combined.TableName = "requestresults";
            return combined;
        }

        public static DataTable GetMonitors(CancellationToken cancellationToken, DatabaseActions databaseActions, string where, params string[] selectColumns) { return GetMonitors(cancellationToken, databaseActions, where, new int[0], selectColumns); }
        public static DataTable GetMonitors(CancellationToken cancellationToken, DatabaseActions databaseActions, int stressTestId, params string[] selectColumns) { return GetMonitors(cancellationToken, databaseActions, null, new int[] { stressTestId }, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="stressTestIds">If only one Id is given for a tests divided over multiple stress tests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetMonitors(CancellationToken cancellationToken, DatabaseActions databaseActions, string where, int[] stressTestIds, params string[] selectColumns) {
            DataTable combined = null;
            if (databaseActions != null) {
                if (stressTestIds.Length == 0) {
                    combined = databaseActions.GetDataTable(string.Format("Select {0} From monitors{1};", GetValidSelect(selectColumns), GetValidWhere(where, true)));
                } else {
                    stressTestIds = GetStressTestIdsAndSiblings(cancellationToken, databaseActions, stressTestIds);
                    if (cancellationToken.IsCancellationRequested) return null;

                    combined = databaseActions.GetDataTable(string.Format("Select {0} From monitors Where StressTestId In({1}){2};", GetValidSelect(selectColumns), stressTestIds.Combine(", "), GetValidWhere(where, false)));
                }
            }
            if (combined != null) combined.TableName = "monitors";
            return combined;
        }

        public static DataTable GetMonitor(CancellationToken cancellationToken, DatabaseActions databaseActions, int monitorId, params string[] selectColumns) {
            return databaseActions.GetDataTable(string.Format("Select {0} From monitors Where Id={1};", GetValidSelect(selectColumns), monitorId));
        }

        public static DataTable GetMonitorResults(DatabaseActions databaseActions, int monitorId, params string[] selectColumns) { return GetMonitorResults(databaseActions, new int[] { monitorId }, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        private static DataTable GetMonitorResults(DatabaseActions databaseActions, int[] monitorIds, params string[] selectColumns) {
            DataTable combined = null;
            if (databaseActions != null)
                combined = (monitorIds.Length == 0) ?
                    databaseActions.GetDataTable(string.Format("Select {0} From monitorresults;", GetValidSelect(selectColumns))) :
                    databaseActions.GetDataTable(string.Format("Select {0} From monitorresults Where MonitorId In({1});", GetValidSelect(selectColumns), monitorIds.Combine(", ")));

            if (combined != null) combined.TableName = "monitorresults";
            return combined;
        }

        public static string GetCombinedStressTestToString(string stressTest) {
            string combined = string.Empty;
            int dotCount = 0;
            foreach (char c in stressTest) {
                if (c == '.') ++dotCount;
                if (dotCount != 2) combined += c;
            }
            if (dotCount == 2) combined += "] ";
            return combined;
        }
        #endregion

        #region Private
        /// <summary>
        /// The number of result rows in a datatable is the number of divided stress tests.
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="columns">If not null the column "StressTest" is added is not there.</param>
        /// <param name="stressTestIds"></param>
        /// <returns></returns>
        private static Dictionary<string, DataTable> GetStressTestsAndDividedStressTestRows(CancellationToken cancellationToken, DatabaseActions databaseActions, int[] stressTestIds, params string[] selectColumns) {
            if (cancellationToken.IsCancellationRequested) return null;

            var dict = new Dictionary<string, DataTable>();
            if (databaseActions != null) {
                DataTable dt = null;
                string select = GetValidSelect(selectColumns);

                if (stressTestIds.Length == 0) {
                    dt = databaseActions.GetDataTable(string.Format("Select {0} From stresstests;", select));
                } else {
                    stressTestIds = GetStressTestIdsAndSiblings(cancellationToken, databaseActions, stressTestIds);
                    if (cancellationToken.IsCancellationRequested) return null;

                    dt = databaseActions.GetDataTable(string.Format("Select {0} From stresstests Where Id In({1});", select, stressTestIds.Combine(", ")));
                }

                DataRow[] allRows = dt.Select();
                foreach (DataRow row in allRows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    string stressTest = GetCombinedStressTestToString(row["StressTest"] as string);
                    if (!dict.ContainsKey(stressTest))
                        dict.Add(stressTest, dt.Clone());
                    dict[stressTest].ImportRow(row);
                }
            }
            return dict;
        }
        /// <summary>
        /// The number of result rows in a datatable is the number of divided stress tests.
        /// </summary>
        private static Dictionary<string, DataTable> GetStressTestsAndDividedStressTestResultRows(CancellationToken cancellationToken, DatabaseActions databaseActions, int[] stressTestIds, params string[] selectColumns) {
            if (cancellationToken.IsCancellationRequested) return null;

            var dict = new Dictionary<string, DataTable>();
            if (databaseActions != null) {
                DataTable dt = null;
                if (stressTestIds.Length == 0) {
                    dt = databaseActions.GetDataTable("Select Id, StressTest From stresstests;");
                } else {
                    stressTestIds = GetStressTestIdsAndSiblings(cancellationToken, databaseActions, stressTestIds);
                    if (cancellationToken.IsCancellationRequested) return null;

                    dt = databaseActions.GetDataTable(string.Format("Select Id, StressTest From stresstests Where Id In({0});", stressTestIds.Combine(", ")));
                }

                var dictStressTest = new Dictionary<string, List<int>>();
                foreach (DataRow row in dt.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    string stressTest = GetCombinedStressTestToString(row.ItemArray[1] as string);
                    if (!dictStressTest.ContainsKey(stressTest))
                        dictStressTest.Add(stressTest, new List<int>());
                    dictStressTest[stressTest].Add((int)row.ItemArray[0]);
                }

                string select = GetValidSelect(selectColumns);
                foreach (string stressTest in dictStressTest.Keys) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    foreach (int stressTestId in dictStressTest[stressTest]) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        dt = databaseActions.GetDataTable(string.Format("Select {0} From stresstestresults Where StressTestId={1};", select, stressTestId));

                        if (dt.Rows.Count != 0) {
                            if (!dict.ContainsKey(stressTest))
                                dict.Add(stressTest, dt.Clone());

                            dict[stressTest].ImportRow(dt.Rows[0]);
                        }
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="where"></param>
        /// <param name="stressTestResultIds"></param>
        /// <param name="selectColumns"></param>
        /// <returns>Key: StressTest, Value: List of data tables, each entry stands for one divided test.</returns>
        private static Dictionary<string, List<DataTable>> GetStressTestsAndDividedConcurrencyResultRows(CancellationToken cancellationToken, DatabaseActions databaseActions, string where, int[] stressTestResultIds, params string[] selectColumns) {
            if (cancellationToken.IsCancellationRequested) return null;

            var dict = new Dictionary<string, List<DataTable>>();
            if (databaseActions != null) {
                var dictStressTestResults = GetStressTestsAndDividedStressTestResultRows(cancellationToken, databaseActions, new int[0], "Id");
                if (cancellationToken.IsCancellationRequested) return null;

                DataTable dt = null;
                if (stressTestResultIds.Length == 0) {
                    dt = databaseActions.GetDataTable(
                        string.Format("Select {0} From concurrencyresults{1};", GetValidSelect(selectColumns),
                        GetValidWhere(where, true)));
                } else {
                    stressTestResultIds = GetStressTestResultIdsAndSiblings(cancellationToken, databaseActions, stressTestResultIds);
                    if (cancellationToken.IsCancellationRequested) return null;

                    dt = databaseActions.GetDataTable(
                        string.Format("Select {0} From concurrencyresults Where StressTestResultId In({1}){2};", GetValidSelect(selectColumns),
                        stressTestResultIds.Combine(", "), GetValidWhere(where, false)));
                }

                DataRow[] allRows = dt.Select();
                foreach (string stressTest in dictStressTestResults.Keys) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    dict.Add(stressTest, new List<DataTable>());
                    foreach (DataRow row in dictStressTestResults[stressTest].Rows) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        var emptyCopy = dt.Clone();
                        foreach (DataRow toAdd in allRows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            if (toAdd["StressTestResultId"].Equals(row["Id"]))
                                emptyCopy.ImportRow(toAdd);
                        }

                        if (emptyCopy.Rows.Count != 0)
                            dict[stressTest].Add(emptyCopy);
                    }
                    if (dict[stressTest].Count == 0)
                        dict.Remove(stressTest);
                }
            }
            return dict;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="where"></param>
        /// <param name="concurrencyResultIds"></param>
        /// <param name="selectColumns"></param>
        /// <returns>Key: StressTest, Value: List of data tables, each entry stands for one divided test.</returns>
        private static Dictionary<string, List<DataTable>> GetStressTestsAndDividedRunResultRows(CancellationToken cancellationToken, DatabaseActions databaseActions, string where, int[] concurrencyResultIds, params string[] selectColumns) {
            if (cancellationToken.IsCancellationRequested) return null;

            var dict = new Dictionary<string, List<DataTable>>();
            if (databaseActions != null) {
                var dictStressTestConcurrencyResults = GetStressTestsAndDividedConcurrencyResultRows(cancellationToken, databaseActions, null, new int[0], "Id", "StressTestResultId");
                if (cancellationToken.IsCancellationRequested) return null;

                DataTable dt = null;
                if (concurrencyResultIds.Length == 0) {
                    dt = databaseActions.GetDataTable(
                        string.Format("Select {0} From runresults{1} Order By TotalRequestCount Desc;", GetValidSelect(selectColumns),
                        GetValidWhere(where, true)));
                } else {
                    concurrencyResultIds = GetConcurrencyResultIdsAndSiblings(cancellationToken, databaseActions, concurrencyResultIds);
                    if (cancellationToken.IsCancellationRequested) return null;

                    dt = databaseActions.GetDataTable(
                        string.Format("Select {0} From runresults Where ConcurrencyResultId In({1}){2} Order By TotalRequestCount Desc;", GetValidSelect(selectColumns),
                        concurrencyResultIds.Combine(", "), GetValidWhere(where, false)));
                }

                DataRow[] allRows = dt.Select();
                foreach (string stressTest in dictStressTestConcurrencyResults.Keys) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    dict.Add(stressTest, new List<DataTable>());
                    foreach (DataTable crDt in dictStressTestConcurrencyResults[stressTest]) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        var emptyCopy = dt.Clone();
                        foreach (DataRow row in crDt.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            foreach (DataRow toAdd in allRows) {
                                if (cancellationToken.IsCancellationRequested) return null;

                                if (toAdd["ConcurrencyResultId"].Equals(row["Id"]))
                                    emptyCopy.ImportRow(toAdd);
                            }
                        }

                        if (emptyCopy.Rows.Count != 0)
                            dict[stressTest].Add(emptyCopy);
                    }
                    if (dict[stressTest].Count == 0)
                        dict.Remove(stressTest);
                }
            }
            return dict;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="selectColumns"></param>
        /// <param name="where"></param>
        /// <param name="runResultIds"></param>
        /// <returns>Key: StressTest, Value: Key: Divided Results Value: Number of divided stress tests</returns>
        private static Dictionary<string, List<DataTable>> GetStressTestsAndDividedRequestResultRows(CancellationToken cancellationToken, DatabaseActions databaseActions, string[] selectColumns, string where, int runResultId) {
            if (cancellationToken.IsCancellationRequested) return null;

            var dict = new Dictionary<string, List<DataTable>>();
            if (databaseActions != null) {
                Dictionary<string, List<DataTable>> dictStressTestRunResults = null;
                if (cancellationToken.IsCancellationRequested) return null;

                DataTable dt = null;

                int[] runResultIds = GetRunResultIdsAndSiblings(cancellationToken, databaseActions, runResultId);
                if (cancellationToken.IsCancellationRequested) return null;

                string formattedRunResultIds = runResultIds.Combine(", ");
                dictStressTestRunResults = GetStressTestsAndDividedRunResultRows(cancellationToken, databaseActions, string.Format("Id In({0})", formattedRunResultIds), new int[0], "Id", "ConcurrencyResultId");

                dt = databaseActions.GetDataTable(
                    string.Format("Select {0} From requestresults Where RunResultId In({1}){2};", GetValidSelect(selectColumns), formattedRunResultIds, GetValidWhere(where, false)));

                if (runResultIds.Length == 1) { //No expensive copying needed.
                    string stressTest = dictStressTestRunResults.GetKeyAt(0);
                    dict.Add(stressTest, new List<DataTable>());
                    dict[stressTest].Add(dt);

                    if (dict[stressTest].Count == 0) dict.Remove(stressTest);
                } else {

                    DataRow[] allRows = dt.Select();
                    foreach (string stressTest in dictStressTestRunResults.Keys) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        dict.Add(stressTest, new List<DataTable>());
                        foreach (DataTable rrDt in dictStressTestRunResults[stressTest]) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            var copy = dt.Clone();
                            foreach (DataRow row in rrDt.Rows)
                                foreach (DataRow toAdd in allRows)
                                    if (toAdd["RunResultId"].Equals(row["Id"]))
                                        copy.ImportRow(toAdd);

                            if (copy.Rows.Count != 0)
                                dict[stressTest].Add(copy);
                        }
                        if (dict[stressTest].Count == 0) dict.Remove(stressTest);
                    }
                }
            }
            return dict;
        }

        private static int[] GetStressTestIdsAndSiblings(CancellationToken cancellationToken, DatabaseActions databaseActions, int[] stressTestIds) {
            if (cancellationToken.IsCancellationRequested) return null;

            var l = new List<int>();
            if (databaseActions != null) {
                var dt = databaseActions.GetDataTable("Select Id, StressTest From stresstests;");
                var foundCombinedStressTestToStrings = new List<string>();
                foreach (DataRow row in dt.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    int id = (int)row.ItemArray[0];
                    if (stressTestIds.Contains(id)) {
                        string stressTest = row.ItemArray[1] as string;
                        string combined = TrimDividedPart(stressTest);
                        if (!foundCombinedStressTestToStrings.Contains(combined))
                            foundCombinedStressTestToStrings.Add(combined);
                    }
                }
                foreach (DataRow row in dt.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    string stressTest = row.ItemArray[1] as string;
                    foreach (string combined in foundCombinedStressTestToStrings) {
                        if (cancellationToken.IsCancellationRequested) return null;

                        if (stressTest.Contains(combined)) {
                            int id = (int)row.ItemArray[0];
                            l.Add((int)id);
                            break;
                        }
                    }
                }
            }
            l.Sort();
            return l.ToArray();
        }
        private static int[] GetStressTestResultIdsAndSiblings(CancellationToken cancellationToken, DatabaseActions databaseActions, int[] stressTestResultIds) {
            if (cancellationToken.IsCancellationRequested) return null;

            var l = new List<int>();
            if (databaseActions != null) {
                //Link to FK table.
                var dt = databaseActions.GetDataTable(string.Format("Select StressTestId From stresstestresults Where Id in({0});", stressTestResultIds.Combine(", ")));

                var stressTestIds = new int[dt.Rows.Count];
                int i = 0;
                foreach (DataRow row in dt.Rows)
                    stressTestIds[i++] = (int)row.ItemArray[0];

                stressTestIds = GetStressTestIdsAndSiblings(cancellationToken, databaseActions, stressTestIds);
                if (cancellationToken.IsCancellationRequested) return null;

                dt = databaseActions.GetDataTable(string.Format("Select Id From stresstestresults Where StressTestId in({0});", stressTestIds.Combine(", ")));

                foreach (DataRow row in dt.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    l.Add((int)row.ItemArray[0]);
                }
            }

            l.Sort();
            return l.ToArray();
        }
        private static int[] GetConcurrencyResultIdsAndSiblings(CancellationToken cancellationToken, DatabaseActions databaseActions, int[] concurrencyResultIds) {
            if (cancellationToken.IsCancellationRequested) return null;

            var l = new List<int>();
            if (databaseActions != null) {
                //Find to which combined stress test the different concurrencies belong to.
                var dt = databaseActions.GetDataTable(string.Format("Select Id, StressTestResultId From concurrencyresults Where Id in({0});", concurrencyResultIds.Combine(", ")));

                foreach (DataRow row in dt.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    int concurrencyResultId = (int)row["Id"];
                    l.Add(concurrencyResultId);

                    int stressTestResultId = (int)row["StressTestResultId"];

                    int[] stressTestResultIds = GetStressTestResultIdsAndSiblings(cancellationToken, databaseActions, new int[] { stressTestResultId });
                    if (cancellationToken.IsCancellationRequested) return null;

                    if (stressTestResultIds.Length != 1) {
                        //Find the index of the concurrency.
                        int concurrencyIndex = 0;
                        var filterOnStressTestResultId = databaseActions.GetDataTable(string.Format("Select Id, StressTestResultId From concurrencyresults Where StressTestResultId={0};", stressTestResultId));
                        foreach (DataRow row2 in filterOnStressTestResultId.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            if ((int)row2["Id"] == concurrencyResultId)
                                break;
                            ++concurrencyIndex;
                        }

                        //Find the other concurrency indices.
                        for (int i = 0; i != stressTestResultIds.Length; i++) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            if (stressTestResultIds[i] != stressTestResultId) {
                                filterOnStressTestResultId = databaseActions.GetDataTable(string.Format("Select Id From concurrencyresults Where StressTestResultId={0};", stressTestResultIds[i]));
                                if (concurrencyIndex < filterOnStressTestResultId.Rows.Count) {
                                    DataRow row2 = filterOnStressTestResultId.Rows[concurrencyIndex];
                                    int id = (int)row2["Id"];
                                    if (!l.Contains(id))
                                        l.Add(id);
                                }
                            }
                        }
                    }
                }
            }
            l.Sort();
            return l.ToArray();
        }
        private static int[] GetRunResultIdsAndSiblings(CancellationToken cancellationToken, DatabaseActions databaseActions, int[] runResultIds) {
            if (cancellationToken.IsCancellationRequested) return null;

            var l = new List<int>();
            if (databaseActions != null) {
                //Find to which concurrencies the runs belong to.
                var dt = databaseActions.GetDataTable(string.Format("Select Id, ConcurrencyResultId From runresults Where Id in({0});", runResultIds.Combine(", ")));

                foreach (DataRow row in dt.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    int runResultId = (int)row["Id"];
                    l.Add(runResultId);

                    int concurrencyResultId = (int)row["ConcurrencyResultId"];

                    int[] concurrencyResultIds = GetConcurrencyResultIdsAndSiblings(cancellationToken, databaseActions, new int[] { concurrencyResultId });
                    if (cancellationToken.IsCancellationRequested) return null;

                    if (concurrencyResultIds.Length != 1) {
                        //Find the index of the run.
                        int runIndex = 0;
                        var filterOnConcurrencyResultId = databaseActions.GetDataTable(string.Format("Select Id, ConcurrencyResultId From runresults Where ConcurrencyResultId={0};", concurrencyResultId));
                        foreach (DataRow row2 in filterOnConcurrencyResultId.Rows) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            if ((int)row2["Id"] == runResultId)
                                break;
                            ++runIndex;
                        }

                        //Find the other concurrency indices.
                        for (int i = 0; i != concurrencyResultIds.Length; i++) {
                            if (cancellationToken.IsCancellationRequested) return null;

                            if (concurrencyResultIds[i] != concurrencyResultId) {
                                filterOnConcurrencyResultId = databaseActions.GetDataTable(string.Format("Select Id From runresults Where ConcurrencyResultId={0};", concurrencyResultIds[i]));
                                if (runIndex < filterOnConcurrencyResultId.Rows.Count) {
                                    DataRow row2 = filterOnConcurrencyResultId.Rows[runIndex];
                                    int id = (int)row2["Id"];
                                    if (!l.Contains(id))
                                        l.Add(id);
                                }
                            }
                        }
                    }
                }
            }
            l.Sort();
            return l.ToArray();
        }
        private static int[] GetRunResultIdsAndSiblings(CancellationToken cancellationToken, DatabaseActions databaseActions, int runResultId) {
            return GetRunResultIdsAndSiblings(cancellationToken, databaseActions, new int[] { runResultId });
        }

        private static string TrimDividedPart(string stressTest) {
            string s = string.Empty;
            int dotCount = 0;
            foreach (char c in stressTest) {
                if (c == '.') ++dotCount;
                if (dotCount != 2) s += c;
            }
            return s;
        }

        private static string GetValidSelect(string[] selectColumns) { return (selectColumns == null || selectColumns.Length == 0) ? "*" : selectColumns.Combine(", "); }
        private static string GetValidWhere(string where, bool mustStartWithWhere) {
            if (where == null) {
                where = string.Empty;
            } else {
                where = where.Trim();
                if (!mustStartWithWhere && where.StartsWith("where", StringComparison.OrdinalIgnoreCase))
                    where = where.Substring(5);
                else if (mustStartWithWhere && !where.StartsWith("where", StringComparison.OrdinalIgnoreCase))
                    where = " Where " + where;

                if (!mustStartWithWhere && !where.StartsWith("And", StringComparison.OrdinalIgnoreCase))
                    where = " And " + where;
            }
            return where;
        }

        private static DataTable CreateEmptyDataTable(string name, string[] columnNames) {
            var objectType = typeof(object);
            var dataTable = new DataTable(name);
            foreach (string columnName in columnNames) dataTable.Columns.Add(columnName, objectType);
            return dataTable;
        }

        private static List<List<object[]>> Pivot(CancellationToken cancellationToken, List<DataTable> dt) {
            int count = dt.Count;

            //Add the rows to a more handleble structure.
            var toBePivotedRows = new List<List<object[]>>();
            int maxRowCount = 0;
            foreach (var part in dt) {
                if (cancellationToken.IsCancellationRequested) return null;

                var l = new List<object[]>(part.Rows.Count);
                foreach (DataRow row in part.Rows) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    l.Add(row.ItemArray);
                }
                toBePivotedRows.Add(l);

                if (l.Count > maxRowCount) maxRowCount = l.Count;
            }

            //Pivot the parts
            var pivotedRows = new List<List<object[]>>();
            for (int i = 0; i != maxRowCount; i++) {
                if (cancellationToken.IsCancellationRequested) return null;

                var l = new List<object[]>(count);
                for (int j = 0; j < count; j++) {
                    if (cancellationToken.IsCancellationRequested) return null;

                    var part = toBePivotedRows[j];
                    if (i < part.Count) l.Add(part[i]);
                }
                pivotedRows.Add(l);
            }

            return pivotedRows;
        }
        #endregion
    }
}
