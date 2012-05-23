using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using vApus.SolutionTree;
using vApus.Stresstest;
using vApus.Util;

namespace vApus.DistributedTesting
{
    public class BasicTileStresstest : BaseItem
    {
        #region Fields
        private Connection _connection;
        private Monitor.MonitorProject _monitorProject;
        private int[] _monitorIndices = { };
        private Monitor.Monitor[] _monitors = { };

        private int[] _slaveIndices = { };
        private Slave[] _slaves = { };
        private int[] _WorkDistribution = { };
        #endregion

        #region Properties
        [Description("The connection to the application to test.")]
        [PropertyControl(0), SavableCloneable]
        public Connection Connection
        {
            get
            {
                if (_connection != null)
                    _connection.SetDescription("The connection to the application to test. [" + ConnectionProxy + "]");

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
                if (_monitorProject != null)
                {
                    List<Monitor.Monitor> l = new List<Monitor.Monitor>(_monitorIndices.Length);
                    foreach (int index in _monitorIndices)
                        if (index < _monitorProject.Count)
                        {
                            var monitor = _monitorProject[index] as Monitor.Monitor;
                            if (!l.Contains(monitor))
                                l.Add(monitor);
                        }

                    _monitors = l.ToArray();
                    _monitors.SetParent(_monitorProject);
                }
            }
        }
        [PropertyControl(1)]
        public Monitor.Monitor[] Monitors
        {
            get { return _monitors; }
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

                    List<int> l = new List<int>(_monitors.Length);
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

                ClientsAndSlaves slavesParent = SlavesParent;
                if (slavesParent != null)
                {
                    List<Slave> l = new List<Slave>(_slaveIndices.Length);
                    foreach (int index in _slaveIndices)
                        if (index < slavesParent.Count)
                        {
                            var slave = SlavesParent[index] as Slave;
                            if (!l.Contains(slave))
                                l.Add(slave);
                        }

                    _slaves = l.ToArray();
                    _slaves.SetParent(slavesParent);
                }
            }
        }
        [PropertyControl(2)]
        public Slave[] Slaves
        {
            get
            {

                _slaves.SetParent(SlavesParent);
                return _slaves;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Can be empty but not null.");

                _slaves = value;

                ClientsAndSlaves slavesParent = SlavesParent;
                if (slavesParent != null)
                {
                    _slaves.SetParent(slavesParent);

                    List<int> l = new List<int>(_slaves.Length);
                    for (int index = 0; index != slavesParent.Count; index++)
                        if (_slaves.Contains(slavesParent[index]) && !l.Contains(index))
                            l.Add(index);

                    _slaveIndices = l.ToArray();
                }
            }
        }
        private ClientsAndSlaves SlavesParent
        {
            get
            {
                try
                {
                    if (this.Parent != null &&
                        this.Parent.GetParent() != null &&
                        this.Parent.GetParent().GetParent() != null &&
                        this.Parent.GetParent().GetParent().GetParent() != null)
                        return (this.Parent.GetParent().GetParent().GetParent() as DistributedTest).ClientsAndSlaves;
                }
                catch { }
                return null;
            }
        }
        #endregion

        #region Constructor
        public BasicTileStresstest()
        {
            ShowInGui = false;
            if (Solution.ActiveSolution != null)
                Init();
            else
                Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);
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
            Connection = SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Connection), Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.Connections))) as Stresstest.Connection;
            _monitorProject = Solution.ActiveSolution.GetSolutionComponent(typeof(Monitor.MonitorProject)) as Monitor.MonitorProject;

            Monitors = new Monitor.Monitor[0];
            Slaves = new Slave[0];

            SolutionComponentChanged += new EventHandler<SolutionComponentChangedEventArgs>(SolutionComponentChanged_SolutionComponentChanged);
        }
        private void _connection_ParentIsNull(object sender, EventArgs e)
        {
            Connection = SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Connection), Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.Connections))) as Stresstest.Connection;
        }
        private void SolutionComponentChanged_SolutionComponentChanged(object sender, SolutionComponentChangedEventArgs e)
        {
            //Cleanup _monitors if _monitorProject Changed
            if (sender == _monitorProject || sender is Monitor.Monitor)
            {
                List<Monitor.Monitor> l = new List<Monitor.Monitor>(_monitorProject.Count);
                foreach (Monitor.Monitor monitor in _monitors)
                    if (!l.Contains(monitor) && _monitorProject.Contains(monitor))
                        l.Add(monitor);

                Monitors = l.ToArray();
            }
            else //Cleanup slaves
            {
                ClientsAndSlaves slavesParent = SlavesParent;
                if (sender == slavesParent || sender is Slave)
                {
                    List<Slave> l = new List<Slave>(slavesParent.Count);
                    foreach (Slave slave in _slaves)
                        if (!l.Contains(slave) && slavesParent.Contains(slave))
                            l.Add(slave);

                    Slaves = l.ToArray();
                }
            }
        }

        /// <summary>
        /// Create clone of this.
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