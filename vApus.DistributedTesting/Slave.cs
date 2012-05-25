/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using vApus.SolutionTree;

namespace vApus.DistributedTesting
{
    public class Slave : LabeledBaseItem
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
        [SavableCloneable]
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

        private void _tileStresstest_ParentIsNull(object sender, System.EventArgs e)
        {
         //  TileStresstest = BaseItem.GetNextOrEmptyChild(typeof(TileStresstest)
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
        public override string ToString()
        {
            object parent = Parent;
            if (parent != null)
                return parent.ToString() + " - " + _port;

            return base.ToString();
        }
        #endregion
    }
}
