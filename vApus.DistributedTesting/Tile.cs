/*
 * 2010 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Generic;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTest {
    /// <summary>
    ///     A tile of stress tests.
    /// </summary>
    public class Tile : LabeledBaseItem {

        #region Properties
        [SavableCloneable]
        public bool Use { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        ///     A tile of stress tests.
        /// </summary>
        public Tile() { ShowInGui = false; }
        #endregion

        #region Functions
        public Tile Clone() {
            var clone = new Tile();
            clone.Use = Use;
            foreach (TileStressTest ts in this)
                clone.AddWithoutInvokingEvent(ts.Clone());
            return clone;
        }
        #endregion
    }
}