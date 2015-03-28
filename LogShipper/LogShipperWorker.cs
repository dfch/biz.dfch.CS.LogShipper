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

namespace biz.dfch.CS.LogShipper
{
    class LogShipperWorker
    {
        private bool _active = true;
        private Timer _stateTimer = null;

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
        //public LogShipperWorker(string Uri, string ManagementUri, int UpdateIntervalMinutes, int ServerNotReachableRetries)
        public LogShipperWorker()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            //this.Initialise(Uri, ManagementUri, UpdateIntervalMinutes, ServerNotReachableRetries, true);
        }
        public bool Update()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            var fReturn = false;
            if (!_active) return fReturn;
            return _active;
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

        public bool Run()
        {
            bool fReturn = false;


            var fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = "C:\\Logs\\biz.dfch.PS.System.Logging\\2015-03";
            fileSystemWatcher.Filter = "*.log";
            fileSystemWatcher.NotifyFilter = 
                NotifyFilters.LastAccess | 
                NotifyFilters.LastWrite | 
                NotifyFilters.FileName | 
                NotifyFilters.DirectoryName | 
                NotifyFilters.CreationTime;
            fileSystemWatcher.Created += new FileSystemEventHandler(OnEvent);
            fileSystemWatcher.Changed += new FileSystemEventHandler(OnEvent);
            fileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(OnEvent);
            fileSystemWatcher.Error += new ErrorEventHandler(OnError);
            fileSystemWatcher.EnableRaisingEvents = true;

            //var fileName = "C:\\Logs\\biz.dfch.PS.System.Logging\\2015-03\\2015-03-28.log";
            //using (StreamReader streamReader = new StreamReader(
            //        new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            //    ))
            //{
            //    //var msg = string.Format("{0}:{1}.{2} streamReader", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    //Debug.WriteLine(msg);

            //    var lastMaxOffset = streamReader.BaseStream.Length;
            //    while (true)
            //    {
            //        //TODO change to FileSystemWatcher
            //        System.Threading.Thread.Sleep(100);

            //        // check if file has changed - obsolete when using FileSystemWatcher
            //        if (streamReader.BaseStream.Length == lastMaxOffset)
            //        {
            //            continue;
            //        }

            //        // seek to the last read file pointer (always from the beginning)
            //        streamReader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

            //        // read all lines
            //        var line = "";
            //        while (null != (line = streamReader.ReadLine()))
            //        {
            //            Console.WriteLine(line);
            //        }

            //        // update to last read offset
            //        lastMaxOffset = streamReader.BaseStream.Position;
            //    }
            //}

            fReturn = true;
            return fReturn;
        }
        ~LogShipperWorker()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

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
