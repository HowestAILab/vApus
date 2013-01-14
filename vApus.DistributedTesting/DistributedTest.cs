/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Stresstest;

namespace vApus.DistributedTesting
{
    [ContextMenu(new[] {"Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click"},
        new[] {"Edit", "Remove", "Copy", "Cut", "Duplicate"})]
    [Hotkeys(new[] {"Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click"},
        new[] {Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D)})]
    [DisplayName("Distributed Test")]
    public class DistributedTest : LabeledBaseItem
    {
        #region Fields

        private string _resultPath;
        private RunSynchronization _runSynchronization = RunSynchronization.BreakOnFirstFinished;

        #endregion

        #region Properties

        /// <summary>
        ///     True if you want vApus to open remote desktop connections to the used clients.
        ///     Regardless if you check it or not, you need to be logged into the clients to be able to stresstest.
        /// </summary>
        [SavableCloneable]
        [DisplayName("Use RDP")]
        public bool UseRDP { get; set; }

        /// <summary>
        ///     Run Synchronization exists to keep all the tests equal in duration.
        ///     That way the tested applications are never idle and results can be matched/compared.
        ///     Break on First: If a run from a test is finished the other runs will break.
        ///     Break on Last: Runs will re-run untill the longest one is finished for the first time.
        ///     Note that the vApus think time is included in the test duration of a run.
        /// </summary>
        [SavableCloneable]
        [DisplayName("Run Synchronization")]
        public RunSynchronization RunSynchronization
        {
            get { return _runSynchronization; }
            set { _runSynchronization = value; }
        }

        /// <summary>
        ///     The path where to the results are saved.
        /// </summary>
        [SavableCloneable]
        public string ResultPath
        {
            get
            {
                if (_resultPath != DefaultResultPath || !Directory.Exists(_resultPath))
                    _resultPath = DefaultResultPath;
                return _resultPath;
            }
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

        public Clients Clients
        {
            get { return this[1] as Clients; }
        }

        #endregion

        #region Constructor

        public DistributedTest()
        {
            _resultPath = DefaultResultPath;

            AddAsDefaultItem(new Tiles());
            AddAsDefaultItem(new Clients());
        }

        #endregion

        #region Functions

        public override void Activate()
        {
            SolutionComponentViewManager.Show(this, typeof (DistributedTestView));
        }

        #endregion
    }
}