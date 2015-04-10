using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using System.Management;
using System.Management.Automation;

using biz.dfch.CS.LogShipper;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "LogShipperTests.dll.config", Watch = true)]
namespace biz.dfch.CS.LogShipperTests
{
    [TestClass]
    [DeploymentItem("LogShipperTests.dll.config")]
    public class UnitTest1
    {
        private TestContext _testContext;
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
        public static void classInitialize(TestContext testContext)
        {
            Trace.WriteLine(String.Format("classInitialize: '{0}'", testContext.TestName));
        }

        [ClassCleanup()]
        public static void classCleanup()
        {
            Trace.WriteLine("classCleanup");
        }

        [TestInitialize()]
        public void testInitialize()
        {
            Trace.WriteLine("testInitialize");
        }

        [TestCleanup()]
        public void testCleanup()
        {
            Trace.WriteLine("testCleanup");
        }

        [TestMethod]
        public void doNothingReturnsTrue()
        {
            Assert.AreEqual(true, true);
        }
        
        [TestMethod]
        public void doNothingReturnsFalse()
        {
            Assert.AreEqual(false, false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void doStartThrowsInvalidOperationException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Start();
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void doStartEmptyThrowsArgumentNullException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Start("");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void doStartNullThrowsArgumentNullException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Start(null);
        }
        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void doStartInvalidDirectoryThrowsArgumentException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Start("C:\\non-existent-directory\\non-existant-file.log");
        }
        [TestMethod]
        public void doStartReturnsTrue()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var fReturn = worker.Start(path);
            Assert.AreEqual(true, fReturn);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void doStopThrowsInvalidOperationException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Stop();
        }
        [TestMethod]
        public void doStartStopStartReturnsTrue()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var fReturn = worker.Start(path);
            worker.Stop();
            fReturn = worker.Start();
            Assert.AreEqual(true, fReturn);
        }
        [TestMethod]
        public void doUpdateReturnsFalse()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var fReturn = worker.Update();
            Assert.AreEqual(false, fReturn);
        }
        [TestMethod]
        public void doUpdateReturnsTrue()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var fReturn = worker.Start(path);
            fReturn = worker.Update();
            Assert.AreEqual(true, fReturn);
        }
        [TestMethod]
        public void doUpdateWithPathReturnsTrue()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var fReturn = worker.Start(path);
            fReturn = worker.Update(path);
            Assert.AreEqual(true, fReturn);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void doUpdateEmptyThrowsArgumentNullException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var fReturn = worker.Start("");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            fReturn = worker.Update(path);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void doUpdateNullThrowsArgumentNullException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var fReturn = worker.Start(null);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            fReturn = worker.Update(path);
        }
        [TestMethod]
        public void doAppendText()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var tempFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var fReturn = worker.Start(tempFile);
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
                    streamWriter.Flush();
                    System.Threading.Thread.Sleep(1000);
                    streamWriter.WriteLine("This is the third line of text.");
                    streamWriter.Flush();
                    System.Threading.Thread.Sleep(1000);
                }
                System.Threading.Thread.Sleep(5000);
                worker.Stop();
                System.Threading.Thread.Sleep(5000);
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
        public void doAppendText2()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var tempFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var fReturn = worker.Start(tempFile);
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
                    streamWriter.Flush();
                    System.Threading.Thread.Sleep(1000);
                    streamWriter.WriteLine("This is the third line of text.");
                    streamWriter.Flush();
                    System.Threading.Thread.Sleep(1000);
                }
                System.Threading.Thread.Sleep(5000);
                worker.Stop();
                System.Threading.Thread.Sleep(5000);
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
        public void doRenameFile()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "some-file.log");
            var tempFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var newFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var fReturn = worker.Start(tempFile);
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
        public void doAppendManyLines()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var tempFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var fReturn = worker.Start(tempFile);
            System.Threading.Thread.Sleep(1000);
            try
            {
                using (StreamWriter streamWriter = File.AppendText(tempFile))
                {
                    streamWriter.WriteLine("{0} - This is a line of text.", "FirstLine");
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

                    for (var c = 1000; c < 1200; c++)
                    {
                        streamWriter.WriteLine("{0} - This is a line of text.", c);
                        streamWriter.Flush();
                    }
                    streamWriter.WriteLine("{0} - This is a line of text.", "LastLine");
                    streamWriter.Flush();
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
        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void doAppendManyLinesExpectTimeoutException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var tempFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(Path.GetRandomFileName(), ".log"));
            var fReturn = worker.Start(tempFile);
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

                    for (var c = 1000; c < 1500; c++)
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
        [TestMethod]
        public void doTestPowershellCreate()
        {
            for(var c = 0; c < 1000*1000*10; c++)
            {
                 PowerShell ps = PowerShell.Create();
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
