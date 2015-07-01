using System;
using System.Collections.Generic;
using System.Linq;
// Install-Package Microsoft.Net.Http
// https://www.nuget.org/packages/Microsoft.Net.Http
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Management;
using System.Management.Automation;
// Install-Package Newtonsoft.Json
// https://www.nuget.org/packages/Newtonsoft.Json/6.0.1
// http://james.newtonking.com/json/help/index.html?topic=html/SelectToken.htm#
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.CompilerServices;

namespace biz.dfch.CS.LogShipper
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
        readonly PowerShell _ps = PowerShell.Create();

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
        }
        public LogShipperWorker(String logFile, String scriptFile)
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
                                //Trace.WriteLine("*** {0} ***", line, "");
                                //var ps = PowerShell
                                //.Create()
                                    //.AddCommand("Log-Debug")
                                    //.AddParameter("fn", "fn")
                                    //.AddParameter("msg", line)
                                    //.AddScript(String.Format("[System.Diagnostics.Trace]::WriteLine('{0}')", line))
                                _ps.Commands.Clear();
                                _ps
                                    .AddScript(String.Format("[System.Diagnostics.Trace]::WriteLine('{0}')", line))
                                    //.AddScript(String.Format("return @('and the result is: {0}')", line))
                                    .AddCommand(_scriptFile)
                                    .AddParameter("InputObject", line)
                                ;
                                var results = _ps
                                    .Invoke();
                                foreach (var result in results)
                                {
                                    System.Diagnostics.Trace.WriteLine(String.Format("result: '{0}'", result.BaseObject.ToString()));
                                    //foreach (var member in result.Members)
                                    //{
                                    //    System.Diagnostics.Trace.WriteLine(String.Format("{0}:{1}", member.Name, member.Value));
                                    //}
                                    //System.Diagnostics.Trace.WriteLine(result.Members.ToString());
                                }
                            }

                            // update to last read offset
                            streamLength = streamReader.BaseStream.Position;
                            System.Threading.Thread.Sleep(100);
                        } while (this.IsActive);
                    }
                } while (this.IsActive);
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

/**
 *
 *
 * Copyright 2015 Ronald Rink, d-fens GmbH
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
 *
 */
