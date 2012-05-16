using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vApus.Stresstest;
using vApus.SolutionTree;
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
        private Slave[] _slaves = { };
        private int[] _WorkDistribution = { };
        #endregion

        #region Properties
        [PropertyControl(0), SavableCloneable]
        public Connection Connection
        {
            get { return _connection; }
            set
            {
                value.ParentIsNull -= _connection_ParentIsNull;
                _connection = value;
                _connection.ParentIsNull += _connection_ParentIsNull;
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
                    {
                        var monitor = _monitorProject[index] as Monitor.Monitor;
                        if (index < _monitorProject.Count && !l.Contains(monitor))
                            l.Add(monitor);
                    }
                    _monitors = l.ToArray();
                    _monitors.SetParent(_monitorProject);
                }
            }
        }
       // [PropertyControl(1)]
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
       // [PropertyControl(2), SavableCloneable]
        public Slave[] Slaves
        {
            get { return _slaves; }
            set { _slaves = value; }
        }
        [PropertyControl(3), SavableCloneable]
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
                Solution.ActiveSolutionChanged += new EventHandler<ActiveSolutionChangedEventArgs>(Solution_ActiveSolutionChanged);
        }
        #endregion

        #region Functions
        private void Init()
        {
            Connection = SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Connection), Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.Connections))) as Stresstest.Connection;
            _monitorProject = Solution.ActiveSolution.GetSolutionComponent(typeof(Monitor.MonitorProject)) as Monitor.MonitorProject;

            Monitors = new Monitor.Monitor[0];
        }
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e)
        {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            Init();
        }
        private void _connection_ParentIsNull(object sender, EventArgs e)
        {
            Connection = SolutionComponent.GetNextOrEmptyChild(typeof(Stresstest.Connection), Solution.ActiveSolution.GetSolutionComponent(typeof(Stresstest.Connections))) as Stresstest.Connection;
        }
        #endregion
    }
}
