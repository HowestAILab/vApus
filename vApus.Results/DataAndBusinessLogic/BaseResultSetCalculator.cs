/*
 * Copyright 2014 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using vApus.Util;

namespace vApus.Results {
    internal abstract class BaseResultSetCalculator {
        public abstract DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stressTestIds);
        protected DataTable CreateEmptyDataTable(string name, params string[] columnNames) {
            var objectType = typeof(object);
            var dataTable = new DataTable(name);
            foreach (string columnName in columnNames) dataTable.Columns.Add(columnName, objectType);
            return dataTable;
        }

        /// <summary>
        /// All tables needed must be gattered here.
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="stressTestIds"></param>
        /// <returns>Label and data table</returns>
        protected abstract ConcurrentDictionary<string, DataTable> GetData(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stressTestIds);
        /// <summary>
        /// Get data tables per run. It is possible that one data table has requests with different run result ids, because of combining homogeneous results. Take this into account!
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="runResults"></param>
        /// <param name="threads">The number of threads that should be used to query the database.</param>
        /// <param name="columns"></param>
        /// <returns></returns>
        protected DataTable[] GetRequestResultsPerRunThreaded(DatabaseActions databaseActions, CancellationToken cancellationToken, DataTable runResults, int threads, params string[] columns) {
            int runCount = runResults.Rows.Count;

            //Adaptive parallelization.
            if (threads > Environment.ProcessorCount) threads = Environment.ProcessorCount;
            if (threads > runCount) threads = runCount;
            if (threads < 1) threads = 1;

            int partRange = runCount / threads;
            int remainder = runCount % threads;

            int[][] runResultIds = new int[threads][];

            int inclLower = 0;
            for (int thread = 0; thread != threads; thread++) {
                int exclUpper = inclLower + partRange;
                if (remainder != 0) {
                    ++exclUpper;
                    --remainder;
                }

                runResultIds[thread] = new int[exclUpper - inclLower];
                for (int i = inclLower; i != exclUpper; i++)
                    runResultIds[thread][i - inclLower] = (int)runResults.Rows[i][0];

                inclLower = exclUpper;
            }

            var parts = new DataTable[runResultIds.Length];
            Parallel.For(0, runResultIds.Length, (i, loopState) => {
                using (var dba = new DatabaseActions() { ConnectionString = databaseActions.ConnectionString, CommandTimeout = 600 }) {
                    if (cancellationToken.IsCancellationRequested) loopState.Break();
                    try {
                        parts[i] = ReaderAndCombiner.GetRequestResults(cancellationToken, dba, runResultIds[i], columns);
                    } catch { 
                    }
                }
            });
            return parts;
        }
    }
}
