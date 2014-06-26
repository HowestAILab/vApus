using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vApus.Util;

namespace vApus.Results {
    internal abstract class BaseResultSetCalculator {
        public abstract DataTable Get(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stresstestIds);
        protected DataTable CreateEmptyDataTable(string name, params string[] columnNames) {
            var objectType = typeof(object);
            var dataTable = new DataTable(name);
            foreach (string columnName in columnNames) dataTable.Columns.Add(columnName, objectType);
            return dataTable;
        }

        protected abstract ConcurrentDictionary<string, DataTable> GetData(DatabaseActions databaseActions, CancellationToken cancellationToken, params int[] stresstestIds);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseActions"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="runResults"></param>
        /// <param name="threads"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        protected DataTable[] GetLogEntryResultsThreaded(DatabaseActions databaseActions, CancellationToken cancellationToken, DataTable runResults, int threads, params string[] columns) {
            int runCount = runResults.Rows.Count;

            //Adaptive parallelization.
            if (threads > Environment.ProcessorCount) threads = Environment.ProcessorCount;
            if (threads > runCount) threads = runCount;

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
                    parts[i] = ReaderAndCombiner.GetLogEntryResults(cancellationToken, dba, runResultIds[i], columns);
                }
            });
            return parts;
        }
    }
}
