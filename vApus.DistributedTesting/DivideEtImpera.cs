/*
 * Copyright 2013 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using vApus.Results;
using vApus.Server.Shared;
using vApus.StressTest;
using vApus.Util;

namespace vApus.DistributedTest {
    /// <summary>
    /// Holds functionality to divide workload over slaves.
    /// </summary>
    public static class DivideEtImpera {
        private static object _usedTileStressTestsLock = new object();

        /// <summary>
        /// Calculates the division of workload over slaves. Jumpstarting the slaves and sending the stress tests to them happens elsewhere.
        /// </summary>
        /// <param name="distributedTest"></param>
        /// <param name="notACleanDivision">Notifies if there is a reset after dividing. This is important to know for calculating the fast results.</param>
        /// <returns>key = Divided, value = original</returns>
        public static Dictionary<TileStressTest, TileStressTest> DivideTileStressTestsOverSlaves(DistributedTest distributedTest, out bool notACleanDivision) {
            notACleanDivision = false;
            var dividedAndOriginalTileStressTests = new Dictionary<TileStressTest, TileStressTest>();
            foreach (TileStressTest tileStressTest in distributedTest.UsedTileStressTests) {
                bool b;
                foreach (var kvp in DivideTileStressTestOverSlaves(tileStressTest, out b))
                    dividedAndOriginalTileStressTests.Add(kvp.Key, kvp.Value);

                if (b) notACleanDivision = true;
            }

            return dividedAndOriginalTileStressTests;
        }
        /// <summary>
        /// Calculates the division of workload over slaves. Jumpstarting the slaves and sending the stress tests to them happens elsewhere.
        /// </summary>
        /// <param name="tileStressTest">A tile stress test that is 'Used'.</param>
        /// <returns>key = Divided, value = original</returns>
        private static Dictionary<TileStressTest, TileStressTest> DivideTileStressTestOverSlaves(TileStressTest tileStressTest, out bool notACleanDivision) {
            notACleanDivision = false;
            int slaves = tileStressTest.BasicTileStressTest.SlaveIndices.Length;
            var dividedTileStressTestsAndOriginal = new Dictionary<TileStressTest, TileStressTest>(slaves);
            if (slaves == 1) {
                dividedTileStressTestsAndOriginal.Add(tileStressTest, tileStressTest);
            } else if (slaves != 0) {
                var addOnesPerConcurrency = new List<bool[]>();

                var concurrencies = new int[tileStressTest.AdvancedTileStressTest.Concurrencies.Length];
                for (int i = 0; i != concurrencies.Length; i++) {
                    int concurrency = tileStressTest.AdvancedTileStressTest.Concurrencies[i];
                    concurrencies[i] = concurrency / slaves;

                    int mod = concurrency % slaves;

                    bool[] addOne = new bool[slaves];
                    for (int j = 0; j != mod; j++)
                        addOne[j] = true;
                    addOnesPerConcurrency.Add(addOne);

                    notACleanDivision = mod != 0;
                    if (notACleanDivision)
                        Loggers.Log(Level.Warning, tileStressTest.ToString() +
                            " The averages in the fast results will NOT be correct because one or more given concurrencies divided by the number of slaves is not an integer! Please use the detailed results." +
                            "\nIn the following example both outcomes should be the same, but that is not possible:\n\t3 concurrencies; 1 slave; a scenario of one entry.\n\tAvg.Response time: (10 + 7 + 9) / 3 = 26 / 3 = 8,67." +
                            "\n\t---\n\t3 concurrencies; 2 slaves; a scenario of one entry.\n\tAvg.Response time: (10 + (7 + 9) / 2) / 2 = 18 / 2 = 9.");
                }

                for (int i = 0; i != tileStressTest.BasicTileStressTest.Slaves.Length; i++) {
                    var clone = tileStressTest.Clone();
                    clone.DividedStressTestIndex = tileStressTest.TileStressTestIndex + "." + (i + 1);
                    clone.Parent = tileStressTest.Parent;
                    concurrencies.CopyTo(clone.AdvancedTileStressTest.Concurrencies, 0);
                    clone.BasicTileStressTest.Slaves = new Slave[] { tileStressTest.BasicTileStressTest.Slaves[i] };
                    dividedTileStressTestsAndOriginal.Add(clone, tileStressTest);
                }

                //Add the mod to the concurrencies.
                for (int j = 0; j != addOnesPerConcurrency.Count; j++) {
                    var addOnes = addOnesPerConcurrency[j];

                    int k = 0;
                    foreach (var clone in dividedTileStressTestsAndOriginal.Keys)
                        if (addOnes[k++])
                            clone.AdvancedTileStressTest.Concurrencies[j] += 1;
                }
            }
            return dividedTileStressTestsAndOriginal;
        }

        public static RunStateChange PreProcessTestProgressMessage(RunSynchronization runSynchronization, TileStressTest originalTileStressTest, TestProgressMessage tpm, Dictionary<TileStressTest, Dictionary<string, TestProgressMessage>> testProgressMessages,
            Dictionary<TileStressTest, TileStressTest> usedTileStressTests, Dictionary<TileStressTest, List<string>> dividedRunInitializedOrDoneOnce) {
            lock (_usedTileStressTestsLock) {                
                var dictParts = testProgressMessages[originalTileStressTest];
                dictParts[tpm.TileStressTestIndex] = tpm;
                if (tpm.RunStateChange == RunStateChange.ToRunInitializedFirstTime || 
                    ((runSynchronization == RunSynchronization.None || runSynchronization == RunSynchronization.BreakOnFirstFinished) && tpm.RunStateChange == RunStateChange.ToRunDoneOnce) || 
                    (runSynchronization == RunSynchronization.BreakOnLastFinished && tpm.RunFinished)) {
                    if (!dividedRunInitializedOrDoneOnce.ContainsKey(originalTileStressTest)) dividedRunInitializedOrDoneOnce.Add(originalTileStressTest, new List<string>());
                    if (!dividedRunInitializedOrDoneOnce[originalTileStressTest].Contains(tpm.TileStressTestIndex)) dividedRunInitializedOrDoneOnce[originalTileStressTest].Add(tpm.TileStressTestIndex);

                    if (GetDividedCount(originalTileStressTest.TileStressTestIndex, usedTileStressTests) == dividedRunInitializedOrDoneOnce[originalTileStressTest].Count) {
                        MasterSideCommunicationHandler.SendDividedContinue(originalTileStressTest.BasicTileStressTest.Slaves);
                        dividedRunInitializedOrDoneOnce.Remove(originalTileStressTest);

                        return tpm.RunStateChange;
                    }
                }

                return RunStateChange.None;
            }
        }

        /// <summary>
        /// Get the original tile stress test for the given divided index.
        /// </summary>
        /// <param name="dividedTileStressTestIndex"></param>
        /// <returns></returns>
        public static TileStressTest GetOriginalTileStressTest(string dividedTileStressTestIndex, Dictionary<TileStressTest, TileStressTest> usedTileStressTests) {
            foreach (TileStressTest ts in usedTileStressTests.Values)
                if (dividedTileStressTestIndex.StartsWith(ts.TileStressTestIndex)) //Take divided stress tests into account.
                    return ts;
            return null;
        }
        /// <summary>
        /// Get the count of the divided stress tests for a certain tile stress test.
        /// </summary>
        /// <param name="originalTileStressTestIndex"></param>
        /// <returns></returns>
        private static int GetDividedCount(string originalTileStressTestIndex, Dictionary<TileStressTest, TileStressTest> usedTileStressTests) {
            int count = 0;
            foreach (TileStressTest ts in usedTileStressTests.Keys)
                if (ts.TileStressTestIndex.StartsWith(originalTileStressTestIndex)) //Take divided stress tests into account.
                    ++count;
            return count;
        }

        /// <summary>
        /// Returns the merged test progress message; if the ICollection contains only 1 value, that value is returned.
        /// </summary>
        /// <param name="tileStressTest"></param>
        /// <param name="toBeMerged"></param>
        /// <returns></returns>
        public static TestProgressMessage GetMergedTestProgressMessage(TileStressTest tileStressTest, ICollection<TestProgressMessage> toBeMerged) {
            if (toBeMerged.Count == 1) foreach (var tpm in toBeMerged) return tpm;

            var stressTestMetricsCaches = new List<FastStressTestMetricsCache>(toBeMerged.Count);
            foreach (var tpm in toBeMerged) stressTestMetricsCaches.Add(tpm.StressTestMetricsCache);

            if (stressTestMetricsCaches.Contains(null)) {
                //Try to return the first with a cache that is not null, otherwise return the first tpm.
                foreach (var tpm in toBeMerged) if (tpm.StressTestMetricsCache != null) return tpm;
                foreach (var tpm in toBeMerged) return tpm;
            }

            //First merge the status, events and resource usage
            var testProgressMessage = new TestProgressMessage();

            testProgressMessage.StressTestStatus = StressTestStatus.Error;
            testProgressMessage.StartedAt = DateTime.MaxValue;
            testProgressMessage.Events = new List<EventPanelEvent>();
            foreach (var tpm in toBeMerged) {
                if (tpm.CPUUsage > testProgressMessage.CPUUsage) testProgressMessage.CPUUsage = tpm.CPUUsage;

                testProgressMessage.Events.AddRange(tpm.Events);

                if (!string.IsNullOrEmpty(tpm.Exception)) {
                    if (testProgressMessage.Exception == null) testProgressMessage.Exception = string.Empty;
                    testProgressMessage.Exception += tpm.Exception + "\n";
                }
                if (tpm.MemoryUsage > testProgressMessage.MemoryUsage) testProgressMessage.MemoryUsage = tpm.MemoryUsage;
                if (tpm.NicReceived > testProgressMessage.NicReceived) testProgressMessage.NicReceived = tpm.NicReceived;
                if (tpm.NicSent > testProgressMessage.NicSent) testProgressMessage.NicSent = tpm.NicSent;
                //if (tpm.RunStateChange > testProgressMessage.RunStateChange) testProgressMessage.RunStateChange = tpm.RunStateChange; //OKAY for run sync?
                if (tpm.StressTestStatus < testProgressMessage.StressTestStatus) testProgressMessage.StressTestStatus = tpm.StressTestStatus;
                if (tpm.StartedAt < testProgressMessage.StartedAt) testProgressMessage.StartedAt = tpm.StartedAt;
                if (tpm.MeasuredRuntime > testProgressMessage.MeasuredRuntime) testProgressMessage.MeasuredRuntime = tpm.MeasuredRuntime;
                if (tpm.EstimatedRuntimeLeft > testProgressMessage.EstimatedRuntimeLeft) testProgressMessage.EstimatedRuntimeLeft = tpm.EstimatedRuntimeLeft;
                testProgressMessage.ThreadsInUse += tpm.ThreadsInUse;
                testProgressMessage.TileStressTestIndex = tileStressTest.TileStressTestIndex;
                if (tpm.TotalVisibleMemory > testProgressMessage.TotalVisibleMemory) testProgressMessage.TotalVisibleMemory = tpm.TotalVisibleMemory;
            }
            //Then the test progress
            testProgressMessage.StressTestMetricsCache = FastStressTestMetricsHelper.MergeStressTestMetricsCaches(stressTestMetricsCaches);

            return testProgressMessage;
        }
    }
}
