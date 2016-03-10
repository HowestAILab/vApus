/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils;
using RandomUtils.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml;
using vApus.Communication.Shared;
using vApus.SolutionTree;
using vApus.StressTest;
using vApus.Util;

namespace vApus.DistributedTest {
    public class TileStressTest : LabeledBaseItem {

        #region Fields
        private readonly object _lock = new object();

        private bool _use = true;

        /// <summary>
        ///     Only when the solution is fully loaded.
        ///     Use in some cases (like changing the Use property).
        /// </summary>
        private StressTest.StressTest _defaultAdvancedSettingsTo;
        private bool _automaticDefaultAdvancedSettings = true;

        #endregion

        #region Properties
        /// <summary>
        ///     To be able to link the stress test to the right tile stress test.
        ///     #.# (TileIndex.TileStressTestIndex eg 0.0);
        /// </summary>
        public string TileStressTestIndex {
            get {
                if (DividedStressTestIndex == null) {
                    object parent = Parent;
                    if (parent == null)
                        return "-1";
                    return (parent as Tile).Index + "." + Index;
                }
                return DividedStressTestIndex;
            }
        }

        public BasicTileStressTest BasicTileStressTest { get { return this[0] as BasicTileStressTest; } }

        public AdvancedTileStressTest AdvancedTileStressTest { get { return this[1] as AdvancedTileStressTest; } }

        [SavableCloneable]
        public bool Use {
            get { return _use; }
            set { _use = value; }
        }

        [SavableCloneable]
        public StressTest.StressTest DefaultAdvancedSettingsTo {
            get {
                if (Solution.ActiveSolution != null && (_defaultAdvancedSettingsTo.IsEmpty || _defaultAdvancedSettingsTo.Parent == null)) {
                    _defaultAdvancedSettingsTo = GetNextOrEmptyChild(typeof(StressTest.StressTest), Solution.ActiveSolution.GetSolutionComponent(typeof(StressTestProject))) as StressTest.StressTest;

                    SynchronizationContextWrapper.SynchronizationContext.Send(delegate { InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited); }, null);
                }
                return _defaultAdvancedSettingsTo;
            }
            set {
                _defaultAdvancedSettingsTo = value;
            }
        }

        [SavableCloneable]
        public bool AutomaticDefaultAdvancedSettings {
            get { return _automaticDefaultAdvancedSettings; }
            set { _automaticDefaultAdvancedSettings = value; }
        }

        /// <summary>
        /// If the work is divided on multiple slaves and this is a clone of the original stress test this must be filled in.
        /// Must be the original index + . + # (last number being the part of the division)
        /// </summary>
        public string DividedStressTestIndex { get; set; }
        #endregion

        #region Constructors
        public TileStressTest() {
            ShowInGui = false;
            AddAsDefaultItem(new BasicTileStressTest());
            AddAsDefaultItem(new AdvancedTileStressTest());

            if (Solution.ActiveSolution != null) {
                DefaultAdvancedSettingsTo = GetNextOrEmptyChild(typeof(StressTest.StressTest), Solution.ActiveSolution.GetSolutionComponent(typeof(StressTestProject))) as StressTest.StressTest;
            } else {
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
            }
        }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            DefaultAdvancedSettingsTo = GetNextOrEmptyChild(typeof(StressTest.StressTest), Solution.ActiveSolution.GetSolutionComponent(typeof(StressTestProject))) as StressTest.StressTest;
        }

        /// <summary>
        /// Use this after adding a slave.
        /// </summary>
        internal void ForceDefaultTo() {
            if (DefaultAdvancedSettingsTo != null)
                AdvancedTileStressTest.DefaultTo(_defaultAdvancedSettingsTo);
        }

        /// <summary>
        /// Select a slave if one is available, if not no slave will be selected.
        /// Call this function after adding a new tile stress test or duplicating one in the GUI.
        /// This will not invoke an event to notify the GUI.
        /// </summary>
        public void SelectAvailableSlave() {
            try {
                //Get the ones that are available
                var availableSlaves = new List<Slave>();

                var distributedTest = Parent.GetParent().GetParent() as DistributedTest;
                if (distributedTest != null) {
                    foreach (Client client in distributedTest.Clients)
                        foreach (Slave slave in client)
                            availableSlaves.Add(slave);

                    foreach (Tile tile in distributedTest.Tiles)
                        if (tile.Use)
                            foreach (TileStressTest tileStressTest in tile)
                                if (tileStressTest.Use && tileStressTest.BasicTileStressTest.SlaveIndices.Length != 0)
                                    availableSlaves.Remove(tileStressTest.BasicTileStressTest.Slaves[0]);

                    BasicTileStressTest.Slaves = availableSlaves.Count == 0 ? new Slave[0] : new Slave[] { availableSlaves[0] };
                }
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed selecting next available slave.", ex);
            }
        }

        /// <summary>
        ///     Create a clone of this.
        /// </summary>
        /// <returns></returns>
        public TileStressTest Clone() {
            var clone = new TileStressTest();
            clone.Use = _use;
            clone._defaultAdvancedSettingsTo = DefaultAdvancedSettingsTo;
            clone.AutomaticDefaultAdvancedSettings = _automaticDefaultAdvancedSettings;

            clone.ClearWithoutInvokingEvent();
            clone.AddWithoutInvokingEvent(BasicTileStressTest.Clone());
            clone.AddWithoutInvokingEvent(AdvancedTileStressTest.Clone());
            return clone;
        }

        /// <summary>
        /// This is sent to a slave.
        /// </summary>
        /// <param name="stressTestIdInDb">-1 for none</param>
        /// <param name="runSynchronization"></param>
        /// <returns></returns>
        public StressTestWrapper GetStressTestWrapper(FunctionOutputCache functionOutputCache, RunSynchronization runSynchronization, int maxRerunsBreakOnLast) {
            lock (_lock) {
                string tileStressTestIndex = TileStressTestIndex;
                var stressTest = new StressTest.StressTest();
                stressTest.ForDistributedTest = true;
                stressTest.IsDividedStressTest = DividedStressTestIndex != null;
                stressTest.ShowInGui = false;
                stressTest.ActionDistribution = AdvancedTileStressTest.ActionDistribution;
                stressTest.MaximumNumberOfUserActions = AdvancedTileStressTest.MaximumNumberOfUserActions;
                stressTest.Concurrencies = AdvancedTileStressTest.Concurrencies;

                var connections = new Connections();
                var connection = BasicTileStressTest._connection.Clone();

                connection.RemoveDescription();
                connections.AddWithoutInvokingEvent(connection);
                connection.ForceSettingChildsParent();

                stressTest.Connection = connection;

                stressTest.Label = ToString();

                var allScenarios = new Scenarios();

                var scenarios = new KeyValuePair<Scenario, uint>[AdvancedTileStressTest.Scenarios.Length];
                for (int i = 0; i != scenarios.Length; i++) {
                    var kvp = AdvancedTileStressTest.Scenarios[i];

                    var scenario = CloneScenario(functionOutputCache, kvp.Key);

                    allScenarios.AddWithoutInvokingEvent(scenario);
                    scenario.ForceSettingChildsParent();

                    scenarios[i] = new KeyValuePair<Scenario, uint>(scenario, kvp.Value);
                }

                stressTest.ScenariosOverride = scenarios;

                stressTest.MinimumDelayOverride = AdvancedTileStressTest.MinimumDelay;
                stressTest.MaximumDelayOverride = AdvancedTileStressTest.MaximumDelay;
                stressTest.Runs = AdvancedTileStressTest.Runs;
                stressTest.Shuffle = AdvancedTileStressTest.Shuffle;
                stressTest.UseParallelExecutionOfRequests = AdvancedTileStressTest.UseParallelExecutionOfRequests;
                stressTest.MaximumPersistentConnections = AdvancedTileStressTest.MaximumPersistentConnections;
                stressTest.PersistentConnectionsPerHostname = AdvancedTileStressTest.PersistentConnectionsPerHostname;

                stressTest.SimplifiedFastResults = AdvancedTileStressTest.SimplifiedFastResults;

                stressTest.ForceSettingChildsParent();


                string[] monitors = new string[BasicTileStressTest.Monitors.Length];
                for (int i = 0; i != monitors.Length; i++) monitors[i] = BasicTileStressTest.Monitors[i].ToString();

                return new StressTestWrapper {
                    DistributedTest = Parent.ToString(), StressTest = stressTest,
                    PublishResultSetId = Publish.Publisher.LastGeneratedResultSetId,
                    Publish = Publish.Publisher.Settings.PublisherEnabled, PublishHost = Publish.Publisher.Settings.TcpHost,
                    PublishPort = Publish.Publisher.Settings.TcpPort,
                    TileStressTest = this.ToString(), TileStressTestIndex = tileStressTestIndex, RunSynchronization = runSynchronization,
                    Monitors = monitors,
                    MaxRerunsBreakOnLast = maxRerunsBreakOnLast
                };
            }
        }

        private Scenario CloneScenario(FunctionOutputCache functionOutputCache, Scenario scenario) {
            var cacheEntry = functionOutputCache.GetOrAdd(MethodInfo.GetCurrentMethod(), scenario);
            var clone = cacheEntry.ReturnValue as Scenario;
            if (clone == null) {
                clone = scenario.Clone(true, false, true, false);
                clone.RemoveDescription();
                cacheEntry.ReturnValue = clone;
            }
            return cacheEntry.ReturnValue as Scenario;
        }

        public override string ToString() { return "[TS " + TileStressTestIndex + "] "; }
        #endregion
    }
}
