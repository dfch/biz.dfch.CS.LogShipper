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
        private bool _fInitialised = false;
        private bool _active = false;
        private FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();
        private String _path;
        private String _filter;
        private Timer _stateTimer = null;

        public bool Active
        {
            get { return _active; }
            set 
            {
                if(!_fInitialised)
                {
                    throw new InvalidOperationException(String.Format("LogShipperWorker not initialised. Cannot modify property 'Active'."));
                }
                _active = value;
                _fileSystemWatcher.EnableRaisingEvents = value;
            }
        }
        //public LogShipperWorker(string Uri, string ManagementUri, int UpdateIntervalMinutes, int ServerNotReachableRetries)
        public LogShipperWorker()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
        public LogShipperWorker(String path, String filter)
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            var fReturn = false;
            fReturn = this.Initialise(path, filter);
        }

        private bool Initialise(String path, String filter)
        {
            var fReturn = false;
            try
            {
                // Parameter validation
                if (null == path || String.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path", String.Format("path: Parameter validation FAILED. Parameter cannot be null or empty."));
                if (null == filter || String.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("filter", String.Format("filter: Parameter validation FAILED. Parameter cannot be null or empty."));

                if(_fInitialised)
                {
                    Stop();
                }
                _fileSystemWatcher.Path = (_path = path);
                _fileSystemWatcher.Filter = (_filter = filter);
                _fileSystemWatcher.NotifyFilter =
                    NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite |
                    NotifyFilters.FileName |
                    NotifyFilters.DirectoryName |
                    NotifyFilters.CreationTime;
                _fileSystemWatcher.Created += new FileSystemEventHandler(OnEvent);
                _fileSystemWatcher.Changed += new FileSystemEventHandler(OnEvent);
                _fileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);
                _fileSystemWatcher.Deleted += new FileSystemEventHandler(OnEvent);
                _fileSystemWatcher.Error += new ErrorEventHandler(OnError);
                _fInitialised = true;
                this.Active = true;
                fReturn = _fInitialised;
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

            return this.Active;
        }
        public bool Update(String path, String filter)
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            var fReturn = false;

            // Parameter validation
            if (null == path || String.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path", String.Format("path: Parameter validation FAILED. Parameter cannot be null or empty."));
            if (null == filter || String.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("filter", String.Format("filter: Parameter validation FAILED. Parameter cannot be null or empty."));

            Stop();
            _path = path;
            _filter = filter;
            fReturn = Start();

            return fReturn;
        }

        private static void OnEvent(object source, FileSystemEventArgs e)
        {
            Debug.WriteLine("{0} - {1} : {2} [{3}]", e.FullPath, e.Name, e.ChangeType.ToString(), e.ToString());
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Debug.WriteLine("{0} - {1} : {2} [{3}]", e.FullPath, e.Name, e.ChangeType.ToString(), e.ToString());
        }

        private static void OnError(object source, ErrorEventArgs e)
        {
            Debug.WriteLine("{0} : {1} : {2}", e.GetException().ToString(), e.GetType().ToString(), e.ToString());
        }

        public void Stop()
        {
            this.Active = false;
        }

        public bool Start()
        {
            this.Active = true;
            return this.Active;
        }
        public bool Start(String path, String filter)
        {
            return this.Initialise(path, filter);
        }

        ~LogShipperWorker()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (null != _fileSystemWatcher)
            {
                _fileSystemWatcher.Dispose();
            }
            if (null != _stateTimer)
            {
                _stateTimer.Dispose();
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
