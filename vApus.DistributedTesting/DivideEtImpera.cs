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
        public static Dictionary<TileStresstest, TileStresstest> DivideTileStresstestsOverSlaves(DistributedTest distributedTest, out bool addedOneConcurrencyToTheFirstCloneForATest) {
            addedOneConcurrencyToTheFirstCloneForATest = false;
            var dividedAndOriginalTileStresstests = new Dictionary<TileStresstest, TileStresstest>();
            foreach (TileStresstest tileStresstest in distributedTest.UsedTileStresstests) {
                bool b;
                foreach (var kvp in DivideTileStresstestOverSlaves(tileStresstest, out b))
                    dividedAndOriginalTileStresstests.Add(kvp.Key, kvp.Value);
               
                if (b)
                    addedOneConcurrencyToTheFirstCloneForATest = true;
            }

            return dividedAndOriginalTileStresstests;
        }
        /// <summary>
        /// Divide the load over the multiple slaves.
        /// </summary>
        /// <param name="tileStresstest">A tile stresstest that is 'Used'.</param>
        /// <returns></returns>
        private static Dictionary<TileStresstest, TileStresstest> DivideTileStresstestOverSlaves(TileStresstest tileStresstest, out bool addedOneConcurrencyToTheFirstClone) {
            addedOneConcurrencyToTheFirstClone = false;
            int count = tileStresstest.BasicTileStresstest.SlaveIndices.Length;
            var dividedTileStresstestsAndOriginal = new Dictionary<TileStresstest, TileStresstest>(count);
            if (count == 1) {
                dividedTileStresstestsAndOriginal.Add(tileStresstest, tileStresstest);
            } else if (count != 0) {
                var concurrencies = new int[tileStresstest.AdvancedTileStresstest.Concurrencies.Length];
                var addOneToFirstClone = new bool[concurrencies.Length]; //When numbers after the dot.
                for (int i = 0; i != concurrencies.Length; i++) {
                    int concurrency = tileStresstest.AdvancedTileStresstest.Concurrencies[i];
                    concurrencies[i] = concurrency / count;

                    addedOneConcurrencyToTheFirstClone = concurrency % count != 0;
                    addOneToFirstClone[i] = addedOneConcurrencyToTheFirstClone;

                    if (addedOneConcurrencyToTheFirstClone) 
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

                if (addOneToFirstClone.Contains(true)) {
                    var first = new List<int>(concurrencies);
                    for (int i = 0; i != addOneToFirstClone.Length; i++)
                        if (addOneToFirstClone[i])
                            ++first[i];
                    foreach (var clone in dividedTileStresstestsAndOriginal.Keys) {
                        clone.AdvancedTileStresstest.Concurrencies = first.ToArray();
                        break;
                    }
                }
            }
            return dividedTileStresstestsAndOriginal;
        }
    }
}
