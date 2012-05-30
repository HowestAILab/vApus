/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using vApus.SolutionTree;
using System.Collections.Generic;

namespace vApus.DistributedTesting
{
    public class Client : BaseItem
    {
        #region Fields
        private bool _use = false;
        private string _ip = string.Empty, _hostName = string.Empty;
        #endregion

        #region Properties
        [SavableCloneable]
        public bool Use
        {
            get { return _use; }
            set { _use = value; }
        }
        [SavableCloneable]
        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }
        [SavableCloneable]
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
        #endregion

        #region Constructors
        public Client()
        {
            ShowInGui = false;
            AddAsDefaultItem(new Slave());
        }
        #endregion

        #region Functions
        public void Sort()
        {
            List<Slave> slaves = new List<Slave>();
            foreach (Slave slave in this)
                slaves.Add(slave);

            if (!IsSorted(slaves))
            {
                slaves.Sort(vApus.DistributedTesting.Slave.SlaveComparer.GetInstance());

                this.ClearWithoutInvokingEvent();
                this.AddRange(slaves);
            }
        }
        private bool IsSorted(List<Slave> slaves)
        {
            for (int i = 0; i < slaves.Count - 1; i++)
                if (slaves[i].Port.CompareTo(slaves[i + 1].Port) > 0)
                    return false;
            return true;
        }
        public Client Clone()
        {
            Client clone = new Client();
            clone.ClearWithoutInvokingEvent();

            clone.Use = _use;
            clone.HostName = _hostName;
            clone.IP = _ip;

            foreach (Slave slave in this)
                clone.AddWithoutInvokingEvent(slave.Clone());

            return clone;
        }
        public override string ToString()
        {
            string hostname = (_hostName.Length == 0) ? string.Empty : _hostName;
            string ip = (_ip.Length == 0) ? string.Empty : _ip;

            return "Host Name: " + hostname + "  -  IP: " + ip;
        }
        #endregion
    }
}
