using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using vApus.Stresstest;
using System.IO;

namespace vApus.DistributedTesting
{
    public partial class Wizard : Form
    {
        #region Fields
        private DistributedTest _distributedTest;
        private Bitmap _transparentImage = new Bitmap(16, 16);
        #endregion

        /// <summary>
        /// Don't forget to call SetDistributedTest(DistributedTest).
        /// </summary>
        public Wizard()
        {
            InitializeComponent();

            Graphics g = Graphics.FromImage(_transparentImage);
            g.FillRectangle(Brushes.Transparent, 0, 0, 16, 16);
            g.Dispose();
        }

        #region Functions
        /// <summary>
        /// Set this, otherwise nothing will happen.
        /// </summary>
        /// <param name="distributedTest"></param>
        public void SetDistributedTest(DistributedTest distributedTest)
        {
            if (_distributedTest != distributedTest)
            {
                _distributedTest = distributedTest;
                SetDistributedTestToGui();
            }
        }
        private void SetDistributedTestToGui()
        {
            SetDefaultTestSettings();
            SetGenerateTiles();
            SetAddClientsAndSlaves();
        }
        private void SetDefaultTestSettings()
        {
            chkUseRDP.Checked = _distributedTest.UseRDP;
            cboRunSync.SelectedIndex = (int)_distributedTest.RunSynchronization;
            lblResultPath.Text = _distributedTest.ResultPath;
        }
        private void SetGenerateTiles()
        {
            nudTiles.Value = _distributedTest.Tiles.Count == 0 ? 1 : 0;
        }
        private void SetAddClientsAndSlaves()
        {
            StresstestProject stresstestProject = SolutionTree.Solution.ActiveSolution.GetProject("StresstestProject") as StresstestProject;
            nudTests.Value = stresstestProject.CountOf(typeof(Stresstest.Stresstest));

            foreach (Client client in _distributedTest.Clients)
                dgvClients.Rows.Add(client.HostName.Length == 0 ? client.IP : client.HostName,
                     client.UserName, client.Domain, client.Password, client.Count, 0);

            SetCountsInGui();

            nudTiles.ValueChanged += new EventHandler(this.nudTiles_ValueChanged);
            nudTests.ValueChanged += new EventHandler(this.nudTests_ValueChanged);
            dgvClients.CellEndEdit += new DataGridViewCellEventHandler(dgvClients_CellEndEdit);
            dgvClients.RowsRemoved += new DataGridViewRowsRemovedEventHandler(this.dgvClients_RowsRemoved);
        }

        private void SetCountsInGui()
        {
            int totalNewTestCount = (int)(nudTiles.Value * nudTests.Value);

            int totalTestCount = totalNewTestCount, totalUsedTestCount = totalNewTestCount;

            foreach (Tile tile in _distributedTest.Tiles)
                foreach (TileStresstest ts in tile)
                {
                    ++totalTestCount;
                    if (ts.Use)
                        ++totalUsedTestCount;
                }

            int totalSlaveCount = 0;
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (row.Cells[4].Value == row.Cells[4].DefaultNewRowValue)
                    break;

                totalSlaveCount += int.Parse(row.Cells[4].Value.ToString());
            }
            int totalAssignedTestCount = totalSlaveCount <= totalUsedTestCount ? totalSlaveCount : totalUsedTestCount;

            clmSlaves.HeaderText = "Number of Slaves (" + totalSlaveCount + ")";
            clmTests.HeaderText = "Number of Tests (" + totalAssignedTestCount + ")";

            int yetToAssingTestCount = totalAssignedTestCount;
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (row.Cells[4].Value == row.Cells[4].DefaultNewRowValue)
                    break;

                if (yetToAssingTestCount == 0)
                {
                    row.Cells[5].Value = 0;
                }
                else
                {
                    int slaveCount = int.Parse(row.Cells[4].Value.ToString());

                    int tests = yetToAssingTestCount - slaveCount >= 0 ? slaveCount : yetToAssingTestCount;
                    row.Cells[5].Value = tests;
                    yetToAssingTestCount -= tests;

                }
            }


            lblNotAssignedTests.Text = (totalTestCount - totalAssignedTestCount) + " Tests are not Assigned to a Slave";
            if (totalUsedTestCount != 0)
                lblNotAssignedTests.Text += "; " + (totalTestCount - totalUsedTestCount) + " are not Used (Checked)";
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            SetGuiToDistributedTest();
            this.Close();
        }
        private void SetGuiToDistributedTest()
        {

        }


        private void chkUseRDP_CheckedChanged(object sender, EventArgs e)
        {
            clmUserName.HeaderText = chkUseRDP.Checked ? "* User Name" : "User Name";
            clmPassword.HeaderText = chkUseRDP.Checked ? "* Password" : "Password";
        }

        private void llblResultPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Directory.Exists(_distributedTest.ResultPath))
                folderBrowserDialog.SelectedPath = _distributedTest.ResultPath;
            else
                folderBrowserDialog.SelectedPath = Application.StartupPath;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK && lblResultPath.Text != folderBrowserDialog.SelectedPath)
                lblResultPath.Text = folderBrowserDialog.SelectedPath;
        }

        private void dgvClients_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvClients.Columns[e.ColumnIndex].Name == "clmPassword" && e.Value != null)
            {
                dgvClients.Rows[e.RowIndex].Tag = e.Value;
                e.Value = new String('*', e.Value.ToString().Length);
            }
        }

        private void dgvClients_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvClients.CurrentRow.Tag != null)
                e.Control.Text = dgvClients.CurrentRow.Tag.ToString();
        }
        #endregion

        private void nudTiles_ValueChanged(object sender, EventArgs e)
        {
            SetCountsInGui();
        }

        private void nudTests_ValueChanged(object sender, EventArgs e)
        {
            SetCountsInGui();
        }

        private void dgvClients_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SetCountsInGui();
        }

        private void dgvClients_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            SetCountsInGui();
        }
    }
}
