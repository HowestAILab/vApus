/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace vApus.Util {
    public partial class LogPanel : Panel {
        private readonly object _lock = new object();
        private readonly Timer _tmrFireLogChangedEvent = new Timer(5000);
        private volatile int _logErrorCountCache;

        private const string newLineReplacement = "◦";

        public LogPanel() {
            InitializeComponent();
            HandleCreated += LogPanel_HandleCreated;
            VisibleChanged += LogPanel_VisibleChanged;
            LogWrapper.Default.AfterLogging += Default_AfterLogging;

            _tmrFireLogChangedEvent.Elapsed += tmrFireLogChangedEvent_Elapsed;
        }

        #region Event Handling

        private void Default_AfterLogging(object source, LogEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate { SetGui(); }, null);

            if (e.LogLevel >= LogLevel.Error) {
                ++_logErrorCountCache;
                //Fire after 5 seconds.
                if (_tmrFireLogChangedEvent != null) {
                    _tmrFireLogChangedEvent.Stop();
                    _tmrFireLogChangedEvent.Start();
                }
            }
        }

        private void tmrFireLogChangedEvent_Elapsed(object sender, ElapsedEventArgs e) {
            lock (_lock) {
                if (_tmrFireLogChangedEvent != null) {
                    _tmrFireLogChangedEvent.Stop();
                    int count = _logErrorCountCache;
                    _logErrorCountCache = 0;

                    if (LogErrorCountChanged != null)
                        SynchronizationContextWrapper.SynchronizationContext.Send(
                            delegate { LogErrorCountChanged(this, new LogErrorCountChangedEventArgs(count)); }, null);
                }
            }
        }

        private void LogPanel_HandleCreated(object sender, EventArgs e) {
            HandleCreated -= LogPanel_HandleCreated;
            SetGui();
        }

        private void LogPanel_VisibleChanged(object sender, EventArgs e) {
            SetGui();
        }

        #endregion

        #region Get the current log in the gui

        private void SetGui() {
            lock (_lock)
                if (Visible && IsHandleCreated) {
                    cboLogLevel.SelectedIndexChanged -= cboLogLevel_SelectedIndexChanged;
                    llblPath.Text = Logger.DEFAULT_LOCATION;
                    cboLogLevel.SelectedIndex = (int)LogWrapper.LogLevel;
                    btnWarning.Visible = cboLogLevel.SelectedIndex > 1;

                    SeCurrentLog();
                    SetEntries();
                    cboLogLevel.SelectedIndexChanged += cboLogLevel_SelectedIndexChanged;
                }
        }

        /// <summary>
        ///     The current log or the latest if not available.
        /// </summary>
        private void SeCurrentLog() {
            FileInfo fi = null;
            if (File.Exists(LogWrapper.Default.Logger.LogFile))
                fi = new FileInfo(LogWrapper.Default.Logger.LogFile);
            else if (Directory.Exists(LogWrapper.Default.Logger.Location))
                foreach (string file in Directory.GetFiles(LogWrapper.Default.Logger.Location)) {
                    var tempfi = new FileInfo(file);
                    if (fi == null || tempfi.CreationTime > fi.CreationTime)
                        if (IsLog(tempfi.Name)) {
                            fi = tempfi;
                            break;
                        }
                }

            if (fi == null) {
                llblLatestLog.Text = "<None>";
                llblLatestLog.Tag = null;
            } else {
                llblLatestLog.Text = fi.Name;
                llblLatestLog.Tag = fi.FullName;
            }
        }

        private bool IsLog(string file) {
            if (file.EndsWith(".txt")) {
                string[] split = file.Split(' ');
                if (split.Length == 2) {
                    DateTime timestamp;
                    return (DateTime.TryParse(split[0], out timestamp) && split[1].StartsWith("PID_"));
                }
            }
            return false;
        }

        private void SetEntries() {
            var latestLog = llblLatestLog.Tag as string;
            latestLog = @"C:\Users\Didjeeh\Desktop\vapus\Build\Logs\2013-03-13 PID_444.txt";
            if (File.Exists(latestLog)) {
                //Fast read this, if it fails once it is not a problem.
                var lines = new List<string>();
                try {
                    LogWrapper.Default.Logger.CloseWriter();
                    using (var sr = new StreamReader(latestLog))
                        while (sr.Peek() != -1) {
                            string line = sr.ReadLine();
                            if (!string.IsNullOrWhiteSpace(line))
                                lines.Add(line);
                        }
                } catch {
                } finally {
                    try {
                        LogWrapper.Default.Logger.OpenOrReOpenWriter();
                    } catch {
                    }
                }

                dgv.DataSource = null;

                var dt = new DataTable("log");
                dt.Columns.AddRange("Timestamp", "Type", "Text");
                AddLinesToDataTable(lines, dt);

                dgv.DataSource = dt;

                LockWindowUpdate(0);
            }
        }

        private void AddLinesToDataTable(List<string> lines, DataTable dt) {
            foreach (string line in lines) {
                string[] entry = line.Split(';');

                if (entry.Length >= 3) {
                    DateTime timeStamp;
                    LogLevel logLevel;
                    string message = string.Empty;

                    string[] timeStampSplit = entry[0].Split(',');
                    string dateTimePart = timeStampSplit[0];
                    if (DateTime.TryParse(dateTimePart, out timeStamp) && Enum.TryParse(entry[1], out logLevel))
                        if ((int)logLevel >= cboLogLevel.SelectedIndex) {
                            if (timeStampSplit.Length > 1) {
                                double ms = 0.0d;
                                if (double.TryParse(timeStampSplit[1], out ms))
                                    timeStamp = timeStamp.AddMilliseconds(ms);
                            }

                            for (int i = 2; i != entry.Length; i++)
                                message += entry[i] + ';';

                            message = message.Substring(0, message.Length - 1);


                            dt.Rows.Add(timeStamp.ToString("yyyy'-'MM'-'dd HH':'mm':'ss'.'fff"), logLevel.ToString(), message);
                            //Continue if valid line
                            continue;
                        }
                }

                if (dt.Rows.Count != 0) {
                    int index = dt.Rows.Count - 1;
                    object[] row = dt.Rows[index].ItemArray;

                    dt.Rows[index].ItemArray = new object[] { row[0], row[1], string.Concat(row[2], newLineReplacement, line) };
                }
            }
        }
        #endregion

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler<LogErrorCountChangedEventArgs> LogErrorCountChanged;

        private void cboLogLevel_SelectedIndexChanged(object sender, EventArgs e) {
            LogWrapper.LogLevel = (LogLevel)cboLogLevel.SelectedIndex;
            dgv.DataSource = null;
            SetGui();
        }

        private void cboLogLevel_DrawItem(object sender, DrawItemEventArgs e) {
            if (e.Index == -1)
                return;
            Brush brush = null;
            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                brush = Brushes.LightBlue;
            else
                brush = Brushes.SteelBlue;
            e.Graphics.FillRectangle(brush, e.Bounds);

            var logLevel = (LogLevel)e.Index;
            switch (logLevel) {
                case LogLevel.Info:
                    brush = new SolidBrush(Color.FromArgb(224, 224, 224));
                    break;
                case LogLevel.Warning:
                    brush = Brushes.Yellow;
                    break;
                case LogLevel.Error:
                    brush = new SolidBrush(Color.FromArgb(255, 128, 0));
                    break;
                case LogLevel.Fatal:
                    brush = Brushes.Red;
                    break;
            }
            e.Graphics.DrawString(cboLogLevel.Items[e.Index].ToString(), cboLogLevel.Font, brush, e.Bounds);
        }

        private void btnWarning_Click(object sender, EventArgs e) {
            if (MessageBox.Show(
                "You are advised to keep the log level at 'Info' or 'Warning'.\nReset to 'Warning' now?",
                string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) ==
                DialogResult.Yes)
                cboLogLevel.SelectedIndex = 1;
        }

        private void llblPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (Directory.Exists(llblPath.Text))
                Process.Start(llblPath.Text);
            else
                MessageBox.Show("There is not yet a log folder.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void llblLatestLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var latestLog = llblLatestLog.Tag as string;
            if (File.Exists(latestLog))
                Process.Start(latestLog);
            else
                MessageBox.Show("The file does not exist anymore.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public override string ToString() {
            return "Application Logging";
        }

        public class LogErrorCountChangedEventArgs : EventArgs {
            public readonly int LogErrorCount;

            public LogErrorCountChangedEventArgs(int logErrorCount) {
                LogErrorCount = logErrorCount;
            }
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            var row = (dgv.DataSource as DataTable).Rows[e.RowIndex];
            var lmd = new LogMessageDialog();

            string type = (row.ItemArray[1] as string).ToLower();
            if (type != "info")
                lmd.Title = "You can report this bug, be sure it is not because of a configuration problem.";
            else
                lmd.ReportThisBugVisible = false;


            lmd.Text = row.ItemArray.Combine("; ").Replace(newLineReplacement, Environment.NewLine);
            lmd.StartPosition = FormStartPosition.CenterParent;
            lmd.ShowDialog(this);
        }

        private void dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            try {
                var cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (string.IsNullOrEmpty(cell.ToolTipText))
                    cell.ToolTipText = "Double-click for details...";
            } catch { }
        }
    }
}