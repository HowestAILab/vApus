/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.Threading;
using vApus.SolutionTree;

namespace vApus.Stresstest
{
    [Serializable]
    public class Parameters : BaseItem
    {
        private static readonly object _lock = new object();

        public Parameters()
        {
            AddAsDefaultItem(new CustomListParameters());
            AddAsDefaultItem(new NumericParameters());
            AddAsDefaultItem(new TextParameters());
            AddAsDefaultItem(new CustomRandomParameters());
        }

        /// <summary>
        ///     Threadsafe call.
        /// </summary>
        /// <returns></returns>
        public List<BaseParameter> GetAllParameters()
        {
            lock (_lock)
            {
                var l = new List<BaseParameter>();
                int failedTries = 0;
                Retry:
                try
                {
                    foreach (BaseItem item in this)
                        foreach (BaseParameter parameter in item)
                            l.Add(parameter);
                }
                catch
                {
                    //Handle if the collection changed.
                    if (++failedTries != 3)
                    {
                        Thread.Sleep(1000*failedTries);
                        goto Retry;
                    }
                }
                return l;
            }
        }

        /// <summary>
        ///     Needed for the token synchronization to the log and will visualize this in the gui.
        /// </summary>
        /// <param name="oldAndNewIndices">
        ///     The key and the value of the kvp are respectively the old and new index.
        ///     This is a reversed collection, the last occurs first so that the synchronization in the log entries is correct.
        ///     Otherwise, 3 can become 4 and after this 4 can become 5.
        /// </param>
        public void SynchronizeTokenNumericIdentifierToIndices(
            out Dictionary<BaseParameter, KeyValuePair<int, int>> oldAndNewIndices)
        {
            var l1 = new List<BaseParameter>();
            var l2 = new List<KeyValuePair<int, int>>();

            int i = 1;
            foreach (BaseItem item in this)
                foreach (BaseParameter parameter in item)
                {
                    if (parameter.TokenNumericIdentifier != i)
                    {
                        l1.Add(parameter);
                        l2.Add(new KeyValuePair<int, int>(parameter.TokenNumericIdentifier, i));
                        parameter.TokenNumericIdentifier = i;
                    }
                    ++i;
                }

            oldAndNewIndices = new Dictionary<BaseParameter, KeyValuePair<int, int>>(l1.Count);
            l1.Reverse();
            l2.Reverse();

            for (int j = 0; j != l1.Count; j++)
                oldAndNewIndices.Add(l1[j], l2[j]);

            if (oldAndNewIndices.Count != 0)
            {
                BaseSolutionComponentView view = SolutionComponentViewManager.Show(this,
                                                                                   typeof (ParameterTokenSynchronization
                                                                                       ));
                (view as ParameterTokenSynchronization).VisualizeSynchronization(oldAndNewIndices);
            }
        }
    }
}