/*
 * Copyright 2008 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace vApus.Util
{
    public partial class ProcessorAffinityPanel : Panel
    {
        public ProcessorAffinityPanel()
        {
            InitializeComponent();
            this.HandleCreated += new EventHandler(ProcessorAffinityPanel_HandleCreated);
        }
        private void ProcessorAffinityPanel_HandleCreated(object sender, EventArgs e)
        {
            try
            {
                //Get the affinity from the current process in order to get the cpu's.
                List<int> cpus = new List<int>(ProcessorAffinityCalculator.FromBitmaskToArray(Process.GetCurrentProcess().ProcessorAffinity));
                //Add all cpu's to the listview and check them if they are in 'cpus'.
                for (int i = 0; i < Environment.ProcessorCount; i++)
                {
                    ListViewItem lvwi = null;
                    if (i < lvw.Items.Count)
                    {
                        lvwi = lvw.Items[i];
                    }
                    else
                    {
                        lvwi = new ListViewItem();
                        lvwi.Text = "CPU " + (i + 1);
                        lvw.Items.Add(lvwi);
                    }
                    lvwi.Checked = (cpus.Contains(i));
                }
            }
            catch (Exception ex)
            {
                this.Enabled = false;
                MessageBox.Show("Processor affinity is not supported for this machine.\n" + ex, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSet_Click(object sender, EventArgs e)
        {
            List<int> cpus = new List<int>();
            for (int i = 0; i < lvw.Items.Count; i++)
                if (lvw.Items[i].Checked)
                    cpus.Add(i);
            Process.GetCurrentProcess().ProcessorAffinity = ProcessorAffinityCalculator.FromArrayToBitmask(cpus.ToArray());
            btnSet.Enabled = false;
        }

        private void lvw_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            List<int> cpus = new List<int>();
            for (int i = 0; i < lvw.Items.Count; i++)
                if (lvw.Items[i].Checked)
                    cpus.Add(i);
            btnSet.Enabled = lvw.CheckedItems.Count > 0 && Process.GetCurrentProcess().ProcessorAffinity != ProcessorAffinityCalculator.FromArrayToBitmask(cpus.ToArray());
        }
        public override string ToString()
        {
            return "Processor Affinity";
        }
    }
}