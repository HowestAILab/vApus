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

namespace vApus.Util
{
    public partial class DisableFirewallAutoUpdatePanel : Panel
    {
        public DisableFirewallAutoUpdatePanel()
        {
            InitializeComponent();
        }
        public override string ToString()
        {
            return "Windows Firewall / Auto Update";
        }
    }
}
