/*
 * 2014 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace vApus.Util {
    public static class DistributionCalculator<T> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="population"></param>
        /// <returns>Key = entry, value = count</returns>
        public static Dictionary<T, long> GetEntriesAndCounts(IEnumerable<T> population) {
            var entriesAndCounts = new Dictionary<T, long>();
            foreach (T entry in population) {
                if (!entriesAndCounts.ContainsKey(entry)) entriesAndCounts.Add(entry, 0L);
                ++entriesAndCounts[entry];
            }

            return entriesAndCounts.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Adds zero count at ranges to even out the data. This gives us a nicer chart.
        /// </summary>
        /// <param name="distribution"></param>
        /// <param name="rangeLength"></param>
        /// <returns></returns>
        public static Dictionary<double, long> EvenOutRanges(Dictionary<double, long> distribution, double rangeLength) {
            if (distribution.Count == 0) return distribution;

            var newDistribution = new Dictionary<double, long>();

            double last = distribution.Last().Key + rangeLength;
            for (double range = 0d; range < last; range += rangeLength)
                newDistribution.Add(Math.Round(range, 1, MidpointRounding.AwayFromZero), 0L); //Must do math round or we get irregularities (5.9999 instead of 6.0 for example). Strange.

            foreach (var kvp in distribution)
                newDistribution[kvp.Key] = kvp.Value;

            return newDistribution;
        }
    }
}
