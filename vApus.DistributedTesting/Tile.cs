/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Generic;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTesting {
    /// <summary>
    ///     A tile of stresstests.
    /// </summary>
    public class Tile : LabeledBaseItem {

        #region Properties
        [SavableCloneable]
        public bool Use { get; set; }

        [SavableCloneable]
        public bool UseOverride { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        ///     A tile of stresstests.
        /// </summary>
        public Tile() { ShowInGui = false; }
        #endregion

        #region Functions
        public Tile Clone() {
            var clone = new Tile();
            clone.Use = Use;
            foreach (TileStresstest ts in this)
                clone.AddWithoutInvokingEvent(ts.Clone());
            return clone;
        }
        #endregion
    }
}