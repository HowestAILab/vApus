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
using vApus.Util;

namespace vApus.Results {
    /// <summary>
    /// An extra layer that serves to handeling results from many-to-one distributed tests (a stresstests workload divided over multiple slaves). It makes the data look like it is for 1 test instead of multiple; ResultsHelper should use this layer.
    /// Caching combined data is something that does not happen, but this can be put in place if the need is there.
    /// </summary>
    public static class ReaderAndCombiner {

        #region Public
        public static DataTable GetDescription(DatabaseActions databaseActions) { return databaseActions.GetDataTable("Select * FROM Description"); }
        public static DataTable GetTags(DatabaseActions databaseActions) { return databaseActions.GetDataTable("Select * FROM Tags"); }

        /// <summary>
        /// Get all the vApus instances used, divided stresstests are not taken into account.
        /// </summary>
        public static DataTable GetvApusInstances(DatabaseActions databaseActions) { return databaseActions.GetDataTable("Select * FROM vApusInstances"); }

        public static DataTable GetStresstests(DatabaseActions databaseActions, params string[] selectColumns) { return GetStresstests(databaseActions, new int[0], selectColumns); }
        public static DataTable GetStresstests(DatabaseActions databaseActions, int stresstestId, params string[] selectColumns) { return GetStresstests(databaseActions, new int[] { stresstestId }, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="stresstestIds">If only one Id is given for a tests divided over multiple stresstests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetStresstests(DatabaseActions databaseActions, int[] stresstestIds, params string[] selectColumns) {
            //The select columns can place a column at another index.
            int stresstestIndex = 2;

            if (selectColumns.Length != 0) {
                stresstestIndex = selectColumns.IndexOf("Stresstest");
                if (!selectColumns.Contains("Stresstest")) {
                    var copy = new string[selectColumns.Length + 1];
                    selectColumns.CopyTo(copy, 0);
                    copy[selectColumns.Length] = "Stresstest";
                    selectColumns = copy;
                }
            }

            var stresstestsAndDividedRows = GetStresstestsAndDividedStresstestRows(databaseActions, stresstestIds, selectColumns);

            DataTable combined = null;

            foreach (string stresstest in stresstestsAndDividedRows.Keys) {
                var part = stresstestsAndDividedRows[stresstest];
                object[] row = part.Rows[0].ItemArray;

                var combinedRow = new object[row.Length];
                row.CopyTo(combinedRow, 0);

                if (stresstestIndex != -1)
                    combinedRow[stresstestIndex] = stresstest;

                if (combined == null) combined = MakeEmptyCopy(part);
                combined.Rows.Add(combinedRow);
            }

            return combined;
        }

        public static DataTable GetStresstestResults(DatabaseActions databaseActions, int stresstestId, params string[] selectColumns) { return GetStresstestResults(databaseActions, new int[] { stresstestId }, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="stresstestIds">If only one Id is given for a tests divided over multiple stresstests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetStresstestResults(DatabaseActions databaseActions, int[] stresstestIds, params string[] selectColumns) {
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

            var stresstestsAndDividedRows = GetStresstestsAndDividedStresstestResultRows(databaseActions, stresstestIds, selectColumns);

            DataTable combined = null;

            foreach (string stresstest in stresstestsAndDividedRows.Keys) {
                var part = stresstestsAndDividedRows[stresstest];
                object[] row = part.Rows[0].ItemArray;

                var combinedRow = new object[row.Length];
                row.CopyTo(combinedRow, 0);

                for (int i = 1; i != part.Rows.Count; i++) {
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

                if (combined == null) combined = MakeEmptyCopy(part);
                combined.Rows.Add(combinedRow);
            }

            return combined;
        }

        public static DataTable GetConcurrencyResults(DatabaseActions databaseActions, int stresstestResultId, params string[] selectColumns) { return GetConcurrencyResults(databaseActions, null, stresstestResultId, selectColumns); }
        public static DataTable GetConcurrencyResults(DatabaseActions databaseActions, string where, int stresstestResultId, params string[] selectColumns) { return GetConcurrencyResults(databaseActions, where, new int[] { stresstestResultId }, selectColumns); }
        public static DataTable GetConcurrencyResults(DatabaseActions databaseActions, int[] stresstestResultIds, params string[] selectColumns) { return GetConcurrencyResults(databaseActions, null, stresstestResultIds, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="where"></param>
        /// <param name="stresstestResultIds">If only one Id is given for a tests divided over multiple stresstests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetConcurrencyResults(DatabaseActions databaseActions, string where, int[] stresstestResultIds, params string[] selectColumns) {
            int concurrencyIndex = 2;
            int startedAtIndex = 3;
            int stoppedAtIndex = 4;

            if (selectColumns.Length != 0) {
                concurrencyIndex = selectColumns.IndexOf("Concurrency");
                startedAtIndex = selectColumns.IndexOf("StartedAt");
                stoppedAtIndex = selectColumns.IndexOf("StoppedAt");

                if (!selectColumns.Contains("StresstestResultId")) {
                    var copy = new string[selectColumns.Length + 1];
                    selectColumns.CopyTo(copy, 0);
                    copy[selectColumns.Length] = "StresstestResultId";
                    selectColumns = copy;
                }
            }
            var stresstestsAndDividedRows = GetStresstestsAndDividedConcurrencyResultRows(databaseActions, where, stresstestResultIds, selectColumns);

            DataTable combined = null;

            foreach (string stresstest in stresstestsAndDividedRows.Keys) {
                var dividedParts = stresstestsAndDividedRows[stresstest];

                //Pivot the data table (in a more handleble structure) to be able to do the right merge.
                var pivotedRows = Pivot(dividedParts);

                //Merge
                foreach (var part in pivotedRows) {
                    object[] row = part[0];

                    var combinedRow = new object[row.Length];
                    row.CopyTo(combinedRow, 0);

                    for (int i = 1; i != part.Count; i++) {
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
                    if (combined == null) combined = MakeEmptyCopy(dividedParts[0]);
                    combined.Rows.Add(combinedRow);
                }
            }

            return combined;
        }

        public static DataTable GetRunResults(DatabaseActions databaseActions, int concurrencyResultId, params string[] selectColumns) { return GetRunResults(databaseActions, null, concurrencyResultId, selectColumns); }
        public static DataTable GetRunResults(DatabaseActions databaseActions, string where, int concurrencyResultId, params string[] selectColumns) { return GetRunResults(databaseActions, where, new int[] { concurrencyResultId }, selectColumns); }
        public static DataTable GetRunResults(DatabaseActions databaseActions, int[] concurrencyResultIds, params string[] selectColumns) { return GetRunResults(databaseActions, null, concurrencyResultIds, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="where"></param>
        /// <param name="concurrencyResultIds">If only one Id is given for a tests divided over multiple stresstests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetRunResults(DatabaseActions databaseActions, string where, int[] concurrencyResultIds, params string[] selectColumns) {
            int totalLogEntryCountIndex = 3;
            int rerunCountIndex = 4;
            int startedAtIndex = 5;
            int stoppedAtIndex = 6;

            if (selectColumns.Length != 0) {
                totalLogEntryCountIndex = selectColumns.IndexOf("TotalLogEntryCount");
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
            var stresstestsAndDividedRows = GetStresstestsAndDividedRunResultRows(databaseActions, where, concurrencyResultIds, selectColumns);

            DataTable combined = null;

            foreach (string stresstest in stresstestsAndDividedRows.Keys) {
                var dividedParts = stresstestsAndDividedRows[stresstest];

                //Pivot the data table (in a more handleble structure) to be able to do the right merge.
                var pivotedRows = Pivot(dividedParts);

                //Merge
                foreach (var part in pivotedRows) {
                    object[] row = part[0];

                    var combinedRow = new object[row.Length];
                    row.CopyTo(combinedRow, 0);

                    for (int i = 1; i != part.Count; i++) {
                        row = part[i];

                        if (totalLogEntryCountIndex != -1)
                            combinedRow[totalLogEntryCountIndex] = (ulong)combinedRow[totalLogEntryCountIndex] + (ulong)row[totalLogEntryCountIndex];

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
                    if (combined == null) combined = MakeEmptyCopy(dividedParts[0]);
                    combined.Rows.Add(combinedRow);
                }
            }

            return combined;
        }

        public static DataTable GetLogEntryResults(DatabaseActions databaseActions, int runResultId, params string[] selectColumns) { return GetLogEntryResults(databaseActions, null, runResultId, selectColumns); }
        public static DataTable GetLogEntryResults(DatabaseActions databaseActions, string where, int runResultId, params string[] selectColumns) { return GetLogEntryResults(databaseActions, where, new int[] { runResultId }, selectColumns); }
        public static DataTable GetLogEntryResults(DatabaseActions databaseActions, int[] runResultIds, params string[] selectColumns) { return GetLogEntryResults(databaseActions, null, runResultIds, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="where"></param>
        /// <param name="runResultIds">If only one Id is given for a tests divided over multiple stresstests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetLogEntryResults(DatabaseActions databaseActions, string where, int[] runResultIds, params string[] selectColumns) {
            int idIndex = 0;
            int runResultIdIndex = 1;
            int virtualUserIndex = 2;
            if (selectColumns.Length != 0) {
                idIndex = selectColumns.IndexOf("Id");
                virtualUserIndex = selectColumns.IndexOf("VirtualUser");
                runResultIdIndex = selectColumns.IndexOf("RunResultId");

                if (!selectColumns.Contains("RunResultId")) {
                    var copy = new string[selectColumns.Length + 1];
                    selectColumns.CopyTo(copy, 0);
                    copy[selectColumns.Length] = "RunResultId";
                    selectColumns = copy;
                }
            }

            DataTable combined = null;
            string virtualUserFirstPart = "vApus Thread Pool Thread #";

            //Instead of pivotting we need to update the ids and the virtual user names, however we need to do this for each divided run in the right order.
            foreach (int runResultId in runResultIds) {
                DataTable combinedRun = null;

                var stresstestsAndDividedRows = GetStresstestsAndDividedLogEntryResultRows(databaseActions, selectColumns, where, runResultId);

                foreach (string stresstest in stresstestsAndDividedRows.Keys) {
                    var dividedParts = stresstestsAndDividedRows[stresstest];
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
                                    if (virtualUserIndex != -1) {
                                        string virtualUser = row.ItemArray[virtualUserIndex] as string;
                                        if (!linkReplaceVirtualUsers.ContainsKey(virtualUser))
                                            linkReplaceVirtualUsers.Add(virtualUser, null);
                                    }

                                    if (runResultIdIndex != -1 && correctRunResultId == 0)
                                        correctRunResultId = (int)row.ItemArray[correctRunResultId];
                                }
                        }
                    }

                    //Add instead of merge, just increase the Id and the vApus Thread Pool Thread # if needed.
                    string[] linkReplaceVirtualUsersKeys = new string[linkReplaceVirtualUsers.Count];
                    linkReplaceVirtualUsers.Keys.CopyTo(linkReplaceVirtualUsersKeys, 0);
                    for (int i = 1; i != dividedParts.Count; i++) {
                        if (virtualUserIndex != -1) {
                            linkReplaceVirtualUsersCount += linkReplaceVirtualUsers.Count;
                            foreach (var s in linkReplaceVirtualUsersKeys) {
                                int j = int.Parse(s.Substring(virtualUserFirstPart.Length)) + linkReplaceVirtualUsersCount;
                                linkReplaceVirtualUsers[s] = virtualUserFirstPart + j;
                            }
                        }

                        var dividedPart = dividedParts[i];
                        foreach (DataRow row in dividedPart.Rows) {
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
                        combined = MakeEmptyCopy(combinedRun);

                    foreach (DataRow row in combinedRun.Rows)
                        combined.Rows.Add(row.ItemArray);
                }
            }
            return combined;
        }

        public static DataTable GetMonitors(DatabaseActions databaseActions, params string[] selectColumns) { return GetMonitors(databaseActions, new int[0], selectColumns); }
        public static DataTable GetMonitors(DatabaseActions databaseActions, int stresstestId, params string[] selectColumns) { return GetMonitors(databaseActions, new int[] { stresstestId }, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="stresstestIds">If only one Id is given for a tests divided over multiple stresstests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetMonitors(DatabaseActions databaseActions, int[] stresstestIds, params string[] selectColumns) {
            DataTable dt = null;
            if (databaseActions != null)
                if (stresstestIds.Length == 0) {
                    dt = databaseActions.GetDataTable(string.Format("Select {0} From Monitors;", GetValidSelect(selectColumns)));
                } else {
                    stresstestIds = GetStresstestIdsAndSiblings(databaseActions, stresstestIds);
                    dt = databaseActions.GetDataTable(string.Format("Select {0} From Monitors Where StresstestId In({1});", GetValidSelect(selectColumns), stresstestIds.Combine(", ")));
                }
            return dt;
        }

        public static DataTable GetMonitorResults(DatabaseActions databaseActions, int monitorId, params string[] selectColumns) { return GetMonitorResults(databaseActions, new int[] { monitorId }, selectColumns); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="stresstestIds">If only one Id is given for a tests divided over multiple stresstests those will be found and combined for you.</param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        public static DataTable GetMonitorResults(DatabaseActions databaseActions, int[] monitorIds, params string[] selectColumns) {
            if (databaseActions != null)
                return (monitorIds.Length == 0) ?
                    databaseActions.GetDataTable(string.Format("Select {0} From MonitorResults;", GetValidSelect(selectColumns))) :
                    databaseActions.GetDataTable(string.Format("Select {0} From MonitorResults Where MonitorId In({1});", GetValidSelect(selectColumns), monitorIds.Combine(", ")));
            return null;
        }

        public static string GetCombinedStresstestToString(string stresstest) {
            string combined = string.Empty;
            int dotCount = 0;
            foreach (char c in stresstest) {
                if (c == '.') ++dotCount;
                if (dotCount != 2) combined += c;
            }
            if (dotCount == 2) combined += "] ";
            return combined;
        }
        #endregion

        #region Private
        /// <summary>
        /// The number of result rows in a datatable is the number of divided stresstests.
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="columns">If not null the column "Stresstest" is added is not there.</param>
        /// <param name="stresstestIds"></param>
        /// <returns></returns>
        private static Dictionary<string, DataTable> GetStresstestsAndDividedStresstestRows(DatabaseActions databaseActions, int[] stresstestIds, params string[] selectColumns) {
            var dict = new Dictionary<string, DataTable>();
            if (databaseActions != null) {
                DataTable dt = null;
                string select = GetValidSelect(selectColumns);

                if (stresstestIds.Length == 0) {
                    dt = databaseActions.GetDataTable(string.Format("Select {0} From Stresstests;", select));
                } else {
                    stresstestIds = GetStresstestIdsAndSiblings(databaseActions, stresstestIds);
                    dt = databaseActions.GetDataTable(string.Format("Select {0} From Stresstests Where Id In({1});", select, stresstestIds.Combine(", ")));
                }

                foreach (DataRow row in dt.Rows) {
                    string stresstest = GetCombinedStresstestToString(row["Stresstest"] as string);
                    if (!dict.ContainsKey(stresstest))
                        dict.Add(stresstest, MakeEmptyCopy(dt));
                    dict[stresstest].Rows.Add(row.ItemArray);
                }
            }
            return dict;
        }
        /// <summary>
        /// The number of result rows in a datatable is the number of divided stresstests.
        /// </summary>
        private static Dictionary<string, DataTable> GetStresstestsAndDividedStresstestResultRows(DatabaseActions databaseActions, int[] stresstestIds, params string[] selectColumns) {
            var dict = new Dictionary<string, DataTable>();
            if (databaseActions != null) {

                DataTable dt = null;
                if (stresstestIds.Length == 0) {
                    dt = databaseActions.GetDataTable("Select Id, Stresstest From Stresstests;");
                } else {
                    stresstestIds = GetStresstestIdsAndSiblings(databaseActions, stresstestIds);
                    dt = databaseActions.GetDataTable(string.Format("Select Id, Stresstest From Stresstests Where Id In({0});", stresstestIds.Combine(", ")));
                }

                var dictStresstest = new Dictionary<string, List<int>>();
                foreach (DataRow row in dt.Rows) {
                    string stresstest = GetCombinedStresstestToString(row.ItemArray[1] as string);
                    if (!dictStresstest.ContainsKey(stresstest))
                        dictStresstest.Add(stresstest, new List<int>());
                    dictStresstest[stresstest].Add((int)row.ItemArray[0]);
                }

                string select = GetValidSelect(selectColumns);
                foreach (string stresstest in dictStresstest.Keys) {
                    foreach (int stresstestId in dictStresstest[stresstest]) {
                        dt = databaseActions.GetDataTable(string.Format("Select {0} From StresstestResults Where StresstestId={1};", select, stresstestId));

                        if (dt.Rows.Count != 0) {
                            if (!dict.ContainsKey(stresstest))
                                dict.Add(stresstest, MakeEmptyCopy(dt));

                            dict[stresstest].Rows.Add(dt.Rows[0].ItemArray);
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
        /// <param name="stresstestResultIds"></param>
        /// <param name="selectColumns"></param>
        /// <returns>Key: Stresstest, Value: List of data tables, each entry stands for one divided test.</returns>
        private static Dictionary<string, List<DataTable>> GetStresstestsAndDividedConcurrencyResultRows(DatabaseActions databaseActions, string where, int[] stresstestResultIds, params string[] selectColumns) {
            var dict = new Dictionary<string, List<DataTable>>();
            if (databaseActions != null) {
                var dictStresstestResults = GetStresstestsAndDividedStresstestResultRows(databaseActions, new int[0], "Id");

                DataTable dt = null;
                if (stresstestResultIds.Length == 0) {
                    dt = databaseActions.GetDataTable(
                        string.Format("Select {0} From ConcurrencyResults{1};", GetValidSelect(selectColumns),
                        GetValidWhere(where, true)));
                } else {
                    stresstestResultIds = GetStresstestResultIdsAndSiblings(databaseActions, stresstestResultIds);
                    dt = databaseActions.GetDataTable(
                        string.Format("Select {0} From ConcurrencyResults Where StresstestResultId In({1}){2};", GetValidSelect(selectColumns),
                        stresstestResultIds.Combine(", "), GetValidWhere(where, false)));
                }

                foreach (string stresstest in dictStresstestResults.Keys) {
                    dict.Add(stresstest, new List<DataTable>());
                    foreach (DataRow row in dictStresstestResults[stresstest].Rows) {
                        var emptyCopy = MakeEmptyCopy(dt);
                        foreach (DataRow toAdd in dt.Rows)
                            if (toAdd["StresstestResultId"].Equals(row["Id"]))
                                emptyCopy.Rows.Add(toAdd.ItemArray);

                        if (emptyCopy.Rows.Count != 0)
                            dict[stresstest].Add(emptyCopy);
                    }
                    if (dict[stresstest].Count == 0)
                        dict.Remove(stresstest);
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
        /// <returns>Key: Stresstest, Value: List of data tables, each entry stands for one divided test.</returns>
        private static Dictionary<string, List<DataTable>> GetStresstestsAndDividedRunResultRows(DatabaseActions databaseActions, string where, int[] concurrencyResultIds, params string[] selectColumns) {
            var dict = new Dictionary<string, List<DataTable>>();
            if (databaseActions != null) {
                var dictStresstestConcurrencyResults = GetStresstestsAndDividedConcurrencyResultRows(databaseActions, null, new int[0], "Id", "StresstestResultId");

                DataTable dt = null;
                if (concurrencyResultIds.Length == 0) {
                    dt = databaseActions.GetDataTable(
                        string.Format("Select {0} From RunResults{1} Order By TotalLogEntryCount Desc;", GetValidSelect(selectColumns),
                        GetValidWhere(where, true)));
                } else {
                    concurrencyResultIds = GetConcurrencyResultIdsAndSiblings(databaseActions, concurrencyResultIds);
                    dt = databaseActions.GetDataTable(
                        string.Format("Select {0} From RunResults Where ConcurrencyResultId In({1}){2} Order By TotalLogEntryCount Desc;", GetValidSelect(selectColumns),
                        concurrencyResultIds.Combine(", "), GetValidWhere(where, false)));
                }

                foreach (string stresstest in dictStresstestConcurrencyResults.Keys) {
                    dict.Add(stresstest, new List<DataTable>());
                    foreach (var crDt in dictStresstestConcurrencyResults[stresstest]) {
                        var emptyCopy = MakeEmptyCopy(dt);
                        foreach (DataRow row in crDt.Rows)
                            foreach (DataRow toAdd in dt.Rows)
                                if (toAdd["ConcurrencyResultId"].Equals(row["Id"]))
                                    emptyCopy.Rows.Add(toAdd.ItemArray);

                        if (emptyCopy.Rows.Count != 0)
                            dict[stresstest].Add(emptyCopy);
                    }
                    if (dict[stresstest].Count == 0)
                        dict.Remove(stresstest);
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
        /// <returns>Key: Stresstest, Value: Key: Divided Results Value: Number of divided stresstests</returns>
        private static Dictionary<string, List<DataTable>> GetStresstestsAndDividedLogEntryResultRows(DatabaseActions databaseActions, string[] selectColumns, string where, params int[] runResultIds) {
            var dict = new Dictionary<string, List<DataTable>>();
            if (databaseActions != null) {
                var dictStresstestRunResults = GetStresstestsAndDividedRunResultRows(databaseActions, null, new int[0], "Id", "ConcurrencyResultId");

                DataTable dt = null;
                if (runResultIds.Length == 0) {
                    dt = databaseActions.GetDataTable(string.Format("Select {0} From LogEntryResults{1};", GetValidSelect(selectColumns), GetValidWhere(where, true)));
                } else {
                    runResultIds = GetRunResultIdsAndSiblings(databaseActions, runResultIds);
                    dt = databaseActions.GetDataTable(
                        string.Format("Select {0} From LogEntryResults Where RunResultId In({1}){2};", GetValidSelect(selectColumns),
                        runResultIds.Combine(", "), GetValidWhere(where, false)));
                }

                foreach (string stresstest in dictStresstestRunResults.Keys) {
                    dict.Add(stresstest, new List<DataTable>());
                    foreach (var rrDt in dictStresstestRunResults[stresstest]) {
                        var emptyCopy = MakeEmptyCopy(dt);
                        foreach (DataRow row in rrDt.Rows)
                            foreach (DataRow toAdd in dt.Rows)
                                if (toAdd["RunResultId"].Equals(row["Id"]))
                                    emptyCopy.Rows.Add(toAdd.ItemArray);

                        if (emptyCopy.Rows.Count != 0)
                            dict[stresstest].Add(emptyCopy);
                    }
                    if (dict[stresstest].Count == 0)
                        dict.Remove(stresstest);
                }
            }
            return dict;
        }

        //private static Dictionary<string, List<int>> GetStresstestAndDividedIds(DatabaseActions databaseActions) {
        //    var dict = new Dictionary<string, List<int>>();
        //    if (databaseActions != null) {
        //        var dt = databaseActions.GetDataTable("Select Id, Stresstest From Stresstests;");
        //        foreach (DataRow row in dt.Rows) {
        //            int id = (int)row.ItemArray[0];
        //            string stresstest = row.ItemArray[1] as string;
        //            string combined = TrimDividedPart(stresstest);
        //            if (!dict.Keys.Contains(combined))
        //                dict.Add(combined, new List<int>());

        //            dict[combined].Add(id);
        //        }

        //        foreach (var l in dict.Values)
        //            l.Sort();
        //    }
        //    return dict;
        //}
        private static int[] GetStresstestIdsAndSiblings(DatabaseActions databaseActions, int[] stresstestIds) {
            var l = new List<int>();
            if (databaseActions != null) {
                var dt = databaseActions.GetDataTable("Select Id, Stresstest From Stresstests;");
                var foundCombinedStresstestToStrings = new List<string>();
                foreach (DataRow row in dt.Rows) {
                    int id = (int)row.ItemArray[0];
                    if (stresstestIds.Contains(id)) {
                        string stresstest = row.ItemArray[1] as string;
                        string combined = TrimDividedPart(stresstest);
                        if (!foundCombinedStresstestToStrings.Contains(combined))
                            foundCombinedStresstestToStrings.Add(combined);
                    }
                }
                foreach (DataRow row in dt.Rows) {
                    string stresstest = row.ItemArray[1] as string;
                    foreach (string combined in foundCombinedStresstestToStrings)
                        if (stresstest.Contains(combined)) {
                            int id = (int)row.ItemArray[0];
                            l.Add((int)id);
                            break;
                        }
                }
            }
            l.Sort();
            return l.ToArray();
        }
        private static int[] GetStresstestResultIdsAndSiblings(DatabaseActions databaseActions, int[] stresstestResultIds) {
            var l = new List<int>();
            if (databaseActions != null) {
                //Link to FK table.
                var dt = databaseActions.GetDataTable(string.Format("Select StresstestId From StresstestResults Where Id in({0});", stresstestResultIds.Combine(", ")));

                var stresstestIds = new int[dt.Rows.Count];
                int i = 0;
                foreach (DataRow row in dt.Rows)
                    stresstestIds[i++] = (int)row.ItemArray[0];

                stresstestIds = GetStresstestIdsAndSiblings(databaseActions, stresstestIds);

                dt = databaseActions.GetDataTable(string.Format("Select Id From StresstestResults Where StresstestId in({0});", stresstestIds.Combine(", ")));

                foreach (DataRow row in dt.Rows)
                    l.Add((int)row.ItemArray[0]);
            }

            l.Sort();
            return l.ToArray();
        }
        private static int[] GetConcurrencyResultIdsAndSiblings(DatabaseActions databaseActions, int[] concurrencyResultIds) {
            var l = new List<int>();
            if (databaseActions != null) {
                //Find to which combined stresstest the different concurrencies belong to.
                var dt = databaseActions.GetDataTable(string.Format("Select Id, StresstestResultId From ConcurrencyResults Where Id in({0});", concurrencyResultIds.Combine(", ")));

                foreach (DataRow row in dt.Rows) {
                    int concurrencyResultId = (int)row["Id"];
                    l.Add(concurrencyResultId);

                    int stresstestResultId = (int)row["StresstestResultId"];

                    int[] stresstestResultIds = GetStresstestResultIdsAndSiblings(databaseActions, new int[] { stresstestResultId });

                    if (stresstestResultIds.Length != 1) {
                        //Find the index of the concurrency.
                        int concurrencyIndex = 0;
                        var filterOnStresstestResultId = databaseActions.GetDataTable(string.Format("Select Id, StresstestResultId From ConcurrencyResults Where StresstestResultId={0};", stresstestResultId));
                        foreach (DataRow row2 in filterOnStresstestResultId.Rows) {
                            if ((int)row2["Id"] == concurrencyResultId)
                                break;
                            ++concurrencyIndex;
                        }

                        //Find the other concurrency indices.
                        for (int i = 0; i != stresstestResultIds.Length; i++)
                            if (stresstestResultIds[i] != stresstestResultId) {
                                filterOnStresstestResultId = databaseActions.GetDataTable(string.Format("Select Id From ConcurrencyResults Where StresstestResultId={0};", stresstestResultIds[i]));
                                if (concurrencyIndex < filterOnStresstestResultId.Rows.Count) {
                                    DataRow row2 = filterOnStresstestResultId.Rows[concurrencyIndex];
                                    int id = (int)row2["Id"];
                                    if (!l.Contains(id))
                                        l.Add(id);
                                }
                            }
                    }
                }
            }
            l.Sort();
            return l.ToArray();
        }
        private static int[] GetRunResultIdsAndSiblings(DatabaseActions databaseActions, int[] runResultIds) {
            var l = new List<int>();
            if (databaseActions != null) {
                //Find to which concurrencies the runs belong to.
                var dt = databaseActions.GetDataTable(string.Format("Select Id, ConcurrencyResultId From RunResults Where Id in({0});", runResultIds.Combine(", ")));

                foreach (DataRow row in dt.Rows) {
                    int runResultId = (int)row["Id"];
                    l.Add(runResultId);

                    int concurrencyResultId = (int)row["ConcurrencyResultId"];

                    int[] concurrencyResultIds = GetConcurrencyResultIdsAndSiblings(databaseActions, new int[] { concurrencyResultId });

                    if (concurrencyResultIds.Length != 1) {
                        //Find the index of the run.
                        int runIndex = 0;
                        var filterOnConcurrencyResultId = databaseActions.GetDataTable(string.Format("Select Id, ConcurrencyResultId From RunResults Where ConcurrencyResultId={0};", concurrencyResultId));
                        foreach (DataRow row2 in filterOnConcurrencyResultId.Rows) {
                            if ((int)row2["Id"] == runResultId)
                                break;
                            ++runIndex;
                        }

                        //Find the other concurrency indices.
                        for (int i = 0; i != concurrencyResultIds.Length; i++)
                            if (concurrencyResultIds[i] != concurrencyResultId) {
                                filterOnConcurrencyResultId = databaseActions.GetDataTable(string.Format("Select Id From RunResults Where ConcurrencyResultId={0};", concurrencyResultIds[i]));
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
            l.Sort();
            return l.ToArray();
        }

        private static string TrimDividedPart(string stresstest) {
            string s = string.Empty;
            int dotCount = 0;
            foreach (char c in stresstest) {
                if (c == '.') ++dotCount;
                if (dotCount != 2) s += c;
            }
            return s;
        }

        private static string GetValidSelect(string[] selectColumns) {
            return (selectColumns == null || selectColumns.Length == 0) ? "*" : selectColumns.Combine(", ");
        }
        private static string GetValidWhere(string where, bool mustStartWithWhere) {
            if (where == null) {
                where = string.Empty;
            } else {
                where = where.Trim();
                if (!mustStartWithWhere && where.StartsWith("where", StringComparison.OrdinalIgnoreCase))
                    where = where.Substring(5);
                else if (mustStartWithWhere && !where.StartsWith("where", StringComparison.OrdinalIgnoreCase))
                    where = " Where " + where;

                if (!where.StartsWith("And", StringComparison.OrdinalIgnoreCase))
                    where = " And " + where;
            }
            return where;
        }


        private static DataTable MakeEmptyCopy(DataTable from) {
            var objectType = typeof(object);
            var dataTable = new DataTable(from.TableName);
            foreach (DataColumn column in from.Columns) dataTable.Columns.Add(column.ColumnName, objectType);
            return dataTable;
        }
        private static DataTable CreateEmptyDataTable(string name, string[] columnNames) {
            var objectType = typeof(object);
            var dataTable = new DataTable(name);
            foreach (string columnName in columnNames) dataTable.Columns.Add(columnName, objectType);
            return dataTable;
        }

        private static List<List<object[]>> Pivot(List<DataTable> dt) {
            int count = dt.Count;

            //Add the rows to a more handleble structure.
            var toBePivotedRows = new List<List<object[]>>();
            int maxRowCount = 0;
            foreach (var part in dt) {
                var l = new List<object[]>(part.Rows.Count);
                foreach (DataRow row in part.Rows)
                    l.Add(row.ItemArray);
                toBePivotedRows.Add(l);

                if (l.Count > maxRowCount) maxRowCount = l.Count;
            }

            //Pivot the parts
            var pivotedRows = new List<List<object[]>>();
            for (int i = 0; i != maxRowCount; i++) {
                var l = new List<object[]>(count);
                for (int j = 0; j < count; j++) {
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
