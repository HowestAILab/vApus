/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Stresstest;

namespace vApus.DistributedTesting {
    /// <summary>
    /// Holds Tiles and Clients.
    /// </summary>
    [ContextMenu(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { "Edit", "Remove", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    [DisplayName("Distributed Test")]
    public class DistributedTest : LabeledBaseItem {

        #region Fields
        private RunSynchronization _runSynchronization = RunSynchronization.BreakOnFirstFinished;
        private int _maxRerunsBreakOnLast = 10;

        //For in the results database
        private string _description = string.Empty;
        private string[] _tags = new string[0];
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
        public RunSynchronization RunSynchronization {
            get { return _runSynchronization; }
            set { _runSynchronization = value; }
        }

        [SavableCloneable]
        [DisplayName("Maximum Reruns for Break on Last Run Synchronisation"),
        Description("The minumum allowed value is 0 (infinite) the maximum is 99.")]
        public int MaxRerunsBreakOnLast {
            get { return _maxRerunsBreakOnLast; }
            set {
                if (value < 0) value = 0;
                if (value > 99) value = 99;
                _maxRerunsBreakOnLast = value;
            }
        }

        [SavableCloneable]
        public string Description {
            get { return _description; }
            set { _description = value; }
        }

        [SavableCloneable]
        public string[] Tags {
            get { return _tags; }
            set { _tags = value; }
        }

        public Tiles Tiles { get { return this[0] as Tiles; } }

        public Clients Clients { get { return this[1] as Clients; } }

        /// <summary>
        /// Yield returns the used (checked in GUI) tile stresstests.
        /// </summary>
        public IEnumerable<TileStresstest> UsedTileStresstests {
            get {
                foreach (Tile tile in Tiles)
                    if (tile.Use)
                        foreach (TileStresstest tileStresstest in tile)
                            if (tileStresstest.Use && tileStresstest.BasicTileStresstest.SlaveIndices.Length != 0)
                                yield return tileStresstest;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Holds Tiles and Clients.
        /// </summary>
        public DistributedTest() {
            AddAsDefaultItem(new Tiles());
            AddAsDefaultItem(new Clients());
        }
        #endregion

        #region Functions
        public override void Activate() { SolutionComponentViewManager.Show(this); }
        #endregion
    }
}