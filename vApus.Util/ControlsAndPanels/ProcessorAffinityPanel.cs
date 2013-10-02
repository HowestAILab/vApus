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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    /// Used in OptionsDialog, Handy to keep threads for different running instances of vApus separated.
    /// </summary>
    public partial class ProcessorAffinityPanel : Panel {
        public ProcessorAffinityPanel() {
            InitializeComponent();
            HandleCreated += ProcessorAffinityPanel_HandleCreated;
        }

        /// <summary>
        ///     to know how much cores a group contains
        /// </summary>
        /// <param name="groupNumber">the group number if any, or ALL_PROCESSOR_GROUPS (0xffff) for every group</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetActiveProcessorCount(ushort groupNumber);

        private void ProcessorAffinityPanel_HandleCreated(object sender, EventArgs e) {
            try {
                //Get the affinity from the current process in order to get the cpu's.
                var cpus = new List<int>(ProcessorAffinityHelper.FromBitmaskToArray(Process.GetCurrentProcess().ProcessorAffinity));

                //Add all cpu's to the listview and check them if they are in 'cpus'. (0xFFFF) to include all processor groups
                for (int i = 0; i < GetActiveProcessorCount(0xFFFF); i++) {
                    ListViewItem lvwi = null;
                    if (i < lvw.Items.Count) {
                        lvwi = lvw.Items[i];
                    } else {
                        lvwi = new ListViewItem();
                        lvwi.Text = "CPU " + (i + 1);
                        lvw.Items.Add(lvwi);
                    }
                    lvwi.Checked = (cpus.Contains(i));
                }
            } catch (Exception ex) {
                Enabled = false;
                MessageBox.Show("Processor affinity is not supported for this machine.\n" + ex, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSet_Click(object sender, EventArgs e) {
            var cpus = new List<int>();
            for (int i = 0; i < lvw.Items.Count; i++)
                if (lvw.Items[i].Checked)
                    cpus.Add(i);
            Process.GetCurrentProcess().ProcessorAffinity = ProcessorAffinityHelper.FromArrayToBitmask(cpus.ToArray());
            btnSet.Enabled = false;
        }

        private void lvw_ItemChecked(object sender, ItemCheckedEventArgs e) {
            var cpus = new List<int>();
            for (int i = 0; i < lvw.Items.Count; i++)
                if (lvw.Items[i].Checked)
                    cpus.Add(i);
            btnSet.Enabled = lvw.CheckedItems.Count > 0 &&
                             Process.GetCurrentProcess().ProcessorAffinity !=
                             ProcessorAffinityHelper.FromArrayToBitmask(cpus.ToArray());
        }

        public override string ToString() {
            return "Processor Affinity";
        }
    }
}