/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using vApus.Util;
using System.Collections.Generic;

namespace vApus.Report
{
    public partial class Report : Form
    {
        #region Fields
        private string _fileName;

        private AppDomain _reportAppDomain;
        private List<string> _assemblyDirs;
        private int _currentAssemblyDirIndex;

        private const string REPORTDLLSDIR = "BackwardsCompatibleReportDlls";
        private const string STRESSTESTDLL = "vApus.Stresstest.dll";
        #endregion

        public Report(string fileName = null)
        {
            InitializeComponent();
            _fileName = fileName;
            SetAssemblyDirs();
            if (this.IsHandleCreated)
                SetGui();
            else
                this.HandleCreated += new EventHandler(Report_HandleCreated);
        }
        private void SetAssemblyDirs()
        {
            _assemblyDirs = new List<string>();
            _assemblyDirs.Add(Application.StartupPath);
            _currentAssemblyDirIndex = 0;

            string path = Path.Combine(Application.StartupPath, REPORTDLLSDIR);
            if (Directory.Exists(path))
                foreach (string dir in Directory.GetDirectories(path))
                    foreach (string file in Directory.GetFiles(dir))
                        if (file.EndsWith(STRESSTESTDLL))
                        {
                            _assemblyDirs.Add(dir);
                            break;
                        }
        }
        private void Report_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= Report_HandleCreated;
            SetGui();
        }
        private void SetGui()
        {
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
            LoadControlFromCurrentDirectory();
            if (_fileName != null)
                LoadReport();
        }
        /// <summary>
        /// Current directory is determined by the index (on loading error redetermined).
        /// </summary>
        private void LoadControlFromCurrentDirectory()
        {
            pnlPlaceHolder.Controls.Clear();

            //Load in a new appdomain to be able to unload assemblies (references)
            if (_reportAppDomain != null)
            {
                AppDomain.Unload(_reportAppDomain);
                _reportAppDomain = null;
            }
            _reportAppDomain = AppDomain.CreateDomain("reportDomain");

            //Load the assembly with the report control
            Assembly ass = _reportAppDomain.Load(File.ReadAllBytes(Path.Combine(_assemblyDirs[_currentAssemblyDirIndex], STRESSTESTDLL)));
            //Load all the references, once in memory it cannot be replaced, making this work
            foreach (var reference in ass.GetReferencedAssemblies())
                foreach (string file in Directory.GetFiles(_assemblyDirs[_currentAssemblyDirIndex]))
                {
                    string dll = reference.Name + ".dll";
                    if (file.EndsWith(dll))
                    {
                        Assembly foo = _reportAppDomain.Load(File.ReadAllBytes(file));
                    }
                }
            Type t = ass.GetType("vApus.Stresstest.StresstestReportControl");

            UserControl stresstestReportControl = Activator.CreateInstance(t) as UserControl;
            stresstestReportControl.Dock = DockStyle.Fill;

            //Hook to LoadingError event.
            EventInfo loadingError = t.GetEvent("LoadingError");
            Type tDelegate = loadingError.EventHandlerType;

            MethodInfo handler = typeof(Report).GetMethod("stresstestReportControl_LoadingError", BindingFlags.NonPublic | BindingFlags.Instance);
            Delegate d = Delegate.CreateDelegate(tDelegate, this, handler);

            MethodInfo addHandler = loadingError.GetAddMethod();
            object[] addHandlerArgs = { d };
            addHandler.Invoke(stresstestReportControl, addHandlerArgs);

            pnlPlaceHolder.Controls.Add(stresstestReportControl);
        }
        private void stresstestReportControl_LoadingError(object sender, ErrorEventArgs e)
        {
            if (++_currentAssemblyDirIndex != _assemblyDirs.Count)
            {
                LoadControlFromCurrentDirectory();
                if (_fileName != null)
                    LoadReport();
            }
            else
            {
                _currentAssemblyDirIndex = 0;
                MessageBox.Show(this, e.GetException().ToString(), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _fileName = ofd.FileName;
                LoadReport();
            }
        }
        private void LoadReport()
        {
            this.Text = "Report - " + _fileName;
            Control control = pnlPlaceHolder.Controls[0];
            control.GetType().InvokeMember("LoadRFile", BindingFlags.Default | BindingFlags.InvokeMethod, null, control, new object[] { _fileName });
        }
    }
}
