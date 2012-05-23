/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    [ContextMenu(new string[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new string[] { "Edit", "Remove", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new string[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new Keys[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    [DisplayName("Distributed Test")]
    public class DistributedTest : LabeledBaseItem
    {
        //#region Events
        //public event EventHandler TilesSynchronized;
        //#endregion

        #region Fields
        private RunSynchronization _runSynchronization;
        private string _resultPath = SpecialFolder.GetPath(SpecialFolder.Folder.Desktop);
        #endregion

        #region Properties
        [SavableCloneable]
        [DisplayName("Run Synchronization")]
        public RunSynchronization RunSynchronization
        {
            get { return _runSynchronization; }
            set { _runSynchronization = value; }
        }
        /// <summary>
        /// The path where to the results are saved.
        /// </summary>
        
        [SavableCloneable]
        public string ResultPath
        {
            get { return _resultPath; }
            set { _resultPath = value; }
        }

        public Tiles Tiles
        {
            get { return this[0] as Tiles; }
        }
        public ClientsAndSlaves ClientsAndSlaves
        {
            get { return this[1] as ClientsAndSlaves; }
        }
        #endregion

        #region Constructor
        public DistributedTest()
        {
            SolutionComponent.SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponent_SolutionComponentChanged);
            AddAsDefaultItem(new Tiles());
            AddAsDefaultItem(new ClientsAndSlaves());
        }
        #endregion

        #region Functions
        private void SolutionComponent_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            //if (sender is Stresstest.Stresstest || sender is Stresstest.StresstestProject)
            //{
            //    bool didSynchronize = true;
            //    //Only needs to be checked once.
            //    foreach (BaseItem item in Tiles)
            //        if (!(item as Tile).SynchronizeTileStresstests())
            //        {
            //            didSynchronize = false;
            //            break;
            //        }
            //    if (didSynchronize && TilesSynchronized != null)
            //        TilesSynchronized(this, null);
            //}
        }
        public override void Activate()
        {
            SolutionComponentViewManager.Show(this, typeof(NewDistributedTestView));
        }
        #endregion
    }
}
