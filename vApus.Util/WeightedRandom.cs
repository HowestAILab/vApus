/*
 * Copyright (C) 2008 Dieter Vandroemme  <dieter.vandroemme@gmail.com>
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 *
 * This file is available with multiple licenses, with the given permission of the original author, Dieter Vandroemme.
 * The user is free to choose the license he/she wants to use, as long as all the above copyright messages
 * and this disclaimer are included.
 */

/*
 * Copyright 2007-2008 (c) Kets Brecht, Vandroemme Dieter
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */

using System;
using System.Collections.Generic;

namespace vApus.Util
{
    /// <summary>
    /// Used to randomly pick an index with a certain probability of being chosen (weight).
    /// </summary>
    public class WeightedRandom
    {
        #region Fields
        private Random _r;
        private HashSet<int> _usedKeys;
        private int _max;
        private int _weightCount;
        private Dictionary<int, int> _weightedIndices;
        #endregion

        #region Properties
        /// <summary>
        /// Redetermine the seed of the random.
        /// </summary>
        public int Seed
        {
            set { _r = new Random(value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Used to randomly pick an index with a certain probability of being chosen (weight).
        /// </summary>
        public WeightedRandom()
        {
            _r = new Random();
            _weightedIndices = new Dictionary<int, int>();
            _usedKeys = new HashSet<int>();
        }
        /// <summary>
        /// Used to randomly pick an index with a certain probability of being chosen (weight).
        /// </summary>
        /// <param name="seed">The seed to determine the randomization pattern.</param>
        public WeightedRandom(int seed)
            : this()
        {
            _r = new Random(seed);
        }
        /// <summary>
        /// Used to randomly pick an index with a certain probability of being chosen (weight).
        /// </summary>
        /// <param name="weight">A weight is linked to an index. So if the value at index 0 is 30 and the total of weights equals 100, there will be 30% chance this index will be chosen using the 'Next ()' function.</param>
        /// <param name="precision">How precise the randomization will be, for example: at precision 10 an index with value 33,33 has 33,3% chance to be chosen when the total of weights equals 100.</param>
        public WeightedRandom(List<double> weights, int precision)
            : this()
        {
            if (weights == null)
                throw new ArgumentNullException("weights");
            if (weights.Count == 0)
                throw new ArgumentException("weights");
            if (precision < 1)
                throw new ArgumentOutOfRangeException("precision");

            foreach (double weight in weights)
                AddWeight(weight, precision);
        }
        /// <summary>
        /// Used to randomly pick an index (of weights) with a probability of being chosen (weight).
        /// </summary>
        /// <param name="weight">A weight is linked to an index. So if the value at index 0 is 30 and the total of weights equals 100, there will be 30% chance this index will be chosen using the 'Next ()' function.</param>
        /// <param name="precision">How precise the randomization will be, for example: at precision 10 an index with value 33,33 has 33,3% chance to be chosen when the total of weights equals 100.</param>
        /// <param name="seed">The seed to determine the randomization pattern.</param>
        public WeightedRandom(List<double> weights, int precision, int seed)
            : this(weights, precision)
        {
            _r = new Random(seed);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Adds a weight.
        /// </summary>
        /// <param name="weight">A weight is linked to an index. So if the value at index 0 is 30 and the total of weights equals 100, there will be 30% chance this index will be chosen using the 'Next ()' function.</param>
        /// <param name="precision">How precise the randomization will be, for example: at precision 10 an index with value 33,33 has 33,3% chance to be chosen when the total of weights equals 100.</param>
        public void AddWeight(double weight, int precision)
        {
            if (weight < 0)
                throw new ArgumentOutOfRangeException("weight");
            if (precision < 1)
                throw new ArgumentOutOfRangeException("precision");
            //The max value of the Random.Next ().
            _max += (int)Math.Round(weight * precision);
            //For example: index 0 with weight 3: { [0,0],[1,0],[2,0] }.
            for (int j = _weightedIndices.Count; j < _max; j++)
                _weightedIndices.Add(j, _weightCount);

            //The index linked to the weight.
            ++_weightCount;
        }
        /// <summary>
        /// Returns the next weighted index.
        /// </summary>
        /// <param name="deepRandomize">If true, a value at a certain index will be returned only once. This is reseted if all possible values have been fetched.</param>
        /// <returns></returns>
        public int Next(bool deepRandomize)
        {
            int key = _r.Next(_max);

            if (deepRandomize)
            {
                while (!_usedKeys.Add(key))
                    key = _r.Next(_max);

                if (_usedKeys.Count == _weightedIndices.Count)
                    ResetUsedKeys();
            }
            return _weightedIndices[key];
        }
        /// <summary>
        /// Resets the used keys for deep randomization (Next(deepRandomize)).
        /// </summary>
        public void ResetUsedKeys()
        {
            _usedKeys.Clear();
        }
        #endregion
    }
}
