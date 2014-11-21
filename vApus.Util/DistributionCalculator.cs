/*
 * Copyright 2014 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace vApus.Util {
    public static class DistributionCalculator<T> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="population"></param>
        /// <returns>Key = entry, value = count</returns>
        public static ConcurrentDictionary<T, long> GetEntriesAndCounts(IEnumerable<T> population) {
            var entriesAndCounts = new ConcurrentDictionary<T, long>();
            Parallel.ForEach(population, entry => {
                entriesAndCounts.TryAdd(entry, 0);
                ++entriesAndCounts[entry];
            });
            entriesAndCounts.OrderBy(x => x.Key);
            return entriesAndCounts;
        }
    }
}
