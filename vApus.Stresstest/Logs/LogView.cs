/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest
{
    public partial class LogView : BaseSolutionComponentView
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LockWindowUpdate(int hWnd);

        #region Fields
        private Log _log;
        private static string _multilineComment = string.Empty;
        private const string VBLRn = "<16 0C 02 12n>";
        private const string VBLRr = "<16 0C 02 12r>";
        #endregion

        #region Constructors
        public LogView()
        {
            InitializeComponent();
        }
        public LogView(SolutionComponent solutionComponent, params object[] args)
            : base(solutionComponent, args)
        {
            InitializeComponent();
            _log = solutionComponent as Log;
            if (this.IsHandleCreated)
                Init();
            else
                this.HandleCreated += new EventHandler(LogView_HandleCreated);
        }
        #endregion

        #region Functions
        private void LogView_HandleCreated(object sender, EventArgs e)
        {
            HandleCreated -= LogView_HandleCreated;
            Init();
            _log.LogRuleSet.LogRuleSetChanged += new EventHandler(LogRuleSet_LogRuleSetChanged);
        }
        private void Init()
        {
            SetGui();
            FillLargeList();
        }
        private void LogRuleSet_LogRuleSetChanged(object sender, EventArgs e)
        {
            if (_log != null && this.IsHandleCreated)
            {
                ApplyLogRuleSet(_log);
                ValidateSelectedControls();
                SetCollapseUncollapseButton();
            }
        }
        private void chk_CheckStateChanged(object sender, EventArgs e)
        {
            if (chk.CheckState == CheckState.Checked)
            {
                largelist.SelectAll();
                foreach (Control control in largelist.Selection)
                {
                    LogChildControlBase logChildControlBase = control as LogChildControlBase;
                    logChildControlBase.CheckedChanged -= logChildControlBase_CheckedChanged;
                    logChildControlBase.Checked = true;
                    logChildControlBase.CheckedChanged += logChildControlBase_CheckedChanged;
                }
            }
            else if (chk.CheckState == CheckState.Unchecked)
            {
                foreach (Control control in largelist.Selection)
                {
                    LogChildControlBase logChildControlBase = control as LogChildControlBase;
                    logChildControlBase.CheckedChanged -= logChildControlBase_CheckedChanged;
                    logChildControlBase.Checked = false;
                    logChildControlBase.CheckedChanged += logChildControlBase_CheckedChanged;
                }
                largelist.ClearSelection();
            }
            ValidateSelectedControls();
            lblCount.Text = GetSelectionDeepCount() + " [" + GetDeepCount() + "]";
        }
        /// <summary>
        /// Get the count of all log entries, useractions and the log entries in the user actions in the selection.
        /// </summary>
        /// <returns></returns>
        private int GetSelectionDeepCount()
        {
            int count = 0;
            foreach (Control ctrl in largelist.Selection)
                if (ctrl is UserActionControl)
                {
                    count += 1 + (ctrl as UserActionControl).LogEntryControls.Count;
                }
                else
                {
                    var lec = ctrl as LogEntryControl;
                    var uac = lec.UserActionControl;
                    if (uac == null || uac.CheckState != CheckState.Checked)
                        ++count;
                }
            return count;
        }
        /// <summary>
        /// Get the count of all log entries, useractions and the log entries in the user actions.
        /// </summary>
        /// <returns></returns>
        private int GetDeepCount()
        {
            int count = 0;
            if (_log != null)
            {
                count = _log.Count;
                foreach (var item in _log)
                    if (item is UserAction)
                        count += item.Count;
            }
            return count;
        }
        private void logChildControlBase_CheckedChanged(object sender, EventArgs e)
        {
            largelist.Select((sender as LogChildControlBase), Hotkeys.Ctrl);
            ValidateSelectedControls();
        }
        private void logEntryControl_Removed(object sender, EventArgs e)
        {
            LogEntryControl logEntryControl = sender as LogEntryControl;
            if (logEntryControl.UserActionControl != null)
                logEntryControl.UserActionControl.LogEntryControls.Remove(logEntryControl);
            largelist.Remove(logEntryControl);

            ApplyChanges();
        }
        public override void Refresh()
        {
            base.Refresh();
            SetGui();
            logSolutionComponentPropertyPanel.Refresh();
        }
        private void SetGui()
        {
            logSolutionComponentPropertyPanel.SolutionComponent = SolutionComponent;

            if (_log.LogRuleSet.IsEmpty)
            {
                toolStripImport.Enabled = false;
                toolStripEdit.Enabled = false;
                largelist.Enabled = false;
            }
            else
            {
                toolStripImport.Enabled = true;
                toolStripEdit.Enabled = true;
                largelist.Enabled = true;
            }
        }
        private void ApplyLogRuleSet(BaseItem item)
        {
            //Will clear the error selector, eventing will make sure new errors are added tot the selector if needed.
            if (item is Log)
                errorAndFindSelector.ClearErrors();
            if (item is LogEntry)
                (item as LogEntry).ApplyLogRuleSet();
            foreach (BaseItem child in item)
                ApplyLogRuleSet(child);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            lblCount.Text = GetSelectionDeepCount() + " [" + GetDeepCount() + "]";
        }

        #region Load
        private void FillLargeList()
        {
            largelist.Clear();
            foreach (BaseItem item in _log)
                FillLargeList(item);
            ApplyLogRuleSet(_log);
            if (largelist.IsHandleCreated)
                SetCollapseUncollapseButton();
            else
            {
                largelist.HandleCreated += new EventHandler(largelist_HandleCreated);
                largelist.ControlCollectionChanged += new EventHandler(largelist_ControlCollectionChanged);
            }
        }
        private void largelist_HandleCreated(object sender, EventArgs e)
        {
            largelist.HandleCreated -= largelist_HandleCreated;
            SetCollapseUncollapseButton();
        }
        private void largelist_ControlCollectionChanged(object sender, EventArgs e)
        {
            largelist.ControlCollectionChanged -= largelist_ControlCollectionChanged;
            SetCollapseUncollapseButton();
        }
        private void FillLargeList(BaseItem item, UserActionControl userActionControl = null)
        {
            if (item is LogEntry)
            {
                LogEntryControl logEntryControl = new LogEntryControl(item as LogEntry);
                logEntryControl.CheckedChanged += new EventHandler(logChildControlBase_CheckedChanged);
                logEntryControl.Removed += new EventHandler(logEntryControl_Removed);
                if (item.Parent is UserAction)
                {
                    logEntryControl.SetUserActionControl(userActionControl);
                    userActionControl.LogEntryControls.Add(logEntryControl);
                    if (!userActionControl.Collapsed)
                        largelist.Add(logEntryControl);
                }
                else
                {
                    largelist.Add(logEntryControl);
                }
                logEntryControl.LexicalResultChanged += new EventHandler(logEntryControl_LexicalResultChanged);
            }
            else if (item is UserAction)
            {
                userActionControl = new UserActionControl(item as UserAction);
                userActionControl.CheckedChanged += new EventHandler(logChildControlBase_CheckedChanged);
                userActionControl.CollapsedChanged += new EventHandler(userActionControl_CollapsedChanged);
                largelist.Add(userActionControl);
            }
            foreach (BaseItem childItem in item)
                FillLargeList(childItem, userActionControl);
        }
        private void logEntryControl_LexicalResultChanged(object sender, EventArgs e)
        {
            LogEntryControl logEntryControl = sender as LogEntryControl;
            if (logEntryControl.LexicalResult == LexicalResult.Error && largelist.Contains(logEntryControl))
                errorAndFindSelector.AddError(logEntryControl);
            else
                errorAndFindSelector.RemoveError(logEntryControl);
        }
        #endregion

        #region Import
        private void btnImportLogFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
                ImportLogFiles(ofd.FileNames);
        }
        private void ImportLogFiles(params string[] fileNames)
        {
            foreach (string fileName in fileNames)
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string logFile = sr.ReadToEnd();
                    UserAction userAction = null;
                    if (_log.LogRuleSet.ActionizeOnFile)
                    {
                        userAction = new UserAction(fileName);
                        _log.AddWithoutInvokingEvent(userAction, false);
                    }
                    logFile = logFile.Replace(Environment.NewLine, "\n").Replace('\r', '\n');
                    foreach (string line in logFile.Split('\n'))
                    {
                        if (line.Trim().Length == 0)
                            continue;

                        string output;
                        if (DetermineComment(line, out output))
                        {
                            if (_log.LogRuleSet.ActionizeOnComment)
                            {
                                userAction = new UserAction(output);
                                _log.AddWithoutInvokingEvent(userAction, false);
                            }
                        }
                        else if (userAction == null)
                        {
                            LogEntry logEntry = new LogEntry(output.Replace(VBLRn, "\n").Replace(VBLRr, "\r"));
                            _log.AddWithoutInvokingEvent(logEntry, false);
                        }
                        else
                        {
                            LogEntry logEntry = new LogEntry(output.Replace(VBLRn, "\n").Replace(VBLRr, "\r"));
                            userAction.AddWithoutInvokingEvent(logEntry, false);
                        }
                    }
                }

            RemoveEmptyUserActions();

            //#if EnableBetaFeature
            //            bool successfullyParallized = SetParallelExecutions();
            //#else
#warning Parallel executions temp not available
            bool successfullyParallized = true;
            //#endif
            SetIgnoreDelays();
            FillLargeList();

            if (!successfullyParallized)
            {
                string message = this.Text + ": Could not determine the begin- and end timestamps for one or more log entries in the different user actions, are they correctly formatted?";
                LogWrapper.LogByLevel(message, LogLevel.Error);
            }

            _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns>True if is comment</returns>
        private bool DetermineComment(string input, out string output)
        {
            int singleline = -1;
            int multiline = -1;

            input = input.TrimStart().Trim();
            output = input;

            if (_multilineComment.Length > 0)
            {
                multiline = input.IndexOf(_log.LogRuleSet.EndCommentString);
                if (multiline > -1)
                {
                    output = input.Substring(multiline + _log.LogRuleSet.EndCommentString.Length);
                    if (output.Length == 0)
                    {
                        if (input.Length > _log.LogRuleSet.EndCommentString.Length + 1)
                            output = FormatComment(_multilineComment.TrimStart() + ' ' + input.Substring(0, input.Length - _log.LogRuleSet.EndCommentString.Length));
                        else
                            output = FormatComment(_multilineComment.TrimStart());
                        _multilineComment = string.Empty;
                        return true;
                    }
                    _multilineComment = string.Empty;
                    output = input.Substring(multiline + _log.LogRuleSet.EndCommentString.Length);
                    return true;
                }
                if (input.Length > 0)
                    _multilineComment = _multilineComment + ' ' + input;
                output = string.Empty;
                return true;
            }

            if (_log.LogRuleSet.SingleLineCommentString.Length > 0)
                singleline = input.IndexOf(_log.LogRuleSet.SingleLineCommentString);
            if (_log.LogRuleSet.BeginCommentString.Length > 0)
                multiline = input.IndexOf(_log.LogRuleSet.BeginCommentString);

            if (singleline > -1 && multiline == -1)
                multiline = int.MaxValue;
            else if (multiline > -1 && singleline == -1)
                singleline = int.MaxValue;

            if (singleline > -1 && singleline < multiline)
            {
                _multilineComment = string.Empty;
                if (singleline == 0)
                    output = FormatComment(input.Substring(_log.LogRuleSet.SingleLineCommentString.Length));
                return true;
            }
            else if (multiline > -1 && multiline < singleline)
            {
                int multilineCopy = input.IndexOf(_log.LogRuleSet.EndCommentString);
                if (multilineCopy > -1 && multilineCopy > multiline)
                {
                    _multilineComment = string.Empty;
                    output = input.TrimStart().Substring(0, multiline) + input.Substring(multilineCopy + _log.LogRuleSet.EndCommentString.Length);
                    if (output.Length == 0)
                        output = FormatComment(input.Substring(multiline + _log.LogRuleSet.BeginCommentString.Length, input.Length - _log.LogRuleSet.EndCommentString.Length - _log.LogRuleSet.BeginCommentString.Length));
                    return true;
                }
                else
                {
                    _multilineComment = input.Substring(multiline + _log.LogRuleSet.BeginCommentString.Length);
                    if (_multilineComment.Length == 0)
                        _multilineComment = " ";
                    output = input.Substring(0, multiline);
                    return true;
                }
            }
            return false;
        }
        private string FormatComment(string input)
        {
            int i = 0;
            input = input.TrimStart();
            StringBuilder sb = new StringBuilder(255);
            foreach (char c in input)
            {
                sb.Append(c);
                if (++i == 255)
                    break;
            }
            return sb.ToString();
        }
        private void RemoveEmptyUserActions()
        {
            List<BaseItem> emptyUserActions = new List<BaseItem>(_log.Count);
            foreach (BaseItem item in _log)
                if (item is UserAction && item.Count == 0)
                    emptyUserActions.Add(item);

            foreach (BaseItem item in emptyUserActions)
                _log.RemoveWithoutInvokingEvent(item);
        }
        /// <summary>
        /// Must be called before SetIgnoreDelays()
        /// </summary>
        /// <returns>false on error</returns>
        private bool SetParallelExecutions()
        {
            if (CanSetParallelExecutions())
            {
                //Make one based zero based
                int beginTimestampIndex = (int)_log.LogRuleSet.BeginTimestampIndex - 1;
                int endTimestampIndex = (int)_log.LogRuleSet.EndTimestampIndex - 1;

                //Apply the rule set and get the token delimiters to be able to get the parameterized structure
                //for each log entry in a user action.
                //This way the time stamps can be extracted.
                _log.ApplyLogRuleSet();

                string b, e;
                bool error, warning;
                _log.GetUniqueParameterTokenDelimiters(out b, out e, out error, out warning);
                //I presume there are no errors, there are 14348907 possible combinations of delimiters
                foreach (BaseItem item in _log)
                    if (item is UserAction)
                    {
                        UserAction userAction = item as UserAction;
                        //Only usable when there is more then one loge entry.
                        if (userAction.Count > 1)
                        {
                            //Get the first ones
                            DateTime previousBeginTimestamp, previousEndTimestamp;
                            if (!GetTimestamps(userAction[0] as LogEntry, b, e, beginTimestampIndex, endTimestampIndex,
                                out previousBeginTimestamp, out previousEndTimestamp))
                                return false;

                            for (int i = 1; i < userAction.Count; i++)
                            {
                                DateTime beginTimestamp, endTimestamp;
                                LogEntry logEntry = userAction[i] as LogEntry;
                                if (GetTimestamps(logEntry, b, e, beginTimestampIndex, endTimestampIndex,
                                    out beginTimestamp, out endTimestamp))
                                {
                                    //This is enough to determine the window, as easy as that.
                                    if (beginTimestamp > previousBeginTimestamp && beginTimestamp < previousEndTimestamp)
                                    {
                                        int offset = (int)(beginTimestamp - previousBeginTimestamp).TotalMilliseconds;
                                        if (offset > -1)
                                        {
                                            logEntry.ExecuteInParallelWithPrevious = true;
                                            logEntry.ParallelOffsetInMs = offset;
                                        }
                                    }

                                    //Replace the previous
                                    previousBeginTimestamp = beginTimestamp;
                                    previousEndTimestamp = endTimestamp;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
            }
            return true;
        }
        private bool CanSetParallelExecutions()
        {
            uint beginTimestampIndex = _log.LogRuleSet.BeginTimestampIndex;
            uint endTimestampIndex = _log.LogRuleSet.EndTimestampIndex;
            if (beginTimestampIndex >= endTimestampIndex)
                return false;
            if (beginTimestampIndex == 0 || endTimestampIndex == 0)
                return false;
            else if (beginTimestampIndex > _log.LogRuleSet.Count || endTimestampIndex > _log.LogRuleSet.Count)
                return false;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logEntry"></param>
        /// <param name="beginTimestamp">DateTime.MinValue on parse error</param>
        /// <param name="endTimestamp">DateTime.MinValue on parse error</param>
        /// <returns>false on parse error</returns>
        private bool GetTimestamps(LogEntry logEntry, string beginTokenDelimiter, string endTokenDelimiter, int beginTimestampIndex, int endTimestampIndex, out DateTime beginTimestamp, out DateTime endTimestamp)
        {
            beginTimestamp = endTimestamp = DateTime.MinValue;

            //Catch if no timestamps are available
            try
            {
                //We need to have a StringTree for the log entrym we can get that calling GetParameterizedStructure.
                var parameterizedLogEntry = logEntry.GetParameterizedStructure(beginTokenDelimiter, endTokenDelimiter, new HashSet<BaseParameter>());
                string begin = null, end = null;

                if (endTimestampIndex < parameterizedLogEntry.Count)
                {
                    begin = parameterizedLogEntry[beginTimestampIndex].Value;
                    end = parameterizedLogEntry[endTimestampIndex].Value;

                    if (DateTime.TryParse(begin, out beginTimestamp))
                        DateTime.TryParse(end, out endTimestamp);
                }
            }
            catch { }
            return beginTimestamp != DateTime.MinValue;
        }

        private void SetIgnoreDelays()
        {
            foreach (BaseItem item in _log)
                if (item is UserAction)
                {
                    UserAction userAction = item as UserAction;
                    //Determine the non parallel log entries, set ignore delay for the other ones (must always be ignored for these)
                    List<LogEntry> nonParallelLogEntries = new List<LogEntry>();
                    foreach (LogEntry entry in userAction)
                        if (entry.ExecuteInParallelWithPrevious)
                            entry.IgnoreDelay = true;
                        else
                            nonParallelLogEntries.Add(entry);

                    //Then set ignore delays for all but the last
                    for (int i = 0; i < nonParallelLogEntries.Count - 1; i++)
                        nonParallelLogEntries[i].IgnoreDelay = true;
                }
        }
        #endregion

        private void btnExportToTextFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt";

            bool hasCommentDelimiters =
                _log.LogRuleSet.BeginCommentString.Length != 0 &&
                _log.LogRuleSet.EndCommentString.Length != 0;

            if (!hasCommentDelimiters && MessageBox.Show("The log rule set has no begin- and/ or end comment string. These are used to add the user action labels in the log.\nDo you want to continue? User action labels will be discarded!", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;

            if (sfd.ShowDialog() == DialogResult.OK)
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {

                    StringBuilder sb = new StringBuilder();
                    foreach (BaseItem item in _log)
                    {
                        if (item is UserAction)
                        {
                            if (hasCommentDelimiters)
                            {
                                sb.Append(_log.LogRuleSet.BeginCommentString);
                                sb.Append((item as UserAction).Label);
                                sb.AppendLine(_log.LogRuleSet.EndCommentString);
                            }
                            foreach (LogEntry logEntry in item)
                                sb.AppendLine(logEntry.LogEntryString.Replace(VBLRn, "\n").Replace(VBLRr, "\r"));
                        }
                        else
                        {
                            sb.AppendLine((item as LogEntry).LogEntryString.Replace(VBLRn, "\n").Replace(VBLRr, "\r"));
                        }
                    }

                    string toWrite = sb.ToString();
                    toWrite = toWrite.Length == 0 ? toWrite : toWrite.Substring(0, toWrite.Length - 1);

                    sw.Write(toWrite);
                    sw.Flush();
                }
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            string jarPath = Path.Combine(Application.StartupPath, "Lupus-Proxy.jar");
            string config = "--vapus -slupusProxyLog -d\"" + _log.LogRuleSet.ChildDelimiter + "\"";

            if (_log.RecordIps.Length != 0)
                config += " -f\"" + _log.RecordIps.Combine(",");
            if (_log.RecordPorts.Length != 0)
                config += "\" -p\"" + _log.RecordPorts.Combine(",") + "\"";

            if (File.Exists(jarPath))
            {
                ProxyHelper.SetProxy("127.0.0.1:5555");

                string logPath = Path.Combine(Application.StartupPath, "lupusProxyLog");
                Process lupusProcess = Process.Start("\"" + jarPath + "\"", config);
                lupusProcess.WaitForExit();

                ProxyHelper.UnsetProxy();

                if (!File.Exists(logPath))
                    return;

                bool aoc = _log.LogRuleSet.ActionizeOnComment;
                bool aof = _log.LogRuleSet.ActionizeOnFile;

                _log.LogRuleSet.ActionizeOnComment = true;
                _log.LogRuleSet.ActionizeOnFile = false;

                int i = 0;
            Retry:
                try
                {
                    ImportLogFiles(logPath);
                }
                catch (Exception ex)
                {
                    if (i++ != 10)
                    {
                        Thread.Sleep(500);
                        goto Retry;
                    }

                    string message = this.Text + ": Could not import " + logPath + ".\n" + ex.ToString();
                    LogWrapper.LogByLevel(message, LogLevel.Error);
                }

                try
                {
                    File.Delete(logPath);
                }
                catch { }

                _log.LogRuleSet.ActionizeOnComment = aoc;
                _log.LogRuleSet.ActionizeOnFile = aof;

                string[] recordIps;
                int[] recordPorts;
                GetConfig(out recordIps, out recordPorts);

                _log.RecordIps = recordIps;
                _log.RecordPorts = recordPorts;
            }
            else
            {
                LogWrapper.LogByLevel("Could not find Lupus-Proxy.jar!", LogLevel.Error);
            }
        }
        private void GetConfig(out string[] recordIps, out int[] recordPorts)
        {
            recordIps = new string[] { };
            recordPorts = new int[] { };

            string configPath = Path.Combine(Application.StartupPath, "config.ini");
            if (File.Exists(configPath))
            {
                var sr = new StreamReader(configPath);
                while (sr.Peek() != -1)
                {
                    string line = sr.ReadLine();
                    if (line.TrimStart().StartsWith("fixatedIpsAndHosts"))
                    {
                        recordIps = line.Substring(line.IndexOf('=') + 1).Trim().Split(',');
                    }
                    else if (line.TrimStart().StartsWith("fixatedPorts"))
                    {
                        var ports = line.Substring(line.IndexOf('=') + 1).Trim().Split(',');
                        var l = new List<int>();
                        for (int i = 0; i != ports.Length; i++)
                        {
                            int port;
                            if (int.TryParse(ports[i], out port))
                                l.Add(port);
                        }
                        recordPorts = l.ToArray();
                    }
                }
            }
        }

        private void btnBulkEditLog_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            BulkEditLog bulkEditLog = new BulkEditLog(_log);
            if (bulkEditLog.ShowDialog() == DialogResult.OK)
            {
                _log.ClearWithoutInvokingEvent();
                _log.AddRangeWithoutInvokingEvent(bulkEditLog.Log);

                FillLargeList();

                _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            }

            this.Cursor = Cursors.Default;
        }

        private void btnRedetermineTokens_Click(object sender, EventArgs e)
        {
            RedetermineTokens redetermineTokens = new RedetermineTokens(_log);
            redetermineTokens.ShowDialog();
        }

        #region Toolbar

        #region General Stuff
        private void ApplyChanges()
        {
            this.Cursor = Cursors.WaitCursor;
            _log.ClearWithoutInvokingEvent(false);
            for (int i = 0; i < largelist.ViewCount; i++)
                for (int j = 0; j < largelist[i].Count; j++)
                {
                    LogChildControlBase control = largelist[i][j] as LogChildControlBase;
                    control.ClearLogChild();

                    if (control.IndentationLevel == 0)
                    {
                        _log.AddWithoutInvokingEvent(control.LogChild, false);
                        if (control is UserActionControl)
                            foreach (LogEntryControl logEntryControl in (control as UserActionControl).LogEntryControls)
                                control.LogChild.AddWithoutInvokingEvent(logEntryControl.LogChild, false);
                    }
                }
            ApplyLogRuleSet(_log);
            _log.InvokeSolutionComponentChangedEvent(SolutionComponentChangedEventArgs.DoneAction.Edited);
            ValidateSelectedControls();
            SetCollapseUncollapseButton();
            this.Cursor = Cursors.Default;
        }
        private void ValidateSelectedControls()
        {
            this.Cursor = Cursors.WaitCursor;
            LogEntryControl logEntryControl;
            UserActionControl userActionControl;

            btnLevelDown.Enabled = false;
            btnLevelUp.Enabled = false;
            btnUp.Enabled = false;
            btnDown.Enabled = false;
            btnActionizeUnactionize.Enabled = false;
            btnRemove.Enabled = false;

            logEntryControl = largelist.LastClickedControl as LogEntryControl;
            userActionControl = largelist.LastClickedControl as UserActionControl;
            if (logEntryControl != null && logEntryControl.UserActionControl != null)
                CheckParent(logEntryControl.UserActionControl);
            else if (userActionControl != null)
                CheckChilds(userActionControl);

            if (largelist.ActiveControl != null && largelist.Selection.Count > 0)
            {
                btnLevelDown.Enabled = true;
                btnLevelUp.Enabled = true;
                btnUp.Enabled = true;
                btnDown.Enabled = true;
                btnActionizeUnactionize.Enabled = true;
                btnRemove.Enabled = true;

                largelist.OrderSelection();
                largelist.ScrollIntoView(largelist.ActiveControl);

                bool selectionHasHoles = SelectionHasHoles();
                bool selectionHasAction = SelectionHasAction();
                LogChildControlBase beginControl = largelist.Selection[0] as LogChildControlBase;
                userActionControl = beginControl as UserActionControl;
                logEntryControl = beginControl as LogEntryControl;

                if (largelist.BeginOfSelection.Key == 0 && largelist.BeginOfSelection.Value == 0)
                {
                    btnLevelUp.Enabled = false;
                    btnLevelDown.Enabled = false;
                    btnUp.Enabled = false;
                }
                else if (largelist.BeginOfSelection.Key > 0 || (largelist.BeginOfSelection.Key == 0 && largelist.BeginOfSelection.Value > 0))
                {
                    if (userActionControl != null)
                    {
                        btnLevelUp.Enabled = false;
                        btnLevelDown.Enabled = false;
                    }
                    else if (logEntryControl != null)
                    {
                        if (logEntryControl.UserActionControl == null)
                        {
                            LogEntryControl previousLogEntryControl = PreviousControl() as LogEntryControl;
                            bool previousControlIsChild = previousLogEntryControl == null ? false : previousLogEntryControl.UserActionControl != null;

                            if (!previousControlIsChild)
                                btnLevelUp.Enabled = false;
                            btnLevelDown.Enabled = false;
                        }
                        else
                        {
                            LogEntryControl nextLogEntryControl = GetClosestNextSibling(logEntryControl) as LogEntryControl;

                            btnLevelUp.Enabled = false;
                            if (nextLogEntryControl != null && nextLogEntryControl.Parent == logEntryControl.Parent)
                                btnLevelDown.Enabled = false;
                        }
                    }

                    if (GetClosestNextSibling(beginControl) == null)
                        btnDown.Enabled = false;
                    if (GetClosestPreviousSibling(largelist.Selection[0]) == null)
                    {
                        btnUp.Enabled = false;
                        if (logEntryControl != null && logEntryControl.UserActionControl != null && logEntryControl.UserActionControl.LogChild.Count < 2)
                            btnRemove.Enabled = false;
                    }
                }

                if (largelist.EndOfSelection.Key == largelist.ViewCount - 1 && largelist.EndOfSelection.Value == largelist[largelist.EndOfSelection.Key].Count - 1)
                    btnDown.Enabled = false;

                //Actionize
                if (userActionControl != null)
                {
                    btnActionizeUnactionize.Image = global::vApus.Stresstest.Properties.Resources.Unactionize;
                    btnActionizeUnactionize.Text = "Unactionize Selected Log Entries";
                    btnActionizeUnactionize.ToolTipText = btnActionizeUnactionize.Text;
                }
                else if (logEntryControl.UserActionControl == null && !selectionHasAction && !selectionHasHoles)
                {
                    btnActionizeUnactionize.Image = global::vApus.Stresstest.Properties.Resources.Actionize;
                    btnActionizeUnactionize.Text = "Actionize Selected Log Entries";
                    btnActionizeUnactionize.ToolTipText = btnActionizeUnactionize.Text;
                }
                else
                {
                    btnActionizeUnactionize.Enabled = false;
                }
            }

            int selectionDeepCount = GetSelectionDeepCount();
            int deepCount = GetDeepCount();
            chk.CheckStateChanged -= chk_CheckStateChanged;
            if (selectionDeepCount == deepCount && deepCount != 0)
                chk.CheckState = CheckState.Checked;
            else if (selectionDeepCount == 0)
                chk.CheckState = CheckState.Unchecked;
            else
                chk.CheckState = CheckState.Indeterminate;
            chk.CheckStateChanged += chk_CheckStateChanged;
            lblCount.Text = selectionDeepCount + " [" + deepCount + "]";

            if (logEntryControl != null)
                logEntryControl.Select();
            this.Cursor = Cursors.Default;
        }

        private bool SelectionHasHoles()
        {
            if (largelist.Selection.Count > 0)
                if (largelist.BeginOfSelection.Key == largelist.EndOfSelection.Key)
                {
                    for (int j = largelist.BeginOfSelection.Value; j <= largelist.EndOfSelection.Value; j++)
                        if (!largelist.SelectionContains(largelist[largelist.BeginOfSelection.Key][j]))
                            return true;
                }
                else
                {
                    for (int i = largelist.BeginOfSelection.Key; i <= largelist.EndOfSelection.Key; i++)
                        if (i == largelist.BeginOfSelection.Key)
                        {
                            for (int j = largelist.BeginOfSelection.Value; j < largelist[i].Count; j++)
                                if (!largelist.SelectionContains(largelist[i][j]))
                                    return true;
                        }
                        else if (i == largelist.EndOfSelection.Key)
                        {
                            for (int j = 0; j <= largelist.EndOfSelection.Value; j++)
                                if (!largelist.SelectionContains(largelist[i][j]))
                                    return true;
                        }
                        else
                        {
                            for (int j = largelist.BeginOfSelection.Value; j <= largelist.EndOfSelection.Value; j++)
                                if (!largelist.SelectionContains(largelist[i][j]))
                                    return true;
                        }
                }
            return false;
        }
        private bool SelectionHasAction()
        {
            if (largelist.Selection.Count > 0)
                foreach (Control control in largelist.Selection)
                    if (control is UserActionControl)
                        return true;
            return false;
        }
        private void CheckParent(UserActionControl parent)
        {
            int checkedCount = 0;

            foreach (LogEntryControl child in parent.LogEntryControls)
                if (child.Checked)
                    ++checkedCount;

            parent.CheckedChanged -= logChildControlBase_CheckedChanged;
            if (checkedCount == 0)
            {
                if (parent.CheckState == CheckState.Checked)
                    largelist.Select(parent, Hotkeys.Ctrl);
                parent.CheckState = CheckState.Unchecked;
            }
            else if (checkedCount == parent.LogEntryControls.Count)
            {
                if (parent.CheckState != CheckState.Checked)
                    largelist.Select(parent, Hotkeys.Ctrl);
                parent.CheckState = CheckState.Checked;
            }
            else
            {
                if (parent.CheckState == CheckState.Checked)
                    largelist.Select(parent, Hotkeys.Ctrl);
                parent.CheckState = CheckState.Indeterminate;
            }
            parent.CheckedChanged += logChildControlBase_CheckedChanged;
        }
        private void CheckChilds(UserActionControl parent)
        {
            List<Control> toSelect = new List<Control>(largelist.Selection.Count);
            foreach (Control control in largelist.Selection)
                toSelect.Add(control);

            if (parent.CheckState == CheckState.Checked)
            {
                if (!toSelect.Contains(largelist.LastClickedControl))
                    toSelect.Add(largelist.LastClickedControl);
                foreach (LogEntryControl controlToSelect in parent.LogEntryControls)
                    if (!toSelect.Contains(controlToSelect))
                    {
                        toSelect.Add(controlToSelect);
                        controlToSelect.CheckedChanged -= logChildControlBase_CheckedChanged;
                        controlToSelect.Checked = true;
                        controlToSelect.CheckedChanged += logChildControlBase_CheckedChanged;
                    }
            }
            else if (parent.CheckState == CheckState.Unchecked)
            {
                toSelect.Remove(largelist.LastClickedControl);
                foreach (LogEntryControl controlToDeSelect in parent.LogEntryControls)
                {
                    controlToDeSelect.CheckedChanged -= logChildControlBase_CheckedChanged;
                    controlToDeSelect.Checked = false;
                    controlToDeSelect.CheckedChanged += logChildControlBase_CheckedChanged;
                    toSelect.Remove(controlToDeSelect);
                }
            }
            largelist.SelectRange(toSelect);
        }
        /// <summary>Gets the closest next sibling.</summary>
        /// <returns></returns>
        private LogChildControlBase GetClosestNextSibling(Control control)
        {
            int margin = control.Margin.Left;
            KeyValuePair<int, int> index = largelist.IndexOf(control);
            for (int i = index.Key; i < largelist.ViewCount; i++)
                if (i == index.Key)
                {
                    for (int j = index.Value + 1; j < largelist[i].Count; j++)
                        if (largelist[i][j].Margin.Left == margin)
                            return largelist[i][j] as LogChildControlBase;
                        else if (largelist[i][j].Margin.Left < margin)
                            return null;
                }
                else
                {
                    for (int j = 0; j < largelist[i].Count; j++)
                        if (largelist[i][j].Margin.Left == margin)
                            return largelist[i][j] as LogChildControlBase;
                        else if (largelist[i][j].Margin.Left < margin)
                            return null;
                }
            return null;
        }
        /// <summary>Gets the closest previous sibling.</summary>
        /// <returns></returns>
        private LogChildControlBase GetClosestPreviousSibling(Control control)
        {
            int margin = control.Margin.Left;
            KeyValuePair<int, int> index = largelist.IndexOf(control);
            for (int i = index.Key; i >= 0; i--)
            {
                if (i == index.Key)
                {
                    for (int j = index.Value - 1; j >= 0; j--)
                        if (largelist[i][j].Margin.Left == margin)
                            return largelist[i][j] as LogChildControlBase;
                        else if (largelist[i][j].Margin.Left < margin)
                            return null;
                }
                else
                {
                    for (int j = largelist[i].Count - 1; j >= 0; j--)
                        if (largelist[i][j].Margin.Left == margin)
                            return largelist[i][j] as LogChildControlBase;
                        else if (largelist[i][j].Margin.Left < margin)
                            return null;
                }
            }
            return null;
        }
        private LogChildControlBase PreviousControl()
        {
            if (largelist.BeginOfSelection.Value > 0)
            {
                return largelist[0][largelist.BeginOfSelection.Value - 1] as LogChildControlBase;
            }
            else if (largelist.BeginOfSelection.Key > 0 && largelist.BeginOfSelection.Value == 0)
            {
                List<Control> l = largelist[largelist.BeginOfSelection.Key - 1];
                return (l[l.Count - 1]) as LogChildControlBase;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Move and indent
        /// <summary>Moves the selected preview(s) a level down.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLevelDown_Click(object sender, EventArgs e)
        {
            foreach (Control control in largelist.Selection)
            {
                LogEntryControl logEntryControl = control as LogEntryControl;
                UserActionControl parent = logEntryControl.UserActionControl;
                logEntryControl.RemoveUserActionControl();
                parent.LogEntryControls.Remove(logEntryControl);

                CheckParent(parent);
            }
            ApplyChanges();
        }
        /// <summary>Moves the selected preview(s) a level up.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLevelUp_Click(object sender, EventArgs e)
        {
            largelist.OrderSelection();
            UserActionControl parent = GetClosestPreviousSibling(largelist.Selection[0]) as UserActionControl;
            foreach (Control control in largelist.Selection)
            {
                LogEntryControl logEntryControl = control as LogEntryControl;
                logEntryControl.SetUserActionControl(parent);
                parent.LogEntryControls.Add(logEntryControl);

                CheckParent(parent);
            }
            ApplyChanges();
        }
        /// <summary>Moves the selected preview up.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {
            largelist.OrderSelection();
            Control closestPreviousSibling = GetClosestPreviousSibling(largelist.Selection[0]);

            if (closestPreviousSibling is LogEntryControl)
            {
                LogEntryControl logEntryControl = null;
                foreach (Control control in largelist.Selection)
                    if (control is LogEntryControl)
                    {
                        logEntryControl = control as LogEntryControl;
                        if (logEntryControl.UserActionControl != null)
                            logEntryControl.UserActionControl.LogEntryControls.Remove(logEntryControl);
                    }

                logEntryControl = closestPreviousSibling as LogEntryControl;
                if (logEntryControl.UserActionControl != null)
                {
                    int i = logEntryControl.UserActionControl.LogEntryControls.IndexOf(logEntryControl);
                    List<LogEntryControl> selection = new List<LogEntryControl>(largelist.Selection.Count);
                    foreach (Control control in largelist.Selection)
                        selection.Add(control as LogEntryControl);
                    logEntryControl.UserActionControl.LogEntryControls.InsertRange(i, selection);
                }

            }
            largelist.PutRangeAboveControl(largelist.Selection, closestPreviousSibling);
            ApplyChanges();
        }
        /// <summary>Moves the selected preview down.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            ////Search last on same indent.
            largelist.OrderSelection();
            LogChildControlBase lastSiblingInCollection = largelist[largelist.BeginOfSelection.Key][largelist.BeginOfSelection.Value] as LogChildControlBase;
            uint indentationLevel = lastSiblingInCollection.IndentationLevel;
            foreach (Control control in largelist.Selection)
            {
                LogChildControlBase logChildControlBase = control as LogChildControlBase;
                if (logChildControlBase.IndentationLevel == indentationLevel)
                    lastSiblingInCollection = logChildControlBase;
            }
            Control nextControl = GetClosestNextSibling(lastSiblingInCollection);
            KeyValuePair<int, int> index = largelist.IndexOf(nextControl);
            int margin = lastSiblingInCollection.Margin.Left;
            for (int i = index.Key; i < largelist.ViewCount; i++)
            {
                if (i == index.Key)
                {
                    for (int j = index.Value + 1; j < largelist[i].Count; j++)
                        if (largelist[i][j].Margin.Left <= margin)
                            break;
                        else
                            nextControl = largelist[i][j];
                }
                else
                {
                    for (int j = 0; j < largelist[i].Count; j++)
                        if (largelist[i][j].Margin.Left <= margin)
                            break;
                        else
                            nextControl = largelist[i][j];
                }
            }
            foreach (Control control in largelist.Selection)
                if (control is LogEntryControl)
                {
                    LogEntryControl logEntryControl = control as LogEntryControl;
                    if (logEntryControl.UserActionControl != null)
                        logEntryControl.UserActionControl.LogEntryControls.Remove(logEntryControl);
                }
            if (nextControl is LogEntryControl)
            {
                LogEntryControl logEntryControl = nextControl as LogEntryControl;
                if (logEntryControl.UserActionControl != null)
                {
                    int i = logEntryControl.UserActionControl.LogEntryControls.IndexOf(logEntryControl) + 1;
                    List<LogEntryControl> selection = new List<LogEntryControl>(largelist.Selection.Count);
                    foreach (Control control in largelist.Selection)
                        selection.Add(control as LogEntryControl);
                    if (i == logEntryControl.UserActionControl.LogEntryControls.Count)
                        logEntryControl.UserActionControl.LogEntryControls.AddRange(selection);
                    else
                        logEntryControl.UserActionControl.LogEntryControls.InsertRange(i, selection);
                }

            }
            largelist.PutRangeBelowControl(largelist.Selection, nextControl);
            ApplyChanges();
        }
        #endregion

        #region Other Actions
        private void btnAddLogEntry_Click(object sender, EventArgs e)
        {
            LogEntry logEntry = BaseItem.Empty(typeof(LogEntry), _log) as LogEntry;
            _log.Add(logEntry);
            LogEntryControl logEntryControl = new LogEntryControl(logEntry);
            logEntryControl.CheckedChanged += new EventHandler(logChildControlBase_CheckedChanged);
            logEntryControl.Removed += new EventHandler(logEntryControl_Removed);
            logEntryControl.LexicalResultChanged += new EventHandler(logEntryControl_LexicalResultChanged);
            largelist.Add(logEntryControl);

            ApplyChanges();

            logEntryControl.Select();
            largelist.ScrollIntoView(logEntryControl);
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (Control control in largelist.Selection)
                if (control is LogEntryControl)
                {
                    LogEntryControl logEntryControl = control as LogEntryControl;
                    UserActionControl userActionControl = logEntryControl.UserActionControl;
                    if (userActionControl != null)
                    {
                        userActionControl.CheckedChanged -= logChildControlBase_CheckedChanged;
                        userActionControl.CheckState = CheckState.Unchecked;
                        userActionControl.CheckedChanged += logChildControlBase_CheckedChanged;
                        userActionControl.LogEntryControls.Remove(logEntryControl);
                    }
                }
            largelist.RemoveRange(largelist.Selection);

            ApplyChanges();
        }

        private void btnActionizeUnactionize_Click(object sender, EventArgs e)
        {
            if (largelist.Selection.Count != 0)
            {
                this.Cursor = Cursors.WaitCursor;
                if (btnActionizeUnactionize.Text.Contains("Unactionize"))
                {
                    List<UserActionControl> userActionControls = new List<UserActionControl>();

                    largelist.OrderSelection();
                    foreach (Control control in largelist.Selection)
                        if (control is UserActionControl)
                            userActionControls.Add(control as UserActionControl);

                    foreach (var uac in userActionControls)
                    {
                        LockWindowUpdate(this.Handle.ToInt32());

                        uac.Collapsed = false;
                        foreach (LogEntryControl logEntryControl in uac.LogEntryControls)
                        {
                            (logEntryControl.LogChild as LogEntry).Pinned = false;
                            logEntryControl.RemoveUserActionControl();

                            logEntryControl.CheckedChanged -= logChildControlBase_CheckedChanged;
                            logEntryControl.Checked = false;
                            largelist.Selection.Remove(logEntryControl);
                            logEntryControl.CheckedChanged += logChildControlBase_CheckedChanged;
                        }
                        uac.LogEntryControls.Clear();

                        LockWindowUpdate(0);
                        largelist.Remove(uac);
                    }
                    if (largelist.ActiveControl != null)
                        largelist.ScrollIntoView(largelist.ActiveControl);
                }
                else
                {
                    UserAction userAction = new UserAction();
                    userAction.Parent = _log;
                    UserActionControl userActionControl = new UserActionControl(userAction);
                    userActionControl.Collapsed = false;
                    userActionControl.Checked = true;
                    largelist.Insert(userActionControl, largelist.IndexOf(largelist.Selection[0]));
                    largelist.Select(userActionControl, Hotkeys.Ctrl);
                    userActionControl.CheckedChanged += new EventHandler(logChildControlBase_CheckedChanged);
                    userActionControl.CollapsedChanged += new EventHandler(userActionControl_CollapsedChanged);
                    foreach (Control control in largelist.Selection)
                        if (control is LogEntryControl)
                        {
                            LogEntryControl logEntryControl = control as LogEntryControl;
                            (logEntryControl.LogChild as LogEntry).Pinned = true;
                            logEntryControl.SetUserActionControl(userActionControl);
                            userActionControl.LogEntryControls.Add(logEntryControl);
                        }

                    if (largelist.ActiveControl != null)
                        largelist.ScrollIntoView(largelist.ActiveControl);
                }
                ApplyChanges();
                this.Cursor = Cursors.Default;
            }
        }

        private void userActionControl_CollapsedChanged(object sender, EventArgs e)
        {
            UserActionControl userActionControl = sender as UserActionControl;
            if (userActionControl.Collapsed)
            {
                largelist.RemoveRange(new List<Control>(userActionControl.LogEntryControls.ToArray()));
            }
            else
            {
                KeyValuePair<int, int> index = largelist.IndexOf(GetClosestNextSibling(userActionControl));
                if (index.Value == -1)
                    largelist.AddRange(new List<Control>(userActionControl.LogEntryControls.ToArray()));
                else
                    largelist.InsertRange(new List<Control>(userActionControl.LogEntryControls.ToArray()), index);
            }
            SetCollapseUncollapseButton();
        }
        private void errorAndFindSelector_SelectError(object sender, SelectErrorEventArgs e)
        {
            SelectLogChildControlBase(e.Error);
        }
        private void errorAndFindSelector_Find(object sender, FindEventArgs e)
        {
            if (e.Find == string.Empty && largelist.ControlCount > 0)
            {
                SelectLogChildControlBase(largelist[0][0] as LogChildControlBase);
            }
            else
            {
                LogChildControlBase firstFound = null;
                bool startingPointFound = errorAndFindSelector.Found == null;
                foreach (List<Control> controls in largelist)
                    foreach (Control control in controls)
                    {
                        LogChildControlBase logChildControlBase = control as LogChildControlBase;
                        if (logChildControlBase.LogChild.ToString().ToLower().Contains(e.Find.ToLower()))
                        {
                            if (firstFound == null)
                                firstFound = logChildControlBase;
                            if (startingPointFound)
                            {
                                errorAndFindSelector.Found = logChildControlBase;
                                SelectLogChildControlBase(logChildControlBase);
                                return;
                            }
                            else
                            {
                                startingPointFound = errorAndFindSelector.Found == control;
                            }
                        }
                    }
                if (firstFound != null)
                {
                    errorAndFindSelector.Found = firstFound;
                    SelectLogChildControlBase(firstFound);
                    return;
                }
            }
            errorAndFindSelector.Found = null;
        }
        private void SelectLogChildControlBase(LogChildControlBase logChildControlBase)
        {
            if (!largelist.Contains(logChildControlBase))
                (logChildControlBase as LogEntryControl).UserActionControl.Collapsed = false;
            largelist.ScrollIntoView(logChildControlBase);
            logChildControlBase.Select();
        }

        private void btnCollapseExpand_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            var uacs = GetUserActionControls();
            if (uacs.Count != 0)
            {
                if (btnCollapseExpand.Text == "+")
                {
                    btnCollapseExpand.Text = "-";
                    foreach (var uac in uacs)
                        if (uac.Collapsed)
                            uac.Collapsed = false;
                }
                else
                {
                    btnCollapseExpand.Text = "+";
                    foreach (var uac in uacs)
                        if (!uac.Collapsed)
                            uac.Collapsed = true;
                }
            }
            Cursor = Cursors.Default;
        }
        private List<UserActionControl> GetUserActionControls()
        {
            List<UserActionControl> uacs = new List<UserActionControl>();
            for (int view = 0; view < largelist.ViewCount; view++)
                foreach (Control control in largelist[view])
                    if (control is UserActionControl)
                        uacs.Add(control as UserActionControl);
            return uacs;
        }
        private void SetCollapseUncollapseButton()
        {
            bool collapsed = false;
            List<UserActionControl> uacs = GetUserActionControls();
            foreach (var uac in uacs)
                if (uac.Collapsed)
                {
                    collapsed = true;
                    break;
                }
            btnCollapseExpand.Text = collapsed ? "+" : "-";
        }
        #endregion

        #endregion

        #endregion
    }
}