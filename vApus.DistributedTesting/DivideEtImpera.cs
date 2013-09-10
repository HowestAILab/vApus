using System;
/*
 * Copyright 2013 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System.Collections.Generic;
using vApus.Results;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting {
    /// <summary>
    /// Holds functionality to divide workload over slaves.
    /// </summary>
    public static class DivideEtImpera {
        /// <summary>
        /// Calculates the division of workload over slaves. Jumpstarting the slaves and sending the stresstests to them happens elsewhere.
        /// </summary>
        /// <param name="distributedTest"></param>
        /// <param name="notACleanDivision">Notifies if there is a reset after dividing. This is important to know for calculating the fast results.</param>
        /// <returns>key = Divided, value = original</returns>
        public static Dictionary<TileStresstest, TileStresstest> DivideTileStresstestsOverSlaves(DistributedTest distributedTest, out bool notACleanDivision) {
            notACleanDivision = false;
            var dividedAndOriginalTileStresstests = new Dictionary<TileStresstest, TileStresstest>();
            foreach (TileStresstest tileStresstest in distributedTest.UsedTileStresstests) {
                bool b;
                foreach (var kvp in DivideTileStresstestOverSlaves(tileStresstest, out b))
                    dividedAndOriginalTileStresstests.Add(kvp.Key, kvp.Value);

                if (b) notACleanDivision = true;
            }

            return dividedAndOriginalTileStresstests;
        }
        /// <summary>
        /// Calculates the division of workload over slaves. Jumpstarting the slaves and sending the stresstests to them happens elsewhere.
        /// </summary>
        /// <param name="tileStresstest">A tile stresstest that is 'Used'.</param>
        /// <returns>key = Divided, value = original</returns>
        private static Dictionary<TileStresstest, TileStresstest> DivideTileStresstestOverSlaves(TileStresstest tileStresstest, out bool notACleanDivision) {
            notACleanDivision = false;
            int slaves = tileStresstest.BasicTileStresstest.SlaveIndices.Length;
            var dividedTileStresstestsAndOriginal = new Dictionary<TileStresstest, TileStresstest>(slaves);
            if (slaves == 1) {
                dividedTileStresstestsAndOriginal.Add(tileStresstest, tileStresstest);
            } else if (slaves != 0) {
                var addOnesPerConcurrency = new List<bool[]>();

                var concurrencies = new int[tileStresstest.AdvancedTileStresstest.Concurrencies.Length];
                for (int i = 0; i != concurrencies.Length; i++) {
                    int concurrency = tileStresstest.AdvancedTileStresstest.Concurrencies[i];
                    concurrencies[i] = concurrency / slaves;

                    int mod = concurrency % slaves;

                    bool[] addOne = new bool[slaves];
                    for (int j = 0; j != mod; j++)
                        addOne[j] = true;
                    addOnesPerConcurrency.Add(addOne);

                    notACleanDivision = mod != 0;
                    if (notACleanDivision)
                        LogWrapper.LogByLevel(tileStresstest.ToString() +
                            " The averages in the fast results will NOT be correct because one or more given concurrencies divided by the number of slaves is not an integer! Please use the detailed results." +
                            "\nIn the following example both outcomes should be the same, but that is not possible:\n\t3 concurrencies; 1 slave; a log of one entry.\n\tAvg.Response time: (10 + 7 + 9) / 3 = 26 / 3 = 8,67." +
                            "\n\t---\n\t3 concurrencies; 2 slaves; a log of one entry.\n\tAvg.Response time: (10 + (7 + 9) / 2) / 2 = 18 / 2 = 9.", LogLevel.Warning);
                }

                for (int i = 0; i != tileStresstest.BasicTileStresstest.Slaves.Length; i++) {
                    var clone = tileStresstest.Clone();
                    clone.DividedStresstestIndex = tileStresstest.TileStresstestIndex + "." + (i + 1);
                    clone.Parent = tileStresstest.Parent;
                    concurrencies.CopyTo(clone.AdvancedTileStresstest.Concurrencies, 0);
                    clone.BasicTileStresstest.Slaves = new Slave[] { tileStresstest.BasicTileStresstest.Slaves[i] };
                    dividedTileStresstestsAndOriginal.Add(clone, tileStresstest);
                }

                //Add the mod to the concurrencies.
                for (int j = 0; j != addOnesPerConcurrency.Count; j++) {
                    var addOnes = addOnesPerConcurrency[j];

                    int k = 0;
                    foreach (var clone in dividedTileStresstestsAndOriginal.Keys)
                        if (addOnes[k++])
                            clone.AdvancedTileStresstest.Concurrencies[j] += 1;
                }
            }
            return dividedTileStresstestsAndOriginal;
        }

        /// <summary>
        /// Returns the merged test progress message; if the ICollection contains only 1 value, that value is returned.
        /// </summary>
        /// <param name="tileStresstest"></param>
        /// <param name="toBeMerged"></param>
        /// <returns></returns>
        public static TestProgressMessage GetMergedTestProgressMessage(TileStresstest tileStresstest, ICollection<TestProgressMessage> toBeMerged) {
            if (toBeMerged.Count == 1) foreach (var tpm in toBeMerged) return tpm;

            var stresstestMetricsCaches = new List<StresstestMetricsCache>(toBeMerged.Count);
            foreach (var tpm in toBeMerged) stresstestMetricsCaches.Add(tpm.StresstestMetricsCache);

            if (stresstestMetricsCaches.Contains(null)) {
                //Try to return the first with a cache that is not null, otherwise return the first tpm.
                foreach (var tpm in toBeMerged) if (tpm.StresstestMetricsCache != null) return tpm;
                foreach (var tpm in toBeMerged) return tpm;
            }

            //First merge the status, events and resource usage
            var testProgressMessage = new TestProgressMessage();

            testProgressMessage.StresstestStatus = StresstestStatus.Error;
            testProgressMessage.StartedAt = DateTime.MaxValue;
            testProgressMessage.Events = new List<EventPanelEvent>();
            foreach (var tpm in toBeMerged) {
                if (tpm.CPUUsage > testProgressMessage.CPUUsage) testProgressMessage.CPUUsage = tpm.CPUUsage;
                if (tpm.ContextSwitchesPerSecond > testProgressMessage.ContextSwitchesPerSecond) testProgressMessage.ContextSwitchesPerSecond = tpm.ContextSwitchesPerSecond;

                testProgressMessage.Events.AddRange(tpm.Events);

                if (!string.IsNullOrEmpty(tpm.Exception)) {
                    if (testProgressMessage.Exception == null) testProgressMessage.Exception = string.Empty;
                    testProgressMessage.Exception += tpm.Exception + "\n";
                }
                if (tpm.MemoryUsage > testProgressMessage.MemoryUsage) testProgressMessage.MemoryUsage = tpm.MemoryUsage;
                if (tpm.NicsReceived > testProgressMessage.NicsReceived) testProgressMessage.NicsReceived = tpm.NicsReceived;
                if (tpm.NicsSent > testProgressMessage.NicsSent) testProgressMessage.NicsSent = tpm.NicsSent;
                //if (tpm.RunStateChange > testProgressMessage.RunStateChange) testProgressMessage.RunStateChange = tpm.RunStateChange; //OKAY for run sync?
                if (tpm.StresstestStatus < testProgressMessage.StresstestStatus) testProgressMessage.StresstestStatus = tpm.StresstestStatus;
                if (tpm.StartedAt < testProgressMessage.StartedAt) testProgressMessage.StartedAt = tpm.StartedAt;
                if (tpm.MeasuredRuntime > testProgressMessage.MeasuredRuntime) testProgressMessage.MeasuredRuntime = tpm.MeasuredRuntime;
                if (tpm.EstimatedRuntimeLeft > testProgressMessage.EstimatedRuntimeLeft) testProgressMessage.EstimatedRuntimeLeft = tpm.EstimatedRuntimeLeft;
                testProgressMessage.ThreadsInUse += tpm.ThreadsInUse;
                testProgressMessage.TileStresstestIndex = tileStresstest.TileStresstestIndex;
                if (tpm.TotalVisibleMemory > testProgressMessage.TotalVisibleMemory) testProgressMessage.TotalVisibleMemory = tpm.TotalVisibleMemory;
            }
            //Then the test progress
            testProgressMessage.StresstestMetricsCache = StresstestMetricsHelper.MergeStresstestMetricsCaches(stresstestMetricsCaches);

            return testProgressMessage;
        }
    }
}
