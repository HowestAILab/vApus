/*
 * Copyright 2012 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System.ServiceProcess;

namespace vApus.JumpStartService
{
    public partial class JumpStartService : ServiceBase
    {
        private SocketListener _socketListener;
        
        public JumpStartService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _socketListener = SocketListener.GetInstance();
                _socketListener.Start();
            }
            catch { }
        }

        protected override void OnStop()
        {
            StopSocketListener();
        }

        private void StopSocketListener()
        {
            if (_socketListener != null)
            {
                try
                {
                    if (_socketListener.IsRunning)
                        _socketListener.Stop();
                }
                catch { }
                _socketListener = null;
            }
        }
    }
}
