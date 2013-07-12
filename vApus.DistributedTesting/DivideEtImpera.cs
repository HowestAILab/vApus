/*
 * Copyright 2013 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vApus.Util;

namespace vApus.DistributedTesting {
    public static class DivideEtImpera {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="distributedTest"></param>
        /// <returns>key = Divided, value = original</returns>
        public static Dictionary<TileStresstest, TileStresstest> DivideTileStresstestsOverSlaves(DistributedTest distributedTest, out bool notACleanDivision) {
            notACleanDivision = false;
            var dividedAndOriginalTileStresstests = new Dictionary<TileStresstest, TileStresstest>();
            foreach (TileStresstest tileStresstest in distributedTest.UsedTileStresstests) {
                bool b;
                foreach (var kvp in DivideTileStresstestOverSlaves(tileStresstest, out b))
                    dividedAndOriginalTileStresstests.Add(kvp.Key, kvp.Value);

                if (b)
                    notACleanDivision = true;
            }

            return dividedAndOriginalTileStresstests;
        }
        /// <summary>
        /// Divide the load over the multiple slaves.
        /// </summary>
        /// <param name="tileStresstest">A tile stresstest that is 'Used'.</param>
        /// <returns></returns>
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
                    notACleanDivision = mod != 0;

                    if (notACleanDivision) {
                        addOnesPerConcurrency.Add(DetermineModConcurrencyOverSlaves(mod, slaves));
                        LogWrapper.LogByLevel(tileStresstest.ToString() +
                            " The averages in the fast results will NOT be correct because one or more given concurrencies divided by the number of slaves is not an integer! Please use the detailed results." +
                            "\nIn the following example both outcomes should be the same, but that is not possible:\n\t3 concurrencies; 1 slave; a log of one entry.\n\tAvg.Response time: (10 + 7 + 9) / 3 = 26 / 3 = 8,67." +
                            "\n\t---\n\t3 concurrencies; 2 slaves; a log of one entry.\n\tAvg.Response time: (10 + (7 + 9) / 2) / 2 = 18 / 2 = 9.", LogLevel.Warning);
                    } else {
                        addOnesPerConcurrency.Add(new bool[slaves]);
                    }
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
        /// 
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="slaves"></param>
        /// <returns>Returns a bool at each slave index if there must be added one to the divided concurrency for a slave.</returns>
        private static bool[] DetermineModConcurrencyOverSlaves(int mod, int slaves) {
            bool[] addOne = new bool[slaves];

            for (int i = 0; i != mod; i++)
                addOne[i] = true;

            return addOne;
        }
    }
}
