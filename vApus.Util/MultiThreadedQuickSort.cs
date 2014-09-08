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
using System.Threading;

namespace vApus.Util {
    /// <summary>
    /// MultiThreaded 'local MapReduce' approach for quick sorting.
    /// </summary>
    public static class MultiThreadedQuickSort {
        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="threads">If less than 1, the number of logical cores will be chosen for this value. Fallbacks to the regular function if equals to 1.</param>
        /// <returns>A System.Linq.IOrderedEnumerable<TElement> whose elements are sorted according to a key.</returns>
        public static IEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source, int threads) where TSource : IComparable {
            if (threads == 1) return source.OrderBy(x => x);

            Bucket<TSource>[] buckets = GetBuckets<TSource>(source, threads);

            for (int i = 0; i != buckets.Length; i++)
                buckets[i].OrderBy();

            return Merge(AggregateBucketResultsAndDisposeBuckets(buckets));
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="threads">If less than 1, the number of logical cores will be chosen for this value. Fallbacks to the regular function if equals to 1.</param>
        /// <returns>A System.Linq.IOrderedEnumerable<TElement> whose elements are sorted according to a key.</returns>
        public static IEnumerable<TSource> OrderByDescending<TSource>(this IEnumerable<TSource> source, int threads) where TSource : IComparable {
            if (threads == 1) return source.OrderByDescending(x => x);

            Bucket<TSource>[] buckets = GetBuckets<TSource>(source, threads);

            for (int i = 0; i != buckets.Length; i++)
                buckets[i].OrderByDescending();

            return MergeDescending(AggregateBucketResultsAndDisposeBuckets(buckets));
        }

        private static List<TSource> Merge<TSource>(List<List<TSource>> sortedSources) where TSource : IComparable {
            while (sortedSources.Count() != 1) {
                sortedSources.Add(Merge(sortedSources[0], sortedSources[1]));
                sortedSources.RemoveAt(0);
                sortedSources.RemoveAt(1);
            }

            return sortedSources.First();
        }

        private static List<TSource> Merge<TSource>(List<TSource> a, List<TSource> b) where TSource : IComparable {
            var c = new List<TSource>(a.Count() + b.Count());
            while (a.Count() != 0 && b.Count() != 0) {
                TSource itemA = a.First();
                TSource itemB = b.First();

                if (itemA.CompareTo(itemB) < 0) { //Smaller than itemB
                    c.Add(itemA);
                    a.RemoveAt(0);
                } else {
                    c.Add(itemB);
                    b.RemoveAt(0);
                }
            }

            //A and / or b are empty, we can safely copy all items.
            c.AddRange(a);
            c.AddRange(b);

            return c;
        }

        private static List<TSource> MergeDescending<TSource>(List<List<TSource>> sortedSources) where TSource : IComparable {
            while (sortedSources.Count() != 1) {
                sortedSources.Add(MergeDescending(sortedSources[0], sortedSources[1]));
                sortedSources.RemoveAt(0);
                sortedSources.RemoveAt(0);
            }

            return sortedSources.First();
        }
        private static List<TSource> MergeDescending<TSource>(List<TSource> a, List<TSource> b) where TSource : IComparable {
            var c = new List<TSource>(a.Count() + b.Count());
            while (a.Count() != 0 && b.Count() != 0) {
                TSource itemA = a.First();
                TSource itemB = b.First();

                if (itemA.CompareTo(itemB) > 0) { //Bigger than itemB
                    c.Add(itemA);
                    a.RemoveAt(0);
                } else {
                    c.Add(itemB);
                    b.RemoveAt(0);
                }
            }

            //A and / or b are empty, we can safely copy all items.
            c.AddRange(a);
            c.AddRange(b);

            return c;
        }

        private static Bucket<TSource>[] GetBuckets<TSource>(IEnumerable<TSource> source, int threads) {
            if (threads < 1) threads = Environment.ProcessorCount;

            int size = source.Count();

            int bucketSize = size / threads;
            int remainder = size % threads;

            var buckets = new Bucket<TSource>[threads];

            int bucketIndex = 0;
            for (int from = 0; from != size; from += bucketSize) {
                int rangeSize = bucketSize;
                if (remainder != 0) {
                    ++rangeSize;
                    ++from;
                    --remainder;
                }
                buckets[bucketIndex] = new Bucket<TSource>(bucketIndex + 1, source.Skip(from).Take(rangeSize));
                ++bucketIndex;
            }

            return buckets;
        }
        private static List<List<TSource>> AggregateBucketResultsAndDisposeBuckets<TSource>(Bucket<TSource>[] buckets) {
            var aggregatedResults = new List<List<TSource>>();
            for (int i = 0; i != buckets.Length; i++) {
                Bucket<TSource> bucket = buckets[i];
                aggregatedResults.Add(bucket.GetResult().ToList());
                bucket.Dispose();
            }

            return aggregatedResults;
        }
        private class Bucket<TSource> : IDisposable {
            private readonly int _index;
            private IEnumerable<TSource> _source;
            private IOrderedEnumerable<TSource> _result;

            private Thread _thread;
            private AutoResetEvent _waitHandle;
            private Action _action;

            private bool _isDisposed;

            /// <summary></summary>
            /// <param name="index">An index to name the background thread.</param>
            /// <param name="source">The bucket source to sort.</param>
            public Bucket(int index, IEnumerable<TSource> source) {
                _index = index;
                _source = source;
                _waitHandle = new AutoResetEvent(false);

                _thread = new Thread(Func);
                _thread.Name = "MultiThreadedQuickSort Bucket #" + _index;
                _thread.IsBackground = true;
                _thread.Start();
            }
            /// <summary>
            /// Sorts in another thread.
            /// </summary>
            /// <param name="keySelector"></param>
            public void OrderBy() {
                _action = () => _result = _source.OrderBy(x => x);
                _waitHandle.Set();

            }
            /// <summary>
            /// Sorts in another thread.
            /// </summary>
            /// <param name="keySelector"></param>
            public void OrderByDescending() {
                _action = () => _result = _source.OrderByDescending(x => x);
                _waitHandle.Set();
            }

            private void Func() {
                _waitHandle.WaitOne();
                if (!_isDisposed)
                    _action.Invoke();
            }

            /// <summary>
            /// Waits for the thread to finish and returns the result.
            /// </summary>
            /// <returns></returns>
            public IOrderedEnumerable<TSource> GetResult() {
                if (_thread != null && _thread.IsAlive)
                    _thread.Join();
                return _result;
            }

            /// <summary></summary>
            public void Dispose() {
                if (!_isDisposed) {
                    _isDisposed = true;
                    if (_waitHandle != null) {
                        _waitHandle.Set();
                        _waitHandle.Dispose();
                        _waitHandle = null;
                    }
                    if (_thread != null && _thread.IsAlive) {
                        _thread.Abort();
                        _thread = null;
                    }

                    _source = null;
                    _result = null;
                }
            }
        }

    }
}
