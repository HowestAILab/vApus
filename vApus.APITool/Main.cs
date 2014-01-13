/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using FastColoredTextBoxNS;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.APITool {
    public partial class Main : Form {
        private CheatSheet _cheatsheet;
        private SocketWrapper _socketWrapper;
        private CancellationTokenSource _cts;

        private string _apiKey;
        private readonly byte[] Salt =
            {
                0x49, 0x16, 0x49, 0x2e, 0x11, 0x1e, 0x45, 0x24, 0x86, 0x05, 0x01, 0x03,
                0x62
            };

        private string _scriptFullFileName, _scriptFileName;
        private bool _scriptChanged;

        public Main() {
            InitializeComponent();
            this.HandleCreated += Main_HandleCreated;
        }

        private void Main_HandleCreated(object sender, EventArgs e) {
            this.HandleCreated -= Main_HandleCreated;
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
            _cts = new CancellationTokenSource();
            this.fctxtScript.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fctxtScript_TextChangedDelayed);
        }

        async private void btnStart_Click(object sender, EventArgs e) {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            fctxtScript.ReadOnly = true;
            fctxtIn.Clear();
            fctxtOut.Clear();

            await Task.Run(() => RunScript());

            _cts = new CancellationTokenSource();

            fctxtScript.ReadOnly = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }
        private void btnStop_Click(object sender, EventArgs e) {
            btnStop.Enabled = false;
            _cts.Cancel();
            Disconnect();
        }

        private void RunScript() {
            try {
                foreach (string line in fctxtScript.Lines) {
                    if (_cts.Token.IsCancellationRequested) throw new Exception();

                    string msg = line.Trim();
                    if (msg.StartsWith("#") || msg.Length == 0) {
                        //Do nothing
                    } else if (msg.StartsWith("/")) {
                        //RPC
                        WriteToOut(msg);

                        if (_socketWrapper == null || !_socketWrapper.Connected) {
                            msg = "Not connected to the server. Call 'Connect <IP address>' first.";
                        } else {
                            _socketWrapper.Send(msg, SendType.Text, Encoding.UTF8);
                            msg = _socketWrapper.Receive(SendType.Text, Encoding.UTF8) as string;
                            msg = msg.Decrypt(_apiKey, Salt);
                        }

                        WriteToIn(msg);
                    } else {
                        WriteToOut(msg);

                        //Connect or disconnect
                        if (msg.StartsWith("Connect ")) {
                            var split = msg.Split(' ');
                            if (split.Length == 3)
                                try {
                                    string ip = split[1];
                                    string apiKey = split[2];
                                    Connect(ip, apiKey);
                                    msg = "Connected to " + ip + ":1537.";
                                } catch (Exception ex) {
                                    msg = "Failed connecting to the server:\n" + ex.Message;
                                } else
                                msg = "Syntax error, should be 'Connect <IP Address>'.";

                        } else if (msg == "Disconnect") {
                            Disconnect();
                            msg = "Disconnected.";
                        } else if (msg.StartsWith("Wait ")) {
                            var split = msg.Split(' ');
                            if (split.Length == 2)
                                try {
                                    int s = int.Parse(split[1]);
                                    Thread.Sleep(s * 1000);
                                    msg = "Waited " + s + " seconds.";
                                } catch {
                                    msg = "Syntax error, should be 'Wait <Seconds>'.";
                                } else
                                msg = "Syntax error, should be 'Wait <Seconds>'.";
                        } else {
                            msg = "Command " + msg + " Not recognized.";
                        }
                        WriteToIn(msg);
                    }
                }
            } catch {
                WriteToOut("Script stopped.");
            }
        }
        private void Connect(string ip, string apikey) {
            _apiKey = apikey;
            if (_socketWrapper != null) Disconnect();

            try {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socketWrapper = new SocketWrapper(ip, 1537, socket);
                _socketWrapper.Connect();
            } catch {
                _socketWrapper = null;
                throw;
            }
        }
        private void Disconnect() {
            if (_socketWrapper != null) {
                try { _socketWrapper.Close(); } catch { }
                _socketWrapper = null;
            }
        }

        private void WriteToOut(string message) { WriteTo(message, fctxtOut); }
        private void WriteToIn(string message) { WriteTo(message, fctxtIn); }
        private void WriteTo(string message, FastColoredTextBox fctxt) {
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                if (fctxt.Text.Length == 0) fctxt.Text = message; else fctxt.Text += "\n" + message;
            }, null);
        }

        #region Main menu
        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            var dialogResult = DialogResult.None;
            if (_scriptChanged) {
                string script = _scriptFileName == null ? "this script" : "'" + _scriptFileName + "'";
                dialogResult = MessageBox.Show("Do you want to save " + script + " before opening another script?", string.Empty, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }

            if (dialogResult == DialogResult.Yes) saveToolStripMenuItem_Click(saveToolStripMenuItem, null);
            if (dialogResult == DialogResult.Cancel) return;

            _scriptFullFileName = _scriptFileName = null;

            fctxtScript.Text = string.Empty;
            fctxtScript.ClearUndo();

            _scriptChanged = false;
            Text = "<New> - vApus API Tool";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            var dialogResult = DialogResult.None;
            if (_scriptChanged) {
                string script = _scriptFileName == null ? "this script" : "'" + _scriptFileName + "'";
                dialogResult = MessageBox.Show("Do you want to save " + script + " before opening another script?", string.Empty, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }

            if (dialogResult == DialogResult.Yes) saveToolStripMenuItem_Click(saveToolStripMenuItem, null);
            if (dialogResult == DialogResult.Cancel) return;

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                _scriptFullFileName = openFileDialog.FileName;
                var split = _scriptFullFileName.Split('\\');
                _scriptFileName = split[split.Length - 1];

                StreamReader sr = null;
                using (sr = new StreamReader(_scriptFullFileName))
                    fctxtScript.Text = sr.ReadToEnd();
                fctxtScript.ClearUndo();


                _scriptChanged = false;
                Text = _scriptFileName + " - vApus API Tool";
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_scriptFileName == null)
                saveAsToolStripMenuItem_Click(sender, e);
            else
                Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                _scriptFullFileName = saveFileDialog.FileName;
                var split = _scriptFullFileName.Split('\\');
                _scriptFileName = split[split.Length - 1];

                Save();
            }
        }
        private void Save() {
            StreamWriter sw = null;
            using (sw = new StreamWriter(_scriptFullFileName))
                sw.Write(fctxtScript.Text);

            _scriptChanged = false;
            Text = _scriptFileName + " - vApus API Tool";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) { this.Close(); }

        private void fctxtScript_TextChangedDelayed(object sender, TextChangedEventArgs e) {
            if (e.ChangedRange.Start.iChar == 0 && e.ChangedRange.Start.iLine == 0 && e.ChangedRange.End.iChar == 0 && e.ChangedRange.End.iLine == 0) return;
            _scriptChanged = true;
            if (_scriptFileName != null)
                Text = "*" + _scriptFileName + " - vApus API Tool";
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) { fctxtScript.Undo(); }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e) { fctxtScript.Redo(); }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e) { fctxtScript.Cut(); }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            if (fctxtIn.Focused)
                fctxtIn.Copy();
            else if (fctxtOut.Focus())
                fctxtOut.Focus();
            else
                fctxtScript.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) { fctxtScript.Paste(); }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) { fctxtScript.SelectAll(); }

        private void cheatSheetToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_cheatsheet == null || _cheatsheet.IsDisposed) _cheatsheet = new CheatSheet();
            _cheatsheet.Show();
            if (_cheatsheet.WindowState == FormWindowState.Minimized) _cheatsheet.WindowState = FormWindowState.Normal;
            _cheatsheet.TopMost = true;
            _cheatsheet.TopMost = false;
        }
        #endregion

        private void Main_FormClosing(object sender, FormClosingEventArgs e) {
            var dialogResult = DialogResult.None;
            if (_scriptChanged) {
                string script = _scriptFileName == null ? "this script" : "'" + _scriptFileName + "'";
                dialogResult = MessageBox.Show("Do you want to save " + script + " before closing?", string.Empty, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }

            if (dialogResult == DialogResult.Yes) saveToolStripMenuItem_Click(saveToolStripMenuItem, null);
        }

    }
}
