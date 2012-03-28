/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Generic;
using System.ComponentModel;
using vApus.SolutionTree;
using vApus.Stresstest;

namespace vApus.DistributedTesting
{
    /// <summary>
    /// A tile of stresstests.
    /// </summary>
    public class Tile : LabeledBaseItem
    {
        #region Fields
        private bool _use = false;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl(0)]
        public new string Label
        {
            get { return base.Label; }
            set { base.Label = value; }
        }
        
        [SavableCloneable, PropertyControl(1)]
        [DisplayName("Use this Tile")]
        public bool Use
        {
            get { return _use; }
            set { _use = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// A tile of stresstests.
        /// </summary>
        public Tile()
        {
            ShowInGui = false;
            if (Solution.ActiveSolution != null)
                InitTileStresstests();
        }
        #endregion

        #region Functions
        private void InitTileStresstests()
        {
            if (this.Count == 0)
            {
                SolutionComponent stresstestProject = Solution.ActiveSolution.GetSolutionComponent(typeof(StresstestProject));
                foreach (BaseItem item in stresstestProject)
                    if (item is Stresstest.Stresstest)
                    {
                        TileStresstest tileStresstest = new TileStresstest(item as Stresstest.Stresstest);
                        this.AddWithoutInvokingEvent(tileStresstest);
                    }
            }
        }
        /// <summary>
        /// Synchronize the tile stresstests with the real ones, meaning if one is deleted or added the number of tile stresstests must equal those of the real. 
        /// </summary>
        /// <returns>True when it did synchronize, false when this was not necessary.</returns>
        public bool SynchronizeTileStresstests()
        {
            bool didSynchronize = false;
            SolutionComponent stresstestProject = Solution.ActiveSolution.GetSolutionComponent(typeof(StresstestProject));
            List<Stresstest.Stresstest> stresstests = new List<Stresstest.Stresstest>(), synchedStresstests = new List<Stresstest.Stresstest>();
            foreach (BaseItem item in stresstestProject)
                if (item is Stresstest.Stresstest)
                    stresstests.Add(item as Stresstest.Stresstest);
            TileStresstest[] inOrder = new TileStresstest[stresstests.Count];

            //Put the tile stresstests in order and check the synchronization.
            for (int i = 0; i < this.Count; i++)
            {
                TileStresstest tileStresstest = this[i] as TileStresstest;
                for (int j = 0; j < stresstests.Count; j++)
                {
                    Stresstest.Stresstest stresstest = stresstests[j];
                    if (tileStresstest.DefaultStresstest == stresstest)
                    {
                        inOrder[j] = tileStresstest;
                        //Store for removal from stresstests later on.
                        synchedStresstests.Add(stresstest);
                        if (i != j)
                            didSynchronize = true;
                        break;
                    }
                }
            }
            if (didSynchronize || this.Count != stresstests.Count)
            {

                didSynchronize = true;
                foreach (Stresstest.Stresstest stresstest in synchedStresstests)
                    stresstests.Remove(stresstest);
                this.ClearWithoutInvokingEvent();
                for (int i = 0; i < inOrder.Length; i++)
                    if (inOrder[i] == null)
                    {
                        this.AddAsDefaultItem(new TileStresstest(stresstests[0]));
                        stresstests.RemoveAt(0);
                    }
                    else
                    {
                        this.AddAsDefaultItem(inOrder[i]);
                    }
            }
            return didSynchronize;
        }
        #endregion
    }
}
