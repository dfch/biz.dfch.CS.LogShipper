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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.LogShipper;
using biz.dfch.CS.LogShipper.Public;
using biz.dfch.CS.LogShipper.Extensions;
using System.Collections.Specialized;
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.LogShipper.Core.Tests
{
    [TestClass]
    [DeploymentItem("LogShipper.Tests.dll.config")]
    public class LogShipperExtensionDefaultTextParserTest
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
        [ExpectContractFailure]
        public void ParseEmptyDataThrowsContractException()
        {
            // Arrange
            var data = String.Empty;
            var parser = new DefaultTextParser();
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nameValueCollection;

            // Act
            var list = parser.Parse(data);
            
            // Assert
            // N/A
        }
        [TestMethod]
        public void ParseHalfLineDataShouldReturnEmptyList()
        {
            // Arrange
            var data = "hello, world!";
            var parser = new DefaultTextParser();
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nameValueCollection;

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
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nameValueCollection;

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
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nameValueCollection; 

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
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nameValueCollection; 

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
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nameValueCollection;

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
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nameValueCollection; 

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
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            parser.Configuration = nameValueCollection; 

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
        [ExpectContractFailure]
        public void UpdateConfigurationThrowsContractException()
        {
            // Arrange
            var output = new DefaultTextParser();
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("arbitrary-name", "arbitrary-value");
            output.Configuration = nameValueCollection;

            // Act
            var fReturn = output.UpdateConfiguration(null);

            // Assert
            // N/A
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
