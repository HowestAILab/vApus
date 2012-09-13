/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;
using vApusSMT.Base;

namespace vApus.Monitor
{
    [ContextMenu(new string[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new string[] { "Edit", "Remove", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new string[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" }, new Keys[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    [Serializable]
    public class Monitor : LabeledBaseItem
    {
        #region Fields
        protected internal List<MonitorSource> _monitorSources = new List<MonitorSource>();
        private MonitorSource _monitorSource = new MonitorSource(string.Empty);
        private int _monitorSourceIndex;

        private int _previousMonitorSourceIndexForCounters;
        private Dictionary<Entity, List<CounterInfo>> _wiw = new Dictionary<Entity, List<CounterInfo>>();

        private object[] _parameters = new object[0];
        private string[] _filter = new string[0];
        private bool _batchResultSaving;
        #endregion

        #region Properties
        /// <summary>
        /// To check if the counters match the monitor source.
        /// </summary>
        [SavableCloneable]
        public int PreviousMonitorSourceIndexForCounters
        {
            get { return _previousMonitorSourceIndexForCounters; }
            set { _previousMonitorSourceIndexForCounters = value; }
        }
        [SavableCloneable]
        public int MonitorSourceIndex
        {
            get { return _monitorSourceIndex; }
            set { _monitorSourceIndex = value; }
        }
        [DisplayName("Monitor Source"), PropertyControl(1)]
        public MonitorSource MonitorSource
        {
            get
            {
                _monitorSource.SetParent(_monitorSources);
                return _monitorSource;
            }
            set
            {
                _monitorSource = value;
                _monitorSourceIndex = _monitorSources.IndexOf(_monitorSource);
            }
        }

        /// <summary>
        ///The counters you want to monitor.
        /// </summary>
        public Dictionary<Entity, List<CounterInfo>> Wiw
        {
            get { return _wiw; }
            internal set { _wiw = value; }
        }
        /// <summary>
        /// All the parameters, just the values, the names and types and such come from the monitor source.
        /// </summary>
        public object[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
        /// <summary>
        /// To be able to load and save this.
        /// </summary>
        [SavableCloneable]
        public string WIWRepresentation
        {
            get { return _wiw.ToBinaryToString(); }
            set
            {
                _wiw = value.ToByteArrayToObject() as Dictionary<Entity, List<CounterInfo>>;
                if (_wiw == null)
                    _wiw = new Dictionary<Entity, List<CounterInfo>>();
            }
        }
        /// <summary>
        /// To be able to load and save this.
        /// </summary>
        [SavableCloneable]
        public string[] ParametersRepresentation
        {
            get
            {
                string[] repr = new string[_parameters.Length];
                for (int i = 0; i != _parameters.Length; i++)
                    repr[i] = _parameters[i].ToBinaryToString();
                return repr;
            }
            set
            {
                _parameters = new object[value.Length];
                for (int i = 0; i != value.Length; i++)
                    _parameters[i] = value[i].ToByteArrayToObject();
            }
        }
        [SavableCloneable()]
        [Description("To filter the counters in a (large) counter collection. Wild card * can be used. Not case sensitive. All entries are in OR-relation with each other.")]
        public string[] Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        #endregion

        #region Functions
        /// <summary>
        /// Set the monitor sources to be able to monitor.
        /// </summary>
        /// <param name="monitorSources"></param>
        public void SetMonitorSources(string[] monitorSources)
        {
            _monitorSources.Clear();
            if (monitorSources != null)
                foreach (string source in monitorSources)
                {
                    MonitorSource monitorSource = new MonitorSource(source);
                    _monitorSources.Add(monitorSource);
                    monitorSource.SetParent(_monitorSources);
                }

            if (monitorSources == null || monitorSources.Length == 0)
            {
                _monitorSourceIndex = -1;
            }
            else
            {

                if (_monitorSourceIndex == -1)
                    _monitorSourceIndex = 0;
                else if (_monitorSourceIndex >= _monitorSources.Count)
                    _monitorSourceIndex = _monitorSources.Count - 1;

                _monitorSource = _monitorSources[_monitorSourceIndex];
            }
        }
        public override void Activate()
        {
            SolutionComponentViewManager.Show(this);
        }
        #endregion
    }
}
