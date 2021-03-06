﻿/**
 * Copyright 2015-2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using biz.dfch.CS.LogShipper.Public;
using System.Collections.Specialized;

// Install-Package Microsoft.Net.Http
// https://www.nuget.org/packages/Microsoft.Net.Http
// Install-Package Newtonsoft.Json
// https://www.nuget.org/packages/Newtonsoft.Json/6.0.1
// http://james.newtonking.com/json/help/index.html?topic=html/SelectToken.htm#
namespace biz.dfch.CS.LogShipper.Core
{
    public class LogShipperWorker
    {
        private bool _isInitialised = false;
        private volatile bool _isActive = false;
        private readonly FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();
        private String _logFile;
        private String _logPath;
        private String _logFilter;
        private String _scriptFile;
        private Thread _thread;

        private readonly CompositionContainer _container;
        [ImportMany]
        private IEnumerable<Lazy<ILogShipperParser, ILogShipperParserData>> _parsers;
        private Lazy<ILogShipperParser, ILogShipperParserData> _parser;
        [ImportMany]
        private IEnumerable<Lazy<ILogShipperOutput, ILogShipperOutputData>> _outputs;
        private List<Lazy<ILogShipperOutput, ILogShipperOutputData>> _outputsActive = new List<Lazy<ILogShipperOutput, ILogShipperOutputData>>();

        private void InitialiseExtensions(CompositionContainer container)
        {
            // An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();

            // Adds all the parts found in the given directory
            var folder = ConfigurationManager.AppSettings["ExtensionsFolder"];
            try
            {
                if (!Path.IsPathRooted(folder))
                {
                    folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);
                }
                catalog.Catalogs.Add(new DirectoryCatalog(folder));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(String.Format("AppSettings.ExtensionsFolder: Loading extensions from '{0}' FAILED.\n{1}", folder, ex.Message));
            }
            finally
            {
                // Adds all the parts found in the same assembly as the Program class
                catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
            }

            // Create the CompositionContainer with the parts in the catalog
            container = new CompositionContainer(catalog);
            try
            {
                // Fill the imports of this object
                container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Trace.WriteLine(compositionException.ToString());
                throw;
            }

            // Get parser
            LoadParserExtension(ConfigurationManager.AppSettings["ParserName"]);

            // Get output
            LoadOutputExtension(ConfigurationManager.AppSettings["OutputName"]);
        }
 
        private void LoadParserExtension(String name)
        {
            if(String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("LoadParserExtension.name: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            try
            {
                var parserNameNormalised = name.Trim();
                _parser = _parsers
                                .Where(
                                    p => p.Metadata.Name.Equals(
                                        parserNameNormalised,
                                        StringComparison.InvariantCultureIgnoreCase))
                                .Single();
                var section = ConfigurationManager.GetSection(_parser.Metadata.Name) as NameValueCollection;
                _parser.Value.Configuration = section;

                Trace.WriteLine(String.Format("{0}: Loading parser extension SUCCEEDED.", parserNameNormalised));
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(String.Format("AppSettings.ParserName: Parameter validation FAILED. Parameter must not be null or empty and '{0}' has to be a valid parser extension.\n{1}", name, ex.Message));
                throw;
            }
            catch(ConfigurationErrorsException ex)
            {
                Trace.WriteLine(String.Format("AppSettings.configSections: Section name '{0}' does not exist.\n{1}", name, ex.Message));
                throw;
            }
        }
 
        private void LoadOutputExtension(String name)
        {
            if(String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("LoadOutputExtension.name: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            var outputNames = name.Split(new char[] { ',', ';' });
            foreach (var outputName in outputNames)
            {
                try
                {
                    var outputNameNormalised = outputName.Trim();
                    var output = _outputs
                                         .Where(
                                              p => p.Metadata.Name.Equals(
                                                  outputNameNormalised,
                                                  StringComparison.InvariantCultureIgnoreCase))
                                         .Single();
                    var section = ConfigurationManager.GetSection(outputNameNormalised) as NameValueCollection;
                    output.Value.Configuration = section;
                    _outputsActive.Add(output);
                    Trace.WriteLine(String.Format("{0}: Loading output extension SUCCEEDED.", outputNameNormalised));
                }
                catch (InvalidOperationException ex)
                {
                    Trace.WriteLine(String.Format("AppSettings.OutputName: Parameter validation FAILED. Parameter must not be null or empty and '{0}' has to be a valid output extension.\n{1}", outputName, ex.Message));
                    throw;
                }
                catch (ConfigurationErrorsException ex)
                {
                    Trace.WriteLine(String.Format("AppSettings.configSections: Section name '{0}' does not exist.\n{1}", outputName, ex.Message));
                    throw;
                }
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set 
            {
                lock (this)
                {
                    if (!_isInitialised)
                    {
                        throw new InvalidOperationException(String.Format("LogShipperWorker not initialised. Cannot modify property 'IsActive'."));
                    }
                    _isActive = value;
                    _fileSystemWatcher.EnableRaisingEvents = value;
                }
            }
        }
        public LogShipperWorker()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            InitialiseExtensions(_container);
        }
 
        public LogShipperWorker(String logFile, String scriptFile)
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            InitialiseExtensions(_container);

            var fReturn = false;
            fReturn = this.Initialise(logFile, scriptFile);
        }

        private bool Initialise(String logFile, String scriptFile)
        {
            var fReturn = false;
            try
            {
                // Parameter validation
                if (null == logFile || String.IsNullOrWhiteSpace(logFile)) throw new ArgumentNullException("logFile", String.Format("logFile: Parameter validation FAILED. Parameter cannot be null or empty."));
                if (null == scriptFile || String.IsNullOrWhiteSpace(scriptFile)) throw new ArgumentNullException("scriptFile", String.Format("scriptFile: Parameter validation FAILED. Parameter cannot be null or empty."));
                if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(scriptFile)))
                {
                    scriptFile = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(scriptFile));
                }
                char[] achFileName = Path.GetFileName(scriptFile).ToCharArray();
                char[] achFileInvalidChars = Path.GetInvalidFileNameChars();
                if('\0' != achFileName.Intersect(achFileInvalidChars).FirstOrDefault())
                {
                    throw new ArgumentException("ScriptFile: Parameter validation FAILED. ScriptFile name contains invalid characters.", scriptFile);
                }
                if (!File.Exists(scriptFile)) throw new FileNotFoundException(String.Format("scriptFile: Parameter validation FAILED. File '{0}' does not exist.", scriptFile), scriptFile);
                _scriptFile = scriptFile;

                char[] achPathName = Path.GetDirectoryName(logFile).ToCharArray();
                char[] achPathInvalidChars = Path.GetInvalidPathChars();
                if('\0' != achPathName.Intersect(achPathInvalidChars).FirstOrDefault())
                {
                    throw new ArgumentException("logFile: Parameter validation FAILED. logFile path contains invalid characters.", scriptFile);
                }
                _logPath = Path.GetDirectoryName(logFile);
                if (!Directory.Exists(_logPath))
                {
                    throw new DirectoryNotFoundException(String.Format("logFile: Parameter validation FAILED. Path '{0}' does not exist.", _logPath));
                }
                _logFile = logFile;
                _logFilter = String.Format("*{0}", Path.GetExtension(logFile));

                if (_isInitialised)
                {
                    Stop();
                }
                _fileSystemWatcher.Path = _logPath;
                _fileSystemWatcher.Filter = _logFilter;
                _fileSystemWatcher.NotifyFilter =
                    NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite |
                    NotifyFilters.FileName |
                    NotifyFilters.DirectoryName |
                    NotifyFilters.CreationTime;
                _fileSystemWatcher.Created += new FileSystemEventHandler(OnCreated);
                //_fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
                _fileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);
                _fileSystemWatcher.Deleted += new FileSystemEventHandler(OnDeleted);
                _fileSystemWatcher.Error += new ErrorEventHandler(OnError);

                var ps = PowerShell
                    .Create()
                    .AddScript("return true;");
                ps.Invoke();
                ps.Commands.Clear();

                _isInitialised = true;
                fReturn = _isInitialised;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0}@{1}: {2}\r\n{3}", ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace);
                throw ex;
            }
            return fReturn;
        }
        public bool Update()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            var fReturn = this.IsActive;
            if (!fReturn)
            {
                return fReturn;
            }
            fReturn = Stop();
            if (!fReturn)
            {
                return fReturn;
            }
            fReturn = Start();
            return this.IsActive;
        }
        public bool Update(String logFile, String scriptFile)
        {
            Trace.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            var fReturn = false;

            // Parameter validation
            if (null == logFile || String.IsNullOrWhiteSpace(logFile)) throw new ArgumentNullException("logFile", String.Format("logFile: Parameter validation FAILED. Parameter cannot be null or empty."));
            if (null == scriptFile || String.IsNullOrWhiteSpace(scriptFile)) throw new ArgumentNullException("scriptFile", String.Format("scriptFile: Parameter validation FAILED. Parameter cannot be null or empty."));
            if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(scriptFile)))
            {
                scriptFile = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(scriptFile));
            }
            char[] achFileName = Path.GetFileName(scriptFile).ToCharArray();
            char[] achFileInvalidChars = Path.GetInvalidFileNameChars();
            if('\0' != achFileName.Intersect(achFileInvalidChars).FirstOrDefault())
            {
                throw new ArgumentException("ScriptFile: Parameter validation FAILED. ScriptFile name contains invalid characters.", scriptFile);
            }
            if (!File.Exists(scriptFile)) throw new FileNotFoundException(String.Format("scriptFile: Parameter validation FAILED. File '{0}' does not exist.", scriptFile), scriptFile);
            _scriptFile = scriptFile;

            char[] achPathName = Path.GetDirectoryName(logFile).ToCharArray();
            char[] achPathInvalidChars = Path.GetInvalidPathChars();
            if('\0' != achPathName.Intersect(achPathInvalidChars).FirstOrDefault())
            {
                throw new ArgumentException("logFile: Parameter validation FAILED. logFile path contains invalid characters.", scriptFile);
            }

            fReturn = this.IsActive;
            if (!fReturn)
            {
                return fReturn;
            }
            fReturn = Stop();
            if (!fReturn)
            {
                return fReturn;
            }

            _logPath = Path.GetDirectoryName(logFile);
            if (!Directory.Exists(_logPath))
            {
                throw new DirectoryNotFoundException(String.Format("logFile: Parameter validation FAILED. Path '{0}' does not exist.", _logPath));
            }
            _logFile = logFile;
            _logFilter = String.Format("*.{0}", Path.GetExtension(logFile));

            fReturn = Start(_logFile, _scriptFile);
            return fReturn;
        }

        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            Trace.WriteLine("{0} - {1} : {2} [{3}]", e.FullPath, e.Name, e.ChangeType.ToString(), e.ToString());
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Trace.WriteLine("{0} - {1} : {2} [{3}]", e.FullPath, e.Name, e.ChangeType.ToString(), e.ToString());
        }

        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            Trace.WriteLine("{0} - {1} : {2} [{3}]", e.FullPath, e.Name, e.ChangeType.ToString(), e.ToString());
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Trace.WriteLine("{0} - {1} : {2} [{3}]", e.FullPath, e.Name, e.ChangeType.ToString(), e.ToString());
        }

        private static void OnError(object source, ErrorEventArgs e)
        {
            Trace.WriteLine("{0} : {1} : {2}", e.GetException().ToString(), e.GetType().ToString(), e.ToString());
        }

        public bool Stop()
        {
            var fReturn = false;
            this.IsActive = fReturn;
            
            int nRetries = 10 * 30;
            int nWaitMs = 100;
            for(var c = 0; c < nRetries; c++)
            {
                if(!_thread.IsAlive)
                {
                    break;
                }
                System.Threading.Thread.Sleep(nWaitMs);
            }
            fReturn = _thread.IsAlive;
            if(fReturn)
            {
                var msg = String.Format("Stopping thread FAILED. Operation timed out after '{0}' seconds.", (nRetries * nWaitMs) / 1000);
                System.Diagnostics.Trace.WriteLine(msg);
                _thread.Abort();
                throw new TimeoutException(msg);
            }
            return !fReturn;
        }

        public bool Start()
        {
            var fReturn = false;
            if (!this._isInitialised)
            {
                throw new InvalidOperationException("Worker is not initialised and therefore cannot be started.");
            }
            if (this.IsActive)
            {
                return fReturn;
            }
            if(null != _thread && _thread.IsAlive)
            {
                return fReturn;
            }
            Trace.WriteLine("Creating worker thread ...");
            _thread = new Thread(DoWork);
            Trace.WriteLine("Creating worker thread COMPLETED. _thread.ManagedThreadId '{0}'. _thread.IsAlive '{1}'", _thread.ManagedThreadId, _thread.IsAlive, "");
            Trace.WriteLine("Starting worker thread ...");
            _thread.Start();
            Trace.WriteLine("Starting worker thread COMPLETED. _thread.IsAlive '{0}'", _thread.IsAlive, "");

            fReturn = true;
            this.IsActive = fReturn;
            return this.IsActive;
        }
        public bool Start(String logFile, String scriptFile)
        {
            var fReturn = this.Initialise(logFile, scriptFile);
            if(fReturn)
            {
                fReturn = this.Start();
            }
            return fReturn;
        }

        public void DoWork()
        {
            var fn = String.Format("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            Trace.WriteLine("{0} Do Work ...", fn, "");
            bool fCreated = false;
            try
            {
                Trace.WriteLine("Do Work IsActive ...");
                do
                {
                    if (!File.Exists(_logFile))
                    {
                        fCreated = true;
                        Trace.WriteLine("File '{0}' does not exist. Waiting ...", _logFile, "");
                        System.Threading.Thread.Sleep(500);
                        continue;
                    }

                    using (StreamReader streamReader = new StreamReader(
                           new FileStream(_logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                       ))
                    {
                        long streamLength = 0;
                        if (!fCreated)
                        {
                            streamLength = streamReader.BaseStream.Length;
                        }
                        do
                        {
                            //Trace.WriteLine("{0}:{1}.{2} streamReader. BaseStream.Length {3}. streamLength {4}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, streamReader.BaseStream.Length, streamLength);
                            if (streamReader.BaseStream.Length == streamLength)
                            {
                                continue;
                            }

                            // seek to the last read file pointer (always from the beginning)
                            streamReader.BaseStream.Seek(streamLength, SeekOrigin.Begin);

                            // read all lines
                            var line = String.Empty;
                            while (null != (line = streamReader.ReadLine()))
                            {
                                var list = _parser.Value.Parse(line);
                                foreach(var output in _outputsActive)
                                {
                                    list.ForEach(l => output.Value.Log(l));
                                }
                            }

                            // update to last read offset
                            streamLength = streamReader.BaseStream.Position;
                            System.Threading.Thread.Sleep(100);
                        }
                        while (this.IsActive);
                    }
                }
                while (this.IsActive);
            }
            catch(ThreadAbortException ex)
            {
                System.Diagnostics.Trace.WriteLine(String.Format("{0}: {1} '{2}'", fn, ex.GetType().ToString(), ex.Message));
            }
        }

        ~LogShipperWorker()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            if(this.IsActive)
            {
                this.IsActive = false;
            }
            if (null != _fileSystemWatcher)
            {
                _fileSystemWatcher.Dispose();
            }
        }
    }
}
