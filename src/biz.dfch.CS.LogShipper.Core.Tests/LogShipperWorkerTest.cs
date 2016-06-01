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

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.LogShipper.Core;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "biz.dfch.CS.LogShipper.Core.Tests.dll.config", Watch = true)]
namespace biz.dfch.CS.LogShipper.Core.Tests
{
    [TestClass]
    [DeploymentItem("biz.dfch.CS.LogShipper.Core.Tests.dll.config")]
    public class LogShipperWorkerTest
    {
        private static TestContext _testContext;
        public TestContext testContext
        {
            get
            {
                return _testContext;
            }
            set
            {
                _testContext = value;
            }
        }

        [AssemblyInitialize]
        public static void Configure(TestContext testContext)
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            _testContext = testContext;
            Trace.WriteLine(String.Format("ClassInitialize: '{0}'", testContext.TestName));
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            Trace.WriteLine("ClassCleanup");
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            Trace.WriteLine("TestInitialize");
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            Trace.WriteLine("TestCleanup");
        }

        [TestMethod]
        public void DoNothingReturnsTrue()
        {
            Assert.AreEqual(true, true);
        }
        
        [TestMethod]
        public void DoNothingReturnsFalse()
        {
            Assert.AreEqual(false, false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DoStartThrowsInvalidOperationException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Start();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DoStartEmptyThrowsArgumentNullException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Start("", "");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DoStartNullThrowsArgumentNullException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Start(null, null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void DoStartInvalidDirectoryThrowsDirectoryNotFoundException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            worker.Start("C:\\non-existent-directory\\non-existant-file.log", scriptFile);
        }
        
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void DoStartInvalidScriptFileThrowsFileNotFoundException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Start("C:\\non-existent-directory\\non-existant-file.log", "C:\\non-existent-directory\\non-existant-file.ps1");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DoStartInvalidFileNameThrowsArgumentException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Start("C:\\irrelevant-directory\\irrelevant-file.log", "invalid-file-???.ps1");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DoStartInvalidDirectoryNameThrowsArgumentException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            worker.Start("C:\\invalid-directory-\t\\irrelevant-file.log", scriptFile);
        }
        
        [TestMethod]
        public void DoStartReturnsTrue()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var logFile = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var fReturn = worker.Start(logFile, System.Configuration.ConfigurationManager.AppSettings["ScriptFile"]);
            Assert.AreEqual(true, fReturn);
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DoStopThrowsInvalidOperationException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Stop();
        }
        
        [TestMethod]
        public void DoStartStopStartReturnsTrue()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var logFile = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(logFile, scriptFile);
            worker.Stop();
            fReturn = worker.Start();
            Assert.AreEqual(true, fReturn);
        }
        
        [TestMethod]
        public void DoUpdateReturnsFalse()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var fReturn = worker.Update();
            Assert.AreEqual(false, fReturn);
        }
        
        [TestMethod]
        public void DoUpdateReturnsTrue()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var logFile = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(logFile, scriptFile);
            fReturn = worker.Update();
            Assert.AreEqual(true, fReturn);
        }

        [TestMethod]
        public void DoUpdateWithPathReturnsTrue()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var logFile = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(logFile, scriptFile);
            fReturn = worker.Update(logFile, scriptFile);
            Assert.AreEqual(true, fReturn);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DoUpdateEmptyThrowsArgumentNullException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var logFile = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(logFile, scriptFile);
            fReturn = worker.Update("", "");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DoUpdateNullThrowsArgumentNullException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var logFile = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(logFile, scriptFile);
            fReturn = worker.Update(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void DoUpdateInvalidDirectoryThrowsDirectoryNotFoundException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var logFile = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(logFile, scriptFile);
            fReturn = worker.Update("C:\\non-existent-directory\\non-existant-file.log", scriptFile);
        }
        
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void DoUpdateInvalidScriptFileThrowsFileNotFoundException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var logFile = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(logFile, scriptFile);
            fReturn = worker.Update(logFile, "C:\\non-existent-directory\\non-existant-file.ps1");
        }
        
        [TestMethod]
        public void DoAppendText()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var tempFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(tempFile, scriptFile);
            System.Threading.Thread.Sleep(1000);
            try
            {
                using (StreamWriter streamWriter = File.AppendText(tempFile))
                {
                    streamWriter.WriteLine("This is the first line of text.");
                    streamWriter.Flush();
                    if (!File.Exists(tempFile))
                    {
                        throw new FileNotFoundException(String.Format("tempFile: File not found. Create operation FAILED."), tempFile);
                    }
                    System.Threading.Thread.Sleep(1000);
                    streamWriter.WriteLine("This is the second line of text.");
                    streamWriter.WriteLine("This is the third line of text.");
                    streamWriter.Flush();
                }
                worker.Stop();
            }
            finally
            {
                if(null != tempFile) File.Delete(tempFile);
                if (File.Exists(tempFile))
                {
                    throw new FileNotFoundException(String.Format("tempFile: File exists. Delete operation FAILED."), tempFile);
                }
            }
        }
        
        [TestMethod]
        public void DoAppendText2()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var tempFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(tempFile, scriptFile);
            try
            {
                using (StreamWriter streamWriter = File.AppendText(tempFile))
                {
                    streamWriter.WriteLine("This is the first line of text.");
                    streamWriter.Flush();
                    if (!File.Exists(tempFile))
                    {
                        throw new FileNotFoundException(String.Format("tempFile: File not found. Create operation FAILED."), tempFile);
                    }
                    System.Threading.Thread.Sleep(1000);
                    streamWriter.WriteLine("This is the second line of text.");
                    streamWriter.WriteLine("This is the third line of text.");
                    streamWriter.Flush();
                }
                System.Threading.Thread.Sleep(5000);
                worker.Stop();
            }
            finally
            {
                if (null != tempFile) File.Delete(tempFile);
                if (File.Exists(tempFile))
                {
                    throw new FileNotFoundException(String.Format("tempFile: File exists. Delete operation FAILED."), tempFile);
                }
            }
        }
        
        [TestMethod]
        public void DoRenameFile()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var tempFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var newFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(tempFile, scriptFile);
            try
            {
                using (StreamWriter streamWriter = File.AppendText(tempFile))
                {
                    streamWriter.WriteLine("This is a line of text on the original file name.");
                    streamWriter.Flush();

                    if (!File.Exists(tempFile))
                    {
                        throw new FileNotFoundException(String.Format("tempFile: File not found. Create operation FAILED."), tempFile);
                    }
                }

                File.Move(tempFile, newFile);

                if (!File.Exists(newFile))
                {
                    throw new FileNotFoundException(String.Format("newFile: File not found. Rename operation FAILED."), newFile);
                }

                using (StreamWriter streamWriter = File.AppendText(newFile))
                {
                    streamWriter.WriteLine("This is a line of text on the new file name.");
                    streamWriter.Flush();
                }
            }
            finally
            {
                worker.Stop();
                if (null != tempFile) File.Delete(tempFile);
                if (File.Exists(tempFile))
                {
                    throw new FileNotFoundException(String.Format("tempFile: File exists. Delete operation FAILED."), tempFile);
                }

                if (null != newFile) File.Delete(newFile);
                if (File.Exists(newFile))
                {
                    throw new FileNotFoundException(String.Format("newFile: File exists. Delete operation FAILED."), newFile);
                }
            }
        }
        
        [TestMethod]
        public void DoAppendManyLines()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var tempFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(tempFile, scriptFile);
            System.Threading.Thread.Sleep(1000);
            try
            {
                using (StreamWriter streamWriter = File.AppendText(tempFile))
                {
                    streamWriter.WriteLine("{0} - This is a line of text.", "FirstLine");
                    streamWriter.Flush();
                    if (!File.Exists(tempFile))
                    {
                        throw new FileNotFoundException(String.Format("tempFile: File not found. Create operation FAILED."), tempFile);
                    }
                }
                System.Threading.Thread.Sleep(1000);
                using (StreamWriter streamWriter = File.AppendText(tempFile))
                {

                    for (var c = 1000; c < 1200; c++)
                    {
                        streamWriter.WriteLine("{0} - This is a line of text.", c);
                        streamWriter.Flush();
                    }
                    streamWriter.WriteLine("{0} - This is a line of text.", "LastLine");
                    streamWriter.Flush();
                }
                System.Diagnostics.Trace.WriteLine("WriteLine completed.");
                worker.Stop();
            }
            finally
            {
                if (null != tempFile) File.Delete(tempFile);
                if (File.Exists(tempFile))
                {
                    throw new FileNotFoundException(String.Format("tempFile: File exists. Delete operation FAILED."), tempFile);
                }
            }
        }
        
        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoAppendManyLinesThrowsTimeoutException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var tempFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            var fReturn = worker.Start(tempFile, scriptFile);
            System.Threading.Thread.Sleep(1000);
            try
            {
                using (StreamWriter streamWriter = File.AppendText(tempFile))
                {
                    streamWriter.WriteLine("{0} - This is a line of text.", -1);
                    streamWriter.Flush();
                    //streamWriter.Close();
                    if (!File.Exists(tempFile))
                    {
                        throw new FileNotFoundException(String.Format("tempFile: File not found. Create operation FAILED."), tempFile);
                    }
                }
                System.Threading.Thread.Sleep(1000);
                using (StreamWriter streamWriter = File.AppendText(tempFile))
                {

                    for (var c = 1000; c < 15000; c++)
                    {
                        streamWriter.WriteLine("{0} - This is a line of text.", c);
                        streamWriter.Flush();
                    }
                }
                System.Diagnostics.Trace.WriteLine("WriteLine completed.");
                System.Threading.Thread.Sleep(1000);
                worker.Stop();
            }
            finally
            {
                if (null != tempFile) File.Delete(tempFile);
                if (File.Exists(tempFile))
                {
                    throw new FileNotFoundException(String.Format("tempFile: File exists. Delete operation FAILED."), tempFile);
                }
            }
        }
    }
}
