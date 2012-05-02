/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Windows.Forms;

namespace vApus.Util
{
    public partial class TracertDialog : Form
    {
        public TracertDialog()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="hostName"></param>
        /// <param name="roundtripTime">Already formatted</param>
        public void AddHop(string ip, string hostName, string roundtripTime)
        {
            lvw.Items.Add(new ListViewItem(new string[] { (lvw.Items.Count + 1).ToString(), ip, hostName, roundtripTime }));
        }
        public void ClearHops()
        {
            lvw.Items.Clear();
        }
    }
}
