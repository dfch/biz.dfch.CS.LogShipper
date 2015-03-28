using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using biz.dfch.CS.LogShipper;

namespace LogShipperTests
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
    }
}
