/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Collections.Generic;
using vApus.SolutionTree;

namespace vApus.DistributedTest {
    public class Client : BaseItem {

        #region Fields
        private string _domain = string.Empty;
        private string _hostName = string.Empty;
        private string _ip = string.Empty;

        //RDP credentials.
        private string _password = string.Empty;
        private string _userName = string.Empty;
        #endregion

        #region Properties
        [SavableCloneable]
        public string HostName {
            get { return _hostName; }
            set { _hostName = value; }
        }

        [SavableCloneable]
        public string IP {
            get { return _ip; }
            set { _ip = value; }
        }

        [SavableCloneable]
        public string UserName {
            get { return _userName; }
            set { _userName = value; }
        }

        [SavableCloneable(true)]
        public string Password {
            get { return _password; }
            set { _password = value; }
        }

        [SavableCloneable]
        public string Domain {
            get { return _domain; }
            set { _domain = value; }
        }

        /// <summary>
        ///     The count of the slaves that are used.
        /// </summary>
        public int UsedSlaveCount {
            get {
                int count = 0;
                try {
                    foreach (Slave s in this)
                        if (s.TileStressTest != null)
                            ++count;
                } catch {
                }
                return count;
            }
        }
        #endregion

        #region Constructors
        public Client() { ShowInGui = false; }
        #endregion

        #region Functions
        public void Sort() {
            var slaves = new List<Slave>();
            foreach (Slave slave in this) slaves.Add(slave);

            if (!IsSorted(slaves)) {
                slaves.Sort(Slave.SlaveComparer.GetInstance());

                ClearWithoutInvokingEvent();
                AddRange(slaves);
            }
        }

        private bool IsSorted(List<Slave> slaves) {
            for (int i = 0; i < slaves.Count - 1; i++)
                if (slaves[i].Port.CompareTo(slaves[i + 1].Port) > 0)
                    return false;
            return true;
        }

        public Client Clone() {
            var clone = new Client();
            clone.ClearWithoutInvokingEvent();

            clone.HostName = _hostName;
            clone.IP = _ip;

            foreach (Slave slave in this) clone.AddWithoutInvokingEvent(slave.Clone());

            return clone;
        }

        public override string ToString() {
            string hostname = (_hostName.Length == 0) ? string.Empty : _hostName;
            string ip = (_ip.Length == 0) ? string.Empty : _ip;

            return hostname + " - " + ip;
        }
        #endregion
    }
}