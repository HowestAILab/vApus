/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using vApus.Monitor;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public class BasicTileStresstest : BaseItem
    {
        #region Fields

        private readonly List<Slave> _slavesParent = new List<Slave>();
        private int[] _WorkDistribution = {};
        protected internal Connection _connection;
        private int[] _monitorIndices = {};
        private MonitorProject _monitorProject;
        private Monitor.Monitor[] _monitors = {};

        private int[] _slaveIndices = {};
        private Slave[] _slaves = {};

        #endregion

        #region Properties

        [Description("The connection to the application to test.")]
        [PropertyControl(0), SavableCloneable]
        public Connection Connection
        {
            get
            {
                if (_connection != null)
                {
                    if (_connection.IsEmpty)
                        Connection =
                            GetNextOrEmptyChild(typeof (Connection),
                                                Solution.ActiveSolution.GetSolutionComponent(typeof (Connections))) as
                            Connection;

                    _connection.SetDescription("The connection to the application to test. [" + ConnectionProxy + "]");
                }
                return _connection;
            }
            set
            {
                value.ParentIsNull -= _connection_ParentIsNull;
                _connection = value;
                _connection.ParentIsNull += _connection_ParentIsNull;
            }
        }

        [ReadOnly(true)]
        [DisplayName("Connection Proxy")]
        public string ConnectionProxy
        {
            get
            {
                if (_connection == null || _connection.IsEmpty || _connection.ConnectionProxy.IsEmpty)
                    return "Connection Proxy: <none>";
                return _connection.ConnectionProxy.ToString();
            }
        }

        [SavableCloneable]
        public int[] MonitorIndices
        {
            get { return _monitorIndices; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");

                _monitorIndices = value;
            }
        }

        [PropertyControl(1)]
        public Monitor.Monitor[] Monitors
        {
            get
            {
                if (_monitors.Length != _monitorIndices.Length && _monitorProject != null)
                {
                    var l = new List<Monitor.Monitor>(_monitorIndices.Length);
                    foreach (int index in _monitorIndices)
                        if (index < _monitorProject.Count)
                        {
                            var monitor = _monitorProject[index] as Monitor.Monitor;
                            if (!l.Contains(monitor))
                                l.Add(monitor);
                        }

                    _monitors = l.ToArray();
                }
                _monitors.SetParent(_monitorProject);

                return _monitors;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");
                if (value.Length > 5)
                    throw new ArgumentOutOfRangeException("Maximum 5 allowed.");

                _monitors = value;
                if (_monitorProject != null)
                {
                    _monitors.SetParent(_monitorProject);

                    var l = new List<int>(_monitors.Length);
                    for (int index = 0; index != _monitorProject.Count; index++)
                        if (_monitors.Contains(_monitorProject[index]) && !l.Contains(index))
                            l.Add(index);

                    _monitorIndices = l.ToArray();
                }
            }
        }

        [SavableCloneable]
        public int[] SlaveIndices
        {
            get { return _slaveIndices; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");

                _slaveIndices = value;
            }
        }

        [PropertyControl(2)]
        [Description(
            "Currently limited to one (only the first one counts). More than one slave will be handy in the future for many-to-one testing."
            )]
        public Slave[] Slaves
        {
            get
            {
                List<Slave> slavesParent = SlavesParent;
                if (_slaves.Length != _slaveIndices.Length && slavesParent != null)
                {
                    var l = new List<Slave>(_slaveIndices.Length);
                    foreach (int index in _slaveIndices)
                        if (index < slavesParent.Count)
                        {
                            Slave slave = slavesParent[index];
                            if (!l.Contains(slave))
                                l.Add(slave);
                        }

                    Slaves = l.ToArray();
                }
                _slaves.SetParent(slavesParent);
                return _slaves;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");

                _slaves = value;

                List<Slave> slavesParent = SlavesParent;
                if (slavesParent != null)
                {
                    _slaves.SetParent(slavesParent);

                    var l = new List<int>(_slaves.Length);
                    for (int index = 0; index != slavesParent.Count; index++)
                        if (_slaves.Contains(slavesParent[index]) && !l.Contains(index))
                            l.Add(index);

                    _slaveIndices = l.ToArray();
                }
            }
        }

        internal List<Slave> SlavesParent
        {
            get
            {
                try
                {
                    if (Parent != null &&
                        Parent.GetParent() != null &&
                        Parent.GetParent().GetParent() != null &&
                        Parent.GetParent().GetParent().GetParent() != null)
                    {
                        _slavesParent.Clear();
                        Clients clientsAndSlaves =
                            (Parent.GetParent().GetParent().GetParent() as DistributedTest).Clients;

                        foreach (Client client in clientsAndSlaves)
                            foreach (Slave slave in client)
                                _slavesParent.Add(slave);

                        return _slavesParent;
                    }
                }
                catch
                {
                }
                return null;
            }
        }

        [SavableCloneable]
        public int[] WorkDistribution
        {
            get { return _WorkDistribution; }
            set { _WorkDistribution = value; }
        }

        #endregion

        #region Constructor

        public BasicTileStresstest()
        {
            ShowInGui = false;
            if (Solution.ActiveSolution != null)
                Init();
            else
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
        }

        #endregion

        #region Functions

        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            Init();
        }

        private void Init()
        {
            Connection =
                GetNextOrEmptyChild(typeof (Connection),
                                    Solution.ActiveSolution.GetSolutionComponent(typeof (Connections))) as Connection;
            _monitorProject = Solution.ActiveSolution.GetSolutionComponent(typeof (MonitorProject)) as MonitorProject;

            _monitors = new Monitor.Monitor[0];
            _slaves = new Slave[0];

            SolutionComponentChanged += SolutionComponentChanged_SolutionComponentChanged;
        }

        private void _connection_ParentIsNull(object sender, EventArgs e)
        {
            if (_connection == sender)
                Connection =
                    GetNextOrEmptyChild(typeof (Connection),
                                        Solution.ActiveSolution.GetSolutionComponent(typeof (Connections))) as
                    Connection;
        }

        private void SolutionComponentChanged_SolutionComponentChanged(object sender,
                                                                       SolutionComponentChangedEventArgs e)
        {
            //Cleanup _monitors if _monitorProject Changed
            if (sender == _monitorProject || sender is Monitor.Monitor)
            {
                var l = new List<Monitor.Monitor>(_monitorProject.Count);
                foreach (Monitor.Monitor monitor in _monitors)
                    if (!l.Contains(monitor) && _monitorProject.Contains(monitor))
                        l.Add(monitor);

                Monitors = l.ToArray();
            }
            else //Cleanup slaves
            {
                List<Slave> slavesParent = SlavesParent;
                if (slavesParent != null && (sender == slavesParent || sender is Client || sender is Slave))
                {
                    var l = new List<Slave>(slavesParent.Count);
                    foreach (Slave slave in _slaves)
                        if (!l.Contains(slave) && slavesParent.Contains(slave))
                            l.Add(slave);

                    Slaves = l.ToArray();
                }
            }
        }

        /// <summary>
        ///     Create clone of this.f
        /// </summary>
        /// <returns></returns>
        public BasicTileStresstest Clone()
        {
            var clone = new BasicTileStresstest();
            clone.Connection = _connection;
            clone.MonitorIndices = new int[_monitorIndices.Length];
            _monitorIndices.CopyTo(clone.MonitorIndices, 0);
            clone.SlaveIndices = new int[_slaveIndices.Length];
            _slaveIndices.CopyTo(clone.SlaveIndices, 0);
            return clone;
        }

        #endregion
    }
}