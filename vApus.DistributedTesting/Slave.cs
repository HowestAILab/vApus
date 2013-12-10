/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Generic;
using System.Linq;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.DistributedTesting {
    /// <summary>
    /// Holds information about the slave and client (via properties) to be able to let a stresstest happen there.
    /// </summary>
    public class Slave : BaseItem {

        #region Fields
        private int _port = 1347;
        private int[] _processorAffinity = { };
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
        ///     Use AssigneTileStresstest to set.
        /// </summary>
        public TileStresstest TileStresstest {
            get {
                try {
                    foreach (TileStresstest ts in TileStesstests)
                        if (ts.BasicTileStresstest.Slaves.Contains(this))
                            return ts;
                } catch {
                }
                return null;
            }
        }

        private IEnumerable<TileStresstest> TileStesstests {
            get {
                if (Parent != null &&
                    Parent.GetParent() != null &&
                    Parent.GetParent().GetParent() != null) {
                    var dt = Parent.GetParent().GetParent() as DistributedTest;
                    foreach (Tile t in dt.Tiles)
                        foreach (TileStresstest ts in t)
                            yield return ts;
                }
            }
        }

        /// <summary>
        ///     One-based.
        /// </summary>
        [SavableCloneable]
        public int[] ProcessorAffinity {
            get { return _processorAffinity; }
            set { _processorAffinity = new int[] { }; }// value; }
        }

        #endregion

        #region Constructor
        public Slave() { ShowInGui = false; }
        #endregion

        #region Functions
        /// <summary>
        ///     Clears the stresstest if it is null.
        /// </summary>
        /// <param name="tileStresstest"></param>
        public void AssignTileStresstest(TileStresstest tileStresstest) {
            ClearTileStresstest();

            if (tileStresstest != null) {
                var slaves = new List<Slave>(tileStresstest.BasicTileStresstest.Slaves);

                slaves.Add(this);
                tileStresstest.BasicTileStresstest.Slaves = slaves.ToArray();
            }
        }

        private void ClearTileStresstest() {
            //Remove it from the tile stresstests (can be used only once atm).
            foreach (TileStresstest ts in TileStesstests)
                if (ts.BasicTileStresstest.Slaves.Contains(this)) {
                    var sl = new List<Slave>(ts.BasicTileStresstest.Slaves);
                    sl.Remove(this);
                    ts.BasicTileStresstest.Slaves = sl.ToArray();
                }
        }

        public Slave Clone() {
            var clone = new Slave();
            clone.Port = _port;

            clone.ProcessorAffinity = new int[_processorAffinity.Length];
            for (int i = 0; i != clone.ProcessorAffinity.Length; i++)
                clone.ProcessorAffinity[i] = _processorAffinity[i];

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