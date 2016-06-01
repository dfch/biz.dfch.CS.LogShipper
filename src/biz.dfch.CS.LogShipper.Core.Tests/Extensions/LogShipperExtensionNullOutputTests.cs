using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.LogShipper;
using biz.dfch.CS.LogShipper.Public;
using biz.dfch.CS.LogShipper.Extensions;
using System.Collections.Specialized;

namespace biz.dfch.CS.LogShipper.Core.Tests
{
    [TestClass]
    [DeploymentItem("LogShipper.Tests.dll.config")]
    public class LogShipperExtensionNullOutputTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LogEmptyShouldThrowArgumentNullException()
        {
            // Arrange
            String data = null;
            var output = new NullOutput();
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            output.Configuration = nameValueCollection;

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
            var output = new NullOutput();
            output.Configuration = null;

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
            var output = new NullOutput();
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            output.Configuration = nameValueCollection;

            // Act
            var fReturn = output.Log(data);

            // Assert
            Assert.IsTrue(fReturn);
        }
        [TestMethod]
        public void UpdateConfigurationShouldReturnTrue()
        {
            // Arrange
            var output = new NullOutput();
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");

            // Act
            var fReturn = output.UpdateConfiguration(nameValueCollection);

            // Assert
            Assert.IsTrue(fReturn);
            Assert.IsTrue(Helpers.CompareNameValueCollections(nameValueCollection, output.Configuration, false));
        }
        [TestMethod]
        public void UpdateConfigurationShouldReturnFalse()
        {
            // Arrange
            var output = new NullOutput();
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            output.Configuration = nameValueCollection;

            // Act
            var fReturn = output.UpdateConfiguration(null);

            // Assert
            Assert.IsFalse(fReturn);
            Assert.IsTrue(Helpers.CompareNameValueCollections(nameValueCollection, output.Configuration, false));
        }
        [TestMethod]
        public void UpdateLocalConfigurationKeepsExtensionConfigurationUnchanged()
        {
            // Arrange
            var output = new NullOutput();
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");

            // Act
            output.Configuration = nameValueCollection;
            var fReturn = output.UpdateConfiguration(nameValueCollection);
            nameValueCollection.Add("a-new-local-name", "another-arbitrary-value");

            // Assert
            Assert.IsTrue(fReturn);
            Assert.AreEqual(2, nameValueCollection.Count);
            Assert.AreEqual(1, output.Configuration.Count);
            Assert.IsFalse(Helpers.CompareNameValueCollections(nameValueCollection, output.Configuration, false));
        }
    }
}
