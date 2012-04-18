/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vApus.Util
{
    public partial class LogPanel : Panel
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        public event EventHandler<LogErrorCountChangedEventArgs> LogErrorCountChanged;
        private volatile int _logErrorCountCache;
        private System.Timers.Timer tmrFireLogChangedEvent = new System.Timers.Timer(5000);

        private object _lock = new object();

        public LogPanel()
        {
            InitializeComponent();
            this.HandleCreated += new EventHandler(LogPanel_HandleCreated);
            this.VisibleChanged += new EventHandler(LogPanel_VisibleChanged);
            LogWrapper.Default.AfterLogging += new AfterLoggingEventHandler(Default_AfterLogging);

            tmrFireLogChangedEvent.Elapsed += new System.Timers.ElapsedEventHandler(tmrFireLogChangedEvent_Elapsed);
        }

        #region Event Handling
        private void Default_AfterLogging(object source, LogEventArgs e)
        {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate
            {
                SetGui();
            });

            if (e.LogLevel >= LogLevel.Error)
            {
                ++_logErrorCountCache;
                //Fire after 5 seconds.
                tmrFireLogChangedEvent.Stop();
                tmrFireLogChangedEvent.Start();
            }
        }
        private void tmrFireLogChangedEvent_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmrFireLogChangedEvent.Stop();
            int count = _logErrorCountCache;
            _logErrorCountCache = 0;

            if (LogErrorCountChanged != null)
                SynchronizationContextWrapper.SynchronizationContext.Send(delegate
                {
                    LogErrorCountChanged(this, new LogErrorCountChangedEventArgs(count));
                });
        }
        private void LogPanel_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= LogPanel_HandleCreated;
            SetGui();
        }
        private void LogPanel_VisibleChanged(object sender, EventArgs e)
        {
            SetGui();
        }
        #endregion

        #region Get the current log in the gui
        private void SetGui()
        {
            lock (_lock)
                if (this.Visible && this.IsHandleCreated)
                {
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
        /// The current log or the latest if not available.
        /// </summary>
        private void SeCurrentLog()
        {
            FileInfo fi = null;
            if (File.Exists(LogWrapper.Default.Logger.LogFile))
                fi = new FileInfo(LogWrapper.Default.Logger.LogFile);
            else if (Directory.Exists(LogWrapper.Default.Logger.Location))
                foreach (string file in Directory.GetFiles(LogWrapper.Default.Logger.Location))
                {
                    var tempfi = new FileInfo(file);
                    if (fi == null || tempfi.CreationTime > fi.CreationTime)
                        if (IsLog(tempfi.Name))
                        {
                            fi = tempfi;
                            break;
                        }
                }

            if (fi == null)
            {
                llblLatestLog.Text = "<None>";
                llblLatestLog.Tag = null;
            }
            else
            {
                llblLatestLog.Text = fi.Name;
                llblLatestLog.Tag = fi.FullName;
            }
        }
        private bool IsLog(string file)
        {
            if (file.EndsWith(".txt"))
            {
                string[] split = file.Split(' ');
                if (split.Length == 2)
                {
                    DateTime timestamp;
                    return (DateTime.TryParse(split[0], out timestamp) && split[1].StartsWith("PID_"));
                }
            }
            return false;
        }

        private void SetEntries()
        {
            string latestLog = llblLatestLog.Tag as string;
            if (File.Exists(latestLog))
            {
                //Fast read this, if it fails once it is not a problem.
                var lines = new List<string>();
                try
                {
                    LogWrapper.Default.Logger.CloseWriter();
                    using (var sr = new StreamReader(latestLog))
                        while (sr.Peek() != -1)
                            lines.Add(sr.ReadLine());
                }
                catch { }
                finally
                {
                    try
                    {
                        LogWrapper.Default.Logger.OpenOrReOpenWriter();
                    }
                    catch { }
                }

                //Get the key value par controls data
                List<KeyValuePair<string[], string>> linesWithMetaData = ExtractValidLinesWithMetaData(lines);

                //Delete the ones that aren't needed anymore
                int linecount = linesWithMetaData.Count;
                while (linecount < flp.Controls.Count)
                    flp.Controls.RemoveAt(linecount);

                //Recycle the kvp's if needed, otherwise add new
                LockWindowUpdate(this.Handle.ToInt32());
                for (int i = 0; i != linecount; i++)
                {
                    var lineWithMetaData = linesWithMetaData[i];
                    if (i < flp.Controls.Count)
                    {
                        var entrieKVP = flp.Controls[i] as KeyValuePairControl;
                        if (entrieKVP.Key != lineWithMetaData.Key[0])
                        {
                            var newEntrieKVP = GetEntrieKVP(lineWithMetaData.Key[0],
                                 lineWithMetaData.Key[1],
                                 lineWithMetaData.Key[2],
                                 lineWithMetaData.Value);
                            flp.Controls.Add(newEntrieKVP);
                            flp.Controls.SetChildIndex(entrieKVP, i);

                            flp.Controls.Remove(entrieKVP);
                        }
                    }
                    else
                    {
                        try
                        {
                            flp.Controls.Add(GetEntrieKVP(lineWithMetaData.Key[0],
                                lineWithMetaData.Key[1],
                                lineWithMetaData.Key[2],
                                lineWithMetaData.Value));
                        }
                        catch { }
                    }
                    Application.DoEvents();
                }
                SizeKVPs();
                LockWindowUpdate(0);
            }
        }
        private List<KeyValuePair<string[], string>> ExtractValidLinesWithMetaData(List<string> lines)
        {
            var linesWithMetaData = new List<KeyValuePair<string[], string>>();

            foreach (string line in lines)
            {
                string[] entry = line.Split(';');

                if (entry.Length >= 3)
                {
                    DateTime timeStamp;
                    LogLevel logLevel;
                    string message = string.Empty;

                    string[] timeStampSplit = entry[0].Split(',');
                    string dateTimePart = timeStampSplit[0];
                    if (DateTime.TryParse(dateTimePart, out timeStamp) && Enum.TryParse<LogLevel>(entry[1], out logLevel))
                        if ((int)logLevel >= cboLogLevel.SelectedIndex)
                        {
                            if (timeStampSplit.Length > 1)
                            {
                                double ms = 0.0d;
                                if (double.TryParse(timeStampSplit[1], out ms))
                                    timeStamp = timeStamp.AddMilliseconds(ms);
                            }

                            for (int i = 2; i != entry.Length; i++)
                                message += entry[i] + ';';

                            message = message.Substring(0, message.Length - 1);


                            linesWithMetaData.Add(new KeyValuePair<string[], string>(new string[] { timeStamp.ToString("dd/MM/yyyy HH:mm:ss.fff"), logLevel.ToString(), message }, string.Empty));
                            //Continue if valid line
                            continue;
                        }
                }

                if (linesWithMetaData.Count != 0)
                {
                    int index = linesWithMetaData.Count - 1;
                    var kvp = linesWithMetaData[index];
                    kvp = new KeyValuePair<string[], string>(kvp.Key, kvp.Value + '\n' + line);
                    linesWithMetaData[index] = kvp;
                }
            }
            return linesWithMetaData;
        }
        private KeyValuePairControl GetEntrieKVP(string timeStamp, string logLevel, string message, string metaData)
        {
            KeyValuePairControl kvp = new KeyValuePairControl();
            kvp.Key = timeStamp;
            kvp.Tooltip = "Click for details...";
            kvp.Tag = timeStamp + ';' + logLevel + ';' + message + '\n' + metaData;
            kvp.Cursor = Cursors.Hand;
            kvp.Click += new EventHandler(kvp_Click);

            //Stupid user controls.
            foreach (Control ctrl in kvp.Controls)
            {
                ctrl.Click += kvp_Click;
                ctrl.Tag = kvp.Tag;
            }

            kvp.Value = message;
            switch (logLevel)
            {
                case "Info":
                    kvp.BackColor = Color.FromArgb(224, 224, 224);
                    break;
                case "Warning":
                    kvp.BackColor = Color.Yellow;
                    break;
                case "Error":
                    kvp.BackColor = Color.FromArgb(255, 128, 0);
                    break;
                case "Fatal":
                    kvp.BackColor = Color.Red;
                    break;
            }
            return kvp;
        }

        private void kvp_Click(object sender, EventArgs e)
        {
            var kvp = sender as Control;
            LogMessageDialog lmd = new LogMessageDialog();

            if (kvp.BackColor == Color.FromArgb(255, 128, 0) || kvp.BackColor == Color.Red)
                lmd.Title = "You can report this bug, be sure it is not the cause of a configuration problem.";
            else
                lmd.ReportThisBugVisible = false;

            lmd.Text = (sender as Control).Tag as string;
            lmd.StartPosition = FormStartPosition.CenterParent;
            lmd.ShowDialog(this);
        }
        #endregion

        private void cboLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogWrapper.LogLevel = (LogLevel)cboLogLevel.SelectedIndex;
            flp.Controls.Clear();
            SetGui();
        }
        private void cboLogLevel_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;
            Brush brush = null;
            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                brush = Brushes.LightBlue;
            else
                brush = Brushes.SteelBlue;
            e.Graphics.FillRectangle(brush, e.Bounds);

            LogLevel logLevel = (LogLevel)e.Index;
            switch (logLevel)
            {
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
        private void btnWarning_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("You are advised to keep the log level at 'Info' or 'Warning'.\nReset to 'Warning' now?",
                string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                cboLogLevel.SelectedIndex = 1;
        }
        private void llblPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Directory.Exists(llblPath.Text))
                Process.Start(llblPath.Text);
            else
                MessageBox.Show("There is not yet a log folder.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void llblLatestLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string latestLog = llblLatestLog.Tag as string;
            if (File.Exists(latestLog))
                Process.Start(latestLog);
            else
                MessageBox.Show("The file does not exist anymore.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void flp_SizeChanged(object sender, EventArgs e)
        {
            SizeKVPs();
        }
        private void SizeKVPs()
        {
            try
            {
                flp.SuspendLayout();
                foreach (KeyValuePairControl kvp in flp.Controls)
                    kvp.MaximumSize = kvp.MinimumSize = new Size(flp.Width - 24, kvp.Height);
                flp.ResumeLayout(true);
            }
            catch { }
        }
        public override string ToString()
        {
            return "Application Logging";
        }

        public class LogErrorCountChangedEventArgs : EventArgs
        {
            public readonly int LogErrorCount;
            public LogErrorCountChangedEventArgs(int logErrorCount)
            {
                LogErrorCount = logErrorCount;
            }
        }
    }
}
