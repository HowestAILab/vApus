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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;

namespace vApus.Util
{
    public partial class DisableFirewallAutoUpdatePanel : Panel
    {
        public enum Status
        {
            AllDisabled = 0,
            WindowsFirewallEnabled = 1,
            WindowsAutoUpdateEnabled = 2,
            AllEnabled = 3
        }
        private Status _status;
        private delegate void DisableThemDel();
        private DisableThemDel _disableThemCallback;
        private ActiveObject _activeObject = new ActiveObject();

        public Status __Status
        {
            get { return _status; }
        }

        public DisableFirewallAutoUpdatePanel()
        {
            InitializeComponent();
            _disableThemCallback = DisableThemCallback;
            _activeObject.OnResult += new EventHandler<ActiveObject.OnResultEventArgs>(_activeObject_OnResult);
            this.HandleCreated += new EventHandler(DisableFirewallAutoUpdatePanel_HandleCreated);
        }

        private void DisableFirewallAutoUpdatePanel_HandleCreated(object sender, EventArgs e)
        {
            CheckStatus();
        }
        public Status CheckStatus()
        {
            _status = Status.AllDisabled;

            //Firewall
            EvaluateValue(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\FirewallPolicy\DomainProfile", "EnableFirewall", 0), 0, Status.WindowsFirewallEnabled);
            EvaluateValue(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\FirewallPolicy\PublicProfile", "EnableFirewall", 0), 0, Status.WindowsFirewallEnabled);
            EvaluateValue(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\FirewallPolicy\StandardProfile", "EnableFirewall", 0), 0, Status.WindowsFirewallEnabled);
            //windows update
            EvaluateValue(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update", "AUOptions", 1), 1, Status.WindowsAutoUpdateEnabled);
            EvaluateValue(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update", "IncludeRecommendedUpdates", 0), 0, Status.WindowsAutoUpdateEnabled);
            EvaluateValue(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update", "ElevateNonAdmins", 1), 1, Status.WindowsAutoUpdateEnabled);

            if (Handle != null)
                SetGui();
            return _status;
        }
        private void EvaluateValue(object value, int validValue, Status append)
        {
            try
            {
                if ((int)value != validValue)
                    _status |= append;
            }
            catch
            {
                LogWrapper.LogByLevel("[" + this + "] Failed checking if the firewall and Windows auto update are enabled or not!\nCould not find a registry key.", LogLevel.Error);
            }
        }
        private void SetGui()
        {
            kvpFirewall.BackColor = kvpWindowsAutoUpdate.BackColor = Color.LightGreen;
            kvpFirewall.Value = kvpWindowsAutoUpdate.Value = "Off";
            btnDisableThem.Enabled = false;

            switch (_status)
            {
                case Status.WindowsFirewallEnabled:
                    kvpFirewall.BackColor = Color.Orange;
                    kvpFirewall.Value = "On";
                    btnDisableThem.Enabled = true;
                    break;
                case Status.WindowsAutoUpdateEnabled:
                    kvpWindowsAutoUpdate.BackColor = Color.Orange;
                    kvpWindowsAutoUpdate.Value = "On";
                    btnDisableThem.Enabled = true;
                    break;
                case Status.AllEnabled:
                    kvpFirewall.BackColor = kvpWindowsAutoUpdate.BackColor = Color.Orange;
                    kvpFirewall.Value = kvpWindowsAutoUpdate.Value = "On";
                    btnDisableThem.Enabled = true;
                    break;
            }
        }
        private void btnDisableThem_Click(object sender, EventArgs e)
        {
            btnDisableThem.Enabled = false;
            btnDisableThem.Text = "Wait...";
            _activeObject.Send(_disableThemCallback);
        }
        private void DisableThemCallback()
        {
            try
            {
                //Disabling Windows Firewall
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\FirewallPolicy\DomainProfile", "EnableFirewall", 0, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\FirewallPolicy\PublicProfile", "EnableFirewall", 0, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\FirewallPolicy\StandardProfile", "EnableFirewall", 0, RegistryValueKind.DWord);

                //Restarting the process
                StartProcess("NET", "STOP MpsSvc");
                StartProcess("NET", "START MpsSvc");
            }
            catch (Exception ex)
            {
                LogWrapper.LogByLevel("[" + this + "] Failed disabling the firewall!\nCould not find a registry key.\n" + ex.ToString(), LogLevel.Error);
            }

            try
            {
                //Disabling Auto Update
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update", "AUOptions", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update", "IncludeRecommendedUpdates", 0, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update", "ElevateNonAdmins", 1, RegistryValueKind.DWord);

                //Restarting the process
                StartProcess("NET", "STOP wuauserv");
                StartProcess("NET", "START wuauserv");
            }
            catch (Exception ex)
            {
                LogWrapper.LogByLevel("[" + this + "] Failed disabling Windows auto update!\nCould not find a registry key.\n" + ex.ToString(), LogLevel.Error);
            }
        }
        private void _activeObject_OnResult(object sender, ActiveObject.OnResultEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                btnDisableThem.Text = "Disable Them";
                CheckStatus();
            }, null);
        }
        private void StartProcess(string process, string arguments)
        {
            Process p = null;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(process, arguments);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p = Process.Start(startInfo);
                p.WaitForExit();
            }
            catch { }
            if (p != null)
                try { p.Dispose(); }
                catch { }
            p = null;
        }
        public override string ToString()
        {
            return "Windows Firewall / Auto Update";
        }
    }
}
