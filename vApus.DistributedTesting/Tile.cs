/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using vApus.SolutionTree;

namespace vApus.DistributedTesting {
    /// <summary>
    ///     A tile of stresstests.
    /// </summary>
    public class Tile : LabeledBaseItem {
        #region Fields

        private bool _use;

        #endregion

        #region Properties

        [SavableCloneable]
        public bool Use {
            get { return _use; }
            set { _use = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        ///     A tile of stresstests.
        /// </summary>
        public Tile() {
            ShowInGui = false;
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Create a clone of this.
        /// </summary>
        /// <returns></returns>
        public Tile Clone() {
            var clone = new Tile();
            clone.Use = _use;
            foreach (TileStresstest ts in this)
                clone.AddWithoutInvokingEvent(ts.Clone());
            return clone;
        }

        ///// <summary>
        ///// Synchronize the tile stresstests with the real ones, meaning if one is deleted or added the number of tile stresstests must equal those of the real. 
        ///// </summary>
        ///// <returns>True when it did synchronize, false when this was not necessary.</returns>
        //public bool SynchronizeTileStresstests()
        //{
        //    bool didSynchronize = false;
        //    SolutionComponent stresstestProject = Solution.ActiveSolution.GetSolutionComponent(typeof(StresstestProject));
        //    List<Stresstest.Stresstest> stresstests = new List<Stresstest.Stresstest>(), synchedStresstests = new List<Stresstest.Stresstest>();
        //    foreach (BaseItem item in stresstestProject)
        //        if (item is Stresstest.Stresstest)
        //            stresstests.Add(item as Stresstest.Stresstest);
        //    OldTileStresstest[] inOrder = new OldTileStresstest[stresstests.Count];

        //    //Put the tile stresstests in order and check the synchronization.
        //    for (int i = 0; i < this.Count; i++)
        //    {
        //        OldTileStresstest tileStresstest = this[i] as OldTileStresstest;
        //        for (int j = 0; j < stresstests.Count; j++)
        //        {
        //            Stresstest.Stresstest stresstest = stresstests[j];
        //            if (tileStresstest.DefaultStresstest == stresstest)
        //            {
        //                inOrder[j] = tileStresstest;
        //                //Store for removal from stresstests later on.
        //                synchedStresstests.Add(stresstest);
        //                if (i != j)
        //                    didSynchronize = true;
        //                break;
        //            }
        //        }
        //    }
        //    if (didSynchronize || this.Count != stresstests.Count)
        //    {

        //        didSynchronize = true;
        //        foreach (Stresstest.Stresstest stresstest in synchedStresstests)
        //            stresstests.Remove(stresstest);
        //        this.ClearWithoutInvokingEvent();
        //        for (int i = 0; i < inOrder.Length; i++)
        //            if (inOrder[i] == null)
        //            {
        //                this.AddAsDefaultItem(new OldTileStresstest(stresstests[0]));
        //                stresstests.RemoveAt(0);
        //            }
        //            else
        //            {
        //                this.AddAsDefaultItem(inOrder[i]);
        //            }
        //    }
        //    return didSynchronize;
        //}

        #endregion
    }
}