/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.StressTest {
    /// <summary>
    /// Holds collections of parameters.
    /// </summary>
    [Serializable]
    public class Parameters : BaseItem, ISerializable {

        #region Fields
        private readonly object _lock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Holds collections of parameters.
        /// </summary>
        public Parameters() {
            AddAsDefaultItem(new CustomListParameters());
            AddAsDefaultItem(new NumericParameters());
            AddAsDefaultItem(new TextParameters());
            AddAsDefaultItem(new CustomRandomParameters());

            Loaded += Parameters_Loaded;
        }
        public Parameters(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                AddRangeWithoutInvokingEvent(sr.ReadCollection<BaseItem>(new List<BaseItem>()));
            }
            sr = null;
        }
        #endregion

        #region Functions
        private void Parameters_Loaded(object sender, EventArgs e) {
            Loaded -= Parameters_Loaded;

            //Make sure this value is set, otherwise synchronizing parameter indices will not work the first time.
            int i = 1;
            foreach (BaseItem item in this)
                foreach (BaseParameter parameter in item)
                    parameter.TokenNumericIdentifier = i++;
        }

        /// <summary>
        ///     Threadsafe call. Used in RedetermineTokens, BaseParameter and ASTNode.
        /// </summary>
        /// <returns></returns>
        public List<BaseParameter> GetAllParameters() {
            lock (_lock) {
                var l = new List<BaseParameter>();
                int failedTries = 0;
            Retry:
                try {
                    foreach (BaseItem item in this)
                        foreach (BaseParameter parameter in item)
                            l.Add(parameter);
                } catch {
                    //Handle if the collection changed.
                    if (++failedTries != 3) {
                        Thread.Sleep(1000 * failedTries);
                        goto Retry;
                    }
                }
                return l;
            }
        }

        /// <summary>
        ///     Needed for the token synchronization to the scenario and will visualize this in the gui.
        /// </summary>
        /// <param name="oldAndNewIndices">
        ///     The key and the value of the kvp are respectively the old and new index.
        ///     This is a reversed collection, the last occurs first so that the synchronization in the requests is correct.
        ///     Otherwise, 3 can become 4 and after this 4 can become 5.
        /// </param>
        public void SynchronizeTokenNumericIdentifierToIndices(out Dictionary<BaseParameter, KeyValuePair<int, int>> oldAndNewIndices) {
            var l1 = new List<BaseParameter>();
            var l2 = new List<KeyValuePair<int, int>>();

            int i = 1;
            foreach (BaseItem item in this)
                foreach (BaseParameter parameter in item) {
                    if (parameter.TokenNumericIdentifier != i) {
                        l1.Add(parameter);
                        l2.Add(new KeyValuePair<int, int>(parameter.TokenNumericIdentifier, i));
                        parameter.TokenNumericIdentifier = i;
                    }
                    ++i;
                }

            oldAndNewIndices = new Dictionary<BaseParameter, KeyValuePair<int, int>>(l1.Count);
            l1.Reverse();
            l2.Reverse();

            for (int j = 0; j != l1.Count; j++) oldAndNewIndices.Add(l1[j], l2[j]);

            //For debugging purposes only.
            //if (oldAndNewIndices.Count != 0)
            //    (SolutionComponentViewManager.Show(this, typeof(ParameterTokenSynchronizationView)) as ParameterTokenSynchronizationView).VisualizeSynchronization(oldAndNewIndices);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(this);
                sw.AddToInfo(info);
            }
            sw = null;
        }
        #endregion
    }
}