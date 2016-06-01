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
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using biz.dfch.CS.LogShipper.Public;

namespace biz.dfch.CS.LogShipper.Extensions
{
    [Export(typeof(ILogShipperOutput))]
    [ExportMetadata("Name", "ConsoleOutput")]
    public class ConsoleOutput : ILogShipperOutput
    {
        private NameValueCollection _configuration;
        public NameValueCollection Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException("Configuration", "ConsoleOutput.Configuration: Parameter validation FAILED. Parameter must not be null.");
                }
                _configuration = value;
            }
        }

        public bool Log(string data)
        {
            if (null == _configuration)
            {
                throw new ArgumentNullException("Configuration", "Configuration: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            if (String.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException("data", "data: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            Console.WriteLine(String.Format("{0:yyyy-MM-dd HH:mm:ss.fffzzz}: {1}", DateTime.Now, data));
            return true;
        }
        public bool UpdateConfiguration(NameValueCollection configuration)
        {
            return VerifyAndSetConfiguration(configuration);
        }
        private bool VerifyAndSetConfiguration(NameValueCollection configuration)
        {
            var fReturn = false;
            try
            {
                if (null == configuration)
                {
                    throw new ArgumentNullException("configuration", "configuration: Parameter validation FAILED. Parameter must not be null.");
                }
                _configuration = new NameValueCollection(configuration);
                fReturn = true;
            }
            catch(Exception ex)
            {
                fReturn = false;
            }
            return fReturn;
        }
    }
}
