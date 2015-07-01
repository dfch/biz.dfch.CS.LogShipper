using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.LogShipper;
using biz.dfch.CS.LogShipper.Contracts;
using biz.dfch.CS.LogShipper.Extensions;

namespace biz.dfch.CS.LogShipper.Tests
{
    [TestClass]
    [DeploymentItem("LogShipper.Tests.dll.config")]
    public class LogShipperExtensionConsoleOutputTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LogEmptyShouldThrowArgumentNullException()
        {
            // Arrange
            String data = null;
            var output = new DefaultConsoleOutput();
            output.Context = new Object();

            // Act
            var fReturn = output.Log(data);

            // Assert
            Assert.IsNull(fReturn);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LogUninitialisedShouldThrowArgumentNullException()
        {
            // Arrange
            var data = "hello, world!";
            var output = new DefaultConsoleOutput();
            output.Context = null;

            // Act
            var fReturn = output.Log(data);

            // Assert
            Assert.IsNull(fReturn);
        }
        [TestMethod]
        public void LogTextShouldReturnTrue()
        {
            // Arrange
            var data = "Hello, world!";
            var output = new DefaultConsoleOutput();
            output.Context = new Object();

            // Act
            var fReturn = output.Log(data);

            // Assert
            Assert.IsTrue(fReturn);
        }
    }
}
