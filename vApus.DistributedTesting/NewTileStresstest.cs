using System;
using vApus.SolutionTree;

namespace vApus.DistributedTesting
{
    public class TileStresstest : LabeledBaseItem
    {
        private Stresstest.Stresstest _defaultTo;
        private bool _use = true;

        [SavableCloneable, PropertyControl]
        public Stresstest.Stresstest DefaultTo
        {
            get { return _defaultTo; }
            set
            {
                value.ParentIsNull -= _defaultTo_ParentIsNull;
                _defaultTo = value;
                _defaultTo.ParentIsNull += new EventHandler(_defaultTo_ParentIsNull);
            }
        }
        [SavableCloneable]
        public bool Use
        {
            get { return _use; }
            set { _use = value; }
        }

        public BasicTileStresstest BasicTileStresstest
        {
            get { return this[0] as BasicTileStresstest; }
        }
        public AdvancedTileStresstest AdvancedTileStresstest
        {
            get { return this[1] as AdvancedTileStresstest; }
        }

        public TileStresstest()
        {
            ShowInGui = false;
            AddAsDefaultItem(new BasicTileStresstest());
            AddAsDefaultItem(new AdvancedTileStresstest());

            if (Solution.ActiveSolution != null)
                DefaultTo = SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Stresstest), Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.StresstestProject))) as Stresstest.Stresstest;
            else
                Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);
        }

        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            DefaultTo = SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Stresstest), Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.StresstestProject))) as Stresstest.Stresstest;
        }
        private void _defaultTo_ParentIsNull(object sender, EventArgs e)
        {
            DefaultTo = SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Stresstest), Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.StresstestProject))) as Stresstest.Stresstest;
        }
    }
}
