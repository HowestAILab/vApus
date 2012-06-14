/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public class TileStresstest : LabeledBaseItem
    {
        #region Fields
        private Stresstest.Stresstest _defaultSettingsTo;
        private bool _use = true, _automaticDefaultAdvancedSettings = true;
        #endregion

        #region Properties
        [SavableCloneable, PropertyControl]
        [DisplayName("Default Settings To")]
        public Stresstest.Stresstest DefaultSettingsTo
        {
            get { return _defaultSettingsTo; }
            set
            {
                value.ParentIsNull -= _defaultTo_ParentIsNull;
                _defaultSettingsTo = value;
                _defaultSettingsTo.ParentIsNull += new EventHandler(_defaultTo_ParentIsNull);
            }
        }
        [SavableCloneable]
        public bool AutomaticDefaultAdvancedSettings
        {
            get { return _automaticDefaultAdvancedSettings; }
            set { _automaticDefaultAdvancedSettings = value; }
        }
        [SavableCloneable]
        public bool Use
        {
            get { return _use; }
            set { _use = value; }
        }
        /// <summary>
        /// To be able to link the stresstest to the right tile stresstest.
        /// #.# (TileIndex.TileStresstestIndex eg 0.0); 
        /// </summary>
        public string TileStresstestIndex
        {
            get
            {
                object parent = Parent;
                if (parent == null)
                    return "-1";
                return (parent as Tile).Index + "." + this.Index;
            }
        }

        public BasicTileStresstest BasicTileStresstest
        {
            get { return this[0] as BasicTileStresstest; }
        }
        public AdvancedTileStresstest AdvancedTileStresstest
        {
            get { return this[1] as AdvancedTileStresstest; }
        }
        #endregion

        #region Constructors
        public TileStresstest()
        {
            ShowInGui = false;
            AddAsDefaultItem(new BasicTileStresstest());
            AddAsDefaultItem(new AdvancedTileStresstest());

            if (Solution.ActiveSolution != null)
                DefaultSettingsTo = SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Stresstest), Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.StresstestProject))) as Stresstest.Stresstest;
            else
                Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);
        }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            DefaultSettingsTo = SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Stresstest), Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.StresstestProject))) as Stresstest.Stresstest;
        }
        private void _defaultTo_ParentIsNull(object sender, EventArgs e)
        {
            DefaultSettingsTo = SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Stresstest), Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.StresstestProject))) as Stresstest.Stresstest;
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                this.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }, null);
        }

        public override string ToString()
        {
            return "[TS " + TileStresstestIndex + "] " + Label;
        }
        /// <summary>
        /// Create a clone of this.
        /// </summary>
        /// <returns></returns>
        public TileStresstest Clone()
        {
            var clone = new TileStresstest();
            clone.Label = Label;
            clone.Use = _use;
            clone.DefaultSettingsTo = _defaultSettingsTo;
            clone.AutomaticDefaultAdvancedSettings = _automaticDefaultAdvancedSettings;

            clone.ClearWithoutInvokingEvent();
            clone.AddWithoutInvokingEvent(BasicTileStresstest.Clone());
            clone.AddWithoutInvokingEvent(AdvancedTileStresstest.Clone());
            return clone;
        }
        //This is sent to a slave.
        public StresstestWrapper GetStresstestWrapper(RunSynchronization runSynchronization)
        {
            string tileStresstestIndex = TileStresstestIndex;
            Stresstest.Stresstest stresstest = new Stresstest.Stresstest();
            stresstest.ForDistributedTest = true;
            stresstest.ShowInGui = false;
            stresstest.Distribute = AdvancedTileStresstest.Distribute;
            stresstest.ConcurrentUsers = AdvancedTileStresstest.ConcurrentUsers;

            Connections connections = new Connections();
            Connection connection = BasicTileStresstest._connection;
            ObjectExtension.RemoveDescription(connection);
            connections.AddWithoutInvokingEvent(connection, false);
            connection.ForceSettingChildsParent();

            stresstest.Connection = connection;
            stresstest.DynamicRunMultiplier = AdvancedTileStresstest.DynamicRunMultiplier;

            stresstest.Label = this.ToString();

            Logs logs = new Logs();
            Log log = AdvancedTileStresstest._log;
            ObjectExtension.RemoveDescription(log);
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
            stresstest.Precision = AdvancedTileStresstest.Precision;
            stresstest.Shuffle = AdvancedTileStresstest.Shuffle;
            stresstest.UseParallelExecutionOfLogEntries = false;// AdvancedTileStresstest.useParallelExecutionOfLogEntries;

            stresstest.ForceSettingChildsParent();

            return new StresstestWrapper { Stresstest = stresstest, TileStresstestIndex = tileStresstestIndex, RunSynchronization = runSynchronization };
        }
        #endregion
    }
}
