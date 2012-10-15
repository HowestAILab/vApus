/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using vApus.Util;
using vApus.Monitor;

namespace vApus.Stresstest
{
    public partial class MonitorReportControl : UserControl
    {
        #region Fields
        private List<ListViewItem> _averages;
        private string[] _headers;
        private Dictionary<DateTime, float[]> _monitorValues;
        private StresstestResults _stresstestResults;
        private string _configuration;
        #endregion

        public MonitorReportControl()
        {
            InitializeComponent();
            cboDrillDownAverages.SelectedIndex = 0;
            cboDrillDownDetailed.SelectedIndex = 0;
        }

        #region Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="monitorValues"></param>
        /// <param name="stresstestResults">Match timestamps using this</param>
        public void SetConfig_Headers_MonitorValuesAndStresstestResults(string configuration, string[] headers, Dictionary<DateTime, float[]> monitorValues, StresstestResults stresstestResults)
        {
            _configuration = configuration;
            _stresstestResults = stresstestResults;
            SetHeaders(headers);
            SetMonitorValues(monitorValues);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers">Keeps this in a field.</param>
        private void SetHeaders(string[] headers)
        {
            lvwAveragesListing.SuspendLayout();

            _headers = headers;
            //Clear the headers first
            int startIndex = 1;
            if (cboDrillDownAverages.SelectedIndex == 1)
                ++startIndex;
            else if (cboDrillDownAverages.SelectedIndex == 2)
                startIndex += 2;
            // -1 for not removing the fill column
            int count = lvwAveragesListing.Columns.Count - startIndex - 1;
            for (int i = 0; i < count; i++)
                lvwAveragesListing.Columns.RemoveAt(startIndex);

            //To Fill the rest of the header, otherwise other widths will not be correct.
            clmFill.Width = -2;

            if (headers.Length != 0)
            {
                ColumnHeader[] clms = new ColumnHeader[headers.Length - 1];
                // The first is an empty one for the timestamp
                for (int i = 1; i != headers.Length; i++)
                {
                    ColumnHeader clm = new ColumnHeader();
                    clm.Text = headers[i];
                    clm.Width = -2;
                    clms[i - 1] = clm;
                }

                lvwAveragesListing.Columns.AddRange(clms);
            }
            lvwAveragesListing.Columns.Remove(clmFill);
            lvwAveragesListing.Columns.Add(clmFill);
            clmFill.Width = -2;

            lvwAveragesListing.ResumeLayout();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="monitorValues">Keeps this in a field.</param>
        private void SetMonitorValues(Dictionary<DateTime, float[]> monitorValues)
        {
            lblWarningInvalidAverages.Visible = false;

            lvwAveragesListing.SuspendLayout();
            lvwAveragesListing.Items.Clear();
            _monitorValues = monitorValues;
            switch (cboDrillDownAverages.SelectedIndex)
            {
                case 0:
                    SetConcurrentUsersAverages();
                    break;
                case 1:
                    SetPrecisionAverages();
                    break;
                case 2:
                    SetRunAverages();
                    break;
            }

            lvwAveragesListing.ResumeLayout();

            flpDetailedResults.SuspendLayout();
            flpDetailedResults.Controls.Clear();

            switch (cboDrillDownDetailed.SelectedIndex)
            {
                case 0:
                    SetStresstestDetails();
                    break;
                case 1:
                    SetConcurrentUsersDetails();
                    break;
                case 2:
                    SetPrecisionDetails();
                    break;
                case 3:
                    SetRunDetails();
                    break;
            }

            flpDetailedResults.ResumeLayout();
        }

        private void btnConfiguration_Click(object sender, EventArgs e)
        {
            if (_configuration != null)
                (new ConfigurationDialog(_configuration)).ShowDialog();
        }

        #region Averages
        private void SetConcurrentUsersAverages()
        {
            if (_stresstestResults != null && _monitorValues != null)
                foreach (ConcurrentUsersResult result in _stresstestResults.ConcurrentUsersResults)
                {
                    List<RunResult> runResults = new List<RunResult>();
                    foreach (PrecisionResult pr in result.PrecisionResults)
                        foreach (RunResult rr in pr.RunResults)
                            runResults.Add(rr);

                    AddAverage(runResults, result.ConcurrentUsers);
                }
        }
        private void SetPrecisionAverages()
        {
            if (_stresstestResults != null && _monitorValues != null)
                foreach (ConcurrentUsersResult cur in _stresstestResults.ConcurrentUsersResults)
                    foreach (PrecisionResult result in cur.PrecisionResults)
                    {
                        List<RunResult> runResults = new List<RunResult>();
                        foreach (RunResult rr in result.RunResults)
                            runResults.Add(rr);

                        // From zero based to one based
                        AddAverage(runResults, cur.ConcurrentUsers, result.Precision + 1);
                    }
        }
        private void SetRunAverages()
        {
            if (_stresstestResults != null && _monitorValues != null)
                foreach (ConcurrentUsersResult cur in _stresstestResults.ConcurrentUsersResults)
                    foreach (PrecisionResult pr in cur.PrecisionResults)
                        foreach (RunResult result in pr.RunResults)
                        {
                            List<RunResult> runResults = new List<RunResult>();
                            runResults.Add(result);

                            // From zero based to one based
                            AddAverage(runResults, cur.ConcurrentUsers, pr.Precision + 1, result.Run + 1);
                        }
        }

        /// <summary>
        /// Gets averages from _monitor values and adds a list view item to the lvw and to _averages (if not present already).
        /// Make sure the lvw is cleared.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="ids">ConcurrentUsers and/or Precision and/or Run</param>
        private void AddAverage(List<RunResult> results, params int[] ids)
        {
            //Check if it is present already, do nothing in that case, just add it to the lvw.
            ListViewItem lvwi = HasAverages(results);

            if (lvwi == null)
            {
                //Get all the subranges based on the from and the to times.
                var range = new Dictionary<DateTime, float[]>();
                foreach (RunResult result in results)
                {
                    var subRange = GetRange(result.RunStartedAndStopped);
                    foreach (DateTime timeStamp in subRange.Keys)
                        range.Add(timeStamp, subRange[timeStamp]);
                }

                //Average divider
                int rangeCount = range.Count;
                if (rangeCount != 0)
                {
                    lvwi = new ListViewItem();
                    lvwi.UseItemStyleForSubItems = false;

                    Font font = new Font("Consolas", 10.25f);

                    lvwi.Font = font;
                    foreach (int i in ids)
                        lvwi.SubItems.Add(i.ToString()).Font = font;

                    float[] averages = null;

                    //Get all the values for a timestamp
                    foreach (DateTime at in range.Keys)
                    {
                        float[] values = range[at];

                        // The averages length must be the same as the values length.
                        if (averages == null)
                            averages = new float[values.Length];

                        for (int i = 0; i != values.Length; i++)
                        {
                            float value = values[i], average = averages[i];

                            if (value == -1) //Detect invalid values.
                                averages[i] = -1;
                            else if (average != -1) //Add the value to the averages at the same index (i), divide it first (no overflow).
                                averages[i] = average + (value / rangeCount);
                        }
                    }

                    //This is null when there are no values.
                    if (averages != null)
                    {
                        //Add the averages to the lvwi.
                        foreach (float average in averages)
                            if (average == -1)
                            {
                                lblWarningInvalidAverages.Visible = true;
                                var subItem = lvwi.SubItems.Add("N/A: The counter became unavailable");
                                subItem.Font = font;
                                subItem.ForeColor = Color.Red;
                            }
                            else
                            {
                                lvwi.SubItems.Add(StringUtil.FloatToLongString(average)).Font = font;
                            }

                        //Remove the empty sub item
                        lvwi.SubItems.RemoveAt(0);


                        lvwi.Tag = results;
                        //Keep this in memory so it can be restored if the averages were determined once.
                        //The tag is used to check if the lvwi exists for the run result, or results (precision has more than one for example)
                        if (_averages == null)
                            _averages = new List<ListViewItem>();
                        _averages.Add(lvwi);
                    }
                }
            }

            if (lvwi != null)
                lvwAveragesListing.Items.Add(lvwi);
        }
        /// <summary>
        /// Checks if it (_averages) already has the averages with the given tag and returns it if so.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private ListViewItem HasAverages(List<RunResult> tag)
        {
            if (_averages != null)
                foreach (ListViewItem lvwi in _averages)
                    if (lvwi.Tag == tag)
                        return lvwi;
            return null;
        }

        private void cboDrillDownAverages_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblWarningInvalidAverages.Visible = false;

            lvwAveragesListing.SuspendLayout();
            lvwAveragesListing.Items.Clear();
            switch (cboDrillDownAverages.SelectedIndex)
            {
                case 0:
                    if (!lvwAveragesListing.Columns.Contains(clmConcurrentUsers))
                    {
                        lvwAveragesListing.Columns.Insert(0, clmConcurrentUsers);
                        clmConcurrentUsers.Width = -2;
                    }
                    if (lvwAveragesListing.Columns.Contains(clmPrecision))
                        lvwAveragesListing.Columns.Remove(clmPrecision);
                    if (lvwAveragesListing.Columns.Contains(clmRun))
                        lvwAveragesListing.Columns.Remove(clmRun);

                    //To Fill the rest of the header, otherwise other widths will not be correct.
                    clmFill.Width = -2;
                    lvwAveragesListing.Columns.Remove(clmFill);

                    SetConcurrentUsersAverages();

                    lvwAveragesListing.Columns.Add(clmFill);
                    clmFill.Width = -2;
                    break;
                case 1:
                    if (!lvwAveragesListing.Columns.Contains(clmConcurrentUsers))
                    {
                        lvwAveragesListing.Columns.Insert(0, clmConcurrentUsers);
                        clmConcurrentUsers.Width = -2;
                    }
                    if (!lvwAveragesListing.Columns.Contains(clmPrecision))
                    {
                        lvwAveragesListing.Columns.Insert(1, clmPrecision);
                        clmPrecision.Width = -2;
                    }
                    if (lvwAveragesListing.Columns.Contains(clmRun))
                        lvwAveragesListing.Columns.Remove(clmRun);

                    //To Fill the rest of the header, otherwise other witdths will not be correct.
                    clmFill.Width = -2;
                    lvwAveragesListing.Columns.Remove(clmFill);

                    SetPrecisionAverages();

                    lvwAveragesListing.Columns.Add(clmFill);
                    clmFill.Width = -2;
                    break;
                case 2:
                    if (!lvwAveragesListing.Columns.Contains(clmConcurrentUsers))
                    {
                        lvwAveragesListing.Columns.Insert(0, clmConcurrentUsers);
                        clmConcurrentUsers.Width = -2;
                    }
                    if (!lvwAveragesListing.Columns.Contains(clmPrecision))
                    {
                        lvwAveragesListing.Columns.Insert(1, clmPrecision);
                        clmPrecision.Width = -2;
                    }
                    if (!lvwAveragesListing.Columns.Contains(clmRun))
                    {
                        lvwAveragesListing.Columns.Insert(2, clmRun);
                        clmRun.Width = -2;
                    }
                    //To Fill the rest of the header, otherwise other witdths will not be correct.
                    clmFill.Width = -2;
                    lvwAveragesListing.Columns.Remove(clmFill);

                    SetRunAverages();

                    lvwAveragesListing.Columns.Add(clmFill);
                    clmFill.Width = -2;
                    break;
            }
            lvwAveragesListing.ResumeLayout();
        }
        private void btnSaveDisplayedAverages_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Where do you want to save the displayed averages?";
            sfd.FileName = "Averages";
            sfd.Filter = "TXT|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        sw.Write(GetDisplayedAverages());
                        sw.Flush();
                    }
                }
                catch
                {
                    MessageBox.Show("Could not access file: " + sfd.FileName, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }
        private string GetDisplayedAverages()
        {
            StringBuilder sb = new StringBuilder();
            //Don't use the 'fill' column
            int lastColumnIndex = lvwAveragesListing.Columns.Count - 2;
            if (lastColumnIndex > 0)
            {
                for (int i = 0; i != lvwAveragesListing.Columns.Count - 1; i++)
                {
                    sb.Append(lvwAveragesListing.Columns[i].Text);
                    if (i != lastColumnIndex)
                        sb.Append('\t');
                }
                sb.AppendLine();
            }

            foreach (ListViewItem lvwi in lvwAveragesListing.Items)
            {
                if (lvwi.SubItems.Count != 0)
                {
                    ListViewItem.ListViewSubItem lastSubItem = lvwi.SubItems[lvwi.SubItems.Count - 1];
                    foreach (ListViewItem.ListViewSubItem subItem in lvwi.SubItems)
                    {
                        sb.Append(subItem.Text);
                        if (subItem != lastSubItem)
                            sb.Append('\t');
                    }
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }
        #endregion

        #region Details
        private void SetStresstestDetails()
        {
            if (_stresstestResults != null && _monitorValues != null)
            {
                List<RunResult> runResults = new List<RunResult>();
                foreach (ConcurrentUsersResult cr in _stresstestResults.ConcurrentUsersResults)
                    foreach (PrecisionResult pr in cr.PrecisionResults)
                        foreach (RunResult rr in pr.RunResults)
                            runResults.Add(rr);

                AddDetail(runResults, _stresstestResults.Stresstest.ReplaceInvalidWindowsFilenameChars('_').Replace(' ', '_') + ".txt");
            }
        }
        private void SetConcurrentUsersDetails()
        {
            if (_stresstestResults != null && _monitorValues != null)
                foreach (ConcurrentUsersResult result in _stresstestResults.ConcurrentUsersResults)
                {
                    List<RunResult> runResults = new List<RunResult>();
                    foreach (PrecisionResult pr in result.PrecisionResults)
                        foreach (RunResult rr in pr.RunResults)
                            runResults.Add(rr);

                    AddDetail(runResults, _stresstestResults.Stresstest.ReplaceInvalidWindowsFilenameChars('_').Replace(' ', '_') +
                        "_Concurrent_Users_" + result.ConcurrentUsers + ".csv");
                }
        }
        private void SetPrecisionDetails()
        {
            if (_stresstestResults != null && _monitorValues != null)
                foreach (ConcurrentUsersResult cur in _stresstestResults.ConcurrentUsersResults)
                    foreach (PrecisionResult result in cur.PrecisionResults)
                    {
                        List<RunResult> runResults = new List<RunResult>();
                        foreach (RunResult rr in result.RunResults)
                            runResults.Add(rr);

                        //zero based to one based
                        AddDetail(runResults, _stresstestResults.Stresstest.ReplaceInvalidWindowsFilenameChars('_').Replace(' ', '_') +
                        "_Concurrent_Users_" + cur.ConcurrentUsers +
                        "_Precision_" + (result.Precision + 1) + ".csv");
                    }
        }
        private void SetRunDetails()
        {
            if (_stresstestResults != null && _monitorValues != null)
                foreach (ConcurrentUsersResult cur in _stresstestResults.ConcurrentUsersResults)
                    foreach (PrecisionResult pr in cur.PrecisionResults)
                        foreach (RunResult result in pr.RunResults)
                        {
                            List<RunResult> runResults = new List<RunResult>();
                            runResults.Add(result);

                            //zero based to one based
                            AddDetail(runResults, _stresstestResults.Stresstest.ReplaceInvalidWindowsFilenameChars('_').Replace(' ', '_') +
                            "_Concurrent_Users_" + cur.ConcurrentUsers +
                            "_Precision_" + (pr.Precision + 1) +
                            "_Run_" + (result.Run + 1).ToString() + ".csv");
                        }
        }
        private void AddDetail(List<RunResult> results, string text)
        {
            CheckBox chk = new CheckBox();
            chk.AutoSize = true;
            chk.Text = text;
            chk.Checked = true;

            //Determine the averages of a range of values.
            Dictionary<DateTime, float[]> range = new Dictionary<DateTime, float[]>();
            foreach (RunResult result in results)
            {
                Dictionary<DateTime, float[]> subRange = GetRange(result.RunStartedAndStopped);
                foreach (DateTime timeStamp in subRange.Keys)
                    range.Add(timeStamp, subRange[timeStamp]);
            }

            chk.Tag = range;
            flpDetailedResults.Controls.Add(chk);
        }
        private void cboDrillDownDetailed_SelectedIndexChanged(object sender, EventArgs e)
        {
            flpDetailedResults.SuspendLayout();
            flpDetailedResults.Controls.Clear();

            switch (cboDrillDownDetailed.SelectedIndex)
            {
                case 0:
                    SetStresstestDetails();
                    break;
                case 1:
                    SetConcurrentUsersDetails();
                    break;
                case 2:
                    SetPrecisionDetails();
                    break;
                case 3:
                    SetRunDetails();
                    break;
            }

            flpDetailedResults.ResumeLayout();
        }
        private void btnSaveCheckedDetailedResults_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Where do you want to save the detailed results?";
                if (fbd.ShowDialog() == DialogResult.OK)
                    foreach (CheckBox chk in flpDetailedResults.Controls)
                        SaveDetailedResults(fbd.SelectedPath, chk.Text.ReplaceInvalidWindowsFilenameChars('_'), chk.Tag as Dictionary<DateTime, float[]>);
            }
        }

        private void SaveDetailedResults(string folder, string fileName, Dictionary<DateTime, float[]> values)
        {
            string path = Path.Combine(folder, fileName);
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                try
                {
                    sw.WriteLine(GetHeaders("\t"));
                    foreach (string value in GetMonitorValues(values, "\t"))
                        sw.WriteLine(value);

                    sw.Flush();
                }
                catch
                {
                    MessageBox.Show("Cannot access '" + path + "' because it is in use!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private string GetHeaders(string separator)
        {
            return _headers.Combine(separator);
        }
        private IEnumerable<string> GetMonitorValues(Dictionary<DateTime, float[]> values, string separator)
        {
            foreach (DateTime timeStamp in values.Keys)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(timeStamp.ToString("dd/MM/yyyy HH:mm:ss.fff"));
                foreach (float value in values[timeStamp])
                {
                    sb.Append(separator);
                    sb.Append(StringUtil.FloatToLongString(value));
                }
                yield return sb.ToString();
            }
        }

        #endregion

        /// <summary>
        /// Get a range of values from _monitorValues.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private Dictionary<DateTime, float[]> GetRange(Dictionary<DateTime, DateTime> fromsAndTos)
        {
            Dictionary<DateTime, float[]> range = new Dictionary<DateTime, float[]>();
            foreach (DateTime from in fromsAndTos.Keys)
            {
                var subRange = GetRange(from, fromsAndTos[from]);
                foreach (DateTime timeStamp in subRange.Keys)
                    range.Add(timeStamp, subRange[timeStamp]);
            }
            return range;

        }
        /// <summary>
        /// Get a range of values from _monitorValues.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private Dictionary<DateTime, float[]> GetRange(DateTime from, DateTime to)
        {
            Dictionary<DateTime, float[]> range = new Dictionary<DateTime, float[]>();
            foreach (DateTime key in _monitorValues.Keys)
                if (key >= from && key <= to)
                    range.Add(key, _monitorValues[key]);
            return range;
        }

        private void btnWarning_Click(object sender, EventArgs e)
        {
            MessageBox.Show("No stresstest initialization times are taken into account aka monitoring results match only to the real stresstesting times.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

    }
}
