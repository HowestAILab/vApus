using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

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
            finally
            {
            }
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
