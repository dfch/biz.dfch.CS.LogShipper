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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.LogShipper;
using biz.dfch.CS.LogShipper.Public;
using biz.dfch.CS.LogShipper.Extensions;
using System.Collections.Specialized;
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.LogShipper.Core.Tests
{
    [TestClass]
    [DeploymentItem("biz.dfch.CS.LogShipper.Core.Tests.dll.config")]
    public class LogShipperExtensionNullOutputTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LogEmptyThrowsContractException()
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
            // N/A
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
        [ExpectContractFailure]
        public void UpdateConfigurationThrowsContractException()
        {
            // Arrange
            var output = new NullOutput();
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
