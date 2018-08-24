/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using vApus.Monitor.Sources.Base;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Monitor {
    /// <summary>
    /// Keeps all the monitoring settings the previous "what I want" counter info if any.
    /// </summary>
    [ContextMenu(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { "Edit", "Remove", "Copy", "Cut", "Duplicate" })]
    [Hotkeys(new[] { "Activate_Click", "Remove_Click", "Copy_Click", "Cut_Click", "Duplicate_Click" },
        new[] { Keys.Enter, Keys.Delete, (Keys.Control | Keys.C), (Keys.Control | Keys.X), (Keys.Control | Keys.D) })]
    [Serializable]
    public class Monitor : LabeledBaseItem {

        #region Fields
        private string[] _filter = new string[0];

        private MonitorSourceClient _monitorSourceClient = new MonitorSourceClient();
        private int _monitorSourceClientIndex, _previousMonitorSourceIndexForCounters;
        private string _monitorSourceClientName = string.Empty;
        protected internal List<MonitorSourceClient> _monitorSourceClients = new List<MonitorSourceClient>();

        private Entities _wiw = new Entities();

        private object[] _parameterValues = new object[0];
        #endregion

        #region Properties

        /// <summary>
        ///     To check if the counters match the monitor source.
        /// </summary>
        [SavableCloneable]
        public int PreviousMonitorSourceIndexForCounters {
            get { return _previousMonitorSourceIndexForCounters; }
            set { _previousMonitorSourceIndexForCounters = value; }
        }

        [SavableCloneable]
        public int MonitorSourceIndex {
            get { return _monitorSourceClientIndex; }
            set { _monitorSourceClientIndex = value; }
        }

        [SavableCloneable]
        public string MonitorSourceName {
            get { return _monitorSourceClientName; }
            set { _monitorSourceClientName = value; }
        }

        [DisplayName("Monitor source"), PropertyControl(1)]
        public MonitorSourceClient MonitorSource {
            get {
                _monitorSourceClient.SetParent(_monitorSourceClients);
                return _monitorSourceClient;
            }
            set {
                _monitorSourceClient = value;
                _monitorSourceClientIndex = _monitorSourceClients.IndexOf(_monitorSourceClient);
                _monitorSourceClientName = _monitorSourceClient.ToString();
            }
        }

        /// <summary>
        ///     The counters you want to monitor.
        /// </summary>
        public Entities Wiw {
            get { return _wiw; }
            set { _wiw = value; }
        }

        /// <summary>
        ///     All the parameters, just the values, the names and types and such come from the monitor source.
        /// </summary>
        public object[] ParameterValues {
            get { return _parameterValues; }
            set { _parameterValues = value; }
        }

        /// <summary>
        ///     To be able to load and save this.
        /// </summary>
        [SavableCloneable]
        public string WIWRepresentation {
            get {
                if (_wiw == null) _wiw = new Entities();
                return JsonConvert.SerializeObject(_wiw, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            }
            set {
                try {
                    _wiw = JsonConvert.DeserializeObject<Entities>(value);
                } catch {
                    //To make it 'backwards compatible' with older vass files.
                }
                if (_wiw == null) _wiw = new Entities();
            }
        }

        /// <summary>
        ///     To be able to load and save this.
        /// </summary>
        [SavableCloneable]
        public string[] ParametersRepresentation {
            get {
                var repr = new string[_parameterValues.Length];
                for (int i = 0; i != _parameterValues.Length; i++)
                    repr[i] = _parameterValues[i].ToBinaryToString();
                return repr;
            }
            set {
                _parameterValues = new object[value.Length];
                for (int i = 0; i != value.Length; i++)
                    _parameterValues[i] = value[i].ToByteArrayToObject();
            }
        }

        [SavableCloneable]
        [Description("To filter the counters in a (large) counter collection. Wild card * can be used. Not case sensitive. All entries are in OR-relation with each other.")]
        public string[] Filter {
            get { return _filter; }
            set { _filter = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Get all monitor source clients.
        /// </summary>
        /// <param name="monitorSources"></param>
        public void InitMonitorSourceClients() {
            if (_monitorSourceClients.Count == 0) {
                Dictionary<string, Type> clients = ClientFactory.Clients;
                foreach (var kvp in clients) {
                    var monitorSource = new MonitorSourceClient(kvp.Key, kvp.Value);
                    _monitorSourceClients.Add(monitorSource);
                    monitorSource.SetParent(_monitorSourceClients);
                }

                if (clients.Count == 0) {
                    _monitorSourceClientIndex = -1;
                    _monitorSourceClientName = string.Empty;
                } else {
                    if (_monitorSourceClientName == string.Empty) {
                        //Backwards compatible.
                        if (_monitorSourceClientIndex == -1)
                            _monitorSourceClientIndex = 0;
                        else if (_monitorSourceClientIndex >= _monitorSourceClients.Count)
                            _monitorSourceClientIndex = _monitorSourceClients.Count - 1;
                    } else {

                        //Match names instead of indices #727 
                        int candidate = 0;
                        for (; candidate != _monitorSourceClients.Count; candidate++)
                            if (_monitorSourceClients[candidate].ToString() == _monitorSourceClientName)
                                break;

                        _monitorSourceClientIndex = candidate;
                    }

                    _monitorSourceClient = _monitorSourceClients[_monitorSourceClientIndex];
                    _monitorSourceClientName = _monitorSourceClient.ToString();
                }
            }
        }

        public override BaseSolutionComponentView Activate() { return SolutionComponentViewManager.Show(this); }

        #endregion
    }
}