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
namespace vApus.DistributedTesting
{
    public class Slave : BaseItem
    {
        #region Fields
        private bool _use = true;
        private int _port = 1337;

        private TileStresstest _tileStresstest;
        private int[] _processorAffinity = { };
        #endregion

        #region Properties
        [SavableCloneable]
        public bool Use
        {
            get { return _use; }
            set { _use = value; }
        }
        [SavableCloneable]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        // [SavableCloneable]
        public TileStresstest TileStresstest
        {
            get { return _tileStresstest; }
            set
            {
                _tileStresstest.ParentIsNull -= _tileStresstest_ParentIsNull;
                _tileStresstest = value;
                _tileStresstest.ParentIsNull += _tileStresstest_ParentIsNull;
            }
        }
        private Tile TileStesstestParent
        {
            get
            {
                try
                {
                    if (this.Parent != null &&
                        this.Parent.GetParent() != null &&
                        this.Parent.GetParent().GetParent() != null &&
                        this.Parent.GetParent().GetParent().GetParent() != null)
                    {
                        var dt = this.Parent.GetParent().GetParent().GetParent() as DistributedTest;
                        if (dt.Tiles.Count != 0)
                            return dt.Tiles[0] as Tile;
                    }
                }
                catch { }
                return null;
            }
        }


        private void _tileStresstest_ParentIsNull(object sender, System.EventArgs e)
        {
            Tile t = TileStesstestParent;
            if (t != null)
                TileStresstest = BaseItem.GetNextOrEmptyChild(typeof(TileStresstest), t) as TileStresstest;
        }
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
        public Slave Clone()
        {
            Slave clone = new Slave();
            clone.Use = _use;
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
