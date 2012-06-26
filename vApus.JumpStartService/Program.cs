/*
 * Copyright 2012 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Windows.Forms;

namespace vApus.JumpStartService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //Self installer
            if (System.Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        MessageBox.Show("The vApus JumpStart Service must be installed to allow distributed stresstesting.\nFill in your Full Qualified ADMINISTRATOR Username ({domain or Local computer name}\\{user name}) and password on the next dialog.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Retry:
                        try
                        {
                            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });

                            try
                            {
                                foreach (ServiceController sc in ServiceController.GetServices())
                                    if (sc.ServiceName == "vApus.JumpStartService")
                                    {
                                        sc.Start();
                                        break;
                                    }
                            }
                            catch { }
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null && ex.InnerException.Message.StartsWith("User has canceled installation") &&
                                MessageBox.Show("Are you sure?\nYou won't be able to do distributed tests.", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                return;

                            goto Retry;
                        }
                        break;
                    case "--uninstall":
                        try
                        {
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        }
                        catch { }
                        break;
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			    { 
				    new JumpStartService() 
			    };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
