/**
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
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace biz.dfch.CS.LogShipper.Core
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
                Debug.WriteLine(String.Format("CancelKeyPress detected. Stopping interactive mode."));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("{0}@{1}: {2}\r\n{3}", ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace));
                Debug.WriteLine(String.Format("Stopping interactive mode."));
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
                var updateIntervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["UpdateIntervalMinutes"]);
                var serverNotReachableRetries = Convert.ToInt32(ConfigurationManager.AppSettings["ServerNotReachableRetries"]);

                String logFile = ConfigurationManager.AppSettings["LogFile"];
                if (2 <= args.Length) logFile = args[0];
                Trace.Assert(!string.IsNullOrWhiteSpace(logFile), "LogFile: Parameter validation FAILED.");

                String scriptFile = ConfigurationManager.AppSettings["ScriptFile"];
                if (2 <= args.Length) scriptFile = args[1];
                Trace.Assert(!string.IsNullOrWhiteSpace(scriptFile), "ScriptFile: Parameter validation FAILED.");
                
                _worker = new LogShipperWorker();
                _worker.Start(logFile, scriptFile);
            }
            catch (Exception ex)
            {
                var msg = String.Format("{0}@{1}: {2}\r\n{3}", ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace);
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
