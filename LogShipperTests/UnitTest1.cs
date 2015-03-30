using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using biz.dfch.CS.LogShipper;

namespace biz.dfch.CS.LogShipperTests
{
    [TestClass]
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
            Debug.WriteLine("testInitialize.");
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
            worker.Start("", "");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void doStartNullThrowsArgumentNullException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Start(null, null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void doStartInvalidDirectoryThrowsArgumentException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            worker.Start("C:\\non-existent-directory", "*.log");
        }
        [TestMethod]
        public void doStartReturnsTrue()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var fReturn = worker.Start(Directory.GetCurrentDirectory(), "*.log");
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
            var fReturn = worker.Start(Directory.GetCurrentDirectory(), "*.log");
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
            var fReturn = worker.Start(Directory.GetCurrentDirectory(), "*.log");
            fReturn = worker.Update();
            Assert.AreEqual(true, fReturn);
        }
        [TestMethod]
        public void doUpdateWithPathReturnsTrue()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var fReturn = worker.Start(Directory.GetCurrentDirectory(), "*.log");
            fReturn = worker.Update(Directory.GetCurrentDirectory(), "*.log");
            Assert.AreEqual(true, fReturn);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void doUpdateEmptyThrowsArgumentNullException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var fReturn = worker.Start("", "");
            fReturn = worker.Update(Directory.GetCurrentDirectory(), "*.log");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void doUpdateNullThrowsArgumentNullException()
        {
            LogShipperWorker worker = new LogShipperWorker();
            var fReturn = worker.Start(null, null);
            fReturn = worker.Update(Directory.GetCurrentDirectory(), "*.log");
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
