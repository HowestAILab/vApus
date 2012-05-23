/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using vApus.SolutionTree;

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
        public override string ToString()
        {
            if (_hostName.Length == 0)
                return _ip;
            if (_ip.Length == 0)
                return _hostName;
            
            return _hostName + " / " + _ip;
        }
        #endregion
    }
}
