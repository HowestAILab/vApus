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
using System.IO;

namespace vApus.DistributedTesting
{
    [ContextMenu(new string[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new string[] { "Edit", "Remove", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new string[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new Keys[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    [DisplayName("Distributed Test")]
    public class DistributedTest : LabeledBaseItem
    {
        #region Fields
        private RunSynchronization _runSynchronization;
        private string _resultPath;
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
            get {
                if (_resultPath != DefaultResultPath || !Directory.Exists(_resultPath))
                    _resultPath = DefaultResultPath;
                return _resultPath; }
            set { _resultPath = value; }
        }
        private string DefaultResultPath
        {
            get { return Path.Combine(Application.StartupPath, "DistributedTestResults"); }
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
            _resultPath = DefaultResultPath;

            AddAsDefaultItem(new Tiles());
            AddAsDefaultItem(new ClientsAndSlaves());
        }
        #endregion

        #region Functions
        public override void Activate()
        {
            SolutionComponentViewManager.Show(this, typeof(DistributedTestView));
        }
        #endregion
    }
}
