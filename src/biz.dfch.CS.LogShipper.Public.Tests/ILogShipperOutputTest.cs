/**
 * Copyright 2016 d-fens GmbH
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
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.LogShipper.Public;

namespace biz.dfch.CS.LogShipper.Public.Tests
{
    [TestClass]
    public class ILogShipperOutputTest
    {
        class LogShipperOutputImpl : ILogShipperOutput
        {
            public NameValueCollection Configuration { get; set; }

            public bool Log(string data)
            {
                return true;
            }

            public bool UpdateConfiguration(NameValueCollection configuration)
            {
                Configuration = configuration;
                return true;
            }
        }

        [TestMethod]
        public void LogSucceeds()
        {
            // Arrange
            var message = "arbitrary-string";
            var sut = new LogShipperOutputImpl();

            // Act
            var result = sut.Log(message);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UpdateConfigurationSucceeds()
        {
            // Arrange
            var configuration = new NameValueCollection();
            configuration.Add("arbitrary-key", "arbitrary-value");
            var sut = new LogShipperOutputImpl();

            // Act
            var result = sut.UpdateConfiguration(configuration);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(configuration["arbitrary-key"], sut.Configuration["arbitrary-key"]);
        }
    }
}
