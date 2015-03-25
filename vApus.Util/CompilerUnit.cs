/*
 * Copyright 2010 (c) Vandroemme Dieter
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Vandroemme Dieter
 */
using Microsoft.CSharp;
using RandomUtils.Log;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace vApus.Util {
    /// <summary>
    ///     A C# .net v4.5 compiler unit.
    ///     For references there is linked in Application.StartupPath or in the folders given in ReferenceResolver.ini if the Use property of ReferenceResolver is set to true.
    /// </summary>
    public class CompilerUnit {
        private readonly List<TempFileCollection> _tempFiles = new List<TempFileCollection>();
        private readonly string _tempFilesDirectory = Path.Combine(Application.StartupPath, "CompilerUnitTempFiles");

        public CompilerUnit() { Application.ApplicationExit += Application_ApplicationExit; }

        /// <summary>
        ///     A threadsafe compile.
        ///     A source should have 1 commented line with
        ///     the dll references sepparated by a semicolon. eg: "// dllreferences: myDll.dll;myOtherDll.dll", empty entries are removed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="debug">For compiling with an attached debugger. Compile warnings are threated as errros.</param>
        /// <param name="compilerResults"></param>
        /// <returns></returns>
        public Assembly Compile(string source, bool debug, out CompilerResults compilerResults) {
            return Compile(source, null, debug, out compilerResults);
        }

        /// <summary>
        ///     A threadsafe compile.
        ///     Each source should have 1 commented line with
        ///     the dll references sepparated by a semicolon. eg: "// dllreferences: myDll.dll;myOtherDll.dll", empty entries are removed.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="debug">For compiling with an attached debugger. Compile warnings are threated as errros.</param>
        /// <param name="compilerResults"></param>
        /// <returns></returns>
        public Assembly Compile(string[] sources, bool debug, out CompilerResults compilerResults) {
            return Compile(sources, null, debug, out compilerResults);
        }

        /// <summary>
        ///     A threadsafe compile.
        ///     A source should have 1 commented line with
        ///     the dll references sepparated by a semicolon. eg: "// dllreferences: myDll.dll;myOtherDll.dll", empty entries are removed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="outputAssembly"></param>
        /// <param name="debug">For compiling with an attached debugger. Compile warnings are threated as errros.</param>
        /// <param name="compilerResults"></param>
        /// <returns></returns>
        public Assembly Compile(string source, string outputAssembly, bool debug, out CompilerResults compilerResults) {
            return Compile(new[] { source }, outputAssembly, debug, out compilerResults);
        }

        /// <summary>
        ///     A threadsafe compile.
        ///     Each source should have 1 commented line with
        ///     the dll references sepparated by a semicolon. eg: "// dllreferences: myDll.dll;myOtherDll.dll", empty entries are removed.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="outputAssembly"></param>
        /// <param name="debug">For compiling with an attached debugger. Compile warnings are threated as errros.</param>
        /// <param name="compilerResults"></param>
        /// <returns></returns>
        public Assembly Compile(string[] sources, string outputAssembly, bool debug, out CompilerResults compilerResults) {
            if (sources.Length == 0)
                throw new Exception("Nothing to compile.");

            var providerOptions = new Dictionary<string, string>();
            providerOptions.Add("CompilerVersion", "v4.0");
            CodeDomProvider compiler = new CSharpCodeProvider(providerOptions);
            var compilerParameters = new CompilerParameters();

            compilerParameters.GenerateExecutable = false;
            if (outputAssembly != null)
                compilerParameters.OutputAssembly = outputAssembly;

            if (debug) {
                //Will temp output the assembly while it is compiled in memory = no locking
                if (!Directory.Exists(_tempFilesDirectory))
                    Directory.CreateDirectory(_tempFilesDirectory);

                string readme = Path.Combine(_tempFilesDirectory, "README.TXT");
                if (!File.Exists(readme))
                    using (var sw = new StreamWriter(readme)) {
                        sw.Write("These files are compiled connection proxies used for stresstesting, this folder can be removed safely.");
                        sw.Flush();
                    }
                compilerParameters.GenerateInMemory = false;
                compilerParameters.IncludeDebugInformation = true;
                compilerParameters.TempFiles = new TempFileCollection(_tempFilesDirectory, true);
                _tempFiles.Add(compilerParameters.TempFiles);
            } else {
                compilerParameters.GenerateInMemory = true;
                compilerParameters.IncludeDebugInformation = false;
            }

            compilerParameters.CompilerOptions = "/Optimize";
            compilerParameters.TreatWarningsAsErrors = false;
            compilerParameters.WarningLevel = 4;

            foreach (string source in sources)
                AddAllReferencedAssemblies(source, compilerParameters);

            compilerResults = null;

            for (int i = 1; i != 4; i++)
                try {
                    compilerResults = compiler.CompileAssemblyFromSource(compilerParameters, sources);
                    if (compilerResults.Errors.HasErrors && i == 3 || !compilerResults.Errors.HasErrors) {
                        break;
                    } else {
                        if (debug) break;

                        compilerResults.Errors.Clear();
                        Thread.Sleep(1000 * i);
                    }
                } catch {
                    //Ignore. Will be handled later on.
                }

            if (!debug)
                DeleteTempFiles();

            try {
                return compilerResults.CompiledAssembly;
            } catch {
                return null;
            }
        }

        /// <summary>
        ///     Each source should have 1 commented line with
        ///     the dll references sepparated by a semicolon. eg: "// dllreferences: myDll.dll;myOtherDll.dll", empty entries are removed.
        ///     If the ReferenceResolver is used ('Use' property == true) there will be searched in the folders given (ReferenceResolver.ini).
        /// </summary>
        /// <param name="compilerParamaters"></param>
        /// <param name="fileName"></param>
        private void AddAllReferencedAssemblies(string source, CompilerParameters compilerParamaters) {
            bool found = false;
            source = source.Replace(Environment.NewLine, "\n").Replace('\r', '\n');
            foreach (string line in source.Split('\n'))
                if (line.StartsWith("// dllreferences:")) {
                    string[] dllReferences = line.Split(':')[1].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string dllReference in dllReferences) {

                        string[] matches = null;
                        Exception matchException = null;
                        for (int t = 1; t != 11; t++) //Retry 9 times if necessary. IO exception possible.
                            try {
                                matches = Directory.GetFiles(Application.StartupPath, dllReference, SearchOption.AllDirectories);
                                break;
                            } catch (Exception ex) {
                                matchException = ex;
                                Thread.Sleep(100 *t);
                            }

                        if (matchException == null) throw matchException;

                        if (matches.Length != 0) {
                            bool matchFound = false;
                            foreach (string match in matches) {
                                string[] splitMatch = match.Split('\\');
                                if (splitMatch[splitMatch.Length - 1] == dllReference) {
                                    compilerParamaters.ReferencedAssemblies.Add(match);
                                    matchFound = true;
                                    break;
                                }
                            }
                            if (!matchFound)
                                compilerParamaters.ReferencedAssemblies.Add(dllReference);
                        } else {
                            compilerParamaters.ReferencedAssemblies.Add(dllReference);
                        }
                    }
                    found = true;
                } else if (found) {
                    break;
                }
        }

        /// <summary>
        ///     Delete the tempFiles generated on compiling when debugging, if possible.
        /// </summary>
        public void DeleteTempFiles() {
            foreach (TempFileCollection tempFiles in _tempFiles) {
                tempFiles.KeepFiles = false;
                try {
                    tempFiles.Delete();
                } catch (Exception ex) {
                    Loggers.Log(Level.Warning, "Failed deleting a temp file.", ex);
                }
            }
            _tempFiles.Clear();

            if (Directory.Exists(_tempFilesDirectory))
                try {
                    Directory.Delete(_tempFilesDirectory, true);
                } catch {
                    //Not important. Don't care.
                }
        }

        private void Application_ApplicationExit(object sender, EventArgs e) {
            if (Directory.Exists(_tempFilesDirectory))
                try {
                    Directory.Delete(_tempFilesDirectory, true);
                } catch {
                    //Not important. Don't care.
                }
        }
    }
}