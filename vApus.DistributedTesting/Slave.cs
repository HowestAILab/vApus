/*
 * 2012 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Generic;
using System.Linq;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTest {
    /// <summary>
    /// Holds information about the slave and client (via properties) to be able to let a stress test happen there.
    /// </summary>
    public class Slave : BaseItem {

        #region Fields
        private int _port = 1347;
        #endregion

        #region Properties
        [SavableCloneable]
        public int Port {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        ///     Get the ip from the client.
        /// </summary>
        public string IP {
            get {
                if (Parent == null) return null;
                return (Parent as Client).IP;
            }
        }

        /// <summary>
        ///     Get the hostname from the client.
        /// </summary>
        public string HostName {
            get {
                if (Parent == null) return null;
                return (Parent as Client).HostName;
            }
        }

        /// <summary>
        ///     Search the slaves for this.
        ///     Use AssigneTileStressTest to set.
        /// </summary>
        public TileStressTest TileStressTest {
            get {
                try {
                    foreach (TileStressTest ts in TileStessTests)
                        if (ts.BasicTileStressTest.Slaves.Contains(this))
                            return ts;
                } catch {
                    //Handled later on.
                }
                return null;
            }
        }

        private IEnumerable<TileStressTest> TileStessTests {
            get {
                if (Parent != null &&
                    Parent.GetParent() != null &&
                    Parent.GetParent().GetParent() != null) {
                    var dt = Parent.GetParent().GetParent() as DistributedTest;
                    foreach (Tile t in dt.Tiles)
                        foreach (TileStressTest ts in t)
                            yield return ts;
                }
            }
        }

        #endregion

        #region Constructor
        public Slave() { ShowInGui = false; }
        #endregion

        #region Functions
        /// <summary>
        ///     Clears the stress test if it is null.
        /// </summary>
        /// <param name="tileStressTest"></param>
        public void AssignTileStressTest(TileStressTest tileStressTest) {
            ClearTileStressTest();

            if (tileStressTest != null)
                tileStressTest.BasicTileStressTest.Slaves = new Slave[] { this };
        }

        private void ClearTileStressTest() {
            //Remove it from the tile stress tests (can be used only once atm).
            foreach (TileStressTest ts in TileStessTests)
                if (ts.BasicTileStressTest.Slaves.Contains(this)) {
                    var sl = new List<Slave>(ts.BasicTileStressTest.Slaves);
                    sl.Remove(this);
                    ts.BasicTileStressTest.Slaves = sl.ToArray();
                }
        }

        public Slave Clone() {
            var clone = new Slave();
            clone.Port = _port;

            return clone;
        }

        public override string ToString() {
            object parent = Parent;
            if (parent != null)
                return parent + " - " + _port;

            return base.ToString();
        }
        #endregion

        /// <summary>
        /// Compares the ports.
        /// </summary>
        public class SlaveComparer : IComparer<Slave> {
            private static readonly SlaveComparer _labeledBaseItemComparer = new SlaveComparer();

            private SlaveComparer() { }

            /// <summary>
            /// Compares the ports.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(Slave x, Slave y) { return x.Port.CompareTo(y.Port); }

            /// <summary>
            /// Compares the ports.
            /// </summary>
            /// <returns></returns>
            public static SlaveComparer GetInstance() { return _labeledBaseItemComparer; }
        }
    }
}