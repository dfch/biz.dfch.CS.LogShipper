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
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.LogShipper.Public;

namespace biz.dfch.CS.LogShipper.Public.Tests
{
    [TestClass]
    public class ILogShipperParserTest
    {
        class LogShipperParserImpl : ILogShipperParser
        {
            public NameValueCollection Configuration { get; set; }

            public uint OffsetParsed { get; private set; }

            public List<string> Parse(string data)
            {
                OffsetParsed += (uint) data.Length;

                var result = new List<string>();
                result.Add(data);

                return result;
            }

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
            var sut = new LogShipperParserImpl();

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
            var sut = new LogShipperParserImpl();

            // Act
            var result = sut.UpdateConfiguration(configuration);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(configuration["arbitrary-key"], sut.Configuration["arbitrary-key"]);
        }

        [TestMethod]
        public void ParseSucceeds()
        {
            // Arrange
            var message = "arbitrary-string";
            var sut = new LogShipperParserImpl();
            var offsetParsed = sut.OffsetParsed;

            // Act
            var result = sut.Parse(message);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(1 == result.Count);
            Assert.AreEqual(message, result[0]);
            Assert.AreEqual(offsetParsed + message.Length, sut.OffsetParsed);
        }

    }
}
