using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.LogShipper;
using biz.dfch.CS.LogShipper.Contracts;
using biz.dfch.CS.LogShipper.Extensions;
using System.Collections.Specialized;

namespace biz.dfch.CS.LogShipper.Tests
{
    [TestClass]
    [DeploymentItem("LogShipper.Tests.dll.config")]
    public class LogShipperExtensionDefaultTextParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseUninitialisedContextShouldThrowArgumentNullException()
        {
            // Arrange
            var data = "hello, world!\n";
            var parser = new DefaultTextParser();
            parser.Configuration = null;

            // Act
            var list = parser.Parse(data);

            // Assert
            Assert.IsNull(list);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseUninitialisedShouldThrowArgumentNullException()
        {
            // Arrange
            var data = "hello, world!\n";
            var parser = new DefaultTextParser();
            parser.Configuration = null;

            // Act
            var list = parser.Parse(data);

            // Assert
            Assert.IsNull(list);
        }
        [TestMethod]
        public void ParseEmptyDataShouldReturnEmptyList()
        {
            // Arrange
            var data = String.Empty;
            var parser = new DefaultTextParser();
            var nvc = new NameValueCollection();
            nvc.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nvc;

            // Act
            var list = parser.Parse(data);
            
            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }
        [TestMethod]
        public void ParseHalfLineDataShouldReturnEmptyList()
        {
            // Arrange
            var data = "hello, world!";
            var parser = new DefaultTextParser();
            var nvc = new NameValueCollection();
            nvc.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nvc;

            // Act
            var list = parser.Parse(data);

            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }
        [TestMethod]
        public void ParseLfDataShouldReturnOneListItem()
        {
            // Arrange
            var data = "hello, world!\n";
            var parser = new DefaultTextParser();
            var nvc = new NameValueCollection();
            nvc.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nvc;

            // Act
            var list = parser.Parse(data);
            
            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("hello, world!", list[0]);
        }
        public void ParseCrlfShouldReturnOneListItem()
        {
            // Arrange
            var data = "hello, world!\r\nsome more text without line break ...";
            var parser = new DefaultTextParser();
            var nvc = new NameValueCollection();
            nvc.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nvc; 

            // Act
            var list = parser.Parse(data);
            
            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("hello, world!", list[0]);
        }
        public void ParseCrShouldReturnOneListItem()
        {
            // Arrange
            var data = "hello, world!\rsome more text without line break ...";

            var parser = new DefaultTextParser();
            var nvc = new NameValueCollection();
            nvc.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nvc; 

            // Act
            var list = parser.Parse(data);

            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            var line = "hello, world!";
            Assert.AreEqual(line, list[0]);
        }
        [TestMethod]
        public void ParseTwoLinesDataShouldReturnOneListItem()
        {
            // Arrange
            var data = "hello, world!\r\nsome more text without line break ...";
            var parser = new DefaultTextParser();
            var nvc = new NameValueCollection();
            nvc.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nvc;

            // Act
            var list = parser.Parse(data);

            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            var line = "hello, world!";
            Assert.AreEqual(line, list[0]);
        }
        [TestMethod]
        public void ParseEmptyLineDataShouldReturnOneListItem()
        {
            // Arrange
            var data = "hello, world!\r\n\nsome more text without line break ...";
            var parser = new DefaultTextParser();
            var nvc = new NameValueCollection();
            nvc.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nvc; 

            // Act
            var list = parser.Parse(data);

            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("hello, world!", list[0]);
            Assert.AreEqual("", list[1]);
        }
        [TestMethod]
        public void ParseManyLinesDataShouldReturnHugeList()
        {
            // Arrange
            var count = 1000000;
            var sb = new System.Text.StringBuilder(count);
            for (var c = 0; c < count; c++)
            {
                sb.AppendFormat("This is line '{0}' ...\r\n", c);
            }
            var data = sb.ToString();

            var parser = new DefaultTextParser();
            var nvc = new NameValueCollection();
            nvc.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nvc; 

            // Act
            var list = parser.Parse(data);
            
            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(count, list.Count);
            for (var c = 0; c < count; c++)
            {
                var line = "This is line '42' ...";
                Assert.AreEqual(line, list[42]);
            }
        }
        [TestMethod]
        public void UpdateConfigurationShouldReturnTrue()
        {
            // Arrange
            var output = new DefaultTextParser();
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
            var output = new DefaultTextParser();
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
            var output = new DefaultTextParser();
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
