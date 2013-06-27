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

namespace vApus.DistributedTesting {
    public static class DivideEtImpera {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="distributedTest"></param>
        /// <returns>key = Divided, value = original</returns>
        public static Dictionary<TileStresstest, TileStresstest> DivideTileStresstestsOverSlaves(DistributedTest distributedTest) {
            var dividedAndOriginalTileStresstests = new Dictionary<TileStresstest, TileStresstest>();
            foreach (TileStresstest tileStresstest in distributedTest.UsedTileStresstests)
                foreach (var kvp in DivideTileStresstestOverSlaves(tileStresstest))
                    dividedAndOriginalTileStresstests.Add(kvp.Key, kvp.Value);

            return dividedAndOriginalTileStresstests;
        }
        /// <summary>
        /// Divide the load over the multiple slaves.
        /// </summary>
        /// <param name="tileStresstest">A tile stresstest that is 'Used'.</param>
        /// <returns></returns>
        private static Dictionary<TileStresstest, TileStresstest> DivideTileStresstestOverSlaves(TileStresstest tileStresstest) {
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
                    addOneToFirstClone[i] = concurrency % count != 0;
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
