/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using vApus.Util;

namespace vApus.UpdateTool {
    internal static class Program {
        private static readonly Mutex _namedMutex = new Mutex(true, Assembly.GetExecutingAssembly().FullName);

        [STAThread]
        private static void Main(string[] args) {
            if (_namedMutex.WaitOne(0, true)) {
                if (args.Length < 1 || args[0] != "{A84E447C-3734-4afd-B383-149A7CC68A32}") {
                    MessageBox.Show("This tool must be called from within vApus!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try {
                    MessageBox.Show("");
                    Application.Run(new Update(args));
                } catch (Exception ex) {
                    MessageBox.Show("Could not start the update tool.\n" + ex, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                } finally {
                    _namedMutex.ReleaseMutex();
                }
            } else {
                var msgHandler = new Win32WindowMessageHandler();
                msgHandler.PostMessage();
            }
        }
    }
}