/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using vApusSMT.Base;

namespace vApus.Monitor
{
    public partial class OldMonitorControl : UserControl
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Fields
        private Dictionary<string, AccordeonControl> _accordeonControls = new Dictionary<string, AccordeonControl>();
        private Dictionary<string, Dictionary<string, HashSet<MonitorValueCollection>>> _monitorValues = new Dictionary<string, Dictionary<string, HashSet<MonitorValueCollection>>>();
        #endregion

        #region Properties
        public Dictionary<string, Dictionary<string, HashSet<MonitorValueCollection>>> MonitorValues
        {
            get { return _monitorValues; }
        }
        #endregion

        #region Constructor
        public OldMonitorControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        /// <summary>
        /// This will add values to the collection and will update the Gui.
        /// </summary>
        /// <param name="monitorValues"></param>
        public void AddMonitorValues(Dictionary<string, Dictionary<string, HashSet<MonitorValueCollection>>> monitorValues)
        {
            foreach (string entity in monitorValues.Keys)
                foreach (string counterGroup in monitorValues[entity].Keys)
                    foreach (MonitorValueCollection monitorValueCollection in monitorValues[entity][counterGroup])
                    {
                        IncrementMonitorValues(entity, counterGroup, monitorValueCollection);
                        SetAccordeonControls(entity, counterGroup, monitorValueCollection);
                    }
            btnSave.Enabled = true;
            btnToggle.Enabled = true;
        }
        private void IncrementMonitorValues(string entity, string counterGroup, MonitorValueCollection monitorValueCollection)
        {
            if (!_monitorValues.ContainsKey(entity))
                _monitorValues.Add(entity, new Dictionary<string, HashSet<MonitorValueCollection>>());
            if (!_monitorValues[entity].ContainsKey(counterGroup))
                _monitorValues[entity].Add(counterGroup, new HashSet<MonitorValueCollection>());

            HashSet<MonitorValueCollection> hashSet = _monitorValues[entity][counterGroup];
            MonitorValueCollection original = GetMonitorValueCollection(hashSet, monitorValueCollection.Counter, monitorValueCollection.Instance);
            if (original == null)
                original = monitorValueCollection;
            else
                foreach (MonitorValue value in monitorValueCollection)
                    original.Add(value);
            hashSet.Add(original);
        }
        private MonitorValueCollection GetMonitorValueCollection(HashSet<MonitorValueCollection> hashSet, string counter, string instance)
        {
            foreach (MonitorValueCollection monitorValueCollection in hashSet)
                if (monitorValueCollection.Counter == counter && monitorValueCollection.Instance == instance)
                    return monitorValueCollection;
            return null;
        }
        private void SetAccordeonControls(string entity, string counterGroup, MonitorValueCollection monitorValueCollection)
        {
            string key = counterGroup + " " + monitorValueCollection.Counter;
            AccordeonControl accordeonControl = null;

            if (_accordeonControls.ContainsKey(key))
            {
                accordeonControl = _accordeonControls[key];
            }
            else
            {
                accordeonControl = new AccordeonControl();

                //Toggle the first to collapsed anyway
                //if (_accordeonControls.Count != 0)
                    accordeonControl.Toggle(false);

                accordeonControl.Toggled += accordeonControl_Toggled;
                _accordeonControls.Add(key, accordeonControl);
            }

            accordeonControl.AddMonitorValueCollection(entity, counterGroup, monitorValueCollection);

            if (!pnl.Controls.Contains(accordeonControl))
            {
                LockWindowUpdate(this.Handle.ToInt32());

                accordeonControl.Left = pnl.Padding.Left;
                accordeonControl.Width = pnl.Width - pnl.Padding.Left - pnl.Padding.Right;
                accordeonControl.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                if (pnl.Controls.Count == 0)
                {
                    accordeonControl.Top = pnl.Padding.Top;

                    btnToggle.Text = "Toggle ( - )";
                }
                else
                {
                    Control previousControl = pnl.Controls[pnl.Controls.Count - 1];
                    accordeonControl.Top = previousControl.Bottom + previousControl.Margin.Bottom + accordeonControl.Margin.Top;

                    btnToggle.Text = "Toggle ( + )";
                }
                pnl.Controls.Add(accordeonControl);

                LockWindowUpdate(0);
            }
        }
        private void accordeonControl_Toggled(object sender, EventArgs e)
        {
            SetYPos();
        }
        private void SetYPos()
        {
            if (pnl.Controls.Count == 0)
                return;
            Control previousControl = pnl.Controls[0];
            for (int i = 1; i < pnl.Controls.Count; i++)
            {
                Control accordeonControl = pnl.Controls[i];
                accordeonControl.Top = previousControl.Bottom + previousControl.Margin.Bottom + accordeonControl.Margin.Top;
                previousControl = accordeonControl;
            }
        }
        /// <summary>
        /// This will clear all the values and will remove the accordeon controls.
        /// </summary>
        public void ClearMonitorValues()
        {
            pnl.Controls.Clear();
            _accordeonControls.Clear();
            _monitorValues.Clear();

            btnSave.Enabled = false;
            btnToggle.Enabled = false;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }
        private void Save()
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                Save(saveFileDialog.FileName);
                this.Cursor = Cursors.Default;
            }
        }
        /// <summary>
        /// Save all monitor values.
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                try
                {
                    sw.WriteLine(GetHeaders(";"));
                    foreach (string s in GetMonitorValues(";"))
                        sw.WriteLine(s);

                    sw.Flush();
                }
                catch
                {
                    MessageBox.Show("Cannot access '" + saveFileDialog.FileName + "' because it is in use!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public string[] GetHeaders()
        {
            List<string> l = new List<string>();
            l.Add(string.Empty);
            foreach (string entity in _monitorValues.Keys)
                foreach (string counterGroup in _monitorValues[entity].Keys)
                    foreach (MonitorValueCollection monitorValueCollection in _monitorValues[entity][counterGroup])
                    {
                        StringBuilder sb = new StringBuilder();
                        if (monitorValueCollection.Instance == String.Empty)
                            sb.Append(entity);
                        else
                            sb.Append(entity + "/" + monitorValueCollection.Instance);
                        sb.Append("//");
                        if (counterGroup != null && counterGroup.Length != 0)
                        {
                            sb.Append(counterGroup);
                            sb.Append('.');
                        }
                        sb.Append(monitorValueCollection.Counter);
                        if (monitorValueCollection.Unit != null && monitorValueCollection.Unit.Length != 0)
                        {
                            sb.Append(" [");
                            sb.Append(monitorValueCollection.Unit);
                            sb.Append("]");
                        }
                        l.Add(sb.ToString());
                    }
            return l.ToArray();
        }
        /// <summary>
        /// Returns the header of all monitor values.
        /// </summary>
        /// <returns></returns>
        public string GetHeaders(string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string entity in _monitorValues.Keys)
                foreach (string counterGroup in _monitorValues[entity].Keys)
                    foreach (MonitorValueCollection monitorValueCollection in _monitorValues[entity][counterGroup])
                    {
                        sb.Append(";");
                        if (monitorValueCollection.Instance == String.Empty)
                            sb.Append(entity);
                        else
                            sb.Append(entity + "/" + monitorValueCollection.Instance);
                        sb.Append("//");
                        sb.Append(counterGroup);
                        sb.Append('.');
                        sb.Append(monitorValueCollection.Counter);
                        sb.Append(" [");
                        sb.Append(monitorValueCollection.Unit);
                        sb.Append("]");
                    }
            return sb.ToString();
        }
        /// <summary>
        /// Returns all monitor values.
        /// </summary>
        /// <returns></returns>
        public Dictionary<DateTime, float[]> GetMonitorValues()
        {
            return GetMonitorValues(DateTime.MinValue, DateTime.MaxValue);
        }
        /// <summary>
        /// Returns monitor values filtered.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public Dictionary<DateTime, float[]> GetMonitorValues(DateTime from, DateTime to)
        {
            var values = new Dictionary<DateTime, float[]>();
            MonitorValueCollection timeStampMonitorValueCollection = GetTimeStampMonitorValueCollection();

            foreach (MonitorValue timeStampMonitorValue in timeStampMonitorValueCollection)
                if (timeStampMonitorValue.TimeStamp >= from && timeStampMonitorValue.TimeStamp <= to)
                {
                    List<float> l = new List<float>();
                    foreach (string entity in _monitorValues.Keys)
                        foreach (string counterGroup in _monitorValues[entity].Keys)
                            foreach (MonitorValueCollection monitorValueCollection in _monitorValues[entity][counterGroup])
                                foreach (MonitorValue monitorValue in monitorValueCollection)
                                    if (monitorValue.TimeStamp == timeStampMonitorValue.TimeStamp)
                                    {
                                        l.Add(monitorValue.Value);
                                        break;
                                    }
                    values.Add(timeStampMonitorValue.TimeStamp, l.ToArray());
                }

            return values;
        }
        private MonitorValueCollection GetTimeStampMonitorValueCollection()
        {
            foreach (string entity in _monitorValues.Keys)
                foreach (string counterGroup in _monitorValues[entity].Keys)
                    foreach (MonitorValueCollection monitorValueCollection in _monitorValues[entity][counterGroup])
                        // Just pick one, doesn't matter, all timestamps are the same
                        return monitorValueCollection;
            return null;
        }
        /// <summary>
        /// Returns all monitor values.
        /// </summary>
        /// <returns></returns>
        public List<string> GetMonitorValues(string separator)
        {
            return GetMonitorValues(DateTime.MinValue, DateTime.MaxValue, separator);
        }
        /// <summary>
        /// Returns monitor values filtered.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<string> GetMonitorValues(DateTime from, DateTime to, string separator)
        {
            List<string> values = new List<string>();
            MonitorValueCollection timeStampMonitorValueCollection = GetTimeStampMonitorValueCollection();

            foreach (MonitorValue timeStampMonitorValue in timeStampMonitorValueCollection)
                if (timeStampMonitorValue.TimeStamp >= from && timeStampMonitorValue.TimeStamp <= to)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(timeStampMonitorValue.TimeStamp);
                    foreach (string entity in _monitorValues.Keys)
                        foreach (string counterGroup in _monitorValues[entity].Keys)
                            foreach (MonitorValueCollection monitorValueCollection in _monitorValues[entity][counterGroup])
                                foreach (MonitorValue monitorValue in monitorValueCollection)
                                    if (monitorValue.TimeStamp == timeStampMonitorValue.TimeStamp)
                                    {
                                        sb.Append(';');
                                        sb.Append(monitorValue.Value);
                                        break;
                                    }

                    values.Add(sb.ToString());
                }

            return values;
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (btnToggle.Text == "Toggle ( - )")
            {
                btnToggle.Text = "Toggle ( + )";
                Toggle(false);
            }
            else
            {
                btnToggle.Text = "Toggle ( - )";
                Toggle(true);
            }
            this.Cursor = Cursors.Default;
        }
        private void Toggle(bool expand)
        {
            this.SuspendLayout();
            foreach (AccordeonControl accordeonControl in _accordeonControls.Values)
            {
                accordeonControl.Toggled -= accordeonControl_Toggled;
                accordeonControl.Toggle(expand);
                accordeonControl.Toggled += accordeonControl_Toggled;
            }
            this.ResumeLayout();
            SetYPos();
        }
        #endregion
    }
}