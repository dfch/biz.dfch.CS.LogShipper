using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.LogShipper;
using biz.dfch.CS.LogShipper.Contracts;
using biz.dfch.CS.LogShipper.Extensions;

namespace biz.dfch.CS.LogShipper.Tests
{
    [TestClass]
    [DeploymentItem("LogShipperTests.dll.config")]
    public class LogShipperExtensionDefaultTextParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseUninitialisedDataShouldThrowArgumentNullException()
        {
            // Arrange
            var data = "hello, world!\n";
            var parser = new DefaultTextParser();
            parser.Context = new Object();
            parser.Data = String.Empty;

            // Act
            var list = parser.Parse(data);

            // Assert
            Assert.IsNull(list);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseUninitialisedContextShouldThrowArgumentNullException()
        {
            // Arrange
            var data = "hello, world!\n";
            var parser = new DefaultTextParser();
            parser.Context = null;
            parser.Data = "some-meaningless-data";

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
            parser.Context = null;
            parser.Data = "    ";

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
            parser.Context = new Object();
            parser.Data = "some-meaningless-data";

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
            parser.Context = new Object();
            parser.Data = "some-meaningless-data";

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
            parser.Context = new Object();
            parser.Data = "some-meaningless-data";

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
            parser.Context = new Object();
            parser.Data = "some-meaningless-data";

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
            parser.Context = new Object();
            parser.Data = "some-meaningless-data";

            // Act
            var list = parser.Parse(data);

            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            var line = (data.Split(System.Environment.NewLine.ToCharArray()))[0];
            Assert.AreEqual(line, list[0]);
        }
        [TestMethod]
        public void ParseTwoLinesDataShouldReturnOneListItem()
        {
            // Arrange
            var data = "hello, world!\r\nsome more text without line break ...";
            var parser = new DefaultTextParser();
            parser.Context = new Object();
            parser.Data = "some-meaningless-data";

            // Act
            var list = parser.Parse(data);

            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            var line = (data.Split(System.Environment.NewLine.ToCharArray()))[0];
            Assert.AreEqual(line, list[0]);
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
            parser.Context = new Object();
            parser.Data = "some-meaningless-data";

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
    }
}
