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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vApus.Util;
using vApusSMT.Base;

namespace vApus.Monitor
{
    public partial class AccordeonControl : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler Toggled;

        #region Fields
        private const int MAXHEIGHT = 350;
        private List<KeyValuePairControl> _progressControls = new List<KeyValuePairControl>();
        #endregion

        #region Constructor
        public AccordeonControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (this.Height == MAXHEIGHT)
                Toggle(false);
            else
                Toggle(true);
        }
        /// <summary>
        /// Expand or collapse this control.
        /// </summary>
        /// <param name="expand"></param>
        public void Toggle(bool expand)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            this.SuspendLayout();

            if (expand)
            {
                this.Height = MAXHEIGHT;
                btnToggle.Text = "-";
            }
            else
            {
                this.Height = this.MinimumSize.Height;
                chart.LegendViewState = LegendViewState.Collapsed;
                btnToggle.Text = "+";
            }
            this.ResumeLayout(true);

            if (Toggled != null)
                Toggled(this, null);

            LockWindowUpdate(0);
        }
        private void AccordeonControl_SizeChanged(object sender, EventArgs e)
        {
            LockWindowUpdate(this.Handle.ToInt32());
            splitContainer.Panel2Collapsed = !splitContainer.Panel2Collapsed;
            splitContainer.Panel2Collapsed = !splitContainer.Panel2Collapsed;
            LockWindowUpdate(0);
        }
        public void AddMonitorValueCollection(string entity, string counterGroup, MonitorValueCollection monitorValueCollection)
        {
            lblHeader.Text = monitorValueCollection.Counter;
            if (monitorValueCollection.Unit != null && monitorValueCollection.Unit.Length != 0)
                lblHeader.Text += " [" + monitorValueCollection.Unit + "]";

            chart.ChartAreaTitleText = lblHeader.Text;
            chart.XTitleText = "Timestamp";
            chart.YTitleText = monitorValueCollection.Unit;

            Series series = chart.AddNewSeries(entity, monitorValueCollection.Instance);
            string progressInstanceKey = monitorValueCollection.Instance == String.Empty ? "" : "/" + monitorValueCollection.Instance;
            SetProgressControls(entity + progressInstanceKey, monitorValueCollection);

            foreach (MonitorValue monitorValue in monitorValueCollection)
                chart.AddSeriesXYValue(series, monitorValue.TimeStamp.ToString() + '.' + monitorValue.TimeStamp.Millisecond, monitorValue.Value);
        }
        private void SetProgressControls(string key, MonitorValueCollection monitorValueCollection)
        {
            KeyValuePairControl progressControl = null;
            MonitorValue lastMonitorValue = monitorValueCollection[monitorValueCollection.Count - 1];
            foreach (KeyValuePairControl pc in _progressControls)
                if (pc.Key == key)
                {
                    progressControl = pc;
                    break;
                }
            if (progressControl == null)
            {
                progressControl = new KeyValuePairControl();
                progressControl.Key = key;
                progressControl.BackColor = Color.GhostWhite;
                progressControl.Margin = new Padding(3, 6, 0, 0);
                _progressControls.Add(progressControl);
                flp.Controls.Add(progressControl);
            }
            float maximumValue = 0;

            if (progressControl.Tag != null)
                maximumValue = (float)progressControl.Tag;
            float newMaximumValue = monitorValueCollection.MaxValue > lastMonitorValue.Value ? monitorValueCollection.MaxValue : lastMonitorValue.Value;
            if (newMaximumValue > maximumValue)
                maximumValue = newMaximumValue;

            progressControl.Tag = maximumValue;
            progressControl.Value = lastMonitorValue.Value + " / " + maximumValue;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.FileName = lblHeader.Text;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                {
                    try
                    {
                        Series series;

                        sw.WriteLine(lblHeader.Text);
                        sw.Write(chart.XTitleText);
                        for (int i = 0; i < chart.Count; i++)
                        {
                            series = chart.GetSeriesAt(i);
                            sw.Write(";");
                            sw.Write(series.Label);
                            if (series.Instance.Length != 0)
                            {
                                sw.Write("/");
                                sw.Write(series.Instance);
                            }
                        }
                        sw.WriteLine();

                        series = chart.GetSeriesAt(0);
                        foreach (string timeStamp in series.Keys)
                        {
                            sw.Write(timeStamp);
                            for (int i = 0; i < chart.Count; i++)
                                sw.Write(";" + chart.GetSeriesAt(i)[timeStamp]);
                            sw.WriteLine();
                        }
                        sw.Flush();
                    }
                    catch
                    {
                        MessageBox.Show("Cannot access '" + saveFileDialog.FileName + "' because it is in use!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
        }
        #endregion
    }
}