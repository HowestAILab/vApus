/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting {
    public class TileStresstest : LabeledBaseItem {
        #region Fields
        //For encrypting the mysql password
        private static string _passwordGUID = "{51E6A7AC-06C2-466F-B7E8-4B0A00F6A21F}";
        private static readonly byte[] _salt =
            {
                0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03,
                0x62
            };
        private bool _automaticDefaultAdvancedSettings = true;

        /// <summary>
        ///     Only when the solution is fully loaded.
        ///     Use in some cases (like changing the Use property).
        /// </summary>
        internal bool _canDefaultAdvancedSettingsTo = false;

        private Stresstest.Stresstest _defaultAdvancedSettingsTo;
        private bool _use = true;

        private readonly object _lock = new object();

        #endregion

        #region Properties

        [SavableCloneable]
        public Stresstest.Stresstest DefaultAdvancedSettingsTo {
            get {
                if (_defaultAdvancedSettingsTo.IsEmpty)
                    DefaultAdvancedSettingsTo =
                        GetNextOrEmptyChild(typeof(Stresstest.Stresstest),
                                            Solution.ActiveSolution.GetSolutionComponent(typeof(StresstestProject))) as
                        Stresstest.Stresstest;

                return _defaultAdvancedSettingsTo;
            }
            set {
                value.ParentIsNull -= _defaultAdvancedSettingsTo_ParentIsNull;
                _defaultAdvancedSettingsTo = value;
                _defaultAdvancedSettingsTo.ParentIsNull += _defaultAdvancedSettingsTo_ParentIsNull;
            }
        }

        [SavableCloneable]
        public bool AutomaticDefaultAdvancedSettings {
            get { return _automaticDefaultAdvancedSettings; }
            set { _automaticDefaultAdvancedSettings = value; }
        }

        [SavableCloneable]
        public bool Use {
            get { return _use; }
            set { _use = value; }
        }

        /// <summary>
        ///     To be able to link the stresstest to the right tile stresstest.
        ///     #.# (TileIndex.TileStresstestIndex eg 0.0);
        /// </summary>
        public string TileStresstestIndex {
            get {
                object parent = Parent;
                if (parent == null)
                    return "-1";
                return (parent as Tile).Index + "." + Index;
            }
        }

        public BasicTileStresstest BasicTileStresstest {
            get { return this[0] as BasicTileStresstest; }
        }

        public AdvancedTileStresstest AdvancedTileStresstest {
            get { return this[1] as AdvancedTileStresstest; }
        }

        #endregion

        #region Constructors

        public TileStresstest() {
            ShowInGui = false;
            AddAsDefaultItem(new BasicTileStresstest());
            AddAsDefaultItem(new AdvancedTileStresstest());

            if (Solution.ActiveSolution != null) {
                DefaultAdvancedSettingsTo =
                    GetNextOrEmptyChild(typeof(Stresstest.Stresstest),
                                        Solution.ActiveSolution.GetSolutionComponent(typeof(StresstestProject))) as
                    Stresstest.Stresstest;
                _canDefaultAdvancedSettingsTo = true;
            } else {
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
            }
        }

        #endregion

        #region Functions

        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            _canDefaultAdvancedSettingsTo = false;
            DefaultAdvancedSettingsTo =
                GetNextOrEmptyChild(typeof(Stresstest.Stresstest),
                                    Solution.ActiveSolution.GetSolutionComponent(typeof(StresstestProject))) as
                Stresstest.Stresstest;
            _canDefaultAdvancedSettingsTo = true;
        }

        private void _defaultAdvancedSettingsTo_ParentIsNull(object sender, EventArgs e) {
            if (_defaultAdvancedSettingsTo == sender) {
                DefaultAdvancedSettingsTo =
                    GetNextOrEmptyChild(typeof(Stresstest.Stresstest),
                                        Solution.ActiveSolution.GetSolutionComponent(typeof(StresstestProject))) as
                    Stresstest.Stresstest;
                SynchronizationContextWrapper.SynchronizationContext.Send(
                    delegate { InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited); },
                    null);
            }
        }

        public override string ToString() {
            return "[TS " + TileStresstestIndex + "] ";
        }

        /// <summary>
        ///     Create a clone of this.
        /// </summary>
        /// <returns></returns>
        public TileStresstest Clone() {
            var clone = new TileStresstest();
            clone.Use = _use;
            clone.DefaultAdvancedSettingsTo = _defaultAdvancedSettingsTo;
            clone.AutomaticDefaultAdvancedSettings = _automaticDefaultAdvancedSettings;

            clone.ClearWithoutInvokingEvent();
            clone.AddWithoutInvokingEvent(BasicTileStresstest.Clone());
            clone.AddWithoutInvokingEvent(AdvancedTileStresstest.Clone());
            return clone;
        }

        /// <summary>
        /// This is sent to a slave.
        /// </summary>
        /// <param name="stresstestIdInDb">-1 for none</param>
        /// <param name="runSynchronization"></param>
        /// <returns></returns>
        public StresstestWrapper GetStresstestWrapper(long stresstestIdInDb, string databaseName, RunSynchronization runSynchronization) {
            lock (_lock) {
                string tileStresstestIndex = TileStresstestIndex;
                var stresstest = new Stresstest.Stresstest();
                stresstest.SetSolution();
                stresstest.ForDistributedTest = true;
                stresstest.ShowInGui = false;
                stresstest.Distribute = AdvancedTileStresstest.Distribute;
                stresstest.Concurrencies = AdvancedTileStresstest.Concurrencies;

                var connections = new Connections();
                var connection = BasicTileStresstest._connection.Clone();

                connection.RemoveDescription();
                connections.AddWithoutInvokingEvent(connection, false);
                connection.ForceSettingChildsParent();

                stresstest.Connection = connection;

                stresstest.Label = ToString();

                var logs = new Logs();
                var log = AdvancedTileStresstest._log;//.Clone();

                log.RemoveDescription();
                logs.AddWithoutInvokingEvent(log);
                log.ForceSettingChildsParent();

                stresstest.Log = log;

                //Nothing happens with this on the other side
                //var monitors = Monitors;
                //stresstest.Monitors = new Monitor.Monitor[monitors.Length];
                //for (int i = 0; i != monitors.Length; i++)
                //    stresstest.Monitors[i] = monitors[i];

                stresstest.MinimumDelayOverride = AdvancedTileStresstest.MinimumDelay;
                stresstest.MaximumDelayOverride = AdvancedTileStresstest.MaximumDelay;
                stresstest.Runs = AdvancedTileStresstest.Runs;
                stresstest.Shuffle = AdvancedTileStresstest.Shuffle;
                stresstest.UseParallelExecutionOfLogEntries = false;
                // AdvancedTileStresstest.useParallelExecutionOfLogEntries;

                stresstest.ForceSettingChildsParent();

                string user, host, password;
                int port;
                vApus.Results.SettingsManager.GetCurrentCredentials(out user, out host, out port, out password);

                return new StresstestWrapper {
                    StresstestIdInDb = stresstestIdInDb,
                    Stresstest = stresstest, TileStresstestIndex = tileStresstestIndex, RunSynchronization = runSynchronization
                    , MySqlHost = host, MySqlPort = port, MySqlDatabaseName = databaseName, MySqlUser = user, MySqlPassword = password.Encrypt(_passwordGUID, _salt)
                };
            }
        }

        #endregion
    }
}
