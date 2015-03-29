using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace biz.dfch.CS.LogShipper
{
    public partial class LogShipperService : ServiceBase
    {
        public ManualResetEvent fAbort = new ManualResetEvent(false);
        LogShipperWorker _worker;

        public LogShipperService()
        {
            this.CanPauseAndContinue = true; 
            InitializeComponent();
        }

        internal void OnStartInteractive(string[] args)
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            try
            {
                OnStart(args);
                fAbort.WaitOne();
                Debug.WriteLine(string.Format("CancelKeyPress detected. Stopping interactive mode."));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}@{1}: {2}\r\n{3}", ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace));
                Debug.WriteLine(string.Format("Stopping interactive mode."));
            }
            finally
            {
                OnStop();
            }
        }
        protected override void OnStart(string[] args)
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                var Uri = string.Empty;
                Uri = ConfigurationManager.AppSettings["Uri"];
                if (2 <= args.Length) Uri = args[0];
                Trace.Assert(!string.IsNullOrWhiteSpace(Uri), "Uri: Parameter validation FAILED.");

                var ManagementUri = string.Empty;
                ManagementUri = ConfigurationManager.AppSettings["ManagementUri"];
                if (2 <= args.Length) ManagementUri = args[1];
                Trace.Assert(!string.IsNullOrWhiteSpace(ManagementUri), "ManagementUri: Parameter validation FAILED.");

                var UpdateIntervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["UpdateIntervalMinutes"]);

                var ServerNotReachableRetries = Convert.ToInt32(ConfigurationManager.AppSettings["ServerNotReachableRetries"]);

                //_worker = new ScheduledTaskWorker(Uri, ManagementUri, UpdateIntervalMinutes, ServerNotReachableRetries);
                _worker = new LogShipperWorker();
                String path = "C:\\Logs\\biz.dfch.PS.System.Logging\\2015-03";
                String filter = "*.log";
                _worker.Start(path, filter);
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}@{1}: {2}\r\n{3}", ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace);
                Debug.WriteLine(msg);
                Environment.FailFast(msg, ex);
            }

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            _worker.Stop();

            base.OnStop();
        }
        protected override void OnPause()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            _worker.Stop();

            base.OnPause();
        }
        protected override void OnContinue()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            _worker.Update();
            _worker.Start();

            base.OnContinue();
        }
        protected override void OnCustomCommand(int command)
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            base.OnCustomCommand(command);
        }
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            base.OnSessionChange(changeDescription);
        }
        protected override void OnShutdown()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            _worker.Stop();

            base.OnShutdown();
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
