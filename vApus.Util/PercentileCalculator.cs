/*
 * Copyright 2014 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
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
        /// 
        /// </summary>
        /// <param name="nthPercentile">Ranging from one to 99</param>
        /// <returns></returns>
        public static T Get(IEnumerable<T> values, int nthPercentile) {
            if (nthPercentile < 1 || nthPercentile > 99)
                throw new ArgumentOutOfRangeException("percentile must be between 0 and 100.");

            double dNthPercentile = (100.0 - (double)nthPercentile) / 100.0;
            int percentN = (int)(Math.Round(values.Count() * dNthPercentile, MidpointRounding.AwayFromZero)) + 1;

            T[] orderedUpperValues = values.OrderByDescending(x => x).Take(percentN).ToArray();
            return orderedUpperValues[orderedUpperValues.Length - 1];
        }
    }
}
