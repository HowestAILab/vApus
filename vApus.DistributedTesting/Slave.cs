/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using vApus.SolutionTree;
using vApus.Util;
using System.Collections.Generic;
using System.Linq;
namespace vApus.DistributedTesting
{
    public class Slave : BaseItem
    {
        #region Fields
        private int _port = 1337;
        private int[] _processorAffinity = { };
        #endregion

        #region Properties
        [SavableCloneable]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Get the ip from the client.
        /// </summary>
        public string IP
        {
            get
            {

                if (Parent == null)
                    return null;
                return (Parent as Client).IP;
            }
        }
        /// <summary>
        /// Search the slaves for this.
        /// 
        /// Use AssigneTileStresstest to set.
        /// </summary>
        public TileStresstest TileStresstest
        {
            get
            {
                try
                {
                    foreach (TileStresstest ts in TileStesstests)
                        if (ts.BasicTileStresstest.Slaves.Contains(this))
                            return ts;
                }
                catch { }
                return null;
            }
        }

        private IEnumerable<TileStresstest> TileStesstests
        {
            get
            {
                if (this.Parent != null &&
                    this.Parent.GetParent() != null &&
                    this.Parent.GetParent().GetParent() != null)
                {
                    var dt = this.Parent.GetParent().GetParent() as DistributedTest;
                    foreach (Tile t in dt.Tiles)
                        foreach (TileStresstest ts in t)
                            yield return ts;
                }
            }
        }

        /// <summary>
        /// One-based.
        /// </summary>
        [SavableCloneable]
        public int[] ProcessorAffinity
        {
            get { return _processorAffinity; }
            set { _processorAffinity = value; }
        }
        #endregion

        #region Constructor
        public Slave()
        {
            ShowInGui = false;
            if (Solution.ActiveSolution != null)
                Init();
            else
                Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);

        }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            Init();
        }
        private void Init()
        {

        }

        /// <summary>
        /// Clears the stresstest if it is null.
        /// </summary>
        /// <param name="tileStresstest"></param>
        public void AssignTileStresstest(TileStresstest tileStresstest)
        {
            ClearTileStresstest();

            if (tileStresstest != null)
            {
                var slaves = new List<Slave>(tileStresstest.BasicTileStresstest.Slaves);

                slaves.Add(this);
                tileStresstest.BasicTileStresstest.Slaves = slaves.ToArray();
            }
        }
        private void ClearTileStresstest()
        {
            //Remove it from the tile stresstests (can be used only once atm).
            foreach (TileStresstest ts in TileStesstests)
                if (ts.BasicTileStresstest.Slaves.Contains(this))
                {
                    var sl = new List<Slave>(ts.BasicTileStresstest.Slaves);
                    sl.Remove(this);
                    ts.BasicTileStresstest.Slaves = sl.ToArray();
                }
        }
        public Slave Clone()
        {
            Slave clone = new Slave();
            clone.Port = _port;

            clone.ProcessorAffinity = new int[_processorAffinity.Length];
            for (int i = 0; i != clone.ProcessorAffinity.Length; i++)
                clone.ProcessorAffinity[i] = _processorAffinity[i];

            return clone;
        }
        public override string ToString()
        {
            object parent = Parent;
            if (parent != null)
                return parent.ToString() + " - " + _port;

            return base.ToString();
        }
        #endregion

        public class SlaveComparer : IComparer<Slave>
        {
            private static SlaveComparer _labeledBaseItemComparer = new SlaveComparer();
            private SlaveComparer()
            { }
            public static SlaveComparer GetInstance()
            {
                return _labeledBaseItemComparer;
            }
            public int Compare(Slave x, Slave y)
            {
                return x.Port.CompareTo(y.Port);
            }
        }
    }
}
