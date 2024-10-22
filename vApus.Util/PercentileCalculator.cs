﻿/*
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
    public static class PercentileCalculator<T> {
        /// <summary>
        /// Gets a percentile for a full population of values. 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="nthPercentile">Ranging from one to 99.</param>
        /// <returns></returns>
        public static T Get(IEnumerable<T> values, int nthPercentile) {
            if (nthPercentile < 1 || nthPercentile > 99)
                throw new ArgumentOutOfRangeException("percentile must be between 0 and 100.");

            double dNthPercentile = (100.0 - (double)nthPercentile) / 100.0;
            int percentN = (int)(Math.Round(values.Count() * dNthPercentile, MidpointRounding.AwayFromZero)) + 1;

            T[] orderedUpperValues = values.OrderByDescending(x => x).Take(percentN).ToArray();
            return orderedUpperValues[orderedUpperValues.Length - 1];
        }
        /// <summary>
        /// Gets a percentile for a full population of values. 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="nthPercentile">Ranging from one to 99.</param>
        /// <param name="orderedValues">Values ordered by descending.</param>
        /// <returns></returns>
        public static T Get(IEnumerable<T> values, int nthPercentile, out IEnumerable<T> orderedValues) {
            if (nthPercentile < 1 || nthPercentile > 99)
                throw new ArgumentOutOfRangeException("percentile must be between 0 and 100.");

            orderedValues = values.OrderByDescending(x => x).AsEnumerable<T>();

            return Get(orderedValues, nthPercentile);
        }
    }
}
